using RSign.Common.Enums;
using RSign.ManageDocument.Models;
using RSign.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IESignHelper
    {
        void SetApiCallFlag();
        IList<int> GetFontSize();
        string GetImagesFolderPath(Guid iD, string envelopeFolderUNCPath);
        string GetLocalTime(DateTime now, string userTimezone, Guid? guid = null);     
        string ImageToBase64(Image stampImage);
        byte[] GetCheckedImage(string fileName);
        string GetPreviewFolderPath(Guid envelopeId, string dirPath);
        bool CreateEnvelopeXML(Guid envelopeId, string envelopeFolderUNCPath);
        bool UpdateEnvelopeXML(Guid envelopeId, Dictionary<EnvelopeNodes, string> fieldsWithValue, string dirPath);
        string CreateXmlFileToSend(Envelope envelopeObj, EnvelopeContent envelopeContent);
        string GetImages300FolderPath(Guid envelopeId, string UNCPath);
        string GetConvertedDocumentsFolderPath(Guid envelopeId, string UNCPath);
        string GetUploadedDocumentsFolderPath(Guid envelopeId, string UNCPath);
        bool DeleteFilesAndDirectory(string srcDir);
        string GetHashSha256(string documentFolderPath);
        bool UpdateSignatureCertificateStore(Guid envelopeId, bool isStored);
        List<RecipientsDetailAPI> GetRecipientHistoryListForManage(Guid EnvelopeID);
        List<DropdownOptions> GetDropdownOptionsList(string Type, string LanguageCode);
        MetaDataAndHistory GetMetaDataAndHistory(Dictionary<Guid, string> languageText);
        AdminGeneralAndSystemSettings TransformSettingsDictionaryToEntity(APISettings settingDetails);
        EnvelopeSettingsDetail GetEnvelopeSettingsDetail(Guid envelopeID);
        string GetTimeZoneAbbreviation(string timeZoneName);
        string GetEnvelopeImagesFolderPath(Guid envelopeId, string envelopeFolderUNCPath);
        bool IsFailureStatusCode(HttpStatusCode httpStatusCode);
        
    }
}
