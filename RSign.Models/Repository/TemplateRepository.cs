using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Common.Mailer;
using RSign.ManageDocument.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.Models.APIModels;
using RSign.Models.APIModels.Envelope;
using RSign.Models.EmailQueueProcessor;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Repository
{
    public class TemplateRepository : ITemplateRepository
    {
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly IRecipientRepository _recipientRepository;
        private readonly IGenericRepository _genericRepository;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private readonly IUserRepository _userRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly IConditionalControlRepository _conditionalControlRepository;
        private readonly IAsposeHelper _asposeHelper;
        public TemplateRepository(IOptions<AppSettingsConfig> configuration, IConditionalControlRepository conditionalControlRepository, ISettingsRepository settingsRepository, IConfiguration appConfiguration,
            IRecipientRepository recipientRepository, IGenericRepository genericRepository, IEnvelopeHelperMain envelopeHelperMain, IAsposeHelper asposeHelper, IUserRepository userRepository)
        {
            _configuration = configuration;
            _conditionalControlRepository = conditionalControlRepository;
            _settingsRepository = settingsRepository;
            _userRepository = userRepository;
            _recipientRepository = recipientRepository;
            _genericRepository = genericRepository;
            _envelopeHelperMain = envelopeHelperMain;
            _userRepository = userRepository;
            _appConfiguration = appConfiguration;
            rsignlog = new RSignLogger(_appConfiguration);
            _asposeHelper = asposeHelper;
        }
        
        public Template GetCreatedDateTime(Guid templateID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Template template = dbContext.Template.Where(e => e.ID == templateID).FirstOrDefault();
                    return template;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ErrorResponseModel> ReSendPasswordEmail(UserVerificationModel userVerificationModel)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            loggerModelNew = new LoggerModelNew("", "TemplateRepository", "ReSendPasswordEmail", "Initiate the process for Update Verification Code using API.", userVerificationModel.envelopeID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                bool isEmailSend = false; bool isMobileSend = false;
                bool isPwdEmailSend = false; bool isPwdMobileSend = false;

                int deliveryMode = 1;
                if (!string.IsNullOrEmpty(userVerificationModel.DeliveryMode))
                {
                    deliveryMode = Convert.ToInt32(userVerificationModel.DeliveryMode);
                }
                Recipients recSigner = new Recipients() { DeliveryMode = deliveryMode, EmailAddress = userVerificationModel.EmailId, Mobile = userVerificationModel.MobileNumber };
                _envelopeHelperMain.GetEmailDeiliveryModeOptions(recSigner, ref isEmailSend, ref isMobileSend);
                _envelopeHelperMain.GetMobileDeiliveryModeOptions(recSigner, ref isPwdEmailSend, ref isPwdMobileSend);


                //if (userVerificationModel.DeliveryMode == "1")
                //{
                //    isSendEmail = true;
                //}
                //else if (userVerificationModel.DeliveryMode == "2")
                //{
                //    if (!string.IsNullOrEmpty(userVerificationModel.EmailId))
                //    {
                //        isSendEmail = true;
                //    }
                //    if (!string.IsNullOrEmpty(userVerificationModel.MobileNumber))
                //    {
                //        isMobileSMS = true;
                //    }
                //}
                //else if (userVerificationModel.DeliveryMode == "3")
                //{
                //    isMobileSMS = true;
                //}
                //else
                //{
                //    isSendEmail = true;
                //}

                bool isEmailSent = false;
                bool isSMSSent = false;

                if (isPwdEmailSend && userVerificationModel.EmailId != null)
                {
                    var template = _genericRepository.GetTemplateDetails(new Guid(userVerificationModel.envelopeID));

                    var IsPasswordMailToSigner = template.IsPasswordMailToSigner;
                    if (Convert.ToBoolean(template.EnableMessageToMobile))
                    {
                        IsPasswordMailToSigner = true;
                    }


                    if (IsPasswordMailToSigner)
                    {
                        var userprofile = _userRepository.GetUserProfile(template.UserID);
                        var allSettingsDetails = _envelopeHelperMain.GetAllSettingsDetails(template.UserID, userprofile.EmailID);
                        string mailTemplate = string.Empty;
                        string EmailDisclaimer = string.Empty;
                        int SignReqReplyToAddressValue = 1;
                        string[] toAddressForPw, toDisplayNameForPw;

                        var settingsDetails = _settingsRepository.GetEntityForByKeyConfig(userprofile.CompanyID.Value, Constants.SettingsKeyConfig.EmailDisclaimer);
                        if (settingsDetails != null)
                            EmailDisclaimer = Convert.ToString(settingsDetails.OptionValue);

                        var settingsSignatureRequestDetails = _settingsRepository.GetEntityForByKeyConfig(userprofile.UserID, Constants.SettingsKeyConfig.SignatureRequestReplyAddress);
                        if (settingsSignatureRequestDetails != null)
                            SignReqReplyToAddressValue = Convert.ToInt32(settingsSignatureRequestDetails.OptionValue);

                        string recipientCultureInfo = !string.IsNullOrEmpty(userVerificationModel.CultureInfo) ? userVerificationModel.CultureInfo : template.CultureInfo;
                        string mailTemplatepassword = string.Empty, password = string.Empty;
                        if (template.PasswordKeySize.HasValue)
                            password = ModelHelper.Decrypt(template.PasswordtoSign, template.PasswordKey, (int)template.PasswordKeySize);
                        mailTemplate = _genericRepository.GetNewMailTemplate(Constants.String.MailTemplateName.PasswordSign, recipientCultureInfo, EmailDisclaimer);

                        toAddressForPw = userVerificationModel.EmailId.Split(',').ToArray();
                        toDisplayNameForPw = userVerificationModel.EmailId.Split(',').ToArray();

                        mailTemplate = mailTemplate.Replace("#SenderName", userprofile.FirstName);
                        mailTemplate = mailTemplate.Replace("#SenderEmailId", userprofile.EmailID);
                        mailTemplate = mailTemplate.Replace("#RecipientName", userVerificationModel.EmailId.Split('@')[0]);
                        mailTemplate = mailTemplate.Replace("#SignerName", userVerificationModel.EmailId.Split('@')[0]);

                       
                            string emailMobileNumberDetails = _asposeHelper.AppendSignerEmailMobileDetails(deliveryMode, userVerificationModel.EmailId, userVerificationModel.DialCode, userVerificationModel.MobileNumber);
                            mailTemplate = mailTemplate.Replace("#RecipientEmailId", emailMobileNumberDetails);
                        
                        //if (isMobileSMS)
                        //{
                        //    if (userVerificationModel.DeliveryMode == "2")
                        //        mailTemplate = mailTemplate.Replace("#RecipientEmailId", userVerificationModel.EmailId + ", " + (userVerificationModel.DialCode + userVerificationModel.MobileNumber));
                        //    else
                        //        mailTemplate = mailTemplate.Replace("#RecipientEmailId", (userVerificationModel.DialCode + userVerificationModel.MobileNumber));
                        //}
                        //else
                        //    mailTemplate = mailTemplate.Replace("#RecipientEmailId", userVerificationModel.EmailId);
                        mailTemplate = mailTemplate.Replace("#EnvelopeDisplayCode", "");
                        mailTemplate = mailTemplate.Replace("#PasswordToOpenDoc", System.Web.HttpUtility.HtmlEncode(password));
                        //mailTemplate = mailTemplate.Replace("#ImageURL", imageLogoURl);
                        mailTemplate = mailTemplate.Replace("#clientLogo", string.Empty);

                        mailTemplate = _envelopeHelperMain.EmailBannerSettings(mailTemplate, allSettingsDetails, Constants.String.MailTemplateName.PasswordSign);

                        mailTemplate = _genericRepository.AppendFooterText(mailTemplate, userVerificationModel.EmailId, Constants.String.MailTemplateName.PasswordSign, userVerificationModel.CultureInfo, "FinalContractFooter");

                        if (Convert.ToString(_appConfiguration["SendEmailFromService"]) != null && Convert.ToString(_appConfiguration["SendEmailFromService"]).ToUpper() == "TRUE")
                        {
                            //To Add data in Emailqueue and EmailqueueRecipients for Electronic Signature Carbon Copy 
                            // update toAddressForPw for signer recipients

                            EmailQueueData emailQueueData = new EmailQueueData();
                            EmailSendInfo emailSendData = new EmailSendInfo();
                            Recipients Senderrecipients = new Recipients();
                            Senderrecipients.Name = userprofile.FirstName;
                            Senderrecipients.EmailAddress = userprofile.EmailID;
                            emailQueueData.template = template;
                            emailQueueData.MailMessageBody = mailTemplate;
                            emailQueueData.SignReqReplyToAddressValue = SignReqReplyToAddressValue;
                            //emailQueueData.Signer = signer; 
                            emailQueueData.EmailType = Constants.EmailTypes.ESPN;
                            emailQueueData.EmailSubject = _envelopeHelperMain.GetEmailSubjectPrefix(recipientCultureInfo, template.Subject, Constants.EmailSubject.PasswordToSign);

                            emailSendData.RecipientName = string.Join(",", toDisplayNameForPw);
                            emailSendData.RecipientEmail = string.Join(",", toAddressForPw);
                            emailQueueData.emailSendInfo = emailSendData;
                            emailQueueData.Signer = Senderrecipients;
                            _envelopeHelperMain.EmailQueueTemplateFunction(emailQueueData);
                        }
                        else
                        {
                            var Chilkat = new ChilkatHelper(_appConfiguration);
                            Chilkat.SendMailUsingChilKet(toAddressForPw, toDisplayNameForPw, null, null, null, null, userprofile.EmailID, userprofile.FirstName, _envelopeHelperMain.GetEmailSubjectPrefix(recipientCultureInfo, template.Subject, Constants.EmailSubject.PasswordToSign), mailTemplate, null, null, "", Constants.String.EmailOperation.PasswordToSign, SignReqReplyToAddressValue);
                        }
                    }
                    isEmailSent = true;   
                }

                if (isPwdMobileSend && userVerificationModel.MobileNumber != null)
                {
                    var template = _genericRepository.GetTemplateDetails(new Guid(userVerificationModel.envelopeID));

                    //if (Convert.ToBoolean(template.IsSMSAccessCode))
                    //{
                        //if (template.AccessAuthType == eSign.Core.Helpers.Constants.ConfigurationalProperties.PasswordType.Endtoend)
                        //{

                        //}
                        var userprofile = _userRepository.GetUserProfile(template.UserID);                       
                        string mailTemplate = string.Empty;
                        string EmailDisclaimer = string.Empty;
                        int SignReqReplyToAddressValue = 1;
                        string[] toAddressForPw, toDisplayNameForPw;

                        var settingsDetails = _settingsRepository.GetEntityForByKeyConfig(userprofile.CompanyID.Value, Constants.SettingsKeyConfig.EmailDisclaimer);
                        if (settingsDetails != null)
                            EmailDisclaimer = Convert.ToString(settingsDetails.OptionValue);


                        var settingsSignatureRequestDetails = _settingsRepository.GetEntityForByKeyConfig(userprofile.UserID, Constants.SettingsKeyConfig.SignatureRequestReplyAddress);
                        if (settingsSignatureRequestDetails != null)
                            SignReqReplyToAddressValue = Convert.ToInt32(settingsSignatureRequestDetails.OptionValue);

                        string recipientCultureInfo = !string.IsNullOrEmpty(userVerificationModel.CultureInfo) ? userVerificationModel.CultureInfo : template.CultureInfo;
                        string mailTemplatepassword = string.Empty, password = string.Empty;
                        if (template.PasswordKeySize.HasValue)
                            password = ModelHelper.Decrypt(template.PasswordtoSign, template.PasswordKey, (int)template.PasswordKeySize);
                        mailTemplate = _genericRepository.GetNewMailTemplate(Constants.String.MailTemplateName.PasswordSign, recipientCultureInfo, EmailDisclaimer, 3, "mobile");

                        //toAddressForPw = userVerificationModel.EmailId.Split(',').ToArray();
                        //toDisplayNameForPw = userVerificationModel.EmailId.Split(',').ToArray();

                        mailTemplate = mailTemplate.Replace("#SenderName", userprofile.FirstName);
                        mailTemplate = mailTemplate.Replace("#EnvelopeDisplayCode", "");
                        mailTemplate = mailTemplate.Replace("#Password", System.Web.HttpUtility.HtmlEncode(password));

                        if (!string.IsNullOrEmpty(userVerificationModel.DialCode) && !string.IsNullOrEmpty(userVerificationModel.MobileNumber))
                        {
                            if (_appConfiguration["SendEmailFromService"] != null && Convert.ToString(_appConfiguration["SendEmailFromService"]).ToUpper() == "TRUE")
                            {
                                string emailSubject = _envelopeHelperMain.GetEmailSubjectPrefix(recipientCultureInfo, template.Subject, Constants.EmailSubject.PasswordToSign);
                                _envelopeHelperMain.SendTemplateSMSTThroughEmailService(template, mailTemplate, SignReqReplyToAddressValue, Constants.EmailTypes.ESPN, (userVerificationModel.DialCode + userVerificationModel.MobileNumber), emailSubject, userprofile);
                            }
                            else
                            {
                                string signerMobileNumber = userVerificationModel.DialCode + userVerificationModel.MobileNumber;
                                _envelopeHelperMain.SendMobileSMS(null, mailTemplate, signerMobileNumber, template);
                            }
                        }
                    //}
                    isSMSSent = true;
                }

                if (isEmailSent || isSMSSent)
                {
                    errorResponseModel.Message = "Success";
                    errorResponseModel.Status = true;
                    loggerModelNew.Message = "Process completed for Send Email Password using API and " + errorResponseModel.Message;
                    rsignlog.RSignLogInfo(loggerModelNew);
                    return errorResponseModel;
                }
                else
                {
                    errorResponseModel.Message = "No recipient found";
                    errorResponseModel.Status = false;
                    loggerModelNew.Message = errorResponseModel.Message;
                    rsignlog.RSignLogWarn(loggerModelNew);
                    return errorResponseModel;
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                errorResponseModel.Status = false;
                errorResponseModel.Message = "Error occurred in TemplateRepository and ReSendPasswordEmail method." + ex.Message;
                return errorResponseModel;
            }
        }
        public EnvelopeInfo FillEnvelopeInfoFromInitalizeSignDocumentAPI(ResponseMessageForInitalizeSignDocument responseEnvelope)
        {
            EnvelopeInfo controlsInfo = new EnvelopeInfo();
            if (responseEnvelope.EnvelopeInfo != null)
            {
                List<EnvelopeImageInformation> envelopeImageCollections = new List<EnvelopeImageInformation>();
                List<ConditionalControlRules> conditionalControlRules = new List<ConditionalControlRules>();
                List<ControlsData> controlsDatas = new List<ControlsData>();

                controlsInfo.GlobalEnvelopeID = responseEnvelope.EnvelopeInfo.GlobalEnvelopeID;
                controlsInfo.RecipientTypeId = responseEnvelope.EnvelopeInfo.RecipientTypeId;
                controlsInfo.SignersList = responseEnvelope.EnvelopeInfo.SignersList;
                controlsInfo.FontsList = responseEnvelope.EnvelopeInfo.FontsList;
                controlsInfo.FontSizes = responseEnvelope.EnvelopeInfo.FontSizes;
                controlsInfo.IsSendConfirmationEmail = responseEnvelope.EnvelopeInfo.IsSendConfirmationEmail;
                controlsInfo.IsInvitedBySigner = responseEnvelope.EnvelopeInfo.IsInvitedBySigner;
                //controlsInfo.EnvelopeImageCollection = responseEnvelope.EnvelopeInfo.EnvelopeImageCollection;
                loggerModelNew = new LoggerModelNew("", "TemplateRepository", "FillEnvelopeInfoFromInitalizeSignDocumentAPI", "Process started for Fill Envelope Info From Initalize SignDocument API method", "", "", "","", "APP");
                loggerModelNew.EnvelopeId = controlsInfo.GlobalEnvelopeID.ToString();
                rsignlog.RSignLogInfo(loggerModelNew);
                if (responseEnvelope.EnvelopeInfo.EnvelopeImageCollection != null && responseEnvelope.EnvelopeInfo.EnvelopeImageCollection.Count() > 0)
                {
                    List<EnvelopeImageInformation> EnvelopeImageInformationList = new List<EnvelopeImageInformation>();
                    foreach (var item in responseEnvelope.EnvelopeInfo.EnvelopeImageCollection)
                    {
                        EnvelopeImageInformation EnvelopeImageInformationResponse = new EnvelopeImageInformation();
                        EnvelopeImageInformationResponse.Id = item.Id;
                        EnvelopeImageInformationResponse.ImagePath = item.ImagePath;
                        EnvelopeImageInformationResponse.PageNo = item.PageNo;
                        EnvelopeImageInformationResponse.DocPageNo = item.DocPageNo;
                        Dimension dimension = new Dimension();
                        if (item.Dimension != null)
                        {
                            dimension.Height = item.Dimension.Height;
                            dimension.Width = item.Dimension.Width;
                            EnvelopeImageInformationResponse.Dimension = dimension;
                        }
                        else
                            EnvelopeImageInformationResponse.Dimension = null;

                        DocumentInfo documentInfo = new DocumentInfo();
                        if (item.Document != null)
                        {
                            documentInfo.Id = item.Document.Id;
                            documentInfo.Name = item.Document.Name;
                            EnvelopeImageInformationResponse.Document = documentInfo;
                        }
                        EnvelopeImageInformationList.Add(EnvelopeImageInformationResponse);
                    }
                    controlsInfo.EnvelopeImageCollection = EnvelopeImageInformationList;
                }

                controlsInfo.MaxCharacters = responseEnvelope.EnvelopeInfo.MaxCharacters;
                controlsInfo.TextTypes = responseEnvelope.EnvelopeInfo.TextTypes;
                //controlsInfo.ConditionalControlRules = responseEnvelope.EnvelopeInfo.ConditionalControlRules;
                //ConditionalControlRules conditionalControlRulesResponse = new ConditionalControlRules();
                if (responseEnvelope.EnvelopeInfo.ConditionalControlRules != null)
                {
                    controlsInfo.ConditionalControlRules.TextRules = responseEnvelope.EnvelopeInfo.ConditionalControlRules.TextRules;
                    controlsInfo.ConditionalControlRules.InitialRules = responseEnvelope.EnvelopeInfo.ConditionalControlRules.InitialRules;
                    controlsInfo.ConditionalControlRules.CheckBoxRules = responseEnvelope.EnvelopeInfo.ConditionalControlRules.CheckBoxRules;
                    controlsInfo.ConditionalControlRules.DropdownRules = responseEnvelope.EnvelopeInfo.ConditionalControlRules.DropdownRules;
                    controlsInfo.ConditionalControlRules.RadioGroupRules = responseEnvelope.EnvelopeInfo.ConditionalControlRules.RadioGroupRules;
                }
                /* Get User Settings */
                controlsInfo.UploadSignature = responseEnvelope.EnvelopeInfo.UploadSignature;
                controlsInfo.SignatureCaptureHanddrawn = responseEnvelope.EnvelopeInfo.SignatureCaptureHanddrawn;
                controlsInfo.SignatureCaptureType = responseEnvelope.EnvelopeInfo.SignatureCaptureType;
                controlsInfo.ElectronicSignIndication = responseEnvelope.EnvelopeInfo.ElectronicSignIndication;
                controlsInfo.IsDisclaimerEnabled = responseEnvelope.EnvelopeInfo.IsDisclaimerEnabled;
                controlsInfo.IsEnvelopeEditable = responseEnvelope.EnvelopeInfo.IsEnvelopeEditable;
                controlsInfo.Disclaimer = responseEnvelope.EnvelopeInfo.Disclaimer;
                controlsInfo.IsDisclaimerInCertificate = responseEnvelope.EnvelopeInfo.IsDisclaimerInCertificate;
                controlsInfo.IsSignerAttachFileReq = responseEnvelope.EnvelopeInfo.IsSignerAttachFileReq;
                controlsInfo.IsSignerAttachFileReqNew = responseEnvelope.EnvelopeInfo.IsSignerAttachFileReqNew;
                bool isSignerattachmentProcess = false;
                if (responseEnvelope.EnvelopeInfo.IsSignerAttachFileReqNew > 0)
                {
                    isSignerattachmentProcess = responseEnvelope.EnvelopeInfo.IsAdditionalAttmReq != null ? Convert.ToBoolean(responseEnvelope.EnvelopeInfo.IsAdditionalAttmReq) : false;
                }
                controlsInfo.IsAdditionalAttmReq = isSignerattachmentProcess;//responseEnvelope.EnvelopeInfo.IsAdditionalAttmReq.HasValue ? Convert.ToBoolean(responseEnvelope.EnvelopeInfo.IsAdditionalAttmReq) : false;

                controlsInfo.SignerDocs = responseEnvelope.EnvelopeInfo.SignerDocs;

                //controlsInfo.ControlsData = responseEnvelope.EnvelopeInfo.ControlsData;
                List<ControlsData> ControlsDataList = new List<ControlsData>();
                if (responseEnvelope.EnvelopeInfo.ControlsData != null && responseEnvelope.EnvelopeInfo.ControlsData.Count > 0)
                {
                    foreach (var item in responseEnvelope.EnvelopeInfo.ControlsData)
                    {
                        ControlsData controlsData = new ControlsData();
                        controlsData.Id = item.Id;
                        controlsData.PageNo = item.PageNo;
                        controlsData.DocPage = item.DocPage;
                        controlsData.Required = item.Required;
                        controlsData.Height = item.Height;
                        controlsData.Width = item.Width;
                        controlsData.XCoordinate = item.XCoordinate;
                        controlsData.YCoordinate = item.YCoordinate;
                        controlsData.ZCoordinate = item.ZCoordinate;
                        controlsData.Label = item.Label;
                        controlsData.Left = item.Left;
                        controlsData.Top = item.Top;
                        controlsData.DocumentId = item.DocumentId;
                        controlsData.RecipientId = item.RecipientId;
                        controlsData.ControlHtmlData = item.ControlHtmlData;
                        controlsData.ControlHtmlID = item.ControlHtmlID;
                        controlsData.ControlName = item.ControlName;
                        controlsData.ControlValue = item.ControlValue;
                        controlsData.signerName = item.signerName;
                        controlsData.SenderControlValue = item.SenderControlValue;
                        controlsData.IsCurrentRecipient = item.IsCurrentRecipient;
                        controlsData.IsSigned = item.IsSigned;
                        controlsData.SignatureScr = item.SignatureScr;
                        controlsData.FontColor = item.FontColor;
                        controlsData.FontName = item.FontName;
                        controlsData.FontSize = item.FontSize;
                        controlsData.FontBold = item.FontBold;
                        controlsData.FontItalic = item.FontItalic;
                        controlsData.FontUnderline = item.FontUnderline;
                        controlsData.AdditionalValidationName = item.AdditionalValidationName;
                        controlsData.AdditionalValidationOption = item.AdditionalValidationOption;
                        controlsData.SignatureText = item.SignatureText;
                        controlsData.SignatureFont = item.SignatureFont;
                        controlsData.SignatureType = item.SignatureType;
                        controlsData.GroupName = item.GroupName;
                        controlsData.ControlOptions = item.ControlOptions;
                        controlsData.MaxLength = item.MaxLength;
                        controlsData.ControlType = item.ControlType;
                        controlsData.ConditionDetails = item.ConditionDetails;
                        controlsData.LanguageControlName = item.LanguageControlName;
                        controlsData.IsReadOnly = item.IsReadOnly;
                        controlsData.TabIndex = item.TabIndex;
                        controlsData.IsSignatureFromDocumentContent = item.IsSignatureFromDocumentContent;
                        controlsData.IsDefaultRequired = item.IsDefaultRequired;
                        controlsData.CustomToolTip = item.CustomToolTip;
                        controlsData.IsFixedWidth = item.IsFixedWidth == null ? true : Convert.ToBoolean(item.IsFixedWidth) ? true : false;
                        ControlsDataList.Add(controlsData);
                    }
                }
                controlsInfo.ControlsData = ControlsDataList;

                controlsInfo.DicLabelText = responseEnvelope.EnvelopeInfo.DicLabelText;
                controlsInfo.EDisplayCode = responseEnvelope.EnvelopeInfo.EDisplayCode;
                controlsInfo.CompletedDate = responseEnvelope.EnvelopeInfo.CompletionDate;
                controlsInfo.IsTemplateShared = responseEnvelope.EnvelopeInfo.IsTemplateShared;
                controlsInfo.SubEnvelopeId = responseEnvelope.EnvelopeInfo.SubEnvelopeId;
                controlsInfo.IsSingleSigning = responseEnvelope.EnvelopeInfo.IsSingleSigning;
                controlsInfo.Controls = responseEnvelope.EnvelopeInfo.Controls;
                controlsInfo.IsStatic = responseEnvelope.EnvelopeInfo.IsStatic;
                controlsInfo.IsDefaultSignatureForStaticTemplate = responseEnvelope.EnvelopeInfo.IsDefaultSignatureForStaticTemplate;
                controlsInfo.IsSharedTemplateContentUnEditable = responseEnvelope.EnvelopeInfo.IsSharedTemplateContentUnEditable;
                controlsInfo.IsTemplateDatedBeforePortraitLandscapeFeature = responseEnvelope.EnvelopeInfo.IsTemplateDatedBeforePortraitLandscapeFeature;
                controlsInfo.ISNewSigner = responseEnvelope.EnvelopeInfo.ISNewSigner;
            }
            loggerModelNew.Message = "Process completed for Fill Envelope Info From Initalize SignDocument API method";
            rsignlog.RSignLogInfo(loggerModelNew);
            return controlsInfo;
        }

        public TemplateRoles GetRoleEntity(Guid roleId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.TemplateRoles.Where(rc => rc.ID == roleId).FirstOrDefault();
            }
        }
    }
}
