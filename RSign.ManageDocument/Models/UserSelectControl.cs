using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Models
{
    public class UserSelectControl
    {
        public string OptionText { get; set; }
        public int Order { get; set; }
        public bool IsSelected { get; set; }
        public bool IsActive { get; set; }
    }
}
