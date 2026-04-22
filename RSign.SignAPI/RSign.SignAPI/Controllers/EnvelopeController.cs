using Aspose.Pdf.Operators;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Models;
using RSign.Models.Repository;
using RSign.Notification;
using System.Net;
using System.Text;
using System.Xml.Linq;


namespace RSign.SignAPI.Controllers
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class EnvelopeController : ControllerBase
    {
        RSignLogger rSignLogger = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration;
        private IHttpContextAccessor _accessor;

        private readonly ICompanyRepository _companyRepository;
        private readonly IDocumentContentsRepository _documentContentsRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IMasterDataRepository _masterDataRepository;
        private readonly IRecipientRepository _recipientRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private readonly IModelHelper _modelHelper;
        private readonly IAsposeHelper _asposeHelper;
        private readonly IESignHelper _eSignHelper;
        private readonly IGenericRepository _genericRepository;
        private readonly IConditionalControlRepository _conditionalControlRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IApiHelper _apiHelper;
        public EnvelopeController(IHttpContextAccessor accessor, IConfiguration appConfiguration, ICompanyRepository companyRepository, IDocumentContentsRepository documentContentsRepository,
            IEnvelopeRepository envelopeRepository, ILookupRepository lookupRepository, IMasterDataRepository masterDataRepository, IApiHelper apiHelper,
            IRecipientRepository recipientRepository, ISettingsRepository settingsRepository, ITemplateRepository templateRepository, IUserRepository userRepository,
            IEnvelopeHelperMain envelopeHelperMain, IModelHelper modelHelper, IAsposeHelper asposeHelper, IDocumentRepository documentRepository,
        IESignHelper eSignHelper, IGenericRepository genericRepository, IConditionalControlRepository conditionalControlRepository, IUserTokenRepository userTokenRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _companyRepository = companyRepository;
            _documentContentsRepository = documentContentsRepository;
            _envelopeRepository = envelopeRepository;
            _lookupRepository = lookupRepository;
            _masterDataRepository = masterDataRepository;
            _recipientRepository = recipientRepository;
            _settingsRepository = settingsRepository;
            _templateRepository = templateRepository;
            _userRepository = userRepository;
            _envelopeHelperMain = envelopeHelperMain;
            _modelHelper = modelHelper;
            _asposeHelper = asposeHelper;
            _eSignHelper = eSignHelper;
            _genericRepository = genericRepository;
            _conditionalControlRepository = conditionalControlRepository;
            _userTokenRepository = userTokenRepository;
            _apiHelper = apiHelper;
            _documentRepository = documentRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
        }

        [ProducesResponseType(typeof(HttpResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetImages/{id}/{envelopeId}/{uncPath}")]
        [HttpGet]
        public async Task<IActionResult> GetImages(string id, string envelopeID, string uncPath)
        {
            loggerModelNew = new LoggerModelNew("", "EnvelopeController", "GetImages", "Process started for Get Image path by envelopeId using API", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                // byte[] imgData = System.IO.File.ReadAllBytes(_envelopeHelperMain.GetImagePath(Convert.ToInt32(id), new Guid(envelopeID)).ToString());
                string uncPathName = _modelHelper.GetEnvelopeDirectoryByName(uncPath);
                byte[] imgData = uncPathName != "0" ? System.IO.File.ReadAllBytes(_envelopeHelperMain.GetEnvelopeImagePath(Convert.ToInt32(id), new Guid(envelopeID), uncPathName).ToString()) : System.IO.File.ReadAllBytes(_envelopeHelperMain.GetImagePathNew(Convert.ToInt32(id), new Guid(envelopeID)).ToString());

                MemoryStream ms = new MemoryStream(imgData);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                response.Content.Headers.ContentLength = ms.Length;
                return File(imgData, "image/png");
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return BadRequest();
            }
        }

        [ProducesResponseType(typeof(HttpResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetImagesForSubEnvelopePrefill/{id}/{envelopeId}/{imageName}/{uncPath}")]
        [HttpGet]
        public async Task<IActionResult> GetImagesForSubEnvelopePrefill(string id, string envelopeID, string imageName, string uncPath)
        {
            loggerModelNew = new LoggerModelNew("", "EnvelopeController", "GetImagesForSubEnvelopePrefill", "Process started for Get group envelope Image path by envelopeId using API", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                string uncPathName = _modelHelper.GetEnvelopeDirectoryByName(uncPath);
                byte[] imgData = System.IO.File.ReadAllBytes(_envelopeHelperMain.GetSubEnvelopeImagePathForPrefillNew(Convert.ToInt32(id), new Guid(envelopeID), imageName).ToString());
                MemoryStream ms = new MemoryStream(imgData);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                response.Content.Headers.ContentLength = ms.Length;
                return File(imgData, "image/png");
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return BadRequest();
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForRedirectURL), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetRedirectURL/{PageName}")]
        [HttpGet]
        public IActionResult GetRedirectURL(string PageName)
        {
            loggerModelNew = new LoggerModelNew("", "Envelope", "GetRedirectURL", "Process started for Get Redirect URL using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                ResponseMessageForRedirectURL responseMessage = new ResponseMessageForRedirectURL();
                string URL = string.Empty;
                if (!string.IsNullOrEmpty(PageName))
                {
                    using (StreamReader r = new StreamReader(_appConfiguration["SiteURLs"]))
                    {
                        string json = r.ReadToEnd();
                        List<CorporateURLs> corporateURLs = JsonConvert.DeserializeObject<List<CorporateURLs>>(json).ToList();
                        CorporateURLs keyValue = corporateURLs.FirstOrDefault(x => x.Name == PageName);
                        if (keyValue != null)
                        {
                            URL = keyValue.URL;
                        }
                    }
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "Fetch URL from JSON file";
                    responseMessage.Message = URL;
                    return Ok(responseMessage);
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "PageName";
                    responseMessage.Message = URL;
                    return Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in Envelope Controller controller GetRedirectURL action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForInitalizeSignDocument), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("CreateInviteSignerEnvelope")]
        [HttpPost]
        public async Task<IActionResult> CreateInviteSignerEnvelope(EnvelopeSignDocumentSubmitInfo envelopeSignDocumentSubmitInfo)
        {
            object _locker = new object();
            HttpResponseMessage responseToClient = new HttpResponseMessage();
            const int passwordKeySize = 128;
            bool lockTaken = false, AvaibleUnits = true, enablePostSigningLoginPopup = true;
            int fileSize = 0;
            string ClientId = "405BDFE4-828B-4656-8795-11FCCA993622";
            Guid globalEnvelopeID = Guid.Empty;
            string postSigningPage = Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]);
            string userToken = string.Empty, currentMethod = "CreateInviteSignerEnvelope";
            loggerModelNew = new LoggerModelNew("", "Envelope Controller", "CreateInviteSignerEnvelope", "Process started for Creating a static envelope using API.", envelopeSignDocumentSubmitInfo.EnvelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            bool IsAllowMultiSigner = false;
            bool IncludeStaticLinksValidation = false;
            try
            {
                globalEnvelopeID = envelopeSignDocumentSubmitInfo.EnvelopeID != null && envelopeSignDocumentSubmitInfo.EnvelopeID != Guid.Empty ? envelopeSignDocumentSubmitInfo.EnvelopeID : Guid.NewGuid();
                var rpostRestService = new RpostRestService(_appConfiguration);
                string completeEncodedKey = ModelHelper.GenerateKey(passwordKeySize);
                Guid newEnvelopeID = globalEnvelopeID;//Guid.NewGuid();
                Guid newsenderRecID = Guid.NewGuid();
                Guid newRecipientID = Guid.NewGuid();
                Guid newDocID = Guid.NewGuid();
                Guid newContentID = Guid.NewGuid();
                bool isSignDocument = false;
                string errorIfFound = string.Empty;
                InfoResultResonse responseMessage = new InfoResultResonse();
                var isConfirmationReq = false;

                EnvelopeContent envelopeContent = new EnvelopeContent();
                var userSettingsForfinalMergePDF = new FinalContractSettings();
                Template template = _genericRepository.GetTemplateEntity(envelopeSignDocumentSubmitInfo.StaticTemplateID);
                UserProfile userProfile = _userRepository.GetUserProfileByUserID(template.UserID);

                string tokenType = "Basic";
                //Call RCS API For Deduct Units And Set AvaibleUnits True
                userToken = string.Format("{0}:{1}", ClientId, userProfile.EmailID);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(userToken);
                string base64UserToken = Convert.ToBase64String(plainTextBytes);

                //Intialized Envelope
                APIEnvelope envelope = new APIEnvelope();
                envelope.StaticTemplateID = template.ID;
                envelope.DateFormatID = Convert.ToString(template.DateFormatID);
                envelope.ExpiryTypeID = Convert.ToString(template.ExpiryTypeID);
                envelope.PasswordRequiredToSign = template.PasswordReqdtoSign;
                envelope.PasswordRequiredToOpen = template.PasswordReqdtoOpen;
                envelope.PasswordToSign = template.PasswordtoSign;
                envelope.PasswordToOpen = template.PasswordtoOpen;
                envelope.IsTransparencyDocReq = Convert.ToBoolean(template.IsTransparencyDocReq);
                envelope.IsSignerAttachFileReq = (template.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                envelope.IsSignerAttachFileReqNew = template.IsSignerAttachFileReq != null ? template.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                envelope.IpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                envelope.IsEnvelopeComplete = envelopeSignDocumentSubmitInfo.IsEnvelopeComplete;
                envelope.RecipientEmail = envelopeSignDocumentSubmitInfo.RecipientEmail;
                envelope.ControlCollection = envelopeSignDocumentSubmitInfo.IsEnvelopeComplete ? _envelopeRepository.getDocumentContentDetails(envelopeSignDocumentSubmitInfo.ControlCollection) : null;
                envelope.Comment = envelopeSignDocumentSubmitInfo.Comment;
                envelope.IsConfirmationEmailReq = envelopeSignDocumentSubmitInfo.IsConfirmationEmailReq;
                envelope.CultureInfo = template.CultureInfo;
                envelope.CertificateSignature = envelopeSignDocumentSubmitInfo.CertificateSignature;
                envelope.PostSigningLandingPage = template.PostSigningLandingPage;
                envelope.IsStoreSignatureCertificate = template.IsStoreSignatureCertificate;
                envelope.IsEnableFileReview = template.IsEnableFileReview;
                bool isSignerattachmentProcess = false;
                if (template.IsSignerAttachFileReq > 0)
                {
                    isSignerattachmentProcess = (template.IsAdditionalAttmReq.HasValue && template.IsAdditionalAttmReq == true) ? true : false;
                }
                envelope.IsAdditionalAttmReq = isSignerattachmentProcess; //template.IsAdditionalAttmReq.HasValue ? template.IsAdditionalAttmReq : false;
                envelope.CreatedSource = Constants.CreatedSource.WebApp;
                envelope.EnableRecipientLanguage = template.EnableRecipientLanguage;
                envelope.EnableMessageToMobile = (template.EnableMessageToMobile == null || template.EnableMessageToMobile == false) ? false : true;
                envelope.RestrictRecipientsToContact = template.RestrictRecipientsToContact;
                envelope.ReVerifySignerDocumentSubmit = template.ReVerifySignerStaticTemplate;

                APIRecipientEntityModel recipientEntity = new APIRecipientEntityModel();
                Guid? reqtemplateKey = Guid.Empty;
                if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.TemplateKey))
                {
                    reqtemplateKey = new Guid(envelopeSignDocumentSubmitInfo.TemplateKey);
                }

                if (!Convert.ToBoolean(envelope.EnableMessageToMobile))
                {
                    recipientEntity = await _recipientRepository.GetRecipientEntity(envelopeSignDocumentSubmitInfo.RecipientEmail, envelopeSignDocumentSubmitInfo.StaticTemplateID, reqtemplateKey);
                }
                else
                {
                    if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientEmail))
                    {
                        recipientEntity = await _recipientRepository.GetRecipientEntity(envelopeSignDocumentSubmitInfo.RecipientEmail, envelopeSignDocumentSubmitInfo.StaticTemplateID, reqtemplateKey);
                    }
                    else
                    {
                        string recipientMobile = envelopeSignDocumentSubmitInfo.RecipientDialCode + envelopeSignDocumentSubmitInfo.RecipientMobile;
                        recipientEntity = await _recipientRepository.GetRecipientEntity("", envelopeSignDocumentSubmitInfo.StaticTemplateID, reqtemplateKey, recipientMobile);
                    }
                }

                // recipientEntity = await _recipientRepository.GetRecipientEntity(envelopeSignDocumentSubmitInfo.RecipientEmail, envelopeSignDocumentSubmitInfo.StaticTemplateID, reqtemplateKey);
                if (recipientEntity != null)
                {
                    int waitingTime = Convert.ToInt32(_appConfiguration["StaticUrlWatingTimeInSeconds"]);
                    DateTime currentDate = DateTime.Now;
                    double seconds = 0;
                    if (recipientEntity.CreatedOn != null)
                    {
                        seconds = System.Math.Abs((currentDate - recipientEntity.CreatedOn.Value).TotalSeconds);
                        if (seconds < waitingTime)
                        {
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.success = false;
                            responseMessage.returnUrl = "Info/Index";
                            responseMessage.field = "Staticdelay";
                            responseMessage.data = seconds;
                            return BadRequest(responseMessage);
                        }
                    }
                }

                string displayCode = Convert.ToString(template.EDisplayCode);
                loggerModelNew.Message = "In the process of singing static template";
                rSignLogger.RSignLogInfo(loggerModelNew);

                if (template.IsStatic == false)
                {
                    loggerModelNew.Message = "Document has been expired";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = _envelopeHelperMain.GetLanguageBasedApiMessge(template.UserID, "staticLinkDocExpired");
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }

                if (reqtemplateKey != Guid.Empty)
                {
                    if (template.TemplateKey != reqtemplateKey)
                    {
                        loggerModelNew.Message = "Not a valid link";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.message = _envelopeHelperMain.GetLanguageBasedApiMessge(template.UserID, "staticLinkDocExpired");
                        responseMessage.success = false;
                        responseMessage.returnUrl = "Info/Index";
                        return BadRequest(responseMessage);
                    }
                }

                if (template.TemplateKey != null && reqtemplateKey == Guid.Empty)
                {
                    loggerModelNew.Message = "Not a valid link";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = _envelopeHelperMain.GetLanguageBasedApiMessge(template.UserID, "staticLinkDocExpired");
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    responseMessage.field = "Staticdelay";
                    return BadRequest(responseMessage);
                }

                HttpResponseMessage responseRCS = rpostRestService.RestUserInfo(userProfile.EmailID, base64UserToken, tokenType);
                var jSonResponseRCS = JsonConvert.DeserializeObject(responseRCS.Content.ReadAsStringAsync().Result).ToString();
                RestResponseUserInfo rcsUserInfo = JsonConvert.DeserializeObject<RestResponseUserInfo>(jSonResponseRCS);
                if (rcsUserInfo.ResultContent == null || rcsUserInfo.ResultContent.Plan == null || rcsUserInfo.ResultContent.Customer == null)
                {
                    loggerModelNew.Message = "User is not associated with either plan or result-content for user is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = "Process can not be completed. Please contact sender";
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }
                else if ((rcsUserInfo.ResultContent.Plan.AllowedUnits - rcsUserInfo.ResultContent.Plan.UnitsSent) <= 0)
                {
                    AvaibleUnits = false;
                }
                if (AvaibleUnits == false)
                {
                    loggerModelNew.Message = "Not have enough units";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = "Insufficient units." + System.Environment.NewLine + "&nbsp &nbsp &nbsp Please contact sender";
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                    //return Json(new InfoResult { success = false, message = "Insufficient units." + System.Environment.NewLine + "&nbsp &nbsp &nbsp Please contact sender" }, JsonRequestBehavior.AllowGet);
                }

                //This changes is for static link template US1348

                //envelope.PasswordRequiredToSign = envelope.PasswordRequiredToOpen = false;
                //envelope.PasswordToSign = envelope.PasswordToOpen = null;

                if (envelope.DateFormatID != null && envelope.ExpiryTypeID != null)
                {
                    if (!_masterDataRepository.ValidateDateFormatId(new Guid(envelope.DateFormatID)))
                    {
                        responseMessage.StatusCode = HttpStatusCode.Forbidden;
                        responseMessage.StatusMessage = "Forbidden";
                        responseMessage.success = false;
                        responseMessage.message = Convert.ToString(_appConfiguration["DateFormatIdInvalid"]);
                        return BadRequest(responseMessage);
                    }
                    if (!_masterDataRepository.ValidateExpiryTypeId(new Guid(envelope.ExpiryTypeID)))
                    {
                        responseMessage.StatusCode = HttpStatusCode.Forbidden;
                        responseMessage.StatusMessage = "Forbidden";
                        responseMessage.success = false;
                        responseMessage.message = Convert.ToString(_appConfiguration["ExpiryTypeIdInvalid"]);
                        return BadRequest(responseMessage);
                    }

                    /* Get User Settings */
                    APISettings apiSettings = _settingsRepository.GetEntityByParam(template.UserID, string.Empty, Constants.String.SettingsType.User);
                    var userSettings = _eSignHelper.TransformSettingsDictionaryToEntity(apiSettings);
                    userSettings.UserID = template.UserID;
                    _eSignHelper.SetApiCallFlag();
                    responseMessage.AttachSignedPdfID = userSettings.AttachSignedPdfID;

                    var RequireSignerConfirmationSetting = _settingsRepository.GetEntityForByKeyConfig((Guid)userSettings.UserID, Constants.SettingsKeyConfig.IncludeStaticTemplates);
                    if (RequireSignerConfirmationSetting != null)
                        IncludeStaticLinksValidation = Convert.ToBoolean(RequireSignerConfirmationSetting.OptionValue);


                    bool isEnvelopeTosign = envelope.IsEnvelopeComplete;
                    isConfirmationReq = envelope.IsConfirmationEmailReq && !IncludeStaticLinksValidation;

                    //Insert  Envelope
                    Envelope envlp = new Envelope();
                    envlp.ID = newEnvelopeID;
                    envlp.EnvelopeCode = _genericRepository.GetMaxEnvelopeCode() + 1;
                    envlp.UserID = template.UserID;
                    envlp.DateFormatID = new Guid(envelope.DateFormatID);
					     envlp.ReminderDays = template.ReminderDays;
                            envlp.FinalReminderDays = template.FinalReminderDays;
                            envlp.ReminderRepeatDays = template.ReminderRepeatDays;
                            envlp.FinalReminderTypeID = template.FinalReminderTypeID;
                            envlp.ReminderTypeID = template.ReminderTypeID;
                            envlp.ThenReminderTypeID = template.ThenReminderTypeID;
                    envlp.ExpiryTypeID = new Guid(envelope.ExpiryTypeID);
                    envlp.IsTransparencyDocReq = envelope.IsTransparencyDocReq;
                    envlp.IsSignerAttachFileReq = envelope.IsSignerAttachFileReqNew;
                    envlp.IsFinalCertificateReq = template.IsFinalCertificateReq;
                    envlp.IsAttachXML = Convert.ToBoolean(template.IsAttachXML);
                    envlp.IsPrivateMode = Convert.ToBoolean(template.IsPrivateMode);
                    envlp.IsSeparateMultipleDocumentsAfterSigningRequired = Convert.ToBoolean(template.IsSeparateMultipleDocumentsAfterSigningRequired);
                    envlp.CultureInfo = template.CultureInfo;
                    envlp.IsStatic = true;
                    envlp.IsDefaultSignatureForStaticTemplate = template.IsDefaultSignatureForStaticTemplate;
                    envlp.ReferenceCode = template.ReferenceCode;
                    envlp.ReferenceEmail = template.ReferenceEmail;
                    envlp.IsStoreOriginalDocument = envelope.IsStoreOriginalDocument;
                    envlp.IsStoreSignatureCertificate = envelope.IsStoreSignatureCertificate;
                    envlp.IsDisclaimerInCertificate = Convert.ToBoolean(userSettings.IsDisclaimerInCertificate);
                    envlp.DisclaimerText = userSettings.Disclaimer;

                    if (envelope.IsSignerAttachFileReqNew > 0)
                    {
                        isSignerattachmentProcess = envelope.IsAdditionalAttmReq != null ? Convert.ToBoolean(envelope.IsAdditionalAttmReq) : Convert.ToBoolean(template.IsAdditionalAttmReq);
                    }
                    envlp.IsAdditionalAttmReq = isSignerattachmentProcess;

                    if (template.TemplateKey != Guid.Empty)
                    {
                        envlp.TemplateKey = template.TemplateKey;
                    }
                    else
                    {
                        envlp.TemplateKey = Constants.StatusCode.Envelope.TemplateKey;
                    }

                    if (!isConfirmationReq && envelopeSignDocumentSubmitInfo.InviteSignerModels != null && envelopeSignDocumentSubmitInfo.InviteSignerModels.Count() == 0)
                    {
                        envlp.StatusID = envelope.IsEnvelopeComplete == true ? Constants.StatusCode.Envelope.Completed : Constants.StatusCode.Envelope.Terminated;
                    }
                    else
                    {
                        envlp.TemplateDescription = envelope.Comment;
                        envlp.StatusID = Constants.StatusCode.Envelope.Waiting_For_Signature;
                    }
                    envlp.SigningCertificateName = Convert.ToString(_appConfiguration["SigningCertificateName"]);
                    envlp.CreatedDateTime = DateTime.Now;
                    envlp.ModifiedDateTime = DateTime.Now;
                    envlp.Subject = template.TemplateName;
                    envlp.Message = template.Message;
                    envlp.IsEnvelope = true;
                    envlp.IsTemplateDeleted = false;
                    envlp.IsTemplateEditable = true;
                    envlp.IsEnvelopeComplete = true;
                    envlp.EDisplayCode = EnvelopeHelperMain.TakeUniqueDisplayCode();
                    envlp.IsEnvelopePrepare = true;
                    envlp.IpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                    envlp.SharedTemplateID = envelope.StaticTemplateID;
                    envlp.PasswordReqdtoOpen = envelope.PasswordRequiredToOpen;
                    envlp.PasswordReqdtoSign = envelope.PasswordRequiredToSign;
                    if (envelope.PasswordRequiredToOpen)
                        envlp.PasswordtoOpen = ModelHelper.Decrypt(envelope.PasswordToOpen, template.PasswordKey, (int)template.PasswordKeySize);
                    if (envelope.PasswordRequiredToOpen)
                        envlp.PasswordtoSign = ModelHelper.Decrypt(envelope.PasswordToSign, template.PasswordKey, (int)template.PasswordKeySize);
                    envlp.PasswordKey = template.PasswordKey;
                    envlp.PasswordKeySize = template.PasswordKeySize;
                    envlp.IsPasswordMailToSigner = template.IsPasswordMailToSigner;
                    envlp.IsRandomPassword = template.IsRandomPassword;
                    envlp.AccessAuthType = template.AccessAuthType;
                    envlp.HeaderFooterOption = userSettings.HeaderFooterSettingID;
                    envlp.ElectronicSignIndicationOptionID = userSettings.ElectronicSignIndicationSelectedID;
                    envlp.PostSigningLandingPage = envelope.PostSigningLandingPage;
                    envlp.IpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                    envlp.IsEnableFileReview = envelope.IsEnableFileReview;
                    envlp.IsRule = false;
                    envlp.CreatedSource = Constants.CreatedSource.WebApp;
                    envlp.InviteMessage = envelopeSignDocumentSubmitInfo.InviteSignerModels != null ? envelopeSignDocumentSubmitInfo.InviteSignerModels[0].InvitesignerMessage : string.Empty;
                    IsAllowMultiSigner = envelopeSignDocumentSubmitInfo.InviteSignerModels != null && envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(d => d.SignNowOrInvitedEmail == 2).Count() > 0 ? true : false;
                    envlp.IsInvitedBySigner = IsAllowMultiSigner;
                    envlp.AllowMultiSigner = template.AllowMultiSigner;
                    envlp.SendConfirmationEmail = isConfirmationReq;
                    envlp.IsEnableAutoFillTextControls = template.IsEnableAutoFillTextControls;
                    envlp.EnableRecipientLanguage = envelope.EnableRecipientLanguage;
                    envlp.EnableMessageToMobile = envelope.EnableMessageToMobile;
                    envlp.RestrictRecipientsToContact = envelope.RestrictRecipientsToContact;
                    envlp.ReVerifySignerDocumentSubmit = template.ReVerifySignerStaticTemplate;
                    envlp.IsSendMessageCodetoAvailableEmailorMobile = envelopeSignDocumentSubmitInfo.IsSendMessageCodetoAvailableEmailorMobile;

                    var watermarkStamp = _envelopeHelperMain.GetWatermarkStamp(envlp.UserID);
                    if (watermarkStamp != null)
                    {
                        envlp.IsWaterMark = watermarkStamp.IsWaterMark;
                        envlp.WatermarkTextForSender = watermarkStamp.WatermarkTextForSender;
                        envlp.WatermarkTextForOther = watermarkStamp.WatermarkTextForOther;
                    }
                    _genericRepository.SetInitializeEnvelopeFlag();
                    _genericRepository.Save(envlp);

                    //Insert Sender Recipient
                    Recipients senderRec = new Recipients();
                    senderRec.ID = newsenderRecID;
                    senderRec.EnvelopeID = newEnvelopeID;
                    senderRec.RecipientTypeID = Constants.RecipientType.Sender;
                    senderRec.Name = userProfile.FirstName + " " + userProfile.LastName;
                    senderRec.EmailAddress = userProfile.EmailID;
                    responseMessage.InfoSenderEmail = senderRec.EmailAddress;
                    senderRec.Order = null;
                    senderRec.CreatedDateTime = DateTime.Now;
                    _recipientRepository.Save(senderRec);

                    //Insert  Sender Signer Status
                    SignerStatus senderSignerStatus = new SignerStatus();
                    senderSignerStatus.ID = Guid.NewGuid();
                    senderSignerStatus.RecipientID = newsenderRecID;
                    senderSignerStatus.StatusID = Constants.StatusCode.Signer.Sent;
                    senderSignerStatus.RejectionRemarks = null;
                    senderSignerStatus.CreatedDateTime = DateTime.Now;
                    senderSignerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                    senderSignerStatus.DelegateTo = null;
                    _recipientRepository.Save(senderSignerStatus);

                    //Insert Sender Recipient Details
                    _recipientRepository.SaveRecipientDetailOnSend(newsenderRecID, Constants.StatusCode.Signer.Sent, envelope.IpAddress);

                    if (envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(i => i.IscurrentRecipient == true).Count() > 0)
                    {
                        //Insert  Recipient
                        foreach (var recip in envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(i => i.IscurrentRecipient == true))
                        {
                            RSign.Models.Recipients recipient = new RSign.Models.Recipients();
                            recipient.ID = newRecipientID;//Guid.NewGuid();
                            recipient.EnvelopeID = newEnvelopeID;
                            recipient.RecipientTypeID = Constants.RecipientType.Signer;
                            recipient.Name = string.IsNullOrEmpty(recip.SignerName) ? envelopeSignDocumentSubmitInfo.CurrentEmail : recip.SignerName;
                            recipient.EmailAddress = string.IsNullOrEmpty(recip.SignerEmail) ? string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.CurrentEmail) ? "" : envelopeSignDocumentSubmitInfo.CurrentEmail : recip.SignerEmail;
                            recipient.Order = 1;
                            recipient.CreatedDateTime = DateTime.Now;
                            recipient.IsReviewed = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.IsReviewed) ? Convert.ToInt32(envelopeSignDocumentSubmitInfo.IsReviewed) : 0;
                            recipient.SignNowOrInviteEmail = 1;
                            recipient.CanEdit = false;
                            recip.RecipientID = newRecipientID;
                            recipient.CultureInfo = !string.IsNullOrEmpty(recip.CultureInfo) || recip.CultureInfo.ToLower() != "null" ? recip.CultureInfo : template.CultureInfo;
                            if (Convert.ToBoolean(envelope.EnableMessageToMobile) && (recip.SignerDeliveryMode != "1" && recip.SignerDeliveryMode != "11"))
                            {
                                recipient.DeliveryMode = Convert.ToInt32(recip.SignerDeliveryMode);
                                recipient.DialCode = !string.IsNullOrEmpty(recip.SignerDialCode) ? recip.SignerDialCode : "";
                                recipient.CountryCode = (recip.SignerCountryCode != null && recip.SignerCountryCode != "") ? recip.SignerCountryCode.ToUpper() : recip.SignerCountryCode;
                                recipient.Mobile = recip.SignerMobile;
                               // recipient.ReminderType = recipient.DeliveryMode;
                            }
                            else
                            {
                                recipient.DeliveryMode = !string.IsNullOrEmpty(recip.SignerDeliveryMode) ? Convert.ToInt32(recip.SignerDeliveryMode) : 1;
                                recipient.DialCode = null;
                                recipient.CountryCode = null;
                                recipient.Mobile = null;
                               // recipient.ReminderType = recipient.DeliveryMode;
                            }

                            envlp.Recipients.Add(recipient);
                            envelopeSignDocumentSubmitInfo.CurrentEmail = recip.SignerEmail;

                            /*Start - To Update Envelope Recipient ID instead of Template Role Id*/
                            _envelopeRepository.UpdateDocumentRequestRecipient(new Guid(recip.RoleId), newRecipientID, newEnvelopeID, recipient.EmailAddress);
                            /*End - To Update Envelope Recipient ID instead of Template Role Id*/
                        }
                    }
                    else
                    {
                        RSign.Models.Recipients recipient = new RSign.Models.Recipients();
                        recipient.ID = newRecipientID;//Guid.NewGuid();
                        recipient.EnvelopeID = newEnvelopeID;
                        recipient.RecipientTypeID = Constants.RecipientType.Signer;
                        recipient.Name = envelopeSignDocumentSubmitInfo.CurrentEmail;
                        recipient.EmailAddress = string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.CurrentEmail) ? "" : envelopeSignDocumentSubmitInfo.CurrentEmail;
                        recipient.Order = 1;
                        recipient.CreatedDateTime = DateTime.Now;
                        recipient.IsReviewed = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.IsReviewed) ? Convert.ToInt32(envelopeSignDocumentSubmitInfo.IsReviewed) : 0;
                        recipient.SignNowOrInviteEmail = 1;
                        recipient.CultureInfo = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.CultureInfo) ? envelopeSignDocumentSubmitInfo.CultureInfo : "en-us";
                      //  recipient.ReminderType = null;
                        if (Convert.ToBoolean(envelope.EnableMessageToMobile) && (envelopeSignDocumentSubmitInfo.RecipientDeliveryMode != "1" && envelopeSignDocumentSubmitInfo.RecipientDeliveryMode != "11"))
                        {
                            recipient.DeliveryMode = Convert.ToInt32(envelopeSignDocumentSubmitInfo.RecipientDeliveryMode);
                            recipient.DialCode = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientDialCode) ? envelopeSignDocumentSubmitInfo.RecipientDialCode : ""; ;
                            recipient.CountryCode = (envelopeSignDocumentSubmitInfo.RecipientCountryCode != null && envelopeSignDocumentSubmitInfo.RecipientCountryCode != "") ? envelopeSignDocumentSubmitInfo.RecipientCountryCode.ToUpper() : envelopeSignDocumentSubmitInfo.RecipientCountryCode;
                            recipient.Mobile = envelopeSignDocumentSubmitInfo.RecipientMobile;
                           // recipient.ReminderType = recipient.DeliveryMode;
                        }
                        else
                        {
                            recipient.DeliveryMode = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientDeliveryMode) ? Convert.ToInt32(envelopeSignDocumentSubmitInfo.RecipientDeliveryMode) : 1;
                            recipient.DialCode = null;
                            recipient.CountryCode = null;
                            recipient.Mobile = null;
                           // recipient.ReminderType = recipient.DeliveryMode;
                        }
                        envlp.Recipients.Add(recipient);
                    }

                    Guid InviteRecipientID = Guid.NewGuid();
                    foreach (var recip in envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(i => i.IscurrentRecipient != true))
                    {
                        InviteRecipientID = Guid.NewGuid();
                        //Insert  Recipient
                        Recipients inviterecipient = new Recipients();
                        inviterecipient.ID = InviteRecipientID;
                        inviterecipient.EnvelopeID = newEnvelopeID;
                        inviterecipient.RecipientTypeID = Constants.RecipientType.Signer;
                        inviterecipient.Name = recip.SignerName;
                        inviterecipient.EmailAddress = string.IsNullOrEmpty(recip.SignerEmail) ? "" : recip.SignerEmail;
                        inviterecipient.Order = 1;
                        inviterecipient.CreatedDateTime = DateTime.Now;
                        inviterecipient.IsReviewed = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.IsReviewed) ? Convert.ToInt32(envelopeSignDocumentSubmitInfo.IsReviewed) : 0;
                        inviterecipient.CanEdit = recip.CanEdit;
                        inviterecipient.SignNowOrInviteEmail = recip.SignNowOrInvitedEmail;
                        inviterecipient.CultureInfo = !string.IsNullOrEmpty(recip.CultureInfo) || recip.CultureInfo.ToLower() != "null" ? recip.CultureInfo : template.CultureInfo;
                       // inviterecipient.ReminderType = recip.ReminderType;
                        if (Convert.ToBoolean(envelope.EnableMessageToMobile) && (recip.SignerDeliveryMode != "1" && recip.SignerDeliveryMode != "11"))
                        {
                            inviterecipient.DeliveryMode = Convert.ToInt32(recip.SignerDeliveryMode);
                            inviterecipient.DialCode = !string.IsNullOrEmpty(recip.SignerMobile) ? recip.SignerDialCode : "";
                            inviterecipient.CountryCode = (recip.SignerCountryCode != null && recip.SignerCountryCode != "") ? recip.SignerCountryCode.ToUpper() : recip.SignerCountryCode;
                            inviterecipient.Mobile = recip.SignerMobile;
                           // inviterecipient.ReminderType = inviterecipient.ReminderType;
                        }
                        else
                        {
                            inviterecipient.DeliveryMode = !string.IsNullOrEmpty(recip.SignerDeliveryMode) ? Convert.ToInt32(recip.SignerDeliveryMode) : 1;
                            inviterecipient.DialCode = null;
                            inviterecipient.CountryCode = null;
                            inviterecipient.Mobile = null;
                            //inviterecipient.ReminderType = inviterecipient.ReminderType;
                        }

                        envlp.Recipients.Add(inviterecipient);
                        recip.RecipientID = InviteRecipientID;

                        /*Start - To Update Envelope Recipient ID instead of Template Role Id*/
                        _envelopeRepository.UpdateDocumentRequestRecipient(new Guid(recip.RoleId), InviteRecipientID, newEnvelopeID, inviterecipient.EmailAddress);
                        /*End - To Update Envelope Recipient ID instead of Template Role Id*/
                    }

                    foreach (var recp in envlp.Recipients)
                    {
                        _recipientRepository.Save(recp);
                    }

                    List<EnvelopeMapping> envelopeMappingsList = new List<EnvelopeMapping>();
                    //To store selected Template/Rule Reference For Envelope (S3-1946).
                    if (template.ID != Guid.Empty)
                    {
                        EnvelopeTemplateMappingDetails mappingDetails = new EnvelopeTemplateMappingDetails();
                        EnvelopeMapping envelopeMapping = new EnvelopeMapping();
                        mappingDetails.EnvelopeId = newEnvelopeID;
                        mappingDetails.TemplateId = new List<Guid> { template.ID };
                        mappingDetails.UserId = template.UserID;
                        envelopeMapping.TemplateId = template.ID;
                        envelopeMapping.TemplateCode = template.TemplateCode;
                        envelopeMapping.TemplateName = template.TemplateName;
                        envelopeMappingsList.Add(envelopeMapping);
                        mappingDetails.envelopeMappingsToTemp = envelopeMappingsList;
                        mappingDetails.EnvelopeTypeId = template.EnvelopeTypeId;
                        _envelopeRepository.SaveEnvelopeTemplateMapping(mappingDetails);
                    }

                    //Insert  Recipient Signer Status
                    if (envelope.IsEnvelopeComplete && !isConfirmationReq)
                    {
                        SignerStatus signerStatus = new SignerStatus();
                        signerStatus.ID = Guid.NewGuid();
                        signerStatus.RecipientID = newRecipientID;
                        signerStatus.CreatedDateTime = DateTime.Now.AddSeconds(1);
                        signerStatus.StatusID = Constants.StatusCode.Signer.Signed;
                        signerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                        _recipientRepository.Save(signerStatus);

                        foreach (var recip in envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(i => i.IscurrentRecipient != true))
                        {
                            if (recip.SignNowOrInvitedEmail != 2 && newRecipientID != recip.RecipientID)
                            {
                                SignerStatus SignNowsignerStatus = new SignerStatus();
                                SignNowsignerStatus.ID = Guid.NewGuid();
                                SignNowsignerStatus.RecipientID = recip.RecipientID;
                                SignNowsignerStatus.CreatedDateTime = DateTime.Now.AddSeconds(3);
                                SignNowsignerStatus.StatusID = Constants.StatusCode.Signer.Signed;
                                SignNowsignerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                                _recipientRepository.Save(SignNowsignerStatus);
                            }
                        }
                        foreach (var recip in envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(i => i.IscurrentRecipient != true))
                        {
                            if (recip.SignNowOrInvitedEmail == 2)
                            {
                                //Insert  Recipient
                                SignerStatus SignNowsignerStatus = new SignerStatus();
                                SignNowsignerStatus.ID = Guid.NewGuid();
                                SignNowsignerStatus.RecipientID = recip.RecipientID;
                                SignNowsignerStatus.CreatedDateTime = DateTime.Now.AddSeconds(5);
                                SignNowsignerStatus.StatusID = Constants.StatusCode.Signer.Pending;
                                SignNowsignerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                                _recipientRepository.Save(SignNowsignerStatus);
                            }

                        }
                    }
                    else if (envelope.IsEnvelopeComplete && isConfirmationReq)
                    {
                        foreach (var recip in envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(i => i.IscurrentRecipient != true))
                        {
                            if (recip.SignNowOrInvitedEmail != 2 && newRecipientID != recip.RecipientID)
                            {
                                SignerStatus SignNowsignerStatus = new SignerStatus();
                                SignNowsignerStatus.ID = Guid.NewGuid();
                                SignNowsignerStatus.RecipientID = recip.RecipientID;
                                SignNowsignerStatus.CreatedDateTime = DateTime.Now.AddSeconds(3);
                                SignNowsignerStatus.StatusID = Constants.StatusCode.Signer.Signed;
                                SignNowsignerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                                _recipientRepository.Save(SignNowsignerStatus);
                            }
                        }
                        //foreach (var recip in envelopeSignDocumentSubmitInfo.inviteSignerModels.Where(i => i.IscurrentRecipient != true))
                        //{
                        //    if (recip.SignNowOrInvitedEmail == 2)
                        //    {
                        //        //Insert  Recipient
                        //        SignerStatus SignNowsignerStatus = new SignerStatus();
                        //        SignNowsignerStatus.ID = Guid.NewGuid();
                        //        SignNowsignerStatus.RecipientID = recip.RecipientID;
                        //        SignNowsignerStatus.CreatedDateTime = DateTime.Now.AddSeconds(5);
                        //        SignNowsignerStatus.StatusID = Constants.StatusCode.Signer.Pending;
                        //        SignNowsignerStatus.IPAddress = envelope.IpAddress;
                        //        recipientRepository.Save(SignNowsignerStatus);

                        //    }

                        //}
                    }
                    else if (!isConfirmationReq)
                    {
                        SignerStatus signerStatus = new SignerStatus();
                        signerStatus.ID = Guid.NewGuid();
                        signerStatus.RecipientID = newRecipientID;
                        signerStatus.CreatedDateTime = DateTime.Now.AddSeconds(1);
                        signerStatus.StatusID = Constants.StatusCode.Signer.Rejected;
                        signerStatus.DeclineReasonID = envelopeSignDocumentSubmitInfo.DeclineReasonID;
                        signerStatus.RejectionRemarks = envelope.Comment;
                        signerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                        _recipientRepository.Save(signerStatus);
                    }

                    //inviteSignerModel.RoleName = envelopeSignDocumentSubmitInfo.c
                    //Insert  Recipient Details
                    if (envelope.IsEnvelopeComplete && !isConfirmationReq)
                    {
                        _recipientRepository.SaveRecipientDetailOnSend(newRecipientID, Constants.StatusCode.Signer.Signed, envelope.IpAddress);
                        foreach (var recip in envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(i => i.IscurrentRecipient != true))
                        {
                            if (recip.SignNowOrInvitedEmail == 1)
                            {
                                _recipientRepository.SaveRecipientDetailOnSend(recip.RecipientID, Constants.StatusCode.Signer.Signed, envelope.IpAddress);
                            }
                        }
                    }
                    int confirmcount = 1;
                    if (isConfirmationReq || envelopeSignDocumentSubmitInfo.DeclineReasonID != null)
                    {

                        foreach (var recip in envelopeSignDocumentSubmitInfo.InviteSignerModels.Where(i => i.IscurrentRecipient != true))
                        {
                            if (recip.SignNowOrInvitedEmail == 1)
                            {
                                _recipientRepository.SaveRecipientDetailOnSend(recip.RecipientID, Constants.StatusCode.Signer.Signed, envelope.IpAddress);
                            }

                        }
                        if (confirmcount == 1)
                        {
                            if (!isConfirmationReq)
                                _recipientRepository.SaveRecipientDetailForEmailConfirm(newRecipientID, Constants.StatusCode.Signer.Rejected, envelope.IpAddress);
                            else
                                _recipientRepository.SaveRecipientDetailForEmailConfirm(newRecipientID, Constants.StatusCode.Signer.AwaitingConfirmation, envelope.IpAddress);
                        }
                    }


                    Dictionary<Guid, Guid> roleToRecipientMapping = new Dictionary<Guid, Guid>();
                    Guid firstRecipientID = Guid.Empty;

                    Dictionary<Guid, Guid> controlIDMapping = new Dictionary<Guid, Guid>();
                    Dictionary<Guid, Guid> ruleIDMapping = new Dictionary<Guid, Guid>();
                    Documents tempDoc = new Documents();
                    //S3-1088: Added code to set the time zone as per user setting for DateTimeStamp Control 
                    string dateTimeStampControlValue = string.Empty;
                    if (template.DateFormatID != null)
                    {
                        dateTimeStampControlValue = _envelopeHelperMain.GetDateTimeStampControlValue(template.DateFormatID, string.IsNullOrEmpty(userSettings.SelectedTimeZone) ? "UTC" : userSettings.SelectedTimeZone);
                    }
                    // Insert Document
                    string[] validFileTypes = {
                                                      "docx", "pdf", "doc", "xls", "xlsx", "htm", "html","txt", "ppt", "pptx", "bmp", "gif", "ico", "jpg", "jpeg", "png", "tif", "tiff",
                                                      "DOCX", "PDF", "DOC", "XLS", "XLSX", "HTM", "HTML","TXT", "PPT", "PPTX", "BMP", "GIF", "ICO", "JPG", "JPEG", "PNG", "TIF", "TIFF"
                                                  };
                    bool isValidType = false;
                    foreach (var doc in template.TemplateDocuments)
                    {
                        newDocID = Guid.NewGuid();
                        if (template.EnvelopeTypeId != Constants.EnvelopeType.TemplateRule)
                        {
                            isValidType = false;
                            tempDoc = new Documents();
                            tempDoc.ID = newDocID;
                            tempDoc.EnvelopeID = newEnvelopeID;
                            tempDoc.DocumentName = doc.DocumentName;
                            tempDoc.UploadedDateTime = DateTime.Now;
                            tempDoc.TemplateDocumentName = null;
                            tempDoc.Order = doc.Order;
                            tempDoc.DocumentSource = doc.DocumentSource;
                            string ext = Path.GetExtension(doc.DocumentName);
                            for (int j = 0; j < validFileTypes.Length; j++)
                            {
                                if (ext == "." + validFileTypes[j])
                                {
                                    isValidType = true;
                                    break;
                                }
                            }
                            if (isValidType)
                            {
                                tempDoc.ActionType = Constants.ActionTypes.Sign;
                            }
                            else
                            {
                                tempDoc.ActionType = Constants.ActionTypes.Review;
                            }
                            _documentRepository.Save(tempDoc);

                        }
                        else
                        {
                            newDocID = new Guid("00000000-0000-0000-0000-000000000000");
                        }

                        // Insert Document Contents
                        foreach (var Rec in envelopeSignDocumentSubmitInfo.InviteSignerModels)
                        {
                            foreach (var content in doc.TemplateDocumentContents)
                            {
                                if (content.RecipientID == new Guid(Rec.RoleId))
                                {
                                    DocumentContents tempContent = new DocumentContents();
                                    newContentID = Guid.NewGuid();
                                    tempContent.ID = newContentID;
                                    if (content.ControlID == Constants.Control.Radio)
                                    {
                                        if (!ruleIDMapping.ContainsKey(content.ID))
                                            ruleIDMapping.Add(content.ID, newContentID);
                                    }
                                    tempContent.ControlID = content.ControlID;
                                    tempContent.DocumentID = newDocID;
                                    tempContent.RecipientID = Rec.RecipientID;
                                    tempContent.RecipientName = Rec.SignerEmail == null ? " " : Rec.SignerEmail;
                                    tempContent.ControlHtmlID = content.ControlHtmlID;
                                    tempContent.Required = content.Required;
                                    tempContent.DocumentPageNo = content.DocumentPageNo;
                                    tempContent.PageNo = content.PageNo;
                                    tempContent.XCoordinate = content.XCoordinate;
                                    tempContent.YCoordinate = content.YCoordinate;
                                    tempContent.ZCoordinate = content.ZCoordinate;
                                    tempContent.TabIndex = content.TabIndex;
                                    tempContent.IntControlId = content.IntControlId;
                                    tempContent.CustomToolTip = content.CustomToolTip;

                                    var documentContent = content.IsControlDeleted == false;
                                    string DropdownControlValue = string.Empty;
                                    if (envelopeSignDocumentSubmitInfo.CurrentEmail == Rec.SignerEmail)
                                    {
                                        if (envelope.ControlCollection != null && (Rec.SignNowOrInvitedEmail == 1))
                                        {
                                            var documentContentWithValue = envelope.ControlCollection.FirstOrDefault(d => d.ID == content.ID);
                                            if (documentContentWithValue != null)
                                            {
                                                if (documentContentWithValue.ControlHtmlID == Constants.SignatureType.Auto.ToString().ToUpper() || documentContentWithValue.ControlHtmlID == Constants.SignatureType.Hand.ToString().ToUpper() || documentContentWithValue.ControlHtmlID == Constants.SignatureType.UploadSignature.ToString().ToUpper())
                                                {
                                                    tempContent.RecipientName = Rec.SignerEmail;
                                                    tempContent.ControlValue = "Signed";
                                                    if (documentContentWithValue.ControlValue != null)
                                                    {
                                                        string signValue = documentContentWithValue.ControlValue;
                                                        signValue = signValue.Replace("data:image/png;base64,", string.Empty);
                                                        byte[] signByte = Convert.FromBase64String(signValue);
                                                        // signature stored in new table 
                                                        if (Rec.CertificateSignature != "" && content.ControlHtmlID == Rec.CertificateSignature)
                                                        {
                                                            var signerSignature = new SignerSignature
                                                            {
                                                                ID = Guid.NewGuid(),
                                                                RecipientID = Rec.RecipientID,
                                                                Signature = signByte,
                                                                SignatureTypeId = Guid.Parse(documentContentWithValue.ControlHtmlID)
                                                            };
                                                            _recipientRepository.Save(signerSignature);
                                                        }
                                                        else if (Rec.CertificateSignature != "" && documentContentWithValue.Label == "Signature")
                                                        {
                                                            var signerSignature = new SignerSignature
                                                            {
                                                                ID = Guid.NewGuid(),
                                                                RecipientID = Rec.RecipientID,
                                                                Signature = signByte,
                                                                SignatureTypeId = Guid.Parse(documentContentWithValue.ControlHtmlID)
                                                            };
                                                            _recipientRepository.Save(signerSignature);
                                                        }
                                                        tempContent.SignatureControlValue = signByte;
                                                    }
                                                }
                                                else
                                                {
                                                    if (documentContentWithValue.ControlID == Constants.Control.DateTimeStamp && !string.IsNullOrEmpty(dateTimeStampControlValue))
                                                    {
                                                        documentContentWithValue.ControlValue = dateTimeStampControlValue;
                                                    }
                                                    tempContent.ControlValue = documentContentWithValue.ControlValue;

                                                    if (content.ControlID == Constants.Control.Text && !Convert.ToBoolean(content.IsFixedWidth))
                                                    {
                                                       string controlContentType = EnvelopeHelper.GetTextType((Guid)content.ControlType);
                                                        if (controlContentType.ToLower() == "text" || controlContentType.ToLower() == "alphabet" || controlContentType.ToLower() == "numeric" || controlContentType.ToLower() == "email")
                                                        {
                                                            if (documentContentWithValue.Height != null && documentContentWithValue.Height != 0)
                                                            {
                                                                tempContent.Height = documentContentWithValue.Height;
                                                            }
                                                            if (documentContentWithValue.Width != null && documentContentWithValue.Width != 0)
                                                            {
                                                                tempContent.Width = documentContentWithValue.Width;
                                                            }
                                                            if (documentContentWithValue.ZCoordinate != null && documentContentWithValue.ZCoordinate != 0)
                                                            {
                                                                tempContent.ZCoordinate = documentContentWithValue.ZCoordinate;
                                                            }                                                          
                                                        }
                                                    }
                                                    if (content.ControlID == Constants.Control.Name && !Convert.ToBoolean(content.IsFixedWidth))
                                                    {
                                                        if (documentContentWithValue.Height != null && documentContentWithValue.Height != 0)
                                                        {
                                                            tempContent.Height = documentContentWithValue.Height;
                                                        }
                                                        if (documentContentWithValue.Width != null && documentContentWithValue.Width != 0)
                                                        {
                                                            tempContent.Width = documentContentWithValue.Width;
                                                        }
                                                        if (documentContentWithValue.ZCoordinate != null && documentContentWithValue.ZCoordinate != 0)
                                                        {
                                                            tempContent.ZCoordinate = documentContentWithValue.ZCoordinate;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (envelope.ControlCollection != null && (tempContent.RecipientID == new Guid(Rec.RoleId) || Rec.SignNowOrInvitedEmail == 1))
                                        {
                                            var documentContentWithValue = envelope.ControlCollection.FirstOrDefault(d => d.ID == content.ID);
                                            if (documentContentWithValue != null)
                                            {
                                                if (documentContentWithValue.ControlHtmlID == Constants.SignatureType.Auto.ToString().ToUpper() || documentContentWithValue.ControlHtmlID == Constants.SignatureType.Hand.ToString().ToUpper() || documentContentWithValue.ControlHtmlID == Constants.SignatureType.UploadSignature.ToString().ToUpper())
                                                {
                                                    tempContent.RecipientName = Rec.SignerEmail;
                                                    tempContent.ControlValue = "Signed";
                                                    if (documentContentWithValue.ControlValue != null)
                                                    {
                                                        string signValue = documentContentWithValue.ControlValue;
                                                        signValue = signValue.Replace("data:image/png;base64,", string.Empty);
                                                        byte[] signByte = Convert.FromBase64String(signValue);
                                                        // signature stored in new table 
                                                        //foreach (var docControl in envelopeSignDocumentSubmitInfo.signerSignatureNames)
                                                        //{
                                                        if (documentContentWithValue.Label == "Signature")
                                                        {
                                                            var signerSignature = new SignerSignature
                                                            {
                                                                ID = Guid.NewGuid(),
                                                                RecipientID = Rec.RecipientID,
                                                                Signature = signByte,
                                                                SignatureTypeId = Guid.Parse(documentContentWithValue.ControlHtmlID)
                                                            };
                                                            _recipientRepository.Save(signerSignature);
                                                        }
                                                        //}

                                                        tempContent.SignatureControlValue = signByte;
                                                    }
                                                }
                                                else
                                                {
                                                    if (documentContentWithValue.ControlID == Constants.Control.DateTimeStamp && !string.IsNullOrEmpty(dateTimeStampControlValue))
                                                    {
                                                        documentContentWithValue.ControlValue = dateTimeStampControlValue;
                                                    }
                                                    tempContent.ControlValue = documentContentWithValue.ControlValue;
                                                    if (content.ControlID == Constants.Control.Text && !Convert.ToBoolean(content.IsFixedWidth))
                                                    {
                                                        string controlContentType = EnvelopeHelper.GetTextType((Guid)content.ControlType);
                                                        if (controlContentType.ToLower() == "text" || controlContentType.ToLower() == "alphabet" || controlContentType.ToLower() == "numeric" || controlContentType.ToLower() == "email")
                                                        {
                                                            if (documentContentWithValue.Height != null && documentContentWithValue.Height != 0)
                                                            {
                                                                tempContent.Height = documentContentWithValue.Height;
                                                            }
                                                            if (documentContentWithValue.Width != null && documentContentWithValue.Width != 0)
                                                            {
                                                                tempContent.Width = documentContentWithValue.Width;
                                                            }
                                                            if (documentContentWithValue.ZCoordinate != null && documentContentWithValue.ZCoordinate != 0)
                                                            {
                                                                tempContent.ZCoordinate = documentContentWithValue.ZCoordinate;
                                                            }
                                                        }
                                                    }
                                                    if (content.ControlID == Constants.Control.Name && !Convert.ToBoolean(content.IsFixedWidth))
                                                    {
                                                        if (documentContentWithValue.Height != null && documentContentWithValue.Height != 0)
                                                        {
                                                            tempContent.Height = documentContentWithValue.Height;
                                                        }
                                                        if (documentContentWithValue.Width != null && documentContentWithValue.Width != 0)
                                                        {
                                                            tempContent.Width = documentContentWithValue.Width;
                                                        }
                                                        if (documentContentWithValue.ZCoordinate != null && documentContentWithValue.ZCoordinate != 0)
                                                        {
                                                            tempContent.ZCoordinate = documentContentWithValue.ZCoordinate;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tempContent.Label = content.Label;
                                    if (content.ControlID == Constants.Control.Text || content.ControlID == Constants.Control.Name)
                                    {
                                        tempContent.Height = tempContent.Height == null ? content.Height : tempContent.Height;
                                        tempContent.Width = tempContent.Width == null ? content.Width : tempContent.Width;
                                    }
                                    else
                                    {
                                        tempContent.Height = content.Height;
                                        tempContent.Width = content.Width;
                                    }
                                    if (content.ControlID == Constants.Control.Checkbox || content.ControlID == Constants.Control.Radio)
                                    {
                                        tempContent.Size = _documentContentsRepository.CheckControlSize(content.Size);
                                        tempContent.Height = _documentContentsRepository.GetControlSize(tempContent.Size);
                                        tempContent.Width = _documentContentsRepository.GetControlSize(tempContent.Size);
                                    }
                                    tempContent.GroupName = content.GroupName;
                                    //tempContent.ControlHtmlData = "data-rcptid=" + "\"" + content.RecipientID;
                                    tempContent.ControHtmlData = content.ControlHtmlData;
                                    tempContent.IsControlDeleted = content.IsControlDeleted;
                                    tempContent.MaxLength = content.MaxLength;
                                    tempContent.ControlType = content.ControlType;
                                    tempContent.SenderControlValue = content.SenderControlValue;
                                    tempContent.IsDefaultRequired = content.IsDefaultRequired;
                                    tempContent.IsFixedWidth = content.IsFixedWidth == null ? true : Convert.ToBoolean(content.IsFixedWidth) ? true : false;
                                    _documentContentsRepository.SaveDocumentContent(tempContent, newDocID);

                                    if (content.ControlID == Constants.Control.DropDown)
                                    {
                                        foreach (var cOpt in content.TemplateSelectControlOptions)
                                        {
                                            SelectControlOptions tempOption = new SelectControlOptions();
                                            tempOption.ID = Guid.NewGuid();
                                            if (!ruleIDMapping.ContainsKey(cOpt.ID))
                                                ruleIDMapping.Add(cOpt.ID, tempOption.ID);
                                            tempOption.OptionText = cOpt.OptionText;
                                            tempOption.Order = cOpt.Order;
                                            tempOption.DocumentContentID = newContentID;
                                            _documentContentsRepository.SaveControlOptions(tempOption, newContentID);
                                            if (!string.IsNullOrEmpty(tempContent.ControlValue) && Guid.Parse(tempContent.ControlValue) == cOpt.ID)
                                            {
                                                DocumentContents dc = _documentContentsRepository.GetEntity(newContentID);
                                                if (dc != null)
                                                {
                                                    dc.ControlValue = tempOption.ID.ToString();
                                                    _documentContentsRepository.Save(dc);
                                                }
                                            }
                                        }
                                    }
                                    else if (content.ControlID != Constants.Control.Radio && content.ControlID != Constants.Control.Checkbox)
                                    {
                                        ControlStyle style = new ControlStyle();
                                        style.DocumentContentId = newContentID;
                                        style.FontID = content.TemplateControlStyle.FontID;
                                        style.FontSize = content.TemplateControlStyle.FontSize;
                                        style.FontColor = content.TemplateControlStyle.FontColor;
                                        style.IsBold = content.TemplateControlStyle.IsBold;
                                        style.IsItalic = content.TemplateControlStyle.IsItalic;
                                        style.IsUnderline = content.TemplateControlStyle.IsUnderline;
                                        style.AdditionalValidationName = content.TemplateControlStyle.AdditionalValidationName;
                                        style.AdditionalValidationOption = content.TemplateControlStyle.AdditionalValidationOption;
                                        _documentContentsRepository.SaveControlStyle(style, newContentID);
                                    }
                                    if (!controlIDMapping.ContainsKey(content.ID))
                                        controlIDMapping.Add(content.ID, newContentID);
                                }
                            }
                        }

                        // Labels and HyperControls storing into DocumentContent Table
                        foreach (var content in doc.TemplateDocumentContents.Where(dc => dc.ControlID == Constants.Control.Label || dc.ControlID == Constants.Control.Hyperlink).ToList())
                        {
                            if (content.RecipientID == null && (content.ControlID == Constants.Control.Label || content.ControlID == Constants.Control.Hyperlink))
                            {
                                DocumentContents tempContent = new DocumentContents();
                                newContentID = Guid.NewGuid();
                                tempContent.ID = newContentID;

                                tempContent.ControlID = content.ControlID;
                                tempContent.DocumentID = newDocID;
                                tempContent.ControlHtmlID = content.ControlHtmlID;
                                tempContent.Required = content.Required;
                                tempContent.DocumentPageNo = content.DocumentPageNo;
                                tempContent.PageNo = content.PageNo;
                                tempContent.XCoordinate = content.XCoordinate;
                                tempContent.YCoordinate = content.YCoordinate;
                                tempContent.ZCoordinate = content.ZCoordinate;
                                tempContent.TabIndex = content.TabIndex;
                                tempContent.IntControlId = content.IntControlId;

                                var documentContent = content.IsControlDeleted == false;
                                string DropdownControlValue = string.Empty;

                                tempContent.Label = content.Label;
                                tempContent.Height = content.Height;
                                tempContent.Width = content.Width;

                                tempContent.GroupName = content.GroupName;
                                tempContent.ControHtmlData = content.ControlHtmlData;
                                tempContent.IsControlDeleted = content.IsControlDeleted;
                                tempContent.MaxLength = content.MaxLength;
                                tempContent.ControlType = content.ControlType;
                                tempContent.SenderControlValue = content.SenderControlValue;
                                tempContent.IsFixedWidth = content.IsFixedWidth == null ? true : Convert.ToBoolean(content.IsFixedWidth) ? true : false;
                                _documentContentsRepository.SaveDocumentContent(tempContent, newDocID);
                            }
                        }

                    }

                    //Added by Tparker-S3-1506 Improvement Envelope Settings View Settings - current settings
                    _envelopeRepository.SaveEnvelopeSettingsDetail(userSettings, newEnvelopeID);

                    /*Update recipients for Out of Office Setting if any*/
                    int updatedOutOfOffice = _recipientRepository.UpdateRecipientsforOutOfOffice(newEnvelopeID);

                    //////////// Conditinal Control Mapping ////////////////////////
                    foreach (var control in controlIDMapping)
                    {
                        ConditionalControlsDetailsNew templateControlRules = _conditionalControlRepository.GetAllConditionalControl(Constants.String.RSignStage.InitializeUseTemplate, envelope.StaticTemplateID, control.Key, null);
                        if (templateControlRules == null || templateControlRules.DependentFields == null || templateControlRules.DependentFields.Count < 1)
                            continue;
                        ConditionalControlsDetailsNew tempRule = new ConditionalControlsDetailsNew();
                        tempRule.ID = Guid.NewGuid();
                        tempRule.ControlID = control.Value;
                        tempRule.ControllingFieldID = templateControlRules.ControllingFieldID;
                        tempRule.ControllingConditionID = templateControlRules.ControllingConditionID;
                        tempRule.ControllingSupportText = templateControlRules.ControllingSupportText;
                        tempRule.EnvelopeID = newEnvelopeID;
                        tempRule.GroupCode = templateControlRules.GroupCode;
                        tempRule.EnvelopeStage = Constants.String.RSignStage.InitializeUseTemplate;
                        foreach (var cond in templateControlRules.DependentFields)
                        {
                            if (controlIDMapping.ContainsKey(cond.ControlID))
                            {
                                tempRule.DependentFields.Add(new DependentFieldsPOCO
                                {
                                    ID = Guid.NewGuid(),
                                    ControlID = controlIDMapping[cond.ControlID],
                                    ConditionID = cond.ConditionID != null ? (ruleIDMapping.ContainsKey(cond.ConditionID.Value) ? ruleIDMapping[cond.ConditionID.Value] : cond.ConditionID) : cond.ConditionID,
                                    SupportText = cond.SupportText
                                });
                            }

                        }
                        //newControlRules.Add(tempRule);
                        _conditionalControlRepository.SaveConditionalControl(tempRule);
                    }

                    string envelopeId = newEnvelopeID.ToString();
                    string envelopeFolderUNCPath = _modelHelper.GetEnvelopeDirectoryNew(new Guid(envelopeId), string.Empty);
                    string templateDirectory = _modelHelper.GetTemplateDirectory(template.ID, string.Empty);

                    if (template.EnvelopeTypeId != Constants.EnvelopeType.TemplateRule)
                    {
                        _envelopeHelperMain.PrepareEnvelopeDocumentsFromStaticTemplate(envelope.StaticTemplateID, newEnvelopeID, envelopeFolderUNCPath, templateDirectory);
                    }

                    // Get Static Envelop
                    string tempEnvelopeDirectory = Path.Combine(envelopeFolderUNCPath, envelopeId);
                    Envelope StaticEnvelope = new Envelope();
                    StaticEnvelope = _genericRepository.GetEntity(newEnvelopeID);
                    StaticEnvelope.IpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);

                    EnvelopeFolderMapping envelopeFolderMapping = new EnvelopeFolderMapping();
                    envelopeFolderMapping.EnvelopeId = new Guid(envelopeId);
                    envelopeFolderMapping.ServerName = System.Environment.MachineName;
                    envelopeFolderMapping.StorageLevel = Constants.StorageLevels.Level1;
                    envelopeFolderMapping.UNCPath = envelopeFolderUNCPath;
                    envelopeFolderMapping.ModifiedDate = DateTime.Now;
                    _envelopeRepository.SaveEnvelopeFolderMapping(envelopeFolderMapping);

                    // Create Envelop XML
                    _envelopeHelperMain.SetApiCallFlag();
                    _eSignHelper.SetApiCallFlag();

                    bool isXmlCreate = _eSignHelper.CreateEnvelopeXML(newEnvelopeID, envelopeFolderUNCPath);

                    // Handle Role
                    // Insert Envelope Content XML                       
                    envelopeContent = _envelopeHelperMain.CreateXml(StaticEnvelope);
                    XDocument docs = XDocument.Parse(envelopeContent.ContentXML);
                    envelopeContent.ContentXML = docs.ToString();
                    _envelopeRepository.Save(envelopeContent);

                    var isConfirmationMailSent = false;

                    // Unit Deduction Retrieving Values
                    //string envelopeId = newEnvelopeID.ToString();                    
                    if (!Directory.Exists(tempEnvelopeDirectory))
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["EnvelopeIdMissing"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.returnUrl = "Info/Index";
                        return BadRequest(responseMessage);
                    }
                    foreach (FileInfo folderfiles in new DirectoryInfo(Path.Combine(tempEnvelopeDirectory, "UploadedDocuments")).GetFiles())
                    {
                        fileSize += Convert.ToInt32(folderfiles.Length);
                    }

                    // End Of Unit Deduction Retrieving Values
                    var usersettingsDetails = _settingsRepository.GetEntityForByKeyConfig(StaticEnvelope.UserID, Constants.SettingsKeyConfig.EnablePostSigningLoginPopup);
                    if (usersettingsDetails != null)
                        enablePostSigningLoginPopup = Convert.ToBoolean(usersettingsDetails.OptionValue);
                    if (!string.IsNullOrEmpty(template.PostSigningLandingPage))
                        postSigningPage = template.PostSigningLandingPage;
                    else if (userProfile.CompanyID != null && userProfile.CompanyID != Guid.Empty)
                    {
                        Company senderCompany = new Company();
                        senderCompany = _companyRepository.GetCompanyByID(userProfile.CompanyID);
                        if (senderCompany != null && !string.IsNullOrEmpty(senderCompany.PostSigningLandingPage))
                        {
                            postSigningPage = senderCompany.PostSigningLandingPage;
                        }
                    }

                    if (isConfirmationReq == true)// && IsAllowMultiSigner == false
                    {
                        isConfirmationMailSent = _envelopeHelperMain.InviteSenderConfirmationEmailForStaticTemplate(StaticEnvelope.ID, userProfile, Convert.ToString(newRecipientID), isEnvelopeTosign, _envelopeHelperMain.GetDocumentContents(envelope.ControlCollection), false, envelopeSignDocumentSubmitInfo.SendConfirmationData, envelopeSignDocumentSubmitInfo.RecipientDeliveryMode);
                        //isConfirmationMailSent = objEnvelopeHelperMain.SendConfirmationEmailForStaticTemplate(StaticEnvelope.ID, userProfile, Convert.ToString(newRecipientID), isEnvelopeTosign, objEnvelopeHelperMain.GetDocumentContents(envelope.ControlCollection));
                        if (!envelope.IsEnvelopeComplete)
                        {
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.StatusMessage = "OK";

                            var rectDeliveryModes = envelopeSignDocumentSubmitInfo.RecipientDeliveryMode;

                            if (rectDeliveryModes == "1" || rectDeliveryModes == "2" || rectDeliveryModes == "3" || rectDeliveryModes == "11")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else if (rectDeliveryModes == "8" || rectDeliveryModes == "9" || rectDeliveryModes == "10" || rectDeliveryModes == "12")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "EmailMobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else if (rectDeliveryModes == "4" || rectDeliveryModes == "5" || rectDeliveryModes == "6" || rectDeliveryModes == "7")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "MobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                                                     
                            responseMessage.EnvelopeStatus = _envelopeHelperMain.GetEnvelopeStatus(newEnvelopeID);
                            responseMessage.success = true;
                            responseMessage.data = newEnvelopeID;
                            responseMessage.returnUrl = isConfirmationReq ? "Info/Index" : postSigningPage;
                            responseMessage.postSigningLogin = enablePostSigningLoginPopup;//true;
                            return Ok(responseMessage);
                        }
                    }
                    else if (envelope.IsEnvelopeComplete)// || (isConfirmationReq == true && IsAllowMultiSigner == true)
                    {
                        // Lock Unit
                        isSignDocument = _envelopeHelperMain.InviteSignStaticDocument(_envelopeHelperMain.GetDocumentContents(envelope.ControlCollection), StaticEnvelope, newRecipientID, userSettings.SelectedTimeZone, out errorIfFound);
                        // Send Final Contract Email
                        if (isSignDocument == true)
                        {
                            HttpResponseMessage commitResponse = rpostRestService.CommitStaticLinkUnit(userProfile.EmailID, StaticEnvelope.EDisplayCode, fileSize, 1, base64UserToken, tokenType);
                            var jSonResponse = JsonConvert.DeserializeObject(commitResponse.Content.ReadAsStringAsync().Result).ToString();
                            var ResponseMessage = JsonConvert.DeserializeObject<RestResponseUserInfo>(commitResponse.Content.ReadAsStringAsync().Result);
                            loggerModelNew.Message = "Commit Static Link Unit Response : " + JsonConvert.SerializeObject(ResponseMessage);
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            if (!commitResponse.IsSuccessStatusCode && ResponseMessage.Status != "Success")
                            {
                                //Rollback
                                rpostRestService.RollBackLockUnits(StaticEnvelope.EDisplayCode, base64UserToken, tokenType);
                                //End rollback
                            }
                        }
                    }
                    else
                    {
                        // Update "Envelope.xml" file at temp location. Update "IsEnvelopeCompleted" field.
                        int isContractToGenerateFromImages = Convert.ToInt32(userSettings.FinalContractOptionID) > 0 ? userSettings.FinalContractOptionID : Constants.FinalContractOptions.Aspose;
                        var dictionary = new Dictionary<EnvelopeNodes, string> { { EnvelopeNodes.IsEnvelopeRejected, "true" } };
                        string finalPdfFilePath = string.Empty;
                        _eSignHelper.UpdateEnvelopeXML(newEnvelopeID, dictionary, envelopeFolderUNCPath);
                        var senderDetails = _recipientRepository.GetSenderDetails(StaticEnvelope.ID);
                        var signerDetails = _recipientRepository.GetEntity(newRecipientID);
                        userSettingsForfinalMergePDF = new FinalContractSettings
                        {
                            WatermarkAuthText = string.Empty,
                            WatermarkBackgroundText = string.Empty,
                            FinalContractOptions = isContractToGenerateFromImages,
                            UserTimeZone = userSettings.SelectedTimeZone,
                            IsControlDisplayInTag = false
                        };
                        try
                        {
                            finalPdfFilePath = _apiHelper.finalMergePDFApi(StaticEnvelope, userSettingsForfinalMergePDF, envelopeFolderUNCPath, string.Empty, Convert.ToBoolean(StaticEnvelope.IsSeparateMultipleDocumentsAfterSigningRequired) ? true : false);
                        }
                        catch
                        {
                            _envelopeHelperMain.DeleteContractFileInCaseOfError(StaticEnvelope.ID, envelopeFolderUNCPath);
                            userSettingsForfinalMergePDF.FinalContractOptions = Convert.ToInt32(userSettingsForfinalMergePDF.FinalContractOptions) != Constants.FinalContractOptions.iText ? Constants.FinalContractOptions.iText : Constants.FinalContractOptions.Aspose;
                            finalPdfFilePath = _apiHelper.finalMergePDFApi(StaticEnvelope, userSettingsForfinalMergePDF, envelopeFolderUNCPath, string.Empty, Convert.ToBoolean(StaticEnvelope.IsSeparateMultipleDocumentsAfterSigningRequired) ? true : false);
                        }
                        string subject = Convert.ToString(_appConfiguration["Subject_EnvelopeRejected"]);

                        HttpResponseMessage commitResponse = rpostRestService.CommitStaticLinkUnit(userProfile.EmailID, StaticEnvelope.EDisplayCode, fileSize, 1, base64UserToken, tokenType);
                        var jSonResponse = JsonConvert.DeserializeObject(commitResponse.Content.ReadAsStringAsync().Result).ToString();
                        var ResponseMessage = JsonConvert.DeserializeObject<RestResponseUserInfo>(commitResponse.Content.ReadAsStringAsync().Result);
                        if (!commitResponse.IsSuccessStatusCode && ResponseMessage.Status != "Success")
                        {
                            responseMessage.StatusCode = HttpStatusCode.Forbidden;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.message = "The Process can not be completed. Please Contact Sender.";
                            responseMessage.data = newEnvelopeID;
                            responseMessage.success = false;
                            responseMessage.returnUrl = "Info/Index";
                            loggerModelNew.Message = responseMessage.message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return BadRequest(responseMessage);
                        }
                        else
                        {
                            string comments = string.Empty;
                            if (envelope.Comment != null)
                            {
                                var declineTemplateSetting = JsonConvert.DeserializeObject<List<DeclineTemplateResponses>>(envelope.Comment);
                                if (declineTemplateSetting != null)
                                {
                                    if (declineTemplateSetting.Count > 0)
                                    {
                                        foreach (var item in declineTemplateSetting)
                                            comments = comments + item.ResponseText + ",";
                                        comments = comments.Remove(comments.Length - 1);
                                    }
                                }
                            }
                            _envelopeHelperMain.RejectToMail(!string.IsNullOrEmpty(comments) ? comments : envelope.Comment, "Terminated: " + subject, finalPdfFilePath, StaticEnvelope, signerDetails, senderDetails, "", envelopeFolderUNCPath);
                            _envelopeHelperMain.RejectMailToSender(!string.IsNullOrEmpty(comments) ? comments : envelope.Comment, "Terminated: " + subject, finalPdfFilePath, StaticEnvelope, signerDetails, senderDetails, "", envelopeFolderUNCPath);
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.StatusMessage = "OK";
                            responseMessage.success = true;
                            var rectDeliveryModes = envelopeSignDocumentSubmitInfo.RecipientDeliveryMode;
                            if (rectDeliveryModes == "1" || rectDeliveryModes == "2" || rectDeliveryModes == "3" || rectDeliveryModes == "11")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else if (rectDeliveryModes == "8" || rectDeliveryModes == "9" || rectDeliveryModes == "10" || rectDeliveryModes == "12")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "EmailMobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else if (rectDeliveryModes == "4" || rectDeliveryModes == "5" || rectDeliveryModes == "6" || rectDeliveryModes == "7")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "MobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();

                            responseMessage.EnvelopeStatus = _envelopeHelperMain.GetEnvelopeStatus(newEnvelopeID);
                            responseMessage.data = newEnvelopeID;
                            responseMessage.postSigningLogin = enablePostSigningLoginPopup;//true;
                            responseMessage.returnUrl = isConfirmationReq ? "Info/Index" : postSigningPage;
                            loggerModelNew.Message = "Process completed for Create Static Envelope action.";
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            return Ok(responseMessage);
                        }
                    }

                    string rsignanonymoustoken = string.Empty;
                    APIRecipientEntity recipients = new APIRecipientEntity();
                    if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientEmail))
                    {
                        userProfile = _userRepository.GetUserProfileByEmailID(envelopeSignDocumentSubmitInfo.RecipientEmail);
                        recipients.EmailAddress = envelopeSignDocumentSubmitInfo.RecipientEmail;
                        recipients.Name = envelopeSignDocumentSubmitInfo.RecipientEmail.Split('@')[0].ToString();
                        recipients.Mobile = "";
                        if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientMobile))
                        {
                            recipients.Mobile = envelopeSignDocumentSubmitInfo.RecipientDialCode + envelopeSignDocumentSubmitInfo.RecipientMobile;
                        }
                    }
                    else
                    {
                        userProfile = null;
                        recipients.EmailAddress = "";
                        recipients.Name = "";
                        recipients.Mobile = "";
                        if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientMobile))
                        {
                            recipients.Mobile = envelopeSignDocumentSubmitInfo.RecipientDialCode + envelopeSignDocumentSubmitInfo.RecipientMobile;
                        }
                    }

                    recipients.Order = userProfile != null ? 1 : 0;
                    recipients.EnvelopeID = newEnvelopeID;
                    responseMessage.EnvelopeStatus = _envelopeHelperMain.GetEnvelopeStatus(newEnvelopeID);
                    var rectDeliveryMode = envelopeSignDocumentSubmitInfo.RecipientDeliveryMode;
                    if (rectDeliveryMode == "1" || rectDeliveryMode == "2" || rectDeliveryMode == "3" || rectDeliveryMode == "11")
                    {
                        responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                    }
                    else if (rectDeliveryMode == "8" || rectDeliveryMode == "9" || rectDeliveryMode == "10" || rectDeliveryMode == "12")
                    {
                        responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "EmailMobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                    }
                    else if (rectDeliveryMode == "4" || rectDeliveryMode == "5" || rectDeliveryMode == "6" || rectDeliveryMode == "7")
                    {
                        responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "MobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                    }
                    else
                        responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();


                    lockTaken = true;
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.data = recipients;
                    responseMessage.recpDetail = recipients;
                    responseMessage.returnUrl = isConfirmationReq ? "Info/Index" : postSigningPage;
                    responseMessage.success = true;
                    responseMessage.postSigningLogin = enablePostSigningLoginPopup;//true;
                    responseMessage.postSigningUrl = postSigningPage;
                    loggerModelNew.Message = "Process completed for Create Static Envelope action.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    responseMessage.StatusMessage = "NotAcceptable";
                    responseMessage.message = Convert.ToString(_appConfiguration["RequiredFieldIsMissing"]);
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller CreateInviteSignerEnvelope action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForInitalizeSignDocument), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("CreateStaticEnvelope")]
        [HttpPost]
        public async Task<IActionResult> CreateStaticEnvelope(EnvelopeSignDocumentSubmitInfo envelopeSignDocumentSubmitInfo)
        {
            object _locker = new object();
            HttpResponseMessage responseToClient = new HttpResponseMessage();
            const int passwordKeySize = 128;
            bool lockTaken = false, AvaibleUnits = true, enablePostSigningLoginPopup = true;
            int fileSize = 0;
            string ClientId = "405BDFE4-828B-4656-8795-11FCCA993622";
            Guid globalEnvelopeID = Guid.Empty;
            string postSigningPage = Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]);
            string userToken = string.Empty, currentMethod = "CreateEnvelopByStaticLink";
            loggerModelNew = new LoggerModelNew("", "Envelope", "CreateStaticEnvelope", "Process started for Creating a static envelope using API.", envelopeSignDocumentSubmitInfo.EnvelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                globalEnvelopeID = envelopeSignDocumentSubmitInfo.EnvelopeID != null && envelopeSignDocumentSubmitInfo.EnvelopeID != Guid.Empty ? envelopeSignDocumentSubmitInfo.EnvelopeID : Guid.NewGuid();
                var rpostRestService = new RpostRestService(_appConfiguration);
                string completeEncodedKey = ModelHelper.GenerateKey(passwordKeySize);
                Guid newEnvelopeID = globalEnvelopeID;//Guid.NewGuid();
                Guid newsenderRecID = Guid.NewGuid();
                Guid newRecipientID = Guid.NewGuid();
                Guid newDocID = Guid.NewGuid();
                Guid newContentID = Guid.NewGuid();
                bool isSignDocument = false;
                string errorIfFound = string.Empty;
                InfoResultResonse responseMessage = new InfoResultResonse();
                var isConfirmationReq = false;
                bool IncludeStaticLinksValidation = false;

                EnvelopeContent envelopeContent = new EnvelopeContent();
                var userSettingsForfinalMergePDF = new FinalContractSettings();
                Template template = _genericRepository.GetTemplateEntity(envelopeSignDocumentSubmitInfo.StaticTemplateID);
                UserProfile userProfile = _userRepository.GetUserProfileByUserID(template.UserID);

                string tokenType = "Basic";
                //Call RCS API For Deduct Units And Set AvaibleUnits True
                userToken = string.Format("{0}:{1}", ClientId, userProfile.EmailID);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(userToken);
                string base64UserToken = Convert.ToBase64String(plainTextBytes);

                //Intialized Envelope 
                APIEnvelope envelope = new APIEnvelope();
                envelope.StaticTemplateID = template.ID;
                envelope.DateFormatID = Convert.ToString(template.DateFormatID);
                envelope.ExpiryTypeID = Convert.ToString(template.ExpiryTypeID);
                envelope.PasswordRequiredToSign = template.PasswordReqdtoSign;
                envelope.PasswordRequiredToOpen = template.PasswordReqdtoOpen;
                envelope.PasswordToSign = template.PasswordtoSign;
                envelope.PasswordToOpen = template.PasswordtoOpen;
                envelope.IsTransparencyDocReq = Convert.ToBoolean(template.IsTransparencyDocReq);
                envelope.IsSignerAttachFileReq = (template.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                envelope.IsSignerAttachFileReqNew = template.IsSignerAttachFileReq != null ? template.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                envelope.IpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                envelope.IsEnvelopeComplete = envelopeSignDocumentSubmitInfo.IsEnvelopeComplete;
                envelope.RecipientEmail = envelopeSignDocumentSubmitInfo.RecipientEmail;
                envelope.ControlCollection = envelopeSignDocumentSubmitInfo.IsEnvelopeComplete ? _envelopeRepository.getDocumentContentDetails(envelopeSignDocumentSubmitInfo.ControlCollection) : null;
                envelope.Comment = envelopeSignDocumentSubmitInfo.Comment;
                envelope.IsConfirmationEmailReq = envelopeSignDocumentSubmitInfo.IsConfirmationEmailReq;
                envelope.CultureInfo = template.CultureInfo;
                envelope.CertificateSignature = envelopeSignDocumentSubmitInfo.CertificateSignature;
                envelope.PostSigningLandingPage = template.PostSigningLandingPage;
                envelope.IsStoreSignatureCertificate = template.IsStoreSignatureCertificate;
                envelope.IsEnableFileReview = template.IsEnableFileReview;
                envelope.EnableMessageToMobile = (template.EnableMessageToMobile == null || template.EnableMessageToMobile == false) ? false : true;
                envelope.RestrictRecipientsToContact = template.RestrictRecipientsToContact;

                bool isSignerattachmentProcess = false;
                if (template.IsSignerAttachFileReq > 0)
                {
                    isSignerattachmentProcess = (template.IsAdditionalAttmReq.HasValue && template.IsAdditionalAttmReq == true) ? true : false;
                }
                envelope.IsAdditionalAttmReq = isSignerattachmentProcess; //template.IsAdditionalAttmReq.HasValue ? template.IsAdditionalAttmReq : false;
                envelope.CreatedSource = Constants.CreatedSource.WebApp;

                APIRecipientEntityModel recipientEntity = new APIRecipientEntityModel();
                Guid? reqtemplateKey = Guid.Empty;
                if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.TemplateKey))
                {
                    reqtemplateKey = new Guid(envelopeSignDocumentSubmitInfo.TemplateKey);
                }

                if (!Convert.ToBoolean(envelope.EnableMessageToMobile))
                {
                    recipientEntity = await _recipientRepository.GetRecipientEntity(envelopeSignDocumentSubmitInfo.RecipientEmail, envelopeSignDocumentSubmitInfo.StaticTemplateID, reqtemplateKey);
                }
                else
                {
                    if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientEmail))
                    {
                        recipientEntity = await _recipientRepository.GetRecipientEntity(envelopeSignDocumentSubmitInfo.RecipientEmail, envelopeSignDocumentSubmitInfo.StaticTemplateID, reqtemplateKey);
                    }
                    else
                    {
                        string recipientMobile = envelopeSignDocumentSubmitInfo.RecipientDialCode + envelopeSignDocumentSubmitInfo.RecipientMobile;
                        recipientEntity = await _recipientRepository.GetRecipientEntity("", envelopeSignDocumentSubmitInfo.StaticTemplateID, reqtemplateKey, recipientMobile);
                    }
                }
                if (recipientEntity != null)
                {
                    int waitingTime = Convert.ToInt32(_appConfiguration["StaticUrlWatingTimeInSeconds"]);
                    DateTime currentDate = DateTime.Now;
                    double seconds = 0;
                    if (recipientEntity.CreatedOn != null)
                    {
                        seconds = System.Math.Abs((currentDate - recipientEntity.CreatedOn.Value).TotalSeconds);
                        if (seconds < waitingTime)
                        {
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.success = false;
                            responseMessage.returnUrl = "Info/Index";
                            responseMessage.field = "Staticdelay";
                            responseMessage.data = seconds;
                            return BadRequest(responseMessage);
                            //return Json(new InfoResult { success = false, data = seconds, field = "Staticdelay" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                string displayCode = Convert.ToString(template.EDisplayCode);
                loggerModelNew.Message = "In the process of singing static template";
                rSignLogger.RSignLogInfo(loggerModelNew);
                if (template.IsStatic == false)
                {
                    loggerModelNew.Message = "Document has been expired";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = _envelopeHelperMain.GetLanguageBasedApiMessge(template.UserID, "staticLinkDocExpired");
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }

                if (reqtemplateKey != Guid.Empty)
                {
                    if (template.TemplateKey != reqtemplateKey)
                    {
                        loggerModelNew.Message = "Not a valid link";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.message = _envelopeHelperMain.GetLanguageBasedApiMessge(template.UserID, "staticLinkDocExpired");
                        responseMessage.success = false;
                        responseMessage.returnUrl = "Info/Index";
                        return BadRequest(responseMessage);
                    }
                }

                if (template.TemplateKey != null && reqtemplateKey == Guid.Empty)
                {
                    loggerModelNew.Message = "Not a valid link";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = _envelopeHelperMain.GetLanguageBasedApiMessge(template.UserID, "staticLinkDocExpired");
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    responseMessage.field = "Staticdelay";
                    return BadRequest(responseMessage);
                }

                HttpResponseMessage responseRCS = rpostRestService.RestUserInfo(userProfile.EmailID, base64UserToken, tokenType);
                var jSonResponseRCS = JsonConvert.DeserializeObject(responseRCS.Content.ReadAsStringAsync().Result).ToString();
                RestResponseUserInfo rcsUserInfo = JsonConvert.DeserializeObject<RestResponseUserInfo>(jSonResponseRCS);
                if (rcsUserInfo.ResultContent == null || rcsUserInfo.ResultContent.Plan == null || rcsUserInfo.ResultContent.Customer == null)
                {
                    loggerModelNew.Message = "User is not associated with either plan or result-content for user is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = "Process can not be completed. Please contact sender";
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }
                else if ((rcsUserInfo.ResultContent.Plan.AllowedUnits - rcsUserInfo.ResultContent.Plan.UnitsSent) <= 0)
                {
                    AvaibleUnits = false;
                }
                if (AvaibleUnits == false)
                {
                    loggerModelNew.Message = "Not have enough units";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = "Insufficient units." + System.Environment.NewLine + "&nbsp &nbsp &nbsp Please contact sender";
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }
                //This changes is for static link template US1348

                //envelope.PasswordRequiredToSign = envelope.PasswordRequiredToOpen = false;
                //envelope.PasswordToSign = envelope.PasswordToOpen = null;

                if (envelope.DateFormatID != null && envelope.ExpiryTypeID != null)
                {
                    if (!_masterDataRepository.ValidateDateFormatId(new Guid(envelope.DateFormatID)))
                    {
                        responseMessage.StatusCode = HttpStatusCode.Forbidden;
                        responseMessage.StatusMessage = "Forbidden";
                        responseMessage.success = false;
                        responseMessage.message = Convert.ToString(_appConfiguration["DateFormatIdInvalid"]);
                        return BadRequest(responseMessage);
                    }
                    if (!_masterDataRepository.ValidateExpiryTypeId(new Guid(envelope.ExpiryTypeID)))
                    {
                        responseMessage.StatusCode = HttpStatusCode.Forbidden;
                        responseMessage.StatusMessage = "Forbidden";
                        responseMessage.success = false;
                        responseMessage.message = Convert.ToString(_appConfiguration["ExpiryTypeIdInvalid"]);
                        return BadRequest(responseMessage);
                    }

                    /* Get User Settings */
                    APISettings apiSettings = _settingsRepository.GetEntityByParam(template.UserID, string.Empty, Constants.String.SettingsType.User);
                    var userSettings = _eSignHelper.TransformSettingsDictionaryToEntity(apiSettings);
                    userSettings.UserID = template.UserID;
                    _eSignHelper.SetApiCallFlag();
                    responseMessage.AttachSignedPdfID = userSettings.AttachSignedPdfID;
                    var RequireSignerConfirmationSetting = _settingsRepository.GetEntityForByKeyConfig((Guid)userSettings.UserID, Constants.SettingsKeyConfig.IncludeStaticTemplates);
                    if (RequireSignerConfirmationSetting != null)
                        IncludeStaticLinksValidation = Convert.ToBoolean(RequireSignerConfirmationSetting.OptionValue);

                    bool isEnvelopeTosign = envelope.IsEnvelopeComplete;
                    isConfirmationReq = envelope.IsConfirmationEmailReq && !IncludeStaticLinksValidation;

                    //Insert  Envelope
                    Envelope envlp = new Envelope();
                    envlp.ID = newEnvelopeID;
                    envlp.EnvelopeCode = _genericRepository.GetMaxEnvelopeCode() + 1;
                    envlp.UserID = template.UserID;
                    envlp.DateFormatID = new Guid(envelope.DateFormatID);
                    envlp.ExpiryTypeID = new Guid(envelope.ExpiryTypeID);
                    envlp.IsTransparencyDocReq = envelope.IsTransparencyDocReq;
                    envlp.IsSignerAttachFileReq = envelope.IsSignerAttachFileReqNew;
                    envlp.IsFinalCertificateReq = template.IsFinalCertificateReq;
                    envlp.IsAttachXML = Convert.ToBoolean(template.IsAttachXML);
                    envlp.IsPrivateMode = Convert.ToBoolean(template.IsPrivateMode);
                    envlp.IsSeparateMultipleDocumentsAfterSigningRequired = Convert.ToBoolean(template.IsSeparateMultipleDocumentsAfterSigningRequired);
                    envlp.CultureInfo = template.CultureInfo;
                    envlp.IsStatic = true;
                    envlp.IsDefaultSignatureForStaticTemplate = template.IsDefaultSignatureForStaticTemplate;
                    envlp.ReferenceCode = template.ReferenceCode;
                    envlp.ReferenceEmail = template.ReferenceEmail;
                    envlp.IsStoreOriginalDocument = envelope.IsStoreOriginalDocument;
                    envlp.IsStoreSignatureCertificate = envelope.IsStoreSignatureCertificate;
                    envlp.IsDisclaimerInCertificate = Convert.ToBoolean(userSettings.IsDisclaimerInCertificate);
                    envlp.DisclaimerText = userSettings.Disclaimer;
                    bool isSignerattachProcess = false;
                    if (envelope.IsSignerAttachFileReqNew > 0)
                    {
                        isSignerattachmentProcess = envelope.IsAdditionalAttmReq != null ? Convert.ToBoolean(envelope.IsAdditionalAttmReq) : Convert.ToBoolean(template.IsAdditionalAttmReq);
                    }
                    envlp.IsAdditionalAttmReq = isSignerattachProcess;

                    if (template.TemplateKey != Guid.Empty)
                    {
                        envlp.TemplateKey = template.TemplateKey;
                    }
                    else
                    {
                        envlp.TemplateKey = Constants.StatusCode.Envelope.TemplateKey;
                    }

                    if (!isConfirmationReq)
                    {
                        envlp.StatusID = envelope.IsEnvelopeComplete == true ? Constants.StatusCode.Envelope.Completed : Constants.StatusCode.Envelope.Terminated;
                    }
                    else
                    {
                        envlp.TemplateDescription = envelope.Comment;
                        envlp.StatusID = Constants.StatusCode.Envelope.Waiting_For_Signature;
                    }
                    envlp.SigningCertificateName = Convert.ToString(_appConfiguration["SigningCertificateName"]);
                    envlp.CreatedDateTime = DateTime.Now;
                    envlp.ModifiedDateTime = DateTime.Now;
                    envlp.Subject = template.TemplateName;
                    envlp.Message = template.Message;
                    envlp.IsEnvelope = true;
                    envlp.IsTemplateDeleted = false;
                    envlp.IsTemplateEditable = true;
                    envlp.IsEnvelopeComplete = true;
                    envlp.EDisplayCode = EnvelopeHelperMain.TakeUniqueDisplayCode();
                    envlp.IsEnvelopePrepare = true;
                    envlp.IpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                    envlp.SharedTemplateID = envelope.StaticTemplateID;
                    envlp.PasswordReqdtoOpen = envelope.PasswordRequiredToOpen;
                    envlp.PasswordReqdtoSign = envelope.PasswordRequiredToSign;
                    if (envelope.PasswordRequiredToOpen)
                        envlp.PasswordtoOpen = ModelHelper.Decrypt(envelope.PasswordToOpen, template.PasswordKey, (int)template.PasswordKeySize);
                    if (envelope.PasswordRequiredToSign)
                        envlp.PasswordtoSign = ModelHelper.Decrypt(envelope.PasswordToSign, template.PasswordKey, (int)template.PasswordKeySize);
                    envlp.PasswordKey = template.PasswordKey;
                    envlp.PasswordKeySize = template.PasswordKeySize;
                    envlp.IsPasswordMailToSigner = template.IsPasswordMailToSigner;
                    envlp.IsRandomPassword = template.IsRandomPassword;
                    envlp.AccessAuthType = template.AccessAuthType;
                    envlp.HeaderFooterOption = userSettings.HeaderFooterSettingID;
                    envlp.ElectronicSignIndicationOptionID = userSettings.ElectronicSignIndicationSelectedID;
                    envlp.PostSigningLandingPage = envelope.PostSigningLandingPage;
                    envlp.IpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                    envlp.IsEnableFileReview = envelope.IsEnableFileReview;
                    envlp.IsRule = false;
                    envlp.CreatedSource = Constants.CreatedSource.WebApp;
                    envlp.ISSendReminderTillExpiration = (template.ISSendReminderTillExpiration == null ? (userSettings.IsSendReminderTillExpiration ? 1 : 0) : template.ISSendReminderTillExpiration);
                    envlp.IsEnvelopeExpirationRemindertoSender = (template.IsEnvelopeExpirationRemindertoSender == null ? (userSettings.IsEnvelopeExpirationRemindertoSender ? 1 : 0) : template.IsEnvelopeExpirationRemindertoSender);
                    envlp.SendReminderTillExpiration = (template.SendReminderTillExpiration == null
                        ? (userSettings.SendReminderTillExpirationOptionSelected == null ? Constants.DropdownFieldKeyType.OneEmailperEnvelope : userSettings.SendReminderTillExpirationOptionSelected)
                        : template.SendReminderTillExpiration);
                    envlp.EnvelopeExpirationRemindertoSender = (template.EnvelopeExpirationRemindertoSender == null
                        ? userSettings.EnvelopeExpirationRemindertoSenderDropdownSelected == null ? Constants.ReminderDropdownOptions.Days.ToString() : userSettings.EnvelopeExpirationRemindertoSenderDropdownSelected.ToString()
                        : template.EnvelopeExpirationRemindertoSender);
                    envlp.EnvelopeExpirationRemindertoSenderReminderDays = (template.EnvelopeExpirationRemindertoSenderReminderDays == null ? userSettings.EnvelopeExpirationRemindertoSenderReminderDays : template.EnvelopeExpirationRemindertoSenderReminderDays);
                    envlp.SendConfirmationEmail = isConfirmationReq;
                    envlp.IsEnableAutoFillTextControls = template.IsEnableAutoFillTextControls;
                    envlp.EnableMessageToMobile = (template.EnableMessageToMobile == null || template.EnableMessageToMobile == false) ? false : true;
                    envlp.RestrictRecipientsToContact = template.RestrictRecipientsToContact;
                    envlp.ReVerifySignerDocumentSubmit = template.ReVerifySignerStaticTemplate;
                    envlp.IsSendMessageCodetoAvailableEmailorMobile = envelopeSignDocumentSubmitInfo.IsSendMessageCodetoAvailableEmailorMobile;
                    // envlp.EnableRecipientLanguage = envelope.EnableRecipientLanguage;

                    var watermarkStamp = _envelopeHelperMain.GetWatermarkStamp(envlp.UserID);
                    if (watermarkStamp != null)
                    {
                        envlp.IsWaterMark = watermarkStamp.IsWaterMark;
                        envlp.WatermarkTextForSender = watermarkStamp.WatermarkTextForSender;
                        envlp.WatermarkTextForOther = watermarkStamp.WatermarkTextForOther;
                    }
                    _genericRepository.SetInitializeEnvelopeFlag();
                    _genericRepository.Save(envlp);

                    //Insert Sender Recipient
                    Recipients senderRec = new Recipients();
                    senderRec.ID = newsenderRecID;
                    senderRec.EnvelopeID = newEnvelopeID;
                    senderRec.RecipientTypeID = Constants.RecipientType.Sender;
                    senderRec.Name = userProfile.FirstName + " " + userProfile.LastName;
                    senderRec.EmailAddress = userProfile.EmailID;
                    responseMessage.InfoSenderEmail = senderRec.EmailAddress;
                    senderRec.Order = null;
                    senderRec.CreatedDateTime = DateTime.Now;
                    senderRec.CultureInfo = template.CultureInfo;
                    _recipientRepository.Save(senderRec);

                    //Insert  Sender Signer Status
                    SignerStatus senderSignerStatus = new SignerStatus();
                    senderSignerStatus.ID = Guid.NewGuid();
                    senderSignerStatus.RecipientID = newsenderRecID;
                    senderSignerStatus.StatusID = Constants.StatusCode.Signer.Sent;
                    senderSignerStatus.RejectionRemarks = null;
                    senderSignerStatus.CreatedDateTime = DateTime.Now;
                    senderSignerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                    senderSignerStatus.DelegateTo = null;
                    _recipientRepository.Save(senderSignerStatus);

                    //Insert Sender Recipient Details
                    _recipientRepository.SaveRecipientDetailOnSend(newsenderRecID, Constants.StatusCode.Signer.Sent, envelope.IpAddress);

                    //Insert  Recipient
                    Recipients recipient = new Recipients();
                    recipient.ID = newRecipientID;//Guid.NewGuid();
                    recipient.EnvelopeID = newEnvelopeID;
                    recipient.RecipientTypeID = Constants.RecipientType.Signer;
                    recipient.Name = envelope.RecipientEmail;//tokenRepository.GetUserProfileNameByEmail(userEmailAddress);
                    recipient.EmailAddress = envelope.RecipientEmail;
                    recipient.Order = null;
                    recipient.CreatedDateTime = DateTime.Now;
                    recipient.IsReviewed = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.IsReviewed) ? Convert.ToInt32(envelopeSignDocumentSubmitInfo.IsReviewed) : 0;
                    recipient.CultureInfo = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo;
                    //recipient.ReminderType = null;
                    if (Convert.ToBoolean(envelope.EnableMessageToMobile) && (envelopeSignDocumentSubmitInfo.RecipientDeliveryMode != "1" && envelopeSignDocumentSubmitInfo.RecipientDeliveryMode != "11"))
                    {
                        recipient.DeliveryMode = Convert.ToInt32(envelopeSignDocumentSubmitInfo.RecipientDeliveryMode);
                        recipient.DialCode = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientDialCode) ? envelopeSignDocumentSubmitInfo.RecipientDialCode : "";
                        recipient.CountryCode = (envelopeSignDocumentSubmitInfo.RecipientCountryCode != null && envelopeSignDocumentSubmitInfo.RecipientCountryCode != "") ? envelopeSignDocumentSubmitInfo.RecipientCountryCode.ToUpper() : envelopeSignDocumentSubmitInfo.RecipientCountryCode;
                        recipient.Mobile = envelopeSignDocumentSubmitInfo.RecipientMobile;
                       // recipient.ReminderType = Convert.ToInt32(envelopeSignDocumentSubmitInfo.RecipientDeliveryMode);
                    }
                    else
                    {
                        recipient.DeliveryMode = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientDeliveryMode) ? Convert.ToInt32(envelopeSignDocumentSubmitInfo.RecipientDeliveryMode) : 1;
                        recipient.DialCode = null;
                        recipient.CountryCode = null;
                        recipient.Mobile = null;
                       // recipient.ReminderType = recipient.DeliveryMode;
                    }
                    envlp.Recipients.Add(recipient);

                    foreach (var recp in envlp.Recipients)
                    {
                        _recipientRepository.Save(recp);
                    }

                    //To store selected Template/Rule Reference For Envelope (S3-1946).
                    List<EnvelopeMapping> envelopeMappingsList = new List<EnvelopeMapping>();
                    if (template.ID != Guid.Empty)
                    {
                        EnvelopeTemplateMappingDetails mappingDetails = new EnvelopeTemplateMappingDetails();
                        EnvelopeMapping envelopeMapping = new EnvelopeMapping();
                        mappingDetails.EnvelopeId = newEnvelopeID;
                        mappingDetails.TemplateId = new List<Guid> { template.ID };
                        mappingDetails.UserId = template.UserID;
                        envelopeMapping.TemplateId = template.ID;
                        envelopeMapping.TemplateCode = template.TemplateCode;
                        envelopeMapping.TemplateName = template.TemplateName;
                        envelopeMappingsList.Add(envelopeMapping);
                        mappingDetails.EnvelopeTypeId = template.EnvelopeTypeId;
                        mappingDetails.envelopeMappingsToTemp = envelopeMappingsList;
                        _envelopeRepository.SaveEnvelopeTemplateMapping(mappingDetails);
                    }

                    //Insert  Recipient Signer Status
                    if (envelope.IsEnvelopeComplete && !isConfirmationReq)
                    {
                        SignerStatus signerStatus = new SignerStatus();
                        signerStatus.ID = Guid.NewGuid();
                        signerStatus.RecipientID = newRecipientID;
                        signerStatus.CreatedDateTime = DateTime.Now;
                        signerStatus.StatusID = Constants.StatusCode.Signer.Signed;
                        signerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                        _recipientRepository.Save(signerStatus);
                    }
                    else if (!isConfirmationReq)
                    {
                        SignerStatus signerStatus = new SignerStatus();
                        signerStatus.ID = Guid.NewGuid();
                        signerStatus.RecipientID = newRecipientID;
                        signerStatus.CreatedDateTime = DateTime.Now.AddSeconds(1);
                        signerStatus.StatusID = Constants.StatusCode.Signer.Rejected;
                        signerStatus.DeclineReasonID = envelopeSignDocumentSubmitInfo.DeclineReasonID;
                        signerStatus.RejectionRemarks = envelope.Comment;
                        signerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                        _recipientRepository.Save(signerStatus);
                    }
                    //Insert  Recipient Details
                    if (envelope.IsEnvelopeComplete && !isConfirmationReq)
                        _recipientRepository.SaveRecipientDetailOnSend(newRecipientID, Constants.StatusCode.Signer.Signed, envelope.IpAddress);
                    else if (!isConfirmationReq)
                        _recipientRepository.SaveRecipientDetailForEmailConfirm(newRecipientID, Constants.StatusCode.Signer.Rejected, envelope.IpAddress);
                    else
                        _recipientRepository.SaveRecipientDetailForEmailConfirm(newRecipientID, Constants.StatusCode.Signer.AwaitingConfirmation, envelope.IpAddress);

                    Dictionary<Guid, Guid> roleToRecipientMapping = new Dictionary<Guid, Guid>();
                    Guid firstRecipientID = Guid.Empty;

                    Dictionary<Guid, Guid> controlIDMapping = new Dictionary<Guid, Guid>();
                    Dictionary<Guid, Guid> ruleIDMapping = new Dictionary<Guid, Guid>();
                    Documents tempDoc = new Documents();
                    //S3-1088: Added code to set the time zone as per user setting for DateTimeStamp Control 
                    string dateTimeStampControlValue = string.Empty;
                    if (template.DateFormatID != null)
                    {
                        dateTimeStampControlValue = _envelopeHelperMain.GetDateTimeStampControlValue(template.DateFormatID, string.IsNullOrEmpty(userSettings.SelectedTimeZone) ? "UTC" : userSettings.SelectedTimeZone);
                    }
                    // Insert Document
                    string[] validFileTypes = {
                                                      "docx", "pdf", "doc", "xls", "xlsx", "htm", "html","txt", "ppt", "pptx", "bmp", "gif", "ico", "jpg", "jpeg", "png", "tif", "tiff",
                                                      "DOCX", "PDF", "DOC", "XLS", "XLSX", "HTM", "HTML","TXT", "PPT", "PPTX", "BMP", "GIF", "ICO", "JPG", "JPEG", "PNG", "TIF", "TIFF"
                                                  };
                    bool isValidType = false;
                    foreach (var doc in template.TemplateDocuments)
                    {
                        newDocID = Guid.NewGuid();
                        if (template.EnvelopeTypeId != Constants.EnvelopeType.TemplateRule)
                        {
                            isValidType = false;
                            tempDoc = new Documents();
                            tempDoc.ID = newDocID;
                            tempDoc.EnvelopeID = newEnvelopeID;
                            tempDoc.DocumentName = doc.DocumentName;
                            tempDoc.UploadedDateTime = DateTime.Now;
                            tempDoc.TemplateDocumentName = null;
                            tempDoc.Order = doc.Order;
                            tempDoc.DocumentSource = doc.DocumentSource;
                            string ext = Path.GetExtension(doc.DocumentName);
                            for (int j = 0; j < validFileTypes.Length; j++)
                            {
                                if (ext == "." + validFileTypes[j])
                                {
                                    isValidType = true;
                                    break;
                                }
                            }
                            if (isValidType)
                            {
                                tempDoc.ActionType = Constants.ActionTypes.Sign;
                            }
                            else
                            {
                                tempDoc.ActionType = Constants.ActionTypes.Review;
                            }
                            _documentRepository.Save(tempDoc);
                        }
                        else
                        {
                            newDocID = new Guid("00000000-0000-0000-0000-000000000000");
                        }
                        int defaultSignatureCount = 0;
                        // Insert Document Contents
                        foreach (var content in doc.TemplateDocumentContents)
                        {
                            DocumentContents tempContent = new DocumentContents();
                            newContentID = Guid.NewGuid();
                            tempContent.ID = newContentID;
                            if (content.ControlID == Constants.Control.Radio)
                            {
                                if (!ruleIDMapping.ContainsKey(content.ID))
                                    ruleIDMapping.Add(content.ID, newContentID);
                            }
                            tempContent.ControlID = content.ControlID;
                            tempContent.DocumentID = newDocID;
                            tempContent.RecipientID = newRecipientID;
                            tempContent.RecipientName = envelope.RecipientEmail;
                            tempContent.ControlHtmlID = content.ControlHtmlID;
                            tempContent.Required = content.Required;
                            tempContent.DocumentPageNo = content.DocumentPageNo;
                            tempContent.PageNo = content.PageNo;
                            tempContent.XCoordinate = content.XCoordinate;
                            tempContent.YCoordinate = content.YCoordinate;
                            tempContent.ZCoordinate = content.ZCoordinate;
                            tempContent.TabIndex = content.TabIndex;
                            tempContent.IntControlId = content.IntControlId;

                            var documentContent = content.IsControlDeleted == false;
                            string DropdownControlValue = string.Empty;
                            if (envelope.ControlCollection != null)
                            {
                                var documentContentWithValue = envelope.ControlCollection.FirstOrDefault(d => d.ID == content.ID);
                                if (documentContentWithValue != null)
                                {
                                    if (documentContentWithValue.ControlHtmlID == Constants.SignatureType.Auto.ToString().ToUpper() || documentContentWithValue.ControlHtmlID == Constants.SignatureType.Hand.ToString().ToUpper() || documentContentWithValue.ControlHtmlID == Constants.SignatureType.UploadSignature.ToString().ToUpper())
                                    {
                                        tempContent.ControlValue = "Signed";
                                        if (documentContentWithValue.ControlValue != null)
                                        {
                                            string signValue = documentContentWithValue.ControlValue;
                                            signValue = signValue.Replace("data:image/png;base64,", string.Empty);
                                            byte[] signByte = Convert.FromBase64String(signValue);
                                            // signature stored in new table 
                                            if (envelope.CertificateSignature != "" && content.ControlHtmlID == envelope.CertificateSignature)
                                            {
                                                var signerSignature = new SignerSignature
                                                {
                                                    ID = Guid.NewGuid(),
                                                    RecipientID = newRecipientID,
                                                    Signature = signByte,
                                                    SignatureTypeId = Guid.Parse(documentContentWithValue.ControlHtmlID)
                                                };
                                                _recipientRepository.Save(signerSignature);
                                                defaultSignatureCount = 1;
                                            }
                                            tempContent.SignatureControlValue = signByte;
                                        }
                                    }
                                    else
                                    {
                                        if (documentContentWithValue.ControlID == Constants.Control.DateTimeStamp && !string.IsNullOrEmpty(dateTimeStampControlValue))
                                        {
                                            documentContentWithValue.ControlValue = dateTimeStampControlValue;
                                        }
                                        tempContent.ControlValue = documentContentWithValue.ControlValue;
                                        if (content.ControlID == Constants.Control.Text && !Convert.ToBoolean(content.IsFixedWidth))
                                        {
                                            string controlContentType = EnvelopeHelper.GetTextType((Guid)content.ControlType);
                                            if (controlContentType.ToLower() == "text" || controlContentType.ToLower() == "alphabet" || controlContentType.ToLower() == "numeric" || controlContentType.ToLower() == "email")
                                            {
                                                if (documentContentWithValue.Height != null && documentContentWithValue.Height != 0)
                                                {
                                                    tempContent.Height = documentContentWithValue.Height;
                                                }
                                                if (documentContentWithValue.Width != null && documentContentWithValue.Width != 0)
                                                {
                                                    tempContent.Width = documentContentWithValue.Width;
                                                }
                                                if (documentContentWithValue.ZCoordinate != null && documentContentWithValue.ZCoordinate != 0)
                                                {
                                                    tempContent.ZCoordinate = documentContentWithValue.ZCoordinate;
                                                }
                                            }
                                        }
                                        if (content.ControlID == Constants.Control.Name && !Convert.ToBoolean(content.IsFixedWidth))
                                        {
                                            if (documentContentWithValue.Height != null && documentContentWithValue.Height != 0)
                                            {
                                                tempContent.Height = documentContentWithValue.Height;
                                            }
                                            if (documentContentWithValue.Width != null && documentContentWithValue.Width != 0)
                                            {
                                                tempContent.Width = documentContentWithValue.Width;
                                            }
                                            if (documentContentWithValue.ZCoordinate != null && documentContentWithValue.ZCoordinate != 0)
                                            {
                                                tempContent.ZCoordinate = documentContentWithValue.ZCoordinate;
                                            }
                                        }
                                    }
                                }
                                else if (documentContentWithValue == null && envelope.ControlCollection.Any(dc => dc.ID == Guid.Empty && !string.IsNullOrEmpty(dc.ControlValue)
                                        && (dc.ControlHtmlID == Constants.SignatureType.Auto.ToString().ToUpper() || dc.ControlHtmlID == Constants.SignatureType.Hand.ToString().ToUpper() || dc.ControlHtmlID == Constants.SignatureType.UploadSignature.ToString().ToUpper())))
                                {
                                    foreach (var Cval in envelope.ControlCollection)
                                    {
                                        if (!string.IsNullOrEmpty(Cval.ControlValue)
                                    && (Cval.ControlHtmlID == Constants.SignatureType.Auto.ToString().ToUpper() || Cval.ControlHtmlID == Constants.SignatureType.Hand.ToString().ToUpper() || Cval.ControlHtmlID == Constants.SignatureType.UploadSignature.ToString().ToUpper()) && defaultSignatureCount == 0)
                                        {
                                            string signValue = Cval.ControlValue;
                                            signValue = signValue.Replace("data:image/png;base64,", string.Empty);
                                            byte[] signByte = Convert.FromBase64String(signValue);
                                            // signature default signature 

                                            var signerSignature = new SignerSignature
                                            {
                                                ID = Guid.NewGuid(),
                                                RecipientID = newRecipientID,
                                                Signature = signByte,
                                                SignatureTypeId = Guid.Parse(Cval.ControlHtmlID)
                                            };
                                            _recipientRepository.Save(signerSignature);
                                            defaultSignatureCount = 1;
                                        }
                                    }
                                }
                            }
                            tempContent.Label = content.Label;
                            //tempContent.Height = content.Height;
                            //tempContent.Width = content.Width;
                            if (content.ControlID == Constants.Control.Text || content.ControlID == Constants.Control.Name)
                            {
                                tempContent.Height = tempContent.Height == null ? content.Height : tempContent.Height;
                                tempContent.Width = tempContent.Width == null ? content.Width : tempContent.Width;
                            }
                            else
                            {
                                tempContent.Height = content.Height;
                                tempContent.Width = content.Width;
                            }
                            if (content.ControlID == Constants.Control.Checkbox || content.ControlID == Constants.Control.Radio)
                            {
                                tempContent.Size = _documentContentsRepository.CheckControlSize(content.Size);
                                tempContent.Height = _documentContentsRepository.GetControlSize(tempContent.Size);
                                tempContent.Width = _documentContentsRepository.GetControlSize(tempContent.Size);
                            }
                            tempContent.GroupName = content.GroupName;
                            //tempContent.ControlHtmlData = "data-rcptid=" + "\"" + content.RecipientID;
                            tempContent.ControHtmlData = content.ControlHtmlData;
                            tempContent.IsControlDeleted = content.IsControlDeleted;
                            tempContent.MaxLength = content.MaxLength;
                            tempContent.ControlType = content.ControlType;
                            tempContent.SenderControlValue = content.SenderControlValue;
                            tempContent.IsFixedWidth = content.IsFixedWidth == null ? true : Convert.ToBoolean(content.IsFixedWidth) ? true : false;
                            _documentContentsRepository.SaveDocumentContent(tempContent, newDocID);

                            if (content.ControlID == Constants.Control.DropDown)
                            {
                                foreach (var cOpt in content.TemplateSelectControlOptions)
                                {
                                    SelectControlOptions tempOption = new SelectControlOptions();
                                    tempOption.ID = Guid.NewGuid();
                                    if (!ruleIDMapping.ContainsKey(cOpt.ID))
                                        ruleIDMapping.Add(cOpt.ID, tempOption.ID);
                                    tempOption.OptionText = cOpt.OptionText;
                                    tempOption.Order = cOpt.Order;
                                    tempOption.DocumentContentID = newContentID;
                                    _documentContentsRepository.SaveControlOptions(tempOption, newContentID);
                                    if (!string.IsNullOrEmpty(tempContent.ControlValue) && Guid.Parse(tempContent.ControlValue) == cOpt.ID)
                                    {
                                        DocumentContents dc = _documentContentsRepository.GetEntity(newContentID);
                                        if (dc != null)
                                        {
                                            dc.ControlValue = tempOption.ID.ToString();
                                            _documentContentsRepository.Save(dc);
                                        }
                                    }
                                }
                            }
                            else if (content.ControlID != Constants.Control.Radio && content.ControlID != Constants.Control.Checkbox)
                            {
                                ControlStyle style = new ControlStyle();
                                style.DocumentContentId = newContentID;
                                style.FontID = content.TemplateControlStyle.FontID;
                                style.FontSize = content.TemplateControlStyle.FontSize;
                                style.FontColor = content.TemplateControlStyle.FontColor;
                                style.IsBold = content.TemplateControlStyle.IsBold;
                                style.IsItalic = content.TemplateControlStyle.IsItalic;
                                style.IsUnderline = content.TemplateControlStyle.IsUnderline;
                                style.AdditionalValidationName = content.TemplateControlStyle.AdditionalValidationName;
                                style.AdditionalValidationOption = content.TemplateControlStyle.AdditionalValidationOption;
                                _documentContentsRepository.SaveControlStyle(style, newContentID);
                            }
                            if (!controlIDMapping.ContainsKey(content.ID))
                                controlIDMapping.Add(content.ID, newContentID);
                        }
                    }

                    //Added by Tparker-S3-1506 Improvement Envelope Settings View Settings - current settings
                    _envelopeRepository.SaveEnvelopeSettingsDetail(userSettings, newEnvelopeID);

                    /*Update recipients for Out of Office Setting if any*/
                    int updatedOutOfOffice = _recipientRepository.UpdateRecipientsforOutOfOffice(newEnvelopeID);

                    //////////// Conditinal Control Mapping ////////////////////////
                    foreach (var control in controlIDMapping)
                    {
                        ConditionalControlsDetailsNew templateControlRules = _conditionalControlRepository.GetAllConditionalControl(Constants.String.RSignStage.InitializeUseTemplate, envelope.StaticTemplateID, control.Key, null);
                        if (templateControlRules == null || templateControlRules.DependentFields == null || templateControlRules.DependentFields.Count < 1)
                            continue;
                        ConditionalControlsDetailsNew tempRule = new ConditionalControlsDetailsNew();
                        tempRule.ID = Guid.NewGuid();
                        tempRule.ControlID = control.Value;
                        tempRule.ControllingFieldID = templateControlRules.ControllingFieldID;
                        tempRule.ControllingConditionID = templateControlRules.ControllingConditionID;
                        tempRule.ControllingSupportText = templateControlRules.ControllingSupportText;
                        tempRule.EnvelopeID = newEnvelopeID;
                        tempRule.GroupCode = templateControlRules.GroupCode;
                        tempRule.EnvelopeStage = Constants.String.RSignStage.InitializeUseTemplate;
                        foreach (var cond in templateControlRules.DependentFields)
                        {
                            if (controlIDMapping.ContainsKey(cond.ControlID))
                            {
                                tempRule.DependentFields.Add(new DependentFieldsPOCO
                                {
                                    ID = Guid.NewGuid(),
                                    ControlID = controlIDMapping[cond.ControlID],
                                    ConditionID = cond.ConditionID != null ? (ruleIDMapping.ContainsKey(cond.ConditionID.Value) ? ruleIDMapping[cond.ConditionID.Value] : cond.ConditionID) : cond.ConditionID,
                                    SupportText = cond.SupportText
                                });
                            }

                        }
                        //newControlRules.Add(tempRule);
                        _conditionalControlRepository.SaveConditionalControl(tempRule);
                    }

                    string envelopeId = newEnvelopeID.ToString();
                    string envelopeFolderUNCPath = _modelHelper.GetEnvelopeDirectoryNew(new Guid(envelopeId), string.Empty);
                    string templateDirectory = _modelHelper.GetTemplateDirectory(template.ID, string.Empty);


                    if (template.EnvelopeTypeId != Constants.EnvelopeType.TemplateRule)
                    {
                        _envelopeHelperMain.PrepareEnvelopeDocumentsFromStaticTemplate(envelope.StaticTemplateID, newEnvelopeID, envelopeFolderUNCPath, templateDirectory);
                    }

                    // Get Static Envelop
                    string tempEnvelopeDirectory = Path.Combine(envelopeFolderUNCPath, envelopeId);
                    Envelope StaticEnvelope = new Envelope();
                    StaticEnvelope = _genericRepository.GetEntity(newEnvelopeID);
                    StaticEnvelope.IpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);

                    // Create Envelop XML
                    _envelopeHelperMain.SetApiCallFlag();
                    _eSignHelper.SetApiCallFlag();

                    bool isXmlCreate = _eSignHelper.CreateEnvelopeXML(newEnvelopeID, envelopeFolderUNCPath);

                    EnvelopeFolderMapping envelopeFolderMapping = new EnvelopeFolderMapping();
                    envelopeFolderMapping.EnvelopeId = newEnvelopeID;
                    envelopeFolderMapping.ServerName = System.Environment.MachineName;
                    envelopeFolderMapping.StorageLevel = Constants.StorageLevels.Level1;
                    envelopeFolderMapping.UNCPath = envelopeFolderUNCPath;
                    envelopeFolderMapping.ModifiedDate = DateTime.Now;
                    _envelopeRepository.SaveEnvelopeFolderMapping(envelopeFolderMapping);

                    // Handle Role
                    // Insert Envelope Content XML
                    envelopeContent = _envelopeHelperMain.CreateXml(StaticEnvelope);
                    XDocument docs = XDocument.Parse(envelopeContent.ContentXML);
                    envelopeContent.ContentXML = docs.ToString();
                    _envelopeRepository.Save(envelopeContent);

                    var isConfirmationMailSent = false;

                    // Unit Deduction Retrieving Values
                    //string envelopeId = newEnvelopeID.ToString();                    
                    if (!Directory.Exists(tempEnvelopeDirectory))
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["EnvelopeIdMissing"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.returnUrl = "Info/Index";
                        return BadRequest(responseMessage);
                    }
                    foreach (FileInfo folderfiles in new DirectoryInfo(Path.Combine(tempEnvelopeDirectory, "UploadedDocuments")).GetFiles())
                    {
                        fileSize += Convert.ToInt32(folderfiles.Length);
                    }
                    //string userToken = string.Format("{0}:{1}", ClientId, userProfile.EmailID);
                    //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(userToken);
                    //string base64UserToken = Convert.ToBase64String(plainTextBytes);


                    // End Of Unit Deduction Retrieving Values
                    var usersettingsDetails = _settingsRepository.GetEntityForByKeyConfig(StaticEnvelope.UserID, Constants.SettingsKeyConfig.EnablePostSigningLoginPopup);
                    if (usersettingsDetails != null)
                        enablePostSigningLoginPopup = Convert.ToBoolean(usersettingsDetails.OptionValue);
                    if (!string.IsNullOrEmpty(template.PostSigningLandingPage))
                        postSigningPage = template.PostSigningLandingPage;
                    else if (userProfile.CompanyID != null && userProfile.CompanyID != Guid.Empty)
                    {
                        Company senderCompany = new Company();
                        senderCompany = _companyRepository.GetCompanyByID(userProfile.CompanyID);
                        if (senderCompany != null && !string.IsNullOrEmpty(senderCompany.PostSigningLandingPage))
                        {
                            postSigningPage = senderCompany.PostSigningLandingPage;
                        }
                    }

                    if (isConfirmationReq == true)
                    {
                        isConfirmationMailSent = _envelopeHelperMain.SendConfirmationEmailForStaticTemplate(StaticEnvelope.ID, userProfile, Convert.ToString(newRecipientID), isEnvelopeTosign, _envelopeHelperMain.GetDocumentContents(envelope.ControlCollection),false, envelopeSignDocumentSubmitInfo.SendConfirmationData, recipient.DeliveryMode);
                        if (!envelope.IsEnvelopeComplete)
                        {
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.StatusMessage = "OK";
                            responseMessage.EnvelopeStatus = _envelopeHelperMain.GetEnvelopeStatus(newEnvelopeID);
                            var rectDeliveryMode = envelopeSignDocumentSubmitInfo.RecipientDeliveryMode;
                            if (rectDeliveryMode == "1" || rectDeliveryMode == "2" || rectDeliveryMode == "3" || rectDeliveryMode == "11")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else if (rectDeliveryMode == "8" || rectDeliveryMode == "9" || rectDeliveryMode == "10" || rectDeliveryMode == "12")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "EmailMobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else if (rectDeliveryMode == "4" || rectDeliveryMode == "5" || rectDeliveryMode == "6" || rectDeliveryMode == "7")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "MobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();

                            responseMessage.success = true;
                            responseMessage.data = newEnvelopeID;
                            responseMessage.returnUrl = isConfirmationReq ? "Info/Index" : postSigningPage;
                            responseMessage.postSigningLogin = enablePostSigningLoginPopup;//true;
                            return Ok(responseMessage);
                        }
                    }
                    else if (envelope.IsEnvelopeComplete)
                    {
                        // Lock Unit
                        isSignDocument = _envelopeHelperMain.SignStaticDocument(_envelopeHelperMain.GetDocumentContents(envelope.ControlCollection), StaticEnvelope, newRecipientID, userSettings.SelectedTimeZone, envelopeFolderUNCPath, out errorIfFound);
                        // Send Final Contract Email
                        if (isSignDocument == true)
                        {
                            HttpResponseMessage commitResponse = rpostRestService.CommitStaticLinkUnit(userProfile.EmailID, StaticEnvelope.EDisplayCode, fileSize, 1, base64UserToken, tokenType);
                            var jSonResponse = JsonConvert.DeserializeObject(commitResponse.Content.ReadAsStringAsync().Result).ToString();
                            var ResponseMessage = JsonConvert.DeserializeObject<RestResponseUserInfo>(commitResponse.Content.ReadAsStringAsync().Result);
                            loggerModelNew.Message = "Commit Static Link Unit Response : " + JsonConvert.SerializeObject(ResponseMessage);
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            if (!commitResponse.IsSuccessStatusCode && ResponseMessage.Status != "Success")
                            {
                                //Rollback
                                rpostRestService.RollBackLockUnits(StaticEnvelope.EDisplayCode, base64UserToken, tokenType);
                                //End rollback
                            }
                        }
                    }
                    else
                    {
                        // Update "Envelope.xml" file at temp location. Update "IsEnvelopeCompleted" field.
                        int isContractToGenerateFromImages = Convert.ToInt32(userSettings.FinalContractOptionID) > 0 ? userSettings.FinalContractOptionID : Constants.FinalContractOptions.Aspose;
                        var dictionary = new Dictionary<EnvelopeNodes, string> { { EnvelopeNodes.IsEnvelopeRejected, "true" } };
                        string finalPdfFilePath = string.Empty;
                        _eSignHelper.UpdateEnvelopeXML(newEnvelopeID, dictionary, envelopeFolderUNCPath);
                        var senderDetails = _recipientRepository.GetSenderDetails(StaticEnvelope.ID);
                        var signerDetails = _recipientRepository.GetEntity(newRecipientID);
                        userSettingsForfinalMergePDF = new FinalContractSettings
                        {
                            WatermarkAuthText = string.Empty,
                            WatermarkBackgroundText = string.Empty,
                            FinalContractOptions = isContractToGenerateFromImages,
                            UserTimeZone = userSettings.SelectedTimeZone,
                            IsControlDisplayInTag = false
                        };
                        try
                        {
                            finalPdfFilePath = _apiHelper.finalMergePDFApi(StaticEnvelope, userSettingsForfinalMergePDF, envelopeFolderUNCPath, string.Empty, Convert.ToBoolean(StaticEnvelope.IsSeparateMultipleDocumentsAfterSigningRequired) ? true : false);
                        }
                        catch
                        {
                            _envelopeHelperMain.DeleteContractFileInCaseOfError(StaticEnvelope.ID, envelopeFolderUNCPath);
                            userSettingsForfinalMergePDF.FinalContractOptions = Convert.ToInt32(userSettingsForfinalMergePDF.FinalContractOptions) != Constants.FinalContractOptions.iText ? Constants.FinalContractOptions.iText : Constants.FinalContractOptions.Aspose;
                            finalPdfFilePath = _apiHelper.finalMergePDFApi(StaticEnvelope, userSettingsForfinalMergePDF, string.Empty, envelopeFolderUNCPath, Convert.ToBoolean(StaticEnvelope.IsSeparateMultipleDocumentsAfterSigningRequired) ? true : false);
                        }
                        string subject = Convert.ToString(_appConfiguration["Subject_EnvelopeRejected"]);

                        HttpResponseMessage commitResponse = rpostRestService.CommitStaticLinkUnit(userProfile.EmailID, StaticEnvelope.EDisplayCode, fileSize, 1, base64UserToken, tokenType);
                        var jSonResponse = JsonConvert.DeserializeObject(commitResponse.Content.ReadAsStringAsync().Result).ToString();
                        var ResponseMessage = JsonConvert.DeserializeObject<RestResponseUserInfo>(commitResponse.Content.ReadAsStringAsync().Result);
                        if (!commitResponse.IsSuccessStatusCode && ResponseMessage.Status != "Success")
                        {
                            responseMessage.StatusCode = HttpStatusCode.Forbidden;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.message = "The Process can not be completed. Please Contact Sender.";
                            responseMessage.data = newEnvelopeID;
                            responseMessage.success = false;
                            responseMessage.returnUrl = "Info/Index";
                            loggerModelNew.Message = responseMessage.message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return BadRequest(responseMessage);
                        }
                        else
                        {
                            string comments = string.Empty;
                            if (envelope.Comment != null)
                            {
                                var declineTemplateSetting = JsonConvert.DeserializeObject<List<DeclineTemplateResponses>>(envelope.Comment);
                                if (declineTemplateSetting != null)
                                {
                                    if (declineTemplateSetting.Count > 0)
                                    {
                                        foreach (var item in declineTemplateSetting)
                                            comments = comments + item.ResponseText + ",";
                                        comments = comments.Remove(comments.Length - 1);
                                    }
                                }
                            }
                            _envelopeHelperMain.RejectToMail(!string.IsNullOrEmpty(comments) ? comments : envelope.Comment, "Terminated: " + subject, finalPdfFilePath, StaticEnvelope, signerDetails, senderDetails, "", envelopeFolderUNCPath);
                            _envelopeHelperMain.RejectMailToSender(!string.IsNullOrEmpty(comments) ? comments : envelope.Comment, "Terminated: " + subject, finalPdfFilePath, StaticEnvelope, signerDetails, senderDetails, "", envelopeFolderUNCPath);
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.StatusMessage = "OK";
                            responseMessage.success = true;
                            responseMessage.EnvelopeStatus = _envelopeHelperMain.GetEnvelopeStatus(newEnvelopeID);
                            var rectDeliveryMode = envelopeSignDocumentSubmitInfo.RecipientDeliveryMode;
                            if (rectDeliveryMode == "1" || rectDeliveryMode == "2" || rectDeliveryMode == "3" || rectDeliveryMode == "11")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else if (rectDeliveryMode == "8" || rectDeliveryMode == "9" || rectDeliveryMode == "10" || rectDeliveryMode == "12")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "EmailMobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else if (rectDeliveryMode == "4" || rectDeliveryMode == "5" || rectDeliveryMode == "6" || rectDeliveryMode == "7")
                            {
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "MobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            }
                            else
                                responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                            responseMessage.data = newEnvelopeID;
                            responseMessage.postSigningLogin = enablePostSigningLoginPopup;//true;
                            responseMessage.returnUrl = isConfirmationReq ? "Info/Index" : postSigningPage;
                            loggerModelNew.Message = "Process completed for Create Static Envelope action.";
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            return Ok(responseMessage);
                        }
                    }

                    string rsignanonymoustoken = string.Empty;
                    APIRecipientEntity recipients = new APIRecipientEntity();
                    if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientEmail))
                    {
                        userProfile = _userRepository.GetUserProfileByEmailID(envelopeSignDocumentSubmitInfo.RecipientEmail);
                        recipients.EmailAddress = envelopeSignDocumentSubmitInfo.RecipientEmail;
                        recipients.Name = envelopeSignDocumentSubmitInfo.RecipientEmail.Split('@')[0].ToString();
                        recipients.Mobile = "";
                        if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientMobile))
                        {
                            recipients.Mobile = envelopeSignDocumentSubmitInfo.RecipientDialCode + envelopeSignDocumentSubmitInfo.RecipientMobile;
                        }
                    }
                    else
                    {
                        userProfile = null;
                        recipients.EmailAddress = "";
                        recipients.Name = "";
                        recipients.Mobile = "";
                        if (!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.RecipientMobile))
                        {
                            recipients.Mobile = envelopeSignDocumentSubmitInfo.RecipientDialCode + envelopeSignDocumentSubmitInfo.RecipientMobile;
                        }
                    }

                    recipients.Order = userProfile != null ? 1 : 0;
                    recipients.EnvelopeID = newEnvelopeID;

                    lockTaken = true;
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.EnvelopeStatus = _envelopeHelperMain.GetEnvelopeStatus(newEnvelopeID);
                    var rectDeliveryModes = envelopeSignDocumentSubmitInfo.RecipientDeliveryMode;
                    if (rectDeliveryModes == "1" || rectDeliveryModes == "2" || rectDeliveryModes == "3" || rectDeliveryModes == "11")
                    {
                        responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                    }
                    else if (rectDeliveryModes == "8" || rectDeliveryModes == "9" || rectDeliveryModes == "10" || rectDeliveryModes == "12")
                    {
                        responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "EmailMobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                    }
                    else if (rectDeliveryModes == "4" || rectDeliveryModes == "5" || rectDeliveryModes == "6" || rectDeliveryModes == "7")
                    {
                        responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "MobileRequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                    }
                    else
                        responseMessage.message = isConfirmationReq ? _envelopeHelperMain.GetLanguageCodeBasedApiMessge((!string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.LanguageCode) ? envelopeSignDocumentSubmitInfo.LanguageCode : template.CultureInfo), "RequiredConfirmation") : _appConfiguration["SuccessInitializeEnvelope"].ToString();//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();

                    responseMessage.data = recipients;
                    responseMessage.recpDetail = recipients;
                    responseMessage.returnUrl = isConfirmationReq ? "Info/Index" : postSigningPage;
                    responseMessage.postSigningUrl = postSigningPage;
                    responseMessage.success = true;
                    responseMessage.postSigningLogin = enablePostSigningLoginPopup;//true;
                    loggerModelNew.Message = "Process completed for Create Static Envelope action.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    responseMessage.StatusMessage = "NotAcceptable";
                    responseMessage.message = Convert.ToString(_appConfiguration["RequiredFieldIsMissing"]);
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller CreateStaticEnvelope action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }
    }
}
