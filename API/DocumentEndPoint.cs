using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Net;
using System.Web.Mvc;


namespace RSign.SendAPI.API
{
    public class DocumentEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentContentsRepository _documentContentsRepository;
        private readonly IIntegrationRepository _integrationRepository;
        private readonly string _module = "DocumentEndPoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserRepository _userRepository;
        private readonly IBulkUploadRepository _bulkUploadRepository;

        public DocumentEndPoint(IHttpContextAccessor accessor, IDocumentRepository documentRepository, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration, IDocumentContentsRepository documentContentsRepository,
           IIntegrationRepository integrationRepository, IUserRepository userRepository, IBulkUploadRepository bulkUploadRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _documentRepository = documentRepository;
            _userTokenRepository = userTokenRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _documentContentsRepository = documentContentsRepository;
            _integrationRepository = integrationRepository;
            _userRepository = userRepository;
            _bulkUploadRepository = bulkUploadRepository;
        }
        public void RegisterDocumentAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Document/LoadDocumentDetails", LoadDocumentDetails);
            app.MapGet("/api/v1/Document/LoadiManageDocumentsList", LoadiManageDocumentsList);
            app.MapGet("/api/v1/Document/LoadnetDocsCabinetsList", LoadnetDocsCabinetsList);
            app.MapGet("/api/v1/Document/GetDocumentControlsCount", GetDocumentControlsCount);
            app.MapGet("/api/v1/Document/LoadVincereDocumentsList", LoadVincereDocumentsList);
            app.MapGet("/api/v1/Document/LoadAppliedEpicDocumentsList", LoadAppliedEpicDocumentsList);
            app.MapGet("/api/v1/Document/LoadBullhornDocumentsList", LoadBullhornDocumentsList);          
            app.MapPost("/api/v1/Document/DeleteDocFromDirectory", DeleteDocFromDirectory);           
            app.MapPost("/api/v1/Document/DeleteDraftDocument", DeleteDraftDocument);          
            app.MapPost("/api/v1/Document/BulkUpload", BulkUpload);
            app.MapPost("/api/v1/Document/ValidateBulkUploadDocument", ValidateBulkUploadDocument);
            app.MapPost("/api/v1/Document/UploadBulkSendTemplate", UploadBulkSendTemplate);          
            app.MapPost("/api/v1/Document/LoadnetDocsClientsList", LoadnetDocsClientsList);
            app.MapPost("/api/v1/Document/LoadnetDocsMattersList", LoadnetDocsMattersList);
            app.MapPost("/api/v1/Document/LoadnetDocsFoldersList", LoadnetDocsFoldersList);
            app.MapPost("/api/v1/Document/LoadnetDocumentsList", LoadnetDocumentsList);
            app.MapDelete("/api/v1/Document/DeleteDocument", DeleteDocument);
        }
        /// <summary>
        /// This method used to Get Document Details for envelope id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> LoadDocumentDetails(HttpRequest request, string envelopeId, string culture, Guid subenvelopeId, string envelopeStage)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadDocumentDetails", "Endpoint Initialized,to Get Document Details for:" + envelopeId, envelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadDocumentDetails";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _documentRepository.LoadDocumentDetails(new Guid(envelopeId), culture, subenvelopeId, envelopeStage));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadDocumentDetails";
                loggerModelNew.Message = "API EndPoint - Exception at LoadDocumentDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This method used to Delete Document From Directory
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteDocFromDirectory(HttpRequest request, DeleteDocFromDirecotoryModel deleteDocFromDirecotoryModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteDocFromDirectory", "Endpoint Initialized,for Deleting document from mentioned Envelope id using API:" + deleteDocFromDirecotoryModel.EnvelopeId, deleteDocFromDirecotoryModel.EnvelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteDocFromDirectory";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _documentRepository.DeleteDocFromDirectory(deleteDocFromDirecotoryModel));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "DeleteDocFromDirectory";
                loggerModelNew.Message = "API EndPoint - Exception at DeleteDocFromDirectory method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Delete Document based on document id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteDocument(HttpRequest request, string envelopeId, string documentId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteDocument", "Process started for Deleting document from mentioned Envelope id using API:" + envelopeId, envelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteDocument";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _documentRepository.DeleteDocument(envelopeId, documentId));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "DeleteDocument";
                loggerModelNew.Message = "API EndPoint - Exception at DeleteDocument method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Delete draft Document based on document id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteDraftDocument(HttpRequest request, DeleteDraftDocumentModel deleteDraftDocumentModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteDraftDocument", "Endpoint Initialized,for Deleting document from mentioned Envelope id using API:" + deleteDraftDocumentModel.EnvelopeId, deleteDraftDocumentModel.EnvelopeId, "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteDraftDocument";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _documentRepository.DeleteDraftDocument(deleteDraftDocumentModel));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "DeleteDraftDocument";
                loggerModelNew.Message = "API EndPoint - Exception at DeleteDraftDocument method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method used to Get Document Controls Count based on document id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        public async Task<IResult> GetDocumentControlsCount(HttpRequest request, string documentId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetDocumentControlsCount", "Endpoint Initialized,to Get Document Control Count using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetDocumentControlsCount";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok(await _documentContentsRepository.GetDocumentContentCount(new Guid(documentId)));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "GetDocumentControlsCount";
                loggerModelNew.Message = "API EndPoint - Exception at GetDocumentControlsCount method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }


        public IResult LoadVincereDocumentsList(HttpRequest request, string EntityId, string EntityType)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadVincereDocumentsList", "Getting Vincere documents using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadVincereDocumentsList";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from Vincere";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var docs = _integrationRepository.GetVincereDocumentsList(1, 100, Convert.ToInt32(EntityId), EntityType, userProfile);
                    return Results.Ok(docs);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadVincereDocumentsList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadVincereDocumentsList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult LoadAppliedEpicDocumentsList(HttpRequest request, string EntityId, int PageNo = 1, int PageSize = 25)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadAppliedEpicDocumentsList", "Getting Applied Epic documents using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadAppliedEpicDocumentsList";
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from Applied Epic";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var docs = _integrationRepository.GetAppliedEpicDocumentsList(PageNo, PageSize, Convert.ToInt32(EntityId), userProfile);
                    return Results.Ok(docs);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadAppliedEpicDocumentsList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadAppliedEpicDocumentsList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult LoadBullhornDocumentsList(HttpRequest request, string entityId, string entityType, int PageNo = 1, int PageSize = 25)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadBullhornDocumentsList", "Getting Bullhorn documents using external APIs.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadBullhornDocumentsList";
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from Bullhorn";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var docs = _integrationRepository.GetBullhornDocumentsList(PageNo, PageSize, Convert.ToInt32(entityId), entityType, userProfile);
                    return Results.Ok(docs);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadBullhornDocumentsList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadBullhornDocumentsList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method will be used for Bulk upload
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        public async Task<IResult> BulkUpload(HttpRequest request, BulkUploadFormat uploadFile)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "BulkUpload", "Endpoint Initialized,for sending the BulkUpload.", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "BulkUpload";
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    string senderAddress = _userTokenRepository.GetSenderAddressFromHeaders(request, "");
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var bulkUploadRespResult = await _bulkUploadRepository.BulkUpload(request, userProfile, senderAddress, uploadFile, userToken);
                    if (bulkUploadRespResult.StatusCode == HttpStatusCode.OK) return Results.Ok(bulkUploadRespResult);
                    else return Results.BadRequest(bulkUploadRespResult);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "BulkUpload";
                loggerModelNew.Message = "BulkUpload EndPoint - Exception at BulkUpload method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new CustomAPIResponse()
                {
                    Status = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    StatusMessage = "BadRequest",
                    Message = "Unable to process the documents."
                });
            }
        }
        /// <summary>
        /// This method used to Validate Bulk Upload Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        public async Task<IResult> ValidateBulkUploadDocument(HttpRequest request, List<ValidateBulkUploadEntity> validateBulkUploadEntity)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "ValidateBulkUploadDocument", "Endpoint Initialized,for Validate Bulk Upload Document.", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "BulkUpload";
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    string senderAddress = _userTokenRepository.GetSenderAddressFromHeaders(request, "");
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var bulkUploadRespResult = await _bulkUploadRepository.ValidateBulkUploadDocument(request, validateBulkUploadEntity, senderAddress, userProfile);
                    if (bulkUploadRespResult.StatusCode == HttpStatusCode.OK) return Results.Ok(bulkUploadRespResult);
                    else return Results.BadRequest(bulkUploadRespResult);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "BulkUpload";
                loggerModelNew.Message = "BulkUpload EndPoint - Exception at BulkUpload method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new CustomAPIResponse()
                {
                    Status = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    StatusMessage = "BadRequest",
                    Message = "Unable to process the documents."
                });
            }
        }

        public async Task<IResult> UploadBulkSendTemplate(HttpRequest request)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UploadBulkSendTemplate", "Endpoint Initialized,for Validate Bulk Upload Document.", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "BulkUpload";
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    string senderAddress = _userTokenRepository.GetSenderAddressFromHeaders(request, "");
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);

                    if (request.Form.Files.Count > 0)
                    {
                        var Ifile = request.Form.Files[0];
                        Stream fs = Ifile.OpenReadStream();
                        var xistfileName = Ifile.FileName.Replace("%20", " ").Trim('\"');
                        System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                        Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                        string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                        UploadLocalDocument documetToUpload = new UploadLocalDocument() { FileName = xistfileName, DocumentBase64Data = base64String };

                        var bulkUploadRespResult = await _bulkUploadRepository.UploadBulkSendTemplate(request, documetToUpload, senderAddress, userProfile, userToken);
                        return Results.Ok(bulkUploadRespResult);
                    }
                    else
                    {
                        InfoResultResonse responseMessage = new InfoResultResonse();
                        responseMessage.StatusCode = HttpStatusCode.BadRequest;
                        responseMessage.StatusMessage = "Failed";
                        responseMessage.message = "Please select CSV file only.";
                        responseMessage.success = false;
                        return Results.BadRequest(responseMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "BulkUpload";
                loggerModelNew.Message = "BulkUpload EndPoint - Exception at BulkUpload method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new CustomAPIResponse()
                {
                    Status = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    StatusMessage = "BadRequest",
                    Message = "Unable to process the documents."
                });
            }
        }

        public IResult LoadiManageDocumentsList(HttpRequest request, Guid userID, string type = "Document")
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadiManageDocumentsList", "Getting iManage documents using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                //UserToken userToken = _userTokenRepository.ValidateToken(request);
                //if (userToken == null)
                //{
                //    loggerModelNew.Module = _module;
                //    loggerModelNew.Method = "LoadiManageDocumentsList";
                //    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                //    rSignLogger.RSignLogWarn(loggerModelNew);
                //    return Results.Unauthorized();
                //}
                //else
                //{
                    loggerModelNew.Message = "User Token validated and getting documents from iManage";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userID);
                    var docs = _integrationRepository.LoadiManageDocumentsList(userProfile, type);
                    return Results.Redirect(docs);
                //}
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadiManageDocumentsList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadiManageDocumentsList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult LoadnetDocsCabinetsList(HttpRequest request)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadnetDocsCabinetsList", "Getting iManage documents using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadnetDocsCabinetsList";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from netdocuments";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var docs = _integrationRepository.LoadnetDocsCabinetsList(userProfile);
                    return Results.Ok(docs);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadnetDocsCabinetsList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadnetDocsCabinetsList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult LoadnetDocsClientsList(HttpRequest request, LoadNetDocsDropdownOptions netdocsDropdownOptions)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadnetDocsClientsList", "Getting netdocuments documents using External APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadnetDocsClientsList";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from netdocuments clients";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var docs = _integrationRepository.LoadnetDocsClientsList(userProfile, netdocsDropdownOptions.Settings, netdocsDropdownOptions.CabinetId, netdocsDropdownOptions.Source);
                    return Results.Ok(docs);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadnetDocsClientsList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadnetDocsClientsList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult LoadnetDocsMattersList(HttpRequest request, LoadNetDocsDropdownOptions netdocsDropdownOptions)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadnetDocsMattersList", "Getting iManage documents using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadnetDocsMattersList";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from netdocuments";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var docs = _integrationRepository.LoadnetDocsMattersList(userProfile, netdocsDropdownOptions.Settings, netdocsDropdownOptions.CabinetId, netdocsDropdownOptions.ClientId, netdocsDropdownOptions.Source);
                    return Results.Ok(docs);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadnetDocsMattersList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadnetDocsMattersList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult LoadnetDocsFoldersList(HttpRequest request, LoadNetDocsDropdownOptions netdocsDropdownOptions)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadnetDocsFoldersList", "Getting iManage documents using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadnetDocsFoldersList";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from netdocuments";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var docs = _integrationRepository.LoadnetDocsFoldersList(userProfile, netdocsDropdownOptions.Settings, netdocsDropdownOptions.CabinetId, netdocsDropdownOptions.ClientId, netdocsDropdownOptions.FolderId, netdocsDropdownOptions.Source);
                    return Results.Ok(docs);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadnetDocsFoldersList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadnetDocsFoldersList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult LoadnetDocumentsList(HttpRequest request, netDocsAdvanceSearch ApplyFiltersData)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadnetDocumentsList", "Getting netdocuments documents using Externall APIs using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "LoadnetDocumentsList";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    loggerModelNew.Message = "User Token validated and getting documents from netdocuments";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var docs = _integrationRepository.NetDocsAdvanceSearch(userProfile, ApplyFiltersData);
                    return Results.Ok(docs);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "LoadiManageDocumentsList";
                loggerModelNew.Message = "API EndPoint - Exception at LoadiManageDocumentsList method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
