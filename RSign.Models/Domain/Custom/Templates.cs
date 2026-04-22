using RSign.Models.APIModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Xml.Serialization;

namespace RSign.Models
{
    public partial class Template
    {
        [NotMapped]
        public int SignerCount { get; set; }
        [NotMapped]
        public string? EnvelopeStage { get; set; }
        [NotMapped]
        public Guid GlobalEnvelopeID { get; set; }
        [NotMapped]
        public string DocumentJson { get; set; }
        [NotMapped]
        public string? RoleJson { get; set; }
        /// <summary>
        /// This will return the list of attachments requested
        /// </summary>
        public List<TemplateGroupDocumentUploadDetails> TemplateGroupDocumentUploadDetailsList { get; set; }

    }
    public class TemplateModel
    {
        public string? TemplatePath { set; get; }
        public string? TemplateName { set; get; }
    }
    /// <summary>
    /// Templates/Rules short metadata for consume template on Envelope page
    /// </summary>
    public class APITemplateMetaDetails
    {
        /// <summary>
        /// Get/Set ID of template/rule
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Get/Set hash ID of template/rule
        /// </summary>
        public string? HashID { get; set; }
        /// <summary>
        /// Get/Set code of template/rule
        /// </summary>
        public int? Code { get; set; }
        /// <summary>
        /// Get/Set name of template/rule
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Get/Set description of template/rule
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Get/Set TypeName of template/rule
        /// </summary>
        public string? TypeName { get; set; }
        /// <summary>
        /// Get/Set DocumentNameString of template/rule
        /// </summary>
        public string? DocumentNameString { get; set; }

        /// <summary>
        /// Get/Set Created Date Time of template/rule
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// Get/Set UserName of template/rule
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Get/Set UserEmailId of template/rule
        /// </summary>
        public string? UserEmailId { get; set; }
        /// <summary>
        /// Get/Set IsMasterSelected of template/rule
        /// </summary>
        public bool? IsMasterSelected { get; set; }
    }
    /// <summary>
    /// Get/Set templates/rules full details required on Template Tab
    /// </summary>
    public class APITemplates
    {
        /// <summary>
        /// Get/Set ID of template/rule
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Get/Set owner of template in case if template is shared
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// Get/Set hash ID of template/rule
        /// </summary>
        public string? HashID { get; set; }
        /// <summary>
        /// Get/Set the type of entity. Template/Rule in terms of master guid
        /// </summary>
        public Guid? TypeID { get; set; }
        /// <summary>
        /// Get/Set the type of entity. Template/Rule in terms of string
        /// </summary>
        public string? TypeName { get; set; }
        /// <summary>
        /// Get/Set code of template/rule
        /// </summary>
        public int? Code { get; set; }
        /// <summary>
        /// Get/Set name of template/rule
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Get/Set description of template/rule
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Get/Set created date of template/rule
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Get/Set the created date time of template in local time zone based on user settings
        /// </summary>
        public string? CreationDateLocal { get; set; }
        /// <summary>
        /// Get/Set is template/rule editable or not. 
        /// </summary>
        public bool IsTemplateEditable { get; set; }
        /// <summary>
        /// Get/Set if template is shared or not
        /// </summary>
        /// Updated by Tparker-Enhancement Sharing TemplatesRules with Share MasterAllow Copy Options
        public int? IsTemplateShared { get; set; }
        /// <summary>
        /// This will return if template content editable flag when template shared
        /// </summary>
        public bool IsSharedTemplateContentUnEditable { get; set; }
        /// <summary>
        /// Get/Set Comma(,) separated role list of tempalte/rule
        /// </summary>
        public string? Roles { get; set; }
        /// <summary>
        /// Get/Set Comma(,) separated document list of template/rule
        /// </summary>
        public string? Documents { get; set; }
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
        /// This will return envelope ShareId flag
        /// </summary>
        public Guid? ShareId { get; set; }
        /// <summary>
        /// This will envelope DefaultSignatureRequiredForStaticTemplate for template flag
        /// </summary>
        public bool? IsDefaultSignatureRequiredForStaticTemplate { get; set; }
        public Nullable<bool> IsStaticLinkDisabled { get; set; }

        public string? FileReviewDocuments { get; set; }
        /// <summary>
        /// This will return envelope IsRuleEditable flag
        /// </summary>
        public Nullable<bool> IsRuleEditable { get; set; }

        /// <summary>
        /// This will return Template Created User EmailId
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// This will return Template Created date
        /// </summary>
        public string? SharedCreatedDate { get; set; }

        /// <summary>
        /// This will return shared Template Code
        /// </summary>
        public Nullable<int> SharedTemplateCode { get; set; }

        /// <summary>
        /// This will return shared Template  Name
        /// </summary>
        public string? SharedTemplateName { get; set; }

        /// <summary>
        /// This will return Shared Template Created User EmailId
        /// </summary>
        public string? SharedBy { get; set; }

        /// <summary>
        /// This will retuen Template Modified Date
        /// </summary>
        public string? ModifiedDate { get; set; }
        public Nullable<bool> SendConfirmationEmail { get; set; }
    }
    /// <summary>
    /// Get/Set data required on Template tab, i.e. Templates, Shared Templates, Rules
    /// </summary>
    public class TemplateTabDetails
    {
        /// <summary>
        /// Get template tab details for the requested user
        /// </summary>
        /// <param name="userid">Get user id for which details are required</param>
        public TemplateTabDetails(Guid userid)
        {
            this.UserID = userid;
            TemplatesAndRule = new List<APITemplates>();
            SharedTemplates = new List<APITemplates>();
        }
        /// <summary>
        /// Get/Set the user id for which templates, shared templates and rules are retrieving
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// Get/Set company id of user if any
        /// </summary>
        public Guid? UserCompanyID { get; set; }
        /// <summary>
        /// Get/Set if rule feature is enabled/disabled for user
        /// </summary>
        public bool IsRuleFeatureActiveForUser { get; set; }
        /// <summary>
        /// Get/Set all templates and rules created by user which are active
        /// </summary>
        public List<APITemplates> TemplatesAndRule { get; set; }
        /// <summary>
        /// Get/Set all templates shared with organization and which are active
        /// </summary>
        public List<APITemplates> SharedTemplates { get; set; }
        /// <summary>
        /// Get/Set templates Static Link
        /// </summary>
        public string? StaticLink { get; set; }
        /// <summary>
        /// This will return Key and value for Multilingual
        /// </summary>
        public Dictionary<Guid?, string> DicLabelText { get; set; }
        public bool IsDefaultCompanyUser { get; set; }
        public bool? IsStatic { get; set; }
        /// <summary>
        /// This will return Key and value for DefaultSignatureRequiredForStaticTemplate
        /// </summary>
        public bool? IsDefaultSignatureRequiredForStaticTemplate { get; set; }

        public int? totalCount { get; set; }
        public Nullable<bool> IsStaticLinkDisabled { get; set; }
    }
    /// <summary>
    /// Get the response for template tab details
    /// </summary>
    public class APITemplateTabResponse
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
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Get/Set template tab details for the requested user
        /// </summary>
        public TemplateTabDetails TemplateTabDetails { get; set; }
    }
    public class APITemplateDetails
    {
        /// <summary>
        /// This will return template guid id
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// This will return Encrypted Template ID
        /// </summary>
        public string? TemplateHashID { get; set; }
        /// <summary>
        /// This will return Template Code.
        /// </summary>
        public int TemplateCode { get; set; }
        /// <summary>
        ///  This will return Template Name.
        /// </summary>
        public string? TemplateName { get; set; }
        /// <summary>
        ///  This will return template description.
        /// </summary>
        public string? TemplateDescription { get; set; }
        /// <summary>
        /// This will return template's created date and time.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        ///  This will return template as editable or not.
        /// </summary>
        public bool IsTemplateEditable { get; set; }
        /// <summary>
        ///This will return template as deleted or not.
        /// </summary>
        public bool IsTemplateDeleted { get; set; }
        /// <summary>
        /// This will return template as shared or not.
        /// </summary>
        public bool IsTemplateShared { get; set; }

        /// <summary>
        /// This will return roles of the template.
        /// </summary>
        public string? Roles { get; set; }
        /// This will return role details of the template.
        /// </summary>
        public string? DocumentDetails { get; set; }
        /// This will return role details of the template.
        /// </summary>
        public Guid? EnvelopeTypeId { get; set; }


    }
    public class TemplateDetails
    {
        public bool IsCompanyTransparencyFlagSet { get; set; }
        /// <summary>
        /// This will return Template Code.
        /// </summary>
        public int TemplateCode { get; set; }
        /// <summary>
        /// This will return Template Name.
        /// </summary>
        public string? TemplateName { get; set; }
        /// <summary>
        /// This will return template description.
        /// </summary>
        public string? TemplateDescription { get; set; }
        /// <summary>
        /// This will return template's created date and time.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// This will return template as editable or not.
        /// </summary>
        public bool IsTemplateEditable { get; set; }
        /// <summary>
        /// This will return template as deleted or not.
        /// </summary>
        public bool IsTemplateDeleted { get; set; }
        /// <summary>
        /// This will return Date Format ID of template.
        /// </summary>
        public Guid DateFormatID { get; set; }
        /// <summary>
        /// This will return Expiry Date and Time of template.
        /// </summary>
        public DateTime ExpiryDateTime { get; set; }
        /// <summary>
        /// This will return Expiry Type ID of template.
        /// </summary>
        public Guid ExpiryTypeID { get; set; }
        /// <summary>
        /// This will return Template Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// This will return last modified date and time.
        /// </summary>
        public DateTime ModifiedDateTime { get; set; }
        /// <summary>
        /// This will return password key.
        /// </summary>
        public string? PasswordKey { get; set; }
        /// <summary>
        /// This will return password key size.
        /// </summary>
        public int? PasswordKeySize { get; set; }
        /// <summary>
        /// This will return flag for password required to open signed document.
        /// </summary>
        public bool PasswordReqdToOpen { get; set; }
        /// <summary>
        ///  This will return flag for password required to sign.
        /// </summary>
        public bool PasswordReqdToSign { get; set; }
        /// <summary>
        /// This will return password to open the signed document.
        /// </summary>
        public string? PasswordToOpen { get; set; }
        /// <summary>
        /// This will return password to sign.
        /// </summary>
        public string? PasswordToSign { get; set; }
        /// <summary>
        /// This will return days to send the reminder.
        /// </summary>
        public int? RemainderDays { get; set; }
        /// <summary>
        /// This will return days to repeat the reminder.
        /// </summary>
        public int? RemainderRepeatDays { get; set; }
        /// <summary>
        /// This will return Status Id of the template.
        /// </summary>
        public Guid StatusID { get; set; }
        /// <summary>
        /// This will return GUID for the mentioned Date Format.
        /// </summary>
        public string? DateFormat { get; set; }
        /// <summary>
        /// This will return document details 
        /// </summary>
        public List<DocumentDetails> documentDetails { get; set; }
        /// <summary>
        /// This will return User Id.
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// This will return expiry type of the template.
        /// </summary>
        public string? ExpiryType { get; set; }
        /// <summary>
        /// This will return role details of the template.
        /// </summary>
        public List<RolesDetails> RoleList { get; set; }
        /// <summary>
        /// This will return Reminder type ID weeks or Days
        /// </summary>
        public Guid? ReminderTypeID { get; set; }

        public Guid? ThenReminderTypeID { get; set; }

        public int? FinalReminderDays { get; set; }
        /// <summary>
        /// This will return the Final reminder type ID 
        /// </summary>
        public Guid? FinalReminderTypeID { get; set; }
    }

    public class RolesDetails
    {
        /// <summary>
        /// This will return Role Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        ///  This will return Template Id.
        /// </summary>
        public Guid TemplateID { get; set; }
        /// <summary>
        /// This will return Role name.
        /// </summary>
        public string? RoleName { get; set; }
        /// <summary>
        /// This will return Role's order.
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// This will return Role's created date and time.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// This will return Role's type.
        /// </summary>
        public string? RoleType { get; set; }
    }
    public class APIStaticTemplate
    {
        /// <summary>
        /// Get Set Template Code.
        /// </summary>
        public int TemplateCode { get; set; }
        /// <summary>
        /// Get Set Recipient Email.
        /// </summary>
        public string? RecipientEmail { get; set; }
        /// <summary>
        /// Get Set Recipient Name.
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// Get Set if Signing email required.
        /// </summary>
        public bool IsSigningEmail { get; set; }
        /// <summary>
        /// Get Set post signing url
        /// </summary>
        public string? PostSigningUrl { get; set; }
        /// <summary>
        /// Get Set Ip Address
        /// </summary>
        public string? IpAddress { get; set; }
        /// <summary>
        /// Get Set Control Details
        /// </summary>
        public List<TemplateTrasnaction> TransDetails { get; set; }

        /// <summary>
        /// Get set CultureInfo
        /// </summary>
        public string? CultureInfo { get; set; }

        /// <summary>
        /// Get Set MessageTemplateTextID
        /// </summary>
        public Guid? MessageTemplateTextID { get; set; }
        /// <summary>
        /// Get Set UserSignatureTextID
        /// </summary>
        public Guid? UserSignatureTextID { get; set; }

        /// Get set Reference Code
        /// </summary>
        public string? ReferenceCode { get; set; }
        /// <summary>
        /// Get set Reference Email
        /// </summary>
        public string? ReferenceEmail { get; set; }

        /// <summary>
        /// This will set Date Format Id of envelope.
        /// </summary>
        public Guid DateFormatID { get; set; }
        /// <summary>
        /// This will set Expiry Type Id of envelope.
        /// </summary>
        public Guid ExpiryTypeID { get; set; }
        /// <summary>
        /// Get set ReminderDays
        /// </summary>
        public int? ReminderDays { get; set; }
        /// <summary>
        /// Get set ReminderRepeatDays
        /// </summary>
        public int? ReminderRepeatDays { get; set; }
        /// <summary>
        /// Get set ReminderTypeID
        /// </summary>
        public Guid? ReminderTypeID { get; set; }
        /// <summary>
        /// Get set ThenReminderTypeID
        /// </summary>
        public Guid? ThenReminderTypeID { get; set; }
        /// <summary>
        /// Get set FinalReminderDays
        /// </summary>
        public int? FinalReminderDays { get; set; }
        /// <summary>
        /// Get set FinalReminderTypeID
        /// </summary>
        public Guid? FinalReminderTypeID { get; set; }

        /// <summary>
        /// Get set IsSequenceCheck
        /// </summary>
        public bool? IsSequenceCheck { get; set; }
        /// Get set SignatureCertificateRequired
        /// </summary>
        public bool? SignatureCertificateRequired { get; set; }
        /// Get set IsFinalDocLinkReq
        /// </summary>
        public bool? IsFinalDocLinkReq { get; set; }
        /// <summary>
        /// Get set IsTransparencyDocReq
        /// </summary>
        public bool? IsTransparencyDocReq { get; set; }
        /// <summary>
        /// Get set IsSignerAttachFileReq
        /// </summary>
        public bool? IsSignerAttachFileReq { get; set; }
        public int IsSignerAttachFileReqNew { get; set; }
        //// <summary>
        /// Get set IsAttachXMLDataReq
        /// </summary>
        public bool? IsAttachXMLDataReq { get; set; }
        /// <summary>
        /// Get set IsSeparateMultipleDocumentsAfterSigningRequired
        /// </summary>
        public bool? IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
        //// <summary>
        /// Get set SendIndividualSignatureNotifications
        /// </summary>
        public bool? SendIndividualSignatureNotifications { get; set; }
        //// <summary>
        /// Get Set IsPasswordMailToSigner
        /// </summary>
        public bool? IsPasswordMailToSigner { get; set; }
        /// <summary>
        /// This will get Subject 
        /// </summary>
        public string? Subject { get; set; }
        /// <summary>
        /// This will get Body 
        /// </summary>
        public string? Body { get; set; }
        /// <summary>
        /// This will set AccessAuthType
        /// </summary>
        ///
        [XmlIgnore]
        public Guid? AccessAuthType { get; set; }
        /// <summary>
        /// This will set AccessAuthPassword
        /// </summary>
        ///
        [XmlIgnore]
        public string? AccessAuthPassword { get; set; }

        public Nullable<bool> IsAdditionalAttmReq { get; set; }

        public List<SendEnvelopeFromGroupAttachmentRequests> AttachmentRequests { get; set; }

        //public bool IsSignerIdentity { get; set; }
        public string? CreatedSource { get; set; }
        public string? EnvelopeExpirationRemindertoSender { get; set; }
        public string? SendReminderTillExpiration { get; set; }
        public Nullable<int> IsEnvelopeExpirationRemindertoSender { get; set; }
        public Nullable<int> ISSendReminderTillExpiration { get; set; }
        public Nullable<int> EnvelopeExpirationRemindertoSenderReminderDays { get; set; }

        public bool EnableCcOptions { get; set; }
        public string? AppKey { get; set; }
        public bool? EnableRecipientLanguage { get; set; }
        public bool? EnableMessageToMobile { get; set; }
      //  public bool IsSMSAccessCode { get; set; }
        public int? DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
      //  public int? ReminderType { get; set; }
    }

    public class TemplateTrasnaction
    {
        /// <summary>
        /// Get Set Control Id
        /// </summary>
        public Guid ControlId { get; set; }
        /// <summary>
        /// Get Set Control Name
        /// </summary>
        public string? ControlName { get; set; }
        /// <summary>
        /// Get Set Control Value
        /// </summary>
        public string? ControlValue { get; set; }
        /// <summary>
        /// Get Set Control Read only property
        /// </summary>
        public bool IsReadOnly { get; set; }

        public string? CustomToolTip { get; set; }

        public string? FontTypeMeasurement { get; set; }
    }
    public partial class TemplateGroupContents
    {
        public int? TemplateCode { get; set; }
    }
    public class BulkUploadFormat
    {
        public string? FileType { get; set; }
        public string? DocumentBase64Data { get; set; }
        public List<BulkUploadTemplate> BulkUploadTemplate { get; set; }
        public bool? IsFromSendUI { get; set; }
    }

    public partial class BulkUploadTemplate
    {
        /// <summary>
        /// This will return the First Name 
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// This will return the Last Name 
        /// </summary>
        public string? LastName { get; set; }
    }
    public class APITemplateFilter
    {
        /// <summary>
        /// Get/Set TypeId of template/rule
        /// </summary>
        public Guid TypeId { get; set; }
        /// <summary>
        /// Get/Set PageSize of template/rule
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// Get/Set Page of template/rule
        /// </summary>
        public int? Page { get; set; }
        /// <summary>
        /// Get/Set SortBy of template/rule
        /// </summary>
        public string? SortBy { get; set; }
        /// <summary>
        /// Get/Set search of template/rule
        /// </summary>
        public string? search { get; set; }
        /// <summary>
        /// Get/Set IsForTemplateAndRule of template/rule
        /// </summary>
        public bool IsForTemplateAndRule { get; set; }
        /// <summary>
        ///  Get/Set Is Template Master or Personal of template/rule
        ///  Added by Tparker-Enhancement Sharing TemplatesRules with Share MasterAllow Copy Options
        /// </summary>
        public string? TemplateType { get; set; }
    }

    public class ResponseMessageWithTemplateApi
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
        /// This will return the list of envelopes.
        /// </summary>
        public List<APITemplateMetaDetails> templates { get; set; }

        /// <summary>
        /// Get/Set TotalTempRowsCount of template/rule
        /// </summary>
        public int? TotalTempRowsCount { get; set; }
    }

    public partial class TemplateAPI
    {
        public System.Guid ID { get; set; }
        public System.Guid UserID { get; set; }
        public System.Guid DateFormatID { get; set; }
        public System.Guid ExpiryTypeID { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public Nullable<int> ReminderDays { get; set; }
        public Nullable<int> ReminderRepeatDays { get; set; }
        public bool PasswordReqdtoSign { get; set; }
        public string? PasswordtoSign { get; set; }
        public bool PasswordReqdtoOpen { get; set; }
        public string? PasswordtoOpen { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public System.DateTime ModifiedDateTime { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public Nullable<int> PasswordKeySize { get; set; }
        public string? PasswordKey { get; set; }
        public bool IsSequenceCheck { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> TemplateCode { get; set; }
        public string? TemplateName { get; set; }
        public string? TemplateDescription { get; set; }
        public bool IsTemplateEditable { get; set; }
        public string? CultureInfo { get; set; }
        public Nullable<bool> IsTemplatePrepare { get; set; }
        public Nullable<int> IsTemplateShared { get; set; }
        public Nullable<bool> IsFinalCertificateReq { get; set; }
        public Nullable<bool> IsFinalDocLinkReq { get; set; }
        public Nullable<bool> IsTransparencyDocReq { get; set; }
        public Nullable<int> IsSignerAttachFileReq { get; set; }
        public Nullable<bool> IsAttachXML { get; set; }
        public Nullable<bool> IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
        public string? TemplateJson { get; set; }
        public Nullable<System.Guid> EnvelopeTypeId { get; set; }
        public Nullable<System.Guid> SharedTemplateID { get; set; }
        public Nullable<bool> IsStatic { get; set; }
        public string? EDisplayCode { get; set; }
        public Nullable<System.Guid> TemplateKey { get; set; }
        public Nullable<System.Guid> AccessAuthType { get; set; }
        public bool IsPasswordMailToSigner { get; set; }
        public bool IsRandomPassword { get; set; }
        public bool IsSharedTemplateContentUnEditable { get; set; }
        public bool IsTemplateCompleted { get; set; }
        public string? PostSigningLandingPage { get; set; }
        public Nullable<System.Guid> ReminderTypeID { get; set; }
        public Nullable<System.Guid> ThenReminderTypeID { get; set; }
        public Nullable<System.Guid> UserSignatureTextID { get; set; }
        public Nullable<System.Guid> MessageTemplateTextID { get; set; }
        public Nullable<bool> SendIndividualSignatureNotifications { get; set; }
        public Nullable<int> FinalReminderDays { get; set; }
        public Nullable<System.Guid> FinalReminderTypeID { get; set; }
        public Nullable<System.DateTime> LastAccessedDateTime { get; set; }
        public Nullable<bool> IsDefaultSignatureForStaticTemplate { get; set; }
        public string? ReferenceCode { get; set; }
        public string? ReferenceEmail { get; set; }
        public Nullable<bool> IsPrivateMode { get; set; }
        public Nullable<bool> IsStoreOriginalDocument { get; set; }
        public Nullable<bool> IsStoreSignatureCertificate { get; set; }
        public Nullable<bool> IsStaticLinkDisabled { get; set; }
        public Nullable<bool> IsEnableFileReview { get; set; }
        public Nullable<bool> IsRuleEditable { get; set; }
        public Nullable<bool> IsAdditionalAttmReq { get; set; }
        public Nullable<bool> SendConfirmationEmail { get; set; }
        public virtual DateFormat DateFormat { get; set; }
        public virtual ExpiryType ExpiryType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TemplateDocuments> TemplateDocuments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TemplateRoles> TemplateRoles { get; set; }
    }
}
