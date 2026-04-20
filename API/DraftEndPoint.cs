using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;

namespace RSign.SendAPI.API
{
    public class DraftEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private IDraftRepository _draftRepository;
        private readonly string _module = "DraftEndPoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;   
        private readonly IConfiguration _appConfiguration;
        private ISettingsRepository _settingsRepository;     
        private readonly IESignHelper _esignHelper;
        private readonly IMasterDataRepository _masterDataRepository;

        public DraftEndPoint(IHttpContextAccessor accessor, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration, IDraftRepository draftRepository,
            IESignHelper esignHelper, ISettingsRepository settingsRepository, IMasterDataRepository masterDataRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;           
            _userTokenRepository = userTokenRepository;
            _draftRepository= draftRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _settingsRepository = settingsRepository;
            _esignHelper = esignHelper;  
            _masterDataRepository = masterDataRepository;
        }
        public void RegisterDraftAPI(WebApplication app)
        {
            app.MapPost("/api/v1/Draft/EditDraft", EditDraft);
            app.MapPost("/api/v1/Draft/SaveDraftUpdated", SaveDraftUpdated);
        }
        /// <summary>
        /// This method used to get Edit Draft envelope details 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sendDraftModal"></param>
        /// <returns></returns>
        public async Task<IResult> EditDraft(HttpRequest request, EditDraftModal editDraftModal)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "EditDraft", "Process started for getting the details for draft envelope", "", "", "", remoteIpAddress, "SendDraft");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "EditDraft";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {                    
                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "Envelope";
                    rSignAPIPayload.APIMethod = "EditDraft";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = JsonConvert.SerializeObject(editDraftModal); ;
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _masterDataRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    return Results.Ok(await _draftRepository.EditDraft(editDraftModal, userToken));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "EditDraft";
                loggerModelNew.Message = "API EndPoint - Exception at EditDraft method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to get save draft envelopes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="saveDraftStr"></param>
        /// <returns></returns>
        public async Task<IResult> SaveDraftUpdated(HttpRequest request, string saveDraftStr)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SaveDraftUpdated", "Process started for getting the details for draft envelope", "", "", "", remoteIpAddress, "SaveDraftUpdated");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SaveDraftUpdated";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "Envelope";
                    rSignAPIPayload.APIMethod = "SaveDraftUpdated";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = saveDraftStr;
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _masterDataRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    SaveDraft tempSaveDraftDetails = JsonConvert.DeserializeObject<SaveDraft>(saveDraftStr, settings);
                    APISettings apiUserSettings = _settingsRepository.GetEntityByParam(userToken.UserID, string.Empty, Constants.String.SettingsType.User);
                    var userSettings = _esignHelper.TransformSettingsDictionaryToEntity(apiUserSettings);
                    return Results.Ok(await _draftRepository.SaveDraftUpdated(userToken, tempSaveDraftDetails, userSettings));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SaveDraftUpdated";
                loggerModelNew.Message = "API EndPoint - Exception at SaveDraftUpdated method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
