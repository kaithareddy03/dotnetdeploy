using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly IDocumentContentsRepository _documentContentsRepository;
        public DocumentRepository(IOptions<AppSettingsConfig> configuration, IDocumentContentsRepository documentContentsRepository)
        {
            _configuration = configuration;
            _documentContentsRepository = documentContentsRepository;
        }
        public IQueryable<Documents> GetAll(Guid envelopeID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.Documents.Where(d => d.EnvelopeID == envelopeID).AsQueryable();
            }
        }
        public bool SaveEmailLogsRecord(List<EmailLogs> emailLogs)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    foreach (var item in emailLogs)
                    {
                        var emailLogsModel = new EmailLogs();
                        emailLogsModel.MessageId = item.MessageId;
                        emailLogsModel.EnvelopeCode = item.EnvelopeCode;
                        emailLogsModel.InnerException = item.InnerException;
                        emailLogsModel.FullException = item.FullException;
                        emailLogsModel.ResponseMessage = item.ResponseMessage;
                        emailLogsModel.ServerName = item.ServerName;
                        emailLogsModel.CreatedDate = DateTime.Now;
                        dbContext.EmailLogs.Add(emailLogsModel);
                        dbContext.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void SaveDestinationRecord(Destination destinationRecord)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var destination = new Destination();
                    destination.MessageId = destinationRecord.MessageId;
                    destination.EnvelopeCode = destinationRecord.EnvelopeCode;
                    destination.RecipientEmail = destinationRecord.RecipientEmail;
                    destination.IsSent = destinationRecord.IsSent;
                    destination.DeliveryStatus = destinationRecord.DeliveryStatus;
                    destination.DeliveryStatusDetail = destinationRecord.DeliveryStatusDetail;
                    destination.IsProcessed = destinationRecord.IsProcessed;
                    destination.CreatedDate = DateTime.Now;
                    destination.LastUpdatedDate = DateTime.Now;
                    destination.EmailSentType = destinationRecord.EmailSentType;
                    destination.RetryCount = destinationRecord.RetryCount;
                    dbContext.Destination.Add(destination);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
        }
        public bool Save(Documents document)
        {          
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var documnentDetail = dbContext.Documents.Where(d => d.ID == document.ID).SingleOrDefault();
                    if (documnentDetail == null)
                    {
                        dbContext.Documents.Add(document);
                        if (document.DocumentContents != null)
                            foreach (var docContents in document.DocumentContents)
                                _documentContentsRepository.Save(docContents);
                    }
                    else
                    {
                        documnentDetail.Order = document.Order;
                        documnentDetail.UploadedDateTime = document.UploadedDateTime;
                        documnentDetail.DocumentName = document.DocumentName;
                        documnentDetail.ActionType = document.ActionType;
                    }
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
