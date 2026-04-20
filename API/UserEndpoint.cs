using RSign.Common;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Net;

namespace RSign.SendAPI.API
{
    public class UserEndpoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly string _module = "UserEndpoint";
        private readonly IUserRepository _userRepository;
        private IHttpContextAccessor _accessor;
        private readonly IConfiguration _appConfiguration;
        private readonly IAuthenticateRepository _authenticateRepository;
        private IUserTokenRepository _userTokenRepository;

        public UserEndpoint(IUserRepository userRepository, IHttpContextAccessor accessor, IConfiguration appConfiguration, IAuthenticateRepository authenticateRepository, IUserTokenRepository userTokenRepository)
        {
            _userRepository = userRepository;
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _authenticateRepository = authenticateRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _userTokenRepository = userTokenRepository;
        }
        public void RegisterEnvelopeAPI(WebApplication app)
        {
            app.MapGet("/api/v1/User/GetUserPlanDetails", GetUserPlanDetails);
            app.MapGet("/api/v1/User/GetUserSignatureText", GetUserSignatureText);
            app.MapGet("/api/v1/User/GetAddressList", GetAddressList);
            app.MapGet("/api/v1/User/GetCompanyList", GetCompanyList);
            app.MapGet("/api/v1/User/GetUserCompanyList", GetUserCompanyList);
            app.MapGet("/api/v1/User/GetCompanyorGroupList", GetCompanyorGroupList);
            app.MapPost("/api/v1/User/GetMailTemplatePreview", GetMailTemplatePreview);
        }

        /// <summary>
        /// This method used to get the user plan details based on user email
        /// </summary>
        /// <param name="request"></param>        
        /// <param name="userEmail"></param>
        /// <returns>user details</returns>
        //[Authorize]
        public async Task<IResult> GetUserPlanDetails(HttpRequest request, string userEmail)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew(userEmail, _module, "GetUserPlanDetails", "Method Initialized,to Get User Details by user id ", "", "", "", remoteIpAddress, "GetUserPlanDetails");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");
                else return Results.Unauthorized();
                UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                CustomUserUsageRemaining customUserUsageRemaining = new CustomUserUsageRemaining();
                RestResponseUserInfo rSignPlan = _authenticateRepository.GetUserInfoFromRCS(authToken, userEmail);
                bool isError = false;
                if (rSignPlan == null || rSignPlan.ResultContent == null)
                {
                    loggerModelNew.Message = _appConfiguration["NotAssociatedPlan"];
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    customUserUsageRemaining.Message = loggerModelNew?.Message;
                    isError = true;
                }
                else if (rSignPlan.ResultContent.Plan == null)
                {
                    loggerModelNew.Message = _appConfiguration["NoPlanAssociatedForUser"];
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    customUserUsageRemaining.Message = loggerModelNew?.Message;
                    isError = true;
                }
                else if (rSignPlan.ResultContent.Customer == null)
                {
                    loggerModelNew.Message = _appConfiguration["NotAssociatedWithCompany"];
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    customUserUsageRemaining.Message = loggerModelNew?.Message;
                    isError = true;
                }
                if (isError)
                {
                    customUserUsageRemaining.Status = "Fail";
                    customUserUsageRemaining.StatusCode = HttpStatusCode.BadRequest;
                    customUserUsageRemaining.StatusText = "Error";
                    customUserUsageRemaining.UsageRemaining = null;
                    return Results.BadRequest(customUserUsageRemaining);
                }
                else
                {
                    customUserUsageRemaining = await _userRepository.GetUserPlanDetails(authToken, userEmail);
                    if (customUserUsageRemaining != null)
                    {
                        customUserUsageRemaining.PlanName = rSignPlan.ResultContent.Plan.Name;
                        customUserUsageRemaining.Unit = rSignPlan.ResultContent.Plan.AllowedUnits - rSignPlan.ResultContent.Plan.UnitsSent;
                    }
                    return Results.Ok(customUserUsageRemaining);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetUserPlanDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogWarn(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to get the user signature texts based on user id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IResult> GetUserSignatureText(HttpRequest request, string userId)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetUserSignatureText", "Endpoint Initialized,to Get User Signature Text List", "", "", "", remoteIpAddress, "SendAPI");
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
                    return Results.Ok(await _userRepository.GetUserSignatureText(new Guid(userId), !string.IsNullOrEmpty(userToken.EmailId) ? userToken.EmailId : ""));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetUserSignatureText method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to get the user address list on search input text
        /// </summary>
        /// <param name="request"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public async Task<IResult> GetAddressList(HttpRequest request, string term)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetAddressList", "Endpoint Initialized, for Get recipeints email & name which are already in database using Autocomplete", "", "", "", remoteIpAddress, "SendAPI");
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
                    return Results.Ok(await _userRepository.GetAddressList(term, userToken.UserID));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetAddressList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to get the company information based on search input text
        /// </summary>
        /// <param name="request"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public async Task<IResult> GetCompanyList(HttpRequest request, string term)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetCompanyList", "Process started for Get Company list using API by term.", "", "", "", remoteIpAddress, "SendAPI");
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
                    return Results.Ok(await _userRepository.GetCompanyList(term));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetCompanyList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<IResult> GetUserCompanyList(HttpRequest request, string term, string companyName, string userType, string companyId)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetUserCompanyList", "Process started for Get user details based on Company using API by term.", "", "", "", remoteIpAddress, "SendAPI");
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
                    return Results.Ok(await _userRepository.GetUserCompanyList(term, companyName, userType, companyId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetUserCompanyList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetCompanyorGroupList(HttpRequest request, string term)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetUserCompanyList", "Process started for Get user details based on Company using API by term.", "", "", "", remoteIpAddress, "SendAPI");
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
                    return Results.Ok(_userRepository.GetCompanyorGroupList(term));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetUserCompanyList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetMailTemplatePreview(HttpRequest request, MailTemplateBanner mailTemplateBanner)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetUserSignatureText", "Endpoint Initialized,to Get User Signature Text List", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessage responseMessage = new ResponseMessage();
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
                    //List<UserSignature> list = await _userRepository.GetUserSignatureText(new Guid(userId), !string.IsNullOrEmpty(userToken.EmailId) ? userToken.EmailId : "")
                    responseMessage.data = _userRepository.GetMailTemplatePreview(mailTemplateBanner);

                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetUserSignatureText method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

    }
}

