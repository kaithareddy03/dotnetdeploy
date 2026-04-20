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
using System.Web.Mvc;

namespace RSign.SendAPI.API
{
    public class TemplateEndpoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly string _module = "TemplateEndpoint";
        private IUserTokenRepository _userTokenRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly ITemplateRepository _templateRepository;
        private IHttpContextAccessor _accessor;
        private readonly IESignHelper _esignHelper;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private ISettingsRepository _settingsRepository;
        private readonly IMasterDataRepository _masterDataRepository;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly ITemplateInfoRepository _templateInfoRepository;
        private readonly ICommonHelper _commonHelper;

        public TemplateEndpoint(IHttpContextAccessor accessor, ITemplateRepository templateRepository, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration,
           ISettingsRepository settingsRepository, IESignHelper esignHelper, IEnvelopeHelperMain envelopeHelperMain,
            IMasterDataRepository masterDataRepository, IEnvelopeRepository envelopeRepository,
           ITemplateInfoRepository templateInfoRepository, ICommonHelper commonHelper)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _templateRepository = templateRepository;
            _userTokenRepository = userTokenRepository;
            _settingsRepository = settingsRepository;
            _esignHelper = esignHelper;
            rSignLogger = new RSignLogger(_appConfiguration);
            _envelopeHelperMain = envelopeHelperMain;
            _masterDataRepository = masterDataRepository;
            _envelopeRepository = envelopeRepository;
            _templateInfoRepository = templateInfoRepository;
            _commonHelper = commonHelper;
        }

        public void RegisterTemplateAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Template/GetTemplateDetails", GetTemplateDetails);
            app.MapGet("/api/v1/Template/GetImages", GetImages);
            app.MapGet("/api/v1/Template/GetTemplateSettingsDetails/{envelopeId}/{isEnvelopeArichived?}", GetTemplateSettingsDetails);
            app.MapGet("/api/v1/Template/GetMessageTemplateById", GetMessageTemplateById);
            app.MapGet("/api/v1/Template/CreateStaicLink", CreateStaicLink);
            app.MapGet("/api/v1/Template/UpdateStaticLink", UpdateStaticLink);
            app.MapGet("/api/v1/Template/GetTemplateHistory", GetTemplateHistory);
            app.MapPost("/api/v1/Template/SaveTemplateOrRule", SaveTemplateOrRule);
            app.MapPost("/api/v1/Template/SaveTemplateStep1Details", SaveTemplateStep1Details);
            app.MapPost("/api/v1/Template/GetDocumentImageDetails", GetDocumentImageDetails);
            app.MapPost("/api/v1/Template/GetConsumableListForEnvelope", GetConsumableListForEnvelope);
            app.MapPost("/api/v1/Template/GetTemplateList", GetTemplateList);
            app.MapPost("/api/v1/Template/GetTemplateListDetails", GetTemplateListDetails);
            app.MapPost("/api/v1/Template/LoadTemplateInitialDetails", LoadTemplateInitialDetails);
            app.MapPost("/api/v1/Template/EditTemplate", EditTemplate);
            app.MapPost("/api/v1/Template/ShareTemplateMessageTemplate", ShareTemplateMessageTemplate);
            app.MapPost("/api/v1/Template/SaveSharedTemplateOrMessageTemplate", SaveSharedTemplateOrMessageTemplate);
            app.MapPost("/api/v1/Template/CreateorUpdateMessageTemplate", CreateorUpdateMessageTemplate);
            app.MapGet("/api/v1/Template/EditMessageTemplate", EditMessageTemplate);
            app.MapGet("/api/v1/Template/CopyTemplateOrRule", CopyTemplateOrRule);
            app.MapPost("/api/v1/Template/InitializeTemplateGroup", InitializeTemplateGroup);
            app.MapPost("/api/v1/Template/EditDocumentGroup", EditDocumentGroup);
            app.MapPost("/api/v1/Template/SaveTemplateGroups", SaveTemplateGroups);
            app.MapPost("/api/v1/Template/GetConsumableListForTemplateGroup", GetConsumableListForTemplateGroup);
            app.MapPost("/api/v1/Template/DeleteTemplateGroupContent", DeleteTemplateGroupContent);
            app.MapPost("/api/v1/Template/DeleteTemplateGroupUploads", DeleteTemplateGroupUploads);
            app.MapDelete("/api/v1/Template/DeleteTemplate", DeleteTemplate);
            app.MapPost("/api/v1/Template/PrepareEditTemplateRolesAndDocuments", PrepareEditTemplateRolesAndDocuments);
        }

        /// <summary>
        /// This api will be used to SaveTemplateStep1Details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templatePrepareModal"></param>
        /// <returns></returns>
        public async Task<IResult> SaveTemplateStep1Details(HttpRequest request, TemplatePrepareModal templatePrepareModal)
        {
            try
            {
                string methodName = "SaveTemplateStep1Details";
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "SaveTemplateStep1Details", "Process started for Save Template Step1 Details", "", "", "", remoteIpAddress, "SaveTemplateStep1Details ");
                rSignLogger.RSignLogInfo(loggerModelNew);
                TemplatePrepareRequest templateAPI = new TemplatePrepareRequest();
                string additionalAttachments = string.Empty;
                if (templatePrepareModal != null && !string.IsNullOrEmpty(templatePrepareModal.Template))
                {
                    templateAPI = JsonConvert.DeserializeObject<TemplatePrepareRequest>(templatePrepareModal.Template);
                    loggerModelNew.Message = "Serializing template string object is:" + templatePrepareModal.Template;
                    loggerModelNew.EnvelopeId = Convert.ToString(templateAPI.GlobalEnvelopeID);
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "Template";
                    rSignAPIPayload.APIMethod = "SaveTemplateStep1Details";
                    rSignAPIPayload.PayloadTypeId = Convert.ToString(templateAPI.GlobalEnvelopeID);
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = JsonConvert.SerializeObject(templatePrepareModal.Template);
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _masterDataRepository.InsertRSignAPIPayload(rSignAPIPayload);
                }

                if (templatePrepareModal != null && !string.IsNullOrEmpty(templatePrepareModal.AdditionalAttachments))
                    additionalAttachments = templatePrepareModal.AdditionalAttachments;

                if (templateAPI != null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process started for Save Template Step1 Details method for template creation:" + templateAPI.GlobalEnvelopeID;
                    loggerModelNew.EnvelopeId = Convert.ToString(templateAPI.GlobalEnvelopeID);
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    string userEmail = string.Empty;
                    request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                    string? authToken = iHeader.ElementAt(0);
                    if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                    UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken);
                    if (userToken == null)
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = methodName;
                        loggerModelNew.Message = "User Token is null for User Id:";
                        loggerModelNew.EnvelopeId = Convert.ToString(templateAPI.GlobalEnvelopeID);
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
                    userEmail = userToken != null && !string.IsNullOrEmpty(userToken.EmailId) ? userToken.EmailId : _userTokenRepository.GetUserEmailByToken(authToken);
                    string authRefKey = userToken != null && !string.IsNullOrEmpty(userToken.ReferenceKey) ? userToken.ReferenceKey : string.Empty;

                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process started for get user profile for user email:" + userEmail;
                    loggerModelNew.AuthRefKey = authRefKey;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    UserProfile userProfile = _userTokenRepository.GetUserProfileByEmail(userEmail);
                    Guid userId = userToken != null ? userToken.UserID : userProfile.UserID;
                    templateAPI.IpAddress = remoteIpAddress;
                    templateAPI.CultureInfo = userProfile.LanguageCode;

                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process completed for get user and company settings for user id:" + Convert.ToString(userId) + " and company id:" + Convert.ToString(userProfile.CompanyID);
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

                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process started for Template Repository-TemplateIndex for Template id:" + Convert.ToString(templateAPI.GlobalEnvelopeID);
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    string domainUrlType = "2";
                    try
                    {
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                    }
                    catch (Exception)
                    {
                        loggerModelNew.Message = "API EndPoint - Exception at SaveTemplateStep1Details method and error - Original source url is:" + domainUrlType;
                        rSignLogger.RSignLogError(loggerModelNew);
                    }

                    ResponseMessageWithTemplateGuid responseMessageWithTemplateGuid = await _templateRepository.TemplateIndex(templateAPI, additionalAttachments, userId, userProfile, userSettings, companySettings, templatePrepareModal.IsDocumentUpload, domainUrlType);

                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "Process completed for Template Repository-TemplateIndex for Template id:" + Convert.ToString(templateAPI.GlobalEnvelopeID);
                    rSignLogger.RSignLogInfo(loggerModelNew);
                   
                    if (responseMessageWithTemplateGuid.StatusCode == HttpStatusCode.OK)
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = methodName;
                        loggerModelNew.Message = "Process started for generating encrypted new prepare page URL for template id:" + Convert.ToString(templateAPI.GlobalEnvelopeID);
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        string encryptedGlobalEnvelopeID = EncryptDecryptQueryString.Encrypt(Convert.ToString(templateAPI.GlobalEnvelopeID), Convert.ToString(_appConfiguration["AppKey"]));
                        string encryptedTokenEnvelopeId = HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("rpk={0}&eid={1}&type={2}&EmailId={3}&su={4}&sha={5}", authToken, Convert.ToString(templateAPI.GlobalEnvelopeID), "Template", userEmail, domainUrlType, "false"), Convert.ToString(_appConfiguration["AppKey"])));
                        string encryptedNewPreparePageURL = Convert.ToString(_appConfiguration["NewPrepareURL"]) + encryptedTokenEnvelopeId;

                        return Results.Ok(new
                        {
                            success = true,
                            message = responseMessageWithTemplateGuid.Message,
                            data = responseMessageWithTemplateGuid.ErrorTagDetailsResponse,
                            field = encryptedGlobalEnvelopeID,
                            enableSenderUIId = userSettings.EnableSenderUIId,
                            newSenderUIEnabledUrl = userSettings.EnableSenderUIId == 2 ? encryptedNewPreparePageURL : encryptedNewPreparePageURL,
                            
                            GlobalEnvelopeID = Convert.ToString(templateAPI.GlobalEnvelopeID),
                            SourceUrlType = domainUrlType,                           
                            PrepareType = "Template"
                        });
                    }
                    else
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = methodName;
                        loggerModelNew.Message = "Process failed because SaveTemplateStep1Details response is not success for envelope id:" + Convert.ToString(templateAPI.GlobalEnvelopeID);
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        return Results.BadRequest(new
                        {
                            success = false,
                            message = "Unable to process the documents, please try again or contact administrator.",
                            data = new List<ErrorTagDetailsResponse>(),
                            field = Convert.ToString(templateAPI.GlobalEnvelopeID),
                            enableSenderUIId = 1,
                            newSenderUIEnabledUrl = ""
                        });
                    }
                }
                else
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = methodName;
                    loggerModelNew.Message = "template is not serialized and template is null or template id not found.";
                    rSignLogger.RSignLogError(loggerModelNew);

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
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SaveTemplateStep1Details";
                loggerModelNew.Message = "API EndPoint - Exception at SaveTemplateStep1Details method and error message is:" + ex.ToString();
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
        /// This api will be used to GetTemplateDetails based on template id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> GetTemplateDetails(HttpRequest request, string templateId)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetTemplateDetails", "Endpoint Initialized,to Get Template Details by template id:" + templateId, templateId, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetTemplateDetails";
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

                    string domainUrlType = "2";
                    try
                    {
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                    }
                    catch (Exception)
                    {
                        loggerModelNew.Message = "API EndPoint - Exception at GetTemplateDetails method and error - Original source url is:" + domainUrlType;
                        rSignLogger.RSignLogError(loggerModelNew);
                    }

                    return Results.Ok(await _templateRepository.GetTemplateDetails(templateId, userToken.EmailId, domainUrlType, false, false, userToken));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "GetTemplateDetails";
                loggerModelNew.Message = loggerModelNew.Message = "API EndPoint - Exception at GetTemplateDetails method for template id:" + templateId + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This api will be used to GetImages based on template id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <param name="envelopeId"></param>
        /// <param name="uncPath"></param>
        /// <returns></returns>

        [AllowAnonymous]
        public IResult GetImages(HttpRequest request, string id, string envelopeId, string uncPath)
        {
            //loggerModelNew = new LoggerModelNew("", _module, "GetImages", "Method Initialized,to Get Envelope Images by either envelopeId ", envelopeId, "SendAPI");
            //rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                byte[] imgData = _templateRepository.GetImages(request, id, envelopeId, uncPath);

                MemoryStream ms = new MemoryStream(imgData);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                response.Content.Headers.ContentLength = ms.Length;
                return Results.Bytes(imgData, "image/png", $"{envelopeId}.jpg");
            }
            catch (Exception ex)
            {
                //loggerModelNew.Message = loggerModelNew.Message = "API EndPoint - Exception at GetImages method and error message is:" + ex.ToString();
                //rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest();
            }
        }

        /// <summary>
        /// This method used to save template or rule from template page step 2
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templateDetails"></param>
        /// <returns></returns>
        public async Task<IResult> SaveTemplateOrRule(HttpRequest request, SaveTemplateFromPrepare templateDetails)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SaveTemplateOrRule", "Method Initialized,to Save Template Or Rule for template id:" + Convert.ToString(templateDetails.TemplateID), Convert.ToString(templateDetails.TemplateID), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                ResponseMessageForSaveTemplate responseMessageForTemplate = new ResponseMessageForSaveTemplate();
                string userEmail = string.Empty; Guid userId = Guid.NewGuid();
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                authToken = authToken?.Replace("Bearer ", "");
                authToken = authToken?.Replace("bearer ", "");
                authToken = authToken?.Replace("undefined", "");

                if (!string.IsNullOrEmpty(authToken))
                {
                    UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken);
                    if (userToken == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    userEmail = userToken != null && !string.IsNullOrEmpty(userToken.EmailId) ? userToken.EmailId : _userTokenRepository.GetUserEmailByToken(authToken);
                    string authRefKey = userToken != null && !string.IsNullOrEmpty(userToken.ReferenceKey) ? userToken.ReferenceKey : string.Empty;
                    UserProfile userProfile = _userTokenRepository.GetUserProfileByEmail(userEmail);
                    if (userProfile == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    userId = userToken != null ? userToken.UserID : userProfile.UserID;
                }
                else if (string.IsNullOrEmpty(authToken) && templateDetails.requestType.ToLower() == "autosave")
                {
                    userEmail = string.Empty;
                }

                RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                rSignAPIPayload.PayloadType = "Template";
                rSignAPIPayload.APIMethod = "SaveTemplateOrRule";
                rSignAPIPayload.PayloadTypeId = Convert.ToString(templateDetails.TemplateID);
                rSignAPIPayload.UserEmail = "";
                rSignAPIPayload.PayloadInfo = JsonConvert.SerializeObject(templateDetails);
                rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                _masterDataRepository.InsertRSignAPIPayload(rSignAPIPayload);

                bool? isSaveControl = !(string.IsNullOrEmpty(Convert.ToString(templateDetails.IsSaveControl))) ? templateDetails.IsSaveControl : true;
                if (templateDetails.Stage == Constants.String.RSignStage.PrepareEditTemplate && isSaveControl.Value)
                {
                    loggerModelNew.EnvelopeId = Convert.ToString(templateDetails.TemplateID);
                    loggerModelNew.Message = "Calling TransferTemplateJSONWithList api and templateDetails.Stage is:" + templateDetails.Stage;
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    ResponseMessageForEnvelope responseTempJsonTransfer = _templateRepository.TransferTemplateJSONWithList(templateDetails.TemplateID, userEmail, userId);

                    loggerModelNew.Message = "Successfully completed the process of calling TransferTemplateJSONWithList api for template id:" + Convert.ToString(templateDetails.TemplateID);
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }

                EnvelopeControls envelopeControls = new EnvelopeControls();
                envelopeControls.EnvelopeID = Convert.ToString(templateDetails.TemplateID);
                envelopeControls.UserToken = authToken;
                envelopeControls.IpAddress = remoteIpAddress;
                envelopeControls.EnvelopeTypeId = templateDetails.EnvelopeTypeId;
                envelopeControls.Stage = templateDetails.Stage;
                envelopeControls.IsStaticLinkDisabled = !templateDetails.isEnableStaticLink;
                envelopeControls.Controls = templateDetails.Controls;
                envelopeControls.IsSaveControl = isSaveControl.Value;
                //Added by Naveen -  to stop creating version when template autosaved for every 2 mins
                envelopeControls.IsAutoSave = templateDetails.IsAutoSave;
                envelopeControls.UserId = userId;
                if (isSaveControl.Value)
                {
                    envelopeControls.IsMultiBrandingEnabled = templateDetails.EnableMultipleBranding;
                    envelopeControls.Branding = templateDetails.Branding;
                }

                if (templateDetails != null && templateDetails.TemplateID != null && templateDetails.Controls != null)
                {
                    loggerModelNew.Message = "Total no of controls for template id:" + Convert.ToString(templateDetails.TemplateID) + " and count is:" + templateDetails.Controls.Count;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }

                //Save Template Or Rule
                loggerModelNew.Message = "Calling SaveTemplateWithList and Process started for Save Template for:" + Convert.ToString(templateDetails.TemplateID);
                rSignLogger.RSignLogInfo(loggerModelNew);

                ResponseMessageForTemplate responseTemp = _templateRepository.SaveTemplateWithList(envelopeControls, userEmail, userId);
                string successMessage = responseTemp.LanguageBasedApiMessge;

                string errorMessage = "Error occurred while creating/updating template.";
                if (templateDetails.EnvelopeTypeId == Constants.EnvelopeType.TemplateRule)
                {
                    errorMessage = "Error occurred while creating/updating Rule.";
                }
                if (responseTemp.StatusCode == HttpStatusCode.OK && templateDetails.Source == "Back")
                {
                    loggerModelNew.Message = successMessage + " TemplateID=" + responseTemp.TemplateId;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessageForTemplate.StatusMessage = successMessage;
                    responseMessageForTemplate.ReturnURL = "Template/EditTemplate";
                    responseMessageForTemplate.StatusCode = responseTemp.StatusCode;
                    responseMessageForTemplate.TemplateId = Convert.ToString(templateDetails.TemplateID);
                    responseMessageForTemplate.EncryptedEnvelopeID = EncryptDecryptQueryString.Encrypt(Convert.ToString(templateDetails.TemplateID), Convert.ToString(_appConfiguration["AppKey"]));

                    return Results.Ok(responseMessageForTemplate);
                }
                else if (responseTemp.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(responseTemp.IntegrationUrl))
                {
                    loggerModelNew.Message = successMessage + " TemplateID=" + responseTemp.TemplateId;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessageForTemplate.StatusMessage = successMessage;
                    responseMessageForTemplate.StatusCode = responseTemp.StatusCode;
                    responseMessageForTemplate.TemplateId = Convert.ToString(templateDetails.TemplateID);
                    responseMessageForTemplate.EncryptedEnvelopeID = EncryptDecryptQueryString.Encrypt(Convert.ToString(templateDetails.TemplateID), Convert.ToString(_appConfiguration["AppKey"]));
                    if (!string.IsNullOrEmpty(responseTemp.IntegrationUrl))
                        responseMessageForTemplate.ReturnURL = responseTemp.IntegrationUrl;
                    return Results.Ok(responseMessageForTemplate);
                }
                else if (responseTemp.StatusCode == HttpStatusCode.OK)
                {
                    loggerModelNew.Message = successMessage + " TemplateID=" + responseTemp.TemplateId;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessageForTemplate.StatusMessage = successMessage;
                    responseMessageForTemplate.ReturnURL = "Template/Index";
                    responseMessageForTemplate.StatusCode = responseTemp.StatusCode;
                    responseMessageForTemplate.TemplateId = Convert.ToString(templateDetails.TemplateID);
                    return Results.Ok(responseMessageForTemplate);
                }
                else
                {
                    loggerModelNew.Message = "Error occurred while creating/updating template for template id:" + Convert.ToString(templateDetails.TemplateID);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessageForTemplate.StatusMessage = errorMessage;
                    responseMessageForTemplate.ReturnURL = "Info/Index";
                    responseMessageForTemplate.StatusCode = responseTemp.StatusCode;
                    responseMessageForTemplate.TemplateId = Convert.ToString(templateDetails.TemplateID);
                    return Results.Ok(responseMessageForTemplate);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at SaveTemplateOrRule method for template id:" + Convert.ToString(templateDetails.TemplateID) + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest();
            }
        }
        /// <summary>
        /// This method used to Get Template Settings Details based on template id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <param name="isEnvelopeArichived"></param>
        /// <returns></returns>
        public async Task<IResult> GetTemplateSettingsDetails(HttpRequest request, string envelopeId, int isEnvelopeArichived = 0)
        {
            CustomEnvelopeSettingsDetails responseMessage = new CustomEnvelopeSettingsDetails();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
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
                    responseMessage = await _templateRepository.GetTemplateSettings(new Guid(envelopeId), isEnvelopeArichived, userToken.UserID);
                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = loggerModelNew.Message = "API EndPoint - Exception at GetTemplateSettingsDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.EnvelopeViewSettingDetails = null;
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "Error";
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// This method used to Get Document Image Detailson based on template id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="reqObj"></param>
        /// <returns></returns>
        public async Task<IResult> GetDocumentImageDetails(HttpRequest request, SendEnvelopeDocumentRequest reqObj)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetImages", "Method Initialized,to Get Envelope Images by either envelopeId ", reqObj.EnvelopeID.ToString(), "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                List<EnvelopeImageInformationDetails> lstData = _envelopeHelperMain.GetEnvelopeDocumentImageInfoDetails(reqObj, "template");
                return Results.Ok(new
                {
                    statusCode = HttpStatusCode.OK,
                    data = lstData
                });
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = loggerModelNew.Message = "API Template EndPoint - Exception at GetDocumentImageDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest();
            }
        }

        /// <summary>
        /// This method used to Get Template Details based on message Template Id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> GetMessageTemplateById(HttpRequest request, string msgTemplateId)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetMessageTemplateById", "Endpoint Initialized,to Get Message Template Details by id", msgTemplateId, "SendAPI");
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
                    return Results.Ok(await _templateRepository.GetMessageTemplateById(new Guid(msgTemplateId)));
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = loggerModelNew.Message = "API EndPoint - Exception at GetMessageTemplateById method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Get templates or rules based on filer criteria
        /// </summary>
        /// <param name="request"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IResult> GetConsumableListForEnvelope(HttpRequest request, APITemplateFilter filter)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetConsumableListForEnvelope", "Endpoint Initialized, to Get Templates list for envelope and populate in modal popup.", "", "SendAPI");
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
                    return Results.Ok(await _templateRepository.GetConsumableListForEnvelope(filter));
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = loggerModelNew.Message = "API EndPoint - Exception at GetConsumableListForEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        #region 13-10-2025 Template List  
        /// <summary>
        /// API endpoint to retrieve the list of templates associated with a specific user.
        /// Validates the user token, logs activity, and optionally decrypts query parameters for filtering.
        /// </summary>
        /// <param name="request">The incoming HTTP request containing user context and query parameters.</param>
        /// <param name="templatObj">The template search object containing user-specific search criteria.</param>
        /// <returns>
        /// An <see cref="IResult"/> containing either the list of templates for the user or an appropriate error response.
        /// </returns>
        public async Task<IResult> GetTemplateList(HttpRequest request, TemplateSearch templatObj)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetTemplateList", "Endpoint Initialized,to Get Template list by user id:" + templatObj.UserId, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("GetTemplateList");
                    return Results.Unauthorized();
                }

                var rSignAPIPayload = new RSignAPIPayload
                {
                    PayloadType = "Template",
                    APIMethod = "GetTemplateList",
                    PayloadTypeId = "",
                    UserEmail = userToken.EmailId,
                    PayloadInfo = JsonConvert.SerializeObject(templatObj),
                    CreatedDate = DateTime.UtcNow
                };
                _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                if (!string.IsNullOrEmpty(templatObj.Url))
                {
                    var queryString = _commonHelper.ExtractDecryptedQueryString();
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        var parameters = queryString.Split('&');
                        if (parameters.Length == 4)
                        {
                            var templateType = _commonHelper.GetQueryParam(parameters[0]);
                            templatObj.TemplateType = string.IsNullOrEmpty(templateType) ? 1 : Convert.ToInt32(templateType);

                            var page = _commonHelper.GetQueryParam(parameters[1]);
                            templatObj.Page = string.IsNullOrEmpty(page) ? 1 : Convert.ToInt32(page);

                            var pageSize = _commonHelper.GetQueryParam(parameters[2]);
                            templatObj.PageSize = string.IsNullOrEmpty(pageSize) ? 25 : Convert.ToInt32(pageSize);

                            templatObj.SearchText = _commonHelper.GetQueryParam(parameters[3]);
                        }
                    }
                }
                else
                {
                    templatObj.UserId = userToken.UserID;
                }

                _commonHelper.LogInfo("GetTemplateList", userToken, $"Getting user profile details by email={userToken.EmailId}");
                return Results.Ok(await _templateInfoRepository.GetTemplateTabDetailsForUser(templatObj));
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("GetTemplateList", templatObj.UserId.ToString(), ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #endregion 13-10-2025 Template List  


        #region 14-10-2025 Delete Template  
        /// <summary>
        /// This api will be used for Deleting the Templates or Rules or MessageTemplates or Group Templates
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templateID"></param>
        /// <param name="templateType"
        /// <returns></returns>
        public async Task<IResult> DeleteTemplate(HttpRequest request, string templateID, Guid templateType)
        {
            loggerModelNew = new LoggerModelNew("", _module, "DeleteTemplate", "Endpoint Initialized,to Delete Template by Template id:" + templateID, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            Models.ResponseMessageForDeleteTemplate responseMessage = new Models.ResponseMessageForDeleteTemplate();
            try
            {
                if (!string.IsNullOrWhiteSpace(templateID) && templateType != Guid.Empty)
                {
                    UserToken userToken = _userTokenRepository.ValidateToken(request);
                    if (userToken == null)
                    {
                        _commonHelper.LogUnauthorizedAccess("DeleteTemplate");
                        return Results.Unauthorized();
                    }
                    else if (templateType != Guid.Empty)
                    {
                        if (templateType == Constants.EnvelopeType.Template || templateType == Constants.EnvelopeType.TemplateRule || templateType == Constants.EnvelopeType.MessageTemplate)
                        {
                            responseMessage = await _templateInfoRepository.DeleteTemplateOrMessageTemplate(templateID, userToken.UserID, templateType);
                        }
                        else if (templateType == Constants.EnvelopeType.DocumentGroup)
                        {
                            responseMessage = await _templateInfoRepository.DeleteTemplateGroup(templateID);
                        }
                        return Results.Json(new
                        {
                            statusCode = (int)responseMessage.StatusCode,
                            statusMessage = responseMessage.StatusMessage,
                            message = responseMessage.Message,
                            templateId = responseMessage.TemplateId,
                        }, statusCode: (int)responseMessage.StatusCode);
                    }
                }
                loggerModelNew.Message = "Invalid templateId or templatetype provided: " + templateType;
                rSignLogger.RSignLogWarn(loggerModelNew);
                return Results.Json(new
                {
                    statusCode = (int)HttpStatusCode.BadRequest,
                    statusMessage = "BadRequest",
                    message = "Invalid template id or templatetype provided.",
                    templateId = templateID,
                }, statusCode: (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("DeleteTemplate", string.Empty, ex);
                return Results.Json(new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    statusMessage = "InternalServerError",
                    message = "An error occurred while deleting the template.",
                    templateId = templateID,
                }, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion 14-10-2025 Delete Template  

        #region 21-10-2025 CreateStaicLink  
        /// <summary>
        /// This method is used to create the static link
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templateCode"></param>
        /// <param name="createNewLink"></param>
        /// <returns></returns>
        public async Task<IResult> CreateStaicLink(HttpRequest request, int templateCode, bool createNewLink)
        {
            loggerModelNew = new LoggerModelNew("", _module, "CreateStaicLink", "Endpoint Initialized,to Delete Template by Template code:" + templateCode, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            APITemplateTabResponse responseMessage = new APITemplateTabResponse();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("CreateStaicLink");
                    return Results.Unauthorized();
                }
                else
                {
                    responseMessage = await _templateInfoRepository.CreateStaticLink(templateCode, userToken.UserID, createNewLink);
                    return Results.Json(responseMessage, statusCode: (int)responseMessage.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("CreateStaicLink", string.Empty, ex);
                return Results.Json(new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    statusMessage = "InternalServerError",
                    message = "An error occurred while creating the static link.",
                    templateId = templateCode,
                }, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion 21-10-2025 CreateStaicLink  

        #region 14-10-2025 UpdateStaticLink  
        /// <summary>
        /// This method is used to update (enable or disable) the static link
        /// </summary>
        /// <param name="templateCode"></param>
        /// <returns></returns>
        public async Task<IResult> UpdateStaticLink(int templateCode)
        {
            loggerModelNew = new LoggerModelNew("", _module, "UpdateStaticLink", "Endpoint Initialized,to update the static link status as Enable or Disable for Template code:" + templateCode, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForTemplate responseMessage = new ResponseMessageForTemplate();
            try
            {
                responseMessage = await _templateInfoRepository.UpdateStaticLink(templateCode);
                return Results.Json(new
                {
                    statusCode = (int)responseMessage.StatusCode,
                    statusMessage = responseMessage.StatusMessage,
                    message = responseMessage.Message,
                    templateId = responseMessage.TemplateId,
                    templateCode = responseMessage.TemplateCode
                }, statusCode: (int)responseMessage.StatusCode);
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("UpdateStaticLink", string.Empty, ex);
                return Results.Json(new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    statusMessage = "InternalServerError",
                    message = "An error occurred while updating the static link.",
                    templateId = templateCode,
                }, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion 14-10-2025 UpdateStaticLink

        #region 15-10-2025 GetTemplateHistory  
        /// <summary>
        ///  This method is used to fetch the template History for template ID
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns>ResponseMessageForTemplateHistory</returns>
        public async Task<IResult> GetTemplateHistory(Guid templateId, string userSelectedTimeZone)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetTemplateHistory", "Endpoint Initialized,to fetch the template history for Template ID:" + templateId, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForTemplateHistory responseMessage = new ResponseMessageForTemplateHistory();
            try
            {
                responseMessage = await _templateInfoRepository.GetTemplateHistory(templateId, userSelectedTimeZone);
                return Results.Json(new
                {
                    statusCode = (int)responseMessage.StatusCode,
                    statusMessage = responseMessage.StatusMessage,
                    message = responseMessage.Message,
                    templateId = responseMessage.TemplateId,
                    DateFormatID = responseMessage.DateFormatId,
                    Version = responseMessage.TemplateHistory,
                }, statusCode: (int)responseMessage.StatusCode);
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("GetTemplateHistory", string.Empty, ex);
                var errorResponse = new
                {
                    StatusCode = 500,
                    StatusMessage = "InternalServerError",
                    Message = "An error occurred while updating the static link."
                };

                return Results.Json(errorResponse, statusCode: 500);
            }
        }
        #endregion 15-10-2025 GetTemplateHistory

        #region 17-10-2025 Template Details  
        /// <summary>
        /// API endpoint to retrieve the templates details associated with a specific template.
        /// </summary>
        /// <param name="request">The incoming HTTP request containing user context and query parameters.</param>
        /// <param name="templateListDetails">The template search object containing user-specific search criteria.</param>
        /// <returns>
        /// An <see cref="IResult"/> containing either the templates details for the template or an appropriate error response.
        /// </returns>
        public async Task<IResult> GetTemplateListDetails(HttpRequest request, TemplateExpandModal templateListDetails)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetTemplateListDetails", "Endpoint Initialized,to Get Template list by Tempalte Id is " + templateListDetails.TemplateId, "", "", "", "", "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("GetTemplateListDetails");
                    return Results.Unauthorized();
                }

                var rSignAPIPayload = new RSignAPIPayload
                {
                    PayloadType = "Template",
                    APIMethod = "GetTemplateListDetails",
                    PayloadTypeId = "",
                    UserEmail = userToken.EmailId,
                    PayloadInfo = JsonConvert.SerializeObject(templateListDetails),
                    CreatedDate = DateTime.UtcNow
                };
                _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                _commonHelper.LogInfo("GetTemplateListDetails", userToken, $"Getting user profile details by email={userToken.EmailId}");
                return Results.Ok(await _templateInfoRepository.GetTemplateListDetails(templateListDetails, userToken.UserID));
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("GetTemplateListDetails", " Template Id is " + templateListDetails.TemplateId, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #endregion 17-10-2025 Template Details  
        /// <summary>
        /// 21-10-2025
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<IResult> LoadTemplateInitialDetails(HttpRequest request, Guid? CompanyId, string LanguageCode, string initialTemplateId)
        {
            loggerModelNew = new LoggerModelNew("", _module, "LoadTemplateInitialDetails", "Endpoint Initialized,to Load Template with Initial Details by Company Id is " + CompanyId, "", "", "", "", "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("LoadTemplateInitialDetails");
                    return Results.Unauthorized();
                }

                var rSignAPIPayload = new RSignAPIPayload
                {
                    PayloadType = "Template",
                    APIMethod = "LoadTemplateInitialDetails",
                    PayloadTypeId = "",
                    UserEmail = userToken.EmailId,
                    PayloadInfo = "",
                    CreatedDate = DateTime.UtcNow
                };
                _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);
                _commonHelper.LogInfo("LoadTemplateInitialDetails", userToken, $"Getting user profile details by email={userToken.EmailId}");
                return Results.Ok(await _templateInfoRepository.LoadTemplateInitialDetails(userToken.UserID, userToken.EmailId, CompanyId, LanguageCode, initialTemplateId));
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("LoadTemplateInitialDetails", " Company Id is " + CompanyId, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 22-10-2025
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Query"></param>
        /// <returns>Template Details</returns>
        public async Task<IResult> EditTemplate(HttpRequest request, string query)
        {
            loggerModelNew = new LoggerModelNew("", _module, "EditTemplate", "Endpoint Initialized,to EditTemplate", "", "", "", "", "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("EditTemplate");
                    return Results.Unauthorized();
                }

                var rSignAPIPayload = new RSignAPIPayload
                {
                    PayloadType = "Template",
                    APIMethod = "EditTemplate",
                    PayloadTypeId = "",
                    UserEmail = userToken.EmailId,
                    PayloadInfo = "",
                    CreatedDate = DateTime.UtcNow
                };

                _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                //string userURL = query;
                //userURL = HttpUtility.UrlDecode(userURL.Substring(userURL.IndexOf('?') + 1));
                //userURL = userURL.Replace(" ", "+");
                //string globalTemplateID = EncryptDecryptQueryString.Decrypt(userURL, Convert.ToString(_appConfiguration["AppKey"]));

                loggerModelNew.UserId = userToken.UserID.ToString();
                loggerModelNew.Email = userToken.EmailId.ToString();
                loggerModelNew.EnvelopeId = query;
                rSignLogger.RSignLogInfo(loggerModelNew);

                Guid templateID = new Guid(query);
                bool? IsRApp = false, IsEditTemplate = false;
                _commonHelper.LogInfo("EditTemplate", userToken, $"Getting user profile details by email={userToken.EmailId}");

                string domainUrlType = "2";
                request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);

                return Results.Ok(await _templateInfoRepository.EditTemplate(templateID, userToken.EmailId, IsRApp, IsEditTemplate, domainUrlType));
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("EditTemplate", "", ex);
                loggerModelNew.Message = "Error occured in Edit Template API End point and error is: " + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        #region 21-10-2025 ShareTemplateMessageTemplate  
        /// <summary>
        /// This method is used to share or unshare template/rule or message template
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templateID"></param>
        /// <param name="isShare"></param>
        /// <param name="isCopyOrShare"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public async Task<IResult> ShareTemplateMessageTemplate(HttpRequest request, ShareTemplateInfo shareTemplateInfo)//Guid templateID, bool isShare, int isCopyOrShare, Guid templateType)
        {
            loggerModelNew = new LoggerModelNew("", _module, "ShareTemplateMessageTemplate", "Endpoint Initialized,to Share Template for Template ID:" + Convert.ToString(shareTemplateInfo.TemplateID), "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForTemplate responseMessage = new ResponseMessageForTemplate();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("ShareTemplateMessageTemplate");
                    return Results.Unauthorized();
                }
                if (shareTemplateInfo.TemplateTypeID == Constants.EnvelopeType.MessageTemplate)
                {
                    responseMessage = await _templateInfoRepository.ShareMessageTemplate(shareTemplateInfo.TemplateID, shareTemplateInfo.IsShare);
                }
                else if (shareTemplateInfo.TemplateTypeID == Constants.EnvelopeType.Template || shareTemplateInfo.TemplateTypeID == Constants.EnvelopeType.TemplateRule)
                {
                    responseMessage = await _templateInfoRepository.ShareTemplateOrRule(shareTemplateInfo, userToken.UserID);
                }
                return Results.Json(new
                {
                    statusCode = (int)responseMessage.StatusCode,
                    statusMessage = responseMessage.StatusMessage,
                    message = responseMessage.Message,
                    templateId = responseMessage.TemplateId,
                    templateCode = responseMessage.TemplateCode
                }, statusCode: (int)responseMessage.StatusCode);
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("ShareTemplateMessageTemplate", string.Empty, ex);
                return Results.Json(new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    statusMessage = "InternalServerError",
                    message = "An error occurred while share or unshare the template/rule/message template.",
                    templateId = shareTemplateInfo.TemplateID,
                }, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion 21-10-2025 ShareTemplateMessageTemplate

        #region 22-10-2025 SaveSharedTemplateOrMessageTemplate  
        /// <summary>
        /// This method is used to save Shared tempalte/rule or message template
        /// </summary>
        /// <param name="request"></param>
        /// <param name="shareTemplateInfo"></param>
        /// <returns></returns>
        public async Task<IResult> SaveSharedTemplateOrMessageTemplate(HttpRequest request, ShareTemplateInfo shareTemplateInfo)
        {
            loggerModelNew = new LoggerModelNew("", _module, "SaveSharedTemplateOrMessageTemplate", "Endpoint Initialized,to Save Shared Template Or MessageTemplate Template ID:" + Convert.ToString(shareTemplateInfo?.TemplateTypeID), "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForTemplate responseMessage = new ResponseMessageForTemplate();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("SaveSharedTemplateOrMessageTemplate");
                    return Results.Unauthorized();
                }
                else
                {
                    if (shareTemplateInfo?.TemplateTypeID == Constants.EnvelopeType.Template || shareTemplateInfo?.TemplateTypeID == Constants.EnvelopeType.TemplateRule)
                    {
                        responseMessage = await _templateInfoRepository.AddSharedTemplate(shareTemplateInfo.TemplateID, userToken.UserID);
                    }
                    else if (shareTemplateInfo?.TemplateTypeID == Constants.EnvelopeType.MessageTemplate)
                    {
                        responseMessage = await _templateInfoRepository.AddSharedMessageTemplate(shareTemplateInfo.TemplateID, userToken.UserID);
                    }
                    return Results.Json(new
                    {
                        statusCode = (int)responseMessage.StatusCode,
                        statusMessage = responseMessage.StatusMessage,
                        message = responseMessage.Message,
                        templateId = responseMessage.TemplateId,
                        templateCode = responseMessage.TemplateCode,
                    }, statusCode: (int)responseMessage.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("SaveSharedTemplateOrMessageTemplate", string.Empty, ex);
                return Results.Json(new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    statusMessage = "InternalServerError",
                    message = "An error occurred while saving the shared template/rule or message template.",
                    templateId = Convert.ToString(shareTemplateInfo?.TemplateID),
                }, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion 22-10-2025 SaveSharedTemplateOrMessageTemplate  

        #region 27-10-2025 CreateorUpdateMessageTemplate
        /// <summary>
        /// This method is used to save the create or update message tempalte. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiMessageTemplate"></param>
        /// <returns></returns>
        public async Task<IResult> CreateorUpdateMessageTemplate(HttpRequest request, APIMessageTemplate apiMessageTemplate)
        {
            loggerModelNew = new LoggerModelNew("", _module, "CreateorUpdateMessageTemplate", "Endpoint Initialized,to Create MessageTemplate Template of name:" + Convert.ToString(apiMessageTemplate?.Name), "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForTemplate responseMessage = new ResponseMessageForTemplate();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("CreateorUpdateMessageTemplate");
                    return Results.Unauthorized();
                }
                else
                {
                    if (apiMessageTemplate != null)
                    {
                        responseMessage = await _templateInfoRepository.CreateorUpdateMessageTemplate(apiMessageTemplate, userToken.UserID);
                        return Results.Json(new
                        {
                            statusCode = (int)responseMessage.StatusCode,
                            statusMessage = responseMessage.StatusMessage,
                            message = responseMessage.Message,
                            templateId = responseMessage.TemplateId,
                            templateCode = responseMessage.TemplateCode,
                        }, statusCode: (int)responseMessage.StatusCode);
                    }
                    return Results.Json(new
                    {
                        statusCode = HttpStatusCode.BadRequest,
                        statusMessage = "BadRequest",
                        message = "Unable to save the message template.",
                    }, statusCode: (int)HttpStatusCode.BadRequest);

                }
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("CreateorUpdateMessageTemplate", string.Empty, ex);
                return Results.Json(new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    statusMessage = "InternalServerError",
                    message = "An error occurred while creating or updating the message template.",
                    templateId = Convert.ToString(apiMessageTemplate?.ID),
                }, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion 27-10-2025 CreateorUpdateMessageTemplate

        #region 28-10-2025 EditMessageTemplate
        /// <summary>
        /// This method is to fetch the message template details while editing the message template
        /// </summary>
        /// <param name="request"></param>
        /// <param name="messageTemplateID"></param>
        /// <returns></returns>
        public async Task<IResult> EditMessageTemplate(HttpRequest request, Guid messageTemplateID)
        {
            loggerModelNew = new LoggerModelNew("", _module, "EditMessageTemplate", "Endpoint Initialized,to edit MessageTemplate Template of id:" + Convert.ToString(messageTemplateID), "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            APIMessageTemplateResponse responseMessage = new APIMessageTemplateResponse();
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("EditMessageTemplate");
                    return Results.Unauthorized();
                }
                else
                {
                    if (messageTemplateID != Guid.Empty)
                    {
                        responseMessage = await _templateInfoRepository.GetMessageTemplateById(messageTemplateID, userToken.UserID);
                        return Results.Json(new
                        {
                            statusCode = (int)responseMessage.StatusCode,
                            statusMessage = responseMessage.StatusMessage,
                            message = responseMessage.Message,
                            templateId = responseMessage.ID,
                            templateCode = responseMessage.TemplateCode,
                            name = responseMessage.Name,
                            subject = responseMessage.Subject,
                            description = responseMessage.Description,
                            emailBody = responseMessage.EmailBody,
                            userSignatureTextID = responseMessage.UserSignatureTextID
                        }, statusCode: (int)responseMessage.StatusCode);
                    }
                    return Results.Json(new
                    {
                        statusCode = HttpStatusCode.BadRequest,
                        statusMessage = "BadRequest",
                        message = "Unable to edit the message template.",
                    }, statusCode: (int)HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("EditMessageTemplate", string.Empty, ex);
                return Results.Json(new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    statusMessage = "InternalServerError",
                    message = "An error occurred while editing the message template.",
                    templateId = Convert.ToString(messageTemplateID),
                }, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion 28-10-2025 EditMessageTemplate

        #region 22-10-2025 CopyTemplateOrRule  
        /// <summary>
        ///  This method is used to copy template / rule based on ID
        /// </summary>
        /// <param name="[templateId or RuleId]"></param>
        /// <returns>ResponseMessageForTemplateHistory</returns>
        public async Task<IResult> CopyTemplateOrRule(HttpRequest request, Guid templateIdOrRuleId)
        {
            loggerModelNew = new LoggerModelNew("", _module, "CopyTemplateOrRule", "Endpoint Initialized,to copy the template or rule for Template or Rule ID:" + templateIdOrRuleId, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("CopyTemplateOrRule");
                    return Results.Unauthorized();
                }
                else
                {
                    ResponseMessageCommon responseMessage = await _templateInfoRepository.CopyTemplateOrRule(templateIdOrRuleId, userToken.UserID);
                    return Results.Json(new
                    {
                        statusCode = (int)responseMessage.StatusCode,
                        statusMessage = responseMessage.StatusMessage,
                        message = responseMessage.Message,
                        data = responseMessage.Data,

                    }, statusCode: (int)responseMessage.StatusCode);
                }

            }
            catch (Exception ex)
            {
                _commonHelper.LogError("CopyTemplateOrRule", string.Empty, ex);
                return Results.Json(new
                {
                    StatusCode = 500,
                    StatusMessage = "InternalServerError",
                    Message = "An error occurred while add the template or rule."
                }, statusCode: 500);
            }
        }
        #endregion 22-10-2025 CopyTemplateOrRule

        #region 24-10-2025
        /// <summary>
        /// This end point is used to Initialize Template Group
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templateCodes"></param>
        /// <returns></returns>
        public async Task<IResult> InitializeTemplateGroup(HttpRequest request, List<int> templateCodes)
        {
            loggerModelNew = new LoggerModelNew("", _module, "InitializeTemplateGroup", "Endpoint Initialized,to Load Template Group with Initial Details by templateCodes are " + templateCodes, "", "", "", "", "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("InitializeTemplateGroup");
                    return Results.Unauthorized();
                }

                var rSignAPIPayload = new RSignAPIPayload
                {
                    PayloadType = "Template",
                    APIMethod = "InitializeTemplateGroup",
                    PayloadTypeId = "",
                    UserEmail = userToken.EmailId,
                    PayloadInfo = "",
                    CreatedDate = DateTime.UtcNow
                };
                _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);
                _commonHelper.LogInfo("InitializeTemplateGroup", userToken, $"Getting user profile details by email={userToken.EmailId}");
                return Results.Ok(await _templateInfoRepository.LoadTemplateGroupWithDetails(templateCodes, userToken.EmailId));
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("InitializeTemplateGroup", " template codes are" + templateCodes, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #endregion

        #region EditDocumentGroup 24-10-2025
        /// <summary>
        /// This method is used to get template group details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templateGroupID"></param>
        /// <returns></returns>
        public async Task<IResult> EditDocumentGroup(HttpRequest request, string templateGroupID)
        {
            loggerModelNew = new LoggerModelNew("", _module, "EditDocumentGroup", "Endpoint EditDocumentGroup,to get Template Group details template group id is " + templateGroupID, "", "", "", "", "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("EditDocumentGroup");
                    return Results.Unauthorized();
                }

                var rSignAPIPayload = new RSignAPIPayload
                {
                    PayloadType = "Template",
                    APIMethod = "EditDocumentGroup",
                    PayloadTypeId = "",
                    UserEmail = userToken.EmailId,
                    PayloadInfo = "",
                    CreatedDate = DateTime.UtcNow
                };
                _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);
                _commonHelper.LogInfo("EditDocumentGroup", userToken, $"Getting user profile details by email={userToken.EmailId}");
                return Results.Ok(await _templateInfoRepository.GetDocumentGroupById(templateGroupID, userToken.UserID, userToken.EmailId));
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("EditDocumentGroup", " template group id is " + templateGroupID, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #endregion
        #region
        /// <summary>
        /// This method is used to save group templates
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templateGroups"></param>
        /// <returns></returns>
        public async Task<IResult> SaveTemplateGroups(HttpRequest request, TemplateGroupDetails templateGroups)
        {
            loggerModelNew = new LoggerModelNew("", _module, "SaveTemplateGroups", "Endpoint Initialized,to save Template Group with templateGroups is " + templateGroups, "", "", "", "", "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                var userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("SaveTemplateGroups");
                    return Results.Unauthorized();
                }

                var rSignAPIPayload = new RSignAPIPayload
                {
                    PayloadType = "Template",
                    APIMethod = "SaveTemplateGroups",
                    PayloadTypeId = templateGroups.ToString(),
                    UserEmail = userToken.EmailId,
                    PayloadInfo = "",
                    CreatedDate = DateTime.UtcNow
                };
                _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);
                _commonHelper.LogInfo("SaveTemplateGroups", userToken, $"Getting user profile details by email={userToken.EmailId}");
                return Results.Ok(await _templateInfoRepository.AddDocumentGroup(userToken.UserID, userToken.EmailId, templateGroups));
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("SaveTemplateGroups", " template groups are" + templateGroups, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #endregion
        #region GetConsumableListForTemplateGroup 05-11-2025
        /// <summary>
        /// This method is used to get tempalte list other than existing(selected in template list)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IResult> GetConsumableListForTemplateGroup(HttpRequest request, APITemplateFilter filter)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetConsumableListForTemplateGroup", "Endpoint Initialized, to Get Templates list for envelope and populate in modal popup.", "", "SendAPI");
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
                    return Results.Ok(await _templateRepository.GetConsumableListForTemplateGroup(filter));
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = loggerModelNew.Message = "API EndPoint - Exception at GetConsumableListForTemplateGroup method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #endregion

        #region 10-11-2025
        /// <summary>
        /// This method is used to Delete Template Group Content
        /// </summary>
        /// <param name="request"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteTemplateGroupContent(HttpRequest request, string documentGroupId)
        {
            loggerModelNew = new LoggerModelNew("", _module, "DeleteTemplateGroupContent", "Endpoint Initialized, to Delete Template Group Content.", "", "SendAPI");
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
                    return Results.Ok(await _templateRepository.DeleteTemplateGroupContent(Convert.ToInt32(documentGroupId)));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = loggerModelNew.Message = "API EndPoint - Exception at DeleteTemplateGroupContent method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method is used to Delete Template Group Uploads
        /// </summary>
        /// <param name="request"></param>
        /// <param name="documentGroupId"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteTemplateGroupUploads(HttpRequest request, string documentGroupId)
        {
            loggerModelNew = new LoggerModelNew("", _module, "DeleteTemplateGroupUploads", "Endpoint Initialized, to Delete TemplateGroup Uploads.", "", "SendAPI");
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
                    return Results.Ok(await _templateRepository.DeleteTemplateGroupUploads(Convert.ToInt32(documentGroupId)));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = loggerModelNew.Message = "API EndPoint - Exception at DeleteTemplateGroupUploads method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// This method used to Update Draft Roled And Documents
        /// </summary>
        /// <param name="request"></param>
        /// <param name="documentOrderModel"></param>
        /// <returns></returns>
        public async Task<IResult> PrepareEditTemplateRolesAndDocuments(HttpRequest request, TemplateDraftRecipientsAndDocumentsModel documentDraftsModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "PrepareEditTemplateRolesAndDocuments", "Process is started for Prepare Draft Roles And Documents method for template:" + Convert.ToString(documentDraftsModel.TemplateID), Convert.ToString(documentDraftsModel.TemplateID), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "PrepareEditTemplateRolesAndDocuments";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    if (documentDraftsModel.ContractStage == Constants.String.RSignStage.InitializeEditTemplate)
                    {
                        return Results.Ok(await _templateInfoRepository.PrepareDocumentRolesForJSon(documentDraftsModel, userToken.EmailId));
                    }
                    else return Results.Ok();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "PrepareEditTemplateRolesAndDocuments";
                loggerModelNew.Message = "API EndPoint - Exception at PrepareEditTemplateRolesAndDocuments method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

    }
}
