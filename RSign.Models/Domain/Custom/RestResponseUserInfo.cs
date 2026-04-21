using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RSign.Models
{
    public class RestResponseUserInfo
    {
        public string? Status { get; set; }
        public int StatusCode { get; set; }
        public string? StatusText { get; set; }
        public List<object> Message { get; set; }
        public ResultContent ResultContent { get; set; }
        public Dictionary<Guid?, string> DicLabelText { get; set; }
    }


    public class ResultContent
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Display(Name = "Email Address")]
        public string? Email { get; set; }
        [Display(Name = "Phone No.")]
        public string? Phone { get; set; }
        [Display(Name = "Time Zone")]
        public string? TimeZone { get; set; }
        [Display(Name = "Language")]
        public string? Language { get; set; }
        public string? Status { get; set; }
        public Customer Customer { get; set; }
        public Plan Plan { get; set; }
        public string? ExternalIdentityProvider { get; set; }
    }

    public class Customer
    {
        [Display(Name = "Company")]
        public string? Name { get; set; }
        public string? ReferenceKey { get; set; }
        public string? Language { get; set; }
        public string? Status { get; set; }
    }

    public class Plan
    {
        public string? Code { get; set; }
        [Display(Name = "Plan Name")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Allowed Units")]
        public int AllowedUnits { get; set; }
        [Display(Name = "Units Sent")]
        public int UnitsSent { get; set; }
    }

    public class RcsBaseResponse
    {
        public string? Status { get; set; }
        public int StatusCode { get; set; }
        public string? StatusText { get; set; }
        public List<object> Message { get; set; }
    }

    public class UpgradeLinkInfo : RcsBaseResponse
    {
        public string? ResultContent { get; set; }
    }

    public class UsageRemaining : RcsBaseResponse
    {
        public UsageRemainingResultContent ResultContent { get; set; }
    }

    public class UsageRemainingResultContent
    {
        public string? PlanName { get; set; }
        public bool? OnTrialPlan { get; set; }
        public string? PlanType { get; set; }
        public string? PlanRange { get; set; }
        public int? UnitsRemaining { get; set; }
        public string? UpgradeLink { get; set; }
    }

}