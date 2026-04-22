using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSign.Common;
using RSign.Models.APIModels;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using System.Net;

namespace RSign.SignAPI.Controllers
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        RSignLogger rSignLogger = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration;
        private IHttpContextAccessor _accessor;
        private readonly IModelHelper _modelHelper;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;

        public TemplateController(IHttpContextAccessor accessor, IConfiguration appConfiguration, IModelHelper modelHelper, IEnvelopeHelperMain envelopeHelperMain)
        {
            _accessor = accessor;
            _appConfiguration = appConfiguration;
            _modelHelper = modelHelper;
            _envelopeHelperMain = envelopeHelperMain;
            rSignLogger = new RSignLogger(_appConfiguration);
        }

        [ProducesResponseType(typeof(HttpResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetImages/{id}/{envelopeId}/{uncPath}")]
        [HttpGet]
        public async Task<IActionResult> GetImages(string id, string envelopeID, string uncPath)
        {
            loggerModelNew = new LoggerModelNew("", "TemplateController", "GetImages", "Process started for Get Image path by envelopeId using API", envelopeID.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                string uncPathName = _modelHelper.GetTemplateDirectoryName(uncPath);               
                byte[] imgData = uncPathName != "0" ? System.IO.File.ReadAllBytes(_envelopeHelperMain.GetImagTemplatePath(Convert.ToInt32(id), new Guid(envelopeID), uncPathName).ToString()) : System.IO.File.ReadAllBytes(_envelopeHelperMain.GetImagTemplate(Convert.ToInt32(id), new Guid(envelopeID)).ToString());

                MemoryStream ms = new MemoryStream(imgData);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                response.Content.Headers.ContentLength = ms.Length;
                return File(imgData, "image/png");
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return BadRequest();
            }
        }
    }
}
