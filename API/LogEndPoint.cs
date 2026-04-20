using RSign.Common;
using RSign.Models;
using RSign.Models.Interfaces;
using RSign.Models.Repository;

namespace RSign.SendAPI.API
{
    public class LogEndPoint
    {
        private readonly RSignLogger rSignLogger = new();
        private LoggerModelNew loggerModelNew = new();
        private readonly IConfiguration _appConfiguration;
        private IHttpContextAccessor _accessor;
        private IUserTokenRepository _userTokenRepository;
        private readonly string _module = "LogEndPoint";

        public LogEndPoint(IHttpContextAccessor accessor, IConfiguration appConfiguration, IUserTokenRepository userTokenRepository)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _userTokenRepository = userTokenRepository;
            rSignLogger = new RSignLogger(_appConfiguration);
        }
        public void RegisterLogAPI(WebApplication app)
        {
            app.MapPost("/api/v1/Log/Insertlogs", Insertlogs);
        }
        /// <summary>
        /// This method is used for storing the logs based on user email address
        /// </summary>
        /// <param name="request"></param>
        /// <param name="lstEntry"></param>
        /// <returns></returns>

        public async Task<IResult> Insertlogs(HttpRequest request, List<LogEntry> lstEntry)
        {
            var remoteIpAddress = UserTokenRepository.GetIPAddress(request);
            loggerModelNew = new LoggerModelNew("", _module, "GetEnvelopeDetails", "Process started for storing the user logs", "", "", "", remoteIpAddress, "SendAPI");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                if (lstEntry != null && lstEntry.Count() > 0)
                {
                    var directoryPath = _appConfiguration["RSignSenderLogDirectory"];
                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        var path = Path.Combine(directoryPath, "RSignSenderlog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        if (!Directory.Exists(directoryPath))
                            System.IO.Directory.CreateDirectory(directoryPath);

                        using (StreamWriter writer = new StreamWriter(path, append: true))
                        {
                            foreach (var item in lstEntry)
                            {
                                var logText = item.timestamp.Replace('T', ' ') + " :: " + item.level + " :: " + Environment.MachineName + " :: " + item.module + " :: " + item.method + " :: " + item.email + " :: " + item.envelopeId + " :: " + remoteIpAddress + " :: " + item.message + Environment.NewLine;
                                writer.WriteLine(logText);
                            }
                        }
                    }
                }
                return Results.Ok();
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at Insertlogs method and error message is:" + ex.ToString();
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
