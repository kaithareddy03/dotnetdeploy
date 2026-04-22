using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSign.Common;
using RSign.ManageDocument.Interfaces;
using RSign.Models.APIModels;
using RSign.Models.Helpers;
using RSign.Models;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System.Net;
using System.Text;
using Aspose.Pdf.Operators;
using Microsoft.AspNetCore.WebUtilities;
using RSign.Web.Models;
using RSign.Common.Helpers;
using GoogleDriveDownload;
using Microsoft.Net.Http.Headers;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using Chilkat;
using iText.Layout.Element;
using StringBuilder = System.Text.StringBuilder;
using Microsoft.AspNetCore.Authorization;
using Org.BouncyCastle.Asn1.X509;

namespace RSign.SignAPI.Controllers
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        RSignLogger rSignLogger = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly IRecipientRepository _recipientRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IEnvelopeHelperMain _envelopeHelperMain;
        private readonly IModelHelper _modelHelper;
        private readonly IESignHelper _eSignHelper;
        private readonly IUserTokenRepository _userTokenRepository;
        private GoogleDrive gDrive = new GoogleDrive();
        private readonly IGenericRepository _genericRepository;

        public DocumentController(IConfiguration appConfiguration, IEnvelopeRepository envelopeRepository, IRecipientRepository recipientRepository, ISettingsRepository settingsRepository,
            IEnvelopeHelperMain envelopeHelperMain, IModelHelper modelHelper, IESignHelper eSignHelper, IUserTokenRepository userTokenRepository)
        {
            _appConfiguration = appConfiguration;
            _envelopeRepository = envelopeRepository;
            _recipientRepository = recipientRepository;
            _settingsRepository = settingsRepository;
            _envelopeHelperMain = envelopeHelperMain;
            _modelHelper = modelHelper;
            _eSignHelper = eSignHelper;
            _userTokenRepository = userTokenRepository;
        }

        [ProducesResponseType(typeof(ResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("SaveAttachmentRequest")]
        [HttpPost]
        public IActionResult SaveAttachmentRequest(List<EnvelopeAdditionalUploadInfo> SaveDocumentRequest)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "DocumentController", "SaveAttachmentRequest", "Process started for Save Attachment Request", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                List<string> lstFiles = new List<string>();
                Guid EnvelopeId = new Guid(), RecipientId = new Guid();
                foreach (var item in SaveDocumentRequest)
                {
                    EnvelopeId = (Guid)item.MasterEnvelopeID;
                    RecipientId = (Guid)item.RecipientID;
                    int uploadAttachmentID = Convert.ToInt32(item.ID);
                    EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                    envelopeAdditionalUploadInfo = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByID(uploadAttachmentID, (Guid)item.RecipientID);
                    string fileName = string.Empty;
                    if (envelopeAdditionalUploadInfo != null)
                    {
                        envelopeAdditionalUploadInfo.Name = item.Name;
                        envelopeAdditionalUploadInfo.Description = item.Description;
                        envelopeAdditionalUploadInfo.AdditionalInfo = item.AdditionalInfo;
                        envelopeAdditionalUploadInfo.FileName = item.FileName;
                        envelopeAdditionalUploadInfo.OriginalFileName = item.OriginalFileName;
                        bool isUpdated = _envelopeRepository.UpdateEnvelopeAdditionalUploadInfo(envelopeAdditionalUploadInfo);
                    }
                }
                string dirPath = _modelHelper.GetEnvelopeDirectoryNew(EnvelopeId, string.Empty);
                lstFiles = _envelopeHelperMain.GetSignerDocFromDirectory(EnvelopeId, RecipientId, dirPath);
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.data = lstFiles;
                responseMessage.Message = "Document info saved successfully";
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller SaveAttachmentRequest action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DeleteRequestedUploads")]
        [HttpPost]
        public IActionResult DeleteRequestedUploads(DeleteDocumentRequest deleteDocumentRequest)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "DocumentController", "DeleteRequestedUploads", "Process started for Delete requested documents using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                List<string> lstFiles = new List<string>();
                int uploadAttachmentID = Convert.ToInt32(deleteDocumentRequest.UploadAttachmentId);
                EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                envelopeAdditionalUploadInfo = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByID(uploadAttachmentID, deleteDocumentRequest.RecipientID);
                string fileName = string.Empty;

                if (envelopeAdditionalUploadInfo != null)
                {
                    Guid tempRecipentID = Guid.Empty;
                    if (!string.IsNullOrEmpty(deleteDocumentRequest.TempRecipientID))
                    {
                        tempRecipentID = new Guid(deleteDocumentRequest.TempRecipientID);
                    }
                    else
                    {
                        tempRecipentID = deleteDocumentRequest.RecipientID;
                    }
                    
                    var dirPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeAdditionalUploadInfo.MasterEnvelopeID.Value, string.Empty) + envelopeAdditionalUploadInfo.MasterEnvelopeID.Value;
                    var fileNameWithPath = dirPath + "\\SignerAttachments\\" + tempRecipentID + "\\" + envelopeAdditionalUploadInfo.FileName;


                    if (System.IO.File.Exists(fileNameWithPath))
                    {
                        double length = new System.IO.FileInfo(fileNameWithPath).Length;
                        System.IO.File.Delete(fileNameWithPath);
                    }
                    else
                    {
                        fileNameWithPath = dirPath + "\\SignerAttachments\\" + Guid.Empty + "\\" + envelopeAdditionalUploadInfo.FileName;
                        if (System.IO.File.Exists(fileNameWithPath))
                        {
                            double length = new System.IO.FileInfo(fileNameWithPath).Length;
                            System.IO.File.Delete(fileNameWithPath);
                        }
                    }
                    envelopeAdditionalUploadInfo.FileName = null;
                    envelopeAdditionalUploadInfo.OriginalFileName = null;
                    bool isUpdated = _envelopeRepository.UpdateEnvelopeAdditionalUploadInfo(envelopeAdditionalUploadInfo);
                    lstFiles = _envelopeHelperMain.GetSignerDocFromDirectory(deleteDocumentRequest.EnvelopeID, tempRecipentID, dirPath);
                }
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.data = lstFiles;
                responseMessage.Message = "Document Deleted successfully";
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in SignDocumentController controller GetDownloadFileReview action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DeleteNewlyAddedUpload")]
        [HttpPost]
        public IActionResult DeleteNewlyAddedUpload(DeleteDocumentRequest deleteDocumentRequest)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "DocumentController", "DeleteNewlyAddedUpload", "Process started for Delete requested documents using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                List<string> lstFiles = new List<string>();
                EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                envelopeAdditionalUploadInfo = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByID(deleteDocumentRequest.UploadAttachmentId.Value, deleteDocumentRequest.RecipientID);
                string fileName = string.Empty;
                Guid tempRecipentID = Guid.Empty;
                if (!string.IsNullOrEmpty(deleteDocumentRequest.TempRecipientID) && deleteDocumentRequest.TempRecipientID != "null")
                {
                    tempRecipentID = new Guid(deleteDocumentRequest.TempRecipientID);
                }
                else
                {
                    tempRecipentID = deleteDocumentRequest.RecipientID;
                }

                string tempDirPath = _modelHelper.GetEnvelopeDirectoryNew(deleteDocumentRequest.EnvelopeID, string.Empty);
                if (envelopeAdditionalUploadInfo != null)
                {
                    var dirPath = tempDirPath + envelopeAdditionalUploadInfo.MasterEnvelopeID.Value;
                    var fileNameWithPath = dirPath + "\\SignerAttachments\\" + tempRecipentID + "\\" + envelopeAdditionalUploadInfo.FileName;

                    if (System.IO.File.Exists(fileNameWithPath))
                    {
                        double length = new System.IO.FileInfo(fileNameWithPath).Length;
                        System.IO.File.Delete(fileNameWithPath);
                    }
                    else
                    {
                        if (System.IO.File.Exists(dirPath + "\\SignerAttachments\\" + deleteDocumentRequest.RecipientID + "\\" + envelopeAdditionalUploadInfo.FileName))
                        {
                            double length = new System.IO.FileInfo(dirPath + "\\SignerAttachments\\" + deleteDocumentRequest.RecipientID + "\\" + envelopeAdditionalUploadInfo.FileName).Length;
                            System.IO.File.Delete(dirPath + "\\SignerAttachments\\" + deleteDocumentRequest.RecipientID + "\\" + envelopeAdditionalUploadInfo.FileName);
                        }
                    }
                    bool isDeleted = _envelopeRepository.DeleteEnvelopeAdditionalUploadInfo(deleteDocumentRequest.UploadAttachmentId.Value);
                    tempRecipentID = tempRecipentID == Guid.Empty ? deleteDocumentRequest.RecipientID : tempRecipentID;
                }
                lstFiles = _envelopeHelperMain.GetSignerDocFromDirectory(deleteDocumentRequest.EnvelopeID, tempRecipentID, tempDirPath);
                responseMessage.data = lstFiles;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "newly added EnvelopeAdditionalUploadInfo Deleted successfully";
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller DeleteRequestedUploads action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DeleteAndUpdateSignerDocFromDirectory")]
        [HttpPost]
        //Delete And Update Signer Doc From Directory -- WHEN CLICKED ON SAVE ON CONFIRMATION POPUP
        public IActionResult DeleteAndUpdateSignerDocFromDirectory(List<AddUpdatedAttachmentDetails> AddUpdatedAttachmentRequest)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "DocumentController", "DeleteAndUpdateSignerDocFromDirectory", "Process started for Delete requested documents using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                List<string> lstFiles = new List<string>();
                bool isUpdated = false;
                Guid EnvelopeID = Guid.Empty, RecipientID = Guid.Empty;
                string tempDirPath = string.Empty;
                foreach (var req in AddUpdatedAttachmentRequest)
                {
                    EnvelopeAdditionalUploadInfo AttachmentRow = _envelopeRepository.GetEnvelopeUploadInfoByID(Convert.ToInt64(req.AttachmentId));
                    if (AttachmentRow != null)
                    {
                        EnvelopeID = (Guid)AttachmentRow.MasterEnvelopeID;
                        RecipientID = (Guid)req.RecipientId;
                        tempDirPath = _modelHelper.GetEnvelopeDirectoryNew((Guid)AttachmentRow.MasterEnvelopeID, string.Empty);
                        var dirPath = tempDirPath + AttachmentRow.MasterEnvelopeID;
                        if (req.AddUpdate == "A")
                        {
                            var fileNameWithPath = dirPath + "\\SignerAttachments\\" + RecipientID + "\\" + AttachmentRow.FileName;
                            if (System.IO.File.Exists(fileNameWithPath))
                            {
                                System.IO.File.Delete(fileNameWithPath);
                            }
                            else
                            {
                                fileNameWithPath = dirPath + "\\SignerAttachments\\" + Guid.Empty + "\\" + AttachmentRow.FileName;
                                if (System.IO.File.Exists(fileNameWithPath))
                                    System.IO.File.Delete(fileNameWithPath);
                            }
                            if (!AttachmentRow.IsRequired)
                                isUpdated = _envelopeRepository.DeleteEnvelopeAdditionalUploadInfo(Convert.ToInt32(AttachmentRow.ID));
                            else
                            {
                                if (AttachmentRow != null)
                                {
                                    AttachmentRow.FileName = null;
                                    AttachmentRow.OriginalFileName = null;
                                    isUpdated = _envelopeRepository.UpdateEnvelopeAdditionalUploadInfo(AttachmentRow);
                                }
                            }
                        }
                        else
                        {
                            var TempDirectory = dirPath + "\\TempSignerAttachments" + "\\" + AttachmentRow.RecipientID;
                            var EnvelopDirectory = dirPath + "\\SignerAttachments" + "\\" + AttachmentRow.RecipientID;
                            string sourceFile = Path.Combine(TempDirectory, req.FileName);
                            string destFile = Path.Combine(EnvelopDirectory, req.FileName);
                            bool fileAvailable = false;
                            if (!string.IsNullOrEmpty(req.FileName))
                            {
                                if (System.IO.File.Exists(Path.Combine(TempDirectory, req.FileName)))
                                {
                                    System.IO.File.Copy(sourceFile, destFile, true);
                                    System.IO.File.Delete(sourceFile);
                                    fileAvailable = true;
                                }
                                else
                                {
                                    TempDirectory = dirPath + "\\TempSignerAttachments\\" + Guid.Empty;
                                    EnvelopDirectory = dirPath + "\\SignerAttachments\\" + Guid.Empty;
                                    destFile = Path.Combine(EnvelopDirectory, req.FileName);
                                    sourceFile = Path.Combine(TempDirectory, req.FileName);
                                    if (System.IO.File.Exists(destFile))
                                    {
                                        System.IO.File.Copy(sourceFile, destFile, true);
                                        System.IO.File.Delete(sourceFile);
                                    }
                                }
                                if (!string.IsNullOrEmpty(AttachmentRow.FileName))
                                {
                                    if (System.IO.File.Exists(Path.Combine(dirPath + "\\SignerAttachments\\" + AttachmentRow.RecipientID, AttachmentRow.FileName)))
                                    {
                                        System.IO.File.Delete(Path.Combine(dirPath + "\\SignerAttachments\\" + AttachmentRow.RecipientID, AttachmentRow.FileName));
                                    }
                                }
                                if (AttachmentRow != null)
                                {
                                    AttachmentRow.FileName = fileAvailable ? req.FileName : null;
                                    AttachmentRow.OriginalFileName = fileAvailable ? req.OriginalFileName : null;
                                    isUpdated = _envelopeRepository.UpdateEnvelopeAdditionalUploadInfo(AttachmentRow);
                                }
                            }
                        }
                    }
                }
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Document Deleted successfully";
                lstFiles = _envelopeHelperMain.GetSignerDocFromDirectory(EnvelopeID, RecipientID, tempDirPath);
                responseMessage.data = lstFiles;
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller DeleteAndUpdateSignerDocFromDirectory action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessage), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DeleteSignerDocFromDirectory")]
        [HttpPost]
        //Delete Signer Doc From Directory- From Left section
        public IActionResult DeleteSignerDocFromDirectory(DeleteDocumentRequest deleteDocumentRequest)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            loggerModelNew = new LoggerModelNew("", "DocumentController", "DeleteSignerDocFromDirectory", "Process started for Delete requested documents using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                List<TemplateGroupDocumentUploadDetails> additionalDocument = _envelopeRepository.GetEnvelopeAdditionalDocument(deleteDocumentRequest.EnvelopeID);
                foreach (var item in additionalDocument)
                {
                    if (item.FileName == deleteDocumentRequest.FileName)
                    {
                        var dirPath = _modelHelper.GetEnvelopeDirectoryNew(deleteDocumentRequest.EnvelopeID, string.Empty) + deleteDocumentRequest.EnvelopeID;
                        var fileNameWithPath = dirPath + "\\SignerAttachments\\" + item.RecipientId + "\\" + deleteDocumentRequest.FileName;
                        if (System.IO.File.Exists(fileNameWithPath))
                        {
                            System.IO.File.Delete(fileNameWithPath);
                        }
                        else
                        {
                            fileNameWithPath = dirPath + "\\SignerAttachments\\" + Guid.Empty + "\\" + deleteDocumentRequest.FileName;
                            if (System.IO.File.Exists(fileNameWithPath))
                                System.IO.File.Delete(fileNameWithPath);
                        }
                        EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                        envelopeAdditionalUploadInfo = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByID(Convert.ToInt32(item.ID), Guid.Empty);
                        if (envelopeAdditionalUploadInfo != null)
                        {
                            envelopeAdditionalUploadInfo.FileName = "";
                            envelopeAdditionalUploadInfo.OriginalFileName = "";
                            bool isUpdated = _envelopeRepository.UpdateEnvelopeAdditionalUploadInfo(envelopeAdditionalUploadInfo);
                        }
                    }
                }

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Document Deleted successfully";
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller DeleteSignerDocFromDirectory action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageUploadDocument), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("UploadSignerAttachments")]
        [HttpPost]
        //Upload Local Drive Attachments
        public async Task<IActionResult> UploadSignerAttachments([FromForm] IFormCollection uploadRequest)
        {
            ResponseMessageUploadDocument responseMessage = new ResponseMessageUploadDocument();
            List<DocumentUploadFilesResult> uploadedDocuments = new List<DocumentUploadFilesResult>();
            string currentRecipientEmail = string.Empty;
            string envelopeId = string.Empty;
            loggerModelNew = new LoggerModelNew("", "DocumentController", "UploadSignerAttachments", "Process started for Upload Signer requested attachments using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            try
            {
                bool IsMultipartContentTypeExists = !string.IsNullOrEmpty(Request.ContentType) && Request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
                if (!IsMultipartContentTypeExists)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "Error";
                    responseMessage.Message = "The request couldn't be processed";
                    return BadRequest(responseMessage);
                }

                //if (!request.HasFormContentType)
                //{
                //    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                //    responseMessage.StatusMessage = "Error";
                //    responseMessage.Message = "The request couldn't be processed";
                //    return BadRequest(responseMessage);
                //}                    

                var files = HttpContext.Request.Form.Files;

                Guid currentEnvelopeID = new Guid(Convert.ToString(uploadRequest["envelopeIDSiA"]));
                var Name = Convert.ToString(uploadRequest["NameSiA"]);
                var Description = Convert.ToString(uploadRequest["DescriptionSiA"]);
                var AdditionalInfo = Convert.ToString(uploadRequest["AdditionalInfoSiA"]);
                currentRecipientEmail = Convert.ToString(uploadRequest["recipientEmailSiA"]);
                Guid recipientsId = new Guid(Convert.ToString(uploadRequest["recipientIDSiA"])); ;
                int uploadsAttachmentID = Convert.ToInt32(Convert.ToString(uploadRequest["uploadAttachmentIDSiA"]));
                bool IsStaticTemplate = Convert.ToBoolean(Convert.ToString(uploadRequest["IsStaticTemplate"]));
                bool IsNewRow = false;
                Recipients objRecipients = new Recipients();
                List<string> lstFiles = new List<string>();

                string fileNameToSaveStr = "";
                string originalFileName = string.Empty;
                if (!IsStaticTemplate)
                {
                    var recipientList = _recipientRepository.GetEnvelopeSignerRecipientByEmail(currentEnvelopeID, currentRecipientEmail);
                    if (recipientList.Count == 0)
                    {
                        responseMessage.StatusCode = HttpStatusCode.NoContent;
                        responseMessage.StatusMessage = "Error";
                        responseMessage.Message = "Error Occured while uploading Document. Please try again.";
                        loggerModelNew.Message = responseMessage.Message;
                        rSignLogger.RSignLogInfo(loggerModelNew);
                        return BadRequest(responseMessage);
                    }
                }

                EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo = new EnvelopeAdditionalUploadInfo();
                envelopeAdditionalUploadInfo = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByID(uploadsAttachmentID, recipientsId);
                string tblrecipientId = string.Empty;
                Guid orignalRecipientsId = recipientsId;
                if (envelopeAdditionalUploadInfo != null)
                    tblrecipientId = Convert.ToString(envelopeAdditionalUploadInfo.RecipientID);
                else
                    tblrecipientId = Convert.ToString(recipientsId);

                Guid temprecipentID = new Guid();

                if (!string.IsNullOrEmpty(tblrecipientId) && new Guid(tblrecipientId) == temprecipentID)
                {
                    recipientsId = recipientsId;// temprecipentID;
                }

                string tempdirPath = _modelHelper.GetEnvelopeDirectoryNew(currentEnvelopeID, string.Empty);
                var dirPath = tempdirPath + currentEnvelopeID;
                if (IsStaticTemplate)
                {
                    if (!Directory.Exists(dirPath))
                    {
                        dirPath = tempdirPath + currentEnvelopeID;
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);
                    }
                }
                else
                {
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                }

                var uploadedDirectory = (dirPath + "\\SignerAttachments") + "\\" + recipientsId;
                if (!Directory.Exists(uploadedDirectory))
                    Directory.CreateDirectory(uploadedDirectory);
                string[] fileArray = Directory.GetFiles(uploadedDirectory);
                int count = fileArray.Count();
                string[] invalidFileTypes = { "exe", "msi", "js", "jar", "vb", "vbs", "bat" };

                if (envelopeAdditionalUploadInfo == null)  // insert new rec 
                {
                    EnvelopeAdditionalUploadInfo newObj = new EnvelopeAdditionalUploadInfo();
                    newObj.MasterEnvelopeID = currentEnvelopeID;
                    newObj.Name = Name;
                    newObj.Description = Description;
                    newObj.AdditionalInfo = AdditionalInfo;
                    newObj.FileName = null;
                    newObj.OriginalFileName = null;
                    newObj.RecipientEmailID = currentRecipientEmail;
                    newObj.CreatedDateTime = DateTime.Now;
                    newObj.ModifiedDateTime = DateTime.Now;
                    newObj.RecipientID = recipientsId;

                    _envelopeRepository.SaveEnvelopeAdditionalUploadInfo(newObj);

                    envelopeAdditionalUploadInfo = _envelopeRepository.GetEnvelopeAdditionalUploadInfoByID(Convert.ToInt32(newObj.ID), recipientsId);
                    uploadsAttachmentID = Convert.ToInt32(newObj.ID);
                    IsNewRow = true;
                }
                int fileupload = 0;
                for (int i = 0; i < files.Count; i++)
                {
                    if (fileupload == 0)
                    {
                        var file = files[i];
                        var fileName = file.FileName.Replace("%20", " ").Trim('\"');
                        string ext = Path.GetExtension(fileName);
                        bool isValidType = true;
                        double size = 0;
                        var fileNameToSave = fileName;
                        originalFileName = fileName;
                        for (int j = 0; j < invalidFileTypes.Length; j++)
                        {
                            if (ext == "." + invalidFileTypes[j])
                            {
                                isValidType = false;
                                responseMessage.StatusCode = HttpStatusCode.NoContent;
                                responseMessage.StatusMessage = "Error";
                                responseMessage.Message = "Invalid file format";
                                loggerModelNew.Message = responseMessage.Message;
                                rSignLogger.RSignLogWarn(loggerModelNew);
                                return BadRequest(responseMessage);
                            }
                        }
                        count++;
                        foreach (FileInfo folderfiles in new DirectoryInfo(uploadedDirectory).GetFiles())
                        {
                            size += folderfiles.Length;
                        }

                        size = size + file.Length;
                        if (size > 15728640)
                        {
                            if (envelopeAdditionalUploadInfo != null)
                            {
                                _envelopeRepository.RemoveEnvelopeAdditionalUploadInfo(envelopeAdditionalUploadInfo);
                            }
                            responseMessage.StatusCode = HttpStatusCode.NotAcceptable;
                            responseMessage.StatusMessage = "Error";
                            responseMessage.Message = "Cannot attach more than 15MB.";
                            loggerModelNew.Message = responseMessage.Message;
                            rSignLogger.RSignLogWarn(loggerModelNew);
                            return BadRequest(responseMessage);
                        }

                        string datewithext = "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                        string strFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + objRecipients.Order + "_" + objRecipients.Name + datewithext;
                        if (strFileName.Length > 95)
                        {
                            strFileName = (Path.GetFileNameWithoutExtension(fileName) + "_" + objRecipients.Order + "_" + objRecipients.Name).Substring(0, 95 - datewithext.Length) + datewithext;
                        }
                        if (!IsNewRow)
                        {
                            if (!string.IsNullOrEmpty(envelopeAdditionalUploadInfo.FileName))
                            {
                                var TempuploadedDirectory = (dirPath + "\\TempSignerAttachments") + "\\" + recipientsId;
                                if (!Directory.Exists(TempuploadedDirectory))
                                    Directory.CreateDirectory(TempuploadedDirectory);
                                string sourceFile = Path.Combine(uploadedDirectory, envelopeAdditionalUploadInfo.FileName);
                                string destFile = Path.Combine(TempuploadedDirectory, envelopeAdditionalUploadInfo.FileName);
                                if (System.IO.File.Exists(Path.Combine(uploadedDirectory, envelopeAdditionalUploadInfo.FileName)))
                                {
                                    System.IO.File.Copy(sourceFile, destFile, true);
                                    System.IO.File.Delete(sourceFile);
                                }
                            }
                        }
                        using (System.IO.Stream fileToupload = System.IO.File.Create(uploadedDirectory + "\\" + strFileName))
                        {
                            if (!string.IsNullOrEmpty(envelopeAdditionalUploadInfo.FileName))
                            {
                                if (System.IO.File.Exists(uploadedDirectory + "\\" + envelopeAdditionalUploadInfo.FileName))
                                    System.IO.File.Delete(uploadedDirectory + "\\" + envelopeAdditionalUploadInfo.FileName);
                            }
                            fileNameToSaveStr = strFileName;
                            file.CopyTo(fileToupload);
                            fileToupload.Close();
                            fileupload = 1;
                        }
                    }
                }

                if (envelopeAdditionalUploadInfo != null)
                {
                    envelopeAdditionalUploadInfo.FileName = fileNameToSaveStr;
                    envelopeAdditionalUploadInfo.OriginalFileName = originalFileName;
                    responseMessage.UploadedFileName = fileNameToSaveStr;
                    bool isUpdated = _envelopeRepository.UpdateEnvelopeAdditionalUploadInfo(envelopeAdditionalUploadInfo);
                }

                lstFiles = _envelopeHelperMain.GetSignerDocFromDirectory(currentEnvelopeID, orignalRecipientsId, tempdirPath);
                if (lstFiles.Count == 0)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                    responseMessage.StatusMessage = "Error";
                    responseMessage.Message = "Document uploaded Fail";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = Convert.ToString(_appConfiguration["SuccessDocumentUpload"]);
                responseMessage.Files = lstFiles;
                responseMessage.DocumentDetails = new List<DocumentUploadFilesResult> { new DocumentUploadFilesResult { ID = Convert.ToString(uploadsAttachmentID) } };
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller GetDownloadFileReview action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }       

        [ProducesResponseType(typeof(ResponseMessageFile), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("UploadSignerSignature")]
        [HttpPost]
        //Upload Local Drive Signature
        public async Task<IActionResult> UploadSignerSignature([FromForm] IFormCollection uploadRequest)
        {
            ResponseMessageFile responseMessage = new ResponseMessageFile();
            loggerModelNew = new LoggerModelNew("", "DocumentController", "UploadSignerSignature", "Process started for Download Upload Signer Signature using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);

            try
            {
                bool IsMultipartContentTypeExists = !string.IsNullOrEmpty(Request.ContentType) && Request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
                if (!IsMultipartContentTypeExists)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "Error";
                    responseMessage.Message = "The request couldn't be processed";
                    return BadRequest(responseMessage);
                }

                //if (!request.HasFormContentType)
                //{
                //    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                //    responseMessage.StatusMessage = "Error";
                //    responseMessage.Message = "The request couldn't be processed";
                //    return BadRequest(responseMessage);
                //}                    

                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    double size = 0;
                    var fileName = file.FileName.Replace("%20", " ").Trim('\"');
                    string ext = Path.GetExtension(fileName);
                    string base64String = string.Empty;
                    using (var ms = new MemoryStream())
                    {
                        file.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        base64String = Convert.ToBase64String(fileBytes);
                    }

                    if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".bmp")
                    {
                        using (System.IO.Stream memStream = new MemoryStream(Convert.FromBase64String(base64String)))
                        {
                            using (System.Drawing.Image img = System.Drawing.Image.FromStream(memStream))
                            {
                                if (img.Width == 528 && img.Height == 113)
                                {
                                    base64String = base64String;
                                }
                                else
                                {
                                    byte[] imageBytes;
                                    // To preserve the aspect ratio
                                    int maxWidth = 528;
                                    int maxHeight = 113;
                                    float ratioX = (float)maxWidth / (float)img.Width;
                                    float ratioY = (float)maxHeight / (float)img.Height;
                                    float ratio = Math.Min(ratioX, ratioY);

                                    float sourceRatio = (float)img.Width / img.Height;

                                    int newWidth = (int)(img.Width * ratio);
                                    int newHeight = (int)(img.Height * ratio);

                                    Bitmap newImage = new Bitmap(newWidth, newHeight); //, PixelFormat.Format24bppRgb

                                    using (Graphics graphics = Graphics.FromImage(newImage))
                                    {
                                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                                        graphics.DrawImage(img, 0, 0, newWidth, newHeight);
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            Bitmap image = new Bitmap(img, 528, 113);
                                            if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                                            {
                                                image.Save(ms, ImageFormat.Jpeg);
                                            }
                                            else if (ext.ToLower() == ".png")
                                            {
                                                image.Save(ms, ImageFormat.Png);
                                            }
                                            else if (ext.ToLower() == ".bmp")
                                            {
                                                image.Save(ms, ImageFormat.Bmp);
                                            }
                                            else
                                            {
                                                image.Save(ms, ImageFormat.Jpeg);
                                            }

                                            imageBytes = ms.ToArray();

                                            // Convert byte[] to Base64 String
                                            base64String = Convert.ToBase64String(imageBytes);
                                        }
                                    }
                                }
                                responseMessage.Base64FileData = base64String;
                                responseMessage.FileName = fileName;
                                responseMessage.StatusCode = HttpStatusCode.OK;
                                responseMessage.StatusMessage = "OK";
                                responseMessage.Message = "Document downloaded successfully";
                                return Ok(responseMessage);

                            }
                        }
                    }
                }
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = Convert.ToString(_appConfiguration["SuccessDocumentUpload"]);
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller UploadSignerSignature action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }       

        [ProducesResponseType(typeof(ResponseMessageFile), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("UploadSignerSignatureFromOtherDirves")]
        [HttpPost]
        //Upload Drop Box Signature
        public async Task<IActionResult> UploadSignerSignatureFromOtherDirves([FromForm] IFormCollection uploadRequest)
        {
            loggerModelNew = new LoggerModelNew("", "DocumentController", "UploadSignerSignatureFromOtherDirves", "Initiate the process for Upload Signer requested image using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageFile responseMessage = new ResponseMessageFile();

            try
            {
                bool IsMultipartContentTypeExists = !string.IsNullOrEmpty(Request.ContentType) && Request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
                if (!IsMultipartContentTypeExists)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "Error";
                    responseMessage.Message = "The request couldn't be processed";
                    return BadRequest(responseMessage);
                }

                //if (!request.HasFormContentType)
                //{
                //    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                //    responseMessage.StatusMessage = "Error";
                //    responseMessage.Message = "The request couldn't be processed";
                //    return BadRequest(responseMessage);
                //}                    

                var files = HttpContext.Request.Form.Files;
                string url = Convert.ToString(uploadRequest["fileUploadInput"]);

                // string url = formData.GetValues("fileUploadInput")[0].ToString();
                //create an object of StringBuilder type.
                StringBuilder _sb = new StringBuilder();
                //create a byte array that will hold the return value of the getImg method
                byte[] _byte = GetImg(url);

                String base64String = Convert.ToBase64String(_byte);
                string ext = System.IO.Path.GetExtension(url);

                if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".bmp")
                {
                    using (System.IO.Stream memStream = new MemoryStream(Convert.FromBase64String(base64String)))
                    {
                        using (System.Drawing.Image img = System.Drawing.Image.FromStream(memStream))
                        {
                            if (img.Width == 528 && img.Height == 113)
                            {
                                base64String = base64String;
                            }
                            else
                            {
                                byte[] imageBytes;
                                // To preserve the aspect ratio
                                int maxWidth = 528;
                                int maxHeight = 113;
                                float ratioX = (float)maxWidth / (float)img.Width;
                                float ratioY = (float)maxHeight / (float)img.Height;
                                float ratio = Math.Min(ratioX, ratioY);
                                float sourceRatio = (float)img.Width / img.Height;
                                int newWidth = (int)(img.Width * ratio);
                                int newHeight = (int)(img.Height * ratio);
                                Bitmap newImage = new Bitmap(newWidth, newHeight); //, PixelFormat.Format24bppRgb

                                using (Graphics graphics = Graphics.FromImage(newImage))
                                {
                                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                                    graphics.DrawImage(img, 0, 0, newWidth, newHeight);
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        Bitmap image = new Bitmap(img, 528, 113);
                                        if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                                        {
                                            image.Save(ms, ImageFormat.Jpeg);
                                        }
                                        else if (ext.ToLower() == ".png")
                                        {
                                            image.Save(ms, ImageFormat.Png);
                                        }
                                        else if (ext.ToLower() == ".bmp")
                                        {
                                            image.Save(ms, ImageFormat.Bmp);
                                        }
                                        else
                                        {
                                            image.Save(ms, ImageFormat.Jpeg);
                                        }
                                        imageBytes = ms.ToArray();
                                        // Convert byte[] to Base64 String
                                        base64String = Convert.ToBase64String(imageBytes);
                                    }
                                }
                            }

                            responseMessage.Base64FileData = base64String;
                            responseMessage.StatusCode = HttpStatusCode.OK;
                            responseMessage.StatusMessage = "OK";
                            responseMessage.Message = "Document downloaded successfully";
                            return Ok(responseMessage);
                        }
                    }
                }
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = Convert.ToString(_appConfiguration["SuccessDocumentUpload"]);
                loggerModelNew.Message = responseMessage.Message;
                rSignLogger.RSignLogInfo(loggerModelNew);
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller UploadSignerSignatureFromOtherDirves action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }


        //[ProducesResponseType(typeof(ResponseMessageFile), 200)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Route("UploadBase64StringToImage")]
        //[HttpPost]
        ////UploadBase64StringToImage
        //public async Task<IActionResult> UploadBase64StringToImage(UploadSignatureStringModel uploadSignatureStringModel)
        //{
        //    loggerModelNew = new LoggerModelNew("", "DocumentController", "UploadBase64StringToImage", "Initiate process of Upload Base64 String To Image using API.", "", "", "", "", "API");
        //    rSignLogger.RSignLogInfo(loggerModelNew);
        //    ResponseMessageFile responseMessage = new ResponseMessageFile();

        //    try
        //    {
        //        string ext = uploadSignatureStringModel.ExtType;
        //        string base64String = string.Empty;
        //        using (System.IO.Stream memStream = new MemoryStream(Convert.FromBase64String(uploadSignatureStringModel.Base64UploadSignatureString)))
        //        {
        //            using (System.Drawing.Image img = System.Drawing.Image.FromStream(memStream))
        //            {
        //                if (img.Width == 528 && img.Height == 113)
        //                {
        //                    base64String = uploadSignatureStringModel.Base64UploadSignatureString;
        //                }
        //                else
        //                {
        //                    byte[] imageBytes;
        //                    // To preserve the aspect ratio
        //                    int maxWidth = 528;
        //                    int maxHeight = 113;
        //                    float ratioX = (float)maxWidth / (float)img.Width;
        //                    float ratioY = (float)maxHeight / (float)img.Height;
        //                    float ratio = Math.Min(ratioX, ratioY);
        //                    float sourceRatio = (float)img.Width / img.Height;
        //                    int newWidth = (int)(img.Width * ratio);
        //                    int newHeight = (int)(img.Height * ratio);
        //                    Bitmap newImage = new Bitmap(newWidth, newHeight); //, PixelFormat.Format24bppRgb

        //                    using (Graphics graphics = Graphics.FromImage(newImage))
        //                    {
        //                        graphics.CompositingQuality = CompositingQuality.HighQuality;
        //                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //                        graphics.SmoothingMode = SmoothingMode.HighQuality;
        //                        graphics.DrawImage(img, 0, 0, newWidth, newHeight);
        //                        using (MemoryStream ms = new MemoryStream())
        //                        {
        //                            Bitmap image = new Bitmap(img, 528, 113);
        //                            if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
        //                            {
        //                                image.Save(ms, ImageFormat.Jpeg);
        //                            }
        //                            else if (ext.ToLower() == ".png")
        //                            {
        //                                image.Save(ms, ImageFormat.Png);
        //                            }
        //                            else if (ext.ToLower() == ".bmp")
        //                            {
        //                                image.Save(ms, ImageFormat.Bmp);
        //                            }
        //                            else
        //                            {
        //                                image.Save(ms, ImageFormat.Jpeg);
        //                            }

        //                            imageBytes = ms.ToArray();
        //                            // Convert byte[] to Base64 String
        //                            base64String = Convert.ToBase64String(imageBytes);
        //                        }
        //                    }
        //                }
        //                responseMessage.Base64FileData = base64String;
        //                responseMessage.FileName = uploadSignatureStringModel.FileName;
        //                responseMessage.StatusCode = HttpStatusCode.OK;
        //                responseMessage.StatusMessage = "OK";
        //                responseMessage.Message = "Document downloaded successfully";
        //                return Ok(responseMessage);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        loggerModelNew.Message = "Error occurred in DocumentController controller UploadBase64StringToImage action." + ex.Message;
        //        rSignLogger.RSignLogError(loggerModelNew, ex);
        //        return BadRequest(new { ErrorMessage = ex.Message });
        //    }
        //}
        private byte[] GetImg(string url)
        {
            //create a stream object and initialize it to null
            System.IO.Stream stream = null;
            //create a byte[] object. It serves as a buffer.
            byte[] buf;
            try
            {
                //Create a new WebProxy object.
                WebProxy myProxy = new WebProxy();
                //create a HttpWebRequest object and initialize it by passing the colleague api url to a create method.
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                //Create a HttpWebResponse object and initilize it
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                //get the response stream
                stream = response.GetResponseStream();

                using (BinaryReader br = new BinaryReader(stream))
                {
                    //get the content length in integer
                    int len = (int)(response.ContentLength);
                    //Read bytes
                    buf = br.ReadBytes(len);
                    //close the binary reader
                    br.Close();
                }
                //close the stream object
                stream.Close();
                //close the response object 
                response.Close();
            }
            catch (Exception exp)
            {
                //set the buffer to null
                buf = null;
            }
            //return the buffer
            return (buf);
        }

        /// <summary>
        /// Delete Invite By Email Users Attachments
        /// </summary>
        [ProducesResponseType(typeof(ResponseMessageFile), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DeleteInviteByEmailUsersAttachments")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult DeleteInviteByEmailUsersAttachments(DeleteInviteByEmailUserDocumentRequest deleteDocumentRequest)
        {
            loggerModelNew = new LoggerModelNew("", "DocumentController", "DeleteInviteByEmailUsersAttachments", "Delete Invite By Email Users Attachments using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);          
            ResponseMessageUploadDocument responseMessage = new ResponseMessageUploadDocument();

            try
            {
                List<string> lstFiles = new List<string>();              
                List<TemplateGroupDocumentUploadDetails> additionalDocument = _envelopeRepository.GetEnvelopeAdditionalDocument(deleteDocumentRequest.EnvelopeID);

                if (deleteDocumentRequest.RecipientIds != null)
                {
                    string[] lstRecipientIds = deleteDocumentRequest.RecipientIds.Split(',');
                    foreach (var item in additionalDocument)
                    {
                        if (lstRecipientIds.Contains(item.RecipientId.ToString()))
                        {
                            var dirPath = _modelHelper.GetEnvelopeDirectoryNew(deleteDocumentRequest.EnvelopeID, string.Empty) + deleteDocumentRequest.EnvelopeID;
                            var fileNameWithPath = dirPath + "\\SignerAttachments\\" + deleteDocumentRequest.TempRecipientId + "\\" + item.FileName;
                            if (System.IO.File.Exists(fileNameWithPath))
                            {
                                System.IO.File.Delete(fileNameWithPath);
                            }
                            else
                            {
                                fileNameWithPath = dirPath + "\\SignerAttachments\\" + Guid.Empty + "\\" + item.FileName;
                                if (System.IO.File.Exists(fileNameWithPath))
                                    System.IO.File.Delete(fileNameWithPath);
                            }

                            bool isDeleted = _envelopeRepository.UpdateEnvelopeAdditionalUploadInfoByInviteByEmailUserID(Convert.ToInt32(item.ID), item.RecipientId ?? new Guid());
                        }
                    }
                }

                lstFiles = _envelopeRepository.GetEnvelopeAdditionalDocumentName(deleteDocumentRequest.EnvelopeID);

                responseMessage.Files = lstFiles;
                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Document Deleted successfully";
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller DeleteInviteByEmailUsersAttachments action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [ProducesResponseType(typeof(ResponseMessageFile), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("DownloadDocument/{envelopeId}/{documentId}/{IsEnvelopeArichived?}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult DownloadDocument(string envelopeId, string documentId, int IsEnvelopeArichived = 0)
        {
            loggerModelNew = new LoggerModelNew("", "DocumentController", "DownloadDocument", "Process started for Download Document using API.", "", "", "", "", "API");
            rSignLogger.RSignLogInfo(loggerModelNew);
            ResponseMessageFile responseMessage = new ResponseMessageFile();

            try
            {
                Envelope envelope = _genericRepository.GetEntity(new Guid(envelopeId), true, IsEnvelopeArichived);
                Guid DocumentId = new Guid(documentId);

                if (envelope == null || envelope.Documents.FirstOrDefault(d => d.ID == new Guid(documentId)) == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "Document not found";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }

                string dirPath = _modelHelper.GetEnvelopeDirectoryNew(new Guid(envelopeId), string.Empty);
                string fileName = envelope.Documents.FirstOrDefault(d => d.ID == DocumentId).DocumentName;
                string tempDirectoryPath = _eSignHelper.GetUploadedDocumentsFolderPath(new Guid(envelopeId), dirPath);
                string filePath = Path.Combine(tempDirectoryPath, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;
                    responseMessage.StatusMessage = "BadRequest";
                    responseMessage.Message = "Document not found";
                    loggerModelNew.Message = responseMessage.Message;
                    rSignLogger.RSignLogWarn(loggerModelNew);
                    return BadRequest(responseMessage);
                }
                byte[] imgBytes = System.IO.File.ReadAllBytes(filePath);
                string imgBase64 = Convert.ToBase64String(imgBytes);

                loggerModelNew.Message = "Successfully retrieved the document " + fileName;
                rSignLogger.RSignLogInfo(loggerModelNew);
                responseMessage.Base64FileData = imgBase64;
                responseMessage.FileName = fileName;
                responseMessage.FilePath = filePath;
                responseMessage.byteArray = imgBytes;

                responseMessage.StatusCode = HttpStatusCode.OK;
                responseMessage.StatusMessage = "OK";
                responseMessage.Message = "Document downloaded successfully";
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in DocumentController controller DownloadDocument action." + ex.Message;
                rSignLogger.RSignLogError(loggerModelNew, ex);
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }
    }
}
