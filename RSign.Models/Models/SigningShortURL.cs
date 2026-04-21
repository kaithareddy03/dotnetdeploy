using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models
{
    public partial class SigningShortURL
    {
        public int ID { get; set; }
        public System.Guid EnvelopeID { get; set; }
        public string? SigningURI { get; set; }
        public string? ShortURICode { get; set; }
        public string? ShortURI { get; set; }
        public Nullable<bool> IsEnvelopeCompleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string? EDisplayCode { get; set; }
    }
}
