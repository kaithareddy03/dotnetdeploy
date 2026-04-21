using Chilkat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Common.Configuration;
using RSign.Common.Configuration.Interfaces;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Models.APIModels.Data;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Repository
{
    public class LookupRepository : ILookupRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly IConfiguration _appConfiguration;

        public ICacheProvider Cache { get; set; }
        private string cultureInfo = string.Empty;
        public LookupRepository(IOptions<AppSettingsConfig> configuration, IConfiguration appConfiguration)
        {
            _configuration = configuration;
            _appConfiguration = appConfiguration;
            Cache = new DefaultCacheProvider();
        }
        public IEnumerable<LookupItem> GetLookup(Lookup lookup, string languageId = "")
        {
            var lookupList = Cache.Get(Enum.GetName(typeof(Lookup), lookup)) as List<LookupItem>;
            if (lookup != Lookup.TextType)
            {//Because Text Type Is Loading From Database And It Can Be Change Whenever Language Is Change
                if (lookupList != null)
                    return lookupList;
            }
            InvokeLookupLoad(lookup, languageId); // Load the lookup and add into cache.
            lookupList = Cache.Get(Enum.GetName(typeof(Lookup), lookup)) as List<LookupItem>; // Again try to get the lookup from Cache.

            if (lookupList != null)
                return lookupList;
            throw new ArgumentException("The specified lookup is not found in Cache.");
        }

        public Dictionary<Guid?, string> GetLookupMasterLanguage(Lookup lookup, string languageId, string keyType, string userType = "")
        {
            var lookupList = Cache.Get(Enum.GetName(typeof(Lookup), lookup)) as List<LookupMasterKeyItem>;
            if (lookupList != null)
            {
                if (userType == Constants.UserConstants.Admin)
                    return lookupList.Where(l => l.LanguageCode == languageId && l.KeyType == keyType.ToString() && l.OrderBy > 2).OrderBy(l => l.OrderBy).ToList().ToDictionary(x => x.ResourceKeyID, x => x.KeyValue);
                else
                    return lookupList.Where(l => l.LanguageCode == languageId && l.KeyType == keyType.ToString()).OrderBy(l => l.OrderBy).ToList().ToDictionary(x => x.ResourceKeyID, x => x.KeyValue);
            }

            InvokeLookupLoad(lookup); // Load the lookup and add into cache.
            lookupList = Cache.Get(Enum.GetName(typeof(Lookup), lookup)) as List<LookupMasterKeyItem>; // Again try to get the lookup from Cache.

            if (lookupList != null)
                return lookupList.Where(l => l.LanguageCode == languageId && l.KeyType == keyType.ToString()).OrderBy(l => l.OrderBy).ToList().ToDictionary(x => x.ResourceKeyID, x => x.KeyValue);
            throw new ArgumentException("The specified lookup is not found in Cache.");
        }
        public List<LookupKeyItem> GetLookupLanguageList(Lookup lookup, string languageId, string keyType)
        {
            languageId = languageId.ToLower();
            var lookupList = Cache.Get(Enum.GetName(typeof(Lookup), lookup)) as List<LookupKeyItem>;
            if (lookupList != null)
                return lookupList.Where(l => l.LanguageCode == languageId && (l.KeyType == Constants.String.languageKeyType.Both || l.KeyType == keyType)).ToList();

            InvokeLookupLoad(lookup); // Load the lookup and add into cache.
            lookupList = Cache.Get(Enum.GetName(typeof(Lookup), lookup)) as List<LookupKeyItem>; // Again try to get the lookup from Cache.

            if (lookupList != null)
                return lookupList.Where(l => l.LanguageCode == languageId && (l.KeyType == Constants.String.languageKeyType.Both || l.KeyType == keyType)).ToList();
            throw new ArgumentException("The specified lookup is not found in Cache.");
        }
        public Dictionary<Guid, string> GetLookupLanguage(Lookup lookup, string languageId)
        {
            var lookupList = Cache.Get(Enum.GetName(typeof(Lookup), lookup)) as List<LookupKeyItem>;

            if (lookupList != null)
                return lookupList.Where(x => x.LanguageCode == languageId).ToDictionary(x => x.ResourceKeyID, x => x.KeyValue);

            InvokeLookupLoad(lookup); // Load the lookup and add into cache.
            lookupList = Cache.Get(Enum.GetName(typeof(Lookup), lookup)) as List<LookupKeyItem>; // Again try to get the lookup from Cache.

            if (lookupList != null)
                return lookupList.Where(x => x.LanguageCode == languageId).ToDictionary(x => x.ResourceKeyID, x => x.KeyValue);
            throw new ArgumentException("The specified lookup is not found in Cache.");
        }
        private void InvokeLookupLoad(Lookup lookup, string languageId = "")
        {
            // TODO: Need to create a common service which will return only the required columns.
            switch (lookup)
            {
                case Lookup.Fonts:
                    LoadFonts();
                    break;
                case Lookup.DateFormat:
                    LoadDateFormat();
                    break;
                case Lookup.ExpiryType:
                    LoadExpiryType();
                    break;
                case Lookup.RecipientType:
                    LoadRecipientType();
                    break;
                case Lookup.EnvelopeStatus:
                    LoadEnvelopeStatus();
                    break;
                case Lookup.SignerStatus:
                    LoadSignerStatus();
                    break;
                case Lookup.Controls:
                    LoadControls();
                    break;
                //case Lookup.Templates:
                //    LoadTemplates();
                //    break;               
                case Lookup.MaxCharacters:
                    LoadMaxcharacters();
                    break;
                case Lookup.TextType:
                    LoadTextType(lookup);
                    break;
                //case Lookup.ReminderDropdownOptions:
                //    LoadReminderDropdown();
                //    break;
                //case Lookup.ShowSettingsAccessibility:
                //    LoadSettingsAccessibility();
                //    break;
                //case Lookup.TimeZone:
                //    LoadTimeZoneDropdown();
                //    break;
                case Lookup.CheckboxConditions:
                    LoadCheckboxConditions(languageId);
                    break;
                case Lookup.InitialConditions:
                    LoadInitialConditions(languageId);
                    break;
                case Lookup.TextConditions:
                    LoadTextConditions(languageId);
                    break;
                //case Lookup.AccessAuthenticationSettingType:
                //    LoadAccessSettingType();
                //    break;
                case Lookup.LanguageKeyDetails:
                    LoadLanguageKeyMapping();
                    break;
                case Lookup.MasterLanguageKeyDetails:
                    LoadMasterLanguageKeyMapping();
                    break;
                    //case Lookup.SettingsDisplayOptions:
                    //    LoadSettingsDisplayOptions();
                    //    break;
                    //case Lookup.DatetoSignedDocNameSettingsOptions:
                    //    LoadSettingsDateFormattoSignedDocumentNameOptions();
                    //    break;

            }
        }
        private void LoadControls()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var controls = (from control in dbContext.Control
                                select control).ToList();

                var lookupList = new List<LookupItem>();
                foreach (var control in controls.OrderBy(x => x.ID))
                    lookupList.Add(new LookupItem { Key = control.ID.ToString(), Value = control.ControlName, IsActive = true });
                AddToCache(lookupList, Lookup.Controls);
            }
        }
        private void LoadFonts()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var fonts = dbContext.FontList.ToList();
                var lookupList = new List<LookupItem>();
                foreach (var font in fonts.OrderBy(x => x.ID))
                    lookupList.Add(new LookupItem { Key = font.ID.ToString(), Value = font.FontFamily, IsActive = false });
                AddToCache(lookupList, Lookup.Fonts);
            }
        }
        private void LoadCheckboxConditions(string languageId)
        {
            Guid ID = GetLanguageID(languageId);
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var checkboxRuleConfig = dbContext.RuleConfLanguageMapping.Where(r => r.RuleTextFor == Constants.Control.Checkbox && r.LanguageID == ID).OrderBy(r => r.DisplayOrder).ToList();
                var lookupList = new List<LookupItem>();
                foreach (var crc in checkboxRuleConfig)
                {
                    lookupList.Add(new LookupItem { Key = Convert.ToString(crc.MasterReferenceKeyID), Value = crc.RuleText, IsActive = false });
                }
                AddToCache(lookupList, Lookup.CheckboxConditions);
            }
        }
        private void LoadInitialConditions(string languageId)
        {
            Guid ID = GetLanguageID(languageId);
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var initialRuleConfig = dbContext.RuleConfLanguageMapping.Where(r => r.RuleTextFor == Constants.Control.Initials && r.LanguageID == ID).OrderBy(r => r.DisplayOrder).ToList();
                var lookupList = new List<LookupItem>();
                foreach (var irc in initialRuleConfig)
                {
                    lookupList.Add(new LookupItem { Key = Convert.ToString(irc.MasterReferenceKeyID), Value = irc.RuleText, IsActive = false });
                }
                AddToCache(lookupList, Lookup.InitialConditions);
            }
        }
        private void LoadMasterLanguageKeyMapping()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var lookupList = (from vw in dbContext.vw_MasterlanguageMapping
                                  select new LookupMasterKeyItem
                                  {
                                      ResourceKeyID = vw.ResourceKey,
                                      KeyName = vw.KeyName,
                                      LanguageCode = vw.LanguageCode,
                                      KeyValue = vw.KeyValue,
                                      KeyType = vw.LookupName,
                                      OrderBy = vw.OrderBy
                                  }).ToList();

                AddMasterKeyToCache(lookupList, Lookup.MasterLanguageKeyDetails);
            }
        }
        private void AddToCache(List<LookupItem> lookupList, Lookup actiontypeLookup)
        {
            var lookupName = Enum.GetName(typeof(Lookup), actiontypeLookup);
            if (!Cache.IsSet(lookupName))
                Cache.Set(lookupName, lookupList, 30);
        }
        private void AddMasterKeyToCache(List<LookupMasterKeyItem> lookupList, Lookup actiontypeLookup)
        {
            var lookupName = Enum.GetName(typeof(Lookup), actiontypeLookup);
            if (!Cache.IsSet(lookupName))
                Cache.Set(lookupName, lookupList, 30);
        }

        private Guid GetLanguageID(string languageId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                if (!string.IsNullOrEmpty(languageId))
                    return dbContext.Language.Where(l => l.LanguageCode == languageId).FirstOrDefault().ID;
                else
                {
                    return Constants.Languages.English;
                }
            }
        }
        private void LoadTextConditions(string languageId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                Guid ID = GetLanguageID(languageId);
                var textRuleConfig = dbContext.RuleConfLanguageMapping.Where(r => r.RuleTextFor == Constants.Control.Text && r.LanguageID == ID).OrderBy(r => r.DisplayOrder).ToList();
                var lookupList = new List<LookupItem>();
                foreach (var trc in textRuleConfig)
                {
                    lookupList.Add(new LookupItem { Key = Convert.ToString(trc.MasterReferenceKeyID), Value = trc.RuleText, IsActive = false });
                }
                AddToCache(lookupList, Lookup.TextConditions);
            }
        }
        private void LoadMaxcharacters()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                cultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
                var maxCharacter = dbContext.MaxCharacter.ToList();
                var lookupList = new List<LookupItem>();
                foreach (var item in maxCharacter.OrderBy(a => a.TextLength).ToList())
                {
                    lookupList.Add(new LookupItem { Key = item.ID.ToString(), Value = Convert.ToString(item.TextLength), IsActive = false });
                }
                AddToCache(lookupList, Lookup.MaxCharacters);
            }
        }
        private void LoadLanguageKeyMapping()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var lookupList = (from vw in dbContext.vw_LanguageKeyMapping
                                  select new LookupKeyItem
                                  {
                                      ResourceKeyID = vw.ResourceKeyID,
                                      KeyName = vw.KeyName,
                                      LanguageCode = vw.LanguageCode,
                                      KeyValue = vw.KeyValue,
                                      KeyType = vw.KeyType,
                                      PageName = vw.PageName
                                  }).ToList();

                AddKeyToCache(lookupList, Lookup.LanguageKeyDetails);
            }
        }
        private void AddKeyToCache(List<LookupKeyItem> lookupList, Lookup actiontypeLookup)
        {
            var lookupName = Enum.GetName(typeof(Lookup), actiontypeLookup);
            if (!Cache.IsSet(lookupName))
                Cache.Set(lookupName, lookupList, 30);
        }
        public LanguageKeyDetails GetLookupLanguageKeyDetails(Guid? ResourceKeyID, string languageCode)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var lookUpitems = (from lkDetails in dbContext.LanguageKeyDetails
                                   join l in dbContext.Language on lkDetails.LanguageID equals l.ID
                                   where l.LanguageCode == languageCode
                                   && lkDetails.ResourceKeyID == ResourceKeyID
                                   select lkDetails).FirstOrDefault();
                return lookUpitems;
            }
        }
        private void LoadTextType(Lookup lookup)
        {
            RemoveLookup(lookup);
            cultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
            using (var dbContext = new RSignDbContext(_configuration))
            {

                var controlType = (from tt in dbContext.TextType
                                   where tt.CultureInfo == cultureInfo
                                   select new { tt.ID, tt.Type, tt.IsSelected }).ToList();
                var lookupList = new List<LookupItem>();
                foreach (var item in controlType)
                {
                    lookupList.Add(new LookupItem
                    {
                        Key = item.ID.ToString(),
                        Value = Convert.ToString(item.Type),
                        IsActive = item.IsSelected.HasValue ? item.IsSelected.Value : false
                    });
                }
                AddToCache(lookupList, Lookup.TextType);
            }
        }
        public void RemoveLookup(Lookup lookup)
        {
            Cache.Invalidate(Enum.GetName(typeof(Lookup), lookup));
        }
        private void LoadRecipientType()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var recipientTypes = (from recipientType in dbContext.RecipientType
                                      select recipientType).ToList();

                var lookupList = new List<LookupItem>();
                foreach (var recipientType in recipientTypes.OrderBy(x => x.ID))
                    lookupList.Add(new LookupItem { Key = recipientType.ID.ToString(), Value = recipientType.Description, IsActive = true });
                AddToCache(lookupList, Lookup.RecipientType);
            }
        }
        private void LoadSignerStatus()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var statuses = (from status in dbContext.Status
                                where status.Type == 2
                                select status).ToList();

                var lookupList = new List<LookupItem>();
                foreach (var status in statuses.OrderBy(x => x.ID))
                    lookupList.Add(new LookupItem { Key = status.ID.ToString(), Value = status.Description, IsActive = true });
                AddToCache(lookupList, Lookup.SignerStatus);
            }
        }
        private void LoadEnvelopeStatus()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var statuses = (from status in dbContext.Status
                                where status.Type == 1
                                select status).ToList();

                var lookupList = new List<LookupItem>();
                foreach (var status in statuses.OrderBy(x => x.ID))
                    lookupList.Add(new LookupItem { Key = status.ID.ToString(), Value = status.Description, IsActive = true });
                AddToCache(lookupList, Lookup.EnvelopeStatus);
            }
        }
        private void LoadDateFormat()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var dateFormats = (from dateFormat in dbContext.DateFormat
                                   select dateFormat).ToList();

                var lookupList = new List<LookupItem>();
                foreach (var dateFormat in dateFormats.OrderByDescending(x => x.Order))
                    lookupList.Add(new LookupItem { Key = dateFormat.ID.ToString(), Value = dateFormat.Description, IsActive = true });
                AddToCache(lookupList, Lookup.DateFormat);
            }
        }

        /// <summary>
        /// load expiry type
        /// </summary>
        private void LoadExpiryType()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var expiryTypes = (from expiryType in dbContext.ExpiryType
                                   select expiryType).ToList();

                var lookupList = new List<LookupItem>();
                foreach (var expiryType in expiryTypes.OrderBy(x => x.Order))
                    lookupList.Add(new LookupItem { Key = expiryType.ID.ToString(), Value = expiryType.Description, IsActive = true });
                AddToCache(lookupList, Lookup.ExpiryType);
            }
        }
        public Dictionary<Guid, string> GetLanguageKeyDetailsFromJson(string languageId)
        {
            string KeyIdDescriptionPath = Convert.ToString(_appConfiguration["KeyIdDescriptionPath"]);
            Dictionary<Guid, string> LanguagelayoutList = new Dictionary<Guid, string>();
            string[] filePaths = Directory.GetFiles(KeyIdDescriptionPath);
            string languageKeyDetailsFile = (filePaths != null && filePaths.Count() > 0) ? filePaths.Where(x => x.Contains(languageId)).ToList().FirstOrDefault() : null;
            if (languageKeyDetailsFile != null)
            {
                StreamReader reader = new StreamReader(languageKeyDetailsFile);
                var languageKeyDetailsJson = reader.ReadToEnd();
                LanguagelayoutList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<Guid, string>>(languageKeyDetailsJson);
            }
            return LanguagelayoutList;
        }
        public Dictionary<string, string> GetLanguageKeyNameDesc(string languageId)
        {
            string KeyNameDescriptionPath = Convert.ToString(_appConfiguration["KeyNameDescriptionPath"]);
            Dictionary<string, string> KeyNameDescriptionList = new Dictionary<string, string>();
            string[] filePaths = Directory.GetFiles(KeyNameDescriptionPath);
            string languageKeyDetailsFile = (filePaths != null && filePaths.Count() > 0) ? filePaths.Where(x => x.Contains(languageId)).ToList().FirstOrDefault() : null;
            if (languageKeyDetailsFile != null)
            {
                StreamReader reader = new StreamReader(languageKeyDetailsFile);
                var languageKeyDetailsJson = reader.ReadToEnd();
                KeyNameDescriptionList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(languageKeyDetailsJson);
            }
            return KeyNameDescriptionList;
        }
    }
}
