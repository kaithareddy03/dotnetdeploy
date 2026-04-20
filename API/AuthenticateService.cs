using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Models;
using RSign.Models.Repository;
using RSign.Notification;
using System.Net;
using System.Web;
using static RSign.Common.Helpers.Constants;
using ResultContent = RSign.Models.ResultContent;

namespace RSign.SendAPI.API
{
    public class AuthenticateService
    {
        RSignLogger rSignLogger = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _configuration;
        private IUserRepository _userRepository;
        private IUserTokenRepository _userTokenRepository;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private readonly ICompanyRepository _companyRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IAuthenticateRepository _authenticateRepository;
        private readonly IDistributedCache _cache;
        private readonly ILookupRepository _lookupRepository;
        private readonly IOptions<AppSettingsConfig> _dbConfiguration;
        private readonly IESignHelper _esignHelper;
        private IValidationRepository _validationRepository;
        private readonly IGenericRepository _genericRepository;
        private readonly IEnvelopeRepository _envelopeRepository;

        public AuthenticateService(IConfiguration configuration, IUserRepository userRepository, IUserTokenRepository userTokenRepository, IEnvelopeHelperMain envelopeHelperMain, IOptions<AppSettingsConfig> dbConfiguration,
            ICompanyRepository companyRepository, ISettingsRepository settingsRepository, IAuthenticateRepository authenticateRepository, IDistributedCache cache, ILookupRepository lookupRepository, IESignHelper eSignHelper,
            IValidationRepository validationRepository, IGenericRepository genericRepository, IEnvelopeRepository envelopeRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _envelopeHelperMain = envelopeHelperMain;
            _companyRepository = companyRepository;
            _settingsRepository = settingsRepository;
            _authenticateRepository = authenticateRepository;
            _cache = cache;
            _lookupRepository = lookupRepository;
            rSignLogger = new RSignLogger(_configuration);
            _dbConfiguration = dbConfiguration;
            _esignHelper = eSignHelper;
            _validationRepository = validationRepository;
            _genericRepository = genericRepository;
            _envelopeRepository = envelopeRepository;
        }

        public async Task<ResponseTokenWithEmailId> DecryptValidateTokenService(PreparePageUrl userPrepareUrl)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateService", "DecryptValidateTokenService", "Method Initialized", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            string sourceUrlType = string.Empty;
            ResponseTokenWithEmailId responseToken = new ResponseTokenWithEmailId();
            try
            {
                string prepareURL = userPrepareUrl.PrepareUrl;
                prepareURL = HttpUtility.UrlDecode(prepareURL);
                string authToken = string.Empty; string prepareType = string.Empty; string emailId = string.Empty, disableButtonStr = string.Empty, isActingUserPerformingAction = "false";
                var envelopeId = "";
                if (!prepareURL.Equals(""))
                {
                    prepareURL = EncryptDecryptQueryString.Decrypt(prepareURL, Convert.ToString(_configuration["AppKey"]));
                    if (!prepareURL.Equals("Invalid length for a Base-64 char array or string.") && !prepareURL.Equals("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters. ") && !prepareURL.Equals("Length of the data to decrypt is invalid.") && !prepareURL.Equals("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters."))
                    {
                        string[] arrayURL = prepareURL.Split('&');

                        if (arrayURL.Length == 6)
                        {
                            string[] arrayID = arrayURL[0].Split('=');
                            authToken = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[1].Split('=');
                            envelopeId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[2].Split('=');
                            prepareType = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[3].Split('=');
                            emailId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[4].Split('=');
                            sourceUrlType = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[5].Split('=');
                            isActingUserPerformingAction = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        }
                        else if (arrayURL.Length == 5)
                        {
                            string[] arrayID = arrayURL[0].Split('=');
                            authToken = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[1].Split('=');
                            envelopeId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[2].Split('=');
                            prepareType = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[3].Split('=');
                            emailId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[4].Split('=');
                            sourceUrlType = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        }
                        else if (arrayURL.Length == 4)
                        {
                            string[] arrayID = arrayURL[0].Split('=');
                            authToken = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[1].Split('=');
                            envelopeId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[2].Split('=');
                            prepareType = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                            arrayID = arrayURL[3].Split('=');
                            emailId = arrayID.Length == 2 ? arrayID[1].Trim() : string.Empty;
                        }

                        loggerModelNew = new LoggerModelNew("", "AuthenticateService", "DecryptValidateTokenService", "Prepare page url decypted values", envelopeId);
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        if (!string.IsNullOrEmpty(authToken) && !string.IsNullOrEmpty(emailId))
                        {
                            loggerModelNew.Message = "DecryptValidateToken - authToken: " + authToken + " emailId: " + emailId;
                            rSignLogger.RSignLogInfo(loggerModelNew);

                            UserProfile userprofile = _userRepository.ValidateToken(authToken, emailId);
                            if (userprofile == null || userprofile.ID == Guid.Empty)
                            {
                                loggerModelNew.Message = "User Profile is not available and Unauthorized from ValidateToken";
                                rSignLogger.RSignLogInfo(loggerModelNew);
                                responseToken.StatusCode = HttpStatusCode.Unauthorized;
                                responseToken.AuthMessage = "Invalid Token.";
                                responseToken.AuthToken = authToken;
                                responseToken.EmailId = emailId;
                                responseToken.EnvelopeId = "";
                                responseToken.PrepareType = prepareType;
                                responseToken.LanguageKeyTranslations = null;
                                responseToken.SourceUrlType = sourceUrlType;
                                return responseToken;
                            }
                            else
                            {
                                UserViewModel userViewModel = new UserViewModel();
                                userViewModel.UserID = userprofile.UserID;
                                userViewModel.Company = userprofile.Company;
                                userViewModel.CompanyID = userprofile.CompanyID;
                                userViewModel.FirstName = userprofile.FirstName;
                                userViewModel.LastName = userprofile.LastName;
                                userViewModel.EmailID = userprofile.EmailID;
                                userViewModel.LanguageCode = userprofile.LanguageCode;
                                userViewModel.IsActive = userprofile.IsActive;
                                userViewModel.UserTypeID = userprofile.UserTypeID;

                                var userRolesDetails = _userRepository.GetUserRoleMappingList();
                                var data = userRolesDetails.Any(u => u.UserId == userprofile.UserID && u.RoleName == Constants.UsersRoles.LanguageTranslator);
                                if (data)
                                    userViewModel.IsLanguageTranslator = true;
                                else
                                    userViewModel.IsLanguageTranslator = false;

                                TokenViewModel tokenViewModel = new TokenViewModel();
                                tokenViewModel.AccessTokenExpires = userprofile.ExpiresIn;
                                tokenViewModel.RefreshToken = userprofile.RefreshToken;
                                tokenViewModel.RefreshExpires = userprofile.RefreshTokenExpiresOn;

                                LanguageKeyValuesResponseMessage languageKeyValuesResponse = await GetLanguageKeyDetails(userprofile.LanguageCode);

                                loggerModelNew.Message = "User view model retrived.";
                                rSignLogger.RSignLogInfo(loggerModelNew);
                                responseToken.StatusCode = HttpStatusCode.OK;
                                responseToken.AuthMessage = "Token validated.";
                                responseToken.AuthToken = authToken;
                                responseToken.EmailId = emailId;
                                responseToken.UserViewModel = userViewModel;
                                responseToken.EnvelopeId = envelopeId;
                                responseToken.PrepareType = prepareType;
                                responseToken.LanguageKeyTranslations = languageKeyValuesResponse;
                                responseToken.SourceUrlType = sourceUrlType;
                                responseToken.TokenViewModel = tokenViewModel;
                                responseToken.IsActingUserPerformingAction = isActingUserPerformingAction;
                                return responseToken;
                            }
                        }
                    }
                }
                loggerModelNew.Message = "Prepare page url is not valid.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseToken.StatusCode = HttpStatusCode.NotFound;
                responseToken.AuthMessage = "Prepare page url is not valid.";
                responseToken.AuthToken = authToken;
                responseToken.SourceUrlType = sourceUrlType;
                return responseToken;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseToken.StatusCode = HttpStatusCode.BadRequest;
                responseToken.AuthMessage = ex.Message;
                responseToken.SourceUrlType = sourceUrlType;
                return responseToken;
            }
        }

        public async Task<LanguageKeyValuesResponseMessage> GetLanguageKeyDetails(string languageCode)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateService", "GetLanguageKeyDetails", "GetLanguageKeyDetails Method Initialized", "");
            rSignLogger.RSignLogInfo(loggerModelNew);

            LanguageKeyValuesResponseMessage languageKeyValuesResponse = new LanguageKeyValuesResponseMessage();
            try
            {
                string strobjLanguageInfo = string.Empty;
                Dictionary<string, string> listdicionaryData;

                //RedisValue languageDetailsList = await _cache.GetAsync("languageDetails");
                //if (languageDetailsList.HasValue)
                //{
                //    //RSignLanguageDetails languageDetails = new RSignLanguageDetails();
                //    listdicionaryData = JsonConvert.DeserializeObject<Dictionary<string, string>>(languageDetailsList)!;

                //    loggerModelNew.Message = "Language keys retrieved successfully.";
                //    rSignLogger.RSignLogInfo(loggerModelNew);
                //    languageKeyValuesResponse.StatusCode = HttpStatusCode.OK;
                //    languageKeyValuesResponse.LanguageKeyTranslation = listdicionaryData;
                //    //  languageKeyValuesResponse.languageKeyInfos = listdicionaryData.Select(p => new LanguageKeyInfo { Key = p.Key, Value = p.Value }).ToList();
                //    languageKeyValuesResponse.Message = "Language keys retrieved successfully.";
                //    return languageKeyValuesResponse;
                //}
                //else
                //{
                //    var LanguagelayoutList = _lookupRepository.GetLanguageKeyDetailsFromJson(languageCode);
                //    strobjLanguageInfo = JsonConvert.SerializeObject(LanguagelayoutList);
                //    await _cache.SetStringAsync("languageDetails", strobjLanguageInfo);
                //    //var ll = JsonConvert.DeserializeObject<Dictionary<string, string>>(languageInfo)!;
                //    listdicionaryData = JsonConvert.DeserializeObject<Dictionary<string, string>>(strobjLanguageInfo)!;
                //    loggerModelNew.Message = "Language keys retrieved successfully.";
                //    rSignLogger.RSignLogInfo(loggerModelNew);
                //    languageKeyValuesResponse.StatusCode = HttpStatusCode.OK;
                //    languageKeyValuesResponse.LanguageKeyTranslation = listdicionaryData;
                //    // languageKeyValuesResponse.languageKeyInfos = listdicionaryData.Select(p => new LanguageKeyInfo { Key = p.Key, Value = p.Value }).ToList();
                //    languageKeyValuesResponse.Message = "Language keys retrieved successfully.";
                //    return languageKeyValuesResponse;
                //}

                var LanguagelayoutList = _lookupRepository.GetLanguageKeyDetailsFromJson(languageCode);
                strobjLanguageInfo = JsonConvert.SerializeObject(LanguagelayoutList);
                listdicionaryData = JsonConvert.DeserializeObject<Dictionary<string, string>>(strobjLanguageInfo)!;
                loggerModelNew.Message = "Language keys retrieved successfully.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                languageKeyValuesResponse.StatusCode = HttpStatusCode.OK;
                languageKeyValuesResponse.LanguageKeyTranslation = listdicionaryData;
                languageKeyValuesResponse.Message = "Language keys retrieved successfully.";
                return languageKeyValuesResponse;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                languageKeyValuesResponse.StatusCode = HttpStatusCode.BadRequest;
                languageKeyValuesResponse.Message = ex.Message;
                return languageKeyValuesResponse;
            }
        }

        public async Task<UserRefreshTokenResponseMessage> GetRefreshToken(RefreshTokenObj userRefreshTokenModel)
        {
            loggerModelNew = new LoggerModelNew(userRefreshTokenModel.EmailAddress, "AuthenticateService", "GetRefreshToken", "Process started for GetRefreshToken", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            UserRefreshTokenResponseMessage responseUserMessage = new UserRefreshTokenResponseMessage();

            try
            {
                string refreshToken = userRefreshTokenModel.RefreshToken;
                string refreshExpires = userRefreshTokenModel.RefreshExpires;
                string accessTokenExpires = userRefreshTokenModel.AccessTokenExpires;
                string browserName = userRefreshTokenModel.BrowserName;

                if (!string.IsNullOrEmpty(userRefreshTokenModel.AuthToken) && !string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(refreshExpires)
                    && !string.IsNullOrEmpty(accessTokenExpires) && Convert.ToDateTime(accessTokenExpires).AddSeconds(-900) < DateTime.UtcNow)
                {
                    UserTokenModel userModel = new UserTokenModel();
                    userModel.EmailAddress = userRefreshTokenModel.EmailAddress;
                    userModel.RefreshToken = refreshToken;

                    string RPostAuthUrl = Convert.ToString(_configuration["RPostAuthUrl"]);
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(RPostAuthUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    RefreshTokenRequestModel refreshTokenRequestModel = new RefreshTokenRequestModel(refreshToken, userRefreshTokenModel.AuthToken);
                    HttpResponseMessage responseRefreshTokenMessage = client.PostAsJsonAsync("api/v1/auth/RefreshToken", refreshTokenRequestModel).Result;

                    var resultRestAuth = JsonConvert.DeserializeObject<RPostRefreshTokenStatusResponse>(responseRefreshTokenMessage.Content.ReadAsStringAsync().Result);
                    if (!responseRefreshTokenMessage.IsSuccessStatusCode)
                    {
                        responseUserMessage.StatusCode = HttpStatusCode.Forbidden;
                        responseUserMessage.StatusMessage = "Forbidden";
                        responseUserMessage.Message = resultRestAuth.Message.Message.ToString();
                        loggerModelNew.Message = "RefreshToken API Failed and error message is:" + responseUserMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return responseUserMessage;
                    }
                    else
                    {
                        RestResponseUserInfo rSignPlan = _authenticateRepository.GetUserInfoFromRCS(resultRestAuth.ResultContent.access_token, userRefreshTokenModel.EmailAddress);
                        if (rSignPlan == null || rSignPlan.ResultContent == null)
                        {
                            loggerModelNew.Message = "RefreshToken RSignPlan:" + _configuration["NotAssociatedPlan"];
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseUserMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseUserMessage.Message = _configuration["NotAssociatedPlan"];
                            return responseUserMessage;
                        }
                        else if (rSignPlan.ResultContent.Plan == null)
                        {
                            loggerModelNew.Message = "RefreshToken Plan:" + _configuration["NoPlanAssociatedForUser"];
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseUserMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseUserMessage.Message = _configuration["NoPlanAssociatedForUser"];
                            return responseUserMessage;
                        }
                        else if (rSignPlan.ResultContent.Customer == null)
                        {
                            loggerModelNew.Message = "RefreshToken Customer:" + _configuration["NotAssociatedWithCompany"];
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseUserMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseUserMessage.Message = _configuration["NotAssociatedWithCompany"];
                            return responseUserMessage;
                        }
                        else if (rSignPlan.ResultContent.Customer.ReferenceKey == null && rSignPlan.ResultContent.Customer.ReferenceKey == "")
                        {
                            loggerModelNew.Message = "RefreshToken ReferenceKey:" + _configuration["NotAssociatedWithCompany"];
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseUserMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseUserMessage.Message = _configuration["NotAssociatedWithCompany"];
                            return responseUserMessage;
                        }
                        else if (rSignPlan.ResultContent.Status == "Disabled" || rSignPlan.ResultContent.Status == "Deleted")
                        {
                            loggerModelNew.Message = "RefreshToken Disabled:" + _configuration["NotAssociatedWithCompany"];
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseUserMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseUserMessage.Message = rSignPlan.ResultContent.Status == "Disabled" ? "User Not Activated" : "User Not Found";
                            return responseUserMessage;
                        }

                        var userprofile = _userRepository.GetUserProfileByEmailID(userRefreshTokenModel.EmailAddress);
                        if (userprofile != null && userprofile.IsActive == true)
                        {
                            loggerModelNew.Message = "RefreshToken got user profile details for user:" + userRefreshTokenModel.EmailAddress;
                            rSignLogger.RSignLogInfo(loggerModelNew);

                            string tokenRefKey = ModelHelper.CalculateMD5Hash(browserName + "-" + userModel.EmailAddress + "-" + ModelHelper.GetTimestampMilliseconds());
                            UserToken token = new UserToken();
                            token.ID = Guid.NewGuid();
                            token.UserID = userprofile.UserID;
                            token.EmailId = userprofile.EmailID;
                            token.LastUpdated = DateTime.UtcNow;
                            token.ExpiresIn = DateTime.UtcNow.AddSeconds(Convert.ToInt32(resultRestAuth.ResultContent.expires_in));
                            token.AuthToken = resultRestAuth.ResultContent.access_token;
                            token.BrowserName = browserName;
                            token.IPAddress = userRefreshTokenModel.IPAddress;
                            token.ReferenceKey = tokenRefKey;
                            token.RefreshToken = userModel.RefreshToken;
                            if (userModel.RefreshToken != null)
                            {
                                token.RefreshToken = userModel.RefreshToken;
                                token.RefreshTokenExpiresOn = Convert.ToDateTime(refreshExpires);
                                token.TokenType = "jwt";
                            }
                            _userTokenRepository.Save(token);

                            loggerModelNew.Message = "RefreshToken Saved successfully new token details for user:" + userRefreshTokenModel.EmailAddress;
                            rSignLogger.RSignLogInfo(loggerModelNew);

                            UserTokenModel tokenModel = new UserTokenModel();
                            tokenModel.RefreshToken = userModel.RefreshToken;
                            tokenModel.RefreshExpires = Convert.ToString(token.RefreshTokenExpiresOn);
                            tokenModel.AccessTokenExpires = Convert.ToString(token.ExpiresIn);
                            tokenModel.AuthToken = token.AuthToken;

                            responseUserMessage.IsNewToken = true;
                            responseUserMessage.UserTokenModel = tokenModel;

                            loggerModelNew.Message = "RefreshToken Completed for user:" + userRefreshTokenModel.EmailAddress;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                        }
                    }
                }
                else if (userRefreshTokenModel.IsFirstTime)
                {
                    using (var dbContext = new RSignDbContext(_dbConfiguration))
                    {
                        if (!string.IsNullOrEmpty(userRefreshTokenModel.AuthToken) && dbContext.UserToken.Any(u => u.AuthToken == userRefreshTokenModel.AuthToken && DateTime.UtcNow < u.ExpiresIn))
                        {
                            var tokenData = ((from t in dbContext.UserToken
                                              where t.AuthToken == userRefreshTokenModel.AuthToken
                                              && DateTime.UtcNow < t.ExpiresIn && t.EmailId == userRefreshTokenModel.EmailAddress
                                              select new
                                              {
                                                  Email = t.EmailId,
                                                  LastUpdated = t.LastUpdated,
                                                  RefreshToken = t.RefreshToken,
                                                  RefreshTokenExpiresOn = t.RefreshTokenExpiresOn,
                                                  AuthToken = t.AuthToken,
                                                  AuthTokenExpiresIn = t.ExpiresIn
                                              }).OrderByDescending(o => o.LastUpdated)).FirstOrDefault();

                            if (tokenData != null && tokenData.Email != null)
                            {
                                UserTokenModel tokenModel = new UserTokenModel();
                                tokenModel.RefreshToken = tokenData.RefreshToken;
                                tokenModel.RefreshExpires = Convert.ToString(tokenData.RefreshTokenExpiresOn);
                                tokenModel.AccessTokenExpires = Convert.ToString(tokenData.AuthTokenExpiresIn);
                                tokenModel.AuthToken = tokenData.AuthToken;

                                responseUserMessage.UserTokenModel = tokenModel;
                            }
                        }
                    }
                    responseUserMessage.IsNewToken = false;

                    loggerModelNew.Message = "RefreshToken IsFirstTime for user true completed for:" + userRefreshTokenModel.EmailAddress;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }
                responseUserMessage.StatusCode = HttpStatusCode.OK;
                responseUserMessage.StatusMessage = "Success";
                responseUserMessage.Message = "Success";
                loggerModelNew.Message = "Completed the process of Getting Refresh Token and message is:" + responseUserMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return responseUserMessage;
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateService";
                loggerModelNew.Method = "RefreshToken";
                responseUserMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseUserMessage.StatusMessage = "ServiceUnavailable";
                responseUserMessage.Message = Convert.ToString(_configuration["RpostRefreshTokenFailure"]) + " and error message is:" + ex.ToString();
                loggerModelNew.Message = "Got error at RefreshToken for user: " + userRefreshTokenModel.EmailAddress + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return responseUserMessage;
            }
        }

        #region commnetd
        public async Task<ResponseTokenWithEmailId> AuthenticateUserService(UserLoginToken objUser)
        {
            loggerModelNew = new LoggerModelNew(!string.IsNullOrEmpty(objUser.EmailId) ? objUser.EmailId : string.Empty, "AuthenticateService", "AuthenticateUserService", "Process started in AuthenticateUserService", "");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseTokenWithEmailId respToken = new ResponseTokenWithEmailId();
            try
            {
                RpostRestService rpostRestService = new RpostRestService(_configuration);
                string responseToken = string.Empty;
                string restResponse = rpostRestService.AuthenticateUser("password", HttpUtility.UrlEncode(objUser.EmailId), HttpUtility.UrlEncode(objUser.Password));
                responseToken = restResponse;
                loggerModelNew.Message = "Authenticate user token retrived succefully.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                if (!string.IsNullOrEmpty(responseToken))
                {
                    var userProfileExists = _userRepository.GetUserProfileByEmailID(objUser.EmailId);

                    UserToken objToken = new UserToken();
                    objToken.ID = Guid.NewGuid();
                    objToken.UserID = userProfileExists.UserID;
                    objToken.EmailId = objUser.EmailId;
                    objToken.AuthToken = responseToken;
                    objToken.LastUpdated = DateTime.Now;
                    objToken.ExpiresIn = DateTime.Now.AddDays(Convert.ToDouble(_configuration["DefaultTokenExpiryDays"]));
                    objToken.IPAddress = objUser.IPAddress;
                    objToken.BrowserName = "API LogIn";

                    bool result = _userTokenRepository.Save(objToken);
                    loggerModelNew.Message = "User Token details saved successfully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    if (!string.IsNullOrEmpty(objUser.Source) && userProfileExists.status == Constants.UserType.NewUser)
                    {
                        RestResponseUserInfo rSignPlan = _authenticateRepository.GetUserInfoFromRCS(responseToken, objUser.EmailId);
                        if (rSignPlan == null || rSignPlan.ResultContent == null)
                        {
                            loggerModelNew.Message = respToken.AuthMessage;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            respToken.StatusCode = HttpStatusCode.NotFound;
                            respToken.AuthMessage = _configuration["NotAssociatedPlan"];
                            respToken.AuthToken = responseToken;
                            respToken.EmailId = objUser.EmailId;
                            return respToken;
                        }
                        else if (rSignPlan.ResultContent.Plan == null)
                        {
                            loggerModelNew.Message = respToken.AuthMessage;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            respToken.StatusCode = HttpStatusCode.NotFound;
                            respToken.AuthMessage = _configuration["NoPlanAssociatedForUser"];
                            respToken.AuthToken = responseToken;
                            respToken.EmailId = objUser.EmailId;
                            return respToken;
                        }
                        else if (rSignPlan.ResultContent.Customer == null)
                        {
                            loggerModelNew.Message = respToken.AuthMessage;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            respToken.StatusCode = HttpStatusCode.NotFound;
                            respToken.AuthMessage = _configuration["NotAssociatedWithCompany"];
                            respToken.AuthToken = responseToken;
                            respToken.EmailId = objUser.EmailId;
                            return respToken;
                        }

                        var userReviewModel = new UserReviewModel
                        {
                            EmailID = objUser.EmailId,
                            companyName = rSignPlan.ResultContent.Customer.Name,
                            companyReferenceKey = rSignPlan.ResultContent.Customer.ReferenceKey,
                            IsCompanyStatus = rSignPlan.ResultContent.Customer.Status == Constants.UserStatusOptions.Active ? true : false,
                            IsActive = rSignPlan.ResultContent.Status == Constants.UserStatusOptions.Active ? true : false,
                            TimeZone = rSignPlan.ResultContent.TimeZone
                        };

                        UserProfile userprofile = new UserProfile();
                        userReviewModel.companyReferenceKey = userReviewModel.companyReferenceKey.Trim();
                        Company objCompany = _companyRepository.GetUserCompanyInfo(userReviewModel.companyReferenceKey);
                        userprofile = _userRepository.GetUserProfileByEmailID(userReviewModel.EmailID);
                        Guid? oldCompanyId = userprofile.CompanyID;

                        if (objCompany.ID == Guid.Empty)
                        {
                            Company company = new Company();
                            company.ID = Guid.NewGuid();
                            company.Referencekey = userReviewModel.companyReferenceKey;
                            company.Name = userReviewModel.companyName;
                            company.Description = string.Empty;
                            company.IsTransparencyFeatureOn = false;
                            company.PostSigningLandingPage = string.Empty;
                            company.AdminEmailID = string.Empty;
                            company.IsActive = userReviewModel.IsCompanyStatus;
                            //company.LogoPath = null;
                            company.CreatedDate = DateTime.Now;
                            company.ModifiedDate = DateTime.Now;
                            _companyRepository.Save(company);
                            loggerModelNew.Message = "Company details saved successfully.";
                            rSignLogger.RSignLogInfo(loggerModelNew);

                            string outputCompanyID = Convert.ToString(company.ID);
                            if (outputCompanyID != "" && outputCompanyID != null)
                            {
                                userprofile.status = Constants.UserType.Review;
                                userprofile.LanguageCode = string.IsNullOrEmpty(userprofile.LanguageCode) ? Constants.Language.English : userprofile.LanguageCode;
                                userprofile.IsActive = userReviewModel.IsActive;
                                userprofile.CompanyID = new Guid(outputCompanyID);
                                _userRepository.Save(userprofile);
                                loggerModelNew.Message = "User details saved successfully when company is empty.";
                                rSignLogger.RSignLogInfo(loggerModelNew);
                            }
                            else
                            {
                                loggerModelNew.Message = respToken.AuthMessage;
                                rSignLogger.RSignLogInfo(loggerModelNew);
                                respToken.StatusCode = HttpStatusCode.NotFound;
                                respToken.AuthMessage = _configuration["CompanyNotExists"];
                                respToken.AuthToken = responseToken;
                                respToken.EmailId = objUser.EmailId;
                                return respToken;
                            }
                        }
                        else
                        {
                            userprofile.status = Constants.UserType.Review;
                            userprofile.LanguageCode = string.IsNullOrEmpty(userprofile.LanguageCode) ? Constants.Language.English : userprofile.LanguageCode;
                            userprofile.IsActive = userReviewModel.IsActive;
                            userprofile.CompanyID = objCompany.ID;
                            _userRepository.Save(userprofile);
                            loggerModelNew.Message = "User details saved successfully when company is available.";
                            rSignLogger.RSignLogInfo(loggerModelNew);
                        }

                        if (userprofile.CompanyID.HasValue && userprofile.CompanyID != Guid.Empty && userprofile.CompanyID != oldCompanyId)
                        {
                            var companyData = _settingsRepository.GetEntityByCompanyID(userprofile.CompanyID.Value);
                            if (companyData.Count() > 0)
                            {
                                Dictionary<Guid, KeyPairItem> settings = new Dictionary<Guid, KeyPairItem>();
                                foreach (var setting in companyData)
                                {
                                    if (setting.KeyConfig == Constants.SettingsKeyConfig.TimeZone &&
                                        !string.IsNullOrEmpty(userReviewModel.TimeZone) && setting.IsOverride.HasValue && !setting.IsOverride.Value)
                                        setting.OptionValue = userReviewModel.TimeZone;

                                    settings.Add(setting.KeyConfig, new KeyPairItem(setting.OptionFlag, setting.OptionValue,
                                        !string.IsNullOrEmpty(Convert.ToString(setting.IsLock)) ? setting.IsLock : false,
                                        !string.IsNullOrEmpty(Convert.ToString(setting.IsOverride)) ? setting.IsOverride : false));
                                }
                                UpdateSettingsDetails param = new UpdateSettingsDetails
                                {
                                    loggedInUserId = userprofile.UserID,
                                    settingsToSave = new APISettings
                                    {
                                        UserEmail = string.Empty,
                                        SettingsFor = userprofile.CompanyID.Value,
                                        SettingsForType = Constants.String.SettingsType.Company,
                                        SettingDetails = settings
                                    },
                                    isBackupReq = false,
                                    userData = new List<Guid> { userprofile.UserID },
                                    Source = Constants.SettingsSource.LogIn
                                };
                                _settingsRepository.SaveUsersOverrideSettings(param);
                                loggerModelNew.Message = "Users Override setting details saved successfully.";
                                rSignLogger.RSignLogInfo(loggerModelNew);
                            }
                        }
                    }

                    loggerModelNew.Message = "User token retrived succesully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    respToken.StatusCode = HttpStatusCode.OK;
                    respToken.AuthMessage = "Token Validated";
                    respToken.AuthToken = responseToken;
                    respToken.EmailId = objUser.EmailId;
                    return respToken;
                }
                respToken.StatusCode = HttpStatusCode.NotFound;
                respToken.AuthMessage = "Token is not valid";
                respToken.AuthToken = responseToken;
                respToken.EmailId = objUser.EmailId;
                return respToken;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogWarn(loggerModelNew);
                respToken.StatusCode = HttpStatusCode.BadRequest;
                respToken.AuthMessage = ex.Message;
                respToken.AuthToken = respToken.AuthToken;
                respToken.EmailId = objUser.EmailId;
                return respToken;
            }
        }

        public async Task<ResponseTokenWithEmailId> ValidateTokenService(Microsoft.AspNetCore.Http.HttpRequest request, UserTokenModel userTokenModel)
        {
            loggerModelNew = new LoggerModelNew(userTokenModel.EmailAddress, "AuthenticateService", "ValidateTokenService", "Process started in ValidateTokenService", "");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseTokenWithEmailId responseToken = new ResponseTokenWithEmailId();
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                var AuthToken = iHeader.ElementAt(0);
                if (string.IsNullOrEmpty(AuthToken))
                {
                    loggerModelNew.Message = "Please provide valid AuthToken.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseToken.StatusCode = HttpStatusCode.NotFound;
                    responseToken.AuthMessage = "Please provide valid AuthToken.";
                    responseToken.AuthToken = AuthToken;
                    responseToken.EmailId = userTokenModel.EmailAddress;
                    return responseToken;
                    //return Results.NotFound(new { NotFoundResult = "Not Found", AuthMessage = "Please provide valid AuthToken.", AuthToken = "" });
                }

                if (!_authenticateRepository.IsEmailValid(userTokenModel.EmailAddress))
                {
                    loggerModelNew.Message = "Provided Email Id is not valid.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseToken.StatusCode = HttpStatusCode.NotFound;
                    responseToken.AuthMessage = "Provided Email Id is not valid.";
                    responseToken.AuthToken = AuthToken;
                    responseToken.EmailId = userTokenModel.EmailAddress;
                    return responseToken;
                    //return Results.NotFound(new { NotFoundResult = "Not Found", AuthMessage = "Provided Email Id is not valid.", AuthToken = "" });
                }

                UserProfile userprofile = _userRepository.ValidateToken(AuthToken, userTokenModel.EmailAddress);
                if (userprofile == null || userprofile.ID == Guid.Empty)
                {
                    loggerModelNew.Message = "Token not valid.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseToken.StatusCode = HttpStatusCode.NotFound;
                    responseToken.AuthMessage = "Token not valid.";
                    responseToken.AuthToken = AuthToken;
                    responseToken.EmailId = userTokenModel.EmailAddress;
                    return responseToken;
                }
                else
                {
                    UserViewModel userViewModel = new UserViewModel();
                    userViewModel.UserID = userprofile.UserID;
                    userViewModel.Company = userprofile.Company;
                    userViewModel.CompanyID = userprofile.CompanyID;
                    userViewModel.FirstName = userprofile.FirstName;
                    userViewModel.LastName = userprofile.LastName;
                    userViewModel.EmailID = userprofile.EmailID;
                    userViewModel.LanguageCode = userprofile.LanguageCode;
                    userViewModel.IsActive = userprofile.IsActive;

                    loggerModelNew.Message = "Token valiated.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseToken.StatusCode = HttpStatusCode.OK;
                    responseToken.AuthMessage = "Token valiated.";
                    responseToken.AuthToken = AuthToken;
                    responseToken.EmailId = userTokenModel.EmailAddress;
                    responseToken.UserViewModel = userViewModel;
                    return responseToken;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogWarn(loggerModelNew);
                responseToken.StatusCode = HttpStatusCode.BadRequest;
                responseToken.AuthMessage = ex.Message;
                //responseToken.AuthToken = AuthToken;
                responseToken.EmailId = userTokenModel.EmailAddress;
                return responseToken;
            }
        }
        #endregion commented

        public async Task<AuthenticateResponseMessageModel> IntAppToken(UserLoginTokenRequestModel userLoginModel)
        {
            AuthenticateResponseMessageModel responseMessage = new AuthenticateResponseMessageModel();
            loggerModelNew = new LoggerModelNew(userLoginModel.EmailAddress, "AuthenticateService", "IsSSOEnabledForUser", "Get RSign SSO With Email using API", "");
            rSignLogger.RSignLogInfo(loggerModelNew);

            UserAuthenticationResponseModal userResponseMessage = new UserAuthenticationResponseModal();
            RSign.Models.RestResponseUserInfo restResponseUserInfo = new RSign.Models.RestResponseUserInfo();
            HttpResponseMessage restResponse = new HttpResponseMessage();
            UserTokenModel userModel = new UserTokenModel();
            UserInfoModal rSignPlan = new UserInfoModal();
            bool? IsEnableSSO = false;
            int DefaultLandingPageSetting = 0;

            string responseToken = string.Empty, userRefreshToken = string.Empty, userRefreshExpires = string.Empty,
                userAccessTokenExpires = string.Empty, refKey = string.Empty;
            if (string.IsNullOrEmpty(userLoginModel.ReturnUrl))
            {
                IsEnableSSO = IsSSOEnabledForUser(userLoginModel.EmailAddress, UploadDriveType.iManage, "");
                responseMessage.SSOEnabledInRSign = Convert.ToBoolean(IsEnableSSO);
                if(IsEnableSSO == true)
                    return responseMessage;
            }

            RpostRestService rpostRestApi = new RpostRestService(_configuration);
            restResponse = rpostRestApi.IntAppToken(userLoginModel.EmailAddress, userLoginModel.Password, userLoginModel.JwtEnabled);
            if (!_esignHelper.IsFailureStatusCode(restResponse.StatusCode))
            {
                responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseMessage.StatusMessage = "ServiceUnavailable";
                responseMessage.Message = Convert.ToString(_configuration["RpostAuthFailure"]);
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogWarn(loggerModelNew);
                return responseMessage;
            }

            var resultRestAuth = JsonConvert.DeserializeObject<RPostUserStatusResponseModal>(restResponse.Content.ReadAsStringAsync().Result);

            if (resultRestAuth != null && Convert.ToBoolean(resultRestAuth.Success))
            {
                loggerModelNew.Message = "IntAppToken is success";
                rSignLogger.RSignLogInfo(loggerModelNew);

                if (resultRestAuth.ResultContent == null || resultRestAuth.ResultContent.UserInfo == null || resultRestAuth.ResultContent.UserInfo.Plan == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "ExpectationFailed";
                    if (resultRestAuth.ResultContent == null)
                    {
                        responseMessage.Message = Convert.ToString(_configuration["NotAssociatedPlan"]);
                    }
                    else if (resultRestAuth.ResultContent.UserInfo == null)
                    {
                        responseMessage.Message = "User details not found";
                    }
                    else if (resultRestAuth.ResultContent.UserInfo.Plan == null)
                    {
                        responseMessage.Message = Convert.ToString(_configuration["NotAssociatedPlan"]);
                    }
                    responseMessage.ReturnUrl = string.Empty;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                }

                #region S3-4562 Enhansement: Go live changes for the new Interfaces
                //InterfaceType interfaceTypeResp = _userRepository.CheckUserInterfaceType(userLoginModel.EmailAddress);
                //if (interfaceTypeResp == InterfaceType.Old)
                //{
                //   responseMessage.IsOldInterface = true;
                //    string oldInterfaceUrl = ModelHelper.GenerateNewInterfaceURL(userLoginModel.EmailAddress, resultRestAuth.ResultContent.AccessToken.access_token, "rsigndetails");

                //    responseMessage.OldInterfaceReturnUrl = oldInterfaceUrl;
                //    responseMessage.StatusCode = HttpStatusCode.OK;
                //    responseMessage.StatusText = "Success";
                //    responseMessage.StatusMessage = "User successfully authenticated.";
                //    responseMessage.Message = "User successfully authenticated.";
                //    return responseMessage;
                //}
                //else responseMessage.IsOldInterface = false;
                #endregion S3-4562 Enhansement: Go live changes for the new Interfaces

                UserInfoModal userInfoModal = resultRestAuth.ResultContent.UserInfo;
                UserPlanModal userPlanModal = userInfoModal.Plan;
                CustomerModal customerModal = userInfoModal.Customer;

                rSignPlan = userInfoModal;

                ResultContent resultContent = new ResultContent();

                resultContent.FirstName = userInfoModal.firstName;
                resultContent.LastName = userInfoModal.lastName;
                resultContent.Email = userInfoModal.email;
                resultContent.Phone = userInfoModal.phonenumber;
                resultContent.TimeZone = userInfoModal.timezone;
                resultContent.Language = userInfoModal.language;
                resultContent.Status = userInfoModal.status;
                resultContent.ExternalIdentityProvider = userInfoModal.externalIdentityProvider;

                Plan plan = new Plan();
                plan.Code = userPlanModal.code;
                plan.Name = userPlanModal.name;
                plan.Description = userPlanModal.description;
                plan.AllowedUnits = userPlanModal.allowedUnits;
                plan.UnitsSent = userPlanModal.unitsSent;

                Customer customer = new Customer();
                customer.Name = customerModal.name;
                customer.ReferenceKey = customerModal.referenceKey;
                customer.Language = customerModal.language;
                customer.Status = customerModal.status;

                resultContent.Plan = plan;
                resultContent.Customer = customer;
                restResponseUserInfo.ResultContent = resultContent;

                if (userPlanModal == null || customerModal == null || (customerModal.referenceKey == null && customerModal.referenceKey == "") || (userInfoModal.status == "Disabled" || userInfoModal.status == "Deleted"))
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "ExpectationFailed";
                    if (userPlanModal == null)
                    {
                        responseMessage.Message = Convert.ToString(_configuration["NoPlanAssociatedForUser"]);
                    }
                    else if (customerModal == null)
                    {
                        responseMessage.Message = Convert.ToString(_configuration["NotAssociatedWithCompany"]);
                    }
                    else if (customerModal.referenceKey == null && customerModal.referenceKey == "")
                    {
                        responseMessage.Message = "Not associated with any company";
                    }
                    else if (userInfoModal.status == "Disabled" || userInfoModal.status == "Deleted")
                    {
                        responseMessage.Message = userInfoModal.status == "Disabled" ? "User Not Activated" : "User Not Found";
                    }
                    responseMessage.ReturnUrl = string.Empty;
                    responseMessage.RCSUserInfo = null;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return responseMessage;
                }

                string defaultCompanyRefereneKey = Convert.ToString(_configuration["DefaultCompanyRefereneKey"]);
                string defaultTokenExpiryDays = _configuration["DefaultTokenExpiryDays"];

                if (userLoginModel.JwtEnabled)
                {
                    responseToken = resultRestAuth.ResultContent.AccessToken.access_token;
                    userRefreshToken = resultRestAuth.ResultContent.AccessToken.refresh_token;
                    userRefreshExpires = Convert.ToString(DateTime.UtcNow.AddSeconds(Convert.ToInt32(resultRestAuth.ResultContent.AccessToken.refresh_expires_in)));
                    userAccessTokenExpires = Convert.ToString(DateTime.UtcNow.AddSeconds(Convert.ToInt32(resultRestAuth.ResultContent.AccessToken.expires_in)));

                    userModel.EmailAddress = userLoginModel.EmailAddress;
                    userModel.BrowserName = userLoginModel.BrowserName;
                    userModel.RefreshToken = userRefreshToken;
                    userModel.RefreshExpires = userRefreshExpires;
                    userModel.AccessTokenExpires = userAccessTokenExpires;
                }
                else
                {
                    responseToken = resultRestAuth.ResultContent.AccessToken.access_token;
                    userModel.EmailAddress = userLoginModel.EmailAddress;
                    userModel.BrowserName = userLoginModel.BrowserName;
                }
                userResponseMessage = _userRepository.InsertUserData(userModel, restResponseUserInfo, responseToken, userLoginModel.JwtEnabled, defaultCompanyRefereneKey, defaultTokenExpiryDays);

                AdminGeneralAndSystemSettings userSettings = new AdminGeneralAndSystemSettings();
                AdminGeneralAndSystemSettings companySettings = new AdminGeneralAndSystemSettings();
                SettingResponseMessage userSettingsResponseMessage = new SettingResponseMessage();
                SettingResponseMessage companySettingsResponseMessage = new SettingResponseMessage();
                UserAdditionalResponseMessage userAdditionalResponseMessage = new UserAdditionalResponseMessage();

                UserProfile userprofile = new UserProfile();
                if (userResponseMessage != null && userResponseMessage.RCSUserInfo != null)
                {
                    loggerModelNew.Message = "Generated RCSUserInfo is success";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    restResponseUserInfo = userResponseMessage.RCSUserInfo;
                    userprofile = _esignHelper.getUserEntity(userResponseMessage.UserProfile);
                    refKey = userResponseMessage.TokenRefKey;

                    DefaultLandingPageSetting = Constants.DefaultLandingPage.Home;

                    int userArachivalstatus = _userRepository.GetUserArchivalStatus(userprofile.UserID, userprofile.UserTypeID == Constants.UserType.ADMIN ? Convert.ToString(userprofile.CompanyID) : "00000000-0000-0000-0000-000000000000");
                    responseMessage.UserArchivalStatus = Convert.ToInt32(userArachivalstatus);
                    responseMessage.TutorialConfigDetailsData = null;

                    loggerModelNew.Message = "userArachivalstatus" + Convert.ToString(userArachivalstatus);
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    if (userprofile.status == Constants.UserType.Review)
                    {
                        loggerModelNew.Message = "Userprofile status: Review";
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        UserSettingsModel userSettingsModel = new UserSettingsModel();
                        userSettingsModel.CompanyId = userprofile.CompanyID;
                        userSettingsModel.Email = userLoginModel.EmailAddress;

                        userAdditionalResponseMessage = GetUserCompanySettings(userSettingsModel, userprofile, responseToken, userSettingsResponseMessage, companySettingsResponseMessage, userSettings, companySettings);

                        if (userAdditionalResponseMessage.UserSettings == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "Getting user settigs failed";
                            responseMessage.Message = "An application error occured. Please try again or contact support.";//"Getting user settigs failed";
                            responseMessage.ReturnUrl = string.Empty;
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return responseMessage;
                        }
                        else
                        {
                            DefaultLandingPageSetting = userAdditionalResponseMessage.DefaultLandingPageSetting;
                            userSettings = userAdditionalResponseMessage.UserSettings;
                        }

                        if (userprofile.CompanyID != null && !(userprofile.CompanyID == Guid.Empty))
                        {
                            if (userAdditionalResponseMessage.CompanySettings != null)
                            {
                                companySettings = userAdditionalResponseMessage.CompanySettings;
                            }
                            else
                            {
                                companySettings = null;
                            }
                        }
                        else
                        {
                            companySettings = null;
                        }
                    }

                    responseMessage.UserRolesDetails = userAdditionalResponseMessage.UserRolesDetails;
                    responseMessage.UserAdditionalRoles = userAdditionalResponseMessage.UserAdditionalRoles;

                    if (userPlanModal != null && userPlanModal.isDefaultPlan == true)
                    {
                        ArchivalMessageForUser showArchivalMessage = new ArchivalMessageForUser();
                        ArchivalMessageTrace archivalMessageTrace = new ArchivalMessageTrace();
                        showArchivalMessage = _userRepository.GetArchivalMessageForUser(userprofile.UserID);

                        //ArchivalMessageInfo archivalMessageInfo = _userRepository.GetArchivalMessageInfo(userprofile.UserID);

                        if (showArchivalMessage != null && showArchivalMessage.ShowArchivalMessage == true && showArchivalMessage.EnvelopeArchivalDate != null)
                        {
                            DateTime envelopeArchivalDate = showArchivalMessage.EnvelopeArchivalDate.Value;
                            string archivalDateVal = envelopeArchivalDate.ToShortDateString();

                            archivalMessageTrace = _userRepository.GetArchivalMessageTrace(userprofile.UserID, Convert.ToDateTime(archivalDateVal));

                            loggerModelNew.Message = "Successfully retrived archival message trace details for User Email=" + userprofile.EmailID + "";
                            rSignLogger.RSignLogInfo(loggerModelNew);

                            if (archivalMessageTrace == null)
                            {
                                #region DateFormat
                                var dateformatSettingsDetails = _userRepository.GetDateFormatByUserId(userprofile.UserID, Constants.SettingsKeyConfig.DateFormat);
                                string DateFormatID = dateformatSettingsDetails.OptionValue, dateFormat = "";

                                //string DateFormatID = archivalMessageInfo.DateFormatValue, dateFormat = "";
                                dateFormat = GetDateFormatByDateFormatID(DateFormatID, dateFormat);
                                #endregion DateFormat

                                DateTime temparchivalDateVal = showArchivalMessage.EnvelopeArchivalDate.Value;
                                dateFormat = dateFormat.Contains("mmm") ? dateFormat.Replace("mmm", "MMM") : dateFormat.Replace("mm", "MM");

                                string archivalDate = temparchivalDateVal.ToString(dateFormat);

                                ServiceExecutionDetails serviceExecutionDetails = _userRepository.GetServiceExecutionDetails("EnvelopeArchival");

                                string nextExecuteDateTime = string.Empty;
                                if (serviceExecutionDetails != null)
                                {
                                    DateTime nextExecuteVal = serviceExecutionDetails.NextExecuteDateTime.Value;
                                    nextExecuteDateTime = nextExecuteVal.ToString(dateFormat);
                                }

                                string ArchivalFlashPopupMessage = _genericRepository.GetUniqueKey("ArchivalFlashPopupMessage", userprofile.LanguageCode);
                                ArchivalFlashPopupMessage = !string.IsNullOrEmpty(ArchivalFlashPopupMessage) ? ArchivalFlashPopupMessage.Replace("{Date1}", archivalDate) : null;
                                ArchivalFlashPopupMessage = !string.IsNullOrEmpty(ArchivalFlashPopupMessage) ? ArchivalFlashPopupMessage.Replace("{Date2}", nextExecuteDateTime) : null;

                                responseMessage.ArchivalFlashPopupMessage = ArchivalFlashPopupMessage; // "Your envelopes created before " + archivalDate + " will be archived on " + nextExecuteDateTime + ". To download the envelopes, click “Archiving Soon”.";

                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogInfo(loggerModelNew);
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(responseToken) && userprofile != null && userprofile.IsActive == false)
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "Your account has been deactivated, please contact to admin.";
                        responseMessage.Message = "Your account has been deactivated, please contact to admin.";
                        responseMessage.ReturnUrl = string.Empty;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return responseMessage;
                    }
                    else
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "Your company has been deactivated, you can not login here.";
                        responseMessage.Message = "Your company has been deactivated, you can not login here.";
                        responseMessage.ReturnUrl = string.Empty;
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return responseMessage;

                    }
                }
                UsageRemainingResultContent usageRemainingResultContent = new UsageRemainingResultContent();
                usageRemainingResultContent.PlanType = userInfoModal.Plan.type;
                usageRemainingResultContent.UnitsRemaining = userInfoModal.Plan.unitsRemaining;
                usageRemainingResultContent.PlanRange = userInfoModal.Plan.range;
                if (!string.IsNullOrEmpty(usageRemainingResultContent.PlanType) && (usageRemainingResultContent.PlanType.ToLower() == "individual") && (usageRemainingResultContent.UnitsRemaining != null))
                {
                    if (usageRemainingResultContent.UnitsRemaining >= 0)
                    {
                        int minimumUnit = 0;
                        bool isParse = Int32.TryParse(_configuration["MonthlyMessagesRemainingMinimumThreshold"], out minimumUnit);
                        minimumUnit = isParse ? minimumUnit : 5;
                        if (usageRemainingResultContent.UnitsRemaining <= minimumUnit)
                        {
                            usageRemainingResultContent.UpgradeLink = string.Empty;
                            usageRemainingResultContent.UpgradeLink = userInfoModal.upgradeLink;
                            responseMessage.UsageRemainingResultContent = usageRemainingResultContent;
                        }
                        else responseMessage.UsageRemainingResultContent = null;
                    }
                }
               
                responseMessage.DialCodeDropdownList = _validationRepository.LoadDialingCountryCodes();               

                bool enableOutOfOfficeMode = Convert.ToBoolean(userSettings.IsOutOfOfficeModeEnable);
                if (enableOutOfOfficeMode && Convert.ToBoolean(userSettings.DisplayOutOfOfficeLabel))
                {
                    var OOFDateRangeFirstDay = userSettings.DateRangeFirstDay;
                    var OOFDateRangeLastDay = userSettings.DateRangeLastDay;
                    if (enableOutOfOfficeMode && (DateTime.Now.Date >= OOFDateRangeFirstDay && (OOFDateRangeLastDay == null || DateTime.Now.Date <= OOFDateRangeLastDay))) userprofile.OutOfOfficeLabel = true;
                    else userprofile.OutOfOfficeLabel = false;
                }
                else userprofile.OutOfOfficeLabel = false;

                //List<UserRoleMapping> lstUserRoleMapping = _userRepository.GetUserRoleMappingList();
                var userRolesDetails = userAdditionalResponseMessage.UserRolesDetails; // lstUserRoleMapping.Where(u => u.UserId == userprofile.UserID && u.RoleName == Constants.UsersRoles.LanguageTranslator).FirstOrDefault();
                if (userRolesDetails) userprofile.IsLanguageTranslator = true;
                else userprofile.IsLanguageTranslator = false;

                LanguageKeyValuesResponseMessage languageKeyValuesResponse = await GetLanguageKeyDetails(userprofile.LanguageCode);
                responseMessage.LanguageKeyTranslations = languageKeyValuesResponse;

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusText = "Success";
                responseMessage.StatusMessage = "User successfully authenticated.";
                responseMessage.Message = "User successfully authenticated.";
                responseMessage.ReturnUrl = "";
                responseMessage.RCSUserInfo = restResponseUserInfo;
                responseMessage.UserInfoModal = rSignPlan;
                responseMessage.UserProfile = userprofile;
                responseMessage.ReferenceKey = refKey;
                responseMessage.AccessToken = responseToken;
                responseMessage.UserRefreshToken = userRefreshToken;
                responseMessage.UserRefreshExpires = userRefreshExpires;
                responseMessage.UserAccessTokenExpires = userAccessTokenExpires;
                responseMessage.UserSettings = userSettings;
                responseMessage.CompanySettings = companySettings;
                responseMessage.DefaultLandingPageSetting = DefaultLandingPageSetting;
                responseMessage.SSOEnabledInRSign = Convert.ToBoolean(IsEnableSSO);

                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);

            }
            else if (resultRestAuth.StatusCode == Convert.ToString((int)HttpStatusCode.Unauthorized))
            {
                responseMessage.StatusText = resultRestAuth.StatusText;
                responseMessage.StatusMessage = resultRestAuth.Status;
                responseMessage.Message = resultRestAuth.Message.Message;
                HttpResponseMessage responseToClient = new HttpResponseMessage();
                if (resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == RCAPStatusCodes.RCAP_1018)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.Message = "Your account has been blacklisted, please contact the administrator.";
                }
                else if (resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == RCAPStatusCodes.RCAP_1019)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.Message = "Your account is not active, please contact the administrator.";
                }
                else if (resultRestAuth.Message.MessageCode != null &&
                    (resultRestAuth.Message.MessageCode.ToLower() == RCAPStatusCodes.RCAP_1016 || resultRestAuth.Message.MessageCode.ToLower() == RCAPStatusCodes.RCAP_1020 || resultRestAuth.Message.MessageCode.ToLower() == RCAPStatusCodes.RCAP_1021))
                {
                    responseMessage.StatusCode = HttpStatusCode.Unauthorized;
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.Message = "User authentication failed.";
                }

                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogWarn(loggerModelNew);
                return responseMessage; ;
            }
            else
            {
                if (resultRestAuth.ResultContent != null && resultRestAuth.ResultContent.Errors != null && resultRestAuth.ResultContent.Errors.Count > 0)
                {
                    var errorModal = resultRestAuth.ResultContent.Errors[0];
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusText = resultRestAuth.StatusText;
                    responseMessage.StatusMessage = resultRestAuth.Status;
                    responseMessage.Message = errorModal.message;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return responseMessage;
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusText = resultRestAuth.StatusText;
                    responseMessage.StatusMessage = resultRestAuth.Status;
                    responseMessage.Message = resultRestAuth.Message.Message;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return responseMessage;
                }
            }
            return responseMessage;
        }

        private static string GetDateFormatByDateFormatID(string DateFormatID, string dateFormat)
        {
            if ((DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash.ToString()))
            {
                dateFormat = Constants.DateFormatString.US_mm_dd_yyyy_slash;
            }
            else if ((DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan.ToString()))
            {
                dateFormat = Constants.DateFormatString.US_mm_dd_yyyy_colan;
            }
            else if ((DateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots.ToString()))
            {
                dateFormat = Constants.DateFormatString.US_mm_dd_yyyy_dots;
            }
            else if ((DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_slash.ToString()))
            {
                dateFormat = Constants.DateFormatString.Europe_mm_dd_yyyy_slash;
            }
            else if ((DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_colan.ToString()))
            {
                dateFormat = Constants.DateFormatString.Europe_mm_dd_yyyy_colan;
            }
            else if ((DateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_dots.ToString()))
            {
                dateFormat = Constants.DateFormatString.Europe_mm_dd_yyyy_dots;
            }
            else if ((DateFormatID == Constants.DateFormat.Europe_yyyy_mm_dd_dots.ToString()))
            {
                dateFormat = Constants.DateFormatString.Europe_yyyy_mm_dd_dots;
            }
            else if ((DateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan.ToString()))
            {
                dateFormat = Constants.DateFormatString.US_dd_mmm_yyyy_colan;
            }

            return dateFormat;
        }

        public UserAdditionalResponseMessage GetUserCompanySettings(UserSettingsModel userSettingsModel, UserProfile userprofile, string responseToken, SettingResponseMessage userSettingsResponseMessage, SettingResponseMessage companySettingsResponseMessage, AdminGeneralAndSystemSettings userSettings, AdminGeneralAndSystemSettings companySettings)
        {
            loggerModelNew = new LoggerModelNew(userprofile.EmailID, "AuthenticateService", "GetUserCompanySettings", "GetUserCompanySettings method start", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            UserAdditionalResponseMessage userAdditionalResponseMessage = new UserAdditionalResponseMessage();
            try
            {
                int DefaultLandingPageSetting = Constants.DefaultLandingPage.Home;
                userSettingsResponseMessage = _userRepository.GetUserSettingDetails(userprofile, userSettingsModel);
                loggerModelNew.Message = "Retrived the GetUserSettingDetails";
                rSignLogger.RSignLogInfo(loggerModelNew);
                if (userSettingsResponseMessage.SettingInformation == null)
                {
                    userAdditionalResponseMessage.UserSettings = null;
                }
                else
                {
                    userSettings = _esignHelper.TransformSettingsDictionaryToEntity(userSettingsResponseMessage.SettingInformation);
                    userSettings.SelectedTimeZone = string.IsNullOrEmpty(userSettings.SelectedTimeZone) ? "UTC" : userSettings.SelectedTimeZone;
                    userAdditionalResponseMessage.DefaultLandingPageSetting = (userSettings != null && userSettings.DefaultLandingPage != null) ? (int)userSettings.DefaultLandingPage : DefaultLandingPageSetting;
                    userAdditionalResponseMessage.UserSettings = userSettings;
                }

                if (userprofile.CompanyID != null && !(userprofile.CompanyID == Guid.Empty))
                {
                    CompanySettingsModel companySettingsModel = new CompanySettingsModel();
                    companySettingsModel.CompanyId = Convert.ToString(userSettingsModel.CompanyId);
                    companySettingsModel.CompanyName = "";
                    companySettingsModel.TabId = userSettingsModel.TabId;
                    companySettingsModel.UserProfile = userprofile;
                    companySettingsModel.EmailAddress = userSettingsModel.Email;

                    companySettingsResponseMessage = _userRepository.GetCompanySettings(userprofile, companySettingsModel);
                    if (companySettingsResponseMessage.SettingInformation == null)
                    {
                        userAdditionalResponseMessage.CompanySettings = null;
                    }
                    else if (companySettingsResponseMessage.SettingInformation != null)
                    {
                        companySettings = _esignHelper.TransformSettingsDictionaryToEntity(companySettingsResponseMessage.SettingInformation);
                        companySettings.SelectedTimeZone = string.IsNullOrEmpty(companySettings.SelectedTimeZone) ? "UTC" : companySettings.SelectedTimeZone;
                        userAdditionalResponseMessage.CompanySettings = companySettings;
                    }
                }

                var userRolesDetails = _userRepository.GetUsersAdditionalRoles();
                userAdditionalResponseMessage.UserAdditionalRoles = userRolesDetails;
                var data = userRolesDetails.Any(u => u.UserId == userprofile.UserID && u.RoleName == Constants.UsersRoles.LanguageTranslator);
                if (data)                
                    userAdditionalResponseMessage.UserRolesDetails = true;                
                else
                    userAdditionalResponseMessage.UserRolesDetails = false;

                loggerModelNew.Method = "GetUserCompanySettings";
                loggerModelNew.Message = "Successfully Retrived the GetUserCompanySettings for user:" + userprofile.EmailID;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return userAdditionalResponseMessage;
            }
            catch (Exception ex)
            {
                loggerModelNew.Method = "GetUserCompanySettings";
                loggerModelNew.Message = "Got exception at GetUserCompanySettings method and error message is." + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return userAdditionalResponseMessage;
            }
        }

        public bool? IsSSOEnabledForUser(string emailAddress, string integrationType, string companyReferenceKey)
        {
            loggerModelNew = new LoggerModelNew(emailAddress, "AuthenticateService", "IsSSOEnabledForUser", "Get RSign SSO With Email using API", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            Guid CompanyId = Guid.Empty; bool? IsEnableSSO = false;
            try
            {
                UserProfileDetails GetUserProfileDetails = _userRepository.GetUserProfileDetails(emailAddress);
                if (GetUserProfileDetails != null && Convert.ToBoolean(GetUserProfileDetails.IsActive) && GetUserProfileDetails.CompanyID != Guid.Empty)
                {
                    CompanyId = GetUserProfileDetails.CompanyID;
                }
                if ((CompanyId == Guid.Empty) && !string.IsNullOrEmpty(companyReferenceKey))
                {

                    Company company = _companyRepository.GetCompanyByReferenceKey(companyReferenceKey);
                    if (company != null && company.ID != null && company.ID != Guid.Empty)
                    {
                        CompanyId = company.ID;
                    }
                }

                if (CompanyId != null && CompanyId != Guid.Empty && GetUserProfileDetails != null)
                {
                    SettingsExternalSSOEnable settingsExternalSSOEnable = _settingsRepository.GetSettingsExternalIsSSOEnable(CompanyId, GetUserProfileDetails.UserID, integrationType);
                    if (settingsExternalSSOEnable != null)
                    {
                        IsEnableSSO = settingsExternalSSOEnable.comanyIsenableSSO;
                        if (IsEnableSSO == true)
                        {
                            IsEnableSSO = settingsExternalSSOEnable.userIsenableSSO;
                        }
                    }
                }
                return IsEnableSSO;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = $"Exception while retriving the iManage company settings for company {companyReferenceKey}" + ex.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return IsEnableSSO;
            }

        }

        public ResponseMessage ValidateModelvalues(RegistrationModal register, bool checkRegisterModel = true)
        {
            loggerModelNew = new LoggerModelNew(register.EmailId, "AuthenticateService", "ValidateModelvalues", "ValidateModelvalues method start", "");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                if (!_envelopeHelperMain.IsEmailValid(register.EmailId))
                {
                    responseMessage.Message = checkRegisterModel == true ? Convert.ToString(_configuration["EmailWrong"]) : Convert.ToString(_configuration["InvalidEmail"]);
                    responseMessage.StatusCode = HttpStatusCode.Forbidden;
                    responseMessage.StatusMessage = "Forbidden";
                    return responseMessage;
                }
                else if (!string.IsNullOrEmpty(register.PhoneNumber) && !_envelopeHelperMain.IsPhoneNumberValid(register.PhoneNumber) && checkRegisterModel)
                {

                    responseMessage.StatusCode = HttpStatusCode.Forbidden;
                    responseMessage.StatusMessage = "Forbidden";
                    responseMessage.Message = Convert.ToString(_configuration["PhoneNumberWrong"]);
                    return responseMessage;
                }
                else if (string.IsNullOrEmpty(register.Password) && checkRegisterModel)
                {
                    responseMessage.StatusCode = HttpStatusCode.Forbidden;
                    responseMessage.StatusMessage = "Forbidden";
                    responseMessage.Message = Convert.ToString(_configuration["PasswordRequired"]);
                    return responseMessage;
                }
                else if (!string.IsNullOrEmpty(register.Password) && (register.Password.Length < 8 || !register.Password.Any(p => char.IsUpper(p)) || !register.Password.Any(p => char.IsDigit(p))) && checkRegisterModel)
                {
                    responseMessage.StatusCode = HttpStatusCode.Forbidden;
                    responseMessage.StatusMessage = "Forbidden";
                    responseMessage.Message = Convert.ToString(_configuration["InvalidPassword"]);
                    return responseMessage;
                }
                else if (((string.IsNullOrEmpty(register.FirstName) || string.IsNullOrWhiteSpace(register.FirstName)) || (string.IsNullOrEmpty(register.LastName) || string.IsNullOrWhiteSpace(register.LastName))) && checkRegisterModel)
                {
                    if (string.IsNullOrEmpty(register.FirstName) || string.IsNullOrWhiteSpace(register.FirstName))
                    {
                        responseMessage.Message = Convert.ToString(_configuration["FirstNameRequired"]);
                    }
                    else if (string.IsNullOrEmpty(register.LastName) || string.IsNullOrWhiteSpace(register.LastName))
                    {
                        responseMessage.Message = Convert.ToString(_configuration["LastNameRequired"]);
                    }
                    responseMessage.StatusCode = HttpStatusCode.Forbidden;
                    responseMessage.StatusMessage = "Forbidden";

                    return responseMessage;
                }

                UserProfile user = new UserProfile();
                if (!checkRegisterModel)
                {
                    user = _userRepository.GetUserProfileByEmailID(register.EmailId);
                    if (user == null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.Forbidden;
                        responseMessage.StatusMessage = "Forbidden";
                        responseMessage.Message = Convert.ToString(_configuration["EmailWrongAndRegister"]);
                        var registerUrl = Convert.ToString(_configuration["domain"]) + "/register";
                        var urlForSignUp = $"<a style='color: white; text-decoration: underline; font-weight:bold;' href='{registerUrl}'>Sign up</a>";
                        responseMessage.Message = responseMessage.Message?.Replace("signup", urlForSignUp);
                        return responseMessage;
                    }
                }
                responseMessage.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "SingleSignOn";
                loggerModelNew.Method = "SingleSignOn";
                loggerModelNew.Message = "API EndPoint - Exception at ValidateModelvalues method and error message is:" + ex.ToString();
                rSignLogger.RSignLogWarn(loggerModelNew);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.Message = ex.Message;
                return responseMessage;
            }
            return responseMessage;
        }

        public ResponseMessage ValidateForgotPasswordEmail(RegistrationModal register)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            responseMessage.StatusCode = HttpStatusCode.OK;
            return responseMessage;
        }
        public HttpResponseMessage AuthenticateExtToken(ExtTokenRequestModel extTokenRequestModel)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            extTokenRequestModel.client_id = Convert.ToString(_configuration["RSignClientID"]); //"EBC45D19-C8A7-4956-B661-2D6F76616309";
            HttpClient client = new HttpClient();
            string RPostAuthUrl = Convert.ToString(_configuration["RPostAuthUrl"]);
            client.BaseAddress = new Uri(RPostAuthUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                responseMessage = client.PostAsJsonAsync("api/v1/auth/ExtToken", extTokenRequestModel).Result;
            }
            catch (Exception)
            {
                responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                return responseMessage;
            }
            return responseMessage;
        }
        public HttpResponseMessage PartnerIntegrationToken(PartnerIntegrationModel partnerIntegrationModel)
        {
            string clientID = Convert.ToString(_configuration["RSignClientID"]);
            string integrationSecretKey = Convert.ToString(_configuration["IntegrationSecretKey"]);
            string intergrationId = Convert.ToString(_configuration["IntergrationId"]);
            string RPostAuthUrl = Convert.ToString(_configuration["RPostAuthUrl"]);
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            partnerIntegrationModel.client_id = clientID;
            partnerIntegrationModel.secretKey = integrationSecretKey;
            partnerIntegrationModel.intergrationId = intergrationId;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(RPostAuthUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                responseMessage = client.PostAsJsonAsync("api/v1/auth/PartnerIntegrationToken", partnerIntegrationModel).Result;
            }
            catch (Exception)
            {
                responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                return responseMessage;
            }
            return responseMessage;
        }
        public async Task<AuthenticateResponseMessageModel> SingleSignOn(ExtTokenRequestModel extTokenRequestModel)
        {
            AuthenticateResponseMessageModel responseMessage = new AuthenticateResponseMessageModel();
            try
            {
                loggerModelNew = new LoggerModelNew(extTokenRequestModel.client_id, "AuthenticateService", "SingleSignOn", "Get RSign SSO With Email using API", "");
                rSignLogger.RSignLogInfo(loggerModelNew);

                Dictionary<Guid?, string> LanguagelayoutList = new Dictionary<Guid?, string>();
                string browserName = string.Empty,
                    ipAddress = string.Empty,//TODO//RSign.Common.Helpers.Common.GetIPAddress(),
                    userAuthenticationKey = string.Empty, fieldToFocus = string.Empty, refKey = string.Empty,
                    userRefreshToken = string.Empty, userRefreshExpires = string.Empty, userAccessTokenExpires = string.Empty, userEmail = string.Empty, isFromIntegrations = "false", extTokenRequestProvider = string.Empty;
                bool isNewUser = false;
                RestResponseUserInfo rSignPlan = new RestResponseUserInfo();
                HttpResponseMessage restResponse = new HttpResponseMessage();
                UserProfile userprofile = new UserProfile();
                responseMessage.DefaultLandingPageSetting = Constants.DefaultLandingPage.Home;
                RpostRestService rpostRestService = new RpostRestService(_configuration);
                UserAuthenticationResponseModal userResponseMessage = new UserAuthenticationResponseModal();
                if (string.IsNullOrEmpty(extTokenRequestModel.provider))
                {
                    isFromIntegrations = extTokenRequestModel.isFromIntegrations;
                    browserName = "";
                    string jsonTokenData = extTokenRequestModel.provider_Access_Token;
                    jsonTokenData = jsonTokenData.TrimStart('"').TrimEnd('"').Replace("\\", "");
                    JsonTokenRequestModel jsonTokenRequestModel = JsonConvert.DeserializeObject<JsonTokenRequestModel>(jsonTokenData);
                    extTokenRequestModel.provider_Access_Token = jsonTokenRequestModel.accesstoken;
                    extTokenRequestModel.provider = jsonTokenRequestModel.ssotype;
                                      
                    extTokenRequestProvider = jsonTokenRequestModel.ssotype;

                    loggerModelNew.Message = "Process started for Invoking AuthenticateExtToken api";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    restResponse = AuthenticateExtToken(extTokenRequestModel);
                    loggerModelNew.Message = "Process completed for AuthenticateExtToken api and response came";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    if (!_esignHelper.IsFailureStatusCode(restResponse.StatusCode))
                    {
                        responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                        responseMessage.StatusMessage = "ServiceUnavailable";
                        responseMessage.Message = Convert.ToString(_configuration["RpostAuthFailure"]);
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return responseMessage;
                    }
                }
                else if (!string.IsNullOrEmpty(extTokenRequestModel.provider) && extTokenRequestModel.provider == RSign.Common.Helpers.Constants.UploadDriveType.iManage)
                {
                    browserName = extTokenRequestModel.BrowserType;
                    restResponse = PartnerIntegrationToken(extTokenRequestModel.partnerIntegrationModel);
                    if (!_esignHelper.IsFailureStatusCode(restResponse.StatusCode))
                    {
                        responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                        responseMessage.StatusMessage = "ServiceUnavailable";
                        responseMessage.Message = Convert.ToString(_configuration["RpostAuthFailure"]);
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return responseMessage;
                    }
                }

                if (!restResponse.IsSuccessStatusCode)
                {
                    var resultRest = JsonConvert.DeserializeObject<RestLoginResponseErrorMessage>(restResponse.Content.ReadAsStringAsync().Result);
                    responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                    responseMessage.StatusMessage = "ServiceUnavailable";
                    responseMessage.Message = resultRest.error_description;
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return responseMessage;
                }
                else
                {
                    var userStatusResponse = JsonConvert.DeserializeObject<RPostUserStatusResponse>(restResponse.Content.ReadAsStringAsync().Result);
                    if (userStatusResponse != null && userStatusResponse.ResultContent != null)
                    {
                        isNewUser = userStatusResponse.ResultContent.isnewuser;
                        userAuthenticationKey = userStatusResponse.ResultContent.access_token;
                        userEmail = userStatusResponse.ResultContent.emailaddress;
                        userRefreshToken = userStatusResponse.ResultContent.refresh_token;
                        userRefreshExpires = Convert.ToString(DateTime.Now.AddSeconds(Convert.ToInt32(userStatusResponse.ResultContent.refresh_expires_in)));
                        userAccessTokenExpires = Convert.ToString(DateTime.Now.AddSeconds(Convert.ToInt32(userStatusResponse.ResultContent.expires_in)));
                    }
                    responseMessage.AccessToken = userAuthenticationKey;
                    responseMessage.Message = "User successfully authenticated.";
                }

                loggerModelNew.Message = "Invoking ValidateUserSignInDetails method for user email:" + userEmail;
                rSignLogger.RSignLogInfo(loggerModelNew);

                responseMessage = await ValidateUserSignInDetails(userEmail, userAuthenticationKey, browserName, userRefreshToken, userRefreshExpires, userAccessTokenExpires, "singlesignon", extTokenRequestProvider);
                return responseMessage;
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "SingleSignOn";
                loggerModelNew.Method = "SingleSignOn";
                loggerModelNew.Message = "API EndPoint - Exception at SingleSignOn method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.Message = ex.Message;
                return responseMessage;
            }
        }

        public async Task<AuthenticateResponseMessageModel> ValidateUserSignInDetails(string userEmail, string userAuthenticationKey, string browserName, string userRefreshToken, string userRefreshExpires, string userAccessTokenExpires, string type, string extTokenRequestProvider)
        {
            loggerModelNew = new LoggerModelNew("", "AuthenticateService", "ValidateUserSignInDetails", "Process started for Validate User SignIn Details for user enmail:" + userEmail, "", "", "", "", "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            AuthenticateResponseMessageModel responseMessage = new AuthenticateResponseMessageModel();
            RestResponseUserInfo rSignPlan = new RestResponseUserInfo();
            UserProfile userprofile = new UserProfile();
            responseMessage.DefaultLandingPageSetting = Constants.DefaultLandingPage.Home;
            RpostRestService rpostRestService = new RpostRestService(_configuration);
            UserAuthenticationResponseModal userResponseMessage = new UserAuthenticationResponseModal();
            string refKey = string.Empty;

            bool IsJwtAuthentication = _envelopeHelperMain.IsJwtAuthentication(userEmail, userAuthenticationKey);
            if (!string.IsNullOrEmpty(userAuthenticationKey))
            {
                using (var httpClient = new HttpClient())
                {
                    UserTokenModel userTokenModel = new UserTokenModel();
                    userTokenModel.EmailAddress = userEmail;
                    userTokenModel.BrowserName = browserName;

                    if (IsJwtAuthentication && userEmail != null && !string.IsNullOrEmpty(userEmail.Trim()))
                    {
                        loggerModelNew.Email = userEmail;
                        loggerModelNew.Message = "When sender adddress available, checking validate token service.";
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        HttpResponseMessage restResponseV2 = rpostRestService.ValidateTokenServiceV2(userAuthenticationKey, userEmail, _configuration["RSignClientID"]);
                        loggerModelNew.Method = type;
                        if (_esignHelper.IsFailureStatusCode(restResponseV2.StatusCode))
                        {
                            StreamReader readerUser = new StreamReader(restResponseV2.Content.ReadAsStreamAsync().Result);
                            var responseUserDetails = JsonConvert.DeserializeObject<RPostUserStatusResponse>(readerUser.ReadToEnd());
                            if (restResponseV2.IsSuccessStatusCode)
                            {
                                userprofile = _userRepository.GetUserProfileByEmailID(userEmail);
                                refKey = "";
                                rSignPlan = _authenticateRepository.GetUserInfoFromRCS(userAuthenticationKey, userEmail);
                            }
                            else
                            {
                                responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                                responseMessage.StatusMessage = "ServiceUnavailable";
                                if (!string.IsNullOrEmpty(userAuthenticationKey) && userprofile != null && userprofile.IsActive == false)
                                {
                                    responseMessage.Message = "Your account is not active, please contact admin.";
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return responseMessage;
                                }
                                else
                                {
                                    responseMessage.Message = "Your company has been deactivated, you can not login here.";
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return responseMessage;
                                }
                            }
                        }
                        else
                        {
                            responseMessage.StatusCode = HttpStatusCode.Forbidden;
                            responseMessage.StatusMessage = "Forbidden";
                            responseMessage.Message = responseMessage.Message;
                            responseMessage.ReturnUrl = string.Empty;
                            responseMessage.RCSUserInfo = null;
                            loggerModelNew.Message = "ValidateTokenServiceV2 response is null and message is:" + responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return responseMessage;
                        }
                    }
                    else if (!IsJwtAuthentication && userEmail != null && !string.IsNullOrEmpty(userEmail.Trim()))
                    {
                        UserModel userModel = new UserModel();
                        userModel.EmailAddress = userEmail;
                        userModel.BrowserName = browserName;
                        HttpResponseMessage restResponseV2 = rpostRestService.ValidateToken(userAuthenticationKey, userEmail, _configuration["RSignClientID"]);
                        if (_esignHelper.IsFailureStatusCode(restResponseV2.StatusCode))
                        {
                            StreamReader readerUser = new StreamReader(restResponseV2.Content.ReadAsStreamAsync().Result);
                            var responseUserDetails = JsonConvert.DeserializeObject<RestResposeTokenInfo>(readerUser.ReadToEnd());
                            if (restResponseV2.IsSuccessStatusCode)
                            {
                                userprofile = _userRepository.GetUserProfileByEmailID(userEmail);
                                rSignPlan = _authenticateRepository.GetUserInfoFromRCS(userAuthenticationKey, (userprofile != null && !string.IsNullOrWhiteSpace(userprofile.EmailID)) ? userprofile?.EmailID : userEmail);
                            }
                            else
                            {
                                responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                                responseMessage.StatusMessage = "ServiceUnavailable";
                                if (!string.IsNullOrEmpty(userAuthenticationKey) && userprofile != null && userprofile.IsActive == false)
                                {
                                    responseMessage.Message = "Your account is not active, please contact admin.";
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return responseMessage;
                                }
                                else
                                {
                                    responseMessage.Message = "Your company has been deactivated, you can not login here.";
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return responseMessage;
                                }
                            }
                        }
                        else
                        {
                            responseMessage.StatusCode = HttpStatusCode.Forbidden;
                            responseMessage.StatusMessage = "Forbidden";
                            responseMessage.Message = responseMessage.Message;
                            responseMessage.ReturnUrl = string.Empty;
                            responseMessage.RCSUserInfo = null;
                            loggerModelNew.Message = "ValidateToken response is null and message is:" + responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return responseMessage;
                        }
                    }

                    if (!string.IsNullOrEmpty(userEmail) && rSignPlan != null)
                    {
                        if (rSignPlan.ResultContent == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                            responseMessage.StatusMessage = "ServiceUnavailable";
                            responseMessage.Message = _configuration["NotAssociatedPlan"];
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return responseMessage;
                        }
                        if (rSignPlan.ResultContent.Plan == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                            responseMessage.StatusMessage = "ServiceUnavailable";
                            responseMessage.Message = _configuration["NoPlanAssociatedForUser"];
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew); return responseMessage;
                        }
                        if (rSignPlan.ResultContent.Customer == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                            responseMessage.StatusMessage = "ServiceUnavailable";
                            responseMessage.Message = _configuration["NotAssociatedWithCompany"];
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew); return responseMessage;
                        }
                        if (rSignPlan.ResultContent.Customer.ReferenceKey == null && rSignPlan.ResultContent.Customer.ReferenceKey == "")
                        {
                            responseMessage.StatusCode = HttpStatusCode.ExpectationFailed;
                            responseMessage.StatusMessage = "ExpectationFailed";
                            responseMessage.Message = "Not associated with any company";
                            responseMessage.ReturnUrl = string.Empty;
                            responseMessage.RCSUserInfo = null;
                            return responseMessage;
                        }
                        if (rSignPlan.ResultContent.Status == "Disabled" || rSignPlan.ResultContent.Status == "Deleted")
                        {
                            responseMessage.StatusCode = HttpStatusCode.ExpectationFailed;
                            responseMessage.StatusMessage = "ExpectationFailed";
                            responseMessage.Message = rSignPlan.ResultContent.Status == "Disabled" ? "User Not Activated" : "User Not Found";
                            responseMessage.ReturnUrl = string.Empty;
                            return responseMessage;
                        }

                        string defaultCompanyRefereneKey = _configuration["DefaultCompanyRefereneKey"];
                        string defaultTokenExpiryDays = _configuration["DefaultTokenExpiryDays"];
                        long userArachivalstatus = 0;

                        userTokenModel.EmailAddress = !string.IsNullOrEmpty(userprofile?.EmailID) ? userprofile?.EmailID : userEmail;
                        userTokenModel.BrowserName = browserName;
                        userTokenModel.RefreshToken = userRefreshToken;
                        userTokenModel.RefreshExpires = userRefreshExpires;
                        userTokenModel.AccessTokenExpires = userAccessTokenExpires;
                        userTokenModel.GetRefreshToken = false;

                        rSignPlan.ResultContent.ExternalIdentityProvider = !string.IsNullOrEmpty(rSignPlan?.ResultContent?.ExternalIdentityProvider) ? rSignPlan?.ResultContent?.ExternalIdentityProvider : extTokenRequestProvider;

                        userResponseMessage = _userRepository.InsertUserData(userTokenModel, rSignPlan, userAuthenticationKey, IsJwtAuthentication, defaultCompanyRefereneKey, defaultTokenExpiryDays);
                        if (userResponseMessage != null && userResponseMessage.RCSUserInfo != null)
                        {
                            refKey = userResponseMessage.TokenRefKey;
                            userprofile = _userRepository.GetUserProfileByEmailID(userEmail);
                        }
                        else
                        {
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "Error";
                            responseMessage.Message = "User not authenticated successfully";
                            responseMessage.ReturnUrl = string.Empty;
                            responseMessage.RCSUserInfo = null;
                            responseMessage.UserProfile = null;
                            return responseMessage;
                        }
                        if (userprofile.UserTypeID == Constants.UserType.ADMIN)
                        {
                            userArachivalstatus = _userRepository.GetUserArchivalStatus(userprofile.UserID, Convert.ToString(userprofile.CompanyID));
                            responseMessage.UserArchivalStatus = Convert.ToInt32(userArachivalstatus);
                        }
                        else
                        {
                            userArachivalstatus = _userRepository.GetUserArchivalStatus(userprofile.UserID, string.Empty);
                            responseMessage.UserArchivalStatus = Convert.ToInt32(userArachivalstatus);
                        }
                        var userModel = new UserReviewModel
                        {
                            EmailID = userprofile.EmailID,
                            companyName = rSignPlan.ResultContent.Customer.Name,
                            companyReferenceKey = rSignPlan.ResultContent.Customer.ReferenceKey,
                            IsActive = rSignPlan.ResultContent.Status == "Active" ? true : false,
                            IsCompanyStatus = rSignPlan.ResultContent.Customer.Status == "Active" ? true : false,
                            TimeZone = rSignPlan.ResultContent.TimeZone
                        };

                        UserResponseMessage userRespMessage = UpdateReviewDetails(userModel);
                    }
                }

                UserSettingsModel userSettingsModel = new UserSettingsModel();
                UserAdditionalResponseMessage userAdditionalResponseMessage = new UserAdditionalResponseMessage();
                SettingResponseMessage userSettingsResponseMessage = new SettingResponseMessage();
                SettingResponseMessage companySettingsResponseMessage = new SettingResponseMessage();
                AdminGeneralAndSystemSettings userSettings = new AdminGeneralAndSystemSettings();
                AdminGeneralAndSystemSettings companySettings = new AdminGeneralAndSystemSettings();
                userSettingsModel.CompanyId = userprofile?.CompanyID;
                userSettingsModel.Email = userprofile?.EmailID;
                if (userprofile != null)
                    userAdditionalResponseMessage = GetUserCompanySettings(userSettingsModel, userprofile, userAuthenticationKey, userSettingsResponseMessage, companySettingsResponseMessage, userSettings, companySettings);
                HttpResponseMessage response = rpostRestService.GetUserUsageRemaining(userAuthenticationKey);

                loggerModelNew.Method = type;
                loggerModelNew.Message = "Successfully retrived userAdditionalResponseMessage details";
                rSignLogger.RSignLogInfo(loggerModelNew);

                if (response.IsSuccessStatusCode)
                {
                    loggerModelNew.Message = "successfully retrived GetUserUsageRemaining details";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    //if (response.IsSuccessStatusCode)
                    //{
                    var jSonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                    UsageRemaining usageRemainingForUser = JsonConvert.DeserializeObject<UsageRemaining>(jSonResponse);
                    if (usageRemainingForUser != null && usageRemainingForUser.ResultContent != null)
                    {
                        if (!string.IsNullOrEmpty(usageRemainingForUser.ResultContent.PlanType) && (usageRemainingForUser.ResultContent.PlanType.ToLower() == "individual") && (usageRemainingForUser.ResultContent.UnitsRemaining != null))
                        {
                            if (usageRemainingForUser.ResultContent.UnitsRemaining >= 0)
                            {
                                loggerModelNew.Message = "Units remaining for the user " + userprofile.EmailID + " " + usageRemainingForUser.ResultContent.UnitsRemaining;
                                rSignLogger.RSignLogInfo(loggerModelNew);
                                int minimumUnit = 0;
                                bool isParse = Int32.TryParse(_configuration["MonthlyMessagesRemainingMinimumThreshold"], out minimumUnit);
                                minimumUnit = isParse ? minimumUnit : 5;
                                if (usageRemainingForUser.ResultContent.UnitsRemaining <= minimumUnit)
                                {
                                    usageRemainingForUser.ResultContent.UpgradeLink = string.Empty;
                                    HttpResponseMessage responseUpgradePlan = rpostRestService.GetUserPlanUpgradeInfo(userprofile.EmailID);
                                    var jSonUpgradePlanResponse = JsonConvert.DeserializeObject(responseUpgradePlan.Content.ReadAsStringAsync().Result).ToString();
                                    UpgradeLinkInfo userUpgradeLink = JsonConvert.DeserializeObject<UpgradeLinkInfo>(jSonUpgradePlanResponse);
                                    if (userUpgradeLink.StatusCode == 200 && userUpgradeLink.ResultContent != null)
                                        usageRemainingForUser.ResultContent.UpgradeLink = userUpgradeLink.ResultContent;
                                    else
                                        usageRemainingForUser.ResultContent.UpgradeLink = _configuration["DefaultUpgradeLink"];

                                    responseMessage.UsageRemainingResultContent = usageRemainingForUser.ResultContent;
                                }
                                else
                                {
                                    responseMessage.UsageRemainingResultContent = null;
                                }
                            }
                        }
                    }
                    //}
                    //else
                    //{
                    //    loggerModelNew.Message = "User is not associated with plan";
                    //    rSignLogger.RSignLogInfo(loggerModelNew);
                    //}

                    loggerModelNew.Message = "Assigning properties to response from usersettings";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserInfoModal userInfoModal = new UserInfoModal(); // rSignPlan.ResultContent.Plan;
                    UserPlanModal userPlan = new UserPlanModal();
                    CustomerModal customerModal = new CustomerModal();
                    if (rSignPlan?.ResultContent != null)
                    {
                        customerModal.name = rSignPlan?.ResultContent?.Customer?.Name;
                        customerModal.referenceKey = rSignPlan?.ResultContent?.Customer?.ReferenceKey;
                        customerModal.language = rSignPlan?.ResultContent?.Customer?.Language;
                        customerModal.status = rSignPlan?.ResultContent?.Customer?.Status;
                    }
                    userInfoModal.Customer = customerModal;
                    userInfoModal.email = userEmail;
                    if (userprofile != null)
                    {
                        userInfoModal.firstName = userprofile?.FirstName;
                        userInfoModal.lastName = userprofile?.LastName;
                        userInfoModal.language = userprofile?.LanguageCode;
                    }

                    if (userAdditionalResponseMessage != null && userAdditionalResponseMessage.UserSettings != null)
                    {
                        responseMessage.DialCodeDropdownList = _validationRepository.LoadDialingCountryCodes();                        

                        bool enableOutOfOfficeMode = Convert.ToBoolean(userAdditionalResponseMessage.UserSettings.IsOutOfOfficeModeEnable);
                        if (enableOutOfOfficeMode && Convert.ToBoolean(userAdditionalResponseMessage.UserSettings.DisplayOutOfOfficeLabel))
                        {
                            var OOFDateRangeFirstDay = userAdditionalResponseMessage.UserSettings.DateRangeFirstDay;
                            var OOFDateRangeLastDay = userAdditionalResponseMessage.UserSettings.DateRangeLastDay;
                            if (enableOutOfOfficeMode && (DateTime.Now.Date >= OOFDateRangeFirstDay && (OOFDateRangeLastDay == null || DateTime.Now.Date <= OOFDateRangeLastDay))) userprofile.OutOfOfficeLabel = true;
                            else userprofile.OutOfOfficeLabel = false;
                        }
                        else userprofile.OutOfOfficeLabel = false;

                        var userRolesDetails = userAdditionalResponseMessage.UserRolesDetails;
                        if (userRolesDetails) userprofile.IsLanguageTranslator = true;
                        else userprofile.IsLanguageTranslator = false;

                        if (rSignPlan != null && rSignPlan.ResultContent != null && rSignPlan.ResultContent.Plan != null)
                        {
                            userPlan.name = rSignPlan.ResultContent.Plan.Name;
                            userPlan.code = rSignPlan.ResultContent.Plan.Code;
                            userPlan.description = rSignPlan.ResultContent.Plan.Description;
                            userPlan.allowedUnits = rSignPlan.ResultContent.Plan.AllowedUnits;
                            userPlan.unitsSent = rSignPlan.ResultContent.Plan.UnitsSent;
                        }
                        userInfoModal.Plan = userPlan;
                    }
                    else
                    {
                        loggerModelNew.Message = "User settings is null";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                    }

                    LanguageKeyValuesResponseMessage languageKeyValuesResponse = await GetLanguageKeyDetails(userprofile?.LanguageCode);
                    responseMessage.LanguageKeyTranslations = languageKeyValuesResponse;

                    loggerModelNew.Message = "successfully retrived GetLanguageKeyDetails details and User authenticated successfully";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    #region S3-4562 Enhansement: Go live changes for the new Interfaces
                    bool isNewCompany = _userRepository.IsNewCompany(userprofile.CompanyID);
                    if (isNewCompany) responseMessage.IsNewCompany = "true";
                    else responseMessage.IsNewCompany = "false";

                    //User logged to new environment. so save user interface preference as new always
                     _userRepository.SaveUserPreference(userprofile.CompanyID, userprofile.UserID, InterfaceType.New);

                    #endregion S3-4562 Enhansement: Go live changes for the new Interfaces

                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusText = "Success";
                    responseMessage.StatusMessage = "User authenticated successfully";
                    responseMessage.Message = "User authenticated successfully";
                    responseMessage.RCSUserInfo = userResponseMessage.RCSUserInfo;
                    responseMessage.UserInfoModal = userInfoModal;
                    responseMessage.UserProfile = userprofile;
                    responseMessage.ReferenceKey = refKey;
                    responseMessage.AccessToken = userAuthenticationKey;
                    responseMessage.UserRefreshToken = userRefreshToken;
                    responseMessage.UserRefreshExpires = userRefreshExpires;
                    responseMessage.UserAccessTokenExpires = userAccessTokenExpires;
                    responseMessage.UserSettings = userAdditionalResponseMessage!.UserSettings!;
                    responseMessage.CompanySettings = userAdditionalResponseMessage.CompanySettings;
                    responseMessage.UserRolesDetails = userAdditionalResponseMessage.UserRolesDetails;
                    responseMessage.UserAdditionalRoles = userAdditionalResponseMessage.UserAdditionalRoles;
                    responseMessage.DefaultLandingPageSetting = (userSettings != null && userSettings.DefaultLandingPage != null) ? (int)userSettings.DefaultLandingPage : responseMessage.DefaultLandingPageSetting;
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusText = "Error";
                    responseMessage.StatusMessage = "User not authenticated";
                    responseMessage.Message = "User not authenticated";
                    loggerModelNew.Message = "Failed to retrive GetUserUsageRemaining details";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                }
            }
            return responseMessage;
        }

        public UserResponseMessage UpdateReviewDetails(UserReviewModel userModel)
        {
            UserResponseMessage responseMessage = new();
            try
            {
                HttpResponseMessage responseToClient = new();
                loggerModelNew = new LoggerModelNew(userModel != null ? userModel.EmailID : string.Empty, "UpdateReviewDetails", "UpdateReviewDetails", "Get RSign Account revieiw using API", "");
                rSignLogger.RSignLogInfo(loggerModelNew);

                if (userModel != null && string.IsNullOrEmpty(userModel.companyReferenceKey))
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_configuration["ReferenceKeyEmpty"]);
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return responseMessage;
                }
                if (userModel != null && string.IsNullOrEmpty(userModel.EmailID) || !_envelopeHelperMain.IsEmailValid(userModel.EmailID))
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = Convert.ToString(_configuration["EmailWrong"]);
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return responseMessage;
                }

                UserResponseMessage userResponseMessage = _userRepository.UpdateReviewFlagDetailsByUserId(userModel);
                if (userResponseMessage != null && userResponseMessage.UserProfile == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "Update Review Flag Details By UserId failed";
                    responseMessage.Message = "Update Review Flag Details By UserId failed";
                    responseMessage.ReturnUrl = string.Empty;
                    return responseMessage;
                }
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "User successfully authenticated";
                responseMessage.UserProfile = _esignHelper.getUserDetails(userResponseMessage.UserProfileDetails);
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return responseMessage;
            }
            catch (Exception ex)
            {
                return responseMessage;
            }
        }

        public async Task<AuthenticateResponseMessageModel> ValidateUserTokenEnvelopeDetails(HttpRequest request, UserEnvelopeTokenModel userEnvelopeTokenModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", "AuthenticateService", "ValidateUserTokenEnvelopeDetails", "Endpoint Initialized,to Validate User Token Envelope Details", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            AuthenticateResponseMessageModel responseMessage = new AuthenticateResponseMessageModel();           
            try
            {
                var reqClientID = userEnvelopeTokenModel.ClientID;
                string userAuthenticationKey = userEnvelopeTokenModel.AuthToken;
                string userEmail = userEnvelopeTokenModel.EmailId;
                string browserName = userEnvelopeTokenModel.BrowserType;
                string refKey = string.Empty;
                loggerModelNew.Message = "Validating user token details for user email:" + userEmail;
                rSignLogger.RSignLogInfo(loggerModelNew);

                responseMessage = await ValidateUserSignInDetails(userEmail, userAuthenticationKey, browserName, "", "", "", "initializeenvelope","");
                userEnvelopeTokenModel.EnvelopeId = !string.IsNullOrEmpty(userEnvelopeTokenModel.EnvelopeId) ? userEnvelopeTokenModel.EnvelopeId.Replace(" ", "+") : "";
                responseMessage.EnvelopeId = !string.IsNullOrEmpty(userEnvelopeTokenModel.EnvelopeId) ? (EncryptDecryptQueryString.Decrypt(userEnvelopeTokenModel.EnvelopeId.ToString(), Convert.ToString(_configuration["AppKey"]))): "";

                return responseMessage;               
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = "AuthenticateService";
                loggerModelNew.Method = "ValidateUserTokenEnvelopeDetails";
                loggerModelNew.Message = "API EndPoint - Exception at ValidateUserTokenEnvelopeDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.Message = ex.Message;
                return responseMessage;
            }
        }

        public async Task<MFAAndPasswordResponseMessage> GetMFAandPasswordPolicySetting(string emailAddress)
        {
            MFAAndPasswordResponseMessage responseMessage = new MFAAndPasswordResponseMessage();
            HttpResponseMessage restResponse = new HttpResponseMessage();
            loggerModelNew = new LoggerModelNew(emailAddress, "AuthenticateService", "GetMFAandPasswordPolicySetting", "Get RPortal MFA & Password policy details based on Email using API", "");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    RpostRestService rpostRestApi = new RpostRestService(_configuration);
                    restResponse = rpostRestApi.GetMFAandPasswordPolicySettingsFromRPortal(emailAddress);

                    if (!_esignHelper.IsFailureStatusCode(restResponse.StatusCode))
                    {
                        responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                        responseMessage.StatusMessage = "ServiceUnavailable";
                        responseMessage.Message = Convert.ToString(_configuration["RpostAuthFailure"]);
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return responseMessage;
                    }

                    var resultRestAuth = JsonConvert.DeserializeObject<RPortalMFAPasswordSettingResponseModal>(restResponse.Content.ReadAsStringAsync().Result);
                    if (resultRestAuth == null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.Message = "User is not active.";
                        loggerModelNew.Message = responseMessage.Message + " ,got the empty result from mfa/settings api.";
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return responseMessage;
                    }
                    else if (resultRestAuth != null && Convert.ToBoolean(resultRestAuth.Success))
                    {
                        loggerModelNew.Message = "GetMFAandPasswordPolicySettings is success";
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        if (resultRestAuth.ResultContent == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "ExpectationFailed";
                            responseMessage.Message = "result content is empty, mfa/settings api endpoint";
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return responseMessage;
                        }

                        if (resultRestAuth.ResultContent.passwordPolicySetting == null)
                        {
                            responseMessage.StatusCode = HttpStatusCode.BadRequest;
                            responseMessage.StatusMessage = "ExpectationFailed";
                            responseMessage.Message = "User details password policy not found";
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return responseMessage;
                        }

                        if (resultRestAuth.ResultContent.enforcePasswordPolicy == 2 || resultRestAuth.ResultContent.enforcePasswordPolicy == 1 || resultRestAuth.ResultContent.mfaEnabled == true)
                        {
                            loggerModelNew.Message = "GetMFAandPasswordPolicySettings is success, MFA enabled: " + resultRestAuth.ResultContent.mfaEnabled + " ,Password policy: " + resultRestAuth.ResultContent.enforcePasswordPolicy;
                            rSignLogger.RSignLogInfo(loggerModelNew);

                            responseMessage.Success = resultRestAuth.Success;
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.Message = resultRestAuth.Message.Message;
                            responseMessage.MessageCode = resultRestAuth.Message.MessageCode;
                            responseMessage.IsMFAEnabled = resultRestAuth.ResultContent.mfaEnabled;
                            responseMessage.IsPasswordPolicyEnabled = resultRestAuth.ResultContent.enforcePasswordPolicy;
                            responseMessage.ResultContent = resultRestAuth.ResultContent;
                            if ((resultRestAuth.ResultContent.enforcePasswordPolicy == 2 || resultRestAuth.ResultContent.enforcePasswordPolicy == 1) && resultRestAuth.ResultContent.passwordPolicySetting != null)
                            {
                                PasswordPolicySetting passwordPolicySetting = new PasswordPolicySetting();
                                passwordPolicySetting.MinCharLength = resultRestAuth.ResultContent.passwordPolicySetting.MinCharLength;
                                passwordPolicySetting.UpperCaseCharCount = resultRestAuth.ResultContent.passwordPolicySetting.UpperCaseCharCount;
                                passwordPolicySetting.LowerCaseCharCount = resultRestAuth.ResultContent.passwordPolicySetting.LowerCaseCharCount;
                                passwordPolicySetting.SpecialCharCount = resultRestAuth.ResultContent.passwordPolicySetting.SpecialCharCount;
                                passwordPolicySetting.NumberCharCount = resultRestAuth.ResultContent.passwordPolicySetting.NumberCharCount;

                                string passwordCriteria = JsonConvert.SerializeObject($"MinCharLength={passwordPolicySetting.MinCharLength},UpperCaseCharCount={passwordPolicySetting.UpperCaseCharCount},LowerCaseCharCount={passwordPolicySetting.LowerCaseCharCount},SpecialCharCount={passwordPolicySetting.SpecialCharCount},NumberCharCount={passwordPolicySetting.NumberCharCount}");
                                responseMessage.PasswordPolicyCriteria = passwordCriteria;
                            }
                        }
                        else
                        {
                            responseMessage.IsMFAEnabled = false;
                            responseMessage.IsPasswordPolicyEnabled = 0;
                            responseMessage.PasswordPolicyCriteria = string.Empty;
                            responseMessage.StatusText = resultRestAuth.StatusText;
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.StatusMessage = resultRestAuth.Status;
                            responseMessage.Message = resultRestAuth.Message.Message;
                            responseMessage.Success = true;
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            loggerModelNew.Message = "Else condition, password policy or mfa not enabled, MFA enabled: " + responseMessage.IsMFAEnabled + " ,Password policy: " + responseMessage.IsPasswordPolicyEnabled + ", password criteria: " + responseMessage.PasswordPolicyCriteria;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                            return responseMessage;
                        }

                        return responseMessage;
                    }
                    else if (resultRestAuth.StatusCode == Convert.ToString((int)HttpStatusCode.Unauthorized))
                    {
                        responseMessage.IsMFAEnabled = true;
                        responseMessage.StatusText = resultRestAuth.StatusText;
                        responseMessage.StatusMessage = resultRestAuth.Status;
                        responseMessage.Message = resultRestAuth.Message.Message;
                        responseMessage.RCAPMessageCode = resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null ? Convert.ToString(resultRestAuth.Message.MessageCode) : "";
                        responseMessage.RCAPMessageDescription = resultRestAuth.Message != null && resultRestAuth.Message.Message != null ? resultRestAuth.Message.Message : "";

                        if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1061)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1060)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1042)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1052)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1051)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1025)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1040)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1041)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1048)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1050)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1035)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1036)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1037)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1038)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1039)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1056)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else if (resultRestAuth.Message != null && resultRestAuth.Message.MessageCode != null && resultRestAuth.Message.MessageCode.ToLower() == Constants.RCAPStatusCodes.RCAP_1057)
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }
                        else
                        {
                            loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                            rSignLogger.RSignLogWarn(loggerModelNew);

                            responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                            return responseMessage;
                        }

                        loggerModelNew.Message = "RPortal Settings(Login/Setting) endpoint, RCAP Message code: " + resultRestAuth.Message.MessageCode + " , message: " + responseMessage.Message + ", status code: " + responseMessage.StatusCode;
                        rSignLogger.RSignLogWarn(loggerModelNew);

                        responseMessage.RCAPMessageCode = resultRestAuth.Message.MessageCode;
                        return responseMessage;
                    }
                    return responseMessage;
                }
                else
                {
                    loggerModelNew.Message = "GetMFAandPasswordPolicySettings - else, Email address should not be is not empty";
                    rSignLogger.RSignLogError(loggerModelNew, null);
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.Message = loggerModelNew.Message;
                    return responseMessage;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error while getting MFA and Passoword Policy settings " + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.Message = ex.Message;
                return responseMessage;
            }
        }
    }
}
