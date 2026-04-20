using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.ManageDocument.Models;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Models;
using RSign.Models.Repository;
using RSign.Notification;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using Amazon.Runtime.Internal;

namespace RSign.SendAPI.API
{
    public class SettingsEndPoint
    {
        RSignLogger rSignLogger = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly string _module = "SettingsEndPoint";
        private ISettingsRepository _settingsRepository;
        private readonly IConfiguration _appConfiguration;
        private readonly IESignHelper _esignHelper;
        private IUserTokenRepository _userTokenRepository;
        private IContactDetailRepository _contactDetailRepository;
        private IUserRepository _userRepository;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private readonly ISharedAccessRepository _sharedAccessRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IManageAdminRepository _manageAdminRepository;
        public SettingsEndPoint(IConfiguration appConfiguration, ISettingsRepository settingsRepository,
                        IESignHelper esignHelper, IUserTokenRepository userTokenRepository,
                        IContactDetailRepository contactDetailRepository, IUserRepository userRepository,
                        IEnvelopeHelperMain envelopeHelperMain, ICompanyRepository companyRepository,
                        ISharedAccessRepository sharedAccessRepository, IManageAdminRepository manageAdminRepository
            )
        {
            _appConfiguration = appConfiguration;
            _settingsRepository = settingsRepository;
            _esignHelper = esignHelper;
            rSignLogger = new RSignLogger(_appConfiguration);
            _userTokenRepository = userTokenRepository;
            _contactDetailRepository = contactDetailRepository;
            _userRepository = userRepository;
            _envelopeHelperMain = envelopeHelperMain;
            _companyRepository = companyRepository;
            _sharedAccessRepository = sharedAccessRepository;
            _manageAdminRepository = manageAdminRepository;
        }
        public void RegisterSettingsAPI(WebApplication app)
        {
            app.MapGet("/api/v1/Settings/GetUserSettings", GetUserSettings);
            app.MapGet("/api/v1/Settings/GetCompanyAndPersonalSettings", GetCompanyAndPersonalSettings);
            app.MapGet("/api/v1/Settings/GetMultibrandingByCompanyId", GetMultibrandingByCompanyId);
            app.MapPost("/api/v1/Settings/SaveSettings", SaveSettings);
            app.MapPost("/api/v1/Settings/GetContactsDetailsByUserId", GetContactsDetailsByUserId);
            app.MapPost("/api/v1/Settings/IsDuplicateEmailExists", IsDuplicateEmailExists);
            app.MapPost("/api/v1/Settings/AddorUpdateContact", AddorUpdateContact);
            app.MapPost("/api/v1/Settings/GetCompanyIdByNameOrEmail", GetCompanyIdByNameOrEmail);
            app.MapPost("/api/v1/Settings/SaveSettingDetails", SaveSettingDetails);
            app.MapPost("/api/v1/Settings/AddUserSignatureText", AddUserSignatureText);
            app.MapPost("/api/v1/Settings/DeleteSignatureTextByID", DeleteSignatureTextByID);
            app.MapPost("/api/v1/Settings/SetSignatureAsDefault", SetSignatureAsDefault);
            app.MapPost("/api/v1/Settings/SaveUserProfile", SaveUserProfile);
            app.MapPost("/api/v1/Settings/ChangePassword", ChangePassword);
            app.MapPost("/api/v1/Settings/ConvertTextToSignImage", ConvertTextToSignImage);
            app.MapPost("/api/v1/Settings/GetSettingHistoryDetails", GetSettingHistoryDetails);
            app.MapGet("/api/v1/Settings/GetActiveCompanyUsers", GetActiveCompanyUsers);
            app.MapPost("/api/v1/Settings/GetShareAccessListByUserId", GetShareAccessListByUserId);
            app.MapGet("/api/v1/Settings/GetSharedAccessDropdownOptions", GetSharedAccessDropdownOptions);
            app.MapDelete("/api/v1/Settings/DeleteSharedAccessById", DeleteSharedAccessById);
            app.MapPost("/api/v1/Settings/AddOrUpdateShareAccess", AddOrUpdateShareAccess);
            app.MapPost("/api/v1/Settings/RestoreAndSaveUserDefaultSettings", RestoreAndSaveUserDefaultSettings);
            app.MapPost("/api/v1/Settings/DownloadContactFileExcelCSV", DownloadContactFileExcelCSV);
            app.MapPost("/api/v1/Settings/UploadContactsFile", UploadContactsFile);
            app.MapGet("/api/v1/Settings/GetContactDetailsByContactId", GetContactDetailsByContactId);
            app.MapGet("/api/v1/Settings/GetContactsFileDetailsByUserId", GetContactsFileDetailsByUserId);
            app.MapDelete("/api/v1/Settings/DeleteContact", DeleteContact);
            app.MapPost("/api/v1/Settings/GetAdminUsers", GetAdminUsers);
            app.MapPost("/api/v1/Settings/UpdateUserTypeAndRole", UpdateUserTypeAndRole);
            app.MapPost("/api/v1/Settings/SaveIntegrationDetails", SaveIntegrationDetails);
            app.MapPost("/api/v1/Settings/DownloadBulkSendingCSV", DownloadBulkSendingCSV);
            app.MapGet("/api/v1/Settings/GetCompanyAppkeys", GetCompanyAppkeys);
            app.MapPost("/api/v1/Settings/SaveCompanyAppNames", SaveCompanyAppNames);
            app.MapPost("/api/v1/Settings/UploadSignerStamp", UploadSignerStamp);

        }

        /// <summary>
        /// This api will be used to get User Settings based on userid
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //[Authorize]
        public async Task<IResult> GetUserSettings(HttpRequest request, string userId)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetUserSettings", "Endpoint Initialized,to Get user settings for user id:" + Convert.ToString(userId), "", "", userId, remoteIpAddress, "GetUserSettingsAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                APISettings apiUserSettings = _settingsRepository.GetEntityByParam(new Guid(userId), string.Empty, Constants.String.SettingsType.User);
                var userSettings = _esignHelper.TransformSettingsDictionaryToEntity(apiUserSettings);
                return Results.Ok(userSettings);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetUserSettings method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This api will be used to Get Company Settings based on user companyId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        //[Authorize]

        public async Task<IResult> GetCompanyAndPersonalSettings(HttpRequest request, string companyId, string userEmailAddress, int tabId, string companyName = "", string IntegrationType = "")
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);

            loggerModelNew = new LoggerModelNew("", _module, "GetCompanyAndPersonalSettings", "Endpoint Initialized,to Get company and personal settings for company id:" + Convert.ToString(companyId), "", "", "", remoteIpAddress, "GetCompanySettingsAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetCompanyAndPersonalSettings";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                AdminGeneralAndSystemSettings companySettings = new AdminGeneralAndSystemSettings();
                if (tabId != 23)
                {
                    SettingResponseMessage responseMessage = new SettingResponseMessage();
                    if ((!string.IsNullOrEmpty(userEmailAddress) || userProfile != null) && string.IsNullOrWhiteSpace(companyId))
                    {
                        UserSettingsModel userSettingsModel = new UserSettingsModel();
                        userSettingsModel.CompanyId = string.IsNullOrEmpty(companyId) ? Guid.Empty : new Guid(companyId);
                        userSettingsModel.Email = userEmailAddress;
                        userSettingsModel.TabId = tabId;
                        userSettingsModel.UserId = userProfile.UserID;
                        userSettingsModel.IntegrationType = IntegrationType != "" ? IntegrationType : "";
                        //userSettingsModel.isToGetWebhookSettings = isIntegrationManager;

                        responseMessage = _userRepository.GetUserSettingDetails(userProfile, userSettingsModel);
                    }
                    else
                    {
                        CompanySettingsModel companySettingsModel = new CompanySettingsModel();
                        companySettingsModel.CompanyId = companyId;
                        companySettingsModel.CompanyName = companyName;
                        companySettingsModel.TabId = tabId;
                        companySettingsModel.UserProfile = userProfile;
                        companySettingsModel.EmailAddress = userEmailAddress;
                        companySettingsModel.IntegrationType = IntegrationType != "" ? IntegrationType : "";

                        responseMessage = _userRepository.GetCompanySettings(userProfile, companySettingsModel, authToken);
                    }
                    if (responseMessage != null && responseMessage?.SettingInformation != null)
                    {
                        companySettings = _esignHelper.TransformSettingsDictionaryToEntity(responseMessage.SettingInformation);
                        companySettings.AdminAndSystemSettingOptionValues = responseMessage.SettingInformation.AdminGeneralAndSystemSettings;

                    }
                }
                else
                {
                    #region Getting groups data

                    CompanyGroupFullDetailsResponse response = new CompanyGroupFullDetailsResponse();
                    if (!string.IsNullOrWhiteSpace(companyId) && Guid.TryParse(companyId, out Guid parsedCompanyId))
                    {
                        var res = await _userRepository.GetCompanyFullDetails(parsedCompanyId);
                        companySettings.CompanyGroups = res.Groups;
                        companySettings.CompanyGroupUsers = res.GroupUsers;
                        companySettings.GroupBrandingDetails = res.GroupBrandingDetails;
                        companySettings.MultiBrandConfigurations = res.MultiBrandConfigurations;
                        companySettings.UserGroupMappings = res.UserGroupMappings;
                        companySettings.GroupUserCounts = res.GroupUserCounts;
                    }
                   #endregion Getting groups data
                }

                return Results.Ok(companySettings);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetCompanyAndPersonalSettings method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }


        public async Task<IResult> GetSharedAccessDropdownOptions(HttpRequest request)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);

            loggerModelNew = new LoggerModelNew("", _module, "GetSharedAccessDropdownOptions", "Endpoint Initialized,to Get shared access dropdown options", "", "", "", remoteIpAddress, "GetSharedAccessDropdownOptions");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetSharedAccessDropdownOptions";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                ResponseMessageForGetShareAccess responseMessage = new ResponseMessageForGetShareAccess();
                if (userProfile.UserID != null)
                {
                    responseMessage = _userRepository.GetShareAccessDropdownOptions(userProfile);
                }
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetSharedAccessDropdownOptions method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// This API used to get the company ID to search the settings based on the input provided in Either company name or email address
        /// </summary>
        /// <param name="request"></param>
        /// <param name="companyName"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<IResult> GetCompanyIdByNameOrEmail(HttpRequest request, SearchRequest model)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            SearchSettingsResponseMessage settingResponseMessage = new();
            loggerModelNew = new LoggerModelNew("", _module, "GetCompanyIdByNameOrEmail", "Endpoint Initialized,to Get company for company Name:" + Convert.ToString(model.UserEmail) + "User Email: " + model.UserEmail, "", "", "", remoteIpAddress, "GetCompanyIdByNameOrEmail");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetCompanyIdByNameOrEmail";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                settingResponseMessage = _settingsRepository.GetCompanyIdByNameOrEmail(model.CompanyName, model.UserEmail);
                loggerModelNew.Method = "GetCompanyIdByNameOrEmail";
                loggerModelNew.Message = "GetCompanyIdByNameOrEmail ==> END. successfully returning the information.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(settingResponseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetCompanyIdByNameOrEmail method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This api will be used to Save user Settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="SettingsPrepareModal"></param>
        /// <returns></returns>
        public async Task<IResult> SaveSettings(HttpRequest request, SettingsPrepareModal SettingsPrepareModal)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SaveSettings", "Save Settings for User :" + Convert.ToString(SettingsPrepareModal.UserId), "", "", "", remoteIpAddress, "SaveSettings");
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
                    SettingResponseMessage responseMessage = await _settingsRepository.SaveSettings(SettingsPrepareModal.UserSettingsData, new Guid(SettingsPrepareModal.UserId));
                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at SaveSettings method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This api will be used to Get Multibranding By CompanyId
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<IResult> GetMultibrandingByCompanyId(HttpRequest request, Guid companyId, Guid userID, bool fullList = true)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            ResponseBrandingConfiguration responseMessage = new ResponseBrandingConfiguration();
            loggerModelNew = new LoggerModelNew("", "Settings", "GetMultibrandingByCompanyId", "Process started for getting multibranding list", "", "", "", remoteIpAddress, "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                List<MultiBrandConfig> multiBrandConfigurations = _settingsRepository.getBrandsList(companyId, userID, fullList);
                return Results.Ok(multiBrandConfigurations);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetMultibrandingByCompanyId method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This api will be used to Get Contacts Details By UserId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="SettingsPrepareModal"></param>
        /// <returns></returns>
        public async Task<IResult> GetContactsDetailsByUserId(HttpRequest request, FilterContactListforApi filter)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetContactsDetailsByUserId", "Process started for Get Contacts Details By UserId", "", "", "", remoteIpAddress, "GetContactsDetailsByUserId");
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
                    request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                    string? authToken = iHeader.ElementAt(0);
                    if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                    if (string.IsNullOrEmpty(authToken))
                    {
                        return Results.BadRequest(new
                        {
                            success = false,
                            message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                            data = new List<ErrorTagDetailsResponse>()
                        });
                    }
                    UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                    if (userProfile == null)
                    {
                        loggerModelNew.Module = _module;
                        loggerModelNew.Method = "DeleteContact";
                        loggerModelNew.Message = "User Profile is null";
                        rSignLogger.RSignLogWarn(loggerModelNew);

                        return Results.BadRequest(new
                        {
                            success = false,
                            message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                            data = new List<ErrorTagDetailsResponse>()
                        });
                    }

                    if (string.IsNullOrWhiteSpace(Convert.ToString(filter.CompanyID)))
                        filter.CompanyID = userProfile?.CompanyID;

                    return Results.Ok(await _contactDetailRepository.GetAllContactsDetailsByUserId(userToken.UserID, filter, filter.TabType));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetContactsDetailsByUserId method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This api will be used to check IsDuplicateEmailExists 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IResult> IsDuplicateEmailExists(HttpRequest request, DuplicateEmailExistsModel duplicateEmailExistsModel)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "IsDuplicateEmailExists", "Process started for checking duplicate email exists", "", "", "", remoteIpAddress, "IsDuplicateEmailExists");
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
                    return Results.Ok(await _contactDetailRepository.CheckForDuplicateEmail(userToken.UserID, duplicateEmailExistsModel));
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at IsDuplicateEmailExists method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// This api will be used to add or update user conacts from step 1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="contactDetail"></param>
        /// <returns></returns>
        public async Task<IResult> AddorUpdateContact(HttpRequest request, ContactDetail contactDetail)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "AddorUpdateContact", "Process started for adding or update contact", "", "", "", remoteIpAddress, "IsDuplicateEmailExists");
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
                    request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                    if (!string.IsNullOrEmpty(iHeader))
                    {
                        string? authToken = iHeader.ElementAt(0);
                        if (!string.IsNullOrEmpty(authToken))
                        {
                            authToken = authToken?.Replace("Bearer ", "");
                            authToken = authToken?.Replace("bearer ", "");
                            authToken = authToken?.Replace("undefined", "");
                            var user = _userTokenRepository.GetUserProfileByToken(authToken);
                            return Results.Ok(await _contactDetailRepository.AddorUpdateContact(user, contactDetail));
                        }
                        else return Results.BadRequest();
                    }
                    else return Results.BadRequest();
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at AddorUpdateContact method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public IResult GetExternalSettingsByType(HttpRequest request, Guid Userid, string Type)
        {

            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "AddorUpdateContact", "Process started for adding or update contact", "", "", "", remoteIpAddress, "IsDuplicateEmailExists");
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
                    SettingsExternalIntegration settingsExternal = new SettingsExternalIntegration();
                    var ExternalSettings = _settingsRepository.GetExternalSettingsByType(Userid, Type, Userid);

                    UserProfile userProfile = _userTokenRepository.GetUserProfileByEmail(userToken.EmailId);

                    var CompanyExternalSettings = _settingsRepository.GetExternalSettingsByCompanyId((Guid)userProfile.CompanyID, Type);
                    if (Type == Constants.UploadDriveType.netDocuments || Type == Constants.UploadDriveType.Vincere)
                    {
                        CompanyExternalSettings = _settingsRepository.GetExternalSettingsByCompanyId((Guid)userProfile.CompanyID, Type);
                        if (CompanyExternalSettings != null && ExternalSettings != null)
                        {
                            ExternalSettings.ApplicationAPIURL = CompanyExternalSettings.ApplicationAPIURL;
                            ExternalSettings.ApplicationURL = CompanyExternalSettings.ApplicationURL;
                            ExternalSettings.AppClientId = CompanyExternalSettings.AppClientId;
                            ExternalSettings.DefaultRepository = CompanyExternalSettings.DefaultRepository;
                            ExternalSettings.AppClientSecret = CompanyExternalSettings.AppClientSecret;
                            ExternalSettings.RedirectURI = CompanyExternalSettings.RedirectURI;
                        }
                    }
                    var IncludeSignedCertificateSetting = _settingsRepository.GetEntityForByKeyConfig(Userid, Constants.SettingsKeyConfig.IncludeSignedCertificateOnSignedPDF);
                    if (IncludeSignedCertificateSetting != null)
                    {
                        bool IncludeSignedCertificateOnSignedPDF = Convert.ToBoolean(IncludeSignedCertificateSetting.OptionValue);
                    }
                    return Results.Ok(ExternalSettings);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "SettingsEndPoint API EndPoint - Exception at GetExternalSettingsByType method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> SaveSettingDetails(HttpRequest request, APISettings apiSettings)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            SaveSettingResponseMessage settingResponseMessage = new();
            loggerModelNew = new LoggerModelNew("", _module, "SaveSettingDetails", "Endpoint Initialized,to Save Setting details based on the tab selected", "", "", "", remoteIpAddress, "SaveSettingDetails");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SaveSettingDetails";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                settingResponseMessage = _settingsRepository.SaveSettingDetails(userProfile, apiSettings);
                if (settingResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    loggerModelNew.Method = "SaveSettingDetails";
                    loggerModelNew.Message = "SaveSettingDetails ==> Save failed. Skipping user & company settings fetch.";
                    rSignLogger.RSignLogInfo(loggerModelNew);

                    return Results.BadRequest(settingResponseMessage);
                }
                loggerModelNew.Method = "SaveSettingDetails";
                loggerModelNew.Message = "SaveSettingDetails ==> END.Settings saved successfully returning the information.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                loggerModelNew.Method = "SaveSettingDetails";
                loggerModelNew.Message = "SaveSettingDetails ==> Start: Getting user settings after save success.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                if (!string.IsNullOrWhiteSpace(apiSettings.UserEmail))
                    userProfile = _settingsRepository.GetUserProfileByEmailID(apiSettings.UserEmail);
                UserSettingsModel userSettingsModel = new UserSettingsModel();
                userSettingsModel.Email = userProfile.EmailID;
                userSettingsModel.CompanyId = userProfile.CompanyID;
                userSettingsModel.TabId = apiSettings.TabId;
                userSettingsModel.UserId = userProfile.UserID;
                SettingResponseMessage userSettingsResponseMessage = _userRepository.GetUserSettingDetails(userProfile, userSettingsModel);
                UserAdditionalResponseMessage userAdditionalResponseMessage = new UserAdditionalResponseMessage();
                if (userSettingsResponseMessage.SettingInformation == null)
                {
                    settingResponseMessage.userSettings = null;
                    loggerModelNew.Method = "SaveSettingDetails";
                    loggerModelNew.Message = "SaveSettingDetails ==> Getting user settings as null.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }
                else
                {
                    AdminGeneralAndSystemSettings userSettings = _esignHelper.TransformSettingsDictionaryToEntity(userSettingsResponseMessage.SettingInformation);
                    userSettings.SelectedTimeZone = string.IsNullOrEmpty(userSettings.SelectedTimeZone) ? "UTC" : userSettings.SelectedTimeZone;
                    settingResponseMessage.userSettings = userSettings;
                    loggerModelNew.Method = "SaveSettingDetails";
                    loggerModelNew.Message = "SaveSettingDetails ==> End: Getting user settings after save success.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }

                loggerModelNew.Method = "SaveSettingDetails";
                loggerModelNew.Message = "SaveSettingDetails ==> Start: Getting company settings after save success.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                CompanySettingsModel companySettingsModel = new CompanySettingsModel();
                companySettingsModel.CompanyId = Convert.ToString(userSettingsModel.CompanyId);
                companySettingsModel.CompanyName = "";
                companySettingsModel.TabId = userSettingsModel.TabId;
                companySettingsModel.UserProfile = userProfile;
                companySettingsModel.EmailAddress = userSettingsModel.Email;

                SettingResponseMessage companySettingsResponseMessage = _userRepository.GetCompanySettings(userProfile, companySettingsModel);
                if (companySettingsResponseMessage.SettingInformation == null)
                {
                    settingResponseMessage.companySettings = null;
                    loggerModelNew.Method = "SaveSettingDetails";
                    loggerModelNew.Message = "SaveSettingDetails ==> Getting company settings as null.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }
                else if (companySettingsResponseMessage.SettingInformation != null)
                {
                    AdminGeneralAndSystemSettings companySettings = _esignHelper.TransformSettingsDictionaryToEntity(companySettingsResponseMessage.SettingInformation);
                    companySettings.SelectedTimeZone = string.IsNullOrEmpty(companySettings.SelectedTimeZone) ? "UTC" : companySettings.SelectedTimeZone;
                    settingResponseMessage.companySettings = companySettings;
                    loggerModelNew.Method = "SaveSettingDetails";
                    loggerModelNew.Message = "SaveSettingDetails ==> End: Getting company settings after save success.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }
                return Results.Ok(settingResponseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at SaveSettingDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetSettingHistoryDetails(HttpRequest request, RetrieveSettingsHistoryDetails retrieveSettingsHistoryDetails)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetSettingHistoryDetails", "Process started for getting setting history details", "", "", "", remoteIpAddress, "GetSettingHistoryDetails");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetSettingHistoryDetails";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                else
                {
                    List<SettingsHistory> settingsHistories = new();
                    if (retrieveSettingsHistoryDetails.TabId == 15)
                        settingsHistories = await _settingsRepository.SettingIntegrationHistoryDetails(userProfile, retrieveSettingsHistoryDetails);
                    else
                        settingsHistories = await _settingsRepository.SettingHistoryDetails(userProfile, retrieveSettingsHistoryDetails);
                    return Results.Ok(settingsHistories);
                    //return Results.Ok();
                }

            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetSettingHistoryDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> AddUserSignatureText(HttpRequest request, UserSignatureText signatureText)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            APIUserSignatureResponse responseMessage = new();
            loggerModelNew = new LoggerModelNew("", _module, "AddUserSignatureText", "Add user signature", "", "", "", remoteIpAddress, "AddUserSignatureText");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "AddUserSignatureText";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                APIUserSignatureText signature = new APIUserSignatureText();
                signature.ID = signatureText.ID;
                signature.SignatureName = signatureText.SignatureName;
                signature.SignatureText = signatureText.SignatureText;
                signature.UserId = userProfile.UserID;

                responseMessage = _settingsRepository.AddUpdateSignatureText(userProfile, signature);
                loggerModelNew.Method = "AddUserSignatureText";
                loggerModelNew.Message = "AddUserSignatureText ==> END.Settings saved successfully returning the information.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "Success";
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at AddUserSignatureText method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> DeleteSignatureTextByID(HttpRequest request, Guid SignatureID)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            APIUserSignatureResponse settingResponseMessage = new();
            loggerModelNew = new LoggerModelNew("", _module, "DeleteSignatureTextByID", "Delete user signature", "", "", "", remoteIpAddress, "AddUserSignatureText");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteSignatureTextByID";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                settingResponseMessage = _settingsRepository.DeleteUserSignatureText(SignatureID, userProfile.UserID);
                loggerModelNew.Method = "DeleteSignatureTextByID";
                loggerModelNew.Message = "DeleteSignatureTextByID ==> END.Settings saved successfully returning the information.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(settingResponseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at DeleteSignatureTextByID method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> SetSignatureAsDefault(HttpRequest request, APIUserSignatureText signatureText)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "AddUserSignatureText", "Add user signature", "", "", "", remoteIpAddress, "AddUserSignatureText");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SetSignatureAsDefault";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                APIUserSignatureResponse responseMessage = _settingsRepository.SetSignatureAsDefault(userProfile, signatureText);
                loggerModelNew.Method = "SetSignatureAsDefault";
                loggerModelNew.Message = "SetSignatureAsDefault ==> END.Settings saved successfully returning the information.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at SetSignatureAsDefault method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> SaveUserProfile(HttpRequest request, EditUserProfile editUserProfile)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SaveUserProfile", "Save user Profile from settings", "", "", "", remoteIpAddress, "SaveUserProfile");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SaveUserProfile";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                ResponseMessageForEditUserProfile responseMessage = _settingsRepository.SaveUserProfile(userProfile, editUserProfile);
                loggerModelNew.Method = "SaveUserProfile";
                loggerModelNew.Message = "SaveUserProfile ==> END.Settings saved successfully returning the information.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at SaveUserProfile method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        private string GetBrowserName(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Unknown";

            if (userAgent.Contains("Edg")) return "Microsoft Edge";
            if (userAgent.Contains("Chrome") && !userAgent.Contains("Edg")) return "Google Chrome";
            if (userAgent.Contains("Firefox")) return "Mozilla Firefox";
            if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) return "Apple Safari";
            if (userAgent.Contains("OPR") || userAgent.Contains("Opera")) return "Opera";
            if (userAgent.Contains("Trident") || userAgent.Contains("MSIE")) return "Internet Explorer";

            return "Unknown";
        }

        public async Task<IResult> ChangePassword(HttpRequest request, ChangePasswordModel model)
        {
            ResponseMessageForEditUserProfile responseMessage = new();

            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "ChangePassword", "change password of user Profile from settings", "", "", "", remoteIpAddress, "ChangePassword");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SaveUserProfile";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                string userAgent = request.Headers["User-Agent"].ToString();
                string browserName = GetBrowserName(userAgent);
                responseMessage = _userRepository.ChangePasswordOfUser(model, responseMessage, authToken, userProfile, remoteIpAddress, browserName);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    responseMessage.userProfile = userProfile = _userTokenRepository.GetUserProfileByToken(responseMessage.AccessToken);
                }
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at ChangePassword method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> ConvertTextToSignImage(HttpRequest request, ConvertTextToSignImageModel convertTextToSignImage)
        {
            ResponseConvertTextToSignImage responseConvertTextToSignImage = new();
            ResponseMessageForConvertTextToSignImage responseMessage = new();
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "ChangePassword", "change password of user Profile from settings", "", "", "", remoteIpAddress, "ChangePassword");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SaveUserProfile";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                string userAgent = request.Headers["User-Agent"].ToString();
                string browserName = GetBrowserName(userAgent);

                int intHeight = 0;
                int intWidth = 0;
                var pfc = new PrivateFontCollection();
                var FontFolderPath = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["FontFolderPath"]));

                string fontPath = System.IO.Path.Combine(Convert.ToString(FontFolderPath), Convert.ToString(convertTextToSignImage.font + ".ttf"));
                //fontPath = @"D:\Sprint25_DEV\CommonFiles\Fonts\HomemadeApple.ttf";
                pfc.AddFontFile(fontPath);

                string imageSource = _envelopeHelperMain.ConvertSignImage(pfc, convertTextToSignImage.text, convertTextToSignImage.font, convertTextToSignImage.fontsize, convertTextToSignImage.fontColor, convertTextToSignImage.height, convertTextToSignImage.width, out intHeight, out intWidth, convertTextToSignImage.envelopeCode, convertTextToSignImage.electronicSignIndicationId, convertTextToSignImage.dateFormat, convertTextToSignImage.userTimezone, convertTextToSignImage.dateFormatID);
                responseConvertTextToSignImage.imgsrc = imageSource;
                responseConvertTextToSignImage.height = intHeight;
                responseConvertTextToSignImage.width = intWidth;
                responseMessage.Message = "Success";
                responseMessage.StatusMessage = "Success";
                responseMessage.StatusCode = HttpStatusCode.OK;

                responseMessage.ResponseConvertTextToSignImage = responseConvertTextToSignImage;
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at ChangePassword method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetActiveCompanyUsers(HttpRequest request, Guid companyId, string userSearchTerm)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);

            loggerModelNew = new LoggerModelNew("", _module, "GetActiveCompanyUsers", "Endpoint Initialized,to Get user emails and auto complete for company: " + Convert.ToString(companyId), "", "", "", remoteIpAddress, "GetActiveCompanyUsers");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetActiveCompanyUsers";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                AutoCompleteSettingResponseMessage responseMessage = new AutoCompleteSettingResponseMessage();
                if (companyId != null && userProfile != null && !string.IsNullOrWhiteSpace(userSearchTerm))
                {
                    responseMessage.userListWithCompanies = _companyRepository.GetActiveCompanyUsers(companyId, userSearchTerm);
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    return Results.Ok(responseMessage);
                }

                return Results.BadRequest(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetActiveCompanyUsers method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetShareAccessListByUserId(HttpRequest request, FilterShareAccessListforApi filterModel)
        {
            SharedAccessResponeMessage responseMessage = new();

            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetShareAccessListByUserId", "Getting shared access list based on tab shared with me or shared with Others from settings", "", "", "", remoteIpAddress, "ChangePassword");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetShareAccessListByUserId";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                string userAgent = request.Headers["User-Agent"].ToString();
                string browserName = GetBrowserName(userAgent);
                int totalContactsCount = 0;
                filterModel.TotalRowsCount = totalContactsCount;
                filterModel.UserID = userProfile.UserID;
                filterModel.Page = filterModel.Page == null ? 1 : filterModel.Page;
                filterModel.PageSize = filterModel.PageSize == null ? 10 : filterModel.PageSize;
                filterModel.SortBy = filterModel.SortBy;
                filterModel.CompanyID = userProfile.CompanyID;
                filterModel.LanguageID = userProfile.LanguageID;
                filterModel.StartDate = filterModel.StartDate;
                filterModel.EndDate = filterModel.EndDate;
                filterModel.SharedStatus = filterModel.SharedStatus == null ? 0 : filterModel.SharedStatus;
                filterModel.Email = string.IsNullOrEmpty(filterModel.Email) ? "" : filterModel.Email;
                filterModel.Name = string.IsNullOrEmpty(filterModel.Name) ? "" : filterModel.Name;
                filterModel.TabType = filterModel.TabType == null ? 1 : filterModel.TabType;
                responseMessage.ShareAccessDetails = _sharedAccessRepository.GetShareAccessListByUserId(filterModel);
                responseMessage.StatusMessage = "Shared Access List retrived successfully.";
                responseMessage.StatusCode = HttpStatusCode.OK;
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetShareAccessListByUserId method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> DeleteSharedAccessById(HttpRequest request, int id)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "DeleteSharedAccessById", "changing the status of the shared access record based on ID shared with Others from settings", "", "", "", remoteIpAddress, "ChangePassword");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteSharedAccessById";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                bool isDeleted = _sharedAccessRepository.ShareAccessChangeStatus(id, userProfile?.EmailID);
                if (isDeleted)
                {
                    loggerModelNew.Message = "Shared access status updated successfully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.ReasonPhrase = "Shared Access deleted successfully";
                    responseMessage.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    loggerModelNew.Message = "Shared access status not updated successfully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.ReasonPhrase = "Shared Access not deleted successfully";
                    responseMessage.StatusCode = HttpStatusCode.UnprocessableEntity;
                }
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Shared access status not updated successfully." + "Exception occured: " + ex.Message.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(new
                {
                    success = false,
                    message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                    data = new List<ErrorTagDetailsResponse>()
                });
            }
        }

        public async Task<IResult> AddOrUpdateShareAccess(HttpRequest request, ShareAccessDetail shareAccessDetail)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "AddOrUpdateShareAccess", "Saving shared with Others from settings", "", "", "", remoteIpAddress, "AddOrUpdateShareAccess");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageForAddContact responseMessage = new ResponseMessageForAddContact();
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "AddOrUpdateShareAccess";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                shareAccessDetail.companyId = userProfile.CompanyID;
                shareAccessDetail.sharedUserName = userProfile.FullName;
                bool isSuccess = _sharedAccessRepository.ShareAccessAddOrUpdate(shareAccessDetail, "");
                if (isSuccess)
                {
                    loggerModelNew.Message = "Shared access created updated successfully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.Message = "Shared Access deleted successfully";
                    responseMessage.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    loggerModelNew.Message = "Shared access not created successfully.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.Message = loggerModelNew.Message;
                    responseMessage.StatusCode = HttpStatusCode.UnprocessableEntity;
                }
                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Shared access status not updated successfully." + "Exception occured: " + ex.Message.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(new
                {
                    success = false,
                    message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                    data = new List<ErrorTagDetailsResponse>()
                });
            }
        }

        public async Task<IResult> RestoreAndSaveUserDefaultSettings(HttpRequest request, ResetSettings resetSettings)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "RestoreAndSaveUserDefaultSettings", "Restore and saving defaultsettings", "", "", "", remoteIpAddress, "RestoreAndSaveUserDefaultSettings");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = _userTokenRepository.GetUserProfileByEmail(resetSettings.UserEmail);
                if (userProfile == null || userProfile.UserID == Guid.Empty)
                {
                    string msg = userProfile == null ? "Email Id Not Found" : "User Id Not Found";
                    loggerModelNew.Message = msg;
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.NotFound(new SettingResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        StatusMessage = "NotFound",
                        Message = msg
                    });
                }

                string loggedInEmailAddress = _userTokenRepository.GetUserEmailByToken(authToken);
                Guid loggedInUserId = _userTokenRepository.GetUserProfileUserIDByEmail(resetSettings.UserEmail);

                _settingsRepository.SaveDefaultSettingsByGroup(loggedInUserId, userProfile.UserID, Constants.String.SettingsType.User, resetSettings.GroupId, resetSettings.IsLockCheckReq, userProfile.CompanyID, authToken, loggedInEmailAddress, resetSettings.SubtabId);

                var getSetting = _settingsRepository.GetEntityByParam(userProfile.UserID, resetSettings.UserEmail, Constants.String.SettingsType.User);
                
                loggerModelNew.Method = "RestoreAndSaveUserDefaultSettings";
                loggerModelNew.Message = "RestoreAndSaveUserDefaultSettings ==> Start: Getting user settings after save success.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                UserSettingsModel userSettingsModel = new UserSettingsModel();
                userSettingsModel.Email = userProfile.EmailID;
                userSettingsModel.CompanyId = userProfile.CompanyID;
                userSettingsModel.TabId = resetSettings.GroupId;
                userSettingsModel.UserId = userProfile.UserID;
                SettingResponseMessage userSettingsResponseMessage = _userRepository.GetUserSettingDetails(userProfile, userSettingsModel);
                SaveSettingResponseMessage settingResponseMessage = new SaveSettingResponseMessage();

                if (userSettingsResponseMessage.SettingInformation == null)
                {
                    settingResponseMessage.userSettings = null;
                    loggerModelNew.Method = "RestoreAndSaveUserDefaultSettings";
                    loggerModelNew.Message = "RestoreAndSaveUserDefaultSettings ==> Getting user settings as null.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }
                else
                {
                    AdminGeneralAndSystemSettings userSettings = _esignHelper.TransformSettingsDictionaryToEntity(userSettingsResponseMessage.SettingInformation);
                    userSettings.SelectedTimeZone = string.IsNullOrEmpty(userSettings.SelectedTimeZone) ? "UTC" : userSettings.SelectedTimeZone;
                    settingResponseMessage.userSettings = userSettings;
                    loggerModelNew.Method = "RestoreAndSaveUserDefaultSettings";
                    loggerModelNew.Message = "RestoreAndSaveUserDefaultSettings ==> End: Getting user settings after save success.";
                    rSignLogger.RSignLogInfo(loggerModelNew);
                }
                var responseMessage = new SettingResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    StatusMessage = "OK",
                    Message = "",
                    SettingInformation = getSetting,
                    UserSettings = settingResponseMessage.userSettings
                };

                loggerModelNew.Message = "Successfully loaded settings detail.";
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at RestoreAndSaveUserDefaultSettings method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> DownloadContactFileExcelCSV(HttpRequest request, string contactFileType)
        {
            loggerModelNew = new LoggerModelNew("", _module, "DownloadContactFileExcelCSV", "Downloading the Contact File Excel or CSV", "", "", "", "", "DownloadContactExcel");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                string filePath = contactFileType?.ToLower() == "excel" ? String.Format("{0}", _appConfiguration["UploadContactExcelFile"]) : String.Format("{0}", _appConfiguration["UploadContactCSVFile"]);

                if (!System.IO.File.Exists(filePath))
                {
                    loggerModelNew.Message = "Document not found, file path is: " + filePath + ", contactFileType is " + contactFileType;
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.NotFound(new SettingResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        StatusMessage = "NotFound",
                        Message = "Document not found"
                    });
                }

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                string fileBase64 = Convert.ToBase64String(fileBytes);

                loggerModelNew.Message = "Successfully retrieved the document, file path is: " + filePath + ", contactFileType is " + contactFileType;
                rSignLogger.RSignLogInfo(loggerModelNew);

                var responseMessageSuccess = new ResponseMessageFile
                {
                    Base64FileData = fileBase64,
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    byteArray = fileBytes,
                    StatusCode = HttpStatusCode.OK,
                    StatusMessage = "OK",
                    Message = contactFileType?.ToLower() == "excel" ? "Contact Excel downloaded successfully" : "Contact csv downloaded successfully"
                };

                return Results.File(fileBytes, "application/octet-stream", responseMessageSuccess.FileName);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at DownloadContactFileExcelCSV method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> UploadContactsFile(HttpRequest request)
        {
            loggerModelNew = new LoggerModelNew("", _module, "UploadContactFile", "uploading the Contact File Excel or CSV", "", "", "", "", "UploadContactFile");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "UploadContactsFile";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                if (request.Form.Files.Count > 0)
                {
                    var Ifile = request.Form.Files[0];
                    Stream fs = Ifile.OpenReadStream();
                    var xistfileName = Ifile.FileName.Replace("%20", " ").Trim('\"');
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] fileBytes = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(fileBytes, 0, fileBytes.Length);

                    string fileSize = request.Form.Files[0].Length.ToString();

                    UploadContactFile documentToUpload = new UploadContactFile()
                    {
                        FileName = xistfileName,
                        DocumentBase64Data = base64String,
                        Size = fileSize,
                        FileType = Path.GetExtension(xistfileName),
                        CompanyUserLevel = request?.Form["TabType"],
                        IsOverWriteAllowed = request?.Form["IsOverWriteAllowed"],
                        UserProfileData = userProfile
                    };

                    var contactUploadRespResult = await _contactDetailRepository.ProcessContactFileUpload(documentToUpload);
                    return Results.Ok(contactUploadRespResult);
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
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at UploadContactsFile method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetContactDetailsByContactId(HttpRequest request, string contactId)
        {
            var responseMessage = new ResponseMessageForGetContact();
            loggerModelNew = new LoggerModelNew("", _module, "GetContactDetailsByContactId", "Process started for Get Contacts Details By ContactId using API.", "", "", "", "", "GetContactDetailsByContactId");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetContactDetailsByContactId";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                var contact = _contactDetailRepository.GetContactDetailsById(Convert.ToInt32(contactId));

                if (contact == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.NotFound;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = "Contact not found";

                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.BadRequest(responseMessage);
                }

                bool isValidUser = contact != null &&
                                   ((contact.CreatedUserId == userProfile?.UserID && contact?.CompanyUserLevel?.ToLower() == "user") ||
                                   (contact?.CompanyId == userProfile?.CompanyID && contact?.CompanyUserLevel?.ToLower() == "company" &&
                                    (userProfile?.UserTypeID == Constants.UserType.ADMIN || userProfile?.UserTypeID == Constants.UserType.SUPERUSER)));

                if (!isValidUser)
                {
                    responseMessage.StatusCode = HttpStatusCode.NotFound;
                    responseMessage.StatusMessage = "NotFound";
                    responseMessage.Message = "You are not allowed to edit the contact.";

                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    return Results.NotFound(responseMessage);
                }

                // Success response
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.ContactDetails = contact;
                responseMessage.Message = "Contact retrieved successfully";

                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                var loggerModelNew = new LoggerModelNew("", "Settings", "GetContactDetailsByContactId", "", contactId, "", "", "", "API");
                loggerModelNew.Message = ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);

                responseMessage.StatusCode = (HttpStatusCode)422;
                responseMessage.StatusMessage = "Error";
                responseMessage.Message = ex.Message;

                return Results.BadRequest(responseMessage);
            }
        }

        public async Task<IResult> GetContactsFileDetailsByUserId(HttpRequest request, string companyId, string userId, string tabType, int page = 1, int pageSize = 25)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetContactsFileDetailsByUserId", "Process started for Get Contacts Details By UserId.", "", "", "", "", "GetContactsFileDetailsByUserId");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ContactsFileResponeMessage responsecontactFilesMessage = new ContactsFileResponeMessage();

            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetContactsFileDetailsByUserId";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                FilterContactFileListforApi filterContactFileListforApi = new FilterContactFileListforApi();
                filterContactFileListforApi.Page = page;
                filterContactFileListforApi.PageSize = pageSize;
                filterContactFileListforApi.TotalContactsFileCount = 0;
                filterContactFileListforApi.TabType = tabType;
                filterContactFileListforApi.UserID = userProfile?.UserID;

                responsecontactFilesMessage = _contactDetailRepository.GetAllContactsFileDetailsByUserId((Guid)userProfile?.UserID, filterContactFileListforApi);

                responsecontactFilesMessage.StatusCode = HttpStatusCode.OK;
                responsecontactFilesMessage.StatusMessage = "OK";
                responsecontactFilesMessage.Message = $"Successfully retrieved Contacts file Details and count is: {responsecontactFilesMessage.TotalCount}";
                responsecontactFilesMessage.UploadedContactFiles = responsecontactFilesMessage.UploadedContactFiles;
                responsecontactFilesMessage.TotalCount = responsecontactFilesMessage.TotalCount;

                loggerModelNew.Message = responsecontactFilesMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Results.Ok(responsecontactFilesMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in Settings controller GetContactsDetailsByUserId action.";
                rSignLogger.RSignLogError(loggerModelNew, ex);

                responsecontactFilesMessage.StatusCode = (HttpStatusCode)422;
                responsecontactFilesMessage.StatusMessage = "Error";
                responsecontactFilesMessage.Message = ex.Message;

                return Results.BadRequest(responsecontactFilesMessage);
            }
        }

        public async Task<IResult> DeleteContact(HttpRequest request, string contactId)
        {
            var responseMessage = new ResponseMessageForDeleteContact();

            loggerModelNew = new LoggerModelNew("", _module, "DeleteContact", "Process started for Delete Contact using API.", "", "", "", "", "DeleteContact");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DeleteContact";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                var contact = _contactDetailRepository.GetContactDetailsById(Convert.ToInt32(contactId));

                bool isValidUser = contact != null && ((contact.CreatedUserId == userProfile?.UserID && contact?.CompanyUserLevel?.ToLower() == "user") ||
                                            (contact?.CompanyId == userProfile?.CompanyID && contact?.CompanyUserLevel?.ToLower() == "company" &&
                                             (userProfile?.UserTypeID == Constants.UserType.ADMIN || userProfile?.UserTypeID == Constants.UserType.SUPERUSER)));

                if (contact == null || !isValidUser)
                {
                    responseMessage.StatusCode = HttpStatusCode.NotFound;
                    responseMessage.StatusMessage = "NoContent";
                    responseMessage.Message = "Contact not found";
                    responseMessage.ContactId = contactId;

                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.NotFound(new
                    {
                        success = false,
                        message = "Contact not found",
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                bool deleteResult = await _contactDetailRepository.deleteContact(Convert.ToInt32(contactId));

                if (!deleteResult)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "Error";
                    responseMessage.Message = "Unable to delete contact";
                    responseMessage.ContactId = contactId;

                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = "Unable to delete contact",
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Contact Deleted Successfully";
                responseMessage.ContactId = contactId;

                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);

                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);

                responseMessage.StatusCode = (HttpStatusCode)422;
                responseMessage.StatusMessage = "Error";
                responseMessage.Message = ex.Message;

                return Results.BadRequest(responseMessage); ;
            }
        }

        public IResult SaveIntegrationDetails(HttpRequest request, SettingsExternalIntegration settingsToSave)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "SaveIntegrationDetails", "Saving the integration settings using API.", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessage responseMessage = new();

            try
            {
                if (string.IsNullOrWhiteSpace(settingsToSave.IntegrationType))
                    return Results.BadRequest(new { success = false, resultmessage = "Invalid integration type." });


                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SaveIntegrationDetails";
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    UserProfile userProfile = _userRepository.GetUserProfileByUserID(userToken.UserID);
                    var IsSaved = _settingsRepository.SaveExternalSettings(userToken.UserID, settingsToSave);
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "SaveIntegrationDetails";
                    loggerModelNew.Message = "Save External Settings successfull for integration Type " + settingsToSave?.IntegrationType;
                    rSignLogger.RSignLogInfo(loggerModelNew);
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.Message = "Settings saved successfully.";
                    return Results.Ok(responseMessage);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Module = _module;
                loggerModelNew.Method = "SaveIntegrationDetails";
                loggerModelNew.Message = "API EndPoint - Exception at SaveIntegrationDetails method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> GetAdminUsers(HttpRequest request, ManageUsersRequest model)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);

            loggerModelNew = new LoggerModelNew("", _module, "GetAdminUsers", "Endpoint Initialized,to Get admin users for the companyId :" + Convert.ToString(model.SearchCompany) + "User Email: " + model.SearchEmail, "", "", "", remoteIpAddress, "GetAdminUsers");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "GetAdminUsers";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                CommonAPIRequest<ManageUsersRequest> commonAPIRequest = new()
                {
                    UserID = userProfile.UserID,
                    EmailID = userProfile.EmailID!,
                    CompanyID = (Guid)(userProfile?.CompanyID!),
                    Data = model
                };
                CommonAPIResponse<ManageUsersResponse> response = await _manageAdminRepository.GetManageUsersAsync(commonAPIRequest);
                loggerModelNew.Method = "GetAdminUsers";
                loggerModelNew.Message = "GetAdminUsers ==> END. successfully returning the information.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at GetAdminUsers method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> UpdateUserTypeAndRole(HttpRequest request, ManageUsersUpdateRequest model)
        {
            string remoteIpAddress = UserTokenRepository.GetIPAddress(request);

            loggerModelNew = new LoggerModelNew("", _module, "UpdateUserTypeAndRole", "Endpoint Initialized,to update admin users role", "", "", "", remoteIpAddress, "UpdateUserTypeAndRole");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                UserProfile userProfile = new();
                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "UpdateUserTypeAndRole";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                CommonAPIRequest<ManageUsersUpdateRequest> commonAPIRequest = new()
                {
                    UserID = userProfile.UserID,
                    EmailID = userProfile.EmailID!,
                    CompanyID = (Guid)(userProfile?.CompanyID!),
                    Data = model
                };
                CommonAPIResponse<int> response = await _manageAdminRepository.ChangeUserType(commonAPIRequest);
                loggerModelNew.Method = "UpdateUserTypeAndRole";
                loggerModelNew.Message = "UpdateUserTypeAndRole ==> END. successfully update the information.";
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at UpdateUserTypeAndRole method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> DownloadBulkSendingCSV(HttpRequest request)
        {
            loggerModelNew = new LoggerModelNew("", _module, "DownloadBulkSendingCSV", "Download Bulk Sending CSV", "", "", "", "", "DownloadBulkSendingCSV");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                // Retrieve AuthToken from Headers
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "DownloadBulkSendingCSV";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                var settingsEnableSendingMessagesToMobile = _settingsRepository.GetEntityForByKeyConfig(userProfile.UserID, Constants.SettingsKeyConfig.EnableSendingMessagesToMobile);
                string filePath = (settingsEnableSendingMessagesToMobile?.OptionValue?.Trim().ToLower() ?? string.Empty) == "true" ? _appConfiguration["BulkUploadWithSettingY"].ToString() : _appConfiguration["BulkUpload"].ToString();

                if (!System.IO.File.Exists(filePath))
                {
                    loggerModelNew.Message = "Document not found";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = "Document not found",
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                string fileBase64 = Convert.ToBase64String(fileBytes);

                loggerModelNew.Message = "Successfully retrieved the document";
                rSignLogger.RSignLogInfo(loggerModelNew);

                var responseMessage = new ResponseMessageFile
                {
                    StatusCode = HttpStatusCode.OK,
                    StatusMessage = "OK",
                    Message = "Bulk Sending CSV downloaded successfully",
                    Base64FileData = fileBase64,
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    byteArray = fileBytes
                };

                return Results.File(responseMessage.byteArray, "application/octet-stream", responseMessage.FileName);
            }
            catch (Exception e)
            {
                loggerModelNew.Message = "Error occurred in settings controller Download Bulk Sending CSV action.";
                rSignLogger.RSignLogError(loggerModelNew, e);

                var responseMessage = new ResponseMessageFile
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusMessage = "Internal Server Error",
                    Message = e.Message
                };

                return Results.BadRequest();
            }
        }

        public IResult GetCompanyAppkeys(HttpRequest request)
        {
            loggerModelNew = new LoggerModelNew("", _module, "GetCompanyAppkeys", "Process started for getting company app names", "", "", "", "", "GetCompanyAppkeys");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                // Retrieve AuthToken from Headers
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                var companyAppNames = _userRepository.GetCompanyAppKeys();

                loggerModelNew.Message = "Successfully retrieved the company app names";
                rSignLogger.RSignLogInfo(loggerModelNew);

                var responseMessage = new ResponsePayload
                {
                    StatusCode = HttpStatusCode.OK,
                    StatusMessage = "Successfully returned Company Appkeys",
                    companyAppKeys = companyAppNames
                };

                return Results.Ok(responseMessage);
            }
            catch (Exception e)
            {
                loggerModelNew.Message = "Error occurred in settings end point GetCompanyAppkeys action.";
                rSignLogger.RSignLogError(loggerModelNew, e);

                var responseMessage = new ResponsePayload
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusMessage = "Internal Server Error",
                };

                return Results.BadRequest();
            }
        }

        public IResult SaveCompanyAppNames(HttpRequest request, string appKey)
        {
            loggerModelNew = new LoggerModelNew("", _module, "SaveCompanyAppNames", "Process started for save company app names", "", "", "", "", "SaveCompanyAppNames");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                // Retrieve AuthToken from Headers
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }
                appKey = appKey.EndsWith("_") ? appKey.Substring(0, appKey.Length - 1) : appKey;
                string AppSecretKey = appKey + "_" + _userRepository.GenerateAppKeyHash(appKey);
                bool status = _userRepository.SaveAppKey(appKey, AppSecretKey, false);

                loggerModelNew.Message = "Successfully saved the company app names";
                rSignLogger.RSignLogInfo(loggerModelNew);

                var responseMessage = new ResponseMessageForAddAppKey
                {
                    StatusCode = HttpStatusCode.OK,
                    StatusMessage = "Successfully saved Company Appkeys",
                    AppKey = appKey,
                    AppSecretKey = AppSecretKey
                };

                return Results.Ok(responseMessage);
            }
            catch (Exception e)
            {
                loggerModelNew.Message = "Error occurred in settings end point GetCompanyAppkeys action.";
                rSignLogger.RSignLogError(loggerModelNew, e);

                var responseMessage = new ResponseMessageForAddAppKey
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusMessage = "Internal Server Error",
                };

                return Results.BadRequest();
            }
        }

        private ImageFormat GetImageFormat(string ext)
        {
            return ext switch
            {
                ".png" => ImageFormat.Png,
                ".bmp" => ImageFormat.Bmp,
                _ => ImageFormat.Jpeg
            };
        }

        public async Task<IResult> UploadSignerStamp(HttpRequest request)
        {
            ResponseMessageStamp responseMessage = new ResponseMessageStamp();
            loggerModelNew = new LoggerModelNew("", _module, "UploadSignerStamp", "Upload Signer requested image using API", "", "", "", "", "GetCompanyAppkeys");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues iHeader);
                string? authToken = iHeader.ElementAt(0);
                if (!string.IsNullOrEmpty(authToken)) authToken = authToken.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(authToken))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                UserProfile userProfile = _userTokenRepository.GetUserProfileByToken(authToken);
                if (userProfile == null)
                {
                    loggerModelNew.Module = _module;
                    loggerModelNew.Method = "UploadSignerStamp";
                    loggerModelNew.Message = "User Profile is null";
                    rSignLogger.RSignLogWarn(loggerModelNew);

                    return Results.BadRequest(new
                    {
                        success = false,
                        message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]),
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                if (request.Form.Files.Count > 0)
                {
                    var file = request.Form.Files[0];
                    var fileName = Path.GetFileName(file.FileName);
                    var ext = Path.GetExtension(fileName).ToLowerInvariant();

                    if (!new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(ext))
                    {
                        return Results.BadRequest(new
                        {
                            success = false,
                            message = "Invalid file type.",
                            data = new List<ErrorTagDetailsResponse>()
                        });
                    }

                    byte[] fileBytes;
                    await using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }

                    string base64String;

                    await using (var imageStream = new MemoryStream(fileBytes))
                    using (var image = Image.FromStream(imageStream))
                    {
                        if (image.Width == 528 && image.Height == 113)
                        {
                            base64String = Convert.ToBase64String(fileBytes);
                        }
                        else
                        {
                            using var resizedBitmap = new Bitmap(image.Width, image.Height);
                            using (var graphics = Graphics.FromImage(resizedBitmap))
                            {
                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;

                                graphics.DrawImage(image, 0, 0, resizedBitmap.Width, resizedBitmap.Height);
                            }

                            await using var outputStream = new MemoryStream();
                            resizedBitmap.Save(outputStream, GetImageFormat(ext));
                            base64String = Convert.ToBase64String(outputStream.ToArray());
                        }
                    }

                    responseMessage.Base64FileData = base64String;
                    responseMessage.FileName = fileName;
                    responseMessage.IsStampUploaded = true;
                    responseMessage.StatusCode = HttpStatusCode.OK;
                    responseMessage.StatusMessage = "OK";
                    responseMessage.Message = "Stamp uploaded successfully";
                    //return Results.Ok(contactUploadRespResult);
                }
                else
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = "No file uploaded.",
                        data = new List<ErrorTagDetailsResponse>()
                    });
                }

                if (userProfile?.UserID != Guid.Empty)
                {
                    responseMessage.Stamps = _userRepository.GetUserStamps((Guid)userProfile?.UserID);
                }

                return Results.Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in settings end point UploadSignerStamp action.";
                rSignLogger.RSignLogError(loggerModelNew, ex);

                return Results.BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    data = new List<ErrorTagDetailsResponse>()
                });
            }
        }

    }
}
