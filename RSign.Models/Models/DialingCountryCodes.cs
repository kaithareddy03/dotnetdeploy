using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models
{
    public partial class DialingCountryCodes
    {
        public int ID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string DialCode { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> MobileMaxLength { get; set; }
    }
}
