using RSign.Models.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSign.Models
{
    public class UserMetaDetailsWithToken
    {
        public string? EmailID { get; set; }
        public Guid UserID { get; set; }
        public string? Token { get; set; }
        public string? LoginReference { get; set; }
        public Guid? CompanyID { get; set; }
        public Guid? UserTypeID { get; set; }
    }
    public partial class UserProfile
    {
        [NotMapped]
        public int TotalEnvelopeCount { get; set; }
        [NotMapped]
        public int CompletedValue { get; set; }
        [NotMapped]
        public int SentforSignatureValue { get; set; }
        [NotMapped]
        public int ExpiringSoonValue { get; set; }
        [NotMapped]
        public int Terminated { get; set; }
        [NotMapped]
        public double ExpiredValue { get; set; }
        [NotMapped]
        public double SignedValue { get; set; }
        [NotMapped]
        public double ViewedValue { get; set; }
        [NotMapped]
        public double AverageTimeValue { get; set; }
        [NotMapped]
        public string? PhotoString { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "SignatureString")]
        [NotMapped]
        public string? SignatureString { get; set; }
        public string? FullName { get { return FirstName + " " + LastName; } }
        //public List<UserRoleType> UserRole { get { return ModelHelper.GetUserRole(UserTypeID); } }
        public List<UserRoleType> UserRole { get; set; }
        [NotMapped]
        public string? CompanyLogo { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        [NotMapped]
        public Guid NewCompanyId { get; set; }
        public UserInfo UserInfo { get; set; }
        public RestResponseUserInfo RestResponseUserInfo { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "DefaultSignatureName")]
        [NotMapped]
        public string? DefaultSignatureName { get; set; }
    }
    public class UserRoleType
    {
        public Guid Key { get; set; }
        public string? Value { get; set; }
        public bool IsActive { get; set; }
    }
    public class CompanyUserInfo
    {
        /// <summary>
        /// This will set the Company ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// This will set the User Id.
        /// </summary>
        public Guid UserId { get; set; }
    }

    public class DashBoard
    {
        /// <summary>
        /// This will return the average time of completion for an envelope.
        /// </summary>
        public double AvereageTimeofCompletion { get; set; }
        /// <summary>
        /// This will return users Company Name.
        /// </summary>
        public string? Company { get; set; }
        /// <summary>
        /// This will return users CompanyID.
        /// </summary>
        public Guid? CompanyID { get; set; }
        /// <summary>
        /// This will return the count of completed envelopes.
        /// </summary>
        public int Completed { get; set; }
        /// <summary>
        /// This will return the count of signed documents.
        /// </summary>
        public int SignedDocuments { get; set; }
        /// <summary>
        /// This will return user Email ID.
        /// </summary>
        public string? EmailID { get; set; }
        /// <summary>
        /// This will return the percent of envelopes expired.
        /// </summary>
        public double Expired { get; set; }
        /// <summary>
        /// This will return the count of envelopes expiring soon. 
        /// </summary>
        public int Expiring { get; set; }
        /// <summary>
        /// This will return users First Name.
        /// </summary>
        public string? FirstName { get; set; }
        /// <summary>
        /// This will return Font Class.
        /// </summary>
        public string? FontClass { get; set; }
        /// <summary>
        /// This will return ID.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// This will return user initials.
        /// </summary>
        public string? Initials { get; set; }
        /// <summary>
        /// This will return users Last Name.
        /// </summary>
        public string? LastName { get; set; }
        /// <summary>
        /// This will return the count of documents sent for signature.
        /// </summary>
        public int SentForSignature { get; set; }
        /// <summary>
        /// This will return the count of Terminated envelopes.
        /// </summary>
        public int Terminated { get; set; }
        /// <summary>
        /// This will return Photo.
        /// </summary>
        public byte[] Photo { get; set; }
        /// <summary>
        /// This will return Photo String.
        /// </summary>
        public string? PhotoString { get; set; }
        /// <summary>
        /// This will return Signature Image.
        /// </summary>
        public byte[] SignatureImage { get; set; }
        /// <summary>
        /// This will return Signature String.
        /// </summary>
        public string? SignatureString { get; set; }
        /// <summary>
        /// This will return the users signature text.
        /// </summary>
        public string? SignatureText { get; set; }
        /// <summary>
        /// This will return Signature TypeID.
        /// </summary>
        public Guid SignatureTypeID { get; set; }
        /// <summary>
        /// This will return the percent of documents signed.
        /// </summary>
        public double Signed { get; set; }
        /// <summary>
        /// This will return the count of pending documents..
        /// </summary>
        public double Pending { get; set; }
        /// <summary>
        /// This will return percent of viewed documents.
        /// </summary>
        public double Viewed { get; set; }
        /// <summary>
        /// This will return users Job Title.
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// This will return the total count of documents sent.
        /// </summary>
        public int SentDocuments { get; set; }
        /// <summary>
        /// This will return User ID.
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        ///  This will return Profile Pic Location.
        /// </summary>
        public string? ProfilePicLocation { get; set; }
        /// <summary>
        ///  This will return total envelope count
        /// </summary>
        public int totalEnvelopeCount { get; set; }
        /// <summary>
        ///  This will return user IsActive
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        ///  This will return user UserTypeId
        /// </summary>
        public Guid? UserTypeId { get; set; }
        /// <summary>
        /// Get/Set MessageSignatureText for user.
        /// </summary>
        public string? MessageSignatureText { get; set; }
        /// <summary>
        /// This will return user type.
        /// </summary>
        public string? UserType { get; set; }
        public bool IsAutoPopulateSignaturewhileSinging { get; set; }
        /// <summary>
        /// This will return the languageCode of the user
        /// </summary>
        public string? LanguageCode { get; set; }
        /// <summary>
        /// This will return the service language is lock or not
        /// </summary>
        public bool? IsLock { get; set; }

        public string? ExternalIdentityProvider { get; set; }
    }
    public class EditUserProfile
    {
        /// <summary>
        /// This will set user's First Name
        /// </summary>
        public string? FirstName { get; set; }
        /// <summary>
        /// This will set user's Last Name
        /// </summary>
        public string? LastName { get; set; }
        /// <summary>
        /// This will set user's Email Id
        /// </summary>
        public string? EmailId { get; set; }
        /// <summary>
        /// This will set user Initials
        /// </summary>
        public string? Initials { get; set; }
        /// <summary>
        /// This will set user Company
        /// </summary>
        public string? Company { get; set; }
        /// <summary>
        /// This will set user Title
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// This will set user SignatureString
        /// </summary>
        public string? SignatureString { get; set; }
        /// <summary>
        /// This will set user PhotoString
        /// </summary>
        public string? PhotoString { get; set; }
        /// <summary>
        /// This will set user SignatureTypeID
        /// </summary>
        public Guid SignatureTypeID { get; set; }
        /// <summary>
        /// This will set user SignatureText
        /// </summary>
        public string? SignatureText { get; set; }
        /// <summary>
        /// This will set user FontClass
        /// </summary>
        public string? FontClass { get; set; }
        /// <summary>
        /// Get/Set Message Signature Text of user.
        /// </summary>
        public string? MessageSignatureText { get; set; }
        /// <summary>
        /// Get/Set Language Code of user.
        /// </summary>
        public string? LanguageCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAutoPopulateSignaturewhileSinging { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Photo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Photopath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ProfilePicturesPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ProfilePicturesPathtemp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? SessionUserProfileRemoved { get; set; }
    }


    public class UserTypeInformation
    {
        /// <summary>
        /// User to modified its Activation and Type.
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// User type id to set.
        /// </summary>
        public Guid? UserTypeID { get; set; }
        /// <summary>
        /// Boolean to activate or deactivate user. Set 'true' to Active and 'false' to deactivate.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}



