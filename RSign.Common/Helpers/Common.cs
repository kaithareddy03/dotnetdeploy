using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Common.Helpers
{
    public class Common
    {
        public static string GetCultureInfoValue(string CultureInfo)
        {
            string updatedCultureInfo = CultureInfo;
            switch (CultureInfo.ToLower())
            {
                case Constants.Language.NorwegianConst:
                    updatedCultureInfo = "no";
                    break;
                //case Constants.Language.SpanishColombiaConst:
                //    updatedCultureInfo = "es-co";
                //    break;
                case Constants.Language.SpanishConst:
                    updatedCultureInfo = "es";
                    break;
                default:
                    updatedCultureInfo = CultureInfo;
                    break;
            }
            return updatedCultureInfo;
        }
    }
    public class BotClickDataModal
    {
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public string Network { get; set; }
        public string Domain { get; set; }
    }
    public class SendMobileSMSModel
    {
        public string referenceId { get; set; }
        public string to { get; set; }
        public string message { get; set; }
    }

    public class AccessTokenInfoModel
    {
        public string token { get; set; }
        public string tokenValidFrom { get; set; }
        public string tokenValidTo { get; set; }
        public string status { get; set; }
        public HttpStatusCode statusCode { get; set; }
    }
    public class AccessTokenModel
    {
        public AccessTokenInfoModel accessToken { get; set; }
    }
}
