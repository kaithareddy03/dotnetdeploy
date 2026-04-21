using RSign.Common.Enums;
using RSign.Models.APIModels.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface ILookupRepository
    {
        IEnumerable<LookupItem> GetLookup(Lookup lookup, string languageId = "");
        Dictionary<Guid?, string> GetLookupMasterLanguage(Lookup lookup, string languageId, string keyType, string userType = "");
        List<LookupKeyItem> GetLookupLanguageList(Lookup lookup, string languageId, string keyType);
        LanguageKeyDetails GetLookupLanguageKeyDetails(Guid? ResourceKeyID, string languageCode);
        Dictionary<Guid, string> GetLookupLanguage(Lookup lookup, string languageId);
        Dictionary<Guid, string> GetLanguageKeyDetailsFromJson(string languageId);
        Dictionary<string, string> GetLanguageKeyNameDesc(string languageId);
    }
}
