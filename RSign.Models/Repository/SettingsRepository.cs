using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static RSign.Common.Helpers.Constants;

namespace RSign.Models.Repository
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly IConfiguration _appConfiguration;
        public SettingsRepository(IOptions<AppSettingsConfig> configuration, IConfiguration appConfiguration)
        {
            _configuration = configuration;
            _appConfiguration = appConfiguration;
        }
        public APISettings GetEntityByParam(Guid userID, string userEmail, string settingsForType)
        {
            return new APISettings
            {
                UserEmail = userEmail,
                SettingsFor = userID,
                SettingsForType = settingsForType,
                SettingDetails = GetKeyValue(userID, settingsForType)
            };
        }
        private Dictionary<Guid, KeyPairItem> GetKeyValue(Guid SettingsFor, string SettingsForType)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.SettingsDetail.Where(w => w.SettingsFor == SettingsFor && w.SettingsForType == SettingsForType).ToDictionary(s => s.KeyConfig, s => new KeyPairItem(s.OptionFlag, s.OptionValue, s.IsLock, s.IsOverride));
            }
        }
        public SettingsDetail saveDefaultSettingEntity(string keyName, Guid keyConfig, Guid loggedInUserId, Guid settingsFor, string settingsForType)
        {
            string optionValue = string.Empty;
            int optionFlag = 0;

            switch (keyName)
            {
                case "ExpiresIn":
                    optionValue = Constants.ExpiryType.Thirty_Days.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "ThenSendReminderInDropdownOption":
                    optionValue = Constants.ReminderDropdownOptions.Days.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StorageDriveGoogle":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StorageDriveLocal":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StorageiManage":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "Storagenetdocuments":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "StorageBullhorn":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "StorageVincere":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "StorageAppliedEpic":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "ThenSendReminderIn":
                    optionValue = "0";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StorageDriveDropbox":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SendReminderInDropdownOption":
                    optionValue = Constants.ReminderDropdownOptions.Days.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StoredSignedPDF":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AccessCodeRequiredToOpenSignedDoc":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "DateFormat":
                    optionValue = Constants.DateFormat.US_mm_dd_yyyy_slash.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "TimeZone":
                    optionValue = "UTC";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "OverrideUserSettings":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SignInSequence":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StorageDriveSkydrive":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SignatureCaptureTypeSign":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SignatureCaptureHandDrawn":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "UploadSignature":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IncludeTransparencyDocument":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AllowRecipientToAttachFileWhileSigning":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AccessCodeRequiredToSign":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "ShowSettingsTabSelected":
                    optionValue = Constants.SettingsAccessOptions.Yes.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SendReminderIn":
                    optionValue = "0";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IncludeSignedCertificateOnSignedPDF":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "Disclaimer":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsDisclaimerInCertificate":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsDeleteSignedContracts":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsCreateRules":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsCreateMessageTemplate":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IncludeSignerAttachFile":
                    optionValue = "0";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "FormFieldAlignment":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;

                case "SeparateMultipleDocumentsAfterSigning":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;

                case "CreateStaticLink":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AllowMultiSigners":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AttachXML":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AccessAuthentication":
                    optionValue = Convert.ToString(Constants.ConfigurationalProperties.PasswordType.Select);
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "AccessPassword":
                    optionValue = null;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsAccessCodeSendToSignerEnabled":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StoreEmailBody":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AllowUserToDeleteEmailBody":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "FinalContractOptionSetting":
                    optionValue = Constants.FinalContractOptions.Aspose.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SignatureControlRequired":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "EsignMailCopyAddress":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EsignMailRerouteAddress":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "ReceiveSendingEmailConfirmation":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AttachSignedPDFOptionSetting":
                    optionValue = Constants.AttachSignedPDF.SenderSigner.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "HeaderFooterOptionSettings":
                    optionValue = Constants.HeaderFooterSettings.LeftFooter.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsPostSigningLandingPage":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "IsIncludeEnvelopeXmlData":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AllowTemplateEditing":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnablePostSigningLoginPopup":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "EmailDisclaimer":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SendIndividualSignatureNotifications":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "AddDatetoSignedDocumentNameOptionSettings":
                    optionValue = "1";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SendFinalReminderBeforeExpInDropdownOption":
                    optionValue = Constants.ReminderDropdownOptions.Days.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SendFinalReminderBeforeExp":
                    optionValue = "0";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsDefaultSignatureRequiredForStaticTemplate":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnableOutOfOfficeMode":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "OOFDateRangeFirstDay":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "OOFDateRangeLastDay":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "OOFCopyEmailAddr":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "OOFRerouteEmailAddr":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "EnableDependenciesFeature":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "ReferenceCodeOptionSetting":
                    optionValue = "1";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "PostSendingNavigationPage":
                    optionValue = "0";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SignMultipleDocumentIndependently":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SendInvitationEmailToSigner":
                    optionValue = "2";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "EnableIntegrationAccess":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "DocumentPaperSize":
                    optionValue = "1";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StampOnSignerCopySetting":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StampOnSignerCopyAuthrozieText":
                    optionValue = "RSign: Sender Controls Authoritative Copy";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StampOnSignerCopyWatermarkText":
                    optionValue = "COPY";
                    break;
                case "ElectronicSignIndicationSetting":
                    optionValue = "1";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "DeclineTemplateReasonsSettings":
                    optionValue = @"[{'Title':'Please let us know why you want to decline signing this document.','ControlType':1,'DeclineTemplateResponsesList':[{'ID':1,'ResponseText':'I signed using another method','Order':1},{'ID':2,'ResponseText':'I do not know the sender requesting me to sign this document','Order':2},{'ID':3,'ResponseText':'I do not know the reason for this signature request','Order':3},{'ID':4,'ResponseText':'It appears to be spam','Order':4},{'ID':999,'ResponseText':'Other (fill in the reason below)','Order':5}]}]";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                default:
                    break;
                case "SignatureRequestReplyAddress":
                    optionValue = "3";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "ShowCompanyLogo":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "ShowRSignLogo":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "EmailBannerBackgroundColor":
                    optionValue = Constants.EmailBannerColors.BackgroundColor;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "EmailBannerFontColor":
                    optionValue = Constants.EmailBannerColors.FontColor;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "CompanyLogoImage":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "PrivateMode":
                    optionValue = "[{\"Name\":\"PrivateMode\",\"Values\":{\"OptionFlag\":\"" + PrivacyDefaultValues.PrivateModeOptionFlag +
                        "\",\"OptionValue\":\"" + PrivacyDefaultValues.PrivateModeOptionValue + "\",\"IsOverride\":\"" + PrivacyDefaultValues.PrivateModeIsOverride +
                        "\",\"IsLock\":\"" + PrivacyDefaultValues.PrivateModeIsLock + "\"}},{\"Name\":\"DataMasking\",\"Values\":{\"OptionValue\":\"" + PrivacyDefaultValues.DataMaskingOptionValue +
                        "\",\"IsOverride\":\"" + PrivacyDefaultValues.DataMaskingIsOverride + "\"}},{\"Name\":\"DeleteData\",\"Values\":{\"RetentionDays\":\"" + PrivacyDefaultValues.DeleteDataRetentionDays +
                        "\",\"OptionValue\":\"" + PrivacyDefaultValues.DeleteDataOptionValue + "\",\"IsOverride\":\"" + PrivacyDefaultValues.DeleteDataIsOverride + "\"}}]";
                    optionFlag = PrivacyDefaultValues.PrivateModeOptionFlag;
                    break;
                case "StoreOriginalDocument":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "StoreSignatureCertificate":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "DeleteOriginalDocument":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AllowBulkSending":
                    optionValue = "false";
                    break;
                case "PostSigningPageUrl":
                    optionValue = string.Empty;
                    break;
                case "EnvelopePostSigningPageUrl":
                    optionValue = string.Empty;
                    break;
                case "IsEnvelopePostSigningPage":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnableFileReview":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                //Added by Tparker- Enhancement: New Setting to Hide the ID Button in Controls
                case "ShowControlID":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnableWebhook":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AllowRuleEditing":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AllowRuleUse":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsShareTemplateRule":
                    ShareTemplateRuleSettings shareSettings = new ShareTemplateRuleSettings { IsShareTemplateRule = false, IsShareMaster = false, IsAllowCopy = false };
                    optionValue = JsonConvert.SerializeObject(shareSettings);
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AllowMessageTemplate":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnableClickToSign":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "DefaultControlStyle":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnableAutoFillTextControls":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "TypographySize":
                    optionValue = "px";
                    optionFlag = Constants.SettingsDisplayOptions.ViewOnly;
                    break;
                case "SendReminderTillExpiration":
                    optionValue = string.Empty;
                    break;
                case "EnvelopeExpirationRemindertoSender":
                    optionValue = string.Empty;
                    break;
                case "SendConfirmationEmail":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "DigitalCertificate":
                    optionValue = Constants.DigitalCertificate.Default.ToString(); ;
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "AppKey":
                    optionValue = Constants.AppKey.None.ToString(); ;
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "EnableCcOptions":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "DisclaimerLocation":
                    optionValue = Constants.DisclaimerLocation.BottomOfEmail.ToString();
                    break;
                case "DefaultLandingPage":
                    optionValue = Constants.DefaultLandingPage.Home.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "EnableRecipientLanguageSelection":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "RegisteredUserDefaultLandingPage":
                    optionValue = Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]);
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "NonRegisteredUserDefaultLandingPage":
                    optionValue = Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]);
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "ControlPanelPinnedPosition":
                    optionValue = "bottom";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "IsEmailListReasonsforDeclining":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "DeclineReportSendTo":
                    optionValue = string.Empty;
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "DeclineEmailSendingFrequency":
                    optionValue = Constants.DeclineEmailSendingFrequency.Monthly.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "DisableDownloadOptionOnSignersPage":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnableSendingMessagesToMobile":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "DefaultCountryCode":
                    optionValue = Constants.DefaultDialingCode.Select.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "DefaultDeliveryMode":
                    optionValue = Constants.DeliveryModes.EmailSlashMobile.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "RestrictRecipientsToContactListonly":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "RequiresSignersConfirmationonFinalSubmit":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IncludeStaticTemplates":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "SMSProvider":
                    optionValue = Constants.SMSProviders.PrimaryProvider.ToString();
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "IsAllowSignerstoDownloadFinalContract":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnableMultipleAttachmentsCustomizable":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "RenameFileToMode":
                    optionValue = "1";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "AddPrefixToFileNameMode":
                    optionValue = "1";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "AddSuffixFileToMode":
                    optionValue = "1";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "PrefixCustomName":
                    optionValue = string.Empty;
                    break;
                case "SuffixCustomName":
                    optionValue = string.Empty;
                    break;
                case "DateTimeStampForMultipleDocSettingsForPrefix":
                    optionValue = "0";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "DateTimeStampForMultipleDocSettingsForSufix":
                    optionValue = "0";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "DisableFinishLaterOption":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "DisableDeclineOption":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "DisableChangeSigner":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "EnableSignersUI":
                    optionValue = "1";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
                case "AllowChangeEmailSubjectUpdateResend":
                    optionValue = "false";
                    optionFlag = Constants.SettingsDisplayOptions.Disable;
                    break;
                case "SendMessageCodetoAvailableEmailorMobile":
                    optionValue = "true";
                    optionFlag = Constants.SettingsDisplayOptions.Active;
                    break;
            }
            return new SettingsDetail
            {
                ID = Guid.NewGuid(),
                KeyConfig = keyConfig,
                OptionFlag = optionFlag,
                OptionValue = optionValue,
                SettingsFor = settingsFor,
                SettingsForType = settingsForType,
                UpdatedBy = loggedInUserId,
                UpdatedOn = DateTime.UtcNow
            };
        }
        public SettingsDetail GetEntityForByKeyConfig(Guid settingsFor, Guid keyConfig)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return dbContext.SettingsDetail.FirstOrDefault(f => f.SettingsFor == settingsFor && f.KeyConfig == keyConfig);
                }
            }
            catch (Exception ex)
            {
                return null;
                //throw ex;
            }
        }
        public bool saveDefaultSetting(Guid loggedInUserId, Guid settingsFor, string settingsForType, string companyName = "")
        {
            Type type = typeof(Constants.SettingsKeyConfig);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo p in fields)
            {
                string keyName = p.Name.ToString();// do stuff here
                string fieldValue = p.GetValue(null).ToString();
                if (keyName != "EmailDisclaimer")
                {
                    var settingsDetail = saveDefaultSettingEntity(keyName, new Guid(fieldValue), loggedInUserId, settingsFor, settingsForType);
                    if (settingsDetail != null && !string.IsNullOrEmpty(companyName) && keyName.Equals("SignatureRequestReplyAddress"))
                    {
                        settingsDetail.OptionValue = GetDefaultReplyAddressSetting(companyName, settingsDetail.OptionValue);
                    }
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        dbContext.SettingsDetail.Add(settingsDetail);
                        dbContext.SaveChanges();
                    }
                }
            }
            return true;
        }
        public string GetDefaultReplyAddressSetting(string companyName, string currentSetting)
        {
            string optionValue = currentSetting;
            string[] JungheinrichCompanies = Convert.ToString(_appConfiguration["jungheinrich"]).Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (JungheinrichCompanies != null && JungheinrichCompanies.Length > 0)
            {
                foreach (var JungCompany in JungheinrichCompanies)
                {
                    if (companyName.ToLower().StartsWith(JungCompany))
                    {
                        optionValue = Constants.SignatureRequestReplyAddress.SendersEmailRsignRpostNet.ToString();
                    }
                }
            }

            return optionValue;
        }
        public SettingsExternalIntegration GetExternalSettingsByType(Guid userID, string Type)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return dbContext.SettingsExternalIntegration.FirstOrDefault(f => f.UserID == userID && f.IntegrationType.ToLower() == Type.ToLower());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetSettingsValue(string keyValue, Lookup lookupName, string fieldName, int flagValue, string LanguageID)
        {
            string OptionValue = string.Empty;
            OptionValue = GetSettingsOptionDescription(lookupName, LanguageID, keyValue, flagValue, fieldName);
            return OptionValue;
        }
        public string GetSettingsOptionDescription(Lookup lookup, string languageCode, string value, int flag, string dropDownType)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                Guid LanguageId = dbContext.Language.Where(l => l.LanguageCode == languageCode).FirstOrDefault().ID;
                string OptionValue = string.Empty;
                switch (lookup)
                {
                    case Lookup.DateFormat:
                        Guid paramGuid = new Guid(value);
                        OptionValue = dbContext.DateFormat.Where(s => s.ID == paramGuid).Select(s => s.Description).FirstOrDefault();
                        break;
                    case Lookup.SettingsDisplayOptions:
                        OptionValue = dbContext.SettingsDisplayLanguageMapping.Where(r => r.LanguageID == LanguageId && r.MasterReferenceKeyID == flag).Select(s => s.DescriptionValue).FirstOrDefault();
                        break;
                    case Lookup.MasterLanguageKeyDetails:
                        Guid param = new Guid(value);
                        OptionValue = dbContext.vw_MasterlanguageMapping.Where(s => s.ResourceKey == param && s.LanguageID == LanguageId).Select(s => s.KeyValue).FirstOrDefault();
                        break;
                    case Lookup.DatetoSignedDocNameSettingsOptions:
                        int paramVal = Convert.ToInt32(value);
                        OptionValue = dbContext.DatetoSignedDocNameSettingsOptions.Where(s => s.ID == paramVal).Select(s => s.Name).FirstOrDefault();
                        break;
                    case Lookup.DropdownOptions:
                        OptionValue = dbContext.DropdownOptions.Where(r => r.LanguageID == LanguageId && r.OptionValue == value && r.FieldName == dropDownType).Select(s => s.OptionName).FirstOrDefault();
                        break;
                    case Lookup.HeaderFooterSettings:
                        int keyValue = flag;
                        OptionValue = dbContext.DropdownOptions.Where(r => r.LanguageID == LanguageId && r.OptionValue == value && r.FieldName == dropDownType).Select(s => s.OptionName).FirstOrDefault();
                        //OptionValue = dbContext.HeaderFooterSettings.Where(r => r.ID == keyValue).Select(s => s.Description).FirstOrDefault();
                        break;
                }
                return OptionValue;
            }
        }
        public SettingsDetail GetEntityForByKeyConfig(Guid settingsFor, Guid keyConfig, string SettingsForType)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return dbContext.SettingsDetail.FirstOrDefault(f => f.SettingsFor == settingsFor && f.KeyConfig == keyConfig && f.SettingsForType == SettingsForType);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetNonRegisteredUserDefaultLandingPage(Guid userID, Guid nonRegisteredUserDefaultLandingPage)
        {
            var settingsDetailKeyData = GetEntityForByKeyConfig(userID, nonRegisteredUserDefaultLandingPage);
            if (settingsDetailKeyData != null)
            {
                return string.IsNullOrEmpty(settingsDetailKeyData.OptionValue) ? Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]) : settingsDetailKeyData.OptionValue;
            }
            else
                return Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]);
        }
        public string GetRegisteredUserDefaultLandingPage(Guid userID, Guid registeredUserDefaultLandingPage)
        {
            var settingsDetailKeyData = GetEntityForByKeyConfig(userID, registeredUserDefaultLandingPage);
            if (settingsDetailKeyData != null)
            {
                return string.IsNullOrEmpty(settingsDetailKeyData.OptionValue) ? Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]) : settingsDetailKeyData.OptionValue;
            }
            else
                return Convert.ToString(_appConfiguration["RPostPostSigningLandingPage"]);
        }
    }
}
