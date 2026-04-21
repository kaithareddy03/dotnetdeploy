

namespace RSign.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class RecipientsDetail
    {
        [Key]
        public System.Guid ID { get; set; }        
        public System.Guid RecipientID { get; set; }
        public System.Guid StatusTypeID { get; set; }
        public System.DateTime StatusDateTime { get; set; }
        public string? IPAddress { get; set; }
        public string? OldRecipient { get; set; }
        public string? OldEmail { get; set; }
        public string? CurrentRecipient { get; set; }
        public string? CurrentEmail { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
        public string? SignedBy { get; set; }    
        public virtual Recipients Recipients { get; set; }
        [NotMapped]
        public virtual Status Status { get; set; }
        public string? DialCode { get; set; }
        public string? CountryCode { get; set; }
        public string? Mobile { get; set; }
        public string? OldDialCode { get; set; }
        public string? OldCountryCode { get; set; }
        public string? OldMobile { get; set; }
    }
}
