using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Models
{
    public class ControlTag
    {
        public ControlTag()
        {
            Properties = new List<ControlProperties>();
        }
        public double DocHeight { get; set; }
        public double DocWidth { get; set; }
        public double ImgHeight { get; set; }
        public double ImgWidth { get; set; }
        public List<ControlProperties> Properties { get; set; }
        public string? EnvelopeID { get; set; }
        public string? DocumentID { get; set; }
        public string? DocumentName { get; set; }
        public int StartIndex { get; set; }
        public string? pageName { get; set; }
    }
    public class ControlProperties
    {
        public string? RecipientID { get; set; }
        public string? RecipientEmail { get; set; }
        public string? ControlName { get; set; }//
        public string? Required { get; set; }//
        public int FontSize { get; set; }//
        public string? FontFamily { get; set; }//
        public string? color { get; set; }//
        public string? Bold { get; set; }//
        public string? Underline { get; set; }//
        public string? Italic { get; set; }//
        public string? Text { get; set; }//Tx#Mak
        public string? Maxchar { get; set; }//M#20
        public string? TextType { get; set; }//
        public double XCoordinate { get; set; }//
        public double YCoordinate { get; set; }//
        public double ZCoordinate { get; set; }
        public double LeftPosition { get; set; }
        public double TopPosition { get; set; }
        public int PageNo { get; set; }
        public int ImagePageNo { get; set; }
        public double PageHeight { get; set; }
        public double PageWidth { get; set; }
        public string? Tag { get; set; }
        public string? DynamicControlId { get; set; }
        public string? ControlValue { get; set; }
        public string? ReadOnly { get; set; }
        public int? Order { get; set; }
        public string? RecipientType { get; set; }
        public int ControlWidth { get; set; }
    }
    /// <summary>
    /// Get the tag details on a document
    /// </summary>
    public class TagDetailsWithDocument
    {
        public TagDetailsWithDocument()
        {
            TagDetails = new List<TagDetails>();
            ErrorTagDetails = new List<TagDetails>();
        }
        /// <summary>
        /// Get/Set Envelope ID from which document tags needs to be find
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// Get/Set Document ID currently proccessing for finding tags
        /// </summary>
       // public Guid DocumentID { get; set; }
        /// <summary>
        /// Get/Set Document Name currently proccessing for finding tags
        /// </summary>
        public string? DocumentName { get; set; }
        public int StartIndex { get; set; }
        public double DocumentWidth { get; set; }
        public double DocumentHeight { get; set; }
        /// <summary>
        /// Get/Set Document Converted Path currently proccessing for finding tags
        /// </summary>
        public string? ConvertedDocumentPath { get; set; }
        /// <summary>
        /// Get/Set the tag details exist on currently proccessing document
        /// </summary>
        public List<TagDetails> TagDetails { get; set; }
        /// <summary>
        /// Get/Set the errorneous tag details exist on currently proccessing document
        /// </summary>
        public List<TagDetails> ErrorTagDetails { get; set; }
    }
    /// <summary>
    /// Get the tags exist on a document with page number
    /// </summary>
    public class TagDetails
    {
        /// <summary>
        /// Get the tag exist on a document
        /// </summary>
        public string? TagString { get; set; }
        /// <summary>
        /// Get the X position of tag present on document
        /// </summary>
        public double TagXIndent { get; set; }
        /// <summary>
        /// Get the Y position of tag present on document
        /// </summary>
        public double TagYIndent { get; set; }
        /// <summary>
        /// Get document page number on which tag is exist
        /// </summary>
        public int DocumentPageNumber { get; set; }
        /// <summary>
        /// Get page width from using Bitmap on which tag found
        /// </summary>
        public double PageBitmapWidth { get; set; }
        /// <summary>
        /// Get page height from using Bitmap on which tag found
        /// </summary>
        public double PageBitmapHeight { get; set; }
        public string? DynamicControlId { get; set; }
        public string? RecipientType { get; set; }
    }
    /// <summary>
    /// Get the errorneous tags from uploaded documents
    /// </summary>
    public class ErrorTagDetailsResponse
    {
        public ErrorTagDetailsResponse()
        {
            ErrorneousTags = new List<TagDetails>();
        }
        /// <summary>
        /// Get/Set envelope id.
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// Get/Set document id on which error tags found
        /// </summary>
       // public Guid DocumentID { get; set; }
        /// <summary>
        /// Get/Set docuement name on which error tags found
        /// </summary>
        public string? DocumentName { get; set; }
        /// <summary>
        /// Get/Set error tag details found on document
        /// </summary>
        public List<TagDetails> ErrorneousTags { get; set; }
    }
    public class ImageProperties
    {
        public double Height { get; set; }
        public double Width { get; set; }
    }

    public class EnvelopeImageCollection
    {
        public int ID { get; set; }
        public string? FilePath { get; set; }
    }

    public class EnvelopeRecipientRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? Order { get; set; }
        public Guid? ID { get; set; }
        public string? RecipientType { get; set; }
        public Guid? RoleID { get; set; }
        public List<ControlRequest> Controls { get; set; }
    }
    public class ControlRequest
    { 
        public string? Name { get; set; }
        public string? Tag { get; set; }
        /// <summary>
        /// This will return the control value
        /// </summary>
        public string? ControlValue { get; set; }
        /// <summary>
        /// This will return the sign byte for Signature and initial control
        /// </summary>
        public string? SignatureControlValue { get; set; }
        /// <summary>
        /// Get Set Control Read only property
        /// </summary>
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// Get Set Control Required property
        /// </summary>
        public bool IsRequired { get; set; }
        public string? ControlHtmlId { get; set; }

        public Guid DynamicControlId { get; set; }
        public string? LabelText { get; set; }
        public int? TabIndex { get; set; }
    }

    public class TagsRecipientDetails
    {
        /// <summary>
        /// This will return Recipient Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        ///  This will return Envelope Id.
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        ///  This will return Recipient StatusId.
        /// </summary>
        public Guid StatusID { get; set; }
        /// <summary>
        /// This will return Recipient name.
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// This will return old Recipient name.
        /// </summary>

        public Guid RecipientTypeID { get; set; }
        /// <summary>
        /// This will return Recipient Email Address.
        /// </summary>
        public string? EmailID { get; set; }
        /// <summary>
        /// This will return recipient's order.
        /// </summary>
        public int? Order { get; set; }

        public string? RecipientType { get; set; }

    }

    public class TagRecipientDetails
    {
        /// <summary>
        /// This will return Recipient Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        ///  This will return Envelope Id.
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        ///  This will return Recipient StatusId.
        /// </summary>
        public Guid StatusID { get; set; }
        /// <summary>
        /// This will return Recipient name.
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// This will return old Recipient name.
        /// </summary>
        public string? OldRecipient { get; set; }
        /// <summary>
        /// This will return old Recipient Email.
        /// </summary>
        public string? OldEmail { get; set; }
        /// <summary>
        /// This will return recipients type id.
        /// </summary>
        public Guid RecipientTypeID { get; set; }
        /// <summary>
        /// This will return Recipient Email Address.
        /// </summary>
        public string? EmailID { get; set; }
        /// <summary>
        /// This will return recipient's order.
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// This will return Recipient's created date and time.
        /// </summary>
        public DateTime? CreatedDateTime { get; set; }
        /// <summary>
        /// This will return Recipient's type.
        /// </summary>
        public string? RecipientType { get; set; }
        /// <summary>
        /// This will return IpAddress.
        /// </summary>
        public string? IpAddress { get; set; }
        /// <summary>
        /// This will return Template Code.
        /// </summary>
        public Guid? TemplateID { get; set; }
        /// <summary>
        /// This will return Recipient Code.
        /// </summary>
        public string? RecipientCode { get; set; }
        /// <summary>
        /// This will return Template Group id.
        /// </summary>
        public long? TemplateGroupId { get; set; }
        /// <summary>
        /// This will retrun EnvelopeTemplateGroupId
        /// </summary>
        public Guid? EnvelopeTemplateGroupId { get; set; }
        /// <summary>
        /// This will retrun Finish Status
        /// </summary>
        public bool? IsFinished { get; set; }

        public Guid? TemplateRoleId { get; set; }
        public string? CopyEmailID { get; set; }

        public bool? IsSameRecipient { get; set; }

        public string? VerificationCode { get; set; }
    }

    public class TagsAPIEnvelopeRecipientRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? Order { get; set; }
        public Guid? ID { get; set; }
        public string? RecipientType { get; set; }
        public Guid? RoleID { get; set; }
        public List<TagsAPIControlRequest> Controls { get; set; }
    }

    public class TagsAPIControlRequest
    {
        public string? Name { get; set; }
        public string? Tag { get; set; }
        /// <summary>
        /// This will return the control value
        /// </summary>
        public string? ControlValue { get; set; }
        /// <summary>
        /// This will return the sign byte for Signature and initial control
        /// </summary>
        public string? SignatureControlValue { get; set; }
        /// <summary>
        /// Get Set Control Read only property
        /// </summary>
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// Get Set Control Required property
        /// </summary>
        public bool IsRequired { get; set; }
        public string? ControlHtmlId { get; set; }

        public Guid DynamicControlId { get; set; }
        public string? LabelText { get; set; }
        public int? TabIndex { get; set; }
    }

    public class TagControlWidthChars
    {
        public string? MaxCharsAllowed { get; set; }
        public int ControlWidth { get; set; }
        public string? ControlValue { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
