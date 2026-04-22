using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace RSign.Models
{
    public partial class Company
    {
        [NotMapped]
        public string? CompanyHashID { get; set; }
        [NotMapped]
        public string? PhotoString { get; set; }
        public List<Domain> DomainList { get; set; }
    }

    public class EnvelopeCodeList
    {

        public string? EnvelopeCode { get; set; }

    }
    public class UserEmailList
    {
        public Guid? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public Guid UserID { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? UserType { get; set; }

        public bool IsActive { get; set; }
    }
    public class CompanyUserPOCO
    {
        public string? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public string? UserID { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? UserType { get; set; }

        public bool IsExists { get; set; }
    }
    public class CompanyNameList
    {
        public Guid CompanyID { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyDescription { get; set; }

    }
    public class UserListWithCompanyHelper
    {
        public Guid UserID { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public Guid? CompanyID { get; set; }
        public string? CompanyName { get; set; }
    }

    public class CompanyData : CompanyInfo
    {
        /// <summary>
        /// This will set Domain List.
        /// </summary>
        public List<DomainData> DomainList { get; set; }
    }
    public class CompanyInfo
    {
        /// <summary>
        /// This will return Company ID.
        /// </summary>
        public Guid CompanyID { get; set; }

        /// <summary>
        /// This will return Company Name.
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// This will return Company Description.
        /// </summary>
        public string? CompanyDescription { get; set; }

        /// <summary>
        ///This will return company Logo image in the base 64 format..
        /// </summary>
        public byte[] CompanyLogo { get; set; }

        /// <summary>
        /// This will return company created date. 
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// This will return company modified date. 
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// This will return flag for company is active or not.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// This will return registered admin email id.
        /// </summary>
        public string? AdminEmailID { get; set; }
        /// <summary>
        /// This will return company transparency feature status.
        /// </summary>
        public bool IsTransparencyFeatureOn { get; set; }
        /// <summary>
        /// This will return post signer landing page url for company.
        /// </summary>
        public string? PostSigningLandingPage { get; set; }
        /// <summary>
        /// This will return reference key of company.
        /// </summary>
        public string? Referencekey { get; set; }
        /// <summary>
        /// Encrypted Company ID
        /// </summary>
        public string? CompanyHashID { get; set; }
        public string? FormattedCreatedDate { get; set; }
        
    }
    public class DomainData
    {
        /// <summary>
        /// This will return Domain ID.
        /// </summary>
        public Guid DomainID { get; set; }

        /// <summary>
        /// This will return Domain Name.
        /// </summary>
        public string? DomainName { get; set; }
        /// <summary>
        /// This will return domain created date. 
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// This will return domain modified date. 
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// This will return flag for domain is active or not.
        /// </summary>
        public bool IsActive { get; set; }

    }

    public class CompanyDataR : CompanyInfoR
    {
        /// <summary>
        /// This will set Domain List.
        /// </summary>
        public List<DomainDataR> DomainList { get; set; }
    }
    public class CompanyInfoR
    {
        /// <summary>
        /// This will set Company ID.
        /// </summary>
        public Guid CompanyID { get; set; }

        /// <summary>
        /// This will set Company Name.
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// This will set Company Description.
        /// </summary>
        public string? CompanyDescription { get; set; }

        /// <summary>
        ///This will set company Logo image in the base 64 format..
        /// </summary>
        public byte[] CompanyLogo { get; set; }

        //public DateTime? CreatedDate { get; set; }

        //public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// This will set company is active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// This will set company admin emailID .
        /// </summary>
        public string? AdminEmailID { get; set; }
        /// <summary>
        /// This will set Transparency Feature of company .
        /// </summary>
        public bool IsTransparencyFeatureOn { get; set; }

        /// <summary>
        /// This will set Post Signing Landing Page Url.
        /// </summary>
        public string? PostSigningLandingPage { get; set; }

        /// <summary>
        /// This will set RCS Referencekey  of company.
        /// </summary>
        public string? Referencekey { get; set; }
    }


    public class DomainDataR
    {
        /// <summary>
        /// This will set Domain ID.
        /// </summary>
        public Guid DomainID { get; set; }

        /// <summary>
        /// This will set Domain Name.
        /// </summary>
        public string? DomainName { get; set; }
        /// <summary>
        /// This will set Domain is active or not.
        /// </summary>
        public bool IsActive { get; set; }

    }

    public class ResponseMessageWithCompanyGuid
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
        /// This will return  Company Id.
        /// </summary>
        public string? CompanyId { get; set; }

    }   

    public class UserList
    {
        /// <summary>
        /// This will return  Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// This will return User Id.
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// This will return FirstName.
        /// </summary>
        public string? FirstName { get; set; }
        /// <summary>
        /// This will return LastName.
        /// </summary>
        public string? LastName { get; set; }
        /// <summary>
        /// This will return User Email.
        /// </summary>
        public string? UserEmail { get; set; }
        /// <summary>
        /// This will return User UserTypeID.
        /// </summary>
        public Guid? UserTypeID { get; set; }
        /// <summary>
        /// This will return User CompanyID.
        /// </summary>
        public Guid? CompanyID { get; set; }
        /// <summary>
        /// This will return Active and Inactive status.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// This will return User Company.
        /// </summary>
        public string? Company { get; set; }
        /// <summary>
        /// This will return User Photo.
        /// </summary>
        public byte[] Photo { get; set; }
        /// <summary>
        /// This will return User SignatureImage.
        /// </summary>
        public byte[] SignatureImage { get; set; }
        /// <summary>
        /// This will return User Title.
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// This will return User Initials.
        /// </summary>
        public string? Initials { get; set; }
        /// <summary>
        /// This will return User FontClass.
        /// </summary>
        public string? FontClass { get; set; }
        /// <summary>
        /// This will return User SignatureText.
        /// </summary>
        public string? SignatureText { get; set; }
        /// <summary>
        /// This will return User SignatureTypeID.
        /// </summary>
        public Guid SignatureTypeID { get; set; }
        /// <summary>
        /// This will return User SignatureTypeID.
        /// </summary>
        public bool IsUserApproved { get; set; }

    }

    public class ResponseWithSyncUsers
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
        /// This will return  userList
        /// </summary>
        public List<SyncUserData> UserData { get; set; }
    }

    public class SyncUserData
    {
        /// <summary>
        /// This will set user's FistName and Last name
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// This will set user's EmailId
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// This will set user's UsertypeId
        /// </summary>
        public Guid? UserTypeId { get; set; }
        /// <summary>
        /// This will set user's Activation status
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// This will set user's ExternalIdentityProvider
        /// </summary>
        public string? ExternalIdentityProvider { get; set; }
    }
    public class AutoSuggestRequestPOCO
    {
        
        public string? companyName { get; set; }
        public string? userName { get; set; }
        public string? userid { get; set; }
        public string? companyId { get; set; }
        public string? userTypeRole { get; set; }
        public string? userStatus { get; set; }
    }
    public class AutoCompleteSearch
    {
        public string? Term { get; set; }
        public object SearchObj { get; set; }
    }

    public class SyncCompanyAndUser
    {
        /// <summary>
        /// Get/Set caller email address
        /// </summary>
        public string? CallerEmailAddress { get; set; }
        /// <summary>
        /// Get/Set Company Name
        /// </summary>
        public string? CompanyName { get; set; }
        /// <summary>
        /// Get/Set Company Description
        /// </summary>
        public string? CompanyDescription { get; set; }
        /// <summary>
        /// Get/Set Company Admin Email
        /// </summary>
        public string? AdminEmail{ get; set; }
        /// <summary>
        /// Get/Set Company Reference Key
        /// </summary>
        public string? ReferenceKey { get; set; }
        /// <summary>
        /// Get/Set Key for API Integration
        /// </summary>
        public string? APIKey { get; set; }
        /// <summary>
        /// This will set user's envelope and template update status
        /// </summary>
        public bool IsMoveEnvelopeAndTemplate { get; set; }
        /// <summary>
        /// This will set language code
        /// </summary>
        public string? LanguageCode { get; set; }
        /// <summary>
        /// Get/Set UserList
        /// </summary>
        public List<SyncUserData> SyncUserData { get; set; }
    }

    //Updated by TParker- S3-1504-Enhancement: API to update Post Signing URL from RPortal to RSign
    public class CompanyDetails
    {
        /// <summary>
        /// This will set CallerEmailAddress.
        /// </summary>
        public string? CallerEmailAddress { get; set; }
        /// <summary>
        /// This will set Company List.
        /// </summary>
        public List<CompanyInfoR> CompanyData { get; set; }
    }
}
