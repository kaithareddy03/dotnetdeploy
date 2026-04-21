namespace RSign.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class TemplateControlStyle
    {
        [Key]
        public System.Guid DocumentContentId { get; set; }
        public System.Guid FontID { get; set; }
        public byte FontSize { get; set; }
        public string? FontColor { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }
        public string? AdditionalValidationName { get; set; }
        public string? AdditionalValidationOption { get; set; }
    
        //public virtual FontList FontList { get; set; }
        //public virtual TemplateDocumentContents TemplateDocumentContents { get; set; }
    }
}
