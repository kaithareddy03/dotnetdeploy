using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Net;
using System.Web.Mvc;

namespace RSign.SendAPI.API
{
    public class SendEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly IDocumentRepository _documentRepository;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly string _module = "SendEndPoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;
        private ISendRepository _sendRepository;
        private IPdfRepository _pdfRepository;
        private readonly IConfiguration _appConfiguration;

        public SendEndPoint(IHttpContextAccessor accessor, IDocumentRepository documentRepository, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration,
          ISettingsRepository settingsRepository, ISendRepository sendRepository, IEnvelopeRepository envelopeRepository, IPdfRepository pdfRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _documentRepository = documentRepository;
            _userTokenRepository = userTokenRepository;
            _envelopeRepository = envelopeRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _sendRepository = sendRepository;
            _pdfRepository = pdfRepository;
        }
        public void RegisterSendAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Send/LoadEnvelopeWithDetails", LoadEnvelopeWithDetails);
            app.MapGet("/api/v1/Send/GetMessageTemplates", GetMessageTemplates);
            app.MapGet("/api/v1/Send/GetTemplateGroups", GetTemplateGroups);
            app.MapGet("/api/v1/Send/UpdateSignInSequence", UpdateSignInSequence);
            app.MapGet("/api/v1/Send/AddStandardSignDocument", AddStandardSignDocument);
            app.MapPost("/api/v1/Send/SaveDraft", SaveDraft);
            app.MapPost("/api/v1/Send/UpdateDocumentOrdering", UpdateDocumentOrdering);
            app.MapPost("/api/v1/Send/SendTemplateFromPreview", SendTemplateFromPreview);
            app.MapPost("/api/v1/Send/SendDraft", SendDraft);
            app.MapPost("/api/v1/Send/UpdateResend", UpdateResend);
            app.MapPost("/api/v1/Send/PrepareDraftRecipientsAndDocuments", PrepareDraftRecipientsAndDocuments);
            app.MapDelete("/api/v1/Send/DeleteAutocompleteSuggestion", DeleteAutocompleteSuggestion);
        }

        /// <summary>
        /// LoadEnvelopeWithDetails - This method used to intialize envelope with basic details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> LoadEnvelopeWithDetails(HttpRequest request, string cultureInfo, Guid? guid)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadEnvelopeWithDetails", "Endpoint Initialized,to Get Initial Envelope Details", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadEnvelopeWithDetails";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    cultureInfo = !string.IsNullOrEmpty(cultureInfo) ? cultureInfo : "en-us";
                    return Results.Ok(await _sendRepository.LoadEnvelopeWithInitialDetails(userToken, cultureInfo, remoteIpAddress, guid));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadEnvelopeWithDetails";
                loggerModelNew.Message = "API EndPoint - Exception at LoadEnvelopeWithDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to get Message Templates based on user id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> GetMessageTemplates(HttpRequest request, string userId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetMessageTemplates", "Endpoint Initialized,to Get Message Templates List for user id:" + Convert.ToString(userId), "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetMessageTemplates";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _sendRepository.GetMessageTemplatesList(new Guid(userId)));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "GetMessageTemplates";
                loggerModelNew.Message = "API EndPoint - Exception at GetMessageTemplates method for user id:" + Convert.ToString(userId) + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to get Templates groups based on user id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> GetTemplateGroups(HttpRequest request, string userId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetTemplateGroups", "Endpoint Initialized,to Get Message Groups Templates List for user id:" + Convert.ToString(userId), "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetTemplateGroups";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _envelopeRepository.GetTemplateGroups(new Guid(userId)));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "GetTemplateGroups";
                loggerModelNew.Message = "API EndPoint - Exception at GetTemplateGroups method method for user id:" + Convert.ToString(userId) + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to update CheckBoxSequence based on sign in sequence checked or not
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> UpdateSignInSequence(HttpRequest request, bool check, string envelopeID)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UpdateSignInSequence", "Endpoint Initialized,to check CheckBox Sequence for envelope id:" + envelopeID, envelopeID, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "UpdateSignInSequence";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _sendRepository.UpdateSignInSequence(check, new Guid(envelopeID), userToken.EmailId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "UpdateSignInSequence";
                loggerModelNew.Message = "API EndPoint - Exception at UpdateSignInSequence method for envelope id:" + envelopeID + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Save envelope as Draft from step1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="check"></param>
        /// <param name="envelopeID"></param>
        /// <returns></returns>        
        public async Task<IResult> SaveDraft(HttpRequest request, SaveDraftModel saveDraftModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SaveDraft", "Endpoint Initialized,to Save Draft from step1", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SaveDraft";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
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

                    SaveDraftModelResp saveDraftModelResp = await _sendRepository.SaveDraft(saveDraftModel, userToken, domainUrlType);
                    if (saveDraftModel.IsPreview && saveDraftModelResp.StatusCode == HttpStatusCode.OK) //ViewPdf
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = "GetEnvelopeViewPdfDocument";
                        loggerModelNew.Message = "Process started for GetEnvelopeViewPdfDocument method to view pdf of envelope id:" + saveDraftModelResp.EnvelopeID;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return Results.Ok(_pdfRepository.GetEnvelopeViewPdfDocument(userToken.EmailId, saveDraftModelResp.EnvelopeID));
                    }
                    else if (saveDraftModel.IsPreview && saveDraftModelResp.StatusCode == HttpStatusCode.NotFound) //ViewPdf
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = "GetEnvelopeViewPdfDocument";
                        loggerModelNew.Message = "SaveEnvelopePreview-->ViewPDF--> error message is:" + saveDraftModelResp.Message;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return Results.BadRequest(saveDraftModelResp);
                    }
                    else if (saveDraftModelResp.StatusCode == HttpStatusCode.NotAcceptable)
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = "GetEnvelopeViewPdfDocument";
                        loggerModelNew.Message = "SaveEnvelopePreview-->ViewPDF--> error message is:" + saveDraftModelResp.Message;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return Results.BadRequest(saveDraftModelResp);
                    }
                    return Results.Ok(saveDraftModelResp);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SaveDraft";
                loggerModelNew.Message = "API EndPoint - Exception at SaveDraft method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Update Document Ordering based on input
        /// </summary>
        /// <param name="request"></param>
        /// <param name="documentOrderModel"></param>
        /// <returns></returns>
        public async Task<IResult> UpdateDocumentOrdering(HttpRequest request, DocumentOrderModel documentOrderModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UpdateDocumentOrdering", "Process started for Update Document Ordering in case of use template and Rule for id:" + Convert.ToString(documentOrderModel.EnvelopeID), Convert.ToString(documentOrderModel.EnvelopeID), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "UpdateDocumentOrdering";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    if (documentOrderModel.ContractStage == Constants.String.RSignStage.InitializeUseTemplate || documentOrderModel.ContractStage == Constants.String.RSignStage.InitializeUseRule || documentOrderModel.ContractStage == Constants.String.RSignStage.ProcessGroup)
                    {
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };
                        if (!string.IsNullOrEmpty(documentOrderModel.DocumentsStr))
                        {
                            List<Documents> docs = JsonConvert.DeserializeObject<List<Documents>>(documentOrderModel.DocumentsStr, settings);
                            return Results.Ok(await _documentRepository.UpdateDocumentOrdering(docs, documentOrderModel.EnvelopeID));
                        }
                        else return Results.Ok();
                    }
                    else return Results.Ok();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "UpdateDocumentOrdering";
                loggerModelNew.Message = "API EndPoint - Exception at UpdateDocumentOrdering method for id:" + Convert.ToString(documentOrderModel.EnvelopeID) + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Update Draft Recipients And Documents
        /// </summary>
        /// <param name="request"></param>
        /// <param name="documentOrderModel"></param>
        /// <returns></returns>
        public async Task<IResult> PrepareDraftRecipientsAndDocuments(HttpRequest request, DraftRecipientsAndDocumentsModel documentRecipientsModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "PrepareDraftRecipientsAndDocuments", "Process is started for Prepare Draft Recipients And Documents method for envelope:" + Convert.ToString(documentRecipientsModel.EnvelopeID), Convert.ToString(documentRecipientsModel.EnvelopeID), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "PrepareDraftRecipientsAndDocuments";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    if (documentRecipientsModel.ContractStage == Constants.String.RSignStage.InitializeDraft)
                    {
                        return Results.Ok(await _sendRepository.PrepareDraftRecipientsAndDocuments(documentRecipientsModel));
                    }
                    else return Results.Ok();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "PrepareDraftRecipientsAndDocuments";
                loggerModelNew.Message = "API EndPoint - Exception at PrepareDraftRecipientsAndDocuments method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Send envelope From step1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sendTemplateFromPreviewModal"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public async Task<IResult> SendTemplateFromPreview(HttpRequest request, SendTemplateFromPreviewModal sendTemplateFromPreviewModal)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SendTemplateFromPreview", "Process started for Send Template From Send step 1", "", "", "", remoteIpAddress, "SendTemplateFromPreview");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SendTemplateFromPreview";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    bool IsTemplateDirectSend = true;
                    string domainUrlType = "2";
                    try
                    {
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                    }
                    catch (Exception)
                    {
                        loggerModelNew.Message = "API EndPoint - Exception at SendTemplateFromPreview method and error - Original source url is:" + domainUrlType;
                        rSignLogger.RSignLogError(loggerModelNew);
                    }
                    return Results.Ok(await _sendRepository.SendTemplateFromPreview(sendTemplateFromPreviewModal, IsTemplateDirectSend, userToken, remoteIpAddress, domainUrlType));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SendTemplateFromPreview";
                loggerModelNew.Message = "API EndPoint - Exception at SendTemplateFromPreview method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to get draft envelope details
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>        
        public async Task<IResult> SendDraft(HttpRequest request, EnvelopeDraftModal sendDraftModal)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SendDraft", "Process started for getting the details for draft envelope", "", "", "", remoteIpAddress, "SendDraft");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SendDraft";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    string domainUrlType = "2";
                    request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                    domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);

                    sendDraftModal.IsUpdatedResend = false;
                    return Results.Ok(await _sendRepository.GetDraftUpdateResendEnvelope(userToken, sendDraftModal, remoteIpAddress, domainUrlType));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SendDraft";
                loggerModelNew.Message = "API EndPoint - Exception at SendDraft method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to get update resend envelope details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sendDraftModal"></param>
        /// <returns></returns>
        public async Task<IResult> UpdateResend(HttpRequest request, EnvelopeDraftModal sendDraftModal)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UpdateResend", "Process started for getting the details for draft envelope", "", "", "", remoteIpAddress, "SendDraft");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "UpdateResend";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    string domainUrlType = "2";
                    request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                    domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);

                    sendDraftModal.IsUpdatedResend = true;
                    return Results.Ok(await _sendRepository.GetDraftUpdateResendEnvelope(userToken, sendDraftModal, remoteIpAddress, domainUrlType));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "UpdateResend";
                loggerModelNew.Message = "API EndPoint - Exception at UpdateResend method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Add Standard Sign Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IResult> AddStandardSignDocument(HttpRequest request, string masterEnvelopeID, string templateType, string languageCode)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "AddStandardSignDocument", "Endpoint Initialized,to Add Standard Sign Document for envelope id:" + Convert.ToString(masterEnvelopeID), masterEnvelopeID, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "AddStandardSignDocument";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _sendRepository.AddStandardSignDocument(request, masterEnvelopeID, templateType, languageCode, userToken.UserID));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "AddStandardSignDocument";
                loggerModelNew.Message = "API EndPoint - Exception at AddStandardSignDocument method method for envelope id:" + Convert.ToString(masterEnvelopeID) + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This method used to Delete Auto complete Suggestion based on input id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteAutocompleteSuggestion(HttpRequest request, string id)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteAutocompleteSuggestion", "Process,to remove the autoSuggestion id:" + Convert.ToString(id), Convert.ToString(id), "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteAutocompleteSuggestion";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["DeleteAutocompleteSuggestion"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    if (!int.TryParse(id, out var parsedId))
                    {
                        return Results.BadRequest("Invalid ID");
                    }
                    return Results.Ok(await _sendRepository.RemoveAutoFillOption(parsedId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "DeleteAutocompleteSuggestion";
                loggerModelNew.Message = "API EndPoint - Exception Occured in the DeleteAutocompleteSuggestion method for autoSuggestion id:" + Convert.ToString(id) + " and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
