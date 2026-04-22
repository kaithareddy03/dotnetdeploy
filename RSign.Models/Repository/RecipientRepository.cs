using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
//using System.Data.Entity;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages;

namespace RSign.Models.Repository
{
    public class RecipientRepository : IRecipientRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly ILookupRepository _lookupRepository;
        private IHttpContextAccessor _accessor;
        public RecipientRepository(IOptions<AppSettingsConfig> configuration, ILookupRepository lookupRepository, IHttpContextAccessor accessor)
        {
            _configuration = configuration;
            _lookupRepository = lookupRepository;
            _accessor = accessor;
        }
        public List<RecipientDetails> GetActiveRecipientData(Guid envelopeID, bool loadAllRecipient = false)
        {
            try
            {
                if (!loadAllRecipient)
                {
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        List<RecipientDetails> recipients = dbContext.vw_ActiveRecipientWithoutHistory.Where(ar => ar.EnvelopeID == envelopeID && ar.IsSameRecipient != true).Select(ar => new RecipientDetails
                        {
                            ID = ar.ID,
                            EnvelopeID = ar.EnvelopeID,
                            RecipientTypeID = ar.RecipientTypeID,
                            RecipientName = ar.Name,
                            EmailID = ar.EmailAddress,
                            Order = ar.Order,
                            CreatedDateTime = ar.CreatedDateTime,
                            StatusID = ar.StatusID,
                            IsFinished = ar.IsFinished,
                            CopyEmailID = ar.CopyEmailAddress,
                            IsSameRecipient = ar.IsSameRecipient,
                            CCSignerType = ar.CCSignerType,
                            CultureInfo = ar.CultureInfo,
                            TemplateID = ar.TemplateID,
                            DeliveryMode = ar.DeliveryMode,
                            DialCode = ar.DialCode,
                            CountryCode = ar.CountryCode,
                            Mobile = ar.Mobile,
                            //ReminderType = 1,
                            IsReadonlyContact = ar.IsReadonlyContact
                        }).ToList();
                        return recipients;
                    }
                }
                else
                {
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        List<RecipientDetails> recipients = dbContext.vw_ActiveRecipientWithoutHistory.Where(ar => ar.EnvelopeID == envelopeID).Select(ar => new RecipientDetails
                        {
                            ID = ar.ID,
                            EnvelopeID = ar.EnvelopeID,
                            RecipientTypeID = ar.RecipientTypeID,
                            RecipientName = ar.Name,
                            EmailID = ar.EmailAddress,
                            Order = ar.Order,
                            CreatedDateTime = ar.CreatedDateTime,
                            StatusID = ar.StatusID,
                            IsFinished = ar.IsFinished,
                            CopyEmailID = ar.CopyEmailAddress,
                            IsSameRecipient = ar.IsSameRecipient,
                            CCSignerType = ar.CCSignerType,
                            CultureInfo = ar.CultureInfo,
                            TemplateID = ar.TemplateID,
                            DeliveryMode = ar.DeliveryMode,
                            CountryCode = ar.CountryCode,
                            DialCode = ar.DialCode,
                            Mobile = ar.Mobile,
                           // ReminderType = ar.ReminderType,
                            IsReadonlyContact = ar.IsReadonlyContact
                        }).ToList();
                        return recipients;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Guid GetSignerStatusId(Guid recipientId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerStatus.Where(s => s.RecipientID == recipientId).OrderByDescending(s => s.CreatedDateTime).Select(s => s.StatusID).First();
            }
        }
        public SignerStatus GetSignerStatus(Guid recipientId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerStatus.Where(s => s.RecipientID == recipientId).OrderByDescending(s => s.CreatedDateTime).FirstOrDefault();
            }
        }
        public SignerStatus GetSignerSignedStatusId(Guid recipientId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerStatus.Where(s => s.RecipientID == recipientId && s.StatusID == Constants.StatusCode.Signer.Signed).FirstOrDefault();
            }
        }
        public bool Save(SignerStatus signerStatus)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    dbContext.SignerStatus.Add(signerStatus);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool SaveRecipientDetail(Guid recipientId, Guid statusId, string IpAddress, string CopyEmailID = "")
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    RecipientsDetail recipientDetail = new RecipientsDetail();
                    recipientDetail.ID = Guid.NewGuid();
                    recipientDetail.RecipientID = recipientId;
                    recipientDetail.StatusTypeID = statusId;
                    recipientDetail.StatusDateTime = DateTime.Now;
                    recipientDetail.IPAddress = IpAddress; // Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress);
                    recipientDetail.SignedBy = CopyEmailID;
                    dbContext.RecipientsDetail.Add(recipientDetail);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Recipients GetEntity(Guid recipientId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.Recipients.Where(rc => rc.ID == recipientId).FirstOrDefault();
            }
        }
        public bool Save(Recipients recipient)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var recipientDetail = dbContext.Recipients.Where(r => r.ID == recipient.ID).SingleOrDefault();
                    if (recipientDetail == null)
                    {
                        dbContext.Recipients.Add(recipient);
                    }
                    else
                    {
                        recipientDetail.RecipientTypeID = recipient.RecipientTypeID;
                        recipientDetail.Name = recipient.Name;
                        recipientDetail.Order = recipient.Order;
                        recipientDetail.EmailAddress = recipient.EmailAddress;
                        recipientDetail.RecipientCode = recipient.RecipientCode;
                        recipientDetail.TemplateGroupId = recipient.TemplateGroupId;
                        recipientDetail.EnvelopeTemplateGroupID = recipient.EnvelopeTemplateGroupID;
                        recipientDetail.IsReviewed = recipient.IsReviewed;
                        recipientDetail.CCSignerType = recipient.CCSignerType;
                        recipientDetail.CultureInfo = recipient.CultureInfo;
                        recipientDetail.SendDocumentOnDelegate = recipient.SendDocumentOnDelegate;
                       // recipientDetail.ReminderType = recipient.ReminderType;
                        recipientDetail.IsReadonlyContact = recipient.IsReadonlyContact;
                        recipientDetail.DeliveryMode = recipient.DeliveryMode;
                        recipientDetail.DialCode = recipient.DialCode;
                        recipientDetail.CountryCode = recipient.CountryCode;
                        recipientDetail.Mobile = recipient.Mobile;
                        recipientDetail.IsSameRecipient = recipient.IsSameRecipient;
                       // recipientDetail.ReminderType = recipientDetail.ReminderType;
                        recipientDetail.IsFinished = recipient.IsFinished;
                        recipientDetail.VerificationCode = recipient.VerificationCode;
                        dbContext.Entry(recipientDetail).State = EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SignerSignature GetSignerSignature(Guid recipientId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerSignature.Where(s => s.RecipientID == recipientId).FirstOrDefault();
            }
        }
        public bool SaveEmailQueue(EmailQueue emailqueue)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var emailQueueDetail = dbContext.EmailQueue.Where(e => e.ID == emailqueue.ID).SingleOrDefault();
                    if (emailQueueDetail == null)
                    {
                        dbContext.EmailQueue.Add(emailqueue);
                    }
                    else
                    {
                        emailQueueDetail.CreatedDate = DateTime.Now;
                        emailQueueDetail.ModifiedDate = DateTime.Now;
                        emailQueueDetail.Subject = emailqueue.Subject;
                        emailQueueDetail.EnvelopeCode = emailqueue.EnvelopeCode;
                        emailQueueDetail.EnvelopeId = emailqueue.EnvelopeId;
                        emailQueueDetail.Status = 0;
                        emailQueueDetail.CreatedServer = Environment.MachineName;
                        emailQueueDetail.ProcessedServer = Environment.MachineName;
                        emailQueueDetail.isAttachment = emailqueue.isAttachment;
                        emailQueueDetail.SenderEmail = emailqueue.SenderEmail;
                        emailQueueDetail.SenderName = emailqueue.SenderName;
                        emailQueueDetail.ReprocessCount = emailqueue.ReprocessCount + 1;
                        emailQueueDetail.EmailType = emailqueue.EmailType;
                        emailQueueDetail.Body = emailqueue.Message;

                        dbContext.Entry(emailQueueDetail).State = EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SaveEmailQueueRecipients(EmailQueueRecipients emailqueuerecipients)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var emailqueuerecipientsDetail = dbContext.EmailQueueRecipients.Where(e => e.ID == emailqueuerecipients.ID).SingleOrDefault();
                    if (emailqueuerecipientsDetail == null)
                    {
                        dbContext.EmailQueueRecipients.Add(emailqueuerecipients);
                    }
                    else
                    {
                        emailqueuerecipientsDetail.EmailQueueId = emailqueuerecipients.EmailQueueId;
                        emailqueuerecipientsDetail.CreatedDate = DateTime.Now;
                        emailqueuerecipientsDetail.ModifiedDate = DateTime.Now;
                        emailqueuerecipientsDetail.EnvelopeId = emailqueuerecipients.EnvelopeId;
                        emailqueuerecipientsDetail.Status = 0;
                        emailqueuerecipientsDetail.Server = Environment.MachineName;
                        emailqueuerecipientsDetail.RecipientEmail = emailqueuerecipients.RecipientEmail;
                        emailqueuerecipientsDetail.RecipientName = emailqueuerecipients.RecipientName;
                        emailqueuerecipientsDetail.ReprocessCount = emailqueuerecipientsDetail.ReprocessCount + 1;
                        emailqueuerecipientsDetail.EmailType = emailqueuerecipients.EmailType;
                        emailqueuerecipientsDetail.Subject = emailqueuerecipients.Subject;
                        emailqueuerecipientsDetail.Body = emailqueuerecipients.Body;
                        dbContext.Entry(emailqueuerecipientsDetail).State = EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SaveEmailQueueAttachments(EmailQueueAttachment emailQueueAttachment)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var emailqueuerecipientsDetail = dbContext.EmailQueueAttachment.Where(e => e.EmailQueueId == emailQueueAttachment.EmailQueueId && e.AttachmentName.ToLower() == emailQueueAttachment.AttachmentName.ToLower()).SingleOrDefault();
                    if (emailqueuerecipientsDetail == null)
                    {
                        dbContext.EmailQueueAttachment.Add(emailQueueAttachment);
                    }
                    else
                    {
                        emailqueuerecipientsDetail.EmailQueueId = emailQueueAttachment.EmailQueueId;
                        emailqueuerecipientsDetail.AttachmentData = emailQueueAttachment.AttachmentData;
                        emailqueuerecipientsDetail.AttachmentName = emailQueueAttachment.AttachmentName;
                        emailqueuerecipientsDetail.CreatedDate = DateTime.Now;
                        dbContext.Entry(emailqueuerecipientsDetail).State = EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<EmailQueue> GetEmailQueueData(Guid EnvelopeID, string EmailType)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return dbContext.EmailQueue.Where(r => r.EnvelopeId == EnvelopeID && r.EmailType == EmailType && r.Status != 2).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<vw_ActiveRecipientWithoutHistory> GetEnvelopeSignerRecipientByEmail(Guid envelopeId, string emailAddress)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.vw_ActiveRecipientWithoutHistory.Where(rc => rc.EnvelopeID == envelopeId && rc.EmailAddress == emailAddress && rc.RecipientTypeID == Constants.RecipientType.Signer).ToList();
            }
        }
        public Guid GetSignerPrimaryStatusId(Guid recipientId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerStatus.Where(s => s.RecipientID == recipientId).OrderByDescending(s => s.CreatedDateTime).Select(s => s.ID).First();
            }
        }
        public bool ModifyLastRecipientEntry(Guid recipientId, Guid statusId, string IpAddress)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    RecipientsDetail recipientDetail = dbContext.RecipientsDetail.Where(r => r.RecipientID == recipientId).OrderByDescending(r => r.StatusDateTime).FirstOrDefault();
                    if (recipientDetail != null)
                    {
                        recipientDetail.StatusTypeID = statusId;
                        recipientDetail.StatusDateTime = DateTime.Now.AddSeconds(1);
                        recipientDetail.IPAddress = IpAddress;// Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                        dbContext.Entry(recipientDetail).State = EntityState.Modified;
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
        public bool Save(SignerSignature signerDetails)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                SignerSignature signerSignObject = dbContext.SignerSignature.Where(s => s.RecipientID == signerDetails.RecipientID).FirstOrDefault();
                if (signerSignObject == null)
                {
                    dbContext.SignerSignature.Add(signerDetails);
                }
                else
                {
                    signerSignObject.Signature = signerDetails.Signature;
                    if (dbContext.Entry(signerSignObject).State == EntityState.Unchanged)
                        dbContext.Entry(signerSignObject).State = EntityState.Modified;
                }
                dbContext.SaveChanges();
                return true;
            }
        }
        public bool AddDelegatedSigner(Guid recipientId, Guid statusId, Guid delegatedToId, string IPaddress)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    SignerStatus signerStatus = new SignerStatus();
                    signerStatus.RecipientID = recipientId;
                    signerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                    signerStatus.StatusID = statusId;
                    signerStatus.DelegateTo = delegatedToId;
                    signerStatus.ID = Guid.NewGuid();
                    signerStatus.CreatedDateTime = DateTime.Now;
                    dbContext.SignerStatus.Add(signerStatus);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public IQueryable<Recipients> GetAllRecipients(Guid envelopeID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                IList<Recipients> recipients = new List<Recipients>();
                recipients = dbContext.Recipients.Where(r => r.EnvelopeID == envelopeID && r.RecipientTypeID != Constants.RecipientType.Sender).ToList();
                return recipients.AsQueryable();
            }
        }
        public Recipients GetSenderDetails(Guid envelopeId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var recipientTypeId = _lookupRepository.GetLookup(Lookup.RecipientType).Where(s => s.Value == "Sender").Select(s => s.Key).FirstOrDefault();

                var senderEmail = dbContext.Envelope
                    .Where(e => e.ID == envelopeId)
                    .Select(e => e.Recipients).FirstOrDefault()
                    .Where(r => r.RecipientTypeID == Guid.Parse(recipientTypeId)).FirstOrDefault();

                return senderEmail;
            }
        }
        public bool AddSignerRemark(Guid recipientId, string remark, string IPaddress, int? declineReasonID, string CopyEmail = "")
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    SignerStatus signerStatus = new SignerStatus();
                    signerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                    signerStatus.RecipientID = recipientId;
                    signerStatus.ID = Guid.NewGuid();
                    signerStatus.CreatedDateTime = DateTime.Now;
                    signerStatus.StatusID = Guid.Parse(_lookupRepository.GetLookup(Lookup.SignerStatus)
                                            .Where(s => s.Value == "Rejected")
                                            .Select(s => s.Key)
                                            .FirstOrDefault());
                    signerStatus.RejectionRemarks = remark;
                    signerStatus.DeclineReasonID = declineReasonID;
                    signerStatus.SignedBy = CopyEmail;
                    dbContext.SignerStatus.Add(signerStatus);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public int UpdateRecipientsforOutOfOffice(Guid envelopeId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return dbContext.Database.ExecuteSqlRaw("EXEC usp_RerouteRecipients @EnvelopeID", new SqlParameter("EnvelopeID", envelopeId));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<Recipients> GetAll(Guid id)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.Recipients.Where(r => r.EnvelopeID == id).ToList().AsQueryable<Recipients>();
            }
        }
        public SignerStatus GetCopySignerStatus(Guid ID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerStatus.Where(s => s.StatusID == Constants.StatusCode.Signer.Signed && s.RecipientID == ID && !string.IsNullOrEmpty(s.SignedBy)).FirstOrDefault();
            }
        }
        public byte[] GetInitialSignatureValue(Guid recipientId)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var contentObject = dbContext.DocumentContents.Where(dc => dc.ControlID == Constants.Control.NewInitials && dc.SignatureControlValue != null && dc.RecipientID == recipientId).FirstOrDefault();
                    if (contentObject == null)
                        return null;
                    else
                        return contentObject.SignatureControlValue;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool AddSignerStatus(Guid recipientId, Guid statusId, string IPaddress, string copyEmail = "")
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    DateTime signedDateTime = DateTime.Now;
                    if (statusId == Constants.StatusCode.Signer.Signed)
                    {
                        SignerStatus signerLatestStatus = GetStatus(recipientId, dbContext);
                        if (signerLatestStatus != null && signerLatestStatus.CreatedDateTime != null && signedDateTime <= signerLatestStatus.CreatedDateTime)
                        {
                            signedDateTime = signerLatestStatus.CreatedDateTime.AddSeconds(5);
                        }
                    }
                    SignerStatus signerStatus = new SignerStatus();
                    signerStatus.RecipientID = recipientId;
                    signerStatus.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                    signerStatus.StatusID = statusId;
                    signerStatus.ID = Guid.NewGuid();
                    signerStatus.CreatedDateTime = signedDateTime;
                    signerStatus.SignedBy = copyEmail;
                    dbContext.SignerStatus.Add(signerStatus);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public IQueryable<SignerStatus> GetAllSignerStatus(Guid recipientId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerStatus.Where(ss => ss.RecipientID == recipientId).ToList().AsQueryable();
            }
        }
        public SignerStatus GetStatus(Guid recipientId, RSignDbContext dbContext)
        {
            return dbContext.SignerStatus.Where(s => s.RecipientID == recipientId).OrderByDescending(s => s.CreatedDateTime).FirstOrDefault();
        }
        public List<SignerStatus> GetStatusList(List<Guid> recipientIds)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerStatus.Where(s => recipientIds.Any(r => s.RecipientID == r)).ToList();
            }
        }
        public List<SignerSignature> GetSignerSignatureList(List<Guid> recipientIds)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerSignature.Where(s => recipientIds.Any(r => s.RecipientID == r)).ToList();
            }
        }
        public List<SignerStatus> GetCopySignerStatusAllrecipients(List<Guid> recipientIds)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SignerStatus.Where(s => s.StatusID == Constants.StatusCode.Signer.Signed && recipientIds.Any(r => s.RecipientID == r) && !string.IsNullOrEmpty(s.SignedBy)).ToList();
            }
        }
        public bool AddOrModifyLastRecipientEntry(Guid recipientId, Guid statusId, string IpAddress)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    RecipientsDetail recipientDetail = dbContext.RecipientsDetail.Where(r => r.RecipientID == recipientId).OrderByDescending(r => r.StatusDateTime).FirstOrDefault();
                    if (recipientDetail != null)
                    {
                        recipientDetail.StatusTypeID = statusId;
                        recipientDetail.StatusDateTime = DateTime.Now.AddSeconds(1);
                        recipientDetail.IPAddress = IpAddress; // Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                        dbContext.Entry(recipientDetail).State = EntityState.Modified;
                    }
                    else
                    {
                        RecipientsDetail recipientDetailToAdd = new RecipientsDetail();
                        recipientDetailToAdd.ID = Guid.NewGuid();
                        recipientDetailToAdd.RecipientID = recipientId;
                        recipientDetailToAdd.StatusTypeID = statusId;
                        recipientDetailToAdd.StatusDateTime = DateTime.Now;
                        recipientDetailToAdd.IPAddress = IpAddress; // Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                        dbContext.RecipientsDetail.Add(recipientDetailToAdd);
                    }
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SaveRecipientDetailForEmailConfirm(Guid recipientId, Guid statusId, string IpAddress)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    RecipientsDetail recipientDetail = new RecipientsDetail();
                    recipientDetail.ID = Guid.NewGuid();
                    recipientDetail.RecipientID = recipientId;
                    recipientDetail.StatusTypeID = statusId;
                    recipientDetail.StatusDateTime = DateTime.Now.AddSeconds(2);
                    recipientDetail.IPAddress = IpAddress; // Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                    dbContext.RecipientsDetail.Add(recipientDetail);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool ModifyLastRecipientSignedEntry(Guid recipientId, Guid statusId, string IpAddress, RSignDbContext dbContext)
        {
            try
            {
                //delete last Signer Status
                var signerStatus = dbContext.SignerStatus.Where(status => status.RecipientID == recipientId && status.StatusID == Constants.StatusCode.Signer.Signed);
                if (signerStatus != null)
                {
                    foreach (SignerStatus status in signerStatus)
                    {
                        dbContext.SignerStatus.Remove(status);
                    }
                }
                dbContext.SaveChanges();

                //Update Recipients details
                RecipientsDetail recipientDetail = dbContext.RecipientsDetail.Where(r => r.RecipientID == recipientId && r.StatusTypeID == Constants.StatusCode.Signer.Signed).OrderByDescending(r => r.StatusDateTime).FirstOrDefault();
                recipientDetail.StatusTypeID = statusId;
                recipientDetail.StatusDateTime = DateTime.Now;
                recipientDetail.IPAddress = Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                dbContext.Entry(recipientDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Recipients GetRecipientByCode(string recipientCode)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                Recipients recipients = dbContext.Recipients.Where(rc => rc.RecipientCode == recipientCode).SingleOrDefault();
                return recipients;
            }
        }
        public async Task<APIRecipientEntityModel> GetRecipientEntity(string emailID, Guid envelopeID, Guid? templateKey, string mobile = "")
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    System.Text.StringBuilder sql = new System.Text.StringBuilder();
                    sql.Append("EXEC GetRecipientOfEnvelopeByEmailID @EmailID,@EnvelopeID,@TemplateKey,@Mobile");
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter() { ParameterName = "@EmailID", Value = emailID });
                    parameters.Add(new SqlParameter() { ParameterName = "@EnvelopeID", Value = envelopeID });
                    parameters.Add(new SqlParameter() { ParameterName = "@TemplateKey", Value = templateKey });
                    parameters.Add(new SqlParameter() { ParameterName = "@Mobile", Value = mobile });

                    var apiRecipientEntityList = await dbContext.APIRecipientEntityModel
                             .FromSqlRaw(sql.ToString(), parameters.ToArray())
                             .AsNoTracking().ToListAsync();

                    if (apiRecipientEntityList != null && apiRecipientEntityList.Count() > 0)
                        return apiRecipientEntityList.FirstOrDefault();
                    else
                        return null;

                    //APIRecipientEntity apiRecipientEntity = dbContext.APIRecipientEntity.FromSqlRaw("EXEC GetRecipientOfEnvelopeByEmailID @EmailID,@EnvelopeID,@TemplateKey",
                    //new SqlParameter("EmailID", emailID), new SqlParameter("EnvelopeID", envelopeID), new SqlParameter("TemplateKey", templateKey)).FirstOrDefault();

                    //APIRecipientEntity apiRecipientEntity = dbContext.Database.SqlQueryRaw<APIRecipientEntity>("EXEC GetRecipientOfEnvelopeByEmailID @EmailID,@EnvelopeID,@TemplateKey",
                    //new SqlParameter("EmailID", emailID), new SqlParameter("EnvelopeID", envelopeID), new SqlParameter("TemplateKey", templateKey)).FirstOrDefault();
                   // return apiRecipientEntity;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveRecipientDetailOnSend(Guid recipientId, Guid statusId, string IpAddress)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    RecipientsDetail recipientDetail = new RecipientsDetail();
                    recipientDetail.ID = Guid.NewGuid();
                    recipientDetail.RecipientID = recipientId;
                    recipientDetail.StatusTypeID = statusId;
                    recipientDetail.StatusDateTime = DateTime.Now;
                    recipientDetail.IPAddress = IpAddress; // Convert.ToString(_accessor.HttpContext.Connection.RemoteIpAddress); 
                    dbContext.RecipientsDetail.Add(recipientDetail);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void UpdateIsSendFinalDocumentOnDelegate(Recipients recipientDetai)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                recipientDetai.SendDocumentOnDelegate = true;
                dbContext.Entry(recipientDetai).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
        }
        public string GetSignerFinalSubmitOTP(string EnvelopeId, string RecipientId)
        {
            string otpGenerated = string.Empty;
            using (var dbContext = new RSignDbContext(_configuration))
            {
                if (!string.IsNullOrEmpty(EnvelopeId) && !string.IsNullOrEmpty(RecipientId))
                {
                    otpGenerated = dbContext.SignerVerificationOTP.Where(r => r.RecipientID == new Guid(RecipientId) && r.EnvelopeID == new Guid(EnvelopeId) && r.IsCodeActive == true).SingleOrDefault().VerificationCode.ToString();
                }
            }
            return otpGenerated;
        }
        public bool SaveSignerFinalSubmitOTP(SignerVerificationOTP signerVerification)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {                  
                    SignerVerificationOTP signerVerificationOTP = dbContext.SignerVerificationOTP.Where(r => r.RecipientID == signerVerification.RecipientID && r.EnvelopeID == signerVerification.EnvelopeID && r.IsCodeActive == true).SingleOrDefault();
                    if (signerVerificationOTP == null)
                    {
                        signerVerification.IsCodeActive = true;
                        dbContext.SignerVerificationOTP.Add(signerVerification);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        // Updating existing OTP as inactive
                        signerVerificationOTP.CreatedDateTime = DateTime.Now;
                        signerVerificationOTP.IsCodeActive = false;
                        dbContext.Entry(signerVerificationOTP).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        // Inserting new Record
                        signerVerification.ID = Guid.NewGuid();
                        signerVerification.IsCodeActive = true;
                        signerVerificationOTP.CreatedDateTime = DateTime.Now;
                        dbContext.SignerVerificationOTP.Add(signerVerification);
                        dbContext.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
