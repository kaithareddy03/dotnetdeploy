using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models
{
    public partial class vw_LanguageKeyMapping
    {
        public System.Guid ID { get; set; }
        public System.Guid ResourceKeyID { get; set; }
        public string KeyName { get; set; }
        public Nullable<System.Guid> LanguageID { get; set; }
        public string? LanguageCode { get; set; }
        public string? KeyValue { get; set; }
        public string? KeyType { get; set; }
        public Nullable<bool> Active { get; set; }
        public string? PageName { get; set; }
    }
}
