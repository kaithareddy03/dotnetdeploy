using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.ManageDocument.Models;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.APIModels.Envelope;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Net;
using System.Web;

namespace RSign.SendAPI.API
{
    //[Authorize]
    public class EnvelopeEndpoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly string _module = "EnvelopeEndpoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;
        private ISettingsRepository _settingsRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly IESignHelper _esignHelper;
        private IDraftRepository _draftRepository;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private IUserRepository _userRepository;

        public EnvelopeEndpoint(IHttpContextAccessor accessor, IEnvelopeRepository envelopeRepository, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration,
            ISettingsRepository settingsRepository, IESignHelper esignHelper, IDraftRepository draftRepository, IEnvelopeHelperMain envelopeHelperMain, IUserRepository userRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _envelopeRepository = envelopeRepository;
            _userTokenRepository = userTokenRepository;
            _settingsRepository = settingsRepository;
            _esignHelper = esignHelper;
            rSignLogger = new RSignLogger(_appConfiguration);
            _draftRepository = draftRepository;
            _envelopeHelperMain = envelopeHelperMain;
            _userRepository = userRepository;
        }
        public void RegisterEnvelopeAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Envelope/GetEnvelopeDetails", GetEnvelopeDetails);
            app.MapGet("/api/v1/Envelope/GetRequiredEnvelopeDetails", GetRequiredEnvelopeDetails);
            app.MapGet("/api/v1/Envelope/GetImages", GetImages);
            app.MapGet("/api/v1/Envelope/GetDocumentControlsCount", GetDocumentControlsCount);
            app.MapGet("/api/v1/Envelope/LoadForensicAuditTrail", LoadForensicAuditTrail);
            app.MapGet("/api/v1/Envelope/GetRecipients", GetRecipients);
            app.MapGet("/api/v1/Envelope/GetEnvelopeSettingsDetails/{envelopeId}/{languageCode}/{isEnvelopeArichived?}", GetEnvelopeSettingsDetails);
            app.MapGet("/api/v1/Envelope/GetSharedAccessSendersList", GetSharedAccessSendersList);
            app.MapPost("/api/v1/Envelope/SaveEnvelopeStep1Details", SaveEnvelopeStep1Details);
            app.MapPost("/api/v1/Envelope/SendEnvelope", SendEnvelope);
            app.MapPost("/api/v1/Envelope/SaveDraftEnvelope", SaveDraftEnvelope);
            app.MapPost("/api/v1/Envelope/OpenEnvelope", OpenEnvelope);
            app.MapPost("/api/v1/Envelope/GetDocumentImageDetails", GetDocumentImageDetails);
            app.MapPost("/api/v1/Envelope/UseMultiTemplates", UseMultiTemplates);
            app.MapPost("/api/v1/Envelope/UseMultiTemplatesUpdateRecepientsDocument", UseMultiTemplatesUpdateRecepientsDocument);
            app.MapPost("/api/v1/Envelope/SaveUserEnvelopeGridPreferences", SaveUserEnvelopeGridPreferences);
            app.MapPost("/api/v1/Envelope/GetAutoLockActivityLog", GetAutoLockActivityLog);
            app.MapPost("/api/v1/Envelope/GetSenderListForCompanyorGroup", GetSenderListForCompanyorGroup);
            app.MapDelete("/api/v1/Envelope/Discard", DiscardEnvelope);
            app.MapPost("/api/v1/Envelope/UseTemplateGroups", UseTemplateGroups);
            app.MapPost("/api/v1/Envelope/EnvelopeGroupIndex", EnvelopeGroupIndex);
            app.MapPost("/api/v1/Envelope/EnvelopeGroupData", EnvelopeGroupData);
            app.MapPost("/api/v1/Envelope/SendGroupEnvelope", SendGroupEnvelope);
        }

        /// <summary>
        /// GetEnvelopeDetails - This is used for step2 envelope prepare only
        /// </summary>
        /// <param name="envelopeId"></param>
        /// <returns>envelope details</returns>
        public async Task<IResult> GetEnvelopeDetails(HttpRequest request, string envelopeId, string isActingUserPerformingAction = "false")
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopeDetails", "Endpoint Initialized,to Get Envelope Details by either envelopeId or envelopeCode", envelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting envelope details by envelopeId=" + envelopeId;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    string domainUrlType = "2";
                    try
                    {
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                    }
                    catch (Exception)
                    {
                        loggerModelNew.Message = "API EndPoint - Exception at GetEnvelopeDetails method and error - Original source url is:" + domainUrlType;
                        rSignLogger.RSignLogError(loggerModelNew);
                    }

                    return Results.Ok(await _envelopeRepository.GetEnvelopeDetails(envelopeId, userToken.EmailId!, userToken, domainUrlType, isActingUserPerformingAction));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetEnvelopeDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// This api is used for get envelope document images
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <param name="envelopeId"></param>
        /// <param name="uncPath"></param>
        /// <returns></returns> 
        [AllowAnonymous]
        public async Task<IResult> GetImages(HttpRequest request, string id, string envelopeId, string uncPath)
        {
            //loggerModelNew = new LoggerModelNew("", _module, "GetImages", "Method Initialized,to Get Envelope Images by either envelopeId ", envelopeId, "SendAPI");
            //rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                byte[] imgData = _envelopeRepository.GetImages(request, id, envelopeId, uncPath);

                MemoryStream ms = new MemoryStream(imgData);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                response.Content.Headers.ContentLength = ms.Length;
                return Results.Bytes(imgData, "image/png", $"{envelopeId}.jpg");
            }
            catch (Exception ex)
            {
                //loggerModelNew.Message = "API EndPoint - Exception at GetImages method and error message is:" + ex.ToString();
                //rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest();
            }
        }
        /// <summary>
        /// This api is used for save envelope details when clicking next on step1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="EnvelopePrepareModal"></param>
        /// <returns></returns>        
        public async Task<IResult> SaveEnvelopeStep1Details(HttpRequest request, EnvelopePrepareModal EnvelopePrepareModal)
        {
            string methodName = "SaveEnvelopeStep1Details";
            try
            {
                var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "SaveEnvelopeStep1Details", "Process started for initializing envelope and save step1 envelope details", "", "", "", remoteIpAddress, "SaveEnvelopeStep1DetailsAPI");
                rSignLogger.RSignLogInfo(loggerModelNew);
                EnvelopePrepareRequest envelope = new EnvelopePrepareRequest();
                string additionalAttachments = string.Empty;
                string isActingUserPerformingAction = "false";
                if (EnvelopePrepareModal != null && !string.IsNullOrEmpty(EnvelopePrepareModal.Envelope))
                {
                    envelope = JsonConvert.DeserializeObject<EnvelopePrepareRequest>(EnvelopePrepareModal.Envelope);
                    loggerModelNew.Message = "Serializing envelope string object is:" + EnvelopePrepareModal.Envelope;
                    loggerModelNew.EnvelopeId = envelope.GlobalEnvelopeID;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "Envelope";
                    rSignAPIPayload.APIMethod = "SaveEnvelopeStep1Details";
                    rSignAPIPayload.PayloadTypeId = envelope.GlobalEnvelopeID.ToString();
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = EnvelopePrepareModal.Envelope;
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);
                }

                if (envelope != null && envelope.GlobalEnvelopeID != null && envelope.GlobalEnvelopeID != "")
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process started for Save Envelope Step1 Details method for envelope:" + envelope.GlobalEnvelopeID;
                    loggerModelNew.EnvelopeId = envelope.GlobalEnvelopeID;
                    loggerModelNew.UserId = envelope.UserID;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                    string? authToken = iHeader.ElementAt(0);
                    if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                    UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken);
                    if (userToken == null)
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = methodName;
                        loggerModelNew.Message = "User Token is null for User Id:" + envelope.UserID;
                        loggerModelNew.EnvelopeId = envelope.GlobalEnvelopeID;
                        loggerModelNew.UserId = envelope.UserID;
                        rSignLogger.RSignLogWarn(loggerModelNew);

                        return Results.BadRequest(new
                        {
                            success = false,
                            message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                            data = new List<ErrorTagDetailsResponse>(),
                            field = "",
                            enableSenderUIId = 1,
                            newSenderUIEnabledUrl = ""
                        });
                    }

                    envelope.IpAddress = remoteIpAddress;
                    //Shared Envelope Access    
                    UserProfile userProfile = new UserProfile();
                    Guid userId = Guid.Empty;
                    string userEmail = string.Empty;
                    string authRefKey = userToken != null && !string.IsNullOrEmpty(userToken.ReferenceKey) ? userToken.ReferenceKey : string.Empty;
                    if (!string.IsNullOrEmpty(envelope.IsActingUserPerformingAction) && envelope.IsActingUserPerformingAction.ToLower() == "true")
                    {
                        isActingUserPerformingAction = "true";
                        userEmail = envelope.EnvelopeSenderUserEmail;
                        Guid envelopeSenderUserID = new Guid(envelope.EnvelopeSenderUserID);
                        userProfile = _userRepository.GetUserProfileByUserID(envelopeSenderUserID);
                    }
                    else
                    {
                        userEmail = userToken != null && !string.IsNullOrEmpty(userToken.EmailId) ? userToken.EmailId : _userTokenRepository.GetUserEmailByToken(authToken);
                        userProfile = _userTokenRepository.GetUserProfileByEmail(userEmail);
                    }

                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process started for get user profile for user email:" + userEmail;
                    loggerModelNew.AuthRefKey = authRefKey;
                    rSignLogger.RSignLogInfo(loggerModelNew);


                    userId = userProfile.UserID;
                    envelope.CultureInfo = !string.IsNullOrEmpty(envelope.EnvelopeCultureInfo) ? Convert.ToString(envelope.EnvelopeCultureInfo) : userProfile.LanguageCode;
                    //Shared Envelope Access    

                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process started for get user and company settings for user id:" + Convert.ToString(userId) + " and company id:" + Convert.ToString(userProfile.CompanyID);
                    loggerModelNew.AuthRefKey = authRefKey;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    /* Get User Settings */
                    APISettings apiUserSettings = _settingsRepository.GetEntityByParam(userId, string.Empty, Constants.String.SettingsType.User);
                    var userSettings = _esignHelper.TransformSettingsDictionaryToEntity(apiUserSettings);

                    /* Get Company Settings */
                    APISettings apiCompanySettings = _settingsRepository.GetEntityByParam((Guid)userProfile.CompanyID, string.Empty, Constants.String.SettingsType.Company);
                    var companySettings = _esignHelper.TransformSettingsDictionaryToEntity(apiCompanySettings);

                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process completed for get user and company settings for user id:" + Convert.ToString(userId) + " and company id:" + Convert.ToString(userProfile.CompanyID);
                    loggerModelNew.AuthRefKey = authRefKey;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    var EnableMultiBranding = Convert.ToBoolean(envelope.EnableMultiBranding);
                    if (EnvelopePrepareModal != null) additionalAttachments = EnvelopePrepareModal.AdditionalAttachments;

                    string domainUrlType = "2";
                    try
                    {
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                    }
                    catch (Exception)
                    {
                        loggerModelNew.Message = "API EndPoint - Exception at SaveEnvelopeStep1Details method and error - Original source url is:" + domainUrlType;
                        rSignLogger.RSignLogError(loggerModelNew);
                    }

                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process started for Envelope Repository-SaveEnvelopeStep1Details for envelope id:" + Convert.ToString(envelope.GlobalEnvelopeID);
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    envelope.SelectedGroup = envelope.SelectedGroup;
                    ResponseMessageWithEnvlpGuid responseMessageWithEnvlpGuid = await _envelopeRepository.SaveEnvelopeStep1Details(envelope, additionalAttachments, userId, userProfile, userSettings, companySettings, domainUrlType);

                    if (responseMessageWithEnvlpGuid.StatusCode == HttpStatusCode.OK)
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = methodName;
                        loggerModelNew.Message = "Process completed for Envelope Repository-SaveEnvelopeStep1Details for envelope id:" + Convert.ToString(envelope.GlobalEnvelopeID);
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        loggerModelNew.Message = "Process started for generating encrypted new prepare page URL for envelope id:" + Convert.ToString(envelope.GlobalEnvelopeID);
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        string encryptedGlobalEnvelopeID = EncryptDecryptQueryString.Encrypt(Convert.ToString(envelope.GlobalEnvelopeID), Convert.ToString(_appConfiguration["AppKey"]));
                        string encryptedTokenEnvelopeId = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("rpk={0}&eid={1}&type={2}&EmailId={3}&su={4}&sha={5}", authToken, Convert.ToString(envelope.GlobalEnvelopeID), "Envelope", userEmail, domainUrlType, isActingUserPerformingAction), Convert.ToString(_appConfiguration["AppKey"])));
                        string encryptedNewPreparePageURL = Convert.ToString(_appConfiguration["NewPrepareURL"]) + encryptedTokenEnvelopeId;

                        return Results.Ok(new
                        {
                            success = true,
                            message = responseMessageWithEnvlpGuid.Message,
                            data = responseMessageWithEnvlpGuid.ErrorTagDetailsResponse,
                            field = encryptedGlobalEnvelopeID,
                            enableSenderUIId = userSettings.EnableSenderUIId,
                            newSenderUIEnabledUrl = userSettings.EnableSenderUIId == 2 ? encryptedNewPreparePageURL : encryptedNewPreparePageURL,
                            IsEnableMultiBranding = EnableMultiBranding,
                            GlobalEnvelopeID = Convert.ToString(envelope.GlobalEnvelopeID),
                            SourceUrlType = domainUrlType,
                            IsActingUserPerformingAction = isActingUserPerformingAction,
                            SelectedGroup = envelope.SelectedGroup,
                            PrepareType = "Envelope"
                        });
                    }
                    else
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = methodName;
                        loggerModelNew.Message = "Process failed because SaveEnvelopeStep1Details response is not success for envelope id:" + Convert.ToString(envelope.GlobalEnvelopeID);
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        return Results.Ok(new
                        {
                            success = false,
                            message = responseMessageWithEnvlpGuid.Message,
                            data = new List<ErrorTagDetailsResponse>(),
                            field = Convert.ToString(envelope.GlobalEnvelopeID),
                            enableSenderUIId = 1,
                            newSenderUIEnabledUrl = ""
                        });
                    }
                }
                else
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Envelope is not serialized and envelope is null or envelope id not found.";
                    rSignLogger.RSignLogError(loggerModelNew);

                    return Results.Ok(new
                    {
                        success = false,
                        message = "Unable to process the documents, please try again or contact administrator.",
                        data = new List<ErrorTagDetailsResponse>(),
                        field = "",
                        enableSenderUIId = 1,
                        newSenderUIEnabledUrl = ""
                    });
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = methodName;
                loggerModelNew.Message = "API EndPoint - Exception at SaveEnvelopeStep1Details method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Unable to process the documents, please try again or contact administrator.",
                    data = new List<ErrorTagDetailsResponse>(),
                    field = "",
                    enableSenderUIId = 1,
                    newSenderUIEnabledUrl = ""
                });
            }
        }

        /// <summary>
        /// SendEnvelope
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiPrepareEnvelope"></param>
        /// <returns></returns>
        public async Task<IResult> SendEnvelope(HttpRequest request, EnvelopeControls apiEnvelopeControls)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SendEnvelope", "Process startred for saving the Envelope Controls", apiEnvelopeControls.EnvelopeID.ToString(), "", "", remoteIpAddress, "SendEnvelopeWithListSF");
            rSignLogger.RSignLogInfo(loggerModelNew);
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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    Guid UserId = userToken.UserID;
                    apiEnvelopeControls.IpAddress = remoteIpAddress;

                    //Shared Envelope Access                   ;
                    string isActingUserPerformingAction = "false";
                    Guid? actionPerformedBy = null;
                    if (!string.IsNullOrEmpty(apiEnvelopeControls.IsActingUserPerformingAction) && apiEnvelopeControls.IsActingUserPerformingAction.ToLower() == "true")
                    {
                        actionPerformedBy = userToken.UserID; //Logged user ID
                        isActingUserPerformingAction = "true";
                        userToken.EmailId = apiEnvelopeControls.EnvelopeSenderUserEmail;
                        UserId = apiEnvelopeControls.EnvelopeSenderUserID;
                        apiEnvelopeControls.UserId = UserId;
                    }
                    //Shared Envelope Access

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "Envelope";
                    rSignAPIPayload.APIMethod = "SendEnvelope";
                    rSignAPIPayload.PayloadTypeId = Convert.ToString(apiEnvelopeControls.EnvelopeID);
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = JsonConvert.SerializeObject(apiEnvelopeControls);
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    if (apiEnvelopeControls.EnvelopeStage == Constants.String.RSignStage.PrepareDraft || apiEnvelopeControls.EnvelopeStage == Constants.String.RSignStage.UpdateAndResend)
                    {
                        await _envelopeRepository.UpdateEnvelopeJSON(new Guid(apiEnvelopeControls.EnvelopeID), UserId);
                    }
                    apiEnvelopeControls.UserToken = userToken.AuthToken;
                    apiEnvelopeControls.PrefillSourceURL = _userTokenRepository.GetPreparePageSource(request);
                    return Results.Ok(await _envelopeRepository.SendEnvelope(apiEnvelopeControls, UserId, userToken.EmailId!, apiEnvelopeControls.SourceUrlType, actionPerformedBy));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at SendEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                ResponseMessageWithEnvlpGuid responseMessage = new ResponseMessageWithEnvlpGuid();
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.StatusMessage = "SendEnvelope save method is failed.";
                responseMessage.Message = "Envelope prepare is failed.";
                responseMessage.EnvelopeId = apiEnvelopeControls.EnvelopeID;
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// This api will be used for Get Envelope Settings Details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <param name="isEnvelopeArichived"></param>
        /// <returns></returns>
        public async Task<IResult> GetEnvelopeSettingsDetails(HttpRequest request, string envelopeId, string languageCode, int isEnvelopeArichived = 0)
        {
            CustomEnvelopeSettingsDetails responseMessage = new CustomEnvelopeSettingsDetails();
            try
            {
                var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopeSettingsDetails", "Process started for Get Envelope Settings Details for envelopeId:" + envelopeId, envelopeId, "", "", remoteIpAddress, "GetEnvelopeSettingsDetailsAPI");
                rSignLogger.RSignLogInfo(loggerModelNew);
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _envelopeRepository.GetEnvelopeSettings(new Guid(envelopeId), languageCode, isEnvelopeArichived));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetEnvelopeSettingsDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.EnvelopeViewSettingDetails = null;
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "Error";
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// This api will be used for discardning the envelope
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> DiscardEnvelope(HttpRequest request, string envelopeId)
        {
            ResponseMessageForEnvelope responseMessage = new ResponseMessageForEnvelope();
            try
            {
                var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "DiscardEnvelope", "Process started for Discarding the Envelope for envelopeId:" + envelopeId, envelopeId, "", "", remoteIpAddress, "DiscardEnvelopeAPI");
                rSignLogger.RSignLogInfo(loggerModelNew);

                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _envelopeRepository.DiscardEnvelope(new Guid(envelopeId), userToken.EmailId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at DiscardEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "Error";
                responseMessage.Message = Convert.ToString(_appConfiguration["DiscardFail"].ToString());
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// This api will be used for Save Draft Envelope
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<IResult> SaveDraftEnvelope(HttpRequest request, SaveDraft envelope)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SaveDraftEnvelope", "Process startred for saving the Draft Envelope", envelope.EnvelopeID.ToString(), "", "", remoteIpAddress, "SaveDraftEnvelope");
            rSignLogger.RSignLogInfo(loggerModelNew);
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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    //Shared Envelope Access
                    string isActingUserPerformingAction = "false";
                    if (!string.IsNullOrEmpty(envelope.IsActingUserPerformingAction) && envelope.IsActingUserPerformingAction.ToLower() == "true")
                    {
                        isActingUserPerformingAction = "true";
                        userToken.UserID = envelope.EnvelopeSenderUserID;
                        userToken.EmailId = envelope.EnvelopeSenderUserEmail;
                    }
                    //Shared Envelope Access

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "Envelope";
                    rSignAPIPayload.APIMethod = "SaveDraftEnvelope";
                    rSignAPIPayload.PayloadTypeId = Convert.ToString(envelope.EnvelopeID);
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = JsonConvert.SerializeObject(envelope);
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    if (envelope.EnvelopeStage == Constants.String.RSignStage.PrepareDraft || envelope.EnvelopeStage == Constants.String.RSignStage.UpdateAndResend)
                    {
                        await _envelopeRepository.UpdateEnvelopeJSON(new Guid(envelope.EnvelopeID), userToken.UserID);
                    }
                    var data = await _draftRepository.SaveDraft(userToken, envelope);
                    return Results.Ok(data);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at SaveDraftEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                ResponseMessageWithEnvlpGuid responseMessage = new ResponseMessageWithEnvlpGuid();
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.StatusMessage = "SaveDraftEnvelope save method is failed.";
                responseMessage.Message = "Envelope save draft is failed.";
                responseMessage.EnvelopeId = envelope.EnvelopeID;
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// OpenEnvelope
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<IResult> OpenEnvelope(HttpRequest request, SendEnvelopeRequestPOCO envelope)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "OpenEnvelope", "Process startred for saving the Draft Envelope", envelope.EnvelopeID, "", "", remoteIpAddress, "OpenEnvelope");
            rSignLogger.RSignLogInfo(loggerModelNew);
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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    //Shared Envelope Access
                    string isActingUserPerformingAction = "false";
                    if (!string.IsNullOrEmpty(envelope.IsActingUserPerformingAction) && envelope.IsActingUserPerformingAction.ToLower() == "true")
                    {
                        isActingUserPerformingAction = "true";
                        userToken.UserID = envelope.EnvelopeSenderUserID;
                        userToken.EmailId = envelope.EnvelopeSenderUserEmail;
                    }
                    //Shared Envelope Access

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "Envelope";
                    rSignAPIPayload.APIMethod = "OpenEnvelope";
                    rSignAPIPayload.PayloadTypeId = Convert.ToString(envelope.EnvelopeID);
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = JsonConvert.SerializeObject(envelope);
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    Guid envelopeID = new(envelope.EnvelopeID);
                    if (envelope.EnvelopeStage == Constants.String.RSignStage.PrepareDraft || (envelope.EnvelopeStage == Constants.String.RSignStage.PrepareEnvelope))
                    {
                        await _envelopeRepository.UpdateEnvelopeJSON(envelopeID, userToken.UserID);
                    }
                    SaveDraft savePrepareDraft = new()
                    {
                        EnvelopeStage = Constants.String.RSignStage.PrepareDraft,
                        EnvelopeID = Convert.ToString(envelope.EnvelopeID),
                        IsSaveControl = envelope.IsSaveControl,
                        Controls = envelope.Controls,
                        EnableMultipleBranding = envelope.EnableMultipleBranding,
                        Branding = envelope.Branding,
                        SenderCompanyID = envelope.SenderCompanyID,
                        IsActingUserPerformingAction = isActingUserPerformingAction,
                        EnvelopeSenderUserEmail = envelope.EnvelopeSenderUserEmail,
                        EnvelopeSenderUserID = envelope.EnvelopeSenderUserID,
                    };
                    var data = await _draftRepository.SaveDraft(userToken, savePrepareDraft);

                    loggerModelNew.Message = _envelopeHelperMain.GetLanguageCodeBasedApiMessge("EnvelopeDraftSuccess", envelope.SessionCulture);
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    string encryptedGlobalEnvelopeID = EncryptDecryptQueryString.Encrypt(envelope.EnvelopeID, Convert.ToString(_appConfiguration["AppKey"])!);

                    return Results.Ok(new
                    {
                        statusCode = data.StatusCode,
                        message = data.Message,
                        returnUrl = encryptedGlobalEnvelopeID
                    });
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at OpenEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                ResponseMessageWithEnvlpGuid responseMessage = new();
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.StatusMessage = "OpenEnvelope save method is failed.";
                responseMessage.Message = "Envelope save draft is failed.";
                responseMessage.EnvelopeId = envelope.EnvelopeID.ToString();
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// This method used to Get Document Image Details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="reqObj"></param>
        /// <returns></returns>
        public async Task<IResult> GetDocumentImageDetails(HttpRequest request, SendEnvelopeDocumentRequest reqObj)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetImages", "Method Initialized,to Get Envelope Images by either envelopeId ", reqObj.EnvelopeID.ToString(), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                List<EnvelopeImageInformationDetails> lstData = _envelopeHelperMain.GetEnvelopeDocumentImageInfoDetails(reqObj, "envelope");
                return Results.Ok(new
                {
                    statusCode = HttpStatusCode.OK,
                    data = lstData
                });
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API Envelope EndPoint - Exception at GetDocumentImageDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest();
            }
        }
        /// <summary>
        /// This method is used to create envelope with selected templates data for Envelope
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<IResult> UseMultiTemplates(HttpRequest request, TemplateDataList tData)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UseMultiTemplates", "Process startred for saving the multi templates data for Envelope", "", "", "", remoteIpAddress, "UseMultiTemplates");
            rSignLogger.RSignLogInfo(loggerModelNew);
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
                    tData.IPAddress = remoteIpAddress;
                    string domainUrlType = "2";
                    try
                    {
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                    }
                    catch (Exception)
                    {
                        loggerModelNew.Message = "API EndPoint - Exception at UseMultiTemplates method and error - Original source url is:" + domainUrlType;
                        rSignLogger.RSignLogError(loggerModelNew);
                    }
                    if (tData.IsActindUserPerformingAction == "true")
                    {
                        userToken.EmailId = tData.SharedUserEmail;
                        userToken.UserID = (Guid)tData.SharedUserId;
                    }
                    return Results.Ok(await _envelopeRepository.UseMultiTemplates(userToken, tData, domainUrlType));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at UseMultiTemplates method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                ResponseMessageForEnvelopeWithUseTemplate responseMessage = new ResponseMessageForEnvelopeWithUseTemplate();
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.StatusMessage = "Error";
                responseMessage.Message = "UseMultiTemplates failed to saving the multi templates data to store Envelope";
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// This method isused to saving the multi templates data for Envelope
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tData"></param>
        /// <returns></returns>
        public async Task<IResult> UseMultiTemplatesUpdateRecepientsDocument(HttpRequest request, TemplateDataList tData)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UseMultiTemplates", "Process startred for saving the multi templates data for Envelope", "", "", "", remoteIpAddress, "UseMultiTemplates");
            rSignLogger.RSignLogInfo(loggerModelNew);
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
                    tData.IPAddress = remoteIpAddress;
                    string domainUrlType = "2";
                    //string domainUrlType = "11";
                    try
                    {
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                    }
                    catch (Exception)
                    {
                        loggerModelNew.Message = "API EndPoint - Exception at UseMultiTemplates method and error - Original source url is:" + domainUrlType;
                        rSignLogger.RSignLogError(loggerModelNew);
                    }
                    if (tData.IsActindUserPerformingAction == "true")
                    {
                        userToken.EmailId = tData.SharedUserEmail;
                        userToken.UserID = (Guid)tData.SharedUserId;
                    }
                    return Results.Ok(await _envelopeRepository.UseMultiTemplatesUpdateRecepientsDocument(userToken, tData, domainUrlType));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at UseMultiTemplatesUpdateRecepientsDocument method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                ResponseMessageForEnvelopeWithUseTemplate responseMessage = new ResponseMessageForEnvelopeWithUseTemplate();
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.StatusMessage = "Error";
                responseMessage.Message = "UseMultiTemplates failed to saving the multi templates data to store Envelope";
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// GetRequiredEnvelopeDetails - This is used for step1 envelope initialize only when template or rule consumed 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> GetRequiredEnvelopeDetails(HttpRequest request, string envelopeId, string companyId, string userTypeId)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopeDetails", "Endpoint Initialized,to Get Envelope Details by either envelopeId or envelopeCode", envelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                    return Results.Ok(await _envelopeRepository.GetRequiredEnvelopeDetails(envelopeId, userToken, companyId, userTypeId, remoteIpAddress));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetRequiredEnvelopeDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Get Shared Access Senders List
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <param name="companyId"></param>
        /// <param name="userTypeId"></param>
        /// <returns>IResult</returns>
        public async Task<IResult> GetSharedAccessSendersList(HttpRequest request, Guid userId, Guid companyId)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetSharedAccessSendersList", "Process started for Get Shared Access Senders List for mentioned User ID...." + Convert.ToString(userId), "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                    return Results.Ok(await _envelopeRepository.GetSharedAccessSendersList(userId, companyId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetSharedAccessSendersList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        ///  This method used to Save User Envelope Grid Preferences 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="gridPreferencesColumnsModal"></param>
        /// <returns></returns>
        public async Task<IResult> SaveUserEnvelopeGridPreferences(HttpRequest request, UserEnvelopeGridPreferencesColumnsModal gridPreferencesColumnsModal)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SaveUserEnvelopeGridPreferences", "Process started for SaveUserEnvelopeGridPreferences", remoteIpAddress, "Send API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageForEnvelopeGridPreferences responseMessage = new ResponseMessageForEnvelopeGridPreferences();

            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);

                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken!);
                    if (userToken == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                        rSignAPIPayload.PayloadType = "Envelope";
                        rSignAPIPayload.APIMethod = "SaveUserEnvelopeGridPreferences";
                        rSignAPIPayload.PayloadTypeId = "";
                        rSignAPIPayload.UserEmail = "";
                        rSignAPIPayload.PayloadInfo = JsonConvert.SerializeObject(gridPreferencesColumnsModal);
                        rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                        _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                        bool status = _envelopeRepository.SaveUserEnvelopeGridPreferences(gridPreferencesColumnsModal, userToken.UserID.ToString());
                        return Results.Ok(responseMessage);
                    }

                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using DocumentPackageIndex method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Get autolock activity log based on input model
        /// </summary>
        /// <param name="request"></param>
        /// <param name="activityLogModel"></param>
        /// <returns></returns>
        public async Task<IResult> GetAutoLockActivityLog(HttpRequest request, ActivityLogModel activityLogModel)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetAutoLockActivityLog", "Endpoint Initialized,to Get Autolock Activity Log Details by envelopeId", activityLogModel.EnvelopeID.ToString(), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting GetAutoLockActivityLog details by envelopeId=" + activityLogModel.EnvelopeID;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "Envelope";
                    rSignAPIPayload.APIMethod = "GetAutoLockActivityLog";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = activityLogModel.EnvelopeID.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    AutoLockActivityLogResp activityLogResp = new AutoLockActivityLogResp();
                    activityLogResp = _envelopeRepository.GetAutoLockActivityLog(activityLogModel);
                    return Results.Ok(activityLogResp);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetAutoLockActivityLog method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Get Document Controls Count based on document id 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public async Task<IResult> GetDocumentControlsCount(HttpRequest request, string docId, string type)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetDocumentControlsCount", "Endpoint Initialized,to Get document controls by document id:" + docId, docId.ToString(), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting GetDocumentControlsCount details by document id=" + docId.ToString();
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    int controlsCount = _envelopeRepository.GetControlsCount(new Guid(docId), type);

                    ResponseMessageWithEnvlpGuid responseMessage = new ResponseMessageWithEnvlpGuid();
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "Success";
                    responseMessage.Message = Convert.ToString(controlsCount);
                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetDocumentControlsCount method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Load Forensic Audit Trail based on signerDiagnosticsId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="signerDiagnosticsId"></param>
        /// <returns></returns>
        public async Task<IResult> LoadForensicAuditTrail(HttpRequest request, int signerDiagnosticsId)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadForensicAuditTrail", "Endpoint Initialized,to load Forensic Audit Trail by envelopeId", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Loading Forensic Audit Trail details.";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    ForensicAuditTrailAPI loadForensicAuditTrail = new ForensicAuditTrailAPI();
                    loadForensicAuditTrail = _envelopeRepository.LoadForensicAuditTrail(signerDiagnosticsId);
                    return Results.Ok(loadForensicAuditTrail);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at LoadForensicAuditTrail method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<IResult> GetRecipients(HttpRequest request, string term, Guid? userRole)
        {
            ResponseMessageForRecipientList responseMessage = new();
            string payloadData = $"term = {term}, userRole = {userRole}";
            loggerModelNew = new LoggerModelNew("", "Envelope", "GetRecipients" + payloadData, "Get Envelope Recipients by UserId using API.", "");
            string emailID;
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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting GetRecipients";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    emailID = userToken.EmailId ?? string.Empty;
                    responseMessage = await _envelopeRepository.GetRecipients(term, userRole, emailID);
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "Success";
                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetRecipients method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<IResult> GetSenderListForCompanyorGroup(HttpRequest request, GenerateSenderListSuggestions generateSenderListSuggestions)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "User", "GetSenderListForCompanyorGroup", "Process started for Get Sender List For Company or Group", "", "", "", "", "GetSenderListForCompanyorGroup");
            string emailID;
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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting GetSenderListForCompanyorGroup";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    emailID = userToken.EmailId ?? string.Empty;
                    return Results.Ok(await _envelopeRepository.GetSenderListForCompanyorGroup(generateSenderListSuggestions, emailID, userToken.ID));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetSenderListForCompanyorGroup method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This method is used to consume group template
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tData"></param>
        /// <returns></returns>
        public async Task<IResult> UseTemplateGroups(HttpRequest request, TemplateGroupDataList tData)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "User", "UseTemplateGroups", "Process started for Use Template Groups", "", "", "", "", "UseTemplateGroups");
            string emailID;
            var query = request.Query;
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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting UseTemplateGroups";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    emailID = userToken.EmailId ?? string.Empty;
                    return Results.Ok(await _envelopeRepository.UseTemplateGroups(tData, userToken.EmailId, userToken, request.Query));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at UseTemplateGroups method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<IResult> EnvelopeGroupIndex(HttpRequest request, EnvelopePrepareModal EnvelopePrepareModal)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "User", "EnvelopeGroupIndex", "Process has started for Use Template Groups", "", "", "", "", "EnvelopeGroupIndex");
            string emailID;
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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting EnvelopeGroupIndex";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    emailID = userToken.EmailId ?? string.Empty;
                    EnvelopePrepareRequest envelope = new EnvelopePrepareRequest();
                    string additionalAttachments = string.Empty;
                    if (EnvelopePrepareModal != null && !string.IsNullOrEmpty(EnvelopePrepareModal.Envelope))
                    {
                        envelope = JsonConvert.DeserializeObject<EnvelopePrepareRequest>(EnvelopePrepareModal.Envelope);
                        loggerModelNew.Message = "Serializing envelope string object is:" + EnvelopePrepareModal.Envelope;
                    }
                    if (envelope == null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "Failed";
                        responseMessage.Message = "Group envelope index data is not available.";
                        return Results.BadRequest(responseMessage);
                    }
                    return Results.Ok(await _envelopeRepository.EnvelopeGroupIndex(envelope, userToken));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at EnvelopeGroupIndex method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<IResult> EnvelopeGroupData(HttpRequest request, TemplateGroupDataList tData)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "User", "EnvelopeGroupData", "Process has started for pageRefreshGroupsData", "", "", "", "", "EnvelopeGroupData");
            string emailID;
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
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting EnvelopeGroupData";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    emailID = userToken.EmailId ?? string.Empty;
                    return Results.Ok(await _envelopeRepository.EnvelopeGroupData(tData));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at EnvelopeGroupIndex method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<IResult> SendGroupEnvelope(HttpRequest request, SendGroupEnvelopeRequest sendGroupEnvelopeRequest)
        {
            ResponseMessageForEnvelope responseMessage = new ();
            EnvelopeControls envelope = new(); string emailID;
            try
            {
                var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                string additionalAttachments = string.Empty;
                loggerModelNew = new LoggerModelNew("", _module, "SendGroupEnvelope", "Process started for SendGroupEnvelope the Envelope for envelopeId:" + sendGroupEnvelopeRequest.Envelope, "", "", remoteIpAddress, "SendGroupEnvelope");
                rSignLogger.RSignLogInfo(loggerModelNew);
                if (sendGroupEnvelopeRequest != null && !string.IsNullOrEmpty(sendGroupEnvelopeRequest.Envelope))
                    envelope = JsonConvert.DeserializeObject<EnvelopeControls>(sendGroupEnvelopeRequest.Envelope);
                if (sendGroupEnvelopeRequest != null) additionalAttachments = sendGroupEnvelopeRequest.AdditionalAttachments;

                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    emailID = userToken.EmailId ?? string.Empty;
                    if (envelope == null)
                    {
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "Failed";
                        responseMessage.Message = "Group envelope data is not available.";
                        return Results.BadRequest(responseMessage);
                    }
                    return Results.Ok(await _envelopeRepository.SendGroupEnvelope(envelope, additionalAttachments, emailID, userToken));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at SendGroupEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "Error";
                responseMessage.Message = Convert.ToString(_appConfiguration["DiscardFail"].ToString());
                return Results.BadRequest(responseMessage);
            }
        }
    }
}
