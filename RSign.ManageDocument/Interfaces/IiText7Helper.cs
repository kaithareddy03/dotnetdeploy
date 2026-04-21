using RSign.ManageDocument.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Interfaces
{
    public interface IiText7Helper
    {
        string MergePDFiText(List<string> fileEntries, string outputPDF);
        void AddTextToPDFiText7(string inputPDF, string outputPDF, List<DocumentControls> documentControls, string imageDirectory, string eDisplayCode = "", string uSerTimeZone = "", DateTime? completedDate = null, Guid dateFormatID = new Guid(), int intHeaderFooterOptions = 2, int electronicSignatureStampId = 1);
        void AddTextToPDFiText7ForCC(string inputPDF, string outputPDF, List<DocumentControls> documentControls, string imageDirectory, string eDisplayCode = "", string uSerTimeZone = "", DateTime? completedDate = null, Guid dateFormatID = new Guid(), int intHeaderFooterOptions = 2);
        void AddDigitalSignatureToTheDocumentIN(string srcPDF, string srcPDFPassword, string dstPDF, bool IsThrowException = false);
        void AddDigitalSignatureToTheDocument(string srcPDF, string srcPDFPassword, string dstPDF, bool IsThrowException = false);

    }
}
