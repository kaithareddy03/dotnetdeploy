using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.APIModels.Data
{
    public class LookupItem
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        public bool IsActive { get; set; }
    }

    public class LookupKeyItem
    {
        public Guid ResourceKeyID { get; set; }
        public string KeyName { get; set; }
        public string? LanguageCode { get; set; }
        public string? KeyValue { get; set; }
        public string? KeyType { get; set; }
        public string? PageName { get; set; }
    }

    public class LookupMasterKeyItem
    {
        public Guid? ResourceKeyID { get; set; }
        public string? KeyName { get; set; }
        public string? LanguageCode { get; set; }
        public string? KeyValue { get; set; }
        public string? KeyType { get; set; }
        public int? OrderBy { get; set; }
    }
}
