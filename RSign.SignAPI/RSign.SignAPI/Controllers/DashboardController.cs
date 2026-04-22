using Microsoft.AspNetCore.Mvc;
using RSign.Common;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using System.Net;

namespace RSign.SignAPI.Controllers
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        RSignLogger rSignLogger = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration; 
        private readonly IEnvelopeRepository _envelopeRepository;        

        public DashboardController(IConfiguration appConfiguration,IEnvelopeRepository envelopeRepository)
        {          
            _appConfiguration = appConfiguration;           
            _envelopeRepository = envelopeRepository;
            rSignLogger = new RSignLogger(_appConfiguration);

        }
        [ProducesResponseType(typeof(CustomAPIResponse), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetMasterEnvelopeStats")]
        [HttpPost]
        public async Task<IActionResult> GetMasterEnvelopeStats(MasterEnvelopeStatsAPI signingEnvelope)
        {
            loggerModelNew = new LoggerModelNew("", "DashboardController", "GetMasterEnvelopeStats", "Process started for Get Master Envelope Stats using API", signingEnvelope.EnvelopeId.ToString(), "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            MasterEnvelopeStatsResonse responseMessage = new MasterEnvelopeStatsResonse();
            try
            {
                var additionalUploadInfoDetails = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByEnvelope(new Guid(signingEnvelope.EnvelopeId));
                var inboxSigningDetails = _envelopeRepository.GetSigningInbox(new Guid(signingEnvelope.EnvelopeId));
                var totalEnvelopes = inboxSigningDetails.Where(a => a.RecipientEmail == signingEnvelope.EmailId).Count();
                var totalSigned = inboxSigningDetails.Where(a => a.RecipientEmail == signingEnvelope.EmailId && a.IsSigned).Count();
                var customResponse = new
                {
                    PendingAttachRequests = additionalUploadInfoDetails.Where(a => a.IsRequired && string.IsNullOrEmpty(a.FileName)).Count(),
                    TotalEnvelopes = totalEnvelopes,
                    TotalSigned = totalSigned,
                    TotalPending = totalEnvelopes - totalSigned
                };
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";               
                responseMessage.Data = customResponse;               
                return Ok(responseMessage);
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
