using System.Net;

namespace RSign.Models.APIModels
{
    class APIDocumentDetails
    {
    }
    /// <summary>
    /// Entity to delete controls for the envelope id
    /// </summary>
    public class APIControlDeletePOCO
    {
        /// <summary>
        /// Get/Set Envelope ID for which control to delete
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// Get/Set envelope stage from which controls to delete
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// Get/Set List of control ids to delete
        /// </summary>
        public List<Guid> ControlsID { get; set; }
    }
    public class ResponseMessageDocumentWithDocument
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
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
        /// This will return  Document Id .
        /// </summary>
        public string? DocumentId { get; set; }
        /// <summary>
        /// This will return  File Name .
        /// </summary>
        public string? FileName { get; set; }

        public string? ActionType { get; set; }

        public long FileSize { get; set; }
        public List<string> DocumentList { get; set; }
    }
    public class ResponseMessageDocumentWithDocumentSize
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
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
        /// This will return  Document Id .
        /// </summary>
        public string? DocumentId { get; set; }
        /// <summary>
        /// This will return uploaded file size in bytes
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// This will return  File Name .
        /// </summary>
        public string? FileName { get; set; }

        public string? ActionType { get; set; }
    }
    public class UploadLocalDocument
    {
        /// <summary>
        /// Name of the file which is to be uploaded.
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// This will set Envelope Id to which the document will be uploaded.
        /// </summary>
        public string? EnvelopeId { get; set; }
        /// <summary>
        ///  This will set file data in the base 64 format.
        /// </summary>
        public string? DocumentBase64Data { get; set; }
        /// <summary>
        /// This will identify envelope is in which stage - InitializeEnvelope/PrepareEnvelope/InitializeDraft/PrepreaDraft/InitializeTemplate/EditTemplate etc.
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// This will return Company Id.
        /// </summary>
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// This will set or return User Email Id
        /// </summary>
        public string? UserEmailId { get; set; }

        public string? CompanyReferenceKey { get; set; }
        public int? PrintActions { get; set; }
    }

    public class UploadGoogleDocument
    {
        /// <summary>
        ///  Name of the file which is to be uploaded from Google Drive.
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// This will set Envelope Id to which the document will be uploaded.
        /// </summary>
        public string? EnvelopeId { get; set; }
        /// <summary>
        ///  URL of the file which is to be uploaded.
        /// </summary>
        public string? DownloadUrl { get; set; }
        /// <summary>
        /// This will set Access Token.
        /// </summary>
        public string? AccessToken { get; set; }
        /// <summary>
        /// This will identify envelope is in which stage - InitializeEnvelope/PrepareEnvelope/InitializeDraft/PrepreaDraft/InitializeTemplate/EditTemplate etc.
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// This will set update Document ID.
        /// </summary>
        public string? UpdatedDocumentId { get; set; }
    }
    public class UploadDropboxDocument
    {
        /// <summary>
        ///  Name of the file which is to be uploaded from Dropbox.
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// This will set Envelope Id to which the document will be uploaded.
        /// </summary>
        public string? EnvelopeId { get; set; }
        /// <summary>
        /// URL of the file which is to be uploaded.
        /// </summary>
        public string? DownloadUrl { get; set; }
        /// <summary>
        /// This will identify envelope is in which stage - InitializeEnvelope/PrepareEnvelope/InitializeDraft/PrepreaDraft/InitializeTemplate/EditTemplate etc.
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// This will set update Document ID.
        /// </summary>
        public string? UpdatedDocumentId { get; set; }

        public string? IsEnableFileReview { get; set; }
    }
    public class UploadSkydriveDocument
    {
        /// <summary>
        /// Name of the file which is to be uploaded from Skydrive.
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// This will set Envelope Id to which the document will be uploaded.
        /// </summary>
        public string? EnvelopeId { get; set; }
        /// <summary>
        ///  URL of the file which is to be uploaded.
        /// </summary>
        public string? DownloadUrl { get; set; }
        /// <summary>
        /// This will identify envelope is in which stage - InitializeEnvelope/PrepareEnvelope/InitializeDraft/PrepreaDraft/InitializeTemplate/EditTemplate etc.
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// This will set update Document ID.
        /// </summary>
        public string? UpdatedDocumentId { get; set; }

        public string? IsEnableFileReview { get; set; }
    }
    public class UploadDriveDocument
    {
        /// <summary>
        ///  Name of the file which is to be uploaded from Google Drive.
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// This will set Envelope Id to which the document will be uploaded.
        /// </summary>
        public string? EnvelopeId { get; set; }
        /// <summary>
        ///  URL of the file which is to be uploaded.
        /// </summary>
        public string? DownloadUrl { get; set; }
        /// <summary>
        /// This will set Access Token.
        /// </summary>
        public string? AccessToken { get; set; }
        /// <summary>
        /// This will identify envelope is in which stage - InitializeEnvelope/PrepareEnvelope/InitializeDraft/PrepreaDraft/InitializeTemplate/EditTemplate etc.
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// This will set update Document ID.
        /// </summary>
        public string? UpdatedDocumentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? DriveType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? EnvelopeOrTemplate { get; set; }

        public string? IsEnableFileReview { get; set; }

    }


    public class UploadiManageDocument
    {
        /// <summary>
        ///  Name of the file which is to be uploaded from Google Drive.
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// This will set Envelope Id to which the document will be uploaded.
        /// </summary>
        public string? EnvelopeId { get; set; }
        /// <summary>
        ///  URL of the file which is to be uploaded.
        /// </summary>
        public string? DownloadUrl { get; set; }
        /// <summary>
        /// This will set Access Token.
        /// </summary>
        public string? AccessToken { get; set; }
        /// <summary>
        /// This will identify envelope is in which stage - InitializeEnvelope/PrepareEnvelope/InitializeDraft/PrepreaDraft/InitializeTemplate/EditTemplate etc.
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// This will set update Document ID.
        /// </summary>
        public string? UpdatedDocumentId { get; set; }

        /// <summary>
        /// This will set CustomerId
        /// </summary>
        public string? CustomerId { get; set; }

        /// <summary>
        /// This will set the DocumentId
        /// </summary>
        public string? DocumentId { get; set; }
        /// <summary>
        /// This will set FolderId
        /// </summary>
        public string? FolderId { get; set; }

        public string? IsEnableFileReview { get; set; }
        public string? IntegrationType { get; set; }
        public string? Version { get; set; }
    }
    public class DocumentDetails
    {
        /// <summary>
        /// This will return Document Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// This will return Document Name.
        /// </summary>
        public string? DocumentName { get; set; }
        /// <summary>
        /// This will return Envelope Id or Template Id.
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// This will return order in which document was uploaded.
        /// </summary>
        public short? Order { get; set; }
        /// <summary>
        /// This will return document uploaded date and time.
        /// </summary>
        public DateTime UploadedDateTime { get; set; }
        /// <summary>
        /// Read Document File Size
        /// </summary>
        public string? FileSize { get; set; }
        /// <summary>
        /// This will return document content details.
        /// </summary>
        public List<DocumentContentDetails> documentContentDetails { get; set; }
        /// <summary>
        /// This will return document is stored value.
        /// </summary>
        public bool? IsDocumentStored { get; set; }

        public string? DocumentSource { get; set; }

        public string? ActionType { get; set; }
        /// <summary>
        /// This will return Envelope Document Delete Data.
        /// </summary>
        public EnvelopeDocumentDeleteData envelopeDocumentDeleteData { get; set; }

        public Guid? TemplateID { get; set; }

        public string? DocSourceValue { get; set; }

        public string? SourceLocation { get; set; }
        public string? SourceDocumentId { get; set; }

    }
    public class DocumentContentDetails
    {
        public DocumentContentDetails()
        {
            //ConditionalControlsDetails = new ConditionalControlsDetails();
            ConditionalControlsDetails = new ConditionalControlsDetailsNew();
        }
        /// <summary>
        /// This will return Html data of control.
        /// </summary>
        public string? ControlHtmlData { get; set; }
        /// <summary>
        /// This will return Html Id of control.
        /// </summary>
        public string? ControlHtmlID { get; set; }
        /// <summary>
        /// Get/Set the Integration Control html id passed by user        
        /// </summary>        
        public string? IntControlId { get; set; }
        /// <summary>
        /// This will return Contol Id.
        /// </summary>
        public Guid ControlID { get; set; }
        /// <summary>
        /// This will return Document Id.
        /// </summary>
        public Guid DocumentID { get; set; }
        /// <summary>
        /// This will return Group name.
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// This will return height of the control.
        /// </summary>
        public int? Height { get; set; }
        /// <summary>
        /// This will return Control Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// This will return label text
        /// </summary>
        public string? Label { get; set; }
        /// <summary>
        /// This will return page number for the control.
        /// </summary>
        public int? PageNo { get; set; }
        /// <summary>
        /// This will return document page number for the control.
        /// </summary>
        public int? DocumentPageNo { get; set; }
        /// <summary>
        /// This will return Recipient Id.
        /// </summary>
        public Guid? RecipientID { get; set; }
        /// <summary>
        /// This will return Recipient Id.
        /// </summary>
        public Guid? MappedTemplateControlID { get; set; }
        /// <summary>
        /// This will return flag whether control is required or not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// Get or Set Control Value Of Sender.
        /// </summary>
        public string? SenderControlValue { get; set; }
        /// <summary>
        /// This will return width of the control.
        /// </summary>
        public int? Width { get; set; }
        /// <summary>
        /// This will return X Coordinate of the control.
        /// </summary>
        public int? XCoordinate { get; set; }
        /// <summary>
        /// This will return Y Coordinate of the control.
        /// </summary>
        public int? YCoordinate { get; set; }
        /// <summary>
        /// This will return Z Coordinate of the control.
        /// </summary>
        public int? ZCoordinate { get; set; }
        /// <summary>
        /// This will return the provided control value.
        /// </summary>
        public string? ControlValue { get; set; }
        /// <summary>
        /// This will return Recipient name.
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// This will return maximum length of the control text.
        /// </summary>
        public string? MaxLength { get; set; }
        /// <summary>
        /// This will return text type of the control.
        /// </summary>
        public string? TextType { get; set; }
        /// <summary>
        /// This will return control style details.
        /// </summary>
        public List<ControlStyleDetails> controlStyleDetails { get; set; }
        /// <summary>
        /// This will return control style.
        /// </summary>
        public ControlStyleDetails ControlStyle { get; set; }
        /// <summary>
        /// This will return select control option details.
        /// </summary>
        public List<SelectControlOptionDetails> SelectControlOptions { get; set; }
        /// <summary>
        /// This will return conditional Rule.
        /// </summary>
        //public List<ChildCondition> ChildCondition { get; set; }
        public List<DependentFieldsPOCO> DependentFields { get; set; }
        /// <summary>
        /// This will return left index of the control.
        /// </summary>
        public double? LeftIndex { get; set; }
        /// <summary>
        /// This will return Top index of the control.
        /// </summary>
        public double? TopIndex { get; set; }
        /// <summary>
        /// Get/Set the Orignal Page Format (Landscape or portrait)
        /// </summary>
        public string? OrignalPageFormat { get; set; }
        /// <summary>
        /// get/set Size of the control (Small,Standard,Large,ExtraLarge)
        /// </summary>
        public string? Size { get; set; }
        /// <summary>
        /// Get/Set controls child conditional control and parent with rules if any.
        /// </summary>
        //public ConditionalControlsDetails ConditionalControlsDetails { get; set; }
        public ConditionalControlsDetailsNew ConditionalControlsDetails { get; set; }
        /// <summary>
        /// Get/Set the ControlType
        /// </summary>
        public Guid? ControlType { get; set; }
        /// <summary>
        /// Get/Set the IsDefaultRequired
        /// </summary>
        public bool? IsDefaultRequired { get; set; }
        /// <summary>
        /// Get/Set the CustomToolTip
        /// </summary>
        public string? CustomToolTip { get; set; }
        /// <summary>
        /// Get/Set the TabIndex
        /// </summary>
        public int? TabIndex { get; set; }

        /// <summary>
        /// Get/Set the FontTypeMeasurement
        /// </summary>
        public string? FontTypeMeasurement { get; set; }
        /// <summary>
        /// Get/Set the SignatureControlValue
        /// </summary>
        public byte[] SignatureControlValue { get; set; }
        public Nullable<bool> IsFixedWidth { get; set; }

    }
    public class ControlStyleDetails
    {
        /// <summary>
        /// This will set selected font id 
        /// </summary>
        public Guid FontID { get; set; }
        /// <summary>
        /// This will return Font Name.
        /// </summary>
        public string? FontName { get; set; }
        /// <summary>
        /// This will return Font Size.
        /// </summary>
        public byte FontSize { get; set; }
        /// <summary>
        /// This will return Font Color.
        /// </summary>
        public string? FontColor { get; set; }
        /// <summary>
        /// This will return flag whether font is bold or not.
        /// </summary>
        public bool IsBold { get; set; }
        /// <summary>
        /// This will return flag whether font is italic or not.
        /// </summary>
        public bool IsItalic { get; set; }
        /// <summary>
        /// This will return flag whether font is underlined or not.
        /// </summary>
        public bool IsUnderline { get; set; }
        public string? AdditionalValidationName { get; set; }
        public string? AdditionalValidationOption { get; set; }
    }
    public class SelectControlOptionDetails
    {
        /// <summary>
        /// This will return Document Content Id.
        /// </summary>
        public Guid DocumentContentID { get; set; }
        /// <summary>
        /// This will return Select Option Id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// This will return option text.
        /// </summary>
        public string? OptionText { get; set; }
        /// <summary>
        /// This will return order of select options.
        /// </summary>
        public int Order { get; set; }
    }
    public class ResponseMessageDocument
    {
        public ResponseMessageDocument()
        {
            SelectOptions = new List<SelectControlOptionDetails>();
        }
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
        /// This will return  Document Id .
        /// </summary>
        public string? DocumentId { get; set; }
        /// <summary>
        /// Get/Set control html id for which request is made
        /// </summary>
        public string? ControlHtmlID { get; set; }
        /// <summary>
        /// This will return  Document Content Id.
        /// </summary>
        public string? DocumentContentId { get; set; }
        /// <summary>
        /// Get dropdown controls select option details
        /// </summary>
        public List<SelectControlOptionDetails> SelectOptions { get; set; }
    }
    public class ResponseMessageDocumentOnly
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
        /// This will return Total Documents for Envelope.
        /// </summary>
        public int DocumentCount { get; set; }
    }
    public class ResponseMessageEnvelopeDocuments
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
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// This will return all documents count in an envelope
        /// </summary>
        public int DocumentCount { get; set; }
        /// <summary>
        /// This will return list of documents from envelope
        /// </summary>
        public List<DocumentDetails> DocumentDetails { get; set; }
    }
    public class DocumentStatus
    {
        /// <summary>
        /// This will return document uploaded date and time.
        /// </summary>
        public string? StatusDate { get; set; }
        /// <summary>
        /// This will return envelope ID.
        /// </summary>
        public Guid StatusID { get; set; }
        /// <summary>
        /// This will return Signer's status .
        /// </summary>
        public string? SignerStatusDescription { get; set; }
        /// <summary>
        /// This will return Recipient's name.
        /// </summary>
        public string? Recipient { get; set; }
        /// <summary>
        /// This will return Recipient's Email Address.
        /// </summary>
        public string? RecipientEmailAddress { get; set; }
        /// <summary>
        /// This will return Recipient's IP Address.
        /// </summary>
        public string? IPAddress { get; set; }
        /// <summary>
        /// This will return RecipientTypeID.
        /// </summary>
        public Guid RecipientTypeID { get; set; }
        /// <summary>
        /// This will return RecipientTypeID.
        /// </summary>
        public Guid RecipientId { get; set; }
        /// <summary>
        /// This will return Delegated To.
        /// </summary>
        public string? DelegatedTo { get; set; }
        /// <summary>
        /// This will return Recipient History .
        /// </summary>
        public List<RecipientsDetailsAPI> RecipientHistory { get; set; }
        /// <summary>
        /// This will return Sender name .
        /// </summary>
        public string? senderName { get; set; }
        /// <summary>
        /// This will return order
        /// </summary>
        public int? Order { get; set; }
    }
    public class DraftedDocuments
    {
        public Guid DocID { get; set; }
        public string? DocName { get; set; }
        public string? FileSize { get; set; }
        public short? DocOrder { get; set; }
        public string? DocumentSource { get; set; }
        public string? ActionType { get; set; }
    }

    public class DocumentMappingForRule
    {
        /// <summary>
        /// Get Increment No
        /// </summary>
        public int IncrementNo { get; set; }
        /// <summary>
        /// Get Envelope Document Id
        /// </summary>
        public Guid EnvelopeDocID { get; set; }
        /// <summary>
        /// Get Document Name
        /// </summary>
        public string? DocName { get; set; }
        /// <summary>
        /// Get Page No
        /// </summary>
        public int EnvDocPageNo { get; set; }
        /// <summary>
        /// Get Document Order
        /// </summary>
        public int DocOrder { get; set; }
    }
    public class ConditionalControlsDetails
    {
        public ConditionalControlsDetails()
        {
            ChildCondition = new List<ChildCondition>();
        }
        /// <summary>
        /// Get and Set Envelope Id
        /// </summary>
        public Guid EnvelopeId { get; set; }
        /// <summary>
        /// Get and Set Parent Control Id
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Get/Set rule id for parent control
        /// </summary>
        public Guid? RuleIDFromParent { get; set; }
        /// <summary>
        /// Get/Set Specific text for parent rule was set to 'Specific Text' rule of rule control
        /// </summary>
        public string? SpecificText { get; set; }
        /// <summary>
        /// Get/Set envelope stage.
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// Get and Set List of Child condition
        /// </summary>
        public List<ChildCondition> ChildCondition { get; set; }
    }
    public class ChildCondition
    {
        /// <summary>
        /// Get and Set Document Control Id
        /// </summary>
        public Guid DocumentControlId { get; set; }
        /// <summary>
        /// Get and Set Rule
        /// </summary>
        public Guid? RuleId { get; set; }
        /// <summary>
        /// Get and Set Specific Text for child control
        /// </summary>
        public string? SpecificText { get; set; }
    }
    /// <summary>
    /// This will return response message.
    /// </summary>
    public class ConditionalControlMappingResponseMessage
    {
        public ConditionalControlMappingResponseMessage()
        {
            ChildCondition = new List<ChildCondition>();
            //ConditionalControlMapping = new ConditionalControlsDetails();
            ConditionalControlDetails = new ConditionalControlsDetailsNew();
        }
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return response message for corresponding  status code.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return additional message for api response.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Get Parent Id
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Get List of Child condition
        /// </summary>
        public List<ChildCondition> ChildCondition { get; set; }
        /// <summary>
        /// Get Mapping
        /// </summary>
        //public ConditionalControlsDetails ConditionalControlMapping { get; set; }

        public ConditionalControlsDetailsNew ConditionalControlDetails { get; set; }

    }
    /// <summary>
    /// This entity contains the Dependent Field properties
    /// </summary>
    public class DependentFieldsPOCO
    {
        /// <summary>
        /// Get/Set transaction entity id
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Get and Set ControlID of child conditions
        /// </summary>
        public Guid ControlID { get; set; }
        /// <summary>
        /// Get and Set condition id of child control
        /// </summary>
        public Guid? ConditionID { get; set; }
        /// <summary>
        /// Get/Set support text for text control condition e.g Specific Text, Contains Text
        /// </summary>
        public string? SupportText { get; set; }
        /// <summary>
        /// Get/Set Group Code of ControlID
        /// </summary>
        public decimal? GroupCode { get; set; }
        /// Get/Set IsRequired field of ControlID
        /// </summary>
        public bool IsRequired { get; set; }
    }
    /// <summary>
    /// This entity contains controls Controlling Field property and Dependent Fields properties
    /// </summary>
    public class ConditionalControlsDetailsNew
    {
        /// <summary>
        /// Empty initialization of ChildConditions
        /// </summary>
        public ConditionalControlsDetailsNew()
        {
            DependentFields = new List<DependentFieldsPOCO>();
        }
        /// <summary>
        /// Get/Set transaction entity id
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Get/Set Envelope ID of ControlID belongs to
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// Get/Set the control id
        /// </summary>
        public Guid ControlID { get; set; }
        /// <summary>
        /// Get/Set parent id of the current control if any
        /// </summary>
        public Guid? ControllingFieldID { get; set; }
        /// <summary>
        /// Get/Set parents condition id of the current control if any
        /// </summary>
        public Guid? ControllingConditionID { get; set; }
        /// <summary>
        /// Get/Set support text for text control condition e.g Specific Text, Contains Text
        /// </summary>
        public string? ControllingSupportText { get; set; }
        /// <summary>
        /// Get/Set the current stage of envelope
        /// </summary>
        public string? EnvelopeStage { get; set; }
        /// <summary>
        /// Get/Set child conditions of ControlID
        /// </summary>
        public List<DependentFieldsPOCO> DependentFields { get; set; }
        /// <summary>
        /// Get/Set Group Code of ControlID
        /// </summary>
        public decimal? GroupCode { get; set; }
        /// <summary>
        /// Get/Set IsRequired field of ControlID
        /// </summary>
        public string? IsRequired { get; set; }
    }
    /// <summary>
    /// This entity will be used for API
    /// </summary>
    public class ConditionalControlResponse
    {
        public ConditionalControlResponse()
        {
            ConditionalControlsDetails = new ConditionalControlsDetailsNew();
        }
        /// <summary>
        /// Get/Set Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  Get/Set Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// Get/Set response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Get/Set Conditional Controls with the dependent fields and the controliing fields
        /// </summary>
        public ConditionalControlsDetailsNew ConditionalControlsDetails { get; set; }
    }
    /// <summary>
    /// This entity will be used for API
    /// </summary>
    public class ControllingControlResponse
    {
        public ControllingControlResponse()
        {
            ControllingField = new DependentFieldsPOCO();
        }
        /// <summary>
        /// Get/Set Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  Get/Set Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// Get/Set response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Get/Set Conditional Controls with the dependent fields and the controliing fields
        /// </summary>
        public DependentFieldsPOCO ControllingField { get; set; }
    }
    public class DocumentBasicInfo
    {
        /// <summary>
        /// This will return document Id.
        /// </summary>
        public Guid DocumentId { get; set; }
        /// <summary>
        /// This will return Document Name.
        /// </summary>
        public string? DocumentName { get; set; }
        /// <summary>
        /// This will return Document Order.
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// This will return document content details.
        /// </summary>
        public List<DocumentContentBasicInfo> documentContentDetails { get; set; }
    }
    public class DocumentContentBasicInfo
    {
        /// <summary>
        /// This will return Html Id of control.
        /// </summary>
        public Guid ControlID { get; set; }
        /// <summary>
        /// This will return name of control.
        /// </summary>
        public string? ControlName { get; set; }
        /// <summary>
        /// This will return control id which is unique for selected template.
        /// </summary>
        public string? ControlHtmlID { get; set; }
        /// <summary>
        /// Get/Set the Integration Control html id passed by user        
        /// </summary>
        [System.Runtime.Serialization.IgnoreDataMember]
        public string? IntControlId { get; set; }
        /// <summary>
        /// This will return GroupName
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// This will return label text
        /// </summary>
        public string? Label { get; set; }
        /// <summary>
        /// This will return page number for the control.
        /// </summary>
        public int? PageNo { get; set; }
        /// <summary>
        /// This will return flag whether control is required or not.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// This will return the Recipients provided control value.
        /// </summary>
        public string? ControlValue { get; set; }
        /// <summary>
        /// This will return role id associted with control.
        /// </summary>
        public Guid? RoleId { get; set; }
        /// <summary>
        /// This will return role name associted with control.
        /// </summary>
        public string? RoleName { get; set; }
        /// <summary>
        /// This will return drop down Option
        /// </summary>
        public List<SelectControlOptionsPOCO> SelectControlOptions { get; set; }
    }
    public class SelectControlOptionsPOCO
    {
        public System.Guid ID { get; set; }
        public string? OptionText { get; set; }
        public byte Order { get; set; }
    }
    public class ControlPositionPOCO
    {
        public Guid EnvelopeID { get; set; }
        public string? EnvelopeStage { get; set; }
        public string? Type { get; set; }
        public List<ControlPositions> Controls { get; set; }
    }
    public class ControlPositions
    {
        public Guid ID { get; set; }
        public int? XCoordinate { get; set; }
        public int? YCoordinate { get; set; }
        public int? ZCoordinate { get; set; }
        public double? Left { get; set; }
        public double? Top { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
    }
    public class ResponseMessageDownload
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Get Filename to download
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// Get Filepath to download
        /// </summary>
        public string? FilePath { get; set; }
        /// <summary>
        /// File bytes
        /// </summary>
        public byte[] FileBytes { get; set; }
        /// <summary>
        /// Get/Set Culture Info
        /// </summary>
        public string? CultureInfo { get; set; }

        public string? Base64FileData { get; set; }
    }
    public class ResponseMessageReject
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Get/Set recipient order
        /// </summary>
        public int? RecipientOrder { get; set; }
        /// <summary>
        /// Get/Set Enable Post Signing Login Popup
        /// </summary>
        public bool EnablePostSigningLoginPopup { get; set; }
        /// <summary>
        /// Get/Set Email Address
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// Get/Set Recipient Name
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// Get/Set Envelope Code
        /// </summary>
        public string? EnvelopeCode { get; set; }
        /// <summary>
        /// Get/Set Culture Info
        /// </summary>
        public string? CultureInfo { get; set; }
        /// <summary>
        /// Get/Set sender email
        /// </summary>
        public string? SenderEmail { get; set; }
        /// <summary>
        /// Get/Set post sign URL
        /// </summary>
        public bool IsNonRegisteredUser { get; set; }
        public string? EncryptedSender { get; set; }
        public bool? IsEnvelopePurging { get; set; }
        public Guid EnvelopeStatus { get; set; }
        public Guid? AttachSignedPdfID { get; set; }
    }
    public class ChangeSignerRequest
    {
        public string? SignerName { get; set; }
        public string? SignerEmail { get; set; }
        public Guid EnvelopeID { get; set; }
        public Guid CurrentRecipientID { get; set; }
        public List<Guid>? RecipientID { get; set; }
        public string? UserEmailAddress { get; set; }
        public string? IPAddress { get; set; }
        public Guid? ControlId { get; set; }
        public string? SignerComments { get; set; }
        public string? CultureInfo { get; set; }
        public bool SendFinalSignDocumentChangeSigner { get; set; }
        public string DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string MobileNumber { get; set; }
        public string CountryCode { get; set; }
    }
    public class ResponseMessageUploadIndependentDocument
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        public long DocumentKey { get; set; }
    }
    public class ResponseMessageUploadDocument
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }

        public bool Success { get; set; }

        public string? UploadedFileName { get; set; }
        public List<DocumentUploadFilesResult> DocumentDetails { get; set; }
        public List<string> Files { get; set; }
        public string? EncryptedEnvelopeID { get; set; }
    }
    public class DocumentUploadFilesResult
    {

        /// <summary>
        /// This will return  Envelope Id.
        /// </summary>
        public string? EnvelopeId { get; set; }
        public string? ID { get; set; }
        public string? name { get; set; }
        public long size { get; set; }
        public string? showFileSize { get; set; }
        public string? type { get; set; }
        public string? url { get; set; }
        public string? delete_url { get; set; }
        public string? thumbnail_url { get; set; }
        public string? delete_type { get; set; }
        public int DisplayOrder { get; set; }
        public string? sourceName { get; set; }
        public string? sourceLocation { get; set; }
        public string? SourceDocumentId { get; set; }

        public string? ActionType { get; set; }

    }
    public class DeleteDocumentRequest
    {
        public Guid EnvelopeID { get; set; }
        public Guid RecipientID { get; set; }
        public string? FileName { get; set; }
        public int? UploadAttachmentId { get; set; }
        public string? TempRecipientID { get; set; }
    }

    public class AddUpdatedAttachmentDetails
    {
        public string? AddUpdate { get; set; }
        public int? AttachmentId { get; set; }
        public string? FileName { get; set; }
        public string? OriginalFileName { get; set; }
        public Guid RecipientId { get; set; }
    }
    public class DeleteInviteByEmailUserDocumentRequest
    {
        public Guid EnvelopeID { get; set; }
        public string? RecipientIds { get; set; }
        public Guid TempRecipientId { get; set; }
    }

    public class EnvelopeSettingRequest
    {
        public string? EnvelopeId { get; set; }
        public string? Html { get; set; }
    }
    public class DownloadCurrentPDFResponse
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Get/Set Return URL
        /// </summary>
        public string? DownloadURL { get; set; }
    }
    public class BulkUploadEntity
    {
        /// <summary>
        /// This will return Trans ID.
        /// </summary>
        public long TransId { get; set; }
        /// <summary>
        /// This will return Template Code.
        /// </summary>
        public int TemplateCode { get; set; }
        /// <summary>
        ///  This will return Role Name.
        /// </summary>
        public string? RoleName { get; set; }
        /// <summary>
        /// This will return Role Type.
        /// </summary>
        public string? RoleType { get; set; }
        /// <summary>
        /// This will return  Recipient Name.
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// This will return  Email Addres.
        /// </summary>
        public string? RecipientEmail { get; set; }
        /// <summary>
        /// This will return  Comments.
        /// </summary>
        public string? Comments { get; set; }
        /// <summary>
        /// This will return  MessageTemplateID.
        /// </summary>
        public int? MessageTemplateCode { get; set; }
        /// <summary>
        /// This will return  Subject.
        /// </summary>
        public string? Subject { get; set; }
        /// <summary>
        /// This will return  Subject.
        /// </summary>
        public string? Message { get; set; }

        ///// <summary>
        ///// This will return  FirstName.
        ///// </summary>
        //public string? FirstName { get; set; }

        ///// <summary>
        ///// This will return  LastName.
        ///// </summary>
        //public string? LastName { get; set; }
    }

    public class UploadContactFile
    {
        /// <summary>
        /// Name of the file which is to be uploaded.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        ///  This will set file data in the base 64 format.
        /// </summary>
        public string? DocumentBase64Data { get; set; }

        /// <summary>
        /// This will set or return User Id
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// This will set or return FileType
        /// </summary>
        public string? FileType { get; set; }

        /// <summary>
        /// This will set or return CompanyUserLevel
        /// </summary>
        public string? CompanyUserLevel { get; set; }

        /// <summary>
        /// This will set or return ContactUploadTemplate
        /// </summary>
        public List<ContactUploadTemplate> ContactUploadTemplate { get; set; }

        /// <summary>
        /// This will set or return IsOverWriteAllowed
        /// </summary>
        public string? IsOverWriteAllowed { get; set; }

    }
    public class ContactUploadTemplate
    {
        /// <summary>
        /// This will return JobTitle
        /// </summary>
        public string? JobTitle { get; set; }

        /// <summary>
        /// This will return FullName
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        ///  This will return FirstName.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// This will return LastName.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>   
        /// This will return  EmailId.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// This will return  Company.
        /// </summary>
        public string? Company { get; set; }
    }
    public class ResponseMessageUploadContactDocument
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>  
        public string? Message { get; set; }
        /// <summary>
        /// This will return Success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// This will return ErrorType
        /// </summary>
        public int ErrorType { get; set; }

        /// <summary>
        /// This will return Success
        /// </summary>
        public List<string> Emails { get; set; }
    }

    public class UploadSignatureStringModel
    {
        public string? Base64UploadSignatureString { get; set; }

        public string? ExtType { get; set; }

        public string? FileName { get; set; }
    }
}
