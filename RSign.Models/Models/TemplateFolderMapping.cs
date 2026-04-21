using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Models
{
    public partial class TemplateFolderMapping
    {
        [Key]
        public System.Guid TemplateId { get; set; }
        public string? ServerName { get; set; }
        public string? UNCPath { get; set; }
        public string? StorageLevel { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
