using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models
{
    public partial class vw_GetEnvelopeRecipients
    {
        public System.Guid ID { get; set; }
        public System.Guid EnvelopeID { get; set; }
        public System.Guid RecipientTypeID { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string RerouteEmailAddress { get; set; }
        public Nullable<System.Guid> TemplateID { get; set; }
        public string RecipientCode { get; set; }
        public Nullable<long> TemplateGroupId { get; set; }
        public Nullable<System.Guid> EnvelopeTemplateGroupID { get; set; }
        public Nullable<System.Guid> TemplateRoleId { get; set; }
        public Nullable<bool> IsFinished { get; set; }
        public System.Guid StatusID { get; set; }
        public Nullable<System.DateTime> StatusDateTime { get; set; }
        public string CopyEmailAddress { get; set; }
        public bool IsSameRecipient { get; set; }
        public Nullable<int> CCSignerType { get; set; }
        public string CultureInfo { get; set; }
        public Nullable<int> DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
    }
}
