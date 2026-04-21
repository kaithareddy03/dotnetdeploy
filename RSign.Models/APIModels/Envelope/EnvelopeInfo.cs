using Microsoft.AspNetCore.Mvc.Rendering;
using RSign.Models.APIModels;
using System;
using System.Collections.Generic;


namespace RSign.Models.APIModels.Envelope
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EnvelopeInfo
    {
        public EnvelopeInfo()
        {
            ConditionalControlRules = new ConditionalControlRules();
        }
        public Guid GlobalEnvelopeID { get; set; }
        public Guid RecipientTypeId { get; set; }
        public Dictionary<Guid, string> SignersList { get; set; }
        public Dictionary<string, string> FontsList { get; set; }
        public Dictionary<string, string> ControlSizeList { get; set; }
        public IEnumerable<int> FontSizes { get; set; }
        public IEnumerable<EnvelopeImageInformation> EnvelopeImageCollection { get; set; }
        public Dictionary<string, string> MaxCharacters { get; set; }
        public Dictionary<string, string> TextTypes { get; set; }
        public ConditionalControlRules ConditionalControlRules { get; set; }
        /* Get User Settings */
        public bool SignatureCaptureHanddrawn { get; set; }
        public bool SignatureCaptureType { get; set; }
        public int ElectronicSignIndication { get; set; }
        public bool IsDisclaimerEnabled { get; set; }
        public bool IsEnvelopeEditable { get; set; }
        public string? Disclaimer { get; set; }
        public bool IsDisclaimerInCertificate { get; set; }
        public bool IsSignerAttachFileReq { get; set; }
        public int IsSignerAttachFileReqNew { get; set; }
        public List<string> SignerDocs { get; set; }
        public List<ControlsData> ControlsData { get; set; }
        public List<ControlsData> AllDocumentControls { get; set; }
        public Dictionary<Guid?, string> DicLabelText { get; set; }
        public string? EDisplayCode { get; set; }
        public string? CompletedDate { get; set; }
        public bool IsTemplateShared { get; set; }
        public bool IsSingleSigning { get; set; }

        /* Properties required from Envelope Object*/

        public string? CultureInfo { get; set; }
        public string? RecipientEmail { get; set; }
        public string? SenderEmail { get; set; }
        public Guid? RecipientTypeIDPrefill { get; set; }
        public Guid? DateFormatID { get; set; }
        public bool PasswordReqdtoSign { get; set; }
        public bool PasswordReqdtoOpen { get; set; }

        //TODO: Remove reference from response class for both belows properties
        public Guid SignerStatusId { get; set; } //TODO : This should be nullable
        public string? FolderFileSize { get; set; }
        public Guid? recipientStatusId { get; set; }
        public bool IsFinalCertificateReq { get; set; }
        public string? CompletionDate { get; set; }
        public int? HeaderFooterSettings { get; set; }
        public string? FinalDocName { get; set; }
        public string? TimeZoneSettingOptionValue { get; set; }
        public Guid? SubEnvelopeId { get; set; }
        public Dictionary<string, string> Controls { get; set; }
        public bool? IsStatic { get; set; }
        public bool? IsDefaultSignatureForStaticTemplate { get; set; }
        public bool? IsSharedTemplateContentUnEditable { get; set; } //Dynamic Envelope used for Default signature issue
        public int? IsReviewed { get; set; }
        public bool? IsTemplateDatedBeforePortraitLandscapeFeature { get; set; }
        public bool? IsAdditionalAttmReq { get; set; }
        public bool UploadSignature { get; set; }
        public bool IsSendConfirmationEmail { get; set; }
        public bool IsInvitedBySigner { get; set; }
        public bool ISNewSigner { get; set; }  
        public bool IsDisableDownloadOptionOnSignersPage { get; set; }
    }
    public class ConditionalControlRules
    {
        public ConditionalControlRules()
        {
            TextRules = new List<SelectListItem>();
            CheckBoxRules = new List<SelectListItem>();
            InitialRules = new List<SelectListItem>();
            DropdownRules = new List<SelectListItem>();
            RadioGroupRules = new List<SelectListItem>();
        }
        public List<SelectListItem> CheckBoxRules { get; set; }
        public List<SelectListItem> InitialRules { get; set; }
        public List<SelectListItem> TextRules { get; set; }
        public List<SelectListItem> DropdownRules { get; set; }
        public List<SelectListItem> RadioGroupRules { get; set; }
    }

    /* Get Document Content */
    public class ControlsData
    {
        public Guid Id { get; set; }
        public int? PageNo { get; set; }
        public int? DocPage { get; set; }
        public bool Required { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public int? XCoordinate { get; set; }
        public int? YCoordinate { get; set; }
        public int? ZCoordinate { get; set; }
        public string? Label { get; set; }
        public double? Left { get; set; }
        public double? Top { get; set; }
        public Guid DocumentId { get; set; }
        public Guid? RecipientId { get; set; }
        public string? ControlHtmlData { get; set; }
        public string? ControlHtmlID { get; set; }
        public string? ControlName { get; set; }
        public string? ControlValue { get; set; }
        public string? signerName { get; set; }
        public string? SenderControlValue { get; set; }
        public bool? IsCurrentRecipient { get; set; }
        public bool IsSigned { get; set; }
        public string? SignatureScr { get; set; }
       
        public string? FontColor { get; set; }
        public string? FontName { get; set; }
        public int? FontSize { get; set; }
        public string? FontBold { get; set; }
        public string? FontItalic { get; set; }
        public string? FontUnderline { get; set; }
        public string? SignatureText { get; set; }
        public string? SignatureFont { get; set; }
        public Guid? SignatureType { get; set; }
        public string? GroupName { get; set; }
        public IList<SelectControlOptions> ControlOptions { get; set; }
        public string? MaxLength { get; set; }
        public string? ControlType { get; set; }
        public ConditionalControlsDetailsNew ConditionDetails { get; set; }
        public string? LanguageControlName { get; set; }
        public bool? IsReadOnly { get; set; }
        public int? TabIndex { get; set; }
        public string? AdditionalValidationName { get; set; }
        public string? AdditionalValidationOption { get; set; }
        public bool? IsSignatureFromDocumentContent { get; set; }
        public bool? IsDefaultRequired { get; set; }
        public string? CustomToolTip { get; set; }
        public string? FontTypeMeasurement { get; set; }
        public Nullable<bool> IsFixedWidth { get; set; }
        public string? TextType { get; set; }
        public string? TextTypeValue { get; set; }
        public string? TextTypeMask { get; set; }
        public string? CalculatedTop { get; set; }
        public string? CalculatedLeft { get; set; }
        public int? CalculatedModalWidth { get; set; }
        public string? HoverTitle { get; set; }
        public bool ExistSameRecipient { get; set; }
        public bool EditControls { get; set; }
        public string? MaxInputLength { get; set; }
        public string? DefaultDateFormat { get; set; }  
        public string? ControlDataForAttr { get; set; }
        public string? ClassAttribute { get; set; }
       // public string? Tagstop { get; set; }
       // public string? Tagsleft { get; set; }
    }
    public class SendEnvelopeRequestPOCO
    {
        public Guid EnvelopeID { get; set; }
        public string? EnvelopeStage { get; set; }
        public List<DocumentContentPOCO> Controls { get; set; }
        public bool IsSaveControl { get; set; }
    }
}
