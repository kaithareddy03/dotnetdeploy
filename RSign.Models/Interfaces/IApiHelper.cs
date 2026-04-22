using RSign.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IApiHelper
    {
        string GetFilesReviewZip(List<String> files, string documentPaths, string Subject);
        byte[] GetControlImage(DocumentContents control);
        string finalMergePDFApi(Envelope envelopeObject, FinalContractSettings userSettings, string dirPath, string currentStatus = "", bool IsRequiredZip = false);
        string GetRecipientDocumentZip(string documentPaths);
        string GetTransperancyDocument(string tempEnvelopeDir, Envelope envelopeObject);
    }
}
