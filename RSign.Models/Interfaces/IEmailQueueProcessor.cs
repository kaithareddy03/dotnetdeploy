using RSign.Models.EmailQueueProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IEmailQueueProcessor
    {
        List<EmailQueue> GetEmailQueueData(Guid EnvelopeID, string EmailType);
        int CreateUpdateEmailQueue(EmailQueueProcessor.EmailQueueData emailQueueData);
        bool CreateUpdateEmailQueueRecipients(EmailQueueRecipientsData emailQueueRecipientsData);
        bool CreateEmailQueueAttachment(EmailQueueAttachmentData emailQueueAttachmentData, int emailQueueId);
        int CreateUpdateTemplateEmailQueue(EmailQueueData emailQueueData);
        bool CreateUpdateEmailQueueTemplateRecipients(EmailQueueRecipientsData emailQueueRecipientsData);
    }
}
