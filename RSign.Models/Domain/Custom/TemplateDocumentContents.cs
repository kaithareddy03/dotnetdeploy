using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models
{
    public partial class TemplateDocumentContents
    {
        [NotMapped]
        public bool IsCurrentRecipient { get; set; }
        [NotMapped]
        public Control? Control { get; set; }
        [NotMapped]
        public TemplateControlStyle? TemplateControlStyle { get; set; }
        [NotMapped]
        public IList<TemplateSelectControlOptions>? SelectControlOptions { get; set; }
        [NotMapped]
        public List<TemplateSelectControlOptions>? TemplateSelectControlOptions { get; set; }
        [NotMapped]
        public TemplateDocuments? TemplateDocuments { get; set; }
        [NotMapped]
        public virtual TextType TextType { get; set; }
    }
}
