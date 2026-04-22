using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Common
{
    public class AppSettingsConfig
    {
        public ConnectionString ConnectionStrings { get; set; }
    }

    public class ConnectionString
    {      
        public string? RSignContext { get; set; }
    }    
}
