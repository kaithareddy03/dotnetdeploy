using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.License;
using iText.Signatures;
using Microsoft.Extensions.Configuration;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.ManageDocument.Models;
using RSign.ManageDocument.Models.iTextHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static RSign.Common.Helpers.Constants;

namespace RSign.ManageDocument.Helpers
{
    public class iText7Helper : IiText7Helper
    {
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IConfiguration _appConfiguration;
        public static readonly Guid Europe = new Guid("{577D1738-6891-45DE-8481-E3353EB6A963}");
        private readonly string FontFolderPath;
        private readonly string DigitalCertificateIN;
        private readonly string DigitalCertificatePasswordIN;
        private readonly string DigitalCertificate;
        private readonly string DigitalCertificatePassword;
        private readonly string digitalCertTimestampUrl;
        public iText7Helper(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
            FontFolderPath = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["FontFolderPath"]));
            DigitalCertificate = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["DigitalCertificatePath"]));
            DigitalCertificatePassword = Convert.ToString(_appConfiguration["DigitalCertificatePassword"]);
            DigitalCertificateIN = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["DigitalCertificatePathIN"]));
            DigitalCertificatePasswordIN = Convert.ToString(_appConfiguration["DigitalCertificatePasswordIN"]);
            string licPath = System.IO.Path.Combine(Convert.ToString(_appConfiguration["CommonFilesPath"]), Convert.ToString(_appConfiguration["iTextLicPath"]));
            digitalCertTimestampUrl = Convert.ToString(_appConfiguration["DigitalCertTimestampUrl"]);            
            LicenseKey.LoadLicenseFile(licPath);
            rsignlog = new RSignLogger(_appConfiguration);
        }
        public string MergePDFiText(List<string> fileEntries, string outputPDF)
        {
            loggerModelNew = new LoggerModelNew("", "iText7Helper", "MergePDFiText", "Process is started for Merge PDF iText ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            using (PdfDocument outDoc = new PdfDocument(new PdfWriter(new FileStream(outputPDF, FileMode.OpenOrCreate, System.IO.FileAccess.Write), new WriterProperties().SetFullCompressionMode(true))))
            {
                int fromPageNo = 1;
                foreach (var inputItem in fileEntries)
                {
                    PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputItem));
                    int numOfPages = pdfDoc.GetNumberOfPages();
                    pdfDoc.Close();
                    pdfDoc = new PdfDocument(new PdfReader(inputItem));
                    pdfDoc.CopyPagesTo(fromPageNo, numOfPages, outDoc);
                    for (int j = 0; j <= numOfPages - fromPageNo; j++)
                    {
                        outDoc.GetPage(outDoc.GetNumberOfPages() - j).Flush(true);
                    }
                    pdfDoc.Close();
                }
            }
            return outputPDF;
        }
        public void AddTextToPDFiText7ForCC(string inputPDF, string outputPDF, List<DocumentControls> documentControls, string imageDirectory, string eDisplayCode = "", string uSerTimeZone = "", DateTime? completedDate = null, Guid dateFormatID = new Guid(), int intHeaderFooterOptions = 2)
        {
            loggerModelNew = new LoggerModelNew("", "ITextHelper7", "AddTextToPDFiText7ForCC", "Process is started for Add Text In Pdf method", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            // Modify PDF located at "source" and save to "target"
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputPDF), new PdfWriter(new FileStream(outputPDF, FileMode.OpenOrCreate, System.IO.FileAccess.Write), new WriterProperties().SetFullCompressionMode(true))))
            {
                if (!string.IsNullOrEmpty(eDisplayCode) && intHeaderFooterOptions > 1)
                {
                    iText.Kernel.Geom.Rectangle pageSize;
                    PdfCanvas canvas;
                    int n = pdfDocument.GetNumberOfPages();
                    string dateFormat = dateFormatID != Guid.Empty && dateFormatID == Europe ? "{0:dd/MM/yyyy HH:mm tt}" : "{0:MM/dd/yyyy HH:mm tt}";
                    string strCompletedDate = string.Empty;
                    if (string.IsNullOrEmpty(uSerTimeZone))
                        strCompletedDate = string.Format(dateFormat, completedDate) + " " + GetTimeZoneAbbreviation(System.TimeZone.CurrentTimeZone.StandardName);
                    else
                        strCompletedDate = string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(completedDate), uSerTimeZone, dateFormatID));
                    for (int i = 1; i <= n; i++)
                    {
                        PdfPage page = pdfDocument.GetPage(i);
                        page.SetIgnorePageRotationForContent(true);
                        pageSize = page.GetPageSize();
                        canvas = new PdfCanvas(page);
                        //canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 9).MoveText(5, 20).ShowText(eDisplayCode).EndText();
                        //canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 9).MoveText(5, 10).ShowText(strCompletedDate).EndText();
                        float w = pageSize.GetWidth();
                        float h = pageSize.GetHeight();

                        switch (intHeaderFooterOptions)
                        {
                            case 2: //LeftFooter
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(5, 20).ShowText(eDisplayCode).EndText();
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(5, 10).ShowText(strCompletedDate).EndText();
                                break;
                            case 3: //RightFooter
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(w - 180, 20).ShowText(eDisplayCode).EndText();
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(w - 180, 10).ShowText(strCompletedDate).EndText();
                                break;
                            case 4: //LeftHeader
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(5, h - 10).ShowText(eDisplayCode).EndText();
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(5, h - 20).ShowText(strCompletedDate).EndText();
                                break;
                            case 5: //RightHeader
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(w - 180, h - 10).ShowText(eDisplayCode).EndText();
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(w - 180, h - 20).ShowText(strCompletedDate).EndText();
                                break;
                        }
                    }
                }
                // Document to add layout elements: paragraphs, images etc
                Document document = new Document(pdfDocument);
                foreach (var control in documentControls)
                {
                    var controlValueForLink = string.Empty;
                    if (control.ControlName == "Hyperlink" && !string.IsNullOrEmpty(control.ControlValue))
                        controlValueForLink = control.ControlValue;
                    float leftpoint = 0, bottompoint = 0, width = 0, height = 0, imgRatio = 0;
                    DirectoryInfo dirInfo = new DirectoryInfo(imageDirectory);
                    var files = dirInfo.GetFiles(System.IO.Path.GetFileNameWithoutExtension(control.DocumentName) + "_*.png")
                        .Where(item => System.IO.Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(System.IO.Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == System.IO.Path.GetFileNameWithoutExtension(item.ToString()))
                        .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                    if (files.Count == 0)
                    {
                        files = dirInfo.GetFiles((System.IO.Path.GetFileNameWithoutExtension(control.DocumentName)) + "_*.jpg")
                            .Where(item => System.IO.Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(System.IO.Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == System.IO.Path.GetFileNameWithoutExtension(item.ToString()))
                            .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                    }
                    var myFile = files[control.DocumentPageNo - 1];
                    double assumedWidth = 948;
                    double originalWidth = 0.0;
                    using (Bitmap myImage = new Bitmap(System.IO.Path.Combine(imageDirectory, myFile)))
                    {
                        originalWidth = myImage.Width;
                    }
                    if (assumedWidth < originalWidth)
                        imgRatio = (float)((originalWidth - assumedWidth) / originalWidth) * 100;
                    else
                        imgRatio = (float)((assumedWidth - originalWidth) / assumedWidth) * 100;

                    if (control.ControlName == "Checkbox" || control.ControlName == "Radio" || ((control.ControlName == "Signature" || control.ControlName == "NewInitials") && control.ControlValue == "Signed"))
                    {
                        if (control.ImageBytes == null || control.ControlValue == null)
                            continue;
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
                        leftpoint = (float)((control.XLeftposition * 72.00) / 112.00);
                        bottompoint = (float)((control.ZBottompostion * 72.00) / 112.00);
                        width = (control.ControlName == "Checkbox" || control.ControlName == "Radio") ? 10 : (float)((imgWidth * 72.00) / 112.00);
                        height = (control.ControlName == "Checkbox" || control.ControlName == "Radio") ? 10 : (float)((imgHeight * 72.00) / 112.00);
                        // Load image from disk
                        ImageData imageStamp = ImageDataFactory.Create(control.ImageBytes);
                        if (assumedWidth < originalWidth)
                        {//Increase font size
                            width = (float)(width + ((width * imgRatio) / 100));
                            height = (float)(height + ((height * imgRatio) / 100));
                        }
                        else
                        {//Decrease font size
                            width = (float)(width - ((width * imgRatio) / 100));
                            height = (float)(height - ((height * imgRatio) / 100));
                        }

                        //imageStamp.SetRotation(0);
                        //iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData).ScaleAbsolute(100, 200).SetFixedPosition(1, 25, 25);
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageStamp).SetWidth(width).SetHeight(height).SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint);
                        // This adds the image to the page
                        document.Add(image);
                    }
                    else
                    {
                        leftpoint = (float)((control.XLeftposition * 72.00) / 112.00);
                        bottompoint = (float)((control.ZBottompostion * 72.00) / 112.00);
                        width = (float)((control.Width * 72.00) / 112.00);
                        height = (float)((control.Height * 72.00) / 112.00);

                        // create text fragment
                        var textValue = (control.ControlName == "Label" || control.ControlName == "Text" || control.ControlName == "Hyperlink") ? control.Label : "[ " + (control.ControlName == "NewInitials" ? "Initials" : control.ControlName) + " ]";//"[ " + GetControlName(control.ControlName) + " ]";

                        Paragraph paragraph = new Paragraph();
                        if (!string.IsNullOrEmpty(control.ControlValue))
                        {
                            paragraph = new Paragraph(control.ControlValue);
                        }
                        else
                        {
                            paragraph = new Paragraph(textValue);
                        }

                        if (control.UserControlStyle != null)
                        {
                            if (assumedWidth < originalWidth)
                            {
                                var ratio = ((originalWidth - assumedWidth) / originalWidth) * 100;
                                control.UserControlStyle.FontSize = control.UserControlStyle.FontSize + (int)((control.UserControlStyle.FontSize * ratio) / 100);
                            }
                            else
                            {
                                var ratio = ((assumedWidth - originalWidth) / assumedWidth) * 100;
                                control.UserControlStyle.FontSize = control.UserControlStyle.FontSize - (int)((control.UserControlStyle.FontSize * ratio) / 100);
                            }
                            PdfFont font = PdfFontFactory.CreateFont(System.IO.Path.Combine(FontFolderPath, control.UserControlStyle.FontName + ".ttf"), PdfEncodings.IDENTITY_H);
                            paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.ControlName == "Text" ? (control.UserControlStyle.FontSize / 1.52F) : (control.UserControlStyle.FontSize / 1.33F)));
                            if (control.UserControlStyle.IsBold && control.UserControlStyle.IsItalic
                                    && control.UserControlStyle.IsUnderline)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetBold().SetItalic().SetUnderline());
                            }
                            else if (control.UserControlStyle.IsBold && control.UserControlStyle.IsItalic)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetBold().SetItalic());
                            }
                            else if (control.UserControlStyle.IsBold && control.UserControlStyle.IsUnderline)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetBold().SetUnderline());
                            }
                            else if (control.UserControlStyle.IsItalic && control.UserControlStyle.IsUnderline)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetItalic().SetUnderline());
                            }
                            else if (control.UserControlStyle.IsBold)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetBold());
                            }
                            else if (control.UserControlStyle.IsItalic)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetItalic());
                            }
                            else if (control.UserControlStyle.IsUnderline)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetUnderline());
                            }
                        }
                        else
                        {
                            int defaultFontSize = 12;
                            if (assumedWidth < originalWidth)
                            {//Increase font size
                                defaultFontSize = defaultFontSize + (int)((defaultFontSize * imgRatio) / 100);
                            }
                            else
                            {//Decrease font size
                                defaultFontSize = defaultFontSize - (int)((defaultFontSize * imgRatio) / 100);
                            }
                            paragraph.AddStyle(new Style().SetFontSize(defaultFontSize / 1.33F));
                        }
                        if (control.ControlName == "Hyperlink")
                        {
                            paragraph.SetAction(PdfAction.CreateURI(controlValueForLink)).SetUnderline();

                        }
                        paragraph.SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint - 2, (float)(leftpoint + width + 10));
                        document.Add(paragraph);

                    }
                }
                document.Close();
            }
        }
        public void AddTextToPDFiText7(string inputPDF, string outputPDF, List<DocumentControls> documentControls, string imageDirectory, string eDisplayCode = "", string uSerTimeZone = "", DateTime? completedDate = null, Guid dateFormatID = new Guid(), int intHeaderFooterOptions = 2, int electronicSignatureStampId = 1)
        {
            loggerModelNew = new LoggerModelNew("", "ITextHelper7", "AddTextToPDFiText7", "Process is started for Add Text In Pdf method", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            // Modify PDF located at "source" and save to "target"
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputPDF), new PdfWriter(new FileStream(outputPDF, FileMode.OpenOrCreate, System.IO.FileAccess.Write), new WriterProperties().SetFullCompressionMode(true))))
            {
                if (!string.IsNullOrEmpty(eDisplayCode) && intHeaderFooterOptions > 1)
                {
                    iText.Kernel.Geom.Rectangle pageSize;
                    PdfCanvas canvas;
                    int n = pdfDocument.GetNumberOfPages();
                    string dateFormat = dateFormatID != Guid.Empty && dateFormatID == Europe ? "{0:dd/MM/yyyy HH:mm tt}" : "{0:MM/dd/yyyy HH:mm tt}";
                    string strCompletedDate = string.Empty;
                    if (string.IsNullOrEmpty(uSerTimeZone))
                        strCompletedDate = string.Format(dateFormat, completedDate) + " " + GetTimeZoneAbbreviation(System.TimeZone.CurrentTimeZone.StandardName);
                    else
                        strCompletedDate = string.Format(dateFormat, GetLocalTime(Convert.ToDateTime(completedDate), uSerTimeZone, dateFormatID));
                    for (int i = 1; i <= n; i++)
                    {
                        PdfPage page = pdfDocument.GetPage(i);
                        page.SetIgnorePageRotationForContent(true);
                        pageSize = page.GetPageSize();
                        canvas = new PdfCanvas(page);
                        //canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 9).MoveText(5, 20).ShowText(eDisplayCode).EndText();
                        //canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 9).MoveText(5, 10).ShowText(strCompletedDate).EndText();
                        float w = pageSize.GetWidth();
                        float h = pageSize.GetHeight();

                        switch (intHeaderFooterOptions)
                        {
                            case 2: //LeftFooter
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(5, 20).ShowText(eDisplayCode).EndText();
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(5, 10).ShowText(strCompletedDate).EndText();
                                break;
                            case 3: //RightFooter
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(w - 180, 20).ShowText(eDisplayCode).EndText();
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(w - 180, 10).ShowText(strCompletedDate).EndText();
                                break;
                            case 4: //LeftHeader
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(5, h - 10).ShowText(eDisplayCode).EndText();
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(5, h - 20).ShowText(strCompletedDate).EndText();
                                break;
                            case 5: //RightHeader
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(w - 180, h - 10).ShowText(eDisplayCode).EndText();
                                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 9).MoveText(w - 180, h - 20).ShowText(strCompletedDate).EndText();
                                break;
                        }

                    }
                }
                // Document to add layout elements: paragraphs, images etc
                Document document = new Document(pdfDocument);
                foreach (var control in documentControls)
                {
                    var controlValueForLink = string.Empty;
                    if (control.ControlName == "Hyperlink" && !string.IsNullOrEmpty(control.ControlValue))
                        controlValueForLink = control.ControlValue;
                    float leftpoint = 0, bottompoint = 0, width = 0, height = 0, imgRatio = 0;
                    DirectoryInfo dirInfo = new DirectoryInfo(imageDirectory);
                    var files = dirInfo.GetFiles(System.IO.Path.GetFileNameWithoutExtension(control.DocumentName) + "_*.png")
                        .Where(item => System.IO.Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(System.IO.Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == System.IO.Path.GetFileNameWithoutExtension(item.ToString()))
                        .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                    if (files.Count == 0)
                    {
                        files = dirInfo.GetFiles((System.IO.Path.GetFileNameWithoutExtension(control.DocumentName)) + "_*.jpg")
                            .Where(item => System.IO.Path.GetFileNameWithoutExtension(control.DocumentName) + "_" + Regex.Match(System.IO.Path.GetFileNameWithoutExtension(item.ToString()), @"\d+$").Value == System.IO.Path.GetFileNameWithoutExtension(item.ToString()))
                            .OrderBy(o => o.Name).Select(s => s.Name).ToList();
                    }
                    var myFile = files[control.DocumentPageNo - 1];
                    double assumedWidth = 948;
                    double originalWidth = 0.0;
                    using (Bitmap myImage = new Bitmap(System.IO.Path.Combine(imageDirectory, myFile)))
                    {
                        originalWidth = myImage.Width;
                    }
                    if (assumedWidth < originalWidth)
                        imgRatio = (float)((originalWidth - assumedWidth) / originalWidth) * 100;
                    else
                        imgRatio = (float)((assumedWidth - originalWidth) / assumedWidth) * 100;

                    if (control.ControlName == "Checkbox" || control.ControlName == "Radio" || ((control.ControlName == "Signature" || control.ControlName == "NewInitials") && control.ControlValue == "Signed"))
                    {
                        if (control.ImageBytes == null || control.ControlValue == null)
                            continue;
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
                        if (electronicSignatureStampId > 1 && control.ControlName == "Signature")
                        {
                            leftpoint = (float)((control.XLeftposition * 72.00) / 112.00);
                            bottompoint = (float)((control.ZBottompostion * 72.00) / 112.00);
                            width = (control.ControlName == "Checkbox" || control.ControlName == "Radio") ? 10 : (float)((imgWidth * 82.00) / 112.00);
                            height = (control.ControlName == "Checkbox" || control.ControlName == "Radio") ? 10 : (float)((imgHeight * 82.00) / 112.00);
                        }
                        else
                        {
                            leftpoint = (float)((control.XLeftposition * 72.00) / 112.00);
                            bottompoint = (float)((control.ZBottompostion * 72.00) / 112.00);
                            if (electronicSignatureStampId <= 1 && control.ControlName == "Signature" && control.IsTypeSignature != null && control.IsTypeSignature.Value)
                            {
                                bottompoint = bottompoint - 7;
                            }
                            if (control.ControlName == "Checkbox" || control.ControlName == "Radio")
                            {
                                width = imgWidth > 0 ? (float)((imgWidth * 72.00) / 112.00) : 10;
                                height = imgHeight > 0 ? (float)((imgHeight * 72.00) / 112.00) : 10;
                            }
                            else
                            {
                                width = (control.ControlName == "Checkbox" || control.ControlName == "Radio") ? 10 : (float)((imgWidth * 72.00) / 112.00);
                                height = (control.ControlName == "Checkbox" || control.ControlName == "Radio") ? 10 : (float)((imgHeight * 72.00) / 112.00);
                            }
                        }
                        // Load image from disk
                        ImageData imageStamp = ImageDataFactory.Create(control.ImageBytes);
                        if (assumedWidth < originalWidth)
                        {//Increase font size
                            width = (float)(width + ((width * imgRatio) / 100));
                            height = (float)(height + ((height * imgRatio) / 100));
                        }
                        else
                        {//Decrease font size
                            width = (float)(width - ((width * imgRatio) / 100));
                            height = (float)(height - ((height * imgRatio) / 100));
                        }

                        //imageStamp.SetRotation(0);
                        //iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData).ScaleAbsolute(100, 200).SetFixedPosition(1, 25, 25);
                        if ((control.ControlName == "Checkbox" || control.ControlName == "Radio") && control.Height == 14)
                            leftpoint = leftpoint - 1;
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageStamp).SetWidth(width).SetHeight(height).SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint);
                        // This adds the image to the page
                        document.Add(image);
                    }
                    else if (!(control.ControlName == "Signature" || control.ControlName == "Checkbox" || control.ControlName == "Radio"))
                    {
                        leftpoint = (float)((control.XLeftposition * 72.00) / 112.00);
                        bottompoint = (float)((control.ZBottompostion * 72.00) / 112.00);
                        width = (float)((control.Width * 72.00) / 112.00);
                        height = (float)((control.Height * 72.00) / 112.00);
                        if (control.ControlName == "Label" || control.ControlName == "Hyperlink")
                        {
                            control.ControlValue = control.Label;
                        }
                        if (control.ControlValue == null)
                        {
                            continue;
                        }
                        Paragraph paragraph = new Paragraph(control.ControlValue.Replace("\n", Environment.NewLine));
                        int extraHeight = 1;
                        if (control.UserControlStyle != null)
                        {
                            if (assumedWidth < originalWidth)
                            {
                                var ratio = ((originalWidth - assumedWidth) / originalWidth) * 100;
                                control.UserControlStyle.FontSize = control.UserControlStyle.FontSize + (int)((control.UserControlStyle.FontSize * ratio) / 100);
                            }
                            else
                            {
                                var ratio = ((assumedWidth - originalWidth) / assumedWidth) * 100;
                                control.UserControlStyle.FontSize = control.UserControlStyle.FontSize - (int)((control.UserControlStyle.FontSize * ratio) / 100);
                            }
                            PdfFont font = PdfFontFactory.CreateFont(System.IO.Path.Combine(FontFolderPath, control.UserControlStyle.FontName + ".ttf"), PdfEncodings.IDENTITY_H);                           
                            paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.ControlName == "Text" ? (control.UserControlStyle.FontSize / 1.52F) : (control.UserControlStyle.FontSize / 1.33F)));
                            if (control.UserControlStyle.IsBold && control.UserControlStyle.IsItalic
                                  && control.UserControlStyle.IsUnderline)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetBold().SetItalic().SetUnderline());
                            }
                            else if (control.UserControlStyle.IsBold && control.UserControlStyle.IsItalic)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetBold().SetItalic());
                            }
                            else if (control.UserControlStyle.IsBold && control.UserControlStyle.IsUnderline)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetBold().SetUnderline());
                            }
                            else if (control.UserControlStyle.IsItalic && control.UserControlStyle.IsUnderline)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetItalic().SetUnderline());
                            }
                            else if (control.UserControlStyle.IsBold)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetBold());
                            }
                            else if (control.UserControlStyle.IsItalic)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetItalic());
                            }
                            else if (control.UserControlStyle.IsUnderline)
                            {
                                paragraph.AddStyle(new Style().SetFont(font).SetFontSize(control.UserControlStyle.FontSize).SetUnderline());
                            }

                        }
                        else
                        {
                            int defaultFontSize = 12;
                            if (assumedWidth < originalWidth)
                            {//Increase font size
                                defaultFontSize = defaultFontSize + (int)((defaultFontSize * imgRatio) / 100);
                            }
                            else
                            {//Decrease font size
                                defaultFontSize = defaultFontSize - (int)((defaultFontSize * imgRatio) / 100);
                            }
                            paragraph.AddStyle(new Style().SetFontSize(defaultFontSize / 1.33F));
                        }
                        if (control.ControlName == "Text" && control.ControlType != "Email")
                        {
                            //bottompoint = (control.Width < 100 && (string.IsNullOrEmpty(control.ControlValue) || control.ControlValue.Length == 1))
                            //    ? (bottompoint + 7) : (bottompoint + 7);

                            //bottompoint = (control.Height > 30
                            //    ? (bottompoint + 8) : control.Height > 20 && control.Height < 30 ? bottompoint + 5 : (bottompoint + 3));
                            Div d = new Div();

                            d.SetWordSpacing(0);
                            d.SetWidth((float)(width));
                            //d.SetFixedPosition(leftpoint, bottompoint, (leftpoint + width), (bottompoint + height + 7))
                            //d.SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint - 2, (float)(leftpoint + width + 10));
                            d.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.TOP);
                            d.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);
                            //d.SetBorder(new iText.Layout.Borders.DashedBorder(1));
                            //d.SetMargin(2);
                            if (control.Height > 30)
                            {
                                bottompoint = bottompoint + 8;
                                d.SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint - 3, (float)(width));
                            }
                            else
                            {
                                d.SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint - 3, (float)(leftpoint + width + 10));
                            }
                            d.Add(paragraph);
                            document.Add(d);
                        }
                        else if (control.ControlName == "Label")
                        {
                            //Div d = new Div();

                            //d.SetWidth((float)(width + 7));
                            //d.SetMaxHeight((float)(height + 10));
                            //d.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.TOP);
                            //d.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);

                            //d.SetWordSpacing(0);
                            //if (control.Height > 20)
                            //{
                            //    bottompoint = bottompoint + 8;
                            //    d.SetFixedPosition(control.PageNo, (float)(leftpoint - 30), (float)bottompoint, (float)(width + 30));

                            //}
                            //else
                            //{
                            //    d.SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint, (float)(leftpoint + width + 10));
                            //}
                            //d.Add(paragraph);
                            //document.Add(d);

                            extraHeight = control.UserControlStyle.FontSize > 10 ? Convert.ToInt32(control.UserControlStyle.FontSize / 10) : 1;
                            paragraph.SetMaxHeight((float)(height + (25 * extraHeight)));
                            paragraph.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.TOP);
                            paragraph.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);
                            paragraph.SetMargin((float)0);
                            paragraph.SetMultipliedLeading((float)0.8);

                            if (control.Height > 20)
                            {

                                paragraph.SetFixedPosition(control.PageNo, (float)(leftpoint - 25), (float)bottompoint, (float)(width + 30));

                            }
                            else
                            {
                                bottompoint = bottompoint + 8;
                                paragraph.SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint, (float)(leftpoint + width + 10));
                            }
                            document.Add(paragraph);
                        }
                        else if (control.ControlName == "Hyperlink")
                        {
                            paragraph.SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint - 2, (float)(leftpoint + width + 10));
                            paragraph.SetAction(PdfAction.CreateURI(controlValueForLink)).SetUnderline();
                            document.Add(paragraph);
                        }
                        else
                        {
                            paragraph.SetFixedPosition(control.PageNo, (float)leftpoint, (float)bottompoint - 2, (float)(leftpoint + width + 10));
                            document.Add(paragraph);
                        }

                    }
                }
                document.Close();
            }
        }
        string GetTimeZoneAbbreviation(string timeZoneName)
        {
            loggerModelNew = new LoggerModelNew("", "ITextHelper7", "GetTimeZoneAbbreviation", "Process is started for Get TimeZone Abbreviation ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

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
                        outputDate = string.Format("{0:MM/dd/yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_mm_dd_yyyy_colan)
                        outputDate = string.Format("{0:MM-dd-yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_mm_dd_yyyy_dots)
                        outputDate = string.Format("{0:MM.dd.yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_slash)
                        outputDate = string.Format("{0:dd/MM/yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_colan)
                        outputDate = string.Format("{0:dd-MM-yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_mm_dd_yyyy_dots)
                        outputDate = string.Format("{0:dd.MM.yyyy HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.Europe_yyyy_mm_dd_dots)
                        outputDate = string.Format("{0:yyyy.MM.dd. HH:mm tt}", cstTime);
                    else if (dateFormatID == Constants.DateFormat.US_dd_mmm_yyyy_colan)
                        outputDate = string.Format("{0:dd-MMM-yyyy HH:mm tt}", cstTime);
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
        public void AddDigitalSignatureToTheDocumentIN(string srcPDF, string srcPDFPassword, string dstPDF, bool IsThrowException = false)
        {
            loggerModelNew = new LoggerModelNew("", "ITextHelper", "AddDigitalSignatureToTheDocumentIN", "Process is started for Add Digital Signature To The Document ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                ReaderProperties readerOptions = new ReaderProperties();
                if (!string.IsNullOrEmpty(srcPDFPassword))
                    readerOptions.SetPassword(Encoding.ASCII.GetBytes(srcPDFPassword));
                using (PdfReader reader = new PdfReader(srcPDF, readerOptions))
                {
                    PdfSigner signer = new PdfSigner(reader, new FileStream(dstPDF, FileMode.OpenOrCreate, System.IO.FileAccess.Write), new StampingProperties().UseAppendMode());
                    iText.Kernel.Geom.Rectangle rect = new iText.Kernel.Geom.Rectangle(0, 0, 0, 0);
                    DigitalCertIN digitalCert = new DigitalCertIN(DigitalCertificateIN, DigitalCertificatePasswordIN);
                   

                    // Creating the appearance
                    PdfSignatureAppearance appearance = signer.GetSignatureAppearance()
                        .SetSignatureCreator(Convert.ToString(_appConfiguration["DigitalCertSigCreatorIN"]))
                        .SetReason(Convert.ToString(_appConfiguration["DigitalCertReasonIN"]))
                        .SetLocation(Convert.ToString(_appConfiguration["DigitalCertLocationIN"]))
                        .SetReuseAppearance(false)
                        .SetPageRect(rect)
                        .SetPageNumber(1);
                    signer.SetSignDate(DateTime.Now);
                    signer.SetFieldName(Convert.ToString(_appConfiguration["DigitalCertFieldNameIN"]));
                    IExternalSignature pks = new PrivateKeySignature(digitalCert.pk, digitalCert.DigestAlgorithm);                    
                    signer.SignDetached(pks, digitalCert.chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
                    reader.Close();
                }               
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in AddDigitalSignatureToTheDocumentIN method." + ex.Message.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                if (IsThrowException)
                {
                    throw ex;
                }
            }
        }
        public void AddDigitalSignatureToTheDocument(string srcPDF, string srcPDFPassword, string dstPDF, bool IsThrowException = false)
        {
            loggerModelNew = new LoggerModelNew("", "ApiHelper", "AddDigitalSignatureToTheDocument", "Process is started for Add Digital Signature To The Document ", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                ReaderProperties readerOptions = new ReaderProperties();
                if (!string.IsNullOrEmpty(srcPDFPassword))
                    readerOptions.SetPassword(Encoding.ASCII.GetBytes(srcPDFPassword));
                using (PdfReader reader = new PdfReader(srcPDF, readerOptions))
                {
                    PdfSigner signer = new PdfSigner(reader, new FileStream(dstPDF, FileMode.OpenOrCreate, System.IO.FileAccess.Write), new StampingProperties().UseAppendMode());
                    iText.Kernel.Geom.Rectangle rect = new iText.Kernel.Geom.Rectangle(0, 0, 0, 0);
                    DigitalCert digitalCert = new DigitalCert(DigitalCertificate, DigitalCertificatePassword,true);
                    // Creating the appearance

                    var tsaClient = new TSAClientBouncyCastle(digitalCertTimestampUrl);
                    var ocsp = new OcspClientBouncyCastle(null);

                    PdfSignatureAppearance appearance = signer.GetSignatureAppearance()
                        .SetSignatureCreator(Convert.ToString(_appConfiguration["DigitalCertSigCreator"]))
                        .SetReason(Convert.ToString(_appConfiguration["DigitalCertReason"]))
                        .SetLocation(Convert.ToString(_appConfiguration["DigitalCertLocation"]))
                        .SetReuseAppearance(false)
                        .SetPageRect(rect)
                        .SetPageNumber(1);
                    signer.SetSignDate(DateTime.Now);
                    signer.SetFieldName(Convert.ToString(_appConfiguration["DigitalCertFieldName"]));
                    IExternalSignature pks = new PrivateKeySignature(digitalCert.Akp, digitalCert.DigestAlgorithm);
                    signer.SignDetached(pks, digitalCert.Chain, null, ocsp, tsaClient, 0, PdfSigner.CryptoStandard.CADES);
                    reader.Close();
                }
                EnableLTV(srcPDF, dstPDF, srcPDFPassword);
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occurred in AddDigitalSignatureToTheDocument method." + ex.Message.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                if (IsThrowException)
                {
                    throw ex;
                }
            }
        }
        private static void EnableLTV(string sourceFile, string signedPdfPath, string srcPDFPassword)
        {
            ReaderProperties readerOptions = new ReaderProperties();
            if (!string.IsNullOrEmpty(srcPDFPassword))
                readerOptions.SetPassword(Encoding.ASCII.GetBytes(srcPDFPassword));

            string pathNames = sourceFile;

            if (File.Exists(sourceFile))
                File.Delete(sourceFile);

            System.IO.File.Copy(signedPdfPath, pathNames, true);

            string destPath = signedPdfPath;
            if (File.Exists(signedPdfPath))
                File.Delete(signedPdfPath);

            using (PdfReader reader = new PdfReader(pathNames, readerOptions))
            {
                using (PdfWriter writer = new PdfWriter(destPath))
                {
                    using (PdfDocument pdfDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode()))
                    {
                        var v = new LtvVerification(pdfDoc);
                        var signatureUtil = new SignatureUtil(pdfDoc);
                        var names = signatureUtil.GetSignatureNames().ToList();
                        var sigName = names[names.Count - 1];                        
                        ICrlClient crlClient = new CrlClientOnline();
                        OcspClientBouncyCastle ocsp = new OcspClientBouncyCastle(null);                      

                        foreach (string name in names)
                        {
                            v.AddVerification(name, ocsp, crlClient, LtvVerification.CertificateOption.WHOLE_CHAIN,
                                              LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
                        }
                        v.Merge();
                        pdfDoc.Close();
                    }
                }
            }
        }
    }
}
