using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using RSign.Models.SignedDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChilkatHelper = RSign.Common.Mailer.ChilkatHelper;

namespace RSign.Models.EmailQueueProcessor
{
    public class EmailQueueData
    {
        public Envelope envelope { get; set; }
        public Template template { get; set; }
        public string EmailType { get; set; }
        public bool IsAttachment { get; set; }
        public string SenderName { get; set; }
        public string SenderEmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string MailMessageBody { get; set; }
        public int? SignReqReplyToAddressValue { get; set; }
        public Recipients Signer { get; set; }
        public SanboxRecipients SanboxSigner { get; set; }
        public EmailSendInfo emailSendInfo { get; set; }
    }
    public class EmailQueueRecipientsData
    {
        public EmailQueueData emailQueueData { get; set; }
        public int EmailQueueID { get; set; }
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientMobile { get; set; }
    }
    public class EmailQueueAttachmentData
    {
        public int AttachmentId { get; set; }
        public int EmailQueueID { get; set; }
        public List<byte[]> AttachmentData { get; set; }
        public List<string> AttachmentNamesInfo { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class EmailSendInfo
    {
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientMobile { get; set; }
    }
    public class EmailQueueProcessor : IEmailQueueProcessor
    {
        private readonly IConfiguration _appConfiguration;
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IOptions<AppSettingsConfig> _configuration;        
        private readonly IRecipientRepository _recipientRepository;
        private readonly IGenericRepository _genericRepository;
        public EmailQueueProcessor(IOptions<AppSettingsConfig> configuration, IConfiguration appConfiguration, IRecipientRepository recipientRepository, IGenericRepository genericRepository)
        {  
            _configuration = configuration;
            _appConfiguration = appConfiguration;           
            _recipientRepository = recipientRepository;
            _genericRepository = genericRepository;
            rsignlog = new RSignLogger(_appConfiguration);
        }
        /// <summary>
        /// Process Email queue from the Service
        /// </summary>
        /// <returns></returns>
        public bool ProcessEmailQueue()
        {
            bool isCompleted = false;
            int? transId = null;
            List<EmailQueueRecipients> emailQueueRecipients = new List<EmailQueueRecipients>();
            List<EmailQueueAttachment> emailQueueAttachment = new List<EmailQueueAttachment>();
            var attachmentForRec = new List<byte[]>();
            var attachmentNameForRec = new List<string>();
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    loggerModelNew = new LoggerModelNew("", "ProcessEmailQueue", "ProcessEmailQueue", "Process is started for Get transaction id to Send Email and Executing stored procedure EXEC usp_GetandUpdateEmailQueueStatus", "", "", "", "", "API");
                    rsignlog.RSignLogInfo(loggerModelNew);
                    FormattableString sql = $"EXEC usp_GetandUpdateEmailQueueStatus";
                    transId = dbContext.Database.SqlQuery<int?>(sql).FirstOrDefault();
                    if (transId == null || transId <= 0)
                        return false;
                    emailQueueRecipients = dbContext.EmailQueueRecipients.Where(b => b.EmailQueueId == transId && b.Status == 0).ToList();
                    emailQueueAttachment = dbContext.EmailQueueAttachment.Where(e => e.EmailQueueId == transId).ToList();
                    attachmentForRec = new List<byte[]>();
                    attachmentNameForRec = new List<string>();
                    if (emailQueueAttachment.Count > 0)
                    {

                        foreach (EmailQueueAttachment attachment in emailQueueAttachment)
                        {
                            attachmentForRec.Add(attachment.AttachmentData);
                            attachmentNameForRec.Add(attachment.AttachmentName);
                        }
                    }
                    string EmailSentType = string.Empty; int RetryCount = 0;

                    foreach (var recipient in emailQueueRecipients)
                    {
                        var Chilkat = new ChilkatHelper(_appConfiguration);

                        var msgId = Guid.NewGuid();
                        List<ChilkatHelper.EmailLogs> emailLogs = Chilkat.SendMailUsingChilKet(new string[] { recipient.RecipientEmail }, new string[] { recipient.RecipientName },
                            null, null,
                             null, null,
                             recipient.EmailQueue.SenderEmail, recipient.EmailQueue.SenderName,
                           recipient.Subject, recipient.Body,
                             attachmentForRec != null ? attachmentForRec : null, attachmentNameForRec != null ? attachmentNameForRec : null, Convert.ToString(recipient.EmailQueue.EnvelopeCode), GetEmailType(recipient.EmailType), recipient.EmailQueue.SignReqReplyToAddressValue != null ? recipient.EmailQueue.SignReqReplyToAddressValue.Value : 1, msgId);

                        _genericRepository.SaveEmailLogsRecord(emailLogs, out EmailSentType, out RetryCount);
                        _genericRepository.SaveDestinationRecord(new Destination() { MessageId = msgId, EnvelopeCode = recipient.EmailQueue.EnvelopeCode, RecipientEmail = recipient.RecipientEmail, IsSent = true, IsProcessed = false, EmailSentType = EmailSentType, RetryCount = RetryCount });

                        UpdateEmailQueueRecipientStatus(recipient.ID, Constants.EmailQueueRecipientStatus.SuccessFullyProcessed);
                        UpdateEmailQueueStatus(transId.Value, Constants.EmailQueueStatus.PartiallyProcessed);
                    }
                    UpdateEmailQueueStatus(transId.Value, Constants.EmailQueueStatus.SuccessFullyProcessed);

                    loggerModelNew.Message = "Update status as InProgress and get recipient list to send email " + Convert.ToInt32(emailQueueRecipients.Count).ToString();
                    loggerModelNew.EnvelopeId = transId.ToString();
                    rsignlog.RSignLogInfo(loggerModelNew);
                }
            }
            catch (Exception ex)
            {

                loggerModelNew.Message = "Error to get trans ID from DB";
                rsignlog.RSignLogError(loggerModelNew, ex);
                if (transId != null && transId > 0)
                    UpdateEmailQueueStatus(transId.Value, Constants.EmailQueueStatus.ErrorWhileProcessing, ex.Message);
                return false;
            }
            return isCompleted;
        }

        private string GetEmailType(string EmailType)
        {
            switch (EmailType)
            {
                case "ESR":
                    return Constants.String.EmailOperation.Send;
                case "ESPN":
                    return Constants.String.EmailOperation.PasswordToSign;
                case "ESCC":
                    return Constants.String.EmailOperation.SendCC;
                case "DS":
                case "FD":
                    return Constants.String.EmailOperation.SignDocument;
                case "DESR":
                    return Constants.String.EmailOperation.Reject;
                case "RESR":
                    return Constants.String.EmailOperation.Resend;
                case "AESR":
                    return Constants.String.EmailOperation.Accept;
                case "SR":
                    return Constants.String.EmailOperation.SignRecipient;
                case "ESPO":
                case "SCPN":
                    return Constants.String.EmailOperation.PasswordToOpen;
                case "SC":
                    return Constants.String.EmailOperation.SendingConfirmation;
                case "DGESR":
                    return Constants.String.EmailOperation.Delegate;
                default:
                    return Constants.String.EmailOperation.Send;
            }
        }
        private bool UpdateEmailQueueRecipientStatus(int ID, int intStatus)
        {
            loggerModelNew = new LoggerModelNew("", "EmailQueueProcessor", "UpdateEmailQueueRecipientStatus", "Process is started for Update Email Queue Recipient Statuss", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            using (var dbContext = new RSignDbContext(_configuration))
            {
                var emailQueueRecipients = dbContext.EmailQueueRecipients.Where(m => m.ID == ID).FirstOrDefault();
                if (emailQueueRecipients != null)
                {
                    emailQueueRecipients.Status = intStatus;
                    if (intStatus == Constants.EmailQueueRecipientStatus.SuccessFullyProcessed)
                        emailQueueRecipients.Message = "Success";
                    emailQueueRecipients.Server = System.Environment.MachineName;
                    emailQueueRecipients.ModifiedDate = DateTime.Now;
                    emailQueueRecipients.Status = Constants.EmailQueueRecipientStatus.SuccessFullyProcessed;
                    dbContext.SaveChanges();
                }
            }
            return true;
        }
        private bool UpdateEmailQueueStatus(int ID, int intStatus, string errorMessage = "")
        {
            loggerModelNew = new LoggerModelNew("", "EmailQueueProcessor", "UpdateEmailQueueStatus", "Process is started for Update Email Queue Status", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            using (var dbContext = new RSignDbContext(_configuration))
            {
                var emailQueue = dbContext.EmailQueue.Where(m => m.ID == ID).FirstOrDefault();
                if (emailQueue != null)
                {
                    emailQueue.Status = intStatus;
                    if (intStatus == Constants.EmailQueueStatus.PartiallyProcessed)
                        emailQueue.Message = "Success";
                    else if (intStatus == Constants.EmailQueueStatus.ErrorWhileProcessing)
                    {
                        emailQueue.Message = "Error: " + errorMessage;
                        emailQueue.Status = Constants.EmailQueueStatus.PartiallyProcessed;
                    }
                    emailQueue.ProcessedServer = System.Environment.MachineName;
                    emailQueue.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                }
            }
            return true;
        }
        public int CreateUpdateTemplateEmailQueue(EmailQueueData emailQueueData)
        {
            loggerModelNew = new LoggerModelNew("", "EmailQueueProcessor", "CreateUpdateEmailQueue", "Process is started for New Entry in EmailQueue.", emailQueueData.template.ID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    EmailQueue emailQueue = new EmailQueue();
                    emailQueue.CreatedDate = DateTime.Now;
                    emailQueue.ModifiedDate = DateTime.Now;
                    emailQueue.Subject = emailQueueData.EmailSubject;
                    emailQueue.EnvelopeCode = Convert.ToString(emailQueueData.template.ID);
                    emailQueue.EnvelopeId = emailQueueData.template.ID;
                    emailQueue.Status = 0;
                    emailQueue.CreatedServer = Environment.MachineName;
                    emailQueue.ProcessedServer = Environment.MachineName;
                    emailQueue.SignReqReplyToAddressValue = emailQueueData.SignReqReplyToAddressValue;
                    emailQueue.isAttachment = emailQueueData.IsAttachment;
                    emailQueue.SenderEmail = emailQueueData.SenderEmailAddress;
                    emailQueue.SenderName = emailQueueData.SenderName;
                    emailQueue.ReprocessCount = 0;
                    emailQueue.EmailType = emailQueueData.EmailType;
                    emailQueue.Body = emailQueueData.MailMessageBody;
                    _recipientRepository.SaveEmailQueue(emailQueue);                                                    
                    return emailQueue.ID;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error while creating New Entry in EmailQueue.";
                rsignlog.RSignLogError(loggerModelNew, ex);
                return 0;
            }
        }
        public bool CreateUpdateEmailQueueTemplateRecipients(EmailQueueRecipientsData emailQueueRecipientsData)
        {
            loggerModelNew = new LoggerModelNew("", "EmailQueueProcessor", "CreateUpdateEmailQueueRecipients", "Process is started for New Entry in EmailQueueRecipients.",
                emailQueueRecipientsData.emailQueueData.template.ID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {

               using (var dbContext = new RSignDbContext(_configuration))
                {                   
                    EmailQueueRecipients emailQueueRecipients = new EmailQueueRecipients();
                    emailQueueRecipients.EmailQueueId = emailQueueRecipientsData.EmailQueueID;
                    emailQueueRecipients.CreatedDate = DateTime.Now;
                    emailQueueRecipients.ModifiedDate = DateTime.Now;
                    emailQueueRecipients.Subject = emailQueueRecipientsData.emailQueueData.EmailSubject;
                    emailQueueRecipients.Body = emailQueueRecipientsData.emailQueueData.MailMessageBody;
                    emailQueueRecipients.EnvelopeId = emailQueueRecipientsData.emailQueueData.template.ID;
                    emailQueueRecipients.EmailType = emailQueueRecipientsData.emailQueueData.EmailType;
                    emailQueueRecipients.RecipientEmail = emailQueueRecipientsData.RecipientEmail;
                    emailQueueRecipients.RecipientName = emailQueueRecipientsData.RecipientName;
                    emailQueueRecipients.Server = Environment.MachineName;
                    emailQueueRecipients.Status = 0;
                    emailQueueRecipients.ReprocessCount = 0;
                    _recipientRepository.SaveEmailQueueRecipients(emailQueueRecipients);                   
                    return true;
                }
            }
            catch (Exception ex)
            {

                loggerModelNew.Message = "Error while creating New Entry in EmailQueueRecipients.";
                rsignlog.RSignLogError(loggerModelNew, ex);
                return false;
            }

        }
        /// <summary>
        /// Create/Update Entry in EmailQueue
        /// </summary>
        public int CreateUpdateEmailQueue(EmailQueueData emailQueueData)
        {
            loggerModelNew = new LoggerModelNew("", "EmailQueueProcessor", "CreateUpdateEmailQueue", "Process is started for New Entry in EmailQueue.", emailQueueData.envelope.ID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    EmailQueue emailQueue = new EmailQueue();
                    emailQueue.CreatedDate = DateTime.Now;
                    emailQueue.ModifiedDate = DateTime.Now;
                    emailQueue.Subject = emailQueueData.EmailSubject;
                    emailQueue.EnvelopeCode = emailQueueData.envelope.EDisplayCode;
                    emailQueue.EnvelopeId = emailQueueData.envelope.ID;
                    emailQueue.Status = 0;
                    emailQueue.CreatedServer = Environment.MachineName;
                    emailQueue.ProcessedServer = Environment.MachineName;
                    emailQueue.SignReqReplyToAddressValue = emailQueueData.SignReqReplyToAddressValue;
                    emailQueue.isAttachment = emailQueueData.IsAttachment;
                    emailQueue.SenderEmail = emailQueueData.SenderEmailAddress;
                    emailQueue.SenderName = emailQueueData.SenderName;
                    emailQueue.ReprocessCount = 0;
                    emailQueue.EmailType = emailQueueData.EmailType;
                    emailQueue.Body = emailQueueData.MailMessageBody;                   
                    _recipientRepository.SaveEmailQueue(emailQueue);                   
                    return emailQueue.ID;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error while creating New Entry in EmailQueue.";
                rsignlog.RSignLogError(loggerModelNew, ex);
                return 0;
            }
        }
        public bool CreateEmailQueueAttachment(EmailQueueAttachmentData emailQueueAttachmentData, int emailQueueId)
        {
            loggerModelNew = new LoggerModelNew("", "EmailQueueProcessor", "CreateEmailQueueAttachment", "Process is started for Get Email Queue Data", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                if (emailQueueId != 0)
                {
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        EmailQueueAttachment emailQueueAttach = new EmailQueueAttachment();
                        int index = 0;                        
                        foreach (byte[] data in emailQueueAttachmentData.AttachmentData)
                        {
                            emailQueueAttach = new EmailQueueAttachment();
                            emailQueueAttach.CreatedDate = DateTime.Now;
                            emailQueueAttach.EmailQueueId = emailQueueId;
                            emailQueueAttach.AttachmentData = data;
                            emailQueueAttach.AttachmentName = emailQueueAttachmentData.AttachmentNamesInfo[index];
                            _recipientRepository.SaveEmailQueueAttachments(emailQueueAttach);                          
                            index++;
                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error while creating New Entry in EmailQueue.";
                rsignlog.RSignLogError(loggerModelNew, ex);
                return false;
            }
        }

        /// <summary>
        /// Create/Update Entry in EmailQueueRecipients
        /// </summary>
        public bool CreateUpdateEmailQueueRecipients(EmailQueueRecipientsData emailQueueRecipientsData)
        {
            loggerModelNew = new LoggerModelNew("", "EmailQueueProcessor", "CreateUpdateEmailQueueRecipients", "Process is started for New Entry in EmailQueueRecipients.", emailQueueRecipientsData.emailQueueData.envelope.ID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {                   
                    EmailQueueRecipients emailQueueRecipients = new EmailQueueRecipients();
                    emailQueueRecipients.EmailQueueId = emailQueueRecipientsData.EmailQueueID;
                    emailQueueRecipients.CreatedDate = DateTime.Now;
                    emailQueueRecipients.ModifiedDate = DateTime.Now;
                    emailQueueRecipients.Subject = emailQueueRecipientsData.emailQueueData.EmailSubject;
                    emailQueueRecipients.Body = emailQueueRecipientsData.emailQueueData.MailMessageBody;
                    emailQueueRecipients.EnvelopeId = emailQueueRecipientsData.emailQueueData.envelope.ID;
                    emailQueueRecipients.EmailType = emailQueueRecipientsData.emailQueueData.EmailType;
                    emailQueueRecipients.RecipientEmail = emailQueueRecipientsData.RecipientEmail;
                    emailQueueRecipients.RecipientName = emailQueueRecipientsData.RecipientName;
                    emailQueueRecipients.Server = Environment.MachineName;
                    emailQueueRecipients.Status = 0;
                    emailQueueRecipients.ReprocessCount = 0;
                    _recipientRepository.SaveEmailQueueRecipients(emailQueueRecipients);                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error while creating New Entry in EmailQueueRecipients.";
                rsignlog.RSignLogError(loggerModelNew, ex);
                return false;
            }

        }

        /// <summary>
        /// Create/Update Entry in EmailQueueRecipients
        /// </summary>
        public List<EmailQueue> GetEmailQueueData(Guid EnvelopeID, string EmailType)
        {
            loggerModelNew = new LoggerModelNew("", "EmailQueueProcessor", "GetEmailQueueData", "Process is started for Get Email Queue Data", EnvelopeID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return _recipientRepository.GetEmailQueueData(EnvelopeID, EmailType);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error while getting EmailQueueData.";
                rsignlog.RSignLogError(loggerModelNew, ex);
                return null;
            }
        }
      
    }
}
