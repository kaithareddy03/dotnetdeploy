using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.APIModels.Data;
using RSign.Models.APIModels.Envelope;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Data;
using System.Net;
using System.Web;
using System.Web.Http;

namespace RSign.SendAPI.API
{
    public class DashboardEndpoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly string _module = "DashboardEndpoint";
        private readonly string sourceType = "SendAPI";
        private readonly IHttpContextAccessor _accessor;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly ISignRepository _signRepository;
        private readonly IGenericRepository _genericRepository;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private readonly ISettingsRepository _settingsRepository;

        public DashboardEndpoint(IHttpContextAccessor accessor, IDashboardRepository dashboardRepository, IUserTokenRepository userTokenRepository, IConfiguration appConfiguration,
            IEnvelopeRepository envelopeRepository, ISignRepository signRepository, IGenericRepository genericRepository, IEnvelopeHelperMain envelopeHelperMain, ISettingsRepository settingsRepository)
        {
            _accessor = accessor;
            _dashboardRepository = dashboardRepository;
            _userTokenRepository = userTokenRepository;
            _appConfiguration = appConfiguration;
            _envelopeRepository = envelopeRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
            _signRepository = signRepository;
            _genericRepository = genericRepository;
            _envelopeHelperMain = envelopeHelperMain;
            _settingsRepository = settingsRepository;
        }
        public void RegisterDashboardAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Dashboard/GetEnvelopeHistoryByEnvelopeId", GetEnvelopeHistoryByEnvelopeId);
            app.MapGet("/api/v1/Dashboard/GetUserFolders", GetUserFolders);
            app.MapGet("/api/v1/Dashboard/DeleteUserFolder", DeleteUserFolder);
            app.MapPost("/api/v1/Dashboard/GetDashboardSummary", GetDashboardSummary);
            app.MapPost("/api/v1/Dashboard/GetDashboardDetails", GetDashboardDetails);           
            app.MapPost("/api/v1/Dashboard/GetAIAutoLockDetails", GetAIAutoLockDetails);
            app.MapPost("/api/v1/Dashboard/GetTemplateRuleDictionary", GetTemplateRuleDictionary);
            app.MapPost("/api/v1/Dashboard/GetDashboardDetailsWithEmailAndCompanyID", GetDashboardDetailsWithEmailAndCompanyID); //for stats page
            app.MapPost("/api/v1/Dashboard/OpenSignlink", OpenSignlink);
            app.MapPost("/api/v1/Dashboard/ManageEnvelopes", ManageEnvelopes);
            app.MapPost("/api/v1/Dashboard/SaveToManageFolder", SaveToManageFolder);
            app.MapPost("/api/v1/Dashboard/CreateFolder", CreateFolder);           
            app.MapPost("/api/v1/Dashboard/GetEnvelopesbyFolder", GetEnvelopesbyFolder);           
            app.MapPost("/api/v1/Dashboard/UpdateAndResendFromHome", UpdateAndResendFromHome);
            app.MapPost("/api/v1/Dashboard/ActivateDeactivateEnvelope", ActivateDeactivateEnvelope);
            app.MapPost("/api/v1/Dashboard/DeleteFromTable", DeleteFromTable);
            app.MapPost("/api/v1/Dashboard/RestoreEnvelopesFromFolder", RestoreEnvelopesFromFolder);

        }
        public async Task<IResult> GetDashboardSummary(HttpRequest request, DashboardRequestEntity dashboardRequestEntity)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetDashboardSummary", "Method Initialized,to Get Dashboard", string.Empty, string.Empty, string.Empty, string.Empty, sourceType);
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserDetails userDetails = _userTokenRepository.GetUserDetails(Guid.Empty, authToken!, string.Empty);
                    if (userDetails == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userDetails!.ReferenceKey!;
                        loggerModelNew.Email = userDetails!.EmailID!;
                        loggerModelNew.Message = "Getting user profile details by email=" + userDetails.EmailID;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        dashboardRequestEntity.UserID = userDetails!.UserID!;
                        dashboardRequestEntity.UserEmail = userDetails!.EmailID!;
                        dashboardRequestEntity.CompanyID = userDetails!.CompanyID!;
                        dashboardRequestEntity.LanguageCode = userDetails!.LanguageCode!;
                        APIDashboardSummary dashboardSummary = await _dashboardRepository.GetDashboardSummary(dashboardRequestEntity);
                        return Results.Ok(new
                        {
                            statusCode = HttpStatusCode.OK,
                            data = dashboardSummary
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
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest();
            }
        }
        public async Task<IResult> GetDashboardDetails(HttpRequest request, SearchEnvelopeForApi searchObj)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetDashboardDetails", "Method Initialized,to Get Dashboard", string.Empty, string.Empty, string.Empty, string.Empty, sourceType);
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserDetails userDetails = _userTokenRepository.GetUserDetails(Guid.Empty, authToken!, string.Empty);
                    if (userDetails == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userDetails!.ReferenceKey!;
                        loggerModelNew.Email = userDetails!.EmailID!;
                        loggerModelNew.Message = "Getting user profile details by email=" + userDetails.EmailID;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        searchObj.userID = userDetails!.UserID!;
                        searchObj.userEmail = userDetails!.EmailID!;
                        searchObj.companyId = (Guid)userDetails!.CompanyID!;
                        searchObj.languageCode = userDetails!.LanguageCode!;

                        List<DashboardRowDetails> dashboardDetails = await _dashboardRepository.GetDashboardDetails(searchObj);

                        DataSet referenceCodesandEmails = _dashboardRepository.GetReferenceCodesandEmails(userDetails);
                        var serializedData = _dashboardRepository.ConvertDataSetToJson(referenceCodesandEmails);

                        return Results.Ok(new
                        {
                            statusCode = HttpStatusCode.OK,
                            data = new { dashboardDetails, referenceCodesandEmails = serializedData }
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
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest();
            }
        }
        public IResult GetEnvelopeHistoryByEnvelopeId(HttpRequest request, Guid EnvelopeId)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetDashboardDetails", "Method Initialized,to Get Dashboard", string.Empty, string.Empty, string.Empty, string.Empty, sourceType);
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserToken userToken = _userTokenRepository.ValidateToken(request);
                    if (userToken == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    else
                    {
                       // loggerModelNew.AuthRefKey = userDetails!.ReferenceKey!;
                        loggerModelNew.Email = userToken!.EmailId!;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);


                        EnvelopeGetEnvelopeHistoryByEnvelopeCode envelopeHistory = _envelopeRepository.GetEnvelopeMetaDataWithHistory(EnvelopeId, userToken.EmailId!, string.Empty, "UTC");

                        return Results.Ok(new { statusCode = HttpStatusCode.OK, data = envelopeHistory });
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
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest();
            }
        }
        public async Task<IResult> GetAIAutoLockDetails(HttpRequest request, SearchAIAutoLocksForApi searchObj)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetDashboardDetails", "Method Initialized,to Get Dashboard", string.Empty, string.Empty, string.Empty, string.Empty, sourceType);
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserToken userToken = _userTokenRepository.ValidateToken(request);
                    if (userToken == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    else
                    {
                       // loggerModelNew.AuthRefKey = userToken!.ReferenceKey!;
                        loggerModelNew.Email = userToken!.EmailId!;
                        loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        searchObj.Sender = userToken.EmailId;
                        var autoLockDetails = await _dashboardRepository.GetAIAutoLockDetails(searchObj);
                        return Results.Ok(new { statusCode = HttpStatusCode.OK, data = autoLockDetails });
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
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest();
            }
        }
        public async Task<IResult> GetTemplateRuleDictionary(HttpRequest request, DashboardRequestEntity dashboardRequestEntity)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetTemplateRuleDictionary", "Method Initialized,to Get Dashboard", string.Empty, string.Empty, string.Empty, string.Empty, sourceType);
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var iHeader) || string.IsNullOrWhiteSpace(iHeader))
                {
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                string? authToken = iHeader.ElementAt(0);
                authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");

                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                UserDetails? userDetails = _userTokenRepository.GetUserDetails(Guid.Empty, authToken!, string.Empty);

                if (userDetails == null)
                {
                    loggerModelNew.Message = _appConfiguration["UnauthorizedAccess"];
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                loggerModelNew.AuthRefKey = userDetails.ReferenceKey;
                loggerModelNew.Email = userDetails.EmailID;
                loggerModelNew.Message = $"Getting user profile details by email = {userDetails.EmailID}";
                rSignLogger.RSignLogInfo(loggerModelNew);

                // Populate request entity with user info
                dashboardRequestEntity.UserID = userDetails!.UserID!;
                dashboardRequestEntity.UserEmail = userDetails!.EmailID!;
                dashboardRequestEntity.CompanyID = userDetails!.CompanyID!;
                dashboardRequestEntity.LanguageCode = userDetails!.LanguageCode!;

                TemplateRuleDictionary templateRuleDictionary = await _dashboardRepository.GetTemplateRuleDictionary(dashboardRequestEntity);

                return Results.Ok(new
                {
                    statusCode = HttpStatusCode.OK,
                    data = templateRuleDictionary
                });
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = $"Exception in GetTemplateRuleDictionary: {ex.Message}";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new
                {
                    statusCode = HttpStatusCode.BadRequest,
                    error = ex.Message
                });
            }
        }
        /// <summary>
        /// GetDashboardDetailsWithEmailAndCompanyID
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dashboardRequestEntity"></param>
        /// <returns></returns>
        public async Task<IResult> GetDashboardDetailsWithEmailAndCompanyID(HttpRequest request, DashboardRequestEntity dashboardRequestEntity)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetDashboardSummary", "Method Initialized, to Get Dashboard Details With Email And CompanyID", string.Empty, string.Empty, string.Empty, string.Empty, sourceType);
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserDetails userDetails = _userTokenRepository.GetUserDetails(Guid.Empty, authToken!, string.Empty);
                    if (userDetails == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userDetails!.ReferenceKey!;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        dashboardRequestEntity.LanguageCode = userDetails!.LanguageCode!;
                        DashboardResponseEntity dashboardSummary = await _dashboardRepository.GetDashboardDetailsWithEmailAndCompanyID(dashboardRequestEntity, userDetails);
                        return Results.Ok(dashboardSummary);
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
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest();
            }
        }

        public async Task<IResult> OpenSignlink(HttpRequest request, SigningLinkObject signingLinkObject)
        {
            loggerModelNew = new LoggerModelNew("", _module, "OpenSignlink", "Method Initialized,to open signing link", string.Empty, string.Empty, string.Empty, string.Empty, sourceType);
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                EnvelopeSigningUrlResponseModel responseModel = new();
               UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "OpenSignlink";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    string userEmail = userToken.EmailId;
                    var strURLWithData = string.Empty;
                    var isGenerateNewSigningUrl = false;
                    string[] arrID = signingLinkObject.id.Split(',');

                    //if (Convert.ToBoolean(signingLinkObject.IsSignerIdentity))
                    //{
                    //    if (isGenerateNewSigningUrl)
                    //        strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/authenticate-signer?";
                    //    else
                    //        strURLWithData = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/AuthenticateSigner?";
                    //}
                    // EnvelopeHelperMain envelopeHelperMain = new EnvelopeHelperMain();
                    EnvelopeSigningUrlModel envelopeSigningUrlModel = new EnvelopeSigningUrlModel
                    {
                        EnvelopeId = arrID[0],
                        SenderEmail = signingLinkObject.senderEmail
                    };
                    var response = _signRepository.getSigningURL(envelopeSigningUrlModel);
                    if (response.IsSuccessStatusCode)
                    {
                        strURLWithData = await response.Content.ReadAsStringAsync(); // use await if method is async
                        responseModel = JsonConvert.DeserializeObject<EnvelopeSigningUrlResponseModel>(strURLWithData);
                        isGenerateNewSigningUrl = responseModel.IsGenerateNewSigningUrl;
                    }
                    else
                    {
                        strURLWithData = "Error: " + response.StatusCode;
                    }
                    bool isFromportal = false;
                    if (signingLinkObject.isFromportal == "true" || signingLinkObject.isFromportal == "True")
                    {
                        isFromportal = true;
                    }

                    if (responseModel != null && (!string.IsNullOrEmpty(responseModel.PrefillRecpId) && arrID.Length > 0 && string.IsNullOrEmpty(arrID[1])))
                        arrID[1] = responseModel.PrefillRecpId;

                    strURLWithData = GenerateCommonSigningUrl(userEmail, arrID, Convert.ToString(_appConfiguration["AppKey"]), signingLinkObject.senderEmail, isFromportal, isGenerateNewSigningUrl);

                    ResponseSigningUrlModel responseSigningUrlModel = new ResponseSigningUrlModel();
                    if (signingLinkObject.isFromportal == "true")
                        responseSigningUrlModel.IsFromInbox = false;
                    else
                        responseSigningUrlModel.IsFromInbox = true;

                    return Results.Ok(new
                    {
                        statusCode = HttpStatusCode.OK,
                        data = strURLWithData
                    });
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = $"Exception in OpenSignlink: {ex.Message}";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(new
                {
                    statusCode = HttpStatusCode.BadRequest,
                    error = ex.Message
                });
            }
        }

        public string GenerateCommonSigningUrl(string email, string[] arrID, string appKey, string senderEmail = "", bool isFromportal = false, bool isGenerateNewSigningUrl = false)
        {
            var strURLWithData = string.Empty;

            if (isGenerateNewSigningUrl)
                strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/signer-landing?";
            else
                strURLWithData = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/Index?";
            if (isFromportal)
            {
                if (isGenerateNewSigningUrl)
                    strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/signer-landing?";
                else
                    strURLWithData = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/SignMultiTemplate?";
            }

            if (arrID[2] != null)
                strURLWithData = strURLWithData + HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("EnvelopeId={0}&RecipientId={1}&TemplateKey={2}&EmailId={3}", arrID[0], arrID[1], arrID[2], HttpUtility.UrlEncode(email)), appKey));
            else
                strURLWithData = strURLWithData + HttpUtility.UrlEncode(EncryptDecryptQueryString.Encrypt(string.Format("EnvelopeId={0}&RecipientId={1}&EmailId={2}", arrID[0], arrID[1], HttpUtility.UrlEncode(email)), appKey));

            return strURLWithData;
        }

        public async Task<IResult> ManageEnvelopes(HttpRequest request, SearchEnvelopeForApi searchObj)
        {
            loggerModelNew = new LoggerModelNew("Dashboard", "ManageEnvelopes",
                "Process started for Get Master Envelope Stats using API", searchObj.folderId.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            List<Guid> envIds = new List<Guid>();
            //IQueryable<DashboardDetails> searchDetails = null;
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                if (!string.IsNullOrEmpty(iHeader))
                {
                    string? authToken = iHeader.ElementAt(0);
                    authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                    UserDetails userDetails = _userTokenRepository.GetUserDetails(Guid.Empty, authToken!, string.Empty);
                    if (userDetails == null)
                    {
                        loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                        rSignLogger.RSignLogWarn(loggerModelNew);
                        return Results.Unauthorized();
                    }
                    else
                    {
                        loggerModelNew.AuthRefKey = userDetails!.ReferenceKey!;
                        loggerModelNew.Email = userDetails!.EmailID!;
                        loggerModelNew.Message = "Getting user profile details by email=" + userDetails.EmailID;
                        rSignLogger.RSignLogInfo(loggerModelNew);

                        searchObj.userID = userDetails!.UserID!;
                        searchObj.userEmail = userDetails!.EmailID!;
                        searchObj.companyId = (Guid)userDetails!.CompanyID!;
                        searchObj.languageCode = userDetails!.LanguageCode!;

                        string WaitingForOthers = _genericRepository.GetUniqueKey("WaitingforOthers", string.IsNullOrEmpty(searchObj.languageCode) ? "en-us" : searchObj.languageCode);
                        string ExpiringSoon = _genericRepository.GetUniqueKey("ExpiringSoon", string.IsNullOrEmpty(searchObj.languageCode) ? "en-us" : searchObj.languageCode);

                        List<DashboardRowDetails> searchDetails = new();
                        IQueryable<DashboardRowDetails> queryableData = Enumerable.Empty<DashboardRowDetails>().AsQueryable();

                        var inboxTabs = new[] { "Inbox", "Sent", "Completed", "Deleted", WaitingForOthers, ExpiringSoon };

                        if (inboxTabs.Contains(searchObj.activeTab))
                        {
                            searchDetails = await _dashboardRepository.GetDashboardDetails(searchObj);
                            queryableData = searchDetails.AsQueryable();
                        }
                        else
                        {
                            int totalCount;
                            if (searchObj != null && searchObj.filters != null && searchObj.filters.Count != 0 && !string.IsNullOrEmpty(Convert.ToString(searchObj.activeTab)))
                            {
                                searchDetails = await _dashboardRepository.GetEnvelopesByFolder(searchObj, userDetails.EmailID, userDetails.UserID, out totalCount);
                                queryableData = searchDetails.AsQueryable();
                            }

                        }
                        if (searchObj.filters != null && searchObj.filters.Count != 0 && searchDetails != null)
                        {
                            loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return Results.Unauthorized();
                        }
                        else
                        {

                            if (searchObj.filterOption == "All")
                            {
                                var predicate = PredicateBuilder.True<DashboardRowDetails>();
                                foreach (var filter in searchObj.filters)
                                {
                                    if (filter.key == "Move envelopes by sender")
                                    {
                                        predicate = predicate.And(x => x.SenderEmail == filter.value);
                                    }
                                    if (filter.key == "Move envelopes by reference code")
                                    {
                                        predicate = predicate.And(x => x.EnvelopeCode == filter.value);
                                    }
                                    if (filter.key == "Move envelopes by reference address")
                                    {
                                        predicate = predicate.And(x => x.EnvelopeCode == filter.value);
                                    }
                                    if (filter.key == "Move envelopes by status")
                                    {
                                        predicate = predicate.And(x => x.EnvelopeStatus == filter.value);
                                    }
                                    if (filter.key == "Move envelopes by first word in subject")
                                    {
                                        predicate = predicate.And(x => x.Subject.Contains(filter.value));
                                    }
                                }
                                envIds = queryableData.Where(predicate).Select(c => c.EnvelopeID).ToList();
                            }
                            else  //Any 
                            {
                                var predicate = PredicateBuilder.False<DashboardRowDetails>();
                                foreach (var filter in searchObj.filters)
                                {
                                    if (filter.key == "Move envelopes by sender")
                                    {
                                        predicate = predicate.Or(x => x.SenderEmail == filter.value);
                                    }
                                    if (filter.key == "Move envelopes by reference code")
                                    {
                                        predicate = predicate.Or(x => x.EnvelopeCode == filter.value);
                                    }
                                    if (filter.key == "Move envelopes by reference address")
                                    {
                                        predicate = predicate.Or(x => x.EnvelopeCode == filter.value);
                                    }
                                    if (filter.key == "Move envelopes by status")
                                    {
                                        predicate = predicate.Or(x => x.EnvelopeStatus == filter.value);
                                    }
                                    if (filter.key == "Move envelopes by first word in subject")
                                    {
                                        predicate = predicate.Or(x => x.Subject.Contains(filter.value));
                                    }
                                }
                                envIds = queryableData.Where(predicate).Select(c => c.EnvelopeID).ToList();
                            }
                            var manageFolderRequestObj = new ManageFolderRequestObj();
                            manageFolderRequestObj.EnvelopeIds = envIds;
                            manageFolderRequestObj.FolderId = searchObj.folderId;
                            bool status = _dashboardRepository.ManageEnvelopes(manageFolderRequestObj);

                            bool containsHtml = searchObj.FolderName != HttpUtility.HtmlEncode(searchObj.FolderName);
                            if (containsHtml)
                            {
                                loggerModelNew.Message = "Invalid Folder Name.";
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                return Results.Ok(new
                                {
                                    statusCode = HttpStatusCode.BadRequest,
                                    message = "Invalid Folder Name."
                                });
                            }
                            var userFoldersObj = new UserFolders();
                            userFoldersObj.ID = searchObj.folderId;
                            userFoldersObj.FolderName = searchObj.FolderName;
                            userFoldersObj.UserId = userDetails.UserID;
                            bool updateUserFolderStatus = _dashboardRepository.UpdateUserFolder(userFoldersObj);
                            if (!updateUserFolderStatus)
                            {
                                loggerModelNew.Message = "Folder with same Name already exists.";
                            }

                            loggerModelNew.Message = searchObj.activeTab + " envelopes moved to folder successfully.";
                            loggerModelNew.Email = userDetails.EmailID;
                            rSignLogger.RSignLogInfo(loggerModelNew);
                        }

                        return Results.Ok(new
                        {
                            statusCode = HttpStatusCode.OK,
                            data = new { searchDetails }
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
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest();
            }
        }

        public async Task<IResult> SaveToManageFolder(HttpRequest request, [System.Web.Http.FromBody] ManageFolderRequestObj manageFolderRequestObj)
        {
            string payloadData = "";//JsonConvert.SerializeObject(manageFolderRequestObj);
            loggerModelNew = new LoggerModelNew("Dashboard", "SaveToManageFolder",
              "Moving selected envelopes to UserFolder from Tab / folder", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var iHeader))
                {
                    loggerModelNew.Message = "Authorization header is missing.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                string? authToken = iHeader.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = "Authorization token is empty.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                //loggerModelNew.AuthRefKey = userDetails!.ReferenceKey!;
                loggerModelNew.Email = userToken!.EmailId!;
                loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;
                rSignLogger.RSignLogInfo(loggerModelNew);

                Guid enveID = manageFolderRequestObj.EnvelopeIds.FirstOrDefault();
                var envelopsLogSaved = _envelopeHelperMain.SaveEnvelopeWebAppLogs(enveID.ToString(), "", loggerModelNew.Email, "Dashboard/SaveToManageFolder", payloadData, "Process started to save the Envelopes to Manage Folder.");

                var status = _dashboardRepository.SaveManageFolder(manageFolderRequestObj);

                if (status)
                {
                    List<ManageFolder> lstFolders = _dashboardRepository.GetUserFolder(userToken.UserID);
                    loggerModelNew.Message = "Selected envelopes moved successfully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    //_envelopeRepository.UpdateDashboardSummary(userToken.UserID, userToken!.EmailId!);
                    return Results.Ok(new
                    {
                        status = true,
                        statusCode = HttpStatusCode.OK,
                        message = "Selected envelopes moved successfully.",
                        data = lstFolders
                    });
                }
                else
                {
                    loggerModelNew.Message = "Error occurred while deleting envelopes.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.Problem("Validation Failed", statusCode: 417);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in Dashboard controller while deleting envelopes.";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest();
            }
        }

        public async Task<IResult> CreateFolder(HttpRequest request, [System.Web.Http.FromBody] CreateFolderRequestObj createFolderObj)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddressFromAccessor(_accessor);
            //var payloadData = JsonConvert.SerializeObject(createFolderObj);
            loggerModelNew = new LoggerModelNew("", "DashboardController", "CreateFolder", "Process started for user folder creation. using API", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            InfoResultResonse responseMessage = new InfoResultResonse();

            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var iHeader))
                {
                    loggerModelNew.Message = "Authorization header is missing.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                string? authToken = iHeader.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = "Authorization token is empty.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                if (string.IsNullOrWhiteSpace(createFolderObj.FolderName))
                {
                    loggerModelNew.Message = "Folder Name cannot be blank.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.BadRequest(new
                    {
                        statusCode = 400,
                        message = "BadRequest."
                    });
                }

                bool containsHtml = createFolderObj.FolderName != HttpUtility.HtmlEncode(createFolderObj.FolderName);
                if (containsHtml)
                {
                    loggerModelNew.Message = "Invalid Folder Name.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Ok(new
                    {
                        statusCode = HttpStatusCode.BadRequest,
                        message = "Invalid Folder Name."
                    });
                }

                var createdFolder = _dashboardRepository.CreateUserFolder(createFolderObj);

                if (createdFolder != null)
                {
                    loggerModelNew.Message = "Folder created successfully.";
                    loggerModelNew.Email = userToken.EmailId;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.Ok(new
                    {
                        statusCode = HttpStatusCode.OK,
                        data = true
                    });
                }
                else
                {
                    //loggerModelNew.Message = "Folder with same Name already exists.";
                    loggerModelNew.Message = "A folder with this name already exists. Please choose a different name.";
                    loggerModelNew.Email = userToken.EmailId;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    //return Results.Problem("Validation Failed", statusCode: 417);
                    return Results.Problem(loggerModelNew.Message, statusCode: 417);
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in Dashboard controller while creating Folder.";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.UnprocessableEntity();
            }
        }

        public async Task<IResult> GetUserFolders(HttpRequest request)
        {
            loggerModelNew = new LoggerModelNew("", "Dashboard", "GetUserFolders", "Get user's folder on Dashboard.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var iHeader))
                {
                    loggerModelNew.Message = "Authorization header is missing.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                string? authToken = iHeader.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = "Authorization token is empty.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                List<ManageFolder> lstFolders = _dashboardRepository.GetUserFolder(userToken.UserID);
                loggerModelNew.Message = "User's folders retrieved successfully.";
                loggerModelNew.Email = userToken.EmailId;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(new
                {
                    statusCode = HttpStatusCode.OK,
                    data = lstFolders
                });

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in Dashboard controller while getting user's Folder.";
                rSignLogger.RSignLogError(loggerModelNew);
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest();
            }
        }

        public async Task<IResult> GetEnvelopesbyFolder(HttpRequest request, [System.Web.Http.FromBody] SearchEnvelopeForApi searchObj)
        {
            //var payloadData = JsonConvert.SerializeObject(searchObj);
            var loggerModelNew = new LoggerModelNew("", "Dashboard", $"GetEnvelopesByFolders ", "Get envelopes from user's folder on Dashboard.", "");
            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var iHeader))
                {
                    loggerModelNew.Message = "Authorization header is missing.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                string? authToken = iHeader.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = "Authorization token is empty.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                UserDetails userDetails = _userTokenRepository.GetUserDetails(Guid.Empty, authToken!, string.Empty);
                if (userDetails == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                loggerModelNew.AuthRefKey = userDetails!.ReferenceKey;

                // Inject user timezone from settings
                var settingsDetails = _settingsRepository.GetEntityForByKeyConfig(userDetails.UserID, Constants.SettingsKeyConfig.TimeZone);
                if (settingsDetails != null)
                {
                    searchObj.userTimezone = settingsDetails.OptionValue;
                }

                int totalCount;

                searchObj.userID = userDetails!.UserID!;
                searchObj.userEmail = userDetails!.EmailID!;
                searchObj.companyId = (Guid)userDetails!.CompanyID!;
                searchObj.languageCode = userDetails!.LanguageCode!;
                List<DashboardRowDetails> dashboardDetails = await _dashboardRepository.GetEnvelopesByFolder(searchObj, userDetails.EmailID, userDetails.UserID, out totalCount);

                DataSet referenceCodesandEmails = _dashboardRepository.GetReferenceCodesandEmails(userDetails);
                var serializedData = _dashboardRepository.ConvertDataSetToJson(referenceCodesandEmails);

                //List<LookupItem> referenceCodes = await _dashboardRepository.GetReferenceCodes();
                //List<LookupItem> senderEmails = await _dashboardRepository.GetSenderEmails(searchObj.companyId);

                loggerModelNew.Message = "Envelopes retrieved successfully from folder.";
                loggerModelNew.Email = userDetails.EmailID;
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Results.Ok(new
                {
                    Message = loggerModelNew.Message,
                    statusCode = HttpStatusCode.OK,
                    data = new { dashboardDetails, referenceCodesandEmails = serializedData }
                    //TotalCount = totalCount
                });

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in Dashboard controller while retrieving envelopes from folder.";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.UnprocessableEntity();
            }
        }

        public async Task<IResult> DeleteUserFolder(HttpRequest request, [FromQuery] int FolderId, [FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] Guid userId)
        {
            //string payloadData = JsonConvert.SerializeObject($"FolderId = {FolderId}");
            var loggerModelNew = new LoggerModelNew("", "Dashboard", "DeleteUserFolder ", "Delete User folder.", "");

            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var iHeader))
                {
                    loggerModelNew.Message = "Authorization header is missing.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                string? authToken = iHeader.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = "Authorization token is empty.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                bool status = _dashboardRepository.DeleteUserFolder(FolderId, startDate, endDate, (Guid)userId);

                loggerModelNew.Message = status
                    ? "Folder deleted successfully."
                    : "Folder cannot be deleted as it contains envelopes.";

                 return Results.Ok(new
                 {
                     statusCode = status ? HttpStatusCode.OK: HttpStatusCode.BadRequest,
                     data = status,
                     message = loggerModelNew.Message
                 });
                
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DashboardController DeleteUserFolder method.";               
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest();
            }
        }
        public async Task<SendDraftRespModal> UpdateAndResendFromHome(HttpRequest request, UpdateResendObj UpdateResendObj)
        {
            var loggerModelNew = new LoggerModelNew("", "Dashboard", "UpdateAndResendFromHome ", "To get the alert messages for update and resend funtion from home tab", "");
            SendDraftRespModal sendDraftRespModal = new();
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                Guid envelopeId = Guid.TryParse(UpdateResendObj?.EnvelopeId, out var parsedGuid) ? parsedGuid : Guid.Empty;

                bool isEnvelopeUpdate = _genericRepository.IsEnvelopeEditableAfterSend(envelopeId);
                if (!isEnvelopeUpdate)
                {
                    loggerModelNew.Message = "Unable to open sent envelope.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    sendDraftRespModal.StatusCode = HttpStatusCode.BadRequest;
                    sendDraftRespModal.StatusMessage = "BadRequest";
                    sendDraftRespModal.Message = _envelopeHelperMain.GetLanguageCodeBasedApiMessge(!string.IsNullOrEmpty(UpdateResendObj.CultureInfo) ? UpdateResendObj.CultureInfo : "en-us", "EnvelopeSignProcessMsg");
                    return sendDraftRespModal;
                }
                else
                {
                    loggerModelNew.Message = "Able to open sent envelope";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    sendDraftRespModal.StatusCode = HttpStatusCode.OK;
                    sendDraftRespModal.StatusMessage = "Sucess";
                    return sendDraftRespModal;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "UpdateAndResendFromHome";
                loggerModelNew.Message = "Exception at UpdateAndResendFromHome method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                sendDraftRespModal.StatusCode = HttpStatusCode.BadRequest;
                sendDraftRespModal.StatusMessage = "Error";
                sendDraftRespModal.Message = "Error";
                sendDraftRespModal.RetunUrl = "";
                return sendDraftRespModal;
            }
        }

        public async Task<IResult> ActivateDeactivateEnvelope(HttpRequest request, [Microsoft.AspNetCore.Mvc.FromBody] ManageFolderRequestObj reqObj)
        {
            string payloadData = JsonConvert.SerializeObject(reqObj);
            var loggerModelNew = new LoggerModelNew("", "Manage", "ActivateDeactivateEnvelope", "Active/Deactive i.e update the IsActive flag of Envelope to true/false using API.", "");

            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var iHeader))
                {
                    loggerModelNew.Message = "Authorization header is missing.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                string? authToken = iHeader.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = "Authorization token is empty.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                //loggerModelNew.AuthRefKey = userToken!.ReferenceKey!;
                loggerModelNew.Email = userToken!.EmailId!;
                loggerModelNew.Message = reqObj.IsActive == true ? "Restored envelope successfully." : "Deactivated envelope successfully.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                Guid EnveID = reqObj.EnvelopeIds.FirstOrDefault();
                var envelopsLogSaved = _envelopeHelperMain.SaveEnvelopeWebAppLogs(
                    EnveID.ToString(), "", loggerModelNew.Email, "Dashboard/ActivateDeactivateEnvelope", payloadData, "Active/Deactive i.e update the IsActive flag of Envelope to true/false using API.");

                bool isDeactivated = await _dashboardRepository.ActivateDeactivateEnvelope(reqObj.EnvelopeIds, reqObj.IsActive, reqObj.IsDeleted, userToken.EmailId);
               // _envelopeRepository.UpdateDashboardSummary(userToken.UserID, userToken.EmailId);
                return Results.Ok(new CustomAPIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = isDeactivated
                });

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in ActivateDeactivateEnvelope method.";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.UnprocessableEntity();
            }

        }

        public async Task<IResult> DeleteFromTable(HttpRequest request, [Microsoft.AspNetCore.Mvc.FromBody] ManageFolderRequestObj reqObj)
        {
            string payloadData = JsonConvert.SerializeObject(reqObj);
            var loggerModelNew = new LoggerModelNew("", "Dashboard", "DeleteFromTable", "Deleting selected envelopes permanently", "");

            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var iHeader))
                {
                    loggerModelNew.Message = "Authorization header is missing.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                string? authToken = iHeader.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = "Authorization token is empty.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                authToken = authToken?.Replace("Bearer ", ""); authToken = authToken?.Replace("bearer ", "");
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }

                //loggerModelNew.AuthRefKey = userDetails!.ReferenceKey!;
                loggerModelNew.Email = userToken!.EmailId!;
                loggerModelNew.Message = reqObj.IsActive == true ? "Restored envelope successfully." : "Deactivated envelope successfully.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                Guid EnveID = reqObj.EnvelopeIds.FirstOrDefault();
                Guid firstEnvelopeId = reqObj.EnvelopeIds.FirstOrDefault();

                _envelopeHelperMain.SaveEnvelopeWebAppLogs(
                firstEnvelopeId.ToString(), "", loggerModelNew.Email, "Dashboard/DeleteFromTable", payloadData, "Process started to Delete the Envelopes from Delete Folder.");

                bool isDeleted = await _dashboardRepository.DeleteEnvelopes(reqObj);

                if (isDeleted)
                {
                    return Results.Ok(new CustomAPIResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        Data = isDeleted
                    });
                }
                else
                {
                    loggerModelNew.Message = "Error occurred while deleting envelopes.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.BadRequest(new
                    {
                        statusCode = 400,
                        Message = "EnvelopeIds are required.",
                    });
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in ActivateDeactivateEnvelope method.";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.UnprocessableEntity();
            }

        }
        public async Task<IResult> RestoreEnvelopesFromFolder(HttpRequest request, [Microsoft.AspNetCore.Mvc.FromBody] ManageFolderRequestObj reqObj)
        {
            string payloadData = JsonConvert.SerializeObject(reqObj);
            var loggerModelNew = new LoggerModelNew("", "Dashboard", "RestoreEnvelopesFromFolder", "Restoring selected envelopes.", "");
            try
            {
                // 1. Check Authorization header
                if (!request.Headers.TryGetValue("Authorization", out var iHeader))
                {
                    loggerModelNew.Message = "Authorization header is missing.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                string? authToken = iHeader.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    loggerModelNew.Message = "Authorization token is empty.";
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                // 2. Clean token
                authToken = authToken?.Replace("Bearer ", "").Replace("bearer ", "");
                // 3. Get user details
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                //loggerModelNew.AuthRefKey = userDetails.ReferenceKey!;
                loggerModelNew.Email = userToken.EmailId!;
                rSignLogger.RSignLogInfo(loggerModelNew);
                // 4. Save log
                Guid enveID = reqObj.EnvelopeIds.FirstOrDefault();
                var envelopsLogSaved = _envelopeHelperMain.SaveEnvelopeWebAppLogs(enveID.ToString(), "", loggerModelNew.Email, "Dashboard/RestoreEnvelopesFromFolder", payloadData, "Process started to Restore the Envelopes to Folder.");
                // 5. Call repository to restore
                bool restoreStatus = await _dashboardRepository.RestoreAllAsync(reqObj.EnvelopeIds);
                if (restoreStatus)
                {
                    List<ManageFolder> lstFolders = _dashboardRepository.GetUserFolder(userToken.UserID);
                    loggerModelNew.Message = "Selected envelopes restored successfully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.Ok(new CustomAPIResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        Data = lstFolders,
                        Status = true,
                        Message = loggerModelNew.Message
                    });
                }
                else
                {
                    loggerModelNew.Message = "Error occurred while restoring envelopes.";
                    loggerModelNew.Email = loggerModelNew.Email;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.Problem("Validation Failed", statusCode: 417);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in RestoreEnvelopesFromFolder method.";
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.UnprocessableEntity();
            }
        }
    }
}
