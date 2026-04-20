using Aspose.Pdf.Operators;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using schemas.appliedsystems.com.epic.sdk._2009._07;
using schemas.appliedsystems.com.epic.sdk._2011._01._get;
using System.Net;
using System.ServiceModel;
using System.Web;


namespace RSign.SendAPI.API
{
    public class SendToRSignEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly string _module = "SendToRSignEndPoint";
         private IUserTokenRepository _userTokenRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly ITemplateRepository _templateRepository;
        private IHttpContextAccessor _accessor;
        private readonly IESignHelper _esignHelper;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private ISettingsRepository _settingsRepository;
        private IUserRepository _userRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IIntegrationEnvelope _iIntegrationEnvelope;
        private AuthenticateService _authenticateService;
        private readonly IAsposeHelper _asposeHelper;
        private readonly IIntegrationRepository _integrationRepository;


        public SendToRSignEndPoint(IHttpContextAccessor accessor, ITemplateRepository templateRepository, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration,
           ISettingsRepository settingsRepository, IESignHelper esignHelper, IEnvelopeHelperMain envelopeHelperMain, IUserRepository userRepository, ILookupRepository lookupRepository,
           IIntegrationEnvelope iIntegrationEnvelope, AuthenticateService authenticateService, IAsposeHelper asposeHelper, IIntegrationRepository integrationRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _templateRepository = templateRepository;
            _userTokenRepository = userTokenRepository;
            _settingsRepository = settingsRepository;
            _esignHelper = esignHelper;
            rSignLogger = new RSignLogger(_appConfiguration);
            _envelopeHelperMain = envelopeHelperMain;
            _userRepository = userRepository;
            _lookupRepository = lookupRepository;
            _iIntegrationEnvelope = iIntegrationEnvelope;
            _authenticateService = authenticateService;
            _asposeHelper = asposeHelper;
            _integrationRepository = integrationRepository;


        }
        public void RegisterSendToRSignAPI(WebApplication app)
        {
            app.MapGet("/api/v1/SendToRSign/SendToRSign", SendToRSign);
            app.MapGet("/api/v1/SendToRSign/GetExternalIntegrationSettings", GetExternalIntegrationSettings);
            app.MapPost("/api/v1/SendToRSign/InitializeEnvelope", InitializeEnvelope);           
            app.MapPost("/api/v1/SendToRSign/NetDocsAuthorizeUser", NetDocsAuthorizeUser);
            app.MapPost("/api/v1/SendToRSign/InitializeTemplate", InitializeTemplate);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IResult> SendToRSign(HttpRequest request, [FromQuery] string query)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SendToRSign", "Endpoint Initialized,to SendToRSign details", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            string IntegrationType = string.Empty;
            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    var parameters = QueryStringToDictionary(query);
                    if (query.Contains("area")) { IntegrationType = Constants.StoreageDriveNames.Appliedepic; }
                    else if (query.Contains("netDoc")) { IntegrationType = Constants.StoreageDriveNames.netDocuments; }
                    else if (query.ToLower().Contains("entitytype")) { IntegrationType = Constants.StoreageDriveNames.Bullhorn; }
                    else if (query == "/SendToRsign/SendToRSignIndex") { IntegrationType = Constants.StoreageDriveNames.iManage; }
                    else if (query.ToLower().Contains("tenant")) { IntegrationType = Constants.StoreageDriveNames.Vincere; }
                    else if (query.ToLower().Contains("docids")) { IntegrationType = Constants.StoreageDriveNames.eDOCSInfoCenter; }
                    SendToRSignResponse sendToRSignResponse = SendToRSignDetails(request, query, IntegrationType, userToken.UserID);
                    return Results.Ok(sendToRSignResponse);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SendToRSign";
                loggerModelNew.Message = "SendToRSign API EndPoint - Exception at SendToRSign method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public SendToRSignResponse SendToRSignDetails(HttpRequest request, string query, string IntegrationType, Guid userId)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SendToRSignDetails", "Endpoint Initialized,to get Send To RSign Details", "", "", "", remoteIpAddress, "SendAPI");
            loggerModelNew.Message = "SendToRSignDetails " + IntegrationType + " ==> Start";
            rSignLogger.RSignLogInfo(loggerModelNew);
            SendToRSignResponse sendToRSignResponse = new();
            try
            {
                string userURL = query;
                UserProfile userProfile = _userRepository.GetUserProfile(userId);
                string[] integrationKeys = { "netdoc", "area", "entitytype", "tenant", "docids" };
                userURL = HttpUtility.UrlDecode(userURL.Substring(userURL.IndexOf('?') + 1));
                string[] SplitIntoArray = userURL.Split('&');
                List<Attributes> Documents = new List<Attributes>();
                AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new();
                SettingsExternalIntegration settings = new();
                string SelectedDocumentIds = string.Empty;
                ContactDetail contactDetail = new();
                sendToRSignResponse.integrationType = IntegrationType;
                loggerModelNew.Message = "Integration settings for the integration " + IntegrationType + " ==> Start";
                rSignLogger.RSignLogInfo(loggerModelNew);
                if (IntegrationType == Constants.StoreageDriveNames.Bullhorn || IntegrationType == Constants.StoreageDriveNames.Appliedepic)
                {
                    settings = _settingsRepository.GetExternalSettingsByCompanyId(userProfile.CompanyID, IntegrationType);
                }
                else
                {
                    settings = _settingsRepository.GetExternalSettingsByType(userId, IntegrationType, userProfile.CompanyID);
                    if (IntegrationType == Constants.StoreageDriveNames.Vincere)
                    {
                        var CompanySettings = _settingsRepository.GetExternalSettingsByCompanyId(userProfile.CompanyID, IntegrationType);
                        if (CompanySettings != null)
                        {
                            settings.AppClientId = CompanySettings.AppClientId;
                            settings.AppClientSecret = CompanySettings.AppClientSecret;
                            settings.ApplicationAPIURL = CompanySettings.ApplicationAPIURL;
                            settings.ApplicationURL = CompanySettings.ApplicationURL;
                            settings.RedirectURI = CompanySettings.RedirectURI;
                        }
                    }
                }
                adminGeneralAndSystemSettings.settingsExternalIntegration = settings;
                sendToRSignResponse.adminGeneralAndSystemSettings = adminGeneralAndSystemSettings;
                loggerModelNew.Message = "Integration settings for the integration " + IntegrationType + " ==> End";
                rSignLogger.RSignLogInfo(loggerModelNew);
                if (IntegrationType == Constants.StoreageDriveNames.iManage)
                {
                    sendToRSignResponse.StatusMessage = "Success";
                    sendToRSignResponse.StatusCode = HttpStatusCode.OK;
                    return sendToRSignResponse;
                }
                else if (IntegrationType == Constants.StoreageDriveNames.Appliedepic)
                {

                    loggerModelNew.Message = IntegrationType + "Logic starts" + userProfile.UserID;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    try
                    {
                        loggerModelNew.Message = IntegrationType + "Try block starts" + userProfile.FirstName;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        sendToRSignResponse = _integrationRepository.GetAppliedEpicDocumentsContacts(IntegrationType, sendToRSignResponse, userProfile, SplitIntoArray, settings);
                        if (sendToRSignResponse.StatusCode != HttpStatusCode.OK)
                        {
                            sendToRSignResponse.StatusMessage = "The Applied Epic feature requires configuration setup at the company level. Please contact your company administrator to set up the required configuration to enable this feature.";
                            sendToRSignResponse.StatusCode = HttpStatusCode.BadRequest;
                            loggerModelNew.Message = "Company level Settings were not set with correct values" + userProfile.CompanyID;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            return sendToRSignResponse;
                        }
                        loggerModelNew.Message = IntegrationType + "Logic ends" + userProfile.FirstName;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return sendToRSignResponse;
                    }
                    catch (Exception ex)
                    {
                        loggerModelNew.Message = ex.Message + " Inner Exception :" + ex.InnerException;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        SessionHelper.Set(SessionKey.DisplayMessage, "The Applied Epic setup at company level not configured properly.  Please contact your company administrator to setup the required configuration to enable the feature.");
                        SessionHelper.Set(SessionKey.MessageType, MessageType.Warning);
                        sendToRSignResponse.StatusMessage = "The Applied Epic setup at company level not configured properly.  Please contact your company administrator to setup the required configuration to enable the feature.";
                        sendToRSignResponse.StatusCode = HttpStatusCode.BadRequest;
                        return sendToRSignResponse;
                    }

                }
                else if (IntegrationType == Constants.StoreageDriveNames.netDocuments)
                {
                    return _integrationRepository.GetnetDocumentsDocumentsInfo(sendToRSignResponse, SplitIntoArray, Documents, settings, ref SelectedDocumentIds);
                    
                }
                else if (IntegrationType == Constants.StoreageDriveNames.Vincere)
                {
                    return _integrationRepository.GetVincereContact(sendToRSignResponse, SplitIntoArray, settings, contactDetail, userProfile);
                }
                else if (IntegrationType == Constants.StoreageDriveNames.Bullhorn)
                {
                    return _integrationRepository.GetBullhornContact(IntegrationType, sendToRSignResponse, SplitIntoArray, settings, userProfile, ref contactDetail);
                }
                else if (IntegrationType == Constants.StoreageDriveNames.eDOCSInfoCenter)
                {
                    if (string.IsNullOrEmpty(settings.AccessToken))
                    {
                        SessionHelper.Set(SessionKey.DisplayMessage, "The eDOCS InfoCenter setup at personal level not configured properly.  Please setup the required configuration at personal settings to enable the feature.");
                        SessionHelper.Set(SessionKey.MessageType, MessageType.Warning);
                        sendToRSignResponse.StatusMessage = "The eDOCS InfoCenter setup at personal level not configured properly.  Please setup the required configuration at personal settings to enable the feature.";
                        sendToRSignResponse.StatusCode = HttpStatusCode.BadRequest;
                        return sendToRSignResponse;
                    }
                    var response = _integrationRepository.GeteDocsDocumentDetails(IntegrationType, SplitIntoArray, sendToRSignResponse, settings, ref SelectedDocumentIds);
                    response.StatusMessage = "Success";
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
                sendToRSignResponse.StatusMessage = "Success";
                sendToRSignResponse.StatusCode = HttpStatusCode.OK;
                loggerModelNew.Message = "SendToRSignDetails " + IntegrationType + " ==> Start";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return sendToRSignResponse;
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SendToRSign";
                loggerModelNew.Message = "SendToRSign API EndPoint - Exception at SendToRSign method and error message is:" + ex.ToString();
                sendToRSignResponse.StatusMessage = "Failure : Unable to get the information";
                sendToRSignResponse.StatusCode = HttpStatusCode.BadRequest;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return sendToRSignResponse;
            }
        }       

        public async Task<IResult> InitializeEnvelope(HttpRequest request, [FromBody] UploadIntegrationFiles DocumentDetails)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SendToRSign", "Endpoint Initialized,to InitializeEnvelope details", "", "", "", remoteIpAddress, "SendAPI");
            loggerModelNew.Module = _module;
            loggerModelNew.Method = "InitializeEnvelope";
            rSignLogger.RSignLogInfo(loggerModelNew);

            string IntegrationType = string.Empty;
            UploadIntegrationFiles DocumentDetailsDummy = new();
            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            APIEnvelopeResponse aPIEnvelopeResponse = new();

            SendToRSignResponse sendToRSignResponse = new();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    if (DocumentDetails != null)
                    {
                        UserProfile userProfile = _userRepository.GetUserProfile(userToken.UserID);

                        APIEnvelopeRequest tData = await PrepareInitializeEnvelopeWithDetailsObjectAsync(userProfile, DocumentDetails, remoteIpAddress);

                        string reqHostValue = request.Scheme + "://" + request.Host + "/";
                        string domainUrlType = EnvelopeHelper.GetSourceUrlType(reqHostValue);
                        string senderAddress = userToken.EmailId ?? string.Empty;
                        return Results.Ok(_iIntegrationEnvelope.InitializeEnvelopeWithDetails(userToken, senderAddress, tData, remoteIpAddress, domainUrlType));
                    }
                    else
                    {
                        loggerModelNew.Message = "InitializeEnvelope API EndPoint - Exception at InitializeEnvelope method";
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        aPIEnvelopeResponse.StatusCode = HttpStatusCode.BadGateway;
                        return Results.BadRequest(aPIEnvelopeResponse);
                    }
                }
            }
            catch (Exception ex)
            {

                loggerModelNew.Message = "SendToRSign API EndPoint - Exception at InitializeEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<APIEnvelopeRequest> PrepareInitializeEnvelopeWithDetailsObjectAsync(UserProfile userProfile, UploadIntegrationFiles DocumentDetails, string remoteIpAddress)
        {
            APIEnvelopeRequest apiEnvelopeRequest = new();
            loggerModelNew = new LoggerModelNew("", _module, "SendToRSign", "method start Initialize Envelope Prepare Object InitializeEnvelope details", "", "", "", remoteIpAddress, "SendAPI");
            loggerModelNew.Module = _module;
            loggerModelNew.Method = "InitializeEnvelope";
            rSignLogger.RSignLogInfo(loggerModelNew);

            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new();
            SettingsExternalIntegration settings = new();
            if (DocumentDetails.IntegrationType == Constants.StoreageDriveNames.Bullhorn || DocumentDetails.IntegrationType == Constants.StoreageDriveNames.Appliedepic)
            {
                settings = _settingsRepository.GetExternalSettingsByCompanyId(userProfile.CompanyID, DocumentDetails.IntegrationType);
            }
            else
            {
                settings = _settingsRepository.GetExternalSettingsByType(userProfile.UserID, DocumentDetails.IntegrationType, userProfile.CompanyID);
            }
            adminGeneralAndSystemSettings.settingsExternalIntegration = settings;
            string Base64 = null;
            string FolderId = string.Empty;
            string Token = string.Empty;
            string customerId = Convert.ToInt32(settings.CustomerId).ToString();
            string ServerURL = settings.ServerURL;
            string isUploadSignedDocument = settings.IsUploadSignedDocument;
            int? filingFinalSignedDocuments = settings.FilingFinalSignedDocuments;
            UploadExternalDocument uploadExternalDocument = new();
            HttpResponseMessage responseTemp = new();
            AdminGeneralAndSystemSettings userSettings = new();
            UserAdditionalResponseMessage userAdditionalResponseMessage = new();
            SettingResponseMessage userSettingsResponseMessage = new();
            SettingResponseMessage companySettingsResponseMessage = new();
            AdminGeneralAndSystemSettings companySettings = new();
            UserSettingsModel userSettingsModel = new();
            userSettingsModel.CompanyId = Guid.Empty;
            userSettingsModel.Email = userProfile.EmailID;
            string responseToken = string.Empty;
            //UserProfile userprofile = new();
            userAdditionalResponseMessage = _authenticateService.GetUserCompanySettings(userSettingsModel, userProfile, responseToken, userSettingsResponseMessage, companySettingsResponseMessage, userSettings, companySettings);
            userSettings = userAdditionalResponseMessage.UserSettings;
            userSettings.SelectedTimeZone = string.IsNullOrEmpty(userSettings.SelectedTimeZone) ? "UTC" : userSettings.SelectedTimeZone;
            if (DocumentDetails != null && DocumentDetails.Stage == Constants.String.RSignStage.InitializeEnvelope)
            {

                APIEnvelopeRequest apiEnvDetails = new APIEnvelopeRequest();

                apiEnvDetails.TemplateCode = 0;
                APIEnvelopeDocumentRequest documentUpload = new APIEnvelopeDocumentRequest();
                apiEnvDetails.Documents = new List<APIEnvelopeDocumentRequest>();
                apiEnvDetails.Recipients = new List<APIEnvelopeRecipientRequest>();
                apiEnvDetails.IsPrepare = false;
                apiEnvDetails.IsEnableFileReview = companySettings != null ? (companySettings.EnableFileReview.HasValue == true ? companySettings.EnableFileReview : false) : false;
                apiEnvDetails.ReminderDays = 0;
                apiEnvDetails.ReminderRepeatDays = 0;
                apiEnvDetails.PasswordRequiredToSign = false;
                apiEnvDetails.PasswordRequiredToOpen = false;
                apiEnvDetails.IsTransparencyDocReq = userSettings.IncludeTransparencyDoc;
                apiEnvDetails.IsSignerAttachFileReq = userSettings.IncludeSignerAttachFile == Constants.SignerAttachmentOptions.EnableAttachmentRequest ? true : false;
                apiEnvDetails.IsSignerAttachFileReqNew = userSettings.IncludeSignerAttachFile;
                apiEnvDetails.IsSeparateMultipleDocumentsAfterSigningRequired = userSettings.SeparateMultipleDocumentsAfterSigning;
                apiEnvDetails.IsAttachXMLDataReq = userSettings.IncludeAddXMLData;
                apiEnvDetails.IsRandomPassword = false;
                apiEnvDetails.IsPasswordMailToSigner = true;
                apiEnvDetails.CultureInfo = Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();           /* Optional  default is en-us*/
                apiEnvDetails.IsDisclaimerInCertificate = userSettings.IsDisclaimerInCertificate;
                apiEnvDetails.IsEnableFileReview = userSettings.EnableFileReview;
                apiEnvDetails.IsStoreOriginalDocument = userSettings.StoreOriginalDocument;
                apiEnvDetails.IsPrivateMode = (userSettings.PrivateModeSettingsValues != null && userSettings.PrivateModeSettingsValues.Count > 1 ? (userSettings.PrivateModeSettingsValues[0].Values.OptionValue == "False" ? false : true) : false);
                apiEnvDetails.PostSigningLandingPage = userSettings.PostSigningPageUrl;
                apiEnvDetails.CreatedSource = Common.Helpers.Common.GetCreatedSource("webapp", true);
                apiEnvDetails.IsAdditionalAttmReq = userSettings.AllowRecipeintToAttachFile;
                apiEnvDetails.ReminderDays = userSettings.SendReminderIn;
                apiEnvDetails.ReminderRepeatDays = userSettings.ThenSendReminderIn;
                apiEnvDetails.ReminderTypeID = userSettings.SendReminderInDropdownSelected;
                apiEnvDetails.ThenReminderTypeID = userSettings.ThenSendReminderInDropdownSelected;
                apiEnvDetails.FinalReminderDays = userSettings.SendFinalReminderBeforeExp;
                apiEnvDetails.FinalReminderTypeID = userSettings.SendFinalReminderBeforeExpDropdownSelected;
                apiEnvDetails.IsSequenceCheck = userSettings.SignInSequence;
                apiEnvDetails.IsSeparateMultipleDocumentsAfterSigningRequired = userSettings.SeparateMultipleDocumentsAfterSigning;
                apiEnvDetails.IsFinalDocLinkReq = userSettings.StoredSignedPDF;
                apiEnvDetails.SendIndividualSignatureNotifications = userSettings.SendIndividualSignatureNotifications;
                apiEnvDetails.IsPasswordMailToSigner = userSettings.IsAccessCodeSendToSignerEnabled;
                apiEnvDetails.AccessAuthType = userSettings.AccessAuthenticationId;
                apiEnvDetails.DateFormatID = userSettings.DateFormatID;
                apiEnvDetails.ExpiryTypeID = userSettings.ExpiresInID;
                apiEnvDetails.EnableCcOptions = userSettings.EnableCcOptions;
                apiEnvDetails.EnableRecipientLanguage = userSettings.EnableRecipientLanguageSelection;
                apiEnvDetails.IntegrationURL = DocumentDetails == null ? string.Empty : DocumentDetails.IntegrationURL;
                if (userSettings.AccessAuthenticationId == Constants.ConfigurationalProperties.PasswordType.Endtoend)
                {
                    apiEnvDetails.PasswordToOpen = userSettings.AccessPassword;
                    apiEnvDetails.PasswordToSign = userSettings.AccessPassword;
                    apiEnvDetails.PasswordRequiredToOpen = true;
                    apiEnvDetails.PasswordRequiredToSign = true;
                    apiEnvDetails.IsPasswordMailToSigner = userSettings.IsAccessCodeSendToSignerEnabled ? true : false;
                    apiEnvDetails.IsRandomPassword = userSettings.AccessPassword == "" ? true : false;
                }
                else if (userSettings.AccessAuthenticationId == Constants.ConfigurationalProperties.PasswordType.RequiredToOpenSigned)
                {
                    apiEnvDetails.PasswordToOpen = userSettings.AccessPassword;
                    apiEnvDetails.PasswordRequiredToOpen = true;
                    apiEnvDetails.IsPasswordMailToSigner = userSettings.IsAccessCodeSendToSignerEnabled ? true : false;
                    apiEnvDetails.IsRandomPassword = userSettings.AccessPassword == "" ? true : false;
                    apiEnvDetails.PasswordRequiredToSign = false;
                    apiEnvDetails.PasswordToSign = string.Empty;
                }
                else if (userSettings.AccessAuthenticationId == Constants.ConfigurationalProperties.PasswordType.SignerIdentity)
                {
                    apiEnvDetails.PasswordRequiredToOpen = false;
                    apiEnvDetails.PasswordToOpen = string.Empty;
                    apiEnvDetails.PasswordToOpen = null;
                    apiEnvDetails.PasswordRequiredToSign = false;
                    apiEnvDetails.PasswordToSign = string.Empty;
                }
                else
                {
                    apiEnvDetails.PasswordRequiredToSign = false;
                    apiEnvDetails.PasswordRequiredToOpen = false;
                    apiEnvDetails.PasswordToSign = string.Empty;
                    apiEnvDetails.PasswordToOpen = string.Empty;
                }
                apiEnvDetails.AppliedEpicEntityId = DocumentDetails.AppliedEpicEntityId;
                apiEnvDetails.AppliedEpicUser = DocumentDetails.AppliedEpicUser;
                apiEnvDetails.EntityType = DocumentDetails.EntityType;
                apiEnvDetails.IntegrationType = DocumentDetails.IntegrationType;
                apiEnvDetails.IntegrationURL = DocumentDetails.IntegrationURL;


                foreach (var con in DocumentDetails.Recipients)
                {
                    APIEnvelopeRecipientRequest recipientRequest = new APIEnvelopeRecipientRequest();
                    recipientRequest.Email = con.Email;
                    recipientRequest.Name = con.Name;
                    recipientRequest.RecipientType = "signer";
                    apiEnvDetails.Recipients.Add(recipientRequest);
                }
                if (DocumentDetails.sendToRSignDocslist != null)
                {
                    foreach (var Docs in DocumentDetails.sendToRSignDocslist)
                    {
                        string DownloadImanageUrl = Docs.Iwl;
                        string authToken = "/" + DocumentDetails.oauthToken;
                        string documentId = Docs.DocumentId;
                        APIEnvelopeDocumentRequest document = new APIEnvelopeDocumentRequest();
                        byte[] docbytes = null;
                        string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                        foreach (char c in invalidChars)
                        {
                            Docs.DocumentName = Docs.DocumentName.Replace(c.ToString(), "-");
                        }
                        EnvelopeDocumentResult result = new();
                        if (DocumentDetails.IntegrationType == Constants.UploadDriveType.iManage)
                        {
                            result = await _integrationRepository.iManageEnvelopeDocumentsAsync(settings, customerId, isUploadSignedDocument, filingFinalSignedDocuments, uploadExternalDocument, Docs, documentId);
                        }
                        else if (DocumentDetails.IntegrationType == Constants.UploadDriveType.Appliedepic)
                        {
                            result = AppliedEpicEnvelopeDocuments(settings, documentId, Docs);

                            if (result.Success)
                            {
                                string base64 = result.Base64;
                                string folderId = result.FolderId;
                                byte[] fileBytes = result.FileBytes;
                                document.SourceLocation = folderId;
                                document.SourceDocumentId = Docs.DocumentId;
                            }
                            else
                            {
                                // Handle error (e.g., log downloadResult.ErrorMessage)
                            }
                        }
                        else if (DocumentDetails.IntegrationType == Constants.UploadDriveType.netDocuments)
                        {
                            result = _integrationRepository.NetDocsEnvelopeDocuments(settings, Docs, string.Empty);

                            if (result.Success)
                            {
                                string base64 = result.Base64;
                                string folderId = result.FolderId;
                                byte[] fileBytes = result.FileBytes;
                                document.SourceLocation = folderId;
                                document.SourceDocumentId = Docs.DocumentId;
                            }
                            else
                            {
                                // Handle error (e.g., log downloadResult.ErrorMessage)
                            }
                        }
                        if (DocumentDetails.IntegrationType == Constants.UploadDriveType.eDOCSInfoCenter)
                        {
                            result = _integrationRepository.eDocsEnvelopeDocuments(settings, Docs, string.Empty);
                        }
                        document.Name = Docs.DocumentName + "." + Docs.DocumentType;
                        document.DocumentBase64Data = result.Base64;
                        document.FileExtension = Docs.DocumentType;
                        GetFileCount fileinfo = new GetFileCount();
                        fileinfo.DocumentName = Docs.DocumentName;
                        fileinfo.DocumentType = Docs.DocumentType;
                        fileinfo.DocumentContent = result.Base64;

                        bool isValid = GetFileValidOrNot(fileinfo);
                        if (!isValid)
                        {
                            //return Json(new InfoResult { success = false, message = "Uploaded document is Password Protected, Please upload another document to Proceed", returnUrl = null, data = null }, JsonRequestBehavior.AllowGet);

                        }
                        document.DocumentSource = DocumentDetails.IntegrationType;
                        document.SourceLocation = result.FolderId;
                        document.SourceDocumentId = Docs.DocumentId;
                        document.IntegrationLibrary = DocumentDetails.IntegrationType == Constants.StoreageDriveNames.eDOCSInfoCenter ? settings.DefaultRepository : Docs.lib;
                        document.IsDocumentUploadedToSource = false;
                        apiEnvDetails.Documents.Add(document);
                    }
                }
                apiEnvelopeRequest = apiEnvDetails;
            }
            loggerModelNew = new LoggerModelNew("", _module, "SendToRSign", "method end Initialize Envelope Prepare Object InitializeEnvelope details", "", "", "", remoteIpAddress, "SendAPI");
            loggerModelNew.Module = _module;
            loggerModelNew.Method = "InitializeEnvelope";
            rSignLogger.RSignLogInfo(loggerModelNew);
            return apiEnvelopeRequest;
        }
        public EnvelopeDocumentResult AppliedEpicEnvelopeDocuments(SettingsExternalIntegration settings, string documentId, ExternalDocumentslist docs)
        {
            var result = new EnvelopeDocumentResult();
            loggerModelNew = new LoggerModelNew("", _module, "SendToRSign", "method end Applied Epic Envelope Documents", "", "", "", "", "SendAPI");
            loggerModelNew.Module = _module;
            loggerModelNew.Method = "AppliedEpicEnvelopeDocuments";
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                string sURL = settings.ApplicationAPIURL;
                string sDatabaseName = settings.DefaultRepository;
                string sAuthenticationKey = settings.AppClientSecret;

                var oBinding = new BasicHttpBinding();
                var oAttachmentBinding = new BasicHttpBinding();

                if (!string.IsNullOrEmpty(sURL) && sURL.ToUpper().Contains("HTTPS"))
                {
                    oBinding.Security.Mode = BasicHttpSecurityMode.Transport;
                    oAttachmentBinding.Security.Mode = BasicHttpSecurityMode.Transport;
                }

                oAttachmentBinding.MessageEncoding = WSMessageEncoding.Mtom;

                var oEndpointAddress = new EndpointAddress($"{sURL}/v2021_02");
                var oAttachmentEndpointAddress = new EndpointAddress($"{sURL}/Attachments");

                var oHeader = new MessageHeader
                {
                    AuthenticationKey = sAuthenticationKey,
                    DatabaseName = sDatabaseName,
                };

                var oService = new EpicSDK_2021_02Client(oBinding, oEndpointAddress);
                var oAttachmentService = new EpicSDKFileTransferClient(oAttachmentBinding, oAttachmentEndpointAddress);

                int iAttachmentID = Convert.ToInt32(documentId);

                var attachmentResponse = oService.Get_Attachment(oHeader, new AttachmentFilter
                {
                    AttachmentID = iAttachmentID
                }, new AttachmentSorting(), 0, 1);

                var oAttachment = attachmentResponse.Attachments.FirstOrDefault();

                if (oAttachment == null)
                {
                    loggerModelNew.Message = "Downloading document content from the attachment Id " + iAttachmentID + " ==> Document not downloaded from SDK method.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    throw new Exception("Attachment not found.");
                }

                oBinding.MaxReceivedMessageSize = int.MaxValue;
                oBinding.MaxBufferSize = int.MaxValue;
                oAttachmentBinding.MaxReceivedMessageSize = int.MaxValue;
                oAttachmentBinding.MaxBufferSize = int.MaxValue;
                loggerModelNew.Message = "Downloading document content from the attachment Id " + iAttachmentID + " ==> Start";
                rSignLogger.RSignLogInfo(loggerModelNew);
                using (Stream oMemStream = oAttachmentService.Download_Attachment_File(oHeader, iAttachmentID))
                using (var memoryStream = new MemoryStream())
                {
                    oMemStream.CopyTo(memoryStream);
                    result.FileBytes = memoryStream.ToArray();
                }
                result.Base64 = Convert.ToBase64String(result.FileBytes);
                result.FolderId = oAttachment.Folder;
                result.SourceLocation = oAttachment.Folder;
                result.SourceDocumentId = docs.DocumentId;
                loggerModelNew.Message = "Downloading document content from the attachment Id " + iAttachmentID + " ==> End";
                rSignLogger.RSignLogInfo(loggerModelNew);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                loggerModelNew.Message = "Downloading document content from Exception  : " + ex.ToString() + " ==> End";
                rSignLogger.RSignLogInfo(loggerModelNew);
            }

            return result;
        }

        public SettingsExternalIntegration GetExternalIntegrationSettings(HttpRequest request, string IntegrationType)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SendToRSign", "Endpoint Initialized,to Get External Integration Settings details", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            string URL = string.Empty;
            SettingsExternalIntegration settingsExternalIntegration = new();
            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return settingsExternalIntegration;
                }
                else
                {
                    return _settingsRepository.GetExternalSettingsByType(userToken.UserID, IntegrationType, userToken.UserID);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SendToRSign";
                loggerModelNew.Message = "SendToRSign API EndPoint - Exception at GetExternalIntegrationSettings method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return settingsExternalIntegration;
            }
        }

        public async Task<IResult> NetDocsAuthorizeUser(HttpRequest request, [FromQuery] string query)
        {
            IntegrationsAuthorizeUserPOCO authorizeUser = JsonConvert.DeserializeObject<IntegrationsAuthorizeUserPOCO>(query);
            var currentMethodAndParams = $"NetDocsAuthorizeUser:Code={authorizeUser.code}, source={authorizeUser.source}";
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "NetDocsAuthorizeUser", "Process started for Authorizing the  using API.", "", "", "", remoteIpAddress, "SendAPI");
            loggerModelNew.Message = "Process started for " + currentMethodAndParams;
            rSignLogger.RSignLogInfo(loggerModelNew);
            SendToRSignResponse responseMessage = new();

            try
            {
                loggerModelNew.Message = "NetDocsAuthorizeUser ==>END ";
                rSignLogger.RSignLogInfo(loggerModelNew);
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessage.StatusMessage = "UnauthorizedAccess";
                    loggerModelNew.Message = "NetDocsAuthorizeUser ==>UnauthorizedAccess ";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    UserProfile userProfile = _userRepository.GetUserProfile(userToken.UserID);

                    SettingsExternalIntegration companyextSettings = _settingsRepository.GetExternalSettingsByCompanyId(userProfile.CompanyID, Constants.UploadDriveType.netDocuments);
                    SettingsExternalIntegration personalextSettings = _settingsRepository.GetExternalSettingsByType(userProfile.UserID, Constants.UploadDriveType.netDocuments, userProfile.CompanyID);

                    responseMessage = _integrationRepository.GetnetDocsOAuthToken(companyextSettings, personalextSettings, authorizeUser, userProfile);

                    loggerModelNew.Message = "NetDocsAuthorizeUser ==>END ";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "NetDocsAuthorizeUser ==>Exception " + ex.ToString();
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }
        private Dictionary<string, string> QueryStringToDictionary(string query)
        {
            return query.Split('&')
                        .Select(part => part.Split('='))
                        .Where(pair => pair.Length == 2)
                        .ToDictionary(pair => pair[0], pair => Uri.UnescapeDataString(pair[1]));
        }
        public bool GetFileValidOrNot(GetFileCount FileInfo)
        {
            try
            {
                if (!Directory.Exists(_appConfiguration["TempDirectory"]))
                    Directory.CreateDirectory(_appConfiguration["TempDirectory"]);
                System.IO.File.WriteAllBytes(Path.Combine(_appConfiguration["TempDirectory"], FileInfo.DocumentName + "." + FileInfo.DocumentType), Convert.FromBase64String(FileInfo.DocumentContent));
                int DocumentStatus = 0;
                DocumentStatus = _asposeHelper.GetDocumentPageCount(Path.Combine(_appConfiguration["TempDirectory"], FileInfo.DocumentName + "." + FileInfo.DocumentType), _appConfiguration["TempDirectory"]);


                if (System.IO.File.Exists(Path.Combine(_appConfiguration["TempDirectory"], FileInfo.DocumentName + "." + FileInfo.DocumentType)))
                    System.IO.File.Delete(Path.Combine(_appConfiguration["TempDirectory"], FileInfo.DocumentName + "." + FileInfo.DocumentType));

                return DocumentStatus > 1 ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IResult> InitializeTemplate(HttpRequest request, [FromBody] UploadIntegrationFiles DocumentDetails)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SendToRSign", "Endpoint Initialized,to InitializeTemplate details", "", "", "", remoteIpAddress, "SendAPI");
            loggerModelNew.Module = _module;
            loggerModelNew.Method = "InitializeTemplate";
            rSignLogger.RSignLogInfo(loggerModelNew);

            string IntegrationType = string.Empty;
            UploadIntegrationFiles DocumentDetailsDummy = new();
            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            APIEnvelopeResponse aPIEnvelopeResponse = new();

            SendToRSignResponse sendToRSignResponse = new();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    if (DocumentDetails != null)
                    {
                        UserProfile userProfile = _userRepository.GetUserProfile(userToken.UserID);

                        APIEnvelopeRequest tData = new();
                        tData.AppliedEpicUser = DocumentDetails.AppliedEpicUser;
                        tData.AppliedEpicEntityId = DocumentDetails.AppliedEpicEntityId;
                        tData.EntityType = DocumentDetails.EntityType;
                        tData.IntegrationType = DocumentDetails.IntegrationType;
                        tData.IntegrationURL = DocumentDetails.IntegrationURL;
                        string domainUrlType = "2";
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);                       
                        string senderAddress = userToken.EmailId ?? string.Empty;
                        return Results.Ok(_iIntegrationEnvelope.InitializeTemplateWithDetails(userToken, senderAddress, tData, remoteIpAddress, domainUrlType));
                    }
                    else
                    {
                        loggerModelNew.Message = "InitializeTemplate API EndPoint - Exception at InitializeTemplate method";
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        aPIEnvelopeResponse.StatusCode = HttpStatusCode.BadGateway;
                        return Results.BadRequest(aPIEnvelopeResponse);
                    }
                }
            }
            catch (Exception ex)
            {

                loggerModelNew.Message = "SendToRSign API EndPoint - Exception at InitializeTemplate method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }


    }
}

