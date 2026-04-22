using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models
{
    public partial class vw_DasbordInboxAndWaitingForOthers
    {
        public System.Guid EnvelopeID { get; set; }
        public System.Guid UserID { get; set; }
        public string EDisplayCode { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.Guid> TemplateKey { get; set; }
        public string RecipientEmail { get; set; }
        public string ReceipientID { get; set; }
        public string SenderEmail { get; set; }
        public System.Guid StatusID { get; set; }
        public string EnvelopeStatus { get; set; }
        public string ReferenceCode { get; set; }
        public string ReferenceEmail { get; set; }
        public string UserEnvelopeStatus { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public bool IsSequenceCheck { get; set; }
        public int SignerStatus { get; set; }
        public bool IsFinished { get; set; }
        public bool IsTemplateShared { get; set; }
        public System.Guid DateFormatID { get; set; }
        public string RecipientName { get; set; }
        public string DialCode { get; set; }
        public string Mobile { get; set; }
    }
}
