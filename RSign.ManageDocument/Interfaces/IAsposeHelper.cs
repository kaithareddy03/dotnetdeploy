using RSign.Common.Helpers;
using RSign.ManageDocument.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Interfaces
{
    public interface IAsposeHelper
    {
        byte[] ConvertHtmlTextToPdf(string htmlContent);
        string MergepdfFiles(List<string> files, string pdfDocSavePath);
        string AddTextInPdf(String docFinalPath, String docSaveDir, List<DocumentControls> documentControls, List<int> pageNumbers, string imageDirectory, string eDisplayCode = "", string uSerTimeZone = "", DateTime? completedDate = null, Guid dateFormatID = new Guid(), int intHeaderFooterOptions = 2, int electronicSignatureStampId = 1, bool isPreview = false);
        string AddTextInPdf(String docFinalPath, String docSaveDir, List<DocumentControls> documentControls, List<int> pageNumbers, string imageDirectory, string password, string eDisplayCode = "", string uSerTimeZone = "", DateTime? completedDate = null, Guid dateFormatID = new Guid(), int intHeaderFooterOptions = 2, int electronicSignatureStampId = 1);
        bool ConvertPDFToImagesInXResolution(int quality, List<string> convertedPdfFiles, string dstImgDir);
        string ConvertImagesToPDF(List<string> convertedPdfFiles, string srcImagePath, string dstPDFPath);
        bool FlattenAllPdfFields(string messageId, string pdfFileName);
        bool AddWaterMark(string messageId, string pdfFileName, FinalContractSettings userSettings);
        int GetPagecountOfPdf(string inputFilePath);
        void CreateBlankPDF(string fromFile, string fileSavePath);
        string SplitPdfFiles(Dictionary<string, int> dicConvertedFile, bool IsFinalCertificate, List<String> files, String pdfDocSavePath, bool IsPasswordReqdtoOpen, string Password);
        bool IsFileEligibleForDigitallySign(string srcPDF, string password);
        string EncryptPdfFile(string docFinalPath, string userPassword, string ownerPassword, string docSaveDir);
        string CreateSignCertificate(string directorypath, CertificateData certificateData, string imgDirectory, string cultureInfo, string EnvelopeStatus, string userTimeZone, bool isTermsAccepted, bool? IsDisclaimerInCertificate, string strDisclaimer, int PaperSizeID);
        void TransformPDFFromDocxAndCopyToDestination(string inputFilePath, string convertedDir, string fileName);
        string AppendSignerEmailMobileDetails(int? deliveryMode, string EmailAddress, string DialCode, string Mobile);
    }
}
