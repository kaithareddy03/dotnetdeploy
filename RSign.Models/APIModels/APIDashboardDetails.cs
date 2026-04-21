using RSign.Models.APIModels.Data;

namespace RSign.Models.APIModels
{
    /// <summary>
    /// This entity will give dashboard details for Company, Company User, Non ompany User
    /// </summary>
    public class APIDashboardDetails
    {
        /// <summary>
        /// Get/Set User Hash ID
        /// </summary>
        public string? UserHashID { get; set; }
        /// <summary>
        /// Get/Set Company Hash ID       
        /// </summary>
        public string? CompanyHashID { get; set; }
        /// <summary>
        /// Get/Set display name of user (first name + " " + "last name") or company name in case of company details
        /// </summary>
        public string? DisplayName { get; set; }
        /// <summary>
        /// Get/Set email address in case specific user search request made
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// Get/Set envelope count which are expiring soon in near future
        /// </summary>
        public int ExpiringSoon { get; set; }
        /// <summary>
        /// Get/Set envelope count which are sent for signature
        /// </summary>
        public int SentForSignature { get; set; }
        /// <summary>
        /// Get/Set envelope count which are terminated in status
        /// </summary>
        public int Terminated { get; set; }
        /// <summary>
        /// Get/Set envelope count which are completed in status
        /// </summary>
        public int Completed { get; set; }
        /// <summary>
        /// Get/Set recipients envelope viewed percentage of sent envelope
        /// </summary>
        public int Viewed { get; set; }
        /// <summary>
        /// Get/Set recipients envelope expired percentage of sent envelope
        /// </summary>
        public int Expired { get; set; }
        /// <summary>
        /// Get/Set recipients envelope signed percentage of sent envelope
        /// </summary>
        public int Signed { get; set; }
        /// <summary>
        /// Get/Set envelope count which are sent for signing
        /// </summary>
        public int SentDocuments { get; set; }
        /// <summary>
        /// Get/Set envelope count which are signed
        /// </summary>
        public int SignedDocuments { get; set; }
        /// <summary>
        /// Get/Set envelope count which are pending for signing
        /// </summary>
        public int PendingDocuments { get; set; }
        /// <summary>
        /// Get/Set average time of completion of recipients signing process
        /// </summary>
        public int AverageTimeOfCompletion { get; set; }
        /// <summary>
        /// Get/Set user profile/company profile image base64 string
        /// </summary>
        public string? ProfileBase64String { get; set; }
        /// <summary>
        /// Get/Set user signature base64 string, in case of company will be returned as empty/null
        /// </summary>
        public string? SignatureBase64String { get; set; }
        /// <summary>
        /// Get/Set user signature in byte format, in case of company will be returned as null
        /// </summary>
        public byte[] SignatureImage { get; set; }
        /// <summary>
        /// Get/Set user profile in byte format
        /// </summary>
        public byte[] ProfileImage { get; set; }
        /// <summary>
        /// Get/Set is user search made or company search
        /// </summary>
        public bool IsUserSearch { get; set; }
        /// <summary>
        /// This will return Key and value for Multilingual
        /// </summary>
        public Dictionary<Guid?, string> DicLabelText { get; set; }

        public string? CompanyName { get; set; }

        public int WaitingForYou { get; set; }
        public int WaitingForOthers { get; set; }
    }

    /// <summary>
    /// Entity to get dashboard details for user or company
    /// </summary>
    public class DashboardRequestEntity
    {
        /// <summary>
        /// Get/Set UserID of user, for which dashboard details are requested
        /// </summary>
        public Guid? UserID { get; set; }
        ///// <summary>
        ///// Get/Set CompanyID of user/company, for which dashboard details are requested
        ///// </summary>
        public Guid? CompanyID { get; set; }
        ///// <summary>
        ///// Get/Set UserEmail of user/company, for which dashboard details are requested
        ///// </summary>
        public string? UserEmail { get; set; }

        public int Days { get; set; }
        public string? LanguageCode { get; set; }
        public bool IsLoadTempateLangAndOthers { get; set; }
    }
    /// <summary>
    /// Get dashboard details of user/ company in format of HTTP Response Message
    /// </summary>
    public class DashboardResponseEntity : ResponseMessage
    {
        /// <summary>
        /// Initializes DashboardDetails param of entity to new APIDashboardDetails
        /// </summary>        
        public DashboardResponseEntity()
        {
            DashboardDetails = new APIDashboardDetails();
        }
        /// <summary>
        /// Get/Set entity for dashboard details for user/company
        /// </summary>
        public APIDashboardDetails DashboardDetails { get; set; }
    }
    public class APIManageAdminControllerDetails
    {
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int ActiveSuperUsers { get; set; }
        public int SyncUserCountForDomain { get; set; }
        public string? Referencekey { get; set; }
        
    }
    public class APIUserSignatureText
    {
        public Guid ID { get; set; }
        public Guid UserId { get; set; }
        public string? SignatureName { get; set; }
        public string? SignatureText { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class APIUserSignatureResponse
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
        public Guid ID { get; set; }
        public Guid UserId { get; set; }
        public string? MessageSignatureText { get; set; }
        public List<APIUserSignatureText> UserSignatureTextList { get; set; } 

    }
    /// <summary>
    /// This entity will give dashboard summary  and activity report
    /// </summary>
    public class APIDashboardSummary
    {
        /// <summary>
        /// WaitingForYou
        /// </summary>
        public int WaitingForYou { get; set; }
        /// <summary>
        /// WaitingForOthers
        /// </summary>
        public int WaitingForOthers { get; set; }
        /// <summary>
        /// Get/Set envelope count which are expiring soon in near future
        /// </summary>
        public int ExpiringSoon { get; set; }
        /// <summary>
        /// Sent count
        /// </summary>
        public int Sent { get; set; }
        /// <summary>
        /// Viewed count
        /// </summary>
        public int Viewed { get; set; }
        /// <summary>
        /// View Percentage 
        /// </summary>
        public int ViewPercentage { get; set; }
        /// <summary>
        /// Pending count
        /// </summary>
        public int Pending { get; set; }
        /// <summary>
        /// Pending Percentage
        /// </summary>
        public int PendingPercentage { get; set; }
        /// <summary>
        /// Signed count
        /// </summary>
        public int Signed { get; set; }
        /// <summary>
        /// Signed Percentage
        /// </summary>
        public int SignedPercentage { get; set; }
        /// <summary>
        /// Get/Set envelope count which are Expired in status
        /// </summary>
        public int Expired { get; set; }
        /// <summary>
        /// Get/Set envelope count which are Expired percentage
        /// </summary>
        public int ExpiredPercentage { get; set; }
        /// <summary>
        /// Get/Set envelope completed times
        /// </summary>
        public string? CompletedTimes { get; set; }
        /// <summary>
        /// Get/Set envelope completed average times
        /// </summary>
        public string? AvgTimes { get; set; }
        /// <summary>
        /// get list of templates
        /// </summary>
        public Dictionary<Guid, string> templates { get; set; }
        public Dictionary<Guid, string> rules { get; set; }
        public List<LookupKeyItem> LanguageKeyText { get; set; }
        /// <summary>
        /// get the list of maseter templates
        /// </summary>
        public Dictionary<Guid, string> masterTemplates { get; set; }
        /// <summary>
        /// get the list of maseter rules
        /// </summary>
        public Dictionary<Guid, string> masterRules { get; set; }
    }
    public partial class AccountLogRecord
    {
        public string? type { get; set; }
        public string? timeLogged { get; set; }
        public string? timeQueued { get; set; }
        public string? orig { get; set; }
        public string? rcpt { get; set; }
        public string? orcpt { get; set; }
        public string? dsnAction { get; set; }
        public string? dsnStatus { get; set; }
        public string? dsnDiag { get; set; }
        public string? dsnMta { get; set; }
        public string? bounceCat { get; set; }
        public string? srcType { get; set; }
        public string? srcMta { get; set; }

        public string? dlvType { get; set; }
        public string? dlvSourceIp { get; set; }
        public string? dlvDestinationIp { get; set; }

        public string? dlvEsmtpAvailable { get; set; }
        public string? dlvSize { get; set; }
        public string? vmta { get; set; }
        public string? jobId { get; set; }
        public string? envId { get; set; }
        public string? queue { get; set; }
        public string? vmtaPool { get; set; }
    }
}
