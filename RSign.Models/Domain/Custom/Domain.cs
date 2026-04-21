using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSign.Models
{
    //public partial class Domain
    //{
    //    public Guid ID { get; set; }

    //    public Guid CompanyID { get; set; }

    //    public string? DomainName { get; set; }

    //    public DateTime CreateDate { get; set; }

    //    public DateTime ModifiedDate { get; set; }

    //    public bool IsActive { get; set; }
    //}

    public class JsonDomain
    {
        public Guid ID { get; set; }

        public string? DomainName { get; set; }

        public string? Action { get; set; }


    }

}
