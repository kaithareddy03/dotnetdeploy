using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Data.SqlClient;
using RSign.Models.APIModels;
using RSign.ManageDocument.Models;
using Newtonsoft.Json;
using System.Reflection;
using Aspose.Pdf.Operators;
using System.Net;
using System.Data;

namespace RSign.Models.Helpers
{
    public class ESignHelper : IESignHelper
    {
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IGenericRepository _genericRepository;
        private readonly IModelHelper _modelHelper;
        private readonly IConfiguration _appConfiguration;
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly ISettingsRepository _settingsRepository;
        private bool webApiCall = false;
        public ESignHelper(IOptions<AppSettingsConfig> configuration, IGenericRepository genericRepository, IModelHelper modelHelper,
            IConfiguration appConfiguration, ISettingsRepository settingsRepository)
        {
            _appConfiguration = appConfiguration;
            _configuration = configuration;
            _genericRepository = genericRepository;
            _modelHelper = modelHelper;
            _settingsRepository = settingsRepository;
            rsignlog = new RSignLogger(_appConfiguration);
        }
        public void SetApiCallFlag()
        {
            webApiCall = true;
        }
        public string GetImagesFolderPath(Guid envelopeId, string envelopeFolderUNCPath)
        {
            return GetImagesFolderPath(envelopeId, envelopeFolderUNCPath, false);
        }
        public string GetImagesFolderPath(Guid envelopeId, string envelopeFolderUNCPath, bool isPerminant)
        {
            string imagesDirectory = string.Empty;
            if (isPerminant)
            {
                //imagesDirectory = Path.Combine(GetDocDirectory(envelopeId.ToString()), "Images");
                imagesDirectory = Path.Combine(envelopeFolderUNCPath, "Images");
                if (!Directory.Exists(imagesDirectory))
                    Directory.CreateDirectory(imagesDirectory);
            }
            else
            {
                var dirPath = !string.IsNullOrEmpty(envelopeFolderUNCPath) ? envelopeFolderUNCPath : _modelHelper.GetEnvelopeDirectoryNew(envelopeId, string.Empty);
                imagesDirectory = Path.Combine(dirPath, envelopeId.ToString(), "Images");
                if (!Directory.Exists(imagesDirectory))
                    Directory.CreateDirectory(imagesDirectory);
            }
            return imagesDirectory;
        }
        public string GetEnvelopeImagesFolderPath(Guid envelopeId, string envelopeFolderUNCPath)
        {
            return GetEnvelopeImagesFolderPath(envelopeId, envelopeFolderUNCPath, false);
        }
        public string GetEnvelopeImagesFolderPath(Guid envelopeId, string envelopeFolderUNCPath, bool isPerminant)
        {
            bool IsUnitTestApp1 = Convert.ToBoolean(SessionHelper.Get(SessionKey.IsUnitTestApp));
            string imagesDirectory = string.Empty;
            if (isPerminant)
            {
                imagesDirectory = Path.Combine(GetDocDirectory(envelopeId.ToString()), "Images");
                if (!Directory.Exists(imagesDirectory))
                    Directory.CreateDirectory(imagesDirectory);
            }
            else
            {
                var dirPath = envelopeFolderUNCPath;
                imagesDirectory = Path.Combine(dirPath, envelopeId.ToString(), "Images");
                if (!Directory.Exists(imagesDirectory))
                    Directory.CreateDirectory(imagesDirectory);
            }
            return imagesDirectory;
        }
        public string GetDocDirectory(string folderName)
        {
            var tempDirectory = Path.Combine(_genericRepository.GetDocumentFolderPath(), folderName);
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
        public IList<int> GetFontSize()
        {
            return new List<int> { 8, 10, 11, 12, 14, 16, 18, 24, 36, 48, 60 };
        }
        public string ImageToBase64(Image image)
        {
            using (var ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        public string GetLocalTime(DateTime timeUtc, string SelectedTimeZone, Guid? dateFormatID = null)
        {
            try
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(SelectedTimeZone);
                DateTime cstTime = TimeZoneInfo.ConvertTime(timeUtc, timeZone); ;
                string output = string.Empty, outputDate = string.Empty;
                string timeZoneName = Convert.ToString(timeZone);
                if (timeZone.IsDaylightSavingTime(cstTime))
                {
                    timeZoneName = timeZone.DaylightName;
                }
                else
                {
                    timeZoneName = timeZone.StandardName;
                }
                string[] timeZoneWords = Convert.ToString(timeZoneName).Split(' ');
                foreach (string timeZoneWord in timeZoneWords)
                {
                    if (timeZoneWord[0] != '(')
                    {
                        output += timeZoneWord[0];
                    }
                    else
                    {
                        output += timeZoneWord;
                    }
                }
                if (output == "CUT" || output == "U")
                {
                    output = "UTC";
                }
                else if (output == Constants.TimeZone.WEDT || output == Constants.TimeZone.WEST)
                {
                    output = output == Constants.TimeZone.WEDT ? Constants.TimeZone.WEST : Constants.TimeZone.WET;
                }
                //return Convert.ToString(cstTime) + " " + output;
                if (dateFormatID != null)
                {
                    if (dateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash)
                        outputDate = String.Format("{0:MM/dd/yyyy HH:mm}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan)
                        outputDate = String.Format("{0:MM-dd-yyyy HH:mm}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots)
                        outputDate = String.Format("{0:MM.dd.yyyy HH:mm}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_slash)
                        outputDate = String.Format("{0:dd/MM/yyyy HH:mm}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_colan)
                        outputDate = String.Format("{0:dd-MM-yyyy HH:mm}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_dots)
                        outputDate = String.Format("{0:dd.MM.yyyy HH:mm}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_yyyy_mm_dd_dots)
                        outputDate = String.Format("{0:yyyy.MM.dd. HH:mm}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan)
                        outputDate = String.Format("{0:dd-MMM-yyyy HH:mm}", cstTime);
                }
                else
                    outputDate = Convert.ToString(cstTime);
                return outputDate + " " + output;

            }
            catch (TimeZoneNotFoundException)
            {
                return "The registry does not define the" + SelectedTimeZone;
            }
            catch (InvalidTimeZoneException)
            {
                return "Registry data on the " + SelectedTimeZone + " zone has been corrupted.";
            }
            catch (Exception ex)
            {
                return timeUtc.ToString();
            }

        }
        public byte[] GetCheckedImage(string fileName)
        {
            var PdfImagePath = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["PdfImagePath"]));
            string imagePath = PdfImagePath + fileName + ".jpg";
            Image image = Image.FromFile(imagePath);
            return ImageToByteArray(image);
        }
        public byte[] ImageToByteArray(Image image)
        {
            using (var ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, ImageFormat.Png);
                image.Dispose();
                byte[] imageBytes = ms.ToArray();
                // Convert byte[] to Base64 String
                //string base64String = Convert.ToBase64String(imageBytes);
                return imageBytes;
            }
        }
        public string GetPreviewFolderPath(Guid envelopeId, string envDirectoryUNCPath)
        {
            var dirPath = envDirectoryUNCPath;
            return Path.Combine(dirPath, envelopeId.ToString(), "Preview");
        }
        public bool CreateEnvelopeXML(Guid envelopeId, string envelopeFoldrUNCPath)
        {
            try
            {
                string envelopeDirectory = string.Empty;
                if (!webApiCall)
                    envelopeDirectory = Path.Combine(envelopeFoldrUNCPath, envelopeId.ToString());
                else
                {
                    var dirPath = !string.IsNullOrEmpty(envelopeFoldrUNCPath) ? envelopeFoldrUNCPath : _modelHelper.GetEnvelopeDirectoryNew(envelopeId, string.Empty);
                    envelopeDirectory = Path.Combine(dirPath, envelopeId.ToString());
                }
                if (!System.IO.File.Exists(Path.Combine(envelopeDirectory, "Envelope.xml")))
                {
                    var xmlDocument = new XDocument(
                              new XDeclaration("1.0", "utf-8", "yes"),
                              new XElement("Envelope", new XAttribute("id", envelopeId),
                               new XElement(EnvelopeNodes.CreatedDateTime.ToString(), string.Format("{0:MM/dd/yyyy}", DateTime.Now.Date)),
                               new XElement(EnvelopeNodes.ExpiryDate.ToString(), string.Format("{0:MM/dd/yyyy}", DateTime.Now.Date)),
                               new XElement(EnvelopeNodes.IsEnvelopeDiscarded.ToString(), false),
                               new XElement(EnvelopeNodes.IsEnvelopeSaved.ToString(), false),
                               new XElement(EnvelopeNodes.IsEnvelopeRejected.ToString(), false),
                               new XElement(EnvelopeNodes.IsEnvelopeCompleted.ToString(), false)));

                    xmlDocument.Save(Path.Combine(envelopeDirectory, "Envelope.xml"));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string GetDocTempDirectory(string folderName)
        {
            string defaultPath = Convert.ToString(_appConfiguration["TempDirectory"]);
            var path = Convert.ToString(_appConfiguration["TempDirectory"]);
            var tempDirectory = Path.Combine(path, folderName);
            if (!Directory.Exists(tempDirectory))
            {
                path = Convert.ToString(_appConfiguration["AlternativeTempDirectory"]);
                tempDirectory = Path.Combine(path, folderName);
                if (!Directory.Exists(tempDirectory))
                {
                    path = Convert.ToString(_appConfiguration["SecondaryAlternativeDirectory"]);
                    tempDirectory = Path.Combine(path, folderName);
                    if (!Directory.Exists(tempDirectory))
                    {
                        path = Convert.ToString(_appConfiguration["TertiaryAlternativeDirectory"]);
                        tempDirectory = Path.Combine(path, folderName);
                        if (!Directory.Exists(tempDirectory))
                        {
                            path = Convert.ToString(_appConfiguration["TetraAlternativeDirectory"]);
                            tempDirectory = Path.Combine(path, folderName);
                            if (!Directory.Exists(tempDirectory))
                            {
                                path = Convert.ToString(_appConfiguration["fifthAlternativeDirectory"]);
                                tempDirectory = Path.Combine(path, folderName);
                                if (!Directory.Exists(tempDirectory))
                                {
                                    path = Convert.ToString(_appConfiguration["SixthAlternativeDirectory"]);
                                    tempDirectory = Path.Combine(path, folderName);
                                    if (!Directory.Exists(tempDirectory))
                                    {
                                        path = Convert.ToString(_appConfiguration["SeventhAlternativeDirectory"]);
                                        tempDirectory = Path.Combine(path, folderName);
                                        if (!Directory.Exists(tempDirectory))
                                        {
                                            path = Convert.ToString(_appConfiguration["EighthAlternativeDirectory"]);
                                            tempDirectory = Path.Combine(path, folderName);
                                            if (!Directory.Exists(tempDirectory))
                                            {
                                                path = defaultPath;
                                                tempDirectory = Path.Combine(path, folderName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return tempDirectory;
        }
        public bool UpdateEnvelopeXML(Guid envelopeId, Dictionary<EnvelopeNodes, string> fieldsWithValue, string envelopeFolderUNCPath)
        {
            try
            {
                string envelopeDirectory = string.Empty;
                if (!webApiCall)
                    envelopeDirectory = envelopeFolderUNCPath;// GetDocTempDirectory(envelopeId.ToString());
                else
                {
                    var dirPath = envelopeFolderUNCPath;
                    envelopeDirectory = Path.Combine(dirPath, envelopeId.ToString());
                }
                string envelopeXML = System.IO.File.ReadAllText(Path.Combine(envelopeDirectory, "Envelope.xml"));
                XDocument doc = XDocument.Parse(envelopeXML);

                foreach (var field in fieldsWithValue)
                    doc.Element("Envelope").Element(field.Key.ToString()).Value = field.Value;

                doc.Save(Path.Combine(envelopeDirectory, "Envelope.xml"));

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string CreateXmlFileToSend(Envelope envelopeObj, EnvelopeContent envelopeContent)
        {
            string fileName = envelopeObj.ID.ToString() + ".xml";
            byte[] xmlattachment = null;
            Byte[] info;
            try
            {
                var dirPath = _modelHelper.GetEnvelopeDirectoryNew(envelopeObj.ID, string.Empty);
                string path = Path.Combine(dirPath, envelopeObj.ID.ToString(), fileName);

                // Delete the file if it exists.
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.SetAttributes(path, FileAttributes.Normal);
                    System.IO.File.Delete(path);
                }

                //info = new UTF8Encoding(true).GetBytes(envelopeObj.EnvelopeContent.First().ContentXML);

                //Commented because it is moved to envelopeHelperMain due to circulare reference issue is coming

                //EnvelopeContent envelopeContent = new EnvelopeContent();
                //Envelope sessionEnvelope = new Envelope();
                //sessionEnvelope = envelopeObj;
                //envelopeHelperMain.CreateXml(sessionEnvelope, out envelopeContent);

                XDocument docs = XDocument.Parse(envelopeContent.ContentXML);
                info = new UTF8Encoding(true).GetBytes(envelopeContent.ContentXML);
                string xml = Encoding.UTF8.GetString(info);
                if (xml != null)
                {
                    using (FileStream fileStream = System.IO.File.Create(path, 1024))
                    {
                        // Add some information to the file.
                        fileStream.Write(info, 0, info.Length);
                    }
                }
                return path;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public string GetImages300FolderPath(Guid envelopeId, string UNCPath)
        {
            var dirPath = UNCPath;
            string imagesDirectory = Path.Combine(dirPath, envelopeId.ToString(), "Images300");
            if (!Directory.Exists(imagesDirectory))
                Directory.CreateDirectory(imagesDirectory);
            return imagesDirectory;
        }
        public string GetConvertedDocumentsFolderPath(Guid envelopeId, string UNCPath)
        {
            return GetConvertedDocumentsFolderPath(envelopeId, false, UNCPath);
        }
        public string GetConvertedDocumentsFolderPath(Guid envelopeId, bool isPerminant, string UNCPath)
        {
            string convertedDocumentsDirectory = string.Empty;
            if (isPerminant)
            {
                convertedDocumentsDirectory = Path.Combine(GetDocDirectory(envelopeId.ToString()), "Converted");
                if (!Directory.Exists(convertedDocumentsDirectory))
                    Directory.CreateDirectory(convertedDocumentsDirectory);
            }
            else
            {
                var dirPath = UNCPath;
                convertedDocumentsDirectory = Path.Combine(dirPath, envelopeId.ToString(), "Converted");
                if (!Directory.Exists((convertedDocumentsDirectory)))
                    Directory.CreateDirectory((convertedDocumentsDirectory));
            }
            return convertedDocumentsDirectory;
        }
        public string GetUploadedDocumentsFolderPath(Guid envelopeId, string UNCPath)
        {
            return GetUploadedDocumentsFolderPath(envelopeId, false, UNCPath);
        }
        public string GetUploadedDocumentsFolderPath(Guid envelopeId, bool isPerminant, string UNCPath)
        {
            string uploadedDocumentsDirectory = string.Empty;
            if (isPerminant)
            {
                //uploadedDocumentsDirectory = Path.Combine(GetDocDirectory(envelopeId.ToString()), "UploadedDocuments");
                uploadedDocumentsDirectory = Path.Combine(UNCPath, "UploadedDocuments");
                if (!Directory.Exists(uploadedDocumentsDirectory))
                    Directory.CreateDirectory(uploadedDocumentsDirectory);
            }
            else
            {
                var dirPath = UNCPath;
                uploadedDocumentsDirectory = Path.Combine(dirPath, envelopeId.ToString(), "UploadedDocuments");
                if (!Directory.Exists(uploadedDocumentsDirectory))
                    Directory.CreateDirectory(uploadedDocumentsDirectory);
            }
            return uploadedDocumentsDirectory;
        }
        public bool DeleteFilesAndDirectory(string srcDir)
        {
            try
            {
                if (Directory.Exists(srcDir))
                {
                    string[] files = Directory.GetFiles(srcDir);
                    foreach (var fle in files)
                    {
                        File.Delete(fle);
                    }
                    Directory.Delete(srcDir);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string GetHashSha256(string documentFolderPath)
        {
            byte[] data = System.IO.File.ReadAllBytes(documentFolderPath); // Path shows the location of the file.          
            byte[] result;
            SHA256Managed shaM = new SHA256Managed();
            result = shaM.ComputeHash(data);
            string hashCode = result.GetHashCode().ToString();
            return hashCode;
        }
        public bool UpdateSignatureCertificateStore(Guid envelopeId, bool isStored)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Envelope envelope = dbContext.Envelope.Where(d => d.ID == envelopeId).FirstOrDefault();
                    if (envelope != null)
                    {
                        envelope.IsSignatureCertificateStored = isStored;
                        dbContext.Entry(envelope).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<RecipientsDetailAPI> GetRecipientHistoryListForManage(Guid EnvelopeID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                try
                {
                    // List<RecipientsDetailAPI> recipients = dbContext.RecipientsDetailAPI.FromSqlRaw("EXEC GetRecipientHistoryListForManage @EnvelopeID", new SqlParameter("EnvelopeID", EnvelopeID)).ToList();

                    SqlConnection connection = new SqlConnection(Convert.ToString(_configuration.Value.ConnectionStrings.RSignContext));
                    DataSet dataset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand("GetRecipientHistoryListForManage", connection);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@EnvelopeID", EnvelopeID);
                    adapter.Fill(dataset);
                    List<RecipientsDetailAPI> recipients = new List<RecipientsDetailAPI>();
                    recipients = JsonConvert.DeserializeObject<List<RecipientsDetailAPI>>(JsonConvert.SerializeObject(dataset.Tables[0]));

                    //Commented because model is not in db
                    //  List<RecipientsDetailsAPI> recipients = dbContext.Database.SqlQueryRaw<RecipientsDetailsAPI>("EXEC GetRecipientHistoryListForManage @EnvelopeID", new SqlParameter("EnvelopeID", EnvelopeID)).ToList();
                    return recipients;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public List<DropdownOptions> GetDropdownOptionsList(string Type, string LanguageCode)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return (from e in dbContext.DropdownOptions
                        join r in dbContext.Language on e.LanguageID equals r.ID
                        where e.FieldName == Type && r.LanguageCode == LanguageCode
                        select e).ToList();
            }
        }
        public MetaDataAndHistory GetMetaDataAndHistory(Dictionary<Guid, string> languageText)
        {
            return new MetaDataAndHistory
            {
                EnvelopeData = languageText.Single(a => a.Key == Constants.Resourcekey.EnvelopeData).Value,
                EnvelopeID = languageText.Single(a => a.Key == Constants.Resourcekey.EnvelopeID).Value,
                Subject = languageText.Single(a => a.Key == Constants.Resourcekey.Subject).Value,
                Documents = languageText.Single(a => a.Key == Constants.Resourcekey.Documents).Value,
                DocumentHash = languageText.Single(a => a.Key == Constants.Resourcekey.DocumentHash).Value,
                Sent = languageText.Single(a => a.Key == Constants.Resourcekey.Sent).Value,
                Status = languageText.Single(a => a.Key == Constants.Resourcekey.Status).Value,
                StatusDate = languageText.Single(a => a.Key == Constants.Resourcekey.StatusDate).Value,
                Recipients = languageText.Single(a => a.Key == Constants.Resourcekey.Recipients).Value,
                Name = languageText.Single(a => a.Key == Constants.Resourcekey.Name).Value,
                Roles = languageText.Single(a => a.Key == Constants.Resourcekey.Roles).Value,
                Role = languageText.Single(a => a.Key == Constants.Resourcekey.Role).Value,
                Address = languageText.Single(a => a.Key == Constants.Resourcekey.Address).Value,
                Sender = languageText.Single(a => a.Key == Constants.Resourcekey.Sender).Value,
                Prefill = languageText.Single(a => a.Key == Constants.Resourcekey.Prefill).Value,
                Signer = languageText.Single(a => a.Key == Constants.Resourcekey.Signer).Value,
                Email = languageText.Single(a => a.Key == Constants.Resourcekey.Email).Value,
                IPAddress = languageText.Single(a => a.Key == Constants.Resourcekey.IPAddress).Value,
                Date = languageText.Single(a => a.Key == Constants.Resourcekey.Date).Value,
                Event = languageText.Single(a => a.Key == Constants.Resourcekey.Event).Value,
                DocumentEvents = languageText.Single(a => a.Key == Constants.Resourcekey.DocumentEvents).Value,
                Created = languageText.Single(a => a.Key == Constants.Resourcekey.Created).Value,
                SignerSignatures = languageText.Single(a => a.Key == Constants.Resourcekey.SignerSignatures).Value,
                SignerName = languageText.Single(a => a.Key == Constants.Resourcekey.SignerName).Value,
                Signature = languageText.Single(a => a.Key == Constants.Resourcekey.Signature).Value,
                Type = languageText.Single(a => a.Key == Constants.Resourcekey.Type).Value,
                CarbonCopyEvents = languageText.Single(a => a.Key == Constants.Resourcekey.CarbonCopyEvents).Value,
                DateFormatEU = languageText.Single(a => a.Key == Constants.Resourcekey.DateFormatEU).Value,
                DateFormatStr = languageText.Single(a => a.Key == Constants.Resourcekey.DateFormatStr).Value,
                TermsOfService = languageText.Single(a => a.Key == Constants.Resourcekey.TermsOfService).Value,
                Accepted = languageText.Single(a => a.Key == Constants.Resourcekey.Accepted).Value,
                UpdateAndResend = languageText.Single(a => a.Key == Constants.Resourcekey.lang_UpdateResend).Value,
                Initial = languageText.Single(a => a.Key == Constants.Resourcekey.Initials).Value,
                FilesReviewed = "File(s) Reviewed",
                AccessAuthentication = languageText.Single(a => a.Key == Constants.Resourcekey.AccessAuthentication).Value,
                EmailAccessCode = languageText.Single(a => a.Key == Constants.Resourcekey.EmailAccessCode).Value,
                Checked = languageText.Single(a => a.Key == Constants.Resourcekey.Checked).Value,
                UnChecked = languageText.Single(a => a.Key == Constants.Resourcekey.Unchecked).Value,
                None = languageText.Single(a => a.Key == Constants.Resourcekey.lang_none).Value,
                EmailVerification = languageText.Single(a => a.Key == Constants.Resourcekey.EmailVerification).Value,
                DeliveryMode = languageText.Single(a => a.Key == Constants.Resourcekey.DeliveryMode).Value,
                Mobile = languageText.Single(a => a.Key == Constants.Resourcekey.Mobile).Value,
                EmailOrMobile = languageText.Single(a => a.Key == Constants.Resourcekey.EmailOrMobile).Value
            };
        }
        public EnvelopeSettingsDetail GetEnvelopeSettingsDetail(Guid envelopeID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.EnvelopeSettingsDetail.Where(e => e.EnvelopeId == envelopeID).FirstOrDefault();
            }
        }
        public AdminGeneralAndSystemSettings TransformSettingsDictionaryToEntity(APISettings settingDetails)
        {
            loggerModelNew = new LoggerModelNew("", "ESignHelper", "TransformSettingsDictionaryToEntity", "Intializing process for transpose setting details", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            AdminGeneralAndSystemSettings adminGeneralAndSystemSettings = new AdminGeneralAndSystemSettings();
            try
            {
                Type type = typeof(Constants.SettingsKeyConfig);
                FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                List<SettingProperties> settingProperties = new List<SettingProperties>();

                foreach (FieldInfo p in fields)
                {
                    switch (Convert.ToString(p.Name))
                    {
                        case "ShowSettingsTabSelected":
                            adminGeneralAndSystemSettings.ShowSettingsTabSelected = new Guid(Convert.ToString(CheckSettingsKeyExists(p, settingDetails)));
                            break;
                        case "OverrideUserSettings":
                            adminGeneralAndSystemSettings.OverrideUserSettings = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StorageDriveLocal":
                            adminGeneralAndSystemSettings.StorageDriveLocal = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StorageDriveGoogle":
                            adminGeneralAndSystemSettings.StorageDriveGoogle = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StorageDriveDropbox":
                            adminGeneralAndSystemSettings.StorageDriveDropbox = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StorageDriveSkydrive":
                            adminGeneralAndSystemSettings.StorageDriveSkydrive = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StorageiManage":
                            adminGeneralAndSystemSettings.StorageiManage = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "Storagenetdocuments":
                            adminGeneralAndSystemSettings.Storagenetdocuments = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StorageAppliedEpic":
                            adminGeneralAndSystemSettings.StorageAppliedEpic = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StorageBullhorn":
                            adminGeneralAndSystemSettings.StorageBullhorn = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StorageVincere":
                            adminGeneralAndSystemSettings.StorageVincere = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SignatureCaptureTypeSign":
                            adminGeneralAndSystemSettings.SignatureCaptureType = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SignatureCaptureHandDrawn":
                            adminGeneralAndSystemSettings.SignatureCaptureHanddrawn = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "UploadSignature":
                            adminGeneralAndSystemSettings.UploadSignature = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "TimeZone":
                            adminGeneralAndSystemSettings.IsTimeZoneUTC = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.SelectedTimeZone = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DateFormat":
                            adminGeneralAndSystemSettings.DateFormatDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.DateFormatID = new Guid(Convert.ToString(CheckSettingsKeyExists(p, settingDetails)));
                            break;
                        case "ExpiresIn":
                            adminGeneralAndSystemSettings.ExpiresInDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.ExpiresInID = new Guid(Convert.ToString(CheckSettingsKeyExists(p, settingDetails)));
                            break;
                        case "SendReminderIn":
                            adminGeneralAndSystemSettings.SendReminderInDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.SendReminderIn = Convert.ToInt16(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SendReminderInDropdownOption":
                            adminGeneralAndSystemSettings.SendReminderInDropdownSelected = new Guid(Convert.ToString(CheckSettingsKeyExists(p, settingDetails)));
                            break;
                        case "ThenSendReminderIn":
                            adminGeneralAndSystemSettings.ThenSendReminderInDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.ThenSendReminderIn = Convert.ToInt16(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "ThenSendReminderInDropdownOption":
                            adminGeneralAndSystemSettings.ThenSendReminderInDropdownSelected = new Guid(Convert.ToString(CheckSettingsKeyExists(p, settingDetails)));
                            break;
                        case "StoredSignedPDF":
                            adminGeneralAndSystemSettings.StoredSignedPDFDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.StoredSignedPDF = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IncludeSignedCertificateOnSignedPDF":
                            adminGeneralAndSystemSettings.IncludeSignedCertificateOnSignedPDFDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.IncludeSignedCertificateOnSignedPDF = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SignInSequence":
                            adminGeneralAndSystemSettings.SignInSequenceDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.SignInSequence = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IncludeTransparencyDocument":
                            adminGeneralAndSystemSettings.IncludeTransparencyDocDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.IncludeTransparencyDoc = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "Disclaimer":
                            adminGeneralAndSystemSettings.IsDisclaimerEnabled = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.Disclaimer = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsDisclaimerInCertificate":
                            adminGeneralAndSystemSettings.IsDisclaimerInCertificate = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsDeleteSignedContracts":
                            adminGeneralAndSystemSettings.IsDeleteSignedContracts = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsCreateRules":
                            adminGeneralAndSystemSettings.IsCreateRules = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsCreateMessageTemplate":
                            adminGeneralAndSystemSettings.IsCreateMessageTemplate = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IncludeSignerAttachFile":
                            adminGeneralAndSystemSettings.IncludeSignerAttachFile = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));// == 3 ? true : false;
                            break;
                        case "AllowRecipientToAttachFileWhileSigning":
                            adminGeneralAndSystemSettings.AllowRecipeintToAttachFileOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.AllowRecipeintToAttachFile = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "FormFieldAlignment":
                            adminGeneralAndSystemSettings.IsFormFieldAlignmentEnabled = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "CreateStaticLink":
                            adminGeneralAndSystemSettings.CreateStaticLinkDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.IncludeCreateStaticLink = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AttachXML":
                            adminGeneralAndSystemSettings.IsAddXMLDataDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.IncludeAddXMLData = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SeparateMultipleDocumentsAfterSigning":
                            adminGeneralAndSystemSettings.SeparateMultipleDocumentsAfterSigningDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.SeparateMultipleDocumentsAfterSigning = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsShareTemplateRule":
                            //Added by Tparker-Enhancement Sharing TemplatesRules with Share MasterAllow Copy Options
                            var ShareTemplateRuleDetails = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            if (ShareTemplateRuleDetails != null)
                            {
                                var shareSetting = JsonConvert.DeserializeObject<ShareTemplateRuleSettings>(ShareTemplateRuleDetails);
                                if (shareSetting != null)
                                {
                                    adminGeneralAndSystemSettings.ShareTemplateRuleSettingsValues = shareSetting;
                                }
                            }
                            else
                            {
                                ShareTemplateRuleSettings shareSettings = new ShareTemplateRuleSettings { IsShareTemplateRule = false, IsShareMaster = false, IsAllowCopy = false };
                                adminGeneralAndSystemSettings.ShareTemplateRuleSettingsValues = shareSettings;
                            }
                            break;
                        case "AccessAuthentication":
                            adminGeneralAndSystemSettings.AccessAuthenticatedDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.AccessAuthenticationId = new Guid(Convert.ToString(CheckSettingsKeyExists(p, settingDetails)));
                            break;
                        case "AccessPassword":
                            adminGeneralAndSystemSettings.AccessPassword = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));

                            if (!String.IsNullOrEmpty(adminGeneralAndSystemSettings.AccessPassword))
                            {
                                adminGeneralAndSystemSettings.AccessPassword = ModelHelper.Decrypt(adminGeneralAndSystemSettings.AccessPassword,
                                    Constants.ConfigurationalProperties.PasswordProperties.completeEncodedKey, Constants.ConfigurationalProperties.PasswordProperties.passwordKeySize);
                            }
                            break;
                        case "IsAccessCodeSendToSignerEnabled":
                            adminGeneralAndSystemSettings.IsAccessCodeSendToSignerEnabled = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StoreEmailBody":
                            adminGeneralAndSystemSettings.StoreEmailBody = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AllowUserToDeleteEmailBody":
                            adminGeneralAndSystemSettings.AllowUserToDeleteEmailBody = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "FinalContractOptionSetting":
                            adminGeneralAndSystemSettings.FinalContractOptionID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SignatureControlRequired":
                            adminGeneralAndSystemSettings.SignatureControlRequired = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EsignMailCopyAddress":
                            adminGeneralAndSystemSettings.EsignMailCopyAddress = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EsignMailRerouteAddress":
                            adminGeneralAndSystemSettings.EsignMailRerouteAddress = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "ReceiveSendingEmailConfirmation":
                            adminGeneralAndSystemSettings.ReceiveSendingEmailConfirmation = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AttachSignedPDFOptionSetting":
                            //adminGeneralAndSystemSettings.AttachSignedPdfID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            adminGeneralAndSystemSettings.AttachSignedPdfID = new Guid(Convert.ToString(CheckSettingsKeyExists(p, settingDetails)));
                            break;
                        case "HeaderFooterOptionSettings":
                            adminGeneralAndSystemSettings.HeaderFooterSettingID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsPostSigningLandingPage":
                            adminGeneralAndSystemSettings.IsPostSigningLandingPage = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            adminGeneralAndSystemSettings.PostSigningUrlDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            break;
                        case "IsIncludeEnvelopeXmlData":
                            adminGeneralAndSystemSettings.IsIncludeEnvelopeXmlData = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AllowTemplateEditing":
                            adminGeneralAndSystemSettings.IsAllowTemplateEditingDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.AllowTemplateEditing = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnablePostSigningLoginPopup":
                            adminGeneralAndSystemSettings.EnablePostSigningLoginPopup = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EmailDisclaimer":
                            if (settingDetails.SettingDetails.Any(c => c.Key.ToString() == p.GetValue(null).ToString()))
                                adminGeneralAndSystemSettings.EmailDisclaimer = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SendIndividualSignatureNotifications":
                            adminGeneralAndSystemSettings.SendIndividualSignatureNotificationsOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.SendIndividualSignatureNotifications = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AddDatetoSignedDocumentNameOptionSettings":
                            if (settingDetails.SettingDetails.Any(c => c.Key.ToString() == p.GetValue(null).ToString()))
                                adminGeneralAndSystemSettings.DatetoSignedDocNameSettingsOptionID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SendFinalReminderBeforeExp":
                            adminGeneralAndSystemSettings.SendFinalReminderBeforeExpDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.SendFinalReminderBeforeExp = Convert.ToInt16(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SendFinalReminderBeforeExpInDropdownOption":
                            adminGeneralAndSystemSettings.SendFinalReminderBeforeExpDropdownSelected = new Guid(Convert.ToString(CheckSettingsKeyExists(p, settingDetails)));
                            break;
                        case "IsDefaultSignatureRequiredForStaticTemplate":
                            adminGeneralAndSystemSettings.DefaultSignReqForStaticTemplateDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.IncludeDefaultSignReqForStaticTemplate = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableOutOfOfficeMode":
                            adminGeneralAndSystemSettings.IsOutOfOfficeModeEnable = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "OOFDateRangeFirstDay":
                            if (settingDetails.SettingDetails.ContainsKey(new Guid(p.GetValue(null).ToString())) && !String.IsNullOrEmpty(settingDetails.SettingDetails[new Guid(p.GetValue(null).ToString())].SettingValue))
                                adminGeneralAndSystemSettings.DateRangeFirstDay = Convert.ToDateTime(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "OOFDateRangeLastDay":
                            if (settingDetails.SettingDetails.ContainsKey(new Guid(p.GetValue(null).ToString())) && !String.IsNullOrEmpty(settingDetails.SettingDetails[new Guid(p.GetValue(null).ToString())].SettingValue))
                                adminGeneralAndSystemSettings.DateRangeLastDay = Convert.ToDateTime(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "OOFCopyEmailAddr":
                            adminGeneralAndSystemSettings.CopyEmailAddrForOutOfOfficeMode = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "OOFRerouteEmailAddr":
                            adminGeneralAndSystemSettings.RerouteEmailAddrForOutOfOfficeMode = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableDependenciesFeature":
                            adminGeneralAndSystemSettings.EnableDependenciesFeatureDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.EnableDependenciesFeature = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "ReferenceCodeOptionSetting":
                            adminGeneralAndSystemSettings.ReferenceCodeSettingID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "PostSendingNavigationPage":
                            adminGeneralAndSystemSettings.PostSendingNavigationPageId = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SignMultipleDocumentIndependently":
                            adminGeneralAndSystemSettings.SignMultipleDocumentIndependently = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SendInvitationEmailToSigner":
                            adminGeneralAndSystemSettings.SendInvitationEmailToSignerID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableIntegrationAccess":
                            adminGeneralAndSystemSettings.EnableIntegrationAccess = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DocumentPaperSize":
                            adminGeneralAndSystemSettings.DocumentPaperSizeID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StampOnSignerCopySetting":
                            adminGeneralAndSystemSettings.StampOnSignerCopySetting = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StampOnSignerCopyAuthrozieText":
                            if (settingDetails.SettingDetails.Any(c => c.Key.ToString() == p.GetValue(null).ToString()))
                                adminGeneralAndSystemSettings.StampOnSignerCopyAuthrozieText = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StampOnSignerCopyWatermarkText":
                            if (settingDetails.SettingDetails.Any(c => c.Key.ToString() == p.GetValue(null).ToString()))
                                adminGeneralAndSystemSettings.StampOnSignerCopyWatermarkText = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "ElectronicSignIndicationSetting":
                            adminGeneralAndSystemSettings.ElectronicSignIndicationSelectedID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DeclineTemplateReasonsSettings":
                            if (settingDetails.SettingDetails.Any(c => c.Key.ToString() == p.GetValue(null).ToString()))
                                adminGeneralAndSystemSettings.DeclineTemplateReasonsSettings = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SignatureRequestReplyAddress":
                            adminGeneralAndSystemSettings.SignReqReplyAddSettingsID = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "ShowRSignLogo":
                            adminGeneralAndSystemSettings.ShowRSignLogo = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "ShowCompanyLogo":
                            adminGeneralAndSystemSettings.ShowCompanyLogo = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EmailBannerBackgroundColor":
                            adminGeneralAndSystemSettings.EmailBannerBackgroundColor = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EmailBannerFontColor":
                            adminGeneralAndSystemSettings.EmailBannerFontColor = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "CompanyLogoImage":
                            adminGeneralAndSystemSettings.CompanyLogoImage = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "PrivateMode":
                            if (settingDetails.SettingDetails.Any(c => c.Key.ToString() == p.GetValue(null).ToString()))
                                adminGeneralAndSystemSettings.PrivateModeSettings = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            if (adminGeneralAndSystemSettings.PrivateModeSettings != null)
                            {
                                adminGeneralAndSystemSettings.PrivateModeSettings = adminGeneralAndSystemSettings.PrivateModeSettings.Replace("False", "false");
                                var privateModeSetting = JsonConvert.DeserializeObject<List<PrivateModeSettings>>(adminGeneralAndSystemSettings.PrivateModeSettings);
                                if (privateModeSetting != null)
                                {
                                    adminGeneralAndSystemSettings.PrivateModeSettingsValues = privateModeSetting;
                                }
                            }
                            break;
                        case "StoreOriginalDocument":
                            adminGeneralAndSystemSettings.StoreOriginalDocumentSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.StoreOriginalDocument = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "StoreSignatureCertificate":
                            adminGeneralAndSystemSettings.StoreSignatureCertificateSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.StoreSignatureCertificate = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DeleteOriginalDocument":
                            adminGeneralAndSystemSettings.DeleteOriginalDocument = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AllowBulkSending":
                            adminGeneralAndSystemSettings.AllowBulkSending = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "PostSigningPageUrl":
                            adminGeneralAndSystemSettings.PostSigningPageUrl = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnvelopePostSigningPageUrl":
                            adminGeneralAndSystemSettings.EnvelopePostSigningPageUrl = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsEnvelopePostSigningPage":
                            adminGeneralAndSystemSettings.EnvelopePostSigningUrlDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            break;
                        case "EnableFileReview":
                            adminGeneralAndSystemSettings.EnableFileReview = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        //Added by Tparker- Enhancement: New Setting to Hide the ID Button in Controls
                        case "ShowControlID":
                            adminGeneralAndSystemSettings.ShowControlID = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableWebhook":
                            adminGeneralAndSystemSettings.EnableWebhook = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        //Added by Tparker- Enhancement: Add "Allow Rule Editing" Setting
                        case "AllowRuleEditing":
                            adminGeneralAndSystemSettings.IsAllowRuleEditingDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.AllowRuleEditing = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        //Added by Tparker-Enhancement: Allow Creating and Using Rules Based on Settings
                        case "AllowRuleUse":
                            adminGeneralAndSystemSettings.AllowRuleUse = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        //Added by Tparker-Enhancement Sharing TemplatesRules with Share MasterAllow Copy Options
                        case "AllowMessageTemplate":
                            adminGeneralAndSystemSettings.AllowMessageTemplate = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        //Added by Tparker-Enhancement EnableClickToSign
                        case "EnableClickToSign":
                            adminGeneralAndSystemSettings.EnableClickToSign = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableAutoFillTextControls":
                            adminGeneralAndSystemSettings.EnableAutoFillTextControls = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "TypographySize":
                            adminGeneralAndSystemSettings.TypographySizeOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.TypographySize = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SendReminderTillExpiration":
                            var SendReminderTillExpirationSettings = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            if (SendReminderTillExpirationSettings != "")
                            {
                                var ReminderSetting = JsonConvert.DeserializeObject<SendReminderTillExpirationSettings>(SendReminderTillExpirationSettings);
                                if (ReminderSetting != null)
                                {
                                    adminGeneralAndSystemSettings.SendReminderTillExpirationSettingsValues = ReminderSetting;
                                    adminGeneralAndSystemSettings.IsSendReminderTillExpiration = Convert.ToBoolean(ReminderSetting.IsSendReminderTillExpiration);
                                    adminGeneralAndSystemSettings.SendReminderTillExpirationOptionSelected = ReminderSetting.SendReminderTillExpiration;

                                }
                            }
                            else
                            {
                                SendReminderTillExpirationSettings EmptySetting = new SendReminderTillExpirationSettings { IsSendReminderTillExpiration = false, SendReminderTillExpiration = null };
                                adminGeneralAndSystemSettings.IsSendReminderTillExpiration = false;
                                adminGeneralAndSystemSettings.SendReminderTillExpirationOptionSelected = Constants.DropdownFieldKeyType.OneEmailperEnvelope;
                                adminGeneralAndSystemSettings.SendReminderTillExpirationSettingsValues = EmptySetting;
                            }
                            break;
                        case "EnvelopeExpirationRemindertoSender":
                            var EnvelopeExpirationRemindertoSenderSettings = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            if (EnvelopeExpirationRemindertoSenderSettings != "")
                            {
                                var ReminderToSenderSetting = JsonConvert.DeserializeObject<EnvelopeExpirationRemindertoSenderSettings>(EnvelopeExpirationRemindertoSenderSettings);
                                if (ReminderToSenderSetting != null)
                                {
                                    adminGeneralAndSystemSettings.EnvelopeExpirationRemindertoSenderValues = ReminderToSenderSetting;
                                    adminGeneralAndSystemSettings.IsEnvelopeExpirationRemindertoSender = Convert.ToBoolean(ReminderToSenderSetting.IsEnvelopeExpirationRemindertoSender);
                                    adminGeneralAndSystemSettings.EnvelopeExpirationRemindertoSenderReminderDays = ReminderToSenderSetting.DaysForReminder;
                                    adminGeneralAndSystemSettings.EnvelopeExpirationRemindertoSenderDropdownSelected = ReminderToSenderSetting.PeriodForReminder;
                                }
                            }
                            else
                            {
                                EnvelopeExpirationRemindertoSenderSettings EmptySetting = new EnvelopeExpirationRemindertoSenderSettings { IsEnvelopeExpirationRemindertoSender = false, PeriodForReminder = Constants.ReminderDropdownOptions.Days, DaysForReminder = 0 };
                                adminGeneralAndSystemSettings.EnvelopeExpirationRemindertoSenderValues = EmptySetting;
                                adminGeneralAndSystemSettings.IsEnvelopeExpirationRemindertoSender = false;
                                adminGeneralAndSystemSettings.EnvelopeExpirationRemindertoSenderReminderDays = 0;
                                adminGeneralAndSystemSettings.EnvelopeExpirationRemindertoSenderDropdownSelected = Constants.ReminderDropdownOptions.Days;
                            }
                            break;
                        case "AllowMultiSigners":
                            adminGeneralAndSystemSettings.AllowMultiSigners = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SendConfirmationEmail":
                            adminGeneralAndSystemSettings.IsSendConfirmationEmailOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.SendConfirmationEmail = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DigitalCertificate":
                            adminGeneralAndSystemSettings.DigitalCertificate = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AppKey":
                            adminGeneralAndSystemSettings.AppKey = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableCcOptions"://EnableCcOptions
                            adminGeneralAndSystemSettings.EnableCcOptions = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DisclaimerLocation":
                            adminGeneralAndSystemSettings.DisclaimerLocationId = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DefaultLandingPage":
                            adminGeneralAndSystemSettings.DefaultLandingPage = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableRecipientLanguageSelection":
                            adminGeneralAndSystemSettings.EnableRecipientLanguageSelection = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "ControlPanelPinnedPosition":
                            if (settingDetails.SettingDetails.Any(c => c.Key.ToString() == p.GetValue(null).ToString()))
                                adminGeneralAndSystemSettings.ControlPanelPinnedPosition = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsEmailListReasonsforDeclining":
                            adminGeneralAndSystemSettings.IsEmailListReasonsforDeclining = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DeclineReportSendTo":
                            adminGeneralAndSystemSettings.DeclineReportSendTo = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DeclineEmailSendingFrequency":
                            adminGeneralAndSystemSettings.DeclineEmailSendingFrequency = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DisableDownloadOptionOnSignersPage":
                            adminGeneralAndSystemSettings.DisableDownloadOptionOnSignersPage = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableSendingMessagesToMobile":
                            adminGeneralAndSystemSettings.EnableSendingMessagesToMobile = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DefaultCountryCode":
                            adminGeneralAndSystemSettings.DefaultCountryCode = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DefaultDeliveryMode":
                            adminGeneralAndSystemSettings.DefaultDeliveryMode = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "RestrictRecipientsToContactListonly":
                            adminGeneralAndSystemSettings.RestrictRecipientsToContactListonly = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "RequiresSignersConfirmationonFinalSubmit":
                            adminGeneralAndSystemSettings.ReverifyonFinalSubmitDisplayOptionSelected = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.RequiresSignersConfirmationonFinalSubmit = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IncludeStaticTemplates":
                            adminGeneralAndSystemSettings.IncludeStaticTemplatesDisplayOption = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails, "Tab"));
                            adminGeneralAndSystemSettings.IncludeStaticTemplates = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SMSProvider":
                            adminGeneralAndSystemSettings.SMSProvider = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "IsAllowSignerstoDownloadFinalContract":
                            adminGeneralAndSystemSettings.IsAllowSignerstoDownloadFinalContract = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;

                        case "EnableMultipleAttachmentsCustomizable":
                            adminGeneralAndSystemSettings.EnableMultipleAttachmentsCustomizable = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;

                        case "RenameFileToMode":
                            adminGeneralAndSystemSettings.RenameFileToMode = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AddPrefixToFileNameMode":
                            adminGeneralAndSystemSettings.AddPrefixToFileNameMode = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AddSuffixFileToMode":
                            adminGeneralAndSystemSettings.AddSuffixToFileNameMode = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "PrefixCustomName":
                            adminGeneralAndSystemSettings.PrefixCustomName = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SuffixCustomName":
                            adminGeneralAndSystemSettings.SuffixCustomName = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DateTimeStampForMultipleDocSettingsForPrefix":
                            adminGeneralAndSystemSettings.DateTimeStampForMultipleDocSettingsForPrefixMode = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DateTimeStampForMultipleDocSettingsForSufix":
                            adminGeneralAndSystemSettings.DateTimeStampForMultipleDocSettingsForSufixMode = Convert.ToString(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DisableFinishLaterOption":
                            adminGeneralAndSystemSettings.DisableFinishLaterOption = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DisableDeclineOption":
                            adminGeneralAndSystemSettings.DisableDeclineOption = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "DisableChangeSigner":
                            adminGeneralAndSystemSettings.DisableChangeSigner = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "EnableSignersUI":
                            adminGeneralAndSystemSettings.EnableSignersUIId = Convert.ToInt32(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "AllowChangeEmailSubjectUpdateResend":
                            adminGeneralAndSystemSettings.AllowChangeEmailSubjectUpdateResend = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                        case "SendMessageCodetoAvailableEmailorMobile":
                            adminGeneralAndSystemSettings.SendMessageCodetoAvailableEmailorMobile = Convert.ToBoolean(CheckSettingsKeyExists(p, settingDetails));
                            break;
                    }
                    //Added by Tparker - Override And Lock settings                   
                    GetSettingPropertiesData(new Guid(p.GetValue(null).ToString()), settingDetails, settingProperties);

                }
                adminGeneralAndSystemSettings.SettingProperties = settingProperties;
                adminGeneralAndSystemSettings.UserPlanDetails = settingDetails.UserPlanDetails;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred while transpose setting details." + ex.Message.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);

                //throw Ex;
                return null;
            }
            return adminGeneralAndSystemSettings;
        }
        private object CheckSettingsKeyExists(FieldInfo fieldInfo, APISettings settingDetails, string Type = "Value")
        {
            if (settingDetails.SettingDetails.ContainsKey(new Guid(fieldInfo.GetValue(null).ToString())))
            {
                if (Type == "Tab")
                {
                    return settingDetails.SettingDetails[new Guid(fieldInfo.GetValue(null).ToString())].ShowOnSettingsTab;
                }
                else
                {
                    return settingDetails.SettingDetails[new Guid(fieldInfo.GetValue(null).ToString())].SettingValue;
                }
            }
            else
            {
                if (Type == "Tab")
                {
                    return _settingsRepository.saveDefaultSettingEntity(fieldInfo.Name, new Guid(fieldInfo.GetValue(null).ToString()), new Guid(), settingDetails.SettingsFor, settingDetails.SettingsForType).OptionFlag;
                }
                else
                {
                    return _settingsRepository.saveDefaultSettingEntity(fieldInfo.Name, new Guid(fieldInfo.GetValue(null).ToString()), new Guid(), settingDetails.SettingsFor, settingDetails.SettingsForType).OptionValue;
                }
            }
        }
        private void GetSettingPropertiesData(Guid KeyConfig, APISettings settingDetails, List<SettingProperties> settingProperties)
        {
            if (settingDetails.SettingDetails.ContainsKey(KeyConfig))
            {
                settingProperties.Add(new SettingProperties { KeyConfig = KeyConfig, IsLock = Convert.ToBoolean(settingDetails.SettingDetails[KeyConfig].IsLock), IsOverride = Convert.ToBoolean(settingDetails.SettingDetails[KeyConfig].IsOverride) });
            }
            else
            {
                settingProperties.Add(new SettingProperties { KeyConfig = KeyConfig, IsLock = false, IsOverride = false });
            }
        }
        public string GetTimeZoneAbbreviation(string timeZoneName)
        {
            string output = string.Empty;

            string[] timeZoneWords = timeZoneName.Split(' ');
            foreach (string timeZoneWord in timeZoneWords)
            {
                if (timeZoneWord[0] != '(')
                {
                    output += timeZoneWord[0];
                }
                else
                {
                    output += timeZoneWord;
                }
            }
            if (output == "CUT")
            {
                output = "UTC";
            }
            else if (output == Constants.TimeZone.WEDT || output == Constants.TimeZone.WEST)
            {
                output = output == Constants.TimeZone.WEDT ? Constants.TimeZone.WEST : Constants.TimeZone.WET;
            }
            return output;
        }
        public bool IsFailureStatusCode(HttpStatusCode httpStatusCode)
        {
            bool isValid = true;
            if (httpStatusCode == HttpStatusCode.ServiceUnavailable || httpStatusCode == HttpStatusCode.NotFound || httpStatusCode == HttpStatusCode.InternalServerError
                 || httpStatusCode == HttpStatusCode.BadGateway || httpStatusCode == HttpStatusCode.ServiceUnavailable || httpStatusCode == HttpStatusCode.GatewayTimeout
                  || httpStatusCode == HttpStatusCode.NotImplemented || httpStatusCode == HttpStatusCode.HttpVersionNotSupported)
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
