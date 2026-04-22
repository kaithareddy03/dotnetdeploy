using RSign.ManageDocument.Models;
using RSign.Models.APIModels.Data;
using RSign.Models.APIModels.Envelope;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace RSign.Models.APIModels
{
  public class APIEnvelopeDetails
  {

  }
  public class ResponseMessage
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
    public object data { get; set; }
  }
  public class ResponseMessageResend
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
    /// This will set old to new recipient mapping in case of email has been changed
    /// </summary>
    public Dictionary<Guid, Guid> RecipientMapping { get; set; }
  }
  public class ConversionRespose
  {
    public ConversionRespose(Guid eid)
    {
      EnvelopeID = eid;
      ErrorneousTags = new List<ErrorTagDetailsResponse>();
      ValidTags = new List<ControlTag>();
      DocumentsInfo = new List<PrepareDocumentInfo>();
      ResponseType = ConversionResposeType.Success;
      RecipientDetails = new List<APIEnvelopeRecipientRequest>();
    }
    public Guid EnvelopeID { get; set; }
    public List<ErrorTagDetailsResponse> ErrorneousTags { get; set; }
    public List<ControlTag> ValidTags { get; set; }
    public List<PrepareDocumentInfo> DocumentsInfo { get; set; }
    public string? Message { get; set; }
    public ConversionResposeType ResponseType { get; set; }
    public List<APIEnvelopeRecipientRequest> RecipientDetails { get; set; }
  }
  public enum ConversionResposeType
  {
    Success,
    Exception,
    TaskTimeout
  }
  public class ResponseMessageForEnvelope
  {
    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public System.Net.HttpStatusCode StatusCode { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return  Envelope Id.
    /// </summary>
    public Guid EnvelopeId { get; set; }
    /// <summary>
    /// This will return  Envelope Code.
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return  Sign Document Url.
    /// </summary>
    public string? SignDoumentUrl { get; set; }
    /// <summary>
    /// This will return Recipient list with signing url.
    /// </summary>
    public List<APIRecipientList> RecipientList { get; set; }
  }
  public class ResponseMessageWithEnvlpGuid
  {
    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public System.Net.HttpStatusCode StatusCode { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return  Envelope Id.
    /// </summary>
    public string? EnvelopeId { get; set; }
    /// <summary>
    /// This will return  Envelope Display Code.
    /// </summary>
    public string? EDisplayCode { get; set; }
    /// <summary>
    /// Get errorneous tags found on document if any
    /// </summary>
    public List<ErrorTagDetailsResponse> ErrorTagDetailsResponse { get; set; }
  }
  public class ResponseMessageGetEnvelopeDetails
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
    /// This will return  Envelope Id.
    /// </summary>
    public string? EnvelopeId { get; set; }
    /// <summary>
    /// This will return envelope details
    /// </summary>
    public EnvelopeDetails EnvelopeDetails { get; set; }
    /// <summary>
    /// This will return true/false whether template is dated before portrait Landscape feature
    /// </summary>
    public bool IsTemplateDatedBeforePortraitLandscapeFeature { get; set; }
  }
  public class EnvelopeGetEnvelopeHistoryByEnvelopeCode
  {
    /// <summary>
    /// This will return Envelope Code.
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return Envelope's Subject.
    /// </summary>
    public bool IsSequential { get; set; }
    /// <summary>
    /// This will return Is sequential.
    /// </summary>
    public string EnvelopeCodeDisplay { get; set; }
    /// <summary>
    /// This will return Envelope's Subject.
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will return Envelope's Current Status.
    /// </summary>
    public string? CurrentStatus { get; set; }
    /// <summary>
    /// This will return Envelope's Sent details.
    /// </summary>
    public string? Sent { get; set; }
    /// <summary>
    /// This will return Envelope's Completed details.
    /// </summary>
    public string? Completed { get; set; }
    /// <summary>
    /// This will return Document details.
    /// </summary>
    public List<DocumentDetails> DocumentList { get; set; }
    /// <summary>
    /// This will return sender information.
    /// </summary>
    public string? Sender { get; set; }
    /// <summary>
    /// This will return delegated recipient information.
    /// </summary>
    public string? DelegatedTo { get; set; }
    /// <summary>
    /// This will return Envelope's Status .
    /// </summary>
    public string? EnvelopeStatusDescription { get; set; }
    /// <summary>
    /// This will return Document's history.
    /// </summary>
    public List<DocumentStatus> DocumentHistory { get; set; }
    /// <summary>
    /// This will return completed status details.
    /// </summary>
    public string? CompletedStatusDate { get; set; }
    /// This will return flag for password required to open signed document.
    /// </summary>
    public bool PasswordReqdToOpen { get; set; }
    /// <summary>
    ///  This will return flag for password required to sign the envelope.
    /// </summary>
    public bool PasswordReqdToSign { get; set; }
    /// <summary>
    /// This will return password to open the signed document.
    /// </summary>
    public string? PasswordToOpen { get; set; }
    /// <summary>
    /// This will return password to sign the envelope.
    /// </summary>
    public string? PasswordToSign { get; set; }

    /// <summary>
    /// This will return password to sign the envelope.
    /// </summary>
    public string? RecipientNameList { get; set; }
    public string? AllRecipientNameList { get; set; }

    //Manage Tab Optimization
    public Guid? recipientIdForWaitingForSignature { get; set; }

    /// <summary>
    /// This will return Status ID.
    /// </summary>
    public Guid StatusId { get; set; }
    /// <summary>
    /// This will return envelope ID.
    /// </summary>
    public Guid EnvelopeId { get; set; }
    /// <summary>
    /// This will return envelope ID.
    /// </summary>
    public DateTime ExpiryDate { get; set; }
    /// <summary>
    /// This will return bool for final link
    /// </summary>
    public bool? IsFinalDocLinkReq { get; set; }

    /// <summary>
    /// This will return bool for final link
    /// </summary>
    public bool IsTransparencyDocReq { get; set; }
    /// <summary>
    /// This will return bool for final link
    /// </summary>
    public Guid UserID { get; set; }

    /// <summary>
    /// This will return email of sender.
    /// </summary>
    public string? senderEmail { get; set; }

    /// <summary>
    /// This will return order
    /// </summary>
    public string? IPAddressSender { get; set; }
    /// <summary>
    /// Get/Set if signer accepts disclaimer
    /// </summary>
    public bool IsDiclaimerAccepted { get; set; }
    /// <summary>
    /// Get/Set if Final Contract Deleted
    /// </summary>
    public bool IsFinalContractDeleleted { get; set; }
    /// <summary>
    /// Get/Set Recipients Detail 
    /// </summary>
    public List<RecipientsDetailsAPI> RecipientsDetail { get; set; }
    /// <summary>
    /// This will return if Signer has attached any documents at time of signing
    /// </summary>
    public bool IsZipExist { get; set; }
    /// <summary>
    /// This will return file path of zip folder of signer attachments
    /// </summary>
    public string? ZipPath { get; set; }
    /// <summary>
    /// This will return if envelope is created from static template or not
    /// </summary>
    public bool IsEnvelopeStatic { get; set; }
    /// <summary>
    /// This will return Access authentication type for envelope
    /// </summary>
    public Guid AccessAuthenticationCode { get; set; }
    /// <summary>
    /// This will return Email Body for envelope
    /// </summary>
    public string? EnvelopeEmailBody { get; set; }
    /// <summary>
    /// This will return if user removed email body from manage tab
    /// </summary>
    public bool IsEmailBodyDisplay { get; set; }
    /// <summary>
    /// This will return Key and value for Multilingual
    /// </summary>
    public Dictionary<Guid?, string> DicLabelText { get; set; }
    /// <summary>
    /// This will return if sender can update envelope after sent
    /// </summary>
    public bool IsEnvelopeEditableAfterSend { get; set; }
    /// <summary>
    /// This will return if sender has updated envelope after sent
    /// </summary>
    public bool IsEnvelopeUpdated { get; set; }

    /// <summary>
    /// This will return if sender has edited the envelope using Update & Re-send button.
    /// </summary>
    public bool? IsEnvelopeEdited { get; set; }
    /// <summary>
    /// Get Company ID of Sender
    /// </summary>
    public Guid? SenderCompanyID { get; set; }
    /// <summary>
    /// Settings of Sender
    /// </summary>
    public AdminGeneralAndSystemSettings SenderAPISettings { get; set; }
    public string? FormattedSentDate { get; set; }
    public string? ReferenceCode { get; set; }
    public string? ReferenceEmail { get; set; }
    public bool IsTemplateShared { get; set; }
    public bool? IsResendDisabled { get; set; }
    public string? Message { get; set; }
    public bool IsEnvelopeHistory { get; set; }
    public Guid? DateFormatID { get; set; }
    public bool IsDataMasking { get; set; }
    public bool IsDataDeleted { get; set; }
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
    /// <summary>
    /// This will return the IsSignatureCertificateStored for soft delete
    /// </summary>
    public Nullable<bool> IsSignatureCertificateStored { get; set; }
    /// <summary>
    /// This will return the IsAdditionalAttmReq
    /// </summary>
    public bool? IsAdditionalAttmReq { get; set; }
    public string? AccessAuthenticationType { get; set; }
    public int RetentionDays { get; set; }

    public long? TemplateGroupId { get; set; }

    public bool AllowMultiSigners { get; set; }

    public bool IsWaitingforConfirmation { get; set; }

    public int WaitingforConfirmationCount { get; set; }

    public List<EnvelopeTemplateDetails> EnvelopeTemplateList { get; set; }

    public List<EnvelopeTemplateDetails> EnvelopeRuleList { get; set; }

    public string? CopyEnvelopeCode { get; set; }
    public string? SignInSequenceDescription { get; set; }

    public List<AwaitingRecipients> AwaitingRecipientsList { get; set; }
    public string EnableMessageToMobile { get; set; }
    public string DeliveryMode { get; set; }
    public string VerificationCode { get; set; }
    public bool? IsAllowSignerstoDownloadFinalContract { get; set; }
  }
  public class AwaitingRecipients
  {
    public Nullable<int> Order { get; set; }
    public string? Name { get; set; }
    public string? RecipientEmail { get; set; }
    public string CountryCode { get; set; }
    public string DialCode { get; set; }
    public string Mobile { get; set; }
    public int? DeliveryMode { get; set; }
    public string DeliveryModeText { get; set; }
  }

  public class EnvelopeTemplateDetails
  {
    public string? TemplateCode { get; set; }

    public string? TemplateName { get; set; }
    public string? EnvelopeTypeId { get; set; }
  }

  /// <summary>
  /// Filter parameter to get envelope list.
  /// </summary>
  public class FilterEnvelopeListforApi
  {
    /// <summary>
    /// List of users.
    /// </summary>
    public List<Guid> UserList { get; set; }
    /// <summary>
    /// email address.
    /// </summary>
    public string? EmailAddress { get; set; }
    /// <summary>
    /// Display Code.
    /// </summary>
    public string? EDisplayCode { get; set; }
    /// <summary>
    /// Guid Status.
    /// </summary>
    public Guid? StatusCode { get; set; }

    /// <summary>
    /// Envelope start date [Nullable]
    /// </summary>
    public DateTime? DateMinValue { get; set; }
    /// <summary>
    /// Envelope start date [Nullable]
    /// </summary>
    public DateTime? StartFromDate { get; set; }
    /// <summary>
    /// Envelope end date [Nullable]
    /// </summary>
    public DateTime? EndtoDate { get; set; }
    /// <summary>
    /// TotalEnvelopeCount
    /// </summary>
    public int TotalEnvelopeCount { get; set; }
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
    /// Envelopes from company [Nullable - For superuser company id is required.]
    /// </summary>
    public Guid? CompanyID { get; set; }
    /// <summary>
    /// Envelopes of user [Nullable]
    /// </summary>
    public Guid? UserID { get; set; }
    /// <summary>
    /// Envelope status [Nullable- If so All will be selected.]
    /// </summary>
    public int? EnvelopeStatus { get; set; }
    /// <summary>
    /// Envelope Subject
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// Envelope RecipientId
    /// </summary>
    public string? Recipient { get; set; }
    /// <summary>
    /// Show Envelope Data
    /// </summary>
    public int ShowData { get; set; }
    /// <summary>
    /// Is export to excel
    /// </summary>
    public bool IsExportToExcel { get; set; }

    public string? UserRole { get; set; }

    /// <summary>
    /// Is Recipient List Required or not
    /// </summary>
    public int? IsRecipientRequired { get; set; }

    /// <summary>
    /// Sender Required or not
    /// </summary>
    public int? IsSenderRequired { get; set; }
    /// <summary>
    /// This parameter is user to get sent envelopes from bullhorn
    /// </summary>
    public string? EntityTypeId { get; set; }

  }

  /// <summary>
  /// Filter parameter to get envelope list.
  /// </summary>
  public class EnvelopeStatusRequest
  {
    /// <summary>
    /// Envelope ID
    /// </summary>
    public Guid? EnvelopeId { get; set; }
    /// <summary>
    /// Display Code.
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// Display Code.
    /// </summary>
    public string? MasterEnvelopeCode { get; set; }
    /// <summary>
    /// email address.
    /// </summary>
    public int? Period { get; set; }
    /// <summary>
    /// Status.
    /// </summary>
    public string? Status { get; set; }
    /// <summary>
    /// Status Code GUID based on the status passed
    /// </summary>
    public Guid? StatusCode { get; set; }
    /// <summary>
    /// Status Code GUID based on the status passed
    /// </summary>
    public List<Guid> StatusCodeList { get; set; }
    /// <summary>
    /// Envelope start date
    /// </summary>
    public DateTime? StartDate { get; set; }
    /// <summary>
    /// Envelope end date
    /// </summary>
    public DateTime? EndDate { get; set; }
    /// <summary>
    /// Sender Email
    /// </summary>
    public string? SenderEmail { get; set; }
    /// <summary>
    /// Page info
    /// </summary>
    public string? SignerEmail { get; set; }
    /// <summary>
    /// Page size
    /// </summary>
    public bool IsSignerDetails { get; set; }
    /// <summary>
    /// Sort by detail
    /// </summary>
    public string? DetailOrSummary { get; set; }
    /// <summary>
    /// If Result needs to return only Master Envelopes
    /// </summary>
    public bool IsMasterEnvelopeRequest { get; set; }
    /// <summary>
    /// Document type to search
    /// </summary>
    public string? DocumentType { get; set; }
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserID { get; set; }
    /// <summary>
    /// List of all company users
    /// </summary>
    public List<Guid> CompanyUserList { get; set; }

  }
  public class EnvelopeStatusAPI
  {
    /// <summary>
    /// This will return Envelope Code.
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return Envelope ID.
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// This will return Envelope's Subject.
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will return Is sequential.
    /// </summary>
    public string? Status { get; set; }
    /// <summary>
    /// This will return Envelope's Subject.
    /// </summary>
    public DateTime SentDate { get; set; }
    /// <summary>
    /// This will return Last Modified Date.
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }
    /// <summary>
    /// This will return Envelope's Current Status.
    /// </summary>
    public string? SenderName { get; set; }
    /// <summary>
    /// This will return Envelope's Sent details.
    /// </summary>
    public string? SenderEmail { get; set; }
    /// <summary>
    /// This will return Envelope's Recipient details.
    /// </summary>
    public List<RecipientsBasicDetails> RecipientDetails { get; set; }
    /// <summary>
    /// Get Recipients
    /// </summary>
    [System.Runtime.Serialization.IgnoreDataMember]
    public List<Recipients> Recipients { get; set; }
    /// <summary>
    /// CultureInfo
    /// </summary>
    [System.Runtime.Serialization.IgnoreDataMember]
    public string? CultureInfo { get; set; }
  }
  public class MasterEnvelopeStatusAPI
  {
    /// <summary>
    /// This will return Envelope Code.
    /// </summary>
    public string? MasterEnvelopeCode { get; set; }
    /// <summary>
    /// This will return Envelope ID.
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// This will return Envelope's Subject.
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will return Is sequential.
    /// </summary>
    public string? Status { get; set; }
    /// <summary>
    /// This will return Envelope's Subject.
    /// </summary>
    public DateTime SentDate { get; set; }
    /// <summary>
    /// This will return Last Modified Date.
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }
    /// <summary>
    /// This will return Envelope's Current Status.
    /// </summary>
    public string? SenderName { get; set; }
    /// <summary>
    /// This will return Envelope's Sent details.
    /// </summary>
    public string? SenderEmail { get; set; }
    /// <summary>
    /// This will return Sub Envelope detail
    /// </summary>
    public List<SubEnvelopeStatusAPI> Envelopes { get; set; }
    /// <summary>
    /// This will return Envelope's Recipient details.
    /// </summary>
    public List<UploadAttachmentRequestsAPI> UploadAttachmentRequests { get; set; }
  }
  public class SubEnvelopeStatusAPI
  {
    /// <summary>
    /// This will return EnvelopeCode
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return Is sequential.
    /// </summary>
    public string? Status { get; set; }
    [System.Runtime.Serialization.IgnoreDataMember]
    public Guid StatusId { get; set; }
    /// <summary>
    /// This will return Last Modified Date.
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }
    /// <summary>
    /// This will return Envelope's Recipient details.
    /// </summary>
    public List<RecipientsBasicDetails> RecipientDetails { get; set; }
  }
  public class UploadAttachmentRequestsAPI
  {
    /// <summary>
    /// This will return Attachment Request ID.
    /// </summary>
    public long ID { get; set; }
    /// <summary>
    /// This will return Is sequential.
    /// </summary>
    public string? Status { get; set; }
    /// <summary>
    /// Internally used for searching (Uploaded = Completed,Not Uploaded=waiting for signature)
    /// </summary>
    [System.Runtime.Serialization.IgnoreDataMember]
    public Guid StatusId { get; set; }
    /// <summary>
    /// This will return Is sequential.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// This will return Uploaded Date.
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }
  }
  public class SearchEnvelopeForApi
  {
    public string? searchText { get; set; }
    public string? searchCol { get; set; }
    public string? sender { get; set; }
    public string? subject { get; set; }
    public string? referenceCode { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public string? envelopeStatus { get; set; }
    //   public int? option { get; set; }
    public string? activeTab { get; set; }
    public Sort sortObj { get; set; }
    public Paging pagingObj { get; set; }
    public int folderId { get; set; }
    public string? languageCode { get; set; }

    public List<Guid?> EnvelopeIds { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? ModifiedDateTime { get; set; }
    public List<filterObj> filters { get; set; }
    public string? filterOption { get; set; }
    public string? flag { get; set; }
    public string? envelopeStatusId { get; set; }
    public string? userTimezone { get; set; }
    public string? FolderName { get; set; }
    public int isFilterApplied { get; set; }
  }

  public class ManageFolderRequestObj
  {
    public List<Guid> EnvelopeIds { get; set; }
    public int FolderId { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? ModifiedDateTime { get; set; }
    public int? OldFolderId { get; set; }
    public bool IsActive { get; set; }
  }

  public class filterObj
  {
    public string? key { get; set; }
    public string? value { get; set; }
  }


  public class Sort
  {
    public string? SortCol { get; set; }
    public string? SortOrder { get; set; }

  }
  public class Paging
  {
    public int PageSize { get; set; }
    public int ActivePage { get; set; }

  }

  public class ResponseMessageWithEnvelopesForApi
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
    /// This will return Envelope Filter From Date.
    /// </summary>
    public DateTime? FromDate { get; set; }
    /// <summary>
    /// This will return Envelope Filter End Date.
    /// </summary>
    public DateTime? ToDate { get; set; }
    /// <summary>
    /// This will return the list of envelopes.
    /// </summary>
    public List<EnvelopeGetEnvelopeHistoryByEnvelopeCode> Envelopes { get; set; }

    /// <summary>
    /// This will return Total Envelope Count.
    /// </summary>
    public int TotalEnvelopeCount { get; set; }
  }
  public class ResponseMessageForTemplate
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
    /// This will return Template Id.
    /// </summary>
    public string? TemplateId { get; set; }
    /// <summary>
    /// This will return Template Code.
    /// </summary>
    public int? TemplateCode { get; set; }
    /// <summary>
    /// This will return Document ID.
    /// </summary>
    public string? DocumentID { get; set; }
    /// <summary>
    /// This will return Role ID.
    /// </summary>
    public string? Roles { get; set; }

    public string? LanguageBasedApiMessge { get; set; }

    public bool? IsEnvelopeExistByTemplateCode { get; set; }
    public string IntegrationUrl { get; set; }

    public int? DeliveryMode { get; set; }
    public string CountryCode { get; set; }
    public string DialCode { get; set; }

    public bool? EnableMessageToMobile { get; set; }
   // public bool? IsSMSAccessCode { get; set; }
  }
  public class ResponseMessageAddSharedTemplate
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
    /// This will give newly created template details
    /// </summary>
    public TemplateTabDetails TemplateTabDetails { get; set; }
  }
  public class ResponseMessageForDeleteTemplate
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
    /// This will return Template Id.
    /// </summary>
    public string? TemplateId { get; set; }
  }

  public class ResponseMessageForTemplateHistory
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
    /// This will return Template Id.
    /// </summary>
    public string? TemplateId { get; set; }

    public List<TemplateVersionHistory> TemplateHistory { get; set; }

    public Guid DateFormatId { get; set; }
  }
  public class EnvelopeDetails
  {
    /// <summary>
    /// This will return Envelop Id.
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// This will return envelope code
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return User Id.
    /// </summary>
    public Guid UserID { get; set; }
    /// <summary>
    /// This will return Date Format.
    /// </summary>
    public string? DateFormat { get; set; }
    /// <summary>
    /// This will return GUID for the Date Format.
    /// </summary>
    public string? DateFormatID { get; set; }
    /// <summary>
    /// This will return Date, Time for the created envelope.
    /// </summary>
    public DateTime CreatedDateTime { get; set; }
    /// <summary>
    /// This will return last modified Date, Time for the envelope.
    /// </summary>
    public DateTime ModifiedDatetTime { get; set; }
    /// <summary>
    /// This will return flag for password required to open signed document.
    /// </summary>
    ///
    [XmlIgnore]
    public bool PasswordReqdToOpen { get; set; }
    /// <summary>
    ///  This will return flag for password required to sign the envelope.
    /// </summary>
    ///
    [XmlIgnore]
    public bool PasswordReqdToSign { get; set; }
    /// <summary>
    /// This will return password to open the signed document.
    /// </summary>
    ///
    [XmlIgnore]
    public string? PasswordToOpen { get; set; }
    /// <summary>
    /// This will return password to sign the envelope.
    /// </summary>
    ///
    [XmlIgnore]
    public string? PasswordToSign { get; set; }
    /// <summary>
    /// This will return reminder days.
    /// </summary>
    public int? RemainderDays { get; set; }
    /// <summary>
    /// This will return days to repeat the reminder.
    /// </summary>
    public int? ReminderRepeatDays { get; set; }
    /// <summary>
    /// This will return signing certificate name of envelope
    /// </summary>
    public string? SigningCertificateName { get; set; }
    /// <summary>
    /// This will return subject of the envelope.
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will return message of the envelope.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return location of envelope
    /// </summary>
    public string? Location { get; set; }
    /// <summary>
    /// This will return document hash of envelope
    /// </summary>
    public string? DocumentHash { get; set; }
    /// <summary>
    /// This will return IsActive flag of envelope
    /// </summary>
    public bool? IsActive { get; set; }
    /// <summary>
    /// This will return flag for is  envelope.
    /// </summary>
    public bool IsEnvelope { get; set; }
    /// <summary>
    /// This will return expiry type of the envelope.
    /// </summary>
    public string? ExpiryType { get; set; }
    /// <summary>
    /// This will return GUID for the Expiry Type.
    /// </summary>
    public string? ExpiryTypeID { get; set; }
    /// <summary>
    /// This will return Status Id of the envelope.
    /// </summary>
    public Guid StatusID { get; set; }
    /// <summary>
    /// This will return template code.
    /// </summary>
    public int TemplateCode { get; set; }
    /// <summary>
    /// This will return flag for signature certificate.
    /// </summary>
    public bool? SignatureCertificateRequired { get; set; }
    /// <summary>
    /// This will return flag for download document.
    /// </summary>
    public bool? DownloadLinkOnManageRequired { get; set; }
    /// <summary>
    /// This will return document details 
    /// </summary>
    public List<DocumentDetails> DocumentDetails { get; set; }
    /// <summary>
    /// This will return recipient details of the envelope.
    /// </summary>
    public List<RecipientDetails> RecipientList { get; set; }
    /// <summary>
    /// This will return role details in case of template
    /// </summary>
    public List<RoleDetails> RoleList { get; set; }
    /// <summary>
    /// This will return envelope code.
    /// </summary>
    public string? EDisplayCode { get; set; }
    /// <summary>
    /// This will return password key.
    /// </summary>
    public string? PasswordKey { get; set; }
    /// <summary>
    /// This will return password key size.
    /// </summary>
    public int? PasswordKeySize { get; set; }
    /// <summary>
    /// This will return envelope transparency flag
    /// </summary>
    public bool IsTransperancyDocRequired { get; set; }
    /// <summary>
    /// This will return if template is deleted
    /// </summary>
    public bool IsTemplateDeleted { get; set; }
    /// <summary>
    /// This will return if template is editable
    /// </summary>
    public bool IsTemplateEditable { get; set; }
    /// <summary>
    /// This will return if envelope is prepare
    /// </summary>
    public bool IsEnvelopePrepare { get; set; }
    /// <summary>
    /// This will return if envelope is completed
    /// </summary>
    public bool IsEnvelopeComplete { get; set; }
    /// <summary>
    /// This will return template name if any
    /// </summary>
    public string? TemplateName { get; set; }
    /// <summary>
    /// This will return template description if any
    /// </summary>
    public string? TemplateDescription { get; set; }
    /// <summary>
    /// This will return IsDraft flag of envelope
    /// </summary>
    public bool? IsDraft { get; set; }
    /// <summary>
    /// This will return IsDraftDeleted flag of envelope
    /// </summary>
    public bool? IsDraftDeleted { get; set; }
    /// <summary>
    /// This will return IsDraftEditable flag of envelope
    /// </summary>
    public bool? IsDraftSend { get; set; }
    /// <summary>
    /// This will return CultureInfo for an envelope
    /// </summary>
    public string? CultureInfo { get; set; }
    /// <summary>
    /// This will return if signer are in sequence in an envelope
    /// </summary>
    public bool IsSequenceCheck { get; set; }
    /// <summary>
    /// This will return if template then is template shared
    /// </summary>
    public bool IsTemplateShared { get; set; }
    /// <summary>
    /// This will set stage of Envelope
    /// </summary>
    public string? EnvelopeStage { get; set; }
    /// <summary>
    /// This will return EnvelopJson
    /// </summary>
    public string? EnvelopJson { get; set; }
    /// <summary>
    /// This will return EnvelopeTypeId
    /// </summary>
    public Guid? EnvelopeTypeId { get; set; }
    /// <summary>
    /// This will return envelope IsSignerAttachFileReq flag
    /// </summary>
    public bool IsSignerAttachFileReq { get; set; }
    public int IsSignerAttachFileReqNew { get; set; }
    /// <summary>
    /// This will return envelope IsCreateStaticLink for template flag
    /// </summary>
    public bool? IsStatic { get; set; }
    /// <summary>
    /// This will return envelope IsAttachXMLData for template flag
    /// </summary>
    public bool IsAttachXMLDataReq { get; set; }
    /// This will return envelope IsSeparateMultipleDocumentsAfterSigningRequired flag
    /// </summary>
    public bool IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
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
    /// This will return IsEdited flag of envelope
    /// </summary>
    public bool? IsEdited { get; set; }
    /// <summary>
    /// This will set Post Signing Landing Page .
    /// </summary>
    public string? PostSigningLandingPage { get; set; }
    /// <summary>
    /// This will set Reminder Type ID.
    /// </summary>
    public Guid? ReminderTypeID { get; set; }
    /// <summary>
    /// This will set Then Reminder Type ID.
    /// </summary>
    public Guid? ThenReminderTypeID { get; set; }
    /// <summary>
    /// This will set User SignatureTextID.
    /// </summary>
    public Guid? UserSignatureTextID { get; set; }
    /// <summary>
    /// This will set User Message TemplateTextID.
    /// </summary>
    public Guid? MessageTemplateTextID { get; set; }
    /// <summary>
    /// This will return Envelope Status
    /// </summary>
    public string? Status { get; set; }
    /// <summary>
    /// This will return value of SendIndividualSignatureNotifications setting
    /// </summary>
    public bool? SendIndividualSignatureNotifications { get; set; }
    /// <summary>
    /// This will return value of HeaderFooterOption setting
    /// </summary>
    public int? HeaderFooterOption { get; set; }
    /// <summary>
    /// This will return value of DisclaimerText setting
    /// </summary>
    public string? DisclaimerText { get; set; }
    /// <summary>
    /// This will return value of IsDisclaimerInCertificate setting
    /// </summary>
    public bool? IsDisclaimerInCertificate { get; set; }
    /// <summary>
    /// This will return value of list of envelope directories
    /// </summary>
    public string?[] FilePath { get; set; }
    /// <summary>
    ///  This will return lis of Image Dimension Info
    /// </summary>
    public List<EnvelopeImageInformationDetails> EnvelopeImageInfo { get; set; }

    /// This will return the Final Reminder 
    /// </summary>
    public int? FinalReminderDays { get; set; }
    /// <summary>
    /// This will return the Final reminder type ID 
    /// </summary>
    public Guid? FinalReminderTypeID { get; set; }
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
    /// This will return TemplateGroupId
    /// </summary>
    public long? TemplateGroupId { get; set; }
    /// <summary>
    /// This will return TemplateGroupId
    /// </summary>
    public Guid? EnvelopeTemplateGroupId { get; set; }
    public int? ElectronicSignIndicationOptionID { get; set; }
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

    /// <summary>
    /// This will set StoreOriginalDocument required for envelope or not
    /// </summary>       
    public Nullable<bool> IsStoreOriginalDocument { get; set; }

    /// <summary>
    /// This will set  StoreSignatureCertificate required for envelope or not
    /// </summary>       
    public Nullable<bool> IsStoreSignatureCertificate { get; set; }
    public Nullable<bool> IsStaticLinkDisabled { get; set; }

    public Nullable<bool> IsEnableFileReview { get; set; }
    /// <summary>
    /// This will return the Rule or not
    /// </summary>
    public Nullable<bool> IsRule { get; set; }
    /// <summary>
    /// This will return the Rule is editable or not
    /// </summary>
    public Nullable<bool> IsRuleEditable { get; set; }
    /// <summary>
    /// This will return the template is shared or not
    /// </summary>
    public int IsShareOrCopyTemplate { get; set; }
    /// <summary>
    /// This will return the IsAdditionalAttmReq
    /// </summary>
    public bool? IsAdditionalAttmReq { get; set; }
    /// <summary>
    /// This will return the list of attachments requested
    /// </summary>
    public List<TemplateGroupDocumentUploadDetails> TemplateGroupDocumentUploadDetailsList { get; set; }
    /// <summary>
    /// This will return the default control style
    /// </summary>
    public string? DefaultControlStyle { get; set; }

    public Nullable<bool> EnableAutoFillTextControls { get; set; }

    public bool IsSignerIdentity { get; set; }

    public string? CreatedSource { get; set; }

    public string? EnvelopeCreatedDate { get; set; }

    public int? IsEnvelopeExpirationRemindertoSender { get; set; }
    public int? EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
    public string? EnvelopeExpirationRemindertoSender { get; set; }
    public int? IsSendReminderTillExpiration { get; set; }
    public string? SendReminderTillExpiration { get; set; }
    public string? DraftType { get; set; }

    public bool AllowMultiSigners { get; set; }
    public Nullable<bool> SendConfirmationEmail { get; set; }

    public Nullable<bool> IsExistSendConfirmationEmail { get; set; }
    public int? DigitalCertificate { get; set; }
    public string? AppKey { get; set; }

    public bool EnableCcOptions { get; set; }

    public bool? IsSameRecipientForAllTemplates { get; set; }

    public Nullable<System.Guid> CopyEnvelopeId { get; set; }

    public bool? EnableRecipientLanguage { get; set; }
    public bool? EnableMessageToMobile { get; set; }
    public string? AppliedEpicUser { get; set; }
    public string? AppliedEpicEntityId { get; set; }
    public string? EntityType { get; set; }
    public string? IntegrationType { get; set; }
    public bool? HideFixedwidthCheckbox { get; set; }

  }
  public class EnvelopeControls
  {
    /// <summary>
    /// This will set Envelope Id.
    /// </summary>
    public string? EnvelopeID { get; set; }
    /// <summary>
    /// This will set UserId.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Get/Set Envelope Type Id
    /// </summary>
    public Guid EnvelopeTypeId { get; set; }
    /// <summary>
    /// Get/Set Envelope Stages
    /// </summary>
    public string? Stage { get; set; }
    /// <summary>
    /// This will set userToken.
    /// </summary>
    public string? UserToken { get; set; }
    /// <summary>
    /// This will set Envelope display code.
    /// </summary>
    public string? EnvelopeDCode { get; set; }
    /// <summary>
    /// This will set IPAddress.
    /// </summary>
    public string? IpAddress { get; set; }
    /// <summary>
    /// This will get set signing method.
    /// </summary>
    public int? SigningMethod { get; set; }
    public Nullable<bool> IsStaticLinkDisabled { get; set; }
    /// <summary>
    /// Get/Set list of envelopes to add in envelope
    /// </summary>        
    public List<DocumentContentPOCO> Controls { get; set; }
    /// <summary>
    /// This will set the controls need to save or not
    /// </summary>
    public bool? IsSaveControl { get; set; }

    /// <summary>
    /// This will define is autosaved or not
    /// </summary>
    public bool IsAutoSave { get; set; }

    public bool? IsControlForPdfView { get; set; }
    public string? CreatedSource { get; set; }
    public bool IsPrefill { get; set; }

    public bool IsSelfSign { get; set; }
    public bool? IsSameRecipientForAllTemplates { get; set; }
    public string? AppKey { get; set; }

  }
  public class ResponseMessageForSendEnvelope : ResponseMessageForEnvelope
  {
    /// <summary>
    /// This will return envelope display code.
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return  message for transparency document.
    /// </summary>
    public string? TransparencyChangedMessage { get; set; }

    public string? LanguageBasedApiMessge { get; set; }

    public string? PrefillRecipientId { get; set; }
    public string? PostSendingNavigationPage { get; set; }
  }
  public class ViewPdf
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
    /// This will return Envelope Id.
    /// </summary>
    public string? EnvelopeID { get; set; }
    /// <summary>
    /// This will return Base64 string for uploaded document.
    /// </summary>
    public string? PageData { get; set; }
  }
  /// <summary>
  /// This will return drafts of user
  /// </summary>
  public class UserDrafts
  {
    /// <summary>
    /// This will return GUID id of draft
    /// </summary>
    public Guid DraftID { get; set; }
    /// <summary>
    /// This will return envelopes encrypted id
    /// </summary>
    public string? EnvelopeHashID { get; set; }
    /// <summary>
    /// This will return envelope code if any
    /// </summary>
    public int? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return template code if any
    /// </summary>
    public int? TemplateCode { get; set; }
    /// <summary>
    /// This will return Subject of Draft
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will return Message of Draft
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return creation date time of draft
    /// </summary>
    public DateTime CreationDate { get; set; }
    public string? FormattedCreatedDate { get; set; }
    public Guid? DateFormatID { get; set; }
    public string? EDisplayCode { get; set; }
  }
  /// <summary>
  /// Returns list of drafts of user
  /// </summary>
  public class ResponseMessageForDraftList
  {
    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }
    /// <summary>
    /// This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return UserID.
    /// </summary>
    public Guid UserID { get; set; }
    /// <summary>
    /// This will return CompanyID of user if any
    /// </summary>
    public Guid? CompanyID { get; set; }
    /// <summary>
    /// This will return list of drafts
    /// </summary>
    public List<UserDrafts> UserDrafts { get; set; }
    public int TotalDrafts { get; set; }
  }
  public class ResponseMessageDraft
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
    /// This will return  Envelope Id.
    /// </summary>
    public string? EnvelopeId { get; set; }
    public int? DeliveryMode { get; set; }
    public string CountryCode { get; set; }
    public string DialCode { get; set; }
  }
  public class SaveDraft
  {
    /// <summary>
    /// This will set Envelope Id.
    /// </summary>
    public string? EnvelopeID { get; set; }
    /// <summary>
    /// From this will determine, envelope to draft from which stage - Initialize or Prepare.
    /// </summary>
    public string? EnvelopeStage { get; set; }
    /// <summary>
    /// This will set subject of draft envelope.
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// Message of Envelope
    /// </summary>
    [AllowHtml]
    public string? Message { get; set; }
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
    /// This will set Reminder days
    /// </summary>
    public int? SendReminderIn { get; set; }
    /// <summary>
    /// This will set then send reminder days
    /// </summary>
    public int? ThenSendReminderIn { get; set; }
    /// <summary>
    /// This will set Include signature certificate on PDF
    /// </summary>
    public bool? SignatureCertificateRequired { get; set; }
    /// <summary>
    /// This will set Store signed PDF
    /// </summary>
    public bool? DownloadLinkRequired { get; set; }
    /// <summary>
    /// Get/Set sign in sequence for recipient for given envelope id
    /// </summary>
    public bool SignInSequence { get; set; }
    /// <summary>
    /// This will return template code.
    /// </summary>
    public int TemplateCode { get; set; }
    /// <summary>
    /// This will return envelope IsSignerAttachFileReq flag
    /// </summary>
    public bool IsSignerAttachFileReq { get; set; }
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
    /// Get/Set the controls for Draft
    /// </summary>
    public List<DocumentContentPOCO> Controls { get; set; }
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


    public List<Recipients> Recipients { get; set; }

    public List<Documents> Documents { get; set; }

    public int EnvelopeCode { get; set; }

    public Guid? ReminderTypeID { get; set; }

    public Guid? ThenReminderTypeID { get; set; }

    public Guid? UserSignatureTextID { get; set; }

    public Guid? MessageTemplateTextID { get; set; }

    public bool? SendIndividualSignatureNotifications { get; set; }

    /// <summary>
    /// This will return the Final Reminder 
    /// </summary>
    public int? FinalReminderDays { get; set; }
    /// <summary>
    /// This will return the Final reminder type ID 
    /// </summary>
    public Guid? FinalReminderTypeID { get; set; }
    /// <summary>
    /// This will return ReferenceCode
    /// </summary>
    public string? ReferenceCode { get; set; }
    /// <summary>
    /// This will return ReferenceEmail
    /// </summary>
    public string? ReferenceEmail { get; set; }
    public Guid? EnvelopeTemplateGroupId { get; set; }
    ///// <summary>
    ///// This will return IsWaterMark
    ///// </summary>
    //public bool? IsWaterMark { get; set; }
    ///// <summary>
    ///// This will return WatermarkTextForSender
    ///// </summary>
    //public string? WatermarkTextForSender { get; set; }
    ///// <summary>
    ///// This will return WatermarkTextForOther
    ///// </summary>
    //public string? WatermarkTextForOther { get; set; }
    /// <summary>
    /// This will return the Private Mode value
    /// </summary>
    public Nullable<bool> IsPrivateMode { get; set; }

    /// <summary>
    /// This will set StoreOriginalDocument required for envelope or not
    /// </summary>       
    public bool? IsStoreOriginalDocument { get; set; }

    /// <summary>
    /// This will set  StoreSignatureCertificate required for envelope or not
    /// </summary>       
    public bool? IsStoreSignatureCertificate { get; set; }
    /// <summary>
    /// This will set the post signing page url
    /// </summary>
    public string? PostSigningLandingPage { get; set; }

    public Nullable<bool> IsEnableFileReview { get; set; }
    public Nullable<bool> IsRule { get; set; }
    public bool? IsNewUploadOrDeleteDocument { get; set; }

    /// <summary>
    /// This will return the CreatedDateTime
    /// </summary>
    public DateTime CreatedDateTime { get; set; }
    /// <summary>
    /// This will return the IsAdditionalAttmReq
    /// </summary>
    public bool? IsAdditionalAttmReq { get; set; }
    /// <summary>
    /// This will return the list of attachments requested
    /// </summary>
    public List<TemplateGroupDocumentUploadDetails> TemplateGroupDocumentUploadDetailsList { get; set; }

    public bool IsSignerIdentity { get; set; }

    public bool IsSaveControl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Nullable<bool> IsDisclaimerInCertificate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsDisclaimerAccepted { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? DisclaimerText { get; set; }
    public int? IsEnvelopeExpirationRemindertoSender { get; set; }
    public int? EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
    public string? EnvelopeExpirationRemindertoSender { get; set; }
    public int? IsSendReminderTillExpiration { get; set; }
    public string? SendReminderTillExpiration { get; set; }

    public bool? EnableCcOptions { get; set; }
    public bool? IsSameRecipientForAllTemplates { get; set; }
    public bool? EnableRecipientLanguage { get; set; }

  }

  public class ResponseMessageWithTemplateGuid
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
    /// This will return Template Id.
    /// </summary>
    public string? TemplateId { get; set; }
    /// <summary>
    /// This will return Role List.
    /// </summary>
    public List<RoleList> RoleList { get; set; }
    /// <summary>
    /// This will return Template Code.
    /// </summary>
    public int? TemplateCode { get; set; }
    /// <summary>
    /// Get errorneous tags found on document if any
    /// </summary>
    public List<ErrorTagDetailsResponse> ErrorTagDetailsResponse { get; set; }
    /// <summary>
    /// This will return Recipients.
    /// </summary>
    public List<Recipients> Recipients { get; set; }
    /// <summary>
    /// This will return Documents.
    /// </summary>
    public List<Documents> Documents { get; set; }
  }
  public class RoleList
  {
    /// <summary>
    /// This will return Role Name.
    /// </summary>
    public string? RoleName { get; set; }
    /// <summary>
    /// This will return Role Id.
    /// </summary>
    public string? RoleId { get; set; }
  }
  /// <summary>
  /// POCO for Prepare Drafted Details
  /// </summary>
  public class PrepareDraftedDetails
  {
    /// <summary>
    /// Envelope ID
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// Contract Stage
    /// </summary>
    public string? ContractStage { get; set; }
    /// <summary>
    /// Documents to prepare for DraFt
    /// </summary>
    public List<DraftedDocuments> Documents { get; set; }
    /// <summary>
    /// Recipients to prepare for DraFt
    /// </summary>
    public List<DraftedRecipients> DraftRecipients { get; set; }
  }
  public class ResponseMessageForEnvelopeWithUseTemplate
  {
    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public System.Net.HttpStatusCode StatusCode { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return  Envelope Id.
    /// </summary>
    public Guid EnvelopeId { get; set; }
    /// <summary>
    /// This will return TemplateCode.
    /// </summary>
    public int TemplateCode { get; set; }
    /// <summary>
    /// This will return EnvelopeTypeId
    /// </summary>
    public Guid EnvelopeTypeId { get; set; }
    /// <summary>
    /// This will return all the envelope details of newly created.
    /// </summary>
    public EnvelopeDetails EnvelopeDetails { get; set; }
    /// <summary>
    /// This will return  Envelope Id.
    /// </summary>
    public string? EncryptedEnvelopeId { get; set; }
    public string? EncryptedTemplateGroupId { get; set; }
    public string? EncryptedQueryString { get; set; }

    public string? FileReviewTemplateNames { get; set; }

  }
  public class RoleRecipinetMapping
  {
    /// <summary>
    /// This will return Role Id.
    /// </summary>
    public Guid Role { get; set; }
    /// <summary>
    /// This will return Recipient.
    /// </summary>
    public Guid Recipient { get; set; }
  }
  public class TemplateData
  {
    /// <summary>
    /// This will return templateId.
    /// </summary>
    public Guid TemplateID { get; set; }
    /// <summary>
    /// This will return IPAddress.
    /// </summary>
    public string? IPAddress { get; set; }
    /// <summary>
    /// This will return templateId.
    /// </summary>
    public Guid DocID { get; set; }
    /// <summary>
    /// This will get Email ID and Name of Recipient for Role
    /// </summary>
    public List<TemplateRoleBasicInfo> TemplateRoleRecipientMapping { get; set; }
  }
  public class TemplateDataRequest
  {
    /// <summary>
    /// This will return templateId.
    /// </summary>
    public Guid? TemplateID { get; set; }
    /// <summary>
    /// This will return templateId.
    /// </summary>
    public int? TemplateCode { get; set; }
    /// <summary>
    /// This will return IPAddress.
    /// </summary>
    public string? IPAddress { get; set; }
    /// <summary>
    /// This will return templateId.
    /// </summary>
    public Guid DocID { get; set; }
    /// <summary>
    /// This will get Email ID and Name of Recipient for Role
    /// </summary>
    public List<TemplateRoleBasicInfo> TemplateRoleRecipientMapping { get; set; }
    /// <summary>
    /// This will get Prefill control values
    /// </summary>
    public List<PrefillControlInfo> PrefillControls { get; set; }
    /// <summary>
    /// This will get control values to be updated
    /// </summary>
    public List<ControlInfo> UpdateControls { get; set; }
    /// <summary>
    /// This will get Subject 
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will get Body 
    /// </summary>
    public string? Body { get; set; }
    /// <summary>
    /// This will get set signing method.
    /// </summary>
    public int? SigningMethod { get; set; }
    /// <summary>
    /// This will get set Post Signing Url.
    /// </summary>
    public string? PostSigningUrl { get; set; }
    /// <summary>
    /// This will get set Post Signing Url.
    /// </summary>
    public bool IsSingleSigningURL { get; set; }
    /// <summary>
    /// Documents with Base64Data in case of Rule
    /// </summary>
    public List<APIEnvelopeDocumentRequest> Documents { get; set; }
    /// <summary>
    /// Get/Set Trans ID
    /// </summary>
    public long transId { get; set; }
    //// <summary>
    /// Get Set IsPasswordMailToSigner
    /// </summary>
    public bool? IsPasswordMailToSigner { get; set; }
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
    /// <summary>
    /// This will set IsRandomPassword
    /// </summary>
    public bool? IsRandomPassword { get; set; }
    /// <summary>
    /// Get Set MessageTemplateTextID
    /// </summary>
    public Guid? MessageTemplateTextID { get; set; }
    /// <summary>
    /// Get Set UserSignatureTextID
    /// </summary>
    public Guid? UserSignatureTextID { get; set; }
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
    /// Get set CultureInfo
    /// </summary>
    public string? CultureInfo { get; set; }
    /// <summary>
    /// Get set IsSequenceCheck
    /// </summary>
    public bool? IsSequenceCheck { get; set; }
    /// <summary>
    /// Get set Reference Code
    /// </summary>
    public string? ReferenceCode { get; set; }
    /// <summary>
    /// Get set Reference Email
    /// </summary>
    public string? ReferenceEmail { get; set; }
    /// <summary>
    /// Get set SignatureCertificateRequired
    /// </summary>
    public bool? SignatureCertificateRequired { get; set; }
    /// <summary>
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
    /// <summary>
    /// Get set IsAttachXMLDataReq
    /// </summary>
    public bool? IsAttachXMLDataReq { get; set; }
    /// <summary>
    /// Get set IsSeparateMultipleDocumentsAfterSigningRequired
    /// </summary>
    public bool? IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
    /// <summary>
    /// Get set SendIndividualSignatureNotifications
    /// </summary>
    public bool? SendIndividualSignatureNotifications { get; set; }
    /// <summary>
    /// Get set Auth Token
    /// </summary>
    public string? AuthToken { get; set; }
    /// <summary>
    /// Get set User Email
    /// </summary>
    public string? UserEmail { get; set; }
    /// <summary>
    /// This will return the Private Mode Value
    /// </summary>
    public Nullable<bool> IsPrivateMode { get; set; }

    /// <summary>
    /// This will set StoreOriginalDocument required for envelope or not
    /// </summary>       
    public bool? IsStoreOriginalDocument { get; set; }

    /// <summary>
    /// This will set  StoreSignatureCertificate required for envelope or not
    /// </summary>       
    public bool? IsStoreSignatureCertificate { get; set; }
    /// <summary>
    /// Check if validation is required on RoleName or Role ID
    /// </summary>
    public bool UseRoleNameForRecipient { get; set; }

    public bool? IsAdditionalAttmReq { get; set; }
    public List<SendEnvelopeFromGroupAttachmentRequests> AttachmentRequests { get; set; }

    //public bool IsSignerIdentity { get; set; }
    public string? CreatedSource { get; set; }
    public bool? IsDisclaimerInCertificate { get; set; }

    public string? EnvelopeExpirationRemindertoSender { get; set; }
    public string? SendReminderTillExpiration { get; set; }
    public Nullable<int> IsEnvelopeExpirationRemindertoSender { get; set; }
    public Nullable<int> ISSendReminderTillExpiration { get; set; }
    public Nullable<int> EnvelopeExpirationRemindertoSenderReminderDays { get; set; }

    public bool? ISNewSigner { get; set; }

    /// <summary>
    /// Get set EnableCcOptions
    /// </summary>
    public bool? EnableCcOptions { get; set; }
    public string? AppKey { get; set; }
    public bool? EnableRecipientLanguage { get; set; }
  }

  public class CompanySeletedSettings
  {
    public Guid SettingId { get; set; }
    public string? SettingValue { get; set; }

  }
  public class ManageEnvelopeAPI
  {
    /// <summary>
    /// This will return Envelope Code.
    /// </summary>
    public virtual string EnvelopeCode { get; set; }
    /// <summary>
    /// This will return Envelope's Subject.
    /// </summary>
    public virtual bool? IsSequential { get; set; }
    /// <summary>
    /// This will return Is sequential.
    /// </summary>
    public virtual string EnvelopeCodeDisplay { get; set; }
    /// <summary>
    /// This will return Envelope's Subject.
    /// </summary>
    public virtual string Subject { get; set; }
    /// <summary>
    /// This will return Envelope's Current Status.
    /// </summary>
    public virtual string CurrentStatus { get; set; }
    /// <summary>
    /// This will return Envelope's Sent details.
    /// </summary>
    public virtual string Sent { get; set; }
    /// <summary>
    /// This will return Envelope's Completed details.
    /// </summary>
    public string? Completed { get; set; }
    /// <summary>
    /// This will return Envelope's Status .
    /// </summary>
    public virtual string EnvelopeStatusDescription { get; set; }
    /// <summary>
    /// This will return Status ID.
    /// </summary>
    public Guid StatusId { get; set; }
    /// <summary>
    /// This will return envelope ID.
    /// </summary>
    public virtual Guid EnvelopeId { get; set; }
    /// <summary>
    /// This will return Expiry date.
    /// </summary>
    public virtual DateTime ExpiryDate { get; set; }

    /// <summary>
    /// This will return bool for final link
    /// </summary>
    public virtual bool? IsFinalDocLinkReq { get; set; }

    /// <summary>
    /// This will return bool for final link
    /// </summary>
    public virtual bool IsTransparencyDocReq { get; set; }
    /// <summary>
    /// This will return bool for final link
    /// </summary>
    public virtual Guid UserID { get; set; }

    /// <summary>
    /// This will return email of sender.
    /// </summary>
    public virtual string senderEmail { get; set; }
    /// <summary>
    /// This will return Date, Time for the created envelope.
    /// </summary>
    public virtual DateTime CreatedDateTime { get; set; }
    /// <summary>
    /// This will return Modified date.
    /// </summary>
    public virtual DateTime ModifiedDate { get; set; }
    /// <summary>
    /// This will return Recepient Id
    /// </summary>
    public virtual Guid RecipientID { get; set; }
    /// <summary>
    /// This will return Template Key
    /// </summary>
    public virtual Guid? TemplateKey { get; set; }
    /// <summary>
    /// This will return DateFormatID
    /// </summary>
    public Guid? DateFormatID { get; set; }
    /// <summary>
    /// This will return the comma separated recipient list
    /// </summary>
    public string? RecipientList { get; set; }
    /// <summary>
    /// This will return the data masked setting for the sender of the envelope
    /// </summary>
    public bool isDataMasked { get; set; }
    /// <summary>
    /// This will return the total number of records
    /// </summary>
    public int? TotalRecords { get; set; }
    /// <summary>
    /// This will return the company id of the sender
    /// </summary>
    public Guid? SenderCompanyId { get; set; }
    /// <summary>
    /// This will return the data deleted setting for the sender of the envelope
    /// </summary>
    public bool isDataDeleted { get; set; }
    /// <summary>
    /// This will return the privacy setting for the sender of the envelope
    /// </summary>
    public bool? isPrivateMode { get; set; }
    /// <summary>
    /// This will return user Envelope tab grid preferences
    /// </summary>
    public UserEnvelopeGridPreferencesModal userEnvelopeGridPreferencesModal { get; set; }
  }
  public class ResponseMessageWithManageEnvelopesApi
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
    /// This will return Envelope Filter From Date.
    /// </summary>
    public DateTime? FromDate { get; set; }
    /// <summary>
    /// This will return Envelope Filter End Date.
    /// </summary>
    public DateTime? ToDate { get; set; }
    /// <summary>
    /// This will return the list of envelopes.
    /// </summary>
    public List<ManageEnvelopeAPI> Envelopes { get; set; }

    /// <summary>
    /// This will return Total Envelope Count.
    /// </summary>
    public int TotalEnvelopeCount { get; set; }
  }

  public class EnvelopeDBResponse
  {
    /// <summary>
    /// This will return Envelope ID.
    /// </summary>
    public Guid EnvelopeId { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
  }
  public class TemplateDataList
  {
    /// <summary>
    /// This will return templateId.
    /// </summary>
    public List<Guid> TemplateID { get; set; }
    /// <summary>
    /// This will return IPAddress.
    /// </summary>
    public string? IPAddress { get; set; }
    /// <summary>
    /// This will return Envelope Id.
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// Check if request has come from Home Dashboard
    /// </summary>
    public bool IsHomeDashboard { get; set; }

    public Nullable<bool> IsEnableFileReview { get; set; }
    public List<ExistingFilesSource> existingFilesSource { get; set; }

  }

  public class ExistingFilesSource
  {
    public string? FileName { get; set; }

    public string? FileSource { get; set; }
  }
  public class APIMessageTemplate
  {
    /// <summary>
    /// This will return MessageTemplates Id.
    /// </summary>
    public Guid ID { get; set; }
    /// <summary>
    /// This will return MessageTemplates name.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// This will return MessageTemplates description.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// This will return MessageTemplates subject.
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will return MessageTemplates emailBody.
    /// </summary>
    public string? EmailBody { get; set; }
    /// <summary>
    /// This will return MessageTemplates UserSignatureTextID.
    /// </summary>
    public Guid? UserSignatureTextID { get; set; }
    /// <summary>
    /// This will return MessageTemplates is share or not.
    /// </summary>
    public bool IsShared { get; set; }
    /// <summary>
    /// This will return MessageTemplates share id.
    /// </summary>
    public Guid SharedId { get; set; }
  }
  public class TemplateBasicInfo
  {
    /// <summary>
    /// This will return Template ID
    /// </summary>
    public Guid TemplateId { get; set; }
    /// <summary>
    /// This will return template code.
    /// </summary>
    public int TemplateCode { get; set; }
    /// <summary>
    /// This will return Envelop Id.
    /// </summary>
    public string? UserEmail { get; set; }
    /// <summary>
    /// This will return template name if any
    /// </summary>
    public string? TemplateName { get; set; }
    /// <summary>
    /// This will return template description if any
    /// </summary>
    public string? TemplateDescription { get; set; }
    /// <summary>
    /// This will return envelope IsCreateStaticLink for template flag
    /// </summary>
    public bool? IsStatic { get; set; }
    /// <summary>
    /// This will return envelope IsCreateStaticLink for template flag
    /// </summary>
    public DateTime? ExpiryDate { get; set; }
    /// <summary>
    /// This will return Template CreatedDate
    /// </summary>
    public DateTime? CreatedDate { get; set; }
    /// <summary>
    /// This will return envelope LastModifiedDate
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }
    /// <summary>
    /// This will return Template Subject
    /// </summary>   
    public string? Subject { get; set; }
    /// <summary>
    /// This will return Template Body
    /// </summary>   
    public string? Body { get; set; }
    public string? TypeName { get; set; }
    /// <summary>
    /// This will return flag for signature certificate.
    /// </summary>
    public List<DocumentBasicInfo> DocumentList { get; set; }
    /// <summary>
    /// This will return Template Role List
    /// </summary>
    public List<TemplateRoleBasicInfo> TemplateRoleList { get; set; }
  }
  public class TemplateRoleBasicInfo
  {
    public System.Guid RoleID { get; set; }

    public string? RoleName { get; set; }
    public Nullable<int> Order { get; set; }
    public string? RecipientEmail { get; set; }
    public string? RecipientName { get; set; }
    public Guid RecipientTypeID { get; set; }
    public string? RecipientID { get; set; }

    public string? RecipientType { get; set; }

    public string? CcSignerType { get; set; }
    public string? CultureInfo { get; set; }
    public int? DeliveryMode { get; set; }
    public string DialCode { get; set; }
    public string CountryCode { get; set; }
   // public int? ReminderType { get; set; }
    public string Mobile { get; set; }
  }
  public class ResponseMessageGetTemplateBasicDetails
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
    /// This will return  Template Id.
    /// </summary>
    public string? TemplateId { get; set; }
    /// <summary>
    /// This will return template details
    /// </summary>
    public TemplateBasicInfo TemplateBasicInfo { get; set; }
    /// <summary>
    /// This will return List of Tempaltes
    /// </summary>
    public List<TemplateBasicInfo> TemplateList { get; set; }
  }

  public class ResponseMessageForEditUserProfile
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

    public DashBoard DashBoard { get; set; }
  }

  public class ResponseMessageForEnvelopeDetailsList
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
    /// This will return envelope details
    /// </summary>
    public List<EnvelopeDetails> EnvelopeDetailsList { get; set; }
  }

  public class ResponseMessageForResendStaticConfirmationEmail
  {
    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public System.Net.HttpStatusCode StatusCode { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return  Envelope Id.
    /// </summary>
    public Guid EnvelopeId { get; set; }
    /// <summary>
    /// This will return  Envelope Code.
    /// </summary>
    public string? EnvelopeCode { get; set; }

    /// <summary>
    /// This will return envelope culture - info
    /// </summary>
    public string? EnvelopeCultureInfo { get; set; }

    /// <summary>
    /// This wil return recepientname to whom confirmation mail need to be send
    /// </summary>
    public string? RecepientName { get; set; }

  }


  //public class ResponseMessageForSignDocument
  //{
  //    /// <summary>
  //    /// This will return Status Code.
  //    /// </summary>
  //    public HttpStatusCode StatusCode { get; set; }
  //    /// <summary>
  //    ///  This will return Status Message.
  //    /// </summary>
  //    public string? StatusMessage { get; set; }
  //    /// <summary>
  //    /// This will return response message for corresponding  status code.
  //    /// </summary>
  //    public string? Message { get; set; }

  //    public string? LanguageBasedApiMessage { get; set; }

  //    public EnvelopeDetails EnvelopeDetails { get; set; }

  //    public Guid SignerStatusId { get; set; }

  //    //public APISettings APISettings { get; set; }

  //    public DashBoard DashBoard { get; set; }

  //    public string? TempDataResendMessage { get; set; }

  //    public bool IsResendActionRequired { get; set; }

  //    public bool IsEnvelopeExpired { get; set; }

  //    public AdminGeneralAndSystemSettings AdminGeneralAndSystemSettings { get; set; }


  //}


  public class ResponseMessageForInitalizeSignDocument
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
    /// This will retrun Message to to store on Temp["Rsend"]
    /// </summary>
    public string? TempDataResendMessage { get; set; }

    /// <summary>
    /// This will return error action wheather Resend OR Expire or Other Error
    /// </summary>
    public string? ErrorAction { get; set; }

    public EnvelopeInfo EnvelopeInfo { get; set; }
    public string? InfoSenderEmail { get; set; }
    //public EnvelopeDetails EnvelopeDetails { get; set; }
    //////////////Extras
    //public Guid SignerStatusId { get; set; }
    //public string? FolderFileSize { get; set; }
    /// <summary>
    /// This will return default signaute required / not required for static template
    /// </summary>
    public bool? IsDefaultSignatureForStaticTemplate { get; set; }

    public List<DocumentDetails> documentDetails { get; set; }

    public List<EnvelopeAdditionalUploadInfoDetails> EnvelopeAdditionalUploadInfoList { get; set; }

    public int? MaxUploadID { get; set; }

    public UserData userdata { get; set; }

    public bool? EnableClickToSign { get; set; }

    public bool? EnableAutoFillTextControls { get; set; }

    public List<Guid> SameRecipientIds { get; set; }
    public bool IsSameRecipient { get; set; }

    public List<RolsInfo> TemplateRolesInfo { get; set; }

    public bool IsSequenceCheck { get; set; }

    public string? CurrentRoleID { get; set; }
    public string? CurrentEmail { get; set; }

    public bool CanEdit { get; set; }

    public int? InviteSignNowByEmail { get; set; }

    public bool AllowMultipleSigner { get; set; }

    public string? CreatedSource { get; set; }

    public string? SignGlobalTemplateKey { get; set; }

    public bool IsSendConfirmationEmail { get; set; }

    public bool IsPasswordMailToSigner { get; set; }

    public string? DatePlaceHolder { get; set; }
    public string? DateFormat { get; set; }
    public Dictionary<Guid?, string> LanguagelayoutList { get; set; }
    public List<LookupKeyItem> Language { get; set; }
    public List<LookupKeyItem> LanguageValidation { get; set; }
    public bool IsAdditionAttamRequest { get; set; }
    public Guid StaticEnvelopId { get; set; }
    public Guid CurrentRecipientID { get; set; }
    public Guid SignatureTypeID { get; set; }
    public bool IsAnySignatureExists { get; set; }
    public string? ShowDefaultSignatureContol { get; set; }
    public string? DefaultplaceHolder { get; set; }
    public List<CheckListData> CheckListData { get; set; }
    public List<EnvelopeImageControlData> EnvelopeImageControlData { get; set; }
    public int? PageCount { get; set; }
    public string? EncryptedGlobalEnvelopeID { get; set; }
    public string? EncryptedGlobalRecipientID { get; set; }
    public string? EncryptedSender { get; set; }
    public string? Delegated { get; set; }
    public string? Prefill { get; set; }
    public List<DocumentDetails> DocumentNameList { get; set; }
    public List<DocumentDetails> FileReviewInfo { get; set; }
    public int FileReviewCount { get; set; }
    public bool AllowStaticMultiSigner { get; set; }
    public bool IsEnvelopePurging { get; set; }
    public string UNCPath { get; set; }
    public bool? DisableDownloadOptionOnSignersPage { get; set; }
    public Guid? AttachSignedPdfID { get; set; }
    public List<DialingCountryCodes> DialCodeDropdownList { get; set; }
    public string DeliveryMode { get; set; }
    public string DialCode { get; set; }
    public string MobileNumber { get; set; }
    public string CountryCode { get; set; }
    public bool? EnableMessageToMobile { get; set; }
   // public Nullable<bool> IsSMSAccessCode { get; set; }
    public bool? RequiresSignersConfirmationonFinalSubmit { get; set; }
    public bool? IncludeStaticTemplates { get; set; }
    public bool? IsAllowSignerstoDownloadFinalContract { get; set; }
    public int RecipientOrder { get; set; }
    public bool? ReVerifySignerStaticTemplate { get; set; }
    public bool? ReVerifySignerDocumentSubmit { get; set; }
   // public bool? IsReVerifySignerEmailAccessCode { get; set; }
   // public bool? IsReVerifySignerSMSAccessCode { get; set; }
    public DateTime? StaticLinkExpiryDate { get; set; }
    public string EnvelopeId { get; set; }
    public bool? DisableDeclineOption { get; set; }
    public bool? DisableChangeSigner { get; set; }
    public bool? DisableFinishLaterOption { get; set; }
    public bool? SendMessageCodetoAvailableEmailorMobile { get; set; }
    }
  public class RolsInfo
  {
    public Guid? RecipientId { get; set; }

    public int order { get; set; }

    public bool isRequired { get; set; }

    public string? RoleName { get; set; }
    public string? CultureInfo { get; set; }
    public string DeliveryMode { get; set; }
    public string DialCode { get; set; }
    public string CountryCode { get; set; }
   // public int? ReminderType { get; set; }
  }
  public class FirsrtRolsInfo
  {
    public Guid? RecipientId { get; set; }

    public Guid? EnvelopeId { get; set; }

    public string? FirstSignerEmail { get; set; }

    public bool IsStaticPwd { get; set; }

    public string? CultureInfo { get; set; }

    public string? SenderEmail { get; set; }

    public bool IsPasswordMailToSigner { get; set; }
    public string DeliveryMode { get; set; }
    public string DialCode { get; set; }
    public string MobileNumber { get; set; }
    public string CountryCode { get; set; }
   // public bool IsSMSAccessCode { get; set; }
  }
  public class InitializeSignDocumentAPI
  {
    public string? EnvelopeId { get; set; }

    public string? RecipientId { get; set; }

    public string? templateKey { get; set; }

    public string? IPAddress { get; set; }

    public string? currentStatus { get; set; }

    public bool? isTosign { get; set; }

    public string? EmailId { get; set; }

    public bool? isFromSignerLanding { get; set; }
    public bool isFromMultiSignPage { get; set; }
    public string? CopyEmailId { get; set; }

    public bool IsStaticPwd { get; set; }

    public bool IsPasswordMailToSigner { get; set; }
    public string? DeliveryMode { get; set; }
    public string? DialCode { get; set; }
    public string? MobileNumber { get; set; }
    public string? CountryCode { get; set; }
    //public bool? IsSMSAccessCode { get; set; }
  }

  public class ResponseMessageForGetEnvelopeOrTemplateFields
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
    /// This will return error action wheather Resend OR Expire or Other Error
    /// </summary>
    public string? ErrorAction { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    public string? currentRecEmailId { get; set; }
    public string? EDisplayCode { get; set; }
    public int? PasswordKeySize { get; set; }
    public string? PasswordtoOpen { get; set; }
    public string? PasswordtoSign { get; set; }
    public string? PasswordKey { get; set; }
    public string? TemplateKey { get; set; }
    public Guid EnvelopeId { get; set; }
    public Guid TemplateId { get; set; }
    public string? DecryptPassword { get; set; }
    public bool IsEnvelopePurging { get; set; }
    public int? DeliveryMode { get; set; }
    public string Mobile { get; set; }
    public string EnableMessageToMobile { get; set; }
    public string VerificationCode { get; set; }
  }
  /// <summary>
  /// Update the control values after use template
  /// </summary>
  public class ControlValuesForUpdate
  {
    /// <summary>
    /// This willl return the documentContentId
    /// </summary>
    public Guid DocumentContentID { get; set; }
    /// <summary>
    /// This will return the control value
    /// </summary>
    public string? ControlValue { get; set; }
    /// <summary>
    /// This will return the sign byte for Signature and initial control
    /// </summary>
    public string? SignatureControlValue { get; set; }
  }
  /// <summary>
  /// Accept Prefill Control Values
  /// </summary>
  public class PrefillControlInfo
  {
    /// <summary>
    /// This willl return the Template Control
    /// </summary>
    public Guid ControlID { get; set; }
    /// <summary>
    /// This will return the control value
    /// </summary>
    public string? ControlValue { get; set; }
    /// <summary>
    /// This will return the sign byte for Signature and initial control
    /// </summary>
    public string? SignatureControlValue { get; set; }
  }

  public class DashboardDetails
  {
    /// <summary>
    /// This will return senders emails Id
    /// </summary>
    public string? SenderEmail { get; set; }
    /// <summary>
    /// This will return recepients emails Id
    /// </summary>
    public string? RecepientEmail { get; set; }
    /// <summary>
    /// This will return Subject of the envelope 
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will return Current Status of the envelope
    /// </summary>
    public string? EnvelopeStatus { get; set; }
    /// <summary>
    /// This will return envelope Send Date
    /// </summary>
    public DateTime CreatedDateTime { get; set; }
    /// <summary>
    /// This will return envelope code
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return envelope Id
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// This will return envelope expiry date
    /// </summary>
    public DateTime ExpiryDate { get; set; }
    /// <summary>
    /// This will return envelopes encrypted id
    /// </summary>
    public string? EnvelopeHashID { get; set; }
    /// <summary>
    /// This will return Recepient Id
    /// </summary>
    public string? RecipientID { get; set; }
    /// <summary>
    /// This will return Template Key
    /// </summary>
    public Guid? TemplateKey { get; set; }
    /// <summary>
    /// This will return encrypted envelopes code
    /// </summary>
    public string? EnvelopeCodeHashID { get; set; }
    /// <summary>
    /// This will return Total Count
    /// </summary>
    public int TotalCount { get; set; }
    /// <summary>
    /// This will return User Envelope Status
    /// </summary>
    public string? UserEnvelopeStatus { get; set; }
    /// <summary>
    /// This will return Reference Code
    /// </summary>
    public string? ReferenceCode { get; set; }
    /// <summary>
    /// This will return Reference Email
    /// </summary>
    public string? ReferenceEmail { get; set; }
    /// <summary>
    /// This will return User Id
    /// </summary>
    public Guid UserID { get; set; }
    /// <summary>
    /// This will return Sequence Check
    /// </summary>
    public bool? IsSequenceCheck { get; set; }
    /// <summary>
    /// This will return Signer Status Count
    /// </summary>
    public int? SignerStatus { get; set; }
    /// <summary>
    /// This will return user Id mapped with folder
    /// </summary>
    public string? UsersFolder { get; set; }
    /// <summary>
    /// This will return Document Name
    /// </summary>
    public string? Documents { get; set; }
    /// <summary>
    /// This will return Envelope Status Id
    /// </summary>
    public Guid StatusId { get; set; }
    /// <summary>
    /// This will return Document Name
    /// </summary>
    public string? RecNames { get; set; }
    /// <summary>
    /// This will return Created DateTime as per the Timezone setting
    /// </summary>
    public string? CreatedDateTimeTimezoneStr { get; set; }
    /// <summary>
    /// This will return dateformatID
    /// </summary>
    public Guid DateFormatID { get; set; }
    public bool? IsStatic { get; set; }
    public long? TemplateGroupId { get; set; }
    public bool? isDataDeleted { get; set; }
    public string? DraftType { get; set; }
    public string DialCode { get; set; }
    public string Mobile { get; set; }
  }


  public class CustomAPIResponse
  {
    /// <summary>
    /// This will return status of the API response 
    /// </summary>
    public bool Status { get; set; }
    /// <summary>
    /// This will return status code of API response
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }
    /// <summary>
    /// This will return Message of the API reponse
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return Status Message of the API response
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return data object
    /// </summary>
    public object Data { get; set; }
    /// <summary>
    /// This will return total count
    /// </summary>
    public int TotalCount { get; set; }
    /// <summary>
    /// This will return wheather attachment request exist for the envelope in case of the Template Group
    /// </summary>
    public bool? IsAttachmentUploadsExist { get; set; }
  }

  public class CreateFolderRequestObj
  {
    public Guid UserId { get; set; }
    public string? FolderName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? ModifiedDateTime { get; set; }

  }

  public class RestoreReqObj
  {
    public List<Guid> EnvelopeIds { get; set; }
    public bool IsRestore { get; set; }
  }

  public class SignMultiTemplate
  {
    /// <summary>
    ///  This will return envelope Id
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// This will return envelope code
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return Envelope sign url
    /// </summary>
    public string? EnvelopeSignUrl { get; set; }
    /// <summary>
    /// This will return IsSignerAttachFileReq
    /// </summary>
    public bool IsSignerAttachFileReq { get; set; }
    public int IsSignerAttachFileReqNew { get; set; }
    /// <summary>
    /// This will return EnvelopeCodeHashID
    /// </summary>
    public string? EnvelopeHashID { get; set; }
    /// <summary>
    /// This will return signer docs
    /// </summary>
    public List<string> SignerDocs { get; set; }
    /// <summary>
    /// This will return CurrentRecipientId
    /// </summary>
    public Guid CurrentRecipientID { get; set; }
    /// <summary>
    /// This will return CurrentRecipientEmail
    /// </summary>
    public string? CurrentRecipientEmail { get; set; }
    /// <summary>
    /// This will return signed status of recipeint
    /// </summary>
    public bool IsAllSigned { get; set; }
    /// <summary>
    /// this will return finish status of recipients
    /// </summary>
    public bool IsFinished { get; set; }
    /// <summary>
    /// This will return sequence check
    /// </summary>
    public bool IsSequenceCheck { get; set; }
    /// <summary>
    /// This will return sequence check
    /// </summary>
    public int? nextOrder { get; set; }
    /// <summary>
    /// This will return sender email address
    /// </summary>
    public string? SenderEmailAddress { get; set; }
    /// <summary>
    /// this will return list of recipient Id
    /// </summary>
    public List<string> RecipientIdsByEmail { get; set; }
    /// <summary>
    /// this will return Subject
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// this will return Message
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return Sign Multiple template
    /// </summary>
    public List<SignMultipleTemplateDetails> SignMultipleTemplateCollection { get; set; }
    /// <summary>
    /// This will return envelope uploads list
    /// </summary>
    public List<EnvelopeAdditionalUploadInfoDetails> EnvelopeAdditionalUploadInfoList { get; set; }
    public string? CultureInfo { get; set; }
    public bool? EnableMessageToMobile { get; set; }
    public string DialCode { get; set; }
    public string MobileNumber { get; set; }
  }
  public class EnvelopeAdditionalUploadInfoDetails
  {
    public long ID { get; set; }
    public Nullable<System.Guid> MasterEnvelopeID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? FileName { get; set; }
    public string? AdditionalInfo { get; set; }
    public bool IsRequired { get; set; }
    public Nullable<bool> IsActive { get; set; }
    public string? OriginalFileName { get; set; }
    public System.DateTime? UploadedDateTime { get; set; }

    public Nullable<System.Guid> RecipientID { get; set; }
  }

  public class SignMultipleTemplateDetails
  {
    /// <summary>
    /// 
    /// </summary>
    public Guid EnvelopeTemplateGroupsID { get; set; }
    /// <summary>
    /// This will return TemplateName
    /// </summary>
    public string? TemplateName { get; set; }
    /// <summary>
    /// This will return recepients sender FirstName 
    /// </summary>
    public string? PrescriberFirstName { get; set; }
    /// <summary>
    /// This will return recepients sender FirstName 
    /// </summary>
    public string? PrescriberLastName { get; set; }
    /// <summary>
    /// This will return Subject of the envelope 
    /// </summary>
    public string? PrescriberEmail { get; set; }
    /// <summary>
    /// This will return Current Status of the envelope
    /// </summary>
    public string? Status { get; set; }
    /// <summary>
    /// This will return envelope Send Date
    /// </summary>
    public DateTime UpdatedStatusDate { get; set; }
    /// <summary>
    /// This will return envelope code
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// This will return envelope Id
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// This will return Recepient Id
    /// </summary>
    public Guid? RecipientID { get; set; }
    /// <summary>
    /// This will return Template Key
    /// </summary>
    public Guid? TemplateKey { get; set; }
    /// <summary>
    /// This will return RoleName
    /// </summary>
    public string? RoleName { get; set; }
    /// <summary>
    /// This will return Current Recipient Id
    /// </summary>
    public Guid CurrentRecipientId { get; set; }
    /// <summary>
    /// This will return Order
    /// </summary>
    public int? Order { get; set; }
    /// <summary>
    /// This will return StatusID
    /// </summary>
    public Guid StatusID { get; set; }
    /// <summary>
    /// This will return Recipient Email
    /// </summary>
    public string? RecipientEmail { get; set; }
    /// <summary>
    /// This will return Recipient Name
    /// </summary>
    public string? RecipientName { get; set; }
    /// <summary>
    /// Get Signer Status if Signed
    /// </summary>
    public bool IsSigned { get; set; }
    /// <summary>
    /// Get Signer finished status
    /// </summary>
    public bool IsFinished { get; set; }
    /// <summary>
    /// This will return Subject
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// This will return Message
    /// </summary>
    public string? Message { get; set; }
    public string? UpdatedStatusDateTimezoneStr { get; set; }
    /// <summary>
    /// This will return Recepient Id
    /// </summary>
    public Guid DateFormatID { get; set; }
    public string? DialCode { get; set; }
    public string? Mobile { get; set; }
  }

  public class ResponseMessageForSignMultiTemplate
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
    /// This will return Sign Multiple 
    /// </summary>
    public SignMultiTemplate SignMultipleTemplate { get; set; }
    public int? MaxUploadID { get; set; }
    public string DialCode { get; set; }
    public string IsAllowSignerstoDownloadFinalContract { get; set; }
  }

  public class DocumentGroupAPIRequest
  {
    /// <summary>
    /// This will pass GroupName 
    /// </summary>
    public string? GroupName { get; set; }
    /// <summary>
    /// This will pass list of template Ids to add in a group 
    /// </summary>
    public List<Guid> TemplatesIds { get; set; }
  }

  public class ResponseMessageForTemplateGroup
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
    /// This will return  Envelope Id.
    /// </summary>
    public string? TemplateGroupId { get; set; }
    /// <summary>
    /// This will return envelope details
    /// </summary>
    public TemplateGroupDetails TemplateGroups { get; set; }
  }

  public class TemplateGroupDetails
  {
    /// <summary>
    /// 
    /// </summary>
    public long ID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid UserID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid DateFormatID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid ExpiryTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DateTime ExpiryDate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int ReminderDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int ReminderRepeatDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool PasswordReqdtoSign { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordtoSign { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool PasswordReqdtoOpen { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordtoOpen { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedDateTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DateTime ModifiedDateTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int PasswordKeySize { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? GroupName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? GroupDescription { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? CultureInfo { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsFinalCertificateReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsFinalDocLinkReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsTransparencyDocReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsSignerAttachFileReq { get; set; }
    public int IsSignerAttachFileReqNew { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsAttachXML { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? TemplateKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPasswordMailToSigner { get; set; }
    /// <summary>
    /// This will set AccessAuthType
    /// </summary>
    ///
    [XmlIgnore]
    public Guid? AccessAuthType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsRandomPassword { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PostSigningLandingPage { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? ReminderTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? UserSignatureTextID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? ThenReminderTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? MessageTemplateTextID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? SendIndividualSignatureNotifications { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int FinalReminderDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? FinalReminderTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DateTime? LastAccessedDateTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<TemplateGroupContentDetails> TemplateGroupContentDetailsList { get; set; }
    /// <summary>
    /// This will return access auth type for envelope
    /// </summary>
    public Guid? AccessAuthenticationType { get; set; }
    /// <summary>
    /// This will set access authentication password
    /// </summary>
    public string? AccessAuthenticationPassword { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int TotalTemplateInGroups { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool TemplateInGroup { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? TemplateGroupContentJson { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsSequenceCheck { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsEdit { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<TemplateGroupDocumentUploadDetails> TemplateGroupDocumentUploadDetailsList { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? TemplateGroupUploadsJson { get; set; }

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



    public bool IsSignerIdentity { get; set; }
    public int? IsEnvelopeExpirationRemindertoSender { get; set; }
    public int? EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
    public string? EnvelopeExpirationRemindertoSender { get; set; }
    public int? IsSendReminderTillExpiration { get; set; }
    public string? SendReminderTillExpiration { get; set; }

    public bool? EnableCcOptions { get; set; }
    public bool? EnableRecipientLanguage { get; set; }
  }

  public class TemplateGroupContentDetails
  {
    /// <summary>
    /// 
    /// </summary>
    public long? ID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? TypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? TypeName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Nullable<int> Order { get; set; }

    public Guid? TemplateID { get; set; }
    public int? TemplateCode { get; set; }

    public long TemplateGroupID { get; set; }
    public string? DocumentNameString { get; set; }
    public List<TemplateRoleBasicInfo> TemplateRoles { get; set; }
    public List<DocumentBasicInfo> DocumentList { get; set; }
  }
  public class TemplateGroupDataList
  {
    /// <summary>
    /// This will return templateId.
    /// </summary>
    //public List<Guid> TemplateID { get; set; }
    /// <summary>
    /// This will return IPAddress.
    /// </summary>
    public string? IPAddress { get; set; }
    /// <summary>
    /// This will return Envelope Id.
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// Check if request has come from Home Dashboard
    /// </summary>
    public bool IsHomeDashboard { get; set; }
    public Int64 TemplateGroupID { get; set; }
    public List<int> ProcessedTemplateCodes { get; set; }
    public List<string> ProcessedTemplateIDs { get; set; }
    public bool IsNewGroupCreation { get; set; }
    public Guid? SubEnvelopeId { get; set; }
    public bool? IsSameRecipients { get; set; }
  }
  public class SendEnvelopeFromGroupRequest
  {
    /// <summary>
    /// This will return IPAddress.
    /// </summary>
    public string? IPAddress { get; set; }
    /// <summary>
    /// This will return Envelope Id.
    /// </summary>
    public Guid EnvelopeID { get; set; }
    public Int64? DocumentGroupID { get; set; }
    public bool UseSameRecipientforall { get; set; }
    public List<SendEnvelopeFromGroupTemplateRequest> TemplateList { get; set; }
    public List<SendEnvelopeFromGroupRecipient> Recipients { get; set; }
    public List<SendEnvelopeFromGroupAttachmentRequests> AttachmentRequests { get; set; }

    public int SigningMethod { get; set; }


    public string? GroupName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid DateFormatID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid ExpiryTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DateTime ExpiryDate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? ReminderDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? ReminderRepeatDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? PasswordReqdtoSign { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordtoSign { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? PasswordReqdtoOpen { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordtoOpen { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Body { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int PasswordKeySize { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? CultureInfo { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsFinalCertificateReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsFinalDocLinkReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsTransparencyDocReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsSignerAttachFileReq { get; set; }
    public int IsSignerAttachFileReqNew { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsAttachXML { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? TemplateKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsPasswordMailToSigner { get; set; }
    /// <summary>
    /// This will set AccessAuthType
    /// </summary>
    ///
    [XmlIgnore]
    public Guid? AccessAuthType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsRandomPassword { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PostSigningLandingPage { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? ReminderTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? UserSignatureTextID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? ThenReminderTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? MessageTemplateTextID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? SendIndividualSignatureNotifications { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? FinalReminderDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? FinalReminderTypeID { get; set; }
    /// <summary>
    /// This will return access auth type for envelope
    /// </summary>
    public Guid? AccessAuthenticationType { get; set; }
    /// <summary>
    /// This will set access authentication password
    /// </summary>
    public string? AccessAuthenticationPassword { get; set; }
    /// <summary>
    /// IsSequence Check
    /// </summary>
    public bool? IsSequenceCheck { get; set; }
    /// <summary>
    /// Reference Code
    /// </summary>
    public string? ReferenceCode { get; set; }
    /// <summary>
    /// Reference Email
    /// </summary>
    public string? ReferenceEmail { get; set; }
    /// <summary>
    /// This will return the Private Mode Value
    /// </summary>
    public Nullable<bool> IsPrivateMode { get; set; }

    /// <summary>
    /// This will set StoreOriginalDocument required for envelope or not
    /// </summary>       
    public bool? IsStoreOriginalDocument { get; set; }

    /// <summary>
    /// This will set  StoreSignatureCertificate required for envelope or not
    /// </summary>       
    public bool? IsStoreSignatureCertificate { get; set; }

    public Nullable<bool> IsEnableFileReview { get; set; }
    public string? CreatedSource { get; set; }
    public string? EnvelopeExpirationRemindertoSender { get; set; }
    public string? SendReminderTillExpiration { get; set; }
    public Nullable<int> IsEnvelopeExpirationRemindertoSender { get; set; }
    public Nullable<int> ISSendReminderTillExpiration { get; set; }
    public Nullable<int> EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
    public string? AppKey { get; set; }
  }
  public class SendEnvelopeFromGroupTemplateRequest
  {
    public Guid? TemplateId { get; set; }
    public int? TemplateCode { get; set; }
    public int? Order { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public List<SendEnvelopeFromGroupRecipient> Recipients { get; set; }
    public List<SendEnvelopeFromGroupDocumentRequest> Documents { get; set; }
    public List<ControlInfo> UpdateControls { get; set; }
  }
  public class SendEnvelopeFromGroupRecipient
  {
    /// <summary>
    /// This will return Email.
    /// </summary>
    public string? Email { get; set; }
    /// <summary>
    /// This will return Email.
    /// </summary>
    public string? RecipientName { get; set; }
    /// <summary>
    /// This will return Envelope Id.
    /// </summary>
    public Guid TemplateRoleId { get; set; }
    public int? DeliveryMode { get; set; }
    public string DialCode { get; set; }
    public string CountryCode { get; set; }
    public string Mobile { get; set; }
   // public int? ReminderType { get; set; }
  }
  public class SendEnvelopeFromGroupAttachmentRequests
  {
    /// <summary>
    /// This will return ID.
    /// </summary>
    public long? ID { get; set; }
    /// <summary>
    /// This will return Name.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// This will return Description.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// This will return AdditionalInfo.
    /// </summary>
    public string? AdditionalInfo { get; set; }
    /// <summary>
    /// This will return IsRequired flag
    /// </summary>
    public bool IsRequired { get; set; }

    public string? RecipientEmailID { get; set; }
    public string RecipientMobile { get; set; }
    public string DialCode { get; set; }
    public string CountryCode { get; set; }
  }
  public class SendEnvelopeFromGroupDocumentRequest
  {
    /// <summary>
    /// This will TemplateId.
    /// </summary>
    public Guid? TemplateId { get; set; }
    /// <summary>
    /// This will Order.
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// This will return TemplateCode.
    /// </summary>
    public int? TemplateCode { get; set; }
    public long DocumentKey { get; set; }
  }
  public class ResponseMessageSendEnvelopeFromGroup
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
    /// This will return  Envelope Id.
    /// </summary>
    public string? MasterEnvelopeCode { get; set; }
    public List<SendEnvelopeFromGroupEnvelopeDetails> EnvelopeList { get; set; }
    public List<SendEnvelopeFromGroupAttachmentRequestDetails> AttachmentIDList { get; set; }
    //public List<ErrorTagDetailsResponse> ErrorTagDetailsResponse { get; set; }
    /// <summary>
    /// This will return Recipient List 
    /// </summary>
    public List<APIRecipientList> RecipientList { get; set; }

  }
  public class SendEnvelopeFromGroupEnvelopeDetails
  {
    public string? EnvelopeCode { get; set; }
    public string? Name { get; set; }
  }
  public class SendEnvelopeFromGroupAttachmentRequestDetails
  {
    public long AttachmentID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? AdditionalInfo { get; set; }
  }
  public class ResponseMessageEnvelopeGroupDetails
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
    /// This will return  Envelope Id.
    /// </summary>
    public string? EnvelopeId { get; set; }
    /// <summary>
    /// This will return envelope details
    /// </summary>
    public EnvelopeDetails EnvelopeDetails { get; set; }
    public TemplateGroupDetails TemplateGroupDetails { get; set; }
    public string? EncryptedEnvelopeId { get; set; }
    public string? EncryptedTemplateGroupId { get; set; }
    public string? EncryptedQueryString { get; set; }
    public string? SubEnvelopeID { get; set; }
    public string? EncryptedSubEnvelopeId { get; set; }
    public List<Guid> ProcessedEnvelopeTemplateGroupId { get; set; }
    public TemplateBasicInfo TemplateBasicInfo { get; set; }
    public Guid ProcessedTemplateId { get; set; }
    public bool? IsLastTemplateProcessed { get; set; }

  }
  public class APIEnvelopeRequest
  {
    /// <summary>
    /// This will return IPAddress.
    /// </summary>
    public string? IPAddress { get; set; }
    /// <summary>
    /// If Envelope needs to be prepared ( Document conversion is required or not.)
    /// </summary>
    public bool IsPrepare { get; set; }
    public int? TemplateCode { get; set; }
    public List<APIEnvelopeRecipientRequest> Recipients { get; set; }
    public List<APIEnvelopeDocumentRequest> Documents { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid DateFormatID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid ExpiryTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DateTime ExpiryDate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? ReminderDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? ReminderRepeatDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? PasswordRequiredToSign { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordToSign { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? PasswordRequiredToOpen { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordToOpen { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Subject { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Body { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int PasswordKeySize { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PasswordKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? CultureInfo { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? SignatureCertificateRequired { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsFinalDocLinkReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsTransparencyDocReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsSignerAttachFileReq { get; set; }
    public int IsSignerAttachFileReqNew { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsAttachXMLDataReq { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsSeparateMultipleDocumentsAfterSigningRequired { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? TemplateKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsPasswordMailToSigner { get; set; }
    /// <summary>
    /// This will set AccessAuthType
    /// </summary>
    ///
    [XmlIgnore]
    public Guid? AccessAuthType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? IsRandomPassword { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PostSigningLandingPage { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? ReminderTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? UserSignatureTextID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? ThenReminderTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? MessageTemplateTextID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? SendIndividualSignatureNotifications { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? FinalReminderDays { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? FinalReminderTypeID { get; set; }
    /// <summary>
    /// This will return access auth type for envelope
    /// </summary>
    public Guid? AccessAuthenticationType { get; set; }
    /// <summary>
    /// This will set access authentication password
    /// </summary>
    public string? AccessAuthenticationPassword { get; set; }
    /// <summary>
    /// IsSequence Check
    /// </summary>
    public bool? IsSequenceCheck { get; set; }
    public bool? IsSequence { get; set; }
    /// <summary>
    /// Reference Code
    /// </summary>
    public string? ReferenceCode { get; set; }
    /// <summary>
    /// Reference Email
    /// </summary>
    public string? ReferenceEmail { get; set; }
    /// <summary>
    /// This will get set signing method.
    /// </summary>
    public int? SigningMethod { get; set; }
    /// <summary>
    /// This will get set Post Signing Url.
    /// </summary>
    public string? PostSigningUrl { get; set; }
    /// <summary>
    /// This will get set Post Signing Url.
    /// </summary>
    public bool IsSingleSigningURL { get; set; }
    /// <summary>
    /// This will return the Private Mode Value
    /// </summary>
    public Nullable<bool> IsPrivateMode { get; set; }

    /// <summary>
    /// This will set StoreOriginalDocument required for envelope or not
    /// </summary>       
    public bool? IsStoreOriginalDocument { get; set; }

    /// <summary>
    /// This will set  StoreSignatureCertificate required for envelope or not
    /// </summary>       
    public bool? IsStoreSignatureCertificate { get; set; }

    public Nullable<bool> IsEnableFileReview { get; set; }
    /// <summary>
    /// This will return the rule editable value
    /// </summary>
    public Nullable<bool> IsRuleEditable { get; set; }

    public bool? IsAdditionalAttmReq { get; set; }

    public List<SendEnvelopeFromGroupAttachmentRequests> AttachmentRequests { get; set; }

    //public bool IsSignerIdentity { get; set; }
    public bool? IsDisclaimerInCertificate { get; set; }
    public string? CreatedSource { get; set; }

    public string? EnvelopeExpirationRemindertoSender { get; set; }
    public string? SendReminderTillExpiration { get; set; }
    public Nullable<int> IsEnvelopeExpirationRemindertoSender { get; set; }
    public Nullable<int> ISSendReminderTillExpiration { get; set; }
    public Nullable<int> EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
    public Nullable<bool> SendConfirmationEmail { get; set; }

    public bool? EnableCcOptions { get; set; }
    public string? AppKey { get; set; }
    public bool? EnableRecipientLanguage { get; set; }
    public string? AppliedEpicUser { get; set; }
    public string? AppliedEpicEntityId { get; set; }
    public string? EntityType { get; set; }
    public string? IntegrationType { get; set; }
    public string? PostSendingNavigationPage { get; set; }

  }
  public class APIEnvelopeRecipientRequest
  {
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int? Order { get; set; }
    public Guid? ID { get; set; }
    public string? RecipientType { get; set; }
    public Guid? RoleID { get; set; }
    public List<APIControlRequest> Controls { get; set; }

    public string? CcSignerType { get; set; }
    public string? CultureInfo { get; set; }
    public int? DeliveryMode { get; set; }
    public string DialCode { get; set; }
    public string Mobile { get; set; }
    public string CountryCode { get; set; }
   // public int? ReminderType { get; set; }
  }
  public class APIEnvelopeDocumentRequest
  {
    public string? Name { get; set; }
    public string? FileExtension { get; set; }
    public string? DocumentBase64Data { get; set; }
    public string? SourceLocation { get; set; }
    public string? SourceDocumentId { get; set; }
    public string? DocumentSource { get; set; }
    public bool IsDocumentUploadedToSource { get; set; }
    public bool IsNewDocumentUpload { get; set; }
    public bool IsUploadBefore { get; set; }
    public int? Order { get; set; }
  }
  public class APIControlRequest
  {
    public string? Name { get; set; }
    public string? Tag { get; set; }
    /// <summary>
    /// This will return the control value
    /// </summary>
    public string? ControlValue { get; set; }
    /// <summary>
    /// This will return the sign byte for Signature and initial control
    /// </summary>
    public string? SignatureControlValue { get; set; }
    /// <summary>
    /// Get Set Control Read only property
    /// </summary>
    public bool IsReadOnly { get; set; }
    /// <summary>
    /// Get Set Control Required property
    /// </summary>
    public bool IsRequired { get; set; }
    public string? ControlHtmlId { get; set; }

    public Guid DynamicControlId { get; set; }
    public string? LabelText { get; set; }
    public int? TabIndex { get; set; }
  }
  public class APIEnvelopeResponse
  {
    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public System.Net.HttpStatusCode StatusCode { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return EnvelopeCode
    /// </summary>
    public string? EnvelopeCode { get; set; }
    public string? KeyId { get; set; }
    public string? Email { get; set; }
    public string? Token { get; set; }
    public bool IsPrepare { get; set; }
    /// <summary>
    /// This will return  Sign Document Url.
    /// </summary>
    public string? IntegrationUrl { get; set; }
    public string? PostSendingNavigationPage { get; set; }
  }
  public class TemplateGroupDocumentUploadDetails
  {
    /// <summary>
    /// This will return ID
    /// </summary>
    public long ID { get; set; }
    /// <summary>
    /// This will return TemplateGroupId
    /// </summary>
    public long TemplateGroupId { get; set; }
    /// <summary>
    /// This will return Name
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// This will return Description
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// This will return Description
    /// </summary>
    public string? AdditionalInfo { get; set; }
    /// <summary>
    /// This will return FileName
    /// </summary>
    public string? FileName { get; set; }
    /// <summary>
    /// This will return IsRequired flag
    /// </summary>
    public bool IsRequired { get; set; }
    /// <summary>
    /// This will return IsActive
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// This will return RecipientId
    /// </summary>
    public Guid? RecipientId { get; set; }
    /// <summary>
    /// This will return RecipientEmail
    /// </summary>
    public string? RecipientEmail { get; set; }
  }
  public class TemplateGroupListResponse
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
    public TemplateGroupDetails TemplateGroupInfo { get; set; }
    /// <summary>
    /// This will return envelope details
    /// </summary>
    public List<TemplateGroupDetails> TemplateGroups { get; set; }
  }

  public class FinishSubEnvelopeRequest
  {
    public Guid EnvelopeId { get; set; }
    public Guid SubEnvelopeId { get; set; }
    public Guid RecipientId { get; set; }
    public string? RecipientEmail { get; set; }
    public string RecipientMobile { get; set; }
  }
  public class FinishSubEnvelopeResponse
  {
    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public System.Net.HttpStatusCode StatusCode { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    /// This will return error action wheather Resend OR Expire or Other Error
    /// </summary>
    public string? ErrorAction { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// This will return  Envelope Id.
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// This will return Recipient ID.
    /// </summary>
    public string? ReturnURL { get; set; }


  }

  public class APIEnvelopeUploads
  {
    /// <summary>
    /// 
    /// </summary>
    public string? MasterEnvelopeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? TemplateGroupUploadsJson { get; set; }
    /// <summary>
    /// This will return the group or not
    /// </summary>
    public bool? IsEnvelope { get; set; }
  }

  public class TemplateSearch
  {

    public string? filterValue { get; set; }
    public string? searchValue { get; set; }
    public int? page { get; set; }
    public int? pagesize { get; set; }
    public string? UserId { get; set; }
    public Guid id { get; set; }

  }

  public class WatermarkStamp
  {
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
  public class RetentionDaysInfo
  {
    public Guid EnvelopeId { get; set; }
    public string? EnvelopeCode { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public Guid StatusId { get; set; }
    public int RetentionDays { get; set; }
    public string? OptionValue { get; set; }
    public string? SettingText { get; set; }
    public string? EnvelopeTable { get; set; }
  }


  //Added by Tparker-S3-1506 Improvement Envelope Settings View Settings - current settings
  public class ResponseMessageGetEnvelopeHistoryDetails : ResponseMessageGetEnvelopeDetails
  {

    /// <summary>
    /// This will return envelope settings details
    /// </summary>
    public EnvelopeSettingsDetail EnvelopeSettingsDetail { get; set; }
    /// <summary>
    /// This will return the current settings of user
    /// </summary>
    public AdminGeneralAndSystemSettings CurrentSettingsDetail { get; set; }
    /// <summary>
    /// This will return the  settings value 
    /// </summary>
    public SettingAdditionalDetails SettingAdditionalDetails { get; set; }
    /// <summary>
    /// This will return the  settings value 
    /// </summary>
    public SettingAdditionalDetails EnvelopeAdditionalDetails { get; set; }
  }

  public class SettingAdditionalDetails
  {
    /// <summary>
    /// This will return expiresIn Value 
    /// </summary>
    public string? ExpiresIn { get; set; }
    /// <summary>
    /// This will return SendReminderInstr Value
    /// </summary>
    public string? SendReminderInstr { get; set; }
    /// <summary>
    /// This will return ThenReminderInstr Value
    /// </summary>
    public string? ThenReminderInstr { get; set; }
    /// <summary>
    /// This will return FinalReminderDays Value
    /// </summary>
    public string? FinalReminderDays { get; set; }
    /// <summary>
    /// This will return AccessAuthentication Value
    /// </summary>
    public string? AccessAuthentication { get; set; }
    /// <summary>
    /// This will return ElectronicSignatureOption Value
    /// </summary>

    public string? ElectronicSignatureOption { get; set; }
    /// <summary>
    /// This will return HeaderAndFooterDescription Value
    /// </summary>

    public string? HeaderAndFooterDescription { get; set; }
    /// <summary>
    /// This will return ReferenceCodeSetting Value
    /// </summary>

    public string? ReferenceCodeSetting { get; set; }
    /// <summary>
    /// This will return DocumentPaperSize Value
    /// </summary>

    public string? DocumentPaperSize { get; set; }
    /// <summary>
    /// This will return SignReqReplyAddSettings Value
    /// </summary>

    public string? SignReqReplyAddSettings { get; set; }
    /// <summary>
    /// This will return SendInvitationEmailToSigner Value
    /// </summary>

    public string? SendInvitationEmailToSigner { get; set; }
    /// <summary>
    /// This will return AttachSignedPdf Value
    /// </summary>

    public string? AttachSignedPdf { get; set; }
    /// <summary>
    /// This will return DateFormat Value
    /// </summary>

    public string? DateFormat { get; set; }
    /// <summary>
    /// This will return IsPrivateMode Value
    /// </summary>

    public bool? IsPrivateMode { get; set; }
    /// <summary>
    /// This will return DataMasking Value
    /// </summary>

    public bool? IsDataMasking { get; set; }
    /// <summary>
    /// This will return Data Deleted Value
    /// </summary>

    public string? IsDataDeleted { get; set; }
    /// <summary>
    /// This will return Additional Attachement Value
    /// </summary>

    public bool? IsAdditionalAttachement { get; set; }

    public string? SignerAttachmentOptions { get; set; }

    public int? IncludeSignerAttachFile { get; set; }

    /// <summary>
    /// This will return Data Deleted Value
    /// </summary>

    public bool? IsDataDeletedValue { get; set; }

    public bool? EnableAutoFillTextControls { get; set; }

    public int? IsEnvelopeExpirationRemindertoSender { get; set; }
    public int? EnvelopeExpirationRemindertoSenderReminderDays { get; set; }
    public string? EnvelopeExpirationRemindertoSender { get; set; }
    public int? IsSendReminderTillExpiration { get; set; }
    public string? SendReminderTillExpiration { get; set; }

    public bool SendConfirmationEmail { get; set; }
    public string? DigitalCertificateOption { get; set; }
    public string? AppKeyOption { get; set; }
    public bool? EnableCcOptions { get; set; }
    public bool? EnableRecipientLanguage { get; set; }
  }

  public class EnvelopeViewSettingDetails
  {
    /// <summary>
    /// This will return envelope settings
    /// </summary>
    public AdminGeneralAndSystemSettings envelopeSettingsDetail { get; set; }
    /// <summary>
    /// This will return user settings
    /// </summary>
    public AdminGeneralAndSystemSettings currentSettingsDetail { get; set; }
    /// <summary>
    /// This will return settingAdditionalDetails settings
    /// </summary>
    public SettingAdditionalDetails settingAdditionalDetails { get; set; }
    /// <summary>
    /// This will return envelopeAdditionalDetails settings
    /// </summary>
    public SettingAdditionalDetails envelopeAdditionalDetails { get; set; }
    /// <summary>
    /// This will return the IsPrepareDocument value
    /// </summary>
    public string? source { get; set; }

  }
  // Added by Tparker-Enhancement:Manage Envelope - View Settings Miscellaneous Fixes
  public class ResponseMessageForDeleteEnvelope : ResponseMessageForEnvelope
  {
    /// <summary>
    /// this wil return EnvelopeDocumentDeleteData data
    /// </summary>
    public string? documentDeletedBy { get; set; }
    /// <summary>
    /// this wil return date in user settings format 
    /// </summary>
    public string? documentDeletedDate { get; set; }
  }
  public class EnvelopeAdditionalUpload
  {
    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public System.Net.HttpStatusCode StatusCode { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }
    public List<EnvelopeAdditionalUploadInfoDetails> EnvelopeAdditionalUploadInfoList { get; set; }

    public int? MaxUploadID { get; set; }


  }

  public class EnvelopeAdditionalUploadInfoDetailsDelegate
  {
    public long ID { get; set; }
    public Nullable<System.Guid> MasterEnvelopeID { get; set; }

    public Nullable<System.Guid> RecipientID { get; set; }
    public string? RecipientEmailID { get; set; }
  }

  public class UserData
  {
    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserInitials { get; set; }

    public string? UserSignatureImgSrc { get; set; }

    public string? UserInitialsImgSrc { get; set; }

    public System.Guid SignatureTypeID { get; set; }
  }
  public class EditViewTemplate
  {
    public System.Guid ID { get; set; }
    public Nullable<int> TemplateCode { get; set; }
    public Nullable<System.Guid> TypeID { get; set; }
    public bool IsTemplateEditable { get; set; }
    public Nullable<bool> IsRuleEditable { get; set; }
    public Nullable<int> SharedTemplateCode { get; set; }
    public string? SharedTemplateName { get; set; }
    public string? SharedBy { get; set; }
    public System.Guid DateFormatID { get; set; }
    public System.DateTime CreationDate { get; set; }
    public string? Roles { get; set; }
    public string? Documents { get; set; }
    public string? FileReviewDocuments { get; set; }
    public string? CreationDateLocal { get; set; }
    public string? SharedCreatedDate { get; set; }
    public string? ModifiedDateTime { get; set; }
  }
  public class ResponseMessageForEditViewTemplate
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
    /// This will return Template Id.
    /// </summary>
    public EditViewTemplate EditViewTemplate { get; set; }

  }
  public class DefaultControlPropertySettings
  {
    /// <summary>
    /// This will return the Control Name
    /// </summary>
    public string? Control { get; set; }
    /// <summary>
    /// this will return the default property for the control
    /// </summary>
    public DefaultControlPropertyValues Values { get; set; }
  }
  public class DefaultControlPropertyValues
  {
    public bool Required { get; set; }
    public bool Bold { get; set; }
    public bool Underline { get; set; }
    public bool Italics { get; set; }
    public byte FontSize { get; set; }
    public string? FontType { get; set; }
    public string? Color { get; set; }
    public string? DateFormat { get; set; }
    public string? ValidationType { get; set; }
    public int Length { get; set; }
    public bool CheckboxGroup { get; set; }
    public bool CheckRequired { get; set; }
    public bool PreSelected { get; set; }
    public string? Size { get; set; }
    public bool IsDefaultRequired { get; set; }
    public string? AdlnName { get; set; }
    public string? AdlnOption { get; set; }
  }

  public class EnvelopeTemplateMappingDetails
  {
    public Guid EnvelopeId { get; set; }
    public List<Guid> TemplateId { get; set; }
    public Guid UserId { get; set; }
    public Guid? EnvelopeTypeId { get; set; }
    public List<EnvelopeMapping> envelopeMappingsToTemp { get; set; }
  }

  public class EnvelopeMapping
  {
    public Guid TemplateId { get; set; }

    public int? TemplateCode { get; set; }

    public string? TemplateName { get; set; }
  }

  public class ResponseMessageForRAppEnvelopeDetails
  {

    /// <summary>
    /// This will return Status Code.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }
    /// <summary>
    /// This will return Status Message.
    /// </summary>
    public string? StatusMessage { get; set; }
    /// <summary>
    /// This will return response message for corresponding  status code.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// This will return list of drafts
    /// </summary>
    public List<UserDrafts> UserDrafts { get; set; }
    /// <summary>
    /// This will return envelope details
    /// </summary>
    public EnvelopeDetails EnvelopeDetails { get; set; }
  }

  public class ResponseMessageForCopyEnvelope
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
    /// This will return  Envelope Id.
    /// </summary>
    public Guid EnvelopeId { get; set; }
    public string? EnvelopeCode { get; set; }
    public string? EnvelopeURL { get; set; }
  }

  public class APICopyEnvelope
  {
    public Guid envelopeID { get; set; }
    public string? EnvelopeCode { get; set; }
  }
  public class UserEnvelopeGridPreferencesColumnsModal
  {
    public string? availableColumns { get; set; }
    public string? selectedColumns { get; set; }
  }
  public class ResponseMessageForEnvelopeGridPreferences
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
    /// This will return status.
    /// </summary>
    public bool Status { get; set; }
  }

  public class UserEnvelopeGridPreferencesModal
  {
    public List<UserEnvelopeGridPreferences> AvailableColumns { get; set; }
    public List<UserEnvelopeGridPreferences> SelectedColumns { get; set; }
  }

  public class ResponseMessageForUserEnvelopeGridPreferences
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
    /// This will return status.
    /// </summary>
    public bool Status { get; set; }

    public UserEnvelopeGridPreferencesModal UserEnvelopeGridPreferencesModal { get; set; }
  }
  public class UserEnvelopeGridPreferences
  {
    public string? ColumnCode { get; set; }
    public bool IsSelected { get; set; }
    public int Order { get; set; }
  }

  public class InitializeMultiSignDocumentAPI
  {
    public string? EnvelopeId { get; set; }

    public string? RecipientId { get; set; }

    public string? EmailId { get; set; }
    public string? CultureInfo { get; set; }
    public string DeliveryMode { get; set; }
    public string DialCode { get; set; }
    public string MobileNumber { get; set; }
    public string CountryCode { get; set; }
  }

  public class APIEnvelopeData
  {
    public string? Email { get; set; }
  }

  public class RAppFilterEnvelopeListforApi
  {
    public Guid? CompanyID { get; set; }
    public string? UserRole { get; set; }
  }

  public class ApiCompletedResponse
  {
    public List<ApiCompletedEnvelopeCount> ApiCompletedEnvelopeCount { get; set; }
    public List<ApiCompletedEnvelopeResponse> ApiCompletedEnvelopeResponse { get; set; }
  }

  public class ApiCompletedEnvelopeCount
  {
    public int RSignDownloadDocumentsCount { get; set; }
  }

  public class ApiCompletedEnvelopeResponse
  {
    /// <summary>
    /// EnvelopeId
    /// </summary>
    public Guid EnvelopeID { get; set; }
    /// <summary>
    /// Display Code.
    /// </summary>
    public string? EnvelopeCode { get; set; }
    /// <summary>
    /// IsFinalContract
    /// </summary>
    public bool IsFinalContract { get; set; }
    /// <summary>
    /// IsTransparency
    /// </summary>
    public bool IsTransparency { get; set; }
    /// <summary>
    /// IsSignerAttachment
    /// </summary>
    public bool IsSignerAttachment { get; set; }
  }

  public class APIResponseDownloadFinalSignedContracts
  {
    public HttpStatusCode StatusCode { get; set; }
    public string? StatusMessage { get; set; }
    public string? Message { get; set; }
    public List<APIResponseDownloadFinalEnvelope> APIResponseDownloadFinalEnvelope { get; set; }
  }
  public class APIResponseDownloadFinalEnvelope
  {
    public string? EnvelopeId { get; set; }
    public string? EnvelopeCode { get; set; }
    public string? Base64FileData { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
  }

  public class ResponseMessageForInitalizeSignerSignDocument
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
    /// This will retrun Message to to store on Temp["Rsend"]
    /// </summary>
    public string? TempDataResendMessage { get; set; }

    /// <summary>
    /// This will return error action wheather Resend OR Expire or Other Error
    /// </summary>
    public string? ErrorAction { get; set; }

    public EnvelopeInfo EnvelopeInfo { get; set; }
    public string? InfoSenderEmail { get; set; }
    //public EnvelopeDetails EnvelopeDetails { get; set; }
    //////////////Extras
    //public Guid SignerStatusId { get; set; }
    //public string? FolderFileSize { get; set; }
    /// <summary>
    /// This will return default signaute required / not required for static template
    /// </summary>
    public bool? IsDefaultSignatureForStaticTemplate { get; set; }

    public List<DocumentDetails> documentDetails { get; set; }

    public List<EnvelopeAdditionalUploadInfoDetails> EnvelopeAdditionalUploadInfoList { get; set; }

    public int? MaxUploadID { get; set; }

    public UserData userdata { get; set; }

    public bool? EnableClickToSign { get; set; }

    public bool? EnableAutoFillTextControls { get; set; }

    public List<Guid> SameRecipientIds { get; set; }
    public bool IsSameRecipient { get; set; }

    public List<RolsInfo> TemplateRolesInfo { get; set; }

    public bool IsSequenceCheck { get; set; }

    public string? CurrentRoleID { get; set; }
    public string? CurrentEmail { get; set; }

    public bool CanEdit { get; set; }

    public int? InviteSignNowByEmail { get; set; }

    public bool AllowMultipleSigner { get; set; }

    public string? CreatedSource { get; set; }

    public string? SignGlobalTemplateKey { get; set; }

    public bool IsSendConfirmationEmail { get; set; }

    public bool IsPasswordMailToSigner { get; set; }

    public Dictionary<Guid?, string> LanguagelayoutList { get; set; }
    public List<LookupKeyItem> Language { get; set; }

    public List<LookupKeyItem> LanguageValidation { get; set; }

    public string? EncryptedGlobalEnvelopeID { get; set; }
    public string? EncryptedGlobalRecipientID { get; set; }
    public string? EncryptedSender { get; set; }
    public string? Delegated { get; set; }
    public string? Prefill { get; set; }
    public string? DatePlaceHolder { get; set; }
    public string? DateFormat { get; set; }
    public List<DocumentDetails> DocumentNameList { get; set; }
    public List<DocumentDetails> FileReviewInfo { get; set; }
    public int FileReviewCount { get; set; }
    public bool AllowStaticMultiSigner { get; set; }
    public Guid SignatureTypeID { get; set; }
    public bool IsAnySignatureExists { get; set; }
    public string? ShowDefaultSignatureContol { get; set; }
    public string? DefaultplaceHolder { get; set; }
    public List<CheckListData> CheckListData { get; set; }
    public List<EnvelopeImageControlData> EnvelopeImageControlData { get; set; }
    public int? PageCount { get; set; }
    public bool IsEnvelopePurging { get; set; }
    public string? UNCPath { get; set; }
    public LanguageKeyTranslationsModel LanguageTranslationsModel { get; set; }
    public ResponseSigningUrlModel ResponseSigningUrlModel { get; set; }
    public bool? DisableDownloadOptionOnSignersPage { get; set; }
    public int IsBotClick { get; set; }
    public Guid? AttachSignedPdfID { get; set; }
    public List<DialingCountryCodes> DialCodeDropdownList { get; set; }
    public string DeliveryMode { get; set; }
    public string DialCode { get; set; }
    public string MobileNumber { get; set; }
    public string CountryCode { get; set; }
    public bool? EnableMessageToMobile { get; set; }
   // public bool? IsSMSAccessCode { get; set; }
    public bool? RequiresSignersConfirmationonFinalSubmit { get; set; }
    public bool? IncludeStaticTemplates { get; set; }
    public bool? IsAllowSignerstoDownloadFinalContract { get; set; }
    public int RecipientOrder { get; set; }
    public bool? ReVerifySignerStaticTemplate { get; set; }
    public bool? ReVerifySignerDocumentSubmit { get; set; }
   // public bool? IsReVerifySignerEmailAccessCode { get; set; }
   // public bool? IsReVerifySignerSMSAccessCode { get; set; }
    public bool? DisableFinishLaterOption { get; set; }
    public bool? DisableDeclineOption { get; set; }
    public bool? DisableChangeSigner { get; set; }
  }

  public class RequestSigningUrlModel
  {
    public string? SigningUrl { get; set; }
    public bool IsFromSignerPreLanding { get; set; }
    public string? IsFromBotClick { get; set; }
  }
  public class ResponseSigningUrlModel
  {
    public string? EnvelopeID { get; set; }
    public string? RecipientID { get; set; }
    public string? TemplateKey { get; set; }
    public string? EmailId { get; set; }
    public string? IsDirect { get; set; }
    public string? SignerType { get; set; }
    public string? CopyEmailId { get; set; }
    public bool IsFromSignerLanding { get; set; }
    public bool IsFromInbox { get; set; }
    public string? IPAddress { get; set; }
    public bool IsFromSignerPreLanding { get; set; }
    public bool IsFromMultiSignPage { get; set; }
  }
  public class RequestDecryptSecurityCodeModel
  {
    public string? SecurityCodeUrl { get; set; }
  }
  public class ResponseMessageForSecurityCodeModel
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
    public string? CopyMailId { get; set; }
  }

  public class ResponseMessageForResponseSigningUrlModel
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
    public ResponseSigningUrlModel ResponseSigningUrlModel { get; set; }
    public LanguageKeyTranslationsModel LanguageTranslationsModel { get; set; }
  }

  public class CheckListData
  {
    public int? PageNumber { get; set; }
    public List<ControlsData> ControlsData { get; set; }
    public Guid DocumentId { get; set; }
    public string? DocumentName { get; set; }
  }

  public class EnvelopeImageControlData
  {
    public int Id { get; set; }
    public string? ImagePath { get; set; }
    public string? PageNum { get; set; }
    public int DocPageNo { get; set; }
    public string? ImgControlWidth { get; set; }
    public SigningEnvelopeDocumentData SigningEnvelopeDocumentData { get; set; }
    public List<ControlsData> ControlsData { get; set; }

  }

  public class SigningEnvelopeDocumentData
  {
    public int PageNum { get; set; }
    public Guid DocId { get; set; }
    public string? DocName { get; set; }
    public bool IsPageLoaded { get; set; }
  }

  public class ConvertTextToSignImageModel
  {
    public string? text { get; set; }
    public string? font { get; set; }
    public string? fontsize { get; set; }
    public string? fontColor { get; set; }
    public string? height { get; set; }
    public string? width { get; set; }
    public string? emailID { get; set; }
    public string? envelopeCode { get; set; }
    public int electronicSignIndicationId { get; set; }
    public string? dateFormat { get; set; }
    public string? userTimezone { get; set; }
    public string? dateFormatID { get; set; }
  }

  public class ResponseConvertTextToSignImage
  {
    public string? imgsrc { get; set; }
    public int height { get; set; }
    public int width { get; set; }
  }

  public class ResponseMessageForConvertTextToSignImage
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
    public ResponseConvertTextToSignImage ResponseConvertTextToSignImage { get; set; }
  }

  public class ResponseMessageForEncryptQuery
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

    public string? EnvryptedEncodedText { get; set; }
  }

  public class ConvertHandDrawnSignImageModel
  {
    public string? imageBytes { get; set; }
    public string? envelopeCode { get; set; }
    public int electronicSignIndicationId { get; set; }
    public string? dateFormat { get; set; }
    public string? userTimezone { get; set; }
    public string? dateFormatID { get; set; }
  }

  public class ResponseMessageForDownloadDisclaimerPDF
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
    public byte[] byteData { get; set; }

    public string? FileName { get; set; }
    public string? FileType { get; set; }
  }

  public class ResponseMessageForTranslationsModel
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
    public LanguageKeyTranslationsModel LanguageTranslationsModel { get; set; }
  }

  public class TranslationsModel
  {
    public string? EnvelopeId { get; set; }
    public string? RecipientId { get; set; }
    public string? CultureInfo { get; set; }
  }
  public class LanguageKeyTranslationsModel
  {
    public Dictionary<Guid, string> Language { get; set; }
    public Dictionary<string, string> LanguageValidation { get; set; }
  }

  public class ResponseAllowFirstSignerModel
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
    public string? strURL { get; set; }
    public string? strURLWithData { get; set; }
    public string? CopyMail { get; set; }

    public EnvelopePasswordModal EPasswordModal { get; set; }
  }

  public class AuthenticateSignerResponseModel
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
    public string? RecipientId { get; set; }
    public string? Email { get; set; }
    public bool IsEnvelopePurging { get; set; }
    public int? DeliveryMode { get; set; }
    public string Mobile { get; set; }
    public string EnableMessageToMobile { get; set; }
  }

  public class AuthenticateSignerRequestModel
  {
    public string? AuthenticateUrl { get; set; }
    public bool IsResend { get; set; }
  }

  public class ValidateSignerRequestModel
  {
    public string? RecipientId { get; set; }
    public string? VerificationCode { get; set; }
  }

  public class ConfirmationSignerResponseModel
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
    public string? EnvelopeId { get; set; }
    public string? RecipientId { get; set; }
    public bool IsToSign { get; set; }
  }

  [NotMapped]
  public class ArichiveEnvelopesInfo
  {
    public Guid EnvelopeId { get; set; }
    public string? EdisplayCode { get; set; }
    public int? IsFolderArchived { get; set; }
    public Guid UserID { get; set; }
    public string ArchivedEnvelopeMessage { get; set; }
    public bool IsEnvelopePurging { get; set; }
    public RSign.Models.Envelope envelope { get; set; }
  }

  public class MasterEnvelopeStatsAPI
  {
    public string? EnvelopeId { get; set; }
    public string? RecipientId { get; set; }
    public string? EmailId { get; set; }
  }
  public class ArichiveDBCultureInfo
  {
    public string EnvelopeCultureInfo { get; set; }
    public string RecipientsCultureInfo { get; set; }
    public Guid RecipientId { get; set; }
    public string EmailAddress { get; set; }
  }
  public class SignersUIModel
  {
    public string EmailID { get; set; }
  }
  public class ResponseMessageForShortSigningUrl
  {
    public HttpStatusCode StatusCode { get; set; }
    public string StatusMessage { get; set; }
    public string Message { get; set; }
    public string ShortSigningUrl { get; set; }
  }
  public class SettingsDetailsAdditionalValues
  {
    public Guid KeyConfig { get; set; }
    /// <summary>
    ///  This will return Status Message.
    /// </summary>
    public int OptionFlag { get; set; }
    public string OptionValue { get; set; }
    public string KeyText { get; set; }
  }
}
