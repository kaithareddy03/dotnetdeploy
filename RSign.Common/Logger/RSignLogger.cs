using Microsoft.Extensions.Configuration;
using NLog;
using RSign.Common.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Common
{
    public class RSignLogger 
    {
        private readonly IConfiguration _appConfiguration;
        NLog.Logger loggerInfo = LogManager.GetLogger("InfoLog");
        NLog.Logger loggerEml = LogManager.GetLogger("EmlLog");
        NLog.Logger loggerError = LogManager.GetLogger("ErrorLog");
        bool seperateLogError = false;
        public RSignLogger()
        {

        }
        public RSignLogger(IConfiguration configuration)
        {
            _appConfiguration = configuration;
            seperateLogError = Convert.ToBoolean(_appConfiguration["SeperateLogError"]);
        }
        
        public void RSignLogInfo(LoggerModelNew model)
        {
            using (NestedDiagnosticsContext.Push(Environment.MachineName + " :: " + model.UserId + " :: " + model.Email + " :: " + model.AuthRefKey + " :: " + model.EnvelopeId + " :: " + model.Module + " :: " + model.Method + " :: " + model.IPAddress + " :: " + model.SourceType))
            {
                loggerInfo.Info(model.Message);
            }
        }
       
        public void RSignLogWarn(LoggerModelNew model)
        {
            using (NestedDiagnosticsContext.Push(Environment.MachineName + " :: " + model.UserId + " :: " + model.Email + " :: " + model.AuthRefKey + " :: " + model.EnvelopeId + " :: " + model.Module + " :: " + model.Method + " :: " + model.IPAddress + " :: " + model.SourceType))
            {
                loggerInfo.Warn(model.Message);
            }
        }
        public void RSignLogError(LoggerModelNew model)
        {
            using (NestedDiagnosticsContext.Push(Environment.MachineName + " :: " + model.UserId + " :: " + model.Email + " :: " + model.AuthRefKey + " :: " + model.EnvelopeId + " :: " + model.Module + " :: " + model.Method + " :: " + model.IPAddress + " :: " + model.SourceType))
            {
                loggerInfo.Error(model.Message);
            }
        }
        public void RSignLogError(LoggerModelNew model, Exception ex, bool isToSendAnEmail = true)
        {
            using (NestedDiagnosticsContext.Push(Environment.MachineName + " :: " + model.UserId + " :: " + model.Email + " :: " + model.AuthRefKey + " :: " + model.EnvelopeId + " :: " + model.Module + " :: " + model.Method + " :: " + model.IPAddress + " :: " + model.SourceType))
            {
                loggerInfo.Error(model.Message);
                if (seperateLogError)
                    loggerError.Error(ex);
                if (isToSendAnEmail)
                    RSignMail(model, ex);
            }            
        }
        public void RSignLogEmlNew(LoggerModelNew model)
        {
            using (NestedDiagnosticsContext.Push(Environment.MachineName + " :: " + model.UserId + " :: " + model.Email + " :: " + model.AuthRefKey + " :: " + model.EnvelopeId + " :: " + model.Module + " :: " + model.Method + " :: " + model.IPAddress + " :: " + model.SourceType))
            {
                loggerEml.Info(model.Message);
            }
        }

        public void RSignMail(LoggerModelNew model, Exception ex)
        {
            string imageLogoURl = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["Images"]));
            imageLogoURl = System.IO.Path.Combine(imageLogoURl, "RMail-100.png");

            var toEmailAddress = Convert.ToString(_appConfiguration["SystemEmailAddress"].ToString());
            var fromEmailAddress = Convert.ToString(_appConfiguration["FromEmailAddressSupport"].ToString());
            string notificationEmailFrom = Convert.ToString(_appConfiguration["NotificationEmailFrom"].ToString());
            string mailTemplate = string.Empty;
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
            var errorMessage = Environment.MachineName + " :: " + model.IPAddress + " :: " + model.AuthRefKey + " :: " + model.Email + " :: " + model.Module + " :: " + model.Method + " :: " + model.EnvelopeId + "<br/>" + model.Message + "<br/>" + ex.StackTrace;

            mailTemplate = "<div style='color:#000'><table border='0' width='100%' cellspacing='0' cellpadding='5'>";
            mailTemplate += "<tr><td bgcolor='#AC2329' width='80'><img border='0' src='#ImageURL' height='45' align='left'></td>";
            mailTemplate += "<td bgcolor='#AC2329'><p style='margin-top: 0; margin-bottom: 0; font-size:20pt;'><b><font face='calibri,sans-serif' color='#FFFFFF'>Error Notification</font></b></p></td></tr></table>";
            mailTemplate += "<p style='margin-top: 0; margin-bottom: 0'>&nbsp;</p><p style='margin-top: 0; margin-bottom: 0'><span style='font-family: calibri,sans-serif;font-size:11pt;color: #000;'>#Admin, </span></p>";
            mailTemplate += "<p style='margin-top: 0; margin-bottom: 0'>&nbsp;</p><p style='margin-top: 0; margin-bottom: 0'><span style='font-family: calibri,sans-serif; font-size:11pt;'><font color='#000'>#header</font></span></p>";
            mailTemplate += "<p style='margin-top: 0; margin-bottom: 0'>&nbsp;</p><span>#ErrorMessage</span><p style='margin-top: 0; margin-bottom: 0'>&nbsp;</p><p style='margin-top: 0; margin-bottom: 0; font-size:11pt;'><font face='calibri,sans-serif' color='#000'>Thank you.</font></p>";
            mailTemplate += "<p style='margin-top: 0; margin-bottom: 0; font-size:11pt;'><font face='calibri,sans-serif' color='#000'>An RPost Service</font></p><p style='margin-top: 0; margin-bottom: 0'>&nbsp;</p><table border='0' width='100%' cellspacing='0' cellpadding='0'>";
            mailTemplate += "<tbody><tr><td><p align='left' style='margin-top: 0; margin-bottom: 0; font-size:11pt;'><font face='calibri,sans-serif' color='#666666'>An RPost service</font></p></td><td></td></tr></tbody></table></div>";

            mailTemplate = mailTemplate.Replace("#ImageURL", imageLogoURl);
            mailTemplate = mailTemplate.Replace("#Admin", "Hi");
            mailTemplate = mailTemplate.Replace("#header", "Please Check the following Error");
            mailTemplate = mailTemplate.Replace("#ErrorMessage", errorMessage);

            var Chilkat = new ChilkatHelper(_appConfiguration);
            Chilkat.SendMailUsingChilKet(new string[] { toEmailAddress }, new string[] { "" }, null, null, null, null, fromEmailAddress, null, "Error Notification - " + notificationEmailFrom, mailTemplate, null, null, model.EnvelopeId, model.Method);
        }
    }

    public class LoggerModelNew
    {
        public LoggerModelNew()
        {
        }
        public LoggerModelNew(string Email, string Module, string Method, string Message, string EnvelopeId, string AuthRefKey = "",
            string UserId = "", string IPAddress = "", string SourceType = "")
        {
            this.Email = Email;
            this.AuthRefKey = AuthRefKey;
            this.Method = Method;
            this.Module = Module;
            this.Message = Message;
            this.EnvelopeId = EnvelopeId;
            this.UserId = UserId;
            this.IPAddress = IPAddress;
            this.SourceType = SourceType;
        }
        public string? Email { get; set; }
        public string? Method { get; set; }
        public string? Module { get; set; }
        public string? Message { get; set; }
        public string? EnvelopeId { get; set; }
        public string? AuthRefKey { get; set; }
        public string? UserId { get; set; }
        public string? IPAddress { get; set; }
        public string? SourceType { get; set; }
    }
}
