using RSign.Common;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Net;

namespace RSign.SendAPI.API
{
    public class AddOnEndpoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();     
        private readonly string _module = "AddOnEndpoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;    
        private IAddOnRepository _addOnRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly IAuthenticateRepository _authenticateRepository;

        public AddOnEndpoint(IHttpContextAccessor accessor, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration, IAddOnRepository addOnRepository, IAuthenticateRepository authenticateRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;          
            _userTokenRepository = userTokenRepository;
            _addOnRepository = addOnRepository;
            _authenticateRepository = authenticateRepository;
            rSignLogger = new RSignLogger(_appConfiguration);    
        }
        public void RegisterAddOnAPI(WebApplication app)
        {
            app.MapPost("/api/v1/AddOn/GetAddOnPlanDetails", GetAddOnPlanDetails);
            app.MapGet("/api/v1/User/GetUserPlanAndAddOnDetails", GetUserPlanAndAddOnDetails);            
        }
        /// <summary>
        /// GetAddOnPlanDetails - Retrieves AddOn plan details based on the user 
        /// </summary>
        /// <param name="request">request parameter</param>
        /// <param name="AddOnPlanRequest">AddOnPlanRequest model</param>
        /// <returns>AddOn plan details</returns>
        public async Task<IResult> GetAddOnPlanDetails(HttpRequest request, AddOnPlanRequest addOnPlanRequest)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request); 
            loggerModelNew = new LoggerModelNew("", _module, "GetAddOnPlanDetails", "Endpoint Initialized,to get AddOn details", "", "", "", remoteIpAddress, "SendAPI");
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
                    return Results.Ok(await _addOnRepository.GetAddOnPlanDetails(userToken, addOnPlanRequest));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "GetAddOnPlanDetails";
                loggerModelNew.Message = "API EndPoint - Exception at GetAddOnPlanDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This method used to get the user plan and addon details based on user email
        /// </summary>
        /// <param name="request"></param>        
        /// <param name="userEmail"></param>
        /// <returns>user details</returns>
        //[Authorize]
        public async Task<IResult> GetUserPlanAndAddOnDetails(HttpRequest request, string userEmail)
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

                ServicePlanResponse servicePlanResponse = new();
                RestResponseUserInfo rSignPlan = _authenticateRepository.GetUserInfoFromRCS(authToken, userEmail);
                bool isError = false;
                if (rSignPlan == null || rSignPlan.ResultContent == null)
                {
                    loggerModelNew.Message = _appConfiguration["NotAssociatedPlan"];
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    servicePlanResponse.Message = loggerModelNew.Message;
                    isError = true;
                }
                else if (rSignPlan.ResultContent.Plan == null)
                {
                    loggerModelNew.Message = _appConfiguration["NoPlanAssociatedForUser"];
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    servicePlanResponse.Message = loggerModelNew.Message;
                    isError = true;
                }
                else if (rSignPlan.ResultContent.Customer == null)
                {
                    loggerModelNew.Message = _appConfiguration["NotAssociatedWithCompany"];
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    servicePlanResponse.Message = loggerModelNew.Message;
                    isError = true;
                }
                if (isError)
                {
                    servicePlanResponse.Status = "Fail";
                    servicePlanResponse.StatusCode = HttpStatusCode.BadRequest;
                    servicePlanResponse.StatusText = "Error";
                    return Results.BadRequest(servicePlanResponse);
                }
                else
                {
                    //servicePlanResponse.CustomUserUsageRemaining = await _userRepository.GetUserPlanDetails(authToken, userEmail);
                    if (rSignPlan != null && rSignPlan.ResultContent != null && rSignPlan.ResultContent.Plan != null)
                    {
                        servicePlanResponse.PlanName = rSignPlan?.ResultContent?.Plan?.Name;
                        servicePlanResponse.ReminingUnits = rSignPlan.ResultContent.Plan.AllowedUnits - rSignPlan.ResultContent.Plan.UnitsSent;
                        servicePlanResponse.Plan = rSignPlan.ResultContent.Plan;
                        
                    }
                    if (rSignPlan != null && rSignPlan.ResultContent != null && rSignPlan.ResultContent.Customer != null)
                    {
                        servicePlanResponse.Customer = rSignPlan.ResultContent.Customer;
                        servicePlanResponse.AddOnPlan = new AddOnPlan();
                        SMSAddOnResponseMessage SMSAddOnPlanDetails = await _addOnRepository.GetAddOnPlanDetails(userToken, new AddOnPlanRequest { CompanyReference = rSignPlan?.ResultContent?.Customer?.ReferenceKey, EmailId = userEmail });
                        if(SMSAddOnPlanDetails != null && SMSAddOnPlanDetails.AddOnPlan != null && !string.IsNullOrEmpty(SMSAddOnPlanDetails.AddOnPlan.AddOnName))
                        {
                            servicePlanResponse.AddOnPlan = SMSAddOnPlanDetails.AddOnPlan;
                        }
                    }
                    servicePlanResponse.StatusCode = HttpStatusCode.OK;
                    servicePlanResponse.Status = "Success";
                    return Results.Ok(servicePlanResponse);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetUserPlanDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogWarn(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
