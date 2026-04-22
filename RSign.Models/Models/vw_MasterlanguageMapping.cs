using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Models
{
    public partial class vw_MasterlanguageMapping
    {
        public System.Guid ResourceKey { get; set; }
        public string KeyName { get; set; }
        public Nullable<System.Guid> LanguageID { get; set; }
        public string LanguageCode { get; set; }
        public string KeyValue { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public string LookupName { get; set; }
        public System.Guid ID { get; set; }
    }
}
