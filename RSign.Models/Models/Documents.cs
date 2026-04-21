
namespace RSign.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Documents
    {       
        public System.Guid ID { get; set; }
        public System.Guid EnvelopeID { get; set; }
        public string? DocumentName { get; set; }
        public System.DateTime UploadedDateTime { get; set; }
        public string? TemplateDocumentName { get; set; }
        public Nullable<short> Order { get; set; }
        public Nullable<System.Guid> TemplateID { get; set; }
        public Nullable<long> TemplateGroupId { get; set; }
        public Nullable<System.Guid> EnvelopeTemplateGroupID { get; set; }
        public string? OriginalDocName { get; set; }
        public Nullable<bool> IsDocumentStored { get; set; }
        public string? DocumentSource { get; set; }
        public string? SourceLocation { get; set; }
        public Nullable<bool> IsDocumentUploadedToSource { get; set; }
        public string? ActionType { get; set; }
        public string? SourceDocumentId { get; set; }  
    }
}
