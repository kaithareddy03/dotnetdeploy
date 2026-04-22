using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Models
{
    public partial class vw_SigningInbox
    {
        public System.Guid EnvelopeID { get; set; }
        public System.Guid UserID { get; set; }
        public string? EDisplayCode { get; set; }
        public string? Subject { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.Guid> TemplateKey { get; set; }
        public string? RecipientEmail { get; set; }
        public System.Guid ReceipientID { get; set; }
        public int SignerStatus { get; set; }
        public string? SenderEmail { get; set; }
        public System.Guid StatusID { get; set; }
        public string? EnvelopeStatus { get; set; }
        public string? ReferenceCode { get; set; }
        public string? ReferenceEmail { get; set; }
        public string? SenderName { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public bool IsSequenceCheck { get; set; }
        public Nullable<System.Guid> EnvelopeTemplateGroupsID { get; set; }
        public string? TemplateName { get; set; }
        public string? TemplateDescription { get; set; }
        public string? RoleName { get; set; }
        public Nullable<int> Order { get; set; }
        public bool IsFinished { get; set; }
        public Nullable<int> TemplateOrder { get; set; }
        public string? Message { get; set; }
        public System.Guid DateFormatID { get; set; }
        public string? DialCode { get; set; }
        public string? Mobile { get; set; }
    }
}
