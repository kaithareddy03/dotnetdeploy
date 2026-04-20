using RSign.Common;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Text.RegularExpressions;

namespace RSign.SendAPI.API
{
    public class TemplateDocumentEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();       
        private readonly string _module = "TemplateDocumentEndPoint";
        private IHttpContextAccessor _accessor;        
        private readonly IConfiguration _appConfiguration;
        private IUserTokenRepository _userTokenRepository;
        private readonly ICommonHelper _commonHelper;
        private ITemplateDocumentRepository _templateDocumentRepository;
        private readonly IDocumentRepository _documentRepository;

        public TemplateDocumentEndPoint(IHttpContextAccessor accessor, IConfiguration appConfiguration, IUserTokenRepository userTokenRepository, ICommonHelper commonHelper, ITemplateDocumentRepository templateDocumentRepository, IDocumentRepository documentRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _userTokenRepository = userTokenRepository;
            rSignLogger = new RSignLogger(_appConfiguration); 
            _commonHelper = commonHelper;
            _templateDocumentRepository = templateDocumentRepository;
            _documentRepository = documentRepository;
        }
        public void RegisterTemplateDocumentAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Template/LoadTemplateDocumentDetails", LoadTemplateDocumentDetails);
            app.MapPost("/api/v1/Template/DeleteFile", DeleteFile);
            app.MapPost("/api/v1/Template/DeleteDraftFile", DeleteDraftFile);
        }
        /// <summary>
        /// This method used to Get Document Details for template id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> LoadTemplateDocumentDetails(HttpRequest request, string templateId, string culture)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadTemplateDocumentDetails", "Endpoint Initialized,to Get Document Details for:" + templateId, templateId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("LoadTemplateDocumentDetails");
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _templateDocumentRepository.LoadTemplateDocumentDetails(new Guid(templateId), culture));
                }
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("LoadTemplateDocumentDetails", templateId, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to delete Document Details for template id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="templateId"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteFile(HttpRequest request, DeleteDocFromDirecotoryModel deleteDocFromDirecotoryModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteFile", "Endpoint Initialized,to delete document for:" + deleteDocFromDirecotoryModel.EnvelopeId, deleteDocFromDirecotoryModel.EnvelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("DeleteFile");
                    return Results.Unauthorized();
                }
                else
                {                    
                    return Results.Ok(await _documentRepository.DeleteDocFromDirectory(deleteDocFromDirecotoryModel));
                }
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("DeleteFile", deleteDocFromDirecotoryModel.EnvelopeId, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to delete draft document details for template id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="deleteDraftDocumentModel"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteDraftFile(HttpRequest request, DeleteDraftDocumentModel deleteDraftDocumentModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteDraftFile", "Endpoint Initialized,to Delete Draft File for:" + deleteDraftDocumentModel.EnvelopeId, deleteDraftDocumentModel.EnvelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    _commonHelper.LogUnauthorizedAccess("DeleteDraftFile");
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _templateDocumentRepository.DeleteDraftDocument(deleteDraftDocumentModel));
                }
            }
            catch (Exception ex)
            {
                _commonHelper.LogError("DeleteDraftFile", deleteDraftDocumentModel.EnvelopeId, ex);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
