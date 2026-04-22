using RSign.Common.Enums;
using RSign.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface ISettingsRepository
    {
        APISettings GetEntityByParam(Guid userID, string userEmail, string settingsForType);
        SettingsDetail saveDefaultSettingEntity(string keyName, Guid keyConfig, Guid loggedInUserId, Guid settingsFor, string settingsForType);
        SettingsDetail GetEntityForByKeyConfig(Guid settingsFor, Guid keyConfig);
        bool saveDefaultSetting(Guid loggedInUserId, Guid settingsFor, string settingsForType, string companyName = "");
        SettingsExternalIntegration GetExternalSettingsByType(Guid userID, string Type);
        string GetSettingsValue(string keyValue, Lookup lookupName, string fieldName, int flagValue, string LanguageID);
        SettingsDetail GetEntityForByKeyConfig(Guid settingsFor, Guid keyConfig, string SettingsForType);
        string GetNonRegisteredUserDefaultLandingPage(Guid userID, Guid nonRegisteredUserDefaultLandingPage);
        string GetRegisteredUserDefaultLandingPage(Guid userID, Guid registeredUserDefaultLandingPage);
    }
}
