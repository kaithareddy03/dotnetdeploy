using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace RSign.Models
{

    public partial class DocumentContents
    {
        [AllowHtml]
        [NotMapped]
        public string? ControHtmlData { get; set; }
        [NotMapped]
        public Guid GlobalEnvelopeID { get; set; }
        [NotMapped]
        public string? ControlName { get; set; }
        [NotMapped]
        public string? RecipientName { get; set; }
        [NotMapped]
        public string? RecipientEmailAddress { get; set; }
        [NotMapped]
        public bool IsCurrentRecipient { get; set; }
        [NotMapped]
        public bool IsSigned { get; set; }
        [NotMapped]
        public Control? Control { get; set; }
        [NotMapped]
        public ControlStyle? ControlStyle { get; set; }
        [NotMapped]
        public ControlStyle? CurrentControlStyle { get; set; }
        [NotMapped]
        public IList<SelectControlOptions>? ControlOptions { get; set; }
        [NotMapped]
        public List<SelectControlOptions>? SelectControlOptions { get; set; }
        [NotMapped]
        public int TotalControlCount { get; set; }
        [NotMapped]
        public byte[]? SignatureByte { get; set; }
        [NotMapped]
        public string? SignatureString { get; set; }
        [NotMapped]
        public string? SignatureFont { get; set; }
        [NotMapped]
        public Guid SignatureType { get; set; }
        [NotMapped]
        public string? DocumentContentID { get; set; }
        [NotMapped]
        public string? EnvelopeStage { get; set; }
        [NotMapped]
        public double Left { get; set; }
        [NotMapped]
        public double Top { get; set; }
        [NotMapped]
        public string? RadioName { get; set; }
        [NotMapped]
        public Documents? Documents { get; set; }
    }
}
