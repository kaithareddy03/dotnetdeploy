using Aspose.Pdf;
using Aspose.Pdf.Annotations;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Facades;
using Aspose.Pdf.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.ManageDocument.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Helpers
{
    public class AsposeHelper : IAsposeHelper, IDisposable
    {
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration;
        public static readonly Guid Europe = new Guid("{577D1738-6891-45DE-8481-E3353EB6A963}");
        public static string PrefillNotation = "";
        public string FontFolderPath = "";
        public AsposeHelper(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;

            var pdfLicense = new Aspose.Pdf.License();
            var cellLicense = new Aspose.Cells.License();
            var slideLicense = new Aspose.Slides.License();
            var wordLicense = new Aspose.Words.License();
            try
            {
                string licPath = Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["AsposelicPath"])); ;
                pdfLicense.SetLicense(licPath);
                cellLicense.SetLicense(licPath);
                slideLicense.SetLicense(licPath);
                wordLicense.SetLicense(licPath);
                FontFolderPath = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["FontFolderPath"]));
                PrefillNotation = Convert.ToString(_appConfiguration["PrefillNotation"]);
                rsignlog = new RSignLogger(_appConfiguration);
            }
            catch (Exception ex)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        void IDisposable.Dispose() { }
        public void Dispose() { }
        public byte[] ConvertHtmlTextToPdf(string htmlContent)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper.cs", "ConvertHtmlTextToPdf", "Process started for Convert HtmlText To Pdf", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            /* Latest Version 20.x Implementation Starts here */
            // HTML string
            // instantiate HtmlLoadOptions object and set desrired properties.
            Aspose.Pdf.HtmlLoadOptions htmlLoadOptions = new Aspose.Pdf.HtmlLoadOptions();
            htmlLoadOptions.PageInfo.Margin.Left = 15;
            htmlLoadOptions.PageInfo.Margin.Top = 15;
            htmlLoadOptions.PageInfo.Margin.Bottom = 15;
            htmlLoadOptions.PageInfo.Margin.Right = 5;
            //Load HTML string
            Document doc = new Document(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlContent)), htmlLoadOptions);
            doc.FitWindow = true;
            //Save PDF file
            MemoryStream outStream = new MemoryStream();
            doc.Save(outStream);
            // Convert the document to byte form.
            byte[] docBytes = outStream.ToArray();
            loggerModelNew.Message = "Process completed for Convert HtmlText To Pdf";
            rsignlog.RSignLogInfo(loggerModelNew);
            return docBytes;
            /* Latest Version 20.x Implementation Ends here */
        }
        public string MergepdfFiles(List<String> files, String pdfDocSavePath)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "MergepdfFiles", "Process is started for Merge pdf Files method ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            // If incomming file count is one. return without merging
            if (files.Count == 1)
            {
                using (Document newDoc = new Document())
                {
                    var pdfFirst1 = new Document(files[0]);
                    foreach (Page pg in pdfFirst1.Pages)
                    {
                        newDoc.Pages.Add(pg);
                    }
                    newDoc.Save(pdfDocSavePath);
                }
                return pdfDocSavePath;
            }

            // First order all files by Time created
            //files = files.OrderBy(fn => new FileInfo(fn).CreationTime).ToList();

            var firstDocument = files[0];
            var secondDoucment = files[1];

            var pdfFirst = new Document(firstDocument);
            var pdfSecond = new Document(secondDoucment);

            //*******************Added by TParker ********************//
            if (pdfFirst.IsEncrypted == true)
            {
                pdfFirst.Decrypt();
            }
            if (pdfSecond.IsEncrypted == true)
            {
                pdfSecond.Decrypt();
            }
            //*******************End ********************//

            for (var i = 1; i <= pdfSecond.Pages.Count; i++)
            {
                pdfFirst.Pages.Add(pdfSecond.Pages[i]);
            }

            if (!Directory.Exists(pdfDocSavePath.Substring(0, pdfDocSavePath.LastIndexOf("\\"))))
                Directory.CreateDirectory(pdfDocSavePath.Substring(0, pdfDocSavePath.LastIndexOf("\\")));

            pdfFirst.Save(pdfDocSavePath);

            var mainPdfFile = pdfDocSavePath;
            var mainPdf = new Document(mainPdfFile);

            for (int i = 2; i < files.Count; i++)
            {
                var nextPdfFile = new Document(files[i]);
                for (var j = 1; j <= nextPdfFile.Pages.Count; j++)
                {
                    mainPdf.Pages.Add(nextPdfFile.Pages[j]);
                }
            }

            var finalpdfSavePath = pdfDocSavePath;
            mainPdf.Save(finalpdfSavePath);

            loggerModelNew.Message = "Process completed for MergepdfFiles method and PDF generated";
            rsignlog.RSignLogInfo(loggerModelNew);
            return finalpdfSavePath;
        }
        public String AddTextInPdf(String docFinalPath, String docSaveDir, List<DocumentControls> documentControls, List<int> pageNumbers,
            string imageDirectory, string eDisplayCode = "", string uSerTimeZone = "", DateTime? completedDate = null, Guid dateFormatID = new Guid(),
            int intHeaderFooterOptions = 2, int electronicSignatureStampId = 1, bool isPreview = false)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "AddTextInPdf", "Process is started for AddTextInPdf method ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            var pdfDocument = new Document(docFinalPath);
            string dateFormat = dateFormatID != Guid.Empty && dateFormatID == Europe ? "{0:dd/MM/yyyy HH:mm tt}" : "{0:MM/dd/yyyy HH:mm tt}";
            foreach (var pageNumber in pageNumbers)
            {
                // Get the page of selected pdf document
                var pdfPageNo = pdfDocument.Pages[pageNumber];
                var textBuilderFooter = new TextBuilder(pdfPageNo);

                // Get all controls for perticular page
                var controlsByPage = documentControls.Where(x => x.PageNo == pageNumber).ToList();

                foreach (var control in controlsByPage)
                {
                    if (control.ControlName == "Checkbox" || control.ControlName == "Radio" || ((control.ControlName == "Signature" || control.ControlName == "NewInitials") && control.ControlValue == "Signed"))
                    {
                        if (control.ImageBytes != null && control.ImageBytes.Length > 0)
                            InsertImageInPdf(pdfDocument, pageNumber, control, pdfPageNo, imageDirectory, electronicSignatureStampId);
                    }
                    else
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(imageDirectory);
                        var files = dirInfo.GetFiles(Path.GetFileNameWithoutExtension(control.DocumentName) + "_*.png")
                            .Where(item => Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == Path.GetFileNameWithoutExtension(item.ToString()))
                            .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                        if (files.Count == 0)
                        {
                            files = dirInfo.GetFiles((Path.GetFileNameWithoutExtension(control.DocumentName)) + "_*.jpg")
                                .Where(item => Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == Path.GetFileNameWithoutExtension(item.ToString()))
                                .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                        }
                        var myFile = files[control.DocumentPageNo - 1];
                        double assumedWidth = 948;
                        double originalWidth = 0.0;
                        using (Bitmap myImage = new Bitmap(Path.Combine(imageDirectory, myFile)))
                        {
                            originalWidth = myImage.Width;
                        }

                        // create TextBuilder object
                        var textBuilder = new TextBuilder(pdfPageNo);

                        // create text fragment
                        var textValue = (control.ControlName == "Label" || control.ControlName == "Text" || control.ControlName == "Hyperlink") ? control.Label : "[ " + (control.ControlName == "NewInitials" ? "Initials" : control.ControlName) + " ]"; //"[ " + GetControlName(control.ControlName) + " ]";

                        string controlValueForLink = "";
                        if (control.ControlName == "Hyperlink" && !string.IsNullOrEmpty(control.ControlValue))
                            controlValueForLink = control.ControlValue;
                        else
                            controlValueForLink = control.Label;

                        var textFragment = new TextFragment();
                        if (!string.IsNullOrEmpty(control.ControlValue))
                        {
                            textFragment = new TextFragment(control.ControlValue);
                        }
                        else
                        {
                            textFragment = new TextFragment(textValue);
                        }

                        int extraHeight = 1;
                        if (control.UserControlStyle != null)
                        {
                            if (assumedWidth < originalWidth)
                            {//Increase font size
                                var ratio = ((originalWidth - assumedWidth) / originalWidth) * 100;
                                if (control.ControlName == "Label")
                                {
                                    control.UserControlStyle.FontSize = control.UserControlStyle.FontSize > 30 ? (control.UserControlStyle.FontSize - 2) : control.UserControlStyle.FontSize;
                                }
                                else
                                {
                                    control.UserControlStyle.FontSize = control.UserControlStyle.FontSize + (int)((control.UserControlStyle.FontSize * ratio) / 100);
                                }
                                extraHeight = control.UserControlStyle.FontSize > 10 ? Convert.ToInt32(control.UserControlStyle.FontSize / 10) : 1;

                                // control.UserControlStyle.FontSize = control.UserControlStyle.FontSize + (int)((control.UserControlStyle.FontSize * ratio) / 100);
                            }
                            else
                            {//Decrease font size
                                var ratio = ((assumedWidth - originalWidth) / assumedWidth) * 100;
                                control.UserControlStyle.FontSize = control.UserControlStyle.FontSize - (int)((control.UserControlStyle.FontSize * ratio) / 100);
                                extraHeight = control.UserControlStyle.FontSize > 10 ? Convert.ToInt32(control.UserControlStyle.FontSize / 10) : 1;
                            }
                        }
                        else
                        {//If User control style is null then we are considering 12 will be the font size of control(e.g dropdown control)
                            int defaultFontSize = 12;
                            if (assumedWidth < originalWidth)
                            {//Increase font size
                                var ratio = ((originalWidth - assumedWidth) / originalWidth) * 100;
                                defaultFontSize = defaultFontSize + (int)((defaultFontSize * ratio) / 100);
                                extraHeight = Convert.ToInt32(defaultFontSize / 10);
                            }
                            else
                            {//Decrease font size
                                var ratio = ((assumedWidth - originalWidth) / assumedWidth) * 100;
                                defaultFontSize = defaultFontSize - (int)((defaultFontSize * ratio) / 100);
                                extraHeight = Convert.ToInt32(defaultFontSize / 10);
                            }
                            textFragment.TextState.FontSize = defaultFontSize / 1.33F;
                        }
                        var leftpoint = (control.XLeftposition * 72.00) / 112.00;
                        var bottompoint = (control.ZBottompostion * 72.00) / 112.00;
                        var width = (float)((control.Width * 72.00) / 112.00);
                        var height = (float)((control.Height * 72.00) / 112.00);

                        // This method takes left and bottom position to render control
                        textFragment.Position = new Position(leftpoint, bottompoint);

                        if (control.UserControlStyle != null)
                        {
                            // set text properties. The text size is in pixel, to convert it to point we divided it by 1.33
                            textFragment.TextState.FontSize = control.ControlName == "Text" ? (control.UserControlStyle.FontSize / 1.52F) : (control.UserControlStyle.FontSize / 1.33F);

                            if (control.UserControlStyle.IsBold && control.UserControlStyle.IsItalic
                                && control.UserControlStyle.IsUnderline)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Bold | FontStyles.Italic;
                                textFragment.TextState.Underline = control.UserControlStyle.IsUnderline;
                            }
                            else if (control.UserControlStyle.IsBold && control.UserControlStyle.IsItalic)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Bold | FontStyles.Italic;
                            }
                            else if (control.UserControlStyle.IsBold && control.UserControlStyle.IsUnderline)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Bold;
                                textFragment.TextState.Underline = control.UserControlStyle.IsUnderline;
                            }
                            else if (control.UserControlStyle.IsItalic && control.UserControlStyle.IsUnderline)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Italic;
                                textFragment.TextState.Underline = control.UserControlStyle.IsUnderline;
                            }
                            else if (control.UserControlStyle.IsBold)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Bold;
                            }
                            else if (control.UserControlStyle.IsItalic)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Italic;
                            }
                            else if (control.UserControlStyle.IsUnderline)
                            {
                                textFragment.TextState.Underline = control.UserControlStyle.IsUnderline;
                            }

                            try
                            {
                                textFragment.TextState.Font = FontRepository.FindFont(control.UserControlStyle.FontName);
                            }
                            catch (Exception ex)
                            {
                                Aspose.Pdf.Text.Font font = FontRepository.OpenFont(Path.Combine(FontFolderPath, control.UserControlStyle.FontName) + ".ttf");
                                font.IsEmbedded = true;
                                textFragment.TextState.Font = font;
                            }

                            textFragment.TextState.ForegroundColor =
                                Aspose.Pdf.Color.Parse(control.UserControlStyle.FontColor);
                        }

                        bool isTextControl = control.ControlName == "Text" && control.ControlType != "Email" ? true : control.ControlName == "Text" && control.ControlType == "Email" && !Convert.ToBoolean(control.IsFixedWidth) ? true : false;

                        if (isTextControl)
                        {
                            bottompoint = (control.Width < 100 && (string.IsNullOrEmpty(control.ControlValue) || control.ControlValue.Length == 1))
                                  ? (bottompoint - 7) : (bottompoint - 7);
                            TextParagraph paragraph = new TextParagraph();
                            textFragment.TextState.LineSpacing = (float)0;
                            leftpoint = leftpoint < 0 ? 1 : leftpoint;
                            float widthModifier = (control.ControlType == "SSN") ? 9 : 0;
                            paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint, bottompoint, (leftpoint + width + widthModifier), (bottompoint + height + 7));
                            paragraph.FormattingOptions.WrapMode = TextFormattingOptions.WordWrapMode.DiscretionaryHyphenation;
                            paragraph.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;
                            paragraph.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
                            paragraph.Margin = new Aspose.Pdf.MarginInfo(2, 2, 2, 2);
                            // paragraph.AppendLine(!string.IsNullOrEmpty(control.ControlValue) ? control.ControlValue : string.Empty, textFragment.TextState);
                            // paragraph.AppendLine(control.ControlValue, textFragment.TextState);
                            paragraph.AppendLine(textFragment);
                            textBuilder.AppendParagraph(paragraph);
                        }
                        else if (control.ControlName == "Name" && !Convert.ToBoolean(control.IsFixedWidth))
                        {
                            bottompoint = (control.Width < 100 && (string.IsNullOrEmpty(control.ControlValue) || control.ControlValue.Length == 1))
                                ? (bottompoint - 7) : (bottompoint - 7);
                            TextParagraph paragraph = new TextParagraph();
                            textFragment.TextState.LineSpacing = (float)0;
                            leftpoint = leftpoint < 0 ? 1 : leftpoint;
                            float widthModifier = 20;
                            paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint, bottompoint, (leftpoint + width + widthModifier), (bottompoint + height + 7));
                            paragraph.FormattingOptions.WrapMode = TextFormattingOptions.WordWrapMode.DiscretionaryHyphenation;
                            paragraph.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;
                            paragraph.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
                            paragraph.Margin = new Aspose.Pdf.MarginInfo(2, 2, 2, 2);
                            paragraph.AppendLine(control.ControlValue, textFragment.TextState);
                            textBuilder.AppendParagraph(paragraph);
                        }
                        else if (control.ControlName == "Label")
                        {
                            bottompoint = (control.Width < 100 && (string.IsNullOrEmpty(control.ControlValue) || control.ControlValue.Length == 1))
                                ? (bottompoint - 7) : (bottompoint - 10);
                            TextParagraph paragraph = new TextParagraph();
                            textFragment.TextState.LineSpacing = (float)0;

                            paragraph.FormattingOptions.WrapMode = TextFormattingOptions.WordWrapMode.ByWords;
                            paragraph.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;
                            paragraph.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
                            paragraph.Margin = new Aspose.Pdf.MarginInfo(0, 0, 0, 0);
                            paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint, bottompoint, (leftpoint + width + 10), (bottompoint + height + 10));
                            paragraph.AppendLine(textFragment);
                            textBuilder.AppendParagraph(paragraph);
                        }
                        else if (control.ControlName == "Hyperlink")
                        {
                            textFragment.TextState.Underline = true;
                            textFragment.Position = new Position(leftpoint, bottompoint);
                            LinkAnnotation link = new LinkAnnotation(pdfPageNo, textFragment.Rectangle);
                            link.Action = new GoToURIAction(controlValueForLink);
                            Border border = new Border(link);
                            border.Width = 0;
                            link.Border = border;
                            textBuilder.AppendText(textFragment);
                            pdfPageNo.Annotations.Add(link);
                        }
                        else
                        {
                            textFragment.Position = new Position(leftpoint, bottompoint);
                            textBuilder.AppendText(textFragment); // append the text fragment to the PDF page
                        }
                    }

                }
            }

            //create footer
            if (!string.IsNullOrEmpty(eDisplayCode) && intHeaderFooterOptions > 1)
            {
                string strCompletedDate = string.Empty;
                if (string.IsNullOrEmpty(uSerTimeZone))
                    strCompletedDate = String.Format(dateFormat, completedDate) + " " + GetTimeZoneAbbreviation(TimeZone.CurrentTimeZone.StandardName);
                else
                    strCompletedDate = string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(completedDate), uSerTimeZone, dateFormatID));
                foreach (Page page in pdfDocument.Pages)
                {
                    TextParagraph paragraphFooter = GetFooterParagraph(eDisplayCode + System.Environment.NewLine + strCompletedDate, "Courier", intHeaderFooterOptions, page);
                    try
                    {///First trying with Arial Font
                        var textBuilder = new TextBuilder(page);
                        textBuilder.AppendParagraph(paragraphFooter);
                    }
                    catch (Exception exArial)
                    {
                        try
                        {///First trying with Time New Roman Font
                            paragraphFooter = GetFooterParagraph(eDisplayCode + System.Environment.NewLine + strCompletedDate, "TimesNewRoman", intHeaderFooterOptions, page);
                            var textBuilder = new TextBuilder(page);
                            textBuilder.AppendParagraph(paragraphFooter);
                        }
                        catch (Exception exTNR)
                        {
                            try
                            {///First trying with Helvetica Font
                                paragraphFooter = GetFooterParagraph(eDisplayCode + System.Environment.NewLine + strCompletedDate, "Helvetica", intHeaderFooterOptions, page);
                                var textBuilder = new TextBuilder(page);
                                textBuilder.AppendParagraph(paragraphFooter);
                            }
                            catch (Exception exHelvetica)
                            {
                                try
                                {///First trying with Calibri Font
                                    paragraphFooter = GetFooterParagraph(eDisplayCode + System.Environment.NewLine + strCompletedDate, "Calibri", intHeaderFooterOptions, page);
                                    var textBuilder = new TextBuilder(page);
                                    textBuilder.AppendParagraph(paragraphFooter);
                                }
                                catch (Exception exCalibri)
                                {

                                    loggerModelNew.Message = "Error occurred in AddTextInPdf method while rendering footer on final document." + exCalibri.Message.ToString();
                                    rsignlog.RSignLogError(loggerModelNew, exCalibri);
                                    return string.Empty;
                                    //throw exCalibri;
                                }
                            }
                        }
                    }
                }
            }

            // save document
            pdfDocument.Save(docSaveDir);
            loggerModelNew.Message = "Process is completed for Add Text In Pdf method ";
            rsignlog.RSignLogWarn(loggerModelNew);
            return docSaveDir;
        }
        public string WrapText(string sentence)
        {
            int myLimit = 10;
            string[] words = sentence.Split(' ');
            StringBuilder newSentence = new StringBuilder();
            string line = "";
            foreach (string word in words)
            {
                if ((line + word).Length > myLimit)
                {
                    newSentence.AppendLine(line);
                    line = "";
                }
                line += string.Format("{0} ", word);
            }

            if (line.Length > 0)
                newSentence.AppendLine(line);

            return newSentence.ToString();
        }
        private void InsertImageInPdf(Document pdfDocument, int pageNo, DocumentControls control, Page pdfPageNo, string imageDirectory, int electronicSignatureStampId = 1)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "InsertImageInPdf", "Process is started for InsertImageInPdf method to insert image in pdf", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            if (control.ControlValue != null || (control.ControlName == "Radio" || control.ControlName == "Checkbox"))
            {
                System.Drawing.Image objImage = System.Drawing.Image.FromStream(new System.IO.MemoryStream(control.ImageBytes));
                double imgWidth = objImage.Width;
                double imgHeight = objImage.Height;
                var ratio = 0.0;
                if (imgWidth > control.Width)
                {
                    ratio = control.Width / imgWidth;
                    imgWidth = (imgWidth * ratio);
                    imgHeight = (imgHeight * ratio);
                }

                if (imgHeight > control.Height)
                {
                    ratio = control.Height / imgHeight;
                    imgWidth = (imgWidth * ratio);
                    imgHeight = (imgHeight * ratio);
                }
                var margin = (control.Width - imgWidth) / 2;

                DirectoryInfo dirInfo = new DirectoryInfo(imageDirectory);
                var files = dirInfo.GetFiles((Path.GetFileNameWithoutExtension(control.DocumentName)) + "_*.png")
                    .Where(item => Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == Path.GetFileNameWithoutExtension(item.ToString()))
                    .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                if (files.Count == 0)
                {
                    files = dirInfo.GetFiles((Path.GetFileNameWithoutExtension(control.DocumentName)) + "_*.jpg")
                        .Where(item => Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == Path.GetFileNameWithoutExtension(item.ToString()))
                        .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                }
                var myFile = files[control.DocumentPageNo - 1];
                double assumedWidth = 948;
                double originalWidth = 0.0;
                using (Bitmap myImage = new Bitmap(Path.Combine(imageDirectory, myFile)))
                {
                    originalWidth = myImage.Width;
                }

                double width = 0.0;
                double height = 0.0;
                // Calculating control's postion in points

                if (electronicSignatureStampId > 1 && control.ControlName == "Signature")
                {
                    width = (imgWidth * 82.00) / 112.00;

                    // Fixed width is 120 points; // Control width is 160px to point.
                    height = (imgHeight * 82.00) / 112.00;
                }
                else
                {
                    width = (imgWidth * 72.00) / 112.00;

                    // Fixed width is 120 points; // Control width is 160px to point.
                    height = (imgHeight * 72.00) / 112.00;

                }
                // Fixed height is 31.5 points; // Control Height is 42px.
                double leftPosition = ((control.XLeftposition + margin) * 72.00) / 112.00;
                double bottomPosition = (control.ZBottompostion * 72.00) / 112.00;
                //if (electronicSignatureStampId <= 1 && control.ControlName == "Signature" && control.IsTypeSignature != null && control.IsTypeSignature.Value)
                //{
                //    bottomPosition = bottomPosition - 7;
                //    leftPosition = leftPosition - ((margin / 2) + 12);
                //}

                if (electronicSignatureStampId <= 1 && control.ControlName == "Signature")
                {
                    if (control.IsTypeSignature != null && control.IsTypeSignature.Value)
                    {
                        bottomPosition = bottomPosition - 0;
                        leftPosition = leftPosition - ((margin / 2) + 3); //2 or 3px;
                    }
                    else if (control.IsHandSignature != null && control.IsHandSignature.Value)
                    {
                        bottomPosition = bottomPosition - 0;
                        leftPosition = leftPosition - ((margin / 2));
                    }
                    else if (control.IsUploadSignature != null && control.IsUploadSignature.Value)
                    {
                        bottomPosition = bottomPosition - 1;
                        leftPosition = leftPosition - ((margin / 2) + 2);
                    }
                }
                if ((electronicSignatureStampId == 2 || electronicSignatureStampId == 3) && control.ControlName == "Signature")
                {
                    if (control.IsTypeSignature != null && control.IsTypeSignature.Value)
                    {
                        bottomPosition = bottomPosition - 0;
                        leftPosition = leftPosition - ((margin / 2) + 5);
                    }
                    else if (control.IsHandSignature != null && control.IsHandSignature.Value)
                    {
                        bottomPosition = bottomPosition - 0;
                        leftPosition = leftPosition - ((margin / 2) + 3);
                    }
                    else if (control.IsUploadSignature != null && control.IsUploadSignature.Value)
                    {
                        bottomPosition = bottomPosition - 0;
                        leftPosition = leftPosition - ((margin / 2) + 3);
                    }
                }


                // set coordinates
                double lowerLeftX = width;
                double lowerLeftY = height;
                double upperRightX = 0;
                double upperRightY = 0;

                if (control.ControlName == "Checkbox" || control.ControlName == "Radio")
                {
                    lowerLeftX = imgWidth > 0 ? width : 10;
                    lowerLeftY = imgHeight > 0 ? height : 10;
                    if (control.Height == 14)
                        leftPosition = leftPosition - 1;
                }

                using (MemoryStream imgStream = new MemoryStream(control.ImageBytes))
                {
                    // create image stamp
                    ImageStamp imageStamp = new ImageStamp(imgStream)
                    {
                        XIndent = leftPosition,
                        YIndent = bottomPosition,
                        Height = lowerLeftY,
                        Width = lowerLeftX,
                        Rotate = Rotation.None
                    };
                    if (assumedWidth < originalWidth)
                    {//Increase font size
                        var tempratio = ((originalWidth - assumedWidth) / originalWidth) * 100;
                        imageStamp.Width = imageStamp.Width + ((imageStamp.Width * tempratio) / 100);
                        imageStamp.Height = imageStamp.Height + ((imageStamp.Height * tempratio) / 100);
                    }
                    else
                    {//Decrease font size
                        var tempratio = ((assumedWidth - originalWidth) / assumedWidth) * 100;
                        imageStamp.Width = imageStamp.Width - ((imageStamp.Width * tempratio) / 100);
                        imageStamp.Height = imageStamp.Height - ((imageStamp.Height * tempratio) / 100);
                    }
                    // add stamp to particular page
                    pdfDocument.Pages[pageNo].AddStamp(imageStamp);
                }

                #region Temporarily disabled old image insert code
                /*

                var imageStream = new MemoryStream(control.ImageBytes);
                pdfPageNo.Resources.Images.Add(imageStream);

                pdfPageNo.Contents.Add(new Operator.GSave());
                var rectangle = new Aspose.Pdf.Rectangle(lowerLeftX, lowerLeftY, upperRightX, upperRightY);

                var matrix =
                    new Aspose.Pdf.DOM.Matrix(
                        new[]
                            {
                                rectangle.URX - rectangle.LLX, 0, 0, rectangle.URY - rectangle.LLY, leftPosition, // rectangle.LLX, Control's left position from page
                                bottomPosition // rectangle.LLY  Control's botom position from page
                            });

                pdfPageNo.Contents.Add(new Operator.ConcatenateMatrix(matrix));
                XImage ximage = pdfPageNo.Resources.Images[pdfPageNo.Resources.Images.Count];

                pdfPageNo.Contents.Add(new Operator.Do(ximage.Name));
                pdfPageNo.Contents.Add(new Operator.GRestore());

                */
                #endregion
            }
            loggerModelNew.Message = "Process completed for InsertImageInPdf method to insert image in pdf";
            rsignlog.RSignLogInfo(loggerModelNew);
        }
        string GetTimeZoneAbbreviation(string timeZoneName)
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
        private string GetLocalTime(DateTime timeUtc, string SelectedTimeZone, Guid dateFormatID = new Guid())
        {
            //GetTimeZoneBasedOnUserId
            try
            {
                //timeUtc = TimeZoneInfo.ConvertTimeToUtc(timeUtc, TimeZoneInfo.Local);
                string outputDate = string.Empty;
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(SelectedTimeZone);
                DateTime dateTimeToConvert = new DateTime(timeUtc.Ticks, DateTimeKind.Unspecified);
                DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeToConvert, timeZone);
                //DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZone);
                string output = string.Empty;
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
                //if (dateFormatID != Guid.Empty && dateFormatID == Europe)
                //{
                //    outputDate = String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", cstTime);
                //}
                //else
                //{
                //    outputDate = Convert.ToString(cstTime);
                //}
                if (dateFormatID != Guid.Empty)
                {
                    if (dateFormatID == Constants.DateFormat.US_mm_dd_yyyy_slash)
                        outputDate = String.Format("{0:MM/dd/yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan)
                        outputDate = String.Format("{0:MM-dd-yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots)
                        outputDate = String.Format("{0:MM.dd.yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_slash)
                        outputDate = String.Format("{0:dd/MM/yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_colan)
                        outputDate = String.Format("{0:dd-MM-yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_dots)
                        outputDate = String.Format("{0:dd.MM.yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_yyyy_mm_dd_dots)
                        outputDate = String.Format("{0:yyyy.MM.dd. HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan)
                        outputDate = String.Format("{0:dd-MMM-yyyy HH:mm tt}", cstTime);
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
        }
        private TextParagraph GetFooterParagraph(string footerText, string fontType, int intHeaderFooterOptions, Page pageFooter)
        {
            var textFormattedFooterDT = new TextFragment(footerText);
            try
            {
                textFormattedFooterDT.TextState.Font = FontRepository.FindFont(fontType);
            }
            catch (Exception ex)
            {
                Aspose.Pdf.Text.Font font = FontRepository.OpenFont(Path.Combine(FontFolderPath, fontType) + ".ttf");
                font.IsEmbedded = true;
                textFormattedFooterDT.TextState.Font = font;
            }
            //textFormattedFooterDT.TextState.Font = FontRepository.FindFont(fontType);
            textFormattedFooterDT.TextState.FontSize = 9;
            TextParagraph paragraphFooter = new TextParagraph();
            paragraphFooter.AppendLine(textFormattedFooterDT);
            switch (intHeaderFooterOptions)
            {
                case 2: //LeftFooter
                    paragraphFooter.Position = new Position(pageFooter.Rect.LLX + 10, pageFooter.Rect.LLY + 20); // LeftFooter
                    break;
                case 3: //RightFooter
                    paragraphFooter.Position = new Position(pageFooter.Rect.URX - 180, pageFooter.Rect.LLY + 20); // RightFooter

                    break;
                case 4: //LeftHeader
                        // paragraphFooter.Position = new Position(pageFooter.Rect.LLX + 10, pageFooter.Rect.URY - 20); // LeftHeader
                    paragraphFooter.Position = new Position(pageFooter.Rect.LLX, pageFooter.Rect.URY - 20); // LeftHeader
                    break;
                case 5: //RightHeader
                    paragraphFooter.Position = new Position(pageFooter.Rect.URX - 180, pageFooter.Rect.URY - 20); //RightHeader
                    break;
            }
            return paragraphFooter;
        }
        public bool ConvertPDFToImagesInXResolution(int quality, List<string> convertedPdfFiles, string dstImgDir)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "ConvertPDFToImagesInXResolution", "Process is started for Convert PDF To Images In XResolution method ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                var resolutionImg = new Resolution(quality);
                var resolutionThumb = new Resolution(90, 50);
                foreach (var fle in convertedPdfFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(fle);
                    using (var pdfDoc = new Document(fle))
                    {
                        int pagesLength = (pdfDoc.Pages.Count + 1);
                        Parallel.For(1, pagesLength, i =>
                        {
                            new PngDevice(resolutionImg).Process(pdfDoc.Pages[i], Path.Combine(dstImgDir, fileName + "_" + i + ".png"));
                        });
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in ConvertPDFToImagesInXResolution method." + ex.Message.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return false;
            }
        }
        public string ConvertImagesToPDF(List<string> convertedPdfFiles, string srcImagePath, string dstPDFPath)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "ConvertImagesToPDF", "Process is started for Convert Images To PDF method ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(srcImagePath);
                List<EnvelopeImageCollection> imageCollection = new List<EnvelopeImageCollection>();
                using (Document doc = new Document())
                {
                    foreach (var pdf in convertedPdfFiles)
                    {
                        var files = dirInfo.GetFiles(Path.GetFileNameWithoutExtension(pdf) + "_*.png")
                            .Where(item => Path.GetFileNameWithoutExtension(pdf) + "_" + Regex.Match(Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == Path.GetFileNameWithoutExtension(item.ToString()))
                            .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                        imageCollection.AddRange(files.Select(file =>
                              new EnvelopeImageCollection
                              {
                                  ID = int.Parse(file.Substring(file.LastIndexOf('_') + 1, file.LastIndexOf('.') - 1 - file.LastIndexOf('_'))),
                                  FilePath = Path.Combine(srcImagePath, file).Replace(@"\", "/"),
                              }).OrderBy(order => order.ID).ToList());
                        using (Document convPdfDoc = new Document(pdf))
                        {
                            int count = 0;
                            foreach (var img in imageCollection)
                            {
                                count += 1;
                                Page page = doc.Pages.Add();
                                FileStream fs = new FileStream(img.FilePath, FileMode.Open, FileAccess.Read);
                                byte[] tmpBytes = new byte[fs.Length];
                                fs.Read(tmpBytes, 0, Convert.ToInt32(fs.Length));
                                MemoryStream mystream = new MemoryStream(tmpBytes);
                                Bitmap b = new Bitmap(mystream);
                                page.PageInfo.Margin.Left = 0;
                                page.PageInfo.Margin.Bottom = 0;
                                page.PageInfo.Margin.Top = 0;
                                page.PageInfo.Margin.Right = 0;
                                page.CropBox = new Aspose.Pdf.Rectangle(0, 0, b.Width, b.Height);
                                page.SetPageSize(convPdfDoc.Pages[count].MediaBox.Width,
                                    convPdfDoc.Pages[count].MediaBox.Height);
                                Aspose.Pdf.Image image1 = new Aspose.Pdf.Image();
                                page.Paragraphs.Add(image1);
                                image1.ImageStream = mystream;
                            }
                        }
                        imageCollection.Clear();
                    }
                    doc.Save(dstPDFPath);
                }
                return dstPDFPath;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in ConvertImagesToPDF method." + ex.Message.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return string.Empty;
            }
        }
        public bool FlattenAllPdfFields(string messageId, string pdfFileName)
        {
            var convertResult = false;
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "FlattenAllPdfFields", "Initiate the process for Form Flattening All Pdf Fields on final PDF for " + messageId, messageId, "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                if (!File.Exists(pdfFileName))
                {
                    return convertResult;
                }
                Aspose.Pdf.Facades.Form flattenForm = new Aspose.Pdf.Facades.Form();
                flattenForm.BindPdf(pdfFileName);
                if (flattenForm.FieldNames == null || flattenForm.FieldNames.Length == 0)
                {
                    //  loggerModel.Message = "Form Flattening fiels not found.";
                    //  rsignlog.RSignLogInfo(loggerModel);
                    loggerModelNew.Message = "Form Flattening fiels not found.";
                    rsignlog.RSignLogInfo(loggerModelNew);
                    return convertResult;
                }

                String[] allfields = flattenForm.FieldNames;

                for (int i = 0; i < allfields.Length; i++)
                {
                    flattenForm.FlattenField(allfields[i]);
                }

                //  flattenForm.FlattenAllFields();
                flattenForm.Save(pdfFileName);
                var flattenFileInfo = new FileInfo(pdfFileName);
                if (flattenFileInfo.Exists && flattenFileInfo.Length > 0)
                {
                    loggerModelNew.Message = "Form Flattening fields save successfully for file " + Path.GetFileName(pdfFileName);
                    rsignlog.RSignLogInfo(loggerModelNew);
                    convertResult = true;
                }
            }
            catch (Exception exception)
            {
                loggerModelNew.Message = "Form Flattening fields save successfully on path " + exception.ToString();
                rsignlog.RSignLogError(loggerModelNew, exception);
                return false;
            }
            return convertResult;
        }
        public bool AddWaterMark(string messageId, string pdfFileName, FinalContractSettings userSettings)
        {
            var convertResult = false;

            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "AddWaterMark", "Initiate the process for adding watermarks on final PDF for " + messageId, messageId, "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                if (!File.Exists(pdfFileName)) //EnvSigner
                {
                    return convertResult;
                }

                var textStamp = AddTextWatermark(userSettings.WatermarkText, userSettings.IsBackgroundStamp);
                using (Document document = new Document(pdfFileName))
                {
                    // Add stamp to particular page
                    foreach (Page page in document.Pages)
                    {
                        //page.AddStamp(textStampForAuth);
                        page.AddStamp(textStamp);
                    }

                    // Save output document
                    document.Save(pdfFileName);
                }
            }
            catch (Exception exception)
            {
                loggerModelNew.Message = "Watermark saved successfully on path " + exception.ToString();
                rsignlog.RSignLogError(loggerModelNew, exception);
                return false;
            }
            return convertResult;
        }
        private TextStamp AddTextWatermark(string watermarkText, bool isBackgroundText)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "AddTextWatermark", "Add Text Watermark process is started ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            TextStamp textStamp = new TextStamp(watermarkText);
            textStamp.TextState.Font = FontRepository.FindFont("Calibri");
            textStamp.Background = true;
            textStamp.TextState.FontStyle = FontStyles.Bold;
            if (isBackgroundText == false)
            {
                textStamp.TopMargin = 10;
                textStamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Right;
                textStamp.VerticalAlignment = VerticalAlignment.Top;
                textStamp.TextState.FontSize = 14.0F;
                textStamp.Opacity = 0.5;
                textStamp.TextState.FontStyle = FontStyles.Italic;
                textStamp.TextState.ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Red);
            }
            else
            {
                // Set origin
                textStamp.XIndent = 100;
                textStamp.YIndent = 100;
                textStamp.RotateAngle = 45.0F;
                textStamp.Opacity = 0.3;
                if (watermarkText.Length > 7)
                    textStamp.TextState.FontSize = 80.0F;
                else
                    textStamp.TextState.FontSize = 150.0F;
                textStamp.TextState.FontStyle = FontStyles.Regular;
                textStamp.TextState.ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Gray);
            }
            loggerModelNew.Message = "Added Text Watermark process is completed ";
            rsignlog.RSignLogInfo(loggerModelNew);
            return textStamp;
        }
        public string AddTextInPdf(String docFinalPath, String docSaveDir, List<DocumentControls> documentControls, List<int> pageNumbers, string imageDirectory, string password, string eDisplayCode = "", string uSerTimeZone = "", DateTime? completedDate = null, Guid dateFormatID = new Guid(), int intHeaderFooterOptions = 2, int electronicSignatureStampId = 1)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "AddTextInPdf", "Process is started for AddTextInPdf method to write control on document", eDisplayCode, "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            var pdfDocument = new Document(docFinalPath);
            string dateFormat = dateFormatID != Guid.Empty && dateFormatID == Europe ? "{0:dd/MM/yyyy HH:mm tt}" : "{0:MM/dd/yyyy HH:mm tt}";
            foreach (var pageNumber in pageNumbers)
            {
                // Get the page of selected pdf document
                var pdfPageNo = pdfDocument.Pages[pageNumber];

                // Get all controls for perticular page
                var controlsByPage = documentControls.Where(x => x.PageNo == pageNumber).ToList();

                foreach (var control in controlsByPage)
                {
                    if (control.ControlName == "Signature" || control.ControlName == "NewInitials" || control.ControlName == "Checkbox" || control.ControlName == "Radio")
                    {
                        if (control.ImageBytes != null && control.ImageBytes.Length > 0)
                            InsertImageInPdf(pdfDocument, pageNumber, control, pdfPageNo, imageDirectory, electronicSignatureStampId);
                    }
                    else
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(imageDirectory);
                        var files = dirInfo.GetFiles((Path.GetFileNameWithoutExtension(control.DocumentName)) + "_*.png")
                            .Where(item => Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == Path.GetFileNameWithoutExtension(item.ToString()))
                            .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                        if (files.Count == 0)
                        {
                            files = dirInfo.GetFiles((Path.GetFileNameWithoutExtension(control.DocumentName)) + "_*.jpg")
                                .Where(item => Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == Path.GetFileNameWithoutExtension(item.ToString()))
                                .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                        }
                        var myFile = files[control.DocumentPageNo - 1];
                        double assumedWidth = 948;
                        double originalWidth = 0.0;
                        using (Bitmap myImage = new Bitmap(Path.Combine(imageDirectory, myFile)))
                        {
                            originalWidth = myImage.Width;
                        }
                        // create TextBuilder object
                        var textBuilder = new TextBuilder(pdfPageNo);
                        var controlValueForLink = string.Empty;
                        if (control.ControlName == "Hyperlink" && !string.IsNullOrEmpty(control.ControlValue))
                            controlValueForLink = control.ControlValue;
                        // create text fragment                        
                        if (control.ControlName == "Label" || control.ControlName == "Hyperlink")
                        {
                            control.ControlValue = control.Label;
                        }

                        if (control.ControlValue == null)
                        {
                            continue;
                        }

                        var textFragment = new TextFragment(control.ControlValue.Replace("\n", Environment.NewLine));
                        int extraHeight = 1;
                        if (control.UserControlStyle != null)
                        {
                            if (assumedWidth < originalWidth)
                            {//Increase font size
                                var ratio = ((originalWidth - assumedWidth) / originalWidth) * 100;
                                if (control.ControlName == "Label")
                                {
                                    control.UserControlStyle.FontSize = control.UserControlStyle.FontSize > 30 ? (control.UserControlStyle.FontSize - 2) : control.UserControlStyle.FontSize;
                                }
                                else
                                {
                                    control.UserControlStyle.FontSize = control.UserControlStyle.FontSize + (int)((control.UserControlStyle.FontSize * ratio) / 100);
                                }
                                extraHeight = control.UserControlStyle.FontSize > 10 ? Convert.ToInt32(control.UserControlStyle.FontSize / 10) : 1;
                            }
                            else
                            {//Decrease font size
                                var ratio = ((assumedWidth - originalWidth) / assumedWidth) * 100;
                                control.UserControlStyle.FontSize = control.UserControlStyle.FontSize - (int)((control.UserControlStyle.FontSize * ratio) / 100);
                                extraHeight = control.UserControlStyle.FontSize > 10 ? Convert.ToInt32(control.UserControlStyle.FontSize / 10) : 1;
                            }
                        }
                        else
                        {//If User control style is null then we are considering 12 will be the font size of control(e.g dropdown control)
                            int defaultFontSize = 12;
                            if (assumedWidth < originalWidth)
                            {//Increase font size
                                var ratio = ((originalWidth - assumedWidth) / originalWidth) * 100;
                                defaultFontSize = defaultFontSize + (int)((defaultFontSize * ratio) / 100);
                                extraHeight = Convert.ToInt32(defaultFontSize / 10);
                            }
                            else
                            {//Decrease font size
                                var ratio = ((assumedWidth - originalWidth) / assumedWidth) * 100;
                                defaultFontSize = defaultFontSize - (int)((defaultFontSize * ratio) / 100);
                                extraHeight = Convert.ToInt32(defaultFontSize / 10);
                            }
                            textFragment.TextState.FontSize = defaultFontSize / 1.33F;
                        }
                        var leftpoint = (control.XLeftposition * 72.00) / 112.00;
                        var bottompoint = (control.ZBottompostion * 72.00) / 112.00;
                        var width = (float)((control.Width * 72.00) / 112.00);
                        var height = (float)((control.Height * 72.00) / 112.00);

                        // This method takes left and bottom position to render control
                        textFragment.Position = new Position(leftpoint, bottompoint);

                        if (control.UserControlStyle != null)
                        {
                            // The text size is in pixel, to convert it to point we divided it by 1.52

                            textFragment.TextState.FontSize = control.ControlName == "Text" ? (control.UserControlStyle.FontSize / 1.52F) : (control.UserControlStyle.FontSize / 1.33F);

                            if (control.UserControlStyle.IsBold && control.UserControlStyle.IsItalic
                                && control.UserControlStyle.IsUnderline)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Bold | FontStyles.Italic;
                                textFragment.TextState.Underline = control.UserControlStyle.IsUnderline;
                            }
                            else if (control.UserControlStyle.IsBold && control.UserControlStyle.IsItalic)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Bold | FontStyles.Italic;
                            }
                            else if (control.UserControlStyle.IsBold && control.UserControlStyle.IsUnderline)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Bold;
                                textFragment.TextState.Underline = control.UserControlStyle.IsUnderline;
                            }
                            else if (control.UserControlStyle.IsItalic && control.UserControlStyle.IsUnderline)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Italic;
                                textFragment.TextState.Underline = control.UserControlStyle.IsUnderline;
                            }
                            else if (control.UserControlStyle.IsBold)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Bold;
                            }
                            else if (control.UserControlStyle.IsItalic)
                            {
                                textFragment.TextState.FontStyle = FontStyles.Italic;
                            }
                            else if (control.UserControlStyle.IsUnderline)
                            {
                                textFragment.TextState.Underline = control.UserControlStyle.IsUnderline;
                            }

                            try
                            {
                                textFragment.TextState.Font = FontRepository.FindFont(control.UserControlStyle.FontName);

                            }
                            catch (Exception ex)
                            {
                                Aspose.Pdf.Text.Font font = FontRepository.OpenFont(Path.Combine(FontFolderPath, control.UserControlStyle.FontName) + ".ttf");
                                font.IsEmbedded = true;
                                textFragment.TextState.Font = font;
                            }

                            textFragment.TextState.ForegroundColor = Aspose.Pdf.Color.Parse(control.UserControlStyle.FontColor);
                        }

                        bool isTextControl = control.ControlName == "Text" && control.ControlType != "Email" ? true : control.ControlName == "Text" && control.ControlType == "Email" && !Convert.ToBoolean(control.IsFixedWidth) ? true : false;

                        if (isTextControl)
                        {
                            bottompoint = (control.Width < 100 && (string.IsNullOrEmpty(control.ControlValue) || control.ControlValue.Length == 1))
                                ? (bottompoint - 7) : (bottompoint - 7);
                            TextParagraph paragraph = new TextParagraph();
                            textFragment.TextState.LineSpacing = (float)0;
                            leftpoint = leftpoint < 0 ? 1 : leftpoint;
                            float widthModifier = (control.ControlType == "SSN") ? 9 : 0;
                            paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint, bottompoint, (leftpoint + width + widthModifier), (bottompoint + height + 7));
                            paragraph.FormattingOptions.WrapMode = TextFormattingOptions.WordWrapMode.DiscretionaryHyphenation;
                            paragraph.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;
                            paragraph.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
                            paragraph.Margin = new Aspose.Pdf.MarginInfo(2, 2, 2, 2);
                            paragraph.AppendLine(control.ControlValue, textFragment.TextState);
                            textBuilder.AppendParagraph(paragraph);
                        }
                        else if (control.ControlName == "Name" && !Convert.ToBoolean(control.IsFixedWidth))
                        {
                            bottompoint = (control.Width < 100 && (string.IsNullOrEmpty(control.ControlValue) || control.ControlValue.Length == 1))
                                ? (bottompoint - 7) : (bottompoint - 7);
                            TextParagraph paragraph = new TextParagraph();
                            textFragment.TextState.LineSpacing = (float)0;
                            leftpoint = leftpoint < 0 ? 1 : leftpoint;
                            float widthModifier = 20;
                            paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint, bottompoint, (leftpoint + width + widthModifier), (bottompoint + height + 7));
                            paragraph.FormattingOptions.WrapMode = TextFormattingOptions.WordWrapMode.DiscretionaryHyphenation;
                            paragraph.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;
                            paragraph.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
                            paragraph.Margin = new Aspose.Pdf.MarginInfo(2, 2, 2, 2);
                            paragraph.AppendLine(control.ControlValue, textFragment.TextState);
                            textBuilder.AppendParagraph(paragraph);
                        }
                        else if (control.ControlName == "Label")
                        {
                            bottompoint = (control.Width < 100 && (string.IsNullOrEmpty(control.ControlValue) || control.ControlValue.Length == 1))
                                ? (bottompoint - 2) : (bottompoint - 40);

                            TextParagraph paragraph = new TextParagraph();
                            paragraph.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;
                            paragraph.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
                            textFragment.TextState.LineSpacing = (float)0;
                            if (bottompoint > 130)
                            {
                                paragraph.Rectangle = new Aspose.Pdf.Rectangle((leftpoint - (30)), bottompoint, (leftpoint + width), (bottompoint + 18 + height + (25 * extraHeight))); //new Aspose.Pdf.Rectangle(leftpoint, bottompoint, width, control.Height);
                            }
                            else if (bottompoint > 0 && bottompoint <= 130)
                            {
                                paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint - 40, bottompoint + 129, (leftpoint + width), (bottompoint + 38 + height + (25 * extraHeight)));
                            }
                            else if (bottompoint < 0 && bottompoint >= -24)
                            {
                                paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint - 40, bottompoint + 129, (leftpoint + width), (bottompoint + 38 + height + (25 * extraHeight))); //new Aspose.Pdf.Rectangle(leftpoint, bottompoint, width, control.Height);
                            }
                            else if (bottompoint < 0 && control.ZBottompostion <= 15)
                            {
                                paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint, bottompoint + 5, (leftpoint + width), control.Height); //new Aspose.Pdf.Rectangle(leftpoint, bottompoint, width, control.Height);
                            }
                            else
                            {
                                paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint, bottompoint, (leftpoint + width), control.Height);
                            }
                            //else
                            //{
                            //    paragraph.Rectangle = new Aspose.Pdf.Rectangle(leftpoint - 40, bottompoint + 100, (leftpoint + width), (bottompoint + 38 + height + (25 * extraHeight))); //new Aspose.Pdf.Rectangle(leftpoint, bottompoint, width, control.Height);
                            //}

                            paragraph.FormattingOptions.WrapMode = TextFormattingOptions.WordWrapMode.ByWords;
                            paragraph.Position = new Position(leftpoint, bottompoint);
                            paragraph.Margin = new Aspose.Pdf.MarginInfo(0, 0, 0, 0);
                            // textFragment.Position = new Position(leftpoint, bottompoint);
                            paragraph.AppendLine(textFragment);
                            textBuilder.AppendParagraph(paragraph);
                        }
                        else if (control.ControlName == "Hyperlink")
                        {
                            textFragment.TextState.Underline = true;
                            textFragment.Position = new Position(leftpoint, bottompoint);
                            LinkAnnotation link = new LinkAnnotation(pdfPageNo, textFragment.Rectangle);
                            link.Action = new GoToURIAction(controlValueForLink);
                            Border border = new Border(link);
                            border.Width = 0;
                            link.Border = border;
                            textBuilder.AppendText(textFragment);
                            pdfPageNo.Annotations.Add(link);
                        }
                        else
                        {
                            textFragment.Position = new Position(leftpoint, bottompoint);
                            textBuilder.AppendText(textFragment); // append the text fragment to the PDF page
                        }
                        //}
                    }
                }
            }

            //create footer
            if (!string.IsNullOrEmpty(eDisplayCode) && intHeaderFooterOptions > 1)
            {
                string strCompletedDate = string.Empty;
                if (string.IsNullOrEmpty(uSerTimeZone))
                    strCompletedDate = String.Format(dateFormat, completedDate) + " " + GetTimeZoneAbbreviation(TimeZone.CurrentTimeZone.StandardName);
                else
                    strCompletedDate = string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(completedDate), uSerTimeZone, dateFormatID));
                foreach (Page page in pdfDocument.Pages)
                {
                    TextParagraph paragraphFooter = GetFooterParagraph(eDisplayCode + System.Environment.NewLine + strCompletedDate, "Arial", intHeaderFooterOptions, page);
                    try
                    {///First trying with Arial Font
                        var textBuilder = new TextBuilder(page);
                        textBuilder.AppendParagraph(paragraphFooter);
                    }
                    catch (Exception exArial)
                    {
                        try
                        {///First trying with Time New Roman Font
                            paragraphFooter = GetFooterParagraph(eDisplayCode + System.Environment.NewLine + strCompletedDate, "TimesNewRoman", intHeaderFooterOptions, page);
                            var textBuilder = new TextBuilder(page);
                            textBuilder.AppendParagraph(paragraphFooter);
                        }
                        catch (Exception exTNR)
                        {
                            try
                            {///First trying with Helvetica Font
                                paragraphFooter = GetFooterParagraph(eDisplayCode + System.Environment.NewLine + strCompletedDate, "Helvetica", intHeaderFooterOptions, page);
                                var textBuilder = new TextBuilder(page);
                                textBuilder.AppendParagraph(paragraphFooter);
                            }
                            catch (Exception exHelvetica)
                            {
                                try
                                {///First trying with Calibri Font
                                    paragraphFooter = GetFooterParagraph(eDisplayCode + System.Environment.NewLine + strCompletedDate, "Calibri", intHeaderFooterOptions, page);
                                    var textBuilder = new TextBuilder(page);
                                    textBuilder.AppendParagraph(paragraphFooter);
                                }
                                catch (Exception exCalibri)
                                {
                                    loggerModelNew.Message = "Error occurred in AddTextInPdf method while rendering footer on final document." + exCalibri.Message.ToString();
                                    rsignlog.RSignLogError(loggerModelNew, exCalibri);
                                    return string.Empty;
                                    //throw exCalibri;
                                }
                            }
                        }
                    }
                }
            }
            // save document
            pdfDocument.Save(docSaveDir);
            loggerModelNew.Message = "Process completed for AddTextInPdf method to write control on document";
            rsignlog.RSignLogInfo(loggerModelNew);
            return docSaveDir;
        }
        public int GetPagecountOfPdf(string inputFilePath)
        {
            Document pdfDocument = new Document(inputFilePath);
            return pdfDocument.Pages.Count;
        }
        public string SplitPdfFiles(Dictionary<string, int> dicConvertedFile, bool IsFinalCertificate, List<String> files, String pdfDocSavePath, bool IsPasswordReqdtoOpen, string Password)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "SplitPdfFiles", "Process is started for Split Pdf Files method ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            string newSaveFilePath = string.Empty;
            int minCount = 0, maxCount = 0, totalCount = 0;
            var certPdfDoc = new Document();
            var MergedPdfDoc = new Document(files[0]);
            totalCount = MergedPdfDoc.Pages.Count;

            if (!Directory.Exists(pdfDocSavePath))
                Directory.CreateDirectory(pdfDocSavePath);
            if (files.Count > 1 && IsFinalCertificate)
            {
                var pdfSecond = new Document(files[1]);
                for (var i = 1; i <= pdfSecond.Pages.Count; i++)
                {
                    certPdfDoc.Pages.Add(pdfSecond.Pages[i]);
                }
            }

            foreach (var item in dicConvertedFile)
            {
                var newPDFDoc = new Document();

                if (minCount == 0)
                {
                    for (var i = minCount + 1; i <= item.Value && item.Value <= totalCount; i++)
                    {
                        newPDFDoc.Pages.Add(MergedPdfDoc.Pages[i]);
                    }
                    minCount = newPDFDoc.Pages.Count;

                    if (IsFinalCertificate && certPdfDoc.Pages.Count > 0)
                    {
                        for (var j = 1; j <= certPdfDoc.Pages.Count; j++)
                        {
                            newPDFDoc.Pages.Add(certPdfDoc.Pages[j]);
                        }
                        //newPDFDoc.Pages.Add(MergedPdfDoc.Pages[totalCount+1]);
                    }
                    newSaveFilePath = Path.Combine(pdfDocSavePath, item.Key + ".pdf");
                    newPDFDoc.Save(newSaveFilePath);
                    if (IsPasswordReqdtoOpen)
                        this.EncryptPdfFile(newSaveFilePath, Password, Password, newSaveFilePath);
                    else
                    {
                        //this.LockPdfFileFeatures(newSaveFilePath, newSaveFilePath);
                    }
                }
                else
                {
                    maxCount = minCount + item.Value;
                    for (var i = minCount + 1; i <= maxCount && maxCount <= totalCount; i++)
                    {
                        newPDFDoc.Pages.Add(MergedPdfDoc.Pages[i]);
                    }
                    minCount = minCount + newPDFDoc.Pages.Count;
                    //if (IsFinalCertificate)
                    //{
                    //    newPDFDoc.Pages.Add(MergedPdfDoc.Pages[totalCount+1]);
                    //}
                    if (IsFinalCertificate && certPdfDoc.Pages.Count > 0)
                    {
                        for (var j = 1; j <= certPdfDoc.Pages.Count; j++)
                        {
                            newPDFDoc.Pages.Add(certPdfDoc.Pages[j]);
                        }
                    }
                    newSaveFilePath = Path.Combine(pdfDocSavePath, item.Key + ".pdf");
                    newPDFDoc.Save(newSaveFilePath);
                    if (IsPasswordReqdtoOpen)
                        this.EncryptPdfFile(newSaveFilePath, Password, Password, newSaveFilePath);
                    else
                    {
                        //this.LockPdfFileFeatures(newSaveFilePath, newSaveFilePath);
                    }
                }
            }
            return pdfDocSavePath;
        }
        public string EncryptPdfFile(String docFinalPath, String userPassword, String ownerPassword, String docSaveDir)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "EncryptPdfFile", "Process is started for Encrypt Pdf File method", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            var document = new Document(docFinalPath);
            ////encrypt PDF
            document.Encrypt(userPassword, ownerPassword, 0, CryptoAlgorithm.AESx128);
            //save updated PDF
            document.Save(docSaveDir);
            loggerModelNew.Message = "Process is completed for Encrypt Pdf File method";
            rsignlog.RSignLogInfo(loggerModelNew);
            return docSaveDir;
        }
        public bool IsFileEligibleForDigitallySign(string srcPDF, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    using (PdfFileInfo info = new PdfFileInfo(srcPDF))
                    {
                        if (info.HasEditPassword)
                            return false;
                    }
                    using (Document doc = new Document(srcPDF))
                    {
                        return RemoveDigitalSignIfAny(doc, srcPDF);
                    }
                }
                else
                {
                    using (PdfFileInfo info = new PdfFileInfo(srcPDF, password))
                    {
                        if (info.HasEditPassword)
                            return false;
                    }
                    using (Document doc = new Document(srcPDF, password))
                    {
                        return RemoveDigitalSignIfAny(doc, srcPDF);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool RemoveDigitalSignIfAny(Document doc, string srcPDF)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "Aspose Helper", "RemoveDigitalSignIfAny", "Process is started for Remove Digital Sign If Any  ", "", "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);

                PdfFileSignature pdfSign = new PdfFileSignature();

                pdfSign.BindPdf(doc);
                // Get list of signature names
                var names = pdfSign.GetSignNames();
                // Remove all the signatures from the PDF file
                if (names != null)
                {
                    for (int index = 0; index < names.Count; index++)
                    {
                        pdfSign.RemoveSignature((string)names[index], true);
                    }
                }
                // Save updated PDF file
                pdfSign.Save(srcPDF);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string CreateSignCertificate(string directorypath, CertificateData certificateData, string imgDirectory, string cultureInfo, string EnvelopeStatus, string userTimeZone, bool isTermsAccepted, bool? IsDisclaimerInCertificate, string strDisclaimer, int PaperSizeID)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "CreateSignCertificate", "Process is started for Create Sign Certificate method ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            // First Create HTML file with the help of signing certificate data
            string dateFormat = certificateData.DateFormatId != Guid.Empty && certificateData.DateFormatId == Europe ? "{0:dd/MM/yyyy HH:mm tt}" : "{0:MM/dd/yyyy HH:mm tt}";
            string htmlfilePath = CreateHtmlFile(certificateData, directorypath, imgDirectory, cultureInfo, EnvelopeStatus, userTimeZone, isTermsAccepted, IsDisclaimerInCertificate, strDisclaimer);

            // Convert Html file into pdf and return save path
            var pdfFinalpathWithName = directorypath + "/" + certificateData.EnvelopeCode + "_SigningCertificate.pdf";
            var finalPdfPath = ConvertHtmlFileToPdf(htmlfilePath, imgDirectory, pdfFinalpathWithName, directorypath, PaperSizeID);
            return finalPdfPath;
        }
        public string CreateHtmlFile(CertificateData certificateData, string directoryPath, String imgFolderDirectory, string envCulture, string EnvelopeStatus, string uSerTimeZone, bool isTermsAccepted, bool? IsDisclaimerInCertificate, string strDisclemer)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "CreateHtmlFile", "Process is started for Create Html File method ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            var htmlBuilder = new StringBuilder();
            CertificateData mainData = certificateData;
            string dateFormat = mainData.DateFormatId == Europe ? certificateData.MetaDataAndHistory.DateFormatEU : certificateData.MetaDataAndHistory.DateFormatStr;
            if (certificateData.DateFormatId != Guid.Empty)
            {
                if (certificateData.DateFormatId == Constants.DateFormat.US_mm_dd_yyyy_slash)
                    dateFormat = "{0:MM/dd/yyyy HH:mm tt}";
                else if (certificateData.DateFormatId == Constants.DateFormat.US_mm_dd_yyyy_colan)
                    dateFormat = "{0:MM-dd-yyyy HH:mm tt}";
                else if (certificateData.DateFormatId == Constants.DateFormat.US_mm_dd_yyyy_dots)
                    dateFormat = "{0:MM.dd.yyyy HH:mm tt}";
                else if (certificateData.DateFormatId == Constants.DateFormat.Europe_mm_dd_yyyy_slash)
                    dateFormat = "{0:dd/MM/yyyy HH:mm tt}";
                else if (certificateData.DateFormatId == Constants.DateFormat.Europe_mm_dd_yyyy_colan)
                    dateFormat = "{0:dd-MM-yyyy HH:mm tt}";
                else if (certificateData.DateFormatId == Constants.DateFormat.Europe_mm_dd_yyyy_dots)
                    dateFormat = "{0:dd.MM.yyyy HH:mm tt}";
                else if (certificateData.DateFormatId == Constants.DateFormat.Europe_yyyy_mm_dd_dots)
                    dateFormat = "{0:yyyy.MM.dd. HH:mm tt}";
                else if (certificateData.DateFormatId == Constants.DateFormat.US_dd_mmm_yyyy_colan)
                    dateFormat = "{0:dd-MMM-yyyy HH:mm tt}";
            }
            else
                dateFormat = "{0:MM/dd/yyyy HH:mm tt}";
            const string htmlDocTypeDeclaration =
                @"<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01 Transitional//EN 'http://www.w3.org/TR/html4/loose.dtd'>";

            const string headTagOpen = @"<head>";

            const string metaTagsDeclaration = @"<meta http-equiv='Content-type' content='text/html;charset=UTF-8'>
                                   <meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1'>
                                   <title>eSign PDF</title>
                                   <meta name='description' content=''>
                                   <meta name='author' content=''>
                                   <meta name='viewport' content='width=device-width; initial-scale=1.";

            const string cssLinkDeclaration = @"<link rel='stylesheet' href='style.css?'>";

            const string inlineCssDeclaration = @"<style>
                                    html, body, div, span, object, iframe,
                                    h1, h2, h3, h4, h5, h6, p, blockquote, pre,
                                    abbr, address, cite, code,
                                    del, dfn, em, img, ins, kbd, q, samp,
                                    small, strong, sub, sup, var,
                                    b, i,
                                    dl, dt, dd, ol, ul, li,
                                    fieldset, form, label, legend,
                                    table, caption, tbody, tfoot, thead, tr, th, td,
                                    article, aside, canvas, details, figcaption, figure,
                                    footer, header, hgroup, menu, nav, section, summary,
                                    time, mark, audio, video { margin:0; padding:0; border:0; outline:0; vertical-align:baseline;
                                    background:transparent; border-collapse:collapse; list-style:none; } 
                                    body{font-family:trebuchet MS, verdana, arial; font-size: 13px;}
                                    #container {width:750px;}
                                    #header{border-bottom:3px solid #bc0001; padding:10px 0; margin-bottom:15px}
                                    h2 { color: #666666; float: left; font - weight: normal; font-size: 20px; line-height: 25px; }
                                    h3{font-size: 15px;line-height: 25px; margin-bottom:5px;}
                                    h2 span { color: #CC0000; }
                                    .clearfix:after { content: '\0020'; display: block; height: 0; clear: both; visibility: hidden; }
                                    .clearfix { zoom: 1; }
                                    .pdfPage  .logo {float:left; width:144px; height:36px;}
                                    .pdfPage  h2 {float:right; margin-top:5px;}

                                    .envelopeData li{color:#333;}
                                    .envelopeData .title{color:#666;}

                                    .tableWrap{padding-top:15px;}
                                    .tableWrap .table{width:100%;}
                                    .tableWrap td{border:1px solid #999; background: #fafafa; padding:2px 3px; font-size:12px;vertical-align: middle;border-collapse: collapse;}
                                    .tableWrap .headRow td{background: #d3d3d3; font-size:13px; color:#666;}
                                    .tableWrap .nameWidth{width: 140px;}
                                    .tableWrap .addressWidth{width: 200px;}
                                    .tableWrap .typeWidth{width: 100px;}
                                    .tableWrap .ipWidth{width: 100px;}
                                    .tableWrap .sentWidth{width: 100px;}
                                    .tableWrap .signedWidth{width: 225px;}
                                    .smallFont { font-size:12px; padding-right: 10px;}
                                    .envData { margin-top: 15px; margin-bottom: 25px;} 
                                    .envTableWrap { width: 100%; }
                                    .envTdWidth { width: 35%; }
                                    #logo{width:70px;height:44px;}
                                    .page-break-table { width: 100%; display: block;}
                                    .table-head { border-top: 1px solid #999; background: #d3d3d3; border-left: solid 1px #999;border-collapse: collapse;} 
                                    .table-body {border-left: solid 1px #999;}
                                    .table-head .table-col { font-size: 13px; color: #666;}
                                    .table-row { width: 100%; border-bottom: solid 1px #999; border-top: solid 1px #999;border-collapse: collapse;}
                                    .table-row>* { vertical-align: middle; } 
                                    .table-col { display: inline-block; border-right: solid 1px #999; padding: 2px 3px; font-size: 12px; border-collapse: collapse; vertical-align: middle; box-sizing: border-box;}
                                    @media print { .table-row {page-break-inside: avoid !important; -webkit-column-break-inside: avoid; break-inside: avoid; -webkit-region-break-inside: avoid; }}                                   

                                    .recipient-roles-table .table-body .table-col span { display: table-cell; height: 30px; vertical-align: middle; word-break: break-all;}

                                    .document-events-table .table-row div:first-child { width:20%}
                                    .document-events-table .table-row div:nth-child(2) { width:36%}
                                    .document-events-table .table-row div:nth-child(3) { width:16%}
                                    .document-events-table .table-row div:nth-child(4) { width:12%}
                                    .document-events-table .table-row div:nth-child(5) { width:16%}
                                    .document-events-table .table-body .table-col {height: 30px;}
                                    .document-events-table .table-body .table-col span { display: table-cell; height: 30px; vertical-align: middle; word-break: break-all;}
                                    .signer-signature-table .table-row div:first-child {width: 30%;}
                                    .signer-signature-table .table-row div:nth-child(2) {width: 43%;}
                                    .signer-signature-table .table-row div:nth-child(3) {width: 27%;}
                                    .signer-signature-table .table-body .table-row div {height: 120px;}
                                    .signer-signature-table img { max-width: 100%;}
                                    .signer-signature-table .table-body .table-col span { display: table-cell; height: 120px; vertical-align: middle; word-break: break-all;}";

            string styleTagClose = @"</style>";
            string cssDeclarationForNewColumn = "";

            string RSignImagePath = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["Images"]));
            RSignImagePath = System.IO.Path.Combine(RSignImagePath, "RSign.svg");
            const string headTagClose = @"</head>";
            const string bodyTagOpen = @"<body>";
            const string containerDivStart = @"<div id='container'>";
            string headerDiv = @"<div id='header' class='pdfPage clearfix'>" +
                                "<h1><img class='rsign-logo' src='" + ConvertImageToBase64String(RSignImagePath, true) + "' alt='RSign'/></h1>" +
                                "</div>";
            var currentCultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(RSign.Common.Helpers.Common.GetCultureInfoValue(envCulture));
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            string mainDivStart = @" <div id='main'><h2>" + certificateData.MetaDataAndHistory.EnvelopeData + "</h2>";
            //string timeZoneAbbrev = GetTimeZoneAbbreviation(TimeZone.CurrentTimeZone.StandardName);
            /* This list contains Envelope Data */
            string documentNameList = mainData.Documents.Aggregate(string.Empty, (current, docName) => current + (docName + ","));
            string fileReviewNameList = mainData.FileReviewDocuments != null ? mainData.FileReviewDocuments.Aggregate(string.Empty, (current, docName) => current + (docName + ",")) : string.Empty;
            string envelopeDataList = string.Empty;

            /*Add by TParker to fix data missing issue on Envelope Data with Terms of Service label for Dutch user*/
            string envTableWidthCSS = (certificateData.MetaDataAndHistory.TermsOfService.Length > 40) ? "envTableWrap" : string.Empty;
            string envTdWidthCSS = (certificateData.MetaDataAndHistory.TermsOfService.Length > 40) ? "envTdWidth" : string.Empty;
            string spanEmailAccessCode = !Convert.ToBoolean(certificateData.EnableMessageToMobile) ? "<span style='float: left;width: 160px;height: auto;'>" + certificateData.MetaDataAndHistory.EmailAccessCode + ":" + "</span><span  style='display: inherit;'>" + certificateData.EmailAccessCode + "</span>" : "";


            if (isTermsAccepted)
            {
                envelopeDataList = @"<div class='" + envTableWidthCSS + "'><table class='envelopeData " + envTableWidthCSS + "'>" +
                               "<tbody>" +
                               "<tr><td class='smallFont " + envTdWidthCSS + "'>" + certificateData.MetaDataAndHistory.Subject + ": </td><td id='subjectValue' class='smallFont'>" + mainData.Subject + " </td></tr>" +
                               "<tr><td class='smallFont " + envTdWidthCSS + "' style='vertical-align:top;'>" + certificateData.MetaDataAndHistory.Documents + ":  </td><td  id='documentsValue' class='smallFont'><span style='float: left; width: 600px; height: auto;'>" + documentNameList.Remove(documentNameList.Length - 1) + " </span></td></tr>";
                if (!string.IsNullOrEmpty(fileReviewNameList))
                {
                    envelopeDataList += "<tr><td class='smallFont " + envTdWidthCSS + "'  style='vertical-align:top;'>" + certificateData.MetaDataAndHistory.FilesReviewed + ":  </td><td  id='documentsValue' class='smallFont'><span style='float: left; width: 600px; height: auto;'>" + fileReviewNameList.Remove(fileReviewNameList.Length - 1) + " </span></td></tr>";
                }
                envelopeDataList += "<tr><td class='smallFont " + envTdWidthCSS + "'>" + certificateData.MetaDataAndHistory.DocumentHash + ":  </td><td  id='docHashValue' class='smallFont'>" + mainData.DocHash + " </td></tr>" +
                               "<tr><td class='smallFont " + envTdWidthCSS + "'>" + certificateData.MetaDataAndHistory.EnvelopeID + ":  </td><td  id='envIDValue' class='smallFont'>" + mainData.EnvelopeCode + " </td></tr>" +
                               "<tr><td class='smallFont " + envTdWidthCSS + "'>" + certificateData.MetaDataAndHistory.Sender + ":  </td><td  id='senderValue' class='smallFont'>" + mainData.Sender + " </td></tr>" +
                               "<tr><td class='smallFont " + envTdWidthCSS + "'>" + certificateData.MetaDataAndHistory.Sent + ":  </td><td  id='envSentValue' class='smallFont'>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(mainData.EnvelopeSent), uSerTimeZone, mainData.DateFormatId)) + " </td></tr>" +
                               "<tr><td class='smallFont " + envTdWidthCSS + "'>" + certificateData.MetaDataAndHistory.Status + ":  </td><td  id='envStatValue' class='smallFont'><span style='float: left;width: 270px;height: auto;'>" + EnvelopeStatus + "</span> <span style='float: left;width: 160px;height: auto;'>" + certificateData.MetaDataAndHistory.AccessAuthentication + ":" + "</span><span style='display: inherit; word-wrap: break-word; width: 165px;'>" + certificateData.AccessAuthentication + "</span></td></tr>" +
                               "<tr><td class='smallFont " + envTdWidthCSS + "'>" + certificateData.MetaDataAndHistory.StatusDate + ":  </td><td  id='statDateValue' class='smallFont'><span style='float: left;width: 270px;height: auto;'>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(mainData.EnvelopeCompletedDate), uSerTimeZone, mainData.DateFormatId)) + "</span> " + spanEmailAccessCode + "</td></tr>" +
                               "<tr><td class='smallFont " + envTdWidthCSS + "'>" + certificateData.MetaDataAndHistory.TermsOfService + ": </td><td id='termsOfServiceAccepted' class='smallFont'><span style='float: left;width: 270px;height: auto;'>" + certificateData.MetaDataAndHistory.Accepted + "</span> <span style='float: left;width: 160px;height: auto;'>" + certificateData.MetaDataAndHistory.EmailVerification + ":" + "</span><span style='display: inherit;'>" + certificateData.EmailVerification + "</span></td></tr>" +
                               "</tbody>" +
                               "</table></div>";
            }
            else
            {
                envelopeDataList = @"<div><table class='envelopeData'>" +
                               "<tbody>" +
                               "<tr><td class='smallFont'>" + certificateData.MetaDataAndHistory.Subject + ": </td><td id='subjectValue' class='smallFont'>" + mainData.Subject + " </td></tr>" +
                               "<tr><td class='smallFont' style='vertical-align:top;'>" + certificateData.MetaDataAndHistory.Documents + ":  </td><td  id='documentsValue' class='smallFont'><span style='float: left; width: 600px; height: auto;'>" + documentNameList.Remove(documentNameList.Length - 1) + " </span></td></tr>";
                if (!string.IsNullOrEmpty(fileReviewNameList))
                {
                    envelopeDataList += "<tr><td class='smallFont'  style='vertical-align:top;'>" + certificateData.MetaDataAndHistory.FilesReviewed + ":  </td><td  id='documentsValue' class='smallFont'><span style='float: left; width: 600px; height: auto;'>" + fileReviewNameList.Remove(fileReviewNameList.Length - 1) + " </span></td></tr>";
                }
                envelopeDataList += "<tr><td class='smallFont'>" + certificateData.MetaDataAndHistory.DocumentHash + ":  </td><td  id='docHashValue' class='smallFont'>" + mainData.DocHash + " </td></tr>" +
                               "<tr><td class='smallFont'>" + certificateData.MetaDataAndHistory.EnvelopeID + ":  </td><td  id='envIDValue' class='smallFont'>" + mainData.EnvelopeCode + " </td></tr>" +
                               "<tr><td class='smallFont'>" + certificateData.MetaDataAndHistory.Sender + ":  </td><td  id='senderValue' class='smallFont'>" + mainData.Sender + " </td></tr>" +
                               "<tr><td class='smallFont'>" + certificateData.MetaDataAndHistory.Sent + ":  </td><td  id='envSentValue' class='smallFont'>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(mainData.EnvelopeSent), uSerTimeZone, mainData.DateFormatId)) + " </td></tr>" +
                               "<tr><td class='smallFont'>" + certificateData.MetaDataAndHistory.Status + ":  </td><td  id='envStatValue' class='smallFont'><span style='float: left;width: 270px;height: auto;'>" + EnvelopeStatus + "</span> <span style='float: left;width: 160px;height: auto;'>" + certificateData.MetaDataAndHistory.AccessAuthentication + ":" + "</span><span  style='display: inherit; word-wrap: break-word; width: 165px;'>" + certificateData.AccessAuthentication + "</span></td></tr>" +
                               "<tr><td class='smallFont'>" + certificateData.MetaDataAndHistory.StatusDate + ":  </td><td  id='statDateValue' class='smallFont'><span style='float: left;width: 270px;height: auto;'>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(mainData.EnvelopeCompletedDate), uSerTimeZone, mainData.DateFormatId)) + "</span> " + spanEmailAccessCode + "</td></tr>" +
                                "<tr><td class='smallFont " + envTdWidthCSS + "'>&nbsp; </td><td id='termsOfServiceAccepted' class='smallFont'><span style='float: left;width: 270px;height: auto;'>&nbsp;</span> <span style='float: left;width: 160px;height: auto;'>" + certificateData.MetaDataAndHistory.EmailVerification + ":" + "</span><span style='display: inherit;'>" + certificateData.EmailVerification + "</span></td></tr>" +
                               "</tbody>" +
                               "</table></div>";
            }
            /* This string builds Recipients Table Div*/
            StringBuilder recipientTableBuilder = new StringBuilder();
            if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
            {
                cssDeclarationForNewColumn = @"
                                    .recipient-roles-table .table-row div:first-child { width:25%}
                                    .recipient-roles-table .table-row div:nth-child(2) { width:38%}
                                    .recipient-roles-table .table-row div:nth-child(3) { width:18%}
                                    .recipient-roles-table .table-row div:nth-child(4) { width:19%}                                    
                                ";
                recipientTableBuilder = new StringBuilder(@"<div class='tableWrap recipient-roles-table'>" +
                                                                    "<h3>" + certificateData.MetaDataAndHistory.Recipients + " / " + certificateData.MetaDataAndHistory.Roles + "</h3>" +
                                                                    "<div class='page-break-table'>" +
                                                                    "<div class='table-head'>" +
                                                                    "<div class='headRow table-row'>" +
                                                                    "<div class='nameWidth table-col'>" + certificateData.MetaDataAndHistory.Name + " / " + certificateData.MetaDataAndHistory.Role + "</div>" +
                                                                    "<div class='addressWidth table-col'>" + certificateData.MetaDataAndHistory.EmailOrMobile + "</div>" +
                                                                    "<div class='typeWidth table-col'>" + certificateData.MetaDataAndHistory.DeliveryMode + "</div>" +
                                                                    "<div class='typeWidth table-col'>" + certificateData.MetaDataAndHistory.Type + "</div>" +
                                                                    "</div></div>");
            }
            else
            {
                cssDeclarationForNewColumn = @".recipient-roles-table .table-row div:first-child { width:25%}
                                    .recipient-roles-table .table-row div:nth-child(2) { width:45%}
                                    .recipient-roles-table .table-row div:nth-child(3) { width:30%}";


                recipientTableBuilder = new StringBuilder(@"<div class='tableWrap recipient-roles-table'>" +
                                                                    "<h3>" + certificateData.MetaDataAndHistory.Recipients + " / " + certificateData.MetaDataAndHistory.Roles + "</h3>" +
                                                                    "<div class='page-break-table'>" +
                                                                    "<div class='table-head'>" +
                                                                    "<div class='headRow table-row'>" +
                                                                    "<div class='nameWidth table-col'>" + certificateData.MetaDataAndHistory.Name + " / " + certificateData.MetaDataAndHistory.Role + "</div>" +
                                                                    "<div class='addressWidth table-col'>" + certificateData.MetaDataAndHistory.Email + "</div>" +
                                                                    "<div class='typeWidth table-col'>" + certificateData.MetaDataAndHistory.Type + "</div>" +
                                                                    "</div></div>");
            }

            foreach (var recipient in mainData.Recipients.Where(r => r.IsSameRecipient != true).OrderBy(r => r.DisplayOrder))
            {
                string recipientType = string.Empty;
                string emailMobileAddress = string.Empty;
                string mobileNumber = string.Empty;
                string deliveryMode = Constants.MessageDeliveryModes.EmailSlashMobile;
                if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
                {
                    deliveryMode = GetRecipientDeliveryMode(recipient.DeliveryMode);
                }

                if (recipient.RecipientType == "Sender")
                {
                    recipientType = certificateData.MetaDataAndHistory.Sender;
                    deliveryMode = Constants.MessageDeliveryModes.PrefillNA;
                    emailMobileAddress = recipient.EmailAddress;
                }
                else if (recipient.RecipientType == "Prefill")   // V2 Team Prefill Change
                {
                    recipientType = certificateData.MetaDataAndHistory.Prefill;
                    deliveryMode = Constants.MessageDeliveryModes.PrefillNA;
                    emailMobileAddress = recipient.EmailAddress;
                }
                else if (recipient.RecipientType == "Signer")
                {
                    recipientType = certificateData.MetaDataAndHistory.Signer;
                    if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
                    {
                        emailMobileAddress = AppendSignerEmailMobileDetails(recipient.DeliveryMode, recipient.EmailAddress, recipient.DialCode, recipient.MobileNumber);

                        //if (recipient.DeliveryMode == 1 || recipient.DeliveryMode == null)
                        //{
                        //    emailMobileAddress = recipient.EmailAddress;
                        //}
                        //else if (recipient.DeliveryMode == 2)
                        //{
                        //    if (!string.IsNullOrEmpty(recipient.EmailAddress) && !string.IsNullOrEmpty(recipient.MobileNumber))
                        //    {
                        //        emailMobileAddress = recipient.EmailAddress + ", " + recipient.DialCode + recipient.MobileNumber;
                        //    }
                        //    else if (!string.IsNullOrEmpty(recipient.EmailAddress))
                        //    {
                        //        emailMobileAddress = recipient.EmailAddress;
                        //    }
                        //    else if (!string.IsNullOrEmpty(recipient.MobileNumber))
                        //    {
                        //        emailMobileAddress = recipient.DialCode + recipient.MobileNumber;
                        //    }
                        //}
                        //else if (recipient.DeliveryMode == 3)
                        //{
                        //    emailMobileAddress = recipient.DialCode + recipient.MobileNumber;
                        //}
                        //else emailMobileAddress = recipient.EmailAddress;
                    }
                    else
                        emailMobileAddress = recipient.EmailAddress;
                }
                else
                {
                    if (mainData.AsposeDropdownOptions != null && mainData.AsposeDropdownOptions.Count > 0 && recipient.CcSignerType != null)
                    {
                        string CcSignerType = (mainData.AsposeDropdownOptions.Where(d => d.OptionValue == Convert.ToString(recipient.CcSignerType)).FirstOrDefault()).OptionName;
                        recipientType = "Cc - " + CcSignerType;
                    }
                    else
                    {
                        recipientType = "Cc";
                    }
                    if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
                    {
                        emailMobileAddress = AppendSignerEmailMobileDetails(recipient.DeliveryMode, recipient.EmailAddress, recipient.DialCode, recipient.MobileNumber);

                        //if (recipient.DeliveryMode == 1)
                        //{
                        //    emailMobileAddress = recipient.EmailAddress;
                        //}
                        //else if (recipient.DeliveryMode == 2)
                        //{
                        //    if (!string.IsNullOrEmpty(recipient.EmailAddress) && !string.IsNullOrEmpty(recipient.MobileNumber))
                        //    {
                        //        emailMobileAddress = recipient.EmailAddress + ", " + recipient.DialCode + recipient.MobileNumber;
                        //    }
                        //    else if (!string.IsNullOrEmpty(recipient.EmailAddress))
                        //    {
                        //        emailMobileAddress = recipient.EmailAddress;
                        //    }
                        //    else if (!string.IsNullOrEmpty(recipient.MobileNumber))
                        //    {
                        //        emailMobileAddress = recipient.DialCode + recipient.MobileNumber;
                        //    }
                        //}
                        //else if (recipient.DeliveryMode == 3)
                        //{
                        //    emailMobileAddress = recipient.DialCode + recipient.MobileNumber;
                        //}
                        //else emailMobileAddress = recipient.EmailAddress;
                    }
                    else
                        emailMobileAddress = recipient.EmailAddress;
                }

                if (string.IsNullOrEmpty(recipient.Name))
                {
                    if (!string.IsNullOrEmpty(recipient.EmailAddress))
                        recipient.Name = recipient.EmailAddress;
                    else if (!string.IsNullOrEmpty(recipient.MobileNumber))
                        recipient.Name = recipient.DialCode + recipient.MobileNumber;
                }

                if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
                {
                    recipientTableBuilder.Append("<div class='table-body'>" +
                                                                   "<div class='table-row'>" +
                                                                "<div class='table-col'> <span>" + recipient.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                                                "<div class='table-col'><span>" + emailMobileAddress + "</span></div>" +
                                                                "<div class='table-col'><span>" + deliveryMode + "</span></div>" +
                                                                "<div class='table-col'><span>" + recipientType + "</span></div>" +
                                                                "</div></div>");
                }
                else
                {
                    recipientTableBuilder.Append("<div class='table-body'>" +
                                                                   "<div class='table-row'>" +
                                                                "<div class='table-col'> <span>" + recipient.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                                                "<div class='table-col'><span>" + recipient.EmailAddress + "</span></div>" +
                                                                "<div class='table-col'><span>" + recipientType + "</span></div>" +
                                                                "</div></div>");
                }

            }
            recipientTableBuilder.Append("</div>");

            /* This string builds Signer Events Table Div */
            var signerEventsTableBuilder = new StringBuilder();
            string emailMobileColumn = "";
            if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
            {
                emailMobileColumn = certificateData.MetaDataAndHistory.EmailOrMobile;
            }
            else
            {
                emailMobileColumn = certificateData.MetaDataAndHistory.Email;
            }

            signerEventsTableBuilder = new StringBuilder(@"</div>" +
                                                          "<div class='tableWrap document-events-table'>" +
                                                          "<h3>" + certificateData.MetaDataAndHistory.DocumentEvents + "</h3>" +
                                                          "<div class='page-break-table'>" +
                                                          "<div class='table-head'>" +
                                                          "<div class='headRow table-row'>" +
                                                          "<div class='nameWidth table-col'>" + certificateData.MetaDataAndHistory.Name + " / " + certificateData.MetaDataAndHistory.Roles + "</div>" +
                                                          "<div class='addressWidth table-col'>" + emailMobileColumn + "</div>" +
                                                          "<div class='ipWidth table-col'>" + certificateData.MetaDataAndHistory.IPAddress + "</div>" +
                                                          "<div class='sentWidth table-col'>" + certificateData.MetaDataAndHistory.Date + "</div>" +
                                                          "<div class='signedWidth table-col'>" + certificateData.MetaDataAndHistory.Event + "</div>" +
                                                          "</div></div>");


            var senderRecipients = mainData.Recipients.FirstOrDefault(rc => rc.RecipientType == "Sender");
            string senderIPAddress = string.IsNullOrEmpty(senderRecipients.SignerIPAddress) ? " - " : senderRecipients.SignerIPAddress;
            signerEventsTableBuilder.Append("<div class='table-body'><div class='table-row'>" +
                                                "<div class='table-col'> <span>" + senderRecipients.Name + "</span></div>" +
                                                "<div class='table-col'><span>" + senderRecipients.EmailAddress + "</span></div>" +
                                                "<div class='table-col'><span>" + senderIPAddress + "</span></div>" +
                                                "<div class='table-col'><span>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(mainData.EnvelopeSent), uSerTimeZone, mainData.DateFormatId)) + "</span></div>" +
                                                "<div class='table-col'><span>" + certificateData.MetaDataAndHistory.Created + "</span></div>" +
                                                "</div>");
            foreach (var item in mainData.SenderUpdateHistoryDetails.OrderBy(r => r.CreatedDateTime))
            {

                signerEventsTableBuilder.Append("<div class='table-row'>" +
                                            "<div class='table-col'><span>" + item.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                            "<div class='table-col'><span>" + item.Email + "</span></div>" +
                                            "<div class='table-col'><span>" + senderIPAddress + "</span></div>" +
                                            "<div class='table-col'><span>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(item.CreatedDateTime), uSerTimeZone, mainData.DateFormatId)) + "</span></div>" +
                                            "<div class='table-col'><span>" + certificateData.MetaDataAndHistory.UpdateAndResend + "</span></div>" +
                                            "</div>");
            }
            // V2 Team Prefill Change
            //var prefillRecipients = mainData.Recipients.FirstOrDefault(rc => rc.RecipientType == "Prefill");
            //if (prefillRecipients != null)
            //{
            //    signerEventsTableBuilder.Append("<tr>" +
            //                                    "<td>" + prefillRecipients.Name.Replace(PrefillNotation, "") + "</td>" +
            //                                    "<td>" + prefillRecipients.EmailAddress + "</td>" +
            //                                    "<td>" + prefillRecipients.SignerIPAddress + "</td>" +
            //                                    "<td>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(prefillRecipients.StatusDate), uSerTimeZone, mainData.DateFormatId)) + "</td>" +
            //                                    "<td>" + GetStatus(prefillRecipients.Status) + "</td>" +
            //                                    "</tr>");
            //}
            bool isAtleastOneSignerSigned = mainData.Recipients.Any(r => r.StatusID == new Guid("4F564EA5-009C-4F52-A3DE-C6D0AC598617"));
            // V2 Team Prefill Change
            var signerRecipients = mainData.Recipients.Where(rc => rc.RecipientType == "Signer");
            //List<AsposeRecipientDetails> recipientHistory = new List<AsposeRecipientDetails>();
            //foreach (var recipient in signerRecipients.OrderBy(r => r.CreatedDateTime))
            //{
            //    recipientHistory.AddRange(recipient.RecipientDetails);
            //}

            foreach (var item in mainData.AllRecipientsHistoryDetails.OrderBy(r => r.CreatedDateTime))
            {
                string emailMobileAddress = "";
                if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
                {
                    emailMobileAddress = AppendSignerEmailMobileDetails(item.DeliveryMode, item.Email, item.DialCode, item.MobileNumber);

                    //if (item.DeliveryMode == 1)
                    //{
                    //    emailMobileAddress = item.Email;
                    //}
                    //else if (item.DeliveryMode == 2)
                    //{
                    //    if (!string.IsNullOrEmpty(item.Email) && !string.IsNullOrEmpty(item.MobileNumber))
                    //    {
                    //        emailMobileAddress = item.Email + ", " + item.DialCode + item.MobileNumber;
                    //    }
                    //    else if (!string.IsNullOrEmpty(item.Email))
                    //    {
                    //        emailMobileAddress = item.Email;
                    //    }
                    //    else if (!string.IsNullOrEmpty(item.MobileNumber))
                    //    {
                    //        emailMobileAddress = item.DialCode + item.MobileNumber;
                    //    }
                    //}
                    //else if (item.DeliveryMode == 3)
                    //{
                    //    emailMobileAddress = item.DialCode + item.MobileNumber;
                    //}
                    //else
                    //    emailMobileAddress = item.Email;
                }
                else
                    emailMobileAddress = item.Email;

                if (string.IsNullOrEmpty(item.Name))
                {
                    if (!string.IsNullOrEmpty(item.Email))
                        item.Name = item.Email;
                    else if (!string.IsNullOrEmpty(item.MobileNumber))
                        item.Name = item.DialCode + item.MobileNumber;
                }

                signerEventsTableBuilder.Append("<div class='table-row'>" +
                                            "<div class='table-col'><span>" + item.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                            "<div class='table-col'><span>" + emailMobileAddress + "</span></div>" +
                                            "<div class='table-col'><span>" + item.IPAddress + "</span></div>" +
                                            "<div class='table-col'><span>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(item.CreatedDateTime), uSerTimeZone, mainData.DateFormatId)) + "</span></div>" +
                                            "<div class='table-col'><span>" + item.Status + "</span></div>" +
                                            "</div>");
            }
            //foreach (var recipient in signerRecipients.Where(s => s.Status == "Rejected").OrderBy(r => r.CreatedDateTime))
            //{
            //    signerEventsTableBuilder.Append("<tr>" +
            //                                   "<td>" + recipient.Name.Replace(PrefillNotation, "") + "</td>" +
            //                                   "<td>" + recipient.EmailAddress + "</td>" +
            //                                   "<td>" + recipient.SignerIPAddress + "</td>" +
            //                                   "<td>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(recipient.CreatedDateTime), uSerTimeZone, mainData.DateFormatId)) + "</td>" +
            //                                   "<td>" + GetStatus(recipient.Status) + "</td>" +
            //                                   "</tr>");

            //}
            //foreach (var recipient in signerRecipients.OrderBy(r => r.CreatedDateTime))
            //{
            //    foreach (var item in recipient.RecipientDetails.OrderBy(r => r.CreatedDateTime))
            //    {
            //        signerEventsTableBuilder.Append("<tr>" +
            //                                    "<td>" + item.Name.Replace(PrefillNotation, "") + "</td>" +
            //                                    "<td>" + item.Email + "</td>" +
            //                                    "<td>" + item.IPAddress + "</td>" +
            //                                    "<td>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(item.CreatedDateTime), uSerTimeZone, mainData.DateFormatId)) + "</td>" +
            //                                    "<td>" + GetStatus(item.Status) + "</td>" +
            //                                    "</tr>");
            //    }
            //    if (recipient.Status == "Rejected")
            //    {
            //        signerEventsTableBuilder.Append("<tr>" +
            //                                       "<td>" + recipient.Name.Replace(PrefillNotation, "") + "</td>" +
            //                                       "<td>" + recipient.EmailAddress + "</td>" +
            //                                       "<td>" + recipient.SignerIPAddress + "</td>" +
            //                                       "<td>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(recipient.CreatedDateTime), uSerTimeZone, mainData.DateFormatId)) + "</td>" +
            //                                       "<td>" + GetStatus(recipient.Status) + "</td>" +
            //                                       "</tr>");

            //    }
            //}
            if (mainData.RecipientsWithWaitingForSignatureStatus != null && mainData.RecipientsWithWaitingForSignatureStatus.Count > 0)
            {
                foreach (var recWfs in mainData.RecipientsWithWaitingForSignatureStatus)
                {
                    string emailMobileAddress = "";
                    if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
                    {
                        emailMobileAddress = AppendSignerEmailMobileDetails(recWfs.DeliveryMode, recWfs.Email, recWfs.DialCode, recWfs.MobileNumber);

                        //if (recWfs.DeliveryMode == 1)
                        //{
                        //    emailMobileAddress = recWfs.Email;
                        //}
                        //else if (recWfs.DeliveryMode == 2)
                        //{
                        //    if (!string.IsNullOrEmpty(recWfs.Email) && !string.IsNullOrEmpty(recWfs.MobileNumber))
                        //    {
                        //        emailMobileAddress = recWfs.Email + ", " + recWfs.DialCode + recWfs.MobileNumber;
                        //    }
                        //    else if (!string.IsNullOrEmpty(recWfs.Email))
                        //    {
                        //        emailMobileAddress = recWfs.Email;
                        //    }
                        //    else if (!string.IsNullOrEmpty(recWfs.MobileNumber))
                        //    {
                        //        emailMobileAddress = recWfs.DialCode + recWfs.MobileNumber;
                        //    }
                        //}
                        //else if (recWfs.DeliveryMode == 3)
                        //{
                        //    emailMobileAddress = recWfs.DialCode + recWfs.MobileNumber;
                        //}
                        //else
                        //    emailMobileAddress = recWfs.Email;
                    }
                    else
                        emailMobileAddress = recWfs.Email;

                    signerEventsTableBuilder.Append("<div class='table-row'>" +
                                                "<div class='table-col'><span>" + recWfs.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                                "<div class='table-col'><span>" + emailMobileAddress + "</span></div>" +
                                                "<div class='table-col'><span>" + recWfs.IPAddress + "</span></div>" +
                                                "<div class='table-col'><span> - </span></div>" +
                                                "<div class='table-col'><span>" + recWfs.StatusDescription + "</span></div>" +
                                                "</div>");
                }
            }
            signerEventsTableBuilder.Append("<div class='table-row'>" +
                                            "<div class='table-col'><span>" + string.Empty + "</span></div>" +
                                            "<div class='table-col'><span>" + string.Empty + "</span></div>" +
                                            "<div class='table-col'><span>" + string.Empty + "</span></div>" +
                                            "<div class='table-col'><span>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(mainData.EnvelopeCompletedDate), uSerTimeZone, mainData.DateFormatId)) + "</span></div>" +
                                            "<div class='table-col'><span><b>" + certificateData.MetaDataAndHistory.Envelope + " " + certificateData.MetaDataAndHistory.Status + " - " + EnvelopeStatus + "</b></span></div>" +
                                            "</div>");
            signerEventsTableBuilder.Append("</div></div></div>");



            /* This string builds Carbon Copy Events Table Div */
            StringBuilder ccEventsTableBuilder = new StringBuilder();
            if (mainData.Recipients.Count(rc => rc.RecipientType == "CC") > 0)
            {
                string ccEmailMobileColumn = "";
                if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
                {
                    ccEmailMobileColumn = certificateData.MetaDataAndHistory.EmailOrMobile;
                }
                else
                {
                    ccEmailMobileColumn = certificateData.MetaDataAndHistory.Email;
                }

                ccEventsTableBuilder = new StringBuilder(@"<div class='tableWrap'>" +
                                                         "<h3>" + certificateData.MetaDataAndHistory.CarbonCopyEvents + "</h3>" +
                                                         "<table>" +
                                                         "<tbody>" +
                                                         "<tr class='headRow'>" +
                                                         "<td class='nameWidth'>" + certificateData.MetaDataAndHistory.Name + " / " + certificateData.MetaDataAndHistory.Roles + "</td>" +
                                                         "<td class='addressWidth'>" + ccEmailMobileColumn + "</td>" +
                                                         "<td class='sentWidth'>" + certificateData.MetaDataAndHistory.Sent + "</td>" +
                                                         "</tr>");
                var carbonCopyRecipients = mainData.Recipients.Where(rc => rc.RecipientType == "CC");
                string emailMobileAddress = "";
                foreach (var recipient in carbonCopyRecipients)
                {
                    if (Convert.ToBoolean(certificateData.EnableMessageToMobile))
                    {
                        emailMobileAddress = AppendSignerEmailMobileDetails(recipient.DeliveryMode, recipient.EmailAddress, recipient.DialCode, recipient.MobileNumber);

                        //if (recipient.DeliveryMode == 1)
                        //{
                        //    emailMobileAddress = recipient.EmailAddress;
                        //}
                        //else if (recipient.DeliveryMode == 2)
                        //{
                        //    if (!string.IsNullOrEmpty(recipient.EmailAddress) && !string.IsNullOrEmpty(recipient.MobileNumber))
                        //    {
                        //        emailMobileAddress = recipient.EmailAddress + ", " + recipient.DialCode + recipient.MobileNumber;
                        //    }
                        //    else if (!string.IsNullOrEmpty(recipient.EmailAddress))
                        //    {
                        //        emailMobileAddress = recipient.EmailAddress;
                        //    }
                        //    else if (!string.IsNullOrEmpty(recipient.MobileNumber))
                        //    {
                        //        emailMobileAddress = recipient.DialCode + recipient.MobileNumber;
                        //    }
                        //}
                        //else if (recipient.DeliveryMode == 3)
                        //{
                        //    emailMobileAddress = recipient.DialCode + recipient.MobileNumber;
                        //}
                        //else emailMobileAddress = recipient.EmailAddress;
                    }
                    else
                        emailMobileAddress = recipient.EmailAddress;

                    ccEventsTableBuilder.Append("<tr>" +
                                               "<td>" + recipient.Name.Replace(PrefillNotation, "") + "</td>" +
                                                "<td>" + emailMobileAddress + "</td>" +
                                                "<td>" + string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(mainData.EnvelopeCompletedDate), uSerTimeZone, mainData.DateFormatId)) + "</td>" +
                                                "</tr>");
                }
                ccEventsTableBuilder.Append("</tbody></table></div>");
            }

            /* This string builds Signer Signatures in Table Div */
            StringBuilder signerSignatureBuilder = new StringBuilder();
            if (isAtleastOneSignerSigned)
            {
                signerSignatureBuilder = new StringBuilder(@"<div class='tableWrap signer-signature-table'>" +
                                                            "<h3>" + certificateData.MetaDataAndHistory.SignerSignatures + "</h3>" +
                                                            "<div class='page-break-table'>" +
                                                            "<div class='table-head'>" +
                                                            "<div class='headRow table-row'>" +
                                                            "<div class='nameWidth table-col'>" + certificateData.MetaDataAndHistory.SignerName + " / " + certificateData.MetaDataAndHistory.Roles + "</div>" +
                                                            "<div class='signWidth table-col'>" + certificateData.MetaDataAndHistory.Signature + "</div>" +
                                                            "<div class='signWidth table-col'>" + certificateData.MetaDataAndHistory.Initial + "</div>" +
                                                            "</div></div><div class='table-body'>");
                foreach (var recipient in mainData.Recipients.OrderBy(r => r.CreatedDateTime))  // V2 Team Prefill Change
                {
                    if (recipient.RecipientType == "Sender" || recipient.Status != "Signed") continue;
                    if (recipient.SignatureBytes != null)
                    {
                        string imageBase64 = Convert.ToBase64String(recipient.SignatureBytes);
                        System.Drawing.Image img = Base64ToImage(imageBase64);
                        string imageSrc = string.Format("data:image/gif;base64,{0}", imageBase64);
                        if (recipient.InitialBytes != null && recipient.InitialBytes.Length > 0) //If length is zero giving exception at Base64ToImage method
                        {
                            string initImageBase64 = Convert.ToBase64String(recipient.InitialBytes);
                            System.Drawing.Image initImg = Base64ToImage(initImageBase64);
                            string initImageSrc = string.Format("data:image/gif;base64,{0}", initImageBase64);
                            signerSignatureBuilder.Append("<div class='table-row'>" +
                                                       "<div class='table-col'><span>" + recipient.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                                      "<div class='table-col'><img height= '" + String.Format("{0}{1}", img.Height, "px") + "' max-height='70px' width='" + String.Format("{0}{1}", img.Width, "px") + "' max-width='250px' class='logo' src='" + imageSrc + "' alt='RSign'/></div>" +
                                                      "<div class='table-col'><img height= '" + String.Format("{0}{1}", initImg.Height, "px") + "' max-height='70px' width='" + String.Format("{0}{1}", initImg.Width, "px") + "' max-width='200px' class='logo' src='" + initImageSrc + "' alt='RSign'/></div>" +
                                                      "</div>");
                        }
                        else
                        {
                            signerSignatureBuilder.Append("<div class='table-row'>" +
                                                       "<div class='table-col'><span>" + recipient.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                                      "<div class='table-col'><img height= '" + String.Format("{0}{1}", img.Height, "px") + "' max-height='70px' width='" + String.Format("{0}{1}", img.Width, "px") + "' max-width='250px' class='logo' src='" + imageSrc + "' alt='RSign'/></div>" +
                                                      "<div class='table-col'></div>" +
                                                      "</div>");
                        }

                    }
                    else
                    {
                        if (recipient.InitialBytes != null && recipient.InitialBytes.Length > 0)
                        {
                            string initImageBase64 = Convert.ToBase64String(recipient.InitialBytes);
                            System.Drawing.Image initImg = Base64ToImage(initImageBase64);
                            string initImageSrc = string.Format("data:image/gif;base64,{0}", initImageBase64);
                            signerSignatureBuilder.Append("<div class='table-row'>" +
                                                       "<div class='table-col'><span>" + recipient.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                                      "<div class='table-col'></div>" +
                                                      "<div class='table-col'><img height= '" + String.Format("{0}{1}", initImg.Height, "px") + "' max-height='70px' width='" + String.Format("{0}{1}", initImg.Width, "px") + "' max-width='200px' class='logo' src='" + initImageSrc + "' alt='RSign'/></div>" +
                                                      "</div>");
                        }
                        else
                        {
                            signerSignatureBuilder.Append("<div class='table-row'>" +
                                                      "<div class='table-col'><span>" + recipient.Name.Replace(PrefillNotation, "") + "</span></div>" +
                                                     "<div class='table-col'></div>" +
                                                     "<div class='table-col'></div>" +
                                                     "</div>");
                        }
                    }
                }
                signerSignatureBuilder.Append("</div></div></div>");
            }

            StringBuilder signerDisclemerBuilder = new StringBuilder();
            if (IsDisclaimerInCertificate == true && !string.IsNullOrEmpty(strDisclemer))
            {
                signerDisclemerBuilder = new StringBuilder(
                                                        "<div class='tableWrap' style='padding-top:50px;'>" +
                                                        "<h3>" + certificateData.MetaDataAndHistory.TermsOfService + "</h3>" +
                                                        strDisclemer +
                                                           "</div>");
            }


            //const string internalDivEnd = @"</div>";
            const string mainDivEnd = @"</div>";
            const string containerDivEnd = @" </div><!--! end of #container -->";
            const string bodyTagClose = @"</body>";
            const string htmlTagClose = @"</html>";

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(currentCultureInfo);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;


            /* Now Build complete html using above string*/

            // HtmlDoctype
            htmlBuilder.Append(htmlDocTypeDeclaration);
            // Head
            htmlBuilder.Append(headTagOpen);
            // Meta Tags
            htmlBuilder.Append(metaTagsDeclaration);
            // css Link
            htmlBuilder.Append(cssLinkDeclaration);
            // Inline css
            htmlBuilder.Append(inlineCssDeclaration);
            // Additional css for new column            
            htmlBuilder.Append(cssDeclarationForNewColumn);
            //Close Style Tag
            htmlBuilder.Append(styleTagClose);
            // Close Head tag
            htmlBuilder.Append(headTagClose);



            // open Body tag
            htmlBuilder.Append(bodyTagOpen);
            // open Container Div 
            htmlBuilder.Append(containerDivStart);
            // Header Logo section
            htmlBuilder.Append(headerDiv);
            // open Main div
            htmlBuilder.Append(mainDivStart);
            // Add Envelope Data
            htmlBuilder.Append(envelopeDataList);


            //  Recipients Table Div
            htmlBuilder.Append(recipientTableBuilder);
            // Signer Events Table Div
            htmlBuilder.Append(signerEventsTableBuilder);
            // Carbon Copy Events Table Div
            if (ccEventsTableBuilder.Length > 0)
                htmlBuilder.Append(ccEventsTableBuilder);
            // Signer Signature Table Div
            if (isAtleastOneSignerSigned)
                htmlBuilder.Append(signerSignatureBuilder);

            // Signer Signer Disclemer Builder
            if (IsDisclaimerInCertificate == true && !string.IsNullOrEmpty(strDisclemer))
                htmlBuilder.AppendLine();
            htmlBuilder.Append(signerDisclemerBuilder);

            //  Close Main Div
            //htmlBuilder.Append(internalDivEnd);
            //  Close Main Div
            htmlBuilder.Append(mainDivEnd);
            // close ContainerDiv
            htmlBuilder.Append(containerDivEnd);
            // Close body tag
            htmlBuilder.Append(bodyTagClose);
            // Close HTML tag
            htmlBuilder.Append(htmlTagClose);

            string htmlFileSavePath = directoryPath + "render.html";

            return htmlBuilder.ToString();
        }
        public static string ConvertImageToBase64String(string filePath, bool ReturnImgSource)
        {
            string imgBase64 = string.Empty;
            if (System.IO.File.Exists(filePath))
            {
                byte[] imgBytes = File.ReadAllBytes(filePath);
                imgBase64 = Convert.ToBase64String(imgBytes);
                if (ReturnImgSource)
                {
                    imgBase64 = "data:image/png;base64," + imgBase64;
                }
            }
            return imgBase64;
        }
        public System.Drawing.Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                return image;
            }
        }
        public string ConvertHtmlFileToPdf(string htmlFilepath, string imgFolderDirectory, string pdfSavePath, string directorypath, int PaperSizeId)
        {
            #region old code
            /*
                iText7Helper iTextHelper = new iText7Helper();
                return iTextHelper.ConvertHTMLToPDFiText7(htmlFilepath, pdfSavePath);
            */
            // Instantiate an object PDF class
            /*var pdf = new Pdf();
            // add the section to PDF document sections collection
            var section = pdf.Sections.Add();
            //Create text paragraphs containing HTML text
            var text2 = new Text(section, htmlFilepath);
            // enable the property to display HTML contents within their own formatting
            text2.IsHtml5Supported = true;
            text2.IsFitToPage = true;
            text2.IsHtmlTagSupported = true;
            //Add the text paragraphs containing HTML text to the section
            section.PageInfo.Margin = new Aspose.Pdf.Generator.MarginInfo { Left = 15 };
            if (PaperSizeId == 2)
            {
                section.PageInfo.PageWidth = Aspose.Pdf.Generator.PageSize.LetterWidth;
                section.PageInfo.PageHeight = Aspose.Pdf.Generator.PageSize.LetterHeight;
            }
            else if (PaperSizeId == 3)
            {
                section.PageInfo.PageWidth = Aspose.Pdf.Generator.PageSize.LegalWidth;
                section.PageInfo.PageHeight = Aspose.Pdf.Generator.PageSize.LegalHeight;
            }
            else if (PaperSizeId == 4)
            {
                section.PageInfo.PageWidth = Aspose.Pdf.Generator.PageSize.A4Width;
                section.PageInfo.PageHeight = Aspose.Pdf.Generator.PageSize.A4Height;
            }
            else
            {
                string Filepath = Path.Combine(directorypath, "EnvOutput.pdf");
                Document pdfDocument = new Document(Filepath);
                PdfPageEditor pEdit = new PdfPageEditor(pdfDocument);
                Aspose.Pdf.PageSize pSize = pEdit.GetPageSize(1);
                section.PageInfo.PageWidth = pSize.Width;
                section.PageInfo.PageHeight = pSize.Height;
            }
            section.Paragraphs.Add(text2);
            section.IsNewColumn = true;
            //section.IsLandscape = true;  // Enable landscape mode
            // Specify the URL which serves as images database
            pdf.HtmlInfo.ImgUrl = imgFolderDirectory; // eg. = "D:/PDF/img/"
                                                      //Save the pdf document

            /*Add by TParker to fix missing multilingual characters on Envelope Data*/
            /*pdf.SetUnicode();

            pdf.Save(pdfSavePath); // eg. = "D:\\HTML2pdf.pdf"
            return pdfSavePath; */
            #endregion

            /* Latest Version 20.x Implementation Starts here */
            /* DOM Method*/
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "ConvertHtmlFileToPdf", "Process is started for Convert Html File To Pdf method ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            Aspose.Pdf.HtmlLoadOptions htmloptions = new Aspose.Pdf.HtmlLoadOptions();
            htmloptions.PageInfo.Margin.Left = 15;
            htmloptions.PageInfo.Margin.Top = 15;
            htmloptions.PageInfo.Margin.Bottom = 15;
            htmloptions.PageInfo.Margin.Right = 5;
            if (PaperSizeId == 2)
            {
                htmloptions.PageInfo.Width = PageSize.PageLetter.Width;
                htmloptions.PageInfo.Height = PageSize.PageLetter.Height;
            }
            else if (PaperSizeId == 3)
            {
                htmloptions.PageInfo.Width = PageSize.PageLegal.Width;
                htmloptions.PageInfo.Height = PageSize.PageLegal.Height;
            }
            else if (PaperSizeId == 4)
            {
                htmloptions.PageInfo.Width = PageSize.A4.Width;
                htmloptions.PageInfo.Height = PageSize.A4.Height;
            }
            else
            {
                string Filepath = Path.Combine(directorypath, "EnvOutput.pdf");
                Document pdfDocument = new Document(Filepath);
                PdfPageEditor pEdit = new PdfPageEditor(pdfDocument);
                Aspose.Pdf.PageSize pSize = pEdit.GetPageSize(1);
                htmloptions.PageInfo.Width = pSize.Width;
                htmloptions.PageInfo.Height = pSize.Height;
            }
            // Load HTML content
            Document doc = new Document(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlFilepath)), htmloptions);
            //doc.FitWindow = true;

            // Save HTML file
            doc.Save(pdfSavePath);
            return pdfSavePath;
            /* Latest Version 20.x Implementation Ends here */
        }
        public void CreateBlankPDF(string fromFile, string fileSavePath)
        {
            /* Old Code Commented since pdf generator disable in in version 17.9.0*/
            //Document pdfDoc = new Document(fromFile);
            //Aspose.Pdf.Generator.Pdf pdf = new Pdf();
            //foreach (Page page in pdfDoc.Pages)
            //{
            //    Aspose.Pdf.Generator.Section sec = pdf.Sections.Add();
            //    sec.PageInfo.PageHeight = (float)page.MediaBox.Height;
            //    sec.PageInfo.PageWidth = (float)page.MediaBox.Width;
            //}
            //pdf.Save(fileSavePath);
            /* Latest Version 20.x Implementation Starts here */
            Aspose.Pdf.Document pdfDoc = new Aspose.Pdf.Document(fromFile);
            Aspose.Pdf.Document pdf = new Document();
            pdf.PageInfo.Height = pdfDoc.PageInfo.Height;
            pdf.PageInfo.Width = pdfDoc.PageInfo.Width;
            foreach (Page page in pdfDoc.Pages)
            {
                Page sec = pdf.Pages.Add();
                sec.PageInfo.Height = (float)page.MediaBox.Height;
                sec.PageInfo.Width = (float)page.MediaBox.Width;
                if (pdf.PageInfo.Height < sec.PageInfo.Height)
                {
                    pdf.PageInfo.Height = sec.PageInfo.Height;
                }
                if (pdf.PageInfo.Width < sec.PageInfo.Width)
                {
                    pdf.PageInfo.Width = sec.PageInfo.Width;
                }
                sec.SetPageSize(sec.PageInfo.Width, sec.PageInfo.Height);
            }
            pdf.Save(fileSavePath);
            /* Latest Version 20.x Implementation Ends here*/
        }
        public void TransformPDFFromDocxAndCopyToDestination(string inputFilePath, string convertedDir, string fileName)
        {
            loggerModelNew = new LoggerModelNew("", "AsposeHelper", "TransformPDFFromDocxAndCopyToDestination", "Process is started for Transform PDF From Docx And Copy To Destination", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                using (Aspose.Pdf.Document inputDocument = new Document(inputFilePath))
                {
                    Aspose.Pdf.DocSaveOptions saveOptions = new DocSaveOptions();
                    saveOptions.Format = Aspose.Pdf.DocSaveOptions.DocFormat.DocX;
                    inputDocument.Save(Path.Combine(convertedDir, System.IO.Path.GetFileNameWithoutExtension(fileName) + ".docx"), saveOptions);

                    Aspose.Words.Document tempDocument = new Aspose.Words.Document(Path.Combine(convertedDir, System.IO.Path.GetFileNameWithoutExtension(fileName) + ".docx"));
                    tempDocument.Save(Path.Combine(convertedDir, fileName));
                }
                try
                {
                    loggerModelNew.Message = "Created docx file is deleted";
                    rsignlog.RSignLogInfo(loggerModelNew);

                    File.Delete(Path.Combine(convertedDir, System.IO.Path.GetFileNameWithoutExtension(fileName) + ".docx"));
                }
                catch (Exception ex)
                {

                    loggerModelNew.Message = "Error occurred in TransformPDFFromDocxAndCopyToDestination method while deleting docx file." + ex.Message.ToString();
                    rsignlog.RSignLogError(loggerModelNew, ex);
                }
            }
            catch (Exception ex)
            {

                loggerModelNew.Message = "Error occurred in TransformPDFFromDocxAndCopyToDestination method." + ex.Message.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                if (File.Exists(Path.Combine(convertedDir, fileName)))
                    File.Delete(Path.Combine(convertedDir, fileName));
                File.Copy(inputFilePath, Path.Combine(convertedDir, fileName));
            }
        }

        public string GetRecipientDeliveryMode(int? deliveryMode)
        {
            if (deliveryMode == 1 || deliveryMode == null) return Constants.MessageDeliveryModes.EmailSlashEmail;
            else if (deliveryMode == 2) return Constants.MessageDeliveryModes.EmailSlashMobile;
            else if (deliveryMode == 3) return Constants.MessageDeliveryModes.EmailSlashEmailAndMobile;
            else if (deliveryMode == 4) return Constants.MessageDeliveryModes.MobileSlashMobile;
            else if (deliveryMode == 5) return Constants.MessageDeliveryModes.MobileSlashEmail;
            else if (deliveryMode == 6) return Constants.MessageDeliveryModes.MobileSlashNone;
            else if (deliveryMode == 7) return Constants.MessageDeliveryModes.MobileSlashEmailAndMobile;
            else if (deliveryMode == 8) return Constants.MessageDeliveryModes.EmailAndMobileSlashMobile;
            else if (deliveryMode == 9) return Constants.MessageDeliveryModes.EmailAndMobileSlashEmail;
            else if (deliveryMode == 10) return Constants.MessageDeliveryModes.EmailAndMobileSlashEmailAndMobile;
            else if (deliveryMode == 11) return Constants.MessageDeliveryModes.EmailSlashNone;
            else if (deliveryMode == 12) return Constants.MessageDeliveryModes.EmailAndMobileSlashNone;
            else return Constants.MessageDeliveryModes.EmailSlashEmail;
        }

        public string AppendSignerEmailMobileDetails(int? deliveryMode, string EmailAddress, string DialCode, string Mobile)
        {
            string emailMobileNumberDetails = string.Empty;
            if (deliveryMode == Constants.DeliveryModes.EmailSlashEmail || deliveryMode == Constants.DeliveryModes.EmailSlashNone)
            {
                emailMobileNumberDetails = EmailAddress;
            }
            else if (deliveryMode == Constants.DeliveryModes.EmailSlashMobile || deliveryMode == Constants.DeliveryModes.EmailSlashEmailAndMobile || deliveryMode == Constants.DeliveryModes.EmailAndMobileSlashMobile
                || deliveryMode == Constants.DeliveryModes.EmailAndMobileSlashEmail || deliveryMode == Constants.DeliveryModes.EmailAndMobileSlashEmailAndMobile
                || deliveryMode == Constants.DeliveryModes.EmailAndMobileSlashNone || deliveryMode == Constants.DeliveryModes.MobileSlashEmail
                || deliveryMode == Constants.DeliveryModes.MobileSlashEmailAndMobile)
            {
                if (!string.IsNullOrEmpty(EmailAddress) && !string.IsNullOrEmpty(Mobile)) emailMobileNumberDetails = EmailAddress + ", " + DialCode + Mobile;
                else if (!string.IsNullOrEmpty(EmailAddress)) emailMobileNumberDetails = EmailAddress;
                else if (!string.IsNullOrEmpty(Mobile)) emailMobileNumberDetails = DialCode + Mobile;
            }
            else if (!string.IsNullOrEmpty(Mobile) && (deliveryMode == Constants.DeliveryModes.MobileSlashMobile || deliveryMode == Constants.DeliveryModes.MobileSlashNone))
            {
                emailMobileNumberDetails = DialCode + Mobile;
            }
            else if (!string.IsNullOrEmpty(EmailAddress)) emailMobileNumberDetails = EmailAddress;
            else if (!string.IsNullOrEmpty(Mobile)) emailMobileNumberDetails = DialCode + Mobile;

            return emailMobileNumberDetails;
        }

    }
}
