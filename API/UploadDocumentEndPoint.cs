using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Net;

namespace RSign.SendAPI.API
{
    public class UploadDocumentEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly string _module = "DocumentEndPoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly IUploadDocumentRepository _uploadDocumentRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IUserRepository _userRepository;


        public UploadDocumentEndPoint(IHttpContextAccessor accessor, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration,
          ISettingsRepository settingsRepository, IUploadDocumentRepository uploadDocumentRepository, IUserRepository userRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _userTokenRepository = userTokenRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _uploadDocumentRepository = uploadDocumentRepository;
            _settingsRepository = settingsRepository;
            _userRepository = userRepository;
        }

        public void UploadDocumentAPI(WebApplication app)
        {
            app.MapPost("/api/V1/Document/UploadDocuments", UploadDocuments);
            app.MapPost("/api/V1/Document/UpdateTemplateDocuments", UpdateTemplateDocuments);
            app.MapPost("/api/V1/Document/DownloadGoogleDropBoxOneDriveFiles", DownloadGoogleDropBoxOneDriveFiles);
            app.MapPost("/api/V1/Document/DownloadOtherDrivesDocuments", DownloadOtherDrivesDocuments);
            app.MapPost("/api/V1/Document/ConvertReviewDocumentImages", ConvertReviewDocumentImages);
            app.MapPost("/api/V1/Document/UploadUserSignature", UploadUserSignature);
            app.MapPost("/api/V1/Document/UploadUserSignatureFromOtherDrives", UploadUserSignatureFromOtherDrives);
        }
        /// <summary>
        /// This method used to upload user documents
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseMessageUploadDocument> UploadDocuments(HttpRequest request)
        {
            ResponseMessageUploadDocument responseMessageUploadDocument = new ResponseMessageUploadDocument();
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UploadDocuments", "Endpoint Initialized,to UploadDocuments Document using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                        responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                        responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                        responseMessageUploadDocument.Success = false;
                        return responseMessageUploadDocument;
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        ResponseMessageUploadDocument response = await _uploadDocumentRepository.UploadDocuments(remoteIpAddress, request, userToken.EmailId, userToken.UserID);
                        return response;
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                    responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                    responseMessageUploadDocument.Success = false;
                    return responseMessageUploadDocument;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "UploadDocuments EndPoint - Exception at UploadDocument method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                //return Results.BadRequest(ex.Message);
                return null;
            }
        }        
        /// <summary>
        /// This method used to Download Google drive, DropBox and OneDrive dcouments from respective download urls
        /// </summary>
        /// <param name="uploadDriveFilesReqModel"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseMessageUploadDocument> DownloadGoogleDropBoxOneDriveFiles(UploadDriveFiles uploadDriveFilesReqModel, HttpRequest request)
        {
            ResponseMessageUploadDocument responseMessageUploadDocument = new ResponseMessageUploadDocument();
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DownloadGoogleDropBoxOneDriveFiles", "Endpoint Initialized,to Download Google or DropBox or OneDrive documents using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
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
                        responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                        responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                        responseMessageUploadDocument.Success = false;
                        return responseMessageUploadDocument;
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        string domainUrlType = "2";
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                        ResponseMessageUploadDocument response = await _uploadDocumentRepository.DownloadGoogleDropBoxOneDriveFiles(remoteIpAddress, userToken, uploadDriveFilesReqModel, domainUrlType);
                        return response;
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                    responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                    responseMessageUploadDocument.Success = false;
                    return responseMessageUploadDocument;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "UploadDocuments EndPoint - Exception at DownloadGoogleDropBoxOneDriveFiles method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return null;
            }
        }

        public async Task<ResponseMessageUploadDocument> DownloadOtherDrivesDocuments(UploadDriveFiles uploadDriveFilesReqModel, HttpRequest request)
        {
            ResponseMessageUploadDocument responseMessageUploadDocument = new();
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DownloadOtherDrivesDocuments", "Endpoint Initialized,to DownLoad Other drives Document using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                        responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                        responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                        responseMessageUploadDocument.Success = false;
                        return responseMessageUploadDocument;
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        string domainUrlType = "2";
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                        ResponseMessageUploadDocument response = new();                        
                        response = await _uploadDocumentRepository.DownloadGoogleDropBoxOneDriveFiles(remoteIpAddress, userToken, uploadDriveFilesReqModel, domainUrlType);
                        return response;
                    }

                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                    responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                    responseMessageUploadDocument.Success = false;
                    return responseMessageUploadDocument;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "UploadDocuments EndPoint - Exception at UploadDocument method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                //return Results.BadRequest(ex.Message);
                return null;
            }
        }


        /// <summary>
        /// This is used when any review document chnaged to Sign action type
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseMessageUploadDocument> ConvertReviewDocumentImages(HttpRequest request, ReviewDocumentModal reviewDocumentModal)
        {
            ResponseMessageUploadDocument responseMessageUploadDocument = new ResponseMessageUploadDocument();
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "ConvertReviewDocumentImages", "Endpoint Initialized,to convert review document chnaged to Sign action type Document using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                        responseMessageUploadDocument.StatusCode = HttpStatusCode.BadRequest;
                        return responseMessageUploadDocument;
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        await _uploadDocumentRepository.ConvertReviewDocumentImages(remoteIpAddress, request, reviewDocumentModal, userToken.UserID);
                        return responseMessageUploadDocument;
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                    responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                    responseMessageUploadDocument.Success = false;
                    return responseMessageUploadDocument;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "UploadDocuments EndPoint - Exception at ConvertReviewDocumentImages method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return null;
            }
        }
        /// <summary>
        /// This method used to upload user documents
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseMessageUploadDocument> UpdateTemplateDocuments(HttpRequest request)
        {
            ResponseMessageUploadDocument responseMessageUploadDocument = new ResponseMessageUploadDocument();
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UpdateTemplateDocuments", "Endpoint Initialized,to UpdateTemplateDocuments Document using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                        responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                        responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                        responseMessageUploadDocument.Success = false;
                        return responseMessageUploadDocument;
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        ResponseMessageUploadDocument response = await _uploadDocumentRepository.UpdateTemplateDocuments(remoteIpAddress, request, userToken.EmailId, userToken.UserID);
                        return response;
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                    responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                    responseMessageUploadDocument.Success = false;
                    return responseMessageUploadDocument;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "UploadDocuments EndPoint - Exception at UploadDocument method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                //return Results.BadRequest(ex.Message);
                return null;
            }
        }


        /// <summary>
        /// This method used to upload user documents
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseMessageFile> UploadUserSignature(HttpRequest request)
        {
            ResponseMessageFile responseMessageUploadDocument = new ResponseMessageFile();
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UploadDocuments", "Endpoint Initialized,to UploadUserSignature Document using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

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
                        responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                        responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                        return responseMessageUploadDocument;
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        responseMessageUploadDocument = await _uploadDocumentRepository.UploadUserSignature(remoteIpAddress, request, userToken.EmailId, userToken.UserID);
                        return responseMessageUploadDocument;
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                    responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                    return responseMessageUploadDocument;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "UploadDocuments EndPoint - Exception at UploadUserSignature method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                //return Results.BadRequest(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// This method used to Download Google drive, DropBox and OneDrive dcouments from respective download urls
        /// </summary>
        /// <param name="uploadDriveFilesReqModel"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseMessageFile> UploadUserSignatureFromOtherDrives(UploadDriveFiles uploadDriveFilesReqModel, HttpRequest request)
        {
            ResponseMessageFile responseMessageUploadDocument = new ResponseMessageFile();
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "UploadUserSignatureFromOtherDrives", "Endpoint Initialized,to Download Google or DropBox or OneDrive documents using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
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
                        responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                        responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                        return responseMessageUploadDocument;
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        string domainUrlType = "2";
                        request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                        domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);
                        ResponseMessageFile response = await _uploadDocumentRepository.UploadUserSignatureFromOtherDrives(remoteIpAddress, uploadDriveFilesReqModel, request, userToken.EmailId, userToken.UserID);
                        return response;
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    responseMessageUploadDocument.StatusCode = HttpStatusCode.Unauthorized;
                    responseMessageUploadDocument.StatusMessage = "Unauthorized access.";
                    return responseMessageUploadDocument;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "UploadDocuments EndPoint - Exception at UploadUserSignatureFromOtherDrives method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return null;
            }
        }
    }
}
