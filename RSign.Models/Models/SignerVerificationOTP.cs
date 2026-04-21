using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models
{
    public partial class SignerVerificationOTP
    {
        [Key]
        public System.Guid ID { get; set; }
        public System.Guid EnvelopeID { get; set; }
        public System.Guid RecipientID { get; set; }
        public string? Name { get; set; }
        public string EmailAddress { get; set; }
        public string? VerificationCode { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<bool> IsCodeActive { get; set; }
        public string DialCode { get; set; }
        public string Mobile { get; set; }
    }
}
