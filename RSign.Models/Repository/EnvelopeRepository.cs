using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Common.Mailer;
using RSign.Models.APIModels;
using RSign.Models.APIModels.Data;
using RSign.Models.EmailQueueProcessor;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Models;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using EmailQueueData = RSign.Models.EmailQueueProcessor.EmailQueueData;
using eSign.Models.Domain;

namespace RSign.Models.Repository
{
    public class EnvelopeRepository : IEnvelopeRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly IConditionalControlRepository _conditionalControlRepository;       
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        public bool webApiInitialiizeEnvelope { get; set; }

        private readonly IRecipientRepository _recipientRepository;
        private readonly IGenericRepository _genericRepository;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private readonly IUserRepository _userRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly ILookupRepository _lookupRepository;
        private readonly IModelHelper _modelHelper;
        private readonly IESignHelper _esignHelper;
        public EnvelopeRepository(IOptions<AppSettingsConfig> configuration, IConditionalControlRepository conditionalControlRepository, ISettingsRepository settingsRepository, IConfiguration appConfiguration,
            IRecipientRepository recipientRepository, IGenericRepository genericRepository, IEnvelopeHelperMain envelopeHelperMain, IUserRepository userRepository, ILookupRepository lookupRepository, IModelHelper modelHelper,
            IESignHelper eSignHelper)
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
            _lookupRepository = lookupRepository;
            _modelHelper = modelHelper;
            _esignHelper = eSignHelper;           
            rsignlog = new RSignLogger(_appConfiguration);
        }
        public bool IsEnvelopeExist(string ID)
        {
            loggerModelNew = new LoggerModelNew("", "Envelope Repository", "IsEnvelopeExist", "Process is started for Is Envelope Exist", ID, "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Guid envelopeID = Guid.Parse(ID);
                    var envelopeDetail = dbContext.Envelope.Where(e => e.ID == envelopeID).SingleOrDefault();
                    if (envelopeDetail == null)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool IsEnvelopeHistoryExist(string ID)
        {
            loggerModelNew = new LoggerModelNew("", "Envelope Repository", "IsEnvelopeHistoryExist", "Process is started for Is Envelope History Exist", ID, "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Guid envelopeID = Guid.Parse(ID);
                    var envelopeDetail = dbContext.EnvelopeHistory.Where(e => e.ID == envelopeID).SingleOrDefault();
                    if (envelopeDetail == null)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public EnvelopeDBResponse InsertHistoryEnvelopeToPrimary(string envelopeId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    SqlParameter[] parms = new SqlParameter[1];
                    parms[0] = new SqlParameter("@EnvelopeID", new Guid(envelopeId));
                    return dbContext.Database.SqlQueryRaw<EnvelopeDBResponse>("EXEC usp_InsertEnvelopeIntoPrimaryTable @EnvelopeID", parms).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EnvelopeDetails FillEnvelopeDetailsByEnvelopeEntity(Envelope envelope, List<ConditionalControlMapping> conditionalControlMappingData = null, bool checkconditionalControl = true)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "FillEnvelopeDetailsByEnvelopeEntity", "Process started for Fill Envelope Details By Envelope Entity", envelope.ID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            EnvelopeDetails envelopeDetails = new EnvelopeDetails();
            List<ConditionalControlMapping> conditionalControlMappings = null;
            bool checkConditionalControl = true;
            using (var dbContext = new RSignDbContext(_configuration))
            {
                if (envelope.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash || envelope.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan || envelope.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots || envelope.DateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan)
                    envelopeDetails.DateFormat = "US";
                else
                    envelopeDetails.DateFormat = "EU";
                envelopeDetails.DateFormatID = Convert.ToString(envelope.DateFormatID);
                string Password = string.Empty;

                if (envelope.PasswordReqdtoSign == true && string.IsNullOrEmpty(envelope.PasswordtoSign) == false)
                {
                    Password = ModelHelper.Decrypt(envelope.PasswordtoSign, envelope.PasswordKey, (int)envelope.PasswordKeySize);
                    envelopeDetails.PasswordToSign = Password;

                }
                else
                    envelopeDetails.PasswordToSign = null;

                if (envelope.PasswordReqdtoOpen && string.IsNullOrEmpty(envelope.PasswordtoOpen) == false)
                {
                    Password = ModelHelper.Decrypt(envelope.PasswordtoOpen, envelope.PasswordKey, (int)envelope.PasswordKeySize);
                    envelopeDetails.PasswordToOpen = Password;
                }
                else
                    envelopeDetails.PasswordToOpen = null;

                envelopeDetails.ExpiryType = EnvelopeHelper.GetExpiryType(envelope.ExpiryTypeID);
                envelopeDetails.ExpiryTypeID = Convert.ToString(envelope.ExpiryTypeID);
                envelopeDetails.EnvelopeCode = Convert.ToString(envelope.EnvelopeCode);
                envelopeDetails.TemplateCode = Convert.ToInt32(envelope.TemplateCode);
                envelopeDetails.CreatedDateTime = envelope.CreatedDateTime;
                envelopeDetails.ModifiedDatetTime = envelope.ModifiedDateTime;
                envelopeDetails.Location = envelope.Location;
                envelopeDetails.DocumentHash = envelope.DocumentHash;
                envelopeDetails.IsActive = envelope.IsActive;
                envelopeDetails.EDisplayCode = envelope.EDisplayCode;
                envelopeDetails.DownloadLinkOnManageRequired = envelope.IsFinalDocLinkReq;
                envelopeDetails.Subject = envelope.Subject;
                envelopeDetails.EnvelopeID = envelope.ID;
                envelopeDetails.Message = envelope.Message;
                envelopeDetails.IsEnvelope = envelope.IsEnvelope;
                envelopeDetails.TemplateName = envelope.TemplateName;
                envelopeDetails.TemplateDescription = envelope.TemplateDescription;
                envelopeDetails.IsDraft = envelope.IsDraft;
                envelopeDetails.IsDraftDeleted = envelope.IsDraftDeleted;
                envelopeDetails.IsDraftSend = envelope.IsDraftSend;
                envelopeDetails.SigningCertificateName = envelope.SigningCertificateName;
                envelopeDetails.SignatureCertificateRequired = envelope.IsFinalCertificateReq;
                envelopeDetails.PasswordKey = envelope.PasswordKey;
                envelopeDetails.PasswordKeySize = envelope.PasswordKeySize;
                envelopeDetails.PasswordReqdToOpen = envelope.PasswordReqdtoOpen;
                envelopeDetails.PasswordReqdToSign = envelope.PasswordReqdtoSign;
                //envelopeDetails.RemainderDays = envelope.ReminderDays;
                envelopeDetails.ReminderTypeID = envelope.ReminderTypeID;
                if (envelopeDetails.ReminderTypeID == new Guid("08E957DB-5CA5-4F1D-AC65-EEABBB7CE6FD"))
                    envelopeDetails.RemainderDays = Convert.ToInt32((envelope.ReminderDays % 365) / 7);
                else
                    envelopeDetails.RemainderDays = Convert.ToInt32(envelope.ReminderDays);

                envelopeDetails.ThenReminderTypeID = envelope.ThenReminderTypeID;
                if (envelopeDetails.ThenReminderTypeID == new Guid("08E957DB-5CA5-4F1D-AC65-EEABBB7CE6FD"))
                    envelopeDetails.ReminderRepeatDays = Convert.ToInt32((envelope.ReminderRepeatDays % 365) / 7);
                else
                    envelopeDetails.ReminderRepeatDays = Convert.ToInt32(envelope.ReminderRepeatDays);

                envelopeDetails.StatusID = envelope.StatusID;
                envelopeDetails.UserID = envelope.UserID;
                envelopeDetails.IsTransperancyDocRequired = envelope.IsTransparencyDocReq;
                envelopeDetails.IsSignerAttachFileReq = (envelope.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                envelopeDetails.IsSignerAttachFileReqNew = envelope.IsSignerAttachFileReq != null ? envelope.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                envelopeDetails.CultureInfo = envelope.CultureInfo;
                envelopeDetails.IsTemplateShared = envelope.IsTemplateShared;
                envelopeDetails.IsSequenceCheck = envelope.IsSequenceCheck;
                envelopeDetails.DocumentDetails = new List<DocumentDetails>();
                envelopeDetails.IsTemplateDeleted = envelope.IsTemplateDeleted;
                envelopeDetails.IsTemplateEditable = envelope.IsTemplateEditable;
                envelopeDetails.IsEnvelopePrepare = Convert.ToBoolean(envelope.IsEnvelopePrepare);
                envelopeDetails.IsEnvelopeComplete = envelope.IsEnvelopeComplete;
                envelopeDetails.EnvelopeTypeId = envelope.EnvelopeTypeId;
                envelopeDetails.RecipientList = new List<RecipientDetails>();
                envelopeDetails.IsStatic = envelope.IsStatic;
                envelopeDetails.IsAttachXMLDataReq = Convert.ToBoolean(envelope.IsAttachXML);
                envelopeDetails.IsSeparateMultipleDocumentsAfterSigningRequired = envelope.IsSeparateMultipleDocumentsAfterSigningRequired;
                envelopeDetails.IsWaterMark = envelope.IsWaterMark;
                envelopeDetails.WatermarkTextForSender = envelope.WatermarkTextForSender;
                envelopeDetails.WatermarkTextForOther = envelope.WatermarkTextForOther;
                envelopeDetails.AccessAuthType = Convert.ToString(envelope.AccessAuthType);
                envelopeDetails.IsRandomPassword = envelope.IsRandomPassword;
                envelopeDetails.IsPasswordMailToSigner = envelope.IsPasswordMailToSigner;
                envelopeDetails.IsEdited = Convert.ToBoolean(envelope.IsEdited);
                envelopeDetails.PostSigningLandingPage = Convert.ToString(envelope.PostSigningLandingPage);
                envelopeDetails.UserSignatureTextID = envelope.UserSignatureTextID;
                envelopeDetails.MessageTemplateTextID = envelope.MessageTemplateTextID;
                envelopeDetails.SendIndividualSignatureNotifications = envelope.SendIndividualSignatureNotifications;
                envelopeDetails.HeaderFooterOption = envelope.HeaderFooterOption;
                envelopeDetails.DisclaimerText = envelope.DisclaimerText;
                envelopeDetails.IsDisclaimerInCertificate = envelope.IsDisclaimerInCertificate;
                envelopeDetails.IsWaterMark = envelope.IsWaterMark;
                envelopeDetails.WatermarkTextForSender = envelope.WatermarkTextForSender;
                envelopeDetails.WatermarkTextForOther = envelope.WatermarkTextForOther;
                envelopeDetails.EnvelopJson = envelope.EnvelopJson;

                foreach (var recipient in envelope.Recipients)
                {
                    RecipientDetails recipientN = new RecipientDetails();

                    recipientN.ID = recipient.ID;
                    recipientN.CreatedDateTime = recipient.CreatedDateTime;
                    recipientN.RecipientName = recipient.Name;
                    recipientN.EnvelopeID = recipient.EnvelopeID;
                    recipientN.EmailID = recipient.EmailAddress;
                    recipientN.Order = recipient.Order;
                    recipientN.RecipientType = EnvelopeHelper.GetRecipentType(recipient.RecipientTypeID);
                    recipientN.RecipientTypeID = recipient.RecipientTypeID;
                    recipientN.IsSameRecipient = recipient.IsSameRecipient;
                    envelopeDetails.RecipientList.Add(recipientN);
                }
                foreach (var document in envelope.Documents)
                {
                    DocumentDetails newDoc = new DocumentDetails();
                    newDoc.DocumentName = document.DocumentName;
                    newDoc.EnvelopeID = document.EnvelopeID;
                    newDoc.ID = document.ID;
                    newDoc.UploadedDateTime = document.UploadedDateTime;
                    newDoc.Order = document.Order;
                    newDoc.ActionType = document.ActionType;

                    newDoc.documentContentDetails = new List<DocumentContentDetails>();

                    foreach (var documentContent in document.DocumentContents)
                    {
                        if (documentContent.IsControlDeleted)
                            continue;

                        DocumentContentDetails newDocContent = new DocumentContentDetails();
                        newDocContent.ID = documentContent.ID;
                        newDocContent.DocumentID = document.ID;
                        newDocContent.Label = documentContent.Label;
                        newDocContent.ControlID = documentContent.ControlID;
                        newDocContent.RecipientID = documentContent.RecipientID; ;
                        newDocContent.ControlHtmlID = documentContent.ControlHtmlID;
                        newDocContent.Required = documentContent.Required;
                        newDocContent.SenderControlValue = documentContent.SenderControlValue;
                        newDocContent.DocumentPageNo = documentContent.DocumentPageNo;
                        newDocContent.PageNo = documentContent.PageNo;
                        newDocContent.XCoordinate = documentContent.XCoordinate;
                        newDocContent.YCoordinate = documentContent.YCoordinate;
                        newDocContent.ZCoordinate = documentContent.ZCoordinate;
                        newDocContent.ControlValue = documentContent.ControlValue;
                        newDocContent.Height = documentContent.Height;
                        newDocContent.Width = documentContent.Width;
                        newDocContent.GroupName = documentContent.GroupName;
                        newDocContent.ControlHtmlData = documentContent.ControlHtmlData;
                        newDocContent.RecipientName = documentContent.RecName;
                        newDocContent.MaxLength = documentContent.MaxLength == null ? null : EnvelopeHelper.GetMaxCharacter((Guid)documentContent.MaxLength);
                        newDocContent.TextType = documentContent.ControlType == null ? null : EnvelopeHelper.GetTextType((Guid)documentContent.ControlType);
                        newDocContent.IsDefaultRequired = !string.IsNullOrEmpty(Convert.ToString(documentContent.IsDefaultRequired)) ? documentContent.IsDefaultRequired : false;
                        newDocContent.SignatureControlValue = documentContent.SignatureControlValue;
                        if (envelope.EnvelopeStage == Constants.String.RSignStage.PrepareDraft || envelope.EnvelopeStage == Constants.String.RSignStage.PrepareEditTemplate || (envelope.EnvelopeStage == Constants.String.RSignStage.PrepareEnvelope && Convert.ToBoolean(envelope.IsEdited) == true))
                        {
                            newDocContent.ConditionalControlsDetails = _conditionalControlRepository.GetAllConditionalControl(envelope.EnvelopeStage, envelope.ID, documentContent.ID, envelopeDetails, conditionalControlMappingData, false);
                        }
                        else
                        {
                            newDocContent.ConditionalControlsDetails = _conditionalControlRepository.GetAllConditionalControl(envelope.EnvelopeStage, envelope.ID, documentContent.ID, null, conditionalControlMappingData, false);
                        }
                        if (documentContent.ControlStyle != null)
                        {
                            newDocContent.controlStyleDetails = new List<ControlStyleDetails>();
                            ControlStyleDetails controlStyle = new ControlStyleDetails();
                            controlStyle.FontColor = documentContent.ControlStyle.FontColor;
                            controlStyle.FontID = documentContent.ControlStyle.FontID;
                            controlStyle.FontName = EnvelopeHelper.GetFontName(documentContent.ControlStyle.FontID);
                            controlStyle.FontSize = documentContent.ControlStyle.FontSize;
                            controlStyle.IsBold = documentContent.ControlStyle.IsBold;
                            controlStyle.IsItalic = documentContent.ControlStyle.IsItalic;
                            controlStyle.IsUnderline = documentContent.ControlStyle.IsUnderline;
                            controlStyle.AdditionalValidationName = documentContent.ControlStyle.AdditionalValidationName;
                            controlStyle.AdditionalValidationOption = documentContent.ControlStyle.AdditionalValidationOption;

                            newDocContent.ControlStyle = controlStyle;
                        }
                        if (documentContent.SelectControlOptions.Count != 0)
                        {
                            List<SelectControlOptionDetails> selectControlOptionDetails = new List<SelectControlOptionDetails>();
                            List<SelectControlOptionDetails> SelectControlOptionsEnt = new List<SelectControlOptionDetails>();
                            foreach (var opt in documentContent.SelectControlOptions)
                            {
                                SelectControlOptionDetails selectControlOptions = new SelectControlOptionDetails();
                                selectControlOptions.DocumentContentID = opt.DocumentContentID;
                                selectControlOptions.ID = opt.ID;
                                selectControlOptions.OptionText = opt.OptionText;
                                selectControlOptions.Order = opt.Order;
                                selectControlOptionDetails.Add(selectControlOptions);
                                SelectControlOptionsEnt.Add(selectControlOptions);
                            }
                            newDocContent.SelectControlOptions = SelectControlOptionsEnt;
                        }
                        newDoc.documentContentDetails.Add(newDocContent);
                    }
                    envelopeDetails.DocumentDetails.Add(newDoc);
                }
            }

            loggerModelNew.Message = "Process completed for Fill Envelope Details By Envelope Entity";
            rsignlog.RSignLogInfo(loggerModelNew);
            return envelopeDetails;
        }
        public EnvelopeDetails FillEnvelopeDetailsByEnvelopeId(Envelope envelope)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "FillEnvelopeDetailsByEnvelopeId", "Process started for Fill Envelope Details By Envelope Entity", envelope.ID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            EnvelopeDetails envelopeDetails = new EnvelopeDetails();
            using (var dbContext = new RSignDbContext(_configuration))
            {

                if (envelope.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash || envelope.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan || envelope.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots || envelope.DateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan)
                    envelopeDetails.DateFormat = "US";
                else
                    envelopeDetails.DateFormat = "EU";
                envelopeDetails.DateFormatID = Convert.ToString(envelope.DateFormatID);
                string Password = string.Empty;

                if (envelope.PasswordReqdtoSign == true && string.IsNullOrEmpty(envelope.PasswordtoSign) == false)
                {
                    Password = ModelHelper.Decrypt(envelope.PasswordtoSign, envelope.PasswordKey, (int)envelope.PasswordKeySize);
                    envelopeDetails.PasswordToSign = Password;

                }
                else
                    envelopeDetails.PasswordToSign = null;

                if (envelope.PasswordReqdtoOpen && string.IsNullOrEmpty(envelope.PasswordtoOpen) == false)
                {
                    Password = ModelHelper.Decrypt(envelope.PasswordtoOpen, envelope.PasswordKey, (int)envelope.PasswordKeySize);
                    envelopeDetails.PasswordToOpen = Password;
                }
                else
                    envelopeDetails.PasswordToOpen = null;

                envelopeDetails.ExpiryType = EnvelopeHelper.GetExpiryType(envelope.ExpiryTypeID);
                envelopeDetails.ExpiryTypeID = Convert.ToString(envelope.ExpiryTypeID);
                envelopeDetails.EnvelopeCode = Convert.ToString(envelope.EnvelopeCode);
                envelopeDetails.TemplateCode = Convert.ToInt32(envelope.TemplateCode);
                envelopeDetails.CreatedDateTime = envelope.CreatedDateTime;
                envelopeDetails.ModifiedDatetTime = envelope.ModifiedDateTime;
                envelopeDetails.Location = envelope.Location;
                envelopeDetails.DocumentHash = envelope.DocumentHash;
                envelopeDetails.IsActive = envelope.IsActive;
                envelopeDetails.EDisplayCode = envelope.EDisplayCode;
                envelopeDetails.DownloadLinkOnManageRequired = envelope.IsFinalDocLinkReq;
                envelopeDetails.Subject = envelope.Subject;
                envelopeDetails.EnvelopeID = envelope.ID;
                envelopeDetails.Message = envelope.Message;
                envelopeDetails.IsEnvelope = envelope.IsEnvelope;
                envelopeDetails.TemplateName = envelope.TemplateName;
                envelopeDetails.TemplateDescription = envelope.TemplateDescription;
                envelopeDetails.IsDraft = envelope.IsDraft;
                envelopeDetails.IsDraftDeleted = envelope.IsDraftDeleted;
                envelopeDetails.IsDraftSend = envelope.IsDraftSend;
                envelopeDetails.SigningCertificateName = envelope.SigningCertificateName;
                envelopeDetails.SignatureCertificateRequired = envelope.IsFinalCertificateReq;
                envelopeDetails.PasswordKey = envelope.PasswordKey;
                envelopeDetails.PasswordKeySize = envelope.PasswordKeySize;
                envelopeDetails.PasswordReqdToOpen = envelope.PasswordReqdtoOpen;
                envelopeDetails.PasswordReqdToSign = envelope.PasswordReqdtoSign;
                //envelopeDetails.RemainderDays = envelope.ReminderDays;
                envelopeDetails.ReminderTypeID = envelope.ReminderTypeID;
                if (envelopeDetails.ReminderTypeID == new Guid("08E957DB-5CA5-4F1D-AC65-EEABBB7CE6FD"))
                    envelopeDetails.RemainderDays = Convert.ToInt32((envelope.ReminderDays % 365) / 7);
                else
                    envelopeDetails.RemainderDays = Convert.ToInt32(envelope.ReminderDays);

                envelopeDetails.ThenReminderTypeID = envelope.ThenReminderTypeID;
                if (envelopeDetails.ThenReminderTypeID == new Guid("08E957DB-5CA5-4F1D-AC65-EEABBB7CE6FD"))
                    envelopeDetails.ReminderRepeatDays = Convert.ToInt32((envelope.ReminderRepeatDays % 365) / 7);
                else
                    envelopeDetails.ReminderRepeatDays = Convert.ToInt32(envelope.ReminderRepeatDays);

                envelopeDetails.StatusID = envelope.StatusID;
                envelopeDetails.UserID = envelope.UserID;
                envelopeDetails.IsTransperancyDocRequired = envelope.IsTransparencyDocReq;
                envelopeDetails.IsSignerAttachFileReq = (envelope.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                envelopeDetails.IsSignerAttachFileReqNew = envelope.IsSignerAttachFileReq != null ? envelope.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                envelopeDetails.CultureInfo = envelope.CultureInfo;
                envelopeDetails.IsTemplateShared = envelope.IsTemplateShared;
                envelopeDetails.IsSequenceCheck = envelope.IsSequenceCheck;
                envelopeDetails.DocumentDetails = new List<DocumentDetails>();
                envelopeDetails.IsTemplateDeleted = envelope.IsTemplateDeleted;
                envelopeDetails.IsTemplateEditable = envelope.IsTemplateEditable;
                envelopeDetails.IsEnvelopePrepare = Convert.ToBoolean(envelope.IsEnvelopePrepare);
                envelopeDetails.IsEnvelopeComplete = envelope.IsEnvelopeComplete;
                envelopeDetails.EnvelopeTypeId = envelope.EnvelopeTypeId;
                envelopeDetails.RecipientList = new List<RecipientDetails>();
                envelopeDetails.IsStatic = envelope.IsStatic;
                envelopeDetails.IsAttachXMLDataReq = Convert.ToBoolean(envelope.IsAttachXML);
                envelopeDetails.IsSeparateMultipleDocumentsAfterSigningRequired = envelope.IsSeparateMultipleDocumentsAfterSigningRequired;
                envelopeDetails.IsWaterMark = envelope.IsWaterMark;
                envelopeDetails.WatermarkTextForSender = envelope.WatermarkTextForSender;
                envelopeDetails.WatermarkTextForOther = envelope.WatermarkTextForOther;
                envelopeDetails.AccessAuthType = Convert.ToString(envelope.AccessAuthType);
                envelopeDetails.IsRandomPassword = envelope.IsRandomPassword;
                envelopeDetails.IsPasswordMailToSigner = envelope.IsPasswordMailToSigner;
                envelopeDetails.IsEdited = Convert.ToBoolean(envelope.IsEdited);
                envelopeDetails.PostSigningLandingPage = Convert.ToString(envelope.PostSigningLandingPage);
                envelopeDetails.UserSignatureTextID = envelope.UserSignatureTextID;
                envelopeDetails.MessageTemplateTextID = envelope.MessageTemplateTextID;
                envelopeDetails.SendIndividualSignatureNotifications = envelope.SendIndividualSignatureNotifications;
                envelopeDetails.HeaderFooterOption = envelope.HeaderFooterOption;
                envelopeDetails.DisclaimerText = envelope.DisclaimerText;
                envelopeDetails.IsDisclaimerInCertificate = envelope.IsDisclaimerInCertificate;
                envelopeDetails.EnvelopJson = envelope.EnvelopJson;
                envelopeDetails.EnableCcOptions = Convert.ToBoolean(envelope.EnableCcOptions);
                envelopeDetails.EnableRecipientLanguage = envelope.EnableRecipientLanguage;
                envelopeDetails.EnableMessageToMobile = envelope.EnableMessageToMobile;
                Status status = dbContext.Status.Where(e => e.ID == envelope.StatusID).FirstOrDefault();

                if (status != null)
                {
                    envelopeDetails.Status = status.Description;
                }

                foreach (var recipient in envelope.Recipients)
                {
                    RecipientDetails recipientN = new RecipientDetails();

                    recipientN.ID = recipient.ID;
                    recipientN.CreatedDateTime = recipient.CreatedDateTime;
                    recipientN.RecipientName = recipient.Name;
                    recipientN.EnvelopeID = recipient.EnvelopeID;
                    recipientN.EmailID = recipient.EmailAddress;
                    recipientN.Order = recipient.Order;
                    recipientN.RecipientType = EnvelopeHelper.GetRecipentType(recipient.RecipientTypeID);
                    recipientN.RecipientTypeID = recipient.RecipientTypeID;
                    recipientN.DeliveryMode = recipient.DeliveryMode;
                    recipientN.CountryCode = recipient.CountryCode;
                    recipientN.DialCode = recipient.DialCode;
                    recipientN.Mobile = recipient.Mobile;
                   // recipientN.ReminderType = recipient.ReminderType;

                    envelopeDetails.RecipientList.Add(recipientN);
                }
                foreach (var document in envelope.Documents)
                {
                    DocumentDetails newDoc = new DocumentDetails();
                    newDoc.DocumentName = document.DocumentName;
                    newDoc.EnvelopeID = document.EnvelopeID;
                    newDoc.ID = document.ID;
                    newDoc.UploadedDateTime = document.UploadedDateTime;
                    newDoc.Order = document.Order;

                    newDoc.documentContentDetails = new List<DocumentContentDetails>();

                    foreach (var documentContent in document.DocumentContents)
                    {
                        if (documentContent.IsControlDeleted)
                            continue;

                        DocumentContentDetails newDocContent = new DocumentContentDetails();
                        newDocContent.ID = documentContent.ID;
                        newDocContent.DocumentID = document.ID;
                        newDocContent.Label = documentContent.Label;
                        newDocContent.ControlID = documentContent.ControlID;
                        newDocContent.RecipientID = documentContent.RecipientID; ;
                        newDocContent.ControlHtmlID = documentContent.ControlHtmlID;
                        newDocContent.Required = documentContent.Required;
                        newDocContent.SenderControlValue = documentContent.SenderControlValue;
                        newDocContent.DocumentPageNo = documentContent.DocumentPageNo;
                        newDocContent.PageNo = documentContent.PageNo;
                        newDocContent.XCoordinate = documentContent.XCoordinate;
                        newDocContent.YCoordinate = documentContent.YCoordinate;
                        newDocContent.ZCoordinate = documentContent.ZCoordinate;
                        newDocContent.ControlValue = documentContent.ControlValue;
                        newDocContent.Height = documentContent.Height;
                        newDocContent.Width = documentContent.Width;
                        newDocContent.GroupName = documentContent.GroupName;
                        newDocContent.ControlHtmlData = documentContent.ControlHtmlData;
                        newDocContent.RecipientName = documentContent.RecName;
                        newDocContent.MaxLength = documentContent.MaxLength == null ? null : EnvelopeHelper.GetMaxCharacter((Guid)documentContent.MaxLength);
                        newDocContent.TextType = documentContent.ControlType == null ? null : EnvelopeHelper.GetTextType((Guid)documentContent.ControlType);
                        if (envelope.EnvelopeStage == Constants.String.RSignStage.PrepareDraft || envelope.EnvelopeStage == Constants.String.RSignStage.PrepareEditTemplate || (envelope.EnvelopeStage == Constants.String.RSignStage.PrepareEnvelope && Convert.ToBoolean(envelope.IsEdited) == true))
                        {
                            newDocContent.ConditionalControlsDetails = _conditionalControlRepository.GetAllConditionalControl(envelope.EnvelopeStage, envelope.ID, documentContent.ID, envelopeDetails);
                        }
                        else
                        {
                            newDocContent.ConditionalControlsDetails = _conditionalControlRepository.GetAllConditionalControl(envelope.EnvelopeStage, envelope.ID, documentContent.ID, null);
                        }
                        if (documentContent.ControlStyle != null)
                        {
                            newDocContent.controlStyleDetails = new List<ControlStyleDetails>();
                            ControlStyleDetails controlStyle = new ControlStyleDetails();
                            controlStyle.FontColor = documentContent.ControlStyle.FontColor;
                            controlStyle.FontID = documentContent.ControlStyle.FontID;
                            controlStyle.FontName = EnvelopeHelper.GetFontName(documentContent.ControlStyle.FontID);
                            controlStyle.FontSize = documentContent.ControlStyle.FontSize;
                            controlStyle.IsBold = documentContent.ControlStyle.IsBold;
                            controlStyle.IsItalic = documentContent.ControlStyle.IsItalic;
                            controlStyle.IsUnderline = documentContent.ControlStyle.IsUnderline;
                            controlStyle.AdditionalValidationName = documentContent.ControlStyle.AdditionalValidationName;
                            controlStyle.AdditionalValidationOption = documentContent.ControlStyle.AdditionalValidationOption;
                            newDocContent.ControlStyle = controlStyle;
                        }
                        if (documentContent.SelectControlOptions.Count != 0)
                        {
                            List<SelectControlOptionDetails> selectControlOptionDetails = new List<SelectControlOptionDetails>();
                            List<SelectControlOptionDetails> SelectControlOptionsEnt = new List<SelectControlOptionDetails>();
                            foreach (var opt in documentContent.SelectControlOptions)
                            {
                                SelectControlOptionDetails selectControlOptions = new SelectControlOptionDetails();
                                selectControlOptions.DocumentContentID = opt.DocumentContentID;
                                selectControlOptions.ID = opt.ID;
                                selectControlOptions.OptionText = opt.OptionText;
                                selectControlOptions.Order = opt.Order;
                                selectControlOptionDetails.Add(selectControlOptions);
                                SelectControlOptionsEnt.Add(selectControlOptions);
                            }
                            newDocContent.SelectControlOptions = SelectControlOptionsEnt;
                        }
                        newDoc.documentContentDetails.Add(newDocContent);
                    }
                    envelopeDetails.DocumentDetails.Add(newDoc);
                }
            }

            loggerModelNew.Message = "Process completed for Fill Envelope Details By Envelope Entity";
            rsignlog.RSignLogInfo(loggerModelNew);
            return envelopeDetails;
        }
        public List<EnvelopeAdditionalUploadInfoDetails> GetEnvelopeAdditionalUploadInfoByEnvelopebyRecipeint(Guid masterEnvelopeID, Guid RecipientID, string additionalRecipients = null)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    List<Guid> AddlRecipients = new List<Guid>();

                    if (additionalRecipients == null) { additionalRecipients = string.Empty; }

                    if (!string.IsNullOrEmpty(additionalRecipients.Trim()))
                    {
                        AddlRecipients = additionalRecipients.Split(',').Select(g => { Guid temp; return Guid.TryParse(g, out temp) ? temp : Guid.Empty; }).Where(g => g != Guid.Empty).ToList();
                    }
                    var envelopeAdditionalUploadInfoList = dbContext.EnvelopeAdditionalUploadInfo.Where(x => x.MasterEnvelopeID == masterEnvelopeID).Select(x => new EnvelopeAdditionalUploadInfoDetails
                    {
                        ID = x.ID,
                        MasterEnvelopeID = x.MasterEnvelopeID,
                        Name = x.Name,
                        Description = x.Description,
                        AdditionalInfo = x.AdditionalInfo,
                        FileName = x.FileName,
                        OriginalFileName = x.OriginalFileName,
                        IsActive = x.IsActive,
                        IsRequired = x.IsRequired,
                        UploadedDateTime = x.ModifiedDateTime,
                        RecipientID = x.RecipientID
                    }).ToList();

                    if (RecipientID != new Guid())
                    {
                        List<EnvelopeAdditionalUploadInfoDetails> AddlRecipientList = new List<EnvelopeAdditionalUploadInfoDetails>();
                        if (AddlRecipients != null && AddlRecipients.Count > 0)
                        {
                            AddlRecipientList = envelopeAdditionalUploadInfoList.Where(d => d.RecipientID != Guid.Empty && AddlRecipients.Contains(d.RecipientID.Value)).ToList();
                        }

                        var recipentIDs = envelopeAdditionalUploadInfoList.Where(d => d.RecipientID == RecipientID).ToList();
                        envelopeAdditionalUploadInfoList = envelopeAdditionalUploadInfoList.Where(d => d.RecipientID == new Guid()).ToList();
                        envelopeAdditionalUploadInfoList.AddRange(recipentIDs);

                        if (AddlRecipientList != null && AddlRecipientList.Count > 0)
                        {
                            envelopeAdditionalUploadInfoList.AddRange(AddlRecipientList);
                        }
                    }
                    if (envelopeAdditionalUploadInfoList.Count > 0)
                    {
                        envelopeAdditionalUploadInfoList = envelopeAdditionalUploadInfoList.OrderBy(a => a.ID).ToList();
                    }
                    return envelopeAdditionalUploadInfoList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetMaxUploadsID()
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var UploadsIDMax = dbContext.EnvelopeAdditionalUploadInfo.OrderByDescending(x => x.ID).First().ID;
                    return Convert.ToInt32(UploadsIDMax);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //public Envelope GetEnvelopeRecipients(Guid envelopeID)
        //{
        //    try
        //    {
        //        loggerModelNew = new LoggerModelNew("", "EnvelopeRepository", "GetEnvelopeRecipients", "Get Envelope Recipients Entity process is started ", envelopeID.ToString(), "", "", "", "API");
        //        rsignlog.RSignLogInfo(loggerModelNew);

        //        using (var dbContext = new RSignDbContext(_configuration))
        //        {
        //            Envelope envelope = dbContext.Envelope.Where(e => e.ID == envelopeID).FirstOrDefault();
        //            if (envelope == null)
        //                return GetEntityHistory(envelopeID);

        //            var Recipients = (from r in dbContext.Recipients
        //                              where r.EnvelopeID == envelope.ID
        //                              select r).ToList();

        //            foreach (var recipient in Recipients)
        //            {
        //                envelope.Recipients.Add(recipient);
        //            }
        //            return envelope;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        //public Envelope GetEnvelopeEntity(Guid envelopeID)
        //{
        //    try
        //    {
        //        loggerModelNew = new LoggerModelNew("", "EnvelopeRepository", "GetEnvelopeEntity", "Get Envelope Entity process is started ", envelopeID.ToString(), "", "", "", "API");
        //        rsignlog.RSignLogInfo(loggerModelNew);

        //        using (var dbContext = new RSignDbContext(_configuration))
        //        {
        //            Envelope envelope = dbContext.Envelope.Where(e => e.ID == envelopeID).FirstOrDefault();
        //            if (envelope == null)
        //                return GetEntityHistory(envelopeID);

        //            return envelope;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        public EnvelopeSettingsDetail GetEnvelopeSettingsDetail(Guid envelopeID, int IsEnvelopeArichived = 0)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                if (IsEnvelopeArichived != 1)
                    return (dbContext.EnvelopeSettingsDetail.Where(q => q.EnvelopeId == envelopeID).FirstOrDefault());
                else
                    return this.GetArchiveEnvelopeSettingsDetail(envelopeID);
                // return dbContext.EnvelopeSettingsDetail.Where(e => e.EnvelopeId == envelopeID).FirstOrDefault();
            }
        }
        public async Task<ErrorResponseModel> UpdateVerificationCodeEmail(UserVerificationModel userVerificationModel)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            loggerModelNew = new LoggerModelNew("", "EnvelopeRepository", "UpdateVerificationCodeEmail", "Initiate the process for Update Verification Code using API.", userVerificationModel.envelopeID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                string randomNumber = _genericRepository.GenerateRandomVerificationCode();
                var existingRecipients = _recipientRepository.GetEntity(new Guid(userVerificationModel.recipientId));
                if (existingRecipients != null)
                {
                    var envelope = _genericRepository.GetEnvelopeRecipients(new Guid(userVerificationModel.envelopeID));
                    string Message = string.Empty;
                    Guid signerStatusID = Guid.Empty;
                    foreach (var rec in envelope.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Signer && x.ID == new Guid(userVerificationModel.recipientId) && x.IsSameRecipient != true)
                        .OrderBy(o => o.CreatedDateTime))
                    {
                        var signerStatus = _recipientRepository.GetSignerStatus(rec.ID);
                        if (signerStatus == null || (signerStatus != null && signerStatus.StatusID != Constants.StatusCode.Recipients.Transferred))
                            signerStatusID = signerStatus.StatusID;
                    }
                    if (signerStatusID == Constants.StatusCode.Signer.Signed)
                    {
                        Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "ConatctSender");
                        errorResponseModel.Message = Message;
                        errorResponseModel.ErrorAction = "ContactSender";
                        errorResponseModel.Status = false;
                        loggerModelNew.Message = Message;
                        rsignlog.RSignLogInfo(loggerModelNew);
                        return errorResponseModel;
                    }

                    if (envelope.StatusID == Constants.StatusCode.Envelope.Completed)
                    {
                        Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "ConatctSender");
                        errorResponseModel.Message = Message;
                        errorResponseModel.ErrorAction = "ContactSender";
                        errorResponseModel.Status = false;
                        loggerModelNew.Message = errorResponseModel.Message;
                        rsignlog.RSignLogWarn(loggerModelNew);
                        return errorResponseModel;
                    }

                    var userprofile = _userRepository.GetUserProfile(envelope.UserID);
                    bool isEmailSend = false, isPwdEmailSend = false;
                    bool isMobileSend = false, isPwdMobileSend = false;
                    if (Convert.ToBoolean(envelope.EnableMessageToMobile))
                    {
                        _envelopeHelperMain.GetEmailDeiliveryModeOptions(existingRecipients, ref isEmailSend, ref isMobileSend);
                        _envelopeHelperMain.GetMobileDeiliveryModeOptions(existingRecipients, ref isPwdEmailSend, ref isPwdMobileSend);
                    }
                    else
                    {
                        isPwdEmailSend = true;
                        isEmailSend = true;
                    }
                    string EmailDisclaimer = string.Empty;
                    int SignReqReplyToAddressValue = 1;      
                    var settingsDetails = _settingsRepository.GetEntityForByKeyConfig(userprofile.CompanyID.Value, Constants.SettingsKeyConfig.EmailDisclaimer);
                    if (settingsDetails != null)
                        EmailDisclaimer = Convert.ToString(settingsDetails.OptionValue);

                    settingsDetails = _settingsRepository.GetEntityForByKeyConfig(envelope.UserID, Constants.SettingsKeyConfig.SignatureRequestReplyAddress);
                    var envelopeSettingObject = GetEnvelopeSettingsDetail(envelope.ID);
                    if (envelopeSettingObject != null)
                        SignReqReplyToAddressValue = Convert.ToInt32(envelopeSettingObject.SignReqReplyAdrs);

                    if (isPwdEmailSend)
                    {
                        string mailTemplate = string.Empty;
                        string[] toAddressForPw, toDisplayNameForPw;
                        //* Added by RLabs-RSign DEV Team
                        var allSettingsDetails = _envelopeHelperMain.GetAllSettingsDetails(envelope.UserID, userprofile.EmailID);
                        mailTemplate = _genericRepository.GetNewMailTemplate(Constants.String.MailTemplateName.VerificationCode, envelope.CultureInfo, EmailDisclaimer);
                        toAddressForPw = envelope.Recipients.Where(r => r.ID == new Guid(userVerificationModel.recipientId)).Select(r => r.EmailAddress).ToArray();
                        toDisplayNameForPw = envelope.Recipients.Where(r => r.ID == new Guid(userVerificationModel.recipientId)).Select(r => r.Name).ToArray();
                        mailTemplate = _genericRepository.CreateVerificationCodeMailTemplate(envelope, "", userprofile.EmailID, userprofile.FullName, existingRecipients, userprofile.FirstName, mailTemplate, userVerificationModel.EmailId, existingRecipients.Name, randomNumber);
                        mailTemplate = _envelopeHelperMain.EmailBannerSettings(mailTemplate, allSettingsDetails, Constants.String.MailTemplateName.VerificationCode);

                        mailTemplate = _genericRepository.AppendFooterText(mailTemplate, Convert.ToString(toAddressForPw[0]), Constants.String.MailTemplateName.VerificationCode, envelope.CultureInfo, "FinalContractFooter");

                        if (Convert.ToString(_appConfiguration["SendEmailFromService"]) != null && Convert.ToString(_appConfiguration["SendEmailFromService"]).ToUpper() == "TRUE")
                        {
                            //To Add data in Emailqueue and EmailqueueRecipients for Delegate Electronic Signature  Notification 
                            EmailQueueData emailQueueData = new EmailQueueData();
                            EmailSendInfo emailSendData = new EmailSendInfo();
                            emailQueueData.envelope = envelope;
                            emailQueueData.MailMessageBody = mailTemplate;
                            emailQueueData.SignReqReplyToAddressValue = SignReqReplyToAddressValue;
                            emailQueueData.EmailType = Constants.EmailTypes.ESR;
                            emailQueueData.EmailSubject = _envelopeHelperMain.GetEmailSubjectPrefix(envelope.CultureInfo, envelope.Subject, Constants.EmailSubject.ElectronicSignatureVerificationCode);
                            emailSendData.RecipientName = string.Join(",", toDisplayNameForPw);
                            emailSendData.RecipientEmail = string.Join(",", toAddressForPw);
                            emailQueueData.emailSendInfo = emailSendData;
                            _envelopeHelperMain.EmailQueueFunction(emailQueueData);
                        }
                        else
                        {
                            var Chilkat = new ChilkatHelper(_appConfiguration);
                            Chilkat.SendMailUsingChilKet(toAddressForPw, toDisplayNameForPw, null, null, null, null, userprofile.EmailID, userprofile.FullName, _envelopeHelperMain.GetEmailSubjectPrefix(envelope.CultureInfo, envelope.Subject, Constants.EmailSubject.ElectronicSignatureVerificationCode),
                             mailTemplate, null, null, envelope.EDisplayCode, Constants.String.EmailOperation.VerificationCode, SignReqReplyToAddressValue);
                        }
                    }

                    if (isPwdMobileSend)
                    {
                        string mobileTemplate = _genericRepository.GetNewMailTemplate(Constants.String.MailTemplateName.VerificationCode, envelope.CultureInfo, EmailDisclaimer, 3, "mobile");
                        mobileTemplate = mobileTemplate.Replace("#SenderName", userprofile.FullName);
                        mobileTemplate = mobileTemplate.Replace("#Password", randomNumber);
                        mobileTemplate = mobileTemplate.Replace("#EnvelopeDisplayCode", Convert.ToString(envelope.EDisplayCode));

                        if (_appConfiguration["SendEmailFromService"] != null && Convert.ToString(_appConfiguration["SendEmailFromService"]).ToUpper() == "TRUE")
                        {
                            string emailSubject = _envelopeHelperMain.GetEmailSubjectPrefix(envelope.CultureInfo, envelope.Subject, Constants.EmailSubject.ElectronicSignatureVerificationCode);
                            _envelopeHelperMain.SendSMSThroughEmailService(envelope, mobileTemplate, SignReqReplyToAddressValue, Constants.EmailTypes.SR, existingRecipients, emailSubject);
                        }
                        else
                        {
                            string signerMobileNumber = existingRecipients.DialCode + existingRecipients.Mobile;
                            _envelopeHelperMain.SendMobileSMS(envelope, mobileTemplate, signerMobileNumber);
                        }
                    }

                    if (existingRecipients.ID != Guid.Empty)
                    {
                        existingRecipients.VerificationCode = randomNumber;
                        _recipientRepository.Save(existingRecipients);
                        errorResponseModel.DeliveryMode = existingRecipients.DeliveryMode;
                        errorResponseModel.Mobile = existingRecipients.DialCode + existingRecipients.Mobile;
                        errorResponseModel.EnableMessageToMobile = Convert.ToBoolean(envelope.EnableMessageToMobile) ? "1" : "0";
                        errorResponseModel.Message = "Success";
                        errorResponseModel.Status = true;
                        loggerModelNew.Message = "Process completed for Update Verification Code using API and " + errorResponseModel.Message;
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
                else
                {
                    #region Envelope Arichived or not
                    ArichiveEnvelopesInfo envelopesInfo = _genericRepository.GetArchivedEnvelope(string.Empty, new Guid(userVerificationModel.envelopeID));
                    if (envelopesInfo != null)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.OK;
                        errorResponseModel.StatusMessage = "OK";
                        errorResponseModel.Message = envelopesInfo.ArchivedEnvelopeMessage;
                        errorResponseModel.IsEnvelopePurging = true;
                        errorResponseModel.Status = true;
                        loggerModelNew.Message = errorResponseModel.Message;
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
                    #endregion
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                errorResponseModel.Status = false;
                errorResponseModel.Message = "Error occurred in EnvelopeRepository and UpdateVerificationCodeEmail method." + ex.Message;
                return errorResponseModel;
            }
        }
        public EnvelopeAdditionalUploadInfo GetEnvelopeAdditionalUploadInfoByID(int ID, Guid RecipientID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    EnvelopeAdditionalUploadInfo EnvelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                    EnvelopeAdditionalUploadInfo = dbContext.EnvelopeAdditionalUploadInfo.FirstOrDefault(u => u.ID == ID);
                    return EnvelopeAdditionalUploadInfo;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateEnvelopeAdditionalUploadInfo(EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeAdditionalUploadInfoObject = dbContext.EnvelopeAdditionalUploadInfo.FirstOrDefault(x => x.ID == envelopeAdditionalUploadInfo.ID);
                    if (envelopeAdditionalUploadInfoObject != null)
                    {
                        envelopeAdditionalUploadInfoObject.Name = envelopeAdditionalUploadInfo.Name;
                        envelopeAdditionalUploadInfoObject.Description = envelopeAdditionalUploadInfo.Description;
                        envelopeAdditionalUploadInfoObject.AdditionalInfo = envelopeAdditionalUploadInfo.AdditionalInfo;
                        envelopeAdditionalUploadInfoObject.FileName = envelopeAdditionalUploadInfo.FileName;
                        envelopeAdditionalUploadInfoObject.OriginalFileName = envelopeAdditionalUploadInfo.OriginalFileName;
                        envelopeAdditionalUploadInfoObject.ModifiedDateTime = DateTime.Now;
                        dbContext.Entry(envelopeAdditionalUploadInfoObject).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                        dbContext.SaveChanges();
                        var envelope = dbContext.Envelope.Where(d => d.ID == envelopeAdditionalUploadInfoObject.MasterEnvelopeID).FirstOrDefault();
                        if (envelope != null)
                        {
                            envelope.ModifiedDateTime = DateTime.Now;
                            dbContext.Entry(envelope).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool RemoveEnvelopeAdditionalUploadInfo(EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    dbContext.EnvelopeAdditionalUploadInfo.Remove(envelopeAdditionalUploadInfo);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return true;
            }
        }
        public bool DeleteEnvelopeAdditionalUploadInfo(int uploadInfoId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    EnvelopeAdditionalUploadInfo doc = dbContext.EnvelopeAdditionalUploadInfo.Where(d => d.ID == uploadInfoId).FirstOrDefault();
                    if (doc != null)
                    {
                        dbContext.EnvelopeAdditionalUploadInfo.Remove(dbContext.EnvelopeAdditionalUploadInfo.FirstOrDefault(d => d.ID == uploadInfoId));
                        dbContext.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public EnvelopeAdditionalUploadInfo GetEnvelopeUploadInfoByID(long ID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    EnvelopeAdditionalUploadInfo EnvelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                    EnvelopeAdditionalUploadInfo = dbContext.EnvelopeAdditionalUploadInfo.FirstOrDefault(u => u.ID == ID);
                    return EnvelopeAdditionalUploadInfo;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TemplateGroupDocumentUploadDetails> GetEnvelopeAdditionalDocument(Guid masterEnvelopeID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeAdditionalUploadInfoList = dbContext.EnvelopeAdditionalUploadInfo.Where(x => x.MasterEnvelopeID == masterEnvelopeID).Select(x => new TemplateGroupDocumentUploadDetails
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Description = x.Description,
                        AdditionalInfo = x.AdditionalInfo,
                        FileName = x.FileName,
                        IsRequired = x.IsRequired,
                        RecipientId = x.RecipientID,
                        RecipientEmail = x.RecipientEmailID,
                        IsActive = false // x.IsActive.Value

                    }).ToList();

                    return envelopeAdditionalUploadInfoList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveEnvelopeAdditionalUploadInfo(EnvelopeAdditionalUploadInfo newObj)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    dbContext.EnvelopeAdditionalUploadInfo.Add(newObj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateEnvelopePrefillSigner(Guid envelopeID, string draftType)
        {
            var loggerModelNew = new LoggerModelNew("", "EnvelopeRepository", "UpdateEnvelopePrefillSigner", "Process started for Update Envelope Prefill Signer", "", "", "", "", "UpdateEnvelopePrefillSigner");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeDetail = dbContext.Envelope.Where(e => e.ID == envelopeID).SingleOrDefault();
                    if (envelopeDetail != null)
                    {
                        envelopeDetail.DraftType = draftType;
                        envelopeDetail.IsDraft = draftType == string.Empty ? false : envelopeDetail.IsDraft;

                        if (dbContext.Entry(envelopeDetail).State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                            dbContext.Entry(envelopeDetail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                        dbContext.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return false;
            }
        }
        public EnvelopeContent GetEnvelopeContent(Guid envelopeID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.EnvelopeContent.Where(t => t.EnvelopeID == envelopeID).First();
            }
        }

        /// <summary>
        /// Save envelope conr
        /// </summary>
        /// <param name="envelopeContent"></param>
        /// <returns>True, if success, false otherwise</returns>
        public bool Save(EnvelopeContent envelopeContent)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeObject = dbContext.EnvelopeContent.Where(e => e.ID == envelopeContent.ID).FirstOrDefault();
                    if (envelopeObject == null)
                    {
                        dbContext.EnvelopeContent.Add(envelopeContent);
                    }
                    else
                    {
                        envelopeObject.ContentXML = envelopeContent.ContentXML;
                        dbContext.Entry(envelopeObject).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public List<EnvelopeAdditionalUploadInfoDetailsDelegate> GetEnvelopeAdditionalUploadInfoByDelegate(Guid masterEnvelopeID, Guid RecipientID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeAdditionalUploadInfoList = dbContext.EnvelopeAdditionalUploadInfo.Where(x => x.MasterEnvelopeID == masterEnvelopeID && x.RecipientID == RecipientID)
                        .Select(x => new EnvelopeAdditionalUploadInfoDetailsDelegate
                        {
                            ID = x.ID,
                            MasterEnvelopeID = x.MasterEnvelopeID,
                            RecipientID = x.RecipientID,
                            RecipientEmailID = x.RecipientEmailID
                        }).ToList();

                    return envelopeAdditionalUploadInfoList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateEnvelopeAdditionalUploadInfobyDelegate(EnvelopeAdditionalUploadInfoDetailsDelegate envelopeAdditionalUploadInfo)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeAdditionalUploadInfoObject = dbContext.EnvelopeAdditionalUploadInfo.FirstOrDefault(x => x.ID == envelopeAdditionalUploadInfo.ID);
                    if (envelopeAdditionalUploadInfoObject != null)
                    {
                        envelopeAdditionalUploadInfoObject.RecipientEmailID = envelopeAdditionalUploadInfo.RecipientEmailID;
                        envelopeAdditionalUploadInfoObject.RecipientID = envelopeAdditionalUploadInfo.RecipientID;
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void UpdateRAppNotificationEvents(Recipients recipientDetail, Envelope envelope)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var rappNotificationEventList = dbContext.RAppNotificationEvents.Where(n => n.RecipientId == recipientDetail.ID && n.NotificationTypeCode == "SigningRequest" && n.EnvelopeId == envelope.ID).ToList();
                    if (rappNotificationEventList != null && rappNotificationEventList.Count() > 0)
                    {
                        foreach (var item in rappNotificationEventList)
                        {
                            item.IsRead = true;
                            item.DateModified = DateTime.UtcNow;
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        public bool Save(EnvelopeStatus envelopeStatus)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    dbContext.EnvelopeStatus.Add(envelopeStatus);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool RemovePrefillEnvelopeFromDrafts(Guid envelopeId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Envelope envelope = dbContext.Envelope.Where(e => e.ID == envelopeId).FirstOrDefault();
                    if (envelope != null)
                    {
                        envelope.DraftType = "";
                        envelope.ModifiedDateTime = DateTime.Now;
                        dbContext.Entry(envelope).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public List<TemplateGroupDocumentUploadDetails> GetTemplateAdditionalDocument(Guid templateId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var additionalUploadInfoList = dbContext.TemplateGroupDocumentUpload.Where(x => x.TemplateId == templateId).Select(x => new TemplateGroupDocumentUploadDetails
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Description = x.Description,
                        AdditionalInfo = x.AdditionalInfo,
                        FileName = x.FileName,
                        IsRequired = x.IsRequired,
                        RecipientId = x.RecipientID,
                        RecipientEmail = x.RecipientEmailID,
                        IsActive = x.IsActive.Value

                    }).ToList();

                    return additionalUploadInfoList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveEnvelopeAdditionalAttachment(List<TemplateGroupDocumentUploadDetails> envelopeAttachmentsList, Guid? EnvelopeID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    List<TemplateGroupDocumentUploadDetails> envelopeGroupDocumentUploadDetailsList = new List<TemplateGroupDocumentUploadDetails>();
                    if (envelopeAttachmentsList != null && envelopeAttachmentsList.Count > 0)
                    {
                        foreach (var item in envelopeAttachmentsList)
                        {
                            if (!string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(item.Description))
                            {
                                envelopeGroupDocumentUploadDetailsList.Add(item);
                            }
                        }
                    }
                    DeleteEnvelopeAdditionalAttachment(EnvelopeID);
                    if (envelopeGroupDocumentUploadDetailsList.Count > 0)
                    {
                        foreach (var templateUploads in envelopeGroupDocumentUploadDetailsList)
                        {
                            EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                            envelopeAdditionalUploadInfo.MasterEnvelopeID = EnvelopeID;
                            envelopeAdditionalUploadInfo.Name = templateUploads.Name;
                            envelopeAdditionalUploadInfo.Description = templateUploads.Description;
                            envelopeAdditionalUploadInfo.AdditionalInfo = templateUploads.AdditionalInfo;
                            envelopeAdditionalUploadInfo.FileName = templateUploads.FileName;
                            envelopeAdditionalUploadInfo.IsActive = templateUploads.IsActive;
                            envelopeAdditionalUploadInfo.IsRequired = templateUploads.IsRequired;
                            envelopeAdditionalUploadInfo.RecipientID = templateUploads.RecipientId != null ? templateUploads.RecipientId : Guid.Empty;
                            envelopeAdditionalUploadInfo.RecipientEmailID = templateUploads.RecipientEmail;
                            envelopeAdditionalUploadInfo.CreatedDateTime = DateTime.Now;
                            envelopeAdditionalUploadInfo.ModifiedDateTime = DateTime.Now;
                            SaveEnvelopeAdditionalUploadInfo(envelopeAdditionalUploadInfo);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool DeleteEnvelopeAdditionalAttachment(Guid? EnvelopeID)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "Envelope Repository", "DeleteEnvelopeAdditionalAttachment", "Process is started for Delete Envelope Additional Attachment", EnvelopeID.ToString(), "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeAdditionalUploadInfoObjects = dbContext.EnvelopeAdditionalUploadInfo.Where(x => x.MasterEnvelopeID == EnvelopeID);
                    if (envelopeAdditionalUploadInfoObjects != null)
                    {
                        foreach (var envelopeAdditionalUploadInfoObject in envelopeAdditionalUploadInfoObjects)
                        {
                            dbContext.EnvelopeAdditionalUploadInfo.Remove(envelopeAdditionalUploadInfoObject);
                            dbContext.SaveChanges();
                        }
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<string> GetEnvelopeAdditionalDocumentName(Guid masterEnvelopeID)
        {
            List<string> lstFiles = new List<string>();
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    lstFiles = dbContext.EnvelopeAdditionalUploadInfo.Where(x => x.MasterEnvelopeID == masterEnvelopeID && x.FileName != null).Select(f => f.FileName).ToList();
                    return lstFiles;
                }
            }
            catch (Exception ex)
            {
                return lstFiles;
            }
        }
        public bool UpdateEnvelopeAdditionalUploadInfoByInviteByEmailUserID(int ID, Guid RecipientID)
        {
            EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    envelopeAdditionalUploadInfo = dbContext.EnvelopeAdditionalUploadInfo.FirstOrDefault(u => u.ID == ID && u.RecipientID == RecipientID);
                    if (envelopeAdditionalUploadInfo != null)
                    {
                        envelopeAdditionalUploadInfo.FileName = null;
                        envelopeAdditionalUploadInfo.OriginalFileName = null;
                        envelopeAdditionalUploadInfo.ModifiedDateTime = DateTime.Now;
                        dbContext.Entry(envelopeAdditionalUploadInfo).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<DocumentContentDetails> getDocumentContentDetails(List<DocumentContents> documentContents)
        {
            List<DocumentContentDetails> documentContentDetails = new List<DocumentContentDetails>();
            if (documentContents != null)
            {
                foreach (var documentContent in documentContents)
                {
                    if (documentContent.IsControlDeleted)
                        continue;

                    DocumentContentDetails newDocContent = new DocumentContentDetails();
                    newDocContent.ID = documentContent.ID;
                    newDocContent.DocumentID = documentContent.DocumentID;
                    newDocContent.Label = documentContent.Label;
                    newDocContent.ControlID = documentContent.ControlID;
                    if (documentContent.ControlName == "DateTimeStamp")
                        newDocContent.ControlID = Constants.Control.DateTimeStamp;
                    newDocContent.RecipientID = documentContent.RecipientID;
                    newDocContent.ControlHtmlID = documentContent.ControlHtmlID;
                    newDocContent.Required = documentContent.Required;
                    newDocContent.DocumentPageNo = documentContent.DocumentPageNo;
                    newDocContent.PageNo = documentContent.PageNo;
                    newDocContent.XCoordinate = documentContent.XCoordinate;
                    newDocContent.YCoordinate = documentContent.YCoordinate;
                    newDocContent.ZCoordinate = documentContent.ZCoordinate;
                    newDocContent.ControlValue = documentContent.ControlValue;
                    newDocContent.Height = documentContent.Height;
                    newDocContent.Width = documentContent.Width;
                    newDocContent.GroupName = documentContent.GroupName;
                    newDocContent.ControlHtmlData = documentContent.ControHtmlData;
                    newDocContent.RecipientName = documentContent.RecipientName;
                    newDocContent.IsDefaultRequired = documentContent.IsDefaultRequired;
                    newDocContent.CustomToolTip = documentContent.CustomToolTip;
                    newDocContent.FontTypeMeasurement = documentContent.FontTypeMeasurement;
                    newDocContent.IsFixedWidth = documentContent.IsFixedWidth == null ? true : Convert.ToBoolean(documentContent.IsFixedWidth) ? true : false;
                    if (documentContent.ControlStyle != null)
                    {
                        newDocContent.controlStyleDetails = new List<ControlStyleDetails>();
                        ControlStyleDetails controlStyle = new ControlStyleDetails();
                        controlStyle.FontColor = documentContent.ControlStyle.FontColor;
                        controlStyle.FontID = documentContent.ControlStyle.FontID;
                        controlStyle.FontName = GetFontName(documentContent.ControlStyle.FontID);
                        controlStyle.FontSize = documentContent.ControlStyle.FontSize;
                        controlStyle.IsBold = documentContent.ControlStyle.IsBold;
                        controlStyle.IsItalic = documentContent.ControlStyle.IsItalic;
                        controlStyle.IsUnderline = documentContent.ControlStyle.IsUnderline;
                        newDocContent.ControlStyle = controlStyle;
                    }

                    if (documentContent.SelectControlOptions != null && documentContent.SelectControlOptions.Count != 0)
                    {
                        List<SelectControlOptionDetails> selectControlOptionDetails = new List<SelectControlOptionDetails>();
                        List<SelectControlOptionDetails> SelectControlOptionsEnt = new List<SelectControlOptionDetails>();

                        foreach (var opt in documentContent.SelectControlOptions)
                        {
                            SelectControlOptionDetails selectControlOptions = new SelectControlOptionDetails();
                            selectControlOptions.DocumentContentID = opt.DocumentContentID;
                            selectControlOptions.ID = opt.ID;
                            selectControlOptions.OptionText = opt.OptionText;
                            selectControlOptions.Order = opt.Order;
                            selectControlOptionDetails.Add(selectControlOptions);
                            SelectControlOptionsEnt.Add(selectControlOptions);
                        }
                        newDocContent.SelectControlOptions = SelectControlOptionsEnt;
                    }
                    documentContentDetails.Add(newDocContent);
                }
            }
            return documentContentDetails;
        }
        public static string GetFontName(Guid FontID)
        {
            if (FontID == new Guid("1AB25FA7-A294-405E-A04A-3B731AD795AC"))
                return "Arial";
            else if (FontID == new Guid("1875C58D-52BD-498A-BE6D-433A8858357E"))
                return "Cambria";
            else if (FontID == new Guid("D4A45ECD-3865-448A-92FA-929C2295EA34"))
                return "Courier";
            else if (FontID == new Guid("956D8FD3-BB0F-4E30-8E55-D860DEABB346"))
                return "Times New Roman";

            return string.Empty;

        }
        public bool UpdateDocumentRequestRecipient(Guid oldRecipientId, Guid newRecipientId, Guid EnvelopeID, string newEmailId)
        {
            loggerModelNew = new LoggerModelNew("", "Envelope Repository", "UpdateDocumentRequestRecipient", "Process is started for Update Document Request Recipient", EnvelopeID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeAdditionalUploadInfoObjects = dbContext.EnvelopeAdditionalUploadInfo.Where(x => x.MasterEnvelopeID == EnvelopeID && x.RecipientID == oldRecipientId);
                    if (envelopeAdditionalUploadInfoObjects != null)
                    {
                        foreach (var adln in envelopeAdditionalUploadInfoObjects)
                        {
                            adln.RecipientID = newRecipientId;
                            adln.RecipientEmailID = newEmailId;
                            dbContext.Entry(adln).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        }
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool SaveEnvelopeTemplateMapping(EnvelopeTemplateMappingDetails mappingDetails)
        {
            var loggerModelNew = new LoggerModelNew("", "EnvelopeRepository", "SaveEnvelopeTemplateMapping", "To store selected Template/Rule Reference For Envelope", "", "", "", "", "EnvelopeTemplateMapping");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    foreach (var det in mappingDetails.envelopeMappingsToTemp)
                    {
                        EnvelopeTemplateMapping doc = new EnvelopeTemplateMapping();
                        doc.EnvelopeId = mappingDetails.EnvelopeId;
                        doc.TemplateId = det.TemplateId;
                        doc.TemplateName = det.TemplateName;
                        doc.TemplateCode = det.TemplateCode;
                        doc.UserId = mappingDetails.UserId;
                        doc.CreatedDate = DateTime.Now;
                        doc.EnvelopeTypeId = mappingDetails.EnvelopeTypeId;
                        dbContext.EnvelopeTemplateMapping.Add(doc);
                    }
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                throw;
            }
        }
        public bool SaveEnvelopeSettingsDetail(AdminGeneralAndSystemSettings settings, Guid envelopeId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeObject = dbContext.EnvelopeSettingsDetail.Where(e => e.EnvelopeId == envelopeId).FirstOrDefault();
                    Guid? userId = settings.UserID == null ? Guid.Empty : settings.UserID;

                    List<CompanySeletedSettings> companySettings = GetCompanySettings((Guid)userId);
                    if (envelopeObject == null)
                    {
                        EnvelopeSettingsDetail envelope = new EnvelopeSettingsDetail();
                        envelope.EnvelopeId = envelopeId;
                        envelope.DeleteOriginalDocument = settings.DeleteOriginalDocument;
                        envelope.FinalContractOptions = settings.FinalContractOptionID;
                        envelope.AttachSignedPdf = settings.AttachSignedPdfID;
                        envelope.IsDeleteSignedContracts = settings.IsDeleteSignedContracts;
                        envelope.ReferenceCode = settings.ReferenceCodeSettingID;
                        envelope.StoreEmailBody = settings.StoreEmailBody;
                        envelope.AllowUserToDelete = settings.AllowUserToDeleteEmailBody;
                        envelope.ReceiveSendingConfirmation = settings.ReceiveSendingEmailConfirmation;
                        envelope.DocumentsCombinedInOneEnvelope = settings.SignMultipleDocumentIndependently;
                        envelope.SendInvitationEmailToSigner = settings.SendInvitationEmailToSignerID;
                        envelope.CertificatePaperSize = settings.DocumentPaperSizeID;
                        envelope.SignReqReplyAdrs = settings.SignReqReplyAddSettingsID;
                        envelope.AllowAlignField = settings.IsFormFieldAlignmentEnabled;
                        envelope.SignatureControlRequired = settings.SignatureControlRequired;
                        envelope.SignatureCaptureType = settings.SignatureCaptureType;
                        envelope.SignatureCaptureHandDrawn = settings.SignatureCaptureHanddrawn;
                        envelope.UploadSignature = settings.UploadSignature;
                        envelope.TimeZone = settings.SelectedTimeZone;
                        envelope.IsEnableDependencies = settings.EnableDependenciesFeature;
                        envelope.IsEnableClickToSign = settings.EnableClickToSign;
                        envelope.DigitalCertificate = Convert.ToInt32(companySettings.Where(b => b.SettingId == Constants.SettingsKeyConfig.DigitalCertificate).FirstOrDefault().SettingValue);
                        envelope.AppKey = !string.IsNullOrEmpty(settings.AppKey) ? settings.AppKey : companySettings.Where(b => b.SettingId == Constants.SettingsKeyConfig.AppKey).FirstOrDefault().SettingValue;
                        envelope.CreatedDateTime = DateTime.Now;
                        envelope.AppliedEpicEntityId = settings.AppliedEpicEntityId;
                        envelope.AppliedEpicUser = settings.AppliedEpicUser;
                        envelope.EntityType = settings.EntityType;
                        envelope.IntegrationType = settings.IntegrationType;
                        envelope.PostSendingNavigationPage = settings.PostSendingNavigationPage;
                        dbContext.EnvelopeSettingsDetail.Add(envelope);
                    }
                    else
                    {
                        envelopeObject.EnvelopeId = envelopeId;
                        envelopeObject.DeleteOriginalDocument = settings.DeleteOriginalDocument;
                        envelopeObject.FinalContractOptions = settings.FinalContractOptionID;
                        envelopeObject.AttachSignedPdf = settings.AttachSignedPdfID;
                        envelopeObject.IsDeleteSignedContracts = settings.IsDeleteSignedContracts;
                        envelopeObject.ReferenceCode = settings.ReferenceCodeSettingID;
                        envelopeObject.StoreEmailBody = settings.StoreEmailBody;
                        envelopeObject.AllowUserToDelete = settings.AllowUserToDeleteEmailBody;
                        envelopeObject.ReceiveSendingConfirmation = settings.ReceiveSendingEmailConfirmation;
                        envelopeObject.DocumentsCombinedInOneEnvelope = settings.SignMultipleDocumentIndependently;
                        envelopeObject.SendInvitationEmailToSigner = settings.SendInvitationEmailToSignerID;
                        envelopeObject.CertificatePaperSize = settings.DocumentPaperSizeID;
                        envelopeObject.SignReqReplyAdrs = envelopeObject.SignReqReplyAdrs; // settings.SignReqReplyAddSettingsID; //Consider envelope settings
                        envelopeObject.AllowAlignField = settings.IsFormFieldAlignmentEnabled;
                        envelopeObject.SignatureControlRequired = settings.SignatureControlRequired;
                        envelopeObject.SignatureCaptureType = settings.SignatureCaptureType;
                        envelopeObject.SignatureCaptureHandDrawn = settings.SignatureCaptureHanddrawn;
                        envelopeObject.UploadSignature = settings.UploadSignature;
                        envelopeObject.TimeZone = settings.SelectedTimeZone;
                        envelopeObject.IsEnableDependencies = settings.EnableDependenciesFeature;
                        envelopeObject.IsEnableClickToSign = settings.EnableClickToSign;
                        envelopeObject.DigitalCertificate = Convert.ToInt32(companySettings.Where(b => b.SettingId == Constants.SettingsKeyConfig.DigitalCertificate).FirstOrDefault().SettingValue);
                        envelopeObject.AppKey = !string.IsNullOrEmpty(settings.AppKey) ? settings.AppKey : companySettings.Where(b => b.SettingId == Constants.SettingsKeyConfig.AppKey).FirstOrDefault().SettingValue;
                        envelopeObject.ModifiedDateTime = DateTime.Now;
                        dbContext.Entry(envelopeObject).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public List<CompanySeletedSettings> GetCompanySettings(Guid UserId)
        {
            List<CompanySeletedSettings> companySettings = new List<CompanySeletedSettings>();
            if (UserId != Guid.Empty)
            {
                UserProfile envelopeSender = _userRepository.GetUserProfileByUserID(UserId);
                if (envelopeSender != null && envelopeSender.CompanyID.HasValue && envelopeSender.CompanyID != Guid.Empty)
                {
                    var settingsDetails = _settingsRepository.GetEntityForByKeyConfig(envelopeSender.CompanyID.Value, Constants.SettingsKeyConfig.AppKey);
                    if (settingsDetails != null)
                    {
                        string AppKey = string.IsNullOrEmpty(settingsDetails.OptionValue) ? Constants.AppKey.None : settingsDetails.OptionValue;
                        companySettings.Add(new CompanySeletedSettings { SettingId = Constants.SettingsKeyConfig.AppKey, SettingValue = AppKey });
                    }
                    else
                    {
                        string AppKey = Constants.AppKey.None;
                        companySettings.Add(new CompanySeletedSettings { SettingId = Constants.SettingsKeyConfig.AppKey, SettingValue = AppKey });
                    }
                    var DigitalCertificate = _settingsRepository.GetEntityForByKeyConfig(envelopeSender.CompanyID.Value, Constants.SettingsKeyConfig.DigitalCertificate);
                    if (settingsDetails != null)
                    {
                        string digitalCertificate = string.IsNullOrEmpty(DigitalCertificate.OptionValue) ? Convert.ToString(Constants.DigitalCertificate.Default) : DigitalCertificate.OptionValue;
                        companySettings.Add(new CompanySeletedSettings { SettingId = Constants.SettingsKeyConfig.DigitalCertificate, SettingValue = digitalCertificate });
                    }
                    else
                    {
                        string digitalCertificate = Convert.ToString(Constants.DigitalCertificate.Default);
                        companySettings.Add(new CompanySeletedSettings { SettingId = Constants.SettingsKeyConfig.DigitalCertificate, SettingValue = digitalCertificate });
                    }
                }
            }
            else
            {
                companySettings.Add(new CompanySeletedSettings { SettingId = Constants.SettingsKeyConfig.AppKey, SettingValue = Constants.AppKey.None });
                companySettings.Add(new CompanySeletedSettings { SettingId = Constants.SettingsKeyConfig.DigitalCertificate, SettingValue = Convert.ToString(Constants.DigitalCertificate.Default) });
            }
            return companySettings;
        }
        public List<SignMultipleTemplateDetails> GetSigningInbox(Guid envelopeId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var dashboardDetails = (from x in dbContext.vw_SigningInbox
                                            where x.EnvelopeID == envelopeId
                                            orderby x.TemplateOrder
                                            select new SignMultipleTemplateDetails
                                            {
                                                EnvelopeTemplateGroupsID = x.EnvelopeTemplateGroupsID.Value,
                                                TemplateName = x.TemplateName,
                                                PrescriberFirstName = x.SenderName,
                                                PrescriberLastName = x.SenderName,
                                                PrescriberEmail = x.SenderEmail,
                                                Status = x.EnvelopeStatus,
                                                StatusID = x.StatusID,
                                                UpdatedStatusDate = x.CreatedDateTime,
                                                EnvelopeID = x.EnvelopeID,
                                                RecipientID = x.ReceipientID,
                                                CurrentRecipientId = x.ReceipientID,
                                                RecipientEmail = x.RecipientEmail,
                                                RecipientName = x.RoleName,
                                                Order = x.Order,
                                                IsSigned = x.SignerStatus == 1,
                                                IsFinished = x.IsFinished,
                                                Subject = x.Subject,
                                                Message = x.Message,
                                                DateFormatID = x.DateFormatID,
                                                DialCode = x.DialCode,                                               
                                                Mobile = x.Mobile
                                            }).ToList();
                    return dashboardDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnvelopeAdditionalUploadInfoDetails> GetEnvelopeAdditionalUploadInfoByEnvelope(Guid masterEnvelopeID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeAdditionalUploadInfoList = dbContext.EnvelopeAdditionalUploadInfo.Where(x => x.MasterEnvelopeID == masterEnvelopeID).Select(x => new EnvelopeAdditionalUploadInfoDetails
                    {
                        ID = x.ID,
                        MasterEnvelopeID = x.MasterEnvelopeID,
                        Name = x.Name,
                        Description = x.Description,
                        AdditionalInfo = x.AdditionalInfo,
                        FileName = x.FileName,
                        OriginalFileName = x.OriginalFileName,
                        IsActive = x.IsActive,
                        IsRequired = x.IsRequired,
                        UploadedDateTime = x.ModifiedDateTime,
                        RecipientID = x.RecipientID
                    }).ToList();

                    return envelopeAdditionalUploadInfoList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ConditionalControlMapping> GetConditionalControlMapping(Guid envelopeId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.ConditionalControlMapping.Where(c => c.EnvelopeId == envelopeId).ToList();
            }
        }
        public bool SaveEnvelopeFolderMapping(EnvelopeFolderMapping envelopeFolderMapping)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                try
                {
                    dbContext.EnvelopeFolderMapping.Add(envelopeFolderMapping);
                    dbContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    dbContext.EnvelopeFolderMapping.Add(envelopeFolderMapping);
                    dbContext.SaveChanges();
                    return true;
                }
            }
        }
        public EnvelopeGetEnvelopeHistoryByEnvelopeCode GetEnvelopeMetaDataWithHistory(Guid EnvelopeId, string EmailID, string userTimeZone, string userName)
        {
            IEnumerable<LookupItem> envelopeStatus = _lookupRepository.GetLookup(Lookup.EnvelopeStatus);
            //eSignHelper objesignHelper = new eSignHelper();
            //SettingsRepository settingsRepository = new SettingsRepository(dbContext);           

            loggerModelNew = new LoggerModelNew("", "Envelope Repository", "GetEnvelopeMetaDataWithHistory", "Initiate process to get envelope by envelope Id", EnvelopeId.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            using (var dbContext = new RSignDbContext(_configuration))
            {
                const int passwordKeySize = 128;
                string completeEncodedKey = ModelHelper.GenerateKey(passwordKeySize);
                var objEnvelope = _genericRepository.GetEntity(EnvelopeId);
                EnvelopeGetEnvelopeHistoryByEnvelopeCode mngEnvlop = new EnvelopeGetEnvelopeHistoryByEnvelopeCode();
                var dirPath = _modelHelper.GetEnvelopeDirectoryNew(objEnvelope.ID, string.Empty);
                string tempDirectorPath = dirPath;
                mngEnvlop.DocumentList = new List<DocumentDetails>();
                /*TimeZone as per user Settings*/
                string userTimeZoneForRecipients = string.Empty;

                var timeZone = dbContext.SettingsDetail.FirstOrDefault(f => f.SettingsFor == objEnvelope.UserID && f.KeyConfig == Constants.SettingsKeyConfig.TimeZone);

                if (timeZone != null)
                    userTimeZoneForRecipients = timeZone.OptionValue;
                //Documents
                var documentList = objEnvelope.Documents.OrderBy(o => o.Order).ThenBy(o => o.UploadedDateTime).ToList();

                foreach (var tdoc in documentList)
                    mngEnvlop.DocumentList.Add(new DocumentDetails
                    {
                        ID = tdoc.ID,
                        EnvelopeID = tdoc.EnvelopeID,
                        DocumentName = tdoc.DocumentName,
                        Order = tdoc.Order,
                        IsDocumentStored = tdoc.IsDocumentStored,
                        ActionType = tdoc.ActionType,
                        envelopeDocumentDeleteData = dbContext.EnvelopeDocumentDeleteData.FirstOrDefault(s => s.DocumentId == tdoc.ID)
                    });

                //Recipients history
                mngEnvlop.RecipientsDetail = new List<RecipientsDetailsAPI>();
                var recepientDetails = GetRecipientHistoryListForManage(objEnvelope.ID);

                //recepientDetails = recepientDetails.Select(a => { a.FormattedCreatedDateTime = String.Format("{0:MM/dd/yyyy HH:mm tt}", Convert.ToString(objesignHelper.GetLocalTime(a.CreatedDateTime, userTimeZoneForRecipients,objEnvelope.DateFormatID))); return a; }).ToList();
                recepientDetails = recepientDetails.Select(a => { a.FormattedCreatedDateTime = Convert.ToString(_esignHelper.GetLocalTime(a.CreatedDateTime, userTimeZoneForRecipients, objEnvelope.DateFormatID)); return a; }).ToList();
                //recepientDetails = recepientDetails.Select(a => { a.FormattedSentDate = String.Format("{0:MM/dd/yyyy HH:mm tt}", Convert.ToString(objesignHelper.GetLocalTime(objEnvelope.CreatedDateTime, userTimeZoneForRecipients,objEnvelope.DateFormatID))); return a; }).ToList();
                recepientDetails = recepientDetails.Select(a => { a.FormattedSentDate = Convert.ToString(_esignHelper.GetLocalTime(objEnvelope.CreatedDateTime, userTimeZoneForRecipients, objEnvelope.DateFormatID)); return a; }).ToList();
                foreach (var rec in objEnvelope.Recipients.Where(x => x.RecipientTypeID != Constants.RecipientType.Sender))
                {
                    if (rec.EmailDeliveryStatus != null && rec.EmailDeliveryStatus.HasValue)
                    {

                        if (recepientDetails.Where(r => r.RecipientID == rec.ID && r.EmailAddress == rec.EmailAddress).Count() > 0)
                            recepientDetails.Where(r => r.RecipientID == rec.ID && r.EmailAddress == rec.EmailAddress).OrderBy(a => a.CreatedDateTime).FirstOrDefault().EmailDeliveryStatusDecription = rec.EmailDeliveryStatus == 2 ? Constants.EmailDeliveryStatusDecription.OutOfOffice : Constants.EmailDeliveryStatusDecription.DeliveryFailed;
                    }
                }
                var recepientStatusDetails = new List<RecipientsDetailsAPI>();
                foreach (var recp in recepientDetails.OrderByDescending(r => r.CreatedDateTime))
                {
                    if (recepientStatusDetails.Where(s => s.RecipientID == recp.RecipientID).Count() == 0)
                    {
                        recepientStatusDetails.Add(recp);
                    }
                }

                var DeliveryModeOptions = new List<DropdownOptions>();
                if (Convert.ToBoolean(objEnvelope.EnableMessageToMobile))
                {
                    DeliveryModeOptions = GetDropdownOptionsList("DeliveryModeOptions", string.IsNullOrEmpty(objEnvelope.CultureInfo) ? "en-US" : objEnvelope.CultureInfo);
                    if (DeliveryModeOptions != null && DeliveryModeOptions.Count > 0)
                    {
                        foreach (var recp in recepientDetails)
                        {
                            if (recp.RecipientTypeID != Constants.RecipientType.Sender && recp.RecipientTypeID != Constants.RecipientType.Prefill && !string.IsNullOrEmpty(Convert.ToString(recp.DeliveryMode)))
                            {
                                string DeliveryModeOptionName = (DeliveryModeOptions.Where(d => d.OptionValue == Convert.ToString(recp.DeliveryMode)).FirstOrDefault()).OptionName;
                                recp.DeliveryModeText = DeliveryModeOptionName;
                            }
                        }
                    }
                }

                mngEnvlop.WaitingforConfirmationCount = recepientStatusDetails != null && recepientStatusDetails.Count > 0 ? recepientStatusDetails.Where(s => s.StatusTypeID == Constants.StatusCode.Signer.AwaitingConfirmation).Count() : 0;
                mngEnvlop.RecipientsDetail.AddRange(recepientDetails);
                mngEnvlop.PasswordReqdToOpen = objEnvelope.PasswordReqdtoOpen;
                mngEnvlop.PasswordToOpen = objEnvelope.PasswordReqdtoOpen && !string.IsNullOrEmpty(objEnvelope.PasswordtoOpen) ? ModelHelper.Decrypt(objEnvelope.PasswordtoOpen, objEnvelope.PasswordKey, (int)objEnvelope.PasswordKeySize) : null;
                mngEnvlop.PasswordReqdToSign = objEnvelope.PasswordReqdtoSign;
                mngEnvlop.PasswordToSign = objEnvelope.PasswordReqdtoSign && !string.IsNullOrEmpty(objEnvelope.PasswordtoSign) ? ModelHelper.Decrypt(objEnvelope.PasswordtoSign, objEnvelope.PasswordKey, (int)objEnvelope.PasswordKeySize) : null;
                mngEnvlop.EnvelopeCodeDisplay = !string.IsNullOrEmpty(objEnvelope.EDisplayCode) ? "ENV" + objEnvelope.EDisplayCode.ToString() : string.Empty;
                mngEnvlop.EnvelopeCode = objEnvelope.EDisplayCode;
                mngEnvlop.Subject = objEnvelope.Subject;
                mngEnvlop.Message = objEnvelope.Message;
                mngEnvlop.CurrentStatus = objEnvelope.StatusID != Guid.Empty ? envelopeStatus.Where(s => Guid.Parse(s.Key) == objEnvelope.StatusID).Single().Value : string.Empty;
                mngEnvlop.EnvelopeStatusDescription = mngEnvlop.CurrentStatus;
                mngEnvlop.Sender = userName;
                mngEnvlop.IsSequential = objEnvelope.IsSequenceCheck;
                mngEnvlop.StatusId = objEnvelope.StatusID;
                mngEnvlop.EnvelopeId = objEnvelope.ID;
                mngEnvlop.UserID = objEnvelope.UserID;
                mngEnvlop.ExpiryDate = objEnvelope.ExpiryDate;
                mngEnvlop.IsFinalDocLinkReq = objEnvelope.IsFinalDocLinkReq;
                mngEnvlop.IsTransparencyDocReq = objEnvelope.IsTransparencyDocReq;
                mngEnvlop.IsFinalContractDeleleted = objEnvelope.IsFinalContractDeleted;
                mngEnvlop.ReferenceCode = objEnvelope.ReferenceCode;
                mngEnvlop.ReferenceEmail = objEnvelope.ReferenceEmail;
                mngEnvlop.IsTemplateShared = objEnvelope.IsTemplateShared;
                mngEnvlop.Sent = !string.IsNullOrEmpty(userTimeZone) ? Convert.ToString(_esignHelper.GetLocalTime(objEnvelope.CreatedDateTime, userTimeZone, objEnvelope.DateFormatID)) : Convert.ToString(objEnvelope.CreatedDateTime);
                mngEnvlop.FormattedSentDate = !string.IsNullOrEmpty(userTimeZone) ? Convert.ToString(_esignHelper.GetLocalTime(objEnvelope.CreatedDateTime, userTimeZone, objEnvelope.DateFormatID)) : String.Format("{0:MM/dd/yyyy HH:mm tt}", objEnvelope.CreatedDateTime);
                mngEnvlop.AllowMultiSigners = Convert.ToBoolean(objEnvelope.AllowMultiSigner);
                string AccessAuthTypID = GetAccessAuthTypeId(objEnvelope);
                mngEnvlop.AccessAuthenticationType = _settingsRepository.GetSettingsValue(AccessAuthTypID, Lookup.MasterLanguageKeyDetails, string.Empty, 0, objEnvelope.CultureInfo);
                if (objEnvelope.StatusID != Constants.StatusCode.Envelope.Waiting_For_Signature)
                {
                    if (!string.IsNullOrEmpty(userTimeZone))
                        //mngEnvlop.Completed = String.Format("{0:MM/dd/yyyy HH:mm tt}", objesignHelper.GetLocalTime(objEnvelope.ModifiedDateTime, userTimeZone,objEnvelope.DateFormatID));
                        mngEnvlop.Completed = _esignHelper.GetLocalTime(objEnvelope.ModifiedDateTime, userTimeZone, objEnvelope.DateFormatID);
                    else
                        mngEnvlop.Completed = String.Format("{0:MM/dd/yyyy HH:mm tt}", objEnvelope.ModifiedDateTime);
                }
                //Recipients
                string senderName = "", SenderEmail = string.Empty, senderIpAddress = string.Empty, recipients = string.Empty;
                //Optimized foreach loop of active recipients view
                List<string> activeRecipient = recepientDetails.Where(a => a.ActiveRecipients == true).Select(s => string.IsNullOrEmpty(s.CurrentRecipient) ? s.Name : s.CurrentRecipient).ToList();
                List<string> allRecipients = new List<string>();
                foreach (var rec in objEnvelope.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Signer && x.IsSameRecipient != true).OrderBy(o => o.CreatedDateTime))
                {
                    var signerStatus = dbContext.SignerStatus.Where(s => s.RecipientID == rec.ID).OrderByDescending(s => s.CreatedDateTime).FirstOrDefault();
                    if (signerStatus == null || (signerStatus != null && signerStatus.StatusID != Constants.StatusCode.Recipients.Transferred))
                    {
                        string recipientNameWithMobile = rec.Name;
                        allRecipients.Add(recipientNameWithMobile);
                    }
                }
                List<string> signersEmailList = new List<string>();
                UserProfile usersInfo = new UserProfile();
                var CCSignerType = GetDropdownOptionsList("CCSignerType", string.IsNullOrEmpty(objEnvelope.CultureInfo) ? "en-US" : objEnvelope.CultureInfo);
                foreach (var recipient in objEnvelope.Recipients.Where(x => x.RecipientTypeID != Constants.RecipientType.Signer).OrderBy(o => o.RecipientTypeID))
                {
                    string recipientNameWithMobile = recipient.Name;
                    if (recipient.RecipientTypeID == Constants.RecipientType.Prefill)
                    {
                        if (recipient.Name.Contains(Constants.String.Notations.Prefill))
                        {
                            activeRecipient.Add(recipientNameWithMobile);
                            allRecipients.Add(recipientNameWithMobile);
                        }
                        else
                        {
                            activeRecipient.Add(recipient.Name + Constants.String.Notations.Prefill);
                            allRecipients.Add(recipient.Name + Constants.String.Notations.Prefill);
                        }
                    }
                    else if (recipient.RecipientTypeID == Constants.RecipientType.CC && string.IsNullOrEmpty(recipient.RerouteEmailAddress)) // CC
                    {
                        if (recipient.CCSignerType != null && objEnvelope.EnableCcOptions == true)
                        {
                            string CCSignerTypeName = (CCSignerType.Where(d => d.OptionValue == Convert.ToString(recipient.CCSignerType)).FirstOrDefault()).OptionName;
                            activeRecipient.Add(recipientNameWithMobile + " (Cc-" + CCSignerTypeName + ")");
                            allRecipients.Add(recipientNameWithMobile + " (Cc-" + CCSignerTypeName + ")");
                        }
                        else
                        {
                            activeRecipient.Add(recipientNameWithMobile + " (Cc)");
                            allRecipients.Add(recipientNameWithMobile + " (Cc)");
                        }

                    }
                    else if (recipient.RecipientTypeID == Constants.RecipientType.Sender) // Sender
                    {
                        senderName = recipient.Name;
                        SenderEmail = recipient.EmailAddress;
                        senderIpAddress = objEnvelope.IpAddress;
                        //recepientDetails.Where(rd => rd.StatusTypeID == Constants.StatusCode.Signer.Sent).Select(rd => rd.IPAddress).First();//recipient.RecipientsDetail.Select(r => r.IPAddress).FirstOrDefault();
                    }
                }
                foreach (var recipient in objEnvelope.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Signer && x.IsSameRecipient != true).OrderBy(o => o.RecipientTypeID))
                {
                    if (recipient.RecipientTypeID == Constants.RecipientType.Signer && !string.IsNullOrEmpty(recipient.CopyEmailAddress)) // CC
                    {
                        signersEmailList.Clear();

                        if (!string.IsNullOrEmpty(recipient.CopyEmailAddress))
                        {
                            signersEmailList.AddRange(recipient.CopyEmailAddress.Split(','));
                        }

                        foreach (string signerEmail in signersEmailList)
                        {
                            usersInfo = dbContext.UserProfile.Where(s => s.EmailID == signerEmail && s.IsActive == true).FirstOrDefault();

                            if (usersInfo != null)
                            {
                                activeRecipient.Add(usersInfo.FirstName + " " + usersInfo.LastName);
                                allRecipients.Add(usersInfo.FirstName + " " + usersInfo.LastName);
                            }
                            else
                            {
                                activeRecipient.Add(!string.IsNullOrEmpty(signerEmail) ? signerEmail.Split('@')[0] : string.Empty);
                                allRecipients.Add(!string.IsNullOrEmpty(signerEmail) ? signerEmail.Split('@')[0] : string.Empty);
                            }

                        }

                    }

                }
                recipients = string.Join(", ", activeRecipient.ToArray());
                //mngEnvlop.recipientIdForWaitingForSignature = recepientDetails.Where(a => a.ActiveRecipients == true && a.StatusTypeID == Constants.StatusCode.Signer.Pending && (string.IsNullOrEmpty(a.CurrentEmail) ? a.EmailAddress : a.CurrentEmail) == EmailID).Select(s => s.RecipientID).First();
                if (recepientDetails.Any(a => a.StatusTypeID == Constants.StatusCode.Signer.Signed || a.StatusTypeID == Constants.StatusCode.Signer.Viewed
                || a.StatusTypeID == Constants.StatusCode.Signer.Finish_Later_Selected))
                    mngEnvlop.IsEnvelopeEditableAfterSend = false;
                else
                    mngEnvlop.IsEnvelopeEditableAfterSend = true;
                mngEnvlop.IsEnvelopeUpdated = (objEnvelope.IsEnvelope == true && objEnvelope.IsStatic != true && objEnvelope.TemplateKey != null) ? true : false;
                mngEnvlop.RecipientNameList = recipients;
                mngEnvlop.AllRecipientNameList = string.Join(", ", allRecipients.ToArray());
                mngEnvlop.Sender = senderName;
                mngEnvlop.senderEmail = SenderEmail;
                mngEnvlop.IPAddressSender = senderIpAddress;
                mngEnvlop.IsDiclaimerAccepted = objEnvelope.IsDisclaimerAccepted;
                if (mngEnvlop.IsTemplateShared)
                {
                    mngEnvlop.IsZipExist = (objEnvelope.StatusID == Constants.StatusCode.Envelope.Completed && Convert.ToBoolean(objEnvelope.IsFinalDocLinkReq) && !objEnvelope.IsFinalContractDeleted);
                    if (recepientDetails.Where(a => a.ActiveRecipients == true && a.RecipientTypeID == Constants.RecipientType.Signer && a.StatusTypeID != Constants.StatusCode.Signer.Signed).Count() == 0)
                        mngEnvlop.IsResendDisabled = true;
                }
                else
                    mngEnvlop.IsZipExist = (objEnvelope.StatusID == Constants.StatusCode.Envelope.Completed && objEnvelope.IsSignerAttachFileReq > 0 && Convert.ToBoolean(objEnvelope.IsFinalDocLinkReq) && !objEnvelope.IsFinalContractDeleted);
                mngEnvlop.ZipPath = (mngEnvlop.IsZipExist && File.Exists(tempDirectorPath + objEnvelope.ID + "\\SignerAttachments\\SignerDoc.zip")) ? (tempDirectorPath + objEnvelope.ID + "\\SignerAttachments\\SignerDoc.zip") : string.Empty;
                mngEnvlop.IsEnvelopeStatic = Convert.ToBoolean(objEnvelope.IsStatic);
                mngEnvlop.EnvelopeEmailBody = objEnvelope.Message;
                mngEnvlop.IsEmailBodyDisplay = objEnvelope.IsEmailBodyDisplay;
                mngEnvlop.IsEnvelopeEdited = objEnvelope.IsEdited;
                mngEnvlop.IsEnvelopeHistory = Convert.ToBoolean(objEnvelope.IsEnvelopeHistory);
                mngEnvlop.DateFormatID = objEnvelope.DateFormatID;
                mngEnvlop.IsSignatureCertificateStored = objEnvelope.IsSignatureCertificateStored;
                mngEnvlop.IsStoreOriginalDocument = objEnvelope.IsStoreOriginalDocument;
                mngEnvlop.IsStoreSignatureCertificate = objEnvelope.IsStoreSignatureCertificate;
                mngEnvlop.IsPrivateMode = objEnvelope.IsPrivateMode;
                mngEnvlop.TemplateGroupId = objEnvelope.TemplateGroupId;
                //Added by TParker- GDPR Should Not be Visible When Searched with User Email in Company Settings Tab
                var companyId = dbContext.UserProfile.Where(s => s.UserID == objEnvelope.UserID).Select(s => s.CompanyID).FirstOrDefault();
                var companyPrivacySettingDetails = dbContext.SettingsDetail.FirstOrDefault(f => f.SettingsFor == companyId && f.KeyConfig == Constants.SettingsKeyConfig.PrivateMode);
                if (companyPrivacySettingDetails != null)
                {
                    var companyPrivateModeSetting = JsonConvert.DeserializeObject<List<PrivateModeSettings>>(companyPrivacySettingDetails.OptionValue);
                    if (companyPrivateModeSetting != null)
                    {
                        mngEnvlop.IsDataMasking = Convert.ToBoolean(companyPrivateModeSetting.Where(a => a.Name == "DataMasking").Select(b => b.Values).Select(c => c.OptionValue).FirstOrDefault());
                        DeletedEnvelopeFolderHistory deletedEnvelopeFolderHistory = dbContext.DeletedEnvelopeFolderHistory.Where(a => a.EnvelopeId == EnvelopeId).FirstOrDefault();
                        mngEnvlop.IsDataDeleted = deletedEnvelopeFolderHistory != null;
                        if (mngEnvlop.IsDataDeleted)
                        {
                            mngEnvlop.IsDataMasking = true;
                            mngEnvlop.RetentionDays = deletedEnvelopeFolderHistory.RetentionDays;
                        }
                    }

                }
                //End Optimized foreach loop of active recipients view
                var EnvelopeTemplatemapp = (from t in dbContext.EnvelopeTemplateMapping where t.EnvelopeId == objEnvelope.ID && t.TemplateName != null select t).ToList();
                if (EnvelopeTemplatemapp != null && EnvelopeTemplatemapp.Count > 0)
                {

                    mngEnvlop.EnvelopeRuleList = new List<EnvelopeTemplateDetails>();
                    mngEnvlop.EnvelopeTemplateList = new List<EnvelopeTemplateDetails>();
                    foreach (var temp in EnvelopeTemplatemapp)
                    {
                        if (Convert.ToString(temp.EnvelopeTypeId).ToUpper() == Convert.ToString(Constants.EnvelopeType.TemplateRule).ToUpper())
                        {
                            mngEnvlop.EnvelopeRuleList.Add(new EnvelopeTemplateDetails
                            {
                                TemplateName = temp.TemplateName,
                                TemplateCode = Convert.ToString(temp.TemplateCode),
                                EnvelopeTypeId = Convert.ToString(temp.EnvelopeTypeId)

                            });
                        }
                        else
                        {
                            mngEnvlop.EnvelopeTemplateList.Add(new EnvelopeTemplateDetails
                            {
                                TemplateName = temp.TemplateName,
                                TemplateCode = Convert.ToString(temp.TemplateCode),
                                EnvelopeTypeId = Convert.ToString(temp.EnvelopeTypeId)
                            });
                        }
                    }
                }


                if (objEnvelope.CopyEnvelopeId != null)
                {
                    if (dbContext.Envelope.Where(e => e.ID == objEnvelope.CopyEnvelopeId).ToList().Count() > 0)
                    {
                        mngEnvlop.CopyEnvelopeCode = Convert.ToString(dbContext.Envelope.Where(u => u.ID == objEnvelope.CopyEnvelopeId).FirstOrDefault().EDisplayCode);
                    }
                    else if (dbContext.EnvelopeHistory.Where(e => e.ID == objEnvelope.CopyEnvelopeId).ToList().Count() > 0)
                    {
                        mngEnvlop.CopyEnvelopeCode = Convert.ToString(dbContext.EnvelopeHistory.Where(u => u.ID == objEnvelope.CopyEnvelopeId).FirstOrDefault().EDisplayCode);
                    }
                    else
                    {
                        ArichiveEnvelopesInfo envelope = _genericRepository.GetArchivedEnvelope(string.Empty, objEnvelope.CopyEnvelopeId.Value);
                        if (envelope != null)
                        {
                            mngEnvlop.CopyEnvelopeCode = envelope.EdisplayCode;
                        }
                    }
                }

                mngEnvlop.AwaitingRecipientsList = new List<AwaitingRecipients>();
                int updateAndResend = mngEnvlop.RecipientsDetail.Where(x => x.StatusTypeID == Constants.StatusCode.Signer.Update_And_Resend).Count();
                if (updateAndResend > 0 && (objEnvelope.IsSequenceCheck || objEnvelope.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Signer && x.Order > 1).Count() > 0))
                {
                    var rec = new List<RecipientsDetailsAPI>();
                    int i = 0;
                    foreach (var r in mngEnvlop.RecipientsDetail)
                    {
                        if (i == updateAndResend)
                        {
                            rec.Add(r);
                        }
                        if (r.Description == "Update and Resend")
                        {
                            i++;
                        }
                    }

                    foreach (var recipient in objEnvelope.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Signer).OrderBy(x => x.Order))
                    {
                        if (rec.Where(x => x.RecipientID == recipient.ID).Count() == 0)
                        {
                            mngEnvlop.AwaitingRecipientsList.Add(new AwaitingRecipients
                            {
                                Order = recipient.Order,
                                Name = recipient.Name,
                                RecipientEmail = Convert.ToString(recipient.EmailAddress),
                                DialCode = recipient.DialCode,
                                CountryCode = recipient.CountryCode,
                                Mobile = recipient.Mobile,
                                DeliveryMode = recipient.DeliveryMode,
                                DeliveryModeText = (Convert.ToBoolean(objEnvelope.EnableMessageToMobile) && DeliveryModeOptions != null && DeliveryModeOptions.Count > 0) ? (DeliveryModeOptions.Where(d => d.OptionValue == Convert.ToString(recipient.DeliveryMode)).FirstOrDefault()).OptionName : string.Empty
                            });
                        }


                    }
                }
                else if (objEnvelope.IsSequenceCheck || objEnvelope.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Signer && x.Order > 1).Count() > 0)
                {
                    foreach (var recipient in objEnvelope.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Signer).OrderBy(x => x.Order))
                    {
                        if (mngEnvlop.RecipientsDetail.Where(x => x.RecipientID == recipient.ID).Count() == 0)
                        {
                            mngEnvlop.AwaitingRecipientsList.Add(new AwaitingRecipients
                            {
                                Order = recipient.Order,
                                Name = recipient.Name,
                                RecipientEmail = Convert.ToString(recipient.EmailAddress),
                                DialCode = recipient.DialCode,
                                CountryCode = recipient.CountryCode,
                                Mobile = recipient.Mobile,
                                DeliveryMode = recipient.DeliveryMode,
                                DeliveryModeText = (Convert.ToBoolean(objEnvelope.EnableMessageToMobile) && DeliveryModeOptions != null && DeliveryModeOptions.Count > 0) ? (DeliveryModeOptions.Where(d => d.OptionValue == Convert.ToString(recipient.DeliveryMode)).FirstOrDefault()).OptionName : string.Empty
                            });
                        }
                    }
                }

                if (objEnvelope.IsSequenceCheck)
                {
                    mngEnvlop.SignInSequenceDescription = Convert.ToString(Constants.SignInSequenceDesc.Auto);
                }
                else
                {
                    if (objEnvelope.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Signer && x.Order > 1).Count() > 0)
                    {
                        mngEnvlop.SignInSequenceDescription = Convert.ToString(Constants.SignInSequenceDesc.Manual);
                    }
                }

                var settingsDownloadFinalContractDetails = dbContext.SettingsDetail.FirstOrDefault(f => f.SettingsFor == objEnvelope.UserID && f.KeyConfig == Constants.SettingsKeyConfig.IsAllowSignerstoDownloadFinalContract);
                if (settingsDownloadFinalContractDetails != null)
                    mngEnvlop.IsAllowSignerstoDownloadFinalContract = Convert.ToBoolean(settingsDownloadFinalContractDetails.OptionValue);
                return mngEnvlop;
            }
        }
        public List<RecipientsDetailsAPI> GetRecipientHistoryListForManage(Guid EnvelopeID)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "Envelope Repository", "GetRecipientHistoryListForManage", "Process is started for Get Recipient History List For Manage", EnvelopeID.ToString(), "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    List<RecipientsDetailsAPI> recipients = dbContext.Database.SqlQueryRaw<RecipientsDetailsAPI>("EXEC GetRecipientHistoryListForManage @EnvelopeID", new SqlParameter("EnvelopeID", EnvelopeID)).ToList();
                    return recipients;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string GetAccessAuthTypeId(Envelope objEnvelope)
        {
            string AccessAuthTypeId = objEnvelope.AccessAuthType != null ? Convert.ToString(objEnvelope.AccessAuthType) : Convert.ToString(Constants.ConfigurationalProperties.PasswordType.Select);
            if (Convert.ToBoolean(objEnvelope.IsSignerIdentitiy))
            {
                AccessAuthTypeId = Convert.ToString(Constants.ConfigurationalProperties.PasswordType.SignerIdentity);
            }
            if (objEnvelope.PasswordReqdtoSign && objEnvelope.PasswordReqdtoOpen)
            {
                AccessAuthTypeId = Convert.ToString(Constants.ConfigurationalProperties.PasswordType.Endtoend);
            }
            else if (!objEnvelope.PasswordReqdtoSign && objEnvelope.PasswordReqdtoOpen)
            {
                AccessAuthTypeId = Convert.ToString(Constants.ConfigurationalProperties.PasswordType.RequiredToOpenSigned);
            }
            return AccessAuthTypeId;
        }
        public List<DropdownOptions> GetDropdownOptionsList(string Type, string LanguageCode)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return (from e in dbContext.DropdownOptions
                        join r in dbContext.Language on e.LanguageID equals r.ID
                        where e.FieldName == Type && r.LanguageCode == LanguageCode
                        select e).ToList();
            }
        }
        //public ArichiveEnvelopesInfo GetArchivedEnvelope(string EDisplayCode, Guid EnvelopeId, Guid RecipientId = default(Guid), string securityCode = "")
        //{
        //    try
        //    {
        //        loggerModelNew = new LoggerModelNew("", "Envelope Repository", "GetArchivedEnvelope", "Get Envelope data from Archival process is started ", EDisplayCode, "", "", "", "API");
        //        rsignlog.RSignLogInfo(loggerModelNew);
        //        ArichiveEnvelopesInfo envelopesInfo = new ArichiveEnvelopesInfo();
        //        EDisplayCode = !string.IsNullOrEmpty(EDisplayCode) ? EDisplayCode : string.Empty;
        //        SqlParameter[] parms = new SqlParameter[3];
        //        parms[0] = new SqlParameter("@EDisplayCode", EDisplayCode);
        //        parms[1] = new SqlParameter("@EnvelopeId", EnvelopeId);
        //        parms[2] = new SqlParameter("@SecurityCode", securityCode);

        //        using (var dbContext = new RSignDbContext(_configuration))
        //        {
        //            ArichiveEnvelopesInfo envelope = dbContext.Database.SqlQueryRaw<ArichiveEnvelopesInfo>("EXEC USP_GetEnvelopeFromArchiveDb @EDisplayCode,@EnvelopeId,@SecurityCode", parms).FirstOrDefault();
        //            if (envelope != null)
        //            {
        //                if (EnvelopeId == Guid.Empty)
        //                    EnvelopeId = envelope.EnvelopeId;

        //                var userPlanDetails = GetUserPlanByUserID(envelope.UserID);

        //                if (userPlanDetails != null && userPlanDetails.IsDefaultPlan == true)
        //                {
        //                    string recipientCultureInfoFromArchive = string.Empty; string envelopecultureInfo = string.Empty;
        //                    List<ArichiveDBCultureInfo> cultureInfoFromArchiveDB = new List<ArichiveDBCultureInfo>();
        //                    cultureInfoFromArchiveDB = GetCultureInfoFromArchiveDB(EDisplayCode, EnvelopeId);
        //                    if (cultureInfoFromArchiveDB != null && cultureInfoFromArchiveDB.Count > 0 && RecipientId != Guid.Empty)
        //                    {
        //                        recipientCultureInfoFromArchive = cultureInfoFromArchiveDB.Where(r => r.RecipientId == RecipientId).Select(r => r.RecipientsCultureInfo).FirstOrDefault();
        //                    }
        //                    if (cultureInfoFromArchiveDB != null && cultureInfoFromArchiveDB.Count > 0)
        //                    {
        //                        envelopecultureInfo = cultureInfoFromArchiveDB.FirstOrDefault().EnvelopeCultureInfo;
        //                    }

        //                    envelopecultureInfo = !string.IsNullOrEmpty(recipientCultureInfoFromArchive) ? recipientCultureInfoFromArchive : envelopecultureInfo;
        //                    string ArchivedEnvelopeMessage = _genericRepository.GetUniqueKey("ArchivalMessageSender", envelopecultureInfo);

        //                    envelopesInfo.ArchivedEnvelopeMessage = ArchivedEnvelopeMessage;
        //                    envelopesInfo.IsEnvelopePurging = true;
        //                    envelopesInfo.IsFolderArchived = envelope.IsFolderArchived;
        //                    envelopesInfo.EdisplayCode = envelopesInfo.EdisplayCode;
        //                    envelopesInfo.UserID = envelope.UserID;
        //                    envelopesInfo.envelope = null;
        //                }
        //                else if (userPlanDetails != null && userPlanDetails.IsPaidPlan == true)
        //                {
        //                    envelopesInfo.envelope = _genericRepository.GetEntity(EnvelopeId, true, 1);
        //                    envelopesInfo.ArchivedEnvelopeMessage = string.Empty;
        //                    envelopesInfo.IsEnvelopePurging = false;
        //                    envelopesInfo.IsFolderArchived = envelope.IsFolderArchived;
        //                    envelopesInfo.EdisplayCode = envelopesInfo.EdisplayCode;
        //                    envelopesInfo.UserID = envelope.UserID;
        //                }
        //            }

        //            return envelopesInfo;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        loggerModelNew.Message = e.ToString();
        //        rsignlog.RSignLogError(loggerModelNew, ex);
        //        return null;
        //    }
        //}
        public EnvelopeSettingsDetail GetArchiveEnvelopeSettingsDetail(Guid EnvelopeId)
        {
            string currentMethod = "GetEnvelopeMetaDataWithHistory";

            loggerModelNew = new LoggerModelNew("", "GetArchiveEnvelopeSettingsDetail", currentMethod, "Initiate process to get envelope data from archive db", Convert.ToString(EnvelopeId), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            EnvelopeSettingsDetail mngEnvlop = new EnvelopeSettingsDetail();
            using (var dbContext = new RSignDbContext(_configuration))
            {
                SqlConnection connection = new SqlConnection(Convert.ToString(_configuration.Value.ConnectionStrings.RSignContext));
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = Convert.ToString("USP_GetArchiveEnvelopeSettingsDetail");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@EnvelopeId", SqlDbType.UniqueIdentifier).Value = EnvelopeId;
                cmd.Connection = connection;
                cmd.CommandTimeout = 3600;
                connection.Open();

                reader = cmd.ExecuteReader();
                List<EnvelopeSettingsDetail> _listArichveEnvelope = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<EnvelopeSettingsDetail>(reader).ToList();
                reader.NextResult();
                mngEnvlop = _listArichveEnvelope[0];
                connection.Close();

                return mngEnvlop;
            }
        }

        public List<ArichiveDBCultureInfo> GetCultureInfoFromArchiveDB(string EDisplayCode, Guid EnvelopeId)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "Envelope Repository", "GetCultureInfoFromArchiveDB", "Get Culture Info of Envelope and Recipients from Archival Database ", EDisplayCode, "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);

                EDisplayCode = !string.IsNullOrEmpty(EDisplayCode) ? EDisplayCode : string.Empty;
                SqlParameter[] parms = new SqlParameter[2];
                parms[0] = new SqlParameter("@EDisplayCode", EDisplayCode);
                parms[1] = new SqlParameter("@EnvelopeId", EnvelopeId);
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    List<ArichiveDBCultureInfo> envelope = dbContext.Database.SqlQueryRaw<ArichiveDBCultureInfo>("EXEC usp_GetCultureInfoFromArichiveDb @EDisplayCode,@EnvelopeId", parms).ToList();
                    return envelope;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return null;
            }
        }

    }
}
