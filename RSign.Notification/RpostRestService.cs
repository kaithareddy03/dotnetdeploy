using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Notification
{
    public class RpostRestService : IRpostRestService
    {
        RSignLogger rSignLogger = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration;
        public string RESTUri { get; set; }

        public RpostRestService(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
            RESTUri = Convert.ToString(_appConfiguration["REST_API_Url"]);
        }
        public HttpResponseMessage RestUserInfo(string userEmail, string token, string tokenType = "Bearer")
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(RESTUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", tokenType + " " + token);
            if (!string.IsNullOrEmpty(userEmail))
                responseMessage = client.GetAsync("api/v1/RSign/UserInfo?emailaddress=" + userEmail).Result;
            else
                responseMessage = client.GetAsync("api/v1/RSign/UserInfo").Result;
            var Msg = JsonConvert.DeserializeObject<dynamic>(responseMessage.Content.ReadAsStringAsync().Result);
            return responseMessage;
        }
        public HttpResponseMessage RollBackLockUnits(string envelopeId, string token, string tokenType = "Bearer")
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string ClientId = "405BDFE4-828B-4656-8795-11FCCA993622";
            PlanUnitsViewModel planUnitsViewModel = new PlanUnitsViewModel(ClientId, envelopeId);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(RESTUri);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", tokenType + " " + token);
            responseMessage = client.PostAsJsonAsync("api/v1/RSign/Envelope/Rollback", planUnitsViewModel).Result;
            var Msg = JsonConvert.DeserializeObject<dynamic>(responseMessage.Content.ReadAsStringAsync().Result);
            return responseMessage;
        }
        public HttpResponseMessage CommitStaticLinkUnit(string senderAddress, string envelopeId, int envelopeSize, int numberOfRecipients, string basicToken, string tokenType = "Bearer")
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string ClientId = "405BDFE4-828B-4656-8795-11FCCA993622";
            LockUnitsViewModel lockUnits = new LockUnitsViewModel(ClientId, senderAddress, envelopeId, envelopeSize, numberOfRecipients);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(RESTUri);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", tokenType + " " + basicToken);
            responseMessage = client.PostAsJsonAsync("api/v1/RSign/Envelope/DirectCommit", lockUnits).Result;
            var Msg = JsonConvert.DeserializeObject<dynamic>(responseMessage.Content.ReadAsStringAsync().Result);            
            return responseMessage;
        }
        public HttpResponseMessage ValidateBotClick(BotClickDataModal botClickDataModal)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(RESTUri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                responseMessage = httpClient.PostAsJsonAsync("api/v1/Validate/botClick", botClickDataModal).Result;
            }
            catch (Exception)
            {
                responseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;
                return responseMessage;
            }
            return responseMessage;
        }
    }
}
