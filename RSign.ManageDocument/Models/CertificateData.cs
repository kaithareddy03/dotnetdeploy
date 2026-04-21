using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Models
{
    public class CertificateData
    {
        public CertificateData()
        {
            RecipientsWithWaitingForSignatureStatus = new List<AsposeRecipientDetails>();
        }
        public string Subject { get; set; }
        public List<string> Documents { get; set; }
        public string DocHash { get; set; }
        public Guid EnvelopeId { get; set; }
        public string EnvelopeCode { get; set; }
        public string DisplayCode { get; set; }
        public string Sender { get; set; }
        public List<AsposeRecipient> Recipients { get; set; }
        public DateTime EnvelopeSent { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime EnvelopeCompletedDate { get; set; }
        public List<AsposeRecipientDetails> RecipientsWithWaitingForSignatureStatus { get; set; }
        public Guid DateFormatId { get; set; }
        public MetaDataAndHistory MetaDataAndHistory { get; set; }
        public List<AsposeRecipientDetails> AllRecipientsHistoryDetails { get; set; }
        public List<AsposeRecipientDetails> SenderUpdateHistoryDetails { get; set; }
        public List<string> FileReviewDocuments { get; set; }
        public string AccessAuthentication { get; set; }
        public string EmailAccessCode { get; set; }
        public string EmailVerification { get; set; }
        public List<AsposeDropdownOptions> AsposeDropdownOptions { get; set; }
        public bool? EnableMessageToMobile { get; set; }
    }
}
