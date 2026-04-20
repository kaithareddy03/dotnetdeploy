using RSign.Common;
using RSign.Models;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Repository;

namespace RSign.SendAPI.API
{
    public class RoleEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly string _module = "RoleEndPoint";
        private IHttpContextAccessor _accessor;
        private readonly IConfiguration _appConfiguration;
        private IUserTokenRepository _userTokenRepository;
        private readonly ICommonHelper _commonHelper;
        private IRoleRepository _roleRepository;

        public RoleEndPoint(IHttpContextAccessor accessor, IConfiguration appConfiguration, IUserTokenRepository userTokenRepository, ICommonHelper commonHelper, IRoleRepository roleRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _userTokenRepository = userTokenRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _commonHelper = commonHelper;
            _roleRepository = roleRepository;
        }
        public void RegisterRoleAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Template/LoadTemplateRoleDetails", LoadTemplateRoleDetails);
            app.MapDelete("/api/v1/Template/DeleteRole", DeleteRole);
        }
        /// <summary>
        /// This is used to Get Recipients details By EnvelopeId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> LoadTemplateRoleDetails(HttpRequest request, string templateId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadTemplateRoleDetails", "Endpoint Initialized,to Get Role Details by template Id:" + templateId, templateId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("LoadTemplateRoleDetails");
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _roleRepository.LoadTemplateRoleDetails(userToken, new Guid(templateId)));
                }
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("LoadTemplateRoleDetails", templateId, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #region
        /// <summary>Created on 13-11-2025
        /// This method is used to delete tempalate role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="RecipientRowId"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteRole(HttpRequest request, Guid RoleId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteRole", "Endpoint Initialized,to delete Role Details", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteRole";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _roleRepository.DeleteRole(RoleId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "DeleteRole";
                loggerModelNew.Message = "API EndPoint - Exception at DeleteRole method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
