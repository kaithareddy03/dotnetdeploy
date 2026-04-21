using RSign.Models.APIModels.Data;

namespace RSign.Models.APIModels
{
    class APILanguageDetails
    {
    }
    /// <summary>
    /// Entity to get and set LayoutPage
    /// </summary>
    public class LayoutPage
    {
        /// <summary>
        /// Get/Set Home
        /// </summary>
        public string? Home { get; set; }
        /// <summary>
        /// Get/Set Send
        /// </summary>
        public string? Send { get; set; }
        /// <summary>
        /// Get/Set Manage
        /// </summary>
        public string? Manage { get; set; }
        /// <summary>
        /// Get/Set Templates
        /// </summary>
        public string? Templates { get; set; }
        /// <summary>
        /// Get/Set Draft
        /// </summary>
        public string? Draft { get; set; }
        /// <summary>
        /// Get/Set Stats
        /// </summary>
        public string? Stats { get; set; }
        /// <summary>
        /// Get/Set Admin
        /// </summary>
        public string? Admin { get; set; }
        /// <summary>
        /// Get/Set Settings
        /// </summary>
        public string? Settings { get; set; }
        /// <summary>
        /// Get/Set Logout
        /// </summary>
        public string? Logout { get; set; }
        /// <summary>
        /// Get/Set Hello
        /// </summary>
        public string? Hello { get; set; }
        /// <summary>
        /// Get/Set LegalNotice
        /// </summary>
        public string? LegalNotice { get; set; }
        /// <summary>
        /// Get/Set FooterTextLegalNoticeLink
        /// </summary>
        public string? FooterTextLegalNoticeLink { get; set; }
        /// <summary>
        /// Get/Set RPostTechnologies
        /// </summary>
        public string? RPostTechnologies { get; set; }
        /// <summary>
        /// Get/Set FooterTextRpostTechnologyLink
        /// </summary>
        public string? FooterTextRpostTechnologyLink { get; set; }
    }
    public class APILookupDetails
    {
        public List<LookupItem> LookupItemList { get; set; }
        public List<LookupKeyItem> LookupKeyItemList { get; set; }
        public List<LookupMasterKeyItem> LookupMasterKeyItemList { get; set; }
    }
}
