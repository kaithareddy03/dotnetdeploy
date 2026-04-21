using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models
{
    public partial class TemplateControlStyle
    {
        [NotMapped]
        public FontList FontList { get; set; }
    }
}
