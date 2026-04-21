using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSign.Models
{
    public partial class Roles
    {
        //public string? RoleName { get; set; }
        //public string? RoleDescription { get; set; }                        
        public DateTime CreationDate { get; set; }
        public int DisplayOrder { get; set; }
        public string? ContractStage { get; set; }
    }
}
