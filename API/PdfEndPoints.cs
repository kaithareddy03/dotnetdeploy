using RSign.Common;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;

namespace RSign.SendAPI.API
{
    public class PdfEndPoints
    {
        RSignLogger rsignlog = new();

        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration;   
        private readonly IEnvelopeHelperMain _IenvelopeHelperMain;
        Envelope envelope = new Envelope();
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly IModelHelper _modelHelper;
        private readonly IESignHelper _eSignHelper;
        private IUserTokenRepository _userTokenRepository;
        private IHttpContextAccessor _accessor;
        private readonly string _module = "ViewPdfEndpoint";
        private readonly IPdfRepository _pdfRepository;

        public PdfEndPoints(IConfiguration appConfiguration, IEnvelopeRepository envelopeRepository, IEnvelopeHelperMain envelopeHelperMain, IModelHelper modelHelper, IESignHelper eSignHelper, IUserTokenRepository userTokenRepository,IPdfRepository pdfRepository)
        {
            _appConfiguration = appConfiguration;
            _envelopeRepository = envelopeRepository;
            _IenvelopeHelperMain = envelopeHelperMain;
            _modelHelper = modelHelper;
            _eSignHelper = eSignHelper;
            _userTokenRepository = userTokenRepository;
            rsignlog = new RSignLogger(_appConfiguration);
            _pdfRepository = pdfRepository;
        }
        public void PdfApis(WebApplication app)
        {
            app.MapPost("/api/v1/Pdf/ViewPdf", ViewPdf);
            app.MapPost("/api/v1/Pdf/ViewTemplatePdf", ViewTemplatePdf);
        }
        public async Task<IResult> ViewPdf(HttpRequest request, EnvelopeControls prepareEnvelope)
        {
            loggerModelNew = new LoggerModelNew("", _module, "ViewPdf", "Process started for Get Envelope Details By either Envelope Id or Envelope Code", Convert.ToString(prepareEnvelope.EnvelopeID));
            rsignlog.RSignLogInfo(loggerModelNew);
            HttpResponseMessage responseToClient = new();

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rsignlog.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    ResponseMessageGetEnvelopeDetails responseMessage = new ResponseMessageGetEnvelopeDetails();
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;

                    //Shared Envelope Access
                    string isActingUserPerformingAction = "false";
                    if (!string.IsNullOrEmpty(prepareEnvelope.IsActingUserPerformingAction) && prepareEnvelope.IsActingUserPerformingAction.ToLower() == "true")
                    {
                        isActingUserPerformingAction = "true";
                        userToken.UserID = prepareEnvelope.EnvelopeSenderUserID;
                        userToken.EmailId = prepareEnvelope.EnvelopeSenderUserEmail;
                    }
                    //Shared Envelope Access

                    var viewPdfResponse = await _pdfRepository.ViewPdf(request, userToken, prepareEnvelope, userToken.EmailId);
                    return Results.Ok(viewPdfResponse);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at ViewPdf method and error message is:" + ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }

        public async Task<IResult> ViewTemplatePdf(HttpRequest request, EnvelopeControls prepareEnvelope)
        {
            loggerModelNew = new LoggerModelNew("", _module, "ViewTemplatePdf", "Process started for Get Template Details By templateID", Convert.ToString(prepareEnvelope.EnvelopeID));
            rsignlog.RSignLogInfo(loggerModelNew);
            HttpResponseMessage responseToClient = new();
            Template template = new Template();

            try
            {
                UserToken userToken = _userTokenRepository.ValidateToken(request);
                if (userToken == null)
                {
                    loggerModelNew.Message = Convert.ToString(_appConfiguration["UnauthorizedAccess"]);
                    rsignlog.RSignLogWarn(loggerModelNew);
                    return Results.Unauthorized();
                }
                else
                {
                    ResponseMessageGetEnvelopeDetails responseMessage = new ResponseMessageGetEnvelopeDetails();
                    loggerModelNew.AuthRefKey = userToken.ReferenceKey;
                    loggerModelNew.Email = userToken.EmailId;
                    loggerModelNew.Message = "Getting user profile details by email=" + userToken.EmailId;

                    var viewPdfResponse = _pdfRepository.GetTemplateViewPdfDocument(userToken.EmailId, prepareEnvelope, prepareEnvelope.EnvelopeID, userToken.UserID);
                    return Results.Ok(viewPdfResponse);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "API EndPoint - Exception at ViewTemplatePdf method and error message is:" + ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
