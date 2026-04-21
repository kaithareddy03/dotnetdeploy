using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RSign.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RSign.Common.Helpers;

namespace RSign.Notification
{
    public class MobileSMSService:IMobileSMSService
    {
        //RSignLogger rSignLogger = new RSignLogger();
        //LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration;
        public string MobileSMSApiUrl { get; set; }

        public MobileSMSService(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
            MobileSMSApiUrl = Convert.ToString(_appConfiguration["MobileSMSApiUrl"]);
        }
        public HttpResponseMessage AuthticateMobileService()
        {
            LoggerModelNew loggerModelNew = new LoggerModelNew("", "MobileSMSService", "AuthticateMobileService", "Process started for authenticating mobile service", "", "", "", "", "API");
            RSignLogger rSignLogger = new RSignLogger();
            rSignLogger.RSignLogInfo(loggerModelNew);
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(MobileSMSApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            ValidateSMSTokenModel validateSMSTokenModel = new ValidateSMSTokenModel();
            validateSMSTokenModel.appID = Convert.ToString(_appConfiguration["MobileSMSAppID"]);
            validateSMSTokenModel.appKey = Convert.ToString(_appConfiguration["MobileSMSAppKeyID"]);
            validateSMSTokenModel.adminEmail = Convert.ToString(_appConfiguration["MobileSMSAdminEmail"]);

            try
            {
                loggerModelNew.Message = "MobileSMSApiUrl is: " + MobileSMSApiUrl + " and payload is: " + JsonConvert.SerializeObject(validateSMSTokenModel);
                rSignLogger.RSignLogInfo(loggerModelNew);

                responseMessage = httpClient.PostAsJsonAsync("api/v1/auth/token", validateSMSTokenModel).Result;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "MobileSMSService - there is a issue for validating AuthticateMobileService and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseMessage.ReasonPhrase = ex.ToString();
                return responseMessage;
            }
            return responseMessage;
        }
        public HttpResponseMessage SendMobileSMS(List<SendMobileSMSModel> sendMobileSMSModelData, string token)
        {
            LoggerModelNew loggerModelNew = new LoggerModelNew("", "MobileSMSService", "SendMobileSMS", "Process started for sending sms to recipient mobile: ", "", "", "", "", "API");
            RSignLogger rSignLogger = new RSignLogger();
            rSignLogger.RSignLogInfo(loggerModelNew);
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(MobileSMSApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            try
            {
                responseMessage = httpClient.PostAsJsonAsync("api/v1/message/send", sendMobileSMSModelData).Result;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "MobileSMSService - there is a issue for sending sms in SendMobileSMS method and error message is: " + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseMessage.ReasonPhrase = ex.ToString();
                return responseMessage;
            }
            return responseMessage;
        }
    }
    public class ValidateSMSTokenModel
    {
        public string appID { get; set; }
        public string appKey { get; set; }
        public string adminEmail { get; set; }
    }
}
