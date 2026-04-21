using iText.Kernel.Pdf;
using iText.Signatures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.ManageDocument;
using RSign.ManageDocument.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.ManageDocument.Models;
using RSign.ManageDocument.Models.iTextHelper;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static RSign.Common.Helpers.Constants;

namespace RSign.Models.Helpers
{
  public class ApiHelpers : IApiHelper
  {
    private readonly IConfiguration _appConfiguration;
    private readonly IOptions<AppSettingsConfig> _configuration;
    private IHttpContextAccessor _accessor;
    RSignLogger rsignlog = new RSignLogger();
    LoggerModelNew loggerModelNew = new LoggerModelNew();
    private readonly IRecipientRepository _recipientRepository;
    private readonly IESignHelper _eSignHelper;
    private readonly IConditionalControlRepository _conditionalControlRepository;
    private readonly ILookupRepository _lookupRepository;
    private readonly IModelHelper _modelHelper;
    private readonly IAsposeHelper _asposeHelper;
    private readonly ISettingsRepository _settingsRepository;
    // private readonly IEnvelopeRepository _envelopeRepository;
    // private readonly IEnvelopeHelperMain _envelopeHelperMain;
    private readonly IUserRepository _userRepository;
    private readonly string ImageDirectoryPath;

    public ApiHelpers(IHttpContextAccessor accessor, IOptions<AppSettingsConfig> configuration, IRecipientRepository recipientRepository, IESignHelper eSignHelper,
        IConditionalControlRepository conditionalControlRepository, ILookupRepository lookupRepository, IModelHelper modelHelper, IAsposeHelper asposeHelper,
        ISettingsRepository settingsRepository, IConfiguration appConfiguration,
        //IEnvelopeRepository envelopeRepository,
        // IEnvelopeHelperMain envelopeHelperMain,

        IUserRepository userRepository)
    {
      _accessor = accessor;
      _configuration = configuration;
      _recipientRepository = recipientRepository;
      _eSignHelper = eSignHelper;
      _conditionalControlRepository = conditionalControlRepository;
      _lookupRepository = lookupRepository;
      _modelHelper = modelHelper;
      _asposeHelper = asposeHelper;
      _settingsRepository = settingsRepository;
      _appConfiguration = appConfiguration;
      // _envelopeRepository = envelopeRepository;
      // _envelopeHelperMain = envelopeHelperMain;
      _userRepository = userRepository;
      rsignlog = new RSignLogger(_appConfiguration);
      ImageDirectoryPath = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["Images"]));
    }
    public string GetFilesReviewZip(List<string> files, string documentPaths, string Subject)
    {
      List<string> Checkfiles = new List<string>();
      Checkfiles = DirSearch(documentPaths);
      if (files.Count > 0)
      {
        if (!Directory.Exists(documentPaths + "\\" + Subject))
        {
          using (var zip = new Ionic.Zip.ZipFile())
          {
            zip.AddFiles(files, false, "");
            zip.Save(documentPaths + "\\" + Subject + ".zip");
          }
        }

      }
      string finalSignerAttachFilePath = Path.Combine(documentPaths, Subject + ".zip");
      return finalSignerAttachFilePath;
    }
    private List<string> DirSearch(string sDir, bool isWatermark = false)
    {
      List<string> files = new List<string>();
      try
      {
        foreach (string f in Directory.GetFiles(sDir))
        {
          if (isWatermark == true)
          {
            if (Path.GetFileName(f) != "ContractsSigner.zip")
            {
              files.Add(f);
            }
          }
          else
            files.Add(f);
        }
        foreach (string d in Directory.GetDirectories(sDir))
        {
          files.AddRange(DirSearch(d, isWatermark));
        }
      }
      catch (System.Exception excpt)
      {

      }
      return files;
    }
    public byte[] GetControlImage(DocumentContents control)
    {
      if (control.ControlID == Constants.Control.Radio)
      {
        return _eSignHelper.GetCheckedImage(control.Height + (!string.IsNullOrEmpty(control.ControlValue) && control.ControlValue.ToLower() == "true" ? "radio" : "unradio"));
      }

      if (control.ControlID == Constants.Control.Checkbox)
      {
        return _eSignHelper.GetCheckedImage(control.Height + (!string.IsNullOrEmpty(control.ControlValue) && control.ControlValue.ToLower() == "true" ? "checked" : "unchecked"));
      }
      if (control.ControlID == Constants.Control.NewInitials)
      {
        return control.SignatureControlValue;
      }
      if (control.ControlID == Constants.Control.Signature && control.SignatureControlValue != null)
      {
        return control.SignatureControlValue;
      }
      using (var dbContext = new RSignDbContext(_configuration))
      {
        if (control.Required && (control.SignatureControlValue == null || control.ControlValue == null))
        {
          var conditionalControl = _conditionalControlRepository.GetControllingFieldOfControl(control.ID);
          if (conditionalControl != null && conditionalControl.ControlID != Guid.Empty)
          {
            return control.SignatureControlValue;
          }
          else
          {
            return _recipientRepository.GetSignerSignature(control.RecipientID.GetValueOrDefault()) == null ? control.SignatureControlValue : _recipientRepository.GetSignerSignature(control.RecipientID.GetValueOrDefault()).Signature;
          }
        }
        else
        {
          if (control.Required)
            return _recipientRepository.GetSignerSignature(control.RecipientID.GetValueOrDefault()) == null ? control.SignatureControlValue : _recipientRepository.GetSignerSignature(control.RecipientID.GetValueOrDefault()).Signature;
          else
            return control.SignatureControlValue;
        }
      }
    }
    public string finalMergePDFApi(Envelope envelopeObject, FinalContractSettings userSettings, string envelopeDirectoryUNCPath, string currentStatus = "", bool IsRequiredZip = false)
    {
      loggerModelNew = new LoggerModelNew("", "ApiHelpers", "finalMergePDFApi", "Initiate the process for generating final pdf with document paper size " + Convert.ToString(envelopeObject.DocumentPaperSizeID) + "for " + envelopeObject.ID.ToString(), envelopeObject.ID.ToString(), "", "", "", "API");
      rsignlog.RSignLogInfo(loggerModelNew);

      iText7Helper iTextHelper = new iText7Helper(_appConfiguration);
      bool isFinalSignedContractToAttach = true;
      isFinalSignedContractToAttach = (bool)envelopeObject.IsFinalCertificateReq;
      //Handled the default value
      userSettings.FinalContractOptions = userSettings.FinalContractOptions == 0 ? 1 : userSettings.FinalContractOptions;

      List<DocumentControls> controls = null;
      //Changes done by v2 team on dec 16
      if (userSettings.IsControlDisplayInTag == true)
      {
        controls = GetDocumentControlsForCC(envelopeObject);
      }
      else
      {
        controls = GetDocumentControlsForSigningCertifate(envelopeObject);
      }

      var pageNumbers = controls.GroupBy(x => new { x.PageNo }).Select(x => x.Key.PageNo).ToList();
      var dirPath = envelopeDirectoryUNCPath;
      string envelopeFolderPath = dirPath + "/" + envelopeObject.ID;
      string envelopeImagePath = _eSignHelper.GetImagesFolderPath(envelopeObject.ID, dirPath);
      string envelopeImage300Path = _eSignHelper.GetImages300FolderPath(envelopeObject.ID, dirPath);
      string envelopeConvertedPath = _eSignHelper.GetConvertedDocumentsFolderPath(envelopeObject.ID, dirPath);
      string envelopeUploadedPath = _eSignHelper.GetUploadedDocumentsFolderPath(envelopeObject.ID, dirPath);
      //using (var asposeHelper = new AsposeHelper())
      //{
      // _asposeHelper.FontFolderPath = Path.Combine(commonfiles, ConfigurationManager.AppSettings["FontFolderPath"].ToString()); //ConfigurationManager.AppSettings["FontFolderPath"].ToString();

      if (!string.IsNullOrEmpty(currentStatus))
      {
        currentStatus = "/" + currentStatus + DateTime.Now.Ticks;
        System.IO.Directory.CreateDirectory(envelopeFolderPath + currentStatus);
      }
      string mergePdfFilePath = Path.Combine(envelopeFolderPath + currentStatus, "Output.pdf");

      loggerModelNew.Message = string.Format("Initiate the process for creating Output.pdf at root level using {0}", (userSettings.FinalContractOptions == Constants.FinalContractOptions.FromImages) ? "Merge Images" : ((userSettings.FinalContractOptions == Constants.FinalContractOptions.iText) ? "Merge PDF using iText" : "Merge PDF using Aspose"));
      rsignlog.RSignLogInfo(loggerModelNew);

      if (envelopeObject.IsEdited == true && File.Exists(mergePdfFilePath))
        File.Delete(mergePdfFilePath);
      if (userSettings.FinalContractOptions == Constants.FinalContractOptions.FromImages)
      {
        if (!File.Exists(mergePdfFilePath))
        {
          var filepaths = new List<string>();
          foreach (var doc in envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).OrderBy(o => o.Order))
            filepaths.Add(Path.Combine(envelopeConvertedPath, Path.GetFileNameWithoutExtension(doc.DocumentName) + ".pdf"));

          //Step 1: Create images with X resolution in Image300 directory
          _asposeHelper.ConvertPDFToImagesInXResolution(200, filepaths, envelopeImage300Path);

          //Step 2: Create Ouput.pdf from 300 resolution images at folder root level                                                            
          mergePdfFilePath = _asposeHelper.ConvertImagesToPDF(filepaths, envelopeImage300Path, mergePdfFilePath);

          //Step 3: Delete the files created at Images300 folder to free disk space                    
          _eSignHelper.DeleteFilesAndDirectory(envelopeImage300Path);

          loggerModelNew.Message = "Created the images with 300 resolution and Created the Output.pdf at root level using Images300 and Deleted the files and folder of directory - Images300";
          rsignlog.RSignLogInfo(loggerModelNew);
        }
      }
      else
      {
        if (!File.Exists(mergePdfFilePath))
        {
          var filepaths = new List<string>();
          foreach (var Doc in envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).OrderBy(d => d.Order))
          {
            if (!File.Exists(Path.Combine(envelopeConvertedPath, Path.GetFileNameWithoutExtension(Doc.DocumentName) + ".pdf")))
            {
              if (File.Exists(Path.Combine(envelopeUploadedPath, Path.GetFileNameWithoutExtension(Doc.DocumentName) + ".pdf")))
              {
                File.Copy(Path.Combine(envelopeUploadedPath, Path.GetFileNameWithoutExtension(Doc.DocumentName) + ".pdf"), Path.Combine(envelopeConvertedPath, Path.GetFileNameWithoutExtension(Doc.DocumentName) + ".pdf"), true);
                filepaths.Add(Path.Combine(envelopeConvertedPath, Path.GetFileNameWithoutExtension(Doc.DocumentName) + ".pdf"));
              }
            }
            else
            {
              filepaths.Add(Path.Combine(envelopeConvertedPath, Path.GetFileNameWithoutExtension(Doc.DocumentName) + ".pdf"));
            }
          }
          if (userSettings.FinalContractOptions == Constants.FinalContractOptions.iText)
            mergePdfFilePath = iTextHelper.MergePDFiText(filepaths, mergePdfFilePath);
          else
            mergePdfFilePath = _asposeHelper.MergepdfFiles(filepaths, mergePdfFilePath) ?? filepaths[0];
        }

        //Apply From flattening.
        _asposeHelper.FlattenAllPdfFields(envelopeObject.EDisplayCode, mergePdfFilePath);

        //if (userSettings.FinalContractOptions == Constants.FinalContractOptions.iText)
        //    iTextHelper.FlattenAllPdfFieldsUsingiText(envelopeObject.EDisplayCode, mergePdfFilePath, Path.Combine(envelopeFolderPath + currentStatus, "OutputNew.pdf"));
        //else
        //    asposeHelper.FlattenAllPdfFields(envelopeObject.EDisplayCode, mergePdfFilePath);
      }

      loggerModelNew.Message = string.Format("Completed the process for creating Output.pdf at root level using {0}", (userSettings.FinalContractOptions == Constants.FinalContractOptions.FromImages) ? "Merge Images" : ((userSettings.FinalContractOptions == Constants.FinalContractOptions.iText) ? "Merge PDF using iText" : "Merge PDF using Aspose"));
      rsignlog.RSignLogInfo(loggerModelNew);

      var documentFolderPath = Path.Combine(envelopeFolderPath + currentStatus, "EnvOutput.pdf");
      //EnvOutputSigner.pdf
      string password = null;
      if (envelopeObject.PasswordReqdtoOpen)
        password = ModelHelper.Decrypt(envelopeObject.PasswordtoOpen, envelopeObject.PasswordKey, (int)envelopeObject.PasswordKeySize);

      //Changes done by v2 team on dec 16
      if (userSettings.FinalContractOptions == Constants.FinalContractOptions.iText)
      {
        if (userSettings.IsControlDisplayInTag == true)// changes to send doc to cc recp
          iTextHelper.AddTextToPDFiText7ForCC(mergePdfFilePath, documentFolderPath, controls, Path.Combine(envelopeFolderPath, "Images"), string.IsNullOrEmpty(envelopeObject.EDisplayCode) ? envelopeObject.DisplayCode.ToString() : "ENV" + envelopeObject.EDisplayCode, userSettings.UserTimeZone, envelopeObject.ModifiedDateTime, envelopeObject.DateFormatID, GetHeaderFooterSettingOption(envelopeObject.UserID));
        else
        {
          if (File.Exists(documentFolderPath))
            File.Delete(documentFolderPath);

          iTextHelper.AddTextToPDFiText7(mergePdfFilePath, documentFolderPath, controls, Path.Combine(envelopeFolderPath, "Images"), string.IsNullOrEmpty(envelopeObject.EDisplayCode) ? envelopeObject.DisplayCode.ToString() : "ENV" + envelopeObject.EDisplayCode, userSettings.UserTimeZone, envelopeObject.ModifiedDateTime, envelopeObject.DateFormatID, GetHeaderFooterSettingOption(envelopeObject.UserID), envelopeObject.ElectronicSignIndicationOptionID != null ? envelopeObject.ElectronicSignIndicationOptionID.Value : Constants.ElectronicSignIndicationOption.Disable);
          loggerModelNew.Message = "completed the process for Add text in Pdf file using asposeHelper.AddTextToPDFiText7 with Password";
          rsignlog.RSignLogInfo(loggerModelNew);
        }
      }
      else
      {
        if (userSettings.IsControlDisplayInTag == true)// changes to send doc to cc recp
        {
          loggerModelNew.Message = "Initiate the process for Add text in Pdf file using asposeHelper.AddTextInPdf without Password";
          rsignlog.RSignLogInfo(loggerModelNew);
          _asposeHelper.AddTextInPdf(mergePdfFilePath, documentFolderPath, controls, pageNumbers, Path.Combine(envelopeFolderPath, "Images"), string.IsNullOrEmpty(envelopeObject.EDisplayCode) ? envelopeObject.DisplayCode.ToString() : "ENV" + envelopeObject.EDisplayCode, userSettings.UserTimeZone, envelopeObject.ModifiedDateTime, envelopeObject.DateFormatID, GetHeaderFooterSettingOption(envelopeObject.UserID), envelopeObject.ElectronicSignIndicationOptionID != null ? envelopeObject.ElectronicSignIndicationOptionID.Value : Constants.ElectronicSignIndicationOption.Disable);
        }
        else
        {
          loggerModelNew.Message = "Initiate the process for Add text in Pdf file using asposeHelper.AddTextInPdf with Password";
          rsignlog.RSignLogInfo(loggerModelNew);
          _asposeHelper.AddTextInPdf(mergePdfFilePath, documentFolderPath, controls, pageNumbers, Path.Combine(envelopeFolderPath, "Images"), password, string.IsNullOrEmpty(envelopeObject.EDisplayCode) ? envelopeObject.DisplayCode.ToString() : "ENV" + envelopeObject.EDisplayCode, userSettings.UserTimeZone, envelopeObject.ModifiedDateTime, envelopeObject.DateFormatID, GetHeaderFooterSettingOption(envelopeObject.UserID), envelopeObject.ElectronicSignIndicationOptionID != null ? envelopeObject.ElectronicSignIndicationOptionID.Value : Constants.ElectronicSignIndicationOption.Disable);
        }
      }

      loggerModelNew.Message = "Completed the process for Add text in Pdf file";
      rsignlog.RSignLogInfo(loggerModelNew);

      string finalPathSignerPDF = string.Empty;
      if (envelopeObject.IsWaterMark == true && (!string.IsNullOrEmpty(envelopeObject.WatermarkTextForSender) || !string.IsNullOrEmpty(envelopeObject.WatermarkTextForOther)))
      {
        //var finalPDFFilePathForSigner = dirPath + "/" + envelopeObject.ID + currentStatus + "/Final";
        string sourceDirectory = Path.GetDirectoryName(documentFolderPath);
        if (System.IO.Directory.Exists(sourceDirectory))
        {
          System.IO.File.Copy(Path.Combine(envelopeFolderPath + currentStatus, "EnvOutput.pdf"), Path.Combine(sourceDirectory, "EnvOutputSigner.pdf"), true);
          finalPathSignerPDF = Path.Combine(sourceDirectory, "EnvOutputSigner.pdf");

          loggerModelNew.Message = "Successfully created signer final contract.";
          rsignlog.RSignLogInfo(loggerModelNew);

          userSettings.WatermarkText = envelopeObject.WatermarkTextForOther;
          userSettings.IsBackgroundStamp = true;
          _asposeHelper.AddWaterMark(envelopeObject.EDisplayCode, finalPathSignerPDF, userSettings);
        }

        userSettings.WatermarkText = envelopeObject.WatermarkTextForSender;
        userSettings.IsBackgroundStamp = false;
        _asposeHelper.AddWaterMark(envelopeObject.EDisplayCode, documentFolderPath, userSettings);
      }

      //Get document hash and save it to envelope table.
      string documenthash = _eSignHelper.GetHashSha256(documentFolderPath);

      // if (isFinalSignedContractToAttach) // Final Signed Contract is now generated every time as per the new requirement
      GetSigningCertificate(envelopeObject, documenthash, currentStatus);

      string finalPathPDF = string.Empty, finalPathPDFSigner = string.Empty;
      if (isFinalSignedContractToAttach)
      {
        var certificatepath = Path.Combine(envelopeFolderPath + currentStatus, "ENV" + (envelopeObject.EDisplayCode).ToString(CultureInfo.InvariantCulture) + "_SigningCertificate.pdf");
        var filepath = new List<string> { documentFolderPath, certificatepath };
        var filepathSigner = new List<string> { finalPathSignerPDF, certificatepath };
        if (envelopeObject.IsSeparateMultipleDocumentsAfterSigningRequired && envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Count() > 1 && string.IsNullOrEmpty(currentStatus) && IsRequiredZip)
        {
          if (envelopeObject.IsWaterMark == true && (!string.IsNullOrEmpty(envelopeObject.WatermarkTextForSender) || !string.IsNullOrEmpty(envelopeObject.WatermarkTextForOther)))
          {
            string finalPathSigner = GetFinalSeperateMultiDocZipApiSigner(envelopeObject, filepathSigner, password, dirPath);
          }
          return GetFinalSeperateMultiDocZipApi(envelopeObject, filepath, password, dirPath);
        }

        var finalMergePDFFileDirectory = dirPath + "/" + envelopeObject.ID + currentStatus + "/Final";

        loggerModelNew.Message = "Merging the pdf file using asposeHelper.MergepdfFiles as isFinalSignedContractToAttach is required";
        rsignlog.RSignLogInfo(loggerModelNew);
        if (password == null)
        {
          loggerModelNew.Message = "Successfully merged pdf files using asposeHelper.MergepdfFiles as isFinalSignedContractToAttach is required and password not required";
          rsignlog.RSignLogInfo(loggerModelNew);
          finalPathPDF = _asposeHelper.MergepdfFiles(filepath, Path.Combine(finalMergePDFFileDirectory, "Output.pdf")) ?? filepath[0];
          AddDigitalSignatureOnContract(finalPathPDF, null, envelopeObject.ID, true);
          if (envelopeObject.IsWaterMark == true && (!string.IsNullOrEmpty(envelopeObject.WatermarkTextForSender) || !string.IsNullOrEmpty(envelopeObject.WatermarkTextForOther)))
          {
            finalPathPDFSigner = _asposeHelper.MergepdfFiles(filepathSigner, Path.Combine(finalMergePDFFileDirectory, "OutputSigner.pdf")) ?? filepathSigner[0];
            AddDigitalSignatureOnContract(finalPathPDFSigner, null, envelopeObject.ID, true);
          }
          return finalPathPDF;
        }
        string finalpdfPath = _asposeHelper.MergepdfFiles(filepath, Path.Combine(finalMergePDFFileDirectory, "Output.pdf")) ?? filepath[0];
        if (envelopeObject.IsWaterMark == true && (!string.IsNullOrEmpty(envelopeObject.WatermarkTextForSender) || !string.IsNullOrEmpty(envelopeObject.WatermarkTextForOther)))
        {
          string finalpdfPathSigner = _asposeHelper.MergepdfFiles(filepathSigner, Path.Combine(finalMergePDFFileDirectory, "OutputSigner.pdf")) ?? filepathSigner[0];
          finalPathPDFSigner = _asposeHelper.EncryptPdfFile(finalpdfPathSigner, password, password, finalpdfPathSigner);
          AddDigitalSignatureOnContract(finalPathPDFSigner, password, envelopeObject.ID, true);
        }

        loggerModelNew.Message = "Successfully merged pdf files using asposeHelper.MergepdfFiles if isFinalSignedContractToAttach as well as password is required";
        rsignlog.RSignLogInfo(loggerModelNew);
        finalPathPDF = _asposeHelper.EncryptPdfFile(finalpdfPath, password, password, finalpdfPath);
        AddDigitalSignatureOnContract(finalPathPDF, password, envelopeObject.ID, true);
        return finalpdfPath;
      }
      else
      {
        var filepath = new List<string> { documentFolderPath };
        var filepathSigner = new List<string> { finalPathSignerPDF };
        if (envelopeObject.IsSeparateMultipleDocumentsAfterSigningRequired && envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Count() > 1 && string.IsNullOrEmpty(currentStatus) && IsRequiredZip)
        {
          if (envelopeObject.IsWaterMark == true && (!string.IsNullOrEmpty(envelopeObject.WatermarkTextForSender) || !string.IsNullOrEmpty(envelopeObject.WatermarkTextForOther)))
          {
            string finalPathSigner = GetFinalSeperateMultiDocZipApiSigner(envelopeObject, filepathSigner, password, dirPath);
          }
          return GetFinalSeperateMultiDocZipApi(envelopeObject, filepath, password, dirPath);
        }
        var finalMergePDFFileDirectory = dirPath + "/" + envelopeObject.ID + currentStatus + "/Final";

        loggerModelNew.Message = "Creating the final document directory as isFinalSignedContractToAttach is not required ";
        rsignlog.RSignLogInfo(loggerModelNew);
        if (password == null)
        {
          if (!System.IO.Directory.Exists(finalMergePDFFileDirectory))
          {
            System.IO.Directory.CreateDirectory(finalMergePDFFileDirectory);

            loggerModelNew.Message = "Successfully created directory if password not required";
            rsignlog.RSignLogInfo(loggerModelNew);
          }
          System.IO.File.Copy(documentFolderPath, Path.Combine(finalMergePDFFileDirectory, "Output.pdf"), true);
          finalPathPDF = Path.Combine(finalMergePDFFileDirectory, "Output.pdf");
          AddDigitalSignatureOnContract(finalPathPDF, null, envelopeObject.ID, true);
          if (envelopeObject.IsWaterMark == true && (!string.IsNullOrEmpty(envelopeObject.WatermarkTextForSender) || !string.IsNullOrEmpty(envelopeObject.WatermarkTextForOther)))
          {
            System.IO.File.Copy(finalPathSignerPDF, Path.Combine(finalMergePDFFileDirectory, "OutputSigner.pdf"), true);
            finalPathPDFSigner = Path.Combine(finalMergePDFFileDirectory, "OutputSigner.pdf");
            AddDigitalSignatureOnContract(finalPathPDFSigner, null, envelopeObject.ID, true);
          }
          return finalPathPDF;
        }
        if (!System.IO.Directory.Exists(finalMergePDFFileDirectory))
        {
          System.IO.Directory.CreateDirectory(finalMergePDFFileDirectory);

          loggerModelNew.Message = "Successfully created directory if password required";
          rsignlog.RSignLogInfo(loggerModelNew);
        }
        System.IO.File.Copy(documentFolderPath, Path.Combine(finalMergePDFFileDirectory, "Output.pdf"), true);
        finalPathPDF = Path.Combine(finalMergePDFFileDirectory, "Output.pdf");
        _asposeHelper.EncryptPdfFile(finalPathPDF, password, password, finalPathPDF);
        AddDigitalSignatureOnContract(finalPathPDF, password, envelopeObject.ID, true);
        if (envelopeObject.IsWaterMark == true && (!string.IsNullOrEmpty(envelopeObject.WatermarkTextForSender) || !string.IsNullOrEmpty(envelopeObject.WatermarkTextForOther)))
        {
          System.IO.File.Copy(finalPathSignerPDF, Path.Combine(finalMergePDFFileDirectory, "OutputSigner.pdf"), true);
          finalPathPDFSigner = Path.Combine(finalMergePDFFileDirectory, "OutputSigner.pdf");
          _asposeHelper.EncryptPdfFile(finalPathPDFSigner, password, password, finalPathPDFSigner);
          AddDigitalSignatureOnContract(finalPathPDFSigner, password, envelopeObject.ID, true);
        }
        loggerModelNew.Message = "Returning Final PDF Path :" + finalPathPDF.ToString();
        rsignlog.RSignLogInfo(loggerModelNew);
        return finalPathPDF;
      }
    }
    private List<DocumentControls> GetDocumentControlsForCC(Envelope envelopeObject) // changes done to send doc to cc
    {
      string currentMethod = "GetDocumentControlsForCC";
      loggerModelNew = new LoggerModelNew("", "ApiHelpers", currentMethod, "Initiate the process for getting document controls for CC ", envelopeObject.ID.ToString(), "", "", "", "API");
      rsignlog.RSignLogInfo(loggerModelNew);

      var conditionalList = envelopeObject.ConditionalControlMappingList
                            .Where(e => e.ParentId != null && e.RuleId != null)
                            .Select(e => e.DocumentControlId).ToList();

      var documentContentsData = (from recp in envelopeObject.Recipients
                                  join doc in envelopeObject.Documents.SelectMany(d => d.DocumentContents) on recp.ID equals doc.RecipientID
                                  where doc.IsControlDeleted == false
                                 && !conditionalList.Contains(doc.ID)
                                  select doc).ToList();

      var labelControls = envelopeObject.Documents.SelectMany(dc => dc.DocumentContents).Where(dc => dc.RecipientID == null && dc.IsControlDeleted == false && !conditionalList.Contains(dc.ID)).ToList();

      if (labelControls.Any())
        documentContentsData.AddRange(labelControls);

      var docContentDependentControldata = (from controlMappingList in envelopeObject.ConditionalControlMappingList
                                            join doc in envelopeObject.Documents.SelectMany(d => d.DocumentContents) on controlMappingList.ParentId equals doc.ID
                                            where doc.IsControlDeleted == false && (doc.ControlValue.ToLower() == "true" || doc.ControlValue.ToLower() == "signed")
                                            && (doc.ControlID == Constants.Control.Checkbox || doc.ControlID == Constants.Control.Radio)
                                            select controlMappingList.DocumentControlId).ToList();

      var dependentControl = envelopeObject.Documents.SelectMany(c => c.DocumentContents).Where(c => docContentDependentControldata.Contains(c.ID)).ToList();

      if (dependentControl.Any())
        documentContentsData.AddRange(dependentControl);

      var signerlist = envelopeObject.Recipients.Where(x => x.RecipientTypeID == Constants.RecipientType.Prefill).ToList();
      if (signerlist == null)
      {
        var controls = documentContentsData.Select(control => new DocumentControls
        {
          DocumentName = control.Documents.DocumentName,
          ControlHhtmlId = control.ControlHtmlID,
          ControlName = GetControlNameById(control.ControlID),
          ControlType = GetControlTypeById(control.ControlType),
          XLeftposition = control.XCoordinate.GetValueOrDefault(),
          YTopPosition = control.YCoordinate.GetValueOrDefault(),
          ZBottompostion = control.ZCoordinate.GetValueOrDefault(),
          PageNo = control.PageNo.GetValueOrDefault(),
          DocumentPageNo = control.DocumentPageNo.GetValueOrDefault(),
          Label = control.Label,
          Width = control.Width.GetValueOrDefault(),
          Height = control.Height.GetValueOrDefault(),
          Required = control.Required,
          IsFixedWidth = control.IsFixedWidth == null ? true : Convert.ToBoolean(control.IsFixedWidth) ? true : false,
          IsTypeSignature = (control.ControlID == Constants.Control.Signature) ? IsTypeSignature(control) : (bool?)null,
          IsUploadSignature = (control.ControlID == Constants.Control.Signature) ? IsUploadSignature(control) : (bool?)null,
          IsHandSignature = (control.ControlID == Constants.Control.Signature) ? IsHandSignature(control) : (bool?)null,
          ImageBytes =
               (control.ControlID == Constants.Control.Checkbox || control.ControlID == Constants.Control.Radio)
                  ? GetControlImage(control) : null,
          UserControlStyle = control.ControlStyle == null ? null : new UserControlStyle
          {
            FontColor = control.ControlStyle.FontColor,
            FontName = _lookupRepository.GetLookup(Lookup.Fonts).Where(f => f.Key == control.ControlStyle.FontID.ToString()).Select(f => f.Value).FirstOrDefault(),
            FontSize = control.ControlStyle.FontSize,
            IsBold = control.ControlStyle.IsBold,
            IsItalic = control.ControlStyle.IsItalic,
            IsUnderline = control.ControlStyle.IsUnderline
          },
          UserSelectControl = control.ControlOptions == null ? null : control.ControlOptions.Select(options => new UserSelectControl
          {
            OptionText = options.OptionText, //  IsSelected = options.IsSelected,   /* Commented after EDMX CHAGNES
            Order = options.Order
          }).ToList()
        }).ToList();

        loggerModelNew.Message = "Successfully retrieved " + controls != null ? controls.Count.ToString() : "0" + " document controls for CC (signerlist = null)";
        rsignlog.RSignLogInfo(loggerModelNew);
        return controls;
      }
      else
      {
        var controls = documentContentsData.Select(control => new DocumentControls
        {
          DocumentName = control.Documents.DocumentName,
          ControlHhtmlId = control.ControlHtmlID,
          XLeftposition = control.XCoordinate.GetValueOrDefault(),
          YTopPosition = control.YCoordinate.GetValueOrDefault(),
          ZBottompostion = control.ZCoordinate.GetValueOrDefault(),
          PageNo = control.PageNo.GetValueOrDefault(),
          DocumentPageNo = control.DocumentPageNo.GetValueOrDefault(),
          Label = control.Label,
          Required = control.Required,
          ControlValue = (control.ControlID == Constants.Control.DropDown) ?
            !string.IsNullOrEmpty(control.ControlValue) ? control.SelectControlOptions.Where(d => d.ID == Guid.Parse(control.ControlValue)).Select(d => d.OptionText).FirstOrDefault() : control.ControlValue : control.ControlValue,
          ImageBytes =
                (control.ControlID == Constants.Control.Checkbox || control.ControlID == Constants.Control.Radio || (control.ControlID == Constants.Control.Signature && control.ControlValue == "Signed") || (control.ControlID == Constants.Control.NewInitials && control.ControlValue == "Signed"))
                    ? GetControlImage(control) : null,
          ControlName = GetControlNameById(control.ControlID),
          Height = control.Height.GetValueOrDefault(),
          Width = control.Width.GetValueOrDefault(),
          IsFixedWidth = control.IsFixedWidth == null ? true : Convert.ToBoolean(control.IsFixedWidth) ? true : false,
          IsTypeSignature = (control.ControlID == Constants.Control.Signature) ? IsTypeSignature(control) : (bool?)null,
          IsUploadSignature = (control.ControlID == Constants.Control.Signature) ? IsUploadSignature(control) : (bool?)null,
          IsHandSignature = (control.ControlID == Constants.Control.Signature) ? IsHandSignature(control) : (bool?)null,
          UserControlStyle = control.ControlStyle == null ? null : new UserControlStyle
          {
            FontColor = control.ControlStyle.FontColor,
            FontName = _lookupRepository.GetLookup(Lookup.Fonts).Where(f => f.Key == control.ControlStyle.FontID.ToString()).Select(f => f.Value).FirstOrDefault(),
            FontSize = control.ControlStyle.FontSize,
            IsBold = control.ControlStyle.IsBold,
            IsItalic = control.ControlStyle.IsItalic,
            IsUnderline = control.ControlStyle.IsUnderline,
          },
          UserSelectControl = control.SelectControlOptions == null ? null : control.SelectControlOptions.Select(options => new UserSelectControl
          {
            OptionText = options.OptionText,
            Order = options.Order
          }).ToList()
        }).ToList();

        loggerModelNew.Message = "Successfully retrieved " + controls.Count.ToString() + " document controls for CC (signerlist is not null)";
        rsignlog.RSignLogInfo(loggerModelNew);
        return controls;
      }
    }
    private List<DocumentControls> GetDocumentControlsForSigningCertifate(Envelope envelopeObject)
    {
      loggerModelNew = new LoggerModelNew("", "ApiHelpers", "GetDocumentControlsForSigningCertifate", "Initiate the process for getting document controls for Signer Certificate ", envelopeObject.EDisplayCode.ToString(), "", "", "", "API");
      rsignlog.RSignLogInfo(loggerModelNew);

      var documentContentsData = new List<DocumentContents>();
      using (var dbContext = new RSignDbContext(_configuration))
      {
        documentContentsData = (from recp in envelopeObject.Recipients
                                join doc in envelopeObject.Documents.SelectMany(d => d.DocumentContents) on recp.ID equals doc.RecipientID
                                where dbContext.SignerStatus.Where(s => s.RecipientID == recp.ID).OrderByDescending(s => s.CreatedDateTime).Select(s => s.StatusID).FirstOrDefault() == Constants.StatusCode.Signer.Signed && doc.IsControlDeleted == false
                                select doc).ToList();
      }

      //var documentContentsData = (from recp in envelopeObject.Recipients
      //                            join doc in envelopeObject.Documents.SelectMany(d => d.DocumentContents) on recp.ID equals doc.RecipientID
      //                            where recp.SignerStatus.OrderByDescending(s => s.CreatedDateTime).Select(s => s.StatusID).FirstOrDefault() == Constants.StatusCode.Signer.Signed && doc.IsControlDeleted == false
      //                            select doc).ToList();

      var labelControls = envelopeObject.Documents.SelectMany(dc => dc.DocumentContents).Where(dc => dc.RecipientID == null && dc.IsControlDeleted == false).ToList();

      if (labelControls.Any())
        documentContentsData.AddRange(labelControls);

      if (documentContentsData != null && documentContentsData.Count > 0)
      {
        List<Guid?> RecipientIds = documentContentsData.Select(r => r.RecipientID).Distinct().ToList();
        List<string> RecipientEmailIds = envelopeObject.Recipients.Where(a => RecipientIds.Contains(a.ID)).Select(a => a.EmailAddress).ToList();
        List<Guid> SameRecipientIds = envelopeObject.Recipients.Where(a => RecipientEmailIds.Contains(a.EmailAddress) && !RecipientIds.Contains(a.ID) && a.IsSameRecipient == true).Select(a => a.ID).ToList();

        if (SameRecipientIds != null && SameRecipientIds.Count > 0)
        {
          var signerControls = envelopeObject.Documents.SelectMany(dc => dc.DocumentContents).Where(dc => dc.RecipientID != null && dc.RecipientID != Guid.Empty && SameRecipientIds.Contains(dc.RecipientID.Value) && dc.IsControlDeleted == false).ToList();
          if (signerControls.Any())
            documentContentsData.AddRange(signerControls);
        }
      }

      var controls = documentContentsData.Select(control => new DocumentControls
      {
        DocumentName = control.Documents.DocumentName,
        ControlHhtmlId = control.ControlHtmlID,
        XLeftposition = control.XCoordinate.GetValueOrDefault(),
        YTopPosition = control.YCoordinate.GetValueOrDefault(),
        ZBottompostion = control.ZCoordinate.GetValueOrDefault(),
        PageNo = control.PageNo.GetValueOrDefault(),
        DocumentPageNo = control.DocumentPageNo.GetValueOrDefault(),
        Label = control.Label,
        Required = control.Required,
        ControlValue = (control.ControlID == Constants.Control.DropDown) ?
          (!string.IsNullOrEmpty(control.ControlValue) ? control.SelectControlOptions.Where(d => d.ID == Guid.Parse(control.ControlValue)).Select(d => d.OptionText).FirstOrDefault() : control.ControlValue) :
          (control.ControlID == Constants.Control.Hyperlink) ? control.SenderControlValue : control.ControlValue,
        ImageBytes =
              (control.ControlID == Constants.Control.Checkbox || control.ControlID == Constants.Control.Radio || control.ControlID == Constants.Control.Signature || control.ControlID == Constants.Control.NewInitials)
                  ? GetControlImage(control) : null,
        ControlName = GetControlNameById(control.ControlID),
        ControlType = GetControlTypeById(control.ControlType),
        Height = control.Height.GetValueOrDefault(),
        Width = control.Width.GetValueOrDefault(),
        IsFixedWidth = control.IsFixedWidth == null ? true : Convert.ToBoolean(control.IsFixedWidth) ? true : false,
        IsTypeSignature = (control.ControlID == Constants.Control.Signature) ? IsTypeSignature(control) : (bool?)null,
        IsUploadSignature = (control.ControlID == Constants.Control.Signature) ? IsUploadSignature(control) : (bool?)null,
        IsHandSignature = (control.ControlID == Constants.Control.Signature) ? IsHandSignature(control) : (bool?)null,
        UserControlStyle = control.ControlStyle == null ? null : new UserControlStyle
        {
          FontColor = control.ControlStyle.FontColor,
          FontName = _lookupRepository.GetLookup(Lookup.Fonts).Where(f => f.Key == control.ControlStyle.FontID.ToString()).Select(f => f.Value).FirstOrDefault(),
          FontSize = control.ControlStyle.FontSize,
          IsBold = control.ControlStyle.IsBold,
          IsItalic = control.ControlStyle.IsItalic,
          IsUnderline = control.ControlStyle.IsUnderline,
        },
        UserSelectControl = control.SelectControlOptions == null ? null : control.SelectControlOptions.Select(options => new UserSelectControl
        {
          OptionText = options.OptionText,
          Order = options.Order
        }).ToList()
      }).ToList();

      loggerModelNew.Message = "Successfully retrieved " + controls.Count.ToString() + " document controls for Signer Certificate";
      rsignlog.RSignLogInfo(loggerModelNew);
      return controls;
    }
    public string GetControlNameById(Guid controlGuid)
    {
      var controls = _lookupRepository.GetLookup(Lookup.Controls);
      return controls.First(cn => Guid.Parse(cn.Key) == controlGuid).Value;
    }
    public string GetControlTypeById(Guid? controlGuid)
    {
      if (controlGuid == null || controlGuid == Guid.Empty)
        return "Text";

      var controls = _lookupRepository.GetLookup(Lookup.TextType);
      return controls.First(cn => Guid.Parse(cn.Key) == controlGuid).Value;
    }
    public bool IsTypeSignature(DocumentContents control)
    {
      bool isTypeSignature = false;
      using (var dbContext = new RSignDbContext(_configuration))
      {
        if (control.RecipientID != null)
        {
          var signerSignature = _recipientRepository.GetSignerSignature(control.RecipientID.Value);
          isTypeSignature = signerSignature != null && signerSignature.SignatureTypeId == Constants.SignatureType.Auto;
        }
      }
      return isTypeSignature;
    }
    public bool IsHandSignature(DocumentContents control)
    {
      bool isHandSignature = false;
      using (var dbContext = new RSignDbContext(_configuration))
      {
        if (control.RecipientID != null)
        {
          var signerSignature = _recipientRepository.GetSignerSignature(control.RecipientID.Value);
          isHandSignature = signerSignature != null && signerSignature.SignatureTypeId == Constants.SignatureType.Hand;
        }
      }
      return isHandSignature;
    }
    public bool IsUploadSignature(DocumentContents control)
    {
      bool isUploadSignature = false;
      using (var dbContext = new RSignDbContext(_configuration))
      {
        if (control.RecipientID != null)
        {
          var signerSignature = _recipientRepository.GetSignerSignature(control.RecipientID.Value);
          isUploadSignature = signerSignature != null && signerSignature.SignatureTypeId == Constants.SignatureType.UploadSignature;
        }
      }
      return isUploadSignature;
    }
    public int GetHeaderFooterSettingOption(Guid userID)
    {
      int intHFOption = Constants.HeaderFooterSettings.LeftFooter;
      using (var dbContext = new RSignDbContext(_configuration))
      {
        var settingsDetails = _settingsRepository.GetEntityForByKeyConfig(userID, Constants.SettingsKeyConfig.HeaderFooterOptionSettings);
        if (settingsDetails != null)
          intHFOption = string.IsNullOrEmpty(settingsDetails.OptionValue) ? Constants.HeaderFooterSettings.LeftFooter : Convert.ToInt32(settingsDetails.OptionValue);
      }
      return intHFOption;
    }
    public string NameWithoutExtenionsApi(string fileName)
    {
      return fileName.Substring(0, fileName.LastIndexOf('.'));
    }
    public int getPDFPageCountApi(string filePath)
    {
      return _asposeHelper.GetPagecountOfPdf(filePath);
    }
    public string GetFinalSeperateMultiDocZipApiSigner(Envelope envelopeObject, List<string> mergedPDFFile, string password, string envelopeDirectoryUNCPath, bool isWatermark = false)
    {
      loggerModelNew = new LoggerModelNew("", "API Helper", "GetFinalSeperateMultiDocZipApiSigner", "Process is started for Get Final Seperate Multi Doc Zip Api Signer", Convert.ToString(envelopeObject.ID), "", "", "", "API");
      rsignlog.RSignLogInfo(loggerModelNew);

      Dictionary<string, int> convertedFileNames = new Dictionary<string, int>();
      var dirPath = envelopeDirectoryUNCPath;
      string ConvertedFilePath = Path.Combine(dirPath, Convert.ToString(envelopeObject.ID), "Converted");
      if (Directory.Exists(ConvertedFilePath))
      {
        var allConvertedFiles = Directory.GetFiles(ConvertedFilePath);
        foreach (var doc in envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).OrderBy(o => o.Order))
        {
          for (int i = 0; i < allConvertedFiles.Length; i++)
          {
            if (NameWithoutExtenionsApi(Path.GetFileName(allConvertedFiles[i])) == NameWithoutExtenionsApi(doc.DocumentName))
              convertedFileNames.Add(NameWithoutExtenionsApi(doc.DocumentName), this.getPDFPageCountApi(allConvertedFiles[i]));
          }
        }
      }
      string FinalPDF = Path.Combine(dirPath, Convert.ToString(envelopeObject.ID), "FinalZipPDF");
      if (Directory.Exists(FinalPDF))
      {
        //var file = Directory.GetFiles(FinalPDF, "*", SearchOption.AllDirectories).FirstOrDefault();
        //if (file != null)
        //{
        //    string signerattchzip = Path.Combine(FinalPDF, "ContractsSigner.zip");
        //    if (file.Contains("ContractsSigner.zip"))
        //    {
        //        System.IO.File.Delete(signerattchzip);
        //    }
        //}

        var file = Directory.GetFiles(FinalPDF, "*.zip", SearchOption.AllDirectories);//.FirstOrDefault();
        if (file != null && file.Length > 0)
        {
          foreach (var item in file)
          {
            System.IO.File.Delete(item);
          }
        }
      }
      string documentPaths = string.Empty;
      documentPaths = _asposeHelper.SplitPdfFiles(convertedFileNames, Convert.ToBoolean(envelopeObject.IsFinalCertificateReq), mergedPDFFile, FinalPDF, Convert.ToBoolean(envelopeObject.PasswordReqdtoOpen), password);

      List<string> files = new List<string>();
      files = DirSearch(documentPaths);
      foreach (var f in files)
        AddDigitalSignatureOnContract(f, password, envelopeObject.ID);
      if (files.Count > 0)
      {

        using (var zip = new Ionic.Zip.ZipFile())
        {
          zip.AddFiles(files, false, "");
          //zip.AddDirectory(@"D:\SignerDoc\d61536b2-4f52-4820-8ae4-a4414d0015c6", "rootInZipFile");
          zip.Save(documentPaths + "\\" + "ContractsSigner.zip");
        }
        //string fileEntries = Directory.GetFiles(documentPaths, "*-*.zip").FirstOrDefault(); ; 
      }
      string finalCintractZipFilePath = Path.Combine(documentPaths, "ContractsSigner.zip");
      return finalCintractZipFilePath;
    }
    public int GetDigitalSignatureSetting(Guid envelopeID)
    {
      int digitalCertificate = Constants.DigitalCertificate.Default;
      if (envelopeID != Guid.Empty)
      {
        using (var dbContext = new RSignDbContext(_configuration))
        {
          EnvelopeSettingsDetail details = dbContext.EnvelopeSettingsDetail.Where(q => q.EnvelopeId == envelopeID).FirstOrDefault();
          if (details != null)
          {
            digitalCertificate = string.IsNullOrEmpty(Convert.ToString(details.DigitalCertificate)) ? Constants.DigitalCertificate.Default : Convert.ToInt32(details.DigitalCertificate);
          }
        }
      }
      return digitalCertificate;
    }
    public bool AddDigitalSignatureOnContract(string srcPDFWithFullPath, string srcPDFPassword, Guid UserID, bool IsThrowException = false)
    {
      string tempFilePath = string.Empty;
      try
      {
        loggerModelNew = new LoggerModelNew("", "ApiHelper", "AddDigitalSignatureOnContract", "Initiate the process for Add Digital Signature on Contract", "", "", "", "", "API");
        rsignlog.RSignLogInfo(loggerModelNew);

        iText7Helper iTextHelper = new iText7Helper(_appConfiguration);

        if (_asposeHelper.IsFileEligibleForDigitallySign(srcPDFWithFullPath, srcPDFPassword))
        {
          var dir = System.IO.Path.GetDirectoryName(srcPDFWithFullPath);
          tempFilePath = (dir + "\\" + System.IO.Path.GetFileNameWithoutExtension(srcPDFWithFullPath) + "_T.pdf");
          System.IO.File.Move(srcPDFWithFullPath, tempFilePath);
          if (GetDigitalSignatureSetting(UserID) == Constants.DigitalCertificate.RpostLabsInternal)
            iTextHelper.AddDigitalSignatureToTheDocumentIN(tempFilePath,
            (string.IsNullOrEmpty(srcPDFPassword) ? string.Empty : srcPDFPassword), srcPDFWithFullPath, IsThrowException);

          else
            iTextHelper.AddDigitalSignatureToTheDocument(tempFilePath,
            (string.IsNullOrEmpty(srcPDFPassword) ? string.Empty : srcPDFPassword), srcPDFWithFullPath, IsThrowException);
          if (File.Exists(tempFilePath))
            File.Delete(tempFilePath);
          return true;
        }

        loggerModelNew.Message = "Process completed for Add Digital Signature on Contract";
        rsignlog.RSignLogInfo(loggerModelNew);
        return false;
      }
      catch (Exception ex)
      {
        loggerModelNew.Message = ex.Message;
        rsignlog.RSignLogError(loggerModelNew, ex);

        if (File.Exists(tempFilePath))
          File.Delete(tempFilePath);

        if (IsThrowException)
        {
          throw ex;
        }
        else
        {
          return false;
        }
      }
    }
    public string GetFinalSeperateMultiDocZipApi(Envelope envelopeObject, List<string> mergedPDFFile, string password, string envelopeDirectoryUNCPath)
    {
      loggerModelNew = new LoggerModelNew("", "API Helper", "GetFinalSeperateMultiDocZipApi", "Process is started for Get Final Seperate Multi Doc Zip Api", Convert.ToString(envelopeObject.ID), "", "", "", "API");
      rsignlog.RSignLogInfo(loggerModelNew);

      Dictionary<string, int> convertedFileNames = new Dictionary<string, int>();
      var dirPath = envelopeDirectoryUNCPath;
      string ConvertedFilePath = Path.Combine(dirPath, Convert.ToString(envelopeObject.ID), "Converted");
      if (Directory.Exists(ConvertedFilePath))
      {
        var allConvertedFiles = Directory.GetFiles(ConvertedFilePath);
        foreach (var doc in envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).OrderBy(o => o.Order))
        {
          for (int i = 0; i < allConvertedFiles.Length; i++)
          {
            if (NameWithoutExtenionsApi(Path.GetFileName(allConvertedFiles[i])) == NameWithoutExtenionsApi(doc.DocumentName))
              convertedFileNames.Add(NameWithoutExtenionsApi(doc.DocumentName), this.getPDFPageCountApi(allConvertedFiles[i]));
          }
        }
      }
      string FinalPDF = Path.Combine(dirPath, Convert.ToString(envelopeObject.ID), "FinalZipPDF");
      if (Directory.Exists(FinalPDF))
      {
        //var file = Directory.GetFiles(FinalPDF, "*", SearchOption.AllDirectories).FirstOrDefault();
        //if (file != null)
        //{
        //    string signerattchzip = Path.Combine(FinalPDF, "Contracts.zip");
        //    if (file.Contains("Contracts.zip"))
        //    {
        //        System.IO.File.Delete(signerattchzip);
        //    }
        //}

        var file = Directory.GetFiles(FinalPDF, "*.zip", SearchOption.AllDirectories);//.FirstOrDefault();
        if (file != null && file.Length > 0)
        {
          foreach (var item in file)
          {
            if (item.Contains("Contracts.zip"))
            {
              System.IO.File.Delete(item);
            }
          }
        }
      }
      string documentPaths = string.Empty;
      documentPaths = _asposeHelper.SplitPdfFiles(convertedFileNames, Convert.ToBoolean(envelopeObject.IsFinalCertificateReq), mergedPDFFile, FinalPDF, Convert.ToBoolean(envelopeObject.PasswordReqdtoOpen), password);

      List<string> files = new List<string>();
      files = DirSearch(documentPaths, Convert.ToBoolean(envelopeObject.IsWaterMark));
      foreach (var f in files)
        AddDigitalSignatureOnContract(f, password, envelopeObject.ID);
      if (files.Count > 0)
      {
        using (var zip = new Ionic.Zip.ZipFile())
        {
          zip.AddFiles(files, false, "");
          //zip.AddDirectory(@"D:\SignerDoc\d61536b2-4f52-4820-8ae4-a4414d0015c6", "rootInZipFile");
          zip.Save(documentPaths + "\\" + "Contracts.zip");
        }
        //string fileEntries = Directory.GetFiles(documentPaths, "*-*.zip").FirstOrDefault(); ; 
      }
      string finalCintractZipFilePath = Path.Combine(documentPaths, "Contracts.zip");
      return finalCintractZipFilePath;
    }
    private byte[] GetSigningCertificate(Envelope envelopeObject, string documenthash, string currentStatus)
    {
      loggerModelNew = new LoggerModelNew("", "API Helper", "GetSigningCertificate", "Initiate the process for creating Signing Certificate for envelope", envelopeObject.EDisplayCode.ToString(), "", "", "", "API");
      rsignlog.RSignLogInfo(loggerModelNew);

      // signing certificate
      var recipientList = new List<AsposeRecipient>();
      var AsposeDropdownOptionList = new List<AsposeDropdownOptions>();
      var recipientWithWaitingForSignatureStatus = new List<AsposeRecipientDetails>();

      /* Get User Settings */
      APISettings apiSettings = _settingsRepository.GetEntityByParam(envelopeObject.UserID, string.Empty, Constants.String.SettingsType.User);
      var userSettings = _eSignHelper.TransformSettingsDictionaryToEntity(apiSettings);
      var recipientsDetailsOnCertificate = _eSignHelper.GetRecipientHistoryListForManage(envelopeObject.ID);
      string SignerName = string.Empty;
      string SignerEmail = string.Empty;
      UserProfile userProfile = new UserProfile();
      EnvelopeSettingsDetail envelopeSettingsDetail = _eSignHelper.GetEnvelopeSettingsDetail(envelopeObject.ID);

      List<Guid> recipientIds = envelopeObject.Recipients.Where(r => r.IsSameRecipient != true).Select(r => r.ID).ToList();
      var copySignerStatusList = _recipientRepository.GetCopySignerStatusAllrecipients(recipientIds);
      List<UserProfile> userlist = new List<UserProfile>();
      List<string> signerEmailIds = copySignerStatusList.Select(s => s.SignedBy).ToList();
      userlist = _userRepository.GetUserProfileByEmailIDs(signerEmailIds);

      string envelopeStatusDescription = _lookupRepository.GetLookupMasterLanguage(Lookup.MasterLanguageKeyDetails, envelopeObject.CultureInfo, Lookup.EnvelopeStatus.ToString())
                          .Single(s => s.Key == Constants.StatusCode.Envelope.Waiting_For_Signature).Value;

      List<SignerStatus> signerStatusList = _recipientRepository.GetStatusList(recipientIds);
      var lookupRecipientTypeList = _lookupRepository.GetLookup(Lookup.RecipientType);
      var lookupSignerStatusList = _lookupRepository.GetLookup(Lookup.SignerStatus);
      List<SignerSignature> signerSignaturesList = _recipientRepository.GetSignerSignatureList(recipientIds);


      foreach (Recipients recipient in envelopeObject.Recipients.Where(r => r.IsSameRecipient != true).OrderBy(o => o.Order).ThenBy(o => o.CreatedDateTime))
      {
        if (recipient.RecipientTypeID == Constants.RecipientType.Signer)
        {
          var copySignerStatus = copySignerStatusList != null ? copySignerStatusList.Where(s => s.RecipientID == recipient.ID).FirstOrDefault() : null;//
          SignerEmail = copySignerStatus != null ? copySignerStatus.SignedBy : string.Empty;
          //userProfile = !string.IsNullOrEmpty(SignerEmail) ? userRepository.GetUserProfileByEmailID(SignerEmail) : null;
          userProfile = !string.IsNullOrEmpty(SignerEmail) && userlist != null ? userlist.Where(u => u.EmailID == SignerEmail).FirstOrDefault() : null;
          SignerName = userProfile != null ? userProfile.FirstName + " " + userProfile.LastName : (!string.IsNullOrEmpty(SignerEmail) ? SignerEmail.Split('@')[0] : string.Empty);

          var asposeSignerRecipient = new AsposeRecipient
          {
            ID = recipient.ID,
            EmailAddress = !string.IsNullOrEmpty(SignerEmail) ? (SignerEmail) : recipient.EmailAddress,
            Name = !string.IsNullOrEmpty(SignerEmail) ? (SignerName) : recipient.Name,
            RecipientTypeID = recipient.RecipientTypeID,
            //RecipientType =
            //    _lookupRepository.GetLookup(Lookup.RecipientType)
            //                    .Where(s => s.Key == recipient.RecipientTypeID.ToString())
            //                    .Select(s => s.Value)
            //                    .First(),
            RecipientType = lookupRecipientTypeList
                                      .Where(s => s.Key == recipient.RecipientTypeID.ToString())
                                      .Select(s => s.Value)
                                      .First(),
            DisplayOrder = recipient.RecipientTypeID == Constants.RecipientType.Sender ? 0 : recipient.RecipientTypeID == Constants.RecipientType.Prefill ? 1 : recipient.RecipientTypeID == Constants.RecipientType.Signer ? 2 : 3,  // V2 Team Prefill Change
            IsSameRecipient = recipient.IsSameRecipient,
            CcSignerType = recipient.CCSignerType,
            MobileNumber = recipient.Mobile,
            DeliveryMode = recipient.DeliveryMode,
            DialCode = recipient.DialCode
          };
          var signerStatus = signerStatusList.Count > 0 ? signerStatusList.Where(s => s.RecipientID == recipient.ID).OrderByDescending(s => s.CreatedDateTime).FirstOrDefault() : null;// recipientRepository.GetStatus(recipient.ID);

          if (signerStatus != null)
          {
            asposeSignerRecipient.StatusID = signerStatus.StatusID;
            asposeSignerRecipient.Status = lookupSignerStatusList != null ? lookupSignerStatusList.Where(r => r.Key == signerStatus.StatusID.ToString()).Select(r => r.Value).First() : _lookupRepository.GetLookup(Lookup.SignerStatus).Where(r => r.Key == signerStatus.StatusID.ToString()).Select(r => r.Value).First();
            asposeSignerRecipient.SignerIPAddress = signerStatus.IPAddress;
            asposeSignerRecipient.StatusDate = signerStatus.CreatedDateTime;
            asposeSignerRecipient.DelegatedRecipientID = signerStatus.DelegateTo;
            asposeSignerRecipient.CreatedDateTime = signerStatus.CreatedDateTime;
            if (signerStatus.StatusID == Constants.StatusCode.Signer.Pending || signerStatus.StatusID == Constants.StatusCode.Signer.Viewed)
            {
              recipientWithWaitingForSignatureStatus.Add(new AsposeRecipientDetails
              {
                Name = recipient.Name,
                Email = recipient.EmailAddress,
                DialCode = recipient.DialCode,
                MobileNumber = recipient.Mobile,
                IPAddress = "-",
                Status = "Waiting for Signature",
                DeliveryMode = recipient.DeliveryMode,
                //StatusDescription = lookupRepository.GetLookupMasterLanguage(Lookup.MasterLanguageKeyDetails, envelopeObject.CultureInfo, Lookup.EnvelopeStatus.ToString())
                //.Single(s => s.Key == Constants.StatusCode.Envelope.Waiting_For_Signature).Value
                StatusDescription = envelopeStatusDescription
              });
            }
          }

          SignerSignature signerSignature = signerSignaturesList != null ? signerSignaturesList.Where(s => s.RecipientID == recipient.ID).FirstOrDefault() : _recipientRepository.GetSignerSignature(recipient.ID);
          if (signerSignature != null)
            asposeSignerRecipient.SignatureBytes = signerSignature.Signature;
          asposeSignerRecipient.InitialBytes = _recipientRepository.GetInitialSignatureValue(recipient.ID);


          recipientList.Add(asposeSignerRecipient);
          //asposeSignerRecipient.RecipientDetails = new List<AsposeRecipientDetails>();
          ////recipient.RecipientHistory = new List<RecipientsDetail>();
          //var recipientHistory = recipientRepository.GetAllRecipientHistory(recipient.ID);
          //foreach (var recHistory in recipientHistory)
          //{
          //    AsposeRecipientDetails asposeRecipientDetails = new AsposeRecipientDetails();
          //    asposeRecipientDetails.Name = recipient.Name;
          //    asposeRecipientDetails.Email = recipient.EmailAddress;
          //    asposeRecipientDetails.IPAddress = recHistory.IPAddress;
          //    asposeRecipientDetails.Status = lookupRepository.GetLookup(Lookup.SignerStatus).Where(r => r.Key == recHistory.StatusTypeID.ToString()).Select(r => r.Value).First();//envelopeRepository.GetSingnerStatus(recHistory.StatusTypeID);
          //    asposeRecipientDetails.CreatedDateTime = recHistory.StatusDateTime;
          //    asposeSignerRecipient.RecipientDetails.Add(asposeRecipientDetails);
          //}
        }
        else
        {
          var asposeRecipient = new AsposeRecipient
          {
            ID = recipient.ID,
            EmailAddress = recipient.EmailAddress + (recipient.RecipientTypeID == Constants.RecipientType.CC && !string.IsNullOrEmpty(recipient.RerouteEmailAddress) ? " (Copied as requested by " + recipient.RerouteEmailAddress + ")" : string.Empty),
            Name = recipient.Name,
            RecipientTypeID = recipient.RecipientTypeID,
            //RecipientType =
            //    lookupRepository.GetLookup(Lookup.RecipientType)
            //                    .Where(s => s.Key == recipient.RecipientTypeID.ToString())
            //                    .Select(s => s.Value)
            //                    .First(),
            RecipientType = lookupRecipientTypeList
                                      .Where(s => s.Key == recipient.RecipientTypeID.ToString())
                                      .Select(s => s.Value)
                                      .First(),
            DisplayOrder =
                      recipient.RecipientTypeID == Constants.RecipientType.Sender
                          ? 0
                           : recipient.RecipientTypeID == Constants.RecipientType.Prefill ? 1 : recipient.RecipientTypeID == Constants.RecipientType.Signer ? 2 : 3,  // V2 Team Prefill Change
            IsSameRecipient = recipient.IsSameRecipient,
            CcSignerType = recipient.CCSignerType,
            MobileNumber = recipient.Mobile,
            DeliveryMode = recipient.DeliveryMode,
            DialCode = recipient.DialCode
          };
          var signerStatus = signerStatusList.Count > 0 ? signerStatusList.Where(s => s.RecipientID == recipient.ID).OrderByDescending(s => s.CreatedDateTime).FirstOrDefault() : null; //recipientRepository.GetStatus(recipient.ID);
          if (signerStatus != null)
          {
            asposeRecipient.StatusID = signerStatus.StatusID;
            asposeRecipient.Status = lookupSignerStatusList != null ? lookupSignerStatusList.Where(r => r.Key == signerStatus.StatusID.ToString()).Select(r => r.Value).First() : _lookupRepository.GetLookup(Lookup.SignerStatus).Where(r => r.Key == signerStatus.StatusID.ToString()).Select(r => r.Value).First();
            asposeRecipient.SignerIPAddress = signerStatus.IPAddress;
            asposeRecipient.StatusDate = signerStatus.CreatedDateTime;
            asposeRecipient.DelegatedRecipientID = signerStatus.DelegateTo;
            asposeRecipient.CreatedDateTime = signerStatus.CreatedDateTime;
          }

          SignerSignature signerSignature = signerSignaturesList != null ? signerSignaturesList.Where(s => s.RecipientID == recipient.ID).FirstOrDefault() : _recipientRepository.GetSignerSignature(recipient.ID);
          if (signerSignature != null)
            asposeRecipient.SignatureBytes = signerSignature.Signature;
          asposeRecipient.InitialBytes = _recipientRepository.GetInitialSignatureValue(recipient.ID);
          recipientList.Add(asposeRecipient);
        }

      }//foreach end
      string AccessAuthTypeId = envelopeObject.AccessAuthType != null ? Convert.ToString(envelopeObject.AccessAuthType) : Convert.ToString(Constants.ConfigurationalProperties.PasswordType.Select);
      if (Convert.ToBoolean(envelopeObject.IsSignerIdentitiy))
      {
        AccessAuthTypeId = Convert.ToString(Constants.ConfigurationalProperties.PasswordType.SignerIdentity);
      }
      if (envelopeObject.PasswordReqdtoSign && envelopeObject.PasswordReqdtoOpen)
      {
        AccessAuthTypeId = Convert.ToString(Constants.ConfigurationalProperties.PasswordType.Endtoend);
      }
      else if (!envelopeObject.PasswordReqdtoSign && envelopeObject.PasswordReqdtoOpen)
      {
        AccessAuthTypeId = Convert.ToString(Constants.ConfigurationalProperties.PasswordType.RequiredToOpenSigned);
      }

      var metaDataAndHistory = _eSignHelper.GetMetaDataAndHistory(_lookupRepository.GetLookupLanguage(Lookup.LanguageKeyDetails, envelopeObject.CultureInfo));
      string AccessAuthType = _settingsRepository.GetSettingsValue(AccessAuthTypeId, Lookup.MasterLanguageKeyDetails, string.Empty, 0, envelopeObject.CultureInfo);
      string EmailAccessCode = ((AccessAuthTypeId == Convert.ToString(Constants.ConfigurationalProperties.PasswordType.Endtoend) || AccessAuthTypeId == Convert.ToString(Constants.ConfigurationalProperties.PasswordType.RequiredToOpenSigned)) && envelopeObject.IsPasswordMailToSigner) ? metaDataAndHistory.Checked : metaDataAndHistory.UnChecked;
      int SendconfirmEmailsCount = Convert.ToBoolean(envelopeObject.IsStatic) ? envelopeObject.Documents.SelectMany(dc => dc.DocumentContents).Where(dc => dc.ControlID == Constants.Control.Email && dc.SenderControlValue == "1").Count() : 0;


      if (envelopeObject.Recipients.Where(r => r.RecipientTypeID == Constants.RecipientType.CC).Count() > 0)
      {
        List<DropdownOptions> CCSignerType = _eSignHelper.GetDropdownOptionsList("CCSignerType", string.IsNullOrEmpty(envelopeObject.CultureInfo) ? "en-US" : envelopeObject.CultureInfo);
        foreach (var items in CCSignerType)
        {
          AsposeDropdownOptions asposeDropdownOptions = new AsposeDropdownOptions();
          asposeDropdownOptions.FieldName = items.FieldName;
          asposeDropdownOptions.OptionName = items.OptionName;
          asposeDropdownOptions.OptionValue = items.OptionValue;
          AsposeDropdownOptionList.Add(asposeDropdownOptions);
        }
      }

      var certificateData = new CertificateData
      {
        Documents = envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Sign).Select(s => s.DocumentName).ToList(),
        FileReviewDocuments = envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Review).Count() > 0 ? envelopeObject.Documents.Where(d => d.ActionType == Constants.ActionTypes.Review).Select(s => s.DocumentName).ToList() : null,
        Recipients = recipientList,
        RecipientsWithWaitingForSignatureStatus = recipientWithWaitingForSignatureStatus,
        EnvelopeCode = "ENV" + envelopeObject.EDisplayCode,
        EnvelopeId = envelopeObject.ID,
        Sender =
                  envelopeObject.Recipients.Where(r => r.RecipientTypeID == Constants.RecipientType.Sender)
                                .Select(r => r.Name)
                                .First(),
        EnvelopeSent = envelopeObject.CreatedDateTime,
        Status = _lookupRepository.GetLookupMasterLanguage(Lookup.MasterLanguageKeyDetails, envelopeObject.CultureInfo, Lookup.EnvelopeStatus.ToString())
                              .Single(s => s.Key == envelopeObject.StatusID).Value,
        EnvelopeCompletedDate = envelopeObject.ModifiedDateTime,
        Subject = envelopeObject.Subject,
        DocHash = documenthash,
        DateFormatId = envelopeObject.DateFormatID,
        MetaDataAndHistory = metaDataAndHistory,
        AllRecipientsHistoryDetails = (from h in recipientsDetailsOnCertificate
                                       where (h.RecipientTypeID == Constants.RecipientType.Signer || h.RecipientTypeID == Guid.Empty || h.RecipientTypeID == Constants.RecipientType.Prefill)
&& h.StatusTypeID != Constants.StatusCode.Signer.Pending && h.StatusTypeID != Constants.StatusCode.Signer.Viewed
                                       select new AsposeRecipientDetails { Name = (!string.IsNullOrEmpty(h.Name) ? h.Name : (!string.IsNullOrEmpty(h.EmailAddress) ? h.EmailAddress.Split('@')[0] : string.Empty)), Email = h.EmailAddress, IPAddress = h.IPAddress, Status = h.Description, CreatedDateTime = h.CreatedDateTime, MobileNumber = h.MobileNumber, DialCode = h.DialCodeNo, DeliveryMode = h.DeliveryMode }).ToList(),
        SenderUpdateHistoryDetails = (from h in recipientsDetailsOnCertificate
                                      where (h.RecipientTypeID == Constants.RecipientType.Sender && h.StatusTypeID == Constants.StatusCode.Signer.Update_And_Resend)
                                      select new AsposeRecipientDetails { Name = h.Name, Email = h.EmailAddress, IPAddress = h.IPAddress, Status = h.Description, CreatedDateTime = h.CreatedDateTime }).ToList(),
        EmailAccessCode = EmailAccessCode,
        AccessAuthentication = AccessAuthType == "Select" ? metaDataAndHistory.None : AccessAuthType,
        EmailVerification = Convert.ToBoolean(envelopeObject.SendConfirmationEmail) ? "Complete" : (Convert.ToBoolean(envelopeObject.IsStatic) && SendconfirmEmailsCount > 0 ? "Complete" : "Not enabled"),
        AsposeDropdownOptions = AsposeDropdownOptionList,
        EnableMessageToMobile = envelopeObject.EnableMessageToMobile,
      };

      var dirPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeObject.ID, string.Empty);
      var certificatepath = dirPath + "/" + envelopeObject.ID + "/" + currentStatus;
      var imagepathLogo = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["Images"]));

      loggerModelNew.Message = "Initiate the process for creating Signing Certificate for envelope using asposeHelper.CreateSignCertificate";
      rsignlog.RSignLogInfo(loggerModelNew);

      string envelopeTimeZone = (envelopeSettingsDetail != null && !string.IsNullOrEmpty(envelopeSettingsDetail.TimeZone)) ? envelopeSettingsDetail.TimeZone : userSettings.SelectedTimeZone;

      byte[] finalCertificateArray = System.IO.File.ReadAllBytes(_asposeHelper.CreateSignCertificate(certificatepath, certificateData, imagepathLogo, envelopeObject.CultureInfo, certificateData.Status, envelopeTimeZone, !string.IsNullOrEmpty(envelopeObject.DisclaimerText),
                                                                  envelopeObject.IsDisclaimerInCertificate, envelopeObject.DisclaimerText, Convert.ToInt32(envelopeObject.DocumentPaperSizeID)));

      _eSignHelper.UpdateSignatureCertificateStore(envelopeObject.ID, true);

      loggerModelNew.Message = "Successfully return final Certificate Array";
      rsignlog.RSignLogInfo(loggerModelNew);
      return finalCertificateArray;
    }
    public string GetRecipientDocumentZip(string documentPaths)
    {
      List<string> files = new List<string>();
      files = DirSearch(documentPaths);
      if (files.Count > 0)
      {
        using (var zip = new Ionic.Zip.ZipFile())
        {
          zip.AddFiles(files, false, "");
          zip.Save(documentPaths + "\\" + "SignerDoc.zip");
        }
      }
      string finalSignerAttachFilePath = Path.Combine(documentPaths, "SignerDoc.zip");
      return finalSignerAttachFilePath;
    }
    public string GetTransperancyDocument(string tempEnvelopeDir, Envelope envelopeObject)
    {
      if (!Directory.Exists(Path.Combine(tempEnvelopeDir, "Final")))
        Directory.CreateDirectory(Path.Combine(tempEnvelopeDir, "Final"));
      string finalTransperancyPath = Path.Combine(tempEnvelopeDir, "Final", "Transperancy.pdf");

      _asposeHelper.CreateBlankPDF(Path.Combine(tempEnvelopeDir, "Output.pdf"), finalTransperancyPath);
      List<DocumentControls> controls = new List<DocumentControls>();
      controls = GetDocumentControlsForSigningCertifate(envelopeObject);
      var pageNumbers = controls.GroupBy(x => new { x.PageNo }).Select(x => x.Key.PageNo).ToList();
      _asposeHelper.AddTextInPdf(finalTransperancyPath, finalTransperancyPath, controls, pageNumbers, Path.Combine(tempEnvelopeDir, "Images"), null, string.Empty, string.Empty, null, envelopeObject.DateFormatID);

      return finalTransperancyPath;
    }


  }
}
