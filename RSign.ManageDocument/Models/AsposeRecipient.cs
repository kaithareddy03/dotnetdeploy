using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Models
{
    public class AsposeRecipient
    {
        public Guid ID { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public Guid RecipientTypeID { get; set; }
        public string RecipientType { get; set; }
        public Guid StatusID { get; set; }
        public string Status { get; set; }
        public string SignerIPAddress { get; set; }
        public DateTime StatusDate { get; set; }
        public Guid? DelegatedRecipientID { get; set; }
        public string DelegatedTo { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public byte[] SignatureBytes { get; set; }
        public byte[] InitialBytes { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsSameRecipient { get; set; }
        public List<AsposeRecipientDetails> RecipientDetails { get; set; }
        public int? CcSignerType { get; set; }
        public string MobileNumber { get; set; }
        public int? DeliveryMode { get; set; }
        public string DialCode { get; set; }
    }
    public class MetaDataAndHistory
    {
        public string EnvelopeData { get; set; }
        public string EnvelopeID { get; set; }
        public string Subject { get; set; }
        public string Documents { get; set; }
        public string DocumentHash { get; set; }
        public string Sent { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string Recipients { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public string Sender { get; set; }
        public string Prefill { get; set; }
        public string Signer { get; set; }
        public string Email { get; set; }
        public string IPAddress { get; set; }
        public string Date { get; set; }
        public string Event { get; set; }
        public string DocumentEvents { get; set; }
        public string Created { get; set; }
        public string SignerSignatures { get; set; }
        public string SignerName { get; set; }
        public string Signature { get; set; }
        public string Type { get; set; }
        public string CarbonCopyEvents { get; set; }
        public string DateFormatEU { get; set; }
        public string DateFormatStr { get; set; }
        public string TermsOfService { get; set; }
        public string Accepted { get; set; }
        public string Envelope { get; set; }
        public string UpdateAndResend { get; set; }
        public string Initial { get; set; }

        public string FilesReviewed { get; set; }
        public string AccessAuthentication { get; set; }
        public string EmailAccessCode { get; set; }
        public string Checked { get; set; }
        public string UnChecked { get; set; }
        public string None { get; set; }
        public string EmailVerification { get; set; }
        public string Mobile { get; set; }
        public string DeliveryMode { get; set; }
        public string EmailOrMobile { get; set; }

    }

    public class AsposeDropdownOptions
    {
        public string FieldName { get; set; }
        public string OptionName { get; set; }
        public string OptionValue { get; set; }
    }
    public class AsposeRecipientDetails
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string IPAddress { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string MobileNumber { get; set; }
        public int? DeliveryMode { get; set; }
        public string DialCode { get; set; }
    }
}
