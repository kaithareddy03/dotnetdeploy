using System;
using System.Collections.Generic;

namespace RSign.Models.APIModels.Envelope
{
    public class EnvelopeImageInformation
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }
        public int PageNo { get; set; }
        public int DocPageNo { get; set; }
        public Dimension Dimension { get; set; }
        public DocumentInfo Document { get; set; }         
    }
}