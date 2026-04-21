
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace RSign.Models.APIModels
{
    class APIUserDetails
    {
    }
    /// <summary>
    /// Entity to get and set UserModel
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Get/Set Email Address
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// Get/Set Browser Name
        /// </summary>
        public string? BrowserName { get; set; }
        /// <summary>
        /// Get/Set ipAddress
        /// </summary>
        public string? IPAddress { get; set; }
    }
    /// <summary>
    /// Entity to get and set UserProfileToken
    /// </summary>
    public class UserProfileToken
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
        //  /// <summary>
        //  /// This will set user Id
        //  /// </summary>
        //   public Guid UserID { get; set; }
        /// <summary>
        /// This will set user's Password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// This is User Alternate Email Address
        /// </summary>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// This is Use's Password Confirmation
        /// </summary>
        public string? ComparePassword { get; set; }
        /// <summary>
        /// This is User language
        /// </summary>
        public string? Language { get; set; }
        /// <summary>
        /// This is User timezone
        /// </summary>
        public string? TimeZone { get; set; }
        /// <summary>
        /// Get and set Registration App key
        /// </summary>
        public string? RegistrationApp { get; set; }
        /// <summary>
        /// Get and set client ID
        /// </summary>
        public string? ClientId { get; set; }
        /// <summary>
        /// Get and set customer code
        /// </summary>
        public string? CustomerCode { get; set; }
        /// <summary>
        /// User should be added as a Customer Administrator
        /// </summary>
        public Boolean IsAdminUser { get; set; }
        /// <summary>
        /// Get and Set Company Name
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// Get and Set IPAddress
        /// </summary>
        public string? IPAddress { get; set; }
    }
    /// <summary>
    /// This will return user profile POCCO.
    /// </summary>
    public class UserProfilePOCCO
    {
        public Guid UserID { get; set; }
        /// <summary>
        /// This will set user's Title
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// This will set user's Initials
        /// </summary>
        public string? Initials { get; set; }
        /// <summary>
        /// This will set user's Company
        /// </summary>
        public string? Company { get; set; }
        /// <summary>
        /// Photo
        /// </summary>
        public byte[] Photo { get; set; }
        /// <summary>
        /// This will set user SignatureText
        /// </summary>
        public string? SignatureText { get; set; }
        /// <summary>
        /// This will set user FontClass
        /// </summary>
        public string? FontClass { get; set; }
        /// <summary>
        /// This will set user SignatureTypeID
        /// </summary>
        public Guid SignatureTypeID { get; set; }
        /// <summary>
        /// Signature Image
        /// </summary>
        public byte[] SignatureImage { get; set; }
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
        public string? EmailID { get; set; }
        /// <summary>
        /// This will set user IsActive
        /// </summary>
        public Boolean IsActive { get; set; }
        /// <summary>
        /// This will set user IsUserApproved
        /// </summary>
        public Boolean IsUserApproved { get; set; }
        /// <summary>
        /// This will set user CompanyID
        /// </summary>
        public Guid? CompanyID { get; set; }
        /// <summary>
        /// This will set user UserTypeID
        /// </summary>
        public Guid? UserTypeID { get; set; }
        /// <summary>
        /// This will set user MessageSignatureText
        /// </summary>
        public string? MessageSignatureText { get; set; }
        /// <summary>
        /// This will set user CreatedDateTime
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// This will set user IsReview
        /// </summary>
        public Boolean? IsReview { get; set; }
        /// <summary>
        /// This will set user status
        /// </summary>
        public Guid? status { get; set; }
        /// <summary>
        /// This will set user ActiveFrom
        /// </summary>
        public DateTime? ActiveFrom { get; set; }
        /// <summary>
        /// This will set user ActiveTo
        /// </summary>
        public DateTime? ActiveTo { get; set; }
        /// <summary>
        /// This will set user LnguageCode
        /// </summary>
        public string? LanguageCode { get; set; }
        /// <summary>
        /// This will set user autopouplate signature
        /// </summary>
        public Boolean IsAutoPopulateSignaturewhileSinging { get; set; }
    }

    /// <summary>
    /// This will return UserReviewModel.
    /// </summary>
    public class UserReviewModel
    {
        /// <summary>
        /// This will set user email
        /// </summary>
        public string? EmailID { get; set; }
        /// <summary>
        /// This will set company name
        /// </summary>
        public string? companyName { get; set; }
        /// <summary>
        /// This will set company reference
        /// </summary>
        public string? companyReferenceKey { get; set; }
        /// <summary>
        /// This will set user IsActive
        /// </summary>
        public Boolean IsActive { get; set; }
        /// <summary>
        /// This will set company status
        /// </summary>
        public Boolean IsCompanyStatus { get; set; }
        /// <summary>
        /// This will set time zone
        /// </summary>
        public string? TimeZone { get; set; }
    }

    /// <summary>
    /// This will return response message.
    /// </summary>
    public class UserResponseMessage
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
        /// This will return Url.
        /// </summary>
        public string? ReturnUrl { get; set; }
        /// <summary>
        /// /// This will token reference key.
        /// </summary>
        public string? TokenRefKey { get; set; }
        /// <summary>
        /// this will return User Profile
        /// </summary>
        public UserProfilePOCCO UserProfile { get; set; }
        /// <summary>
        /// this will return RCS UserInfo
        /// </summary>
        public RestResponseUserInfo RCSUserInfo { get; set; }

        public UserTokenModel UserTokenModel { get; set; }
    }
    public class UserImageResponse
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
        public string? FileName { get; set; }
        /// <summary>
        /// This will return Url.
        /// </summary>
        /// <summary>
        /// Photo
        /// </summary>
        public byte[] Photo { get; set; }
    }

    public class UserTokenModel
    {
        /// <summary>
        /// Get/Set Email Address
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// Get/Set Browser Name
        /// </summary>
        public string? BrowserName { get; set; }
        /// <summary>
        /// Get/Set ipAddress
        /// </summary>
        public string? IPAddress { get; set; }

        public string? RefreshToken { get; set; }

        public string? RefreshExpires { get; set; }

        public string? AccessTokenExpires { get; set; }

        public string? AuthToken { get; set; }

        public bool GetRefreshToken { get; set; }
    }
    public class RefreshTokenModel
    {
        /// <summary>
        /// Get/Set Email Address
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary> 

        public string? RefreshToken { get; set; } 

        public string? AuthToken { get; set; }
    }

    public class RefreshTokenResponseMessage
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
        /// This will return Url.

        public RefreshTokenResponseModel UserTokenModel { get; set; }
    }

    public class RefreshTokenResponseModel
    {
        /// <summary>
        /// Get/Set Email Address
        /// </summary>
        public string? EmailAddress { get; set; }

        public string? RefreshToken { get; set; }

        public string? RefreshExpires { get; set; }

        public string? AccessTokenExpires { get; set; }

        public string? AuthToken { get; set; }
    }

    public class ForgotPasswordModal
    {
        /// <summary>
        /// Email address for which password has to reset.
        /// </summary>
        public string? UserEmailAddress { get; set; }

        public string? Language { get; set; }
    }

    public class SSOTokenResponseMessage
    {
        public HttpStatusCode StatusCode { get; set; }       
        public string? StatusMessage { get; set; }       
        public string? Message { get; set; }    
        public RefreshTokenResponseModel UserTokenModel { get; set; }
        public RestResponseUserInfo RCSUserInfo { get; set; }      
        public string? AuthMessage { get; set; }      
        public string? AuthToken { get; set; }       
        public string? EmailId { get; set; }      
        public Guid UserId { get; set; }       
    }
 
    public class EnterpriseSSORestResponseMessage
    {
        public string? Status { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusText { get; set; }
        public List<Messages> Message { get; set; }
        public ResultContentSSO ResultContent { get; set; }
    }

    public class ResultContentSSO
    {
        public int SSORst { get; set; }
        public string? Message { get; set; }
    }
    public class ValidateBotClickResponseMessage
    {
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string StatusText { get; set; }
        public List<Messages> Message { get; set; }
        public ResultContentBot ResultContent { get; set; }
    }

    public class ResultContentBot
    {
        public bool IsBotClick { get; set; }
        public bool IsVPN { get; set; }
        public bool IsCDN { get; set; }
    }
}
