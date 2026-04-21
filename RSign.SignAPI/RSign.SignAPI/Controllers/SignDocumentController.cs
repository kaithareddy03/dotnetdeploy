using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.APIModels.Envelope;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using RSign.Web.Models;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Drawing.Imaging;
using GoogleDriveDownload;
using Microsoft.EntityFrameworkCore;
using RSign.Common.Mailer;
using RSign.Models.EmailQueueProcessor;
using System.Data.Entity;
using Org.BouncyCastle.Asn1.X509;

namespace RSign.SignAPI.Controllers
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class SignDocumentController : ControllerBase
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
        private GoogleDrive gDrive = new GoogleDrive();

        public SignDocumentController(IHttpContextAccessor accessor, IConfiguration appConfiguration, ICompanyRepository companyRepository, IDocumentContentsRepository documentContentsRepository,
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

        [ProducesResponseType(typeof(ResponseMessageForResponseSigningUrlModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("InitializeSignerSignDocument")]
        [HttpPost]
        public async Task<IActionResult> InitializeSignerSignDocument(RequestSigningUrlModel signingUrlModel)
        {
            var remoteIpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "InitializeSignerSignDocument", "Process started for Initialize Signer Sign Document", "", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageForInitalizeSignerSignDocument responseMessage = new ResponseMessageForInitalizeSignerSignDocument();

            if (string.IsNullOrEmpty(signingUrlModel.IsFromBotClick))
            {
                var userAgent = Request.Headers.UserAgent.ToString();
                int isBotClickVal = _envelopeHelperMain.IsBotClick(remoteIpAddress, userAgent);
                if (isBotClickVal == 2)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["RpostValidateBotFailure"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.IsBotClick = 2;
                    responseMessage.ResponseSigningUrlModel = null;
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = loggerModelNew.Message;
                    responseMessage.Message = loggerModelNew.Message;
                    responseMessage.EnvelopeInfo = null;
                    return BadRequest(responseMessage);
                }
                else if (isBotClickVal == 3)
                {
                    loggerModelNew.Message = "This is a bot click and IPAddress is" + remoteIpAddress + " and userAgent is:" + userAgent;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.IsBotClick = 3;
                    responseMessage.ResponseSigningUrlModel = null;
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = loggerModelNew.Message;
                    responseMessage.Message = loggerModelNew.Message;
                    responseMessage.EnvelopeInfo = null;
                    return BadRequest(responseMessage);
                }
            }

            ResponseSigningUrlModel responseSigningUrlModel = new ResponseSigningUrlModel();
            string userURL = signingUrlModel.SigningUrl;
            userURL = HttpUtility.UrlDecode(userURL);
            var envelopeID = "";
            var recipientID = "";
            var templateKey = "";
            var emailID = "";
            var isDirect = "";
            var SignerType = "";
            var CopyMailId = "";
            bool isSignerLanding = false;
            bool isFromInbox = false;

            if (!userURL.Equals(""))
            {
                userURL = EncryptDecryptQueryString.Decrypt(userURL, Convert.ToString(_appConfiguration["AppKey"]));
                if (!userURL.Equals("Invalid length for a Base-64 char array or string.") && !userURL.Equals("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters. ") && !userURL.Equals("Length of the data to decrypt is invalid."))  // V2 Team Prefill Change
                {
                    string[] arrayURL = userURL.Split('&');

                    if (arrayURL.Length == 6)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[1].Split('='); //Get the Recipient ID
                        recipientID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[2].Split('='); //Get the Template Key
                        templateKey = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[3].Split('='); //Get the Email ID
                        emailID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[4].Split('='); //Get the Email ID
                        isDirect = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[5].Split('='); //Get the Email ID
                        isFromInbox = arrayID.Length == 2 ? arrayID[1].Trim() == "1" : false;
                        emailID = !string.IsNullOrEmpty(emailID) ? HttpUtility.UrlDecode(emailID) : string.Empty;
                    }
                    else if (arrayURL.Length == 5)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[1].Split('='); //Get the Recipient ID
                        recipientID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[3].Split('='); //Get the Template Key
                        SignerType = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[2].Split('='); //Get the Email ID
                        emailID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[4].Split('='); //Get the Email ID
                        CopyMailId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        emailID = !string.IsNullOrEmpty(emailID) ? HttpUtility.UrlDecode(emailID) : string.Empty;
                        CopyMailId = !string.IsNullOrEmpty(CopyMailId) ? HttpUtility.UrlDecode(CopyMailId) : string.Empty;
                    }
                    else if (arrayURL.Length == 4)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[1].Split('='); //Get the Recipient ID
                        recipientID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[2].Split('='); //Get the Template Key
                        templateKey = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[3].Split('='); //Get the Email ID
                        emailID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        emailID = !string.IsNullOrEmpty(emailID) ? HttpUtility.UrlDecode(emailID) : string.Empty;
                    }
                    else if (arrayURL.Length == 3)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[1].Split('='); //Get the Recipient ID
                        recipientID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[2].Split('='); //Get the Template Key
                        emailID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        emailID = !string.IsNullOrEmpty(emailID) ? HttpUtility.UrlDecode(emailID) : string.Empty;
                    }
                    else if (arrayURL.Length == 2)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[1].Split('='); //Get the Recipient ID
                        recipientID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        isSignerLanding = true;
                    }
                    else if (arrayURL.Length == 1) // Single Signing Url
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        recipientID = Guid.Empty.ToString();
                    }

                    responseSigningUrlModel.EnvelopeID = envelopeID;
                    responseSigningUrlModel.RecipientID = recipientID;
                    responseSigningUrlModel.TemplateKey = templateKey;
                    responseSigningUrlModel.EmailId = emailID;
                    responseSigningUrlModel.IsDirect = isDirect;
                    responseSigningUrlModel.SignerType = SignerType;
                    responseSigningUrlModel.CopyEmailId = CopyMailId;
                    responseSigningUrlModel.IsFromInbox = isFromInbox;
                    responseSigningUrlModel.IsFromMultiSignPage = false;
                    responseSigningUrlModel.IsFromSignerPreLanding = signingUrlModel.IsFromSignerPreLanding;
                    responseSigningUrlModel.IsFromSignerLanding = signingUrlModel.IsFromSignerPreLanding ? false : true;
                    responseSigningUrlModel.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                    responseMessage.ResponseSigningUrlModel = responseSigningUrlModel;
                }
                else
                {
                    responseMessage.ResponseSigningUrlModel = responseSigningUrlModel;
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "The URL of the envelope is incorrect.";
                    responseMessage.Message = "The URL of the envelope is incorrect.";
                    responseMessage.EnvelopeInfo = null;
                    return Ok(responseMessage);
                }
            }
            else
            {
                responseMessage.ResponseSigningUrlModel = responseSigningUrlModel;
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "The URL of the envelope is incorrect.";
                responseMessage.Message = "The URL of the envelope is incorrect.";
                responseMessage.EnvelopeInfo = null;
                return Ok(responseMessage);
            }

            string envelopeCode = string.Empty;
            string emailId = string.Empty;
            bool IsUnitTestApp = false;
            string Message = string.Empty;
            Guid envelopeId = new Guid(responseSigningUrlModel.EnvelopeID);
            Guid recipientId = Guid.Empty;
            if (!string.IsNullOrEmpty(responseSigningUrlModel.RecipientID))
            {
                recipientId = new Guid(responseSigningUrlModel.RecipientID);
            }

            string currentRecipientEmailId = string.Empty;
            string senderEmail = string.Empty;
            string currentenvelopeID = string.Empty;
            EnvelopeDetails envelopeDetails = new EnvelopeDetails();
            DashBoard dashBoard = new DashBoard();
            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            APISettings aPISettings = new APISettings();
            Guid signerStatusId = Guid.Empty;
            EnvelopeInfo controlsInfo = new EnvelopeInfo();
            string folderFileSize = "0";
            List<DocumentDetails> documentDetails = new List<DocumentDetails>();
            List<EnvelopeAdditionalUploadInfoDetails> envelopeAdditionalUploadInfoDetails = new List<EnvelopeAdditionalUploadInfoDetails>();
            List<Guid> SameRecipientIds = new List<Guid>();
            bool IsTemplateDatedBeforePortraitLandscapeFeature = false;
            List<CheckListData> lstCheckListData = new List<CheckListData>();
            List<EnvelopeImageControlData> lstEnvelopeImageControlData = new List<EnvelopeImageControlData>();
            string dirPath = string.Empty;
            try
            {
                bool isEnvelopeFromPrimaryTable = true;
                var envelopeObject = _genericRepository.GetEntity(envelopeId, false);
                if (envelopeObject == null)
                {
                    isEnvelopeFromPrimaryTable = false;
                    envelopeObject = _genericRepository.GetEntityHistory(envelopeId);
                }

                envelopeCode = envelopeObject == null ? "" : Convert.ToString(envelopeObject.EDisplayCode);
                responseMessage.EnvelopeInfo = controlsInfo;
                bool isEnvelopeArchived = false;
                if (envelopeObject == null || string.IsNullOrEmpty(envelopeCode))
                {
                    #region Envelope Arichived or not
                    // int? checkArchiveDBForPurging = await _genericRepository.CheckEnvelopeFromArchiveDatabase(envelopeCode, envelopeId);  
                    ArichiveEnvelopesInfo envelopesInfo = _genericRepository.GetArchivedEnvelope(envelopeCode, envelopeId, recipientId);
                    if (envelopesInfo != null)
                    {
                        if (envelopesInfo.IsEnvelopePurging == true && envelopesInfo.envelope == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.StatusMessage = "OK";
                            responseMessage.Message = envelopesInfo.ArchivedEnvelopeMessage;
                            responseMessage.IsEnvelopePurging = true;
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            return Ok(responseMessage);
                        }
                        else if (envelopesInfo.envelope != null)
                        {
                            isEnvelopeArchived = true;
                            envelopeObject = envelopesInfo.envelope;
                        }
                        else if (envelopesInfo != null && envelopesInfo.envelope == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.NoContent;
                            responseMessage.StatusMessage = "NoContent";
                            responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"]);
                            responseMessage.EnvelopeInfo = null;
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            return BadRequest(responseMessage);
                        }
                    }
                    else
                    {
                        responseMessage.StatusCode = HttpStatusCode.NoContent;
                        responseMessage.StatusMessage = "NoContent";
                        responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"]);
                        responseMessage.EnvelopeInfo = null;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                    #endregion Envelope Arichived or not                 
                }
                if (envelopeObject.IsActive == false)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "The sender has deleted the envelope. For further information, please contact the sender.";
                    responseMessage.EnvelopeInfo = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                var envelopeSettingObject = _eSignHelper.GetEnvelopeSettingsDetail(envelopeId);
                if (envelopeSettingObject != null)
                    responseMessage.AttachSignedPdfID = envelopeSettingObject.AttachSignedPdf;

                var sender = envelopeObject.Recipients.FirstOrDefault(a => a.RecipientTypeID == Constants.RecipientType.Sender);
                /* Get User Settings */
                aPISettings = _settingsRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
                adminGeneralAndSystemSettings = _eSignHelper.TransformSettingsDictionaryToEntity(aPISettings);
                responseMessage.IsAllowSignerstoDownloadFinalContract = adminGeneralAndSystemSettings.IsAllowSignerstoDownloadFinalContract;

                //Check for is envelope discarded
                if (Convert.ToBoolean(envelopeObject.IsDraftDeleted) == true)
                {
                    loggerModelNew.Message = envelopeObject.ID + " Envelope contract is discarded by sender.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeDiscarded");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                //If envelope is already expired
                if (envelopeObject.ExpiryDate < DateTime.Today)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract is expired";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeExpired");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.ErrorAction = "Expire";
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                // Check if envelope is already rejected   
                if (envelopeObject.StatusID == Constants.StatusCode.Envelope.Terminated)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has been rejected";
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeTerminated");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                // Check if envelope is already Cancelled   
                if (envelopeObject.StatusID == Constants.StatusCode.Envelope.CancelledTransaction)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has been cancelled by" + senderEmail;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeCancelled");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                // Check if envelope is already Completed  
                if (envelopeObject.StatusID == Constants.StatusCode.Envelope.Completed)
                {
                    currentRecipientEmailId = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).EmailAddress;
                    UserProfile userProfile1 = _userRepository.GetLatestUserProfile(currentRecipientEmailId);
                    responseMessage.RecipientOrder = userProfile1 != null ? 1 : 0;
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract is completed";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "ConatctSender");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.ErrorAction = "ContactSender";
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                // Check if envelope is updating stage. 
                if (Convert.ToBoolean(envelopeObject.IsEdited) == true)
                {
                    loggerModelNew.Message = envelopeObject.ID + " Envelope contract is updating at this time.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeUpdating");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }

                #region S3-936:Move History envelope to Primary  
                if (!isEnvelopeFromPrimaryTable)
                {
                    if (envelopeObject != null)
                    {
                        var objResponse = _envelopeRepository.InsertHistoryEnvelopeToPrimary(envelopeObject.ID.ToString());
                        if (objResponse.StatusMessage == "Success")
                        {
                            envelopeObject = _genericRepository.GetEntity(envelopeId, true, isEnvelopeArchived == true ? 1 : 0);
                            loggerModelNew.Message = "Inserted EnvelopeHistory is primary for envelopeId ";
                            rSignLogger.RSignLogWarn(loggerModelNew);
                        }
                    }
                }
                #endregion S3-936:Move History envelope to Primary 

                TranslationsModel translationsModel = new TranslationsModel();
                translationsModel.EnvelopeId = envelopeID;
                translationsModel.RecipientId = recipientID;
                translationsModel.CultureInfo = "";
                LanguageKeyTranslationsModel responseTranslations = _genericRepository.GetLanguageKeyTranslations(translationsModel);
                responseMessage.LanguageTranslationsModel = responseTranslations;
                responseMessage.EnableMessageToMobile = envelopeObject.EnableMessageToMobile;
                responseMessage.DialCodeDropdownList = _envelopeHelperMain.LoadDialingCountryCodes();

                if (responseSigningUrlModel.IsFromSignerPreLanding != null && Convert.ToBoolean(responseSigningUrlModel.IsFromSignerPreLanding) && Convert.ToBoolean(envelopeObject.AllowMultiSigner) == true && Convert.ToBoolean(envelopeObject.IsStatic))
                {
                    senderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    controlsInfo.SenderEmail = senderEmail;
                    var recpDetails = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId);
                    controlsInfo.RecipientEmail = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).EmailAddress;
                    controlsInfo.IsStatic = Convert.ToBoolean(envelopeObject.IsStatic);
                    controlsInfo.PasswordReqdtoSign = envelopeObject.PasswordReqdtoSign;
                    responseMessage.IsPasswordMailToSigner = envelopeObject.IsPasswordMailToSigner;
                    responseMessage.AllowMultipleSigner = Convert.ToBoolean(envelopeObject.AllowMultiSigner);
                    responseMessage.EnvelopeInfo = controlsInfo;
                    responseMessage.DialCode = recpDetails.DialCode;
                    responseMessage.DeliveryMode = Convert.ToString(recpDetails.DeliveryMode);
                    responseMessage.MobileNumber = recpDetails.Mobile;
                    responseMessage.CountryCode = recpDetails.CountryCode;

                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    loggerModelNew.Message = "Password ReqdtoSign option enabled " + responseMessage.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }
                if (responseSigningUrlModel.IsFromSignerPreLanding != null && Convert.ToBoolean(responseSigningUrlModel.IsFromSignerPreLanding) && envelopeObject.PasswordReqdtoSign != null && Convert.ToBoolean(envelopeObject.PasswordReqdtoSign))
                {
                    bool CanEdits = Convert.ToBoolean(envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).CanEdit);
                    int? InviteSignNowEmails = envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).SignNowOrInviteEmail;

                    string signerCultureInfo = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).CultureInfo;
                    controlsInfo.CultureInfo = !string.IsNullOrEmpty(signerCultureInfo) ? signerCultureInfo : envelopeObject.CultureInfo;
                    controlsInfo.IsStatic = Convert.ToBoolean(envelopeObject.IsStatic);
                    controlsInfo.PasswordReqdtoSign = envelopeObject.PasswordReqdtoSign;
                    controlsInfo.RecipientEmail = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).EmailAddress;
                    Recipients recipientDetail = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId);
                    senderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    controlsInfo.SenderEmail = senderEmail;
                    controlsInfo.IsInvitedBySigner = Convert.ToBoolean(envelopeObject.IsInvitedBySigner);
                    responseMessage.IsPasswordMailToSigner = envelopeObject.IsPasswordMailToSigner;
                    responseMessage.CreatedSource = envelopeObject.CreatedSource;
                    responseMessage.CanEdit = CanEdits;
                    responseMessage.InviteSignNowByEmail = InviteSignNowEmails;
                    responseMessage.AllowMultipleSigner = Convert.ToBoolean(envelopeObject.AllowMultiSigner);

                    responseMessage.DialCode = recipientDetail.DialCode;
                    responseMessage.DeliveryMode = Convert.ToString(recipientDetail.DeliveryMode);
                    responseMessage.MobileNumber = recipientDetail.Mobile;
                    responseMessage.CountryCode = recipientDetail.CountryCode;

                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    // Message = "Password ReqdtoSign option enabled.";
                    loggerModelNew.Message = "Password ReqdtoSign option enabled " + responseMessage.Message;
                    responseMessage.EnvelopeInfo = controlsInfo;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }

                //check folder size for signer
                double size = 0;
                dirPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeObject.ID, string.Empty);
                var uploadedDirectory = (dirPath + envelopeObject.ID + "\\SignerAttachments") + "\\" + recipientId;
                if (Directory.Exists(uploadedDirectory))
                {
                    foreach (FileInfo folderfiles in new DirectoryInfo(uploadedDirectory).GetFiles())
                    {
                        size += folderfiles.Length;
                    }
                }
                // folderFileSize = size.ToString();
                controlsInfo.IsTemplateShared = envelopeObject.IsTemplateShared;

                List<ConditionalControlMapping> conditionalControlMappings = new List<ConditionalControlMapping>();
                if (envelopeObject.Documents.Any(d => d.DocumentContents != null))
                {
                    conditionalControlMappings = _envelopeRepository.GetConditionalControlMapping(envelopeObject.ID).ToList();
                }

                envelopeDetails = _envelopeRepository.FillEnvelopeDetailsByEnvelopeEntity(envelopeObject, conditionalControlMappings, false);
                //  envelopeDetails = _envelopeRepository.FillEnvelopeDetailsByEnvelopeEntity(envelopeObject);

                var recipientexist = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId);
                if (recipientexist == null)
                {
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeTerminatedForRecipient");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                responseMessage.DialCode = recipientexist.DialCode;
                responseMessage.DeliveryMode = Convert.ToString(recipientexist.DeliveryMode);
                responseMessage.MobileNumber = recipientexist.Mobile;
                responseMessage.CountryCode = recipientexist.CountryCode;

                //Check for order of recipient
                bool isSigned = true;
                if (Convert.ToInt32(recipientexist.Order) > 1)
                {
                    List<RecipientDetails> activeRecipients = _recipientRepository.GetActiveRecipientData(envelopeObject.ID);
                    var tempRecipients = activeRecipients.Where(a => a.Order != null && a.IsSameRecipient != true && a.Order == (Convert.ToInt32(recipientexist.Order) - 1)).ToList();
                    foreach (var item in tempRecipients)
                    {
                        isSigned = _recipientRepository.GetSignerStatusId(item.ID) != Constants.StatusCode.Signer.Signed ? false : true;
                        if (!isSigned)
                            break;
                    }
                }

                if (!Convert.ToBoolean(responseSigningUrlModel.IsFromSignerLanding))
                {
                    if (!string.IsNullOrEmpty(responseSigningUrlModel.TemplateKey))
                    {
                        if (envelopeObject.TemplateKey != new Guid(responseSigningUrlModel.TemplateKey))
                        {
                            if (recipientexist.RecipientTypeID == Constants.RecipientType.Signer)
                            {
                                if (!isSigned)
                                {
                                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "UnauthorisedSigner");
                                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                    responseMessage.StatusMessage = "BadRequest";
                                    responseMessage.Message = Message;
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                    return BadRequest(responseMessage);
                                }
                                else
                                {
                                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeOutOfdateResend");
                                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                    responseMessage.StatusMessage = "BadRequest";
                                    responseMessage.TempDataResendMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeResendLatest");
                                    responseMessage.ErrorAction = "Resend";
                                    responseMessage.Message = Message;
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                    return BadRequest(responseMessage);
                                }

                            }
                            else
                            {
                                Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "UnauthorisedSigner");
                                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                responseMessage.StatusMessage = "BadRequest";
                                responseMessage.Message = Message;
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                responseMessage.EnvelopeInfo = null;
                                return BadRequest(responseMessage);
                            }
                        }
                    }

                    if (envelopeObject.TemplateKey != null && responseSigningUrlModel.TemplateKey == "")
                    {
                        if (recipientexist.RecipientTypeID == Constants.RecipientType.Signer)
                        {
                            if (!isSigned)
                            {
                                Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "UnauthorisedSigner");
                                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                responseMessage.StatusMessage = "BadRequest";
                                responseMessage.Message = Message;
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                return BadRequest(responseMessage);
                            }
                            else
                            {
                                Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeOutOfdateResend");
                                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                responseMessage.StatusMessage = "BadRequest";
                                responseMessage.Message = Message;
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                responseMessage.ErrorAction = "Resend";
                                responseMessage.TempDataResendMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeResendLatest");
                                responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                return BadRequest(responseMessage);
                            }
                        }
                        else
                        {
                            Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "UnauthorisedSigner");
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.Message = Message;
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                            return BadRequest(responseMessage);
                        }
                    }
                }

                currentRecipientEmailId = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).EmailAddress;
                senderEmail = sender != null ? sender.EmailAddress : string.Empty;
                currentenvelopeID = envelopeObject.EDisplayCode.ToString();
                var currentRecipientType = envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).RecipientTypeID;  // V2 Team Prefill Change
                bool CanEdit = Convert.ToBoolean(envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).CanEdit);
                int? InviteSignNowEmail = envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).SignNowOrInviteEmail;
                SameRecipientIds = envelopeObject.Recipients.Where(a => a.EmailAddress == currentRecipientEmailId && a.ID != recipientId && a.IsSameRecipient == true).Select(a => a.ID).ToList();

                if (envelopeObject.DraftType == "Sign" && currentRecipientType != Constants.RecipientType.Prefill)
                {
                    loggerModelNew.Message = "Please complete the prefill signing process for further signing.";
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopePrefillSignMessage");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }

                try
                {
                    signerStatusId = _recipientRepository.GetSignerStatusId(recipientId);
                }
                catch (Exception ex)
                {
                    signerStatusId = Guid.Empty;
                }
                if (signerStatusId == null || signerStatusId == Guid.Empty)
                {
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeAccepted");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.ErrorAction = "EnvelopeAccepted";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                if (signerStatusId == Constants.StatusCode.Recipients.Transferred)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has already been transfer to other signer";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeTransferred");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                if (signerStatusId == Constants.StatusCode.Signer.Delegated)
                {
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeAlreadyDelegate");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }

                if (signerStatusId == Constants.StatusCode.Signer.Signed)
                {
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeAccepted");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.ErrorAction = "EnvelopeAccepted";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                //string ipAddress = Common.GetIPAddress();
                string ipAddress = responseSigningUrlModel.IPAddress;
                int? IsReviewed = recipientexist.IsReviewed;

                var signerSignedStatus = _recipientRepository.GetSignerSignedStatusId(recipientId);
                if (signerStatusId == Constants.StatusCode.Signer.Pending)
                {
                    if ((!IsUnitTestApp && !envelopeObject.PasswordReqdtoSign) || (Convert.ToBoolean(responseSigningUrlModel.IsFromSignerLanding) && envelopeObject.PasswordReqdtoSign))
                    {
                        if (!envelopeObject.IsTemplateShared || (envelopeObject.IsTemplateShared && responseSigningUrlModel.IsFromMultiSignPage))
                        {
                            if (signerSignedStatus == null)
                            {
                                var signerstatus = new SignerStatus
                                {
                                    StatusID = Constants.StatusCode.Signer.Viewed,
                                    ID = Guid.NewGuid(),
                                    RecipientID = recipientId,
                                    CreatedDateTime = DateTime.Now,
                                    IPAddress = ipAddress,
                                    SignedBy = responseSigningUrlModel.CopyEmailId
                                };
                                _recipientRepository.Save(signerstatus);
                            }
                        }
                    }
                }
                if ((!IsUnitTestApp && !envelopeObject.PasswordReqdtoSign) || (Convert.ToBoolean(responseSigningUrlModel.IsFromSignerLanding) && envelopeObject.PasswordReqdtoSign))
                {
                    if (!envelopeObject.IsTemplateShared || (envelopeObject.IsTemplateShared && responseSigningUrlModel.IsFromMultiSignPage))
                    {
                        if (signerSignedStatus == null)
                        {
                            string refEmailId = envelopeObject.IsStatic == true ? responseSigningUrlModel.CopyEmailId : (!string.IsNullOrEmpty(responseSigningUrlModel.CopyEmailId) ? responseSigningUrlModel.CopyEmailId : currentRecipientEmailId);
                            bool saveStatus = _recipientRepository.SaveRecipientDetail(recipientId, Constants.StatusCode.Signer.Viewed, responseSigningUrlModel.IPAddress, refEmailId);
                        }
                    }
                }
                if (recipientexist.EnvelopeTemplateGroupID != null && recipientexist.RecipientTypeID != Constants.RecipientType.Prefill && recipientexist.EnvelopeTemplateGroupID.Value != Guid.Empty)
                {
                    controlsInfo = _envelopeHelperMain.GetEnvelopeImageInfoByRecipientGroup(envelopeObject, recipientId, recipientexist.EnvelopeTemplateGroupID.Value, dirPath);
                    controlsInfo.SubEnvelopeId = recipientexist.EnvelopeTemplateGroupID.Value;
                }
                else
                    controlsInfo = _envelopeHelperMain.GetEnvelopeImageInfo(envelopeObject, dirPath);

                /* Get User Settings */
                //aPISettings = _settingsRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
                //adminGeneralAndSystemSettings = _eSignHelper.TransformSettingsDictionaryToEntity(aPISettings);

                //var envelopeSettingObject = _eSignHelper.GetEnvelopeSettingsDetail(envelopeId);
                //if (envelopeSettingObject != null)
                //    responseMessage.AttachSignedPdfID = envelopeSettingObject.AttachSignedPdf;

                controlsInfo.IsReviewed = recipientexist.IsReviewed;
                controlsInfo.SignatureCaptureHanddrawn = adminGeneralAndSystemSettings.SignatureCaptureHanddrawn;
                controlsInfo.SignatureCaptureType = adminGeneralAndSystemSettings.SignatureCaptureType;
                controlsInfo.UploadSignature = adminGeneralAndSystemSettings.UploadSignature;
                controlsInfo.ElectronicSignIndication = envelopeObject.ElectronicSignIndicationOptionID != null ? envelopeObject.ElectronicSignIndicationOptionID.Value : adminGeneralAndSystemSettings.ElectronicSignIndicationSelectedID;
                controlsInfo = _envelopeHelperMain.GetDocumentControls(envelopeObject, controlsInfo, recipientId, envelopeId, currentRecipientEmailId, currentenvelopeID, conditionalControlMappings, false);

                if (controlsInfo.ControlsData != null && controlsInfo.ControlsData.Count() > 0)
                {
                    var controlscount = controlsInfo.ControlsData.Where(c => c.ControlHtmlID != "FooterSignature").ToList().Count();
                    if (controlscount > 0)
                    {
                        //Getting all signer's controls for expanding the controls story
                        controlsInfo.AllDocumentControls = _envelopeHelperMain.GetAllDocumentControlsRetriveControlData(envelopeId, envelopeObject);
                    }
                }

                controlsInfo.SignerDocs = _envelopeHelperMain.GetSignerDocFromDirectory(envelopeId, recipientId, dirPath);
                controlsInfo.IsSignerAttachFileReq = (envelopeObject.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                controlsInfo.IsSignerAttachFileReqNew = envelopeObject.IsSignerAttachFileReq != null ? envelopeObject.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;

                string recipientCultureInfo = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).CultureInfo;
                controlsInfo.CultureInfo = Convert.ToString(!string.IsNullOrEmpty(recipientCultureInfo) ? recipientCultureInfo : envelopeObject.CultureInfo);

                controlsInfo.SenderEmail = senderEmail;
                controlsInfo.RecipientEmail = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).EmailAddress;
                controlsInfo.DateFormatID = envelopeObject.DateFormatID;
                controlsInfo.RecipientTypeIDPrefill = envelopeObject.Recipients.FirstOrDefault(x => (x.ID == recipientId)).RecipientTypeID;
                controlsInfo.PasswordReqdtoSign = envelopeObject.PasswordReqdtoSign;
                controlsInfo.SignerStatusId = signerStatusId;
                controlsInfo.FolderFileSize = folderFileSize;
                controlsInfo.EDisplayCode = envelopeObject.EDisplayCode.ToString();
                controlsInfo.recipientStatusId = envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).StatusID;
                controlsInfo.GlobalEnvelopeID = envelopeId;
                controlsInfo.Disclaimer = envelopeObject.DisclaimerText;
                controlsInfo.IsDisclaimerEnabled = !string.IsNullOrEmpty(envelopeObject.DisclaimerText);

                controlsInfo.IsInvitedBySigner = Convert.ToBoolean(envelopeObject.IsInvitedBySigner);
                if (envelopeObject.IsTemplateShared)
                {
                    var delegatedControls = _documentContentsRepository.GetDelegatedControls(recipientId);
                    if (delegatedControls != null)
                        envelopeObject.IsTemplateShared = false;
                }

                controlsInfo.IsTemplateShared = envelopeObject.IsTemplateShared;
                controlsInfo.IsSingleSigning = Convert.ToBoolean(envelopeObject.IsStatic);
                controlsInfo.Controls = _masterDataRepository.GetControlID().Where(c => c.ID != Constants.Control.NewInitials).ToDictionary(x => x.ControlName, x => x.ControlName);
                controlsInfo.IsStatic = Convert.ToBoolean(envelopeObject.IsStatic);
                controlsInfo.IsDefaultSignatureForStaticTemplate = envelopeObject.IsDefaultSignatureForStaticTemplate;
                controlsInfo.IsSharedTemplateContentUnEditable = envelopeObject.IsSharedTemplateContentUnEditable;
                controlsInfo.TimeZoneSettingOptionValue = adminGeneralAndSystemSettings.SelectedTimeZone.ToString();
                bool isSignerattachmentProcess = false;
                if (Convert.ToInt32(envelopeObject.IsSignerAttachFileReq) > 0)
                {
                    isSignerattachmentProcess = envelopeObject.IsAdditionalAttmReq != null ? Convert.ToBoolean(envelopeObject.IsAdditionalAttmReq) : false;
                }
                controlsInfo.IsAdditionalAttmReq = isSignerattachmentProcess;
                controlsInfo.ISNewSigner = Convert.ToBoolean(envelopeObject.ISNewSigner);

                /*RS-428: URGENT - All RSign controls shifted down and right - solve with highest priority*/
                var recipient = _recipientRepository.GetEntity(recipientId);
                if (recipient != null)
                {
                    Guid? templateId = recipient.TemplateID;
                    var template = (templateId != null) ? _templateRepository.GetCreatedDateTime((Guid)templateId) : null;
                    if (template != null)
                    {
                        var dateTimeBeforePortraitLandscape = Convert.ToDateTime(Convert.ToString(_appConfiguration["dateTimeBeforePortraitLandscape"]));
                        if (template.CreatedDateTime <= dateTimeBeforePortraitLandscape)
                            IsTemplateDatedBeforePortraitLandscapeFeature = true;
                    }
                    controlsInfo.IsTemplateDatedBeforePortraitLandscapeFeature = IsTemplateDatedBeforePortraitLandscapeFeature;
                }

                if (envelopeObject.Documents.Count > 0)
                {
                    foreach (var doc in envelopeObject.Documents)
                    {
                        DocumentDetails document = new DocumentDetails();
                        document.ID = doc.ID;
                        document.EnvelopeID = doc.EnvelopeID;
                        document.DocumentName = doc.DocumentName;
                        document.DocumentSource = doc.DocumentSource;
                        document.ActionType = doc.ActionType;
                        documentDetails.Add(document);
                    }
                }

                if (controlsInfo.IsSignerAttachFileReqNew == Constants.SignerAttachmentOptions.EnableAttachmentRequest)
                {
                    envelopeAdditionalUploadInfoDetails = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByEnvelopebyRecipeint(envelopeObject.ID, recipientId); //GetEnvelopeAdditionalUploadInfoByEnvelope
                }

                UserProfile userProfile = _userRepository.GetLatestUserProfile(currentRecipientEmailId);
                UserData userData = new UserData();
                if (userProfile != null)
                {
                    userData.UserId = userProfile.UserID;
                    userData.UserName = userProfile.FirstName + " " + userProfile.LastName;
                    userData.UserInitials = userProfile.Initials;
                    if (userProfile.IsAutoPopulateSignaturewhileSinging)
                    {
                        userData.UserInitialsImgSrc = userProfile.SignatureImage == null ? null : "data:image/png;base64," + Convert.ToBase64String(userProfile.SignatureImage);
                    }
                    else
                    {
                        userData.UserInitialsImgSrc = null;
                    }
                    userData.SignatureTypeID = userProfile.SignatureTypeID;
                }
                else if (envelopeObject.Recipients.Where(r => r.ID == recipientId).Count() > 0)
                {
                    foreach (var rec in envelopeObject.Recipients.Where(r => r.ID == recipientId))
                    {
                        userData.UserId = rec.ID;
                        userData.UserName = rec.Name;
                        userData.UserInitials = null;
                        userData.UserInitialsImgSrc = null;
                        userData.SignatureTypeID = Guid.Empty;
                    }
                }

                var senderProfileDetails = _userRepository.GetUserProfileByUserID(envelopeObject.UserID);
                if (sender != null)
                {
                    AdminGeneralAndSystemSettings companysettings = new AdminGeneralAndSystemSettings();
                    Guid CompanyID = (Guid)senderProfileDetails.CompanyID;
                    var getSetting = _settingsRepository.GetEntityByParam(CompanyID, string.Empty, Constants.String.SettingsType.Company);
                    companysettings = _eSignHelper.TransformSettingsDictionaryToEntity(getSetting);
                    responseMessage.DisableDeclineOption = companysettings.DisableDeclineOption;
                    responseMessage.DisableFinishLaterOption = companysettings.DisableFinishLaterOption;
                    responseMessage.DisableChangeSigner = companysettings.DisableChangeSigner;                   
                }

                responseMessage.userdata = userData;
                responseMessage.MaxUploadID = _envelopeRepository.GetMaxUploadsID();
                responseMessage.EnableAutoFillTextControls = Convert.ToBoolean(envelopeObject.IsEnableAutoFillTextControls);
                responseMessage.CreatedSource = envelopeObject.CreatedSource;
                responseMessage.CanEdit = CanEdit;
                responseMessage.InviteSignNowByEmail = InviteSignNowEmail;
                responseMessage.AllowMultipleSigner = Convert.ToBoolean(envelopeObject.AllowMultiSigner);
                responseMessage.UNCPath = _modelHelper.GetIdEnvelopeDirectory(dirPath);

                #region  Signer Landing from Angular
                responseMessage.EncryptedGlobalEnvelopeID = EncryptDecryptQueryString.Encrypt(Convert.ToString(envelopeId), Convert.ToString(_appConfiguration["AppKey"]));
                responseMessage.EncryptedGlobalRecipientID = EncryptDecryptQueryString.Encrypt(Convert.ToString(recipientId), Convert.ToString(_appConfiguration["AppKey"]));
                responseMessage.EncryptedSender = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(Convert.ToString(controlsInfo.SenderEmail), Convert.ToString(_appConfiguration["AppKey"])));
                responseMessage.Delegated = controlsInfo.SignerStatusId == Constants.StatusCode.Signer.Delegated ? "Delegated" : "NotDelegated";
                responseMessage.Prefill = controlsInfo.RecipientTypeIDPrefill == Constants.RecipientType.Prefill ? "prefill" : "";

                responseMessage.Language = _lookupRepository.GetLookupLanguageList(Lookup.LanguageKeyDetails, Convert.ToString(controlsInfo.CultureInfo) == string.Empty ? "en-us" : controlsInfo.CultureInfo.ToLower(), Constants.String.languageKeyType.Label);
                var labelText = responseMessage.Language;

                #region DateFormat
                string DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_slash;
                string DateFormat = "";
                if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_slash;
                    DateFormat = "mm/dd/yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_colan;
                    DateFormat = "mm-dd-yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_dots;
                    DateFormat = "mm.dd.yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_slash))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_slash;
                    DateFormat = "dd/mm/yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_colan;
                    DateFormat = "dd-mm-yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_dots;
                    DateFormat = "dd.mm.yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_yyyy_mm_dd_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_yyyy_mm_dd_dots;
                    DateFormat = "yy.mm.dd.";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_dd_mmm_yyyy_colan;
                    DateFormat = "dd-mmm-yy";
                }
                #endregion DateFormat

                responseMessage.DatePlaceHolder = DatePlaceHolder;
                responseMessage.DateFormat = DateFormat;
                responseMessage.FileReviewInfo = documentDetails.Where(d => d.ActionType == Constants.ActionTypes.Review).ToList();
                responseMessage.FileReviewCount = documentDetails.Where(d => d.ActionType == Constants.ActionTypes.Review).Count();
                responseMessage.DocumentNameList = documentDetails.Where(d => d.ActionType != Constants.ActionTypes.Review).ToList();
                responseMessage.AllowStaticMultiSigner = false;

                Guid SignatureTypeID = Guid.Empty;
                if (currentRecipientType == Constants.RecipientType.Prefill)
                    responseMessage.EnableClickToSign = CanEdit == true ? false : adminGeneralAndSystemSettings.EnableClickToSign;
                else
                    responseMessage.EnableClickToSign = adminGeneralAndSystemSettings.EnableClickToSign;

                responseMessage.DisableDownloadOptionOnSignersPage = adminGeneralAndSystemSettings.DisableDownloadOptionOnSignersPage;
                responseMessage.SignatureTypeID = userData != null ? userData.SignatureTypeID : Guid.Empty;
                responseMessage.IsAnySignatureExists = controlsInfo.ControlsData.Any(c => c.ControlName == "Signature" && (c.IsCurrentRecipient == true));

                if (responseMessage.IsAnySignatureExists == false)
                    responseMessage.ShowDefaultSignatureContol = "block";
                else
                    responseMessage.ShowDefaultSignatureContol = "none";

                var DefaultplaceHolder = string.Empty;
                if (DatePlaceHolder == "dd-mmm-yyyy")
                    responseMessage.DefaultplaceHolder = DatePlaceHolder.Replace("mmm", "MMM");
                else
                    responseMessage.DefaultplaceHolder = DatePlaceHolder.Replace("mm", "MM");

                #region CheckListData 
                var SameRecipientList = SameRecipientIds;
                int? pageCount = 0;
                pageCount = controlsInfo.ControlsData.Where(x => (x.IsCurrentRecipient == true || (x.RecipientId != null && x.RecipientId != Guid.Empty && SameRecipientList != null && SameRecipientList.Contains(x.RecipientId.Value) == true))).Max(x => x.PageNo);
                pageCount = pageCount == null ? 0 : pageCount.Value;
                responseMessage.PageCount = pageCount;
                bool isAnySignatureExists = controlsInfo.ControlsData.Any(c => c.ControlName == "Signature" && (c.IsCurrentRecipient == true));
                var defaultSignature = controlsInfo.ControlsData.FirstOrDefault(c => c.ControlName == "FooterSignature");
                List<string> radioCntrlOndoc = new List<string>();
                List<string> checkboxCntrlOndoc = new List<string>();
                List<DocumentDetails> DocumentNameList = responseMessage.DocumentNameList;
                bool EditControls = CanEdit;

                if (pageCount > 0)
                {
                    if (controlsInfo.ControlsData.Count > 1)
                    {
                        Guid tempDocId = Guid.Empty;
                        string documentName = "";
                        for (var i = 1; i <= pageCount; i++)
                        {
                            if (controlsInfo.ControlsData.Any(x => x.PageNo == i && ((EditControls == true && (x.ControlName != "Signature" && x.ControlName != "Email" && x.ControlName != "NewInitials")) || x.IsCurrentRecipient == true || (x.RecipientId != null && x.RecipientId != Guid.Empty && SameRecipientList != null && SameRecipientList.Contains(x.RecipientId.Value) == true)) && x.ControlName != "Label" && x.ControlName != "DateTimeStamp" && x.ControlName != "Hyperlink"))
                            {
                                List<ControlsData> ControlsData = controlsInfo.ControlsData.OrderBy(item => item.TabIndex).Where(x => x.PageNo == i && ((EditControls == true && (x.ControlName != "Signature" && x.ControlName != "Email" && x.ControlName != "NewInitials")) || x.IsCurrentRecipient == true || (x.RecipientId != null && x.RecipientId != Guid.Empty && SameRecipientList != null && SameRecipientList.Contains(x.RecipientId.Value) == true))).ToList();

                                if ((tempDocId == Guid.Empty || (ControlsData != null && ControlsData.Count > 0 && tempDocId != ControlsData[0].DocumentId)))
                                {
                                    documentName = DocumentNameList.Where(d => d.ID == ControlsData[0].DocumentId).FirstOrDefault().DocumentName;
                                    tempDocId = ControlsData[0].DocumentId;
                                }

                                List<ControlsData> newControlsData = new List<ControlsData>();
                                foreach (var cntrl in ControlsData)
                                {
                                    if (cntrl.IsReadOnly != true)
                                    {
                                        if (cntrl.ControlName.ToLower() == "radio" && !(radioCntrlOndoc.Contains(cntrl.ControlHtmlID)))
                                        {
                                            List<ControlsData> radiogrp = new List<ControlsData>();
                                            radiogrp = controlsInfo.ControlsData.Where(x => x.GroupName == cntrl.GroupName && x.ControlName.ToLower() == "radio" && x.IsCurrentRecipient == true).ToList();
                                            foreach (var radio in radiogrp)
                                            {
                                                radioCntrlOndoc.Add(radio.ControlHtmlID);
                                                radio.Required = cntrl.Required;
                                                newControlsData.Add(radio);
                                            }
                                        }
                                        else if (cntrl.ControlName.ToLower() == "checkbox" && !(checkboxCntrlOndoc.Contains(cntrl.ControlHtmlID)))
                                        {
                                            if (!string.IsNullOrEmpty(cntrl.GroupName))
                                            {
                                                List<ControlsData> checkgrp = new List<ControlsData>();
                                                checkgrp = controlsInfo.ControlsData.Where(x => x.GroupName == cntrl.GroupName && x.ControlName.ToLower() == "checkbox" && x.IsCurrentRecipient == true).ToList();
                                                foreach (var check in checkgrp)
                                                {
                                                    checkboxCntrlOndoc.Add(check.ControlHtmlID);
                                                    check.Required = cntrl.Required;
                                                    newControlsData.Add(check);
                                                }
                                            }
                                            else
                                            {
                                                newControlsData.Add(cntrl);
                                            }
                                        }
                                        else if (cntrl.ControlName != "Label" && cntrl.ControlName != "Checkbox" && cntrl.ControlName.ToLower() != "radio" && cntrl.ControlName.ToLower() != "datetimestamp" && cntrl.ControlName != "Hyperlink")
                                        {
                                            newControlsData.Add(cntrl);
                                        }
                                    }
                                }

                                CheckListData checkListData = new CheckListData();
                                checkListData.PageNumber = i;
                                checkListData.ControlsData = newControlsData;
                                checkListData.DocumentName = documentName;
                                checkListData.DocumentId = tempDocId;
                                lstCheckListData.Add(checkListData);
                            }
                        }
                    }
                }
                else
                {
                    foreach (DocumentDetails docData in DocumentNameList)
                    {
                        CheckListData checkListData = new CheckListData();
                        checkListData.PageNumber = null;
                        checkListData.ControlsData = null;
                        checkListData.DocumentName = docData.DocumentName;
                        checkListData.DocumentId = docData.ID;
                        lstCheckListData.Add(checkListData);
                    }
                }

                #endregion CheckListData

                #region ControlsData
                var pageNo = 1;
                foreach (var envelope in controlsInfo.EnvelopeImageCollection)
                {
                    string imageid = Convert.ToString(envelope.Id);
                    string pageNumber = pageNo.ToString();
                    int DocPageNo = envelope.DocPageNo;
                    string imgControlWidth = "";
                    if (controlsInfo.IsTemplateDatedBeforePortraitLandscapeFeature == true)
                        imgControlWidth = "";
                    else
                    {
                        if (Convert.ToInt32(envelope.Dimension.Width) > Convert.ToInt32(envelope.Dimension.Height))
                            imgControlWidth = "1015px";
                        else
                            imgControlWidth = "915px";
                    }

                    SigningEnvelopeDocumentData signingEnvelopeDocumentData = new SigningEnvelopeDocumentData();
                    signingEnvelopeDocumentData.PageNum = pageNo;
                    signingEnvelopeDocumentData.DocId = envelope.Document.Id;
                    signingEnvelopeDocumentData.DocName = envelope.Document.Name;
                    signingEnvelopeDocumentData.IsPageLoaded = true;


                    string imgPath = "";
                    if (controlsInfo.SubEnvelopeId != null && controlsInfo.SubEnvelopeId != Guid.Empty)
                        imgPath = "/" + imageid + "/" + Convert.ToString(controlsInfo.GlobalEnvelopeID) + "/" + envelope.ImagePath.Substring(envelope.ImagePath.LastIndexOf('/') + 1) + "/" + responseMessage.UNCPath;
                    else if (controlsInfo.SubEnvelopeId == null && controlsInfo.IsTemplateShared == true && responseMessage.Prefill == "prefill")
                        imgPath = "/" + imageid + "/" + Convert.ToString(controlsInfo.GlobalEnvelopeID) + "/" + envelope.ImagePath.Substring(envelope.ImagePath.LastIndexOf('/') + 1) + "/" + responseMessage.UNCPath;
                    else
                        imgPath = "/" + imageid + "/" + Convert.ToString(controlsInfo.GlobalEnvelopeID) + "/" + responseMessage.UNCPath;

                    foreach (var item in controlsInfo.ControlsData)
                    {
                        if (item.DocumentId == envelope.Document.Id && (item.PageNo == envelope.PageNo))
                        {
                            item.LanguageControlName = labelText.Single(a => a.KeyName.ToLower() == item.ControlName.ToLower()).KeyValue;

                            if (item.ControlName.ToLower() == "checkbox" && !string.IsNullOrEmpty(item.GroupName) && item.Required != true)
                            {
                                var checkgrp = controlsInfo.ControlsData.Where(x => x.GroupName == item.GroupName && x.ControlName.ToLower() == "checkbox" && x.IsCurrentRecipient == true).ToList();
                                if (checkgrp.Any(rg => rg.Required != null && rg.Required == true))
                                {
                                    item.Required = true;
                                }
                            }

                            var style = item.ControlHtmlData;
                            string tempTop = "";
                            string left = (item.Left == 0.0 ? style.Substring(style.IndexOf("left"), (style.IndexOf("px") - style.IndexOf("left"))) : item.Left.ToString()) + "px";

                            int topIndex = style.IndexOf("top");
                            int positionIndex = style.IndexOf("position");
                            int topValueIndex = positionIndex < topIndex ? (style.IndexOf(";", topIndex) + 2) : positionIndex;
                            string top = (item.Top == 0.0 ? style.Substring(topIndex, (topValueIndex - (topIndex + 2))) : item.Top.ToString());

                            if (item.ControlName == "Name" || item.ControlName == "Email" || item.ControlName == "Title" || item.ControlName == "Label" || item.ControlName == "Company")
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 2) + "px";
                            }
                            else if (item.ControlName == "Text")
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                            }
                            else if (item.ControlName == "DropDown")
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 4) + "px";
                            }
                            else if (item.ControlName == "Checkbox" && item.Height == 14)
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                                left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";
                            }
                            else if (item.ControlName == "Radio" && item.Height == 14)
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                                left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";
                            }

                            item.CalculatedTop = top;
                            item.CalculatedLeft = left;
                            item.HoverTitle = item.Required || (item.ControlName == "DropDown" && !item.Required && item.ControlOptions[0].OptionText != "") ? "Required" : "Optional";
                            item.CalculatedModalWidth = item.ControlName.ToLower() == "text" || item.ControlName.ToLower() == "name" ? item.Width + 1 : item.Width - 5;

                            if (controlsInfo.ElectronicSignIndication == null)
                            {
                                controlsInfo.ElectronicSignIndication = 1;
                            }
                            if (item.ControlName == "Date")
                            {
                                var dateplaceholder = item.Label;
                                if (dateplaceholder == "dd-mmm-yyyy")
                                    item.DefaultDateFormat = dateplaceholder.Replace("mmm", "MMM");
                                else
                                    item.DefaultDateFormat = dateplaceholder.Replace("mm", "MM");
                            }

                            var ExistSameRecipient = false;
                            if (SameRecipientIds != null && SameRecipientIds.Count > 0 && item.RecipientId != null && item.RecipientId != Guid.Empty)
                                ExistSameRecipient = SameRecipientIds.Contains(item.RecipientId.Value);

                            item.ExistSameRecipient = ExistSameRecipient;
                            item.EditControls = CanEdit;

                            if (item.IsCurrentRecipient == true || ExistSameRecipient == true || (CanEdit == true && (item.ControlName != "Signature" && item.ControlName != "Email" && item.ControlName != "NewInitials")))
                            {
                                if (item.ControlName == "Signature" && controlsInfo.ElectronicSignIndication > 1 && !string.IsNullOrEmpty(item.SignatureScr) && !(Convert.ToBoolean(item.IsSignatureFromDocumentContent)))
                                {
                                    int width; int height;
                                    item.SignatureScr = _envelopeHelperMain.ConvertSignImageWithStamp(item.SignatureScr, out height, out width, controlsInfo.EDisplayCode, controlsInfo.ElectronicSignIndication, DateFormat, controlsInfo.TimeZoneSettingOptionValue, Convert.ToString(controlsInfo.DateFormatID), Convert.ToString(responseMessage.SignatureTypeID));
                                }

                                if (item.ControlName == "Text")
                                {
                                    string textType = "";
                                    string textTypeValue = "";
                                    string textTypeMask = "";

                                    switch (item.ControlType)
                                    {
                                        case "D6FBBFC2-C907-4290-929F-175EB437AA81":
                                        case "D1409FCF-5683-4921-A62B-2F635F4E49B7":
                                        case "B0443A47-89C3-4826-BECC-378D81738D03":
                                        case "C175A449-3A22-4FE0-A009-C3F76F612510":
                                        case "73C17C33-F255-474F-9F46-248542ADDACC":
                                            textType = "Numeric";
                                            textTypeValue = "";
                                            textTypeMask = "";
                                            break;
                                        case "F01331D9-3413-466A-9821-2670A8D9F3EE":
                                        case "26C0ACEA-3CC8-43D6-A255-A870A8524A77":
                                        case "CBAF463C-8287-4C04-B90C-C6E2F1EC5299":
                                        case "F690C267-D10F-40AD-A487-D2035D9C3858":
                                        case "126AF3B7-409E-425E-A9C3-A313254ACB03":
                                            textType = "Text";
                                            break;
                                        case "88A0B11E-5810-4ABF-A8B6-856C436E7C49":
                                            textType = "Alphabet";
                                            break;
                                        case "8348E5CD-59EA-4A77-8436-298553D286BD":
                                            textType = "Date";
                                            break;
                                        case "DCBBE75C-FDEC-472C-AE25-2C42ADFB3F5D":
                                            textType = "SSN";
                                            textTypeValue = "___-__-____";
                                            textTypeMask = "___-__-____";
                                            break;
                                        case "5121246A-D9AB-49F4-8717-4EF4CAAB927B":
                                            textType = "Zip";
                                            break;
                                        case "1AD2D4EC-4593-435E-AFDD-F8A90426DE96":
                                            textType = "Email";
                                            break;
                                    }

                                    switch (item.AdditionalValidationOption)
                                    {
                                        case "Zip":
                                            textTypeValue = "_____";
                                            textTypeMask = "_____";
                                            break;
                                        case "Zip+4":
                                            textTypeValue = "_____-____";
                                            textTypeMask = "_____-____";
                                            break;
                                        case "mm/dd/yyyy":
                                            textTypeValue = "mm/dd/yyyy";
                                            textTypeMask = "mm/dd/yyyy";
                                            break;
                                        case "dd/mm/yyyy":
                                            textTypeValue = "dd/mm/yyyy";
                                            textTypeMask = "dd/mm/yyyy";
                                            break;
                                        case "yyyy/mm/dd":
                                            textTypeValue = "yyyy/mm/dd";
                                            textTypeMask = "yyyy/mm/dd";
                                            break;
                                        case "Period":
                                            textTypeValue = "Period";
                                            textTypeMask = "Period";
                                            break;
                                        case "Comma":
                                            textTypeValue = "Comma";
                                            textTypeMask = "Comma";
                                            break;
                                        case "Both":
                                            textTypeValue = "Both";
                                            textTypeMask = "Both";
                                            break;
                                        case "dd-mmm-yyyy":
                                            textTypeValue = "dd-mmm-yyyy";
                                            textTypeMask = "dd-mmm-yyyy";
                                            break;
                                    }

                                    item.TextType = textType;
                                    item.TextTypeValue = textTypeValue;
                                    item.TextTypeMask = textTypeMask;

                                    int inputMaxLength = 0;
                                    if (textType != "Date" && textType != "Email")
                                    {
                                        if (style.IndexOf("maxlengthallowed") > -1)
                                        {
                                            item.MaxInputLength = Regex.Replace(style.Substring(style.IndexOf("maxlengthallowed") + 18, 4), "[^0-9]+", string.Empty);
                                            inputMaxLength = Convert.ToInt32(item.MaxInputLength);
                                        }
                                        else
                                        {
                                            item.MaxInputLength = "";
                                            inputMaxLength = 0;
                                        }
                                    }

                                    if (textType == "Alphabet" || textType == "Text" || textType == "Numeric")
                                    {
                                        inputMaxLength = inputMaxLength == 0 ? 1 : Convert.ToInt32(item.MaxInputLength);
                                        if (string.IsNullOrEmpty(item.CustomToolTip))
                                        {
                                            if (textType == "Numeric")
                                                item.CustomToolTip = "Maximum " + inputMaxLength + " digits (0-9) allowed.";
                                            else
                                                item.CustomToolTip = "Approximately " + inputMaxLength + " characters or the control boundaries.";
                                        }
                                    }
                                }
                                else if (item.ControlName == "Name")
                                {
                                    int inputMaxLength = 0;
                                    if (style.IndexOf("maxlengthallowed") > -1)
                                    {
                                        item.MaxInputLength = style.Substring(style.IndexOf("maxlengthallowed") + 18, 4);
                                        var isNumber = Regex.IsMatch(item.MaxInputLength, @"^\d+$");
                                        if (!isNumber)
                                        {
                                            item.MaxInputLength = Regex.Replace(item.MaxInputLength, "[^0-9]+", string.Empty);
                                        }
                                        if (!string.IsNullOrEmpty(item.MaxInputLength))
                                        {
                                            inputMaxLength = Convert.ToInt32(item.MaxInputLength);
                                        }
                                        else
                                        {
                                            inputMaxLength = 10;
                                        }
                                    }
                                    else
                                    {
                                        item.MaxInputLength = "";
                                        inputMaxLength = 0;
                                    }
                                    inputMaxLength = inputMaxLength == 0 ? 1 : inputMaxLength;
                                    if (string.IsNullOrEmpty(item.CustomToolTip))
                                    {
                                        item.CustomToolTip = "Approximately " + inputMaxLength + " characters or the control boundaries.";
                                    }
                                }
                            }
                            else if (item.IsSigned)
                            {
                                try
                                {
                                    tempTop = top.Replace("top:", "").Replace("top :", "").Replace("px", "").Trim();
                                    if (item.ControlName == "Signature" || item.ControlName == "NewInitials" || item.ControlName == "DateTimeStamp" || item.ControlName == "Date")
                                    {
                                        tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop)) + "px";
                                    }
                                    else if (item.ControlName == "Title" || item.ControlName == "Company" || item.ControlName == "Email" || item.ControlName == "Radio" || item.ControlName == "Checkbox")
                                    {
                                        tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop) + 2.0) + "px";
                                    }
                                    else
                                    {
                                        tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop) + 5.0) + "px";
                                    }
                                    top = tempTop;
                                }
                                catch (Exception) { }

                                item.CalculatedTop = top;
                            }
                        }
                    }

                    EnvelopeImageControlData envelopeImageControlData = new EnvelopeImageControlData();
                    envelopeImageControlData.Id = envelope.Id;
                    envelopeImageControlData.PageNum = pageNumber;
                    envelopeImageControlData.DocPageNo = DocPageNo;
                    envelopeImageControlData.ImgControlWidth = imgControlWidth;
                    envelopeImageControlData.ImagePath = imgPath;
                    envelopeImageControlData.SigningEnvelopeDocumentData = signingEnvelopeDocumentData;
                    envelopeImageControlData.ControlsData = controlsInfo.ControlsData.Where(c => c.DocumentId == envelope.Document.Id && c.PageNo == envelope.PageNo).ToList();
                    lstEnvelopeImageControlData.Add(envelopeImageControlData);
                    pageNo++;
                }

                if (controlsInfo.AllDocumentControls != null)
                {
                    foreach (var itemCtrl in controlsInfo.AllDocumentControls)
                    {
                        var style = itemCtrl.ControlHtmlData;
                        string tempTop = "";
                        string left = (itemCtrl.Left == 0.0 ? style.Substring(style.IndexOf("left"), (style.IndexOf("px") - style.IndexOf("left"))) : itemCtrl.Left.ToString()) + "px";

                        int topIndex = style.IndexOf("top");
                        int positionIndex = style.IndexOf("position");
                        int topValueIndex = positionIndex < topIndex ? (style.IndexOf(";", topIndex) + 2) : positionIndex;
                        string top = (itemCtrl.Top == 0.0 ? style.Substring(topIndex, (topValueIndex - (topIndex + 2))) : itemCtrl.Top.ToString());

                        if (itemCtrl.ControlName == "Name" || itemCtrl.ControlName == "Email" || itemCtrl.ControlName == "Title" || itemCtrl.ControlName == "Label" || itemCtrl.ControlName == "Company")
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 2) + "px";
                        }
                        else if (itemCtrl.ControlName == "Text")
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                        }
                        else if (itemCtrl.ControlName == "DropDown")
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 4) + "px";
                        }
                        else if (itemCtrl.ControlName == "Checkbox" && itemCtrl.Height == 14)
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                            left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";
                        }
                        else if (itemCtrl.ControlName == "Radio" && itemCtrl.Height == 14)
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                            left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";
                        }

                        itemCtrl.CalculatedTop = top;
                        itemCtrl.CalculatedLeft = left;
                        itemCtrl.CalculatedModalWidth = itemCtrl.ControlName.ToLower() == "text" || itemCtrl.ControlName.ToLower() == "name" ? itemCtrl.Width + 1 : itemCtrl.Width - 5;
                        if (itemCtrl.IsSigned)
                        {
                            try
                            {
                                tempTop = top.Replace("top:", "").Replace("top :", "").Replace("px", "").Trim();
                                if (itemCtrl.ControlName == "Signature" || itemCtrl.ControlName == "NewInitials" || itemCtrl.ControlName == "DateTimeStamp")
                                {
                                    tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop)) + "px";
                                }
                                else
                                {
                                    tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop) + 5.0) + "px";
                                }
                                top = tempTop;
                            }
                            catch (Exception) { }

                            itemCtrl.CalculatedTop = top;
                        }
                    }
                }

                #endregion ControlsData                

                #endregion Signer Landing from Angular

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                Message = "The signer details reterived successully.";
                loggerModelNew.Message = "Process completed for Initialize Sign Document and " + responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseMessage.EnvelopeInfo = controlsInfo;
                responseMessage.documentDetails = documentDetails;
                responseMessage.EnvelopeAdditionalUploadInfoList = envelopeAdditionalUploadInfoDetails;
                responseMessage.SameRecipientIds = SameRecipientIds;
                responseMessage.CheckListData = lstCheckListData;
                responseMessage.EnvelopeImageControlData = lstEnvelopeImageControlData;
                responseMessage.RequiresSignersConfirmationonFinalSubmit = adminGeneralAndSystemSettings.RequiresSignersConfirmationonFinalSubmit;
                responseMessage.IncludeStaticTemplates = adminGeneralAndSystemSettings.IncludeStaticTemplates;
                responseMessage.ReVerifySignerDocumentSubmit = Convert.ToBoolean(envelopeObject.ReVerifySignerDocumentSubmit);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseSigningUrlModel.EnvelopeID = envelopeID;
                responseSigningUrlModel.EmailId = emailID;
                responseMessage.ResponseSigningUrlModel = responseSigningUrlModel;
                return BadRequest(responseMessage);
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForInitalizeSignDocument), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("InitializeMultiSignerStaticDocument")]
        [HttpPost]
        public async Task<IActionResult> InitializeMultiSignerStaticDocument(RequestSigningUrlModel signingUrlModel)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "InitializeMultiSignerStaticDocument", "Process started for Initialize Multi Signer Static Document", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            InitializeMultiSignDocumentAPI objSignParam = new InitializeMultiSignDocumentAPI();
            ResponseMessageForInitalizeSignDocument responseMessage = new ResponseMessageForInitalizeSignDocument();

            bool IsUnitTestApp = false;
            string Message = string.Empty;
            string envelopeCode = string.Empty;
            string emailId = string.Empty;
            var strtemplateID = "";
            var strtemplateKey = "";
            string recipientID = string.Empty;
            string CurrentEmail = string.Empty;

            string userURL = signingUrlModel.SigningUrl;
            userURL = HttpUtility.UrlDecode(userURL);
            if (!userURL.Equals(""))
            {
                userURL = EncryptDecryptQueryString.Decrypt(userURL, Convert.ToString(_appConfiguration["AppKey"]));
                if (!userURL.Equals("Invalid length for a Base-64 char array or string.") && !userURL.Equals("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters. ") && !userURL.Equals("Length of the data to decrypt is invalid."))
                {
                    string[] arrayURL = userURL.Split('&');
                    if (arrayURL.Length == 2)
                    {
                        string[] arraystrtemplateID = arrayURL[0].Split('=');
                        strtemplateID = arraystrtemplateID.Length == 2 ? arraystrtemplateID[1].Trim() : string.Empty;
                        string[] arraystrtemplateKey = arrayURL[1].Split('=');
                        strtemplateKey = arraystrtemplateKey.Length == 2 ? arraystrtemplateKey[1].Trim() : string.Empty;
                    }
                    if (arrayURL.Length == 3)
                    {
                        string[] arraystrtemplateID = arrayURL[0].Split('=');
                        strtemplateID = arraystrtemplateID.Length == 2 ? arraystrtemplateID[1].Trim() : string.Empty;

                        string[] recipientIDKey = arrayURL[1].Split('=');
                        recipientID = recipientIDKey.Length == 2 ? recipientIDKey[1].Trim() : string.Empty;

                        string[] CurrentEmailKey = arrayURL[2].Split('=');
                        CurrentEmail = CurrentEmailKey.Length == 2 ? CurrentEmailKey[1].Trim() : string.Empty;
                    }
                    else
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        strtemplateID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                    }

                    objSignParam.EnvelopeId = strtemplateID;
                    objSignParam.RecipientId = recipientID.ToString();
                    objSignParam.EmailId = CurrentEmail;
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "The URL of the template is incorrect.";
                    return BadRequest(responseMessage);
                }
            }
            else
            {
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "The URL of the template is incorrect.";
                return BadRequest(responseMessage);
            }

            if (string.IsNullOrEmpty(strtemplateID))
            {
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "The URL of the template is incorrect.";
                return BadRequest(responseMessage);
            }

            Guid templateID = new Guid(objSignParam.EnvelopeId);
            if (templateID == Guid.Empty)
            {
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "The URL of the template is incorrect.";
                return BadRequest(responseMessage);
            }

            Guid recipientId = Guid.Empty;
            if (!String.IsNullOrEmpty(objSignParam.RecipientId))
            {
                recipientId = new Guid(objSignParam.RecipientId);
            }

            string currentRecipientEmailId = objSignParam.EmailId;
            string senderEmail = string.Empty;
            string currentenvelopeID = string.Empty;
            EnvelopeDetails envelopeDetails = new EnvelopeDetails();
            DashBoard dashBoard = new DashBoard();
            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            APISettings aPISettings = new APISettings();
            Guid signerStatusId = Guid.Empty;
            EnvelopeInfo controlsInfo = new EnvelopeInfo();
            string folderFileSize = "0";
            string finalDocName = string.Empty;
            Template envelopeObject = new Template();
            bool IsTemplateDatedBeforePortraitLandscapeFeature = false;
            List<DocumentDetails> documentDetails = new List<DocumentDetails>();
            List<DocumentDetails> nonReviewDocumentDetails = new List<DocumentDetails>();
            List<RolsInfo> rolsInfos = new List<RolsInfo>();
            List<CheckListData> lstCheckListData = new List<CheckListData>();
            List<EnvelopeImageControlData> lstEnvelopeImageControlData = new List<EnvelopeImageControlData>();
            string templateFolderNasPath = string.Empty;
            try
            {
                envelopeObject = _genericRepository.GetTemplateEntity(templateID);

                if (envelopeObject == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"].ToString());
                    responseMessage.EnvelopeInfo = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                if (envelopeObject.IsActive == false)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "The sender has deleted the envelope. For further information, please contact the sender.";
                    responseMessage.EnvelopeInfo = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
               // var sender = _userRepository.GetUserProfileByUserID(envelopeObject.UserID);
               //if (sender != null)
                   // responseMessage.InfoSenderEmail = sender.EmailID;              

                /*RS-428: URGENT - All RSign controls shifted down and right - solve with highest priority*/
                if (envelopeObject.ID != null)
                {
                    var dateTimeBeforePortraitLandscape = Convert.ToDateTime(_appConfiguration["dateTimeBeforePortraitLandscape"]);
                    if (envelopeObject.CreatedDateTime <= dateTimeBeforePortraitLandscape)
                        IsTemplateDatedBeforePortraitLandscapeFeature = true;
                }

                // Creating object graph to load the child entity.                    
                var documents = envelopeObject.TemplateDocuments.ToList();
                foreach (var doc in documents)
                {
                    DocumentDetails document = new DocumentDetails();
                    document.ID = doc.ID;
                    //document.EnvelopeID = doc.EnvelopeID;
                    document.DocumentName = doc.DocumentName;
                    document.DocumentSource = doc.DocumentSource;
                    document.ActionType = doc.ActionType;
                    nonReviewDocumentDetails.Add(document);
                }
                int order = 1;
                Guid CurrentRoleId = Guid.Empty;
                var rolesData = envelopeObject.TemplateRoles.OrderBy(d => d.CreatedDateTime).ToList();
                bool IsRequiredRole = false;
                templateFolderNasPath = _modelHelper.GetTemplateDirectory(templateID, string.Empty);

                foreach (var row in rolesData)
                {
                    RolsInfo rolsInfo = new RolsInfo();
                    rolsInfo.RecipientId = row.ID;
                    rolsInfo.order = envelopeObject.IsSequenceCheck == true ? Convert.ToInt32(row.Order) : order;
                    rolsInfo.isRequired = false;
                    rolsInfo.RoleName = row.Name;
                    rolsInfo.CultureInfo = row.CultureInfo;
                    rolsInfo.DeliveryMode = Convert.ToString(row.DeliveryMode);
                    rolsInfo.DialCode = row.DialCode != null ? row.DialCode : "";
                    rolsInfo.CountryCode = row.CountryCode != null ? row.CountryCode : "";
                   // rolsInfo.ReminderType = row.ReminderType;

                    foreach (var document in documents)
                    {
                        if (document.TemplateDocumentContents != null)
                        {
                            IsRequiredRole = document.TemplateDocumentContents.Any(d => row.ID == d.RecipientID && Convert.ToString(d.ControlID).ToUpper() == "E294C207-13FD-4508-95FC-90C5D9C555FA" && d.Required);
                            if (IsRequiredRole)
                            {
                                rolsInfo.isRequired = true;
                                break;
                            }
                        }
                    }

                    rolsInfos.Add(rolsInfo);
                    order++;
                }

                CurrentRoleId = recipientId;

                var TempDocuments = envelopeObject.TemplateDocuments.Where(d => d.ActionType == Constants.ActionTypes.Review).ToList();
                if (TempDocuments.Count > 0)
                {
                    foreach (var doc in TempDocuments)
                    {
                        DocumentDetails document = new DocumentDetails();
                        document.ID = doc.ID;
                        //document.EnvelopeID = doc.EnvelopeID;
                        document.DocumentName = doc.DocumentName;
                        document.DocumentSource = doc.DocumentSource;
                        document.ActionType = doc.ActionType;
                        documentDetails.Add(document);
                    }
                }
                controlsInfo = _envelopeHelperMain.GetSignerLandingStaticTemplateInfo(templateID, envelopeObject);
                aPISettings = _settingsRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
                adminGeneralAndSystemSettings = _eSignHelper.TransformSettingsDictionaryToEntity(aPISettings);
                controlsInfo.SignatureCaptureHanddrawn = adminGeneralAndSystemSettings.SignatureCaptureHanddrawn;
                controlsInfo.UploadSignature = adminGeneralAndSystemSettings.UploadSignature;
                controlsInfo.ElectronicSignIndication = (byte)adminGeneralAndSystemSettings.ElectronicSignIndicationSelectedID;
                controlsInfo.SignatureCaptureType = adminGeneralAndSystemSettings.SignatureCaptureType;

                controlsInfo = _envelopeHelperMain.GetStaticDocumentControlsBYRoleId(controlsInfo, templateID, CurrentRoleId, currentRecipientEmailId);

                controlsInfo.IsSignerAttachFileReq = envelopeObject.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest ? true : false;
                controlsInfo.IsSignerAttachFileReqNew = envelopeObject.IsSignerAttachFileReq != null ? envelopeObject.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                bool isSignerattachmentProcess = false;
                if (Convert.ToInt32(envelopeObject.IsSignerAttachFileReq) > 0)
                {
                    isSignerattachmentProcess = envelopeObject.IsAdditionalAttmReq != null ? Convert.ToBoolean(envelopeObject.IsAdditionalAttmReq) : false;
                }
                controlsInfo.IsAdditionalAttmReq = isSignerattachmentProcess;
                /*Properties which are required in web project from Envelope Object (envelopeObject) assign that props to EnvelopeInfo */
                /*Start of Props*/

                UserProfile userProfile = _userRepository.GetLatestUserProfile(currentRecipientEmailId);
                UserData userData = new UserData();
                if (userProfile != null)
                {
                    userData.UserId = userProfile.UserID;
                    userData.UserName = userProfile.FirstName + " " + userProfile.LastName;
                    userData.UserInitials = userProfile.Initials;
                    if (userProfile.IsAutoPopulateSignaturewhileSinging)
                    {
                        userData.UserInitialsImgSrc = userProfile.SignatureImage == null ? null : "data:image/png;base64," + Convert.ToBase64String(userProfile.SignatureImage);
                    }
                    else
                    {
                        userData.UserInitialsImgSrc = null;
                    }
                    userData.SignatureTypeID = userProfile.SignatureTypeID;
                }

                else if (envelopeObject.TemplateRoles.Where(r => r.ID == recipientId).Count() > 0)
                {
                    foreach (var rec in envelopeObject.TemplateRoles.Where(r => r.ID == recipientId))
                    {
                        userData.UserId = rec.ID;
                        userData.UserName = rec.Name;
                        userData.UserInitials = null;
                        userData.UserInitialsImgSrc = null;
                        userData.SignatureTypeID = Guid.Empty;
                    }
                }

                responseMessage.userdata = userData;
                var senderUserProfile = _userRepository.GetUserProfileByUserID(envelopeObject.UserID);
                if (senderUserProfile != null)
                {
                    AdminGeneralAndSystemSettings companysettings = new AdminGeneralAndSystemSettings();
                    Guid CompanyID = (Guid)senderUserProfile.CompanyID;
                    var getSetting = _settingsRepository.GetEntityByParam(CompanyID, string.Empty, Constants.String.SettingsType.Company);
                    companysettings = _eSignHelper.TransformSettingsDictionaryToEntity(getSetting);
                    responseMessage.DisableFinishLaterOption = companysettings.DisableFinishLaterOption;
                    responseMessage.DisableDeclineOption = companysettings.DisableDeclineOption;
                    responseMessage.DisableChangeSigner = companysettings.DisableChangeSigner;
                    responseMessage.InfoSenderEmail = senderUserProfile.EmailID;
                    responseMessage.SendMessageCodetoAvailableEmailorMobile = companysettings.SendMessageCodetoAvailableEmailorMobile;
                }

                string recipientCultureInfo = envelopeObject.TemplateRoles.Where(t => t.ID == CurrentRoleId).OrderBy(r => r.CreatedDateTime).FirstOrDefault().CultureInfo;
                controlsInfo.CultureInfo = !string.IsNullOrEmpty(recipientCultureInfo) ? recipientCultureInfo : Convert.ToString(envelopeObject.CultureInfo);
                controlsInfo.DateFormatID = envelopeObject.DateFormatID;
                controlsInfo.PasswordReqdtoSign = envelopeObject.PasswordReqdtoSign;
                controlsInfo.GlobalEnvelopeID = templateID;
                controlsInfo.IsSignerAttachFileReq = (envelopeObject.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                controlsInfo.IsSignerAttachFileReqNew = envelopeObject.IsSignerAttachFileReq != null ? envelopeObject.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                controlsInfo.IsFinalCertificateReq = envelopeObject.IsFinalCertificateReq != true;
                controlsInfo.Controls = _masterDataRepository.GetControlID().Where(c => c.ID != Constants.Control.NewInitials).ToDictionary(x => x.ControlName, x => x.ControlName);
                controlsInfo.TimeZoneSettingOptionValue = adminGeneralAndSystemSettings.SelectedTimeZone.ToString();
                if (senderUserProfile != null)
                    controlsInfo.SenderEmail = senderUserProfile.EmailID;
                controlsInfo.IsTemplateDatedBeforePortraitLandscapeFeature = IsTemplateDatedBeforePortraitLandscapeFeature;

                //var disclaimer = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.Disclaimer);
                //var isDisclaimerInCertificate = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.IsDisclaimerInCertificate);
                //controlsInfo.IsDisclaimerEnabled = Convert.ToBoolean(isDisclaimerInCertificate.OptionValue);
                //controlsInfo.Disclaimer = Convert.ToBoolean(isDisclaimerInCertificate.OptionValue) ? Convert.ToString(disclaimer.OptionValue) : null;

                controlsInfo.IsDisclaimerEnabled = Convert.ToBoolean(adminGeneralAndSystemSettings.IsDisclaimerInCertificate);
                controlsInfo.Disclaimer = Convert.ToBoolean(adminGeneralAndSystemSettings.IsDisclaimerInCertificate) ? Convert.ToString(adminGeneralAndSystemSettings.Disclaimer) : null;

                controlsInfo.IsStatic = envelopeObject.IsStatic;
                controlsInfo.IsSendConfirmationEmail = Convert.ToBoolean(envelopeObject.SendConfirmationEmail);
                responseMessage.DialCodeDropdownList = _envelopeHelperMain.LoadDialingCountryCodes();
                responseMessage.AttachSignedPdfID = adminGeneralAndSystemSettings.AttachSignedPdfID;
                responseMessage.IsAllowSignerstoDownloadFinalContract = Convert.ToBoolean(adminGeneralAndSystemSettings.IsAllowSignerstoDownloadFinalContract);
                responseMessage.UNCPath = _modelHelper.GetIdTemplateDirectory(templateFolderNasPath);
                responseMessage.EnableClickToSign = adminGeneralAndSystemSettings.EnableClickToSign;
                responseMessage.EnableAutoFillTextControls = envelopeObject.IsEnableAutoFillTextControls;
                responseMessage.IsDefaultSignatureForStaticTemplate = envelopeObject.IsDefaultSignatureForStaticTemplate;
                responseMessage.AllowMultipleSigner = Convert.ToBoolean(envelopeObject.AllowMultiSigner);
                responseMessage.AllowStaticMultiSigner = Convert.ToBoolean(envelopeObject.AllowMultiSigner);
                responseMessage.EnableMessageToMobile = envelopeObject.EnableMessageToMobile;
                responseMessage.EnvelopeInfo = controlsInfo;
                responseMessage.documentDetails = documentDetails;
                responseMessage.TemplateRolesInfo = rolsInfos;
                responseMessage.CurrentRoleID = objSignParam.RecipientId;
                responseMessage.CurrentEmail = currentRecipientEmailId;
                responseMessage.SignGlobalTemplateKey = Convert.ToString(envelopeObject.TemplateKey);
                responseMessage.RequiresSignersConfirmationonFinalSubmit = adminGeneralAndSystemSettings.RequiresSignersConfirmationonFinalSubmit;
                responseMessage.IncludeStaticTemplates = adminGeneralAndSystemSettings.IncludeStaticTemplates;
                responseMessage.ReVerifySignerStaticTemplate = Convert.ToBoolean(envelopeObject.ReVerifySignerStaticTemplate);

                #region signer landing code
                responseMessage.EnableClickToSign = responseMessage.TemplateRolesInfo.Count > 1 ? false : adminGeneralAndSystemSettings.EnableClickToSign; // adminGeneralAndSystemSettings.EnableClickToSign;                
                if (responseMessage.EnvelopeInfo != null)
                    controlsInfo = _templateRepository.FillEnvelopeInfoFromInitalizeSignDocumentAPI(responseMessage);

                Guid envelopeID = Guid.NewGuid();
                if (responseMessage.EnvelopeInfo != null && responseMessage.EnvelopeInfo.IsSignerAttachFileReqNew > 0)
                {
                    if ((templateID == null || envelopeID == null))
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = "EnvelopeID  or TemplateID not found";
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }

                    var additionalUploadInfoList = _envelopeRepository.GetTemplateAdditionalDocument(templateID);
                    _envelopeRepository.SaveEnvelopeAdditionalAttachment(additionalUploadInfoList, envelopeID);
                    List<EnvelopeAdditionalUploadInfoDetails> envelopeAdditionalUploadInfoDetails = new List<EnvelopeAdditionalUploadInfoDetails>();
                    responseMessage.EnvelopeAdditionalUploadInfoList = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByEnvelopebyRecipeint(envelopeID, Guid.Empty);
                    responseMessage.MaxUploadID = _envelopeRepository.GetMaxUploadsID();
                }
                else
                {
                    responseMessage.EnvelopeAdditionalUploadInfoList = null;
                    responseMessage.MaxUploadID = null;
                }

                #region DateFormat
                string DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_slash;
                string DateFormat = "";
                if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_slash;
                    DateFormat = "mm/dd/yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_colan;
                    DateFormat = "mm-dd-yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_dots;
                    DateFormat = "mm.dd.yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_slash))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_slash;
                    DateFormat = "dd/mm/yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_colan;
                    DateFormat = "dd-mm-yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_dots;
                    DateFormat = "dd.mm.yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_yyyy_mm_dd_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_yyyy_mm_dd_dots;
                    DateFormat = "yy.mm.dd.";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_dd_mmm_yyyy_colan;
                    DateFormat = "dd-mmm-yy";
                }
                #endregion DateFormat           

                responseMessage.DatePlaceHolder = DatePlaceHolder;
                responseMessage.DateFormat = DateFormat;
                responseMessage.FileReviewInfo = documentDetails.Where(d => d.ActionType == Constants.ActionTypes.Review).ToList();
                responseMessage.FileReviewCount = documentDetails.Where(d => d.ActionType == Constants.ActionTypes.Review).Count();
                responseMessage.DocumentNameList = nonReviewDocumentDetails.Where(d => d.ActionType != Constants.ActionTypes.Review).ToList();

                var DefaultplaceHolder = string.Empty;
                if (DatePlaceHolder == "dd-mmm-yyyy")
                    responseMessage.DefaultplaceHolder = DatePlaceHolder.Replace("mmm", "MMM");
                else
                    responseMessage.DefaultplaceHolder = DatePlaceHolder.Replace("mm", "MM");

                responseMessage.Language = _lookupRepository.GetLookupLanguageList(Lookup.LanguageKeyDetails, Convert.ToString(responseMessage.EnvelopeInfo.CultureInfo) == string.Empty ? "en-us" : responseMessage.EnvelopeInfo.CultureInfo, Constants.String.languageKeyType.Label);
                var labelText = responseMessage.Language;

                controlsInfo.GlobalEnvelopeID = new Guid(objSignParam.EnvelopeId);
                responseMessage.StaticEnvelopId = envelopeID;
                responseMessage.CurrentRecipientID = Guid.Empty;

                #region CheckListData  

                int? pageCount = controlsInfo.ControlsData.Max(x => x.PageNo);
                pageCount = pageCount == null ? 0 : pageCount.Value;
                var pageCounter = controlsInfo.EnvelopeImageCollection.Count();
                responseMessage.PageCount = pageCount;
                var IsAdditionAttamRequest = false;
                if (((bool)controlsInfo.IsAdditionalAttmReq) || (responseMessage.EnvelopeAdditionalUploadInfoList != null && responseMessage.EnvelopeAdditionalUploadInfoList.Count > 0))
                {
                    IsAdditionAttamRequest = true;
                    responseMessage.IsAdditionAttamRequest = IsAdditionAttamRequest;
                }
                else
                {
                    responseMessage.IsAdditionAttamRequest = false;
                }

                List<string> radioCntrlOndoc = new List<string>();
                List<string> checkboxCntrlOndoc = new List<string>();
                responseMessage.IsAnySignatureExists = controlsInfo.ControlsData.Any(c => c.ControlName == "Signature" && (c.IsCurrentRecipient == true));
                if (responseMessage.IsAnySignatureExists == false)
                {
                    responseMessage.ShowDefaultSignatureContol = "block";
                }
                else
                {
                    responseMessage.ShowDefaultSignatureContol = "none";
                }

                if (pageCount > 0)
                {
                    if (controlsInfo.ControlsData.Count > 0)
                    {
                        for (var i = 1; i <= pageCount; i++)
                        {
                            if (controlsInfo.ControlsData.Any(x => x.PageNo == i && x.ControlName != "Label" && x.ControlName.ToLower() != "datetimestamp" && x.ControlName != "Hyperlink"))
                            {
                                List<ControlsData> ControlsData = controlsInfo.ControlsData.OrderBy(item => item.TabIndex).Where(x => x.PageNo == i).ToList();
                                List<ControlsData> newControlsData = new List<ControlsData>();
                                foreach (var cntrl in ControlsData)
                                {
                                    if (cntrl.IsReadOnly != true)
                                    {
                                        if (cntrl.ControlName.ToLower() == "radio" && !(radioCntrlOndoc.Contains(cntrl.ControlHtmlID)))
                                        {
                                            List<ControlsData> radiogrp = new List<ControlsData>();
                                            radiogrp = controlsInfo.ControlsData.Where(x => x.GroupName == cntrl.GroupName && x.ControlName.ToLower() == "radio").ToList();
                                            foreach (var radio in radiogrp)
                                            {
                                                radioCntrlOndoc.Add(radio.ControlHtmlID);
                                                radio.Required = cntrl.Required;
                                                newControlsData.Add(radio);
                                            }
                                        }
                                        else if (cntrl.ControlName.ToLower() == "checkbox" && !(checkboxCntrlOndoc.Contains(cntrl.ControlHtmlID)))
                                        {
                                            if (!string.IsNullOrEmpty(cntrl.GroupName))
                                            {
                                                List<ControlsData> checkgrp = new List<ControlsData>();
                                                checkgrp = controlsInfo.ControlsData.Where(x => x.GroupName == cntrl.GroupName && x.ControlName.ToLower() == "checkbox").ToList();
                                                foreach (var check in checkgrp)
                                                {
                                                    checkboxCntrlOndoc.Add(check.ControlHtmlID);
                                                    check.Required = cntrl.Required;
                                                    newControlsData.Add(check);
                                                }
                                            }
                                            else
                                            {
                                                newControlsData.Add(cntrl);
                                            }
                                        }
                                        else if (cntrl.ControlName != "Label" && cntrl.ControlName != "Checkbox" && cntrl.ControlName.ToLower() != "radio" && cntrl.ControlName.ToLower() != "datetimestamp" && cntrl.ControlName != "Hyperlink")
                                        {
                                            newControlsData.Add(cntrl);
                                        }
                                    }
                                }

                                CheckListData checkListData = new CheckListData();
                                checkListData.PageNumber = i;
                                checkListData.ControlsData = newControlsData;
                                checkListData.DocumentName = "";
                                checkListData.DocumentId = Guid.Empty;
                                lstCheckListData.Add(checkListData);
                            }
                        }
                    }
                }
                else
                {
                    CheckListData checkListData = new CheckListData();
                    checkListData.PageNumber = null;
                    checkListData.ControlsData = null;
                    checkListData.DocumentName = "";
                    checkListData.DocumentId = Guid.Empty;
                    lstCheckListData.Add(checkListData);
                }

                #endregion CheckListData

                #region ControlsData
                var pageNo = 1;
                foreach (var envelope in controlsInfo.EnvelopeImageCollection)
                {
                    string imageid = Convert.ToString(envelope.Id);
                    string pageNumber = pageNo.ToString();
                    int DocPageNo = envelope.DocPageNo;
                    string imgControlWidth = "";
                    if (controlsInfo.IsTemplateDatedBeforePortraitLandscapeFeature == true)
                        imgControlWidth = "";
                    else
                    {
                        if (Convert.ToInt32(envelope.Dimension.Width) > Convert.ToInt32(envelope.Dimension.Height))
                            imgControlWidth = "1015px";
                        else
                            imgControlWidth = "915px";
                    }

                    SigningEnvelopeDocumentData signingEnvelopeDocumentData = new SigningEnvelopeDocumentData();
                    signingEnvelopeDocumentData.PageNum = pageNo;
                    signingEnvelopeDocumentData.DocId = envelope.Document.Id;
                    signingEnvelopeDocumentData.DocName = envelope.Document.Name;
                    signingEnvelopeDocumentData.IsPageLoaded = true;

                    string imgPath = "/" + imageid + "/" + Convert.ToString(controlsInfo.GlobalEnvelopeID) + "/" + responseMessage.UNCPath;

                    foreach (var item in controlsInfo.ControlsData)
                    {
                        if (item.DocumentId == envelope.Document.Id && (item.PageNo == envelope.PageNo))
                        {
                            item.LanguageControlName = labelText.Single(a => a.KeyName.ToLower() == item.ControlName.ToLower()).KeyValue;

                            if (item.ControlName.ToLower() == "checkbox" && !string.IsNullOrEmpty(item.GroupName) && item.Required != true)
                            {
                                var checkgrp = controlsInfo.ControlsData.Where(x => x.GroupName == item.GroupName && x.ControlName.ToLower() == "checkbox").ToList();
                                if (checkgrp.Any(rg => rg.Required != null && rg.Required == true))
                                {
                                    item.Required = true;
                                }
                            }

                            var style = item.ControlHtmlData;
                            string left = (item.Left == 0.0 ? style.Substring(style.IndexOf("left"), (style.IndexOf("px") - style.IndexOf("left"))) : item.Left.ToString()) + "px";

                            int topIndex = style.IndexOf("top");
                            int positionIndex = style.IndexOf("position");
                            string top = (item.Top == 0.0 ? style.Substring(topIndex, (positionIndex - (topIndex + 2))) : item.Top.ToString());

                            string Tagsleft = left.Replace("px", "");
                            Tagsleft = Tagsleft.Replace("left: ", "");

                            string[] topArray = top.Split(';');
                            if (topArray.Length > 1)
                            {
                                for (int i = 0; i < topArray.Length; i++)
                                {
                                    if (topArray[i].Contains("top"))
                                    {
                                        top = topArray[i];
                                        break;
                                    }
                                }
                            }

                            string Tagstop = top.Replace("px", "");
                            Tagstop = Tagstop.Replace("top: ", "");
                            if (item.ControlName == "Signature")
                            {
                                Tagstop = "top: " + (Convert.ToDecimal(Tagstop) + 38) + "px";
                            }
                            else
                            {
                                Tagstop = "top: " + (Convert.ToDecimal(Tagstop) + 18) + "px";
                            }

                            item.CalculatedModalWidth = item.ControlName.ToLower() == "text" || item.ControlName.ToLower() == "name" ? item.Width + 1 : item.Width - 4;
                            Tagsleft = "left: " + (Convert.ToDecimal(Tagsleft) + item.CalculatedModalWidth) + "px";

                            if (item.ControlName == "Checkbox" && item.Height == 15)
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                                left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";

                                Tagstop = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 20) + "px";
                                Tagsleft = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) + 10) + "px";
                            }
                            else if (item.ControlName == "Radio" && item.Height == 15)
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                                left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";

                                Tagstop = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 40) + "px";
                                Tagsleft = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) + 15) + "px";
                            }

                            item.CalculatedTop = top;
                            item.CalculatedLeft = left;
                            item.HoverTitle = item.Required || (item.ControlName == "DropDown" && !item.Required && item.ControlOptions[0].OptionText != "") ? "Required" : "Optional";
                            if (controlsInfo.ElectronicSignIndication == null)
                            {
                                controlsInfo.ElectronicSignIndication = 1;
                            }

                            if (item.ControlName == "Date")
                            {
                                var dateplaceholder = item.Label;
                                if (dateplaceholder == "dd-mmm-yyyy")
                                    item.DefaultDateFormat = dateplaceholder.Replace("mmm", "MMM");
                                else
                                    item.DefaultDateFormat = dateplaceholder.Replace("mm", "MM");
                            }

                            var SignatureTypeID = responseMessage.userdata != null ? responseMessage.userdata.SignatureTypeID : Guid.Empty;
                            if (item.ControlName == "Signature" && controlsInfo.ElectronicSignIndication > 1 && !string.IsNullOrEmpty(item.SignatureScr) && !(Convert.ToBoolean(item.IsSignatureFromDocumentContent)))
                            {
                                int width; int height;
                                item.SignatureScr = _envelopeHelperMain.ConvertSignImageWithStamp(item.SignatureScr, out height, out width, controlsInfo.EDisplayCode, controlsInfo.ElectronicSignIndication, DateFormat, controlsInfo.TimeZoneSettingOptionValue, Convert.ToString(controlsInfo.DateFormatID), Convert.ToString(SignatureTypeID));
                            }

                            if (item.ControlName == "Text")
                            {
                                string textType = "";
                                string textTypeValue = "";
                                string textTypeMask = "";

                                switch (item.ControlType)
                                {
                                    case "D6FBBFC2-C907-4290-929F-175EB437AA81":
                                    case "D1409FCF-5683-4921-A62B-2F635F4E49B7":
                                    case "B0443A47-89C3-4826-BECC-378D81738D03":
                                    case "C175A449-3A22-4FE0-A009-C3F76F612510":
                                    case "73C17C33-F255-474F-9F46-248542ADDACC":
                                        textType = "Numeric";
                                        textTypeValue = "";
                                        textTypeMask = "";
                                        break;
                                    case "F01331D9-3413-466A-9821-2670A8D9F3EE":
                                    case "26C0ACEA-3CC8-43D6-A255-A870A8524A77":
                                    case "CBAF463C-8287-4C04-B90C-C6E2F1EC5299":
                                    case "F690C267-D10F-40AD-A487-D2035D9C3858":
                                    case "126AF3B7-409E-425E-A9C3-A313254ACB03":
                                        textType = "Text";
                                        break;
                                    case "88A0B11E-5810-4ABF-A8B6-856C436E7C49":
                                        textType = "Alphabet";
                                        break;
                                    case "8348E5CD-59EA-4A77-8436-298553D286BD":
                                        textType = "Date";
                                        break;
                                    case "DCBBE75C-FDEC-472C-AE25-2C42ADFB3F5D":
                                        textType = "SSN";
                                        textTypeValue = "___-__-____";
                                        textTypeMask = "___-__-____";
                                        break;
                                    case "5121246A-D9AB-49F4-8717-4EF4CAAB927B":
                                        textType = "Zip";
                                        break;
                                    case "1AD2D4EC-4593-435E-AFDD-F8A90426DE96":
                                        textType = "Email";
                                        break;
                                }

                                switch (item.AdditionalValidationOption)
                                {
                                    case "Zip":
                                        textTypeValue = "_____";
                                        textTypeMask = "_____";
                                        break;
                                    case "Zip+4":
                                        textTypeValue = "_____-____";
                                        textTypeMask = "_____-____";
                                        break;
                                    case "mm/dd/yyyy":
                                        textTypeValue = "mm/dd/yyyy";
                                        textTypeMask = "mm/dd/yyyy";
                                        break;
                                    case "dd/mm/yyyy":
                                        textTypeValue = "dd/mm/yyyy";
                                        textTypeMask = "dd/mm/yyyy";
                                        break;
                                    case "yyyy/mm/dd":
                                        textTypeValue = "yyyy/mm/dd";
                                        textTypeMask = "yyyy/mm/dd";
                                        break;
                                    case "Period":
                                        textTypeValue = "Period";
                                        textTypeMask = "Period";
                                        break;
                                    case "Comma":
                                        textTypeValue = "Comma";
                                        textTypeMask = "Comma";
                                        break;
                                    case "Both":
                                        textTypeValue = "Both";
                                        textTypeMask = "Both";
                                        break;
                                    case "dd-mmm-yyyy":
                                        textTypeValue = "dd-mmm-yyyy";
                                        textTypeMask = "dd-mmm-yyyy";
                                        break;
                                }

                                item.TextType = textType;
                                item.TextTypeValue = textTypeValue;
                                item.TextTypeMask = textTypeMask;

                                int inputMaxLength = 0;
                                if (textType != "Date" && textType != "Email")
                                {
                                    if (style.IndexOf("maxlengthallowed") > -1)
                                    {
                                        item.MaxInputLength = Regex.Replace(style.Substring(style.IndexOf("maxlengthallowed") + 18, 4), "[^0-9]+", string.Empty);
                                        inputMaxLength = Convert.ToInt32(item.MaxInputLength);
                                    }
                                    else
                                    {
                                        item.MaxInputLength = "";
                                        inputMaxLength = 0;
                                    }
                                }

                                if (textType == "Alphabet" || textType == "Text" || textType == "Numeric")
                                {
                                    inputMaxLength = inputMaxLength == 0 ? 1 : Convert.ToInt32(item.MaxInputLength);
                                    if (string.IsNullOrEmpty(item.CustomToolTip))
                                    {
                                        if (textType == "Numeric")
                                            item.CustomToolTip = "Maximum " + inputMaxLength + " digits (0-9) allowed.";
                                        else
                                            item.CustomToolTip = "Approximately " + inputMaxLength + " characters or the control boundaries.";
                                    }
                                }
                            }
                            else if (item.ControlName == "Name")
                            {
                                int inputMaxLength = 0;
                                if (style.IndexOf("maxlengthallowed") > -1)
                                {
                                    item.MaxInputLength = style.Substring(style.IndexOf("maxlengthallowed") + 18, 4);
                                    var isNumber = Regex.IsMatch(item.MaxInputLength, @"^\d+$");
                                    if (!isNumber)
                                    {
                                        item.MaxInputLength = Regex.Replace(item.MaxInputLength, "[^0-9]+", string.Empty);
                                    }
                                    if (!string.IsNullOrEmpty(item.MaxInputLength))
                                    {
                                        inputMaxLength = Convert.ToInt32(item.MaxInputLength);
                                    }
                                    else
                                    {
                                        inputMaxLength = 10;
                                    }
                                }
                                else
                                {
                                    item.MaxInputLength = "";
                                    inputMaxLength = 0;
                                }
                                inputMaxLength = inputMaxLength == 0 ? 1 : inputMaxLength;
                                if (string.IsNullOrEmpty(item.CustomToolTip))
                                {
                                    item.CustomToolTip = "Approximately " + inputMaxLength + " characters or the control boundaries.";
                                }
                            }

                        }
                    }

                    EnvelopeImageControlData envelopeImageControlData = new EnvelopeImageControlData();
                    envelopeImageControlData.Id = envelope.Id;
                    envelopeImageControlData.PageNum = pageNumber;
                    envelopeImageControlData.DocPageNo = DocPageNo;
                    envelopeImageControlData.ImgControlWidth = imgControlWidth;
                    envelopeImageControlData.ImagePath = imgPath;
                    envelopeImageControlData.SigningEnvelopeDocumentData = signingEnvelopeDocumentData;
                    envelopeImageControlData.ControlsData = controlsInfo.ControlsData.Where(c => c.DocumentId == envelope.Document.Id && c.PageNo == envelope.PageNo).ToList();
                    lstEnvelopeImageControlData.Add(envelopeImageControlData);

                    pageNo++;
                }
                #endregion ControlsData

                #endregion signer landing code

                responseMessage.CheckListData = lstCheckListData;
                responseMessage.EnvelopeImageControlData = lstEnvelopeImageControlData;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                loggerModelNew.Message = "Process completed for Initialize Static Template using API and " + responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller InitializeMultiSignerStaticDocument action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.CurrentEmail = CurrentEmail;
                responseMessage.EnvelopeId = strtemplateID;
                return BadRequest(responseMessage);
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForInitalizeSignDocument), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("InitializeSignerStaticSignDocument")]
        [HttpPost]
        public async Task<IActionResult> InitializeSignerStaticSignDocument(RequestSigningUrlModel signingUrlModel)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "InitializeSignerStaticSignDocument", "Process started for Initialize Multi Signer Static Document", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            InitializeSignDocumentAPI objSignParam = new InitializeSignDocumentAPI();
            string envelopeCode = string.Empty;
            string emailId = string.Empty;
            ResponseMessageForInitalizeSignDocument responseMessage = new ResponseMessageForInitalizeSignDocument();
            bool IsUnitTestApp = false; //TODO : send this parameter in objSignParam//Convert.ToBoolean(SessionHelper.Get(SessionKey.IsUnitTestApp));
            string Message = string.Empty;

            string userURL = signingUrlModel.SigningUrl;
            userURL = HttpUtility.UrlDecode(userURL);
            var envelopeID = "";
            var recipientID = "";
            var recipientEmail = string.Empty;

            if (!userURL.Equals(""))
            {
                userURL = EncryptDecryptQueryString.Decrypt(userURL, Convert.ToString(_appConfiguration["AppKey"]));
                if (!userURL.Equals("Invalid length for a Base-64 char array or string.") && !userURL.Equals("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters. ") && !userURL.Equals("Length of the data to decrypt is invalid."))  // V2 Team Prefill Change
                {

                    string[] arrayURL = userURL.Split('&');
                    if (arrayURL.Length == 3)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[1].Split('='); //Get the Recipient ID
                        recipientID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[2].Split('='); //Get CopyEmail
                        recipientEmail = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        recipientEmail = !string.IsNullOrEmpty(recipientEmail) ? HttpUtility.UrlDecode(recipientEmail) : string.Empty;

                        objSignParam.EnvelopeId = envelopeID;
                        objSignParam.RecipientId = recipientID;
                        // objSignParam.EmailId = recipientEmail;
                        objSignParam.templateKey = ""; //not applicable for signer landing
                        objSignParam.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); ;
                        objSignParam.CopyEmailId = "";
                        objSignParam.isFromSignerLanding = true;
                    }
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "The URL of the envelope is incorrect.";
                    return BadRequest(responseMessage);
                }
            }
            else
            {
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "The URL of the envelope is incorrect.";
                return BadRequest(responseMessage);
            }

            Guid envelopeguid, recipientGuid;
            Guid envelopeId = Guid.TryParse(envelopeID, out envelopeguid) ? envelopeguid : Guid.Empty;
            Guid recipientId = Guid.TryParse(recipientID, out recipientGuid) ? recipientGuid : Guid.Empty;

            if (envelopeId == Guid.Empty || recipientId == Guid.Empty)
            {
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "The URL of the envelope is incorrect.";
                return BadRequest(responseMessage);
            }

            string currentRecipientEmailId = string.Empty;
            string senderEmail = string.Empty;
            string currentenvelopeID = string.Empty;
            EnvelopeDetails envelopeDetails = new EnvelopeDetails();
            DashBoard dashBoard = new DashBoard();
            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            APISettings aPISettings = new APISettings();
            Guid signerStatusId = Guid.Empty;
            EnvelopeInfo controlsInfo = new EnvelopeInfo();
            string folderFileSize = "0";
            List<DocumentDetails> documentDetails = new List<DocumentDetails>();
            List<EnvelopeAdditionalUploadInfoDetails> envelopeAdditionalUploadInfoDetails = new List<EnvelopeAdditionalUploadInfoDetails>();
            List<Guid> SameRecipientIds = new List<Guid>();
            bool IsTemplateDatedBeforePortraitLandscapeFeature = false;
            List<CheckListData> lstCheckListData = new List<CheckListData>();
            List<EnvelopeImageControlData> lstEnvelopeImageControlData = new List<EnvelopeImageControlData>();
            string envelopeUNCPath = string.Empty;
            try
            {
                bool isEnvelopeFromPrimaryTable = true;
                var envelopeObject = _genericRepository.GetEntity(envelopeId, false);
                if (envelopeObject == null)
                {
                    isEnvelopeFromPrimaryTable = false;
                    envelopeObject = _genericRepository.GetEntityHistory(envelopeId);
                }

                //  var envelopeObject = _genericRepository.GetEntity(envelopeId);
                envelopeCode = envelopeObject == null ? "" : Convert.ToString(envelopeObject.EDisplayCode);
                responseMessage.EnvelopeInfo = controlsInfo;
                if (envelopeObject == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"]);
                    responseMessage.EnvelopeInfo = null;
                    return BadRequest(responseMessage);
                }
                if (envelopeObject.IsActive == false)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "The sender has deleted the envelope. For further information, please contact the sender.";
                    responseMessage.EnvelopeInfo = null;
                    return BadRequest(responseMessage);
                }
                var envelopeSettingObject = _eSignHelper.GetEnvelopeSettingsDetail(envelopeId);
                if (envelopeSettingObject != null)
                    responseMessage.AttachSignedPdfID = envelopeSettingObject.AttachSignedPdf;
                var sender = envelopeObject.Recipients.FirstOrDefault(a => a.RecipientTypeID == Constants.RecipientType.Sender);
                /* Get User Settings */
                aPISettings = _settingsRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
                adminGeneralAndSystemSettings = _eSignHelper.TransformSettingsDictionaryToEntity(aPISettings);
                responseMessage.IsAllowSignerstoDownloadFinalContract = adminGeneralAndSystemSettings.IsAllowSignerstoDownloadFinalContract;

                //Check for is envelope discarded
                if (Convert.ToBoolean(envelopeObject.IsDraftDeleted) == true)
                {
                    loggerModelNew.Message = envelopeObject.ID + " Envelope contract is discarded by sender.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeDiscarded");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                //If envelope is already expired
                if (envelopeObject.ExpiryDate < DateTime.Today)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract is expired";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeExpired");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.ErrorAction = "Expire";
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                // Check if envelope is already rejected   
                if (envelopeObject.StatusID == Constants.StatusCode.Envelope.Terminated)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has been rejected";
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeTerminated");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                // Check if envelope is already Cancelled   
                if (envelopeObject.StatusID == Constants.StatusCode.Envelope.CancelledTransaction)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has been cancelled by" + senderEmail;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeCancelled");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                // Check if envelope is already Completed  
                if (envelopeObject.StatusID == Constants.StatusCode.Envelope.Completed)
                {
                    currentRecipientEmailId = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).EmailAddress;
                    UserProfile userProfile1 = _userRepository.GetLatestUserProfile(currentRecipientEmailId);
                    responseMessage.RecipientOrder = userProfile1 != null ? 1 : 0;
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract is completed";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "ConatctSender");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.ErrorAction = "ContactSender";
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                // Check if envelope is updating stage. 
                if (Convert.ToBoolean(envelopeObject.IsEdited) == true)
                {
                    loggerModelNew.Message = envelopeObject.ID + " Envelope contract is updating at this time.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeUpdating");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }

                #region S3-936:Move History envelope to Primary  
                if (!isEnvelopeFromPrimaryTable)
                {
                    if (envelopeObject != null)
                    {
                        var objResponse = _envelopeRepository.InsertHistoryEnvelopeToPrimary(envelopeObject.ID.ToString());
                        if (objResponse.StatusMessage == "Success")
                        {
                            envelopeObject = _genericRepository.GetEntity(envelopeId);
                            loggerModelNew.Message = "Inserted EnvelopeHistory is primary for envelopeId ";
                            rSignLogger.RSignLogWarn(loggerModelNew);
                        }
                    }
                }
                #endregion

                //check folder size for signer
                double size = 0;
                string dirPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeObject.ID, string.Empty);
                envelopeUNCPath = dirPath;
                var uploadedDirectory = (dirPath + envelopeObject.ID + "\\SignerAttachments") + "\\" + recipientId;
                if (Directory.Exists(uploadedDirectory))
                {
                    foreach (FileInfo folderfiles in new DirectoryInfo(uploadedDirectory).GetFiles())
                    {
                        size += folderfiles.Length;
                    }
                }
                folderFileSize = size.ToString();
                controlsInfo.IsTemplateShared = envelopeObject.IsTemplateShared;

                envelopeDetails = _envelopeRepository.FillEnvelopeDetailsByEnvelopeEntity(envelopeObject);

                var recipientexist = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId);
                if (recipientexist == null)
                {
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeTerminatedForRecipient");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                responseMessage.DialCodeDropdownList = _envelopeHelperMain.LoadDialingCountryCodes();
                responseMessage.DialCode = recipientexist.DialCode;
                responseMessage.DeliveryMode = Convert.ToString(recipientexist.DeliveryMode);
                responseMessage.MobileNumber = recipientexist.Mobile;
                responseMessage.CountryCode = recipientexist.CountryCode;
                responseMessage.EnableMessageToMobile = envelopeObject.EnableMessageToMobile;

                //Check for order of recipient
                bool isSigned = true;
                if (Convert.ToInt32(recipientexist.Order) > 1)
                {
                    List<RecipientDetails> activeRecipients = _recipientRepository.GetActiveRecipientData(envelopeObject.ID);
                    var tempRecipients = activeRecipients.Where(a => a.Order != null && a.IsSameRecipient != true && a.Order == (Convert.ToInt32(recipientexist.Order) - 1)).ToList();
                    foreach (var item in tempRecipients)
                    {
                        isSigned = _recipientRepository.GetSignerStatusId(item.ID) != Constants.StatusCode.Signer.Signed ? false : true;
                        if (!isSigned)
                            break;
                    }
                }
                if (!Convert.ToBoolean(objSignParam.isFromSignerLanding))
                {
                    if (!string.IsNullOrEmpty(objSignParam.templateKey))
                    {
                        if (envelopeObject.TemplateKey != new Guid(objSignParam.templateKey))
                        {
                            if (recipientexist.RecipientTypeID == Constants.RecipientType.Signer)
                            {
                                if (!isSigned)
                                {
                                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "UnauthorisedSigner");
                                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                    responseMessage.StatusMessage = "BadRequest";
                                    responseMessage.Message = Message;
                                    loggerModelNew.Message = responseMessage.Message;
                                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                    return BadRequest(responseMessage);
                                }
                                else
                                {
                                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeOutOfdateResend");
                                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                    responseMessage.StatusMessage = "BadRequest";
                                    responseMessage.TempDataResendMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeResendLatest");
                                    responseMessage.ErrorAction = "Resend";
                                    responseMessage.Message = Message;
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                    return BadRequest(responseMessage);
                                }
                            }
                            else
                            {
                                Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "UnauthorisedSigner");
                                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                responseMessage.StatusMessage = "BadRequest";
                                responseMessage.Message = Message;
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                responseMessage.EnvelopeInfo = null;
                                return BadRequest(responseMessage);
                            }
                        }
                    }

                    if (envelopeObject.TemplateKey != null && objSignParam.templateKey == "")
                    {
                        if (recipientexist.RecipientTypeID == Constants.RecipientType.Signer)
                        {
                            if (!isSigned)
                            {
                                Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "UnauthorisedSigner");
                                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                responseMessage.StatusMessage = "BadRequest";
                                responseMessage.Message = Message;
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                return BadRequest(responseMessage);
                            }
                            else
                            {
                                Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeOutOfdateResend");
                                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                responseMessage.StatusMessage = "BadRequest";
                                responseMessage.Message = Message;
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                responseMessage.ErrorAction = "Resend";
                                responseMessage.TempDataResendMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeResendLatest");
                                responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                                return BadRequest(responseMessage);
                            }
                        }
                        else
                        {
                            Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "UnauthorisedSigner");
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.Message = Message;
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                            return BadRequest(responseMessage);
                        }
                    }
                }

                currentRecipientEmailId = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).EmailAddress;
                senderEmail = sender != null ? sender.EmailAddress : string.Empty;
                currentenvelopeID = envelopeObject.EDisplayCode.ToString();
                var currentRecipientType = envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).RecipientTypeID;  // V2 Team Prefill Change
                bool CanEdit = Convert.ToBoolean(envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).CanEdit);
                int? InviteSignNowEmail = envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).SignNowOrInviteEmail;
                SameRecipientIds = envelopeObject.Recipients.Where(a => a.EmailAddress == currentRecipientEmailId && a.ID != recipientId && a.IsSameRecipient == true).Select(a => a.ID).ToList();

                // if signer is already delegeted this envelope                    
                try
                {
                    signerStatusId = _recipientRepository.GetSignerStatusId(recipientId);
                }
                catch (Exception ex)
                {
                    signerStatusId = Guid.Empty;
                }
                if (signerStatusId == null || signerStatusId == Guid.Empty)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract is already signed";
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeAccepted");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.ErrorAction = "EnvelopeAccepted";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                if (signerStatusId == Constants.StatusCode.Recipients.Transferred)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has already been transfer to other signer";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeTransferred");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                if (signerStatusId == Constants.StatusCode.Signer.Delegated)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has already been delegated to other signer";
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeAlreadyDelegate");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    loggerModelNew.Message = responseMessage.Message;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                if (signerStatusId == Constants.StatusCode.Signer.Signed)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract is already signed";
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeAccepted");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.ErrorAction = "EnvelopeAccepted";
                    loggerModelNew.Message = responseMessage.Message;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.EnvelopeInfo.SenderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    return BadRequest(responseMessage);
                }
                //string ipAddress = Common.GetIPAddress();
                var remoteIpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                objSignParam.IPAddress = remoteIpAddress;
                string ipAddress = objSignParam.IPAddress;
                int? IsReviewed = recipientexist.IsReviewed;

                var signerSignedStatus = _recipientRepository.GetSignerSignedStatusId(recipientId);

                if (recipientexist.EnvelopeTemplateGroupID != null && recipientexist.RecipientTypeID != Constants.RecipientType.Prefill && recipientexist.EnvelopeTemplateGroupID.Value != Guid.Empty)
                {
                    controlsInfo = _envelopeHelperMain.GetEnvelopeImageInfoByRecipientGroup(envelopeObject, recipientId, recipientexist.EnvelopeTemplateGroupID.Value, dirPath);
                    controlsInfo.SubEnvelopeId = recipientexist.EnvelopeTemplateGroupID.Value;
                }
                else
                    controlsInfo = _envelopeHelperMain.GetEnvelopeImageInfo(envelopeObject, dirPath);

                controlsInfo.IsReviewed = recipientexist.IsReviewed;
                controlsInfo.SignatureCaptureHanddrawn = adminGeneralAndSystemSettings.SignatureCaptureHanddrawn;
                controlsInfo.SignatureCaptureType = adminGeneralAndSystemSettings.SignatureCaptureType;
                controlsInfo.UploadSignature = adminGeneralAndSystemSettings.UploadSignature;
                controlsInfo.ElectronicSignIndication = envelopeObject.ElectronicSignIndicationOptionID != null ? envelopeObject.ElectronicSignIndicationOptionID.Value : adminGeneralAndSystemSettings.ElectronicSignIndicationSelectedID;
                if (CanEdit == true)
                {
                    controlsInfo = _envelopeHelperMain.GetAllDocumentControls(envelopeObject, controlsInfo, recipientId, envelopeId, currentRecipientEmailId, currentenvelopeID);
                }
                else
                {
                    controlsInfo = _envelopeHelperMain.GetDocumentControls(envelopeObject, controlsInfo, recipientId, envelopeId, currentRecipientEmailId, currentenvelopeID);
                }

                if (controlsInfo.ControlsData != null && controlsInfo.ControlsData.Count() > 0)
                {
                    var controlscount = controlsInfo.ControlsData.Where(c => c.ControlHtmlID != "FooterSignature").ToList().Count();
                    if (controlscount > 0)
                    {
                        //Getting all signer's controls for expanding the controls story
                        controlsInfo.AllDocumentControls = _envelopeHelperMain.GetAllDocumentControlsRetriveControlData(envelopeId, envelopeObject);
                    }
                }

                controlsInfo.SignerDocs = _envelopeHelperMain.GetSignerDocFromDirectory(envelopeId, recipientId, dirPath);// objEnvelopeHelperMain.GetSignerDocFromDirectory(envelopeId, recipientId);
                controlsInfo.IsSignerAttachFileReq = (envelopeObject.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                controlsInfo.IsSignerAttachFileReqNew = envelopeObject.IsSignerAttachFileReq != null ? envelopeObject.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                /*Properties which are required in web project from Envelope Object (envelopeObject) assign that props to EnvelopeInfo */
                /*Start of Props*/

                controlsInfo.CultureInfo = !string.IsNullOrEmpty(recipientexist.CultureInfo) ? recipientexist.CultureInfo : Convert.ToString(envelopeObject.CultureInfo);
                controlsInfo.SenderEmail = senderEmail;
                controlsInfo.RecipientEmail = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientId).EmailAddress;
                controlsInfo.DateFormatID = envelopeObject.DateFormatID;
                controlsInfo.RecipientTypeIDPrefill = envelopeObject.Recipients.FirstOrDefault(x => (x.ID == recipientId)).RecipientTypeID;
                controlsInfo.PasswordReqdtoSign = envelopeObject.PasswordReqdtoSign;
                controlsInfo.SignerStatusId = signerStatusId;
                controlsInfo.FolderFileSize = folderFileSize;
                controlsInfo.EDisplayCode = envelopeObject.EDisplayCode.ToString();
                controlsInfo.recipientStatusId = envelopeObject.Recipients.FirstOrDefault(a => a.ID == recipientId).StatusID;
                controlsInfo.GlobalEnvelopeID = envelopeId;
                controlsInfo.Disclaimer = envelopeObject.DisclaimerText;
                controlsInfo.IsDisclaimerEnabled = !string.IsNullOrEmpty(envelopeObject.DisclaimerText);
                if (envelopeObject.IsTemplateShared)
                {
                    var delegatedControls = _documentContentsRepository.GetDelegatedControls(recipientId);
                    if (delegatedControls != null)
                        envelopeObject.IsTemplateShared = false;
                }
                controlsInfo.IsTemplateShared = envelopeObject.IsTemplateShared;
                controlsInfo.IsSingleSigning = Convert.ToBoolean(envelopeObject.IsStatic);
                controlsInfo.Controls = _masterDataRepository.GetControlID().Where(c => c.ID != Constants.Control.NewInitials).ToDictionary(x => x.ControlName, x => x.ControlName);
                controlsInfo.IsStatic = Convert.ToBoolean(envelopeObject.IsStatic);
                controlsInfo.IsDefaultSignatureForStaticTemplate = envelopeObject.IsDefaultSignatureForStaticTemplate;
                controlsInfo.IsSharedTemplateContentUnEditable = envelopeObject.IsSharedTemplateContentUnEditable;
                controlsInfo.TimeZoneSettingOptionValue = adminGeneralAndSystemSettings.SelectedTimeZone.ToString();
                // controlsInfo.UNCPath = ModelHelper.GetIdEnvelopeDirectory(envelopeUNCPath);

                bool isSignerattachmentProcess = false;
                if (Convert.ToInt32(envelopeObject.IsSignerAttachFileReq) > 0)
                {
                    isSignerattachmentProcess = envelopeObject.IsAdditionalAttmReq != null ? Convert.ToBoolean(envelopeObject.IsAdditionalAttmReq) : false;
                }
                controlsInfo.IsAdditionalAttmReq = isSignerattachmentProcess;
                /*RS-428: URGENT - All RSign controls shifted down and right - solve with highest priority*/
                var recipient = _recipientRepository.GetEntity(recipientId);
                if (recipient != null)
                {
                    Guid? templateId = recipient.TemplateID;
                    var template = (templateId != null) ? _templateRepository.GetCreatedDateTime((Guid)templateId) : null;
                    if (template != null)
                    {
                        var dateTimeBeforePortraitLandscape = Convert.ToDateTime(_appConfiguration["dateTimeBeforePortraitLandscape"]);
                        if (template.CreatedDateTime <= dateTimeBeforePortraitLandscape)
                            IsTemplateDatedBeforePortraitLandscapeFeature = true;
                    }
                    controlsInfo.IsTemplateDatedBeforePortraitLandscapeFeature = IsTemplateDatedBeforePortraitLandscapeFeature;
                }

                /*End of Props*/
                //UserProfile userProfile = userRepository.GetUserProfile();
                // List<Documents> Documents = _documentRepository.GetAll(envelopeObject.ID).ToList();
                if (envelopeObject.Documents.Count > 0)
                {
                    foreach (var doc in envelopeObject.Documents)
                    {
                        DocumentDetails document = new DocumentDetails();
                        document.ID = doc.ID;
                        document.EnvelopeID = doc.EnvelopeID;
                        document.DocumentName = doc.DocumentName;
                        document.DocumentSource = doc.DocumentSource;
                        document.ActionType = doc.ActionType;
                        documentDetails.Add(document);
                    }
                }

                if (controlsInfo.IsSignerAttachFileReqNew == Constants.SignerAttachmentOptions.EnableAttachmentRequest)
                {
                    envelopeAdditionalUploadInfoDetails = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByEnvelopebyRecipeint(envelopeObject.ID, recipientId); //GetEnvelopeAdditionalUploadInfoByEnvelope
                }
                UserProfile userProfile = _userRepository.GetLatestUserProfile(currentRecipientEmailId);

                UserData userData = new UserData();
                if (userProfile != null)
                {
                    userData.UserId = userProfile.UserID;
                    userData.UserName = userProfile.FirstName + " " + userProfile.LastName;
                    userData.UserInitials = userProfile.Initials;
                    if (userProfile.IsAutoPopulateSignaturewhileSinging)
                    {
                        userData.UserInitialsImgSrc = userProfile.SignatureImage == null ? null : "data:image/png;base64," + Convert.ToBase64String(userProfile.SignatureImage);
                    }
                    else
                    {
                        userData.UserInitialsImgSrc = null;
                    }
                    userData.SignatureTypeID = userProfile.SignatureTypeID;
                }
                else if (envelopeObject.Recipients.Where(r => r.ID == recipientId).Count() > 0)
                {
                    foreach (var rec in envelopeObject.Recipients.Where(r => r.ID == recipientId))
                    {
                        userData.UserId = rec.ID;
                        userData.UserName = rec.Name;
                        userData.UserInitials = null;
                        userData.UserInitialsImgSrc = null;
                        userData.SignatureTypeID = Guid.Empty;
                    }
                }
                responseMessage.userdata = userData;
                responseMessage.MaxUploadID = _envelopeRepository.GetMaxUploadsID();
                responseMessage.EnableAutoFillTextControls = Convert.ToBoolean(envelopeObject.IsEnableAutoFillTextControls);
                responseMessage.CanEdit = CanEdit;
                responseMessage.InviteSignNowByEmail = InviteSignNowEmail;
                responseMessage.UNCPath = _modelHelper.GetIdEnvelopeDirectory(envelopeUNCPath);

                var senderProfileDetails = _userRepository.GetUserProfileByUserID(envelopeObject.UserID);
                if (sender != null)
                {
                    AdminGeneralAndSystemSettings companysettings = new AdminGeneralAndSystemSettings();
                    Guid CompanyID = (Guid)senderProfileDetails.CompanyID;
                    var getSetting = _settingsRepository.GetEntityByParam(CompanyID, string.Empty, Constants.String.SettingsType.Company);
                    companysettings = _eSignHelper.TransformSettingsDictionaryToEntity(getSetting);
                    responseMessage.DisableDeclineOption = companysettings.DisableDeclineOption;
                    responseMessage.DisableFinishLaterOption = companysettings.DisableFinishLaterOption;
                    responseMessage.DisableChangeSigner = companysettings.DisableChangeSigner;
                }

                #region  Signer Landing from Angular
                responseMessage.EncryptedGlobalEnvelopeID = EncryptDecryptQueryString.Encrypt(Convert.ToString(envelopeId), Convert.ToString(_appConfiguration["AppKey"]));
                responseMessage.EncryptedGlobalRecipientID = EncryptDecryptQueryString.Encrypt(Convert.ToString(recipientId), Convert.ToString(_appConfiguration["AppKey"]));
                responseMessage.EncryptedSender = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(Convert.ToString(controlsInfo.SenderEmail), Convert.ToString(_appConfiguration["AppKey"])));
                responseMessage.Delegated = controlsInfo.SignerStatusId == Constants.StatusCode.Signer.Delegated ? "Delegated" : "NotDelegated";
                responseMessage.Prefill = controlsInfo.RecipientTypeIDPrefill == Constants.RecipientType.Prefill ? "prefill" : "";

                string cultureInfo = "";
                if (string.IsNullOrEmpty(Convert.ToString(controlsInfo.CultureInfo)))
                {
                    cultureInfo = "en-us";
                    controlsInfo.CultureInfo = cultureInfo;
                }
                else
                    cultureInfo = controlsInfo.CultureInfo.ToLower();

                responseMessage.Language = _lookupRepository.GetLookupLanguageList(Lookup.LanguageKeyDetails, cultureInfo, Constants.String.languageKeyType.Label);
                var labelText = responseMessage.Language;

                #region DateFormat
                string DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_slash;
                string DateFormat = "";
                if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_slash;
                    DateFormat = "mm/dd/yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_colan;
                    DateFormat = "mm-dd-yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_mm_dd_yyyy_dots;
                    DateFormat = "mm.dd.yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_slash))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_slash;
                    DateFormat = "dd/mm/yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_colan;
                    DateFormat = "dd-mm-yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_mm_dd_yyyy_dots;
                    DateFormat = "dd.mm.yy";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.Europe_yyyy_mm_dd_dots))
                {
                    DatePlaceHolder = Constants.DateFormatString.Europe_yyyy_mm_dd_dots;
                    DateFormat = "yy.mm.dd.";
                }
                else if ((controlsInfo.DateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan))
                {
                    DatePlaceHolder = Constants.DateFormatString.US_dd_mmm_yyyy_colan;
                    DateFormat = "dd-mmm-yy";
                }
                #endregion DateFormat

                responseMessage.DatePlaceHolder = DatePlaceHolder;
                responseMessage.DateFormat = DateFormat;
                responseMessage.FileReviewInfo = documentDetails.Where(d => d.ActionType == Constants.ActionTypes.Review).ToList();
                responseMessage.FileReviewCount = documentDetails.Where(d => d.ActionType == Constants.ActionTypes.Review).Count();
                responseMessage.DocumentNameList = documentDetails.Where(d => d.ActionType != Constants.ActionTypes.Review).ToList();
                responseMessage.AllowStaticMultiSigner = false;

                Guid SignatureTypeID = Guid.Empty;
                if (currentRecipientType == Constants.RecipientType.Prefill)
                    responseMessage.EnableClickToSign = CanEdit == true ? false : adminGeneralAndSystemSettings.EnableClickToSign;
                else
                    responseMessage.EnableClickToSign = adminGeneralAndSystemSettings.EnableClickToSign;

                responseMessage.DisableDownloadOptionOnSignersPage = adminGeneralAndSystemSettings.DisableDownloadOptionOnSignersPage;
                responseMessage.SignatureTypeID = userData != null ? userData.SignatureTypeID : Guid.Empty;
                responseMessage.IsAnySignatureExists = controlsInfo.ControlsData.Any(c => c.ControlName == "Signature" && (c.IsCurrentRecipient == true));

                if (responseMessage.IsAnySignatureExists == false)
                    responseMessage.ShowDefaultSignatureContol = "block";
                else
                    responseMessage.ShowDefaultSignatureContol = "none";

                var DefaultplaceHolder = string.Empty;
                if (DatePlaceHolder == "dd-mmm-yyyy")
                    responseMessage.DefaultplaceHolder = DatePlaceHolder.Replace("mmm", "MMM");
                else
                    responseMessage.DefaultplaceHolder = DatePlaceHolder.Replace("mm", "MM");

                #region CheckListData   
                var SameRecipientList = SameRecipientIds;
                int? pageCount = 0;
                pageCount = controlsInfo.ControlsData.Where(x => (x.IsCurrentRecipient == true || (x.RecipientId != null && x.RecipientId != Guid.Empty && SameRecipientList != null && SameRecipientList.Contains(x.RecipientId.Value) == true))).Max(x => x.PageNo);
                pageCount = pageCount == null ? 0 : pageCount.Value;
                responseMessage.PageCount = pageCount;
                bool isAnySignatureExists = controlsInfo.ControlsData.Any(c => c.ControlName == "Signature" && (c.IsCurrentRecipient == true));
                var defaultSignature = controlsInfo.ControlsData.FirstOrDefault(c => c.ControlName == "FooterSignature");
                List<string> radioCntrlOndoc = new List<string>();
                List<string> checkboxCntrlOndoc = new List<string>();
                List<DocumentDetails> DocumentNameList = responseMessage.DocumentNameList;
                bool EditControls = CanEdit;

                if (pageCount > 0)
                {
                    if (controlsInfo.ControlsData.Count > 1)
                    {
                        Guid tempDocId = Guid.Empty;
                        string documentName = "";
                        for (var i = 1; i <= pageCount; i++)
                        {
                            if (controlsInfo.ControlsData.Any(x => x.PageNo == i && ((EditControls == true && (x.ControlName != "Signature" && x.ControlName != "Email" && x.ControlName != "NewInitials")) || x.IsCurrentRecipient == true || (x.RecipientId != null && x.RecipientId != Guid.Empty && SameRecipientList != null && SameRecipientList.Contains(x.RecipientId.Value) == true)) && x.ControlName != "Label" && x.ControlName != "DateTimeStamp" && x.ControlName != "Hyperlink"))
                            {
                                List<ControlsData> ControlsData = controlsInfo.ControlsData.OrderBy(item => item.TabIndex).Where(x => x.PageNo == i && ((EditControls == true && (x.ControlName != "Signature" && x.ControlName != "Email" && x.ControlName != "NewInitials")) || x.IsCurrentRecipient == true || (x.RecipientId != null && x.RecipientId != Guid.Empty && SameRecipientList != null && SameRecipientList.Contains(x.RecipientId.Value) == true))).ToList();

                                if ((tempDocId == Guid.Empty || (ControlsData != null && ControlsData.Count > 0 && tempDocId != ControlsData[0].DocumentId)))
                                {
                                    documentName = DocumentNameList.Where(d => d.ID == ControlsData[0].DocumentId).FirstOrDefault().DocumentName;
                                    tempDocId = ControlsData[0].DocumentId;
                                }

                                List<ControlsData> newControlsData = new List<ControlsData>();
                                foreach (var cntrl in ControlsData)
                                {
                                    if (cntrl.IsReadOnly != true)
                                    {
                                        if (cntrl.ControlName.ToLower() == "radio" && !(radioCntrlOndoc.Contains(cntrl.ControlHtmlID)))
                                        {
                                            List<ControlsData> radiogrp = new List<ControlsData>();
                                            radiogrp = controlsInfo.ControlsData.Where(x => x.GroupName == cntrl.GroupName && x.ControlName.ToLower() == "radio" && x.IsCurrentRecipient == true).ToList();
                                            foreach (var radio in radiogrp)
                                            {
                                                radioCntrlOndoc.Add(radio.ControlHtmlID);
                                                radio.Required = cntrl.Required;
                                                newControlsData.Add(radio);
                                            }
                                        }
                                        else if (cntrl.ControlName.ToLower() == "checkbox" && !(checkboxCntrlOndoc.Contains(cntrl.ControlHtmlID)))
                                        {
                                            if (!string.IsNullOrEmpty(cntrl.GroupName))
                                            {
                                                List<ControlsData> checkgrp = new List<ControlsData>();
                                                checkgrp = controlsInfo.ControlsData.Where(x => x.GroupName == cntrl.GroupName && x.ControlName.ToLower() == "checkbox" && x.IsCurrentRecipient == true).ToList();
                                                foreach (var check in checkgrp)
                                                {
                                                    checkboxCntrlOndoc.Add(check.ControlHtmlID);
                                                    check.Required = cntrl.Required;
                                                    newControlsData.Add(check);
                                                }
                                            }
                                            else
                                            {
                                                newControlsData.Add(cntrl);
                                            }
                                        }
                                        else if (cntrl.ControlName != "Label" && cntrl.ControlName != "Checkbox" && cntrl.ControlName.ToLower() != "radio" && cntrl.ControlName.ToLower() != "datetimestamp" && cntrl.ControlName != "Hyperlink")
                                        {
                                            newControlsData.Add(cntrl);
                                        }
                                    }
                                }

                                CheckListData checkListData = new CheckListData();
                                checkListData.PageNumber = i;
                                checkListData.ControlsData = newControlsData;
                                checkListData.DocumentName = documentName;
                                checkListData.DocumentId = tempDocId;
                                lstCheckListData.Add(checkListData);
                            }
                        }
                    }
                }
                else
                {
                    foreach (DocumentDetails docData in DocumentNameList)
                    {
                        CheckListData checkListData = new CheckListData();
                        checkListData.PageNumber = null;
                        checkListData.ControlsData = null;
                        checkListData.DocumentName = docData.DocumentName;
                        checkListData.DocumentId = docData.ID;
                        lstCheckListData.Add(checkListData);
                    }
                }

                #endregion CheckListData

                #region ControlsData
                var pageNo = 1;
                foreach (var envelope in controlsInfo.EnvelopeImageCollection)
                {
                    string imageid = Convert.ToString(envelope.Id);
                    string pageNumber = pageNo.ToString();
                    int DocPageNo = envelope.DocPageNo;
                    string imgControlWidth = "";
                    if (controlsInfo.IsTemplateDatedBeforePortraitLandscapeFeature == true)
                        imgControlWidth = "";
                    else
                    {
                        if (Convert.ToInt32(envelope.Dimension.Width) > Convert.ToInt32(envelope.Dimension.Height))
                            imgControlWidth = "1015px";
                        else
                            imgControlWidth = "915px";
                    }

                    SigningEnvelopeDocumentData signingEnvelopeDocumentData = new SigningEnvelopeDocumentData();
                    signingEnvelopeDocumentData.PageNum = pageNo;
                    signingEnvelopeDocumentData.DocId = envelope.Document.Id;
                    signingEnvelopeDocumentData.DocName = envelope.Document.Name;
                    signingEnvelopeDocumentData.IsPageLoaded = true;

                    string imgPath = "";
                    if (controlsInfo.SubEnvelopeId != null && controlsInfo.SubEnvelopeId != Guid.Empty)
                        imgPath = "/" + imageid + "/" + Convert.ToString(controlsInfo.GlobalEnvelopeID) + "/" + envelope.ImagePath.Substring(envelope.ImagePath.LastIndexOf('/') + 1) + "/" + responseMessage.UNCPath;
                    else if (controlsInfo.SubEnvelopeId == null && controlsInfo.IsTemplateShared == true && responseMessage.Prefill == "prefill")
                        imgPath = "/" + imageid + "/" + Convert.ToString(controlsInfo.GlobalEnvelopeID) + "/" + envelope.ImagePath.Substring(envelope.ImagePath.LastIndexOf('/') + 1) + "/" + responseMessage.UNCPath;
                    else
                        imgPath = "/" + imageid + "/" + Convert.ToString(controlsInfo.GlobalEnvelopeID) + "/" + responseMessage.UNCPath;

                    foreach (var item in controlsInfo.ControlsData)
                    {
                        if (item.DocumentId == envelope.Document.Id && (item.PageNo == envelope.PageNo))
                        {
                            item.LanguageControlName = labelText.Single(a => a.KeyName.ToLower() == item.ControlName.ToLower()).KeyValue;

                            if (item.ControlName.ToLower() == "checkbox" && !string.IsNullOrEmpty(item.GroupName) && item.Required != true)
                            {
                                var checkgrp = controlsInfo.ControlsData.Where(x => x.GroupName == item.GroupName && x.ControlName.ToLower() == "checkbox" && x.IsCurrentRecipient == true).ToList();
                                if (checkgrp.Any(rg => rg.Required != null && rg.Required == true))
                                {
                                    item.Required = true;
                                }
                            }

                            var style = item.ControlHtmlData;
                            string tempTop = "";
                            string left = (item.Left == 0.0 ? style.Substring(style.IndexOf("left"), (style.IndexOf("px") - style.IndexOf("left"))) : item.Left.ToString()) + "px";

                            int topIndex = style.IndexOf("top");
                            int positionIndex = style.IndexOf("position");
                            int topValueIndex = positionIndex < topIndex ? (style.IndexOf(";", topIndex) + 2) : positionIndex;
                            string top = (item.Top == 0.0 ? style.Substring(topIndex, (topValueIndex - (topIndex + 2))) : item.Top.ToString());

                            if (item.ControlName == "Name" || item.ControlName == "Email" || item.ControlName == "Title" || item.ControlName == "Label" || item.ControlName == "Company")
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 2) + "px";
                            }
                            else if (item.ControlName == "Text")
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                            }
                            else if (item.ControlName == "DropDown")
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 4) + "px";
                            }
                            else if (item.ControlName == "Checkbox" && item.Height == 14)
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                                left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";
                            }
                            else if (item.ControlName == "Radio" && item.Height == 14)
                            {
                                top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                                left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";
                            }

                            item.CalculatedTop = top;
                            item.CalculatedLeft = left;
                            item.HoverTitle = item.Required || (item.ControlName == "DropDown" && !item.Required && item.ControlOptions[0].OptionText != "") ? "Required" : "Optional";
                            item.CalculatedModalWidth = item.ControlName.ToLower() == "text" || item.ControlName.ToLower() == "name" ? item.Width + 1 : item.Width - 5;

                            if (item.ControlName == "Date")
                            {
                                var dateplaceholder = item.Label;
                                if (dateplaceholder == "dd-mmm-yyyy")
                                    item.DefaultDateFormat = dateplaceholder.Replace("mmm", "MMM");
                                else
                                    item.DefaultDateFormat = dateplaceholder.Replace("mm", "MM");
                            }

                            var ExistSameRecipient = false;
                            if (SameRecipientIds != null && SameRecipientIds.Count > 0 && item.RecipientId != null && item.RecipientId != Guid.Empty)
                                ExistSameRecipient = SameRecipientIds.Contains(item.RecipientId.Value);

                            item.ExistSameRecipient = ExistSameRecipient;
                            item.EditControls = CanEdit;
                            if (item.IsCurrentRecipient == true || ExistSameRecipient == true || (CanEdit == true && (item.ControlName != "Signature" && item.ControlName != "Email" && item.ControlName != "NewInitials")))
                            {
                                if (item.ControlName == "Signature" && controlsInfo.ElectronicSignIndication > 1 && !string.IsNullOrEmpty(item.SignatureScr) && !(Convert.ToBoolean(item.IsSignatureFromDocumentContent)))
                                {
                                    int width; int height;
                                    item.SignatureScr = _envelopeHelperMain.ConvertSignImageWithStamp(item.SignatureScr, out height, out width, controlsInfo.EDisplayCode, controlsInfo.ElectronicSignIndication, DateFormat, controlsInfo.TimeZoneSettingOptionValue, Convert.ToString(controlsInfo.DateFormatID), Convert.ToString(responseMessage.SignatureTypeID));
                                }

                                if (item.ControlName == "Text")
                                {
                                    string textType = "";
                                    string textTypeValue = "";
                                    string textTypeMask = "";

                                    switch (item.ControlType)
                                    {
                                        case "D6FBBFC2-C907-4290-929F-175EB437AA81":
                                        case "D1409FCF-5683-4921-A62B-2F635F4E49B7":
                                        case "B0443A47-89C3-4826-BECC-378D81738D03":
                                        case "C175A449-3A22-4FE0-A009-C3F76F612510":
                                        case "73C17C33-F255-474F-9F46-248542ADDACC":
                                            textType = "Numeric";
                                            textTypeValue = "";
                                            textTypeMask = "";
                                            break;
                                        case "F01331D9-3413-466A-9821-2670A8D9F3EE":
                                        case "26C0ACEA-3CC8-43D6-A255-A870A8524A77":
                                        case "CBAF463C-8287-4C04-B90C-C6E2F1EC5299":
                                        case "F690C267-D10F-40AD-A487-D2035D9C3858":
                                        case "126AF3B7-409E-425E-A9C3-A313254ACB03":
                                            textType = "Text";
                                            break;
                                        case "88A0B11E-5810-4ABF-A8B6-856C436E7C49":
                                            textType = "Alphabet";
                                            break;
                                        case "8348E5CD-59EA-4A77-8436-298553D286BD":
                                            textType = "Date";
                                            break;
                                        case "DCBBE75C-FDEC-472C-AE25-2C42ADFB3F5D":
                                            textType = "SSN";
                                            textTypeValue = "___-__-____";
                                            textTypeMask = "___-__-____";
                                            break;
                                        case "5121246A-D9AB-49F4-8717-4EF4CAAB927B":
                                            textType = "Zip";
                                            break;
                                        case "1AD2D4EC-4593-435E-AFDD-F8A90426DE96":
                                            textType = "Email";
                                            break;
                                    }

                                    switch (item.AdditionalValidationOption)
                                    {
                                        case "Zip":
                                            textTypeValue = "_____";
                                            textTypeMask = "_____";
                                            break;
                                        case "Zip+4":
                                            textTypeValue = "_____-____";
                                            textTypeMask = "_____-____";
                                            break;
                                        case "mm/dd/yyyy":
                                            textTypeValue = "mm/dd/yyyy";
                                            textTypeMask = "mm/dd/yyyy";
                                            break;
                                        case "dd/mm/yyyy":
                                            textTypeValue = "dd/mm/yyyy";
                                            textTypeMask = "dd/mm/yyyy";
                                            break;
                                        case "yyyy/mm/dd":
                                            textTypeValue = "yyyy/mm/dd";
                                            textTypeMask = "yyyy/mm/dd";
                                            break;
                                        case "Period":
                                            textTypeValue = "Period";
                                            textTypeMask = "Period";
                                            break;
                                        case "Comma":
                                            textTypeValue = "Comma";
                                            textTypeMask = "Comma";
                                            break;
                                        case "Both":
                                            textTypeValue = "Both";
                                            textTypeMask = "Both";
                                            break;
                                        case "dd-mmm-yyyy":
                                            textTypeValue = "dd-mmm-yyyy";
                                            textTypeMask = "dd-mmm-yyyy";
                                            break;
                                    }

                                    item.TextType = textType;
                                    item.TextTypeValue = textTypeValue;
                                    item.TextTypeMask = textTypeMask;

                                    int inputMaxLength = 0;
                                    if (textType != "Date" && textType != "Email")
                                    {
                                        if (style.IndexOf("maxlengthallowed") > -1)
                                        {
                                            item.MaxInputLength = Regex.Replace(style.Substring(style.IndexOf("maxlengthallowed") + 18, 4), "[^0-9]+", string.Empty);
                                            inputMaxLength = Convert.ToInt32(item.MaxInputLength);
                                        }
                                        else
                                        {
                                            item.MaxInputLength = "";
                                            inputMaxLength = 0;
                                        }
                                    }

                                    if (textType == "Alphabet" || textType == "Text" || textType == "Numeric")
                                    {
                                        inputMaxLength = inputMaxLength == 0 ? 1 : Convert.ToInt32(item.MaxInputLength);
                                        if (string.IsNullOrEmpty(item.CustomToolTip))
                                        {
                                            if (textType == "Numeric")
                                                item.CustomToolTip = "Maximum " + inputMaxLength + " digits (0-9) allowed.";
                                            else
                                                item.CustomToolTip = "Approximately " + inputMaxLength + " characters or the control boundaries.";
                                        }
                                    }
                                }
                                else if (item.ControlName == "Name")
                                {
                                    int inputMaxLength = 0;
                                    if (style.IndexOf("maxlengthallowed") > -1)
                                    {
                                        item.MaxInputLength = style.Substring(style.IndexOf("maxlengthallowed") + 18, 4);
                                        var isNumber = Regex.IsMatch(item.MaxInputLength, @"^\d+$");
                                        if (!isNumber)
                                        {
                                            item.MaxInputLength = Regex.Replace(item.MaxInputLength, "[^0-9]+", string.Empty);
                                        }
                                        if (!string.IsNullOrEmpty(item.MaxInputLength))
                                        {
                                            inputMaxLength = Convert.ToInt32(item.MaxInputLength);
                                        }
                                        else
                                        {
                                            inputMaxLength = 10;
                                        }
                                    }
                                    else
                                    {
                                        item.MaxInputLength = "";
                                        inputMaxLength = 0;
                                    }
                                    inputMaxLength = inputMaxLength == 0 ? 1 : inputMaxLength;
                                    if (string.IsNullOrEmpty(item.CustomToolTip))
                                    {
                                        item.CustomToolTip = "Approximately " + inputMaxLength + " characters or the control boundaries.";
                                    }
                                }
                            }
                            else if (item.IsSigned)
                            {
                                try
                                {
                                    tempTop = top.Replace("top:", "").Replace("top :", "").Replace("px", "").Trim();
                                    if (item.ControlName == "Signature" || item.ControlName == "NewInitials" || item.ControlName == "DateTimeStamp" || item.ControlName == "Date")
                                    {
                                        tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop)) + "px";
                                    }
                                    else if (item.ControlName == "Title" || item.ControlName == "Company" || item.ControlName == "Email" || item.ControlName == "Radio" || item.ControlName == "Checkbox")
                                    {
                                        tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop) + 2.0) + "px";
                                    }
                                    else
                                    {
                                        tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop) + 5.0) + "px";
                                    }
                                    top = tempTop;
                                }
                                catch (Exception) { }

                                item.CalculatedTop = top;
                            }
                        }
                    }

                    EnvelopeImageControlData envelopeImageControlData = new EnvelopeImageControlData();
                    envelopeImageControlData.Id = envelope.Id;
                    envelopeImageControlData.PageNum = pageNumber;
                    envelopeImageControlData.DocPageNo = DocPageNo;
                    envelopeImageControlData.ImgControlWidth = imgControlWidth;
                    envelopeImageControlData.ImagePath = imgPath;
                    envelopeImageControlData.SigningEnvelopeDocumentData = signingEnvelopeDocumentData;
                    envelopeImageControlData.ControlsData = controlsInfo.ControlsData.Where(c => c.DocumentId == envelope.Document.Id && c.PageNo == envelope.PageNo).ToList();
                    lstEnvelopeImageControlData.Add(envelopeImageControlData);

                    pageNo++;
                }

                if (controlsInfo.AllDocumentControls != null)
                {
                    foreach (var itemCtrl in controlsInfo.AllDocumentControls)
                    {
                        var style = itemCtrl.ControlHtmlData;
                        string tempTop = "";
                        string left = (itemCtrl.Left == 0.0 ? style.Substring(style.IndexOf("left"), (style.IndexOf("px") - style.IndexOf("left"))) : itemCtrl.Left.ToString()) + "px";

                        int topIndex = style.IndexOf("top");
                        int positionIndex = style.IndexOf("position");
                        int topValueIndex = positionIndex < topIndex ? (style.IndexOf(";", topIndex) + 2) : positionIndex;
                        string top = (itemCtrl.Top == 0.0 ? style.Substring(topIndex, (topValueIndex - (topIndex + 2))) : itemCtrl.Top.ToString());

                        if (itemCtrl.ControlName == "Name" || itemCtrl.ControlName == "Email" || itemCtrl.ControlName == "Title" || itemCtrl.ControlName == "Label" || itemCtrl.ControlName == "Company")
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 2) + "px";
                        }
                        else if (itemCtrl.ControlName == "Text")
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                        }
                        else if (itemCtrl.ControlName == "DropDown")
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 4) + "px";
                        }
                        else if (itemCtrl.ControlName == "Checkbox" && itemCtrl.Height == 14)
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                            left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";
                        }
                        else if (itemCtrl.ControlName == "Radio" && itemCtrl.Height == 14)
                        {
                            top = "top: " + (Convert.ToDecimal(top.Substring(top.IndexOf(" "), top.IndexOf("px") - top.IndexOf(" "))) - 1) + "px";
                            left = "left: " + (Convert.ToDecimal(left.Substring(left.IndexOf(" "), left.IndexOf("px") - left.IndexOf(" "))) - 1) + "px";
                        }

                        itemCtrl.CalculatedTop = top;
                        itemCtrl.CalculatedLeft = left;
                        itemCtrl.CalculatedModalWidth = itemCtrl.ControlName.ToLower() == "text" ? itemCtrl.Width + 1 : itemCtrl.Width - 5;
                        if (itemCtrl.IsSigned)
                        {
                            try
                            {
                                tempTop = top.Replace("top:", "").Replace("top :", "").Replace("px", "").Trim();
                                if (itemCtrl.ControlName == "Signature" || itemCtrl.ControlName == "NewInitials" || itemCtrl.ControlName == "DateTimeStamp")
                                {
                                    tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop)) + "px";
                                }
                                else
                                {
                                    tempTop = "top: " + Convert.ToString(Convert.ToDouble(tempTop) + 5.0) + "px";
                                }
                                top = tempTop;
                            }
                            catch (Exception) { }

                            itemCtrl.CalculatedTop = top;
                        }
                    }
                }
                #endregion ControlsData     

                #endregion Signer Landing from Angular

                responseMessage.EnableClickToSign = adminGeneralAndSystemSettings.EnableClickToSign;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "The signer details reterived successully.";
                responseMessage.EnvelopeInfo = controlsInfo;
                responseMessage.documentDetails = documentDetails;
                responseMessage.EnvelopeAdditionalUploadInfoList = envelopeAdditionalUploadInfoDetails;
                responseMessage.SameRecipientIds = SameRecipientIds;
                responseMessage.CheckListData = lstCheckListData;
                responseMessage.EnvelopeImageControlData = lstEnvelopeImageControlData;
                responseMessage.RequiresSignersConfirmationonFinalSubmit = adminGeneralAndSystemSettings.RequiresSignersConfirmationonFinalSubmit;
                responseMessage.IncludeStaticTemplates = adminGeneralAndSystemSettings.IncludeStaticTemplates;
                responseMessage.ReVerifySignerDocumentSubmit = Convert.ToBoolean(envelopeObject.ReVerifySignerDocumentSubmit);
                loggerModelNew.Message = "Process completed for Initialize Sign Document and " + responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller InitializeSignerStaticSignDocument action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.EnvelopeId = envelopeID;
                responseMessage.CurrentEmail = recipientEmail;
                return BadRequest(responseMessage);
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForTranslationsModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetLanguageTranslations")]
        [HttpPost]
        public IActionResult GetLanguageTranslations(TranslationsModel translationsModel)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetLanguageTranslations", "Process started for Get Language Translations", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForTranslationsModel responseMessage = new ResponseMessageForTranslationsModel();
            try
            {
                LanguageKeyTranslationsModel responseTranslations = _genericRepository.GetLanguageKeyTranslations(translationsModel);
                responseMessage.LanguageTranslationsModel = responseTranslations;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "The language translations details reterived successully.";
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForTranslationsModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("ConvertTextToSignImage")]
        [HttpPost]
        public IActionResult ConvertTextToSignImage(ConvertTextToSignImageModel convertTextToSignImage)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "ConvertTextToSignImage", "Process started for Convert Sign Image", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForConvertTextToSignImage responseMessage = new ResponseMessageForConvertTextToSignImage();
            ResponseConvertTextToSignImage responseConvertTextToSignImageModel = new ResponseConvertTextToSignImage();

            try
            {
                var FontFolderPath = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["FontFolderPath"]));
                int intHeight = 0;
                int intWidth = 0;
                var pfc = new System.Drawing.Text.PrivateFontCollection();
                pfc.AddFontFile(Path.Combine(FontFolderPath, convertTextToSignImage.font) + ".ttf");
                string imageSource = _envelopeHelperMain.ConvertSignImage(pfc, convertTextToSignImage.text, convertTextToSignImage.font, convertTextToSignImage.fontsize, convertTextToSignImage.fontColor, convertTextToSignImage.height, convertTextToSignImage.width, out intHeight, out intWidth, convertTextToSignImage.envelopeCode, convertTextToSignImage.electronicSignIndicationId, convertTextToSignImage.dateFormat, convertTextToSignImage.userTimezone, convertTextToSignImage.dateFormatID);

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Process completed for Converting Text into Sign Image";
                responseConvertTextToSignImageModel.imgsrc = imageSource;
                responseConvertTextToSignImageModel.height = intHeight;
                responseConvertTextToSignImageModel.width = intWidth;
                responseMessage.ResponseConvertTextToSignImage = responseConvertTextToSignImageModel;
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(CustomAPIResponse), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetDeclineTemplate/{envelopeId}/{isStaticTemplate?}")]
        [HttpGet]
        public IActionResult GetDeclineTemplate(string envelopeId, bool isStaticTemplate = false)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetDeclineTemplate", "Initiate the process for Get decline template settings by envelopeId using API.", envelopeId, "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                CustomAPIResponse responseMessage = new CustomAPIResponse();
                Guid companyID = Guid.Empty;
                string cultureInfo = string.Empty;
                if (isStaticTemplate == true)
                {
                    var templateObject = _genericRepository.GetTemplateDetails(new Guid(envelopeId));
                    if (templateObject == null)
                    {
                        loggerModelNew.Message = "This template was not found.";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(new { ErrorMessage = loggerModelNew.Message });
                    }
                    var companyProfile = _companyRepository.GetCompanyForUserID(templateObject.UserID);
                    companyID = companyProfile != null ? companyProfile.ID : templateObject.UserID;
                    cultureInfo = templateObject.CultureInfo;
                }
                else
                {
                    var envelopeObject = _genericRepository.GetEntity(new Guid(envelopeId));
                    if (envelopeObject == null)
                    {
                        loggerModelNew.Message = "This envelope was not found.";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(new { ErrorMessage = loggerModelNew.Message });
                    }
                    var companyProfile = _companyRepository.GetCompanyProfileByEnvelopeID(envelopeObject.ID);
                    companyID = companyProfile != null ? companyProfile.ID : envelopeObject.UserID;
                    cultureInfo = envelopeObject.CultureInfo;
                }

                DeclineTemplateSettings declineTemplateSettings = new DeclineTemplateSettings();
                declineTemplateSettings.CultureInfo = string.IsNullOrEmpty(cultureInfo) ? "en-us" : cultureInfo;

                List<DeclineTemplateResponses> declineTemplateResponsesList = new List<DeclineTemplateResponses>();
                var usersettingsDetails = _settingsRepository.GetEntityForByKeyConfig(companyID, Constants.SettingsKeyConfig.DeclineTemplateReasonsSettings);
                // var SenderDetails = envelopeObject.Recipients.Where(r => r.RecipientTypeID == Constants.RecipientType.Sender).FirstOrDefault();
                if (usersettingsDetails != null)
                {
                    var declineTemplateSetting = JsonConvert.DeserializeObject<List<DeclineTemplateSettings>>(usersettingsDetails.OptionValue);
                    if (declineTemplateSetting != null)
                    {

                        declineTemplateSettings.Title = declineTemplateSetting.FirstOrDefault().Title;
                        declineTemplateSettings.ControlType = declineTemplateSetting.FirstOrDefault().ControlType;
                        if (declineTemplateSetting.FirstOrDefault().DeclineTemplateResponsesList.Count > 0)
                        {
                            foreach (var item in declineTemplateSetting.FirstOrDefault().DeclineTemplateResponsesList)
                            {
                                DeclineTemplateResponses declineTemplateResponses = new DeclineTemplateResponses();
                                declineTemplateResponses.ID = item.ID;
                                if (item.ID == 999)
                                {
                                    var otherText = _lookupRepository.GetLookupLanguageKeyDetails(Constants.Resourcekey.Other, declineTemplateSettings.CultureInfo);
                                    declineTemplateResponses.ResponseText = otherText.KeyValue;
                                }
                                else
                                    declineTemplateResponses.ResponseText = item.ResponseText;
                                declineTemplateResponses.Order = item.Order;
                                declineTemplateResponsesList.Add(declineTemplateResponses);
                            }
                            declineTemplateSettings.DeclineTemplateResponsesList = declineTemplateResponsesList.OrderBy(d => d.ID).ToList();
                        }
                    }
                }

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Decline Template settings retrieved successfully.";
                responseMessage.Status = true;
                responseMessage.Data = declineTemplateSettings;
                loggerModelNew.Message = "Decline Template settings retrieved successfully.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocument controller GetDeclineTemplate method.";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageFile), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DownloadFinalContract/{envelopeId}/{currentStatus?}/{recipientTypeId?}")]
        [HttpGet]
        public async Task<IActionResult> DownloadFinalContract(string envelopeID, string currentStatus = "", string recipientTypeId = "")
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "DownloadFinalContract", "Process started for Download Final Contract document by envelopeId using API", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            string finalDocName = string.Empty;
            string fileName = string.Empty;
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            string DeleteFinalContractDisplayMsg = "The signed contract is deleted from RSign server.";

            try
            {
                var envelopeObject = _genericRepository.GetEntity(new Guid(envelopeID));
                var senderDetail = (from r in envelopeObject.Recipients
                                    where r.RecipientTypeID == Constants.RecipientType.Sender
                                    select r).FirstOrDefault();

                UserProfile userProfile = _userRepository.GetUserProfileByEmailID(senderDetail.EmailAddress);
                Guid companyId = userProfile.CompanyID ?? new Guid();

                bool IsMultiDoc = false;
                string strFinalDirectoryPath = string.Empty;
                string strURL = string.Empty;
                int intDtFormat = 0;
               
                if (envelopeObject == null || envelopeObject.ID == Guid.Empty)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"]);
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                finalDocName = ((!string.IsNullOrEmpty(recipientTypeId) && new Guid(recipientTypeId) != Constants.RecipientType.Sender) && envelopeObject.IsWaterMark == true) ? (envelopeObject.Documents.Count() > 1) ? "ContractsSigner" : envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).First().DocumentName :
                 (envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Count() > 1) ? "Contracts" : envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).First().DocumentName;

                if (envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Count() == 1)
                    finalDocName = finalDocName.Substring(0, finalDocName.LastIndexOf('.'));


                bool enableMultipleNameCustomize = false;
                var settingMultiplefileNameCustomize = _settingsRepository.GetEntityForByKeyConfig(companyId, Constants.SettingsKeyConfig.EnableMultipleAttachmentsCustomizable);
                if (settingMultiplefileNameCustomize != null)
                    enableMultipleNameCustomize = Convert.ToBoolean(settingMultiplefileNameCustomize.OptionValue);

                if (enableMultipleNameCustomize == true && envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Count() > 1)
                {
                    finalDocName = _envelopeHelperMain.UpdateFinalSignedDocumentNamingForMultipleDocuments(envelopeObject, companyId);
                }
                else
                {
                    finalDocName = ((!string.IsNullOrEmpty(recipientTypeId) && new Guid(recipientTypeId) != Constants.RecipientType.Sender) && envelopeObject.IsWaterMark == true) ? (envelopeObject.Documents.Count() > 1) ? "ContractsSigner" : envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).First().DocumentName :
                     (envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Count() > 1) ? "Contracts" : envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).First().DocumentName;
                    //finalDocName = envelopeRepository.GetFinalDocumentName(envelopeObject.ID, envelopeObject);
                    if (envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Count() == 1)
                    {
                        if (finalDocName.LastIndexOf('.') > -1)
                            finalDocName = finalDocName.Substring(0, finalDocName.LastIndexOf('.'));
                    }
                    var settingsDateTimeFormat = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.AddDatetoSignedDocumentNameOptionSettings);
                    if (settingsDateTimeFormat != null)
                    {
                        intDtFormat = Convert.ToInt32(settingsDateTimeFormat.OptionValue);
                        finalDocName = _envelopeHelperMain.AppendDateTime(intDtFormat, finalDocName, envelopeObject.ModifiedDateTime.ToString());
                    }
                }

                if (envelopeObject.IsSeparateMultipleDocumentsAfterSigningRequired == true && envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Count() > 1)
                {
                    IsMultiDoc = true;
                    finalDocName = finalDocName + ".zip";
                    fileName = ((!string.IsNullOrEmpty(recipientTypeId) && new Guid(recipientTypeId) != Constants.RecipientType.Sender) && envelopeObject.IsWaterMark == true) ? "ContractsSigner.zip" : "Contracts.zip";
                }
                else
                {
                    finalDocName = finalDocName + ".pdf";
                    fileName = ((!string.IsNullOrEmpty(recipientTypeId) && new Guid(recipientTypeId) != Constants.RecipientType.Sender) && envelopeObject.IsWaterMark == true) ? "OutputSigner.pdf" : "Output.pdf";
                }
                //var settingsDateTimeFormt = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.AddDatetoSignedDocumentNameOptionSettings);
                //if (settingsDateTimeFormt != null)
                //{
                //    intDtFormat = Convert.ToInt32(settingsDateTimeFormt.OptionValue);
                //    finalDocName = _envelopeHelperMain.AppendDateTime(intDtFormat, finalDocName, envelopeObject.ModifiedDateTime.ToString());
                //}
                if (envelopeObject.IsFinalContractDeleted)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = DeleteFinalContractDisplayMsg;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                if (IsMultiDoc == true)
                {
                    string dirPath = _modelHelper.GetEnvelopeDirectoryNew(new Guid(envelopeID), string.Empty);
                    strFinalDirectoryPath = Path.Combine(Convert.ToString(dirPath), envelopeID.ToString(), "FinalZipPDF");
                    if (!Directory.Exists(strFinalDirectoryPath))
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = DeleteFinalContractDisplayMsg;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                    if (!System.IO.File.Exists(Path.Combine(strFinalDirectoryPath, fileName)))
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = DeleteFinalContractDisplayMsg;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                    strURL = Path.Combine(strFinalDirectoryPath, fileName);
                }
                else
                {
                    string dirPath = _modelHelper.GetEnvelopeDirectoryNew(new Guid(envelopeID), string.Empty);
                    if (!string.IsNullOrEmpty(currentStatus) && currentStatus != "0")
                        strFinalDirectoryPath = Path.Combine(Convert.ToString(dirPath), envelopeID.ToString(), currentStatus, "Final");
                    else
                        strFinalDirectoryPath = Path.Combine(Convert.ToString(dirPath), envelopeID.ToString(), "Final");
                    if (!Directory.Exists(strFinalDirectoryPath))
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = DeleteFinalContractDisplayMsg;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                    if (!System.IO.File.Exists(Path.Combine(strFinalDirectoryPath, fileName)))
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = DeleteFinalContractDisplayMsg;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                    strURL = Path.Combine(strFinalDirectoryPath, fileName);
                }


                byte[] buf = null;
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        buf = webClient.DownloadData(strURL);
                    }
                }
                catch (WebException webex)
                {
                    loggerModelNew.Message = "Error occurred in Signdocument controller DownloadFinalContract's webClient.DownloadData() action.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                string imgBase64 = Convert.ToBase64String(buf);
                responseMessage.Base64FileData = imgBase64;
                responseMessage.FileName = finalDocName;
                responseMessage.FileType = IsMultiDoc ? "application/zip" : "application/pdf";
                responseMessage.FilePath = strURL;
                responseMessage.byteArray = buf;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Document downloaded successfully";
                loggerModelNew.Message = "Process completed for Download Final Contract document by envelopeId using API and " + responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in Signdocument controller DownloadFinalContract action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageGetEnvelopeDetails), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetEnvelopeDetails/{envelopeId}/{methodName?}/{currentStatus?}")]
        [HttpGet]
        public IActionResult GetEnvelopeDetails(string envelopeId, string methodName = "GetPDF", string currentStatus = null)
        {
            ResponseMessageGetEnvelopeDetails responseMessage = new ResponseMessageGetEnvelopeDetails();
            string envelopeCode = string.Empty;
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetEnvelopeDetails", "Process started for Get Envelope Details", envelopeId, "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            EnvelopeDetails envelopeDetails = new EnvelopeDetails();
            try
            {
                var envelope = _genericRepository.GetEntity(new Guid(envelopeId));
                if (envelope == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"]);
                    responseMessage.EnvelopeId = envelopeId;
                    responseMessage.EnvelopeDetails = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                if ((envelope.IsDraft != true) && ((envelope.IsTemplateDeleted)))
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.EnvelopeId = envelopeId;
                    responseMessage.Message = Convert.ToString(_appConfiguration["EnvelopeDiscarded"]);
                    responseMessage.EnvelopeDetails = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                //envelopeDetails = _envelopeRepository.FillEnvelopeDetailsByEnvelopeEntity(envelope);
                envelopeDetails = _envelopeRepository.FillEnvelopeDetailsByEnvelopeId(envelope);

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.EnvelopeId = envelopeId;
                responseMessage.Message = Convert.ToString("Envelope found");
                responseMessage.EnvelopeDetails = envelopeDetails;
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in Signdocument controller DownloadFinalContract action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForDownloadDisclaimerPDF), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DownloadDisclaimerPDF/{envelopeId}/{isStaticTemplate}")]
        [HttpGet]
        public IActionResult DownloadDisclaimerPDF(string envelopeId, string isStaticTemplate)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "DownloadDisclaimerPDF", "Process started for Download Disclaimer PDF", envelopeId, "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForDownloadDisclaimerPDF responseMessage = new ResponseMessageForDownloadDisclaimerPDF();
            try
            {
                EnvelopeDetails envelopeDetails = new EnvelopeDetails();
                if (isStaticTemplate != null && isStaticTemplate.ToLower() == "true")
                {
                    Template template = _genericRepository.GetTemplateDetails(new Guid(envelopeId));
                    if (template != null)
                    {
                        envelopeDetails.EDisplayCode = template.TemplateCode.ToString();
                        envelopeDetails.DisclaimerText = _settingsRepository.GetEntityForByKeyConfig(template.UserID, Constants.SettingsKeyConfig.Disclaimer).OptionValue;
                    }
                }
                else
                {
                    var envelope = _genericRepository.GetEnvelopeDisClaimerText(new Guid(envelopeId));
                    if (envelope != null)
                    {
                        envelopeDetails.DisclaimerText = envelope.DisclaimerText;
                        envelopeDetails.EDisplayCode = envelope.EDisplayCode.ToString();
                    }
                }

                responseMessage.byteData = _asposeHelper.ConvertHtmlTextToPdf(envelopeDetails.DisclaimerText);
                responseMessage.FileName = envelopeDetails.EDisplayCode + "_Disclaimer.pdf";
                responseMessage.FileType = System.Net.Mime.MediaTypeNames.Application.Octet;
                responseMessage.Message = "Document downloaded successfully.";
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageFile), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetDownloadFileReview/{envelopeID}/{recipientId}/{Type?}")]
        [HttpGet]
        public IActionResult GetDownloadFileReview(Guid envelopeID, Guid recipientId, string Type = "")
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetDownloadFileReview", "Initiate the process for Download File Review document using API.", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            int intDtFormat = 0;
            try
            {
                var envelope = _genericRepository.GetEntity(envelopeID);
                var recipients = _recipientRepository.GetEntity(recipientId);
                Template template = new Template();
                if (string.IsNullOrEmpty(Type) || Type == "null")
                {
                    recipients.IsReviewed = 1;
                    _recipientRepository.Save(recipients);
                }

                if (Type == "Static")
                {
                    template = _genericRepository.GetTemplateEntity(envelopeID);
                }

                //Guid DocumentId = new Guid(documentId);
                if ((envelope == null || envelope.Documents.FirstOrDefault(d => d.ActionType == Constants.ActionTypes.Review) == null) && Type != "Static")
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "Document not found";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                if ((template == null || template.TemplateDocuments.FirstOrDefault(d => d.ActionType == Constants.ActionTypes.Review) == null) && Type == "Static")
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "Document not found";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                List<String> files = new List<String>();
                string tempDirectoryPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeID, string.Empty) + envelopeID;
                string fileslocationPath = tempDirectoryPath + "\\" + Convert.ToString(_appConfiguration["UploadedDocuments"]);
                string fileName = string.Empty, filePath = string.Empty;

                string envelopeSubject = string.Empty, returnFilePath = string.Empty;
                var settingsDateTimeFormt = _settingsRepository.GetEntityForByKeyConfig(envelope.UserID, Constants.SettingsKeyConfig.AddDatetoSignedDocumentNameOptionSettings);
                if (Type != "Static")
                {
                    envelopeSubject = Regex.Replace(envelope.Subject, "[\\\\/:*?\"<>|]", "");
                    envelopeSubject = envelopeSubject.Length > 150 ? envelopeSubject.Substring(0, 150) : envelopeSubject;
                    returnFilePath = envelopeSubject;
                    if (settingsDateTimeFormt != null && envelope.Documents.Where(d => d.ActionType == Constants.ActionTypes.Review).Count() > 1)
                    {
                        intDtFormat = Convert.ToInt32(settingsDateTimeFormt.OptionValue);
                        envelopeSubject = _envelopeHelperMain.AppendDateTime(intDtFormat, envelopeSubject, envelope.ModifiedDateTime.ToString());
                    }
                    if (envelope.Documents.Where(d => d.ActionType == Constants.ActionTypes.Review).Count() > 1)
                    {
                        foreach (var doc in envelope.Documents.Where(d => d.ActionType == Constants.ActionTypes.Review))
                        {
                            files.Add(Path.Combine(fileslocationPath, doc.DocumentName));
                        }
                        filePath = _envelopeHelperMain.GetFilesReviewZip(files, tempDirectoryPath, returnFilePath);
                        fileName = envelopeSubject + ".zip";
                    }
                    else
                    {
                        fileName = envelope.Documents.FirstOrDefault(d => d.ActionType == Constants.ActionTypes.Review).DocumentName;
                        filePath = Path.Combine(fileslocationPath, fileName);
                    }
                }
                else
                {
                    envelopeSubject = Regex.Replace(template.Subject, "[\\\\/:*?\"<>|]", "");
                    envelopeSubject = envelopeSubject.Length > 150 ? envelopeSubject.Substring(0, 150) : envelopeSubject;
                    returnFilePath = envelopeSubject;
                    if (settingsDateTimeFormt != null && template.TemplateDocuments.Where(d => d.ActionType == Constants.ActionTypes.Review).Count() > 1)
                    {
                        intDtFormat = Convert.ToInt32(settingsDateTimeFormt.OptionValue);
                        envelopeSubject = _envelopeHelperMain.AppendDateTime(intDtFormat, envelopeSubject, envelope.ModifiedDateTime.ToString());
                    }
                    if (template.TemplateDocuments.Where(d => d.ActionType == Constants.ActionTypes.Review).Count() > 1)
                    {
                        foreach (var doc in template.TemplateDocuments.Where(d => d.ActionType == Constants.ActionTypes.Review))
                        {
                            files.Add(Path.Combine(fileslocationPath, doc.DocumentName));
                        }
                        filePath = _envelopeHelperMain.GetFilesReviewZip(files, tempDirectoryPath, returnFilePath);
                        fileName = envelopeSubject + ".zip";
                    }
                    else
                    {
                        fileName = template.TemplateDocuments.FirstOrDefault(d => d.ActionType == Constants.ActionTypes.Review).DocumentName;
                        filePath = Path.Combine(fileslocationPath, fileName);
                    }
                }
                if (!System.IO.File.Exists(filePath))
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "Document not found";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                byte[] imgBytes = System.IO.File.ReadAllBytes(filePath);
                string imgBase64 = Convert.ToBase64String(imgBytes);

                loggerModelNew.Message = "Successfully retrieved the document" + fileName;
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseMessage.Base64FileData = imgBase64;
                responseMessage.FileName = fileName;
                responseMessage.FilePath = filePath;
                responseMessage.byteArray = imgBytes;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Document downloaded successfully";
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetDownloadFileReview action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageDownload), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetDownloadPDFPreview/{envelopeID}/{recipientId}")]
        [HttpGet]
        public IActionResult GetDownloadPDFPreview(Guid envelopeID, Guid recipientId)
        {
            ResponseMessageDownload apiResponseMessage = new ResponseMessageDownload();
            string envelopeCode = string.Empty;
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetDownloadPDFPreview", "Process started for Get Download PDF Preview", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                EnvelopeDetails envelopeDetails = new EnvelopeDetails();
                string globalEnvelopeID = envelopeID.ToString();
                var envelope = _genericRepository.GetEntity(envelopeID);
                envelopeCode = envelope.EnvelopeCodeDisplay;
                string dirPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeID, string.Empty);
                var filestream = _envelopeHelperMain.DownloadPDFOnSignDocument(envelope, dirPath, recipientId);
                string tempDirectoryPath = _eSignHelper.GetPreviewFolderPath(envelopeID, dirPath);
                string fileName = "previewDocument.pdf";
                string filePath = Path.Combine(tempDirectoryPath, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    loggerModelNew.Message = "File not exist";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    apiResponseMessage.StatusCode = HttpStatusCode.BadRequest;
                    apiResponseMessage.StatusMessage = "BadRequest";
                    apiResponseMessage.Message = "File not exist";
                    return BadRequest(apiResponseMessage);
                }

                apiResponseMessage.StatusCode = HttpStatusCode.OK;
                apiResponseMessage.StatusMessage = "OK";
                apiResponseMessage.Message = "Successfully retrieved the document" + fileName;
                apiResponseMessage.FileName = fileName;
                apiResponseMessage.FilePath = filePath;
                byte[] buf = System.IO.File.ReadAllBytes(filePath);
                string imgBase64 = Convert.ToBase64String(buf);
                apiResponseMessage.Base64FileData = imgBase64;
                loggerModelNew.Message = "Process completed for Get Download PDF Preview" + apiResponseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Ok(apiResponseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetDownloadFileReview action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForGetEnvelopeOrTemplateFields), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetEnvelopeOrTemplateFields/{EnvelopeId}/{recipientId}")]
        [HttpGet]
        public IActionResult GetEnvelopeOrTemplateFields(string envelopeId, string recipientId)
        {
            ResponseMessageForGetEnvelopeOrTemplateFields responseMessage = new ResponseMessageForGetEnvelopeOrTemplateFields();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetEnvelopeOrTemplateFields", "Process started for Get Envelope details using API.", envelopeId.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                Guid envelopeID = new Guid(envelopeId);
                Guid recipientID = Guid.Empty;
                string decryptPassword = string.Empty;
                if (!String.IsNullOrEmpty(recipientId) && recipientId != "00000000-0000-0000-0000-000000000000")
                {
                    recipientID = new Guid(recipientId);
                }
                var envelope = _genericRepository.GetEnvelopeRecipients(envelopeID);
                if (envelope == null || envelope.ID == Guid.Empty)
                {
                    var template = _genericRepository.GetTemplateDetails(envelopeID);
                    if (template == null || template.ID == Guid.Empty)
                    {
                        responseMessage.StatusCode = HttpStatusCode.NoContent;
                        responseMessage.StatusMessage = "NoContent";
                        responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"]);
                        responseMessage.EnvelopeId = Guid.Empty;
                        responseMessage.TemplateId = envelopeID;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }

                    decryptPassword = ModelHelper.Decrypt((template.PasswordReqdtoOpen ? template.PasswordtoOpen : template.PasswordtoSign), template.PasswordKey, (int)template.PasswordKeySize);

                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.EnvelopeId = Guid.Empty;
                    responseMessage.TemplateId = template.ID;
                    responseMessage.Message = Convert.ToString("Envelope found");
                    responseMessage.EDisplayCode = null;
                    responseMessage.PasswordKey = template.PasswordKey;
                    responseMessage.PasswordKeySize = template.PasswordKeySize;
                    responseMessage.PasswordtoOpen = template.PasswordtoOpen;
                    responseMessage.PasswordtoSign = template.PasswordtoSign;
                    responseMessage.TemplateKey = template.TemplateKey.ToString();
                    responseMessage.DecryptPassword = decryptPassword;
                    loggerModelNew.Message = "Process completed for Get Envelope details using API and " + responseMessage.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }

                if (recipientID != Guid.Empty && envelope.Recipients != null)
                    responseMessage.currentRecEmailId = envelope.Recipients.FirstOrDefault(r => r.ID == recipientID).EmailAddress;

                decryptPassword = ModelHelper.Decrypt((envelope.PasswordReqdtoOpen ? envelope.PasswordtoOpen : envelope.PasswordtoSign), envelope.PasswordKey, (int)envelope.PasswordKeySize);
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.EnvelopeId = envelope.ID;
                responseMessage.TemplateId = Guid.Empty;
                responseMessage.Message = Convert.ToString("Envelope found");
                responseMessage.EDisplayCode = Convert.ToString(envelope.EDisplayCode);
                responseMessage.PasswordKey = envelope.PasswordKey;
                responseMessage.PasswordKeySize = envelope.PasswordKeySize;
                responseMessage.PasswordtoOpen = envelope.PasswordtoOpen;
                responseMessage.PasswordtoSign = envelope.PasswordtoSign;
                responseMessage.TemplateKey = envelope.TemplateKey.ToString();
                responseMessage.DecryptPassword = decryptPassword;
                loggerModelNew.Message = "Process completed for Get Envelope details using API and " + responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetEnvelopeOrTemplateFields action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(AuthenticateSignerResponseModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("AuthenticateSigner")]
        [HttpPost]
        public async Task<IActionResult> AuthenticateSigner(AuthenticateSignerRequestModel authenticateSignerRequestModel)
        {
            var envelopeID = "";
            var recipientId = "";
            string EmailId = string.Empty;
            HttpResponseMessage responseToClient = new HttpResponseMessage();
            AuthenticateSignerResponseModel responseMessage = new AuthenticateSignerResponseModel();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "AuthenticateSigner", "Process started for Authenticate Signer", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                string finalDocName = string.Empty;
                string userURL = authenticateSignerRequestModel.AuthenticateUrl;
                userURL = HttpUtility.UrlDecode(userURL);
                string strFinalDirectoryPath = string.Empty;
                string strURL = string.Empty, templateKey = string.Empty;
                if (!userURL.Equals(""))
                {
                    userURL = EncryptDecryptQueryString.Decrypt(userURL, Convert.ToString(_appConfiguration["AppKey"]));
                    string[] arrayURL = userURL.Split('&');
                    if (arrayURL.Length == 6)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[1].Split('='); //Get the Recipient ID
                        recipientId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[2].Split('='); //Get the Template Key
                        templateKey = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[3].Split('='); //Get the Email ID
                        EmailId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        EmailId = !string.IsNullOrEmpty(EmailId) ? HttpUtility.UrlDecode(EmailId) : string.Empty;
                    }
                    else if (arrayURL.Length == 5)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        string[] arrayrecipientTypeId = arrayURL[1].Split('=');
                        recipientId = arrayrecipientTypeId.Length == 2 ? arrayrecipientTypeId[1].Trim() : string.Empty;
                        string[] arrayEmailId = arrayURL[2].Split('=');
                        EmailId = arrayEmailId.Length == 2 ? arrayEmailId[1].Trim() : string.Empty;
                        EmailId = !string.IsNullOrEmpty(EmailId) ? HttpUtility.UrlDecode(EmailId) : string.Empty;
                    }
                    else if (arrayURL.Length == 4)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[1].Split('='); //Get the Recipient ID
                        recipientId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[2].Split('='); //Get the Template Key
                        templateKey = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        arrayID = arrayURL[3].Split('='); //Get the Email ID
                        EmailId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        EmailId = !string.IsNullOrEmpty(EmailId) ? HttpUtility.UrlDecode(EmailId) : string.Empty;
                    }
                    else if (arrayURL.Length == 3)
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        string[] arrayrecipientTypeId = arrayURL[1].Split('=');
                        recipientId = arrayrecipientTypeId.Length == 2 ? arrayrecipientTypeId[1].Trim() : string.Empty;
                        string[] arrayEmailId = arrayURL[2].Split('=');
                        EmailId = arrayEmailId.Length == 2 ? arrayEmailId[1].Trim() : string.Empty;
                        EmailId = !string.IsNullOrEmpty(EmailId) ? HttpUtility.UrlDecode(EmailId) : string.Empty;
                    }
                    else
                    {
                        string[] arrayID = arrayURL[0].Split('=');
                        envelopeID = arrayID[1].Trim();
                    }

                    UserVerificationModel userVerificationModel = new UserVerificationModel();
                    userVerificationModel.envelopeID = envelopeID.ToString();
                    userVerificationModel.EmailId = EmailId;
                    userVerificationModel.recipientId = recipientId;
                    var errorResponseModel = await _envelopeRepository.UpdateVerificationCodeEmail(userVerificationModel);

                    if (errorResponseModel.IsEnvelopePurging)
                    {
                        responseMessage.StatusCode = HttpStatusCode.OK;
                        responseMessage.StatusMessage = "OK";
                        responseMessage.Message = errorResponseModel.Message;
                        responseMessage.IsEnvelopePurging = errorResponseModel.IsEnvelopePurging;
                        loggerModelNew.Message = errorResponseModel.Message;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return Ok(responseMessage);
                    }

                    if (!errorResponseModel.Status)
                    {
                        loggerModelNew.Message = errorResponseModel.Message;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = errorResponseModel.Message;
                        responseMessage.Message = errorResponseModel.Message;
                        return BadRequest(responseMessage);
                    }

                    if (authenticateSignerRequestModel.IsResend)
                    {
                        loggerModelNew.Message = "Process completed for ReSend Verification Code";
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        responseMessage.StatusCode = HttpStatusCode.OK;
                        responseMessage.StatusMessage = "OK";
                        responseMessage.Message = "Verification Code is sent.";
                        responseMessage.RecipientId = recipientId;
                        responseMessage.Email = EmailId;
                        responseMessage.DeliveryMode = errorResponseModel.DeliveryMode;
                        responseMessage.EnableMessageToMobile = errorResponseModel.EnableMessageToMobile;
                        responseMessage.Mobile = errorResponseModel.Mobile;
                        return Ok(responseMessage);
                    }

                    loggerModelNew.Message = "Process completed for Authenticate Signer";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.Message = "Verification code sent Successfully";
                    responseMessage.RecipientId = recipientId;
                    responseMessage.Email = EmailId;
                    return Ok(responseMessage);
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "The URL of the envelope is incorrect.";
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller AuthenticateSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForGetEnvelopeOrTemplateFields), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("UpdateVerificationCode")]
        [HttpPost]
        public async Task<IActionResult> UpdateVerificationCode(UserVerificationModel userVerificationModel)
        {
            ResponseMessageForGetEnvelopeOrTemplateFields responseMessage = new ResponseMessageForGetEnvelopeOrTemplateFields();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "UpdateVerificationCode", "Initiate the process for Update Verification Code using API.", userVerificationModel.envelopeID.ToString(), "", "",
                "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                var errorResponseModel = await _envelopeRepository.UpdateVerificationCodeEmail(userVerificationModel);
                if (errorResponseModel.IsEnvelopePurging)
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.Message = errorResponseModel.Message;
                    responseMessage.IsEnvelopePurging = errorResponseModel.IsEnvelopePurging;
                    loggerModelNew.Message = errorResponseModel.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }

                if (!errorResponseModel.Status)
                {
                    loggerModelNew.Message = errorResponseModel.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = errorResponseModel.Message;
                    responseMessage.Message = errorResponseModel.Message;
                    return BadRequest(responseMessage);
                }

                loggerModelNew.Message = "Process completed for Update Verification Code.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Verification code sent Successfully";
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller AuthenticateSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseRecipientsVerificationCode), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetVerificationCode/{recipientID}")]
        [HttpGet]
        public IActionResult GetVerificationCode(string recipientID)
        {
            ResponseRecipientsVerificationCode responseMessage = new ResponseRecipientsVerificationCode();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetVerificationCode", "Initiate the process for Get Verification Code using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                var allRecipients = _recipientRepository.GetEntity(new Guid(recipientID));
                if (allRecipients.VerificationCode == "")
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.Message = "No recipients found.";
                    responseMessage.VerificationCode = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.Message = allRecipients.EmailAddress + " recipients found.";
                    responseMessage.VerificationCode = allRecipients.VerificationCode;
                    loggerModelNew.Message = "Process completed for Get Verification Code using API and " + responseMessage.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetDownloadFileReview action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(AuthenticateSignerResponseModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetCodeAndValidateSigner")]
        [HttpPost]
        public IActionResult GetCodeAndValidateSigner(ValidateSignerRequestModel validateSignerRequestModel)
        {
            var envelopeID = "";
            string EmailId = string.Empty;
            AuthenticateSignerResponseModel responseMessage = new AuthenticateSignerResponseModel();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetCodeAndValidateSigner", "Process started for Get Code And Validate Signer", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                var recipientResp = _recipientRepository.GetEntity(new Guid(validateSignerRequestModel.RecipientId));
                if (recipientResp != null && recipientResp.VerificationCode == validateSignerRequestModel.VerificationCode)
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.Message = "Verification Code is matched.";
                    loggerModelNew.Message = "Process completed and Verification Code is matched.";
                    responseMessage.RecipientId = validateSignerRequestModel.RecipientId;
                    responseMessage.Email = recipientResp.EmailAddress;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Ok(responseMessage);
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "Verification code did not match.";
                    responseMessage.Message = "Verification code did not match.";
                    loggerModelNew.Message = "Process completed and Verification code did not match.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller and GetCodeAndValidateSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForEncryptQuery), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("EncryptQueryParamString/{StrValue}")]
        [HttpGet]
        public IActionResult EncryptQueryParamString(string StrValue)
        {
            ResponseMessageForEncryptQuery responseMessage = new ResponseMessageForEncryptQuery();
            try
            {
                loggerModelNew = new LoggerModelNew("", "SignDocumentController", "EncryptQueryParamString", "Process started for Encrypt Query Param String", "", "", "", "", "API");
                rSignLogger.RSignLogInfo(loggerModelNew);

                string encryptedEncodedText = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(StrValue, Convert.ToString(_appConfiguration["QueryStringKey"])));
                responseMessage.EnvryptedEncodedText = encryptedEncodedText;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Process completed for Encrypt Query Param String";
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller EncryptQueryParamString action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(EnvelopeAdditionalUpload), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetAdditonalfileAttachmentInfo/{envelopeID}/{recipientId}/{additionalRecipients?}")]
        [HttpGet]
        public IActionResult GetAdditonalfileAttachmentInfo(Guid envelopeID, Guid recipientId, string additionalRecipients = null)
        {
            EnvelopeAdditionalUpload recipientResp = new EnvelopeAdditionalUpload();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetAdditonalfileAttachmentInfo", "Initiate the process for Get Additonal File Info Signer requested attachments using API.", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                if (additionalRecipients == null || additionalRecipients == "null" || additionalRecipients == "") { additionalRecipients = string.Empty; }
                List<EnvelopeAdditionalUploadInfoDetails> envelopeAdditionalUploadInfoDetails = new List<EnvelopeAdditionalUploadInfoDetails>();
                envelopeAdditionalUploadInfoDetails = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByEnvelopebyRecipeint(envelopeID, recipientId, additionalRecipients);
                recipientResp.MaxUploadID = _envelopeRepository.GetMaxUploadsID();

                recipientResp.StatusCode = HttpStatusCode.OK;
                recipientResp.StatusMessage = "OK";
                recipientResp.Message = "Envelope finished successfully.";
                recipientResp.EnvelopeAdditionalUploadInfoList = envelopeAdditionalUploadInfoDetails;
                loggerModelNew.Message = "Process completed for Get Additonal File Info Signer requested attachments using API and " + recipientResp.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(recipientResp);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetDownloadFileReview action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForConvertTextToSignImage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("ConvertHandDrawnSignImage")]
        [HttpPost]
        public IActionResult ConvertHandDrawnSignImage(ConvertHandDrawnSignImageModel reqModel)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "ConvertHandDrawnSignImage", "Process started for Convert hand drawn Sign Image", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageForConvertTextToSignImage responseMessage = new ResponseMessageForConvertTextToSignImage();
            ResponseConvertTextToSignImage responseConvertTextToSignImageModel = new ResponseConvertTextToSignImage();
            HttpResponseMessage responseToClient = new HttpResponseMessage();
            try
            {
                int intHeight = 0;
                int intWidth = 0;
                string imageSource = _envelopeHelperMain.ConvertHandDrawnSignImage(reqModel.imageBytes, out intHeight, out intWidth, reqModel.envelopeCode, reqModel.electronicSignIndicationId, reqModel.dateFormat, reqModel.userTimezone, reqModel.dateFormatID);
                loggerModelNew.Message = "Process completed for converting hand drawn into Sign Image";
                rSignLogger.RSignLogInfo(loggerModelNew);

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Process completed for converting hand drawn into Sign Image";
                responseConvertTextToSignImageModel.imgsrc = imageSource;
                responseConvertTextToSignImageModel.height = intHeight;
                responseConvertTextToSignImageModel.width = intWidth;
                responseMessage.ResponseConvertTextToSignImage = responseConvertTextToSignImageModel;
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller ConvertHandDrawnSignImage action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForConvertTextToSignImage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("ConvertHandDrawnSignImageUpload")]
        [HttpPost]
        public IActionResult ConvertHandDrawnSignImageUpload(ConvertHandDrawnSignImageModel reqModel)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "ConvertHandDrawnSignImage", "Process started for Convert hand drawn Sign Image", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageForConvertTextToSignImage responseMessage = new ResponseMessageForConvertTextToSignImage();
            ResponseConvertTextToSignImage responseConvertTextToSignImageModel = new ResponseConvertTextToSignImage();
            try
            {
                int intHeight = 0;
                int intWidth = 0;
                string imageSource = _envelopeHelperMain.ConvertHandDrawnSignImageUpload(reqModel.imageBytes, out intHeight, out intWidth, reqModel.envelopeCode, reqModel.electronicSignIndicationId, reqModel.dateFormat, reqModel.userTimezone, reqModel.dateFormatID);
                loggerModelNew.Message = "Process completed for converting hand drawn image upload into Sign Image";
                rSignLogger.RSignLogInfo(loggerModelNew);

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Process completed for converting hand drawn image upload into Sign Image";
                responseConvertTextToSignImageModel.imgsrc = imageSource;
                responseConvertTextToSignImageModel.height = intHeight;
                responseConvertTextToSignImageModel.width = intWidth;
                responseMessage.ResponseConvertTextToSignImage = responseConvertTextToSignImageModel;
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller ConvertHandDrawnSignImage action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("FinishLaterSubmit")]
        [HttpPost]
        public IActionResult FinishLaterSubmit(EnvelopeSignDocumentSubmitInfo envelopeFinishLaterSubmitInfo)
        {
            var remoteIpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
            envelopeFinishLaterSubmitInfo.IpAddress = remoteIpAddress;

            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "FinishLaterSubmit", "Process started for Finish Later Submit using API", envelopeFinishLaterSubmitInfo.EnvelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            string envelopeCode = string.Empty;
            string emailId = string.Empty, senderEmail = string.Empty;
            string currentRecipientEmailId = string.Empty;
            ResponseMessage apiResponseMessage = new ResponseMessage();
            string responseMessage = string.Empty;

            try
            {
                var envelope = _genericRepository.GetEntity(envelopeFinishLaterSubmitInfo.EnvelopeID);
                if (envelope != null)
                {
                    var sender = envelope.Recipients.FirstOrDefault(r => r.RecipientTypeID == Constants.RecipientType.Sender);
                    senderEmail = sender != null ? sender.EmailAddress : string.Empty;
                    var currentRecipient = envelope.Recipients.FirstOrDefault(r => r.ID == envelopeFinishLaterSubmitInfo.RecipientID);

                    if (currentRecipient == null)
                    {
                        loggerModelNew.Message = "Unable to identify current signer for envelope";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        apiResponseMessage.StatusCode = HttpStatusCode.BadRequest;
                        apiResponseMessage.StatusMessage = "BadRequest";
                        apiResponseMessage.Message = "Unable to to identify you as recipient";
                        apiResponseMessage.data = senderEmail;
                        return BadRequest(responseMessage);
                    }

                    loggerModelNew.Email = currentRecipient.EmailAddress;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    currentRecipientEmailId = currentRecipient.EmailAddress;
                    envelope.IpAddress = envelopeFinishLaterSubmitInfo.IpAddress;
                    if (envelope.StatusID == Constants.StatusCode.Envelope.Waiting_For_Signature)
                    {
                        var UrlForFinishLater = "<a style='color: blue; text-decoration: underline; text-underline: single' href='" + envelopeFinishLaterSubmitInfo.UrlForFinishLater + "'>" + _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "lang_clickHere") + "</a>";

                        if (currentRecipient.RecipientTypeID == Constants.RecipientType.Prefill)
                            responseMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "responsePrefillFinishLater") + " " + UrlForFinishLater + "  to continue.";
                        else if (Convert.ToBoolean(envelope.EnableMessageToMobile))
                        {
                            if (currentRecipient.DeliveryMode == Constants.DeliveryModes.EmailSlashEmail || currentRecipient.DeliveryMode == Constants.DeliveryModes.EmailSlashMobile || currentRecipient.DeliveryMode == Constants.DeliveryModes.EmailSlashEmailAndMobile || currentRecipient.DeliveryMode == Constants.DeliveryModes.EmailSlashNone)
                                responseMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "responseFinishLater") + " " + UrlForFinishLater + "  to continue.";
                            else if (currentRecipient.DeliveryMode == Constants.DeliveryModes.EmailAndMobileSlashMobile || currentRecipient.DeliveryMode == Constants.DeliveryModes.EmailAndMobileSlashEmail || currentRecipient.DeliveryMode == Constants.DeliveryModes.EmailAndMobileSlashEmailAndMobile || currentRecipient.DeliveryMode == Constants.DeliveryModes.EmailAndMobileSlashNone)
                                responseMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "ResponseEmailMobileFinishLater") + " " + UrlForFinishLater + "  to continue.";
                            else if (currentRecipient.DeliveryMode == Constants.DeliveryModes.MobileSlashMobile || currentRecipient.DeliveryMode == Constants.DeliveryModes.MobileSlashEmail || currentRecipient.DeliveryMode == Constants.DeliveryModes.MobileSlashNone || currentRecipient.DeliveryMode == Constants.DeliveryModes.MobileSlashEmailAndMobile)
                                responseMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "ResponseMobileFinishLater") + " " + UrlForFinishLater + "  to continue.";

                        }
                        else
                            responseMessage = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "responseFinishLater") + " " + UrlForFinishLater + "  to continue.";

                        _envelopeHelperMain.SaveControlsDetailForFinishLater(envelope, envelopeFinishLaterSubmitInfo.ControlCollection, currentRecipient, false);

                        Guid newSignerStatusId = _recipientRepository.GetSignerPrimaryStatusId(envelopeFinishLaterSubmitInfo.RecipientID);

                        //Update Webhook Event status.
                        _genericRepository.UpdateEnvelopeCommonWebhookTransaction(envelope, Constants.WebhookEventNames.SignerStatus, senderEmail, envelopeFinishLaterSubmitInfo.RecipientID.ToString(), newSignerStatusId.ToString());

                        // If user is prefil user and finish later selected then save envelope as draft and change envelope status as draft
                        if (currentRecipient.RecipientTypeID == Constants.RecipientType.Prefill)
                        {
                            _envelopeRepository.UpdateEnvelopePrefillSigner(envelope.ID, "Sign");
                        }
                        if (Convert.ToBoolean(envelopeFinishLaterSubmitInfo.IsSendEmailOnFinishLater))
                        {
                            _envelopeHelperMain.SendFinishLaterReminderEmail(envelope, envelopeFinishLaterSubmitInfo.UrlForFinishLater, currentRecipient, sender);
                        }

                        apiResponseMessage.StatusCode = HttpStatusCode.OK;
                        apiResponseMessage.StatusMessage = "OK";
                        apiResponseMessage.Message = responseMessage;
                        loggerModelNew.Message = "Process completed for Finish Later Submit using API and" + apiResponseMessage.Message;
                        apiResponseMessage.data = senderEmail;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return Ok(apiResponseMessage);
                    }
                    else
                    {
                        string tempStatus = envelope.StatusID == Constants.StatusCode.Envelope.Completed ? "Completed" :
                            envelope.StatusID == Constants.StatusCode.Envelope.Incomplete_and_Expired ? "Expired" :
                            envelope.StatusID == Constants.StatusCode.Envelope.Terminated ? "Declined" :
                            envelope.StatusID == Constants.StatusCode.Envelope.CancelledTransaction ? "Cancelled" :
                            "Unable to identify status";
                        loggerModelNew.Message = "Envelope status is - " + tempStatus;
                        loggerModelNew.Email = currentRecipient.EmailAddress;
                        rSignLogger.RSignLogWarn(loggerModelNew);


                        responseMessage = envelope.StatusID == Constants.StatusCode.Envelope.Completed ? _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "ConatctSender") :
                            envelope.StatusID == Constants.StatusCode.Envelope.Incomplete_and_Expired ? _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeExpired") :
                            envelope.StatusID == Constants.StatusCode.Envelope.Terminated ? _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeRejected") :
                            envelope.StatusID == Constants.StatusCode.Envelope.CancelledTransaction ? _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeCancelled") :
                            "Unexpected error occurred, please try to refresh and do the process again, or contact the sender of envelope";

                        apiResponseMessage.StatusCode = HttpStatusCode.BadRequest;
                        apiResponseMessage.StatusMessage = "BadRequest";
                        apiResponseMessage.Message = responseMessage;
                        apiResponseMessage.data = senderEmail;
                        return BadRequest(apiResponseMessage);
                    }
                }
                else
                {
                    loggerModelNew.Message = "This envelope was not found.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    apiResponseMessage.StatusCode = HttpStatusCode.BadRequest;
                    apiResponseMessage.StatusMessage = "BadRequest";
                    apiResponseMessage.Message = "This envelope was not found.";
                    apiResponseMessage.data = senderEmail;
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller FinishLaterSubmit action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageReject), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("ChangeSigner")]
        [HttpPost]
        public IActionResult ChangeSigner(ChangeSignerRequest objChangeSigner)
        {
            var remoteIpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
            objChangeSigner.IPAddress = remoteIpAddress;
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "ChangeSigner", "Initiate the process for Change Signer to " + objChangeSigner.SignerEmail, objChangeSigner.EnvelopeID.ToString(), "", "", remoteIpAddress, "API");
            if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail) && !string.IsNullOrEmpty(objChangeSigner.MobileNumber))
                loggerModelNew.Message = "Initiate the process for Change Signer to " + objChangeSigner.SignerEmail + ", " + objChangeSigner.DialCode + objChangeSigner.MobileNumber;
            else if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail))
                loggerModelNew.Message = "Initiate the process for Change Signer to " + objChangeSigner.SignerEmail;
            else if (!string.IsNullOrEmpty(objChangeSigner.MobileNumber))
                loggerModelNew.Message = "Initiate the process for Change Signer to " + objChangeSigner.DialCode + objChangeSigner.MobileNumber;

            rSignLogger.RSignLogInfo(loggerModelNew);

            string envelopeCode = string.Empty;
            string emailId = string.Empty;
            string currentRecipientEmailId = string.Empty;
            ResponseMessageReject responseMessage = new ResponseMessageReject();
            string authRefKey = string.Empty;
            string delegateMessageKey = string.Empty;
            try
            {
                Dictionary<Guid, Guid> controlIDMapping = new Dictionary<Guid, Guid>();
                Dictionary<Guid, Guid> ruleIDMapping = new Dictionary<Guid, Guid>();
                var envelope = _genericRepository.GetEntity(objChangeSigner.EnvelopeID);
                envelopeCode = envelope != null ? envelope.EDisplayCode.ToString() : "";
                var currentRecipient = _recipientRepository.GetEntity(objChangeSigner.CurrentRecipientID);
                currentRecipientEmailId = currentRecipient == null ? "" : currentRecipient.EmailAddress;
                if (envelope == null)
                {
                    loggerModelNew.Message = "Envelope Is already accepted";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeAccepted");
                    return BadRequest(responseMessage);
                }
                var envelopeContentData = _envelopeRepository.GetEnvelopeContent(envelope.ID);
                //Get original recipient signer status
                var signerStatusId = _recipientRepository.GetSignerStatusId(objChangeSigner.CurrentRecipientID);
                if (signerStatusId.Equals(Constants.StatusCode.Signer.Delegated))
                {
                    loggerModelNew.Message = "Envelope is already delegated";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeAlreadyDelegate");
                    return BadRequest(responseMessage);
                }
                else if (signerStatusId.Equals(Constants.StatusCode.Signer.Rejected))
                {
                    loggerModelNew.Message = "Envelope is already rejected";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeRejected");
                    return BadRequest(responseMessage);
                }
                else if (signerStatusId.Equals(Constants.StatusCode.Signer.Signed))
                {
                    loggerModelNew.Message = "Envelope is already Signed";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeAccepted");
                    return BadRequest(responseMessage);
                }
                else if (signerStatusId.Equals(Constants.StatusCode.Recipients.Transferred))
                {
                    loggerModelNew.Message = "Envelope is already transferred";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeTransferred");
                    return BadRequest(responseMessage);
                }

                //Assuming that sender exists in recipients list, if not than system should throw exception.
                var envelopeSender = envelope.Recipients.FirstOrDefault(r => r.RecipientTypeID == Constants.RecipientType.Sender);
                if (envelopeSender != null)
                    responseMessage.SenderEmail = envelopeSender.EmailAddress;

                string userDeliveryMode = objChangeSigner.DeliveryMode;
                if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail) && (userDeliveryMode == "1" || userDeliveryMode == "10" || userDeliveryMode == null || userDeliveryMode == "0" || userDeliveryMode == "" || userDeliveryMode == "null"))
                {
                    var rcpt = envelope.Recipients.FirstOrDefault(r => r.EmailAddress.ToLower() == objChangeSigner.SignerEmail.ToLower());
                    if (rcpt != null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "UserDelegatedPartOfEnvelope");
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }
                
                if (!string.IsNullOrEmpty(objChangeSigner.MobileNumber) && (userDeliveryMode == "4" || userDeliveryMode == "10"))
                {
                    var rcpt = envelope.Recipients.FirstOrDefault(r => (r.DialCode + r.Mobile) == (objChangeSigner.DialCode + objChangeSigner.MobileNumber));                                        
                    if (rcpt != null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "MobileDelegatedPartOfEnvelope");
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }

                // delegated recipient id
                var delegatedRecipientId = Guid.NewGuid();
                var oldString = "data-rcptid=" + "\"" + objChangeSigner.CurrentRecipientID + "\"";
                var newString = "data-rcptid=" + "\"" + delegatedRecipientId + "\"";

                //delegated control
                if (envelopeContentData == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeContentNotFouund");
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                var doc = XDocument.Parse(envelopeContentData.ContentXML);

                var documentContents = new List<DocumentContents>();
                foreach (var document in envelope.Documents)
                {
                    documentContents.AddRange(document.DocumentContents.Where(d => d.RecipientID == objChangeSigner.CurrentRecipientID));
                }
                foreach (var content in documentContents)
                {
                    var newDocumentContentId = Guid.NewGuid();
                    //content.ControlValue = null;
                    var Control = new DocumentContents
                    {
                        ID = newDocumentContentId,
                        RecipientID = delegatedRecipientId,
                        ControlID = content.ControlID,
                        DocumentID = content.DocumentID,
                        ControlHtmlData = content.ControlHtmlData.Replace(oldString, newString),
                        Required = content.Required,
                        XCoordinate = content.XCoordinate,
                        YCoordinate = content.YCoordinate,
                        ZCoordinate = content.ZCoordinate,
                        PageNo = content.PageNo,
                        Width = content.Width,
                        Height = content.Height,
                        Label = content.Label,
                        ControlHtmlID = content.ControlHtmlID,
                        ControlValue = content.ControlValue,
                        SenderControlValue = content.SenderControlValue,
                        DocumentPageNo = content.DocumentPageNo,
                        GroupName = content.GroupName,
                        MaxLength = content.MaxLength,
                        ControlType = content.ControlType,
                        RecName = objChangeSigner.SignerName,
                        IsControlDeleted = content.IsControlDeleted,
                        LeftIndex = content.LeftIndex,
                        TopIndex = content.TopIndex,
                        SignatureControlValue = content.SignatureControlValue,
                        IsReadOnly = content.IsReadOnly,
                        TabIndex = content.TabIndex,
                        MappedTemplateControlID = content.MappedTemplateControlID,
                        TemplateDocumentPageNo = content.TemplateDocumentPageNo,
                        TemplatePageNo = content.TemplatePageNo,
                        OriginalControlValue = content.OriginalControlValue,
                        LastModifiedBy = content.LastModifiedBy,
                        IntControlId = content.IntControlId,
                        IsDefaultRequired = content.IsDefaultRequired,
                        CustomToolTip = content.CustomToolTip,
                        IsFixedWidth = content.IsFixedWidth == null ? true : Convert.ToBoolean(content.IsFixedWidth) ? true : false,
                    };
                    _documentContentsRepository.Save(Control);
                    var controlStyle = _documentContentsRepository.GetControlStyle(content.ID);
                    if (controlStyle != null)
                    {
                        var controlStyleObject = new ControlStyle
                        {
                            DocumentContentId = newDocumentContentId,
                            FontColor = controlStyle.FontColor,
                            FontID = controlStyle.FontID,
                            FontSize = controlStyle.FontSize,
                            IsBold = controlStyle.IsBold,
                            IsItalic = controlStyle.IsItalic,
                            IsUnderline = controlStyle.IsUnderline,
                            AdditionalValidationName = controlStyle.AdditionalValidationName,
                            AdditionalValidationOption = controlStyle.AdditionalValidationOption
                        };
                        _documentContentsRepository.Save(controlStyleObject);
                    }
                    if (content.ControlID == Constants.Control.DropDown)
                    {
                        IQueryable<SelectControlOptions> selectControlOptions = _documentContentsRepository.GetSelectControlOption(content.ID);
                        if (selectControlOptions != null)
                        {
                            foreach (var control in selectControlOptions)
                            {
                                var selectControlObject = new SelectControlOptions
                                {
                                    ID = Guid.NewGuid(),
                                    DocumentContentID = newDocumentContentId,
                                    OptionText = control.OptionText,
                                    Order = control.Order
                                };
                                _documentContentsRepository.Save(selectControlObject);
                                if (!ruleIDMapping.ContainsKey(control.ID))
                                    ruleIDMapping.Add(control.ID, selectControlObject.ID);
                            }
                        }
                    }
                    if (!controlIDMapping.ContainsKey(content.ID))
                        controlIDMapping.Add(content.ID, newDocumentContentId);

                    string controlName = string.Empty;
                    var objControl = _documentContentsRepository.GetControlData(content.ControlID);
                    controlName = objControl != null ? objControl.ControlName : string.Empty;

                    var xElement = new XElement("Control",
                                                    new XAttribute("ID", Control.ID),
                                                    new XAttribute("Name", controlName ?? ""),
                                                    new XAttribute("label", Control.Label ?? ""),
                                                    new XAttribute("text", Control.ControlValue ?? ""),
                                                    new XAttribute("required", Control.Required),
                                                    new XAttribute("Height", Control.Height.GetValueOrDefault()),
                                                    new XAttribute("Width", Control.Width.GetValueOrDefault()),
                                                    new XAttribute("PageNo", Control.PageNo.GetValueOrDefault()),
                                                    new XAttribute("XCoordinate", Control.XCoordinate.GetValueOrDefault()),
                                                    new XAttribute("YCoordinate", Control.YCoordinate.GetValueOrDefault()),
                                                    new XAttribute("ZCoordinate", Control.ZCoordinate.GetValueOrDefault()),
                                                    new XElement("Signer",
                                                                new XAttribute("Name", objChangeSigner.SignerName ?? ""),
                                                                new XAttribute("EmailAddress", objChangeSigner.SignerEmail ?? "")));

                    var element = doc.Element("Envelope");
                    if (element == null) continue;

                    var firstOrDefault = element.Elements("Documents").Elements("Documents").FirstOrDefault(m => m.Attribute("ID").Value == content.DocumentID.ToString());
                    if (firstOrDefault == null) continue;

                    var controlElements = firstOrDefault.Elements("Controls").FirstOrDefault();
                    if (controlElements != null)
                        controlElements.Add(xElement);
                }
                envelopeContentData.ContentXML = doc.ToString();
                _envelopeRepository.Save(envelopeContentData);

                // get ipaddress

                string ipAddress = objChangeSigner.IPAddress;
                // added new object of signerstatus in db
                _recipientRepository.AddDelegatedSigner(objChangeSigner.CurrentRecipientID, Constants.StatusCode.Signer.Delegated, delegatedRecipientId, ipAddress);

                Recipients recipientDetail = _recipientRepository.GetEntity(objChangeSigner.CurrentRecipientID);
                if (objChangeSigner.SendFinalSignDocumentChangeSigner)
                {
                    _recipientRepository.UpdateIsSendFinalDocumentOnDelegate(recipientDetail);
                }
                //Create new recipient
                var recipient = new Recipients
                {
                    ID = delegatedRecipientId,
                    Name = objChangeSigner.SignerName,
                    EmailAddress = objChangeSigner.SignerEmail,
                    EnvelopeID = recipientDetail.EnvelopeID,
                    RecipientTypeID = recipientDetail.RecipientTypeID,
                    Order = recipientDetail.Order,
                    CreatedDateTime = DateTime.Now,
                    RecipientCode = _envelopeHelperMain.TakeUniqueDisplayCodeForRecipient(),
                    TemplateID = recipientDetail.TemplateID,
                    EnvelopeTemplateGroupID = recipientDetail.EnvelopeTemplateGroupID,
                    TemplateRoleId = recipientDetail.TemplateRoleId,
                    Comments = objChangeSigner.SignerComments,
                    CultureInfo = objChangeSigner.CultureInfo,
                    DeliveryMode = int.Parse(objChangeSigner.DeliveryMode),
                    DialCode = objChangeSigner.DialCode,
                    CountryCode = (objChangeSigner.CountryCode != null && objChangeSigner.CountryCode != "") ? objChangeSigner.CountryCode.ToUpper() : null,
                    Mobile = (objChangeSigner.MobileNumber != null && objChangeSigner.MobileNumber != "") ? objChangeSigner.MobileNumber : null,
                };
                // inserted new recipient
                _recipientRepository.Save(recipient);
                if (recipientDetail.EnvelopeTemplateGroupID != null)
                {
                    recipientDetail.EnvelopeTemplateGroupID = null;
                    recipientDetail.TemplateRoleId = null;
                    recipientDetail.TemplateID = null;
                    _recipientRepository.Save(recipientDetail);
                }
                Guid SignerStatusNewID = Guid.NewGuid();
                _recipientRepository.Save(new SignerStatus { ID = SignerStatusNewID, RecipientID = delegatedRecipientId, StatusID = Constants.StatusCode.Signer.Pending, CreatedDateTime = DateTime.Now.AddSeconds(1) });

                List<EnvelopeAdditionalUploadInfoDetailsDelegate> envelopeAdditionalUploadInfoDetails = new List<EnvelopeAdditionalUploadInfoDetailsDelegate>();
                envelopeAdditionalUploadInfoDetails = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByDelegate(envelope.ID, objChangeSigner.CurrentRecipientID);
                if (envelopeAdditionalUploadInfoDetails != null)
                {
                    foreach (var envelopeDel in envelopeAdditionalUploadInfoDetails)
                    {
                        EnvelopeAdditionalUploadInfoDetailsDelegate envelopeDelegate = new EnvelopeAdditionalUploadInfoDetailsDelegate();
                        envelopeDelegate.ID = envelopeDel.ID;
                        envelopeDelegate.RecipientID = recipient.ID;
                        envelopeDelegate.RecipientEmailID = objChangeSigner.SignerEmail.ToLower();
                        _envelopeRepository.UpdateEnvelopeAdditionalUploadInfobyDelegate(envelopeDelegate);
                    }
                }

                var dirPath = _modelHelper.GetEnvelopeDirectoryNew(envelope.ID, string.Empty) + Convert.ToString(envelope.ID);
                if (Directory.Exists(dirPath))
                {
                    var SignerAttchdirPath = dirPath + "\\" + "SignerAttachments";
                    if (Directory.Exists(SignerAttchdirPath))
                    {
                        var CurrentRecipientdirPath = SignerAttchdirPath + "\\" + objChangeSigner.CurrentRecipientID;
                        if (Directory.Exists(CurrentRecipientdirPath))
                        {
                            var delegatedirPath = SignerAttchdirPath + "\\" + delegatedRecipientId;
                            System.IO.Directory.Move(CurrentRecipientdirPath, delegatedirPath);
                        }
                    }
                }

                // checked rpostuser 
                _userRepository.CheckRpostUser(recipient);

                //Conditional Control mapping                    
                foreach (var control in controlIDMapping)
                {
                    ConditionalControlsDetailsNew templateControlRules = _conditionalControlRepository.GetAllConditionalControl("", envelope.ID, control.Key, null);
                    if (templateControlRules == null || templateControlRules.DependentFields == null || templateControlRules.DependentFields.Count < 1)
                        continue;
                    ConditionalControlsDetailsNew tempRule = new ConditionalControlsDetailsNew();

                    tempRule.ID = Guid.NewGuid();
                    tempRule.ControlID = control.Value;
                    tempRule.ControllingFieldID = templateControlRules.ControllingFieldID;
                    tempRule.ControllingConditionID = templateControlRules.ControllingConditionID;
                    tempRule.ControllingSupportText = templateControlRules.ControllingSupportText;
                    tempRule.EnvelopeID = envelope.ID;
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
                                SupportText = cond.SupportText,
                                IsRequired = documentContents.FirstOrDefault(f => f.ID == cond.ControlID) != null ? documentContents.FirstOrDefault(f => f.ID == cond.ControlID).Required : false
                            });
                        }

                    }
                    _conditionalControlRepository.SaveConditionalControlForSigner(tempRule);
                }
                 
                //delegated to email template
                var mailSender = envelopeSender != null ? envelopeSender.EmailAddress : Convert.ToString(_appConfiguration["SystemEmailAddress"]);
                UserProfile CopyusersInfo = !string.IsNullOrEmpty(mailSender) ? _userRepository.GetUserProfileByEmailID(mailSender) : null;
                var senderName = CopyusersInfo != null ? CopyusersInfo.FirstName + " " + CopyusersInfo.LastName : null; //envelopeRepository.GetUserNameByEmailid(mailSender) 

                _envelopeHelperMain.DelegateToMail(objChangeSigner.SignerName, objChangeSigner.SignerEmail, envelope, recipientDetail, delegatedRecipientId, mailSender, senderName, ipAddress, "", authRefKey, recipient.RecipientCode, recipient);

                _envelopeHelperMain.DelegatedSenderMail(objChangeSigner.SignerName, objChangeSigner.SignerEmail, envelope, recipientDetail, delegatedRecipientId, envelopeSender, mailSender, ipAddress, "", authRefKey, recipient);

                if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail) && !string.IsNullOrEmpty(recipient.Mobile))
                    loggerModelNew.Message = "Delegating to" + objChangeSigner.SignerEmail + ", " + recipient.DialCode + recipient.Mobile + " mail triggering process completed successfully";
                else if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail))
                    loggerModelNew.Message = "Delegating to" + objChangeSigner.SignerEmail + " mail triggering process completed successfully";
                else
                    loggerModelNew.Message = "Delegating to" + recipient.DialCode + recipient.Mobile + " mail triggering process completed successfully";

                rSignLogger.RSignLogInfo(loggerModelNew);

                delegateMessageKey = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeDelegatedTo");

                //Guid newSignerStatusId = recipientRepository.GetSignerPrimaryStatusId(delegatedRecipientId);

                //Update Webhook Event status.
                _genericRepository.UpdateEnvelopeCommonWebhookTransaction(envelope, Constants.WebhookEventNames.SignerStatus, mailSender, delegatedRecipientId.ToString(), SignerStatusNewID.ToString());

                //Update Webhook Event status to add delegated notification and added delegateMsgforNotifications.
                string delegateMsgforNotifications = recipientDetail.Name + " has delegated your document to " + objChangeSigner.SignerName.Split(' ').FirstOrDefault() + " for electronic signature.";
                _genericRepository.UpdateEnvelopeCommonWebhookTransaction(envelope, Constants.WebhookEventNames.NotificationType, mailSender, delegatedRecipientId.ToString(), SignerStatusNewID.ToString(), delegateMsgforNotifications);

                //Update current signer notification signing request to IsRead = 1 because he delegated to some others.
                _envelopeRepository.UpdateRAppNotificationEvents(recipientDetail, envelope);

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = delegateMessageKey;
                responseMessage.EncryptedSender = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(Convert.ToString(responseMessage.SenderEmail), Convert.ToString(_appConfiguration["AppKey"])));
                loggerModelNew.Message = "Process completed for Change signer and " + responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller Change Signer action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageReject), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("UpdatedEvelopeStatusbySigner")]
        [HttpPost]
        public IActionResult UpdatedEvelopeStatusbySigner(RecipientsDetailsAPI objSignDocParam)
        {
            var remoteIpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
            objSignDocParam.IPAddress = remoteIpAddress;

            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "UpdatedEvelopeStatusbySigner", "Process started for Updated Evelope Status by Signer method using API", objSignDocParam.ID.ToString(), "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageReject responseMessage = new ResponseMessageReject();
            string envelopeCode = string.Empty;
            string emailId = string.Empty, senderEmail = string.Empty;
            Guid envelopeUserId = Guid.Empty;
            string comments = string.Empty;
            bool isNonRegisterdUser = false;
            try
            {
                var userSettingsForfinalMergePDF = new FinalContractSettings();
                string subject = Convert.ToString(_appConfiguration["Subject_EnvelopeRejected"]);
                var envelopeObject = _genericRepository.GetEntity(objSignDocParam.ID);
                envelopeUserId = envelopeObject == null ? Guid.Empty : envelopeObject.UserID;
                envelopeCode = envelopeObject == null ? "" : envelopeObject.EDisplayCode.ToString();
                var senderDetails = _recipientRepository.GetSenderDetails(objSignDocParam.ID);
                var signerDetails = _recipientRepository.GetEntity(objSignDocParam.RecipientID);
                Guid signerStatusId = _recipientRepository.GetSignerStatusId(objSignDocParam.RecipientID);
                emailId = signerDetails != null ? signerDetails.EmailAddress : "";
                responseMessage.EnvelopeCode = envelopeCode;

                if (envelopeObject == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"].ToString());
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                var sender = envelopeObject.Recipients.FirstOrDefault(r => r.RecipientTypeID == Constants.RecipientType.Sender);
                senderEmail = sender != null ? sender.EmailAddress : string.Empty;
                if (envelopeObject.StatusID == Constants.StatusCode.Envelope.Incomplete_and_Expired)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Convert.ToString(_appConfiguration["IncompleteAndExpired"].ToString());
                    responseMessage.SenderEmail = senderEmail;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                else if (envelopeObject.StatusID == Constants.StatusCode.Envelope.Terminated)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Convert.ToString(_appConfiguration["Terminated"].ToString());
                    responseMessage.SenderEmail = senderEmail;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }// check signer status
                else if (signerStatusId.Equals(Constants.StatusCode.Signer.Delegated))
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "This envelope is delegated, and is not available for signing.";
                    responseMessage.SenderEmail = senderEmail;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                else if (signerStatusId.Equals(Constants.StatusCode.Signer.Rejected))
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Convert.ToString(_appConfiguration["Rejected"].ToString());
                    responseMessage.SenderEmail = senderEmail;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                else if (signerStatusId.Equals(Constants.StatusCode.Recipients.Transferred))
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "The contract has already been transfer to other signer, in case you have any query please contact to sender of contract";
                    responseMessage.SenderEmail = senderEmail;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                else if (signerStatusId.Equals(Constants.StatusCode.Signer.Signed))
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Convert.ToString(_appConfiguration["SignDocEnvelopeAlreadySigned"].ToString());
                    responseMessage.SenderEmail = senderEmail;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                // added recipient status object
                _recipientRepository.AddSignerRemark(objSignDocParam.RecipientID, objSignDocParam.SignerStatusDescription, objSignDocParam.IPAddress, objSignDocParam.DeclineReasonID, objSignDocParam.CopyEmailAddress);

                var envelopeStatus = new EnvelopeStatus();
                Guid envelopeStatusId = Constants.StatusCode.Envelope.Terminated;
                envelopeStatus.CreatedDateTime = DateTime.Now;
                envelopeStatus.EnvelopeID = objSignDocParam.ID;
                envelopeStatus.ID = Guid.NewGuid();
                envelopeStatus.StatusID = envelopeStatusId;
                // added new object in envelope status
                _envelopeRepository.Save(envelopeStatus);
                // updated status in envelope table
                _genericRepository.UpdateEnvelopeStatus(objSignDocParam.ID, envelopeStatusId);
                envelopeObject.StatusID = envelopeStatusId;

                //If draft type is sign and exists in drafts then when prefill signer decline the envelope then remove from drafts
                if (envelopeObject.DraftType == "Sign" && signerDetails.RecipientTypeID == Constants.RecipientType.Prefill)
                {
                    _envelopeRepository.RemovePrefillEnvelopeFromDrafts(objSignDocParam.ID);
                }

                string dirPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeObject.ID, string.Empty);
                // Update "Envelope.xml" file at temp location. Update "IsEnvelopeCompleted" field.
                var dictionary = new Dictionary<EnvelopeNodes, string> { { EnvelopeNodes.IsEnvelopeRejected, "true" } };
                _eSignHelper.UpdateEnvelopeXML(objSignDocParam.ID, dictionary, dirPath);

                APISettings apiSettings = _settingsRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
                var userSettings = _eSignHelper.TransformSettingsDictionaryToEntity(apiSettings);
                string finalPdfFilePath = string.Empty;
                //var companyProfile = companyRepository.GetCompanyProfileByEnvelopeID(envelopeObject.ID);
                int isContractToGenerateFromImages = Convert.ToInt32(userSettings.FinalContractOptionID) > 0 ? userSettings.FinalContractOptionID : Constants.FinalContractOptions.Aspose;//(companyProfile != null) ? Convert.ToInt32(companyProfile.FinalContractOptionID) : Constants.FinalContractOptions.Aspose;
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
                    finalPdfFilePath = _apiHelper.finalMergePDFApi(envelopeObject, userSettingsForfinalMergePDF, dirPath, "", Convert.ToBoolean(envelopeObject.IsSeparateMultipleDocumentsAfterSigningRequired) ? true : false);
                }
                catch (Exception Ex)
                {
                    _envelopeHelperMain.DeleteContractFileInCaseOfError(envelopeObject.ID, dirPath);
                    userSettingsForfinalMergePDF.FinalContractOptions = Convert.ToInt32(userSettingsForfinalMergePDF.FinalContractOptions) != Constants.FinalContractOptions.iText ? Constants.FinalContractOptions.iText : Constants.FinalContractOptions.Aspose;
                    finalPdfFilePath = _apiHelper.finalMergePDFApi(envelopeObject, userSettingsForfinalMergePDF, dirPath, "", Convert.ToBoolean(envelopeObject.IsSeparateMultipleDocumentsAfterSigningRequired) ? true : false);
                }

                UserProfile userProfile = _userRepository.GetUserProfileByEmailID(emailId);
                responseMessage.RecipientOrder = userProfile != null ? 1 : 0;
                if (!string.IsNullOrEmpty(objSignDocParam.RSignAnonymousToken) && userProfile != null)
                {
                    var userTokenProfile = _userTokenRepository.GetUserProfileByToken(objSignDocParam.RSignAnonymousToken);
                    if (userTokenProfile != null && userTokenProfile.EmailID.Trim() == emailId.Trim())
                        responseMessage.RecipientOrder = 2;
                }

                EnvelopeSettingsDetail envelopeSettingObject = _envelopeRepository.GetEnvelopeSettingsDetail(envelopeObject.ID);
                if (envelopeSettingObject != null)
                {
                    responseMessage.AttachSignedPdfID = envelopeSettingObject.AttachSignedPdf;
                }

                responseMessage.EnablePostSigningLoginPopup = true;
                var usersettingsDetails = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.EnablePostSigningLoginPopup);
                if (usersettingsDetails != null)
                    responseMessage.EnablePostSigningLoginPopup = Convert.ToBoolean(usersettingsDetails.OptionValue);
                responseMessage.RecipientName = signerDetails != null ? signerDetails.Name : string.Empty;
                responseMessage.EmailAddress = string.IsNullOrEmpty(objSignDocParam.CopyEmailAddress) ? emailId : objSignDocParam.CopyEmailAddress;
                if (objSignDocParam.SignerStatusDescription != null)
                {
                    var declineTemplateSetting = JsonConvert.DeserializeObject<List<DeclineTemplateResponses>>(objSignDocParam.SignerStatusDescription);
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
                string envelopeDirectoryPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeObject.ID, string.Empty);
                _envelopeHelperMain.RejectToMail(!string.IsNullOrEmpty(comments) ? comments : objSignDocParam.SignerStatusDescription, "Terminated: " + envelopeObject.Subject, finalPdfFilePath, envelopeObject, signerDetails, senderDetails, "", envelopeDirectoryPath);

                _envelopeHelperMain.RejectMailToSender(!string.IsNullOrEmpty(comments) ? comments : objSignDocParam.SignerStatusDescription, "Terminated: " + envelopeObject.Subject, finalPdfFilePath, envelopeObject, signerDetails, senderDetails, "", envelopeDirectoryPath);

                loggerModelNew.Message = "Process completed for RejectToMail and RejectMailToSender methods";
                rSignLogger.RSignLogInfo(loggerModelNew);

                ////Hack to fix issue : on decline for non registered user on click on No Thanks the user is redirected to global error page
                isNonRegisterdUser = (userProfile == null) ? true : false;
                Guid newSignerStatusId = _recipientRepository.GetSignerPrimaryStatusId(objSignDocParam.RecipientID);

                //Update Webhook Event status.
                _genericRepository.UpdateEnvelopeCommonWebhookTransaction(envelopeObject, Constants.WebhookEventNames.SignerStatus, senderDetails.EmailAddress, objSignDocParam.RecipientID.ToString(), newSignerStatusId.ToString());
                //Added EnvelopeTerminated Notification event
                _genericRepository.UpdateEnvelopeCommonWebhookTransaction(envelopeObject, Constants.WebhookEventNames.EnvelopeStatus, senderDetails.EmailAddress, objSignDocParam.RecipientID.ToString(), newSignerStatusId.ToString());

                responseMessage.EnvelopeStatus = Constants.StatusCode.Envelope.Terminated;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeUserId, "EnvelopeRejectedWithMail");
                responseMessage.SenderEmail = senderEmail;

                //responseMessage.Message = "The envelope is rejected and email notification is sent to sender.";
                loggerModelNew.Message = "Process completed for Updated Evelope Status by Signer and " + responseMessage.Message;
                responseMessage.IsNonRegisteredUser = isNonRegisterdUser;
                responseMessage.EncryptedSender = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(Convert.ToString(senderEmail), Convert.ToString(_appConfiguration["AppKey"])));
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller UpdatedEvelopeStatusbySigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(InfoResultResonse), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("SignDocument")]
        [HttpPost]
        public IActionResult SignDocument(EnvelopeSignDocumentSubmitInfo envelopeSignDocumentSubmitInfo)
        {
            var remoteIpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
            envelopeSignDocumentSubmitInfo.IpAddress = remoteIpAddress;

            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "SignDocument", "Initiated the process of submit document", envelopeSignDocumentSubmitInfo.EnvelopeID.ToString(), "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            InfoResultResonse responseMessage = new InfoResultResonse();
            string strTimeZone = string.Empty, envelopeCode = string.Empty, postSigningPage = string.Empty, errorIfFound = string.Empty, currentRecipientEmailId = string.Empty;
            bool isSignDocument = false, enablePostSigningLoginPopup = true;

            APIRecipientEntity recipients = new APIRecipientEntity();
            try
            {
                Guid signerStatusId = _recipientRepository.GetSignerStatusId(envelopeSignDocumentSubmitInfo.RecipientID);
                Envelope env = _genericRepository.GetEntity(envelopeSignDocumentSubmitInfo.EnvelopeID);
                envelopeCode = env != null ? env.EDisplayCode.ToString() : "";
                currentRecipientEmailId = env.Recipients.FirstOrDefault(r => r.ID == envelopeSignDocumentSubmitInfo.RecipientID).EmailAddress;
                var recipentsdata = env.Recipients.FirstOrDefault(r => r.ID == envelopeSignDocumentSubmitInfo.RecipientID);
                recipentsdata.IsReviewed = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.IsReviewed) ? Convert.ToInt32(envelopeSignDocumentSubmitInfo.IsReviewed) : 0;
                _recipientRepository.Save(recipentsdata);

                if (signerStatusId == Constants.StatusCode.Recipients.Transferred)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract is trasferred";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = "The contract has already been transfer to other signer, in case you have any query please contact to sender of contract";
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }
                if (signerStatusId == Constants.StatusCode.Signer.Delegated)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has already been delegated to other signer";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = "This envelope is delegated, and is not available for signing.";
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }
                if (signerStatusId == Constants.StatusCode.Signer.Rejected)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has already been rejected";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = "Envelope Rejected";
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }
                var signerSignedStatus = _recipientRepository.GetSignerSignedStatusId(envelopeSignDocumentSubmitInfo.RecipientID);
                if (signerSignedStatus != null)
                {
                    loggerModelNew.Message = currentRecipientEmailId + "'s contract has already been signed";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = "This envelope is already submitted.";
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }

                /* Get User Settings */
                var settingsDetails = _settingsRepository.GetEntityForByKeyConfig(env.UserID, Constants.SettingsKeyConfig.TimeZone);
                if (settingsDetails != null)
                    strTimeZone = settingsDetails.OptionValue;
                var usersettingsDetails = _settingsRepository.GetEntityForByKeyConfig(env.UserID, Constants.SettingsKeyConfig.EnablePostSigningLoginPopup);
                if (usersettingsDetails != null)
                    enablePostSigningLoginPopup = Convert.ToBoolean(usersettingsDetails.OptionValue);
                env.IpAddress = !string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.IpAddress) ? envelopeSignDocumentSubmitInfo.IpAddress : remoteIpAddress; //;
                Guid? subEnvelopeId = env.Recipients.FirstOrDefault(r => r.ID == envelopeSignDocumentSubmitInfo.RecipientID).EnvelopeTemplateGroupID;
                if (subEnvelopeId != null && subEnvelopeId.Value != Guid.Empty)
                    env.SubEnvelopeId = subEnvelopeId;

                var envelopeObject = _genericRepository.GetEntity(env.ID);
                if (envelopeSignDocumentSubmitInfo.IsConfirmationEmailReq)
                {
                    UserProfile userProfiles = _userRepository.GetUserProfileByUserID(env.UserID);
                    var isConfirmationMailSent = false;

                    isConfirmationMailSent = _envelopeHelperMain.AllowSendConfirmationEmailForStaticTemplate(envelopeObject, env.ID, userProfiles, Convert.ToString(envelopeSignDocumentSubmitInfo.RecipientID), env.IsEnvelopeComplete, envelopeSignDocumentSubmitInfo.ControlCollection, envelopeSignDocumentSubmitInfo.CertificateSignature);//envelopeSignDocumentSubmitInfo.ControlCollection
                    if (env.IsEnvelopeComplete)
                    {
                        responseMessage.StatusCode = HttpStatusCode.OK;
                        responseMessage.StatusMessage = "OK";//objEnvelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "EnvelopeDiscarded");
                        responseMessage.message = envelopeSignDocumentSubmitInfo.IsConfirmationEmailReq ? _envelopeHelperMain.GetLanguageBasedApiMessge(env.UserID, "RequiredConfirmation") : Convert.ToString(_appConfiguration["SuccessInitializeEnvelope"]);//ConfigurationManager.AppSettings["Subject_EnvelopeRejected"].ToString();
                        responseMessage.success = true;
                        responseMessage.data = env.ID;
                        responseMessage.returnUrl = envelopeSignDocumentSubmitInfo.IsConfirmationEmailReq ? "Info/Index" : postSigningPage;
                        responseMessage.postSigningLogin = enablePostSigningLoginPopup;//true;
                        return Ok(responseMessage);
                    }
                }
                else
                {
                    isSignDocument = _envelopeHelperMain.SignDocument(envelopeObject, envelopeSignDocumentSubmitInfo.ControlCollection, env, envelopeSignDocumentSubmitInfo.RecipientID, strTimeZone, out errorIfFound, envelopeSignDocumentSubmitInfo.CertificateSignature, envelopeSignDocumentSubmitInfo.CopyEmail);
                }

                UserProfile userProfile = _userRepository.GetUserProfileByEmailID(string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.CopyEmail) ? currentRecipientEmailId : envelopeSignDocumentSubmitInfo.CopyEmail);
                recipients.Order = userProfile != null ? 1 : 0;

                try
                {
                    int PostSendingLandingValue = 0;
                    if (env.Recipients.Any(rec => rec.ID == envelopeSignDocumentSubmitInfo.RecipientID && rec.RecipientTypeID == Constants.RecipientType.Prefill))
                    {
                        try
                        {
                            PostSendingLandingValue = Convert.ToInt32(_settingsRepository.GetEntityForByKeyConfig(env.UserID, Constants.SettingsKeyConfig.PostSendingNavigationPage, Constants.String.SettingsType.User).OptionValue);
                            postSigningPage = PostSendingLandingValue == 0 ? "DocumentPackage/DocumentPackageIndex" : "Home/Index";
                        }
                        catch (Exception)
                        {
                            postSigningPage = PostSendingLandingValue == 0 ? "DocumentPackage/DocumentPackageIndex" : "Home/Index";
                        }
                    }

                    else if (env.IsTemplateShared)
                    {
                        Recipients recordExistsForSameRecipientEmail = null;
                        List<Recipients> recordForSameRecipientEmailList = null;
                        if (!string.IsNullOrEmpty(currentRecipientEmailId))
                        {
                            recordForSameRecipientEmailList = env.Recipients.Where(a => a.EmailAddress == currentRecipientEmailId && a.ID != envelopeSignDocumentSubmitInfo.RecipientID && a.RecipientTypeID == Constants.RecipientType.Signer).ToList();
                        }
                        else if (recipentsdata != null && !string.IsNullOrEmpty(recipentsdata.Mobile))
                        {
                            recordForSameRecipientEmailList = env.Recipients.Where(a => (a.DialCode + a.Mobile) == (recipentsdata.DialCode + recipentsdata.Mobile) && a.ID != envelopeSignDocumentSubmitInfo.RecipientID && a.RecipientTypeID == Constants.RecipientType.Signer).ToList();
                        }

                        if (recordForSameRecipientEmailList != null && recordForSameRecipientEmailList.Count > 0)
                        {
                            foreach (var record in recordForSameRecipientEmailList)
                            {
                                Guid sameSignerStatusId = _recipientRepository.GetSignerStatusId(record.ID);
                                if (sameSignerStatusId == Constants.StatusCode.Signer.Pending || sameSignerStatusId == Constants.StatusCode.Signer.Viewed)
                                {
                                    recordExistsForSameRecipientEmail = record;
                                    break;
                                }
                            }
                        }
                        var senderDetails = env.Recipients.Where(a => a.RecipientTypeID == Constants.RecipientType.Sender).FirstOrDefault();
                        var senderEmailAddress = "";

                        if (recordExistsForSameRecipientEmail != null)
                        {
                            var strURL = "";
                            var strURLForSecurityCode = "";
                            if (senderDetails != null)
                            {
                                senderEmailAddress = senderDetails.EmailAddress;
                                if (_envelopeHelperMain.IsGenerateNewSigningUrl(senderEmailAddress))
                                {
                                    strURL = Convert.ToString(_appConfiguration["NewSigningURL"]) + "signer-landing?";
                                    strURLForSecurityCode = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/security-code?"; //URLTOGenerateCode
                                }
                                else
                                {
                                    strURL = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/Index?";
                                    strURLForSecurityCode = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/SignDocuments"; //URLTOGenerateCode
                                }
                            }
                            else
                            {
                                strURL = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/Index?";
                                strURLForSecurityCode = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/SignDocuments"; //URLTOGenerateCode
                            }

                            //var strURL = Convert.ToString(ConfigurationManager.AppSettings["SigningURL"]) + "/SignDocument/Index?";
                            //var strURLForSecurityCode = Convert.ToString(ConfigurationManager.AppSettings["SigningURL"]) + "/SignDocument/SignDocuments"; //URLTOGenerateCode

                            var strURLWithData = strURL;

                            if (env.IsStatic == true)
                            {
                                if (senderDetails != null)
                                {
                                    senderEmailAddress = senderDetails.EmailAddress;
                                }
                                strURLWithData = _envelopeHelperMain.getSigningURL(senderEmailAddress, Convert.ToString(env.TemplateKey), env.ID.ToString(), recordExistsForSameRecipientEmail.ID.ToString(), recordExistsForSameRecipientEmail.EmailAddress);
                            }
                            else
                            {
                                if (env.TemplateKey != null)
                                {
                                    strURLWithData = strURLWithData + HttpUtility.UrlEncode(_envelopeHelperMain.EncryptQueryString(string.Format("EnvelopeId={0}&RecipientId={1}&TemplateKey={2}&EmailId={3}&isDirect=1&IsFromInbox={4}", env.ID, recordExistsForSameRecipientEmail.ID, env.TemplateKey, HttpUtility.UrlEncode(recordExistsForSameRecipientEmail.EmailAddress), envelopeSignDocumentSubmitInfo.IsFromInbox ? 1 : 0)));
                                }
                                else
                                {
                                    strURLWithData = strURLWithData + HttpUtility.UrlEncode(_envelopeHelperMain.EncryptQueryString(string.Format("EnvelopeId={0}&RecipientId={1}&TemplateKey={2}&EmailId={3}&isDirect=1&IsFromInbox={4}", env.ID, recordExistsForSameRecipientEmail.ID, string.Empty, HttpUtility.UrlEncode(recordExistsForSameRecipientEmail.EmailAddress), envelopeSignDocumentSubmitInfo.IsFromInbox ? 1 : 0)));
                                }
                            }
                            postSigningPage = strURLWithData;

                        }
                        responseMessage.DGReturnURL = "/SignDocument/SignMultiTemplate";
                        responseMessage.isDGFlow = true;
                        if (string.IsNullOrEmpty(postSigningPage))
                            postSigningPage = responseMessage.DGReturnURL;
                        else
                            responseMessage.showDGPopup = true;
                    }
                    else if (!string.IsNullOrEmpty(env.PostSigningLandingPage))
                        postSigningPage = env.PostSigningLandingPage;
                    else
                    {
                        //If signer userprofile is not null then he is rsign registered user. so get sender settings
                        UserProfile envOwner = _userRepository.GetUserProfile(env.UserID); //sender userid
                        if (envOwner.CompanyID == null)
                        {
                            if (userProfile == null) //userprofile with Signer email
                            {
                                postSigningPage = _settingsRepository.GetNonRegisteredUserDefaultLandingPage(env.UserID, Constants.SettingsKeyConfig.NonRegisteredUserDefaultLandingPage);
                            }
                            else
                                postSigningPage = _settingsRepository.GetRegisteredUserDefaultLandingPage(envOwner.UserID, Constants.SettingsKeyConfig.RegisteredUserDefaultLandingPage);
                        }
                        else
                        {
                            var company = _companyRepository.GetCompanyByID(envOwner.CompanyID);
                            if (company != null && !string.IsNullOrEmpty(company.PostSigningLandingPage))
                            {
                                postSigningPage = company.PostSigningLandingPage;
                            }
                            else if (userProfile == null) //userprofile with Signer email
                            {
                                postSigningPage = _settingsRepository.GetNonRegisteredUserDefaultLandingPage(env.UserID, Constants.SettingsKeyConfig.NonRegisteredUserDefaultLandingPage);
                            }
                            else
                                postSigningPage = _settingsRepository.GetRegisteredUserDefaultLandingPage(envOwner.UserID, Constants.SettingsKeyConfig.RegisteredUserDefaultLandingPage);
                        }


                        //UserProfile envOwner = userProfileRepository.GetUserProfile(env.UserID);
                        //if (envOwner.CompanyID == null)
                        //{
                        //    postSigningPage = settingRepository.GetNonRegisteredUserDefaultLandingPage(envOwner.UserID, Constants.SettingsKeyConfig.NonRegisteredUserDefaultLandingPage);                                
                        //}                                
                        //else
                        //{
                        //    var company = companyRepositry.GetCompanyByID(envOwner.CompanyID);
                        //    postSigningPage = settingRepository.GetRegisteredUserDefaultLandingPage(envOwner.UserID, Constants.SettingsKeyConfig.RegisteredUserDefaultLandingPage, company.PostSigningLandingPage);

                        //    //postSigningPage = string.IsNullOrEmpty(company.PostSigningLandingPage) ? Convert.ToString(ConfigurationManager.AppSettings["RPostPostSigningLandingPage"]) : company.PostSigningLandingPage;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    postSigningPage = Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]);
                }
                string rsignanonymoustoken = string.Empty;

                //recipients.IsReviewed = envelopeSignDocumentSubmitInfo.IsReviewed;
                //if (Request.Cookies["rsignanonymoustoken"] != null && userProfile != null)
                //{
                //    HttpCookie cookie = Request.Cookies.Get("rsignanonymoustoken");
                //    rsignanonymoustoken = cookie.Value;
                //    var userTokenProfile = userTokenRepository.GetUserProfileByToken(rsignanonymoustoken);
                //    if (userTokenProfile != null && userTokenProfile.EmailID.Trim() == currentRecipientEmailId.Trim())
                //        recipients.Order = 2;
                //}
                recipients.EmailAddress = string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.CopyEmail) ? currentRecipientEmailId : envelopeSignDocumentSubmitInfo.CopyEmail;
                if (userProfile != null)
                {
                    recipients.Name = string.IsNullOrEmpty(envelopeSignDocumentSubmitInfo.CopyEmail) ? env.Recipients.FirstOrDefault(r => r.ID == envelopeSignDocumentSubmitInfo.RecipientID).Name : userProfile.FirstName + " " + userProfile.LastName;
                }
                else
                {
                    recipients.Name = string.Empty;
                }
                recipients.DeliveryMode = recipentsdata.DeliveryMode;
                responseMessage.EnvelopeStatus = _envelopeHelperMain.GetEnvelopeStatus(envelopeSignDocumentSubmitInfo.EnvelopeID);
                var envelopeSettingObject = _eSignHelper.GetEnvelopeSettingsDetail(envelopeSignDocumentSubmitInfo.EnvelopeID);
                if (envelopeSettingObject != null)
                    responseMessage.AttachSignedPdfID = envelopeSettingObject.AttachSignedPdf;

                if (isSignDocument)
                {
                    loggerModelNew.Message = String.Format("{0}'s {1}", currentRecipientEmailId, "Documents submitted successfully.");
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.message = "Documents submitted successfully.";
                    responseMessage.success = true;
                    responseMessage.returnUrl = postSigningPage;
                    responseMessage.postSigningLogin = enablePostSigningLoginPopup;
                    responseMessage.data = recipients;
                    responseMessage.isSignedDocumentService = Convert.ToBoolean(_appConfiguration["IsSignedDocumentService"]);
                    responseMessage.EncryptSender = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(Convert.ToString(responseMessage.InfoSenderEmail), Convert.ToString(_appConfiguration["AppKey"])));
                    responseMessage.EncryptEnvelopeId = EncryptDecryptQueryString.Encrypt(envelopeSignDocumentSubmitInfo.EnvelopeID.ToString(), Convert.ToString(_appConfiguration["AppKey"]));
                    responseMessage.EncryptRecipientId = EncryptDecryptQueryString.Encrypt(envelopeSignDocumentSubmitInfo.RecipientID.ToString(), Convert.ToString(_appConfiguration["AppKey"]));
                    responseMessage.EncryptedQueryString = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt("eId=" + envelopeSignDocumentSubmitInfo.EnvelopeID.ToString(), Convert.ToString(_appConfiguration["AppKey"])));
                    return Ok(responseMessage);
                }
                else
                {
                    string errorMessageForUser = !string.IsNullOrEmpty(errorIfFound) ? errorIfFound : "Error occurred while sending Emails";
                    loggerModelNew.Message = String.Format("{0}'s {1}", currentRecipientEmailId, errorMessageForUser);
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.message = errorMessageForUser;
                    responseMessage.success = false;
                    responseMessage.returnUrl = "Info/Index";
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetDownloadFileReview action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "BadRequest";
                responseMessage.message = ex.Message.ToString();
                responseMessage.success = false;
                responseMessage.returnUrl = "Info/Index";
                return BadRequest(responseMessage);
            }

            #region commentedCode
            //eSignHelper esignHelper = new eSignHelper();
            //ApiHelpers apiHelper = new ApiHelpers();
            //try
            //{
            //    using (var dbContext = new eSignEntities())
            //    {
            //        RecipientRepository recipientRepository = new RecipientRepository(dbContext);
            //        EnvelopeRepository envelopeRepository = new EnvelopeRepository(dbContext);
            //        CompanyRepository companyRepository = new CompanyRepository(dbContext);
            //        SettingsRepository settingRepository = new SettingsRepository(dbContext);
            //        EnvelopeHelperMain objEnvelopeHelperMain = new EnvelopeHelperMain();
            //        UnitOfWork unitOfWork = new UnitOfWork(dbContext);

            //        string subject = Convert.ToString(ConfigurationManager.AppSettings["Subject_EnvelopeRejected"]);
            //        var envelopeObject = envelopeRepository.GetEntity(objSignDocParam.ID);
            //        envelopeCode = envelopeObject == null ? "" : envelopeObject.EDisplayCode.ToString();
            //        var senderDetails = recipientRepository.GetSenderDetails(objSignDocParam.ID);
            //        var signerDetails = recipientRepository.GetEntity(objSignDocParam.RecipientID);
            //        Guid signerStatusId = recipientRepository.GetSignerStatusId(objSignDocParam.RecipientID);
            //        emailId = signerDetails != null ? signerDetails.EmailAddress : "";
            //        string message = string.Format("In API's UpdatedEvelopeStatusbySigner method with IP Address : {0}", objSignDocParam.IPAddress.ToString());
            //        loggerModel = new LoggerModel(emailId, currentModule, currentMethod, message, envelopeCode);
            //        rsignlog.RSignLogInfo(loggerModel);

            //        if (envelopeObject == null)
            //        {
            //            responseMessage.StatusCode = HttpStatusCode.NoContent;
            //            responseMessage.StatusMessage = "NoContent";
            //            responseMessage.Message = Convert.ToString(ConfigurationManager.AppSettings["NoContent"].ToString());
            //            responseToClient = Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            //            loggerModel.Message = responseMessage.Message;
            //            rsignlog.RSignLogWarn(loggerModel);
            //            return responseToClient;
            //        }
            //        if (envelopeObject.StatusID == Constants.StatusCode.Envelope.Incomplete_and_Expired)
            //        {
            //            responseMessage.StatusCode = HttpStatusCode.BadRequest;
            //            responseMessage.StatusMessage = "BadRequest";
            //            responseMessage.Message = Convert.ToString(ConfigurationManager.AppSettings["IncompleteAndExpired"].ToString());
            //            responseToClient = Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            //            loggerModel.Message = responseMessage.Message;
            //            rsignlog.RSignLogWarn(loggerModel);
            //            return responseToClient;
            //        }
            //        else if (envelopeObject.StatusID == Constants.StatusCode.Envelope.Terminated)
            //        {
            //            responseMessage.StatusCode = HttpStatusCode.BadRequest;
            //            responseMessage.StatusMessage = "BadRequest";
            //            responseMessage.Message = Convert.ToString(ConfigurationManager.AppSettings["Terminated"].ToString());
            //            responseToClient = Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            //            loggerModel.Message = responseMessage.Message;
            //            rsignlog.RSignLogWarn(loggerModel);
            //            return responseToClient;
            //        }// check signer status
            //        else if (signerStatusId.Equals(Constants.StatusCode.Signer.Delegated))
            //        {
            //            responseMessage.StatusCode = HttpStatusCode.BadRequest;
            //            responseMessage.StatusMessage = "BadRequest";
            //            responseMessage.Message = "This envelope is delegated, and is not available for signing.";
            //            responseToClient = Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            //            loggerModel.Message = responseMessage.Message;
            //            rsignlog.RSignLogWarn(loggerModel);
            //            return responseToClient;
            //        }
            //        else if (signerStatusId.Equals(Constants.StatusCode.Signer.Rejected))
            //        {
            //            responseMessage.StatusCode = HttpStatusCode.BadRequest;
            //            responseMessage.StatusMessage = "BadRequest";
            //            responseMessage.Message = Convert.ToString(ConfigurationManager.AppSettings["Rejected"].ToString());
            //            responseToClient = Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            //            loggerModel.Message = responseMessage.Message;
            //            rsignlog.RSignLogWarn(loggerModel);
            //            return responseToClient;
            //        }
            //        else if (signerStatusId.Equals(Constants.StatusCode.Recipients.Transferred))
            //        {
            //            responseMessage.StatusCode = HttpStatusCode.BadRequest;
            //            responseMessage.StatusMessage = "BadRequest";
            //            responseMessage.Message = "The contract has already been transfer to other signer, in case you have any query please contact to sender of contract";
            //            responseToClient = Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            //            loggerModel.Message = responseMessage.Message;
            //            rsignlog.RSignLogWarn(loggerModel);
            //            return responseToClient;
            //        }
            //        else if (signerStatusId.Equals(Constants.StatusCode.Signer.Signed))
            //        {
            //            responseMessage.StatusCode = HttpStatusCode.BadRequest;
            //            responseMessage.StatusMessage = "BadRequest";
            //            responseMessage.Message = Convert.ToString(ConfigurationManager.AppSettings["SignDocEnvelopeAlreadySigned"].ToString());
            //            responseToClient = Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            //            loggerModel.Message = responseMessage.Message;
            //            rsignlog.RSignLogWarn(loggerModel);
            //            return responseToClient;
            //        }
            //        // added recipient status object
            //        recipientRepository.AddSignerRemark(objSignDocParam.RecipientID, objSignDocParam.SignerStatusDescription, objSignDocParam.IPAddress);
            //        var envelopeStatus = new EnvelopeStatus();
            //        Guid envelopeStatusId = Constants.StatusCode.Envelope.Terminated;
            //        envelopeStatus.CreatedDateTime = DateTime.Now;
            //        envelopeStatus.EnvelopeID = objSignDocParam.ID;
            //        envelopeStatus.ID = Guid.NewGuid();
            //        envelopeStatus.StatusID = envelopeStatusId;
            //        // added new object in envelope status
            //        envelopeRepository.Save(envelopeStatus);
            //        // updated status in envelope table
            //        envelopeRepository.UpdateEnvelopeStatus(objSignDocParam.ID, envelopeStatusId);
            //        // save changes to db
            //        unitOfWork.SaveChanges();
            //        // Update "Envelope.xml" file at temp location. Update "IsEnvelopeCompleted" field.
            //        var dictionary = new Dictionary<EnvelopeNodes, string> { { EnvelopeNodes.IsEnvelopeRejected, "true" } };
            //        esignHelper.UpdateEnvelopeXML(objSignDocParam.ID, dictionary);

            //        APISettings apiSettings = settingRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
            //        var userSettings = objEnvelopeHelperMain.TransformSettingsDictionaryToEntity(apiSettings);
            //        string finalPdfFilePath = string.Empty;
            //        //var companyProfile = companyRepository.GetCompanyProfileByEnvelopeID(envelopeObject.ID);
            //        int isContractToGenerateFromImages = Convert.ToInt32(userSettings.FinalContractOptionID) > 0 ? userSettings.FinalContractOptionID : Constants.FinalContractOptions.Aspose;//(companyProfile != null) ? Convert.ToInt32(companyProfile.FinalContractOptionID) : Constants.FinalContractOptions.Aspose;
            //        try
            //        {
            //            finalPdfFilePath = apiHelper.finalMergePDFApi(envelopeObject, isContractToGenerateFromImages, "", Convert.ToBoolean(envelopeObject.IsSeparateMultipleDocumentsAfterSigningRequired) ? true : false, userSettings.SelectedTimeZone);
            //        }
            //        catch (Exception Ex)
            //        {
            //            objEnvelopeHelperMain.DeleteContractFileInCaseOfError(envelopeObject.ID);
            //            finalPdfFilePath = apiHelper.finalMergePDFApi(envelopeObject, Convert.ToInt32(isContractToGenerateFromImages) != Constants.FinalContractOptions.iText ? Constants.FinalContractOptions.iText : Constants.FinalContractOptions.Aspose, "", Convert.ToBoolean(envelopeObject.IsSeparateMultipleDocumentsAfterSigningRequired) ? true : false, userSettings.SelectedTimeZone);
            //        }

            //        RejectToMail(objSignDocParam.SignerStatusDescription, "Terminated: " + envelopeObject.Subject, finalPdfFilePath, envelopeObject, signerDetails, senderDetails, dbContext);
            //        RejectMailToSender(objSignDocParam.SignerStatusDescription, "Terminated: " + envelopeObject.Subject, finalPdfFilePath, envelopeObject, signerDetails, senderDetails, dbContext);
            //    }
            //    responseMessage.StatusCode = HttpStatusCode.OK;
            //    responseMessage.StatusMessage = "OK";
            //    responseMessage.Message = "The envelope is rejected and email notification is sent to sender.";
            //    loggerModel.Message = responseMessage.Message;
            //    rsignlog.RSignLogInfo(loggerModel);
            //    responseToClient = Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            //    return responseToClient;
            //}
            //catch (Exception ex)
            //{
            //    loggerModel = new LoggerModel(emailId, currentModule, currentMethod, ex.Message, envelopeCode);
            //    rsignlog.RSignLogError(loggerModel, ex);
            //    responseToClient = Request.CreateResponse((HttpStatusCode)422);
            //    responseToClient.Content = new StringContent(ex.Message, Encoding.Unicode);
            //    throw new HttpResponseException(responseToClient);
            //}
            #endregion
        }

        [ProducesResponseType(typeof(ResponseMessageForSecurityCodeModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DecryptSecurityCodeUrl")]
        [HttpPost]
        public IActionResult DecryptSecurityCodeUrl(RequestDecryptSecurityCodeModel securityCodeModel)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "DecryptSecurityCodeUrl", "Process started for Decrypting Security Code Url", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageForSecurityCodeModel responseMessage = new ResponseMessageForSecurityCodeModel();
            string userURL = securityCodeModel.SecurityCodeUrl;
            userURL = HttpUtility.UrlDecode(userURL);
            var CopyMailId = string.Empty;

            try
            {
                if (!userURL.Equals(""))
                {
                    userURL = EncryptDecryptQueryString.Decrypt(userURL, Convert.ToString(_appConfiguration["AppKey"]));
                    if (!userURL.Equals("Invalid length for a Base-64 char array or string.") && !userURL.Equals("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters. ") && !userURL.Equals("Length of the data to decrypt is invalid."))  // V2 Team Prefill Change
                    {
                        string[] arrayURL = userURL.Split('&');

                        if (arrayURL.Length == 1)
                        {
                            string[] arrayID = arrayURL[0].Split('=');
                            CopyMailId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            CopyMailId = !string.IsNullOrEmpty(CopyMailId) ? HttpUtility.UrlDecode(CopyMailId) : string.Empty;
                        }
                    }
                }
                responseMessage.CopyMailId = CopyMailId;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(InfoResultResonse), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("ValidateRecipientBySecurityCode/{securityCode}/{CopyEmail}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateRecipientBySecurityCode(string securityCode, string CopyEmail)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "ValidateRecipientBySecurityCode", "Process started for Validate Recipient By Security Code using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            InfoResultResonse responseMessage = new InfoResultResonse();
            CopyEmail = CopyEmail.ToUpper() == "FALSE" ? string.Empty : CopyEmail;
            try
            {
                Envelope envelope = null;
                var recipients = _recipientRepository.GetRecipientByCode(securityCode);
                if (recipients != null)
                {
                    envelope = _genericRepository.GetEnvelopeById(recipients.EnvelopeID);
                }

                if (recipients == null || envelope == null)
                {
                    #region Envelope Arichived or not                   
                    ArichiveEnvelopesInfo envelopesInfo = _genericRepository.GetArchivedEnvelope(string.Empty, Guid.Empty, Guid.Empty, securityCode);
                    if (envelopesInfo != null)
                    {
                        if (envelopesInfo.IsEnvelopePurging == true && envelopesInfo.envelope == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.StatusMessage = "OK";
                            responseMessage.message = envelopesInfo.ArchivedEnvelopeMessage;
                            responseMessage.IsEnvelopePurging = true;
                            loggerModelNew.Message = responseMessage.message;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            return Ok(responseMessage);
                        }
                        else if (envelopesInfo.envelope != null)
                        {
                            envelope = envelopesInfo.envelope;
                        }
                    }
                    else
                    {
                        loggerModelNew.Message = "Security Code is invalid";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.message = "Security Code is invalid";
                        responseMessage.success = false;
                        responseMessage.returnUrl = "Info/Index";
                        return BadRequest(responseMessage);
                    }
                    #endregion Envelope Arichived or not     
                }

                var sender = envelope.Recipients.FirstOrDefault(a => a.RecipientTypeID == Constants.RecipientType.Sender);
                var senderEmail = sender != null ? sender.EmailAddress : string.Empty;
                var signer = envelope.Recipients.FirstOrDefault(a => a.RecipientTypeID == Constants.RecipientType.Signer);
                bool isNewUrlApplicable = _envelopeHelperMain.IsGenerateNewSigningUrl(senderEmail);

                string signingURL = _genericRepository.GetNewSigningURL(envelope.TemplateKey != null ? Convert.ToString(envelope.TemplateKey) : string.Empty, envelope.ID.ToString(), recipients != null ? recipients.ID.ToString() : signer.ID.ToString(),
                    recipients != null ? recipients.EmailAddress : signer.EmailAddress, CopyEmail, senderEmail, envelope.IsSignerIdentitiy, recipients != null ? recipients.RecipientTypeID : signer.RecipientTypeID, isNewUrlApplicable);

                // string signingURL = _genericRepository.GetSigningURL(envelope.TemplateKey != null ? Convert.ToString(envelope.TemplateKey) : string.Empty, envelope.ID.ToString(), recipients.ID.ToString(), recipients.EmailAddress, CopyEmail);

                loggerModelNew.Message = String.Format("{0}'s {1}", recipients != null ? recipients.EmailAddress : signer.EmailAddress, "Recipient retrieved successfully by Recipient code.");
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.message = "Recipient retrieved successfully.";
                responseMessage.success = true;
                responseMessage.IsEnvelopePurging = false;
                responseMessage.returnUrl = signingURL;
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForResponseSigningUrlModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DecryptStaticLinkUrl")]
        [HttpPost]
        public IActionResult DecryptStaticLinkUrl(RequestSigningUrlModel signingUrlModel)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "DecryptStaticLinkUrl", "Process started for Decrypting Signing Url", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageForResponseSigningUrlModel responseMessage = new ResponseMessageForResponseSigningUrlModel();
            ResponseSigningUrlModel responseSigningUrlModel = new ResponseSigningUrlModel();
            string userURL = signingUrlModel.SigningUrl;
            userURL = HttpUtility.UrlDecode(userURL);
            var strtemplateID = "";
            var strtemplateKey = "";

            try
            {
                if (!userURL.Equals(""))
                {
                    userURL = EncryptDecryptQueryString.Decrypt(userURL, Convert.ToString(_appConfiguration["AppKey"]));
                    if (!userURL.Equals("Invalid length for a Base-64 char array or string.") && !userURL.Equals("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters. ") && !userURL.Equals("Length of the data to decrypt is invalid."))  // V2 Team Prefill Change
                    {
                        string[] arrayURL = userURL.Split('&');

                        if (arrayURL.Length == 2)
                        {
                            string[] arraystrtemplateID = arrayURL[0].Split('=');
                            strtemplateID = arraystrtemplateID.Length == 2 ? arraystrtemplateID[1].Trim() : string.Empty;
                            string[] arraystrtemplateKey = arrayURL[1].Split('=');
                            strtemplateKey = arraystrtemplateKey.Length == 2 ? arraystrtemplateKey[1].Trim() : string.Empty;
                        }
                        else
                        {
                            string[] arrayID = arrayURL[0].Split('=');
                            strtemplateID = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        }

                        responseSigningUrlModel.EnvelopeID = strtemplateID;
                        responseSigningUrlModel.TemplateKey = strtemplateKey;

                        TranslationsModel translationsModel = new TranslationsModel();
                        translationsModel.EnvelopeId = strtemplateID;
                        translationsModel.RecipientId = "";
                        translationsModel.CultureInfo = "";
                        LanguageKeyTranslationsModel responseTranslations = _genericRepository.GetLanguageKeyTranslations(translationsModel);
                        responseMessage.LanguageTranslationsModel = responseTranslations;

                        responseMessage.ResponseSigningUrlModel = responseSigningUrlModel;
                        responseMessage.StatusCode = HttpStatusCode.OK;
                        responseMessage.StatusMessage = "OK";
                        loggerModelNew.Message = "Process completed for Decrypting Signing Url";
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return Ok(responseMessage);
                    }
                    else
                    {
                        responseMessage.ResponseSigningUrlModel = responseSigningUrlModel;
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "The URL of the template is incorrect.";
                        return BadRequest(responseMessage);
                    }
                }
                else
                {
                    responseMessage.ResponseSigningUrlModel = responseSigningUrlModel;
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "The URL of the template is incorrect.";
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForInitalizeSignDocument), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("InitializeStaticTemplate")]
        [HttpPost]
        public async Task<IActionResult> InitializeStaticTemplate(InitializeSignDocumentAPI objSignParam)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "InitializeStaticTemplate", "Process started for Initialize Static Template", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            string envelopeCode = string.Empty;
            string emailId = string.Empty;
            ResponseMessageForInitalizeSignDocument responseMessage = new ResponseMessageForInitalizeSignDocument();
            bool IsUnitTestApp = false; //TODO : send this parameter in objSignParam//Convert.ToBoolean(SessionHelper.Get(SessionKey.IsUnitTestApp));
            string Message = string.Empty;

            Guid templateID = new Guid(objSignParam.EnvelopeId);
            Guid recipientId = Guid.Empty;
            if (!string.IsNullOrEmpty(objSignParam.RecipientId))
            {
                recipientId = new Guid(objSignParam.RecipientId);
            }

            string currentRecipientEmailId = string.Empty;
            string senderEmail = string.Empty;
            string currentenvelopeID = string.Empty;
            EnvelopeDetails envelopeDetails = new EnvelopeDetails();
            DashBoard dashBoard = new DashBoard();
            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            APISettings aPISettings = new APISettings();
            Guid signerStatusId = Guid.Empty;
            EnvelopeInfo controlsInfo = new EnvelopeInfo();
            string folderFileSize = "0";
            string finalDocName = string.Empty;
            Template envelopeObject = new Template();
            bool IsTemplateDatedBeforePortraitLandscapeFeature = false;
            List<DocumentDetails> documentDetails = new List<DocumentDetails>();
            List<RolsInfo> rolsInfos = new List<RolsInfo>();
            try
            {
                envelopeObject = _genericRepository.GetTemplateEntity(templateID);
                if (envelopeObject == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"].ToString());
                    responseMessage.EnvelopeInfo = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                if (envelopeObject.IsActive == false)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "The sender has deleted the envelope. For further information, please contact the sender.";
                    responseMessage.EnvelopeInfo = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                var sender = _userRepository.GetUserProfileByUserID(envelopeObject.UserID);
                if (sender != null)
                    responseMessage.InfoSenderEmail = sender.EmailID;

                if (envelopeObject.IsStatic == false || envelopeObject.IsStaticLinkDisabled == true)
                {
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "staticLinkDocExpired");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.ErrorAction = "StaticLinkDisabled";
                    responseMessage.Message = Message;
                    responseMessage.EnvelopeInfo = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                if (sender != null)
                {
                    AdminGeneralAndSystemSettings companysettings = new AdminGeneralAndSystemSettings();
                    Guid CompanyID = (Guid)sender.CompanyID;
                    var getSetting = _settingsRepository.GetEntityByParam(CompanyID, string.Empty, Constants.String.SettingsType.Company);
                    companysettings = _eSignHelper.TransformSettingsDictionaryToEntity(getSetting);
                    responseMessage.DisableDeclineOption = companysettings.DisableDeclineOption;
                    responseMessage.DisableFinishLaterOption = companysettings.DisableFinishLaterOption;
                    responseMessage.DisableChangeSigner = companysettings.DisableChangeSigner;
                    responseMessage.SendMessageCodetoAvailableEmailorMobile = companysettings.SendMessageCodetoAvailableEmailorMobile;
                }

                aPISettings = _settingsRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
                adminGeneralAndSystemSettings = _eSignHelper.TransformSettingsDictionaryToEntity(aPISettings);

                if (envelopeObject.IsStatic == true)
                {
                    if (envelopeObject.StaticLinkExpiryDate != null)
                    {
                        Guid dateFormatId = adminGeneralAndSystemSettings.DateFormatID;
                        string slExpiryDateFormat = "";
                        if ((adminGeneralAndSystemSettings.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash))
                        {
                            slExpiryDateFormat = "MM/dd/yyyy";
                        }
                        else if ((adminGeneralAndSystemSettings.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan))
                        {
                            slExpiryDateFormat = "MM-dd-yyyy";
                        }
                        else if ((adminGeneralAndSystemSettings.DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots))
                        {
                            slExpiryDateFormat = "MM.dd.yyyy";
                        }
                        else if ((adminGeneralAndSystemSettings.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_slash))
                        {
                            slExpiryDateFormat = "dd/MM/yyyy";
                        }
                        else if ((adminGeneralAndSystemSettings.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_colan))
                        {
                            slExpiryDateFormat = "dd-MM-yyyy";
                        }
                        else if ((adminGeneralAndSystemSettings.DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_dots))
                        {
                            slExpiryDateFormat = "dd.MM.yyyy";
                        }
                        else if ((adminGeneralAndSystemSettings.DateFormatID == Constants.DateFormat.Europe_yyyy_mm_dd_dots))
                        {
                            slExpiryDateFormat = "yyyy.MM.dd.";
                        }
                        else if ((adminGeneralAndSystemSettings.DateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan))
                        {
                            slExpiryDateFormat = "dd-MMM-yyyy";
                        }
                        else
                            slExpiryDateFormat = "MM/dd/yyyy";

                        if ((envelopeObject.StaticLinkExpiryDate.HasValue && DateTime.Now.Date > envelopeObject.StaticLinkExpiryDate.Value.Date))
                        {
                            Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "staticLinkDocExpired");
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.ErrorAction = "StaticLinkDocExpired";
                            responseMessage.Message = Message;
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseMessage.EnvelopeInfo = null;
                            return BadRequest(responseMessage);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(objSignParam.templateKey))
                {
                    if (envelopeObject.TemplateKey != new Guid(objSignParam.templateKey))
                    {
                        Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "staticLinkDocExpired");
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = Message;
                        responseMessage.EnvelopeInfo = null;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }

                if (envelopeObject.TemplateKey != null && objSignParam.templateKey == "")
                {
                    Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelopeObject.UserID, "staticLinkDocExpired");
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Message;
                    responseMessage.EnvelopeInfo = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                /*RS-428: URGENT - All RSign controls shifted down and right - solve with highest priority*/
                //if (envelopeObject.ID != null)
                //{
                //    var dateTimeBeforePortraitLandscape = Convert.ToDateTime(_appConfiguration["dateTimeBeforePortraitLandscape"]);
                //    if (envelopeObject.CreatedDateTime <= dateTimeBeforePortraitLandscape)
                //        IsTemplateDatedBeforePortraitLandscapeFeature = true;
                //}

                // Creating object graph to load the child entity.                    
                var documents = envelopeObject.TemplateDocuments.ToList();
                int order = 1;
                Guid CurrentRoleId = Guid.Empty;
                var rolesData = envelopeObject.TemplateRoles.OrderBy(d => d.CreatedDateTime).ToList();
                bool IsRequiredRole = false;

                foreach (var row in rolesData)
                {
                    RolsInfo rolsInfo = new RolsInfo();
                    rolsInfo.RecipientId = row.ID;
                    rolsInfo.order = envelopeObject.IsSequenceCheck == true ? Convert.ToInt32(row.Order) : order;
                    rolsInfo.isRequired = false;
                    rolsInfo.RoleName = row.Name;
                    rolsInfo.CultureInfo = row.CultureInfo;
                    rolsInfo.DeliveryMode = Convert.ToString(row.DeliveryMode);
                    rolsInfo.DialCode = row.DialCode != null ? row.DialCode : "";
                    rolsInfo.CountryCode = row.CountryCode != null ? row.CountryCode : "";
                  //  rolsInfo.ReminderType = row.ReminderType;

                    foreach (var document in documents)
                    {
                        if (document.TemplateDocumentContents != null)
                        {
                            IsRequiredRole = document.TemplateDocumentContents.Any(d => row.ID == d.RecipientID && Convert.ToString(d.ControlID).ToUpper() == "E294C207-13FD-4508-95FC-90C5D9C555FA" && d.Required);
                            if (IsRequiredRole)
                            {
                                rolsInfo.isRequired = true;
                                break;
                            }
                        }
                    }

                    rolsInfos.Add(rolsInfo);
                    order++;
                }
                //var TempDocuments = envelopeObject.TemplateDocuments.Where(d => d.ActionType == Constants.ActionTypes.Review).ToList();
                //if (TempDocuments.Count > 0)
                //{
                //    foreach (var doc in TempDocuments)
                //    {
                //        DocumentDetails document = new DocumentDetails();
                //        document.ID = doc.ID;
                //        //document.EnvelopeID = doc.EnvelopeID;
                //        document.DocumentName = doc.DocumentName;
                //        document.DocumentSource = doc.DocumentSource;
                //        document.ActionType = doc.ActionType;
                //        documentDetails.Add(document);
                //    }
                //}

                //controlsInfo = _envelopeHelperMain.GetSignerLandingStaticTemplateInfo(templateID, envelopeObject);
                // aPISettings = _settingsRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
                // adminGeneralAndSystemSettings = _eSignHelper.TransformSettingsDictionaryToEntity(aPISettings);
                //controlsInfo.SignatureCaptureHanddrawn = adminGeneralAndSystemSettings.SignatureCaptureHanddrawn;
                //controlsInfo.UploadSignature = adminGeneralAndSystemSettings.UploadSignature;
                //controlsInfo.ElectronicSignIndication = (byte)adminGeneralAndSystemSettings.ElectronicSignIndicationSelectedID;
                //controlsInfo.SignatureCaptureType = adminGeneralAndSystemSettings.SignatureCaptureType;
                //controlsInfo = _envelopeHelperMain.GetStaticDocumentControls(controlsInfo, templateID);
                //controlsInfo.IsSignerAttachFileReq = envelopeObject.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest ? true : false;
                //controlsInfo.IsSignerAttachFileReqNew = envelopeObject.IsSignerAttachFileReq != null ? envelopeObject.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                // bool isSignerattachmentProcess = false;
                // if (Convert.ToInt32(envelopeObject.IsSignerAttachFileReq) > 0)
                // {
                //     isSignerattachmentProcess = envelopeObject.IsAdditionalAttmReq != null ? Convert.ToBoolean(envelopeObject.IsAdditionalAttmReq) : false;
                // }
                //  controlsInfo.IsAdditionalAttmReq = isSignerattachmentProcess;
                //  controlsInfo.DateFormatID = envelopeObject.DateFormatID;

                controlsInfo.CultureInfo = Convert.ToString(envelopeObject.CultureInfo);
                controlsInfo.PasswordReqdtoSign = envelopeObject.PasswordReqdtoSign;
                controlsInfo.GlobalEnvelopeID = templateID;
                if (sender != null)
                    controlsInfo.SenderEmail = sender.EmailID;

                // controlsInfo.IsSignerAttachFileReq = (envelopeObject.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                // controlsInfo.IsSignerAttachFileReqNew = envelopeObject.IsSignerAttachFileReq != null ? envelopeObject.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                // controlsInfo.IsFinalCertificateReq = envelopeObject.IsFinalCertificateReq != true;
                // controlsInfo.Controls = _masterDataRepository.GetControlID().Where(c => c.ID != Constants.Control.NewInitials).ToDictionary(x => x.ControlName, x => x.ControlName);
                //  controlsInfo.TimeZoneSettingOptionValue = adminGeneralAndSystemSettings.SelectedTimeZone.ToString();
                // controlsInfo.IsTemplateDatedBeforePortraitLandscapeFeature = IsTemplateDatedBeforePortraitLandscapeFeature;
                //  var disclaimer = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.Disclaimer);
                // var isDisclaimerInCertificate = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.IsDisclaimerInCertificate);
                //  controlsInfo.IsDisclaimerEnabled = Convert.ToBoolean(isDisclaimerInCertificate.OptionValue);
                // controlsInfo.Disclaimer = Convert.ToBoolean(isDisclaimerInCertificate.OptionValue) ? Convert.ToString(disclaimer.OptionValue) : null;
                // controlsInfo.IsSendConfirmationEmail = Convert.ToBoolean(envelopeObject.SendConfirmationEmail); //envelopeObject.

                controlsInfo.IsStatic = envelopeObject.IsStatic;
                responseMessage.AllowMultipleSigner = Convert.ToBoolean(envelopeObject.AllowMultiSigner);
                responseMessage.IsPasswordMailToSigner = Convert.ToBoolean(envelopeObject.IsPasswordMailToSigner);
                responseMessage.DialCodeDropdownList = _envelopeHelperMain.LoadDialingCountryCodes();
                responseMessage.EnableMessageToMobile = envelopeObject.EnableMessageToMobile;
                responseMessage.ReVerifySignerStaticTemplate = Convert.ToBoolean(envelopeObject.ReVerifySignerStaticTemplate);
                responseMessage.StaticLinkExpiryDate = Convert.ToDateTime(envelopeObject.StaticLinkExpiryDate);

                // responseMessage.EnableClickToSign = adminGeneralAndSystemSettings.EnableClickToSign;
                // responseMessage.EnableAutoFillTextControls = adminGeneralAndSystemSettings.EnableAutoFillTextControls;
                // responseMessage.IsDefaultSignatureForStaticTemplate = envelopeObject.IsDefaultSignatureForStaticTemplate;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                Message = "The signer details reterived successully.";
                loggerModelNew.Message = "Process completed for Initialize Static Template using API and " + responseMessage.Message;
                responseMessage.EnvelopeInfo = controlsInfo;
                responseMessage.TemplateRolesInfo = rolsInfos;
                // responseMessage.documentDetails = documentDetails;               
                //responseMessage.IsSendConfirmationEmail = Convert.ToBoolean(envelopeObject.SendConfirmationEmail); //envelopeObject.

                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller InitializeStaticTemplate action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseAllowFirstSignerModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("AllowFirstSigner")]
        [HttpPost]
        public async Task<IActionResult> AllowFirstSigner(FirsrtRolsInfo firsrtRolsInfo)
        {
            ResponseAllowFirstSignerModel responseAllowFirstSignerModel = new ResponseAllowFirstSignerModel();
            string strURL = string.Empty, strURLWithData = string.Empty;

            try
            {
                if (firsrtRolsInfo.IsStaticPwd)
                {
                    var isPasswordMailToSigner = false;
                    var template = _genericRepository.GetTemplateDetails(new Guid(Convert.ToString(firsrtRolsInfo.EnvelopeId)));
                    isPasswordMailToSigner = template.IsPasswordMailToSigner;
                    if (Convert.ToBoolean(template.EnableMessageToMobile))
                    {
                        isPasswordMailToSigner = true;
                    }

                    if (firsrtRolsInfo.IsPasswordMailToSigner || isPasswordMailToSigner)
                    {
                        UserVerificationModel userVerificationModel = new UserVerificationModel();
                        userVerificationModel.envelopeID = firsrtRolsInfo.EnvelopeId.ToString();
                        userVerificationModel.EmailId = firsrtRolsInfo.FirstSignerEmail;
                        userVerificationModel.CultureInfo = firsrtRolsInfo.CultureInfo;
                        userVerificationModel.DeliveryMode = firsrtRolsInfo.DeliveryMode;
                        userVerificationModel.DialCode = firsrtRolsInfo.DialCode;
                        userVerificationModel.MobileNumber = firsrtRolsInfo.MobileNumber;
                        userVerificationModel.CountryCode = firsrtRolsInfo.CountryCode;

                        var errorResponseModel = await _templateRepository.ReSendPasswordEmail(userVerificationModel);
                        if (errorResponseModel.Status)
                        {
                            responseAllowFirstSignerModel.CopyMail = "";
                            responseAllowFirstSignerModel.strURL = "static-password-window";
                            string encryptedEncodedText = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("EnvelopeId={0}&RecipientId={1}&SenderEmail={2}&SignerEmail={3}&IsStatic={4}&IsFirstSigner={5}&IsPasswordMailToSigner={6}",
                                firsrtRolsInfo.EnvelopeId, firsrtRolsInfo.RecipientId, firsrtRolsInfo.SenderEmail, firsrtRolsInfo.FirstSignerEmail, true, true, firsrtRolsInfo.IsPasswordMailToSigner), Convert.ToString(_appConfiguration["QueryStringKey"])));
                            responseAllowFirstSignerModel.strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/static-password-window?" + encryptedEncodedText;

                            EnvelopePasswordModal ePasswordModal = new EnvelopePasswordModal(firsrtRolsInfo.EnvelopeId ?? Guid.Empty, firsrtRolsInfo.RecipientId ?? Guid.Empty, false, "", firsrtRolsInfo.FirstSignerEmail, firsrtRolsInfo.SenderEmail,
                                 true, true, firsrtRolsInfo.IsPasswordMailToSigner, firsrtRolsInfo.DeliveryMode, firsrtRolsInfo.DialCode, firsrtRolsInfo.MobileNumber, firsrtRolsInfo.CountryCode);
                            responseAllowFirstSignerModel.EPasswordModal = ePasswordModal;

                            responseAllowFirstSignerModel.StatusCode = HttpStatusCode.OK;
                            responseAllowFirstSignerModel.StatusMessage = "OK";
                            responseAllowFirstSignerModel.Message = "Success";
                            return Ok(responseAllowFirstSignerModel);
                        }
                        else
                        {
                            responseAllowFirstSignerModel.StatusCode = HttpStatusCode.BadRequest;
                            responseAllowFirstSignerModel.StatusMessage = "Bad Request";
                            responseAllowFirstSignerModel.Message = "Error";
                            return BadRequest(responseAllowFirstSignerModel);
                        }
                    }
                    else
                    {
                        responseAllowFirstSignerModel.CopyMail = "";
                        responseAllowFirstSignerModel.strURL = "static-password-window";
                        string encryptedEncodedText = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("EnvelopeId={0}&RecipientId={1}&SenderEmail={2}&SignerEmail={3}&IsStatic={4}&IsFirstSigner={5}&IsPasswordMailToSigner={6}",
                            firsrtRolsInfo.EnvelopeId, firsrtRolsInfo.RecipientId, firsrtRolsInfo.SenderEmail, firsrtRolsInfo.FirstSignerEmail, true, true, firsrtRolsInfo.IsPasswordMailToSigner), Convert.ToString(_appConfiguration["QueryStringKey"])));
                        responseAllowFirstSignerModel.strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/static-password-window?" + encryptedEncodedText;
                        EnvelopePasswordModal ePasswordModal = new EnvelopePasswordModal(firsrtRolsInfo.EnvelopeId ?? Guid.Empty, firsrtRolsInfo.RecipientId ?? Guid.Empty, false, "", firsrtRolsInfo.FirstSignerEmail, firsrtRolsInfo.SenderEmail,
                             true, true, firsrtRolsInfo.IsPasswordMailToSigner, firsrtRolsInfo.DeliveryMode, firsrtRolsInfo.DialCode, firsrtRolsInfo.MobileNumber, firsrtRolsInfo.CountryCode);
                        responseAllowFirstSignerModel.EPasswordModal = ePasswordModal;
                        responseAllowFirstSignerModel.StatusCode = HttpStatusCode.OK;
                        responseAllowFirstSignerModel.StatusMessage = "OK";
                        responseAllowFirstSignerModel.Message = "Success";
                        return Ok(responseAllowFirstSignerModel);
                    }
                }
                else
                {
                    responseAllowFirstSignerModel.CopyMail = "";
                    responseAllowFirstSignerModel.strURL = "multiple-signer";
                    string encryptedEncodedText = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("EnvelopeId={0}&RecipientId={1}&FirstSignerEmail={2}", firsrtRolsInfo.EnvelopeId,
                        firsrtRolsInfo.RecipientId, firsrtRolsInfo.FirstSignerEmail), Convert.ToString(_appConfiguration["QueryStringKey"])));
                    responseAllowFirstSignerModel.strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/multiple-signer?" + encryptedEncodedText;
                    responseAllowFirstSignerModel.StatusCode = HttpStatusCode.OK;
                    responseAllowFirstSignerModel.StatusMessage = "OK";
                    responseAllowFirstSignerModel.Message = "Success";
                    return Ok(responseAllowFirstSignerModel);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller AllowFirstSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseAllowFirstSignerModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("AllowStaticSigner")]
        [HttpPost]
        public IActionResult AllowStaticSigner(InitializeSignDocumentAPI objSignParam)
        {
            ResponseAllowFirstSignerModel responseAllowFirstSignerModel = new ResponseAllowFirstSignerModel();
            string strURL = string.Empty, strURLWithData = string.Empty;

            try
            {
                if (objSignParam.IsStaticPwd)
                {
                    responseAllowFirstSignerModel.CopyMail = "";
                    responseAllowFirstSignerModel.strURL = "static-password-window";
                    string encryptedEncodedText = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("EnvelopeId={0}&RecipientId={1}&SenderEmail={2}&SignerEmail={3}&IsStatic={4}&IsFirstSigner={5}&IsPasswordMailToSigner={6}",
                        objSignParam.EnvelopeId, objSignParam.RecipientId, objSignParam.EmailId, objSignParam.CopyEmailId, true, false, objSignParam.IsPasswordMailToSigner), Convert.ToString(_appConfiguration["QueryStringKey"])));
                    responseAllowFirstSignerModel.strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/static-password-window?" + encryptedEncodedText;

                    EnvelopePasswordModal ePasswordModal = new EnvelopePasswordModal(new Guid(objSignParam.EnvelopeId), new Guid(objSignParam.RecipientId), false, "", objSignParam.CopyEmailId, objSignParam.EmailId,
                                true, false, objSignParam.IsPasswordMailToSigner);
                    responseAllowFirstSignerModel.EPasswordModal = ePasswordModal;

                    responseAllowFirstSignerModel.StatusCode = HttpStatusCode.OK;
                    responseAllowFirstSignerModel.StatusMessage = "OK";
                    responseAllowFirstSignerModel.Message = "Success";
                    return Ok(responseAllowFirstSignerModel);

                }
                else
                {
                    responseAllowFirstSignerModel.CopyMail = "";
                    responseAllowFirstSignerModel.strURL = "signer-landing-static-template";
                    string encryptedEncodedText = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("EnvelopeId={0}&RecipientId={1}&FirstSignerEmail={2}", objSignParam.EnvelopeId, objSignParam.RecipientId, objSignParam.CopyEmailId), Convert.ToString(_appConfiguration["QueryStringKey"])));
                    responseAllowFirstSignerModel.strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/signer-landing-static-template?" + encryptedEncodedText; ;

                    responseAllowFirstSignerModel.StatusCode = HttpStatusCode.OK;
                    responseAllowFirstSignerModel.StatusMessage = "OK";
                    responseAllowFirstSignerModel.Message = "Success";
                    return Ok(responseAllowFirstSignerModel);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller AllowStaticSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseAllowFirstSignerModel), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("ReSendPasswordEmail")]
        [HttpPost]
        public async Task<IActionResult> ReSendPasswordEmail(UserVerificationModel userVerificationModel)
        {
            ResponseAllowFirstSignerModel responseAllowFirstSignerModel = new ResponseAllowFirstSignerModel();
            try
            {
                var errorResponseModel = await _templateRepository.ReSendPasswordEmail(userVerificationModel);
                if (errorResponseModel.Status)
                {
                    responseAllowFirstSignerModel.StatusCode = HttpStatusCode.OK;
                    responseAllowFirstSignerModel.StatusMessage = "OK";
                    responseAllowFirstSignerModel.Message = "Email sent successfully";
                    return Ok(responseAllowFirstSignerModel);
                }
                else
                {
                    responseAllowFirstSignerModel.StatusCode = HttpStatusCode.BadRequest;
                    responseAllowFirstSignerModel.StatusMessage = "Bad Request";
                    responseAllowFirstSignerModel.Message = "Error";
                    return BadRequest(responseAllowFirstSignerModel);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller ReSendPasswordEmail action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(EnvelopeAdditionalUpload), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetEnvelopeAdditionalUploadInfo/{templateID}/{envelopeId}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetEnvelopeAdditionalUploadInfo(Guid templateID, Guid envelopeID)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetEnvelopeAdditionalUploadInfo", "Initiate the process for Get Envelope Additional Data Upload by envelopeId using API.", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            EnvelopeAdditionalUpload recipientResp = new EnvelopeAdditionalUpload();
            try
            {
                if ((templateID == null || envelopeID == null))
                {
                    ResponseMessageFile responseMessage = new ResponseMessageFile();
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "EnvelopeID  or TemplateID not found";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                var additionalUploadInfoList = _envelopeRepository.GetTemplateAdditionalDocument(templateID);
                _envelopeRepository.SaveEnvelopeAdditionalAttachment(additionalUploadInfoList, envelopeID);
                List<EnvelopeAdditionalUploadInfoDetails> envelopeAdditionalUploadInfoDetails = new List<EnvelopeAdditionalUploadInfoDetails>();
                envelopeAdditionalUploadInfoDetails = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByEnvelopebyRecipeint(envelopeID, Guid.Empty);

                recipientResp.StatusCode = HttpStatusCode.OK;
                recipientResp.StatusMessage = "OK";
                recipientResp.Message = "Envelope finished successfully.";
                recipientResp.EnvelopeAdditionalUploadInfoList = envelopeAdditionalUploadInfoDetails;
                recipientResp.MaxUploadID = _envelopeRepository.GetMaxUploadsID();
                loggerModelNew.Message = "Process completed for Get Envelope Additional Data Upload by envelopeId using API and " + recipientResp.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(recipientResp);

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetEnvelopeAdditionalUploadInfo action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForSignMultiTemplate), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetTemplateDetailsForSignMultiTemplate/{envelopeId}/{recipientId}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetTemplateDetailsForSignMultiTemplate(string envelopeId, string recipientId)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetTemplateDetailsForSignMultiTemplate", "Process started for Get list of template to sign the multiple template independently using API.", envelopeId, "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                ResponseMessageForSignMultiTemplate responseMessage = new ResponseMessageForSignMultiTemplate();
                SignMultiTemplate signMultiTemplate = new SignMultiTemplate();

                Guid envelopeID = new Guid(envelopeId);
                Guid recipientID = new Guid(recipientId);
                var envelopeObject = _genericRepository.GetEntity(envelopeID);
                string envelopeFolderUNCPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeID, string.Empty);
                if (envelopeObject == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = Convert.ToString(_appConfiguration["EnvelopeIdMissing"]);
                    responseMessage.SignMultipleTemplate = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                signMultiTemplate.IsSignerAttachFileReq = (envelopeObject.IsSignerAttachFileReq == Constants.SignerAttachmentOptions.EnableAttachmentRequest) ? true : false;
                signMultiTemplate.IsSignerAttachFileReqNew = envelopeObject.IsSignerAttachFileReq != null ? envelopeObject.IsSignerAttachFileReq.Value : Constants.SignerAttachmentOptions.None;
                signMultiTemplate.EnvelopeID = envelopeObject.ID;
                signMultiTemplate.CurrentRecipientID = recipientID;
                signMultiTemplate.Subject = envelopeObject.Subject;
                signMultiTemplate.Message = envelopeObject.Message;
                signMultiTemplate.EnvelopeHashID = EncryptDecryptQueryString.Encrypt(Convert.ToString(envelopeObject.ID), Convert.ToString(_appConfiguration["QueryStringKey"]));
                signMultiTemplate.SignerDocs = _envelopeHelperMain.GetSignerDocFromDirectory(envelopeID, recipientID, envelopeFolderUNCPath);
                signMultiTemplate.CurrentRecipientEmail = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientID).EmailAddress;
                var sender = envelopeObject.Recipients.FirstOrDefault(a => a.RecipientTypeID == Constants.RecipientType.Sender);
                if (sender != null && !string.IsNullOrEmpty(sender.EmailAddress))
                    signMultiTemplate.SenderEmailAddress = sender.EmailAddress;

                var templateDetails = _envelopeRepository.GetSigningInbox(envelopeID);
                string timeZoneSetting = string.Empty;
                var settingsDetails = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.TimeZone);
                if (settingsDetails != null)
                {
                    foreach (var item in templateDetails)
                    {
                        item.UpdatedStatusDateTimezoneStr = _eSignHelper.GetLocalTime(item.UpdatedStatusDate, settingsDetails.OptionValue, item.DateFormatID);
                    }
                }

                var settingsDownloadFinalContractDetails = _settingsRepository.GetEntityForByKeyConfig(envelopeObject.UserID, Constants.SettingsKeyConfig.IsAllowSignerstoDownloadFinalContract);
                if (settingsDownloadFinalContractDetails != null)
                    responseMessage.IsAllowSignerstoDownloadFinalContract = settingsDownloadFinalContractDetails.OptionValue;

                var currentRecipientList = templateDetails.Where(a => a.RecipientEmail == signMultiTemplate.CurrentRecipientEmail).ToList();
                if (currentRecipientList.Count == 0)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "Envelope is already delegated";
                    responseMessage.SignMultipleTemplate = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                signMultiTemplate.IsSequenceCheck = false;
                int? maxOrder = templateDetails.Max(p => p.Order);
                if (Convert.ToInt32(maxOrder) > 1)
                {
                    signMultiTemplate.IsSequenceCheck = true;
                    var nextOrderRecipientList = templateDetails.Where(r => r.StatusID != Constants.StatusCode.Signer.Signed).ToList();
                    if (nextOrderRecipientList.Count > 0)
                        signMultiTemplate.nextOrder = nextOrderRecipientList.Min(p => p.Order);
                    else
                        signMultiTemplate.IsSequenceCheck = false;
                }

                signMultiTemplate.RecipientIdsByEmail = new List<string>();
                signMultiTemplate.RecipientIdsByEmail.AddRange(currentRecipientList.Select(a => a.RecipientID.ToString()));
                if (currentRecipientList.Where(a => a.IsSigned == true).Count() == currentRecipientList.Count)
                    signMultiTemplate.IsAllSigned = true;
                if (currentRecipientList.Where(a => a.IsFinished == true).Count() == currentRecipientList.Count)
                    signMultiTemplate.IsFinished = true;

                signMultiTemplate.CultureInfo = envelopeObject.CultureInfo;
                //Attachment request details 
                var envelopeAdditionalUploadInfoDetailsList = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByEnvelope(envelopeObject.ID);
                responseMessage.MaxUploadID = _envelopeRepository.GetMaxUploadsID();
                loggerModelNew.Message = "Template details for sign multiple template independently retrieved successfully.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                signMultiTemplate.SignMultipleTemplateCollection = templateDetails;
                signMultiTemplate.EnvelopeAdditionalUploadInfoList = envelopeAdditionalUploadInfoDetailsList;
                signMultiTemplate.EnableMessageToMobile = envelopeObject.EnableMessageToMobile;
                signMultiTemplate.DialCode = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientID).DialCode;
                signMultiTemplate.MobileNumber = envelopeObject.Recipients.FirstOrDefault(r => r.ID == recipientID).Mobile;
                responseMessage.SignMultipleTemplate = signMultiTemplate;
                responseMessage.SignMultipleTemplate.EnvelopeID = envelopeID;
                loggerModelNew.Message = "Process completed for Get list of template to sign the multiple template independently using API and " + responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetTemplateDetailsForSignMultiTemplate action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageReject), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DelegateMultiSigner")]
        [HttpPost]
        public IActionResult DelegateMultiSigner(ChangeSignerRequest objChangeSigner)
        {
            string authRefKey = string.Empty, delegateMessageKey = string.Empty;
            loggerModelNew = new LoggerModelNew(objChangeSigner.UserEmailAddress, "SignDocumentController", "DelegateMultiSigner", "Initiate the process for delagating mail to " + objChangeSigner.SignerEmail, "", "", "", "", "API");
            if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail) && !string.IsNullOrEmpty(objChangeSigner.MobileNumber))
                loggerModelNew.Message = "Initiate the process for delagating mail to " + objChangeSigner.SignerEmail + ", " + objChangeSigner.DialCode + objChangeSigner.MobileNumber;
            else if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail))
                loggerModelNew.Message = "Initiate the process for delagating mail to " + objChangeSigner.SignerEmail;
            else if (!string.IsNullOrEmpty(objChangeSigner.MobileNumber))
                loggerModelNew.Message = "Initiate the process for delagating mail to " + objChangeSigner.DialCode + objChangeSigner.MobileNumber;
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageReject responseMessage = new ResponseMessageReject();
            int totalCount = 0;
            var remoteIpAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
            objChangeSigner.IPAddress = remoteIpAddress;
            try
            {
                Dictionary<Guid, Guid> controlIDMapping = new Dictionary<Guid, Guid>();
                Dictionary<Guid, Guid> ruleIDMapping = new Dictionary<Guid, Guid>();
                var envelope = _genericRepository.GetEntity(objChangeSigner.EnvelopeID);

                if (envelope == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["EnvelopeIdMissing"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = loggerModelNew.Message;
                    return BadRequest(responseMessage);
                }
                loggerModelNew.EnvelopeId = envelope.EDisplayCode;
                rSignLogger.RSignLogInfo(loggerModelNew);

                //Get original recipient signer status
                var recipients = _recipientRepository.GetActiveRecipientData(objChangeSigner.EnvelopeID);
                var currentRecipients = recipients.Where(r => objChangeSigner.RecipientID.Contains(r.ID) && r.StatusID != Constants.StatusCode.Signer.Signed).ToList();
                if (currentRecipients.Count != objChangeSigner.RecipientID.Count)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["RecipientIdNotFound"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = loggerModelNew.Message;
                    return BadRequest(responseMessage);
                }
                if (currentRecipients.Any(c => c.StatusID == Constants.StatusCode.Signer.Delegated))
                {
                    loggerModelNew.Message = "Envelope is already delegated";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeAlreadyDelegate");
                    return BadRequest(responseMessage);
                }
                if (currentRecipients.Any(c => c.StatusID == Constants.StatusCode.Signer.Rejected))
                {
                    loggerModelNew.Message = "Envelope is already rejected";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeRejected");
                    return BadRequest(responseMessage);
                }
                if (currentRecipients.Any(c => c.StatusID == Constants.StatusCode.Signer.Signed))
                {
                    loggerModelNew.Message = "Envelope is already Signed";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeAccepted");
                    return BadRequest(responseMessage);
                }
                if (currentRecipients.Any(c => c.StatusID == Constants.StatusCode.Recipients.Transferred))
                {
                    loggerModelNew.Message = "Envelope is already transferred";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeTransferred");
                    return BadRequest(responseMessage);
                }
                var envelopeSender = envelope.Recipients.FirstOrDefault(r => r.RecipientTypeID == Constants.RecipientType.Sender);
                var rcpt = envelope.Recipients.FirstOrDefault(r => r.EmailAddress.ToLower() == objChangeSigner.SignerEmail.ToLower());
                string userDeliveryMode = objChangeSigner.DeliveryMode;

                if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail) && (userDeliveryMode == "1" || userDeliveryMode == "10" || userDeliveryMode == null || userDeliveryMode == "0" || userDeliveryMode == "" || userDeliveryMode == "null"))
                {
                    //Added below validation to allow group envelope signers to delegate to same signer
                    if (rcpt != null)
                    {
                        foreach (var rec in envelope.Recipients)
                        {
                            if (rec.SignerStatus != null && rec.SignerStatus.Count > 1)
                            {
                                if (rec.SignerStatus.Where(x => x.DelegateTo == rcpt.ID).ToList().Count > 0)
                                {
                                    rcpt = null;
                                    break;
                                }
                            }
                        }
                    }

                    if (rcpt != null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "UserDelegatedPartOfEnvelope");
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }
              
                if (!string.IsNullOrEmpty(objChangeSigner.MobileNumber) && (userDeliveryMode == "4" || userDeliveryMode == "10"))
                {
                    rcpt = envelope.Recipients.FirstOrDefault(r => (r.DialCode + r.Mobile) == (objChangeSigner.DialCode + objChangeSigner.MobileNumber));
                    //Added below validation to allow group envelope signers to delegate to same signer
                    if (rcpt != null)
                    {
                        foreach (var rec in envelope.Recipients)
                        {
                            if (rec.SignerStatus != null && rec.SignerStatus.Count > 1)
                            {
                                if (rec.SignerStatus.Where(x => x.DelegateTo == rcpt.ID).ToList().Count > 0)
                                {
                                    rcpt = null;
                                    break;
                                }
                            }
                        }
                    }
                    if (rcpt != null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "MobileDelegatedPartOfEnvelope");
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }

                var envelopeContentData = _envelopeRepository.GetEnvelopeContent(envelope.ID);
                if (envelopeContentData == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeContentNotFouund");
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                var doc = XDocument.Parse(envelopeContentData.ContentXML);

                // delegated recipient id
                foreach (var rec in currentRecipients)
                {
                    objChangeSigner.CurrentRecipientID = rec.ID;

                    var delegatedRecipientId = Guid.NewGuid();

                    var oldString = "data-rcptid=" + "\"" + objChangeSigner.CurrentRecipientID + "\"";
                    var newString = "data-rcptid=" + "\"" + delegatedRecipientId + "\"";


                    //Envelope envelopeObject = envelopeRepository.GetEntity(envelope.ID);
                    //delegated control

                    var documentContents = new List<DocumentContents>();
                    foreach (var document in envelope.Documents)
                    {
                        documentContents.AddRange(document.DocumentContents.Where(d => d.RecipientID == objChangeSigner.CurrentRecipientID));
                    }
                    foreach (var content in documentContents)
                    {
                        var newDocumentContentId = Guid.NewGuid();
                        //content.ControlValue = null;
                        var Control = new DocumentContents
                        {
                            ID = newDocumentContentId,
                            RecipientID = delegatedRecipientId,
                            ControlID = content.ControlID,
                            DocumentID = content.DocumentID,
                            ControlHtmlData = content.ControlHtmlData.Replace(oldString, newString),
                            Required = content.Required,
                            XCoordinate = content.XCoordinate,
                            YCoordinate = content.YCoordinate,
                            ZCoordinate = content.ZCoordinate,
                            PageNo = content.PageNo,
                            Width = content.Width,
                            Height = content.Height,
                            Label = content.Label,
                            ControlHtmlID = content.ControlHtmlID,
                            ControlValue = content.ControlValue,
                            SenderControlValue = content.SenderControlValue,
                            DocumentPageNo = content.DocumentPageNo,
                            GroupName = content.GroupName,
                            MaxLength = content.MaxLength,
                            ControlType = content.ControlType,
                            RecName = objChangeSigner.SignerName,
                            IsControlDeleted = content.IsControlDeleted,
                            LeftIndex = content.LeftIndex,
                            TopIndex = content.TopIndex,
                            SignatureControlValue = content.SignatureControlValue,
                            IsReadOnly = content.IsReadOnly,
                            TabIndex = content.TabIndex,
                            MappedTemplateControlID = content.MappedTemplateControlID,
                            TemplateDocumentPageNo = content.TemplateDocumentPageNo,
                            TemplatePageNo = content.TemplatePageNo,
                            OriginalControlValue = content.OriginalControlValue,
                            LastModifiedBy = content.LastModifiedBy,
                            IntControlId = content.IntControlId,
                            IsDefaultRequired = content.IsDefaultRequired,
                            IsFixedWidth = content.IsFixedWidth == null ? true : Convert.ToBoolean(content.IsFixedWidth) ? true : false
                        };
                        _documentContentsRepository.Save(Control);
                        var controlStyle = _documentContentsRepository.GetControlStyle(content.ID);
                        if (controlStyle != null)
                        {
                            var controlStyleObject = new ControlStyle
                            {
                                DocumentContentId = newDocumentContentId,
                                FontColor = controlStyle.FontColor,
                                FontID = controlStyle.FontID,
                                FontSize = controlStyle.FontSize,
                                IsBold = controlStyle.IsBold,
                                IsItalic = controlStyle.IsItalic,
                                IsUnderline = controlStyle.IsUnderline,
                                AdditionalValidationName = controlStyle.AdditionalValidationName,
                                AdditionalValidationOption = controlStyle.AdditionalValidationOption
                            };
                            _documentContentsRepository.Save(controlStyleObject);
                        }
                        if (content.ControlID == Constants.Control.DropDown)
                        {
                            IQueryable<SelectControlOptions> selectControlOptions = _documentContentsRepository.GetSelectControlOption(content.ID);
                            if (selectControlOptions != null)
                            {
                                foreach (var control in selectControlOptions)
                                {
                                    var selectControlObject = new SelectControlOptions
                                    {
                                        ID = Guid.NewGuid(),
                                        DocumentContentID = newDocumentContentId,
                                        OptionText = control.OptionText,
                                        Order = control.Order
                                    };
                                    _documentContentsRepository.Save(selectControlObject);
                                    if (!ruleIDMapping.ContainsKey(control.ID))
                                        ruleIDMapping.Add(control.ID, selectControlObject.ID);
                                }
                            }
                        }
                        if (!controlIDMapping.ContainsKey(content.ID))
                            controlIDMapping.Add(content.ID, newDocumentContentId);

                        string controlName = string.Empty;
                        var objControl = _documentContentsRepository.GetControlData(content.ControlID);
                        controlName = objControl != null ? objControl.ControlName : string.Empty;

                        var xElement = new XElement("Control",
                                                        new XAttribute("ID", Control.ID),
                                                        new XAttribute("Name", controlName ?? ""),
                                                        new XAttribute("label", Control.Label ?? ""),
                                                        new XAttribute("text", Control.ControlValue ?? ""),
                                                        new XAttribute("required", Control.Required),
                                                        new XAttribute("Height", Control.Height.GetValueOrDefault()),
                                                        new XAttribute("Width", Control.Width.GetValueOrDefault()),
                                                        new XAttribute("PageNo", Control.PageNo.GetValueOrDefault()),
                                                        new XAttribute("XCoordinate", Control.XCoordinate.GetValueOrDefault()),
                                                        new XAttribute("YCoordinate", Control.YCoordinate.GetValueOrDefault()),
                                                        new XAttribute("ZCoordinate", Control.ZCoordinate.GetValueOrDefault()),
                                                        new XElement("Signer",
                                                                    new XAttribute("Name", objChangeSigner.SignerName ?? ""),
                                                                    new XAttribute("EmailAddress", objChangeSigner.SignerEmail ?? "")));

                        var element = doc.Element("Envelope");
                        if (element == null) continue;

                        var firstOrDefault = element.Elements("Documents").Elements("Documents").FirstOrDefault(m => m.Attribute("ID").Value == content.DocumentID.ToString());
                        if (firstOrDefault == null) continue;

                        var controlElements = firstOrDefault.Elements("Controls").FirstOrDefault();
                        if (controlElements != null)
                            controlElements.Add(xElement);
                    }
                    envelopeContentData.ContentXML = doc.ToString();
                    _envelopeRepository.Save(envelopeContentData);

                    // get ipaddress                   
                    string ipAddress = objChangeSigner.IPAddress;
                    // added new object of signerstatus in db
                    _recipientRepository.AddDelegatedSigner(objChangeSigner.CurrentRecipientID, Constants.StatusCode.Signer.Delegated, delegatedRecipientId, ipAddress);

                    Recipients recipientDetail = _recipientRepository.GetEntity(objChangeSigner.CurrentRecipientID);
                    if (objChangeSigner.SendFinalSignDocumentChangeSigner)
                    {
                        _recipientRepository.UpdateIsSendFinalDocumentOnDelegate(recipientDetail);
                    }
                    //Create new recipient
                    var recipient = new Recipients
                    {
                        ID = delegatedRecipientId,
                        Name = objChangeSigner.SignerName,
                        EmailAddress = objChangeSigner.SignerEmail,
                        EnvelopeID = recipientDetail.EnvelopeID,
                        RecipientTypeID = recipientDetail.RecipientTypeID,
                        Order = recipientDetail.Order,
                        CreatedDateTime = DateTime.Now,
                        RecipientCode = _envelopeHelperMain.TakeUniqueDisplayCodeForRecipient(),
                        TemplateID = recipientDetail.TemplateID,
                        EnvelopeTemplateGroupID = recipientDetail.EnvelopeTemplateGroupID,
                        TemplateRoleId = recipientDetail.TemplateRoleId,
                        Comments = objChangeSigner.SignerComments,
                        DeliveryMode = int.Parse(objChangeSigner.DeliveryMode),
                        DialCode = objChangeSigner.DialCode,
                        CountryCode = (objChangeSigner.CountryCode != null && objChangeSigner.CountryCode != "") ? objChangeSigner.CountryCode.ToUpper() : null,
                        Mobile = (objChangeSigner.MobileNumber != null && objChangeSigner.MobileNumber != "") ? objChangeSigner.MobileNumber : null,
                    };
                    // inserted new recipient
                    _recipientRepository.Save(recipient);
                    if (recipientDetail.EnvelopeTemplateGroupID != null)
                    {
                        recipientDetail.EnvelopeTemplateGroupID = null;
                        recipientDetail.TemplateID = null;
                        recipientDetail.TemplateRoleId = null;
                        _recipientRepository.Save(recipientDetail);
                    }

                    Guid SignerStatusNewID = Guid.NewGuid();
                    _recipientRepository.Save(new SignerStatus { ID = SignerStatusNewID, RecipientID = delegatedRecipientId, StatusID = Constants.StatusCode.Signer.Pending, CreatedDateTime = DateTime.Now.AddSeconds(1) });

                    // checked rpostuser 
                    _userRepository.CheckRpostUser(recipient);


                    //Conditional Control mapping                    
                    foreach (var control in controlIDMapping)
                    {
                        ConditionalControlsDetailsNew templateControlRules = _conditionalControlRepository.GetAllConditionalControl("", envelope.ID, control.Key, null);
                        if (templateControlRules == null || templateControlRules.DependentFields == null || templateControlRules.DependentFields.Count < 1)
                            continue;
                        ConditionalControlsDetailsNew tempRule = new ConditionalControlsDetailsNew();

                        tempRule.ID = Guid.NewGuid();
                        tempRule.ControlID = control.Value;
                        tempRule.ControllingFieldID = templateControlRules.ControllingFieldID;
                        tempRule.ControllingConditionID = templateControlRules.ControllingConditionID;
                        tempRule.ControllingSupportText = templateControlRules.ControllingSupportText;
                        tempRule.EnvelopeID = envelope.ID;
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
                                    SupportText = cond.SupportText,
                                    IsRequired = documentContents.FirstOrDefault(f => f.ID == cond.ControlID) != null ? documentContents.FirstOrDefault(f => f.ID == cond.ControlID).Required : false
                                });
                            }

                        }
                        _conditionalControlRepository.SaveConditionalControlForSigner(tempRule);

                    }
                    loggerModelNew.Email = rec.EmailID;
                    loggerModelNew.Message = "Delegating to" + objChangeSigner.SignerEmail + " mail triggering process initiated";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    //delegated to email template
                    var mailSender = envelopeSender != null ? envelopeSender.EmailAddress : Convert.ToString(_appConfiguration["SystemEmailAddress"]);
                    var senderName = _userRepository.GetUserNameByEmailid(mailSender);

                    _envelopeHelperMain.DelegateToMail(objChangeSigner.SignerName, objChangeSigner.SignerEmail, envelope, recipientDetail, delegatedRecipientId, mailSender, senderName, ipAddress, "", authRefKey, recipient.RecipientCode, recipient);
                    _envelopeHelperMain.DelegatedSenderMail(objChangeSigner.SignerName, objChangeSigner.SignerEmail, envelope, recipientDetail, delegatedRecipientId, envelopeSender, mailSender, ipAddress, "", authRefKey, recipient);

                    if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail) && !string.IsNullOrEmpty(recipient.Mobile))
                        loggerModelNew.Message = "Delegating to " + objChangeSigner.SignerEmail + ", " + recipient.DialCode + recipient.Mobile + " mail triggering process completed successfully";
                    else if (!string.IsNullOrEmpty(objChangeSigner.SignerEmail))
                        loggerModelNew.Message = "Delegating to " + objChangeSigner.SignerEmail + " mail triggering process completed successfully";
                    else
                        loggerModelNew.Message = "Delegating to " + recipient.DialCode + recipient.Mobile + " mail triggering process completed successfully";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    //Update Webhook Event status.
                    _genericRepository.UpdateEnvelopeCommonWebhookTransaction(envelope, Constants.WebhookEventNames.SignerStatus, mailSender, delegatedRecipientId.ToString(), SignerStatusNewID.ToString());
                    //Update Webhook Event status to add delegated notification and added delegateMsgforNotifications.
                    string delegateMsgforNotifications = recipientDetail.Name + " has delegated your document to " + objChangeSigner.SignerName.Split(' ').FirstOrDefault() + " for electronic signature.";
                    _genericRepository.UpdateEnvelopeCommonWebhookTransaction(envelope, Constants.WebhookEventNames.NotificationType, mailSender, delegatedRecipientId.ToString(), SignerStatusNewID.ToString(), delegateMsgforNotifications);
                    //Update current signer notification signing request to IsRead = 1 because he delegated to some others.
                    _envelopeRepository.UpdateRAppNotificationEvents(recipientDetail, envelope);
                }

                delegateMessageKey = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeDelegatedTo");
                var PendingRecipient = _recipientRepository.GetEnvelopeSignerRecipientByEmail(objChangeSigner.EnvelopeID, objChangeSigner.UserEmailAddress).ToList();
                if (PendingRecipient != null && PendingRecipient.Count > 0)
                    totalCount = PendingRecipient.Count;

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = delegateMessageKey;
                responseMessage.EncryptedSender = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(Convert.ToString(responseMessage.SenderEmail), Convert.ToString(_appConfiguration["AppKey"])));
                loggerModelNew.Message = "Process completed for Change signer and " + responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller DelegateMultiSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(FinishSubEnvelopeResponse), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("FinishSubEnvelope")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult FinishSubEnvelope(FinishSubEnvelopeRequest finishSubEnvelopeRequest)
        {
            FinishSubEnvelopeResponse responseMessage = new FinishSubEnvelopeResponse();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "FinishSubEnvelope", "Initiate the process for Finish SubEnvelope using API.", finishSubEnvelopeRequest.EnvelopeId.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                Envelope envelope = _genericRepository.GetEntity(finishSubEnvelopeRequest.EnvelopeId);
                if (envelope == null)
                {
                    //Provided envelope is not exist
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    responseMessage.Message = Convert.ToString(_appConfiguration["EnvelopeIdMissing"]);
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                if (envelope.IsActive == false)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    responseMessage.Message = "The sender has deleted the envelope. For further information, please contact the sender.";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);

                }
                if (Convert.ToBoolean(envelope.IsDraftDeleted) == true)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeDiscarded");
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);

                }
                if (envelope.ExpiryDate < DateTime.Today)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeExpired");
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);

                }
                // Check if envelope is already rejected   
                if (envelope.StatusID == Constants.StatusCode.Envelope.Terminated)
                {
                    responseMessage.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeTerminated");
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                // Check if envelope is already Cancelled   
                if (envelope.StatusID == Constants.StatusCode.Envelope.CancelledTransaction)
                {
                    responseMessage.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopeCancelled");
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                // Check if envelope is already Completed  
                if (envelope.StatusID == Constants.StatusCode.Envelope.Completed)
                {
                    responseMessage.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "ConatctSender");
                    responseMessage.ErrorAction = "ContactSender";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                var recipientRecords = _recipientRepository.GetActiveRecipientData(finishSubEnvelopeRequest.EnvelopeId).ToList();

                List<RecipientDetails> currentRecipientRecords = null;
                if (string.IsNullOrEmpty(finishSubEnvelopeRequest.RecipientEmail))
                    currentRecipientRecords = recipientRecords.Where(a => a.RecipientTypeID == Constants.RecipientType.Signer && (a.DialCode + a.Mobile) == finishSubEnvelopeRequest.RecipientMobile).ToList();
                else
                    currentRecipientRecords = recipientRecords.Where(a => a.RecipientTypeID == Constants.RecipientType.Signer && a.EmailID == finishSubEnvelopeRequest.RecipientEmail).ToList();

                var isFinishAllowed = true;
                foreach (var recipient in currentRecipientRecords.Where(r => r.RecipientTypeID == Constants.RecipientType.Signer))
                {
                    if (recipient.StatusID == null || recipient.StatusID == Guid.Empty)
                    {
                        responseMessage.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                        responseMessage.StatusMessage = "NonAuthoritativeInformation";
                        responseMessage.Message = "Please Sign/Decline/Delegate all the documents to Finish.";
                        responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                    if (recipient.StatusID != Constants.StatusCode.Signer.Signed && recipient.StatusID != Constants.StatusCode.Signer.Rejected)
                    {
                        isFinishAllowed = false;
                        responseMessage.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                        responseMessage.StatusMessage = "NonAuthoritativeInformation";
                        responseMessage.Message = "Please Sign/Decline/Delegate all the documents to Finish.";
                        responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }
                if (currentRecipientRecords.Where(a => a.IsFinished == true).Count() == currentRecipientRecords.Count())
                {
                    responseMessage.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    // responseMessage.Message = "Document is already finished.";
                    responseMessage.Message = _envelopeHelperMain.GetLanguageBasedApiMessge(envelope.UserID, "EnvelopefinishedMessage");
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                if (!isFinishAllowed)
                {
                    responseMessage.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    responseMessage.Message = "Please Sign/Decline/Delegate all the documents to Finish.";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                var requiredUploads = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByEnvelope(envelope.ID);
                if (requiredUploads.Where(a => a.IsRequired == true).Any(a => a.FileName == null) == true)
                {
                    isFinishAllowed = false;
                    responseMessage.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                    responseMessage.StatusMessage = "NonAuthoritativeInformation";
                    responseMessage.Message = "Please Upload Required Documents to Finish.";
                    responseMessage.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                foreach (var recipient in currentRecipientRecords)
                {
                    var recipientRecord = _recipientRepository.GetEntity(recipient.ID);
                    recipientRecord.IsFinished = true;
                    _recipientRepository.Save(recipientRecord);
                }
                //envelope.ModifiedDateTime = DateTime.Now;
                //dbRecipient.IsFinished = true;   

                string postSigningPage = string.Empty;
                if (!string.IsNullOrEmpty(envelope.PostSigningLandingPage))
                    postSigningPage = envelope.PostSigningLandingPage;
                else
                {
                    UserProfile envOwner = _userRepository.GetUserProfile(envelope.UserID);
                    if (envOwner.CompanyID == null)
                        postSigningPage = Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]);
                    else
                    {
                        var company = _companyRepository.GetCompanyByID(envOwner.CompanyID);
                        postSigningPage = string.IsNullOrEmpty(company.PostSigningLandingPage) ? Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]) : company.PostSigningLandingPage;
                    }
                }

                //if (!recipientRecords.Any(r => r.RecipientTypeID == Constants.RecipientType.Signer && r.EmailID != finishSubEnvelopeRequest.RecipientEmail && r.IsFinished != true))
                //{
                //    _envelopeHelperMain.CheckAndCompleteGroupEnvelope(envelope, finishSubEnvelopeRequest.RecipientId);
                //}

                if (!string.IsNullOrEmpty(finishSubEnvelopeRequest.RecipientEmail))
                {
                    if (!recipientRecords.Any(r => r.RecipientTypeID == Constants.RecipientType.Signer && r.EmailID != finishSubEnvelopeRequest.RecipientEmail && r.IsFinished != true))
                    {
                        _envelopeHelperMain.CheckAndCompleteGroupEnvelope(envelope, finishSubEnvelopeRequest.RecipientId);
                    }
                }
                else
                {
                    if (!recipientRecords.Any(r => r.RecipientTypeID == Constants.RecipientType.Signer && (r.DialCode + r.Mobile) != finishSubEnvelopeRequest.RecipientMobile && r.IsFinished != true))
                    {
                        _envelopeHelperMain.CheckAndCompleteGroupEnvelope(envelope, finishSubEnvelopeRequest.RecipientId);
                    }
                }

                FinishSubEnvelopeResponse recipientResp = new FinishSubEnvelopeResponse();
                recipientResp.StatusCode = HttpStatusCode.OK;
                recipientResp.StatusMessage = "OK";
                recipientResp.Message = "Envelope finished successfully.";
                recipientResp.EnvelopeID = finishSubEnvelopeRequest.EnvelopeId;
                recipientResp.ReturnURL = postSigningPage;
                loggerModelNew.Message = "Process completed for Finish SubEnvelope using API and " + recipientResp.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(recipientResp);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller FinishSubEnvelope action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(InfoResultResonse), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DownloadOtherDriveDocumentsForSigner")]
        [HttpPost]
        //Upload One Drive Signature
        public async Task<IActionResult> DownloadOtherDriveDocumentsForSigner(UploadSignerDriveFiles driveDetails)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "DownloadOtherDriveDocumentsForSigner", "Initiate the process for Download Other Drive Documents For Signer using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            InfoResultResonse responseMessage = new InfoResultResonse();
            string invalidFileErrorMessage = "Invalid file format";
            byte[] myDataBuffer;
            string base64String = string.Empty;
            try
            {
                string[] invalidFileTypes = { "exe", "msi", "js", "jar", "vb", "vbs", "bat", "doc", "docx", "xls", "xlsx", "pdf" };
                foreach (var file in driveDetails.DriveFiles)
                {

                    string ext = Path.GetExtension(file.FileName);
                    bool isValidType = true;
                    double size = 0;
                    for (int j = 0; j < invalidFileTypes.Length; j++)
                    {
                        if (ext == "." + invalidFileTypes[j])
                        {
                            isValidType = false;
                            loggerModelNew.Message = invalidFileErrorMessage;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.message = "lang_valInvalidFile";
                            responseMessage.success = false;
                            return BadRequest(responseMessage);
                        }
                    }

                    string strFileName = Path.GetFileNameWithoutExtension(file.FileName) + ext;
                    if (driveDetails.UploadDocDriveType == Constants.UploadDriveType.Dropbox)
                    {
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                myDataBuffer = webClient.DownloadData(new Uri(file.FileSource));
                            }
                        }
                        catch (WebException ex)
                        {
                            loggerModelNew.Message = ex.Message;
                            rSignLogger.RSignLogError(loggerModelNew, ex);
                        }
                    }
                    else if (driveDetails.UploadDocDriveType == Constants.UploadDriveType.Skydrive)
                    {
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                myDataBuffer = webClient.DownloadData(new Uri(file.FileSource));
                                base64String = Convert.ToBase64String(myDataBuffer, 0, myDataBuffer.Length);

                                if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".bmp")
                                {
                                    using (System.IO.Stream memStream = new MemoryStream(Convert.FromBase64String(base64String)))
                                    {
                                        using (System.Drawing.Image img = System.Drawing.Image.FromStream(memStream))
                                        {
                                            if (img.Width == 528 && img.Height == 113)
                                            {
                                                base64String = base64String;
                                            }
                                            else
                                            {
                                                byte[] imageBytes;
                                                // To preserve the aspect ratio
                                                int maxWidth = 528;
                                                int maxHeight = 113;
                                                float ratioX = (float)maxWidth / (float)img.Width;
                                                float ratioY = (float)maxHeight / (float)img.Height;
                                                float ratio = Math.Min(ratioX, ratioY);

                                                float sourceRatio = (float)img.Width / img.Height;

                                                int newWidth = (int)(img.Width * ratio);
                                                int newHeight = (int)(img.Height * ratio);

                                                Bitmap newImage = new Bitmap(newWidth, newHeight); //, PixelFormat.Format24bppRgb

                                                using (Graphics graphics = Graphics.FromImage(newImage))
                                                {
                                                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                                                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                                                    graphics.DrawImage(img, 0, 0, newWidth, newHeight);
                                                    using (MemoryStream ms = new MemoryStream())
                                                    {
                                                        Bitmap image = new Bitmap(img, 528, 113);
                                                        if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                                                        {
                                                            image.Save(ms, ImageFormat.Jpeg);
                                                        }
                                                        else if (ext.ToLower() == ".png")
                                                        {
                                                            image.Save(ms, ImageFormat.Png);
                                                        }
                                                        else if (ext.ToLower() == ".bmp")
                                                        {
                                                            image.Save(ms, ImageFormat.Bmp);
                                                        }
                                                        else
                                                        {
                                                            image.Save(ms, ImageFormat.Jpeg);
                                                        }

                                                        imageBytes = ms.ToArray();
                                                        // Convert byte[] to Base64 String
                                                        base64String = Convert.ToBase64String(imageBytes);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (WebException ex)
                        {
                            loggerModelNew.Message = ex.Message;
                            rSignLogger.RSignLogError(loggerModelNew, ex);
                        }
                    }
                    string fileNameToSaveStr = string.Empty, fileNameToSave = string.Empty;
                    //string strFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + objRecipients.Order + "_" + objRecipients.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                }

                responseMessage.data = base64String;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.message = "Base64data saved successfully.";
                responseMessage.success = true;
                loggerModelNew.Message = "Process completed for Download Other Drive Documents For Signer using API and " + responseMessage.message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller DownloadOtherDriveDocumentsForSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(InfoResultResonse), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DownloadDriveImageForSigner")]
        [HttpPost]
        //Upload Google Drive Signature
        public async Task<IActionResult> DownloadDriveImageForSigner(UploadSignerImageDriveFiles driveDetails)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "DownloadDriveImageForSigner", "Initiate the process for Download Drive Image For Signer using API", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            InfoResultResonse responseMessage = new InfoResultResonse();
            string invalidFileErrorMessage = "Invalid file format";
            string base64String = string.Empty;
            try
            {
                string UserId = "";
                var userProfile = _userTokenRepository.GetUserProfileByEmail(driveDetails.recipientEmailSiA);
                if (userProfile != null)
                    UserId = Convert.ToString(userProfile.ID);
                else
                    UserId = Convert.ToString(Guid.NewGuid());

                // Guid UserId = _userTokenRepository.GetUserProfileUserIDByID(_userTokenRepository.GetUserProfileIDByEmail(driveDetails.recipientEmailSiA));              
                string tempDirPath = _modelHelper.GetEnvelopeDirectoryNew(driveDetails.EnvelopeID, string.Empty);
                var dirPath = tempDirPath + driveDetails.EnvelopeID;

                string[] invalidFileTypes = { "exe", "msi", "js", "jar", "vb", "vbs", "bat", "doc", "docx", "xls", "xlsx", "pdf" };

                foreach (var file in driveDetails.DriveFiles)
                {
                    _eSignHelper.CreateEnvelopeXML(driveDetails.EnvelopeID, tempDirPath);
                    UploadDriveDocument uploadDriveDocument = new UploadDriveDocument();
                    uploadDriveDocument.FileName = file.FileName;
                    uploadDriveDocument.AccessToken = driveDetails.GAuthToken;
                    uploadDriveDocument.DownloadUrl = file.FileSource;
                    string ext = Path.GetExtension(uploadDriveDocument.FileName);
                    bool isValidType = true;
                    double size = 0;
                    for (int j = 0; j < invalidFileTypes.Length; j++)
                    {
                        if (ext == "." + invalidFileTypes[j])
                        {
                            isValidType = false;
                            loggerModelNew.Message = invalidFileErrorMessage;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.message = "lang_valInvalidFile";
                            responseMessage.success = false;
                            return BadRequest(responseMessage);
                        }
                    }

                    string strFileName = Path.GetFileNameWithoutExtension(uploadDriveDocument.FileName) + ext;
                    //TempSignatureImageDirectory
                    string signatureImagePath = Path.Combine(Convert.ToString(_appConfiguration["TempSignatureImageDirectory"]), UserId.ToString());
                    try
                    {
                        if (!Directory.Exists(signatureImagePath))
                        {
                            Directory.CreateDirectory(signatureImagePath);
                        }
                        else
                        {
                            if (Directory.Exists(signatureImagePath))
                            {
                                string[] files = Directory.GetFiles(signatureImagePath);
                                foreach (string f1 in files)
                                {
                                    System.IO.File.Delete(f1);
                                }
                            }
                        }
                        string imgebase64Data = gDrive.DownloadFile(uploadDriveDocument.AccessToken, uploadDriveDocument.DownloadUrl, strFileName, signatureImagePath);
                        string signatureImageUrl = Path.Combine(signatureImagePath, strFileName);
                        base64String = Convert.ToBase64String(System.IO.File.ReadAllBytes(signatureImageUrl));
                        if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".bmp")
                        {
                            using (System.IO.Stream memStream = new MemoryStream(Convert.FromBase64String(base64String)))
                            {
                                using (System.Drawing.Image img = System.Drawing.Image.FromStream(memStream))
                                {
                                    if (img.Width == 528 && img.Height == 113)
                                    {
                                        base64String = base64String;
                                    }
                                    else
                                    {
                                        byte[] imageBytes;
                                        // To preserve the aspect ratio
                                        int maxWidth = 528;
                                        int maxHeight = 113;
                                        float ratioX = (float)maxWidth / (float)img.Width;
                                        float ratioY = (float)maxHeight / (float)img.Height;
                                        float ratio = Math.Min(ratioX, ratioY);
                                        float sourceRatio = (float)img.Width / img.Height;
                                        int newWidth = (int)(img.Width * ratio);
                                        int newHeight = (int)(img.Height * ratio);
                                        Bitmap newImage = new Bitmap(newWidth, newHeight);

                                        using (Graphics graphics = Graphics.FromImage(newImage))
                                        {
                                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                                            graphics.DrawImage(img, 0, 0, newWidth, newHeight);
                                            using (MemoryStream ms = new MemoryStream())
                                            {
                                                Bitmap image = new Bitmap(img, 528, 113);
                                                if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                                                {
                                                    image.Save(ms, ImageFormat.Jpeg);
                                                }
                                                else if (ext.ToLower() == ".png")
                                                {
                                                    image.Save(ms, ImageFormat.Png);
                                                }
                                                else if (ext.ToLower() == ".bmp")
                                                {
                                                    image.Save(ms, ImageFormat.Bmp);
                                                }
                                                else
                                                {
                                                    image.Save(ms, ImageFormat.Jpeg);
                                                }
                                                imageBytes = ms.ToArray();

                                                // Convert byte[] to Base64 String
                                                base64String = Convert.ToBase64String(imageBytes);
                                                // lstFiles.Add(base64String);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        if (Directory.Exists(signatureImagePath))
                        {
                            string[] files = Directory.GetFiles(signatureImagePath);
                            foreach (string f1 in files)
                            {
                                System.IO.File.Delete(f1);
                            }
                            Directory.Delete(signatureImagePath);
                        }
                    }
                    catch (WebException ex)
                    {
                        loggerModelNew.Message = ex.Message;
                        rSignLogger.RSignLogError(loggerModelNew, ex);
                    }
                }

                responseMessage.data = base64String;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.message = "Documents Saved successfully.";
                responseMessage.success = true;
                loggerModelNew.Message = "Process completed for Download Drive Image For Signer using API and " + responseMessage.message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller DownloadDriveImageForSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(InfoResultResonse), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DownloadDriveDocumentsForSigner")]
        [HttpPost]
        //Upload Google Drive Attachments
        //Upload Drop Box Attachments
        //Upload One Drive Attachments
        public async Task<IActionResult> DownloadDriveDocumentsForSigner(UploadSignerDriveFiles driveDetails)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "DownloadDriveDocumentsForSigner", "Process started for Download Drive Documents For Signer using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            InfoResultResonse responseMessage = new InfoResultResonse();
            List<DocumentUploadFilesResult> uploadedDocuments = new List<DocumentUploadFilesResult>();
            DocumentUploadFilesResult documentUpload = new DocumentUploadFilesResult();

            string invalidFileErrorMessage = "Invalid file format";
            string exccedLimitMessage = "Cannot upload more than 10 files.";
            string exccedSizeMessage = "Cannot attach more than 15MB.";
            Recipients objRecipients = new Recipients();

            string NameSia = string.Empty, DescriptionSiA = string.Empty, AdditionalInfoSiA = string.Empty;
            int UploadAttachmentID = 0;
            UploadAttachmentID = driveDetails.UploadAttachmentID;
            NameSia = driveDetails.NameSiA;
            DescriptionSiA = driveDetails.DescriptionSiA;
            AdditionalInfoSiA = driveDetails.AdditionalInfoSiA;

            List<string> lstFiles = new List<string>();
            bool IsNewRow = false;
            try
            {
                Guid recipientID = Guid.Parse(driveDetails.Stage);
                if (!driveDetails.IsStaticTemplate)
                {
                    objRecipients = _recipientRepository.GetEntity(recipientID);
                    if (objRecipients == null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.NoContent;
                        responseMessage.StatusMessage = "NoContent";
                        responseMessage.message = Convert.ToString(_appConfiguration["NoContent"]);
                        responseMessage.success = false;
                        loggerModelNew.Message = responseMessage.message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }

                EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                envelopeAdditionalUploadInfo = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByID(UploadAttachmentID, recipientID);
                if (envelopeAdditionalUploadInfo == null)  // insert new rec 
                {
                    EnvelopeAdditionalUploadInfo newObj = new EnvelopeAdditionalUploadInfo();
                    newObj.MasterEnvelopeID = driveDetails.EnvelopeID;
                    newObj.Name = NameSia;
                    newObj.Description = DescriptionSiA;
                    newObj.AdditionalInfo = AdditionalInfoSiA;
                    newObj.FileName = NameSia;
                    newObj.OriginalFileName = NameSia;
                    newObj.RecipientEmailID = driveDetails.recipientEmailSiA;
                    newObj.CreatedDateTime = DateTime.Now;
                    newObj.ModifiedDateTime = DateTime.Now;
                    newObj.RecipientID = recipientID;

                    _envelopeRepository.SaveEnvelopeAdditionalUploadInfo(newObj);

                    envelopeAdditionalUploadInfo = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByID(Convert.ToInt32(newObj.ID), recipientID);
                    UploadAttachmentID = Convert.ToInt32(envelopeAdditionalUploadInfo.ID);
                    IsNewRow = true;
                }

                string dirPath = _modelHelper.GetEnvelopeDirectoryNew(driveDetails.EnvelopeID, string.Empty) + driveDetails.EnvelopeID;
                if (driveDetails.IsStaticTemplate)
                    dirPath = Path.Combine(Convert.ToString(_appConfiguration["TempDirectory"]), driveDetails.EnvelopeID.ToString());

                var uploadedDirectory = dirPath + "\\SignerAttachments" + "\\" + envelopeAdditionalUploadInfo.RecipientID;
                if (!Directory.Exists(uploadedDirectory))
                    Directory.CreateDirectory(uploadedDirectory);
                string[] fileArray = Directory.GetFiles(uploadedDirectory);
                int count = fileArray.Count();
                string[] invalidFileTypes = { "exe", "msi", "js", "jar", "vb", "vbs", "bat" };

                foreach (var file in driveDetails.DriveFiles)
                {
                    _eSignHelper.CreateEnvelopeXML(driveDetails.EnvelopeID, dirPath);
                    UploadDriveDocument uploadDriveDocument = new UploadDriveDocument();
                    uploadDriveDocument.FileName = file.FileName;
                    uploadDriveDocument.AccessToken = driveDetails.GAuthToken;
                    uploadDriveDocument.DownloadUrl = file.FileSource;
                    uploadDriveDocument.EnvelopeStage = driveDetails.Stage;
                    string ext = Path.GetExtension(uploadDriveDocument.FileName);
                    bool isValidType = true;
                    double size = 0;
                    for (int j = 0; j < invalidFileTypes.Length; j++)
                    {
                        if (ext == "." + invalidFileTypes[j])
                        {
                            isValidType = false;
                            loggerModelNew.Message = invalidFileErrorMessage;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "BadRequest";
                            responseMessage.message = "lang_valInvalidFile";
                            responseMessage.success = false;
                            return BadRequest(responseMessage);
                        }
                    }

                    if (count > 10)
                    {
                        loggerModelNew.Message = exccedLimitMessage;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.message = "lang_ValMaxFileSize";
                        responseMessage.success = false;
                        return BadRequest(responseMessage);
                    }
                    foreach (FileInfo folderfiles in new DirectoryInfo(uploadedDirectory).GetFiles())
                    {
                        size += folderfiles.Length;
                    }
                    size = size + file.FileSize;
                    if (size > 15728640)
                    {
                        if (envelopeAdditionalUploadInfo != null)
                        {
                            _envelopeRepository.RemoveEnvelopeAdditionalUploadInfo(envelopeAdditionalUploadInfo);
                        }
                        loggerModelNew.Message = exccedSizeMessage;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        //return Json(new InfoResult { success = false, message = "lang_ValMaxFileSize", returnUrl = null, data = null });
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "BadRequest";
                        responseMessage.message = "lang_ValMaxFileSize";
                        responseMessage.success = false;
                        return BadRequest(responseMessage);
                    }
                    // string strFileName = Path.GetFileNameWithoutExtension(uploadDriveDocument.FileName) + "_" + objRecipients.Order + "_" + objRecipients.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                    string strFileName = Path.GetFileNameWithoutExtension(uploadDriveDocument.FileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;

                    string docFinalPath = Path.Combine(uploadedDirectory, strFileName);

                    if (!IsNewRow)
                    {
                        if (!string.IsNullOrEmpty(envelopeAdditionalUploadInfo.FileName))
                        {
                            var TempuploadedDirectory = (dirPath + "\\TempSignerAttachments") + "\\" + recipientID;
                            if (!Directory.Exists(TempuploadedDirectory))
                                Directory.CreateDirectory(TempuploadedDirectory);
                            string sourceFile = Path.Combine(uploadedDirectory, envelopeAdditionalUploadInfo.FileName);
                            string destFile = Path.Combine(TempuploadedDirectory, envelopeAdditionalUploadInfo.FileName);
                            if (System.IO.File.Exists(Path.Combine(uploadedDirectory, envelopeAdditionalUploadInfo.FileName)))
                            {
                                System.IO.File.Copy(sourceFile, destFile, true);
                                System.IO.File.Delete(sourceFile);
                            }
                        }
                    }

                    if (driveDetails.UploadDocDriveType == Constants.UploadDriveType.Google)
                    {
                        try
                        {
                            gDrive.DownloadFile(uploadDriveDocument.AccessToken, uploadDriveDocument.DownloadUrl, strFileName, uploadedDirectory);
                        }
                        catch (WebException ex)
                        {
                            loggerModelNew.Message = ex.Message;
                            rSignLogger.RSignLogError(loggerModelNew, ex);
                        }
                    }
                    else if (driveDetails.UploadDocDriveType == Constants.UploadDriveType.Dropbox)
                    {
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFile(new Uri(uploadDriveDocument.DownloadUrl), docFinalPath);
                            }
                        }
                        catch (WebException ex)
                        {
                            loggerModelNew.Message = ex.Message;
                            rSignLogger.RSignLogError(loggerModelNew, ex);
                        }
                    }
                    else if (driveDetails.UploadDocDriveType == Constants.UploadDriveType.Skydrive)
                    {
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFile(new Uri(file.FileSource), docFinalPath);
                            }
                        }
                        catch (WebException ex)
                        {
                            loggerModelNew.Message = ex.Message;
                            rSignLogger.RSignLogError(loggerModelNew, ex);
                        }
                    }
                    else if (driveDetails.UploadDocDriveType == Constants.UploadDriveType.iManage)
                    {
                        try
                        {
                            Request.Headers.TryGetValue("AuthToken", out Microsoft.Extensions.Primitives.StringValues iHeader);
                            string authToken = iHeader.ElementAt(0);
                            string userEmail = _userTokenRepository.GetUserEmailByToken(authToken);
                            Guid UserId = _userTokenRepository.GetUserProfileUserIDByID(_userTokenRepository.GetUserProfileIDByEmail(userEmail));
                            SettingsExternalIntegration settings = _settingsRepository.GetExternalSettingsByType(UserId, "iManage");
                            string SeverURL = string.Empty;
                            if (settings != null)
                            {
                                SeverURL = settings.ServerURL;
                            }
                            string DownloadDocumentUrl = string.Format(Convert.ToString(_appConfiguration["iManageDownloadUrl"]), SeverURL, settings.CustomerId, file.FileSource);

                            using (WebClient webClient = new WebClient())
                            {
                                webClient.Headers.Add("X-Auth-Token", settings.AccessToken);
                                webClient.DownloadFile(new Uri(DownloadDocumentUrl), docFinalPath);
                            }

                        }
                        catch (WebException ex)
                        {
                            loggerModelNew.Message = ex.Message;
                            rSignLogger.RSignLogError(loggerModelNew, ex);
                        }
                    }
                    string fileNameToSaveStr = string.Empty, fileNameToSave = string.Empty;
                    //string strFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + objRecipients.Order + "_" + objRecipients.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                    responseMessage.field = strFileName;
                    responseMessage.returnUrl = envelopeAdditionalUploadInfo.ID.ToString();
                    if (envelopeAdditionalUploadInfo != null)
                    {
                        envelopeAdditionalUploadInfo.FileName = strFileName;
                        envelopeAdditionalUploadInfo.OriginalFileName = uploadDriveDocument.FileName;
                        bool isUpdated = _envelopeRepository.UpdateEnvelopeAdditionalUploadInfo(envelopeAdditionalUploadInfo);
                    }
                    lstFiles.Add(strFileName);
                }

                responseMessage.data = lstFiles;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.message = "Documents Saved successfully.";
                responseMessage.success = true;
                loggerModelNew.Message = "Process completed for Download Drive Documents For Signer and " + responseMessage.message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller DownloadDriveDocumentsForSigner action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(EnvelopeGetEnvelopeHistoryByEnvelopeCode), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetBasicEnvelopeHistory/{envelopeId}")]
        [HttpGet]
        public async Task<IActionResult> GetBasicEnvelopeHistory(string envelopeId)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetBasicEnvelopeHistory", "Process started for Get Basic Envelope History using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            EnvelopeGetEnvelopeHistoryByEnvelopeCode envelopeMetadata = null;
            string userName = string.Empty;
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                envelopeMetadata = _envelopeRepository.GetEnvelopeMetaDataWithHistory(new Guid(envelopeId), string.Empty, string.Empty, "UTC");
                if (envelopeMetadata != null)
                {
                    var sender = _userRepository.GetUserProfileByUserID(envelopeMetadata.UserID);
                    if (sender != null)
                        envelopeMetadata.SenderCompanyID = sender.CompanyID;
                }
                APISettings apiSettings = _settingsRepository.GetEntityByParam(envelopeMetadata.UserID, string.Empty, Constants.String.SettingsType.User);
                var userSettings = _eSignHelper.TransformSettingsDictionaryToEntity(apiSettings);
                envelopeMetadata.SenderAPISettings = userSettings;
                if (envelopeMetadata == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_appConfiguration["NoContent"].ToString());
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                return Ok(envelopeMetadata);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetBasicEnvelopeHistory action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// ConfirmUserOnFinalSubmit
        /// </summary>
        /// <param name="signerVerificationOTP"></param>
        /// <returns></returns>

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("ConfirmUserOnFinalSubmit")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmUserOnFinalSubmit(SignerVerificationOTP signerVerificationOTP)
        {
            ResponseMessageForGetEnvelopeOrTemplateFields responseMessage = new ResponseMessageForGetEnvelopeOrTemplateFields();
            Envelope envelope = new Envelope();
            Template template = new Template();
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "ConfirmUserOnFinalSubmit", "Initiate the process for generating Verification Code using API.", signerVerificationOTP.EnvelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                Recipients existingRecipients = new Recipients();
                TemplateRoles templateRoles = new TemplateRoles();
                Recipients templateRecipient = new Recipients();

                string randomNumber = _genericRepository.GenerateRandomVerificationCode();
                envelope = _genericRepository.GetEntity(signerVerificationOTP.EnvelopeID);
                existingRecipients = _recipientRepository.GetEntity(signerVerificationOTP.RecipientID);
                if (existingRecipients == null)
                {
                    templateRoles = _templateRepository.GetRoleEntity(signerVerificationOTP.RecipientID);
                    if (templateRoles != null)
                    {
                        Guid templateid = templateRoles.TemplateID;
                        template = _genericRepository.GetTemplateDetails(templateid);

                        templateRecipient.DeliveryMode = templateRoles.DeliveryMode;
                        templateRecipient.DialCode = signerVerificationOTP.DialCode;
                        templateRecipient.Name = templateRoles.Name;
                        templateRecipient.Mobile = signerVerificationOTP.Mobile;
                        templateRecipient.EmailAddress = signerVerificationOTP.EmailAddress;
                    }
                }

                string cultureInfo = envelope.CultureInfo == null ? templateRoles.CultureInfo : envelope.CultureInfo;
                if (signerVerificationOTP != null)
                {
                    Guid userID = existingRecipients == null ? template.UserID : envelope.UserID;
                    string Message = string.Empty;
                    var userprofile = _userRepository.GetUserProfile(userID);
                    
                    bool isEmailSend = false, isPwdEmailSend = false;
                    bool isMobileSend = false, isPwdMobileSend = false;

                    Recipients recDetails = new Recipients();
                    if (existingRecipients != null)
                    {
                        recDetails.DeliveryMode = existingRecipients.DeliveryMode;
                        recDetails.EmailAddress = existingRecipients.EmailAddress;
                        recDetails.Mobile = existingRecipients.Mobile;
                    }
                    else if (templateRecipient != null)
                    {
                        recDetails.DeliveryMode = templateRecipient.DeliveryMode;
                        recDetails.EmailAddress = templateRecipient.EmailAddress;
                        recDetails.Mobile = templateRecipient.Mobile;
                    }

                    if (Convert.ToBoolean(envelope.EnableMessageToMobile) || Convert.ToBoolean(template.EnableMessageToMobile))
                    {                       
                        _envelopeHelperMain.GetMobileDeiliveryModeOptions(recDetails, ref isPwdEmailSend, ref isPwdMobileSend);
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
                    AdminGeneralAndSystemSettings allSettingsDetails = new AdminGeneralAndSystemSettings();
                    if (existingRecipients != null)
                    {
                        settingsDetails = _settingsRepository.GetEntityForByKeyConfig(userID, Constants.SettingsKeyConfig.SignatureRequestReplyAddress);
                        var envelopeSettingObject = _eSignHelper.GetEnvelopeSettingsDetail(envelope.ID);
                        if (envelopeSettingObject != null)
                            SignReqReplyToAddressValue = Convert.ToInt32(envelopeSettingObject.SignReqReplyAdrs);
                        allSettingsDetails = _envelopeHelperMain.GetAllSettingsDetails(userID, userprofile.EmailID);
                    }
                    else
                    {
                        settingsDetails = _settingsRepository.GetEntityForByKeyConfig(template.UserID, Constants.SettingsKeyConfig.SignatureRequestReplyAddress);
                        if (settingsDetails != null)
                            SignReqReplyToAddressValue = Convert.ToInt32(settingsDetails.OptionValue);
                        allSettingsDetails = _envelopeHelperMain.GetAllSettingsDetails(template.UserID, userprofile.EmailID);
                    }

                    if (isPwdEmailSend)
                    {
                        loggerModelNew.Message = "Process started for Sending verification code to email.";
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        string mailTemplate = string.Empty;
                        string[] toAddressForPw, toDisplayNameForPw;
                        mailTemplate = _genericRepository.GetNewMailTemplate(Constants.String.MailTemplateName.FinalSubmitVerificationCode, cultureInfo, EmailDisclaimer);
                        toAddressForPw = envelope.Recipients.Where(r => r.ID == signerVerificationOTP.RecipientID).Select(r => r.EmailAddress).ToArray();
                        toDisplayNameForPw = envelope.Recipients.Where(r => r.ID == signerVerificationOTP.RecipientID).Select(r => r.Name).ToArray();

                        if (existingRecipients == null)
                        {
                            toAddressForPw = new string[] { templateRecipient.EmailAddress };
                            toDisplayNameForPw = new string[] { templateRecipient.Name };
                        }

                        string imageLogoURl = _appConfiguration["domain"].ToString() + "Content/img/RMail-100.png";
                        if (existingRecipients != null)
                        {
                            mailTemplate = _envelopeHelperMain.CreateVerificationCodeMailTemplate(envelope, imageLogoURl, userprofile.EmailID, userprofile.FullName, existingRecipients, userprofile.FirstName, mailTemplate, "", existingRecipients.Name, randomNumber);
                        }
                        else
                        {
                            mailTemplate = _envelopeHelperMain.CreateVerificationCodeMailTemplate(envelope, imageLogoURl, userprofile.EmailID, userprofile.FullName, templateRecipient, userprofile.FirstName, mailTemplate, "", templateRecipient.Name, randomNumber);
                        }

                        mailTemplate = _envelopeHelperMain.EmailBannerSettings(mailTemplate, allSettingsDetails, Constants.String.MailTemplateName.FinalSubmitVerificationCode);
                        mailTemplate = _genericRepository.AppendFooterText(mailTemplate, Convert.ToString(toAddressForPw[0]), Constants.String.MailTemplateName.FinalSubmitVerificationCode, cultureInfo, "FinalContractFooter");

                        string subject = existingRecipients != null ? envelope.Subject : template.Subject;
                        loggerModelNew.Message = "Sending message to email:" + Convert.ToString(toAddressForPw[0]);
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        if (existingRecipients != null)
                        {
                            if (_appConfiguration["SendEmailFromService"] != null && Convert.ToString(_appConfiguration["SendEmailFromService"]).ToUpper() == "TRUE")
                            {
                                //To Add data in Emailqueue and EmailqueueRecipients for Delegate Electronic Signature  Notification 
                                EmailQueueData emailQueueData = new EmailQueueData();
                                EmailSendInfo emailSendData = new EmailSendInfo();
                                emailQueueData.envelope = envelope;
                                emailQueueData.template = template;
                                emailQueueData.MailMessageBody = mailTemplate;
                                emailQueueData.SignReqReplyToAddressValue = SignReqReplyToAddressValue;

                                emailQueueData.EmailType = Constants.EmailTypes.ESR;
                                emailQueueData.EmailSubject = _envelopeHelperMain.GetEmailSubjectPrefix(cultureInfo, subject, Constants.EmailSubject.ElectronicSignatureVerificationCode);
                                emailSendData.RecipientName = string.Join(",", toDisplayNameForPw);
                                emailSendData.RecipientEmail = string.Join(",", toAddressForPw);
                                emailQueueData.emailSendInfo = emailSendData;
                                _envelopeHelperMain.EmailQueueFunction(emailQueueData);
                            }
                            else
                            {
                                var Chilkat = new ChilkatHelper(_appConfiguration);
                                string eDisplayCode = existingRecipients != null ? envelope.EDisplayCode : "";
                                Chilkat.SendMailUsingChilKet(toAddressForPw, toDisplayNameForPw, null, null, null, null, userprofile.EmailID, userprofile.FullName, _envelopeHelperMain.GetEmailSubjectPrefix(cultureInfo, subject, Constants.EmailSubject.ElectronicSignatureVerificationCode),
                                 mailTemplate, null, null, eDisplayCode, Constants.String.EmailOperation.VerificationCode, SignReqReplyToAddressValue);
                            }
                        }
                        else
                        {
                            if (_appConfiguration["SendEmailFromService"] != null && Convert.ToString(_appConfiguration["SendEmailFromService"]).ToUpper() == "TRUE")
                            {
                                //To Add data in Emailqueue and EmailqueueRecipients for Delegate Electronic Signature  Notification 
                                EmailQueueData emailQueueData = new EmailQueueData();
                                EmailSendInfo emailSendData = new EmailSendInfo();
                                Recipients Senderrecipients = new Recipients();
                                Senderrecipients.Name = userprofile.FirstName;
                                Senderrecipients.EmailAddress = userprofile.EmailID;
                                emailQueueData.template = template;
                                emailQueueData.MailMessageBody = mailTemplate;
                                emailQueueData.SignReqReplyToAddressValue = SignReqReplyToAddressValue;
                                emailQueueData.EmailType = Constants.EmailTypes.ESR;
                                emailQueueData.EmailSubject = _envelopeHelperMain.GetEmailSubjectPrefix(cultureInfo, subject, Constants.EmailSubject.ElectronicSignatureVerificationCode);
                                emailSendData.RecipientName = string.Join(",", toDisplayNameForPw);
                                emailSendData.RecipientEmail = string.Join(",", toAddressForPw);
                                emailQueueData.Signer = Senderrecipients;
                                emailQueueData.emailSendInfo = emailSendData;
                                _envelopeHelperMain.EmailQueueTemplateFunction(emailQueueData);
                            }
                            else
                            {
                                var Chilkat = new ChilkatHelper(_appConfiguration);
                                string eDisplayCode = existingRecipients != null ? envelope.EDisplayCode : "";
                                Chilkat.SendMailUsingChilKet(toAddressForPw, toDisplayNameForPw, null, null, null, null, userprofile.EmailID, userprofile.FullName, _envelopeHelperMain.GetEmailSubjectPrefix(cultureInfo, subject, Constants.EmailSubject.ElectronicSignatureVerificationCode),
                                 mailTemplate, null, null, eDisplayCode, Constants.String.EmailOperation.VerificationCode, SignReqReplyToAddressValue);
                            }
                        }

                    }

                    if (isPwdMobileSend)
                    {
                        loggerModelNew.Message = "Process started for Sending verification code to mobile.";
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        string mobileTemplate = _genericRepository.GetNewMailTemplate(Constants.String.MailTemplateName.FinalSubmitVerificationCode, cultureInfo, EmailDisclaimer, 3, "mobile");
                        mobileTemplate = mobileTemplate.Replace("#SenderName", userprofile.FullName);
                        mobileTemplate = mobileTemplate.Replace("#Password", randomNumber);
                        string eDisplayCode = existingRecipients != null ? envelope.EDisplayCode : "";
                        mobileTemplate = mobileTemplate.Replace("#EnvelopeDisplayCode", eDisplayCode);

                        string subject = existingRecipients != null ? envelope.Subject : template.Subject;
                        if (_appConfiguration["SendEmailFromService"] != null && Convert.ToString(_appConfiguration["SendEmailFromService"]).ToUpper() == "TRUE")
                        {
                            string emailSubject = _envelopeHelperMain.GetEmailSubjectPrefix(cultureInfo, subject, Constants.EmailSubject.ElectronicSignatureVerificationCode);
                            if (existingRecipients != null)
                            {
                                loggerModelNew.Message = "Envelope: Sending message to mobile:" + (existingRecipients.DialCode + existingRecipients.Mobile);
                                _envelopeHelperMain.SendSMSThroughEmailService(envelope, mobileTemplate, SignReqReplyToAddressValue, Constants.EmailTypes.SR, existingRecipients, emailSubject);
                            }
                            else
                            {
                                loggerModelNew.Message = "Template: Sending message to mobile:" + (templateRecipient.DialCode + templateRecipient.Mobile);
                                _envelopeHelperMain.SendTemplateSMSTThroughEmailService(template, mobileTemplate, SignReqReplyToAddressValue, Constants.EmailTypes.SR, (templateRecipient.DialCode + templateRecipient.Mobile), emailSubject, userprofile);
                            }
                            rSignLogger.RSignLogInfo(loggerModelNew);
                        }
                        else
                        {
                            if (existingRecipients != null)
                            {
                                loggerModelNew.Message = "Envelope: Sending message to mobile:" + (existingRecipients.DialCode + existingRecipients.Mobile);
                                string signerMobileNumber = existingRecipients.DialCode + existingRecipients.Mobile;
                                _envelopeHelperMain.SendMobileSMS(envelope, mobileTemplate, signerMobileNumber);
                            }
                            else
                            {
                                loggerModelNew.Message = "Template: Sending message to mobile:" + (templateRecipient.DialCode + templateRecipient.Mobile);
                                string signerMobileNumber = templateRecipient.DialCode + templateRecipient.Mobile;
                                _envelopeHelperMain.SendMobileSMS(null, mobileTemplate, signerMobileNumber, template);
                            }
                            rSignLogger.RSignLogInfo(loggerModelNew);
                        }
                    }

                    if (signerVerificationOTP != null)
                    {
                        loggerModelNew.Message = "Process started for saving signer Verification OTP in table";
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        SignerVerificationOTP signerVerification = new SignerVerificationOTP();
                        signerVerification.ID = Guid.NewGuid();
                        signerVerification.EnvelopeID = signerVerificationOTP.EnvelopeID;
                        signerVerification.EmailAddress = existingRecipients != null ? existingRecipients.EmailAddress : templateRecipient.EmailAddress;
                        signerVerification.VerificationCode = randomNumber;
                        signerVerification.Name = existingRecipients != null ? existingRecipients.Name : templateRecipient.Name;
                        signerVerification.RecipientID = signerVerificationOTP.RecipientID;
                        signerVerification.DialCode = signerVerificationOTP.DialCode;
                        signerVerification.Mobile = signerVerificationOTP.Mobile;
                        signerVerification.CreatedDateTime = DateTime.UtcNow;
                        _recipientRepository.SaveSignerFinalSubmitOTP(signerVerification);

                        responseMessage.VerificationCode = randomNumber;
                        responseMessage.StatusCode = HttpStatusCode.OK;
                        responseMessage.StatusMessage = "OK";
                        responseMessage.Message = "Success";
                        loggerModelNew.Message = "Process completed to create signer Verification Code using API and " + responseMessage.Message;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return Ok(responseMessage);
                    }
                    else
                    {
                        responseMessage.StatusCode = HttpStatusCode.NotFound;
                        responseMessage.Message = "No recipient found";
                        loggerModelNew.Message = "signerVerificationOTP object is null and " + responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.NotFound;
                    responseMessage.Message = "No recipient found";
                    loggerModelNew.Message = "signerVerificationOTP object is null and " + responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.StatusCode = HttpStatusCode.NoContent;
                responseMessage.StatusMessage = "Failed";
                return BadRequest(responseMessage);
            }
        }

        [ProducesResponseType(typeof(ResponseMessageForShortSigningUrl), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("ValidateFinalSubmitOTP/{EnvelopeId}/{RecipientId}/{ValidationCode}")]
        [HttpGet]
        public async Task<IActionResult> ValidateFinalSubmitOTP(string EnvelopeId, string RecipientId, string ValidationCode)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "ValidateFinalSubmitOTP", "Process started for Validate Final Submit OTP using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForShortSigningUrl responseMessage = new ResponseMessageForShortSigningUrl();
            try
            {
                string generatedOtp = _recipientRepository.GetSignerFinalSubmitOTP(EnvelopeId, RecipientId);
                if (!string.IsNullOrEmpty(generatedOtp))
                {
                    if (generatedOtp == ValidationCode)
                    {
                        responseMessage.StatusCode = HttpStatusCode.OK;
                        responseMessage.StatusMessage = "OK";
                        responseMessage.Message = "Success";
                        return Ok(responseMessage);
                    }
                    else
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "Failed";
                        responseMessage.Message = "InvalidCode";
                        return BadRequest(responseMessage);
                    }
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "Failed";
                    responseMessage.Message = "Fail";
                    return BadRequest(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.StatusCode = HttpStatusCode.NoContent;
                responseMessage.StatusMessage = "Failed";
                return BadRequest(responseMessage);
            }
        }
    }
}
