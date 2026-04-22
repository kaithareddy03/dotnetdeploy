using Chilkat;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RSign.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using submitter = port25.pmta.api.submitter;
using System.Net;
using System.Globalization;

namespace RSign.Common.Mailer
{
    public class ChilkatHelper
    {
        private readonly IConfiguration _configuration;
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        public ChilkatHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            rsignlog = new RSignLogger(_configuration);
        }
        public List<EmailLogs> SendMailUsingChilKet(string[] toAddress, string[] toDisplayName,
                                  string[] ccAddress, string[] ccDisplayName,
                                  string[] bccAddress, string[] bccDisplayName,
                                  string fromAddress, string fromDisplayName,
                                  string subject, string messageBody,
                                  List<byte[]> attachments, List<string> fileName, string EnvelopeId = "",
                                  string OperationName = "", int replyTo = 1, Guid msgId = default(Guid)
                                  )
        {

            loggerModelNew = new LoggerModelNew(toAddress != null && toAddress.Length > 0 ? toAddress[0].ToString() : string.Empty, OperationName, "SendMailUsingChilKat", "Start save .eml file using chilkat.", EnvelopeId, "", "", "", "API");
            EmailLogs logs = new EmailLogs();
            List<EmailLogs> emailLogs = new List<EmailLogs>();
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var _configuration = builder.Build();

                bool sendEmailFromService = Convert.ToBoolean(_configuration["SendEmailFromService"]);
                int RetryCount = 0;
                rsignlog.RSignLogEmlNew(loggerModelNew);
                MailMan mailMan = new MailMan();
                bool success = mailMan.UnlockComponent("30-day trial");
                var email = new Email();
                string fromEmailAddress = string.Empty;
                if (subject == "Company Activation" || subject == "Company Deactivation" || subject == "Company Registration Successful")
                {
                    fromEmailAddress = Convert.ToString(_configuration["FromEmailAddressSupport"]);
                }
                else
                {
                    fromEmailAddress = Convert.ToString(_configuration["FromEmailAddress"]);
                    if (replyTo == 3)
                    {
                        fromEmailAddress = Convert.ToString(_configuration["FromEmailAddressRSign"]);
                        replyTo = 2;
                    }
                }
                string viaText = Convert.ToString(_configuration["SendMailViaText"]);


                bool isSendFromMta = _configuration["SendFromMta"] != null ? Convert.ToBoolean(_configuration["SendFromMta"].ToString()) : false;

                bool isAlternativePMTADomain = false;
                if (File.Exists(Convert.ToString(_configuration["DomainsForAlternativeMTA"])))
                {
                    string[] domainsForAlternativeMTA = File.ReadAllLines(Convert.ToString(_configuration["DomainsForAlternativeMTA"]));

                    foreach (var address in toAddress)
                    {
                        if (address.LastIndexOf("@") > 0)
                        {
                            if (domainsForAlternativeMTA.Any(s => !string.IsNullOrEmpty(s.Trim()) && (s.ToUpper() == "ANY" || address.Substring(address.LastIndexOf("@") + 1).Contains(s))))
                            {
                                isAlternativePMTADomain = true;
                                fromEmailAddress = Convert.ToString(_configuration["FromEmailAddressRSign"]);
                                replyTo = 2;
                                isSendFromMta = true;
                            }
                        }
                    }
                }

                bool isBlockedDomain = false;
                string AlternateEmailRouting = Convert.ToString(_configuration["AlternateEmailRouting"]).ToLower();
                string AlternateEmailDomain = Convert.ToString(_configuration["AlternateEmailDomain"]);
                if (!isAlternativePMTADomain && File.Exists(_configuration["DomainsToChangeFromAddress"].ToString()))
                {
                    string[] DomainsToChangeFromAddress = File.ReadAllLines(_configuration["DomainsToChangeFromAddress"].ToString());

                    foreach (var address in toAddress)
                    {
                        if (address.LastIndexOf("@") > 0)
                        {
                            if (DomainsToChangeFromAddress.Any(s => address.Substring(address.LastIndexOf("@") + 1).Contains(s)))
                                isBlockedDomain = true;
                        }
                    }
                }

                if (isBlockedDomain)
                {
                    fromEmailAddress = Convert.ToString(_configuration["FromEmailAddressAlternative"]);
                    isSendFromMta = false;
                }


                string emailDisplayNameToEML = string.Empty;
                if (toAddress != null && toAddress.Length > 0)
                {
                    foreach (var item in toAddress)
                    {
                        if (item.IndexOf("@") > -1)
                        {
                            if (string.IsNullOrEmpty(emailDisplayNameToEML))
                                emailDisplayNameToEML = item.Split('@')[0].ToString();
                            else
                                emailDisplayNameToEML = emailDisplayNameToEML + "_" + item.Split('@')[0].ToString();
                        }
                    }

                    string ToEmailAddress = string.Empty;
                    if (toAddress.Length > 1)
                    {
                        if (isBlockedDomain && AlternateEmailRouting == "rcs")
                        {
                            foreach (string toAddr in toAddress)
                            {
                                string convertedemail = ConvertToASCIIAddress(toAddr);
                                ToEmailAddress = ToEmailAddress + convertedemail.Trim() + "." + AlternateEmailDomain + ",";
                            }
                            ToEmailAddress = ToEmailAddress.TrimEnd(',');

                            //foreach (string toAddr in toAddress)
                            //{
                            //    ToEmailAddress = ToEmailAddress + toAddr.Trim() + "." + AlternateEmailDomain + ",";
                            //}
                            //ToEmailAddress = ToEmailAddress.TrimEnd(',');
                        }
                        else
                        {
                            foreach (string toAddr in toAddress)
                            {
                                string convertedemail = ConvertToASCIIAddress(toAddr);
                                ToEmailAddress = convertedemail.Trim() + ",";
                            }
                            ToEmailAddress = ToEmailAddress.TrimEnd(',');
                            //ToEmailAddress = string.Join(",", toAddress).ToString().TrimEnd(',');
                        }
                        email.AddMultipleTo(ToEmailAddress);
                    }
                    else
                    {
                        string convertedemail = ConvertToASCIIAddress(toAddress[0].ToString());
                        if (isBlockedDomain && AlternateEmailRouting == "rcs")
                        {
                            ToEmailAddress = convertedemail + "." + AlternateEmailDomain;
                        }
                        else
                        {
                            ToEmailAddress = convertedemail;
                        }

                        //if (isBlockedDomain && AlternateEmailRouting == "rcs")
                        //{
                        //    ToEmailAddress = toAddress[0].ToString() + "." + AlternateEmailDomain;
                        //}
                        //else
                        //{
                        //    ToEmailAddress = toAddress[0].ToString();
                        //}
                        email.AddTo(toDisplayName != null && toDisplayName[0] != null ? toDisplayName[0].ToString() : string.Empty, ToEmailAddress);
                    }

                }
                else
                    throw new Exception("Recipients address is not specified.");

                //Assign CC
                if (ccAddress != null && ccAddress.Length > 0)
                {
                    string ccEmailAddress = string.Empty;
                    if (ccAddress.Length > 1)
                    {
                        // email.AddMultipleCC(string.Join(",", ccAddress).ToString().TrimEnd(','));

                        foreach (string ccAddr in ccAddress)
                        {
                            string convertedCCEmail = ConvertToASCIIAddress(ccAddr);
                            ccEmailAddress = convertedCCEmail.Trim() + ",";
                        }
                        ccEmailAddress = ccEmailAddress.TrimEnd(',');
                        email.AddMultipleCC(ccEmailAddress);

                    }
                    else
                    {
                        ccEmailAddress = ConvertToASCIIAddress(ccAddress[0].ToString());
                        email.AddCC(ccDisplayName != null && ccDisplayName[0] != null ? ccDisplayName[0].ToString() : string.Empty, ccEmailAddress);
                    }
                }

                //Assign BCC
                if (bccAddress != null && bccAddress.Length > 0)
                {
                    string bccEmailAddress = string.Empty;
                    if (bccAddress.Length > 1)
                    {
                        // email.AddMultipleBcc(string.Join(",", bccAddress).ToString().TrimEnd(','));
                        foreach (string bccAddr in bccAddress)
                        {
                            string convertedBCCEmail = ConvertToASCIIAddress(bccAddr);
                            bccEmailAddress = convertedBCCEmail.Trim() + ",";
                        }
                        bccEmailAddress = bccEmailAddress.TrimEnd(',');
                        email.AddMultipleBcc(bccEmailAddress);
                    }
                    else
                    {
                        bccEmailAddress = ConvertToASCIIAddress(bccAddress[0].ToString());
                        email.AddBcc(bccDisplayName != null && bccDisplayName[0] != null ? bccDisplayName[0].ToString() : string.Empty, bccEmailAddress);
                    }
                }

                //Assign From
                if (string.IsNullOrEmpty(fromEmailAddress))
                    throw new Exception("From address is not specified.");

                //Assign recipients
                if (string.IsNullOrEmpty(EnvelopeId))
                {
                    EnvelopeId = Guid.NewGuid().ToString();
                }

                if (emailDisplayNameToEML.Length > 150)
                    emailDisplayNameToEML = string.Concat(emailDisplayNameToEML.Take(150));

                string emailBackupLocation = Path.Combine(Convert.ToString(_configuration["EmailBackup"]), EnvelopeId);
                if (!Directory.Exists(emailBackupLocation))
                    Directory.CreateDirectory(emailBackupLocation);

                //If we have | in email like |test or test|test then it giving illegal two many chars. to handle this we are encoding and decoding
                emailDisplayNameToEML = HttpUtility.UrlEncode(emailDisplayNameToEML);

                if (!string.IsNullOrEmpty(OperationName))
                    emailBackupLocation = Path.Combine(emailBackupLocation, EnvelopeId.ToLower() + "_" + OperationName + "_" + emailDisplayNameToEML + "_" + GetTimestamp(DateTime.Now).ToString() + ".eml");
                else
                    emailBackupLocation = Path.Combine(emailBackupLocation, EnvelopeId.ToLower() + "_" + emailDisplayNameToEML + "_" + GetTimestamp(DateTime.Now).ToString() + ".eml");

                emailBackupLocation = HttpUtility.UrlDecode(emailBackupLocation);

                if (File.Exists(emailBackupLocation))
                    emailBackupLocation = RenameDuplicateFilePath(Path.Combine(Convert.ToString(_configuration["EmailBackup"]), EnvelopeId), Path.GetFileName(emailBackupLocation), 0);


                loggerModelNew.Message = "Backup location : " + emailBackupLocation;
                rsignlog.RSignLogEmlNew(loggerModelNew);

                email.FromAddress = fromEmailAddress;
                email.FromName = fromDisplayName + " via " + viaText;
                fromAddress = !string.IsNullOrEmpty(fromAddress) ? fromAddress : fromEmailAddress;
                //email.ReplyTo = !string.IsNullOrEmpty(fromDisplayName) ? fromDisplayName + "<" + fromAddress + ">" : fromAddress;
                //if (OperationName == "UserAccount")
                //    email.ReplyTo = !string.IsNullOrEmpty(fromDisplayName) ? fromDisplayName + "<" + fromAddress + ">" : fromAddress;
                //else
                //    email.ReplyTo = _configuration["ReplyToEmailAddress"];

                if (replyTo == 2)
                    email.ReplyTo = !string.IsNullOrEmpty(fromDisplayName) ? fromDisplayName + "<" + fromAddress + ">" : fromAddress;
                else        //  replyTo = 1
                    email.ReplyTo = _configuration["ReplyToEmailAddress"];

                //if (OperationName == "InitialOffer" || OperationName == "Resend" || OperationName == "Delegate" || OperationName == "PasswordToSign" || OperationName == "SignRecipient")
                //    email.ReplyTo = _configuration["ReplyToEmailAddress"];  // contract@rpost.com
                //else
                //    email.ReplyTo = !string.IsNullOrEmpty(fromDisplayName) ? fromDisplayName + "<" + fromAddress + ">" : fromAddress;

                email.Subject = subject;
                //email.SetTextBody(messageBody, "text/plain");
                //email.AddHtmlAlternativeBody(messageBody);
                messageBody = ReplaceImageWithContentId(messageBody, email);
                email.SetHtmlBody(messageBody);
                var strPlainTextMsgBody = HtmlToPlainText(messageBody);
                email.AddPlainTextAlternativeBody(strPlainTextMsgBody);
                email.AddHeaderField("X-RSign-Type", "R"); // adding headers
                email.Charset = "utf-8"; //

                if (attachments != null && fileName != null)
                {
                    for (int counter = 0; counter < attachments.Count; counter++)
                    {
                        if (counter < fileName.Count)
                        {
                            email.AddDataAttachment(fileName[counter].ToString(), attachments[counter]);
                        }
                    }
                }

                bool isEmailSentWithPMTA = false;

                bool isExcludedDomainForPMTA = false;
                if (!isAlternativePMTADomain && File.Exists(_configuration["PMTADomainsToExclude"].ToString()))
                {
                    string[] domainsToExcludeForPmta = File.ReadAllLines(_configuration["PMTADomainsToExclude"].ToString());
                    if (replyTo == 2 && isSendFromMta)
                    {
                        foreach (var address in toAddress)
                        {
                            if (address.LastIndexOf("@") > 0)
                            {
                                if (domainsToExcludeForPmta.Any(s => address.Substring(address.LastIndexOf("@") + 1).Contains(s)))
                                    isExcludedDomainForPMTA = true;
                            }
                        }
                    }
                }

                //&& OperationName != eSign.Core.Helpers.Constants.String.EmailOperation.Reject && OperationName != eSign.Core.Helpers.Constants.String.EmailOperation.Delegate
                if (sendEmailFromService && (OperationName != Constants.String.EmailOperation.SendingConfirmation && OperationName != Constants.String.EmailOperation.Accept && OperationName != Constants.String.EmailOperation.SignRecipient &&
                    OperationName != Constants.String.EmailOperation.Send && OperationName != Constants.String.EmailOperation.SendCC && OperationName != Constants.String.EmailOperation.Resend &&
                    OperationName != Constants.String.EmailOperation.PasswordToSign && OperationName != Constants.String.EmailOperation.PasswordToOpen))
                    sendEmailFromService = false;
                if (!sendEmailFromService)
                {
                    if (replyTo == 2 && isSendFromMta && !isExcludedDomainForPMTA)
                    {

                        if (msgId == Guid.Empty)
                            msgId = Guid.NewGuid();

                        string VirtualAddress = isBlockedDomain ? _configuration["MTAServerVirtualAddressSecondary"] : _configuration["MTAServerVirtualAddress"];
                        var fromAddressHeader = string.Format("{0}@{1}", msgId.ToString(), VirtualAddress);
                        var fromAddressHeaderWithFriendlyName = fromEmailAddress + "<" + string.Format("{0}rcpt@{1}", msgId.ToString(), VirtualAddress) + ">";

                        email.PrependHeaders = true;
                        email.AddHeaderField("Reply-To", email.ReplyTo); // Reply To Address
                                                                         //email.AddHeaderField("Sender", email.FromName); // Sender
                                                                         //email.AddHeaderField("Return-Path", fromAddressHeader); // Return-Path                

                        /*TP: Commented as per S3-1434: Email Delivery Timestamp In The Future*/
                        /*
                        var msgDate = DateTime.UtcNow;
                        var tzOffset = -420;
                        var ts = TimeSpan.FromMinutes(tzOffset); // Timezone offset from UTC
                        var dateHdrValue = string.Format("{0}, {1} {2} {3} {4}:{5}:{6} {7}{8}", msgDate.ToString("ddd"), msgDate.Day,
                           msgDate.ToString("MMM"), msgDate.Year, msgDate.ToString("HH"), msgDate.ToString("mm"), msgDate.ToString("ss"),
                           tzOffset < 0 ? "-" : "+", ts.ToString("hhmm"));

                        email.AddHeaderField("Date", dateHdrValue);
                        */

                        //email.AddHeaderField("Disposition-Notification-To", fromAddressHeaderWithFriendlyName); //Read Receipt
                        email.AddHeaderField("X-Confirm-Reading-to", fromAddressHeaderWithFriendlyName);
                        email.AddHeaderField("Return-Path", fromAddressHeader);
                        email.AddHeaderField("Return-Receipt-To", fromAddressHeaderWithFriendlyName); //Delivery Receipt - DSN (delivery status notification)


                        #region PMTA process starts here, if email is not successfully sent after 3 tries, send with SMTP virtual                               
                        var pmtaHostName = isAlternativePMTADomain ? _configuration["AlternativeMTAHost"] : _configuration["MTAHost"];
                        var pmtaPortNum = Convert.ToInt32(_configuration["MTAPort"]);
                        for (int i = 0; i < toAddress.Length; i++)
                        {
                            string convertedEmail = toAddress[i];
                            convertedEmail = ConvertToASCIIAddress(convertedEmail);
                            var emailToSend = email.Clone();
                            emailToSend.AddHeaderField("X-Sender", fromAddressHeader);
                            emailToSend.AddHeaderField("X-Receiver", convertedEmail);
                            emailToSend.PrependHeaders = false;
                            var port25Message = new submitter.Message(fromAddressHeader)
                            {
                                Encoding = submitter.Encoding.EightBit,
                                ReturnType = submitter.ReturnType.Full,
                                //JobID = string.Format("{0}.{1}", msgId, i),
                                JobID = string.Format("{0}", msgId),
                                EnvID = string.Format("{0}", EnvelopeId),
                            };
                            byte[] msgData = emailToSend.GetMimeBinary();
                            port25Message.AddData(msgData);
                            var recipient = new submitter.Recipient(convertedEmail, submitter.RecipientOptions.NoEmailSyntaxCheck);
                            recipient.Notify = submitter.Notify.Delay | submitter.Notify.Failure | submitter.Notify.Success;
                            port25Message.AddRecipient(recipient);
                            try
                            {
                                var port25Connection = new submitter.Connection(pmtaHostName, pmtaPortNum);
                                port25Connection.Submit(port25Message);
                                port25Connection.Close();
                                isEmailSentWithPMTA = true;
                                logs = new EmailLogs();
                                logs.EmailSentType = "PMTA";
                                logs.ResponseMessage = "Success";
                                emailLogs.Add(logs);
                            }
                            catch (Exception ex1)
                            {
                                try
                                {
                                    logs = new EmailLogs();
                                    logs.MessageId = Convert.ToString(msgId);
                                    logs.InnerException = ex1.Message;
                                    logs.FullException = JsonConvert.SerializeObject(ex1);
                                    logs.ServerName = pmtaHostName;
                                    logs.CreatedDate = System.DateTime.Now;
                                    logs.EnvelopeCode = EnvelopeId;
                                    logs.EmailSentType = "PMTA";
                                    logs.ResponseMessage = "Failed";
                                    logs.RetryCount = RetryCount + 1;
                                    emailLogs.Add(logs);
                                }
                                catch (Exception exlogs)
                                {
                                    loggerModelNew.Message = "First catch block getting Error while adding emaillogs list" + exlogs.Message;
                                    rsignlog.RSignLogError(loggerModelNew, exlogs);
                                }
                                loggerModelNew.Message = "First catch block is executing and Error while sending email from PMTA server :" + ex1.Message;
                                rsignlog.RSignLogError(loggerModelNew, ex1);

                                try
                                {
                                    var port25Connection = new submitter.Connection(pmtaHostName, pmtaPortNum);
                                    port25Connection.Submit(port25Message);
                                    port25Connection.Close();
                                    isEmailSentWithPMTA = true;
                                    logs = new EmailLogs();
                                    logs.EmailSentType = "PMTA";
                                    logs.ResponseMessage = "Success";
                                    emailLogs.Add(logs);
                                }
                                catch (Exception ex2)
                                {
                                    try
                                    {
                                        logs = new EmailLogs();
                                        logs.MessageId = Convert.ToString(msgId);
                                        logs.InnerException = ex2.Message;
                                        logs.FullException = JsonConvert.SerializeObject(ex2);
                                        logs.ServerName = pmtaHostName;
                                        logs.CreatedDate = System.DateTime.Now;
                                        logs.EnvelopeCode = EnvelopeId;
                                        logs.EmailSentType = "PMTA";
                                        logs.ResponseMessage = "Failed";
                                        logs.RetryCount = RetryCount + 1;
                                        emailLogs.Add(logs);
                                    }
                                    catch (Exception exlogs)
                                    {
                                        loggerModelNew.Message = "second catch block getting Error while adding emaillogs list" + exlogs.Message;
                                        rsignlog.RSignLogError(loggerModelNew, exlogs);
                                    }
                                    loggerModelNew.Message = "Second catch block is executing and Error while sending email from PMTA server :" + ex2.Message;
                                    rsignlog.RSignLogError(loggerModelNew,ex2);

                                    try
                                    {
                                        var port25Connection = new submitter.Connection(pmtaHostName, pmtaPortNum);
                                        port25Connection.Submit(port25Message);
                                        port25Connection.Close();
                                        isEmailSentWithPMTA = true;
                                        logs = new EmailLogs();
                                        logs.EmailSentType = "PMTA";
                                        logs.ResponseMessage = "Success";
                                        emailLogs.Add(logs);
                                    }
                                    catch (Exception ex3)
                                    {
                                        try
                                        {
                                            logs = new EmailLogs();
                                            logs.MessageId = Convert.ToString(msgId);
                                            logs.InnerException = ex3.Message;
                                            logs.FullException = JsonConvert.SerializeObject(ex3);
                                            logs.ServerName = pmtaHostName;
                                            logs.CreatedDate = System.DateTime.Now;
                                            logs.EnvelopeCode = EnvelopeId;
                                            logs.EmailSentType = "PMTA";
                                            logs.ResponseMessage = "Failed";
                                            logs.RetryCount = RetryCount + 1;
                                            emailLogs.Add(logs);
                                        }
                                        catch (Exception exlogs)
                                        {
                                            loggerModelNew.Message = "third catch block getting Error while adding emaillogs list" + exlogs.Message;
                                            rsignlog.RSignLogError(loggerModelNew, exlogs);
                                        }
                                        loggerModelNew.Message = "Third catch block is executing and Error while sending email from PMTA server :" + ex3.Message;
                                        rsignlog.RSignLogError(loggerModelNew,ex3);
                                        isEmailSentWithPMTA = false;
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                        isEmailSentWithPMTA = false;


                    System.Threading.Tasks.Task taskCreateEMLFile = new System.Threading.Tasks.Task(() =>
                    {
                        loggerModelNew.Message = "Eml file saved started at backup location : " + emailBackupLocation;
                        rsignlog.RSignLogEmlNew(loggerModelNew);
                        email.SaveEml(emailBackupLocation);
                    });
                    taskCreateEMLFile.Start();
                    taskCreateEMLFile.Wait();
                    if (!isEmailSentWithPMTA)
                    {
                        //SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                        logs = new EmailLogs();
                        logs.EmailSentType = "SMTP";
                        logs.RetryCount = 0;
                        logs.ResponseMessage = "Success";
                        emailLogs.Add(logs);

                        System.Threading.Tasks.Task taskCopyToPickUpFolder = new System.Threading.Tasks.Task(() =>
                        {
                            loggerModelNew.Message = "Eml file copy started";
                            rsignlog.RSignLogEmlNew(loggerModelNew);
                            try
                            {
                                email.SaveEml(Path.Combine(_configuration["PickupDirectoryLocation"], Path.GetFileName(emailBackupLocation)));
                                loggerModelNew.Message = "Copy Eml file to Pickup directory is done.";
                                rsignlog.RSignLogEmlNew(loggerModelNew);
                            }
                            catch (Exception ex)
                            {
                                loggerModelNew.Message = "Error occured while copy email file directly to pickup location";
                                rsignlog.RSignLogError(loggerModelNew, ex);

                                if (File.Exists(emailBackupLocation))
                                {
                                    //File.Copy(emailBackupLocation, Path.Combine(section.SpecifiedPickupDirectory.PickupDirectoryLocation, Path.GetFileName(emailBackupLocation)));

                                    loggerModelNew.Message = "Copy Eml file from backup to Pickup directory is done.";
                                    rsignlog.RSignLogEmlNew(loggerModelNew);
                                }
                            }
                            loggerModelNew.Message = OperationName + " dispatched to " + toAddress[0].ToString();
                            rsignlog.RSignLogEmlNew(loggerModelNew);
                        });
                        taskCopyToPickUpFolder.Start();
                    }
                    return emailLogs;
                }
                return emailLogs;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error occured while save email file";
                rsignlog.RSignLogError(loggerModelNew, ex);

                return emailLogs;
            }
        }
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        private static string RenameDuplicateFilePath(string FilePath, string FileName, int fileNameCounter)
        {
            fileNameCounter++;
            FileName = Path.GetFileNameWithoutExtension(FileName) + "_" + fileNameCounter.ToString() + Path.GetExtension(FileName);
            if (File.Exists(Path.Combine(FilePath, FileName)))
                FileName = RenameDuplicateFilePath(FilePath, FileName, fileNameCounter);

            return FileName;
        }
        public static string ReplaceImageWithContentId(string htmlText, Email email)
        {
            if (string.IsNullOrEmpty(htmlText) == false)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlText);
                var root = doc.DocumentNode;

                try
                {
                    // This replaces img tag with inner text including link
                    var imgNodes = doc.DocumentNode.Descendants().Where(i => i.Name == "img").ToList();
                    if (imgNodes != null)
                    {
                        bool isAnyBase64 = false;
                        foreach (var imgNode in imgNodes)
                        {
                            var imgSrc = imgNode.GetAttributeValue("src", string.Empty);
                            if (imgSrc.ToLower().Contains("base64,"))
                            {
                                isAnyBase64 = true;
                                BinData imageData = new BinData();
                                imgSrc = System.Text.RegularExpressions.Regex.Replace(imgSrc, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                                imageData.AppendEncoded(imgSrc, "base64");
                                string cid = email.AddRelatedData("imgbytes", imageData.GetBinary());
                                imgNode.SetAttributeValue("src", "cid:" + cid);
                            }
                        }
                        if (isAnyBase64)
                            return doc.DocumentNode.InnerHtml;
                        else
                            return htmlText;
                    }


                }
                catch
                {
                    // do not fail because of html to Plain text conversion.
                }
                return htmlText;

            }
            return null;
        }
        /// <summary>
        /// Takes HTML and grabs the textual content and creates a corresponding Plain Text.  This is different from RPost.StringExtensions.HtmlToText().
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        public static string HtmlToPlainText(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText) == false)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlText);
                var root = doc.DocumentNode;
                var builder = new System.Text.StringBuilder();

                try
                {
                    // This removes script and style tags
                    doc.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Name == "style").ToList().ForEach(n => n.Remove());

                    // This replaces hr tag with a bar
                    var hrNodes = doc.DocumentNode.Descendants().Where(hr => hr.Name == "hr").ToList();
                    if (hrNodes != null)
                    {
                        foreach (var hrNode in hrNodes)
                        {
                            var rhrNode = HtmlTextNode.CreateNode(new string('-', 60));
                            hrNode.ParentNode.ReplaceChild(rhrNode, hrNode);
                        }
                    }

                    // This replaces img tag with inner text including link
                    var imgNodes = doc.DocumentNode.Descendants().Where(i => i.Name == "img").ToList();
                    if (imgNodes != null)
                    {
                        foreach (var imgNode in imgNodes)
                        {
                            var imgSrc = imgNode.GetAttributeValue("src", string.Empty);
                            var imgAlt = imgNode.GetAttributeValue("alt", string.Empty);

                            if ((string.IsNullOrEmpty(imgAlt) == false) && (imgAlt.Length > 0))
                            {
                                var rimgNode = HtmlTextNode.CreateNode("imgAlt");
                                imgNode.ParentNode.ReplaceChild(rimgNode, imgNode);
                            }
                            else if ((string.IsNullOrEmpty(imgSrc) == false) && (imgSrc.Length > 0))
                            {
                                var rimgNode = HtmlTextNode.CreateNode("imgSrc");
                                imgNode.ParentNode.ReplaceChild(rimgNode, imgNode);
                            }
                        }
                    }

                    // This replaces a tag with inner text including link
                    var aNodes = doc.DocumentNode.Descendants().Where(a => a.Name == "a").ToList();
                    if (aNodes != null)
                    {
                        foreach (var aNode in aNodes)
                        {
                            var aHref = aNode.GetAttributeValue("href", string.Empty);

                            var raNode = HtmlTextNode.CreateNode(aHref);
                            aNode.ParentNode.ReplaceChild(raNode, aNode);
                        }
                    }

                    // This aggregates the Text Node(s)
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//text()[normalize-space(.) != '']"))
                    {
                        builder.AppendLine(node.InnerText.Trim());
                    }
                }
                catch
                {
                    // do not fail because of html to Plain text conversion.
                }
                string strHtml = builder.ToString().Replace("&nbsp;", " ");
                return strHtml;
            }
            return null;
        }

        public partial class EmailLogs
        {
            public int LogId { get; set; }
            public string MessageId { get; set; }
            public string EnvelopeCode { get; set; }
            public string ResponseMessage { get; set; }
            public string InnerException { get; set; }
            public string FullException { get; set; }
            public string ServerName { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public string EmailSentType { get; set; }
            public int RetryCount { get; set; }

        }

        /// <summary>
        /// Convert email address to ASCIIAddress
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string ConvertToASCIIAddress(string email)
        {
            if (email.Contains("@"))
            {
                // Split the email address into username and domain parts
                string[] parts = email.Split('@');
                if (parts.Count() < 2)
                    return string.Empty;
                string username = parts[0];
                string domain = parts[1];
                // Check if the domain part contains non-ASCII characters (special characters)
                if (!ContainsNonAsciiCharacters(domain))
                {
                    return email; // Return original email if no special characters found
                }
                // Create an instance of IdnMapping
                IdnMapping idn = new IdnMapping();
                // Convert the domain part to Punycode
                string asciiDomain = idn.GetAscii(domain);
                // Combine the username and Punycode domain to get the converted email address
                string convertedEmailAddress = $"{username}@{asciiDomain}";
                // Return the converted email address, not just the Punycode domain
                return convertedEmailAddress;
            }
            else
                return email;
        }

        /// <summary>
        /// Check if domain has ASCIIAddress
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool ContainsNonAsciiCharacters(string input)
        {
            List<char> _ScandinavianCharList = new List<char>(){
            'Å','å','Ä','ä','Ö','ö','Å','å','Æ','æ','Ø','ø'}; // List reference - https://sites.psu.edu/symbolcode
            foreach (char c in input)
            {
                if (c > 127 || c == 'Ø' || _ScandinavianCharList.Any(x => x == c))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
