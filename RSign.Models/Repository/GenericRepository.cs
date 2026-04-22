using Chilkat;
using eSign.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Common.Mailer;
using RSign.Models.APIModels;
using RSign.Models.APIModels.Data;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RSign.Models.Repository
{
    public class GenericRepository : IGenericRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly IConfiguration _appConfiguration;
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        private readonly IDocumentRepository _documentRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IRecipientRepository _recipientRepository;
        private IHttpContextAccessor _accessor;
        public bool webApiInitialiizeEnvelope { get; set; }
        public GenericRepository(IOptions<AppSettingsConfig> configuration, IConfiguration appConfiguration, IDocumentRepository documentRepository, ILookupRepository lookupRepository, IRecipientRepository recipientRepository, IHttpContextAccessor accessor)
        {
            _configuration = configuration;
            _appConfiguration = appConfiguration;
            _documentRepository = documentRepository;
            _lookupRepository = lookupRepository;
            _recipientRepository = recipientRepository;
            rsignlog = new RSignLogger(_appConfiguration);
            _accessor = accessor;
        }
        public Envelope GetEntity(Guid envelopeID, bool isHistoryRequired = true, int IsEnvelopeArichived = 0)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "GenericRepository", "GetEnvelopeEntity", "Get Envelope Entity process is started ", envelopeID.ToString(), "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);
                Envelope envelope = new Envelope();
                using (var dbContext = new RSignDbContext(_configuration))
                {

                    if (IsEnvelopeArichived != 1)
                    {
                        envelope = dbContext.Envelope.Where(e => e.ID == envelopeID).FirstOrDefault();
                        if (envelope == null && isHistoryRequired)
                        {
                            return GetEntityHistory(envelopeID);
                        }

                        if (envelope != null)
                        {
                            var Recipients = (from r in dbContext.Recipients
                                              where r.EnvelopeID == envelope.ID
                                              select r).ToList();

                            foreach (var recipient in Recipients)
                            {
                                envelope.Recipients.Add(recipient);
                            }

                            var documents = (from d in dbContext.Documents
                                             where d.EnvelopeID == envelope.ID
                                             select d).ToList();

                            foreach (var document in envelope.Documents)
                            {
                                //document.DocumentContents = dbContext.DocumentContents.Include(c => c.SelectControlOptions.Where(c => c.DocumentContentID == document.ID)).Where(dc => dc.DocumentID == document.ID).ToList();

                                document.DocumentContents = dbContext.DocumentContents.Where(dc => dc.DocumentID == document.ID).ToList();
                                foreach (var item in document.DocumentContents)
                                {
                                    item.ControlStyle = dbContext.ControlStyle.Where(dc => dc.DocumentContentId == item.ID).FirstOrDefault();
                                    item.SelectControlOptions = dbContext.SelectControlOptions.Where(dc => dc.DocumentContentID == item.ID).ToList();
                                    item.Documents = document;
                                }
                                envelope.Documents.Add(document);
                            }
                        }
                    }
                    else if (IsEnvelopeArichived == 1)
                    {
                        return this.GetArchiveEnvelopesData(envelopeID);
                    }
                    return envelope;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Envelope GetEntity(Guid envelopeID)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "EnvelopeRepository", "GetEntity", "Get Envelope Entity process is started ", envelopeID.ToString(), "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);

                return GetEntity(envelopeID, true, 0);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Envelope GetEnvelopeRecipients(Guid envelopeID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Envelope envelope = dbContext.Envelope.Where(e => e.ID == envelopeID).FirstOrDefault();
                    if (envelope == null)
                        return GetEntityHistory(envelopeID);

                    var Recipients = (from r in dbContext.Recipients
                                      where r.EnvelopeID == envelope.ID
                                      select r).ToList();

                    foreach (var recipient in Recipients)
                    {
                        envelope.Recipients.Add(recipient);
                    }
                    return envelope;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Envelope GetEntityHistory(Guid envelopeID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeDetails = dbContext.EnvelopeHistory.Where(e => e.ID == envelopeID).FirstOrDefault();
                    Envelope envelope = new Envelope();
                    if (envelopeDetails == null)
                        return envelope;
                    envelope.ID = envelopeDetails.ID;
                    envelope.UserID = envelopeDetails.UserID;
                    envelope.EDisplayCode = envelopeDetails.EDisplayCode;
                    envelope.DateFormatID = envelopeDetails.DateFormatID;
                    envelope.ExpiryTypeID = envelopeDetails.ExpiryTypeID;
                    envelope.ReminderDays = envelopeDetails.ReminderDays;
                    envelope.ReminderRepeatDays = envelopeDetails.ReminderRepeatDays;
                    envelope.PasswordReqdtoSign = envelopeDetails.PasswordReqdtoSign;
                    envelope.PasswordReqdtoOpen = envelopeDetails.PasswordReqdtoOpen;
                    envelope.PasswordtoSign = envelopeDetails.PasswordtoSign;
                    envelope.PasswordtoOpen = envelopeDetails.PasswordtoOpen;
                    envelope.PasswordKeySize = envelopeDetails.PasswordKeySize;
                    envelope.PasswordKey = envelopeDetails.PasswordKey;
                    envelope.SigningCertificateName = envelopeDetails.SigningCertificateName;
                    envelope.StatusID = envelopeDetails.StatusID;
                    envelope.CreatedDateTime = envelopeDetails.CreatedDateTime;
                    envelope.ModifiedDateTime = envelopeDetails.ModifiedDateTime;
                    envelope.Subject = envelopeDetails.Subject;
                    envelope.Message = envelopeDetails.Message;
                    envelope.Location = envelopeDetails.Location;
                    envelope.DocumentHash = envelopeDetails.DocumentHash;
                    envelope.IsActive = envelopeDetails.IsActive;
                    envelope.TemplateCode = envelopeDetails.TemplateCode;
                    envelope.IsEnvelope = envelopeDetails.IsEnvelope;
                    envelope.TemplateName = envelopeDetails.TemplateName;
                    envelope.TemplateDescription = envelopeDetails.TemplateDescription;
                    envelope.IsTemplateDeleted = envelopeDetails.IsTemplateDeleted;
                    envelope.IsTemplateEditable = envelopeDetails.IsTemplateEditable;
                    envelope.EDisplayCode = envelopeDetails.EDisplayCode;
                    envelope.IsDraft = envelopeDetails.IsDraft;
                    envelope.IsDraftSend = envelopeDetails.IsDraftSend;
                    envelope.IsDraftDeleted = envelopeDetails.IsDraftDeleted;
                    envelope.CultureInfo = envelopeDetails.CultureInfo;
                    envelope.IsFinalCertificateReq = envelopeDetails.IsFinalDocLinkReq;
                    envelope.IsFinalDocLinkReq = envelopeDetails.IsFinalDocLinkReq;
                    envelope.IsEnvelopePrepare = envelopeDetails.IsEnvelopePrepare;
                    envelope.IsEnvelopeComplete = envelopeDetails.IsEnvelopeComplete;
                    envelope.IsSequenceCheck = envelopeDetails.IsSequenceCheck;
                    envelope.IsTemplateShared = envelopeDetails.IsTemplateShared;
                    envelope.IsTransparencyDocReq = envelopeDetails.IsTransparencyDocReq;
                    envelope.EnvelopeTypeId = envelopeDetails.EnvelopeTypeId;
                    envelope.IsSignerAttachFileReq = envelopeDetails.IsSignerAttachFileReq;
                    envelope.IsStatic = envelopeDetails.IsStatic;
                    envelope.IsAttachXML = envelopeDetails.IsAttachXML;
                    envelope.IsSeparateMultipleDocumentsAfterSigningRequired = envelopeDetails.IsSeparateMultipleDocumentsAfterSigningRequired;
                    envelope.IsWaterMark = envelopeDetails.IsWaterMark;
                    envelope.WatermarkTextForSender = envelopeDetails.WatermarkTextForSender;
                    envelope.WatermarkTextForOther = envelopeDetails.WatermarkTextForOther;
                    envelope.AccessAuthType = envelopeDetails.AccessAuthType;
                    envelope.IsRandomPassword = envelopeDetails.IsRandomPassword;
                    envelope.IsPasswordMailToSigner = envelopeDetails.IsPasswordMailToSigner;
                    envelope.IsEdited = Convert.ToBoolean(envelopeDetails.IsEdited);
                    envelope.IsEmailBodyDisplay = envelopeDetails.IsEmailBodyDisplay;
                    envelope.PostSigningLandingPage = envelopeDetails.PostSigningLandingPage;
                    envelope.ReminderTypeID = envelopeDetails.ReminderTypeID;
                    envelope.ThenReminderTypeID = envelopeDetails.ThenReminderTypeID;
                    envelope.UserSignatureTextID = envelopeDetails.UserSignatureTextID;
                    envelope.MessageTemplateTextID = envelopeDetails.MessageTemplateTextID;
                    envelope.SendIndividualSignatureNotifications = envelopeDetails.SendIndividualSignatureNotifications;
                    envelope.FinalReminderDays = envelopeDetails.FinalReminderDays;
                    envelope.FinalReminderTypeID = envelopeDetails.FinalReminderTypeID;
                    envelope.IsDefaultSignatureForStaticTemplate = envelopeDetails.IsDefaultSignatureForStaticTemplate;
                    envelope.IsEnvelopeHistory = true;
                    envelope.ExpiryDate = envelopeDetails.ExpiryDate;
                    envelope.IsPrivateMode = envelopeDetails.IsPrivateMode;
                    envelope.IsStoreOriginalDocument = envelopeDetails.IsStoreOriginalDocument;
                    envelope.IsStoreSignatureCertificate = envelopeDetails.IsStoreSignatureCertificate;
                    envelope.IsRule = envelopeDetails.IsRule;
                    envelope.IsAdditionalAttmReq = envelopeDetails.IsAdditionalAttmReq;

                    foreach (var es in dbContext.EnvelopeContentHistory.Where(r => r.EnvelopeID == envelopeID))
                    {
                        envelope.EnvelopeContent.Add(new EnvelopeContent
                        {
                            ID = es.ID,
                            EnvelopeID = es.EnvelopeID,
                            ContentXML = es.ContentXML
                        });
                    }

                    foreach (var rec in dbContext.RecipientsHistory.Where(r => r.EnvelopeID == envelopeID))
                    {
                        envelope.Recipients.Add(new Recipients
                        {
                            ID = rec.ID,
                            EnvelopeID = rec.EnvelopeID,
                            Name = rec.Name,
                            EmailAddress = rec.EmailAddress,
                            RecipientTypeID = rec.RecipientTypeID,
                            Order = rec.Order,
                            CopyEmailAddress = rec.CopyEmailAddress,
                            IsReviewed = rec.IsReviewed
                        });
                    }
                    foreach (var doc in dbContext.DocumentsHistory.Where(d => d.EnvelopeID == envelopeID))
                    {
                        envelope.Documents.Add(new Documents
                        {
                            ID = doc.ID,
                            EnvelopeID = doc.EnvelopeID,
                            DocumentName = doc.DocumentName,
                            UploadedDateTime = doc.UploadedDateTime,
                            Order = doc.Order,
                            DocumentSource = doc.DocumentSource,
                            ActionType = doc.ActionType
                        });
                    }
                    return envelope;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string GetUniqueKey(string keyName, string languageCode)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.vw_LanguageKeyMapping.Where(km => km.KeyName.ToLower() == keyName.ToLower() && km.LanguageCode == languageCode)
                    .Select(km => km.KeyValue).FirstOrDefault();
            }
        }
        public string GetDocumentFolderPath()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.ApplicationSetting.First(x => x.Parameter.Equals(Constants.String.DocumentFolderPath)).Value;
            }
        }
        public string GenerateRandomVerificationCode()
        {
            try
            {
                Random generator = new Random();
                return generator.Next(0, 1000000).ToString("D6");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public string GetExpirySoonInDays()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.ApplicationSetting.First(x => x.Parameter.Equals(Constants.String.ExpirySoonInDays)).Value;
            }
        }
        public string GetNewMailTemplate(string mailTemplateName, string cultureInfo, string emailDisclaimer, int DisclaimerLocationId = 3, string type = "")
        {
            var mailTemplate = string.Empty;
            using (var dbContext = new RSignDbContext(_configuration))
            {
                if (string.IsNullOrEmpty(type))
                {
                    mailTemplate = dbContext.MailTemplateNew.Where(t => t.Name == mailTemplateName && t.CultureInfo == cultureInfo).Select(t => t.Template).SingleOrDefault();
                    return GetAllMailTemplate(mailTemplateName, cultureInfo, emailDisclaimer, mailTemplate, DisclaimerLocationId, type);
                }
                else
                {
                    return dbContext.MailTemplateNew.Where(t => t.Name == mailTemplateName && t.CultureInfo == cultureInfo).Select(t => t.MobileTemplate).SingleOrDefault();
                }              
            }
        }
        public string GetAllMailTemplate(string mailTemplateName, string cultureInfo, string emailDisclaimer, string mailTemplate, int DisclaimerLocationId = 3, string type = "")
        {
            if (string.IsNullOrEmpty(type))
            {
                if (mailTemplateName == Constants.String.MailTemplateName.SendEnvelope || mailTemplateName == Constants.String.MailTemplateName.DelegatedTo || mailTemplateName == Constants.String.MailTemplateName.AuthenticateSigner || mailTemplateName == Constants.String.MailTemplateName.CCRecipient || mailTemplateName == Constants.String.MailTemplateName.SendingConfirmation
                                      || mailTemplateName == Constants.String.MailTemplateName.ReminderTemplate || mailTemplateName == Constants.String.MailTemplateName.ReminderTemplateOld || mailTemplateName == Constants.String.MailTemplateName.FinishLaterReminderTemplate)
                {
                    if (DisclaimerLocationId == Constants.DisclaimerLocation.TopOfTheEmailBody)
                    {
                        mailTemplate = mailTemplate.Replace("#DisclaimerLocationTopofTheEmailBody", "<div style='margin-top: 0; margin-bottom: 10px; font-family: calibri,sans-serif; font-size:11pt; color:#000;' align='left;'>" + emailDisclaimer + "</div>");
                        mailTemplate = mailTemplate.Replace("#DisclaimerLocationBelowEmailBody", string.Empty);
                    }
                    else if (DisclaimerLocationId == Constants.DisclaimerLocation.BelowEmailBody)
                    {
                        mailTemplate = mailTemplate.Replace("#DisclaimerLocationTopofTheEmailBody", string.Empty);
                        mailTemplate = mailTemplate.Replace("#DisclaimerLocationBelowEmailBody", "<div style='margin-top: 10px; margin-bottom: 0; font-family: calibri,sans-serif; font-size:11pt; color:#000;' align='left;'>" + emailDisclaimer + "</div>");
                    }
                    else if (DisclaimerLocationId == Constants.DisclaimerLocation.BottomOfEmail)
                    {
                        mailTemplate = mailTemplate.Replace("#DisclaimerLocationTopofTheEmailBody", string.Empty);
                        mailTemplate = mailTemplate.Replace("#DisclaimerLocationBelowEmailBody", string.Empty);
                        mailTemplate = mailTemplate + "</br>" + Convert.ToString(emailDisclaimer);
                    }
                }
                else
                {
                    mailTemplate = mailTemplate + "</br>" + Convert.ToString(emailDisclaimer);
                }
            }

            return mailTemplate;
        }
        public string CreateVerificationCodeMailTemplate(Envelope envelope, string imageLogoURl, string fromEmailId, string userName, Recipients signer, string firstName, string mailTemplate, string signerEmail = "", string SigneruserName = "", string VerificationCode = "")
        {
            mailTemplate = mailTemplate.Replace("#SenderName", userName);
            mailTemplate = mailTemplate.Replace("#SenderEmailId", fromEmailId);
            mailTemplate = mailTemplate.Replace("#RecipientName", firstName);
            mailTemplate = mailTemplate.Replace("#SignerName", string.IsNullOrEmpty(SigneruserName) ? signer.Name : SigneruserName);
            //mailTemplate = mailTemplate.Replace("#SignerEmailId", string.IsNullOrEmpty(signerEmail) ? signer.EmailAddress : signerEmail);
            //mailTemplate = mailTemplate.Replace("#RecipientEmailId",);

            string emailMobileNumberDetails = AppendSignerEmailMobileDetails(signer.DeliveryMode, signer.EmailAddress, signer.DialCode, signer.Mobile);
            mailTemplate = mailTemplate.Replace("#SignerEmailId", emailMobileNumberDetails);


            mailTemplate = mailTemplate.Replace("#EnvelopeDisplayCode", Convert.ToString(envelope.EDisplayCode));
            mailTemplate = mailTemplate.Replace("#VerificationCode", VerificationCode);
            mailTemplate = mailTemplate.Replace("#clientLogo", string.Empty);
            return mailTemplate;
        }
        public string AppendFooterText(string mailTemplate, string toAddress, string mailTemplateName, string cultureInfo, string footerType = "FinalContractFooter", string type = "replace")
        {
            loggerModelNew = new LoggerModelNew(toAddress, "Generic Repository", "AppendFooterText", "Process is started for Get User footer mail template", "", "", "", "", "AppendFooterText");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                bool isDomainExists = false;

                if (File.Exists(Convert.ToString(_appConfiguration["DomainsForAlternativeMTA"])))
                {
                    string[] domainsForAlternativeMTA = File.ReadAllLines(Convert.ToString(_appConfiguration["DomainsForAlternativeMTA"]));

                    if (!string.IsNullOrEmpty(toAddress) && toAddress.LastIndexOf("@") > 0)
                    {
                        if (domainsForAlternativeMTA.Any(s => !string.IsNullOrEmpty(s.Trim()) && (s.ToUpper() == "ANY" || toAddress.Substring(toAddress.LastIndexOf("@") + 1).Contains(s))))
                        {
                            isDomainExists = true;
                        }
                    }
                }

                if (type == "replace")
                {
                    var mailFooterTemplate = string.Empty;
                    if (mailTemplateName == Constants.String.MailTemplateName.SendEnvelope || mailTemplateName == Constants.String.MailTemplateName.DelegatedTo
                        || mailTemplateName == Constants.String.MailTemplateName.ReminderTemplateOld || mailTemplateName == Constants.String.MailTemplateName.ReminderTemplate)
                    {
                        footerType = isDomainExists ? Constants.String.MailTemplateName.SigningFooterEmpty : Constants.String.MailTemplateName.SigningFooter;
                    }
                    else
                    {
                        footerType = isDomainExists ? Constants.String.MailTemplateName.FinalContractFooterEmpty : Constants.String.MailTemplateName.FinalContractFooter;
                    }
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        mailFooterTemplate = dbContext.MailTemplateNew.Where(t => t.Name == footerType && t.CultureInfo == cultureInfo).Select(t => t.Template).SingleOrDefault();
                    }
                    mailTemplate = mailTemplate.Replace("#FooterText", mailFooterTemplate);
                }

                if (type == "append" && !isDomainExists)
                {
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        mailTemplate += dbContext.MailTemplateNew.Where(t => t.Name == mailTemplateName && t.CultureInfo == cultureInfo).Select(t => t.Template).SingleOrDefault();
                    }
                }
                else if (type == "append" && isDomainExists)
                {
                    mailTemplate = mailTemplate;
                }

                return mailTemplate;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.Message;
                rsignlog.RSignLogInfo(loggerModelNew);
                return mailTemplate;
            }
        }
        public void SaveEmailLogsRecord(List<ChilkatHelper.EmailLogs> emailLogs, out string EmailSentType, out int RetryCount)
        {
            string emailSentType = string.Empty; int retryCount = 0;
            try
            {
                loggerModelNew = new LoggerModelNew("", "Generic Repository", "SaveEmailLogsRecord", "Save EmailLogs Record process started", "", "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);
                List<EmailLogs> emailLogsModel = new List<EmailLogs>();
                foreach (var item in emailLogs)
                {
                    if (item.ResponseMessage == "Failed")
                    {
                        var emailLog = new EmailLogs();
                        emailLog.MessageId = item.MessageId;
                        emailLog.EnvelopeCode = item.EnvelopeCode;
                        emailLog.InnerException = item.InnerException;
                        emailLog.FullException = item.FullException;
                        emailLog.ResponseMessage = "Failed";
                        emailLog.ServerName = item.ServerName;
                        emailLog.CreatedDate = DateTime.Now;
                        emailLogsModel.Add(emailLog);
                        emailSentType = item.EmailSentType;
                        retryCount = emailLogs.Count;
                    }
                    else
                    {
                        emailSentType = item.EmailSentType;
                        retryCount = item.EmailSentType == "SMTP" ? 0 : (item.ResponseMessage != "Failed" && emailLogs.Count == 1) ? 0 : emailLogs.Count;
                    }
                }
                if (emailLogsModel.Count > 0)
                {
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        _documentRepository.SaveEmailLogsRecord(emailLogsModel);
                    }
                }
                EmailSentType = emailSentType;
                RetryCount = retryCount;
            }
            catch (Exception ex)
            {
                EmailSentType = emailSentType;
                RetryCount = retryCount;
                loggerModelNew.Message = "Error while saving EmailLogs Record";
                rsignlog.RSignLogError(loggerModelNew, ex);
            }
        }
        public void SaveDestinationRecord(Destination destinationRecord)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "Generic Repository", "SaveDestinationRecord", "Save Destination Record process started", "", "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);

                using (var dbContext = new RSignDbContext(_configuration))
                {
                    _documentRepository.SaveDestinationRecord(destinationRecord);
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = "Error while saving Destination Record";
                rsignlog.RSignLogError(loggerModelNew, ex);
            }
        }
        public void UpdateEnvelopeCommonWebhookTransaction(Envelope envelopeObject, string EventCode, string SenderEmail, string RecipientId = null, string SignerStatusId = null, string DelegatedMessage = "", string NotificationType = "", string IntegrationType = "")
        {
            loggerModelNew = new LoggerModelNew("", "UpdateEnvelopeCommonWebhookTransaction", "UpdateEnvelopeCommonWebhookTransaction", "Process started for Update Envelope Common Webhook Transaction", envelopeObject.ID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    SqlParameter[] parms = new SqlParameter[11];
                    parms[0] = new SqlParameter("@UserId", envelopeObject.UserID);
                    parms[1] = new SqlParameter("@EventCode", EventCode);
                    parms[2] = new SqlParameter("@EnvelopeId", envelopeObject.ID);
                    if (string.IsNullOrEmpty(RecipientId))
                    {
                        parms[3] = new SqlParameter("@RecipientId", Guid.Empty);
                    }
                    else
                    {
                        parms[3] = new SqlParameter("@RecipientId", new Guid(RecipientId));
                    }
                    if (string.IsNullOrEmpty(SignerStatusId))
                    {
                        parms[4] = new SqlParameter("@SignerStatusId", Guid.Empty);
                    }
                    else
                    {
                        parms[4] = new SqlParameter("@SignerStatusId", new Guid(SignerStatusId));
                    }
                    parms[5] = new SqlParameter("@SenderEmail", SenderEmail);
                    parms[6] = new SqlParameter("@EnvelopeStatusId", envelopeObject.StatusID);
                    parms[7] = new SqlParameter("@EDisplayCode", envelopeObject.EDisplayCode);
                    parms[8] = new SqlParameter("@DelegatedMessage", DelegatedMessage);
                    parms[9] = new SqlParameter("@NotificationType", NotificationType);
                    parms[10] = new SqlParameter("@IntigrationType", string.IsNullOrEmpty(IntegrationType) ? "" : IntegrationType);

                    dbContext.Database.ExecuteSqlRawAsync(@"EXEC usp_InsertUpdateWebhookTransactions @UserId,@EventCode,@EnvelopeId,@RecipientId,@SignerStatusId,@SenderEmail,@EnvelopeStatusId,@EDisplayCode,@DelegatedMessage,@NotificationType,@IntigrationType", parms);

                    if (envelopeObject.ID != null && (envelopeObject.ID != Guid.Empty))
                    {
                        loggerModelNew.Message = "Get envelopeId to process signed Document completed";
                        rsignlog.RSignLogInfo(loggerModelNew);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        public bool UpdateEnvelopePrefillSigner(Guid envelopeID, string draftType)
        {
            var loggerModelNew = new LoggerModelNew("", "Generic Repository", "UpdateEnvelopePrefillSigner", "Process started for Update Envelope Prefill Signer", "", "", "", "", "UpdateEnvelopePrefillSigner");
            rsignlog.RSignLogInfo(loggerModelNew);
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var envelopeDetail = dbContext.Envelope.Where(e => e.ID == envelopeID).SingleOrDefault();
                    envelopeDetail.DraftType = draftType;
                    envelopeDetail.IsDraft = draftType == string.Empty ? false : envelopeDetail.IsDraft;

                    if (dbContext.Entry(envelopeDetail).State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                        dbContext.Entry(envelopeDetail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return false;
            }
        }
        public bool UpdateEnvelopeStatus(Guid envelopeId, Guid envelopeStatusId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Envelope envelope = dbContext.Envelope.Where(e => e.ID == envelopeId).FirstOrDefault();
                    envelope.StatusID = envelopeStatusId;
                    envelope.ModifiedDateTime = DateTime.Now;
                    dbContext.Entry(envelope).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<MailTemplateNew> GetMailTemplateCode()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.MailTemplateNew.ToList();
            }
        }
        public Dictionary<string, string> GetNewMailTemplateList(string mailTemplateName, string[] CultureInfoList, string emailDisclaimer, int DisclaimerLocationId = 3, string type = "")
        {
            Dictionary<string, string> returnMailTemplateAndCultureInfo = new Dictionary<string, string>();
            using (var dbContext = new RSignDbContext(_configuration))
            {
                //var mailTemplate = dbContext.MailTemplateNew.Where(t => CultureInfoList.Contains(t.CultureInfo) && t.Name == mailTemplateName).Select(t => new { t.Template, t.CultureInfo }).ToList();
                //string mainMailTemplate = string.Empty;
                //foreach (var mails in mailTemplate)
                //{
                //    mainMailTemplate = GetAllMailTemplate(mailTemplateName, mails.CultureInfo, emailDisclaimer, mails.Template, DisclaimerLocationId);
                //    returnMailTemplateAndCultureInfo.Add(mails.CultureInfo, mainMailTemplate);
                //}

                if (string.IsNullOrEmpty(type))
                {
                    var mailTemplate = dbContext.MailTemplateNew.Where(t => CultureInfoList.Contains(t.CultureInfo) && t.Name == mailTemplateName).Select(t => new { t.Template, t.CultureInfo }).ToList();
                    string mainMailTemplate = string.Empty;
                    foreach (var mails in mailTemplate)
                    {
                        mainMailTemplate = GetAllMailTemplate(mailTemplateName, mails.CultureInfo, emailDisclaimer, mails.Template, DisclaimerLocationId, "");
                        returnMailTemplateAndCultureInfo.Add(mails.CultureInfo, mainMailTemplate);
                    }
                }
                else if (type == "mobile")
                {
                    var mailTemplate = dbContext.MailTemplateNew.Where(t => CultureInfoList.Contains(t.CultureInfo) && t.Name == mailTemplateName).Select(t => new { t.MobileTemplate, t.CultureInfo }).ToList();
                    string mainMailTemplate = string.Empty;
                    foreach (var mails in mailTemplate)
                    {
                        returnMailTemplateAndCultureInfo.Add(mails.CultureInfo, mails.MobileTemplate);
                    }
                }

                return returnMailTemplateAndCultureInfo;
            }
        }
        public List<LookupItem> GetpageBasedKeys(string pageName, string languageCode)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.vw_LanguageKeyMapping.Where(km => km.PageName.ToLower() == pageName.ToLower() && km.LanguageCode == languageCode)
                .Select(km => new LookupItem { Key = km.KeyName, Value = km.KeyValue, IsActive = true }).ToList();
            }
        }
        public string GetSigningURL(string templateKey, string envelopeID, string recID, string recEmail, string copyEmail = "", bool IsSignerIdentity = false)
        {
            loggerModelNew = new LoggerModelNew("", "GenericRepository", "GetSigningURL", "Process started for getting Signing URL", envelopeID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            // encrypt querystring
            copyEmail = string.IsNullOrEmpty(copyEmail) ? string.Empty : "&SignerType=Copy&CopyMailId=" + copyEmail;
            string strURLWithData = "";
            if (Convert.ToBoolean(IsSignerIdentity))
            {
                strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/authenticate-signer?";
            }
            else
            {
                strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/signer-landing?";
            }

            if (!string.IsNullOrEmpty(templateKey))
                return strURLWithData + HttpUtility.UrlEncode(EncryptQueryString(string.Format("EnvelopeId={0}&RecipientId={1}&TemplateKey={2}&EmailId={3}{4}", envelopeID, recID, templateKey, HttpUtility.UrlEncode(recEmail), HttpUtility.UrlEncode(copyEmail))));
            else
                return strURLWithData + HttpUtility.UrlEncode(EncryptQueryString(string.Format("EnvelopeId={0}&RecipientId={1}&EmailId={2}{3}", envelopeID, recID, HttpUtility.UrlEncode(recEmail), HttpUtility.UrlEncode(copyEmail))));
        }
        public string GetNewSigningURL(string templateKey, string envelopeID, string recID, string recEmail, string copyEmail, string senderEmail, bool? IsSignerIdentitiy, Guid recipientTypeID, bool isNewUrlApplicable)
        {
            loggerModelNew = new LoggerModelNew("", "GenericRepository", "GetNewSigningURL", "Process started for getting Signing URL", envelopeID.ToString(), "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);

            copyEmail = string.IsNullOrEmpty(copyEmail) ? string.Empty : "&SignerType=Copy&CopyMailId=" + copyEmail;
            string strURLWithData = "";
            if (isNewUrlApplicable)
                strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "/signer-landing?";
            else
                strURLWithData = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/Index?";

            if (Convert.ToBoolean(IsSignerIdentitiy) && recipientTypeID != Constants.RecipientType.CC)
            {
                if (isNewUrlApplicable)
                    strURLWithData = Convert.ToString(_appConfiguration["NewSigningURL"]) + "authenticate-signer?";
                else
                    strURLWithData = Convert.ToString(_appConfiguration["SigningURL"]) + "/SignDocument/AuthenticateSigner?";
            }

            if (!string.IsNullOrEmpty(templateKey))
                return strURLWithData = strURLWithData + HttpUtility.UrlEncode(EncryptQueryString(string.Format("EnvelopeId={0}&RecipientId={1}&TemplateKey={2}&EmailId={3}{4}", envelopeID, recID, templateKey, HttpUtility.UrlEncode(recEmail), HttpUtility.UrlEncode(copyEmail))));
            else
                return strURLWithData = strURLWithData + HttpUtility.UrlEncode(EncryptQueryString(string.Format("EnvelopeId={0}&RecipientId={1}&EmailId={2}{3}", envelopeID, recID, HttpUtility.UrlEncode(recEmail), HttpUtility.UrlEncode(copyEmail))));
        }
        public string EncryptQueryString(string strQueryString)
        {
            return EncryptDecryptQueryString.Encrypt(strQueryString, Convert.ToString(_appConfiguration["QueryStringKey"]));
        }
        public Envelope GetEnvelopeById(Guid envelopeID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                Envelope envelope = dbContext.Envelope.Where(e => e.ID == envelopeID).FirstOrDefault();
                if (envelope == null)
                    return GetEntityHistory(envelopeID);
                return envelope;
            }
        }
        public Envelope GetEnvelopeInfo(Guid envelopeID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                Envelope envelope = dbContext.Envelope.Where(e => e.ID == envelopeID).FirstOrDefault();
                return envelope;
            }
        }
        public List<string> GetTemplatDocumentNames(Guid templateID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.TemplateDocuments.Where(d => d.TemplateID == templateID).Select(s => s.DocumentName).ToList();
            }
        }
        public Template GetTemplateEntity(Guid templateID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Template template = dbContext.Template.Where(e => e.ID == templateID).FirstOrDefault();

                    var roles = (from r in dbContext.TemplateRoles
                                 where r.TemplateID == template.ID
                                 select r).ToList();

                    foreach (var role in roles)
                    {
                        template.TemplateRoles.Add(role);
                    }

                    var documents = (from d in dbContext.TemplateDocuments
                                     where d.TemplateID == template.ID
                                     select d).ToList();

                    foreach (var document in template.TemplateDocuments)
                    {
                        document.TemplateDocumentContents = dbContext.TemplateDocumentContents.Where(dc => dc.DocumentID == document.ID).ToList();
                        foreach (var item in document.TemplateDocumentContents)
                        {
                            item.TemplateControlStyle = dbContext.TemplateControlStyle.Where(dc => dc.DocumentContentId == item.ID).FirstOrDefault();
                            item.TemplateSelectControlOptions = dbContext.TemplateSelectControlOptions.Where(dc => dc.DocumentContentID == item.ID).ToList();
                            item.TemplateDocuments = document;
                        }
                        template.TemplateDocuments.Add(document);
                    }

                    return template;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Template GetTemplateDetails(Guid templateID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return (from q in dbContext.Template
                            where q.ID == templateID
                            select q).First();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Template GetStaticTemplateDetails(Guid templateID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return (from q in dbContext.Template
                            where q.ID == templateID
                            select q).First();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [NonAction]
        public int GetMaxEnvelopeCode()
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var EnvelopeCodeMax = dbContext.Envelope.OrderByDescending(x => x.EnvelopeCode).First().EnvelopeCode;
                    return Convert.ToInt32(EnvelopeCodeMax);
                }
            }
            catch (Exception)
            {
                return 1;
            }
        }
        public void SetInitializeEnvelopeFlag()
        {
            webApiInitialiizeEnvelope = true;
        }
        public bool Save(Envelope envelope)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                try
                {
                    if (envelope.IsFinalCertificateReq == null)
                        envelope.IsFinalCertificateReq = true;
                    if (envelope.IsFinalDocLinkReq == null)
                        envelope.IsFinalDocLinkReq = true;
                    var envelopeDetail = dbContext.Envelope.Where(e => e.ID == envelope.ID).SingleOrDefault();
                    const int passwordKeySize = 128;
                    // Generating key
                    string completeEncodedKey = ModelHelper.GenerateKey(passwordKeySize);

                    if (envelopeDetail == null)
                    {
                        envelope.IsEmailBodyDisplay = true;
                        if (envelope.PasswordReqdtoOpen == true)
                        {
                            string userPassword = envelope.PasswordtoOpen;
                            // Encrypt password using key
                            if (string.IsNullOrEmpty(userPassword) == true && envelope.IsDraft == true)
                            {
                            }
                            else if (string.IsNullOrEmpty(userPassword))
                            {
                            }
                            else
                            {
                                envelope.PasswordtoOpen = ModelHelper.Encrypt(userPassword, completeEncodedKey, passwordKeySize);
                                envelope.PasswordKey = completeEncodedKey;
                                envelope.PasswordKeySize = passwordKeySize;
                            }
                        }
                        else
                        {
                            envelope.PasswordtoOpen = null;
                        }
                        if (envelope.PasswordReqdtoSign == true)
                        {
                            string userPassword = envelope.PasswordtoSign;
                            if (string.IsNullOrEmpty(userPassword) == true && envelope.IsDraft == true)
                            {
                            }
                            else if (string.IsNullOrEmpty(userPassword))
                            {
                            }
                            else
                            {
                                envelope.PasswordtoSign = ModelHelper.Encrypt(userPassword, completeEncodedKey, passwordKeySize);
                                envelope.PasswordKey = completeEncodedKey;
                                envelope.PasswordKeySize = passwordKeySize;
                            }
                        }
                        else
                        {
                            envelope.PasswordtoSign = null;
                        }

                        var expirytype = _lookupRepository.GetLookup(Lookup.ExpiryType).Where(e => Guid.Parse(e.Key) == envelope.ExpiryTypeID).FirstOrDefault();

                        DateTime expirydate = DateTime.Now;
                        if (expirytype == null)
                        {
                            expirydate = expirydate.AddDays(6);
                            envelope.ExpiryTypeID = Constants.ExpiryType.One_Weeks;
                            envelope.DateFormatID = Constants.DateFormat.US_mm_dd_yyyy_slash;
                        }
                        else
                        {
                            if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.One_Weeks).ToLower())
                                expirydate = expirydate.AddDays(6);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Two_Weeks).ToLower())
                                expirydate = expirydate.AddDays(13);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Thirty_Days).ToLower())
                                expirydate = expirydate.AddDays(29);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Three_Months).ToLower())
                                expirydate = expirydate.AddDays(89);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Two_Days).ToLower())
                                expirydate = expirydate.AddDays(1);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Three_Days).ToLower())
                                expirydate = expirydate.AddDays(2);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Four_Days).ToLower())
                                expirydate = expirydate.AddDays(3);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Five_Days).ToLower())
                                expirydate = expirydate.AddDays(4);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Six_Days).ToLower())
                                expirydate = expirydate.AddDays(5);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Ten_Days).ToLower())
                                expirydate = expirydate.AddDays(9);
                            else if (expirytype.Key.ToLower() == Convert.ToString(Constants.ExpiryType.Sixty_Days).ToLower())
                                expirydate = expirydate.AddDays(59);
                            //expirydate = expirydate.AddMonths(3);
                        }
                        envelope.ExpiryDate = expirydate;
                        envelope.IsActive = true;
                        envelope.CultureInfo = !string.IsNullOrEmpty(envelope.CultureInfo) ? envelope.CultureInfo : System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
                        envelope.IsEnableAutoFillTextControls = envelope.IsEnableAutoFillTextControls;
                        envelope.IsDisclaimerAccepted = envelope.IsDisclaimerAccepted;
                        envelope.DisclaimerText = envelope.DisclaimerText;
                        envelope.IsDisclaimerInCertificate = envelope.IsDisclaimerInCertificate;
                        envelope.ElectronicSignIndicationOptionID = envelope.ElectronicSignIndicationOptionID;
                        envelope.IsEnvelopeExpirationRemindertoSender = envelope.IsEnvelopeExpirationRemindertoSender == null ? 0 : envelope.IsEnvelopeExpirationRemindertoSender;
                        envelope.ISSendReminderTillExpiration = envelope.ISSendReminderTillExpiration == null ? 0 : envelope.ISSendReminderTillExpiration;
                        envelope.SendReminderTillExpiration = (envelope.SendReminderTillExpiration == null || envelope.SendReminderTillExpiration == "") ? Constants.DropdownFieldKeyType.OneEmailperEnvelope : envelope.SendReminderTillExpiration;
                        envelope.EnvelopeExpirationRemindertoSenderReminderDays = envelope.EnvelopeExpirationRemindertoSenderReminderDays == null ? 0 : envelope.EnvelopeExpirationRemindertoSenderReminderDays;
                        envelope.EnvelopeExpirationRemindertoSender = envelope.EnvelopeExpirationRemindertoSender == null ? Constants.ReminderDropdownOptions.Days.ToString().ToUpper() : envelope.EnvelopeExpirationRemindertoSender.ToUpper();
                        envelope.IsSameRecipientForAllTemplates = envelope.IsSameRecipientForAllTemplates;
                        dbContext.Envelope.Add(envelope);
                        dbContext.SaveChanges();

                        Guid envelopeStatusId = Guid.Parse(_lookupRepository.GetLookup(Lookup.EnvelopeStatus)
                                                    .Where(l => l.Key == Convert.ToString(Constants.StatusCode.Envelope.Waiting_For_Signature))
                                                    .Select(l => l.Key)
                                                    .FirstOrDefault());

                        EnvelopeStatus envelopeStatus = new EnvelopeStatus();
                        envelopeStatus.ID = Guid.NewGuid();
                        envelopeStatus.EnvelopeID = envelope.ID;
                        envelopeStatus.CreatedDateTime = DateTime.Now;
                        envelopeStatus.StatusID = envelopeStatusId;
                        envelopeStatus.IsFinalDocumentsUploaded = false;
                        Save(envelopeStatus);

                        var senderDetail = (from r in envelope.Recipients
                                            where r.RecipientTypeID == Constants.RecipientType.Sender
                                            select r).FirstOrDefault();

                        if (senderDetail != null)
                        {
                            if (webApiInitialiizeEnvelope)
                            {
                                SignerStatus signerStatus = new SignerStatus();
                                signerStatus.ID = Guid.NewGuid();
                                signerStatus.RecipientID = senderDetail.ID;
                                signerStatus.StatusID = Constants.StatusCode.Signer.Sent;
                                signerStatus.CreatedDateTime = DateTime.Now;
                                signerStatus.IPAddress = envelope.IpAddress; //Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                                _recipientRepository.Save(signerStatus);
                            }

                            if (envelope.IpAddress != null && envelope.IpAddress != "")
                            {
                                _recipientRepository.SaveRecipientDetailOnSend(senderDetail.ID, Constants.StatusCode.Signer.Sent, envelope.IpAddress);
                            }
                            else
                            {
                                _recipientRepository.SaveRecipientDetail(senderDetail.ID, Constants.StatusCode.Signer.Sent, envelope.IpAddress);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var TemplateCode = envelope.TemplateCode;
                            if (TemplateCode != 0)
                            {
                                if (envelope.PasswordReqdtoOpen == true)
                                {
                                    string userPassword = envelope.PasswordtoOpen;
                                    // Encrypt password using key
                                    if (string.IsNullOrEmpty(userPassword) == true && envelope.IsDraft == true)
                                    {
                                    }
                                    else if (string.IsNullOrEmpty(userPassword))
                                    {
                                    }
                                    else
                                    {
                                        envelopeDetail.PasswordtoOpen = ModelHelper.Encrypt(userPassword, completeEncodedKey, passwordKeySize);
                                        envelopeDetail.PasswordKey = completeEncodedKey;
                                        envelopeDetail.PasswordKeySize = passwordKeySize;
                                    }
                                }
                                else
                                {
                                    envelopeDetail.PasswordtoOpen = null;
                                }

                                if (envelope.PasswordReqdtoSign == true)
                                {
                                    string userPassword = envelope.PasswordtoSign;
                                    if (string.IsNullOrEmpty(userPassword) == true && envelope.IsDraft == true)
                                    {
                                    }
                                    else if (string.IsNullOrEmpty(userPassword))
                                    {
                                    }
                                    else
                                    {
                                        envelopeDetail.PasswordtoSign = ModelHelper.Encrypt(userPassword, completeEncodedKey, passwordKeySize);
                                        envelopeDetail.PasswordKey = completeEncodedKey;
                                        envelopeDetail.PasswordKeySize = passwordKeySize;
                                    }
                                }
                                else
                                {
                                    envelopeDetail.PasswordtoSign = null;
                                }

                                var expirytype = _lookupRepository.GetLookup(Lookup.ExpiryType).Where(e => Guid.Parse(e.Key) == envelope.ExpiryTypeID).FirstOrDefault();

                                DateTime expirydate = DateTime.Now;
                                if (expirytype == null)
                                {
                                    expirydate = expirydate.AddDays(6);
                                    envelope.ExpiryTypeID = Constants.ExpiryType.One_Weeks;
                                    envelope.DateFormatID = Constants.DateFormat.US_mm_dd_yyyy_slash;
                                }
                                else
                                {
                                    if (expirytype.Value == "7 Days")
                                        expirydate = expirydate.AddDays(6);
                                    else if (expirytype.Value == "14 Days")
                                        expirydate = expirydate.AddDays(13);
                                    else if (expirytype.Value == "30 Days")
                                        expirydate = expirydate.AddDays(29);
                                    else if (expirytype.Value == "3 Months")
                                        expirydate = expirydate.AddDays(89);
                                    else if (expirytype.Value == "2 Days")
                                        expirydate = expirydate.AddDays(1);
                                    else if (expirytype.Value == "3 Days")
                                        expirydate = expirydate.AddDays(2);
                                    else if (expirytype.Value == "4 Days")
                                        expirydate = expirydate.AddDays(3);
                                    else if (expirytype.Value == "5 Days")
                                        expirydate = expirydate.AddDays(4);
                                    else if (expirytype.Value == "6 Days")
                                        expirydate = expirydate.AddDays(5);
                                    else if (expirytype.Value == "60 Days")
                                        expirydate = expirydate.AddDays(59);
                                    else if (expirytype.Value == "10 Days")
                                        expirydate = expirydate.AddDays(9);
                                    //expirydate = expirydate.AddMonths(3);
                                }
                                envelopeDetail.ExpiryDate = expirydate;
                                envelopeDetail.IsSignerIdentitiy = envelope.IsSignerIdentitiy;
                                envelopeDetail.TemplateName = envelope.TemplateName;
                                envelopeDetail.TemplateDescription = envelope.TemplateDescription;
                                envelopeDetail.IsTemplateEditable = envelope.IsTemplateEditable;
                                envelopeDetail.DateFormatID = envelope.DateFormatID;
                                envelopeDetail.ExpiryTypeID = envelope.ExpiryTypeID;
                                envelopeDetail.Message = envelope.Message;
                                envelopeDetail.ReminderDays = envelope.ReminderDays;
                                envelopeDetail.ReminderRepeatDays = envelope.ReminderRepeatDays;
                                envelopeDetail.Subject = envelope.Subject;
                                envelopeDetail.SigningCertificateName = "EsignApplication";
                                envelopeDetail.SignerCount = envelope.SignerCount;
                                envelopeDetail.CreatedDateTime = envelope.StatusID == Constants.StatusCode.Envelope.Waiting_For_Signature ? DateTime.Now : envelope.CreatedDateTime;
                                envelopeDetail.ModifiedDateTime = DateTime.Now;
                                envelopeDetail.DocumentHash = envelope.DocumentHash;
                                envelopeDetail.IsFinalCertificateReq = envelope.IsFinalCertificateReq;
                                envelopeDetail.IsFinalDocLinkReq = envelope.IsFinalDocLinkReq;
                                envelopeDetail.IsSequenceCheck = envelope.IsSequenceCheck;
                                envelopeDetail.IsTransparencyDocReq = envelope.IsTransparencyDocReq;
                                envelopeDetail.IsSignerAttachFileReq = envelope.IsSignerAttachFileReq;
                                envelopeDetail.IsDisclaimerAccepted = envelope.IsDisclaimerAccepted;
                                envelopeDetail.DisclaimerText = envelope.DisclaimerText;
                                envelopeDetail.IsWaterMark = envelope.IsWaterMark;
                                envelopeDetail.WatermarkTextForSender = envelope.WatermarkTextForSender;
                                envelopeDetail.WatermarkTextForOther = envelope.WatermarkTextForOther;
                                envelopeDetail.AccessAuthType = envelope.AccessAuthType;
                                envelopeDetail.IsPasswordMailToSigner = envelope.IsPasswordMailToSigner;
                                envelopeDetail.IsRandomPassword = envelope.IsRandomPassword;
                                envelopeDetail.IsEmailBodyDisplay = true;
                                envelopeDetail.CultureInfo = envelope.CultureInfo;
                                envelopeDetail.ReminderTypeID = envelope.ReminderTypeID;
                                envelopeDetail.ThenReminderTypeID = envelope.ThenReminderTypeID;
                                envelopeDetail.UserSignatureTextID = envelope.UserSignatureTextID;
                                envelopeDetail.MessageTemplateTextID = envelope.MessageTemplateTextID;
                                envelopeDetail.SendIndividualSignatureNotifications = envelope.SendIndividualSignatureNotifications;
                                envelopeDetail.FinalReminderDays = envelope.FinalReminderDays;
                                envelopeDetail.FinalReminderTypeID = envelope.FinalReminderTypeID;
                                envelopeDetail.IsStoreOriginalDocument = envelope.IsStoreOriginalDocument;
                                envelopeDetail.IsStoreSignatureCertificate = envelope.IsStoreSignatureCertificate;
                                envelopeDetail.IsPrivateMode = envelope.IsPrivateMode;
                                envelopeDetail.PostSigningLandingPage = envelope.PostSigningLandingPage;
                                envelopeDetail.IsRule = envelope.IsRule;
                                envelopeDetail.IsAdditionalAttmReq = envelope.IsAdditionalAttmReq;
                                envelopeDetail.IsEnableAutoFillTextControls = (envelope.IsEnableAutoFillTextControls == null || envelope.IsEnableAutoFillTextControls == false) ? false : true;
                                envelopeDetail.IsEnvelopeExpirationRemindertoSender = envelope.IsEnvelopeExpirationRemindertoSender == null ? 0 : envelope.IsEnvelopeExpirationRemindertoSender;
                                envelopeDetail.ISSendReminderTillExpiration = envelope.ISSendReminderTillExpiration == null ? 0 : envelope.ISSendReminderTillExpiration;
                                envelopeDetail.SendReminderTillExpiration = (envelope.SendReminderTillExpiration == null || envelope.SendReminderTillExpiration == "") ? Constants.DropdownFieldKeyType.OneEmailperEnvelope : envelope.SendReminderTillExpiration;
                                envelopeDetail.EnvelopeExpirationRemindertoSenderReminderDays = envelope.EnvelopeExpirationRemindertoSenderReminderDays == null ? 0 : envelope.EnvelopeExpirationRemindertoSenderReminderDays;
                                envelopeDetail.EnvelopeExpirationRemindertoSender = envelope.EnvelopeExpirationRemindertoSender == null ? Constants.ReminderDropdownOptions.Days.ToString().ToUpper() : envelope.EnvelopeExpirationRemindertoSender.ToUpper();
                                envelope.ElectronicSignIndicationOptionID = envelope.ElectronicSignIndicationOptionID;
                                envelopeDetail.EnableCcOptions = envelope.EnableCcOptions;
                                envelopeDetail.IsSameRecipientForAllTemplates = envelope.IsSameRecipientForAllTemplates;
                                envelopeDetail.EnableRecipientLanguage = envelope.EnableRecipientLanguage;
                                envelopeDetail.EnableMessageToMobile = envelope.EnableMessageToMobile;
                                envelopeDetail.ReVerifySignerDocumentSubmit = envelope.ReVerifySignerDocumentSubmit;

                                if (dbContext.Entry(envelopeDetail).State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                                    dbContext.Entry(envelopeDetail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                dbContext.SaveChanges();
                                return true;
                            }

                            var UsageCheck = false; // isTemplateUsage();
                            if (UsageCheck == true || envelope.EnvelopeCode != 0)
                            {
                                int enveCode = envelope.EnvelopeCode.HasValue ? envelope.EnvelopeCode.Value : 0;
                                if (enveCode != 0)
                                {
                                    if (envelope.PasswordReqdtoOpen == true)
                                    {
                                        string userPassword = envelope.PasswordtoOpen;
                                        // Encrypt password using key

                                        if (string.IsNullOrEmpty(userPassword) == true && envelope.IsDraft == true)
                                        {
                                        }
                                        else if (string.IsNullOrEmpty(userPassword))
                                        {
                                        }
                                        else
                                        {
                                            envelopeDetail.PasswordtoOpen = ModelHelper.Encrypt(userPassword, completeEncodedKey, passwordKeySize);
                                            envelopeDetail.PasswordKey = completeEncodedKey;
                                            envelopeDetail.PasswordKeySize = passwordKeySize;
                                        }
                                    }
                                    else
                                    {
                                        envelopeDetail.PasswordtoOpen = null;
                                    }

                                    if (envelope.PasswordReqdtoSign == true)
                                    {
                                        string userPassword = envelope.PasswordtoSign;
                                        if (string.IsNullOrEmpty(userPassword) == true && envelope.IsDraft == true)
                                        {
                                        }
                                        else if (string.IsNullOrEmpty(userPassword))
                                        {
                                        }
                                        else
                                        {
                                            envelopeDetail.PasswordtoSign = ModelHelper.Encrypt(userPassword, completeEncodedKey, passwordKeySize);
                                            envelopeDetail.PasswordKey = completeEncodedKey;
                                            envelopeDetail.PasswordKeySize = passwordKeySize;
                                        }
                                    }
                                    else
                                    {
                                        envelopeDetail.PasswordtoSign = null;
                                    }
                                }

                                var expirytype = _lookupRepository.GetLookup(Lookup.ExpiryType).Where(e => Guid.Parse(e.Key) == envelope.ExpiryTypeID).FirstOrDefault();

                                DateTime expirydate = DateTime.Now;
                                if (expirytype == null)
                                {
                                    expirydate = expirydate.AddDays(6);
                                    envelope.ExpiryTypeID = Constants.ExpiryType.One_Weeks;
                                    envelope.DateFormatID = Constants.DateFormat.US_mm_dd_yyyy_slash;
                                }
                                else
                                {
                                    if (expirytype.Value == "7 Days")
                                        expirydate = expirydate.AddDays(6);
                                    else if (expirytype.Value == "14 Days")
                                        expirydate = expirydate.AddDays(13);
                                    else if (expirytype.Value == "30 Days")
                                        expirydate = expirydate.AddDays(29);
                                    else if (expirytype.Value == "3 Months")
                                        expirydate = expirydate.AddDays(89);
                                    else if (expirytype.Value == "2 Days")
                                        expirydate = expirydate.AddDays(1);
                                    else if (expirytype.Value == "3 Days")
                                        expirydate = expirydate.AddDays(2);
                                    else if (expirytype.Value == "4 Days")
                                        expirydate = expirydate.AddDays(3);
                                    else if (expirytype.Value == "5 Days")
                                        expirydate = expirydate.AddDays(4);
                                    else if (expirytype.Value == "6 Days")
                                        expirydate = expirydate.AddDays(5);
                                    else if (expirytype.Value == "60 Days")
                                        expirydate = expirydate.AddDays(59);
                                    else if (expirytype.Value == "10 Days")
                                        expirydate = expirydate.AddDays(9);
                                    //expirydate = expirydate.AddMonths(3);
                                }
                                envelopeDetail.ExpiryDate = expirydate;
                                envelopeDetail.IsEmailBodyDisplay = true;
                                envelopeDetail.TemplateName = envelope.TemplateName;
                                envelopeDetail.TemplateDescription = envelope.TemplateDescription;
                                envelopeDetail.IsTemplateEditable = envelope.IsTemplateEditable;
                                envelopeDetail.DateFormatID = envelope.DateFormatID;
                                envelopeDetail.ExpiryTypeID = envelope.ExpiryTypeID;
                                envelopeDetail.Message = envelope.Message;
                                envelopeDetail.ModifiedDateTime = DateTime.Now;
                                envelopeDetail.ReminderDays = envelope.ReminderDays;
                                envelopeDetail.ReminderRepeatDays = envelope.ReminderRepeatDays;
                                envelopeDetail.Subject = envelope.Subject;
                                envelopeDetail.SigningCertificateName = "EsignApplication";
                                envelopeDetail.SignerCount = envelope.SignerCount;
                                envelopeDetail.ModifiedDateTime = DateTime.Now;
                                envelopeDetail.DocumentHash = envelope.DocumentHash;
                                envelopeDetail.CreatedDateTime = envelope.StatusID == Constants.StatusCode.Envelope.Waiting_For_Signature ? envelope.IsEdited != true ? DateTime.Now : envelope.CreatedDateTime : envelope.CreatedDateTime;
                                envelopeDetail.IsFinalCertificateReq = envelope.IsFinalCertificateReq;
                                envelopeDetail.IsFinalDocLinkReq = envelope.IsFinalDocLinkReq;
                                envelopeDetail.IsSequenceCheck = envelope.IsSequenceCheck;
                                envelopeDetail.IsTransparencyDocReq = envelope.IsTransparencyDocReq;
                                envelopeDetail.IsSignerAttachFileReq = envelope.IsSignerAttachFileReq;
                                envelopeDetail.DisclaimerText = envelope.DisclaimerText;
                                envelopeDetail.IsWaterMark = envelope.IsWaterMark;
                                envelopeDetail.WatermarkTextForSender = envelope.WatermarkTextForSender;
                                envelopeDetail.WatermarkTextForOther = envelope.WatermarkTextForOther;
                                envelopeDetail.AccessAuthType = envelope.AccessAuthType;
                                envelopeDetail.IsPasswordMailToSigner = envelope.IsPasswordMailToSigner;
                                envelopeDetail.IsRandomPassword = envelope.IsRandomPassword;
                                envelopeDetail.IsEmailBodyDisplay = true;
                                envelopeDetail.CultureInfo = envelope.CultureInfo;
                                envelopeDetail.ReminderTypeID = envelope.ReminderTypeID;
                                envelopeDetail.ThenReminderTypeID = envelope.ThenReminderTypeID;
                                envelopeDetail.UserSignatureTextID = envelope.UserSignatureTextID;
                                envelopeDetail.MessageTemplateTextID = envelope.MessageTemplateTextID;
                                envelopeDetail.SendIndividualSignatureNotifications = envelope.SendIndividualSignatureNotifications;
                                envelopeDetail.FinalReminderDays = envelope.FinalReminderDays;
                                envelopeDetail.FinalReminderTypeID = envelope.FinalReminderTypeID;
                                envelopeDetail.ReferenceCode = envelope.ReferenceCode;
                                envelopeDetail.ReferenceEmail = envelope.ReferenceEmail;
                                envelopeDetail.IsStoreOriginalDocument = envelope.IsStoreOriginalDocument;
                                envelopeDetail.IsStoreSignatureCertificate = envelope.IsStoreSignatureCertificate;
                                envelopeDetail.IsPrivateMode = envelope.IsPrivateMode;
                                envelopeDetail.PostSigningLandingPage = envelope.PostSigningLandingPage;
                                envelopeDetail.IsAdditionalAttmReq = envelope.IsAdditionalAttmReq;
                                envelopeDetail.IsRule = envelope.IsRule;
                                envelopeDetail.IsEnableAutoFillTextControls = (envelope.IsEnableAutoFillTextControls == null || envelope.IsEnableAutoFillTextControls == false) ? false : true;
                                envelopeDetail.IsSignerIdentitiy = envelope.IsSignerIdentitiy;
                                envelopeDetail.IsSameRecipientForAllTemplates = envelope.IsSameRecipientForAllTemplates;
                                envelopeDetail.ReVerifySignerDocumentSubmit = envelope.ReVerifySignerDocumentSubmit;
                                if (dbContext.Entry(envelopeDetail).State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                                    dbContext.Entry(envelopeDetail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                dbContext.SaveChanges();
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    dbContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool Save(EnvelopeStatus envelopeStatus)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    dbContext.EnvelopeStatus.Add(envelopeStatus);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Envelope GetEnvelopeDisClaimerText(Guid envelopeID)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "GenericRepository", "GetEnvelopeDisClaimerText", "Get Envelope Entity process is started ", envelopeID.ToString(), "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);

                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Envelope envelope = dbContext.Envelope.Where(e => e.ID == envelopeID).FirstOrDefault();
                    if (envelope == null)
                        return GetEntityHistory(envelopeID);

                    return envelope;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public LanguageKeyTranslationsModel GetLanguageKeyTranslations(TranslationsModel translationsModel)
        {
            loggerModelNew = new LoggerModelNew("", "SignDocumentController", "GetLanguageTranslations", "Process started for Get Language Translations", "", "", "", "", "API");
            rsignlog.RSignLogInfo(loggerModelNew);
            string cultureInfo = "";
            LanguageKeyTranslationsModel responseMessage = new LanguageKeyTranslationsModel();
            try
            {
                Guid envelopeId = Guid.Empty;
                if (!string.IsNullOrEmpty(translationsModel.EnvelopeId))
                {
                    envelopeId = new Guid(translationsModel.EnvelopeId);
                }

                Guid recipientId = Guid.Empty;
                if (!string.IsNullOrEmpty(translationsModel.RecipientId))
                {
                    recipientId = new Guid(translationsModel.RecipientId);
                    var recipient = _recipientRepository.GetEntity(recipientId);
                    if (recipient != null)
                    {
                        cultureInfo = recipient.CultureInfo;
                    }
                }

                if (string.IsNullOrEmpty(cultureInfo) && !string.IsNullOrEmpty(translationsModel.EnvelopeId))
                {
                    var envelopeObject = GetEnvelopeInfo(envelopeId);
                    if (envelopeObject == null)
                    {
                        var templateObject = GetStaticTemplateDetails(envelopeId);
                        if (templateObject != null)
                        {
                            cultureInfo = templateObject.CultureInfo;
                        }
                    }
                    else
                    {
                        cultureInfo = envelopeObject.CultureInfo;
                    }
                }

                if (string.IsNullOrEmpty(cultureInfo) && !string.IsNullOrEmpty(translationsModel.CultureInfo))
                {
                    cultureInfo = translationsModel.CultureInfo;
                }
                responseMessage.Language = _lookupRepository.GetLanguageKeyDetailsFromJson(Convert.ToString(cultureInfo) == string.Empty ? "en-us" : cultureInfo.ToLower());
                responseMessage.LanguageValidation = _lookupRepository.GetLanguageKeyNameDesc(Convert.ToString(cultureInfo) == string.Empty ? "en-us" : cultureInfo.ToLower());

                return responseMessage;
            }
            catch (Exception)
            {
                return responseMessage;
            }
        }
        public Envelope GetArchiveEnvelopesData(Guid EnvelopeId, string EDisplayCode = "")
        {
            string currentMethod = "GetArchiveEnvelopesData";
            Envelope mngEnvlop = new Envelope();
            try
            {
                loggerModelNew = new LoggerModelNew("", "GetArchiveEnvelopesData", currentMethod, "Initiate process to get envelope data from archive db", Convert.ToString(EnvelopeId), "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);

                #region commented not supporting in core
                //SqlConnection connection = new SqlConnection(Convert.ToString(_configuration.Value.ConnectionStrings.RSignContext));
                //SqlCommand cmd = new SqlCommand();
                //SqlDataReader reader;
                //cmd.CommandText = Convert.ToString("USP_GetDataFromArchiveForCopyEnvelope");
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Add("@EnvelopeId", SqlDbType.UniqueIdentifier).Value = EnvelopeId;
                //cmd.Parameters.Add("@EDisplayCode", SqlDbType.NVarChar).Value = EDisplayCode;
                //cmd.Connection = connection;
                //cmd.CommandTimeout = 3600;
                //connection.Open();

                //reader = cmd.ExecuteReader();

                //List<Envelope> _listArichveEnvelope = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Envelope>(reader).ToList();
                //reader.NextResult();
                //mngEnvlop = _listArichveEnvelope[0];
                //mngEnvlop.Recipients = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Recipients>(reader).ToList();
                //reader.NextResult();
                //mngEnvlop.Documents = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Documents>(reader).ToList();
                //reader.NextResult();

                //connection.Close();
                #endregion commented not supporting in core

                SqlConnection connection = new SqlConnection(Convert.ToString(_configuration.Value.ConnectionStrings.RSignContext));
                DataSet dataset = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand("USP_GetDataFromArchiveForCopyEnvelope", connection);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Parameters.AddWithValue("@EnvelopeId", EnvelopeId);
                adapter.SelectCommand.Parameters.AddWithValue("@EDisplayCode", EDisplayCode);
                adapter.Fill(dataset);

                if (dataset != null && dataset.Tables.Count == 3)
                {
                    List<Envelope> _listArichveEnvelope = JsonConvert.DeserializeObject<List<Envelope>>(JsonConvert.SerializeObject(dataset.Tables[0]));
                    mngEnvlop = _listArichveEnvelope[0];
                    mngEnvlop.Recipients = JsonConvert.DeserializeObject<List<Recipients>>(JsonConvert.SerializeObject(dataset.Tables[1]));
                    mngEnvlop.Documents = JsonConvert.DeserializeObject<List<Documents>>(JsonConvert.SerializeObject(dataset.Tables[2]));
                }
                return mngEnvlop;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogWarn(loggerModelNew);
                return mngEnvlop;
            }
        }
        public async Task<int?> CheckEnvelopeFromArchiveDatabase(string EDisplayCode, Guid EnvelopeId, string securityCode = "")
        {
            ArichiveEnvelopesInfo envelopesInfo = GetArchivedEnvelope(EDisplayCode, EnvelopeId, Guid.Empty, securityCode);
            if (envelopesInfo != null && envelopesInfo.envelope != null)
            {
                return 1;// envelopesInfo.IsFolderArchived;
            }
            return null;
        }
        public List<ArichiveDBCultureInfo> GetCultureInfoFromArchiveDB(string EDisplayCode, Guid EnvelopeId)
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "Envelope Repository", "GetCultureInfoFromArchiveDB", "Get Culture Info of Envelope and Recipients from Archival Database ", EDisplayCode, "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);

                #region commented not supporting in core
                //using (var dbContext = new RSignDbContext(_configuration))
                //{
                //    EDisplayCode = !string.IsNullOrEmpty(EDisplayCode) ? EDisplayCode : string.Empty;
                //    SqlParameter[] parms = new SqlParameter[2];
                //    parms[0] = new SqlParameter("@EDisplayCode", EDisplayCode);
                //    parms[1] = new SqlParameter("@EnvelopeId", EnvelopeId);

                //    List<ArichiveDBCultureInfo> envelope = dbContext.Database.SqlQueryRaw<ArichiveDBCultureInfo>("EXEC usp_GetCultureInfoFromArichiveDb @EDisplayCode,@EnvelopeId", parms).ToList();
                //    return envelope;
                //}
                #endregion commented not supporting in core

                EDisplayCode = !string.IsNullOrEmpty(EDisplayCode) ? EDisplayCode : string.Empty;
                SqlConnection connection = new SqlConnection(Convert.ToString(_configuration.Value.ConnectionStrings.RSignContext));
                DataSet dataset = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand("usp_GetCultureInfoFromArichiveDb", connection);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Parameters.AddWithValue("@EnvelopeId", EnvelopeId);
                adapter.SelectCommand.Parameters.AddWithValue("@EDisplayCode", EDisplayCode);
                adapter.Fill(dataset);
                List<ArichiveDBCultureInfo> envelope = new List<ArichiveDBCultureInfo>();
                if (dataset != null && dataset.Tables.Count > 0)
                {
                    envelope = JsonConvert.DeserializeObject<List<ArichiveDBCultureInfo>>(JsonConvert.SerializeObject(dataset.Tables[0]));                   
                }
                return envelope;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return null;
            }
        }
        public ArichiveEnvelopesInfo GetArchivedEnvelope(string EDisplayCode, Guid EnvelopeId, Guid RecipientId = default(Guid), string securityCode = "")
        {
            try
            {
                loggerModelNew = new LoggerModelNew("", "Envelope Repository", "GetArchivedEnvelope", "Get Envelope data from Archival process is started ", EDisplayCode, "", "", "", "API");
                rsignlog.RSignLogInfo(loggerModelNew);
                ArichiveEnvelopesInfo envelopesInfo = new ArichiveEnvelopesInfo();
                EDisplayCode = !string.IsNullOrEmpty(EDisplayCode) ? EDisplayCode : string.Empty;
                SqlConnection connection = new SqlConnection(Convert.ToString(_configuration.Value.ConnectionStrings.RSignContext));
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = Convert.ToString("USP_GetEnvelopeFromArchiveDb");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@EnvelopeId", SqlDbType.UniqueIdentifier).Value = EnvelopeId;
                cmd.Parameters.Add("@EDisplayCode", SqlDbType.NVarChar).Value = EDisplayCode;
                cmd.Parameters.Add("@SecurityCode", SqlDbType.NVarChar).Value = securityCode;
                cmd.Connection = connection;
                cmd.CommandTimeout = 3600;
                connection.Open();

                reader = cmd.ExecuteReader();

                var dt = new DataTable();
                dt.Load(reader);
                List<ArichiveEnvelopesInfo> listArichiveEnvelopesInfo = JsonConvert.DeserializeObject<List<ArichiveEnvelopesInfo>>(JsonConvert.SerializeObject(dt));
                if (listArichiveEnvelopesInfo != null && listArichiveEnvelopesInfo.Count > 0)
                {
                    ArichiveEnvelopesInfo envelope = listArichiveEnvelopesInfo[0];
                    if (envelope != null)
                    {
                        if (EnvelopeId == Guid.Empty)
                            EnvelopeId = envelope.EnvelopeId;

                        var userPlanDetails = GetUserPlanByUserID(envelope.UserID);  
                        if (userPlanDetails != null && userPlanDetails.IsDefaultPlan == true)
                        {
                            string recipientCultureInfoFromArchive = string.Empty; string envelopecultureInfo = string.Empty;
                            List<ArichiveDBCultureInfo> cultureInfoFromArchiveDB = new List<ArichiveDBCultureInfo>();
                            cultureInfoFromArchiveDB = GetCultureInfoFromArchiveDB(EDisplayCode, EnvelopeId);
                            if (cultureInfoFromArchiveDB != null && cultureInfoFromArchiveDB.Count > 0 && RecipientId != Guid.Empty)
                            {
                                recipientCultureInfoFromArchive = cultureInfoFromArchiveDB.Where(r => r.RecipientId == RecipientId).Select(r => r.RecipientsCultureInfo).FirstOrDefault();
                            }
                            if (cultureInfoFromArchiveDB != null && cultureInfoFromArchiveDB.Count > 0)
                            {
                                envelopecultureInfo = cultureInfoFromArchiveDB.FirstOrDefault().EnvelopeCultureInfo;
                            }

                            envelopecultureInfo = !string.IsNullOrEmpty(recipientCultureInfoFromArchive) ? recipientCultureInfoFromArchive : envelopecultureInfo;
                            string ArchivedEnvelopeMessage = GetUniqueKey("ArchivalMessageSender", envelopecultureInfo);

                            envelopesInfo.ArchivedEnvelopeMessage = ArchivedEnvelopeMessage;
                            envelopesInfo.IsEnvelopePurging = true;
                            envelopesInfo.IsFolderArchived = envelope.IsFolderArchived;
                            envelopesInfo.EdisplayCode = envelope.EdisplayCode;
                            envelopesInfo.EnvelopeId = EnvelopeId;
                            envelopesInfo.UserID = envelope.UserID;
                            envelopesInfo.envelope = null;
                        }
                        else if (userPlanDetails != null && userPlanDetails.IsPaidPlan == true)
                        {
                            envelopesInfo.envelope = GetEntity(EnvelopeId, true, 1);
                            envelopesInfo.ArchivedEnvelopeMessage = string.Empty;
                            envelopesInfo.IsEnvelopePurging = false;
                            envelopesInfo.IsFolderArchived = envelope.IsFolderArchived;
                            envelopesInfo.EdisplayCode = envelope.EdisplayCode;
                            envelopesInfo.EnvelopeId = EnvelopeId;
                            envelopesInfo.UserID = envelope.UserID;
                        }
                    }
                }
                connection.Close();
                return envelopesInfo;
            }
            catch (Exception ex)
            {
                loggerModelNew.Message = ex.ToString();
                rsignlog.RSignLogError(loggerModelNew, ex);
                return null;
            }
        }
        public UserPlan GetUserPlanByUserID(Guid UserID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.UserPlan.Where(r => r.UserId == UserID).FirstOrDefault();
            }
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
