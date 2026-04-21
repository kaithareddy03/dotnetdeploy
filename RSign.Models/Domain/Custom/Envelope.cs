using RSign.Models.APIModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace RSign.Models
{
    public partial class Envelope
    {
        [NotMapped]
        public string? EnvelopeCodeDisplay { get; set; }
        [NotMapped]
        public string? EnvelopeStatusDescription { get; set; }
        [NotMapped]
        public int SignerCount { get; set; }
        public RecipientsDetail SenderDetails { get; set; }
        [NotMapped]
        public bool IsSequential { get; set; }
        [NotMapped]
        public bool IsCompanyTransparencyFlagSet { get; set; }
        [NotMapped]
        public Guid GlobalEnvelopeID { get; set; }
        [NotMapped]
        public string? EnvelopeStage { get; set; }

        public List<RecipientsDetailsAPI> RecipientsDetail { get; set; }
        [NotMapped]
        public string? IpAddress { get; set; }
        [NotMapped]
        public string? DocumentJson { get; set; }
        [NotMapped]
        public string? RecipientsJson { get; set; }
        [NotMapped]
        public Guid? SubEnvelopeId { get; set; }
        [NotMapped]
        public string? TemplateGroupUploadsJson { get; set; }
        [NotMapped]
        public bool? IsAllTemplatesProcessed { get; set; }
        [NotMapped]
        public bool? IsNewGroup { get; set; }
        [NotMapped]
        public int? DocumentPaperSizeID { get; set; }
        [NotMapped]
        public bool? IsEnvelopeHistory { get; set; }
        [NotMapped]
        public bool? IsNewUploadOrDeleteDocument { get; set; }
        [NotMapped]
        public bool IsNextClick { get; set; }
        /// <summary>
        /// This will return the list of attachments requested
        /// </summary>
        public List<TemplateGroupDocumentUploadDetails> TemplateGroupDocumentUploadDetailsList { get; set; }
        /// <summary>
        /// This will return the whether it is step 1 send or not
        /// </summary>
         [NotMapped]
        public bool? IsTemplateDirectSend { get; set; }
        public List<ConditionalControlMapping> ConditionalControlMappingList { get; set; }

        [NotMapped]
        public bool EnableMultipleNameCustomize { get; set; }

        [NotMapped]
        public int RenameFileTo { get; set; }

        [NotMapped]
        public int AddPrefixToFileNameMode { get; set; }

        [NotMapped]
        public int DateTimeStampForMultipleDocSettingsForPrefix { get; set; }
        [NotMapped]
        public string PrefixCustomName { get; set; }

        [NotMapped]
        public int AddSuffixFileToMode { get; set; }

        [NotMapped]
        public int DateTimeStampForMultipleDocSettingsForSufix { get; set; }

        [NotMapped]
        public string SuffixCustomName { get; set; }
    }
    public class DimensionDetails
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
    public class DocumentInfoDetails
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }

    public class EnvelopeImageInformationDetails
    {
        public int Id { get; set; }

        public string? ImagePath { get; set; }

        public int DocPageNo { get; set; }
        public int PageNo { get; set; }
        public DimensionDetails Dimension { get; set; }

        public DocumentInfoDetails Document { get; set; }

    }
    public class PrepareDocumentInfo
    {
        /// <summary>
        /// Document ID to prepare
        /// </summary>
        public Guid DocumentID { get; set; }
        /// <summary>
        /// Document name to prepare
        /// </summary>
        public string? DocumentName { get; set; }
        /// <summary>
        /// Document order to prepare
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Get/Set absolute uploaded file path with file name
        /// </summary>
        public string? UploadedFilePath { get; set; }
        /// <summary>
        /// Get/Set absolute converted file path with file name
        /// </summary>
        public string? ConvertedFilePath { get; set; }
        /// <summary>
        /// Get page count from converted file
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Get/Set if document is elligible for tagging feature
        /// </summary>
        public bool IsElligibleForTaggingFeature { get; set; }

        public string? ActionType { get; set; }

    }
    public class APIEnvelope
    {
        /// <summary>
        /// This will set Date Format Id of envelope.
        /// </summary>
        public string? DateFormatID { get; set; }
        /// <summary>
        /// This will set Expiry Type Id of envelope.
        /// </summary>
        public string? ExpiryTypeID { get; set; }
        /// <summary>
        /// This will set Password required to sign validation of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public bool PasswordRequiredToSign { get; set; }
        /// <summary>
        /// This will set Password required to open validation of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public bool PasswordRequiredToOpen { get; set; }
        /// <summary>
        /// This will set Password to Sign of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public string? PasswordToSign { get; set; }
        /// <summary>
        /// This will set Password to open of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public string? PasswordToOpen { get; set; }
        /// <summary>
        /// This will set transparency document required for envelope or not
        /// </summary>       
        public bool IsTransparencyDocReq { get; set; }
        /// <summary>
        /// This will return envelope IsSignerAttachFileReq flag
        /// </summary>
        public bool IsSignerAttachFileReq { get; set; }
        public int IsSignerAttachFileReqNew { get; set; }
        /// <summary>
        /// This will set Password to open of envelope.
        /// </summary>
        public string? IpAddress { get; set; }
        /// <summary>
        /// This will set Recipient Email.
        /// </summary>
        public string? RecipientEmail { get; set; }
        /// <summary>
        /// This will set Envelope Status.
        /// </summary>
        public bool IsEnvelopeComplete { get; set; }
        /// <summary>
        /// This will set Envelope Status.
        /// </summary>
        public Guid StaticTemplateID { get; set; }
        /// <summary>
        /// This will set Control properties.
        /// </summary>
        public List<DocumentContentDetails> ControlCollection { get; set; }
        /// <summary>
        /// This will set Sender UserId properties.
        /// </summary>
        public Guid SenderUserId { get; set; }

        /// <summary>
        /// This will return comment
        /// </summary>
        public string? Comment { get; set; }

        public bool? IsStatic { get; set; }
        /// <summary>
        /// This will return envelope IsAttachXMLData for template flag
        /// </summary>
        public bool IsAttachXMLDataReq { get; set; }
        /// <summary>
        /// This will return envelope IsSeparateMultipleDocumentsAfterSigningRequired flag
        /// </summary>
        public bool IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
        /// <summary>
        /// This will return if confirmation email is required for static template
        /// </summary>
        public bool IsConfirmationEmailReq { get; set; }
        /// <summary>
        /// This will return Is Disclaimer In Certificate
        /// </summary>
        public bool IsDisclaimerInCertificate { get; set; }
        /// <summary>
        /// This will return access auth type for envelope
        /// </summary>
        public Guid? AccessAuthenticationType { get; set; }
        /// <summary>
        /// This will set access authentication password
        /// </summary>
        public string? AccessAuthenticationPassword { get; set; }
        /// <summary>
        /// This will check if user set password manually or password is set by system
        /// </summary>
        ///
        [XmlIgnore]
        public bool IsRandomPassword { get; set; }
        /// <summary>
        /// This will return if sender want to send password by eSign or manually.
        /// </summary>
        ///
        public bool IsPasswordMailToSigner { get; set; }
        /// <summary>
        /// This will set AccessAuthType
        /// </summary>
        ///
        [XmlIgnore]
        public string? AccessAuthType { get; set; }
        /// <summary>
        /// This will return envelope CultureInfo
        /// </summary>
        public string? CultureInfo { get; set; }
        /// <summary>
        /// This will return certificate Signature
        /// </summary>
        public string? CertificateSignature { get; set; }
        /// <summary>
        /// This will return IsEdited flag of envelope
        /// </summary>
        public bool? IsEdited { get; set; }
        /// <summary>
        /// This will set Post Signing Landing Page .
        /// </summary>
        public string? PostSigningLandingPage { get; set; }

        public Guid? ReminderTypeID { get; set; }

        public Guid? ThenReminderTypeID { get; set; }

        public Guid? MessageTemplateTextID { get; set; }

        public bool? SendIndividualSignatureNotifications { get; set; }

        public int? DeclineReasonID { get; set; }
        /// <summary>
        /// This will return ReferenceCode
        /// </summary>
        public string? ReferenceCode { get; set; }
        /// <summary>
        /// This will return ReferenceEmail
        /// </summary>
        public string? ReferenceEmail { get; set; }
        /// <summary>
        /// This will return IsWaterMark
        /// </summary>
        public bool? IsWaterMark { get; set; }
        /// <summary>
        /// This will return WatermarkTextForSender
        /// </summary>
        public string? WatermarkTextForSender { get; set; }
        /// <summary>
        /// This will return WatermarkTextForOther
        /// </summary>
        public string? WatermarkTextForOther { get; set; }
        /// <summary>
        /// This will return the Private Mode value
        /// </summary>
        public Nullable<bool> IsPrivateMode { get; set; }

        public Nullable<bool> IsStoreOriginalDocument { get; set; }
        public Nullable<bool> IsStoreSignatureCertificate { get; set; }
        public Nullable<bool> IsStaticLinkDisabled { get; set; }
        public Nullable<bool> IsEnableFileReview { get; set; }

        public bool? IsAdditionalAttmReq { get; set; }

        public List<SendEnvelopeFromGroupAttachmentRequests> AttachmentRequests { get; set; }

        public bool IsSignerIdentity { get; set; }

        public string? CreatedSource { get; set; }
        public string? DisclaimerText { get; set; }
        public string? EnvelopeExpirationRemindertoSender { get; set; }
        public string? SendReminderTillExpiration { get; set; }
        public Nullable<int> IsEnvelopeExpirationRemindertoSender { get; set; }
        public Nullable<int> ISSendReminderTillExpiration { get; set; }
        public Nullable<int> EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
        public bool? EnableCcOptions { get; set; }
        public bool? EnableRecipientLanguage { get; set; }
        public bool? EnableMessageToMobile { get; set; }
        public bool? RestrictRecipientsToContact { get; set; }
      //  public bool? IsSMSAccessCode { get; set; }
        public bool? ReVerifySignerDocumentSubmit { get; set; }
        public bool? ReVerifySignerStaticTemplate { get; set; }
      //  public bool? IsReVerifySignerEmailAccessCode { get; set; }
      //  public bool? IsReVerifySignerSMSAccessCode { get; set; }
        public DateTime? StaticLinkExpiryDate { get; set; }
    }
    public class APITemplate : APIEnvelope
    {
        /// <summary>
        /// This will set template is editable or not.
        /// </summary>
        public bool IsTemplateEditable { get; set; }
        /// <summary>
        /// This will set template name.
        /// </summary>
        public string? TemplateName { get; set; }
        /// <summary>
        /// This will set template desctiption.
        /// </summary>
        public string? TemplateDescription { get; set; }

        /// <summary>
        /// This will set template is Default Signature for StaticTemplate.
        /// </summary>
        public bool? IsDefaultSignatureForStaticTemplate { get; set; }
        /// <summary>
        /// This will return the rule editable value
        /// </summary>
        public bool? IsRuleEditable { get; set; }
        /// <summary>
        /// This will return the request from RApp or not
        /// </summary>
        public string? CreatedSource { get; set; }
        public bool AllowMultiSigner { get; set; }
        public bool? SendConfirmationEmail { get; set; }

        public Nullable<bool> IsExistSendConfirmationEmail { get; set; }

        public bool EnableCcOptions { get; set; }
    }
    public class APIControlTagList
    {
        public bool IsConversionSuccess { get; set; }
        public string? Message { get; set; }
        public bool IsTagExistInAnyDocument { get; set; }
        public string? ErrorTagDetails { get; set; }
        public Dictionary<Guid, TempDocumentInfo> DocumentInfo { get; set; }
        public List<APIControlTag> ControlTagLst { get; set; }
    }
    public class TempDocumentInfo
    {
        public TempDocumentInfo(int o, int c, string n)
        {
            Order = o;
            PageCount = c;
            Name = n;
        }
        public int Order { get; set; }
        public int PageCount { get; set; }
        public string? Name { get; set; }
    }
    public class APIRecipient
    {
        /// <summary>
        /// Get/Set envelope id for which recipient to add.
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// Get/Set type for which recipient to add. More info from Master API.
        /// </summary>
        public string? RecipientType { get; set; }
        /// <summary>
        /// Get/Set name for which recipient to add.
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// Get/Set email address for which recipient to add.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        /// <summary>
        /// Get/Set order for which recipient to add.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Get/Set ID for which recipient to add. To add recipient new ID to be provided and to update same ID needs to be provided. New ID can be taken from - https://www.guidgenerator.com/
        /// </summary>
        public string? RecipientID { get; set; }
        public Guid? EnvelopeTemplateGroupID { get; set; }

        public string? CCSignerType { get; set; }

        public int? CCSignerTypeValue { get; set; }
        public string? CultureInfo { get; set; }
        public int? DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
       // public int? ReminderType { get; set; }
        public bool? IsReadonlyContact { get; set; }
    }
    public class APIRole
    {
        /// <summary>
        /// Get/Set template id for which role to add.
        /// </summary>
        public Guid TemplateID { get; set; }
        /// <summary>
        /// Get/Set type for which role to add. More info from Master API.
        /// </summary>
        public string? RoleType { get; set; }
        /// <summary>
        /// Get/Set name for which role to add.
        /// </summary>
        public string? RoleName { get; set; }
        /// <summary>
        /// Get/Set order for which role to add.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Get/Set ID for which role to add. To add role new ID to be provided and to update same ID needs to be provided. New ID can be taken from - https://www.guidgenerator.com/
        /// </summary>
        public string? RoleID { get; set; }

        public string? CCSignerType { get; set; }
        public string? CultureInfo { get; set; }
        public Nullable<int> DeliveryMode { get; set; }
        public string CountryCode { get; set; }
        public string DialCode { get; set; }
     //   public Nullable<int> ReminderType { get; set; }
    }
    public class ShareTemplateInfo
    {
        /// <summary>
        /// This will set Template Id.
        /// </summary>
        public string? TemplateID { get; set; }
        /// <summary>
        /// This will set Template as Shared or Unshared.
        /// </summary>
        public bool IsShare { get; set; }
        /// <summary>
        /// This will set Template as Shared or Unshared.
        /// </summary>
        public int TemplateCode { get; set; }
        //Added by Tparker-Enhancement Sharing TemplatesRules with Share MasterAllow Copy Options
        /// <summary>
        /// This will set isCopyOrShare.
        /// </summary>
        public int isCopyOrShare { get; set; }
    }
    public class CopyTemplateInfo
    {
        /// <summary>
        /// This will set Template Id.
        /// </summary>
        public string? TemplateID { get; set; }
        /// <summary>
        /// This will set Name of the Template
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// This will set Template as Shared or Unshared.
        /// </summary>
        public int TemplateCode { get; set; }
        /// <summary>
        /// This will return Type
        /// </summary>
        public string? Type { get; set; }
    }
    public class Prepare
    {
        /// <summary>
        /// Reminder days to set
        /// </summary>
        public int SendReminderIn { get; set; }
        /// <summary>
        /// Repeat reminder days to set
        /// </summary>
        public int ThenSendReminderIn { get; set; }
        /// <summary>
        /// Is final signature certificate required on final contract
        /// </summary>
        public bool SignatureCertificateRequired { get; set; }
        /// <summary>
        /// IS final contract to store and option to available on Manage Page
        /// </summary>
        public bool DownloadLinkRequired { get; set; }
        /// <summary>
        /// This will set Sequence check required for envelope or not
        /// </summary>       
        public bool IsSequenceCheck { get; set; }
        /// <summary>
        /// Subject of Envelope Stage
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// This will return Reminder type ID weeks or Days
        /// </summary>
        public Guid? ReminderTypeID { get; set; }

        public Guid? ThenReminderTypeID { get; set; }

        /// <summary>
        /// This will return the Final Reminder 
        /// </summary>
        public int? FinalReminderDays { get; set; }
        /// <summary>
        /// This will return the Final reminder type ID 
        /// </summary>
        public Guid? FinalReminderTypeID { get; set; }
        /// <summary>
        /// This will return IsWaterMark
        /// </summary>
        public bool? IsWaterMark { get; set; }
        /// <summary>
        /// This will return WatermarkTextForSender
        /// </summary>
        public string? WatermarkTextForSender { get; set; }
        /// <summary>
        /// This will return WatermarkTextForOther
        /// </summary>
        public string? WatermarkTextForOther { get; set; }

    }


    public class PrepareEnvelope : Prepare
    {
        /// <summary>
        /// This will set Date Format Id of envelope.
        /// </summary>
        public string? DateFormatID { get; set; }
        /// <summary>
        /// This will set Expiry Type Id of envelope.
        /// </summary>
        public string? ExpiryTypeID { get; set; }
        /// <summary>
        /// This will set Password required to sign validation of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public bool? PasswordRequiredToSign { get; set; }
        /// <summary>
        /// This will set Password required to open validation of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public bool? PasswordRequiredToOpen { get; set; }
        /// <summary>
        /// This will set Password to Sign of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public string? PasswordToSign { get; set; }
        /// <summary>
        /// This will set Password to open of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public string? PasswordToOpen { get; set; }
        /// <summary>
        /// This will set transparency document required for envelope or not
        /// </summary>       
        public bool? IsTransparencyDocReq { get; set; }
        /// <summary>
        /// This will set Sequence check required for envelope or not
        /// </summary>       
        public bool IsSequenceCheck { get; set; }
        /// <summary>
        /// Envelope ID of which envelope is to prepare
        /// </summary>
        public string? EnvelopeID { get; set; }
        /// <summary>
        /// Envelope ID of which envelope is to prepare
        /// </summary>
        public int TemplateCode { get; set; }
        /// <summary>
        /// Subject of Envelope
        /// </summary>
        public string? Subject { get; set; }
        /// <summary>
        /// Message of Envelope
        /// </summary>
        [AllowHtml]
        public string? Message { get; set; }

        /// <summary>
        /// This will return envelope IsSignerAttachFileReq flag
        /// </summary>
        public bool? IsSignerAttachFileReq { get; set; }
        public int IsSignerAttachFileReqNew { get; set; }
        /// <summary>
        /// This will return envelope IsSeparateMultipleDocumentsAfterSigningRequired flag
        /// </summary>
        public bool IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
        /// <summary>
        /// This will return envelope IsAttachXMLData for envelope flag
        /// </summary>
        public bool IsAttachXMLDataReq { get; set; }
        /// <summary>
        /// This will return envelope Is Disclaimer InCertificate for envelope flag
        /// </summary>
        public bool? IsDisclaimerInCertificate { get; set; }
        /// <summary>
        /// This will return access auth type for envelope
        /// </summary>
        public Guid? AccessAuthenticationType { get; set; }
        /// <summary>
        /// This will set access authentication password
        /// </summary>
        public string? AccessAuthenticationPassword { get; set; }
        /// <summary>
        /// This will check if user set password manually or password is set by system
        /// </summary>
        [XmlIgnore]
        public bool IsRandomPassword { get; set; }
        /// <summary>
        /// This will return if sender want to send password by eSign or manually.
        /// </summary>
        public bool IsPasswordMailToSigner { get; set; }
        /// <summary>
        /// This will set AccessAuthType
        /// </summary>
        [XmlIgnore]
        public string? AccessAuthType { get; set; }
        /// <summary>
        /// This will return envelope CultureInfo
        /// </summary>
        public string? CultureInfo { get; set; }
        /// <summary>
        /// This will return envelope Recipients
        /// </summary>
        public List<Recipients> Recipients { get; set; }
        /// <summary>
        /// This will return envelope Documents
        /// </summary>
        public List<Documents> Documents { get; set; }
        /// <summary>
        /// This will return User Signature ID
        /// </summary>
        public Guid? UserSignatureTextID { get; set; }
        /// <summary>
        /// This will return IP Address
        /// </summary>
        public string? IpAddress { get; set; }
        /// <summary>
        /// This will return User MessageTemplate ID
        /// </summary>
        public Guid? MessageTemplateTextID { get; set; }

        public bool? SendIndividualSignatureNotifications { get; set; }
        /// <summary>
        /// This will return ReferenceCode
        /// </summary>
        public string? ReferenceCode { get; set; }
        /// <summary>
        /// This will return ReferenceEmail
        /// </summary>
        public string? ReferenceEmail { get; set; }
        public Guid? SubEnvelopeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? TemplateGroupUploadsJson { get; set; }
        public Nullable<bool> IsPrivateMode { get; set; }

        public Nullable<bool> IsStoreOriginalDocument { get; set; }
        public Nullable<bool> IsStoreSignatureCertificate { get; set; }
        /// <summary>
        /// This will return the PostSigningLandingPage url
        /// </summary>

        public string? PostSigningLandingPage { get; set; }
        public bool? IsNewUploadOrDeleteDocument { get; set; }

        public Nullable<bool> IsEnableFileReview { get; set; }
        /// <summary>
        /// This will return the EDisplayCode
        /// </summary>
        public string? EDisplayCode { get; set; }
        /// <summary>
        /// This will return the CreatedDateTime
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// This will return the IsRule
        /// </summary>
        public Nullable<bool> IsRule { get; set; }

        public bool? IsAdditionalAttmReq { get; set; }
        /// <summary>
        /// This will return the whether it is step 1 send or not
        /// </summary>
        public bool? IsTemplateDirectSend { get; set; }
        /// <summary>
        /// Enable AutoFill Text Controls true or false
        /// </summary>
        public bool? IsEnableAutoFillTextControls { get; set; }
        /// <summary>
        /// This will return the list of attachments requested
        /// </summary>
        public List<TemplateGroupDocumentUploadDetails> TemplateGroupDocumentUploadDetailsList { get; set; }

        public bool IsSignerIdentity { get; set; }

        public string? CreatedSource { get; set; }
        public string? DisclaimerText { get; set; }
        public int? IsEnvelopeExpirationRemindertoSender { get; set; }
        public int? EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
        public string? EnvelopeExpirationRemindertoSender { get; set; }
        public int? ISSendReminderTillExpiration { get; set; }
        public string? SendReminderTillExpiration { get; set; }       
        public bool IsSelfSign { get; set; }
        public int ElectronicSignIndicationSelectedID { get; set; }

        public bool EnableCcOptions { get; set; }

        public bool? IsSameRecipientForAllTemplates { get; set; }
        public bool? EnableRecipientLanguage { get; set; }
        public bool? EnableMessageToMobile { get; set; }
        public bool? RestrictRecipientsToContact { get; set; }
      //  public bool IsSMSAccessCode { get; set; }
        public bool? ReVerifySignerDocumentSubmit { get; set; }
      //  public bool? IsReVerifySignerEmailAccessCode { get; set; }
      //  public bool? IsReVerifySignerSMSAccessCode { get; set; }
    }
    public class PrepareTemplate : Prepare
    {
        /// <summary>
        /// This will set Date Format Id of envelope.
        /// </summary>
        public string? DateFormatID { get; set; }
        /// <summary>
        /// This will set Expiry Type Id of envelope.
        /// </summary>
        public string? ExpiryTypeID { get; set; }
        /// <summary>
        /// This will set Password required to sign validation of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public bool? PasswordRequiredToSign { get; set; }
        /// <summary>
        /// This will set Password required to open validation of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public bool? PasswordRequiredToOpen { get; set; }
        /// <summary>
        /// This will set Password to Sign of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public string? PasswordToSign { get; set; }
        /// <summary>
        /// This will set Password to open of envelope.
        /// </summary>
        ///
        [XmlIgnore]
        public string? PasswordToOpen { get; set; }
        /// <summary>
        /// This will set transparency document required for envelope or not
        /// </summary>       
        public bool? IsTransparencyDocReq { get; set; }
        /// <summary>
        /// Template ID to prepare
        /// </summary>
        public string? TemplateID { get; set; }
        /// <summary>
        /// This will set template name.
        /// </summary>
        public string? TemplateName { get; set; }
        /// <summary>
        /// This will set template desctiption.
        /// </summary>
        public string? TemplateDescription { get; set; }
        /// <summary>
        /// This will set template is editable or not.
        /// </summary>
        public bool IsTemplateEditable { get; set; }
        /// <summary>
        /// This will return envelope IsSignerAttachFileReq flag
        /// </summary>
        public bool? IsSignerAttachFileReq { get; set; }
        public int IsSignerAttachFileReqNew { get; set; }
        /// <summary>
        /// This will return envelope IsCreateStaticLink for template flag
        /// </summary>
        public bool? IsStatic { get; set; }
        /// <summary>
        /// This will return envelope IsAttachXMLData for template flag
        /// </summary>
        public bool IsAttachXMLDataReq { get; set; }
        /// <summary>
        /// This will return envelope IsSeparateMultipleDocumentsAfterSigningRequired flag
        /// </summary>
        public bool IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
        /// <summary>
        /// This will return envelope IsDisclaimerInCertificate for template flag
        /// </summary>
        public bool? IsDisclaimerInCertificate { get; set; }
        /// <summary>
        /// This will return access auth type for envelope
        /// </summary>
        public Guid? AccessAuthenticationType { get; set; }
        /// <summary>
        /// This will set access authentication password
        /// </summary>
        public string? AccessAuthenticationPassword { get; set; }
        /// <summary>
        /// This will check if user set password manually or password is set by system
        /// </summary>
        ///
        [XmlIgnore]
        public bool IsRandomPassword { get; set; }
        /// <summary>
        /// This will return if sender want to send password by eSign or manually.
        /// </summary>
        ///
        public bool IsPasswordMailToSigner { get; set; }
        /// <summary>
        /// This will set AccessAuthType
        /// </summary>
        ///
        [XmlIgnore]
        public string? AccessAuthType { get; set; }
        /// <summary>
        /// Subject of Envelope
        /// </summary>
        public string? Subject { get; set; }
        /// <summary>
        /// Message of Envelope
        /// </summary>
        [AllowHtml]
        public string? Message { get; set; }
        /// <summary>
        /// This will return envelope CultureInfo
        /// </summary>
        public string? CultureInfo { get; set; }
        /// <summary>
        /// This will return Template Roles
        /// </summary>
        public List<TemplateRoles> TemplateRoles { get; set; }
        /// <summary>
        /// This will return envelope Template Documents
        /// </summary>
        public List<TemplateDocuments> TemplateDocuments { get; set; }
        /// <summary>
        /// This will set Post Signing Landing Page .
        /// </summary>
        public string? PostSigningLandingPage { get; set; }

        public Guid? UserSignatureTextID { get; set; }

        public Guid? MessageTemplateTextID { get; set; }

        public bool? SendIndividualSignatureNotifications { get; set; }
        /// <summary>
        /// This will return DefaultSignatureForStaticTemplate  
        /// </summary>
        public bool? IsDefaultSignatureForStaticTemplate { get; set; }
        /// <summary>
        /// This will return ReferenceCode
        /// </summary>
        public string? ReferenceCode { get; set; }
        /// <summary>
        /// This will return ReferenceEmail
        /// </summary>
        public string? ReferenceEmail { get; set; }

        /// <summary>
        /// This will set StoreOriginalDocument required for envelope or not
        /// </summary>       
        public bool? IsStoreOriginalDocument { get; set; }

        /// <summary>
        /// This will set  StoreSignatureCertificate required for envelope or not
        /// </summary>       
        public bool? IsStoreSignatureCertificate { get; set; }
        /// <summary>
        /// This will return the Private Mode value
        /// </summary>
        public Nullable<bool> IsPrivateMode { get; set; }

        public Nullable<bool> IsEnableFileReview { get; set; }
        /// <summary>
        /// This will return the rule editable value
        /// </summary>
        public bool? IsRuleEditable { get; set; }

        public bool? IsAdditionalAttmReq { get; set; }
        /// <summary>
        /// Enable AutoFill Text Controls true or false
        /// </summary>
        public bool? IsEnableAutoFillTextControls { get; set; }

        /// <summary>
        /// This will return the list of attachments requested
        /// </summary>
        public List<TemplateGroupDocumentUploadDetails> TemplateGroupDocumentUploadDetailsList { get; set; }

        public bool IsSignerIdentity { get; set; }

        public int? IsEnvelopeExpirationRemindertoSender { get; set; }
        public int? EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
        public string? EnvelopeExpirationRemindertoSender { get; set; }
        public int? IsSendReminderTillExpiration { get; set; }
        public string? SendReminderTillExpiration { get; set; }

        public bool AllowMultiSigner { get; set; }
        public bool? SendConfirmationEmail { get; set; }

        public bool EnableCcOptions { get; set; }
        public bool? EnableRecipientLanguage { get; set; }
        public bool? EnableMessageToMobile { get; set; }
      //  public bool IsSMSAccessCode { get; set; }
        public bool RestrictRecipientsToContact { get; set; }
        public bool? IsAllowSignerstoDownloadFinalContract { get; set; }
        public bool? ReVerifySignerStaticTemplate { get; set; }
      //  public bool? IsReVerifySignerEmailAccessCode { get; set; }
       // public bool? IsReVerifySignerSMSAccessCode { get; set; }
        public DateTime? StaticLinkExpiryDate { get; set; }
    }
    public class APIControlTag
    {
        public double DocHeight { get; set; }
        public double DocWidth { get; set; }
        public double ImgHeight { get; set; }
        public double ImgWidth { get; set; }
        public List<APIControlProperties> Properties { get; set; }
        public string? EnvelopeID { get; set; }
        public string? DocumentID { get; set; }
        public string? DocumentName { get; set; }
        public int ImageDirStartIndex { get; set; }
        public string? pageName { get; set; }
    }
    public class APIControlProperties
    {
        public string? RecipientID { get; set; }
        public string? RecipientEmail { get; set; }
        public string? ControlName { get; set; }//
        public string? Required { get; set; }//
        public int FontSize { get; set; }//
        public string? FontFamily { get; set; }//
        public string? color { get; set; }//
        public string? Bold { get; set; }//
        public string? Underline { get; set; }//
        public string? Italic { get; set; }//
        public string? Text { get; set; }//Tx#Mak
        public string? Maxchar { get; set; }//M#20
        public string? TextType { get; set; }//
        public double XCoordinate { get; set; }//
        public double YCoordinate { get; set; }//
        public int PageNo { get; set; }
        public int ImagePageNo { get; set; }
    }
    public class EnvelopeSignDocumentSubmitInfo
    {
        public Guid EnvelopeID { get; set; }
        public Guid RecipientID { get; set; }
        public List<DocumentContents> ControlCollection { get; set; }
        public string? RecipientEmail { get; set; }
        public Guid StaticTemplateID { get; set; }
        public bool IsEnvelopeComplete { get; set; }
        public string? Comment { get; set; }
        public bool IsConfirmationEmailReq { get; set; }
        public string? TemplateKey { get; set; }
        public string? CertificateSignature { get; set; }
        public string? UrlForFinishLater { get; set; }
        public string? IpAddress { get; set; }
        public int? DeclineReasonID { get; set; }
        public bool IsFromInbox { get; set; }
        public string? CopyEmail { get; set; }
        public string? IsReviewed { get; set; }
        public string? LanguageCode { get; set; }
        public string? CurrentRoleID { get; set; }
        public string? CurrentEmail { get; set; }
        public string? CultureInfo { get; set; }
        public List<InviteSignerModel>? InviteSignerModels { get; set; }
        public bool? IsBack { get; set; }
        public bool? IsSaveControl { get; set; }
        public string? RecipientMobile { get; set; }
        public string? RecipientDeliveryMode { get; set; }
        public string? RecipientCountryCode { get; set; }
        public string? RecipientDialCode { get; set; }
        public List<SendConfirmationDataModel>? SendConfirmationData { get; set; }
        public List<DeclineDataModel>? DeclineData { get; set; }
        public bool? IsSendEmailOnFinishLater { get; set; }
        public bool? IsSendMessageCodetoAvailableEmailorMobile { get; set; }
    }

    public class InviteSignerModel
    {
        public string? RoleId { get; set; }       
        public string? RoleName { get; set; }       
        public string? SignerName { get; set; }
        public string? SignerEmail { get; set; }
        public int SignNowOrInvitedEmail { get; set; } 
        public bool? CanEdit { get; set; }
        public string? InvitesignerMessage { get; set; }
        public Guid RecipientID { get; set; }
        public bool IscurrentRecipient { get; set; }
        public string? CertificateSignature { get; set; }
        public string? CultureInfo { get; set; }
        public string SignerDialCode { get; set; }
        public string SignerCountryCode { get; set; }
        public string SignerMobile { get; set; }
        public string SignerDeliveryMode { get; set; }
      //  public int ReminderType { get; set; }
    }
    public class SendConfirmationDataModel
    {
        public bool IsEmailChecked { get; set; }
        public bool IsMobileChecked { get; set; }
        public string DialCode { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }
    public class DeclineDataModel
    {
        public bool IsEmailExists { get; set; }
        public bool IsMobileExists { get; set; }
        public string DialCode { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string DeliveryMode { get; set; }
    }
    public class ResponseMessageFile
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return File Name.
        /// </summary>
        public string? FileName { get; set; }
        public string FileType { get; set; }
        /// <summary>
        /// This will return File Path.
        /// </summary>
        public string? FilePath { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return Base64 string for final PDF document.
        /// </summary>
        public string? Base64FileData { get; set; }
        /// <summary>
        /// This will return Bytes of xml doc.
        /// </summary>
        public byte[] byteArray { get; set; }
        /// <summary>
        /// This will return Name of Document.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// This will return Description of Document 
        /// </summary>
        public string? Description { get; set; }

    }
    public class ShareMessageTemplateInfo
    {
        /// <summary>
        /// This will set Message Template Id.
        /// </summary>
        public string? MessageTemplateID { get; set; }
        /// <summary>
        /// This will set Message Template as Shared or Unshared.
        /// </summary>
        public bool IsShare { get; set; }
        /// <summary>
        /// This will set TemplateCode.
        /// </summary>
        public int TemplateCode { get; set; }

    }

    public class DownloadDocuments
    {
        public string? EnvelopeCode { get; set; }
        public List<FilesDetails> DocumentList { get; set; }
        public byte[] ReadMe { get; set; }
        public FilesDetails CombinedZip { get; set; }
    }

    public class FilesDetails
    {
        public string? DocumentType { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public byte[] ByteArray { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? AttachmentDescription { get; set; }
    }

    public class MasterEnvelopeDownloadDocuments
    {
        public string? MasterEnvelopeCode { get; set; }
        public List<DownloadDocuments> Envelopes { get; set; }
        public List<FilesDetails> DocumentLists { get; set; }
        //public byte[] ReadMe { get; set; }
        public FilesDetails CombinedZip { get; set; }
        public FilesDetails AttachmentRequest { get; set; }
    }
    public class DocumentsListWithEnvelopedetails
    {
        public Guid EnvelopeID { get; set; }
        public Guid DocumentID { get; set; }
        public string? DocumentName { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderName { get; set; }
        public short? Order { get; set; }
        public string? SourceLocation { get; set; }
        public string? SourceDocumentId { get; set; }

        public bool IsSeparateDocumentsAfterSign { get; set; }
        public string? EnvelopeCode { get; set; }
        public bool? IsFinalCertificateReq { get; set; }
        public string? DocumentSource { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Guid DateFormatID { get; set; }
        public string? EntityId { get; set; }
        public string? User { get; set; }
        public string? EntityType { get; set; }

    }
    //public partial class EnvelopeTemplateGroups
    //{
    //    public string? EnvelopeCodeDisplay { get; set; }
    //    public string? EnvelopeStatusDescription { get; set; }
    //    public int SignerCount { get; set; }
    //    public RecipientsDetail SenderDetails { get; set; }
    //    public bool IsSequential { get; set; }
    //    public bool IsCompanyTransparencyFlagSet { get; set; }
    //    public Guid GlobalEnvelopeID { get; set; }
    //    public string? EnvelopeStage { get; set; }
    //    public List<RecipientsDetailsAPI> RecipientsDetail { get; set; }
    //    /// <summary>
    //    /// This will return IpAddress
    //    /// </summary>
    //    public string? IpAddress { get; set; }
    //    public string? DocumentJson { get; set; }
    //    public string? RecipientsJson { get; set; }
    //}

    public partial class DestinationData
    {
        public long TransId { get; set; }
        public string? EnvelopeCode { get; set; }
        public Nullable<System.Guid> MessageId { get; set; }
    }
    public class ResponseMessageForRedirectURL
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string Message { get; set; }       

    }
    public class CorporateURLs
    {
        public string Name { get; set; }
        public string URL { get; set; }
    }
}
