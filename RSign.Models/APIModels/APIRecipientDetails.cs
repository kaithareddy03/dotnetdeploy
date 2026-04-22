using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSign.Models.APIModels
{
    class APIRecipientDetails
    {
    }
    
    public class APIRecipientEntity
    {
        public Guid ID { get; set; }
        public Guid EnvelopeID { get; set; }
        public string? EmailAddress { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? Order { get; set; }
        public Guid EnvelopeStatusID { get; set; }
        public string Mobile { get; set; }
        public int? DeliveryMode { get; set; }
    }
    [NotMapped]
    public class APIRecipientEntityModel
    {
        public Guid ID { get; set; }
        public Guid EnvelopeID { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? Order { get; set; }
        public Guid EnvelopeStatusID { get; set; }
    }
    public class ResendEnvelopePoco
    {
        /// <summary>
        /// 
        /// </summary>
        public string? EnvelopeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsResendToAll { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool SendToCC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? EmailID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? IPAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<RecipientDetails> resendDetails { get; set; }
        public string DialCode { get; set; }
        public string MobileNumber { get; set; }
        public string CountryCode { get; set; }
    }

    [NotMapped]
    public class RecipientDetails
    {
        /// <summary>
        /// This will return Recipient Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        ///  This will return Envelope Id.
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        ///  This will return Recipient StatusId.
        /// </summary>
        public Guid StatusID { get; set; }
        /// <summary>
        /// This will return Recipient name.
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// This will return old Recipient name.
        /// </summary>
        public string? OldRecipient { get; set; }
        /// <summary>
        /// This will return old Recipient Email.
        /// </summary>
        public string? OldEmail { get; set; }
        /// <summary>
        /// This will return recipients type id.
        /// </summary>
        public Guid RecipientTypeID { get; set; }
        /// <summary>
        /// This will return Recipient Email Address.
        /// </summary>
        public string? EmailID { get; set; }
        /// <summary>
        /// This will return recipient's order.
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// This will return Recipient's created date and time.
        /// </summary>
        public DateTime? CreatedDateTime { get; set; }
        /// <summary>
        /// This will return Recipient's type.
        /// </summary>
        public string? RecipientType { get; set; }
        /// <summary>
        /// This will return IpAddress.
        /// </summary>
        public string? IpAddress { get; set; }
        /// <summary>
        /// This will return Template Code.
        /// </summary>
        public Guid? TemplateID { get; set; }
        /// <summary>
        /// This will return Recipient Code.
        /// </summary>
        public string? RecipientCode { get; set; }
        /// <summary>
        /// This will return Template Group id.
        /// </summary>
        public long? TemplateGroupId { get; set; }
        /// <summary>
        /// This will retrun EnvelopeTemplateGroupId
        /// </summary>
        public Guid? EnvelopeTemplateGroupId { get; set; }
        /// <summary>
        /// This will retrun Finish Status
        /// </summary>
        public bool? IsFinished { get; set; }

        public Guid? TemplateRoleId { get; set; }
        public string? CopyEmailID { get; set; }

        public bool? IsSameRecipient { get; set; }

        public string? VerificationCode { get; set; }

        public int? CCSignerType { get; set; }
        public string? CultureInfo { get; set; }
       // public int? ReminderType { get; set; }
        public bool? IsReadonlyContact { get; set; }
        public bool? SendDocumentOnDelegate { get; set; }
        public int? DeliveryMode { get; set; }
        public string? DialCode { get; set; }
        public string? CountryCode { get; set; }
        public string? Mobile { get; set; }
        public string? OldMobile { get; set; }
        public string? OldDialCode { get; set; }
        public string? OldCountryCode { get; set; }

    }
    public class RoleDetails
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
        /// This will return role type id.
        /// </summary>
        public Guid RoleTypeID { get; set; }
        /// <summary>
        /// This will return role order.
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// This will return role created date and time.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// This will return role type.
        /// </summary>
        public string? RoleType { get; set; }

        public int? CCSignerType { get; set; }
        public string? CultureInfo { get; set; }
        public int? DeliveryMode { get; set; }
        public string CountryCode { get; set; }
        public string DialCode { get; set; }
      //  public int? ReminderType { get; set; }
    }

    [NotMapped]
    public class RecipientsDetailsAPI
    {
        /// <summary>
        /// This will return Trans Id
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// this will return ip address of recipients
        /// </summary>
        public string? IPAddress { get; set; }
        /// <summary>
        /// This will return recipients ID
        /// </summary>
        public Guid RecipientID { get; set; }
        /// <summary>
        /// This will return status description
        /// </summary>
        public string? SignerStatusDescription { get; set; }
        /// <summary>
        /// this will return status date and time
        /// </summary>
        public string? StatusDateTime { get; set; }
        /// <summary>
        /// This will return Recipient StatusId.
        /// </summary>
        public Guid RecipientTypeID { get; set; }
        /// <summary>
        /// Get/Set status of execution
        /// </summary>
        public Guid StatusTypeID { get; set; }
        /// <summary>
        /// This will return old Recipient name.
        /// </summary>
        public string? OldRecipient { get; set; }
        /// <summary>
        /// This will return old Recipient Email.
        /// </summary>
        public string? OldEmail { get; set; }
        /// <summary>
        /// This will return Current Recipient name.
        /// </summary>
        public string? CurrentRecipient { get; set; }
        /// <summary>
        /// This will return Current Recipient Email.
        /// </summary>
        public string? CurrentEmail { get; set; }
        /// <summary>
        /// This will return Current Recipient Email.
        /// </summary>
        public string? UpdatedBy { get; set; }
        /// <summary>
        /// This will return Created Date Time.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// This will return Recipient Action.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// This will return Recipient Action.
        /// </summary>
        public string? DelegateTo { get; set; }
        /// <summary>
        /// This will return Recipient Action.
        /// </summary>
        public string? DelegateFrom { get; set; }
        /// <summary>
        /// This will return Recipient Name.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// This will return Recipient Email.
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// This will return Delegate To Email.
        /// </summary>
        public string? DelegateToEmail { get; set; }
        /// <summary>
        /// This will return Delegate From Email.
        /// </summary>
        public string? DelegateFromEmail { get; set; }
        /// <summary>
        /// This will return whether recipients is active
        /// </summary>
        public bool ActiveRecipients { get; set; }
        /// <summary>
        /// Get/Set if RSign Token passed in cookie
        /// </summary>
        public string? RSignAnonymousToken { get; set; }
        /// <summary>
        /// This will return decline reason id
        /// </summary>
        public int? DeclineReasonID { get; set; }
        public string? FormattedCreatedDateTime { get; set; }
        public string? FormattedSentDate { get; set; }
        public string? EmailDeliveryStatusDecription { get; set; }       
        public string? CopyEmailAddress { get; set; }
        /// <summary>
        /// Gets DNS Action for particular envelope
        /// </summary>
        public string? DsnAction { get; set; }
        public string? DsnDescription { get; set; }

        /// <summary>
        /// Gets DNS status for particular envelope
        /// </summary>
        public string? DsnStatus { get; set; }
        public string DeliveryModeText { get; set; }
        public int? DeliveryMode { get; set; }
    }

    public class DeliveryStatusAPI
    {
        public string? EnvelopeCode { get; set; }
        public string? Recipient { get; set; }
        public string? DsnActionDescription { get; set; }
        public string? DsnAction { get; set; }
        public string? DsnStatus { get; set; }
        public int? DeliveryMode { get; set; }
        public string CountryCode { get; set; }
        public string DialCode { get; set; }
        public string Mobile { get; set; }
        public string VerificationCode { get; set; }
        public string DialCodeNo { get; set; }
        public string MobileNumber { get; set; }
        public string OldCountryCode { get; set; }
        public string OldDialCode { get; set; }
        public string OldMobile { get; set; }
    }

    public class RecipientResponse
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
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// This will return Recipient ID.
        /// </summary>
        public Guid RecipientID { get; set; }
        /// <summary>
        /// This will return Recipient Name
        /// </summary>
        public string? RecipientName { get; set; }

        public int? SignerCount { get; set; }
        public int? DeliveryMode { get; set; }
        public string CountryCode { get; set; }
        public string DialCode { get; set; }
        public string Mobile { get; set; }
       // public int? ReminderType { get; set; }
    }
    public class ResponseMessageEnvelopeRecipients
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
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// This will return all recipients count in an envelope
        /// </summary>
        public int RecipientCount { get; set; }
        /// <summary>
        /// This will return list of recipients from envelope
        /// </summary>
        public List<RecipientDetails> RecipientDetails { get; set; }
    }
    public class ResponseMessageTemplateRoles
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
        /// This will return  Template Id.
        /// </summary>
        public Guid TemplateID { get; set; }
        /// <summary>
        /// This will return roles count from template
        /// </summary>
        public int RecipientCount { get; set; }
        /// <summary>
        /// This will return list of roles from template
        /// </summary>
        public List<RoleDetails> RoleDetails { get; set; }
    }
    public class DraftedRecipients
    {
        /// <summary>
        /// EnvelopeID
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// RecipientID
        /// </summary>
        public Guid RecipientID { get; set; }
        /// <summary>
        /// Recipient Type
        /// </summary>
        public Guid RecipientTypeID { get; set; }
        /// <summary>
        /// Recipients Name
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// Recipients Email Address
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        /// <summary>
        /// Recipients order
        /// </summary>
        public int? Order { get; set; }
    }
    public class RecipientsBasicDetails
    {
        /// <summary>
        /// RecipientID
        /// </summary>
        public Guid RecipientID { get; set; }
        /// <summary>
        /// Recipient Type
        /// </summary>
        public Guid RecipientTypeID { get; set; }
        /// <summary>
        /// Recipient Type
        /// </summary>
        public string? RecipientType { get; set; }
        /// <summary>
        /// Recipients Name
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// Recipients Email Address
        /// </summary>        
        public string? Email { get; set; }
        /// <summary>
        /// Recipients order
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// Get Status Id of Recipient
        /// </summary>
        public Guid StatusId { get; set; }
        /// <summary>
        /// Get Signing Status of Recipient
        /// </summary>
        public string? SigningStatus { get; set; }
        /// <summary>
        /// This will return Last Modified Date.
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }
    }
    /// <summary>
    /// Entity for recipient for Resend Envelope functinality
    /// </summary>
    public class RecipientsForResend
    {
        /// <summary>
        /// Get/Set ID of recipient
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Get/Set type id of recipient
        /// </summary>
        public Guid TypeID { get; set; }
        /// <summary>
        /// Get/Set name of recipient
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Get/Set email address of recipient
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// Get/Set order of recipient for envelope signing
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Entry point time of recipient to the recipient list
        /// </summary>
        public DateTime? CreatedDateTime { get; set; }
        /// <summary>
        /// Get/Set whether recipient is elligible for edit operation
        /// </summary>
        public bool IsElligibleForEdit { get; set; }
        /// <summary>
        /// Get/Set whether recipient is elligible for resend operation
        /// </summary>
        public bool IsElligibleForResend { get; set; }
        /// <summary>
        /// Get/Set whether recipient is elligible for resend operation
        /// </summary>
        public Guid? TemplateID { get; set; }
        public string? RecipientCode { get; set; }
        public string? CultureInfo { get; set; }
        public int? DeliveryMode { get; set; }
        public string CountryCode { get; set; }
        public string DialCode { get; set; }
        public string Mobile { get; set; }
       // public int? ReminderType { get; set; }
    }
    /// <summary>
    /// Resposne Entity for RecipientListForResend
    /// </summary>
    public class ResponseMessageForRecipientListForResend
    {
        public ResponseMessageForRecipientListForResend()
        {
            Recipients = new List<RecipientsForResend>();
        }
        /// <summary>
        /// Get/Set status code for response
        /// </summary>
        public System.Net.HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  Get/Set status message for response
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// Get/Set message for response
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Get/Set envelope id of requested recipients
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// Get/Set current signing order of envelope
        /// </summary>
        public int CurrentSigningOrder { get; set; }
        /// <summary>
        /// Get/Set recipients for resend functionality
        /// </summary>
        public List<RecipientsForResend> Recipients { get; set; }
    }

    public class ResponseMessageForRecipientList
    {
        /// <summary>
        ///  This will return Status Code.
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
        /// Get the recipient list
        /// </summary>
        public List<APIRecipientList> RecipientList { get; set; }
    }

    public class APIRecipientList
    {
        /// <summary>
        /// This will return  Recipient Name.
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// This will return  Recipient Email.
        /// </summary>
        public string? RecipientEmail { get; set; }
        /// <summary>
        /// This will return  Recipient Email.
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// This will return  Recipient Email.
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// This will return  Recipient Signing Url.
        /// </summary>
        public string? SigningUrl { get; set; }
        public int? DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
    }

    public class ResendMultiRecipients
    {
        /// <summary>
        /// This will return the RecipientDetails
        /// </summary>
        public RecipientDetails recipientDetails { get; set; }
        /// <summary>
        /// This will return the recipients
        /// </summary>
        public Recipients recipients { get; set; }
        /// <summary>
        /// This will return the multiSubject
        /// </summary>
        public string? multiSubject { get; set; }
    }

    public class RecipeintsMultipleList
    {
        public System.Guid ID { get; set; }
        public System.Guid EnvelopeID { get; set; }
        public System.Guid RecipientTypeID { get; set; }
        public string? RecipientName { get; set; }
        public string? EmailID { get; set; }
        public Nullable<int> Order { get; set; }
        public bool IsSameRecipient { get; set; }

        public int? CCSignerType { get; set; }
        public string? CultureInfo { get; set; }
       // public int? ReminderType { get; set; }
        public bool? IsReadonlyContact { get; set; }
        public int? DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
        public Guid? EnvelopeTemplateGroupID { get; set; }
    }

    public class UpdateRecipeints
    {
        public Guid EnvelopeID { get; set; }
        public Guid RecipientID { get; set; }
        public int Order { get; set; } 
    }
    public class ResponseRecipientsVerificationCode
    {
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
        public Guid RecipientID { get; set; }
        /// <summary>
        /// This will return all recipients count in an envelope
        /// </summary>
        public string? VerificationCode { get; set; }
    }

    [NotMapped]
    public class RecipientsDetailAPI
    {

        /// <summary>
        /// this will return ip address of recipients
        /// </summary>
        public string? IPAddress { get; set; }
        /// <summary>
        /// This will return recipients ID
        /// </summary>
        public Guid RecipientID { get; set; }

        /// <summary>
        /// This will return Recipient StatusId.
        /// </summary>
        public Guid RecipientTypeID { get; set; }
        /// <summary>
        /// Get/Set status of execution
        /// </summary>
        public Guid StatusTypeID { get; set; }
        /// <summary>
        /// This will return old Recipient name.
        /// </summary>
        public string? OldRecipient { get; set; }
        /// <summary>
        /// This will return old Recipient Email.
        /// </summary>
        public string? OldEmail { get; set; }
        /// <summary>
        /// This will return Current Recipient name.
        /// </summary>
        public string? CurrentRecipient { get; set; }
        /// <summary>
        /// This will return Current Recipient Email.
        /// </summary>
        public string? CurrentEmail { get; set; }
        /// <summary>
        /// This will return Current Recipient Email.
        /// </summary>
        public string? UpdatedBy { get; set; }
        /// <summary>
        /// This will return Created Date Time.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// This will return Recipient Action.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// This will return Recipient Action.
        /// </summary>
        public string? DelegateTo { get; set; }
        /// <summary>
        /// This will return Recipient Action.
        /// </summary>
        public string? DelegateFrom { get; set; }
        /// <summary>
        /// This will return Recipient Name.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// This will return Recipient Email.
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// This will return Delegate To Email.
        /// </summary>
        public string? DelegateToEmail { get; set; }
        /// <summary>
        /// This will return Delegate From Email.
        /// </summary>
        public string? DelegateFromEmail { get; set; }
        /// <summary>
        /// This will return whether recipients is active
        /// </summary>
        public bool ActiveRecipients { get; set; }

        public string? TYPE { get; set; }
        public System.Int64 RowNum { get; set; }
        public string RSignAnonymousToken { get; set; }
        /// <summary>
        /// This will return decline reason id
        /// </summary>
        public int? DeclineReasonID { get; set; }
        public string FormattedCreatedDateTime { get; set; }
        public string FormattedSentDate { get; set; }
        public string EmailDeliveryStatusDecription { get; set; }
        public string? CopyEmailAddress { get; set; }
        /// <summary>
        /// Gets DNS Action for particular envelope
        /// </summary>
        public string DsnAction { get; set; }
        public string DsnDescription { get; set; }

        /// <summary>
        /// Gets DNS status for particular envelope
        /// </summary>
        public string DsnStatus { get; set; }
        public int? DeliveryMode { get; set; }
        public string CountryCode { get; set; }
        public string DialCode { get; set; }
        public string Mobile { get; set; }
        public string VerificationCode { get; set; }
        public string DialCodeNo { get; set; }
        public string MobileNumber { get; set; }
        public string OldCountryCode { get; set; }
        public string OldDialCode { get; set; }
        public string OldMobile { get; set; }
    }
}
