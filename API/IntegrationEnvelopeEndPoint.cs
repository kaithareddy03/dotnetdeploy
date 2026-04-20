using Aspose.Pdf.Operators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Models;
using RSign.Models.Repository;
using RSign.Notification;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using schemas.appliedsystems.com.epic.sdk._2009._07;
using schemas.appliedsystems.com.epic.sdk._2009._07._get;
using System.ServiceModel;
using Azure.Core;
using System.Net.Sockets;
using schemas.appliedsystems.com.epic.sdk._2011._01._account._policy._commercialapplication._applicant;
using static RSign.Common.Helpers.Constants;

namespace RSign.SendAPI.API
{
    public class IntegrationEnvelopeEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly string _module = "IntegrationEnvelopeEndPoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly IIntegrationEnvelope _iIntegrationEnvelope;
        private IAuthenticateRepository _authenticateRepository;
        private AuthenticateService _authenticateService;
        private RpostRestService _rpostRestService;
        private ISettingsRepository _settingsRepository;
        private IIntegrationRepository _integrationRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserRepository _userRepository;

        public IntegrationEnvelopeEndPoint(IHttpContextAccessor accessor, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration, IAuthenticateRepository authenticateRepository, IIntegrationEnvelope iIntegrationEnvelope,
            AuthenticateService authenticateService, ISettingsRepository settingsRepository, IIntegrationRepository integrationRepository, IUserRepository userRepository, IHttpClientFactory httpClientFactory)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _userTokenRepository = userTokenRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _iIntegrationEnvelope = iIntegrationEnvelope;
            _authenticateRepository = authenticateRepository;
            _authenticateService = authenticateService;
            _settingsRepository = settingsRepository;
            _rpostRestService = new RpostRestService(_appConfiguration);
            _integrationRepository = integrationRepository;
            _httpClientFactory = httpClientFactory;
            _userRepository = userRepository;
        }
        public void RegisterIntegrationEnvelopeAPI(WebApplication app)
        {
            app.MapPost("/api/v1/Envelope/InitializeEnvelopeWithDetails", InitializeEnvelopeWithDetails);
            app.MapPost("/api/v1/Envelope/SFInitializeEnvelopeWithDetails", SFInitializeEnvelopeWithDetails);
            app.MapPost("/api/v1/Auth/IManageAuthorizeUser", IManageAuthorizeUser);
            app.MapPost("/api/v1/Auth/VincereAuthorizeUser", VincereAuthorizeUser);
            app.MapPost("/api/v1/Envelope/GetIntegrationContacts", GetIntegrationContacts);
            app.MapGet("/api/v1/Envelope/CheckIntegrationToken", CheckIntegrationToken);
            app.MapPost("/api/v1/Settings/DeRegisterUserAccount", DeRegisterUserAccount);
            app.MapPost("/api/v1/Settings/OpenIManageFolderPickerAsync", OpenIManageFolderPickerAsync);
        }
        /// <summary>
        /// InitializeEnvelopeWithDetails - Initializing a new envelope using API
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tData"></param>
        /// <returns></returns>
        public async Task<IResult> InitializeEnvelopeWithDetails(HttpRequest request, APIEnvelopeRequest tData)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "InitializeEnvelopeWithDetails", "Process started for Initializing a new envelope using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            //string str = Convert.ToString(tData.PasswordToSign);
            try
            {
                loggerModelNew.Message = "Validating user token...";
                rSignLogger.RSignLogInfo(loggerModelNew);

                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                loggerModelNew.Module = _module;
                loggerModelNew.Message = "User token validated successfully for UserID= " + userToken.UserID + ". Proceeding with envelope initialization.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                string reqHostValue = request.Scheme + "://" + request.Host + "/";
                string domainUrlType = EnvelopeHelper.GetSourceUrlType(reqHostValue);


                loggerModelNew.Module = _module;
                loggerModelNew.Message = "Request host: " + reqHostValue + ", Domain type identified as: " + domainUrlType + " and started extracting sender address from GetSenderAddressFromHeaders method.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                string senderAddress = _userTokenRepository.GetSenderAddressFromHeaders(request, "SenderAddress");

                loggerModelNew.Module = _module;
                loggerModelNew.Message = (string.IsNullOrEmpty(senderAddress) ? "Sender address header is missing or empty." : "Sender address extracted successfully : " + senderAddress) + " Initiated a Call to InitializeEnvelopeWithDetails...";
                rSignLogger.RSignLogInfo(loggerModelNew);

                var result = await _iIntegrationEnvelope.InitializeEnvelopeWithDetails(userToken, senderAddress, tData, remoteIpAddress, domainUrlType);

                loggerModelNew.Module = _module;
                loggerModelNew.Message = "Successfully initialized envelope and received response from _iIntegrationEnvelope.InitializeEnvelopeWithDetails.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "InitializeEnvelopeWithDetails";
                loggerModelNew.Message = "API EndPoint - Exception at InitializeEnvelopeWithDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        [System.Web.Http.AllowAnonymous]
        public async Task<IResult> IManageAuthorizeUser(HttpRequest request, [FromBody] IntegrationsAuthorizeUserPOCO authorizeUser)
        {
            string currentMethodAndParams = $"AuthorizeUser:Code={authorizeUser.code}, source={authorizeUser.source}";
            loggerModelNew = new LoggerModelNew("", "iManage Auto Login", currentMethodAndParams, "Process started for AuthorizeUser method", "", "", "", "", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                loggerModelNew = new LoggerModelNew("", "Account", "AuthorizeUser", "iManage Redirect Action method", "", "");
                loggerModelNew.Message = "Method IManageAuthorizeUser --> start";
                rSignLogger.RSignLogInfo(loggerModelNew);
                UserToken userToken = new();
                bool _tokenValidated = false;
                string myToken = string.Empty;

                try
                {
                    loggerModelNew.Message = "Attempting to validate user token from request headers.";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    userToken = _userTokenRepository.ValidateToken(request);
                    if (userToken != null)
                    {
                        _tokenValidated = true;
                        myToken = userToken != null ? (userToken.AuthToken ?? string.Empty) : string.Empty;
                        loggerModelNew.Module = _module;
                        loggerModelNew.Message = "User token validated successfully for UserID = " + userToken.UserID + ". Token: " + (!string.IsNullOrEmpty(myToken) ? "Available" : "Empty");
                        rSignLogger.RSignLogInfo(loggerModelNew);
                    }
                    else
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Message = "User token validation failed. Token is null or invalid.";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                       // return Results.BadRequest(new { Error = "User token validation failed from iManage." });
                    }
                }
                catch (Exception ex)
                {
                    _tokenValidated = false;
                    loggerModelNew.Message = "Exception occurred while validating token. Proceeding with _tokenValidated=false. Exception: " + ex.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                }

                loggerModelNew.Message = "Initiating ValidateIManageAuthenticationAsync call with token validation status: " + _tokenValidated;
                rSignLogger.RSignLogInfo(loggerModelNew);

                AuthenticateResponseMessageModel responseMessage = await ValidateIManageAuthenticationAsync(request, userToken, authorizeUser, _tokenValidated);

                loggerModelNew.Module = _module;
                loggerModelNew.Message = "Method IManageAuthorizeUser --> end";
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SendToRSign controller AuthorizeUser method. " + ex.Message.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, false);
                return Results.BadRequest(new { Error = "Problem with user authentication. at global catch" });

            }
        }

        [System.Web.Http.AllowAnonymous]
        public async Task<IResult> VincereAuthorizeUser(HttpRequest request, [FromBody] IntegrationsAuthorizeUserPOCO authorizeUser)
        {

            string currentMethodAndParams = $"AuthorizeUser:Code={authorizeUser.code}, source={authorizeUser.source}";
            loggerModelNew = new LoggerModelNew("", "Vincere Autorize User", currentMethodAndParams, "Process started for VincereAuthorizeUser method", "", "", "", "", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            SendToRSignResponse responseMessage = new();
            try
            {
                loggerModelNew = new LoggerModelNew("", "Account", "VincereAuthorizeUser", "VincereAuthorizeUser Redirect Action method", "", "");
                loggerModelNew.Message = "Method VincereAuthorizeUser --> start";
                rSignLogger.RSignLogInfo(loggerModelNew);
                UserToken userToken = new();
                bool _tokenValidated = false;
                string myToken = string.Empty;

                try
                {
                    loggerModelNew.Message = "Attempting to validate user token from request headers.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    userToken = _userTokenRepository.ValidateToken(request);
                    if (userToken != null)
                    {
                        _tokenValidated = true;
                        myToken = userToken != null ? (userToken.AuthToken ?? string.Empty) : string.Empty;
                        loggerModelNew.Module = _module;
                        loggerModelNew.Message = "User token validated successfully for UserID = " + userToken.UserID + ". Token: " + (!string.IsNullOrEmpty(myToken) ? "Available" : "Empty");
                        rSignLogger.RSignLogInfo(loggerModelNew);
                    }
                    else
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Message = "User token validation failed. Token is null or invalid.";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.BadRequest(new { Error = "User token validation failed from Vincere." });
                    }
                }
                catch (Exception ex)
                {
                    _tokenValidated = false;
                    loggerModelNew.Message = "Exception occurred while validating token. Proceeding with _tokenValidated=false. Exception: " + ex.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                }

                loggerModelNew.Message = "Initiating ValidateVincereAuthorizeCode call with token validation status: " + _tokenValidated;
                rSignLogger.RSignLogInfo(loggerModelNew);

                responseMessage = await ValidateVincereAuthorizeCode(request, userToken, authorizeUser, _tokenValidated);

                loggerModelNew.Module = _module;
                loggerModelNew.Message = "Method VincereAuthorizeUser --> end";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(responseMessage);

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SendToRSign controller VincereAuthorizeUser method. " + ex.Message.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, false);
                return Results.BadRequest(new { Error = "Problem with user authentication. at global catch" });

            }
        }
        private EnterpriseSSORestResponseMessage GetEnterpriseSSOWithEmail(string forgotEmailAddress)
        {
            loggerModelNew = new LoggerModelNew(forgotEmailAddress, "Authentication", "GetEnterpriseSSOWithEmail", "Process started for get enterprise SSO By emailId", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            EnterpriseSSORestResponseMessage resultRestAuth = new();
            try
            {
                loggerModelNew.Message = "Initiating REST API call to fetch Enterprise SSO details for email: " + forgotEmailAddress;
                rSignLogger.RSignLogInfo(loggerModelNew);
                HttpResponseMessage restSSOResponse = _rpostRestService.GetEnterpriseSSOWithEmail(forgotEmailAddress);
                if (!restSSOResponse.IsSuccessStatusCode)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Message = "Method GetEnterpriseSSOWithEmail Process started for get enterprise SSO By emailId --> Failed";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    resultRestAuth.ResultContent = new();
                    resultRestAuth.ResultContent.SSORst = 2;
                    return resultRestAuth;
                }
                else
                {
                    resultRestAuth = JsonConvert.DeserializeObject<EnterpriseSSORestResponseMessage>(restSSOResponse.Content.ReadAsStringAsync().Result);

                    loggerModelNew.Message = "Enterprise SSO response deserialized successfully for email: " + forgotEmailAddress;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    if (resultRestAuth != null && resultRestAuth.ResultContent != null)
                    {
                        if (resultRestAuth.ResultContent != null)
                        {
                            if (resultRestAuth.ResultContent.SSORst == 1)
                                resultRestAuth.ResultContent.SSORst = 1;
                            else
                                resultRestAuth.ResultContent.SSORst = 0;

                            loggerModelNew.Message = (resultRestAuth.ResultContent.SSORst == 1) ? "Enterprise SSO retrieval succeeded. Setting SSORst = 1." : "Enterprise SSO retrieval completed but returned invalid result. Setting SSORst = 0.";
                            rSignLogger.RSignLogInfo(loggerModelNew);
                        }
                        else
                        {
                            resultRestAuth.ResultContent = new ResultContentSSO();
                            resultRestAuth.ResultContent.SSORst = 2;
                        }
                    }
                    loggerModelNew.Message = "Method GetEnterpriseSSOWithEmail Process started for get enterprise SSO By emailId --> success  with resultRestAuth :" + resultRestAuth;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return resultRestAuth;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Message = "Error occurred in Account controller GetEnterpriseSSOWithEmail action.";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return resultRestAuth;
            }
        }
        public bool? GetRSignSSOWithEmail(string userEmail, string IntegrationType, string companyReferenceKey = "")
        {
            IntegrationResponse responseMessage = new();
            HttpResponseMessage responseToClient = new();
            loggerModelNew = new LoggerModelNew("", "Integration", "GetRSignSSOWithEmail", "Get RSign SSO With Email using API.", "");
            rSignLogger.RSignLogWarn(loggerModelNew);
            bool? IsEnableSSO = false;
            try
            {
                loggerModelNew.Message = "Get RSign SSO With Email using API --> Start and initiating SSO check for userEmail: " + userEmail + " IntegrationType: " + IntegrationType + ", CompanyReferenceKey: " + companyReferenceKey;
                rSignLogger.RSignLogInfo(loggerModelNew);
                IsEnableSSO = _authenticateRepository.IsSSOEnabledForUser(userEmail, IntegrationType, companyReferenceKey);
                responseMessage.data = IsEnableSSO;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                loggerModelNew.Module = _module;
                responseMessage.message = "successfully Retrived RSign SSO Settings by email and integrationType ";
                loggerModelNew.Message = (IsEnableSSO == true) ? "SSO is ENABLED for user " + userEmail + " and (IntegrationType: " + IntegrationType + ")." : (IsEnableSSO == false) ? "SSO is ENABLED for user " + userEmail + " and (IntegrationType: " + IntegrationType + ")." : "SSO status UNKNOWN (null) for user " + userEmail + " and (IntegrationType: " + IntegrationType + ").";
                rSignLogger.RSignLogInfo(loggerModelNew);
                loggerModelNew.Message = "Method GetRSignSSOWithEmail executed successfully. Get RSign SSO With Email using API --> end";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return IsEnableSSO;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in GetRSignSSOWithEmail method. Exception Message : " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return IsEnableSSO;
            }
        }
        public async Task<SendToRSignResponse> ValidateVincereAuthorizeCode(HttpRequest request, UserToken userToken, IntegrationsAuthorizeUserPOCO authorizeUser, bool _tokenValidated)
        {
            SendToRSignResponse sendToRSignResponse = new();
            try
            {
                loggerModelNew = new LoggerModelNew("", "Integration", "ValidateVincereAuthorizeCode", "Get Validate IManage Authentication using API.", "");
                loggerModelNew.Message = "Method ValidateVincereAuthorizeCode --> Start";
                rSignLogger.RSignLogInfo(loggerModelNew);
                AuthVincereToken authVincereToken = new();
                string tokenEndpoint = $"{_appConfiguration["VincereServerUrl"]}/oauth2/token?";
                string myToken = userToken != null ? (userToken.AuthToken ?? string.Empty) : string.Empty;
                try
                {
                    UserProfile userProfile = new();
                    userProfile = _userTokenRepository.GetUserProfileByEmail(userToken.EmailId);
                    SettingsExternalIntegration settings = _settingsRepository.GetExternalSettingsByType(userProfile.UserID, Constants.UploadDriveType.Vincere, userProfile.CompanyID);
                    SettingsExternalIntegration companySettings = _settingsRepository.GetExternalSettingsByCompanyId(userProfile.CompanyID, Constants.UploadDriveType.Vincere);

                    var payload = new Dictionary<string, string>
                    {
                        { "grant_type", "authorization_code" },
                        { "client_id", settings.AppClientId ?? companySettings.AppClientId },
                        { "code", authorizeUser.code },
                    };

                    HttpClient client = new HttpClient();
                    HttpRequestMessage requestEndPoint = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
                    {
                        Content = new FormUrlEncodedContent(payload)
                    };
                    HttpResponseMessage TokenEndPointresponse;
                    try
                    {
                        TokenEndPointresponse = client.Send(requestEndPoint);
                    }
                    catch (Exception ex)
                    {
                        loggerModelNew.Message = "Token request failed during send the request to Vincere authorize User API." + tokenEndpoint;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        throw new Exception("Token request failed during send.", ex);
                    }
                    if (!TokenEndPointresponse.IsSuccessStatusCode)
                    {
                        var errorContent = TokenEndPointresponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        throw new Exception($"Token request failed: {TokenEndPointresponse.StatusCode}, {errorContent}");
                    }
                    var json = TokenEndPointresponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (json != null)
                    {
                        authVincereToken = JsonConvert.DeserializeObject<AuthVincereToken>(json);
                        loggerModelNew.Message = "Token request success and deserialized the response imanage API ." + tokenEndpoint;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                    }
                    AdminGeneralAndSystemSettings userSettings = new();
                    ContactDetail contactDetail = new ContactDetail();
                    iManageAutoLoginModel iManageAutoLogin = new();
                    if (TokenEndPointresponse.StatusCode == HttpStatusCode.OK)
                    {
                        if (authVincereToken != null && authVincereToken.access_token != null)
                        {
                            loggerModelNew.Message = "Method ValidateVincereAuthorizeCode Saving token and refreshtoken --> Start";
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            settings = settings;
                            settings.UserID = userProfile.UserID;
                            settings.UserEmailId = userProfile.EmailID;
                            settings.AccessToken = authVincereToken.id_token;
                            settings.RefreshToken = authVincereToken.refresh_token;
                            settings.CustomerId = 0;
                            settings.CreatedBy = userProfile.UserID;
                            settings.CreatedDate = DateTime.Now;
                            settings.UserName = string.Empty;
                            settings.IntegrationType = Constants.UploadDriveType.Vincere;
                            settings.ApplicationAPIURL = !string.IsNullOrEmpty(companySettings.ApplicationAPIURL) ? companySettings.ApplicationAPIURL.TrimEnd('/') : companySettings.ApplicationAPIURL;
                            settings.CustomerUserId = string.Empty;
                            settings.Settinggsfor = userProfile.UserID;
                            settings.SettingsType = Constants.String.SettingsType.User;
                            settings.ID = settings.ID;
                            settings.DefaultRepository = string.Empty;
                            settings.ApplicationURL = companySettings.ApplicationURL;
                            settings.ServerURL = companySettings.ServerURL;
                            settings.AppClientSecret = companySettings.AppClientSecret;
                            settings.IsEnableActivity = companySettings.IsEnableActivity;
                            settings.IsUploadSignedDocument = companySettings.IsUploadSignedDocument;
                            settings.FinalSignedDocumentNameFormat = companySettings.FinalSignedDocumentNameFormat;
                            settings.FilingFinalSignedDocumentOptions = companySettings.FilingFinalSignedDocumentOptions;
                            settings.FilingFinalSignedDocuments = companySettings.FilingFinalSignedDocuments;
                            settings.SignatureCertificateNameFormat = companySettings.SignatureCertificateNameFormat;
                            settings = _settingsRepository.SaveExternalSettings(userProfile.UserID, settings);
                            loggerModelNew.Message = "Method ValidateVincereAuthorizeCode Saving token and refreshtoken --> End";
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new();
                            sendToRSignResponse.adminGeneralAndSystemSettings = adminGeneralAndSystemSettings;
                            sendToRSignResponse.adminGeneralAndSystemSettings.settingsExternalIntegration = settings;
                            sendToRSignResponse.success = true;
                            sendToRSignResponse.message = "Validate Vincere Authorize Code";
                        }
                    }
                    loggerModelNew.Message = "Method ValidateVincereAuthorizeCode --> End";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    sendToRSignResponse.success = true;
                    sendToRSignResponse.StatusCode = HttpStatusCode.OK;
                    return sendToRSignResponse;
                }
                catch (Exception ex)
                {
                    loggerModelNew.Message = "Error occurred in SendToRSign controller AuthorizeUser method.Error while getting the access and refreshtokens from iManage using the Authorization Code " + ex.Message.ToString();
                    rSignLogger.RSignLogError(loggerModelNew, ex, false);
                    sendToRSignResponse.StatusMessage = "Faile to get the access token from Vincere.";
                    sendToRSignResponse.success = false;
                    return sendToRSignResponse;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in main try block SendToRSign controller AuthorizeUser method.Error while getting the access and refreshtokens from iManage using the Authorization Code " + ex.Message.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return sendToRSignResponse;
            }
        }
        private async Task<AuthenticateResponseMessageModel> ValidateIManageAuthenticationAsync(HttpRequest request, UserToken userToken, IntegrationsAuthorizeUserPOCO authorizeUser, bool _tokenValidated)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "Integration", "ValidateIManageAuthenticationAsync", "Get Validate IManage Authentication using API.", "");
                loggerModelNew.Message = "Method ValidateIManageAuthenticationAsync --> Start";
                rSignLogger.RSignLogInfo(loggerModelNew);

                string tokenEndpoint = $"{_appConfiguration["iMServerURL"]}{_appConfiguration["iMTokenEndpoint"]}";
                AuthenticationTokenResponse authenticationToken = new();
                string IsFromIntegrationPage = authorizeUser.isFromIntegrationPage;
                bool isSSORequired = false;
                bool? SSOEnabledInRSign = false;
                int isSSORestricted = 0;
                bool isNewUser = false;
                string browserName = "";
                string ipAddress = UserTokenRepository.GetIPAddress(request);

                AuthenticateResponseMessageModel responseMessage = new();

                string CustomerId = string.Empty, UserId = string.Empty, Database = string.Empty, CustomerName = string.Empty, Emailaddress = string.Empty, Version = string.Empty, userAuthenticationKey = string.Empty, userRefreshToken = string.Empty, userRefreshExpires = string.Empty, userAccessTokenExpires = string.Empty, refKey = string.Empty, SSOAuthStatus = string.Empty, returnUrl = string.Empty;
                UserProfile userprofile = new UserProfile();
                RestResponseUserInfo rSignPlan = new();
                Dictionary<Guid?, string> LanguagelayoutList = new Dictionary<Guid?, string>();
                string myToken = userToken != null ? (userToken.AuthToken ?? string.Empty) : string.Empty;
                userAuthenticationKey = myToken;
                try
                {
                    var payload = new Dictionary<string, string>
                    {
                        { "grant_type", "authorization_code" },
                        { "client_id", _appConfiguration["iMclientId"] ?? string.Empty },
                        { "client_secret", _appConfiguration["iMclientSecret"] ?? string.Empty },
                        { "code", authorizeUser.code },
                        { "redirect_uri", _appConfiguration["iMRedirectUrl"] ?? string.Empty }
                    };

                    HttpClient client = new HttpClient();
                    HttpRequestMessage requestEndPoint = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
                    {
                        Content = new FormUrlEncodedContent(payload)
                    };
                    HttpResponseMessage iMTokenEndPointresponse;
                    try
                    {
                        iMTokenEndPointresponse = client.Send(requestEndPoint);
                    }
                    catch (Exception ex)
                    {
                        loggerModelNew.Message = "Token request failed during send the request to imanage API ." + tokenEndpoint;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        throw new Exception("Token request failed during send.", ex);
                    }

                    if (!iMTokenEndPointresponse.IsSuccessStatusCode)
                    {
                        var errorContent = iMTokenEndPointresponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        throw new Exception($"Token request failed: {iMTokenEndPointresponse.StatusCode}, {errorContent}");
                    }

                    var json = iMTokenEndPointresponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();


                    if (json != null)
                    {
                        authenticationToken = JsonConvert.DeserializeObject<AuthenticationTokenResponse>(json);
                        loggerModelNew.Message = "Token request success and deserialized the response imanage API ." + tokenEndpoint;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                    }
                    AdminGeneralAndSystemSettings userSettings = new();
                    ContactDetail contactDetail = new ContactDetail();
                    iManageAutoLoginModel iManageAutoLogin = new();
                    if (iMTokenEndPointresponse.StatusCode == HttpStatusCode.OK)
                    {
                        if (authenticationToken != null && authenticationToken.access_token != null)
                        {
                            GetiManageCustomerId(_appConfiguration["iMServerUrl"] ?? string.Empty, authenticationToken.access_token, out iManageAutoLogin);
                            loggerModelNew = new LoggerModelNew("Email Address : " + iManageAutoLogin.Emailaddress, "SendToRSign", "AuthorizeUser", "CustomerId:" + iManageAutoLogin.CustomerId + ",CustomerName:" + iManageAutoLogin.CustomerName, "", "");
                            loggerModelNew.Message = "method GetiManageCustomerId success." + tokenEndpoint;
                            rSignLogger.RSignLogInfo(loggerModelNew);

                            string CompanyReferenceKey = string.Empty;
                            Emailaddress = iManageAutoLogin.Emailaddress;
                            EnterpriseSSORestResponseMessage resultRestAuth = GetEnterpriseSSOWithEmail(Emailaddress);

                            if (resultRestAuth.ResultContent != null)
                            {
                                if (resultRestAuth.ResultContent.SSORst == 1)
                                {
                                    isSSORestricted = 1;
                                }
                                CompanyReferenceKey = !string.IsNullOrEmpty(resultRestAuth.ResultContent.CompanyKey) ? Convert.ToString(resultRestAuth.ResultContent.CompanyKey).Trim() : string.Empty;
                            }
                            else
                            { isSSORestricted = 2; }
                            loggerModelNew.Message = "method GetEnterpriseSSOWithEmail success. isSSORestricted " + isSSORestricted;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            SSOEnabledInRSign = GetRSignSSOWithEmail(Emailaddress, Constants.UploadDriveType.iManage, CompanyReferenceKey);

                            if (SSOEnabledInRSign == true && IsFromIntegrationPage == "Requested")
                            {
                                isSSORequired = true;
                            }
                            if (!_tokenValidated)
                            {
                                if (SSOEnabledInRSign != true && authorizeUser.source != "Login" && !isSSORequired)
                                {
                                    if (IsFromIntegrationPage == "Requested")
                                    {
                                        responseMessage.Success = false;
                                        responseMessage.StatusMessage = _appConfiguration["iManageAuthFailure"] ?? string.Empty;
                                        responseMessage.StatusCode = HttpStatusCode.Unauthorized;
                                        return responseMessage;
                                    }
                                    else
                                    {
                                        returnUrl = "";
                                    }
                                    SSOAuthStatus = "SSONotRequired";
                                    //return PartialView("~/Views/Settings/iManageAuthorized.cshtml", userSettings);
                                }
                                else if ((SSOEnabledInRSign == true && authorizeUser.source == "Login") || isSSORequired)
                                {
                                    loggerModelNew.Message = "iManage SSO Login process started with PartnerIntegrationToken API, Preparing the model Object  SSOEnabledInRSign " + isSSORestricted + "authorizeUser.source " + authorizeUser.source; ;
                                    rSignLogger.RSignLogWarn(loggerModelNew);

                                    string[] emailList = Emailaddress.Split('@');

                                    PartnerIntegrationModel partnerIntegrationModel = new();
                                    partnerIntegrationModel.customerId = iManageAutoLogin.CustomerId;
                                    partnerIntegrationModel.userId = iManageAutoLogin.CustomerName;
                                    partnerIntegrationModel.addUser = true;
                                    partnerIntegrationModel.domain = emailList[1];
                                    partnerIntegrationModel.emailAddress = iManageAutoLogin.Emailaddress;
                                    partnerIntegrationModel.source = "imanage";
                                    partnerIntegrationModel.ipAddress = ipAddress;
                                    partnerIntegrationModel.referencekey = CompanyReferenceKey;
                                    partnerIntegrationModel.secretKey = string.Empty;
                                    partnerIntegrationModel.intergrationId = string.Empty;

                                    ExtTokenRequestModel extTokenRequestModel = new();
                                    extTokenRequestModel.partnerIntegrationModel = partnerIntegrationModel;
                                    extTokenRequestModel.provider = Constants.UploadDriveType.iManage;
                                    extTokenRequestModel.BrowserType = string.Empty;
                                    extTokenRequestModel.isFromIntegrations = "true";

                                    //responseMessage = await _authenticateService.SingleSignOn(extTokenRequestModel);

                                    AuthenticateResponseMessageModel SingleSignOnResult = await _authenticateService.SingleSignOn(extTokenRequestModel);
                                    if (SingleSignOnResult.StatusCode != HttpStatusCode.OK)
                                    {
                                        responseMessage.Success = false;
                                        responseMessage.StatusMessage = _appConfiguration["iManageAuthFailure"] ?? string.Empty;
                                        loggerModelNew.Message = responseMessage.StatusMessage;
                                        rSignLogger.RSignLogInfo(loggerModelNew);
                                        return responseMessage;
                                    }
                                    if (SingleSignOnResult != null && !string.IsNullOrEmpty(SingleSignOnResult.AccessToken))
                                    {
                                        userAuthenticationKey = SingleSignOnResult.AccessToken;
                                        SingleSignOnResult.UserProfile = userprofile;
                                        responseMessage = SingleSignOnResult;
                                        loggerModelNew.Message = responseMessage.StatusMessage;
                                        rSignLogger.RSignLogInfo(loggerModelNew);
                                    }
                                    else
                                    {

                                    }
                                    if (!string.IsNullOrEmpty(userAuthenticationKey))
                                    {
                                        authorizeUser.isFromIntegrationPage = "Validated";
                                    }
                                    else
                                    {
                                        authorizeUser.isFromIntegrationPage = "NotValidated";
                                    }
                                }
                                else if (isSSORestricted == 2 || SSOEnabledInRSign == false)
                                {
                                    responseMessage.Success = false;
                                    responseMessage.StatusMessage = _appConfiguration["iManageAuthFailure"] ?? string.Empty;
                                    loggerModelNew.Message = responseMessage.StatusMessage + " else if block isSSORestricted " + isSSORestricted;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return responseMessage;
                                    //exception
                                }
                            }
                            if (!string.IsNullOrEmpty(userAuthenticationKey))
                            {
                                userprofile = _userTokenRepository.GetUserProfileByToken(userAuthenticationKey);
                                loggerModelNew.Message = "Got the User profile to get the other information --> start";
                                rSignLogger.RSignLogInfo(loggerModelNew);
                                AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new();
                                SettingsExternalIntegration settingsExternalIntegration = new();
                                iManageAutoLogin.iManageServerUrl = _appConfiguration["iMServerURL"] ?? string.Empty;
                                iManageAutoLogin.logInUserCompanyId = userprofile != null ? (Guid)userprofile.CompanyID : Guid.Empty;
                                iManageAutoLogin.logInUserId = userprofile != null ? (Guid)userprofile.UserID : Guid.Empty;
                                iManageAutoLogin.access_token = authenticationToken?.access_token;
                                iManageAutoLogin.refresh_token = authenticationToken?.refresh_token;
                                loggerModelNew = new LoggerModelNew(userprofile?.EmailID, "Settings", "AuthorizeUser", "iManage Registration start ", "", "");
                                loggerModelNew.Message = "iManage Registration start";
                                rSignLogger.RSignLogInfo(loggerModelNew);

                                settingsExternalIntegration = _authenticateRepository.iManageAutoLogin(iManageAutoLogin);
                                loggerModelNew = new LoggerModelNew(userprofile?.EmailID, "Settings", "AuthorizeUser", "iManage Registration End", "", "");
                                loggerModelNew.Message = "iManage Registration End";
                                rSignLogger.RSignLogInfo(loggerModelNew);

                                UserSettingsModel userSettingsModel = new();
                                UserAdditionalResponseMessage userAdditionalResponseMessage = new();
                                SettingResponseMessage userSettingsResponseMessage = new();
                                SettingResponseMessage companySettingsResponseMessage = new();
                                AdminGeneralAndSystemSettings companySettings = new();
                                userSettingsModel.CompanyId = Guid.Empty;
                                userSettingsModel.Email = userprofile?.EmailID;
                                loggerModelNew.Message = "Got the User profile to get the other information --> GetUserCompanySettings logic start";
                                rSignLogger.RSignLogInfo(loggerModelNew);
                                if (userprofile != null)
                                {
                                    userAdditionalResponseMessage = _authenticateService.GetUserCompanySettings(userSettingsModel, userprofile, userAuthenticationKey, userSettingsResponseMessage, companySettingsResponseMessage, userSettings, companySettings);
                                    if (userAdditionalResponseMessage != null)
                                    {
                                        responseMessage.UserProfile = userprofile;
                                        responseMessage.UserSettings = userAdditionalResponseMessage?.UserSettings;
                                        responseMessage.CompanySettings = userAdditionalResponseMessage?.CompanySettings;
                                        responseMessage.DefaultLandingPageSetting = userAdditionalResponseMessage.DefaultLandingPageSetting;
                                        responseMessage.UserRolesDetails = userAdditionalResponseMessage.UserRolesDetails;
                                        responseMessage.UserAdditionalRoles = userAdditionalResponseMessage.UserAdditionalRoles;
                                    }
                                }
                                loggerModelNew.Message = "Got the User profile to get the other information --> GetUserCompanySettings logic end";
                                rSignLogger.RSignLogInfo(loggerModelNew);
                                return responseMessage;
                            }
                        }
                        else
                        {
                            if (IsFromIntegrationPage == "Requested")
                            {
                                responseMessage.Success = false;
                                responseMessage.StatusMessage = _appConfiguration["iManageAuthFailure"] ?? string.Empty;
                                responseMessage.StatusCode = HttpStatusCode.Unauthorized;
                                return responseMessage;
                                //    ViewBag.returnUrl = Url.Action("EmailLinkLogOn", "Account", new { ReturnUrl = "/SendToRsign/SendToRSignIndex", JsonRequestBehavior.AllowGet });
                            }
                            else { returnUrl = ""; }
                            responseMessage.Success = false;
                            responseMessage.StatusMessage = _appConfiguration["iManageAuthFailure"] ?? string.Empty;
                            return responseMessage;
                        }

                    }
                    if (IsFromIntegrationPage == "Requested")
                    {
                        responseMessage.Success = false;
                        responseMessage.StatusMessage = _appConfiguration["iManageAuthFailure"] ?? string.Empty;
                        responseMessage.StatusCode = HttpStatusCode.Unauthorized;
                        return responseMessage;
                        //ViewBag.returnUrl = Url.Action("EmailLinkLogOn", "Account", new { ReturnUrl = "/SendToRsign/SendToRSignIndex", JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        returnUrl = "";
                        responseMessage.Success = false;
                        responseMessage.StatusMessage = _appConfiguration["iManageAuthFailure"] ?? string.Empty;
                        responseMessage.StatusCode = HttpStatusCode.Unauthorized;
                        return responseMessage;
                    }
                }
                catch (Exception ex)
                {
                    loggerModelNew.Message = "Error occurred in SendToRSign controller AuthorizeUser method.Error while getting the access and refreshtokens from iManage using the Authorization Code " + ex.Message.ToString();
                    rSignLogger.RSignLogError(loggerModelNew, ex, false);
                    return (AuthenticateResponseMessageModel)Results.BadRequest(new { Error = "Problem with user authentication. internal catch" });
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in main try block SendToRSign controller AuthorizeUser method.Error while getting the access and refreshtokens from iManage using the Authorization Code " + ex.Message.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return (AuthenticateResponseMessageModel)Results.BadRequest(new { Error = "Problem with user authentication. main catch" });
            }
        }
        private void GetiManageCustomerId(string ServerURL, string Token, out iManageAutoLoginModel iManageAutoLogin)
        {
            iManageAutoLoginModel AutoLoginModel = new();
            try
            {
                loggerModelNew = new LoggerModelNew(ServerURL, "SendToRSign", "External integration getting iManage CustomerId", "GetiManageCustomerId", "", "");
                loggerModelNew.Message = "Got the User profile to get the other information --> GetiManageCustomerId logic start";
                rSignLogger.RSignLogInfo(loggerModelNew);
                //Get CustomerId
                HttpResponseMessage customerResponce;
                ApiInfoData discoveryResponse;
                GetCustomerId(ServerURL, Token, out customerResponce, out discoveryResponse);
                ApiInfoData responceData = new ApiInfoData();
                AutoLoginModel.integrationType = Constants.UploadDriveType.iManage;
                if (!customerResponce.IsSuccessStatusCode || discoveryResponse == null || discoveryResponse.data == null || discoveryResponse.data.user == null
                    || discoveryResponse.data.user == null
                    || discoveryResponse.data.dms_version == null
                    || discoveryResponse.data.work == null
                    || discoveryResponse.data.work.preferred_library == null)
                {
                    AutoLoginModel.CustomerId = string.Empty;
                    AutoLoginModel.CustomerName = string.Empty;
                    AutoLoginModel.UserId = string.Empty;
                    AutoLoginModel.Database = string.Empty;
                    AutoLoginModel.Emailaddress = string.Empty;
                    AutoLoginModel._versions = string.Empty;
                }
                else
                {

                    AutoLoginModel.CustomerId = discoveryResponse.data.user.customer_id.ToString();
                    AutoLoginModel.CustomerName = discoveryResponse.data.user.name.ToString();
                    AutoLoginModel.UserId = discoveryResponse.data.user.id.ToString();
                    AutoLoginModel.Emailaddress = discoveryResponse.data.user.email.ToString();
                    AutoLoginModel._versions = discoveryResponse.data.dms_version.ToString();
                    if (discoveryResponse.data.work.preferred_library != null)
                    {
                        AutoLoginModel.Database = discoveryResponse.data.work.preferred_library.ToString();
                    }
                    else
                    {
                        AutoLoginModel.Database = string.Empty;
                    }
                }
                iManageAutoLogin = AutoLoginModel;
                loggerModelNew.Message = "Got the User profile to get the other information --> GetiManageCustomerId logic end";
                rSignLogger.RSignLogInfo(loggerModelNew);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SendToRSign controller GetiManageCustomerId method. " + ex.Message.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, false);
                iManageAutoLogin = AutoLoginModel;
            }
        }
        private void GetCustomerId(string ServerURL, string Token, out HttpResponseMessage customerResponce, out ApiInfoData discoveryResponse)
        {
            try
            {
                loggerModelNew = new LoggerModelNew(ServerURL, "SendToRSign", "External inegration getting iManage CustomerId", "GetCustomerId", "", ServerURL);
                loggerModelNew.Message = "Got the User profile to get the other information --> GetCustomerId logic start";
                rSignLogger.RSignLogInfo(loggerModelNew);
                HttpClient httpCustomerId = new HttpClient();
                httpCustomerId.BaseAddress = new Uri(_appConfiguration["iMWorkUrl"]);
                httpCustomerId.DefaultRequestHeaders.Accept.Clear();
                httpCustomerId.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpCustomerId.DefaultRequestHeaders.Add("X-Auth-Token", Token);
                customerResponce = httpCustomerId.GetAsync("api").Result;
                string json = customerResponce.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ApiInfoData>(json);
                discoveryResponse = result;
                loggerModelNew.Message = "Got the User profile to get the other information --> GetCustomerId logic end";
                rSignLogger.RSignLogInfo(loggerModelNew);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SendToRSign controller GetCustomerId method. " + ex.Message.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, false);
                customerResponce = null;
                discoveryResponse = null;
            }
        }
        /// <summary>
        /// SFInitializeEnvelopeWithDetails - Initializing a new envelope using API
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tData"></param>
        /// <returns></returns>
        public async Task<IResult> SFInitializeEnvelopeWithDetails(HttpRequest request, APIEnvelopeRequest tData)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SFInitializeEnvelopeWithDetails", "Process started for Initializing a new envelope using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                loggerModelNew.Message = "Validating user token...";
                rSignLogger.RSignLogInfo(loggerModelNew);
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                loggerModelNew.Module = _module;
                loggerModelNew.Message = "User token validated successfully for UserID= " + userToken.UserID + ". Proceeding with envelope initialization.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                string reqHostValue = request.Scheme + "://" + request.Host + "/";
                string domainUrlType = EnvelopeHelper.GetSourceUrlType(reqHostValue);

                loggerModelNew.Module = _module;
                loggerModelNew.Message = "Request host: " + reqHostValue + ", Domain type identified as: " + domainUrlType + " and started extracting sender address from GetSenderAddressFromHeaders method.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                string senderAddress = _userTokenRepository.GetSenderAddressFromHeaders(request, "SenderAddress");
                loggerModelNew.Module = _module;
                loggerModelNew.Message = (string.IsNullOrEmpty(senderAddress) ? "Sender address header is missing or empty." : "Sender address extracted successfully : " + senderAddress) + " Initiated a Call to SFInitializeEnvelopeWithDetails...";
                rSignLogger.RSignLogInfo(loggerModelNew);
                var result = await _iIntegrationEnvelope.SFInitializeEnvelopeWithDetails(userToken, senderAddress, tData, remoteIpAddress, domainUrlType);
                loggerModelNew.Module = _module;
                loggerModelNew.Message = "Successfully initialized envelope and received response from _iIntegrationEnvelope.SFInitializeEnvelopeWithDetails.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SFInitializeEnvelopeWithDetails";
                loggerModelNew.Message = "API EndPoint - Exception at SFInitializeEnvelopeWithDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to retrieve contacts from integrations
        /// </summary>
        /// <param name="request"></param>
        /// <param name="integrationContactDetails"></param>
        /// <returns></returns>
        public async Task<IResult> GetIntegrationContacts(HttpRequest request, [FromBody] IntegrationContactDetails integrationContactDetails)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetIntegrationContacts", "Endpoint Initialized,to retrieve contacts from integrations.", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            IntegrationContactDetails contactsInfo = integrationContactDetails != null ? integrationContactDetails : null;
            string integrationType = contactsInfo.IntegrationType, entityType = contactsInfo.EntityType, entityId = contactsInfo.EntityId;
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                HttpResponseMessage responseMessage = null;
                var client = _httpClientFactory.CreateClient();
                var userprofile = _userRepository.GetUserProfile(userToken.UserID);
                List<ExternalContactsList> contactDetail = new List<ExternalContactsList>();
                SettingsExternalIntegration externalSettingsResponse = _settingsRepository.GetExternalSettingsByCompanyId(userprofile.CompanyID, integrationType);

                if (externalSettingsResponse == null)
                {
                    loggerModelNew.Message = "Process failed to get settings from GetExternalSettingsByCompanyId method.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.BadRequest(loggerModelNew.Message);
                }

                loggerModelNew.Message = "Process started to retrieve contacts from " + integrationType + " integration for " + entityType + " entity.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                if (integrationType == Constants.UploadDriveType.Appliedepic)
                {
                    List<ClientContacts> Contacts = await _integrationRepository.GetAppliedEpicContactsList(externalSettingsResponse, contactsInfo);
                    if (Contacts != null && Contacts.Count > 0) return Results.Ok(Contacts);
                    else return Results.BadRequest(loggerModelNew.Message);
                }
                else if (integrationType == Constants.UploadDriveType.Bullhorn)
                {
                    List<ClientContacts> Contacts = await _integrationRepository.GetBullhornContactsList(externalSettingsResponse, contactsInfo);
                    if (Contacts != null && Contacts.Count > 0) return Results.Ok(Contacts);
                    else return Results.BadRequest(loggerModelNew.Message);
                }
                else if (integrationType == Constants.UploadDriveType.Vincere)
                {
                    List<ExternalContactsList> Contacts = await _integrationRepository.GetVincereContactsList(userToken.UserID, contactsInfo);
                    if (Contacts != null && Contacts.Count > 0) return Results.Ok(Contacts);
                    else return Results.BadRequest(loggerModelNew.Message);
                }
                return Results.Ok(contactDetail);
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "GetIntegrationContacts";
                loggerModelNew.Message = "API EndPoint - Exception at GetIntegrationContacts method when retrieving the contacts from " + integrationType + " integration and " + entityType + " entity. Error message is : " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult CheckIntegrationToken(HttpRequest request, string integrationType)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "CheckIntegrationToken", "Checking the integration access token valid or not using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                if (string.IsNullOrWhiteSpace(integrationType))
                    return Results.BadRequest(new { success = false, resultmessage = "Invalid integration type." });


                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "CheckIntegrationToken";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from netdocuments ==> Start";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var tokenDetails = _integrationRepository.CheckIntegrationToken(userProfile, integrationType);
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "CheckIntegrationToken";
                    loggerModelNew.Message = "User Token validated and getting documents from netdocuments ==> End";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.Ok(tokenDetails);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "CheckIntegrationToken";
                loggerModelNew.Message = "API EndPoint - Exception at CheckIntegrationToken method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult DeRegisterUserAccount(HttpRequest request, SettingsExternalIntegration settingsToSave)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeRegisterUserAccount", "Checking the integration access token valid or not using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessage responseMessage = new();
            try
            {
                if (string.IsNullOrWhiteSpace(settingsToSave.IntegrationType))
                    return Results.BadRequest(new { success = false, resultmessage = "Invalid integration type." });


                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeRegisterUserAccount";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated  for deregister from netdocuments ==> Start";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    settingsToSave.AccessToken = string.Empty;
                    settingsToSave.RefreshToken = string.Empty;
                    var status = _integrationRepository.DeRegisterExternalSettings(userProfile.UserID, settingsToSave);
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeRegisterUserAccount";
                    loggerModelNew.Message = "User Token validated  for deregister from netdocuments ==> End";
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.Message = "De-registered user successfully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "DeRegisterUserAccount";
                loggerModelNew.Message = "API EndPoint - Exception at DeRegisterUserAccount method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> OpenIManageFolderPickerAsync(OpenIManageFolderPickerRequest request)
        {
            loggerModelNew = new LoggerModelNew("", _module, "OpenIManageFolderPicker", "Generating folder picker URL for iManage using API.", "", "", "", "", "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessage responseMessage = new();
            try
            {

                string dialogTokenEndpoint =
                    DialogConstants.DialogTokenEndpoint
                        .Replace("{{customerId}}", request.CustomerId)
                        .Replace("{{serverUrl}}", _appConfiguration["iMServerUrl"]);

                string customerUserId = request.CustomerId;

                // Try dialog token
                var dialogToken = await _integrationRepository.TryGetDialogTokenAsync(dialogTokenEndpoint, request.Token, request.RefreshToken, _appConfiguration["iMServerUrl"]);

                string dialogUrl = string.Format(DialogConstants.DialogFolderPicker, _appConfiguration["iMServerUrl"], _appConfiguration["iMclientSecret"], dialogToken, request.CustomerId, customerUserId ?? request.CustomerId);
                responseMessage.Message = dialogUrl;
                responseMessage.StatusCode = HttpStatusCode.OK;
                loggerModelNew.Message = "Successfully generated the folder picker URL"+ dialogUrl;
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Results.Ok(responseMessage);

            }
            catch (Exception ex)
            {
                loggerModelNew = new LoggerModelNew("", _module, "OpenIManageFolderPicker", "Generating folder picker URL for iManage using API.", "", "", "", "", "SendAPI");
                loggerModelNew.Message = "Failed "+ ex.Message + ex.InnerException;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Unauthorized();
            }
        }
        
    }
}
