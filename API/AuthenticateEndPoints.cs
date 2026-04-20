using Aspose.Pdf.Operators;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cmp;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models.APIModels;
using RSign.Models.Helpers;
using RSign.Models.Models;
using RSign.Models.Repository;
using RSign.Notification;
using System.Net;
using System.Text;
using System.Web.Mvc;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using RSign.Models;
using System.Reflection;
using RSign.Models.Interfaces;
using RSign.Common.Enums;
using System.Web;

namespace RSign.SendAPI.API
{
    public class AuthenticateEndPoints
    {
        RSignLogger rSignLogger = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _configuration;
        private AuthenticateService _authenticateService;
        private IHttpContextAccessor _accessor;
        private RpostRestService _rpostRestService;
        private readonly ILookupRepository _lookupRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticateRepository _authenticateRepository;
        private readonly IConfiguration _appConfiguration;

        public AuthenticateEndPoints(IConfiguration configuration, AuthenticateService authenticateService, IHttpContextAccessor accessor,
            ILookupRepository lookupRepository, ICompanyRepository companyRepository, IUserRepository userRepository, IAuthenticateRepository authenticateRepository, IConfiguration appConfiguration)
        {
            _configuration = configuration;
            _accessor = accessor;
            _authenticateService = authenticateService;
            rSignLogger = new RSignLogger(_configuration);
            _rpostRestService = new RpostRestService(_configuration);
            _lookupRepository = lookupRepository;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _authenticateRepository = authenticateRepository;
            _appConfiguration = appConfiguration;
        }
        public void RegisterAuthApis(WebApplication app)
        {
            app.MapGet("/api/v1/Auth/GetLanguageKeyDetails", GetLanguageKeyDetails);
            app.MapGet("/api/v1/Account/GetTimeZone", GetTimeZone);
            app.MapGet("/api/v1/Account/GetLanguage", GetLanguage);
            app.MapPost("/api/v1/Auth/AuthenticateUser", AuthenticateUser);
            app.MapPost("/api/v1/Auth/ValidateToken", ValidateToken);
            app.MapPost("/api/v1/Auth/DecryptValidateToken", DecryptValidateToken);         
            app.MapPost("/api/v1/Auth/RefreshToken", RefreshToken);
            app.MapPost("/api/v1/Auth/AuthenticateUserV3", AuthenticateUserV3);
            app.MapPost("/api/v1/Auth/SingleSignOn", SingleSignOn);
            app.MapPost("/api/v1/Account/RegisterUser", RegisterUser);
            app.MapPost("/api/v1/Account/ForgotPassword", ForgotPassword);
            app.MapPost("/api/v1/Auth/ResetPassword", ResetPassword);    
            app.MapPost("/api/v1/Account/ValidateUserTokenEnvelopeDetails", ValidateUserTokenEnvelopeDetails);
            app.MapPost("/api/v1/Account/UpdateReviewFlag", UpdateReviewFlagDetailsByUserId);
            app.MapPost("/api/v1/Auth/GetMFAAndPasswordPolicySettings", GetMFAAndPasswordPolicySettings);
            app.MapPost("/api/v1/Account/ReviewDetail", ReviewDetail);
        }

        #region ValidateToken  
        /// <summary>
        /// Autheticate the User, whether its valid user or not
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IResult> AuthenticateUser(UserLoginToken objUser)
        {
            loggerModelNew = new LoggerModelNew(!string.IsNullOrEmpty(objUser.EmailId) ? objUser.EmailId : string.Empty, "AuthenticateEndPoint", "AuthenticateUser", "Process started in AuthenticateUser end point", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseTokenWithEmailId respToken = new ResponseTokenWithEmailId();
            try
            {
                respToken = await _authenticateService.AuthenticateUserService(objUser);
                return Results.Ok(new { Success = respToken.StatusCode, AuthMessage = respToken.AuthMessage, AuthToken = respToken.AuthToken, EmailId = objUser.EmailId, UserId = respToken.UserId });
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at AuthenticateUser method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = respToken.StatusCode, AuthMessage = ex.Message, AuthToken = respToken.AuthToken, EmailId = objUser.EmailId });
            }
        }

        /// <summary>
        /// This api is reffering to Validate a Token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userTokenModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IResult> ValidateToken(HttpRequest request, UserTokenModel userTokenModel)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateEndPoints", "ValidateToken", "Method Initialized", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseTokenWithEmailId responseToken = new ResponseTokenWithEmailId();
            try
            {
                responseToken = await _authenticateService.ValidateTokenService(request, userTokenModel);
                return Results.Ok(new { Success = "Success", userViewModel = responseToken.UserViewModel, AuthMessage = "Token valiated.", AuthToken = responseToken.AuthToken });
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at ValidateToken method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = responseToken.StatusCode, AuthMessage = ex.Message, AuthToken = responseToken.AuthToken });
            }
        }

        /// <summary>
        /// To decrypt the prepare page url, to get the authtoken, envelope id and prepare type whether its envelope or template.
        /// </summary>
        /// <param name="userPrepareUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IResult> DecryptValidateToken(PreparePageUrl userPrepareUrl)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateEndPoints", "DecryptValidateToken", "Method Initialized", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseTokenWithEmailId responseToken = new ResponseTokenWithEmailId();
            try
            {
                responseToken = await _authenticateService.DecryptValidateTokenService(userPrepareUrl);
                return Results.Ok(new
                {
                    StatusCodeResult = responseToken.StatusCode,
                    UserViewModel = responseToken.UserViewModel,
                    TokenViewModel = responseToken.TokenViewModel,
                    AuthMessage = responseToken.AuthMessage,
                    AuthToken = responseToken.AuthToken,
                    EnvelopeId = responseToken.EnvelopeId,
                    PrepareType = responseToken.PrepareType,
                    TranslationDetails = responseToken.LanguageKeyTranslations,
                    SourceUrlType = responseToken.SourceUrlType,
                    IsActingUserPerformingAction = responseToken.IsActingUserPerformingAction
                });
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "DecryptValidateToken";
                loggerModelNew.Message = "API EndPoint - Exception at DecryptValidateToken method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = responseToken.StatusCode, AuthMessage = ex.Message, AuthToken = responseToken.AuthToken, SourceUrlType = responseToken.SourceUrlType });
            }
        }

        #endregion ValidateToken

        /// <summary>
        /// To retrieve the language key value details
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IResult> GetLanguageKeyDetails(string languageCode)
        {
            loggerModelNew = new LoggerModelNew(languageCode, "AuthenticateEndPoint", "GetLanguageKeyDetails", "Process started in GetLanguageKeyDetails end point", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            LanguageKeyValuesResponseMessage languageKeyValuesResponse = new LanguageKeyValuesResponseMessage();
            try
            {
                languageKeyValuesResponse = await _authenticateService.GetLanguageKeyDetails(languageCode);
                return Results.Ok(new { Success = languageKeyValuesResponse.StatusCode, AuthMessage = languageKeyValuesResponse.Message, LanguageKeyDetails = languageKeyValuesResponse.LanguageKeyTranslation });
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "GetLanguageKeyDetails";
                loggerModelNew.Message = "API EndPoint - Exception at GetLanguageKeyDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogWarn(loggerModelNew);
                return Results.BadRequest(new { BadRequest = languageKeyValuesResponse.StatusCode, AuthMessage = ex.Message });
            }
        }

        /// <summary>
        /// Get refresh Token
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IResult> RefreshToken(HttpRequest request, RefreshTokenObj objUser)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew(!string.IsNullOrEmpty(objUser.EmailAddress) ? objUser.EmailAddress : string.Empty, "AuthenticateEndPoint", "RefreshToken", "Process started for get Refresh Token", "", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            UserRefreshTokenResponseMessage respToken = new UserRefreshTokenResponseMessage();
            try
            {
                objUser.IPAddress = remoteIpAddress;
                respToken = await _authenticateService.GetRefreshToken(objUser);
                return Results.Ok(respToken);
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "RefreshToken";
                loggerModelNew.Message = "API EndPoint - Exception at RefreshToken method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = respToken.StatusCode });
            }
        }

        [AllowAnonymous]
        public async Task<IResult> AuthenticateUserV3(HttpRequest request, UserLoginTokenRequestModel userLoginModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew(!string.IsNullOrEmpty(userLoginModel.EmailAddress) ? userLoginModel.EmailAddress : string.Empty, "AuthenticateEndPoint", "AuthenticateUserV3", "Process started AuthenticateUserV3 method", "", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            AuthenticateResponseMessageModel responseMessage = new AuthenticateResponseMessageModel();
            HttpResponseMessage restResponse = new HttpResponseMessage();
            bool? IsEnableSSO = false;
            try
            {

                //int enforcePasswordPolicy = Convert.ToInt32(userLoginModel.IsPasswordPolicyEnabled);
                //int mfaEnabled = Convert.ToInt32(userLoginModel.IsMFAEnabled);

                //if (enforcePasswordPolicy == 1 || enforcePasswordPolicy == 2 || mfaEnabled == 1)
                //{
                //    RpostRestService rpostRestApi = new RpostRestService(_configuration);
                //    restResponse = rpostRestApi.GetMFAToken(userLoginModel.EmailAddress, userLoginModel.Password);
                //}
                if(userLoginModel != null && !string.IsNullOrWhiteSpace(userLoginModel.Password))
                {
                    string encryptedPassword = userLoginModel.Password.Replace(" ", "+");
                    userLoginModel.Password = EncryptDecryptQueryString.DecryptAES(encryptedPassword, Convert.ToString(_appConfiguration["AppKeyAES"]));
                }
                responseMessage = await _authenticateService.IntAppToken(userLoginModel);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    if (responseMessage != null && responseMessage.SSOEnabledInRSign)
                    {
                        return Results.Ok(new { success = false, message = "Single Sign On has been configured on this domain. Please authenticate using iManage by clicking Other options button." });
                    }
                    return Results.Ok(responseMessage);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (responseMessage.Message.Contains("RCAP-1056"))
                    {
                        return Results.Ok(new { success = false, message = responseMessage.Message });
                    }
                    if (responseMessage.SSOEnabledInRSign)
                    {
                        return Results.Ok(new { success = false, message = "Single Sign On has been configured on this domain. Please authenticate using iManage by clicking Other options button." });
                    }
                    var lookupLanguageList = _lookupRepository.GetLookupLanguageList(Lookup.LanguageKeyDetails, "en-us", Constants.String.languageKeyType.Validation).Where(a => a.ResourceKeyID == Constants.Resourcekey.lang_SSOLoginMessage).ToList();
                    var keyValue = lookupLanguageList.FirstOrDefault()?.KeyValue ?? "";
                    return Results.Ok(new { success = false, message = keyValue });
                }
                else
                {
                    if (responseMessage.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        return Results.Ok(new { success = false, message = "Invalid Password." });
                    }
                    else if (string.IsNullOrEmpty(responseMessage.Message))
                    {
                        return Results.Ok(new { success = false, message = "An application error occured. Please try again or contact support." });
                    }
                    else
                        return Results.Ok(new { success = false, message = responseMessage.Message });
                }                
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "AuthenticateUserV3";
                loggerModelNew.Message = "API EndPoint - Exception at AuthenticateUserV3 method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = restResponse.StatusCode });
            }
        }
        [AllowAnonymous]
        public async Task<IResult> SingleSignOn(HttpRequest request, ExtTokenRequestModel extTokenRequestModel)
        {
            Dictionary<Guid?, string> LanguagelayoutList = new Dictionary<Guid?, string>();
            string browserName = string.Empty, ipAddress = UserTokenRepository.GetIPAddress(request), userAuthenticationKey = string.Empty, fieldToFocus = string.Empty, refKey = string.Empty,
                userRefreshToken = string.Empty, userRefreshExpires = string.Empty, userAccessTokenExpires = string.Empty, userEmail = string.Empty, isFromIntegrations = "false";
            bool isNewUser = false;
            RestResponseUserInfo rSignPlan = new RestResponseUserInfo();
            HttpResponseMessage restResponse = new HttpResponseMessage();
            AuthenticateResponseMessageModel responseMessage = new AuthenticateResponseMessageModel();
            bool? IsEnableSSO = false;
            try
            {
                loggerModelNew.Message = "Invoking SingleSignOn method";
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseMessage = await _authenticateService.SingleSignOn(extTokenRequestModel);
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "SingleSignOn";
                loggerModelNew.Message = "API EndPoint - Exception at SingleSignOn method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = restResponse.StatusCode });
            }
        }

        public async Task<IResult> RegisterUser(HttpRequest request, RegistrationModal register)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew(!string.IsNullOrEmpty(register.EmailId) ? register.EmailId : string.Empty, "AuthenticateEndPoint", "RegisterUser", "Process started for get RegisterUser", "", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            HttpResponseMessage responseToClient = new HttpResponseMessage();
            try
            {
                string registerEmailAddress = !string.IsNullOrEmpty(register.EmailId) ? register.EmailId : string.Empty;
                int isSSORestricted = GetEnterpriseSSOWithEmail(registerEmailAddress);

                if (register != null && !string.IsNullOrWhiteSpace(register.Password))
                {
                    string encryptedPassword = register.Password.Replace(" ", "+");
                    register.Password = EncryptDecryptQueryString.DecryptAES(encryptedPassword, Convert.ToString(_appConfiguration["AppKeyAES"]));
                }
                if (register != null && !string.IsNullOrWhiteSpace(register.ComparePassword))
                {
                    string encryptedComparePassword = register.ComparePassword.Replace(" ", "+");
                    register.ComparePassword = EncryptDecryptQueryString.DecryptAES(encryptedComparePassword, Convert.ToString(_appConfiguration["AppKeyAES"]));
                }

                if (isSSORestricted == 1)
                {
                    var lookupLanguageList = _lookupRepository.GetLookupLanguageList(Lookup.LanguageKeyDetails, register.Language, Constants.String.languageKeyType.Validation).Where(a => a.ResourceKeyID == Constants.Resourcekey.lang_SSOForgotPwdMessage).ToList();
                    var keyValue = lookupLanguageList.FirstOrDefault()?.KeyValue ?? "";
                    return Results.Ok(new { success = false, message = keyValue });

                }
                if (isSSORestricted == 2)
                {
                    var lookupLanguageList = _lookupRepository.GetLookupLanguageList(Lookup.LanguageKeyDetails, register.Language, Constants.String.languageKeyType.Validation).Where(a => a.ResourceKeyID == Constants.Resourcekey.lang_applicationError).ToList();
                    var keyValue = lookupLanguageList.FirstOrDefault()?.KeyValue ?? "";
                    return Results.Ok(new { success = false, message = keyValue });
                }
                Guid userId = Guid.NewGuid();
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage = _authenticateService.ValidateModelvalues(register, true);

                if (responseMessage != null && responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    bool isSoapService = Convert.ToBoolean(_configuration["IsSOAPServiceInUsed"]);
                    bool isRegistrationSuccessful = false;
                    RestResponseMessageApp responeMessageRegistrationApp = new RestResponseMessageApp();
                    string tempResponseMessage = string.Empty;
                    var respMessage = string.Empty;
                    var rpostRestService = new RpostRestService(_configuration);
                    HttpResponseMessage response = rpostRestService.RegisterUser(register.EmailId, register.PhoneNumber, register.Password, register.ComparePassword, register.FirstName, register.LastName, register.Language, register.TimeZone, register.IPAddress);
                    responeMessageRegistrationApp = JsonConvert.DeserializeObject<RestResponseMessageApp>(response.Content.ReadAsStringAsync().Result);
                    if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        isRegistrationSuccessful = true;
                        if (responeMessageRegistrationApp.Message.Count > 1)
                        {
                            for (int i = 0; i < responeMessageRegistrationApp.Message.Count; i++)
                            {
                                tempResponseMessage = tempResponseMessage + System.Environment.NewLine + responeMessageRegistrationApp.Message[i].Message;
                            }
                        }
                        else
                        {
                            tempResponseMessage = responeMessageRegistrationApp.Message[0].Message;
                        }
                    }
                    else
                    {
                        if (responeMessageRegistrationApp.Message.Count > 1)
                        {
                            for (int i = 0; i < responeMessageRegistrationApp.Message.Count; i++)
                            {
                                tempResponseMessage = tempResponseMessage + System.Environment.NewLine + responeMessageRegistrationApp.Message[i].Message;
                            }
                        }
                        else
                        {
                            tempResponseMessage = responeMessageRegistrationApp.Message[0].Message;
                        }

                        if (tempResponseMessage == "Email-User with this email already exists")
                        {
                            var userStatus = rpostRestService.RestUserStatus(register.EmailId);
                            if (!userStatus.IsSuccessStatusCode)
                            {
                                tempResponseMessage = "Email-User with this email already exists";
                            }
                            else
                            {
                                var result = JsonConvert.DeserializeObject<RestUserStatusResponse>(userStatus.Content.ReadAsStringAsync().Result);
                                if (result.ResultContent.RegisteredUser && !result.ResultContent.EmailConfirmed)
                                {
                                    tempResponseMessage = "It seems to be user is already registered, but not yet been activated.";
                                }
                            }
                        }
                        return Results.Ok(new { success = false, message = responeMessageRegistrationApp.Message[0].Message });
                    }

                    if (isRegistrationSuccessful)
                    {
                        Company companyObj = null;
                        if (responeMessageRegistrationApp.ResultContent != null && !string.IsNullOrEmpty(responeMessageRegistrationApp.ResultContent.ReferenceKey))
                        {
                            companyObj = _companyRepository.GetCompanyByReferenceKey(responeMessageRegistrationApp.ResultContent.ReferenceKey);
                            if (companyObj == null)
                            {
                                companyObj = new Company
                                {
                                    ID = Guid.NewGuid(),
                                    Referencekey = responeMessageRegistrationApp.ResultContent.ReferenceKey,
                                    Name = responeMessageRegistrationApp.ResultContent.CustomerName,
                                    Description = string.Empty,
                                    IsTransparencyFeatureOn = false,
                                    PostSigningLandingPage = string.Empty,
                                    AdminEmailID = string.Empty,
                                    IsActive = true,
                                    LogoPath = null,
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now
                                };

                                _companyRepository.Save(companyObj);
                            }
                            else
                            {
                                companyObj.Name = responeMessageRegistrationApp.ResultContent.CustomerName;
                                companyObj.ModifiedDate = DateTime.Now;
                                _companyRepository.Save(companyObj);
                            }
                            var userprofile = _userRepository.GetUserProfileByEmailID(register.EmailId);
                            if (userprofile == null)
                            {
                                var userProfile = new UserProfile
                                {
                                    ID = Guid.NewGuid(),
                                    UserID = userId,
                                    EmailID = register.EmailId,
                                    FirstName = register.FirstName,
                                    LastName = register.LastName,
                                    CompanyID = companyObj?.ID,
                                    IsActive = true,
                                    IsAutoPopulateSignaturewhileSinging = true,
                                    UserTypeID = Constants.UserType.USER,
                                    CreatedDateTime = DateTime.Now,
                                    ActiveFrom = DateTime.Now,
                                    LanguageCode = "en-us",
                                    status = Constants.UserType.Review
                                };
                                _userRepository.Save(userProfile);
                            }
                            else
                            {
                                userprofile.FirstName = register.FirstName;
                                userprofile.LastName = register.LastName;
                                userprofile.status = Constants.UserType.Review;
                                userprofile.IsAutoPopulateSignaturewhileSinging = true;
                                userprofile.LanguageCode = string.IsNullOrEmpty(userprofile.LanguageCode) ? "en-us" : userprofile.LanguageCode;
                                _userRepository.Save(userprofile);
                            }
                        }
                        return Results.Ok(new { success = true, message = responeMessageRegistrationApp.Message[0].Message });
                    }
                    return Results.Ok(new { success = true, message = responeMessageRegistrationApp.Message[0].Message });
                }
                else
                {
                    return Results.Ok(new { success = false, message = responseMessage.Message });
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "RegisterUser";
                loggerModelNew.Message = "API EndPoint - Exception at RegisterUser method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = responseToClient.StatusCode });
            }
        }

        public async Task<IResult> ForgotPassword(HttpRequest request, ForgotPasswordModal forgotPassword)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew(!string.IsNullOrEmpty(forgotPassword.UserEmailAddress) ? forgotPassword.UserEmailAddress : string.Empty, "AuthenticateEndPoint", "ForgotPassword", "Process started for get ForgotPassword", "", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                string soapForgotSuccessResponse = Convert.ToString(_configuration["ForgotSuccessResponse"]);
                bool isFogotPasswordSuccess = false;
                string forgotEmailAddress = !string.IsNullOrEmpty(forgotPassword.UserEmailAddress) ? forgotPassword.UserEmailAddress : string.Empty;
                int isSSORestricted = GetEnterpriseSSOWithEmail(forgotEmailAddress);

                if (isSSORestricted == 1)
                {
                    var lookupLanguageList = _lookupRepository.GetLookupLanguageList(Lookup.LanguageKeyDetails, forgotPassword.Language, Constants.String.languageKeyType.Validation).Where(a => a.ResourceKeyID == Constants.Resourcekey.lang_SSOForgotPwdMessage).ToList();
                    var keyValue = lookupLanguageList.FirstOrDefault()?.KeyValue ?? "";
                    return Results.Ok(new { success = false, message = keyValue });
                }
                if (isSSORestricted == 2)
                {
                    var lookupLanguageList = _lookupRepository.GetLookupLanguageList(Lookup.LanguageKeyDetails, forgotPassword.Language, Constants.String.languageKeyType.Validation).Where(a => a.ResourceKeyID == Constants.Resourcekey.lang_applicationError).ToList();
                    var keyValue = lookupLanguageList.FirstOrDefault()?.KeyValue ?? "";
                    return Results.Ok(new { success = false, message = keyValue });
                }

                RegistrationModal register = new RegistrationModal();
                register.EmailId = forgotPassword.UserEmailAddress;
                HttpResponseMessage responseToClient = new HttpResponseMessage();
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage = _authenticateService.ValidateModelvalues(register, false);

                if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    responseToClient.Content = new StringContent(responseMessage.Message, Encoding.Unicode);
                    responseToClient.StatusCode = HttpStatusCode.Forbidden;
                    return Results.Ok(new { success = false, message = responseMessage.Message });
                }

                string errorMessage = string.Empty;
                string languageCode = string.IsNullOrEmpty(forgotPassword.Language) ? "en-us" : forgotPassword.Language;
                bool isForgotPassword = false;
                HttpResponseMessage response = _rpostRestService.ForgotPassword(forgotPassword.UserEmailAddress, Convert.ToString(_configuration["FromEmailAddress"]), Convert.ToString(_configuration["Domain"]) + Convert.ToString(_configuration["ForgotPasswordDomainUrl"]) + languageCode + "&", Convert.ToString(_configuration["ForgotPasswordResetAccountName"]), languageCode);
                var rstRespMsg = JsonConvert.DeserializeObject<RestResponseMessage>(response.Content.ReadAsStringAsync().Result);

                if (rstRespMsg != null && rstRespMsg.StatusCode == "400")
                {
                    if (rstRespMsg.Message != null && rstRespMsg.Message.Count > 0)
                        errorMessage = rstRespMsg.Message[0].Message;
                }

                if (response.IsSuccessStatusCode)
                    isForgotPassword = true;

                if (isForgotPassword)
                {
                    string message = Convert.ToString(_configuration["ForgotPasswordSuccess"]);
                    responseMessage.Message = message.Replace("#UserEmail", forgotPassword.UserEmailAddress);
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    return Results.Ok(new { success = true, message = responseMessage.Message });
                }
                else
                {
                    responseMessage.Message = !string.IsNullOrEmpty(errorMessage) ? errorMessage : Convert.ToString(_configuration["ForgotPasswordFailure"]);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    return Results.Ok(new { success = false, message = responseMessage.Message });
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "ForgotPassword";
                loggerModelNew.Message = "API EndPoint - Exception at ForgotPassword method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                HttpResponseMessage responseToClient = new HttpResponseMessage();
                responseToClient.StatusCode = (HttpStatusCode)422;
                responseToClient.Content = new StringContent(ex.Message, Encoding.Unicode);
                return Results.BadRequest(responseToClient);
            }
        }

        [AllowAnonymous]
        public async Task<IResult> ResetPassword(HttpRequest request, ModifyUser updateuserModel)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew(string.Empty, "AuthenticateEndPoint", "ResetPassword", "Process started ResetPassword method", "", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            if (updateuserModel == null || string.IsNullOrEmpty(updateuserModel.Password) || string.IsNullOrEmpty(updateuserModel.ComparePassword) || string.IsNullOrEmpty(updateuserModel.ResetCode))
            {
                loggerModelNew.Message = "Invalid input parameters.";
                rSignLogger.RSignLogWarn(loggerModelNew);
                return Results.BadRequest(new { Error = "Invalid input parameters." });
            }

            try
            {
                var restResponse = await _rpostRestService.ResetPassword(updateuserModel.Password, updateuserModel.ComparePassword, updateuserModel.ResetCode);

                if (restResponse == null)
                {
                    loggerModelNew.Message = "Null response received from ResetPassword service.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.BadRequest(new { Error = "Service returned a null response." });
                }

                var jsonResponse = await restResponse.Content?.ReadAsStringAsync();
                if (string.IsNullOrEmpty(jsonResponse))
                {
                    loggerModelNew.Message = "Empty response content received from GetTimeZone API.";
                    rSignLogger.RSignLogError(loggerModelNew);
                    return Results.Ok(new { statusCode = 500, success = false, message = "Received empty response." });
                }

                var resetPasswordResponse = JsonConvert.DeserializeObject<RestResponseMessage>(jsonResponse);

                if (resetPasswordResponse == null)
                {
                    loggerModelNew.Message = "Deserialization of resetPasswordResponse failed.";
                    rSignLogger.RSignLogError(loggerModelNew);
                    return Results.Ok(new { statusCode = 500, success = false, message = "Invalid reset Password Response." });
                }

                if (resetPasswordResponse.StatusCode == "200")             
                    return Results.Ok(new { success = true, response = resetPasswordResponse, message = resetPasswordResponse.Message[0].Message });               
                else
                    return Results.Ok(new { success = false, response = resetPasswordResponse, message = resetPasswordResponse.Message[0].Message });
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "ResetPassword";
                loggerModelNew.Message = "API EndPoint - Exception at ResetPassword method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.Problem("An unexpected error occurred. Please try again later.");
            }
        }

        [AllowAnonymous]
        public async Task<IResult> GetTimeZone()
        {
            loggerModelNew = new LoggerModelNew(string.Empty, "AuthenticateEndPoint", "GetTimeZone", "Process started GetTimeZone method", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var restResponse = await _rpostRestService.GetTimeZone();

                if (restResponse == null)
                {
                    loggerModelNew.Message = "Failed to get response from GetTimeZone API.";
                    rSignLogger.RSignLogError(loggerModelNew);
                    return Results.Json(new { statusCode = 500, message = "Failed to retrieve time zone data." }, statusCode: 500);
                }

                var jsonResponse = await restResponse.Content?.ReadAsStringAsync();

                if (string.IsNullOrEmpty(jsonResponse))
                {
                    loggerModelNew.Message = "Empty response content received from GetTimeZone API.";
                    rSignLogger.RSignLogError(loggerModelNew);
                    return Results.Json(new { statusCode = 500, message = "Received empty response." }, statusCode: 500);
                }

                var responseGetTimeZone = JsonConvert.DeserializeObject<RestResponseMessage>(jsonResponse)?.ResultContent;
                if (responseGetTimeZone == null)
                {
                    loggerModelNew.Message = "Deserialization of GetTimeZone response failed.";
                    rSignLogger.RSignLogError(loggerModelNew);
                    return Results.Json(new { statusCode = 500, message = "Invalid response format received." }, statusCode: 500);
                }
                var response = new
                {
                    statusCode = (int)restResponse.StatusCode,
                    data = responseGetTimeZone
                };

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "GetTimeZone";
                loggerModelNew.Message = "API EndPoint - Exception at GetTimeZone method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.Json(new { statusCode = 500, message = "An unexpected error occurred. Please try again later." }, statusCode: 500);
            }
        }

        [AllowAnonymous]
        public async Task<IResult> GetLanguage()
        {
            loggerModelNew = new LoggerModelNew(string.Empty, "AuthenticateEndPoint", "GetLanguage", "Process started GetLanguage method", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var restResponse = await _rpostRestService.GetLanguage();
                if (restResponse == null)
                {
                    loggerModelNew.Message = "Failed to get response from GetLanguage API.";
                    rSignLogger.RSignLogError(loggerModelNew);
                    return Results.Json(new { statusCode = HttpStatusCode.BadRequest, message = "Failed to retrieve time zone data." }, statusCode: 500);
                }

                var jsonResponse = await restResponse.Content?.ReadAsStringAsync();

                if (string.IsNullOrEmpty(jsonResponse))
                {
                    loggerModelNew.Message = "Empty response content received from GetLanguage API.";
                    rSignLogger.RSignLogError(loggerModelNew);
                    return Results.Json(new { statusCode = HttpStatusCode.BadRequest, message = "Received empty response." }, statusCode: 500);
                }

                var responseGetLanguage = JsonConvert.DeserializeObject<RestResponseMessage>(jsonResponse)?.ResultContent.OrderBy(lang => lang.Description).ToList();
                if (responseGetLanguage == null)
                {
                    loggerModelNew.Message = "Deserialization of GetLanguage response failed.";
                    rSignLogger.RSignLogError(loggerModelNew);
                    return Results.Json(new { statusCode = 500, message = "Invalid response format received." }, statusCode: 500);
                }
                var response = new
                {
                    statusCode = (int)restResponse.StatusCode,
                    data = responseGetLanguage
                };

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "GetLanguage";
                loggerModelNew.Message = "API EndPoint - Exception at GetLanguage method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.Json(new { statusCode = 500, message = "An unexpected error occurred. Please try again later." }, statusCode: 500);
            }

        }

        /// <summary>
        /// This api is reffering to Validate a Token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userTokenModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IResult> ValidateUserTokenEnvelopeDetails(HttpRequest request, UserEnvelopeTokenModel userEnvelopeTokenModel)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateEndPoints", "GetUserSettingsRoleDetails", "Method Initialized for Validate User Token and Envelope Details", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            AuthenticateResponseMessageModel responseToken = new AuthenticateResponseMessageModel();
            try
            {
                responseToken = await _authenticateService.ValidateUserTokenEnvelopeDetails(request, userEnvelopeTokenModel);
                return Results.Ok(responseToken);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at ValidateUserTokenEnvelopeDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = responseToken.StatusCode, AuthMessage = ex.Message, AuthToken = string.Empty });
            }
        }
        private int GetEnterpriseSSOWithEmail(string forgotEmailAddress)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateEndPoints", "GetEnterpriseSSOWithEmail", "Method Initialized for Validate User Token and Envelope Details", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                HttpResponseMessage restSSOResponse = _rpostRestService.GetEnterpriseSSOWithEmail(forgotEmailAddress);
                if (!restSSOResponse.IsSuccessStatusCode)
                    return 2;

                var content = restSSOResponse.Content.ReadAsStringAsync().Result;
                var resultRestAuth = JsonConvert.DeserializeObject<EnterpriseSSORestResponseMessage>(content);

                if (resultRestAuth?.ResultContent != null)
                {
                    if (resultRestAuth.ResultContent.SSORst == 1)
                        return 1;
                    else
                        return 0;
                }

                return 2;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetEnterpriseSSOWithEmail method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return 2;
            }
        }

        /// <summary>
        /// To retrieve the Review Detail value details
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IResult> ReviewDetail(ReviewDetailModal ReviewDetail)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateEndPoints", "ReviewDetail", "Method is Initialized for Review Detail.", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            string UsersTimeZoneValue = string.Empty;
            try
            {
                if (ReviewDetail.userEmail != null)
                {
                    RestResponseUserInfo rSignPlan = _authenticateRepository.GetUserInfoFromRCS(ReviewDetail.userToken, ReviewDetail.userEmail);
                    if (rSignPlan.ResultContent == null)
                    {
                        string message = _configuration["NoPlanAssociatedForUser"];
                        return Results.BadRequest(new { success = false, message = message });
                    }
                    if (rSignPlan.ResultContent.Plan == null)
                    {
                        rSignPlan.ResultContent.Plan = new Plan();
                        rSignPlan.ResultContent.Plan.Name = _configuration["NoPlanAssociatedForUser"];
                    }
                    if (rSignPlan.ResultContent.Customer == null)
                    {
                        string message = _configuration["NotAssociatedWithCompany"];
                        return Results.BadRequest(new { success = false, message = message });
                    }
                    HttpResponseMessage responseTimeZone = await _rpostRestService.GetTimeZone();
                    if (responseTimeZone.IsSuccessStatusCode)
                    {

                        var jsonResponse = await responseTimeZone.Content?.ReadAsStringAsync();
                        var responseGetTimeZone = JsonConvert.DeserializeObject<RestResponseMessage>(jsonResponse)?.ResultContent;
                        if (!string.IsNullOrEmpty(rSignPlan.ResultContent.TimeZone))
                            UsersTimeZoneValue = responseGetTimeZone.Where(x => x.Code == rSignPlan.ResultContent.TimeZone).Select(x => x.Description).FirstOrDefault();
                        else
                            UsersTimeZoneValue = string.Empty;
                    }
                    return Results.Ok(new { success = true, response = rSignPlan, usersTimeZoneValue = UsersTimeZoneValue });
                }
                return Results.BadRequest(new { success = false });
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at ReviewDetail method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
            }
            return Results.BadRequest(new { success = false });
        }
        /// <summary>
        /// To update the Review Flag value details
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IResult> UpdateReviewFlagDetailsByUserId(UserReviewModel userReviewModel)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateEndPoints", "UpdateReviewFlagDetailsByUserId", "Method is Initialized to update review flag by UserId", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            UserResponseMessage responseMessage = new();
            try
            {
                responseMessage = _authenticateService.UpdateReviewDetails(userReviewModel);
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at UpdateReviewFlagDetailsByUserId method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
            }
            return Results.BadRequest(new { success = false });
        }


        [AllowAnonymous]
        public async Task<IResult> GetMFAAndPasswordPolicySettings(HttpRequest request, string emailAddress)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew(!string.IsNullOrEmpty(emailAddress) ? emailAddress : string.Empty, "AuthenticateEndPoint", "GetMFAAndPasswordPolicySettings", "Process started GetMFAAndPasswordPolicySettings method", "", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            MFAAndPasswordResponseMessage responseMessage = new MFAAndPasswordResponseMessage();
            HttpResponseMessage restResponse = new HttpResponseMessage();

            try
            {
                //RpostRestService rpostRestApi = new RpostRestService(_configuration);
                responseMessage = await _authenticateService.GetMFAandPasswordPolicySetting(emailAddress);
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateEndPoint";
                loggerModelNew.Method = "GetMFAAndPasswordPolicySettings";
                loggerModelNew.Message = "API EndPoint - Exception at GetMFAAndPasswordPolicySettings method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new { BadRequest = restResponse.StatusCode });
            }
        }
    }
}
