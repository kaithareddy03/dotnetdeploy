using RSign.Models.APIModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSign.Models
{
    public class APIControl
    {
        /// <summary>
        /// This will set XCordinate of Control.
        /// </summary>
        public int XCoordinate { get; set; }
        /// <summary>
        /// This will set YCordinate of Control.
        /// </summary>
        public int YCoordinate { get; set; }
        /// <summary>
        /// This will set ZCordinate of Control.
        /// </summary>
        public int ZCoordinate { get; set; }
        /// <summary>
        /// Get/Set Left of current control
        /// </summary>
        public double Left { get; set; }
        /// <summary>
        /// Get/Set Top of current control
        /// </summary>
        public double Top { get; set; }
        /// <summary>
        /// Get/Set control htmlid for control optional
        /// </summary>
        public string? ControlHtmlID { get; set; }
        /// <summary>
        /// This will set DocumentContentId of Control.
        /// </summary>
        public string? DocumentContentID { get; set; }
        /// <summary>
        /// This will set DocumentId of Control.
        /// </summary>
        
       // [NotMapped]
        public string? DocumentID { get; set; }
        /// <summary>
        /// Get/Set page number of control among all documents
        /// </summary>
        public int? PageNo { get; set; }
        /// <summary>
        /// Get/Set page number of control for a document itself
        /// </summary>
        public int? DocumentPageNo { get; set; }        
        /// <summary>
        /// This will set either an Envelope Id or Template Id.
        /// </summary>
        public string? GlobalEnvelopeID { get; set; }
        /// <summary>
        /// From this will determine envelope stage
        /// </summary>
        public string? EnvelopeStage { get; set; }
    }
    public class ControlToHtmlIDMapping
    {
        /// <summary>
        /// Get/Set control id
        /// </summary>
        public string? DocumentContentID { get; set; }
        /// <summary>
        /// Get/Set ControlHtml id of control
        /// </summary>
        public string? ControlHtmlID { get; set; }
        /// <summary>
        /// Get/Set dropdown control options
        /// </summary>
        public List<SelectControlOptionDetails> SelectOptions { get; set; }
    }
    public class PasteControlsResponsePoco
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public System.Net.HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// This will return  Envelope Id.
        /// </summary>
        public string? EnvelopeId { get; set; }
        /// <summary>
        /// Get/Set controls response
        /// </summary>
        public List<ControlToHtmlIDMapping> ControlsResponse { get; set; }
    }
    public class PasteControlsPoco
    {
        /// <summary>
        /// Get/Set the envelope id for whitch controls to be pasted
        /// </summary>
        public string? EnvelopeID { get; set; }
        /// <summary>
        /// Get/Set controls to paste on document
        /// </summary>
        public List<AllControlsPoco> Controls { get; set; } 
    }
    public class AllControlsPoco : APIControl
    {
        /// <summary>
        /// Get/Set control type id to distinguish the control
        /// </summary>
        public string? ControlID { get; set; }
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set MaxcharID of Control.
        ///</summary>
        public string? MaxLength { get; set; }
        /// <summary>
        /// This will set TextTypeID of Control.
        ///</summary>
        public string? ControlType { get; set; }
        /// <summary>
        /// This will set Label Text of Control.
        ///</summary>
        public string? Label { get; set; }
        /// <summary>
        /// This will set Group Name of Control.
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// Get/Set radio name of control
        /// </summary>
        public string? RadioName { get; set; }
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public string? SenderControlValue { get; set; }
        /// <summary>
        /// Is Confirmation mail required before completing envelope signing process, if set to 1 then true or false
        /// </summary>
        public string? IsSubmitConfirmation { get; set; }
        /// <summary>
        /// This will set Selected Option  of Control.
        /// </summary>
        public List<SelectOption> ControlOptions { get; set; }
    }
    public class SignatureControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
    }
    public class EmailControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// Is Confirmation mail required before completing envelope signing process, if set to 1 then true or false
        /// </summary>
        public string? IsSubmitConfirmation { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
    }
    public class NameControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
    }

    public class TitleControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
    }

    public class CompanyControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
    }

    public class InitialsControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
    }

    public class LabelControl : APIControl
    {
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set Text of Control.
        ///</summary>
        public string? Label { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
    }

    public class HyperlinkControl : APIControl
    {
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set Text of Control.
        ///</summary>
        public string? Label { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
    }

    public class DateControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
    }

    public class RadioControl : APIControl
    {
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Group Name of Control.
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// Get/Set radio name of control
        /// </summary>
        public string? RadioName { get; set; }
    }

    public class CheckboxControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public string? SenderControlValue { get; set; }

    }

    public class TextControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set MaxcharID of Control.
        ///</summary>
        public string? MaxLength { get; set; }
        /// <summary>
        /// This will set TextTypeID of Control.
        ///</summary>
        public string? ControlType { get; set; }
        /// <summary>
        /// This will set Label Text of Control.
        ///</summary>
        public string? Label { get; set; }
    }

    public class ControlResponse
    {
        /// <summary>
        /// This will set Envelope ID of Control.
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// This will set DocumentContentId of Control.
        /// </summary>
        public Guid DocumentContentId { get; set; }
    }

    public class DropDownBoxControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Selected Option  of Control.
        /// </summary>
        public List<SelectOption> ControlOptions { get; set; }
    }

    public class DateTimeStampControl : APIControl
    {
        /// <summary>
        /// This will set the control as Required or Not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will set Recipient Id of Control.
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// This will set Width of Control.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// This will set Color of Control.
        ///</summary>
        public string? Color { get; set; }
        /// <summary>
        /// This will set font Size of Control.
        ///</summary>
        public byte fontSize { get; set; }
        /// <summary>
        /// This will set font FamilyID of Control.
        ///</summary>
        public string? fontFamilyID { get; set; }
        /// <summary>
        /// This will set Bold type of Control.
        ///</summary>
        public bool Bold { get; set; }
        /// <summary>
        /// This will set Underline type of Control.
        ///</summary>
        public bool Underline { get; set; }
        /// <summary>
        /// This will set Italic type of Control.
        ///</summary>
        public bool Italic { get; set; }
        /// <summary>
        /// This will set Height of Control.
        /// </summary>
        public int Height { get; set; }
    }

    public class SelectOption
    {
        /// <summary>
        /// This will set Option  of Control.
        /// </summary>
        public string? OptionText { get; set; }
    }
}
