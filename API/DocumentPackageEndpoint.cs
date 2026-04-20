using Microsoft.AspNetCore.Http.Extensions;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Net;
using System.Web;

namespace RSign.SendAPI.API
{
    public class DocumentPackageEndpoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly string _module = "DocumentPackageEndPoint";
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserRepository _userRepository;
        private readonly IDocumentPackageRepository _documentPackageRepository;
        private ICompanyRepository _companyRepository;
        private IRecipientRepository _recipientRepository;

        public DocumentPackageEndpoint(IHttpContextAccessor accessor, IEnvelopeRepository envelopeRepository, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration,
        ISettingsRepository settingsRepository, IESignHelper esignHelper, IDraftRepository draftRepository, IEnvelopeHelperMain envelopeHelperMain, IUserRepository userRepository,
        IDocumentPackageRepository documentPackageRepository, ICompanyRepository companyRepository, IRecipientRepository recipientRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _envelopeRepository = envelopeRepository;
            _userTokenRepository = userTokenRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _userRepository = userRepository;
            _documentPackageRepository = documentPackageRepository;
            _companyRepository = companyRepository;
            _recipientRepository = recipientRepository;
        }

        public void DocumentPackageAPI(WebApplication app)
        {
            app.MapGet("/api/v1/DocumentPackage/AutocompleteEnvelopeCode", AutocompleteEnvelopeCode);
            app.MapGet("/api/V1/Envelope/GetEnvelopeRecipientsAudit", GetEnvelopeRecipientsAudit);
            app.MapGet("/api/v1/DocumentPackage/GetEnvelopeSettingsDetails", GetEnvelopeSettingsDetails);
            app.MapGet("/api/v1/DocumentPackage/GetRecipientsForResendEnvelope", GetRecipientsForResendEnvelope);
            app.MapGet("/api/v1/DocumentPackage/CopyEnvelope", CopyEnvelope);
            app.MapGet("/api/v1/DocumentPackage/CancelEnvelope", CancelEnvelope);
            app.MapPost("/api/v1/DocumentPackage/DocumentPackageIndex", DocumentPackageIndex);
            app.MapPost("/api/v1/DocumentPackage/GetEnvelopesBySubmit", GetEnvelopesBySubmit);
            app.MapPost("/api/v1/DocumentPackage/LoadMetaDataWithHistory", LoadMetaDataWithHistory);           
            app.MapPost("/api/v1/DocumentPackage/RemoveUploadedDocument", RemoveUploadedDocument);
            app.MapPost("/api/v1/DocumentPackage/RemoveEmailBodyForEnvelope", RemoveEmailBodyForEnvelope);
            app.MapPost("/api/v1/DocumentPackage/ViewDocument", ViewDocument);
            app.MapPost("/api/v1/DocumentPackage/DownloadEnvelopeDataXML", DownloadEnvelopeDataXML);
            app.MapPost("/api/v1/DocumentPackage/DownloadSignatureCertificate", DownloadSignatureCertificate);
            app.MapPost("/api/v1/DocumentPackage/DeleteSignatureCertificate", DeleteSignatureCertificate);
            app.MapPost("/api/v1/DocumentPackage/DownloadTransparency", DownloadTransparency);
            app.MapPost("/api/v1/DocumentPackage/GetFinalSignEnvelopeRecipients", GetFinalSignEnvelopeRecipients);
            app.MapPost("/api/v1/DocumentPackage/GetHiddenControlValuesByEnvelopeId", GetHiddenControlValuesByEnvelopeId);
            app.MapPost("/api/v1/DocumentPackage/GetSharedDocumentManageEnvelopes", GetSharedDocumentManageEnvelopes);
            app.MapPost("/api/v1/DocumentPackage/GetSharedEnvelopeDetailsByEDisplayCode", GetSharedEnvelopeDetailsByEDisplayCode);
            app.MapPost("/api/v1/DocumentPackage/SendStaticConfirmationMail", SendStaticConfirmationMail);
            app.MapPost("/api/v1/DocumentPackage/SendmailReminder", SendmailReminder);
            app.MapPost("/api/v1/DocumentPackage/GetUserEnvelopeGridPreferences", GetUserEnvelopeGridPreferences);
            app.MapPost("/api/v1/DocumentPackage/GenerateDocument", GenerateDocument);
            app.MapPost("/api/v1/DocumentPackage/ResendExpiredEnvelope", ResendExpiredEnvelope);
            app.MapPost("/api/v1/DocumentPackage/DownloadPDF", DownloadPDF);
            app.MapPost("/api/v1/DocumentPackage/DeleteFinalContract", DeleteFinalContract);
            app.MapPost("/api/v1/DocumentPackage/CancelUpdateEnvelope", CancelUpdateEnvelope);
            app.MapPost("/api/v1/DocumentPackage/RefreshPDF", RefreshPDF);
            app.MapPost("/api/v1/DocumentPackage/SendFinalDocument", SendFinalDocument);
            app.MapPost("/api/v1/DocumentPackage/DownloadFileReview", DownloadFileReview);
            app.MapPost("/api/v1/DocumentPackage/DownloadEnvelopeSettings", DownloadEnvelopeSettings);          
            app.MapPost("/api/v1/DocumentPackage/IsCompanyUserExists", IsCompanyUserExists);
            app.MapPost("/api/v1/DocumentPackage/DownloadCurrentStatus", DownloadCurrentStatus);        
            app.MapPost("/api/V1/Envelope/DownloadZipFile", DownloadZipFile);
            app.MapPost("/api/v1/DocumentPackage/LoadMetaDataWithHistoryDocuments", LoadMetaDataWithHistoryDocuments);
        }
        public async Task<IResult> DocumentPackageIndex(HttpRequest request, FilterEnvelopeListforApi filter)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DocumentPackageIndex", "Process started for DocumentPackageIndex for user id:" + filter.UserID, "", "", filter.UserID.ToString(), remoteIpAddress, "Send API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            string EntityTypeId = string.Empty, IntegrationType = string.Empty, EntityType = string.Empty, emailToConsider = string.Empty;
            Guid? companyIDToConsider = null;

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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;

                        // Commented on 06-02-2025 to get CompanyId from client side to avoid DB call
                        //UserProfile userProfile = _userRepository.GetUserProfileByUserID(UserId);
                        //CompanyId = userProfile.CompanyID.ToString();
                        List<ManageEnvelopeAPI> manageEnvelopeList = new List<ManageEnvelopeAPI>();

                        string URL = _accessor.HttpContext.Request.GetDisplayUrl();
                        string decryptUserID;
                        string decryptCompanyID = string.Empty;

                        if (filter.UserID.ToString() != "undefined" && filter.UserID != null && filter.UserID.ToString() != "")
                        {
                            string userHashID = _accessor.HttpContext.Request.Query["UserID"];
                            userHashID = userHashID.Replace(" ", "+");
                            decryptUserID = EncryptDecryptQueryString.Decrypt(userHashID, Convert.ToString(_appConfiguration["AppKey"]));
                        }
                        else
                        {
                            decryptUserID = null;
                        }

                        if (filter.CompanyID != Guid.Empty && filter.UserID != null)
                        {
                            string companyHashID = _accessor.HttpContext.Request.Query["CompanyId"];
                            if (!string.IsNullOrEmpty(companyHashID))
                            {
                                companyHashID = companyHashID.Replace(" ", "+");
                                decryptCompanyID = EncryptDecryptQueryString.Decrypt(companyHashID, Convert.ToString(_appConfiguration["AppKey"]));
                            }
                        }
                        else
                        {
                            decryptCompanyID = null;
                        }
                        if (filter.CompanyID != Guid.Empty)
                        {
                            UserProfile userProfile = _userRepository.GetUserProfileByUserID(UserId);
                            filter.CompanyID = userProfile.CompanyID;
                        }

                        #region requests coming from stats tab
                        if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.Query["IsFromStats"]))
                        {
                            string isFromStats = _accessor.HttpContext.Request.Query["IsFromStats"];
                            if (isFromStats == "true")
                            {
                                if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.Query["UserID"]))
                                {
                                    string userHashID = _accessor.HttpContext.Request.Query["UserID"];
                                    if (!string.IsNullOrEmpty(userHashID) && userHashID != "null" && userHashID != "undefined")
                                    {
                                        userHashID = userHashID.Replace(" ", "+");
                                        decryptUserID = EncryptDecryptQueryString.Decrypt(userHashID, Convert.ToString(_appConfiguration["AppKey"]));
                                        filter.UserID = new Guid(decryptUserID);
                                    }
                                }

                                if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.Query["CompanyId"]))
                                {
                                    string companyHashID = _accessor.HttpContext.Request.Query["CompanyId"];
                                    if (!string.IsNullOrEmpty(companyHashID) && companyHashID != "null" && companyHashID != "undefined")
                                    {
                                        companyHashID = companyHashID.Replace(" ", "+");
                                        decryptCompanyID = EncryptDecryptQueryString.Decrypt(companyHashID, Convert.ToString(_appConfiguration["AppKey"]));
                                        filter.CompanyID = new Guid(decryptCompanyID);
                                    }
                                }
                            }
                        }
                        #endregion requests coming from stats tab

                        DateTime? fromDate = new DateTime(2000, 01, 01);
                        DateTime? toDate = DateTime.Now;
                        string userName = string.Empty;
                        if (filter != null)
                        {
                            if (filter.StartFromDate != null)
                            {
                                if (filter.EndtoDate != null && filter.EndtoDate != DateTime.MinValue && filter.StartFromDate > filter.EndtoDate)
                                {
                                    string errorMessage = "Please provide valid Start Date and End Date.";
                                    ResponseMessage responseMessage = new ResponseMessage();
                                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                    responseMessage.StatusMessage = "BadRequest";
                                    responseMessage.Message = errorMessage;
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return Results.BadRequest(responseMessage);
                                }
                                fromDate = filter.StartFromDate;
                            }
                            if (filter.EndtoDate != null)
                            {
                                if (filter.EndtoDate == DateTime.MinValue)
                                {
                                    string errorMessageEndDate = "End Date provided is in invalid format, it should be in mm/dd/yyyy.";
                                    ResponseMessage responseMessage = new ResponseMessage();
                                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                    responseMessage.StatusMessage = "BadRequest";
                                    responseMessage.Message = errorMessageEndDate;
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return Results.BadRequest(responseMessage);
                                }
                            }
                            if (filter.EnvelopeStatus != null && (filter.EnvelopeStatus < 1 || filter.EnvelopeStatus > 8))
                            {
                                string errorMessageEnvelopeStatus = "Envelope status must be between 1 to 7.";
                                ResponseMessage responseMessage = new ResponseMessage();
                                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                responseMessage.StatusMessage = "BadRequest";
                                responseMessage.Message = errorMessageEnvelopeStatus;
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                return Results.BadRequest(responseMessage);
                            }
                            else if (filter.EnvelopeStatus == null)
                                filter.EnvelopeStatus = 1;
                        }
                        UserEnvelopeGridPreferencesModal userEnvelopeGridPreferencesModal = new UserEnvelopeGridPreferencesModal(); // _documentPackageRepository.GetUserEnvelopeGridPreferences(userToken.UserID.ToString());
                        //if (string.IsNullOrEmpty(filter.SortBy))
                        //{
                        //    if (userEnvelopeGridPreferencesModal != null && userEnvelopeGridPreferencesModal.SelectedColumns != null && userEnvelopeGridPreferencesModal.SelectedColumns.Count > 0)
                        //    {
                        //        if (userEnvelopeGridPreferencesModal.SelectedColumns.Select(c => c.ColumnCode).Contains("Sent"))
                        //        {
                        //            filter.SortBy = "Sent desc";
                        //        }
                        //        else
                        //        {
                        //            filter.SortBy = "Subject desc";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        filter.SortBy = "Sent desc";
                        //    }
                        //}
                        if (Convert.ToInt32(filter.Page) <= 0 || !string.IsNullOrEmpty(filter.EDisplayCode)) filter.Page = 1;
                        if (filter.EnvelopeStatus == 2) filter.Recipient = userToken.EmailId;
                        int TotalEnvelopeCount = 0;
                        //if (filter.IsExportToExcel)
                        //{
                        //    filter.IsRecipientRequired = 1;
                        //    filter.IsSenderRequired = 1;
                        //    filter.IsLastUpdatedRequired = 1;
                        //}
                        //else
                        //{
                        //    if (userEnvelopeGridPreferencesModal != null && userEnvelopeGridPreferencesModal.SelectedColumns != null && userEnvelopeGridPreferencesModal.SelectedColumns.Select(c => c.ColumnCode).Contains("Recipient"))
                        //        filter.IsRecipientRequired = 1;
                        //    else filter.IsRecipientRequired = 0;

                        //    if (userEnvelopeGridPreferencesModal != null && userEnvelopeGridPreferencesModal.SelectedColumns != null && userEnvelopeGridPreferencesModal.SelectedColumns.Select(c => c.ColumnCode).Contains("Sender"))
                        //        filter.IsSenderRequired = 1;
                        //    else filter.IsSenderRequired = 0;

                        //    if (userEnvelopeGridPreferencesModal != null && userEnvelopeGridPreferencesModal.SelectedColumns != null && userEnvelopeGridPreferencesModal.SelectedColumns.Select(c => c.ColumnCode).Contains("LastUpdated"))
                        //        filter.IsLastUpdatedRequired = 1;
                        //    else
                        //        filter.IsLastUpdatedRequired = 0;
                        //}

                        filter.SearchType = Convert.ToInt32(filter.SearchType) < 1 ? Constants.ShowEnvelopeDataThirtyDays.ThirtyDay : Convert.ToInt32(filter.SearchType);

                        manageEnvelopeList = _documentPackageRepository.GetEnvelopeData(UserId, filter, remoteIpAddress, userEnvelopeGridPreferencesModal);
                        return Results.Ok(manageEnvelopeList);
                    }

                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using DocumentPackageIndex method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetEnvelopesBySubmit(HttpRequest request, FilterEnvelopeListforApi searchObj)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopesBySubmit", "Process started for GetEnvelopesBySubmit for user id:" + searchObj.UserID, remoteIpAddress, "Send API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            string EntityTypeId = string.Empty, IntegrationType = string.Empty, EntityType = string.Empty;

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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;

                        UserProfile userProfile = _userRepository.GetUserProfileByUserID(UserId);
                        //searchObj.CompanyID = userProfile.CompanyID;
                        List<ManageEnvelopeAPI> manageEnvelopeList = new List<ManageEnvelopeAPI>();

                        string URL = _accessor.HttpContext.Request.GetDisplayUrl();
                        URL = HttpUtility.UrlDecode(URL.Substring(URL.IndexOf('?') + 1));
                        if (URL.ToLower().Contains("entitytype"))
                        {
                            string[] SplitIntoArray = URL.Split('&');
                            foreach (var item in SplitIntoArray)
                            {
                                var itemValue = item.Split('=')[0];
                                switch (itemValue.ToLower())
                                {
                                    case "entityid":
                                        EntityTypeId = item.Split('=')[1];
                                        break;
                                }
                            }
                            IntegrationType = RSign.Common.Helpers.Constants.UploadDriveType.Bullhorn;
                        }
                        if (URL.ToLower().Contains("tenant"))
                        {
                            string[] SplitIntoArray = URL.Split('&');
                            foreach (var item in SplitIntoArray)
                            {
                                var itemValue = item.Split('=')[0];
                                switch (itemValue.ToLower())
                                {
                                    case "candidateid":
                                        EntityTypeId = item.Split('=')[1];
                                        EntityType = "Candidate";
                                        break;
                                    case "contactid":
                                        EntityTypeId = item.Split('=')[1];
                                        EntityType = "Contact";
                                        break;
                                    case "jobid":
                                        EntityTypeId = item.Split('=')[1];
                                        EntityType = "Job";
                                        break;
                                    case "companyid":
                                        EntityTypeId = item.Split('=')[1];
                                        EntityType = "Company";
                                        break;
                                }
                            }

                            IntegrationType = RSign.Common.Helpers.Constants.UploadDriveType.Vincere;
                        }

                        manageEnvelopeList = await _documentPackageRepository.GetManageEnvelopeData(userProfile, searchObj, EntityTypeId, IntegrationType);

                        #region Export and return
                        string count = manageEnvelopeList == null ? "0" : manageEnvelopeList.Count.ToString();
                        if (searchObj.IsExportToExcel)
                        {
                            if (manageEnvelopeList != null && manageEnvelopeList.Count > 0)
                                manageEnvelopeList = _documentPackageRepository.GetEnvelopesExportToExcelData(manageEnvelopeList, userProfile, searchObj.LoginUserTimeZone);

                        }
                        else
                        {
                            if (manageEnvelopeList != null && manageEnvelopeList.Count > 0)
                            {
                                manageEnvelopeList = _documentPackageRepository.GetEnvelopeUpdatedData(manageEnvelopeList, userProfile.LanguageCode, UserId, Convert.ToString(userProfile.CompanyID), searchObj.LoginUserTimeZone);

                            }
                        }
                        #endregion Export and return    
                        return Results.Ok(manageEnvelopeList);
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using GetEnvelopesBySubmit method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<IResult> LoadMetaDataWithHistory(HttpRequest request, LoadMetaDataHistory loadMetaDataHistory)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadMetaDataWithHistory", "Process started for LoadMetaDataWithHistory for Envelope ID:" + loadMetaDataHistory.envelopeId, "", "", "", "Send API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            EnvelopeGetEnvelopeHistoryByEnvelopeCode envelopeMetadata = null;

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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;

                        envelopeMetadata = _envelopeRepository.GetEnvelopeMetaDataWithHistory(new Guid(loadMetaDataHistory.envelopeId), userToken.EmailId, "", "", loadMetaDataHistory.envelopeArichiveData, loadMetaDataHistory.getDataType, loadMetaDataHistory.envelopeCode);
                        return Results.Ok(envelopeMetadata);
                    }

                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using LoadMetaDataWithHistory method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetEnvelopeSettingsDetails(HttpRequest request, string envelopeId, string languageCode, int isEnvelopeArichived = 0)
        {
            CustomEnvelopeSettingsDetails responseMessage = new CustomEnvelopeSettingsDetails();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopeSettingsDetails", "Process started for Get Envelope Settings Details for envelopeId:" + envelopeId, envelopeId, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }

                    responseMessage = await _envelopeRepository.GetEnvelopeSettings(new Guid(envelopeId), languageCode, isEnvelopeArichived);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using DocumentPackageIndex method and error message is:\" + ex.ToString()";
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.EnvelopeViewSettingDetails = null;
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "Error";
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> GetRecipientsForResendEnvelope(HttpRequest request, Guid EnvelopeID)
        {
            ResponseMessageForRecipientListForResend responseMessage = new ResponseMessageForRecipientListForResend();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "GetRecipientsForResendEnvelope", "Process started for Get GetRecipientsForResendEnvelope for envelopeId:" + EnvelopeID, EnvelopeID.ToString(), "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }
                    int currentSigningOrder = 0;
                    responseMessage = _documentPackageRepository.GetRecipientsForResendEnvelope(EnvelopeID, out currentSigningOrder);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using GetRecipientsForResendEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> CancelEnvelope(HttpRequest request, Guid EnvelopeID)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "CancelEnvelope", "Process started for CancelEnvelope for envelopeId:" + EnvelopeID, EnvelopeID.ToString(), "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }
                    responseMessage = _documentPackageRepository.CancelEnvelopeTrans(EnvelopeID);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using CancelEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> CopyEnvelope(HttpRequest request, string envelopeID, int IsEnvelopeArichived = 0)
        {
            ResponseMessageForCopyEnvelope responseMessage = new ResponseMessageForCopyEnvelope();
            try
            {
                var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "CopyEnvelope", "Process started for CopyEnvelope for envelopeId:" + envelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken!);
                    //UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    Guid envID = Guid.Empty;
                    if (string.IsNullOrEmpty(envelopeID))
                    {
                        string userURL = _accessor.HttpContext.Request.GetDisplayUrl();
                        userURL = HttpUtility.UrlDecode(userURL.Substring(userURL.IndexOf('?') + 1));
                        userURL = userURL.Replace(" ", "+");
                        string globalEnvelopeID = EncryptDecryptQueryString.Decrypt(userURL, Convert.ToString(_appConfiguration["AppKey"]));
                        loggerModelNew.EnvelopeId = globalEnvelopeID.ToString();
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        envID = new Guid(globalEnvelopeID);
                    }
                    else if (!string.IsNullOrEmpty(envelopeID))
                    {
                        envID = new Guid(envelopeID);
                    }
                    APICopyEnvelope apiCopyEnvelope = new APICopyEnvelope();
                    apiCopyEnvelope.envelopeID = envID;
                    apiCopyEnvelope.EnvelopeCode = string.Empty;
                    apiCopyEnvelope.IsEnvelopeArichived = IsEnvelopeArichived;
                    apiCopyEnvelope.IsCopiedFromWeb = true;
                    if (userToken == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }

                    string domainUrlType = "2";
                    request.Headers.TryGetValue("Source", out Microsoft.Extensions.Primitives.StringValues requestSource);
                    domainUrlType = EnvelopeHelper.GenerateDomainRedirectURL(requestSource);

                    responseMessage = _documentPackageRepository.CopyEnvelope(apiCopyEnvelope, userToken.UserID, userToken.EmailId, domainUrlType);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using CopyEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> RemoveUploadedDocument(HttpRequest request, string envelopeId, string documentId, string type, int IsEnvelopeArichived = 0)
        {
            ResponseMessageForDeleteEnvelope responseMessage = new ResponseMessageForDeleteEnvelope();
            try
            {
                var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "DeleteUploadedDocument", "Process started for Delete Uploaded Document for envelopeId:" + envelopeId, envelopeId, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken!);
                    //UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    if (userToken == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    responseMessage = _documentPackageRepository.DeleteUploadedDocument(userToken.UserID, envelopeId, documentId, type, IsEnvelopeArichived, userToken.EmailId);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using RemoveUploadedDocument method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> RemoveEmailBodyForEnvelope(HttpRequest request, string envelopeID, string documentId, int IsEnvelopeArichived = 0)
        {
            ResponseMessageForEnvelope responseMessage = new ResponseMessageForEnvelope();
            try
            {
                var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "RemoveEmailBodyForEnvelope", "Process started for Remove EmailBody For Envelope for envelopeId:" + envelopeID, envelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }
                    responseMessage = _documentPackageRepository.UpdateEnvelopeEmailBodyFlag(envelopeID, documentId, IsEnvelopeArichived, userToken.EmailId);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using RemoveEmailBodyForEnvelope method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> ViewDocument(HttpRequest request, Guid EnvelopeId, Guid DocumentId, int IsEnvelopeArichived = 0, int getDatatype = 0)
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            string BrowserName = request.Headers["User-Agent"];
            try
            {
                loggerModelNew = new LoggerModelNew("", _module, "ViewDocument", "Process started for View Document for envelopeId:" + EnvelopeId, "", "", "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }
                    responseMessage = await _documentPackageRepository.DownloadDocument(EnvelopeId, DocumentId, IsEnvelopeArichived, userToken?.EmailId, remoteIpAddress, BrowserName);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using ViewDocument method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> DownloadEnvelopeDataXML(HttpRequest request, Guid EnvelopeId, int IsEnvelopeArichived = 0, int getDatatype = 0)
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            string BrowserName = request.Headers["User-Agent"];
            try
            {
                loggerModelNew = new LoggerModelNew("", _module, "DocumentPackageEndPoint", "Process started for Download EnvelopeData XML for envelopeId:" + EnvelopeId, "", "", "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken!);
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    if (userToken == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    responseMessage = _documentPackageRepository.DownloadEnvelopeDataXML(userProfile, userToken.UserID, EnvelopeId, IsEnvelopeArichived, userToken.EmailId, remoteIpAddress, BrowserName);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using DownloadEnvelopeDataXML method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> DownloadSignatureCertificate(HttpRequest request, string envelopeId, string envelopeCode)
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            string BrowserName = request.Headers["User-Agent"];
            try
            {
                loggerModelNew = new LoggerModelNew("", _module, "DocumentPackageEndPoint", "Process started for Download Signature Certificate for envelopeId:" + envelopeId, "", "", remoteIpAddress, "DownloadSignatureCertificate");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }
                    responseMessage = await _documentPackageRepository.DownloadSignatureCertificate(userToken.UserID, new Guid(envelopeId), envelopeCode, userToken.EmailId, remoteIpAddress, BrowserName);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using DownloadSignatureCertificate method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> DeleteSignatureCertificate(HttpRequest request, string envelopeId, string documentId, int IsEnvelopeArichived = 0)
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            try
            {
                loggerModelNew = new LoggerModelNew("", _module, "DeleteSignatureCertificate", "Process started for Delete Signature Certificate for envelopeId:" + envelopeId, "", "", "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                string BrowserName = request.Headers["User-Agent"];
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserToken userToken = _userTokenRepository.GetUserTokenByToken(authToken!);
                    //UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    if (userToken == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    responseMessage = _documentPackageRepository.DeleteSignatureCertificate(envelopeId, documentId, IsEnvelopeArichived, userToken.EmailId, remoteIpAddress = "", BrowserName = "");
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using DeleteSignatureCertificate method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> DownloadTransparency(HttpRequest request, Guid id, int IsEnvelopeArichived = 0, int getDatatype = 0)
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            string BrowserName = request.Headers["User-Agent"];
            try
            {
                loggerModelNew = new LoggerModelNew("", _module, "DownloadTransparency", "Process started for Download Transparency Certificate for envelopeId:" + id, "", "", "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }
                    responseMessage = await _documentPackageRepository.DownloadTransparency(userToken.UserID, id, IsEnvelopeArichived, getDatatype, userToken.EmailId, remoteIpAddress, BrowserName);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using DownloadTransparency method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> GetFinalSignEnvelopeRecipients(HttpRequest request, Guid envelopeId)
        {
            ResponseMessageEnvelopeRecipients responseMessage = new ResponseMessageEnvelopeRecipients();
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            string BrowserName = request.Headers["User-Agent"];
            try
            {
                loggerModelNew = new LoggerModelNew("", _module, "GetFinalSignEnvelopeRecipients", "Process started for Get Final Sign Envelope Recipients for envelopeId:" + envelopeId, "", "", "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }
                    responseMessage = _documentPackageRepository.GetFinalSignEnvelopeRecipients(envelopeId, userToken.UserID, userToken.EmailId, remoteIpAddress, BrowserName);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using GetFinalSignEnvelopeRecipients method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> GetHiddenControlValuesByEnvelopeId(HttpRequest request, string envelopeId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetHiddenControlValuesByEnvelopeId", "Process started for Get Hidden Control Values By EnvelopeId for envelopeId:" + envelopeId, "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessageGetHiddenControl envelopeControldata = null;

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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;

                        envelopeControldata = _documentPackageRepository.GetHiddenControlValuesByEnvelopeId(envelopeId);
                        return Results.Ok(envelopeControldata);
                    }

                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using LoadMetaDataWithHistory method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// GetSharedDocumentManageEnvelopes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IResult> GetSharedDocumentManageEnvelopes(HttpRequest request, FilterSharedEnvelopeListforApi filter)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopesBySubmit", "Process started for Get Shared Document Manage Envelopes", "", "", "", remoteIpAddress, "Send API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            string EntityTypeId = string.Empty, IntegrationType = string.Empty, EntityType = string.Empty;

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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;

                        DateTime? fromDate = new DateTime(2000, 01, 01);
                        DateTime? toDate = DateTime.Now;
                        string userName = string.Empty;
                        if (filter != null)
                        {
                            if (filter.StartFromDate != null)
                            {
                                if (filter.EndtoDate != null && filter.EndtoDate != DateTime.MinValue && filter.StartFromDate > filter.EndtoDate)
                                {
                                    string errorMessage = "Please provide valid Start Date and End Date.";
                                    ResponseMessage responseMessage = new ResponseMessage();
                                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                    responseMessage.StatusMessage = "BadRequest";
                                    responseMessage.Message = errorMessage;
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return Results.BadRequest(responseMessage);
                                }
                                fromDate = filter.StartFromDate;
                            }
                            if (filter.EndtoDate != null)
                            {
                                if (filter.EndtoDate == DateTime.MinValue)
                                {
                                    string errorMessageEndDate = "End Date provided is in invalid format, it should be in mm/dd/yyyy.";
                                    ResponseMessage responseMessage = new ResponseMessage();
                                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                    responseMessage.StatusMessage = "BadRequest";
                                    responseMessage.Message = errorMessageEndDate;
                                    loggerModelNew.Message = responseMessage.Message;
                                    rSignLogger.RSignLogWarn(loggerModelNew);
                                    return Results.BadRequest(responseMessage);
                                }

                                filter.EndtoDate = filter.EndtoDate.Value == DateTime.MaxValue ? filter.EndtoDate : filter.EndtoDate.Value.AddDays(1);
                            }
                            if (filter.EnvelopeStatus != null && (filter.EnvelopeStatus < 1 || filter.EnvelopeStatus > 8))
                            {
                                string errorMessageEnvelopeStatus = "Envelope status must be between 1 to 7.";
                                ResponseMessage responseMessage = new ResponseMessage();
                                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                                responseMessage.StatusMessage = "BadRequest";
                                responseMessage.Message = errorMessageEnvelopeStatus;
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                return Results.BadRequest(responseMessage);
                            }
                            else if (filter.EnvelopeStatus == null)
                                filter.EnvelopeStatus = 1;
                        }
                        UserEnvelopeGridPreferencesModal userEnvelopeGridPreferencesModal = new UserEnvelopeGridPreferencesModal(); // _documentPackageRepository.GetUserEnvelopeGridPreferences(userToken.UserID.ToString());
                        //if (string.IsNullOrEmpty(filter.SortBy))
                        //{
                        //    if (userEnvelopeGridPreferencesModal != null && userEnvelopeGridPreferencesModal.SelectedColumns != null && userEnvelopeGridPreferencesModal.SelectedColumns.Count > 0)
                        //    {
                        //        if (userEnvelopeGridPreferencesModal.SelectedColumns.Select(c => c.ColumnCode).Contains("Sent"))
                        //            filter.SortBy = "Sent desc";
                        //        else filter.SortBy = "Subject desc";
                        //    }
                        //    else filter.SortBy = "Sent desc";
                        //}
                        if (Convert.ToInt32(filter.Page) <= 0 || !string.IsNullOrEmpty(filter.EDisplayCode)) filter.Page = 1;
                        if (filter.EnvelopeStatus == 2) filter.Recipient = userToken.EmailId;
                        int TotalEnvelopeCount = 0;
                        //if (filter.IsExportToExcel)
                        //{
                        //    filter.IsRecipientRequired = 1;
                        //    filter.IsSenderRequired = 1;
                        //    filter.IsLastUpdatedRequired = 1;
                        //}
                        //else
                        //{
                        //    if (userEnvelopeGridPreferencesModal != null && userEnvelopeGridPreferencesModal.SelectedColumns != null && userEnvelopeGridPreferencesModal.SelectedColumns.Select(c => c.ColumnCode).Contains("Recipient"))
                        //        filter.IsRecipientRequired = 1;
                        //    else filter.IsRecipientRequired = 0;

                        //    if (userEnvelopeGridPreferencesModal != null && userEnvelopeGridPreferencesModal.SelectedColumns != null && userEnvelopeGridPreferencesModal.SelectedColumns.Select(c => c.ColumnCode).Contains("Sender"))
                        //        filter.IsSenderRequired = 1;
                        //    else filter.IsSenderRequired = 0;

                        //    if (userEnvelopeGridPreferencesModal != null && userEnvelopeGridPreferencesModal.SelectedColumns != null && userEnvelopeGridPreferencesModal.SelectedColumns.Select(c => c.ColumnCode).Contains("LastUpdated"))
                        //        filter.IsLastUpdatedRequired = 1;
                        //    else
                        //        filter.IsLastUpdatedRequired = 0;
                        //}
                        var manageEnvelopeList = new List<ManageEnvelopeAPI>();
                        filter.SearchType = Convert.ToInt32(filter.SearchType) < 1 ? Constants.ShowEnvelopeDataThirtyDays.ThirtyDay : Convert.ToInt32(filter.SearchType);
                        manageEnvelopeList = _documentPackageRepository.GetSharedAccessEnvelopesData(userToken.UserID, out TotalEnvelopeCount, filter, userEnvelopeGridPreferencesModal);

                        if (manageEnvelopeList.Count == 0)
                        {
                            string errorMessageForNoContent = Convert.ToString(_appConfiguration["NoContent"].ToString());
                            ResponseMessage responseMessage = new ResponseMessage();
                            responseMessage.StatusCode = HttpStatusCode.NoContent;
                            responseMessage.StatusMessage = "NoContent";
                            responseMessage.Message = errorMessageForNoContent;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return Results.Ok(responseMessage);
                        }
                        else
                        {
                            ResponseMessageWithManageEnvelopesApi responseMessageSuccess = new ResponseMessageWithManageEnvelopesApi();
                            responseMessageSuccess.StatusCode = HttpStatusCode.OK;
                            responseMessageSuccess.StatusMessage = "OK";
                            responseMessageSuccess.FromDate = manageEnvelopeList.OrderBy(e => e.CreatedDateTime).FirstOrDefault().CreatedDateTime;
                            responseMessageSuccess.ToDate = manageEnvelopeList.OrderByDescending(e => e.CreatedDateTime).FirstOrDefault().CreatedDateTime;
                            responseMessageSuccess.Envelopes = manageEnvelopeList;
                            responseMessageSuccess.TotalEnvelopeCount = TotalEnvelopeCount;
                            loggerModelNew.Message = "Successfully retrieved " + TotalEnvelopeCount + " envelopes";
                            rSignLogger.RSignLogInfo(loggerModelNew);

                            #region Export and return
                            string count = manageEnvelopeList == null ? "0" : manageEnvelopeList.Count.ToString();
                            string LanguageCode = filter.LanguageCode;
                            if (filter.IsExportToExcel)
                            {
                                if (manageEnvelopeList != null && manageEnvelopeList.Count > 0)
                                {
                                    manageEnvelopeList = _documentPackageRepository.GetSharedExportToExcelData(manageEnvelopeList, LanguageCode, UserId, filter.LoginUserCompanyId, filter.LoginUserTimeZone);
                                    if (manageEnvelopeList.Count > 0)
                                    {
                                        responseMessageSuccess.Envelopes = manageEnvelopeList;
                                        responseMessageSuccess.FileName = "RSignEnvelopes_" + DateTime.Now.ToString("yyyyMMdd");
                                        responseMessageSuccess.Message = "Successfully exported data " + count + " envelopes";
                                        loggerModelNew.Message = "Successfully retrieved " + TotalEnvelopeCount + " envelopes";
                                        rSignLogger.RSignLogInfo(loggerModelNew);
                                    }
                                    else responseMessageSuccess.Message = "No envelopes found.";
                                }
                            }
                            else
                            {
                                if (manageEnvelopeList != null && manageEnvelopeList.Count > 0)
                                {
                                    manageEnvelopeList = _documentPackageRepository.GetEnvelopeUpdatedData(manageEnvelopeList, LanguageCode, UserId, filter.LoginUserCompanyId, filter.LoginUserTimeZone);
                                    responseMessageSuccess.Envelopes = manageEnvelopeList;
                                }
                            }
                            #endregion Export and return                             
                            return Results.Ok(responseMessageSuccess);
                        }
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using GetSharedDocumentManageEnvelopes method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        public async Task<IResult> GetSharedEnvelopeDetailsByEDisplayCode(HttpRequest request, SharedEnvelopeCodeExistsModel sharedEnvelopeCodeExistsModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetSharedEnvelopeDetailsByEDisplayCode", "Process started for Get Envelope details by envelope code using API.", sharedEnvelopeCodeExistsModel.EDisplayCode, "", "", remoteIpAddress, "SendAPI");
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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;
                        return Results.Ok(_documentPackageRepository.GetSharedEnvelopeDetailsByEDisplayCode(sharedEnvelopeCodeExistsModel));
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using LoadMetaDataWithHistory method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> SendStaticConfirmationMail(HttpRequest request, Guid envelopeId)
        {
            ResponseMessageForResendStaticConfirmationEmail responseMessage = new ResponseMessageForResendStaticConfirmationEmail();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "SendStaticConfirmationMail", "Process started for SendStaticConfirmationMail envelopeId:" + envelopeId, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "SendStaticConfirmationMail";
                    rSignAPIPayload.PayloadTypeId = envelopeId.ToString();
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = envelopeId.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    responseMessage = _documentPackageRepository.SendStaticConfirmationMail(envelopeId, userToken.UserID);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using SendStaticConfirmationMail method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                //responseMessage.EnvelopeViewSettingDetails = null;
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.StatusMessage = "Error";
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> SendmailReminder(HttpRequest request, List<RecipientDetails> lstRecipientDetails)
        {
            ResponseMessageResend responseMessage = new ResponseMessageResend();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "SendmailReminder", "Process started for Send mail Reminder envelopeId:" + lstRecipientDetails[0].EnvelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);

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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "SendmailReminder";
                    rSignAPIPayload.PayloadTypeId = lstRecipientDetails[0].EnvelopeID.ToString();
                    rSignAPIPayload.UserEmail = "";
                    rSignAPIPayload.PayloadInfo = lstRecipientDetails[0].EnvelopeID.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    responseMessage = _documentPackageRepository.ResendEmail(lstRecipientDetails, userToken.UserID, userToken.EmailId, remoteIpAddress);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using SendmailReminder method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                //responseMessage.StatusMessage = "Error";
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> GetUserEnvelopeGridPreferences(HttpRequest request)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            try
            {
                UserEnvelopeGridPreferencesModal userEnvelopeGridPreferencesModal = new UserEnvelopeGridPreferencesModal();
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "GetUserEnvelopeGridPreferences", "Process started for Get User Envelope GridPreferences", "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }
                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "GenerateDocument";
                    rSignAPIPayload.PayloadTypeId = "Envelope";
                    rSignAPIPayload.UserEmail = userToken.EmailId;
                    rSignAPIPayload.PayloadInfo = "GetUserEnvelopeGridPreferences";
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);
                    userEnvelopeGridPreferencesModal = _documentPackageRepository.GetUserEnvelopeGridPreferences(userToken.UserID.ToString());
                    return Results.Ok(userEnvelopeGridPreferencesModal);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using SendmailReminder method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> GenerateDocument(HttpRequest request, Guid envelopeId)
        {
            ResponseMessageEnvelopeRecipients responseMessage = new ResponseMessageEnvelopeRecipients();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "GenerateDocument", "Process started for Generate Document envelopeId:" + envelopeId, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "GenerateDocument";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = userToken.EmailId;
                    rSignAPIPayload.PayloadInfo = envelopeId.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);
                    responseMessage = await _documentPackageRepository.GenerateDocument(envelopeId, userToken.UserID, userToken.EmailId);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using SendmailReminder method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> ResendExpiredEnvelope(HttpRequest request, Guid envelopeID, string isActingUserPerformingAction = "false")
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "ResendExpiredEnvelope", "Process started for Resend Expired Envelope for envelopeId:" + envelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "ResendExpiredEnvelope";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = userToken.EmailId;
                    rSignAPIPayload.PayloadInfo = envelopeID.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);
                    //responseMessage = await _documentPackageRepository.GenerateDocument(envelopeID, userToken.UserID, userToken.EmailId);
                    responseMessage = _documentPackageRepository.ResendExpiredEnvelope(envelopeID, remoteIpAddress, isActingUserPerformingAction);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using ResendExpiredEnvelope method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> DownloadPDF(HttpRequest request, string envelopeID, int IsEnvelopeArichived = 0)
        {
            DownloadPDFResponse responseMessage = new();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "DownloadPDF", "Process started for Download PDF for envelopeId:" + envelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "DownloadPDF";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = userToken.EmailId;
                    rSignAPIPayload.PayloadInfo = envelopeID.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    responseMessage= await _documentPackageRepository.GetDownloadedTerminatedAndIncompleteExipredDocument(envelopeID, userToken.UserID, userToken.EmailId, IsEnvelopeArichived);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using DownloadPDF method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> DeleteFinalContract(HttpRequest request, string envelopeID, int IsEnvelopeArichived = 0)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "DeleteFinalContract", "Process started for Delete Final Contract for envelopeId:" + envelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "DeleteFinalContract";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = userToken.EmailId;
                    rSignAPIPayload.PayloadInfo = envelopeID.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);


                    responseMessage = _documentPackageRepository.DeleteFinalContract(envelopeID, userToken.UserID, userToken.EmailId, IsEnvelopeArichived);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using DeleteFinalContract method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> CancelUpdateEnvelope(HttpRequest request, string envelopeID, bool IsActingUserPerformingAction, bool IsEdited)
        {
            ResponseMessageGetEnvelopeDetails responseMessage = new ResponseMessageGetEnvelopeDetails();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "CancelUpdateEnvelope", "Process started for Cancel Update Envelope for envelopeId:" + envelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "CancelUpdateEnvelope";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = userToken.EmailId;
                    rSignAPIPayload.PayloadInfo = envelopeID.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);


                    //responseMessage = _documentPackageRepository.DeleteFinalContract(envelopeID, userToken.UserID, userToken.EmailId, IsEnvelopeArichived);
                    responseMessage = _documentPackageRepository.UpdateEnvelopeEditedStatus(envelopeID, userToken.UserID, userToken.EmailId, IsEdited, IsActingUserPerformingAction);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using DeleteFinalContract method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> RefreshPDF(HttpRequest request, Guid id, bool privateMode)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "RefreshPDF", "Process started for RefreshPDF Envelope for envelopeId:" + id, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "RefreshPDF";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = userToken.EmailId;
                    rSignAPIPayload.PayloadInfo = id.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);


                    responseMessage = _documentPackageRepository.DeleteAndRegenerateFinalContract(id, userToken.UserID, userToken.EmailId, remoteIpAddress);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using DeleteFinalContract method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> SendFinalDocument(HttpRequest request, ManageFinalDocument manageFinalDocument)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "SendFinalDocument", "Process started for Cancel Update Envelope for envelopeId:" + Convert.ToString(manageFinalDocument.EnvelopeId), "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    //RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    //rSignAPIPayload.PayloadType = "DocumentPackage";
                    //rSignAPIPayload.APIMethod = "SendFinalDocument";
                    //rSignAPIPayload.PayloadTypeId = "";
                    //rSignAPIPayload.UserEmail = userToken.EmailId;
                    //rSignAPIPayload.PayloadInfo = envelopeId.ToString();
                    //rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    //_envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);

                    responseMessage = await _documentPackageRepository.SendFinalDocument(manageFinalDocument, remoteIpAddress);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using DeleteFinalContract method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        public async Task<IResult> DownloadFileReview(HttpRequest request, Guid envelopeID, Guid recipientId, string Type = "")
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "DownloadFileReview", "Process started for Cancel Update Envelope for envelopeId:" + envelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    RSignAPIPayload rSignAPIPayload = new RSignAPIPayload();
                    rSignAPIPayload.PayloadType = "DocumentPackage";
                    rSignAPIPayload.APIMethod = "DownloadFileReview";
                    rSignAPIPayload.PayloadTypeId = "";
                    rSignAPIPayload.UserEmail = userToken.EmailId;
                    rSignAPIPayload.PayloadInfo = envelopeID.ToString();
                    rSignAPIPayload.CreatedDate = DateTime.UtcNow;
                    _envelopeRepository.InsertRSignAPIPayload(rSignAPIPayload);


                    responseMessage = await _documentPackageRepository.GetDownloadFileReview(envelopeID, recipientId, remoteIpAddress, Type);
                    return Results.Ok(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using DownloadFileReview method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> DownloadEnvelopeSettings(HttpRequest request, EnvelopeSettingRequest settingRequest)
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "DownloadEnvelopeSettings", "Process started for download envelope settings pdf for envelopeId:" + settingRequest.EnvelopeId.ToString(), "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }

                    responseMessage = _documentPackageRepository.DownloadEnvelopeSettings(settingRequest);
                    if (responseMessage.StatusCode == HttpStatusCode.OK) return Results.Ok(responseMessage);
                    else return Results.BadRequest(responseMessage);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using DownloadEnvelopeSettings method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseMessage);
            }
        }
        /// <summary>
        /// This method is used for get Envelope Code suggestions
        /// </summary>
        /// <param name="request"></param>
        /// <param name="term"></param>
        /// <param name="compid"></param>
        /// <returns></returns>
        public async Task<IResult> AutocompleteEnvelopeCode(HttpRequest request, string term, string compid)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "AutocompleteEnvelopeCode", "Process started for AutocompleteEnvelopeCode", "", "", "", "Send API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            ResponseMessage filteredItems = new();

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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;
                        List<Guid> usersId = new List<Guid>();
                        if (!string.IsNullOrEmpty(compid))
                        {
                            usersId = _userRepository.GetAllUsersOfCompany(new Guid(compid));
                        }
                        else
                        {
                            usersId.Add(userToken.UserID);
                        }
                        filteredItems = _documentPackageRepository.GetEnvelopeCodeForUsers(usersId, term);
                        return Results.Ok(filteredItems);
                    }

                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using LoadMetaDataWithHistory method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This method is used for check company user exists or not
        /// </summary>
        /// <param name="request"></param>
        /// <param name="companyUserExists"></param>
        /// <returns></returns>
        public async Task<IResult> IsCompanyUserExists(HttpRequest request, CompanyUserExists companyUserExists)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "IsCompanyUserExists", "Process started for IsCompanyUserExists", "", "", "", "Send API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessage response = new();
            string emailID;
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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;
                        emailID = userToken.EmailId ?? string.Empty;
                        List<Guid> usersId = new();
                        if (companyUserExists.UserTypeID == Constants.UserType.ADMIN)
                        {
                            response = await _documentPackageRepository.GetCompanyDetails(companyUserExists, emailID, userToken.UserID);
                        }
                        else
                        {
                            response.data = _companyRepository.GetCompanyByName(companyUserExists.CompanyName);
                            response.StatusCode = HttpStatusCode.OK;
                            response.StatusMessage = "Success";
                            response.Message = "Successfully reterived Company Data";
                            response.data = response.data;
                        }

                        return Results.Ok(response);
                    }

                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using LoadMetaDataWithHistory method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult DownloadCurrentStatus(HttpRequest request, Guid envelopeId, bool preview = false)
        {
            var responseMessage = new ResponseMessageFile();
            var loggerModelNew = new LoggerModelNew("", "DocumentPackageEndPoint", $"Process started for DownloadCurrentStatus for envelopeId: {envelopeId}", "", "", UserTokenRepository.GetIPAddress(request), "DownloadCurrentStatus");

            try
            {
                rSignLogger.RSignLogInfo(loggerModelNew);

                if (!request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrEmpty(authHeader))
                {
                    return Results.Unauthorized();
                }

                string? authToken = authHeader.FirstOrDefault()?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.Unauthorized();
                }

                var userToken = _userTokenRepository.GetUserTokenByToken(authToken);
                if (userToken == null)
                {
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                responseMessage = _documentPackageRepository.DownloadCurrentStatus(envelopeId, preview);
                return preview
                    ? Results.Json(new { Message = "File exists", Base64String = responseMessage.Base64FileData, FileName = responseMessage.FileName })
                    : Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = $"Error in DownloadCurrentStatus: {ex.Message}";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.StatusMessage = "Error";
                responseMessage.Message = "An unexpected error occurred.";
                return Results.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This method is used to get next user who are at waiting for signature
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeID"></param>
        /// <returns></returns>
        public async Task<IResult> GetEnvelopeRecipientsAudit(HttpRequest request, Guid envelopeID)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopeRecipientsAudit", "Endpoint Initialized,to Get Get Envelope Recipients Audit:" + envelopeID, envelopeID.ToString(), "", "", remoteIpAddress, "SendAPI");
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
                    ResponseMessageForRecipientAuditList responseMessage = new();
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting GetEnvelopeRecipientsAudit details by envelopeID id=" + envelopeID.ToString();
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    string listofRecipient = await _recipientRepository.GetRecipientsAudit(envelopeID);
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "Success";
                    responseMessage.NextRecipientList = listofRecipient;
                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetEnvelopeRecipientsAudit method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Signer attachments Zip file download
        /// </summary>
        /// <param name="request"></param>
        /// <param name="envelopeID"></param>
        /// <returns></returns>
        public async Task<IResult> DownloadZipFile(HttpRequest request, Guid envelopeID, int IsEnvelopeArichived = 0)
        {
            ResponseMessageFile responseSharedTemplate = new();
            string emailID;
            try
            {
                string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
                loggerModelNew = new LoggerModelNew("", _module, "DownloadZipFile", "Process started for Download PDF for envelopeId:" + envelopeID, "", "", remoteIpAddress, "Send API");
                rSignLogger.RSignLogInfo(loggerModelNew);
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
                        return Results.Unauthorized();
                    }
                    emailID = userToken.EmailId ?? string.Empty;
                    responseSharedTemplate = await _documentPackageRepository.DownloadSignerAttachment(envelopeID, userToken.UserID, emailID, remoteIpAddress, IsEnvelopeArichived);
                    return Results.Ok(responseSharedTemplate);
                }
                else return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while using DownloadPDF method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex, true);
                responseSharedTemplate.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(responseSharedTemplate);
            }
        }

        /// <summary>
        /// get Document Details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="loadMetaDataHistory"></param>
        /// <returns></returns>
        public async Task<IResult> LoadMetaDataWithHistoryDocuments(HttpRequest request, LoadMetaDataHistory loadMetaDataHistory)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "LoadMetaDataWithHistoryDocuments", "Process started for LoadMetaDataWithHistoryDocuments for Envelope ID:" + loadMetaDataHistory.envelopeId, "", "", "", "Send API");
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
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                        loggerModelNew.Email = userToken.EmailId;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        Guid UserId = userToken.UserID;
                        EnvelopeGetEnvelopeHistoryByEnvelopeCodeDocuments envelopeMetadata = await _envelopeRepository.GetEnvelopeMetaDataWithHistoryDocuments(new Guid(loadMetaDataHistory.envelopeId), userToken.EmailId, loadMetaDataHistory.envelopeArichiveData, loadMetaDataHistory.getDataType);
                        return Results.Ok(new
                        {
                            statusCode = HttpStatusCode.OK,
                            data = envelopeMetadata
                        });
                    }
                }
                else
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while getting data using LoadMetaDataWithHistory method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }

        }

    }

}
