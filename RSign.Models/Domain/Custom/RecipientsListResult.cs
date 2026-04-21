using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSign.Models
{
    public class RecipientsListResult
    {
        public Guid? Id { get; set; }
        public Guid RecipientId { get; set; }
        public string? RecipientEmailAddress { get; set; }
        public Int32 EnvelopeCode { get; set; }
        public Int32 EDisplayCode { get; set; }
        public DateTime ExpiryDate { get; set; } 

    }
}
