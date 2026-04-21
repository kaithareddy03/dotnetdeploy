using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RSign.Models
{
    public partial class Recipients
    {
        [NotMapped]
        public string? ContractStage { get; set; }
        [NotMapped]
        public string? GhostName { get; set; }
        [NotMapped]
        public string? RecipientTypeDescription { get; set; }
        [NotMapped]
        public Guid StatusID { get; set; }
        [NotMapped]
        public string? SignerStatusDescription { get; set; }
        [NotMapped]
        public string? SignerIPAddress { get; set; }
        [NotMapped]
        public Guid? DelegatedRecipientID { get; set; }
        [NotMapped]
        public string? DelegatedTo { get; set; }
        [NotMapped]
        public DateTime StatusDate { get; set; }
        [NotMapped]
        public int DisplayOrder { get; set; }
        public List<RecipientsDetail> RecipientHistory { get; set; }        
    }

    public class RecipientSuggestions
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string DialCode { get; set; }
        public string Mobile { get; set; }
    }

    public class RecipientList
    {
        public string? RecipientName { get; set; }

        public string? RecipientEmail { get; set; }

    }
}
