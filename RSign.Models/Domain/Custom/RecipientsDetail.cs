using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RSign.Models
{
    public partial class RecipientsDetail
    {
        [NotMapped]
        public string? SignerStatusDescription { get; set; }
    }
}
