using Microsoft.AspNetCore.Http.Features;
using RSign.Common;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Web.Mvc;

namespace RSign.SendAPI.API
{
    public class RecipientEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly IRecipientRepository _recipientRepository;
        private readonly IGenericRepository _genericRepository;
        private readonly string _module = "RecipientEndPoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;
        private readonly IConfiguration _appConfiguration;

        public RecipientEndPoint(IHttpContextAccessor accessor, IRecipientRepository recipientRepository, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration,
          ISettingsRepository settingsRepository, IGenericRepository genericRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _recipientRepository = recipientRepository;
            _userTokenRepository = userTokenRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _genericRepository = genericRepository;
        }
        public void RegisterRecipientAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Recipient/LoadRecipientDetails", LoadRecipientDetails);
            app.MapGet("/api/v1/Recipient/GetEnvelopeRecipients", GetEnvelopeRecipients);
            app.MapPost("/api/v1/Recipient/AddMultipleRecipeints", AddMultipleRecipeints);
            app.MapPost("/api/v1/Recipient/AddUpdateRecipient", AddUpdateRecipient);
            app.MapDelete("/api/v1/Recipient/DeleteRecipient", DeleteRecipient);
            app.MapGet("/api/v1/Recipient/GetRecipientControls", GetRecipientControls);
        }
        /// <summary>
        /// This is used to Get Recipients details By EnvelopeId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> LoadRecipientDetails(HttpRequest request, string envelopeId, string senderName, Guid subenvelopeId, string envelopeStage)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadRecipientDetails", "Endpoint Initialized,to Get Recipients Details by either envelopeId:" + envelopeId, envelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadRecipientDetails";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    var envelope = _genericRepository.GetEnvelopeRecipients(new Guid(envelopeId));
                    return Results.Ok(await _recipientRepository.LoadRecipientDetails(userToken, new Guid(envelopeId), envelope, senderName, subenvelopeId, envelopeStage));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadRecipientDetails";
                loggerModelNew.Message = "API EndPoint - Exception at LoadRecipientDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This is used to Delete Recipient by recipient Id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteRecipient(HttpRequest request, string envelopeId, string recipientId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteRecipient", "Endpoint Initialized,to delete Recipients Details", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteRecipient";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _recipientRepository.DeleteRecipient(envelopeId, recipientId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "DeleteRecipient";
                loggerModelNew.Message = "API EndPoint - Exception at DeleteRecipient method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This is used to add or update Recipient
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> AddUpdateRecipient(HttpRequest request, APIRecipient recipient)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "AddUpdateRecipient", "Endpoint Initialized,to add or update Recipients Details", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "AddUpdateRecipient";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    var envelope = _genericRepository.GetEnvelopeRecipients(recipient.EnvelopeID);
                    return Results.Ok(await _recipientRepository.AddUpdateRecipient(recipient, envelope));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "AddUpdateRecipient";
                loggerModelNew.Message = "API EndPoint - Exception at AddUpdateRecipient method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Get Recipients Details by envelopeId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> GetEnvelopeRecipients(HttpRequest request, string envelopeId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopeRecipients", "Endpoint Initialized,to Get Recipients Details by envelopeId:" + envelopeId, envelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetEnvelopeRecipients";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _recipientRepository.GetEnvelopeRecipients(new Guid(envelopeId)));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "GetEnvelopeRecipients";
                loggerModelNew.Message = "API EndPoint - Exception at GetEnvelopeRecipients method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Add Recipeints once entry added in recipient section
        /// </summary>
        /// <param name="request"></param>
        /// <param name="saveDraftModel"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public async Task<IResult> AddMultipleRecipeints(HttpRequest request, AddMultipleRecipeintsModel addMultipleRecipeintsModel)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "AddMultipleRecipeints", "Endpoint Initialized, for AddMultipleRecipeints method for envelopeId:" + Convert.ToString(addMultipleRecipeintsModel.EnvelopeID), Convert.ToString(addMultipleRecipeintsModel.EnvelopeID), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "AddMultipleRecipeints";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _recipientRepository.AddMultipleRecipeints(addMultipleRecipeintsModel));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "AddMultipleRecipeints";
                loggerModelNew.Message = "API EndPoint - Exception at AddMultipleRecipeints method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This is used to Get the Recipient Controls in the Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <param name="recipientId"></param>
        /// <returns></returns>
        /// 
        public async Task<IResult> GetRecipientControls(HttpRequest request, Guid recipientId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetRecipientControls", "Endpoint Initialized,to get Recipients Controls", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "Get Recipient controls";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _recipientRepository.GetRecipientControls(recipientId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "GetRecipientControls";
                loggerModelNew.Message = "API EndPoint - Exception at GetRecipientControls method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
