using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RSign.Models
{
    public class UserInfo
    {
        [Display(Name = "Email ID")]
        public string? EmailId { get; set; }
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Display(Name = "Phone No.")]
        public string? PhoneNumber { get; set; }
        [Display(Name = "Language")]
        public string? Language { get; set; }
        [Display(Name = "Time Zone")]
        public string? TimeZone { get; set; }
        public Customer Customer { get; set; }
        public Plan Plan { get; set; }
    }

    //public class Customer
    //{
    //    [Display(Name = "Company")]
    //    public string? Name { get; set; }
    //    public string? Language { get; set; }
    //    public string? ReferenceKey { get; set; }
    //}

    //public class Plan
    //{
    //    public string? Code { get; set; }
    //    [Display(Name = "Plan Name")]
    //    public string? Description { get; set; }
    //    public string? Status { get; set; }
    //    public int AllowedUnits { get; set; }
    //    public int UnitsSent { get; set; }
    //}
}