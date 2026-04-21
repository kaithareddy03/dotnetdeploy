
using eSign.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace RSign.Models.APIModels
{
    public class APISettingDetails
    {

    }
    /// <summary>
    /// This will contain settings for user/comapny
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// This will return logged in user user type id
        /// </summary>
        public Guid? CurrentLoggedInUserTypeID { get; set; }
        /// <summary>
        /// This will return logged in user email address
        /// </summary>
        public string? CurrentLoggedInUserEmail { get; set; }
        /// <summary>
        /// This will return list of companies
        /// </summary>
        public List<SelectListItem> Companies { get; set; }
        /// <summary>
        /// This will return search user
        /// </summary>
        public string? SearchUser { get; set; }
        /// <summary>
        /// This will return selected company to change settings
        /// </summary>
        public string? SelectedCompany { get; set; }
        /// <summary>
        /// This will return if company search is done or personal setting page is open
        /// </summary>
        public bool IsCompanySearchActive { get; set; }
        /// <summary>
        /// This will return Admin, General And System Settings.
        /// </summary>
        public AdminGeneralAndSystemSettings AdminGeneralAndSystemSettings { get; set; }
        /// <summary>
        /// This will return the UserProfile for the settings
        /// </summary>
        public UserProfile UserProfile { get; set; }
        /// <summary>
        /// This will return the SelectedCompanyId for the settings
        /// </summary>
        public string? SelectedCompanyId { get; set; }

    }
    /// <summary>
    /// This will return/set Setting Details for User or Company
    /// </summary>
    public class APISettings
    {
        /// <summary>
        /// Get/Set Uesr's Email in case of user Setting and null for Company Settings.
        /// </summary>
        public string? UserEmail { get; set; }
        /// <summary>
        /// Get/Set settings for user/company (UserID/CompanyID)
        /// </summary>
        public Guid SettingsFor { get; set; }
        /// <summary>
        /// Get/Set type of settings for User/Company.(Type is available in Master API)
        /// </summary>
        public string? SettingsForType { get; set; }
        /// <summary>
        /// This will return setting details in key value pair
        /// </summary>
        public Dictionary<Guid, KeyPairItem> SettingDetails { get; set; }
        public bool IsDefaultCompanyUser { get; set; }
        public List<APIHeaderFooterSettings> HeaderFooterSettingsList { get; set; }
        /// <summary>
        ///  This will return reference Key For Integration
        /// </summary>
        public string? referenceKeyForIntegration { get; set; }
        /// <summary>
        /// This is used to identify the group
        /// </summary>
        public int TabId { get; set; }
        /// <summary>
        /// This will return the Company name for the company
        /// </summary>
        public string? CompanyName { get; set; }
        /// <summary>
        /// This will return list of settings properties
        /// </summary>
        public List<SettingProperties> SettingProperties { get; set; }
        /// <summary>
        /// This will return the company details
        /// </summary>
        public CompanySettings CompanySettings { get; set; }
        /// <summary>
        /// This will return the CompanyReferenceKey 
        /// </summary>
        public string? CompanyReferenceKey { get; set; }

        public string? SendingConfirmEmail { get; set; }
        public UserPlan UserPlanDetails { get; set; }
    }
    public class CompanySettings
    {
        /// <summary>
        /// This will return the Company language code
        /// </summary>
        public string? LanguageCode { get; set; }
    }
    public class KeyPairItem
    {
        public KeyPairItem()
        {

        }
        /// <summary>
        /// Get/Set Show on Setting tab property.
        /// </summary>
        public int ShowOnSettingsTab { get; set; }
        /// <summary>
        /// Get/Set value for settings property
        /// </summary>
        public string? SettingValue { get; set; }
        /// <summary>
        /// This will set the IsLock property
        /// </summary>
        public Nullable<bool> IsLock { get; set; }
        /// <summary>
        /// This will set the IsOverride property
        /// </summary>
        public Nullable<bool> IsOverride { get; set; }
        /// <summary>
        /// This will set the IsLockChanged property
        /// </summary>
        public Nullable<bool> IsLockChanged { get; set; }
        /// <summary>
        /// This will set the UpdatedSettingName property
        /// </summary>
        public string? UpdatedSettingName { get; set; }
        public KeyPairItem(int _showOnSettingsTab, string _settingValue, bool? _isLock, bool? _isOverride)
        {
            ShowOnSettingsTab = _showOnSettingsTab;
            SettingValue = _settingValue;
            IsLock = _isLock;
            IsOverride = _isOverride;
        }
    }


    /// <summary>
    /// This class contains Admin, GeneralAndSystemSettings
    /// </summary>
    public class AdminGeneralAndSystemSettings
    {
        /// <summary>
        /// This will return ID.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// This will return UserID.
        /// </summary>
        public Guid? UserID { get; set; }
        /// <summary>
        /// This will return CompanyId.
        /// </summary>
        public Guid? CompanyID { get; set; }
        /// <summary>
        /// This will return IsCompany Search active or not.
        /// </summary>
        public bool IsCompanySearchActive { get; set; }
        /// <summary>
        /// This will return SettingTabId.
        /// </summary>
        public Guid? ShowSettingsTabSelected { get; set; }
        /// <summary>
        /// This will return ShowSettingsTabDropdown.
        /// </summary>
        public List<SelectListItem> ShowSettingsTabDropdown { get; set; }
        /// <summary>
        /// This will return OverrideUserSettings.
        /// </summary>
        public bool OverrideUserSettings { get; set; }
        /// <summary>
        /// This will return DateFormatDisplayOptionSelected.
        /// </summary>
        public int DateFormatDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return DateFormatID.
        /// </summary>
        public Guid DateFormatID { get; set; }
        /// <summary>
        /// This will return DateFormats.
        /// </summary>
        public List<SelectListItem> DateFormats { get; set; }
        /// <summary>
        /// This will return ExpiresInDisplayOptionSelected.
        /// </summary>
        public int ExpiresInDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return ExpiresInID.
        /// </summary>
        public Guid ExpiresInID { get; set; }
        /// <summary>
        /// This will return ExpiresIn.
        /// </summary>
        public List<SelectListItem> ExpiresIn { get; set; }
        /// <summary>
        /// This will return SendReminderInDisplayOptionSelected.
        /// </summary>
        public int SendReminderInDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return SendReminderIn.
        /// </summary>
        public int SendReminderIn { get; set; }
        /// <summary>
        /// This will return SendReminderInDropdownSelected.
        /// </summary>
        public Guid SendReminderInDropdownSelected { get; set; }
        /// <summary>
        /// This will return ThenSendReminderInDisplayOptionSelected.
        /// </summary>
        public int ThenSendReminderInDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return ThenSendReminderIn.
        /// </summary>
        public int ThenSendReminderIn { get; set; }
        /// <summary>
        /// This will return ThenSendReminderInDropdownSelected.
        /// </summary>
        public Guid ThenSendReminderInDropdownSelected { get; set; }
        /// <summary>
        /// This will return RemindersDropdown.
        /// </summary>
        public List<SelectListItem> RemindersDropdown { get; set; }
        /// <summary>
        /// This will return IncludeSignedCertificateOnSignedPDFDisplayOptionSelected.
        /// </summary>
        public int IncludeSignedCertificateOnSignedPDFDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return IncludeSignedCertificateOnSignedPDF.
        /// </summary>
        public bool IncludeSignedCertificateOnSignedPDF { get; set; }
        /// <summary>
        /// This will return SignInSequenceDisplayOptionSelected.
        /// </summary>
        public int SignInSequenceDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return SignInSequence.
        /// </summary>
        public bool SignInSequence { get; set; }
        /// <summary>
        /// This will return StoredSignedPDFDisplayOptionSelected.
        /// </summary>
        public int StoredSignedPDFDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return StoredSignedPDF.
        /// </summary>
        public bool StoredSignedPDF { get; set; }
        /// <summary>
        /// This will return IncludeTransparencyDocDisplayOptionSelected.
        /// </summary>
        public int IncludeTransparencyDocDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return IncludeTransparencyDoc.
        /// </summary>
        public bool IncludeTransparencyDoc { get; set; }
        /// <summary>
        /// This will return StorageDriveLocal.
        /// </summary>
        public bool StorageDriveLocal { get; set; }
        /// <summary>
        /// This will return StorageDriveGoogle.
        /// </summary>
        public bool StorageDriveGoogle { get; set; }
        /// <summary>
        /// This will return StorageDriveDropbox.
        /// </summary>
        public bool StorageDriveDropbox { get; set; }
        /// <summary>
        /// This will return StorageDriveSkydrive.
        /// </summary>
        public bool StorageDriveSkydrive { get; set; }
        ///  <summary>
        /// This will return StorageiManage.
        /// </summary>
        public bool StorageiManage { get; set; }
        /// <summary>
        /// This will return Storage netDocuments.
        /// </summary>
        public bool Storagenetdocuments { get; set; }
        ///  <summary>
        /// This will return StorageiManage.
        /// </summary>
        public bool StorageAppliedEpic { get; set; }
        /// <summary>
        /// This will return Storage netDocuments.
        /// </summary>
        public bool StorageBullhorn { get; set; }

        public bool StorageVincere { get; set; }
        /// <summary>
        /// This will return SignatureCaptureType.
        /// </summary>
        public bool SignatureCaptureType { get; set; }
        /// <summary>
        /// This will return SignatureCaptureHanddrawn.
        /// </summary>
        public bool SignatureCaptureHanddrawn { get; set; }
        /// <summary>
        /// This will return IsTimeZoneUTC.
        /// </summary>
        public bool IsTimeZoneUTC { get; set; }
        /// <summary>
        /// This will return SelectedTimeZone.
        /// </summary>
        public string? SelectedTimeZone { get; set; }
        /// <summary>
        /// This will return TimeZone name.
        /// </summary>
        public List<SelectListItem> TimeZone { get; set; }
        /// <summary>
        /// This will return selected Disclaimer
        /// </summary>
        public bool IsDisclaimerEnabled { get; set; }
        /// <summary>
        /// This will return Selected Disclaimer
        /// </summary>       
        public string? Disclaimer { get; set; }

        /// <summary>
        /// This will return Selected Email Disclaimer
        /// </summary>
        public string? EmailDisclaimer { get; set; }
        /// <summary>
        /// This will return selected Trace settings
        /// </summary>
        public bool IsDeleteSignedContracts { get; set; }
        /// <summary>
        /// This will return Rule Setting
        /// </summary>
        public bool IsCreateRules { get; set; }
        /// <summary>
        /// This will return IsIncludeSignerAttachFileDisplayOptionSelected.
        /// </summary>
       // public int IsIncludeSignerAttachFileDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return Signer Attach File Setting
        /// </summary>
        public int IncludeSignerAttachFile { get; set; }

        public List<SelectListItem> SignerAttachmentOptions { get; set; }
        public int AllowRecipeintToAttachFileOptionSelected { get; set; }
        public bool AllowRecipeintToAttachFile { get; set; }

        /// <summary>
        /// This will return Form Field Alignment Setting
        /// </summary>
        public bool IsFormFieldAlignmentEnabled { get; set; }
        /// <summary>
        /// This will return Create Static Link Setting 
        /// </summary>
        public bool IncludeCreateStaticLink { get; set; }
        /// <summary>
        /// This will return Create Static Link Setting enabled
        /// </summary>
        public int CreateStaticLinkDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return Add XML Data Setting
        /// </summary>
        public bool IncludeAddXMLData { get; set; }
        /// <summary>
        /// This will return Add XML Data Setting enabled
        /// </summary>
        public int IsAddXMLDataDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return SeparateMultipleDocumentsAfterSigningDisplayOptionSelected.
        /// </summary>
        public int SeparateMultipleDocumentsAfterSigningDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return SeparateMultipleDocumentsAfterSigning.
        /// </summary>
        public bool SeparateMultipleDocumentsAfterSigning { get; set; }
        /// <summary>
        /// This will return IsDisclaimerInCertificate.
        /// </summary>
        public bool? IsDisclaimerInCertificate { get; set; }

        /// <summary>
        /// This will return AccessAuthentication flag for envelope
        /// </summary>
        public int AccessAuthenticatedDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return AccessAuthentication value
        /// </summary>
        public Guid AccessAuthenticationId { get; set; }
        /// <summary>
        /// This will return AccessAuthenticationType.
        /// </summary>
        public List<SelectListItem> AccessAuthenticationType { get; set; }
        /// <summary>
        /// This will return Default Password
        /// </summary>
        public string? AccessPassword { get; set; }
        /// <summary>
        /// This will return if sender want to share password to signer
        /// </summary>
        public bool IsAccessCodeSendToSignerEnabled { get; set; }
        /// <summary>
        /// This will return If user want to store email body for envelopes
        /// </summary>
        public bool StoreEmailBody { get; set; }
        /// <summary>
        /// This will return If send want to delete email body for perticular envelope
        /// </summary>
        public bool AllowUserToDeleteEmailBody { get; set; }
        /// <summary>
        /// This will return Key and value for Multilingual
        /// </summary>
        public Dictionary<Guid?, string> DicLabelText { get; set; }
        /// <summary>
        /// This will return Final Contract Option.
        /// </summary>
        public List<SelectListItem> FinalContractOptions { get; set; }
        /// <summary>
        /// This will return FinalContractOptionID
        /// </summary>
        public int FinalContractOptionID { get; set; }
        /// This will return If sender want to make signature required default
        /// </summary>
        public bool SignatureControlRequired { get; set; }
        /// <summary>
        /// This will return EsignMailCopyAddress
        /// </summary>       
        public string? EsignMailCopyAddress { get; set; }
        /// <summary>
        /// This will return EsignMailRerouteAddress
        /// </summary>       
        public string? EsignMailRerouteAddress { get; set; }
        /// <summary>
        /// This will return If send want receive sending email confirmation
        /// </summary>
        public bool ReceiveSendingEmailConfirmation { get; set; }
        /// <summary>
        /// This will return Attach Signed PDF Option.
        /// </summary>
        public List<SelectListItem> AttachSignedPdf { get; set; }
        /// <summary>
        /// This will return AttachSignedPdfID
        /// </summary>
        public Guid AttachSignedPdfID { get; set; }
        /// This will return Header Footer Setting Option.
        /// </summary>
        public List<SelectListItem> HeaderFooterSettingOptions { get; set; }
        /// <summary>
        /// This will return HeaderFooterSettingID
        /// </summary>
        public int HeaderFooterSettingID { get; set; }
        /// <summary>
        /// This will return Post-Signing LandingPage Setting
        /// </summary>
        public bool IsPostSigningLandingPage { get; set; }
        /// <summary>
        /// This will return setting to include envelope data (XML) in the Manage Tab
        /// </summary>
        public bool IsIncludeEnvelopeXmlData { get; set; }
        /// <summary>
        /// This will return Allow Template Editing is Active/ Disable/View Only
        /// </summary>
        public int IsAllowTemplateEditingDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return AllowTemplateEditing.
        /// </summary>
        public bool AllowTemplateEditing { get; set; }
        /// <summary>
        /// This will return setting to enable login popup after signing.
        /// </summary>
        public bool EnablePostSigningLoginPopup { get; set; }
        /// <summary>
        /// This will return Create message template Setting
        /// </summary>
        public bool IsCreateMessageTemplate { get; set; }

        /// <summary>
        /// Settings display options
        /// </summary>
        public List<SelectListItem> SettingDisplayOptions { get; set; }

        /// <summary>
        /// This will return SendIndividualSignatureNotificationsOptionSelected.
        /// </summary>
        public int SendIndividualSignatureNotificationsOptionSelected { get; set; }
        /// <summary>
        /// This will return IncludeSignedCertificateOnSignedPDF.
        /// </summary>
        public bool SendIndividualSignatureNotifications { get; set; }
        /// <summary>
        /// This will return Date format Option to add to signed document name.
        /// </summary>
        public List<SelectListItem> DatetoSignedDocNameSettingsOptions { get; set; }
        /// <summary>
        /// This will return Date format Option Id
        /// </summary>
        public int DatetoSignedDocNameSettingsOptionID { get; set; }

        /// <summary>
        /// This will return SendFinalReminderBeforeExpirationDisplayOptionSelected.
        /// </summary>
        public int SendFinalReminderBeforeExpDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return SendFinalReminderBeforeExpiration.
        /// </summary>
        public int SendFinalReminderBeforeExp { get; set; }

        /// <summary>
        /// This will return RemindersDropdown for Then Send Reminder every
        /// </summary>
        public List<SelectListItem> ThenRemindersDropdown { get; set; }
        /// <summary>
        /// This will return Default Signature is required to sign for static template required
        /// </summary>
        public bool IncludeDefaultSignReqForStaticTemplate { get; set; }
        /// <summary>
        /// This will return Create Static Link Setting enabled
        /// </summary>
        public int DefaultSignReqForStaticTemplateDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return if Enable Out Of Office Mode setting is enable
        /// </summary>
        public bool IsOutOfOfficeModeEnable { get; set; }
        /// <summary>
        /// This will return Date Range First Day setting value
        /// </summary>
        public DateTime? DateRangeFirstDay { get; set; }
        /// <summary>
        /// This will return Date Range Last Day setting value
        /// </summary>
        public DateTime? DateRangeLastDay { get; set; }
        /// <summary>
        /// This will return Copy Email Address setting value
        /// </summary>
        public string? CopyEmailAddrForOutOfOfficeMode { get; set; }
        /// <summary>
        /// This will return re-route Email Address setting value
        /// </summary>
        public string? RerouteEmailAddrForOutOfOfficeMode { get; set; }
        /// <summary>
        /// This will return Enable Dependencies Feature setting value
        /// </summary>
        public bool EnableDependenciesFeature { get; set; }
        /// <summary>
        /// This will return Enable Dependencies Feature setting's display option value 
        /// </summary>
        public int EnableDependenciesFeatureDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return 
        /// </summary>
        public List<SelectListItem> ReferenceCodeSettingOptions { get; set; }
        /// <summary>
        /// This will return 
        /// </summary>
        public int ReferenceCodeSettingID { get; set; }
        /// <summary>
        /// This will return Sign Multiple Document Independently
        /// </summary>
        public bool SignMultipleDocumentIndependently { get; set; }
        /// <summary>
        /// This will return true for SendInvitationEmailToSigner
        /// </summary>
        public List<SelectListItem> SendInvitationEmailToSigner { get; set; }
        /// <summary>
        /// This will return true for SendInvitationEmailToSignerID
        /// </summary>
        /// 
        public int SendInvitationEmailToSignerID { get; set; }
        /// <summary>
        /// This will return Enable intergration access setting value
        /// </summary>
        public bool EnableIntegrationAccess { get; set; }
        /// <summary>
        ///  This will return reference Key For Integration
        /// </summary>
        public string? referenceKeyForIntegration { get; set; }
        /// <summary>
        /// This will return Document Size LookupItem
        /// </summary>
        public List<SelectListItem> DocumentPaperSize { get; set; }
        /// <summary>
        /// This will return Document Size ID
        /// </summary>
        public int DocumentPaperSizeID { get; set; }
        /// <summary>
        /// This will return Stamp on Signer Copy
        /// </summary>
        public bool StampOnSignerCopySetting { get; set; }
        /// <summary>
        /// This will return Authorize text
        /// </summary>
        public string? StampOnSignerCopyAuthrozieText { get; set; }
        /// <summary>
        /// This will return watermark text
        /// </summary>
        public string? StampOnSignerCopyWatermarkText { get; set; }
        /// This will return Electronic Sign Indication Options 
        /// </summary>
        public List<SelectListItem> ElectronicSignIndicationOptions { get; set; }
        /// <summary>
        /// This will return Electronic Sign Indication selected ID
        /// </summary>
        public int ElectronicSignIndicationSelectedID { get; set; }
        /// <summary>
        /// This will return Decline Template Title Setting
        /// </summary>
        public string? DeclineTemplateTitleSetting { get; set; }
        /// <summary>
        /// This will return Decline template controls options
        /// </summary>
        public List<SelectListItem> DeclineTemplateControlsSettingOptions { get; set; }
        /// <summary>
        /// This will return Decline template controls selected control ID
        /// </summary>
        public int DeclineTemplateControlsSelectedID { get; set; }
        /// <summary>
        /// This will return Decline template reasons
        /// </summary>
        public string? DeclineTemplateReasonsSettings { get; set; }
        /// <summary>
        /// This will return list of Decline Template responses 
        /// </summary>
        public List<DeclineTemplateResponses> DeclineTemplateResponsesList { get; set; }

        /// <summary>
        /// This will return string of Signature Request Reply to Address settings
        /// </summary>
        public List<SelectListItem> SignatureRequestReplyAddress { get; set; }

        /// <summary>
        /// This will return ID of Signature Request Reply to Address settings
        /// </summary>
        public int SignReqReplyAddSettingsID { get; set; }

        /// <summary>
        /// This will return Show RSign Logo
        /// </summary>
        public bool ShowRSignLogo { get; set; }

        /// <summary>
        /// This will return Show Company Logo
        /// </summary>
        public bool ShowCompanyLogo { get; set; }

        /// <summary>
        /// This will return Background Color code of Email banner
        /// </summary>
        public string? EmailBannerBackgroundColor { get; set; }

        /// <summary>
        /// This will return Font Color code of Email banner
        /// </summary>
        public string? EmailBannerFontColor { get; set; }

        /// <summary>
        /// This will return Company Logo Image of Email banner
        /// </summary>
        public string? CompanyLogoImage { get; set; }
        /// <summary>
        /// This will return Company Logo Image in bytes format
        /// </summary>
        public string? CompanyLogoBytes { get; set; }
        public string? PrivateModeSettings { get; set; }
        public List<PrivateModeSettings> PrivateModeSettingsValues { get; set; }

        /// <summary>
        /// This will return StoreOriginalDocumentSelected.
        /// </summary>
        public int StoreOriginalDocumentSelected { get; set; }
        /// <summary>
        /// This will return StoreOriginalDocument.
        /// </summary>
        public bool? StoreOriginalDocument { get; set; }
        /// <summary>
        /// This will return StoredSignedPDFDisplayOptionSelected.
        /// </summary>
        public int StoreSignatureCertificateSelected { get; set; }
        /// <summary>
        /// This will return StoreSignatureCertificate.
        /// </summary>
        public bool? StoreSignatureCertificate { get; set; }
        /// <summary>
        /// This will return orginal document to be deleted or not
        /// </summary>
        public bool DeleteOriginalDocument { get; set; }
        /// <summary>
        /// This will return AllowBulkSending.
        /// </summary>
        public bool AllowBulkSending { get; set; }
        /// <summary>
        /// This will return list of settings properties
        /// </summary>
        public List<SettingProperties> SettingProperties { get; set; }
        /// <summary>
        /// This will return the company details
        /// </summary>
        public CompanySettings CompanySettings { get; set; }
        /// <summary>
        /// This will return Post signing page flag
        /// </summary>
        public int PostSigningUrlDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return Post signing page url
        /// </summary>
        public string? PostSigningPageUrl { get; set; }
        /// <summary>
        /// This will return Envelope Post signing page flag
        /// </summary>
        public int EnvelopePostSigningUrlDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return Envelope Post signing page url
        /// </summary>
        public string? EnvelopePostSigningPageUrl { get; set; }
        /// <summary>
        /// This will return Envelope Post-Signing LandingPage Setting
        /// </summary>
        public bool IsEnvelopePostSigningPage { get; set; }

        public SettingsExternalIntegration settingsExternalIntegration { get; set; }

        public bool? EnableFileReview { get; set; }
        /// <summary>
        /// This will return Show Control ID Setting
        /// </summary>
        public bool? ShowControlID { get; set; }
        /// <summary>
        /// This will return Allow Rule Editing Setting
        /// </summary>
        public bool? AllowRuleEditing { get; set; }
        /// <summary>
        /// This will return Allow Rule Editing is Active/ Disable/View Only
        /// </summary>
        public int IsAllowRuleEditingDisplayOptionSelected { get; set; }
        /// <summary>
        /// This will return Rule can be used or not Setting
        /// </summary>
        public bool? AllowRuleUse { get; set; }
        /// <summary>
        /// This will return Share Message Setting
        /// </summary>
        public bool? AllowMessageTemplate { get; set; }
        /// <summary>
        /// This will return Share templare/Rule Setting
        /// </summary>
        public ShareTemplateRuleSettings ShareTemplateRuleSettingsValues { get; set; }

        /// <summary>
        /// This will return Enable Click To Sign Setting
        /// </summary>
        public bool? EnableClickToSign { get; set; }
        /// <summary>
        /// This will return Post Sending Navigation Page Options
        /// </summary>
        public List<SelectListItem> PostSendingNavigationPageOptions { get; set; }

        /// <summary>
        /// This will return Post Sending Navigation Page ID
        /// </summary>
        public int PostSendingNavigationPageId { get; set; }
        public bool? EnableAutoFillTextControls { get; set; }
        /// <summary>
        /// This will return Enable Webhook Setting
        /// </summary>
        public bool? EnableWebhook { get; set; }
        /// <summary>
        /// This will return Typography Size Setting
        /// </summary>
        public string? TypographySize { get; set; }
        /// <summary>
        /// This will return Typography Size Options 
        /// </summary>
        public List<SelectListItem> TypographySizeOptions { get; set; }


        public int TypographySizeOptionSelected { get; set; }

        /// <summary>
        /// This will return UploadSignature.
        /// </summary>
        public bool UploadSignature { get; set; }
        public List<SelectListItem> BannerBackgroundColorOptions { get; set; }
        public List<SelectListItem> BannerTextColorOptions { get; set; }

        public bool IsEnvelopeExpirationRemindertoSender { get; set; }
        public bool IsSendReminderTillExpiration { get; set; }
        /// <summary>
        /// This will return Typography Size Options 
        /// </summary>
        public List<SelectListItem> SendReminderTillExpiration { get; set; }
        /// <summary>
        /// This will return SendReminderTillExpiration Settings
        /// </summary>
        public SendReminderTillExpirationSettings SendReminderTillExpirationSettingsValues { get; set; }


        /// <summary>
        /// This will return EnvelopeExpirationRemindertoSender.
        /// </summary>
        public Guid SendFinalReminderBeforeExpDropdownSelected { get; set; }

        /// <summary>
        /// This will return EnvelopeExpirationRemindertoSenderDisplayOptionSelected.
        /// </summary>
        public int EnvelopeExpirationRemindertoSenderReminderDays { get; set; }

        /// <summary>
        /// This will return EnvelopeExpirationRemindertoSenderDropdownSelected.
        /// </summary>
        public Guid EnvelopeExpirationRemindertoSenderDropdownSelected { get; set; }

        public List<SelectListItem> EnvelopeExpirationRemindertoSenderDropdown { get; set; }

        /// <summary>
        /// This will return EnvelopeExpirationRemindertoSender Settings
        /// </summary>
        public EnvelopeExpirationRemindertoSenderSettings EnvelopeExpirationRemindertoSenderValues { get; set; }

        public string? SendReminderTillExpirationOptionSelected { get; set; }

        public bool AllowMultiSigners { get; set; }
        /// <summary>
        /// This will return send confirmation email value for send to display.
        /// </summary>
        public int IsSendConfirmationEmailOptionSelected { get; set; }
        /// <summary>
        /// This will return send confirmation email value
        /// </summary>
        public bool SendConfirmationEmail { get; set; }
        /// <summary>
        /// This will return digital certificate value
        /// </summary>
        public int? DigitalCertificate { get; set; }
        /// <summary>
        /// This will return ShowSettingsTabDropdown.
        /// </summary>
        public List<SelectListItem> DigitalCertificateDropdown { get; set; }

        public List<SelectListItem> AppKeyDropdown { get; set; }

        public string? AppKey { get; set; }

        public List<SelectListItem> AppKeyOptions { get; set; }
        public bool EnableCcOptions { get; set; }

        public int DisclaimerLocationId { get; set; }
        public List<SelectListItem> DisclaimerLocationOptions { get; set; }

        /// <summary>
        /// This will return DefaultLandingPage value
        /// </summary>
        public int? DefaultLandingPage { get; set; }
        /// <summary>
        /// This will return DefaultLandingPage Dropdown.
        /// </summary>
        public List<SelectListItem> DefaultLandingPageDropdown { get; set; }
        public int? DeclineEmailSendingFrequency { get; set; }
        /// <summary>
        /// This will return DeclineEmailSendingFrequency Dropdown.
        /// </summary>
        public List<SelectListItem> DeclineEmailSendingFrequencyDropdown { get; set; }
        public bool? EnableRecipientLanguageSelection { get; set; }

        /// <summary>
        /// This will return DefaultLandingPage Dropdown.
        /// </summary>
        public List<SelectListItem> FilingFinalSignedDocuments { get; set; }

        /// <summary>
        /// This will return DefaultLandingPage Dropdown.
        /// </summary>
        public List<SelectListItem> FilingFinalSignedDocumentOptions { get; set; }

        /// <summary>
        /// This will return DefaultLandingPage Dropdown.
        /// </summary>
        public List<SelectListItem> FinalSignedDocumentNameFormat { get; set; }

        /// <summary>
        /// This will return DefaultLandingPage Dropdown.
        /// </summary>
        public List<SelectListItem> SignatureCertificateDocumentFormat { get; set; }
        public string? APIId { get; set; }
        public string? APIKey { get; set; }
        public string? APIAdminEmail { get; set; }
        public string? ResellerName { get; set; }
        public bool? APIIntegrationAccess { get; set; }
        public bool? APIIntegrationRSign { get; set; }
        public string? IntegrationEmail { get; set; }
        public string? AppliedEpicUser { get; set; }
        public string? AppliedEpicEntityId { get; set; }
        public string? EntityType { get; set; }
        public string? IntegrationType { get; set; }
        public string? RegionOptionSelected { get; set; }

        public List<SelectListItem> RegionOptions { get; set; }
        public string? PostSendingNavigationPage { get; set; }
        public string SignInSequenceDesc { get; set; }
        public UserPlan UserPlanDetails { get; set; }
        public string? ControlPanelPinnedPosition { get; set; }
        public bool IsEmailListReasonsforDeclining { get; set; }
        public string? DeclineReportSendTo { get; set; }
        public bool? DisableDownloadOptionOnSignersPage { get; set; }
        public bool? EnableSendingMessagesToMobile { get; set; }
        public string DefaultCountryCode { get; set; }
        public int DefaultDeliveryMode { get; set; }
        public List<LookUpItemDialingCodes> DialingCountryCodes { get; set; }
        public List<SelectListItem> DeliveryModeOptionsDropdown { get; set; }
        public bool? RestrictRecipientsToContactListonly { get; set; }
        public bool? RequiresSignersConfirmationonFinalSubmit { get; set; }
        public bool IncludeStaticTemplates { get; set; }
        public int IncludeStaticTemplatesDisplayOption { get; set; }
        public int SMSProvider { get; set; }
        public List<SelectListItem> SMSProviderOptions { get; set; }
        public bool? IsAllowSignerstoDownloadFinalContract { get; set; }
        public bool? DisableFinishLaterOption { get; set; }
        public bool? DisableDeclineOption { get; set; }
        public bool? DisableChangeSigner { get; set; }

        public bool? EnableMultipleAttachmentsCustomizable { get; set; }
        public string RenameFileToMode { get; set; }
        public string AddPrefixToFileNameMode { get; set; }
        public string AddSuffixToFileNameMode { get; set; }
        public string SuffixCustomName { get; set; }
        public string PrefixCustomName { get; set; }
        public string DateTimeStampForMultipleDocSettingsForPrefixMode { get; set; }
        public string DateTimeStampForMultipleDocSettingsForSufixMode { get; set; }
        public List<SelectListItem> RenameFileModeOptionsDropdown { get; set; }
        public List<SelectListItem> AddPrefixToFileNameModeOptionsDropdown { get; set; }
        public List<SelectListItem> AddSuffixToFileNameModeOptionsDropdown { get; set; }
        public List<SelectListItem> DateTimeStampForMultipleDropdown { get; set; }

        public bool? SendMessageCodetoAvailableEmailorMobile { get; set; }
        public bool? AllowChangeEmailSubjectUpdateResend { get; set; }
        public int EnableSignersUIId { get; set; }
        public int ReverifyonFinalSubmitDisplayOptionSelected { get; set; }
    }
    public class LookUpItemDialingCodes
    {
        public int ID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string DialCode { get; set; }
        public bool? IsActive { get; set; }
        public int? MobileMaxLength { get; set; }
    }
    public class PrivateModeSettings
    {
        public string? Name { get; set; }
        public PrivateModeValues Values { get; set; }
    }
    public class PrivateModeValues
    {
        public string? OptionFlag { get; set; }
        public string? OptionValue { get; set; }
        public string? RetentionDays { get; set; }
        public bool? IsLock { get; set; }
        public bool? IsOverride { get; set; }
    }
    public class SettingProperties
    {
        public Guid KeyConfig { get; set; }
        public bool? IsLock { get; set; }
        public bool? IsOverride { get; set; }

    }
    public class DeclineTemplateResponses
    {
        public int ID { get; set; }
        public string? ResponseText { get; set; }
        public int Order { get; set; }
    }

    public class DeclineTemplateSettings
    {
        /// <summary>
        /// This will return Title of the sustom decline template
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// This will return Control Type selected for custom decline template responses
        /// </summary>
        public int ControlType { get; set; }
        /// <summary>
        /// This will return the list of responses selected 
        /// </summary>
        public List<DeclineTemplateResponses> DeclineTemplateResponsesList { get; set; }

        public string? CultureInfo { get; set; }
    }
    /// <summary>
    /// This class contains Timezone name and ID
    /// </summary>
    public class SettingsTimeZone
    {
        /// <summary>
        /// This will return ID of timezone
        /// </summary>
        public string? Text { get; set; }
        /// <summary>
        /// This will return actual timezone
        /// </summary>
        public string? Value { get; set; }
    }
    /// <summary>
    /// This will return response message.
    /// </summary>
    public class SettingResponseMessage
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return response message for corresponding  status code.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return additional message for api response.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return SettingInformation. If not found will send as null.
        /// </summary>
        public APISettings SettingInformation { get; set; }
    }
    /// <summary>
    /// This will return MailTemplate Laebl Settings.
    /// </summary>
    public class MailTemplateLabelSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid EnvelopeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid RecipientTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid AttachedPdfOption { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? UrlGetPdf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? RetrievalLink { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsWatermark { get; set; }

        public Guid RecipientTypeIdCC { get; set; }
        public string EnableMessageToMobile { get; set; }
        public string LanguageCode { get; set; }
    }
    public class APIHeaderFooterSettings
    {
        public string? Description { get; set; }
        public int ID { get; set; }

    }

    public class ResetSettings
    {/// <summary>
     /// This will return the userEmail
     /// </summary>
        public string? UserEmail { get; set; }
        /// <summary>
        /// This will return the GroupId
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// This will return the isLockCheckReq
        /// </summary>

        public bool IsLockCheckReq { get; set; }
    }

    public class SettingsHistoryData
    {
        /// <summary>
        /// This will return the DisplayOnSendTab
        /// </summary>
        public int DisplayOnSendTab { get; set; }
        /// <summary>
        /// This will return the DefaultSettings
        /// </summary>
        public string? DefaultSettings { get; set; }
        /// <summary>
        /// This will return the IsOverride
        /// </summary>
        public bool? IsOverride { get; set; }
        /// <summary>
        /// This will return the IsLock
        /// </summary>
        public bool? IsLock { get; set; }
        /// <summary>
        /// This will return the DisplayOnSendTabValue
        /// </summary>
        public string? DisplayOnSendTabValue { get; set; }
        /// <summary>
        /// This will return the FieldName
        /// </summary>
        public string? FieldName { get; set; }
    }
    public class SettingsHistory
    {  /// <summary>
       /// This will return the PreviousData
       /// </summary>
        public SettingsHistoryData PreviousData { get; set; }
        /// <summary>
        /// This will return the UpdatedData
        /// </summary>
        public SettingsHistoryData UpdatedData { get; set; }
        /// <summary>
        /// This will return the ModifiedOn
        /// </summary>
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// This will return the ModifiedBy
        /// </summary>

        public string? ModifiedBy { get; set; }
    }

    public class SettingsHistoryDetailsData
    {
        /// <summary>
        /// This will return the HistoryData
        /// </summary>
        public List<SettingsHistory> HistoryData { get; set; }
        /// <summary>
        /// This will return Key and value for SettingsHistoryDetail
        /// </summary>
        public SettingsHistoryDetail SettingsHistoryDetail { get; set; }
        /// <summary>
        /// This will return Key and value for Multilingual
        /// </summary>
        public Dictionary<Guid?, string> DicLabelText { get; set; }
        /// <summary>
        /// Get/Set KeyConfigId
        /// </summary>
        public Guid KeyConfigId { get; set; }
        /// <summary>
        /// Get/Set ResourceKeyId
        /// </summary>
        public Guid ResourceKeyId { get; set; }
        /// <summary>
        /// Get/Set SettingsFor
        /// </summary>
        public string? SettingsForType { get; set; }
        /// <summary>
        /// Get/Set TabId
        /// </summary>
        public int? TabId { get; set; }
        /// <summary>
        /// Get/Set FieldName
        /// </summary>
        public string? FieldName { get; set; }
    }
    public class RetrieveSettingsHistoryDetails
    {
        /// <summary>
        /// Get/Set User'ss Email
        /// </summary>
        public string? UserEmail { get; set; }
        /// <summary>
        /// Get/Set settings for user/company (UserID/CompanyID)
        /// </summary>
        public Guid SettingsFor { get; set; }
        /// <summary>
        /// Get/Set type of settings for User/Company.
        /// </summary>
        public string? SettingsForType { get; set; }
        /// <summary>
        /// Get/Set KeyConfig
        /// </summary>
        public Guid KeyConfigId { get; set; }
        /// <summary>
        /// Get/Set TabId
        /// </summary>
        public int? TabId { get; set; }
        /// <summary>
        /// Get/Set ResourceKeyId
        /// </summary>
        public Guid ResourceKeyId { get; set; }
        /// <summary>
        /// Get/Set FieldName
        /// </summary>
        public string? FieldName { get; set; }
    }
    public class ExtermalIntegrationResponseMessage
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return response message for corresponding  status code.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return additional message for api response.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return SettingInformation. If not found will send as null.
        /// </summary>
        public SettingsExternalIntegration SettingsExternalIntegration { get; set; }

        public bool IncludeSignedCertificateOnSignedPDF { get; set; }
    }

    public class ShareTemplateRuleSettings
    {
        public bool? IsShareTemplateRule { get; set; }
        public bool? IsShareMaster { get; set; }
        public bool? IsAllowCopy { get; set; }
    }

    public class SendReminderTillExpirationSettings
    {
        public bool? IsSendReminderTillExpiration { get; set; }
        public string? SendReminderTillExpiration { get; set; }
    }

    public class EnvelopeExpirationRemindertoSenderSettings
    {
        public bool? IsEnvelopeExpirationRemindertoSender { get; set; }
        public int DaysForReminder { get; set; }
        public Guid PeriodForReminder { get; set; }
    }
    public class MailTemplateBanner
    {
        public string? LanguageCode { get; set; }
        public string? TempCompanyLogo { get; set; }
        public string? MainFolderCompanyLogo { get; set; }
        public string? BannerFontColor { get; set; }
        public string? BannerBackgroundColor { get; set; }
        public bool showRSignLogo { get; set; }

    }

    public class ContactsResponeMessage
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return response message for corresponding  status code.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return additional message for api response.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return SettingInformation. If not found will send as null.
        /// </summary>

        /// <summary>
        /// This will return Key and value for Multilingual
        /// </summary>
        public Dictionary<Guid?, string> DicLabelText { get; set; }

        /// <summary>
        /// This will return list of contacts
        /// </summary>
        public List<ContactDetailResultModel> ContactsDetails { get; set; }

        public int TotalCount { get; set; }
    }

    public class ContactDetails
    {
        /// <summary>
        /// This will return ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This will return JobTitle
        /// </summary>
        public string? JobTitle { get; set; }

        /// <summary>
        /// This will return Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// This will return FirstName
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// This will return LastName
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// This will return Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// This will return Company
        /// </summary>
        public string? Company { get; set; }

        /// <summary>
        /// This will return CompanyId
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// This will return PhoneNumber
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// This will return CreatedSource
        /// </summary>
        public string? CreatedSource { get; set; }

        /// <summary>
        /// This will return CreatedUserId
        /// </summary>
        public Guid CreatedUserId { get; set; }

        /// <summary>
        /// This will return UpdatedUserId
        /// </summary>
        public Guid UpdatedUserId { get; set; }

    }

    public class ContactsFileResponeMessage
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return response message for corresponding  status code.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return additional message for api response.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// This will return Key and value for Multilingual
        /// </summary>
        public Dictionary<Guid?, string> DicLabelText { get; set; }

        /// <summary>
        /// This will return list of contacts
        /// </summary>
        public List<UploadedContactFilesViewModel> UploadedContactFiles { get; set; }

        public int TotalCount { get; set; }
    }

    public partial class UploadedContactFilesViewModel
    {
        public int DocumentId { get; set; }
        public string? FileName { get; set; }
        public string? SizeInKbs { get; set; }
        public string? Company { get; set; }
        public string? UserName { get; set; }
        public string? CreatedDateTime { get; set; }
        public int TotalRecords { get; set; }
    }

    public class ResponseMessageForDeleteContact
    {
        /// <summary>
        ///  This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return Contact Id.
        /// </summary>
        public string? ContactId { get; set; }
    }

    public class ResponseMessageForAddContact
    {
        /// <summary>
        ///  This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return Contact Id.
        /// </summary>
        public string? ContactId { get; set; }
    }

    public class ResponseMessageForDuplicateEmail
    {
        /// <summary>
        ///  This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return email.
        /// </summary>
        public bool IsEmailExists { get; set; }
    }
    public class ResponseMessageForGetContact
    {
        /// <summary>
        ///  This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return Contact Id.
        /// </summary>
        public ContactDetail ContactDetails { get; set; }
    }

    public class ContactDetailDBResponse
    {
        /// <summary>
        /// This will return Envelope ID.
        /// </summary>
        public int returnStatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
    }

    public class FilterContactListforApi
    {
        /// <summary>
        /// TotalContactsCount
        /// </summary>
        public int TotalContactsCount { get; set; }
        /// <summary>
        /// Page info
        /// </summary>
        public int? Page { get; set; }
        /// <summary>
        /// Page size
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// Sort by detail
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Envelopes of user [Nullable]
        /// </summary>
        public Guid? UserID { get; set; }

        /// <summary>
        /// SearchValue
        /// </summary>
        public string? SearchValue { get; set; }

        /// <summary>
        /// TabType
        /// </summary>
        public string? TabType { get; set; }

        /// <summary>
        /// CompanyId
        /// </summary>
        public Guid? CompanyID { get; set; }
    }

    public class FilterContactFileListforApi
    {
        /// <summary>
        /// TotalContactsCount
        /// </summary>
        public int TotalContactsFileCount { get; set; }
        /// <summary>
        /// Page info
        /// </summary>
        public int? Page { get; set; }
        /// <summary>
        /// Page size
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Envelopes of user [Nullable]
        /// </summary>
        public Guid? UserID { get; set; }

        // <summary>
        /// TabType
        /// </summary>
        public string? TabType { get; set; }
    }

    public class ContactDetailResultModel
    {
        /// <summary>
        /// ContactId
        /// </summary>
        public int ContactId { get; set; }
        /// <summary>
        /// JobTitle
        /// </summary>
        public string? JobTitle { get; set; }
        /// <summary>
        /// Company
        /// </summary>
        public string? Company { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// FirstName
        /// </summary>
        public string? FirstName { get; set; }
        /// <summary>
        /// LastName
        /// </summary>
        public string? LastName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string? EmailID { get; set; }
        /// <summary>
        /// TotalRecords
        /// </summary>
        public int TotalRecords { get; set; }
        /// <summary>
        /// CountryCode
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// MobileNumber
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// IsReadOnly
        /// </summary>
        public Nullable<bool> IsReadOnly { get; set; }
        /// <summary>
        /// DialCode
        /// </summary>
        public string DialCode { get; set; }
        /// <summary>
        /// CountryName
        /// </summary>
        public string CountryName { get; set; }
    }
    public class UpdateSettingsDetails
    {
        public Guid loggedInUserId { get; set; }
        public APISettings settingsToSave { get; set; }
        public bool? isBackupReq { get; set; }
        public List<Guid> userData { get; set; }
        public string? Source { get; set; }
    }
    public class ResponsePayload
    {
        /// <summary>
        ///  This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response RSignAPIPayload
        /// </summary>
        public List<RSignAPIPayload> Payload { get; set; }
    }

    public class UserSettingsModel
    {
        public string? Email { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class UserVerificationModel
    {
        public string? envelopeID { get; set; }
        public string? EmailId { get; set; }
        public string? recipientId { get; set; }
        public string? CultureInfo { get; set; }
        public string DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string MobileNumber { get; set; }
        public string CountryCode { get; set; }
    }
    public class ErrorResponseModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? Message { get; set; }
        public bool Status { get; set; }
        public string? ErrorAction { get; set; }
        public bool IsEnvelopePurging { get; set; }
        public int? DeliveryMode { get; set; }
        public string Mobile { get; set; }
        public string EnableMessageToMobile { get; set; }
    }

    public class UserForCompanyorGroupModel
    {
        public Guid CompanyId { get; set; }
        public string? EmailId { get; set; }
    }

    public class SendingConfirmEmailModel
    {
        public Guid? UserIdorCompanyId { get; set; }
        public string? searchUserEmail { get; set; }

        public string? UserType { get; set; }
    }

    public class ResellerDetails
    {

        public string? Status { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusText { get; set; }
        public ResellerContent ResultContent { get; set; }

        public string? Success { get; set; }

    }
    public class ResellerContent
    {
        public string? APIId { get; set; }
        public string? APIKey { get; set; }
        public string? IntegrationEmail { get; set; }
        public string? ResellerName { get; set; }
        public bool? APIIntegrationAccess { get; set; }
        public bool? APIIntegrationRSign { get; set; }
        public string? APIAdminEmail { get; set; }
    }
}
