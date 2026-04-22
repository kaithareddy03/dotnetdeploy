using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSign.Models
{
    public partial class UserToken
    {     
        
        public DateTime issuedInTime { get; set; }
        public DateTime expiresInTime { get; set; }        
    }
}
