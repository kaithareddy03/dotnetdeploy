namespace RSign.Models.APIModels
{
    /// <summary>
    /// Request Entity param for the Controls API
    /// </summary>
    public class ControlRequestPOCO
    {
        /// <summary>
        /// Get/Set the envelope id of the controls
        /// </summary>
        public Guid EnvelopeID { get; set; }
        /// <summary>
        /// Get/Set the list of controls of the envelope
        /// </summary>
        public List<DocumentContentPOCO> Controls { get; set; }
    }
    /// <summary>
    /// Entity for the control and its properties
    /// </summary>
    public class DocumentContentPOCO
    {
        /// <summary>
        /// Get/Set the primary identification of the control
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Get/Set the control type id
        /// </summary>
        public Guid ControlID { get; set; }
        /// <summary>
        /// Get/Set the document id for the control
        /// </summary>
        public Guid DocumentID { get; set; }
        /// <summary>
        /// Get/Set the recipient id for the control
        /// For Label Control RecipientID will be empty guid "00000000-0000-0000-0000-000000000000"
        /// </summary>
        public Guid RecipientID { get; set; }
        /// <summary>
        /// Get/Set the control html id for control which is unique for each control
        /// For e.g for 1. Sign Control - SignControl{5 digit unique number} - SignControl12345, 2. Name Control - NameControl{5 digit unique number} - NameControl54321
        /// </summary>
        public string? ControlHtmlID { get; set; }
        /// <summary>
        /// Get/Set the Integration Control html id passed by user        
        /// </summary>
        public string? IntControlId { get; set; }
        /// <summary>
        /// Get/Set if the control is required or not
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// Get/Set the page number of control from all documents
        /// </summary>
        public int DocumentPageNumber { get; set; }
        /// <summary>
        /// Get/Set the page number of control from the document
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Get/Set the X Coordinate position from left bottom corner
        /// </summary>
        public int XCoordinate { get; set; }
        /// <summary>
        /// Get/Set the Y Coordinate position from left bottom corner
        /// </summary>
        public int YCoordinate { get; set; }
        /// <summary>
        /// Get/Set the Z Coordinate position from left bottom corner
        /// </summary>
        public int ZCoordinate { get; set; }
        /// <summary>
        /// Get/Set the control value of the control
        /// </summary>
        public string? ControlValue { get; set; }
        /// <summary>
        /// Get/Set the Label of the control
        /// For Text Control Label will be 'Label' set by Sender while adding the text control
        /// For Radio Control Label will be Radio Name
        /// For Others Label will be - 'ControlName assigned to RecipientName'. E.g 'Title control assigned to Jonathan', 'Sign control assigned to Jonathan'
        /// </summary>
        public string? Label { get; set; }
        /// <summary>
        /// Get/Set the height of the control
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Get/Set the width of the control
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Get/Set the group name of control in case of Radio Control
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// Get/Set the control html data for the control to show for signer or sender on browser
        /// </summary>
        public string? ControlHtmlData { get; set; }
        /// <summary>
        /// Get/Set the recipient name assigned for the control
        /// </summary>
        public string? RecipientName { get; set; }
        /// <summary>
        /// Get/Set if the control is deleted
        /// </summary>
        public bool IsControlDeleted { get; set; }
        /// <summary>
        /// Get/Set the control type in case of TextControl either Numeric or Text
        /// For all other control control type will be empty guid i.e "00000000-0000-0000-0000-000000000000"
        /// </summary>
        public Guid ControlType { get; set; }
        /// <summary>
        /// Get/Set the Left position of the control for browser display
        /// </summary>
        public double Left { get; set; }
        /// <summary>
        /// Get/Set the top position of the control for browser display
        /// </summary>
        public double Top { get; set; }
        /// <summary>
        /// Get/Set the senders control value for the control
        /// </summary>
        public string? SenderControlValue { get; set; }
        /// <summary>
        /// Mapping Template Control Id
        /// </summary>
        public Guid? MappedTemplateControlID { get; set; }
        
        /// <summary>
        /// Get/Set the control style of the control
        /// </summary>
        public ControlStylePOCO ControlStyle { get; set; }
        /// <summary>
        /// Get/Set the Control Options for the dropdown control
        /// </summary>
        public List<ControlOptionsPOCO> Options { get; set; }
        /// <summary>
        /// Get/Set the control dependencies
        /// </summary>
        public List<ControlDependencyPOCO> Dependencies { get; set; }
        /// <summary>
        /// Get/Set the Parent height of the control
        /// </summary>
        public int ParentHeight { get; set; }
        /// <summary>
        /// Get/Set the Image Original height of the control
        /// </summary>
        public int ImageOriginalHeight { get; set; }
        /// <summary>
        /// Get/Set the Image height of the control
        /// </summary>
        public int ImageHeight { get; set; }
        /// <summary>
        /// Get/Set the Image Original Width of the control
        /// </summary>
        public int ImageOriginalWidth { get; set; }
        /// <summary>
        /// Get/Set the Image Width of the control
        /// </summary>
        public int ImageWidth { get; set; }
        /// <summary>
        /// Get/Set the Signature Control Value of the control
        /// </summary>
        public byte[] SignatureControlValue { get; set; }

        public List<SelectOption> ControlOptions { get; set; }
        /// <summary>
        /// Get/Set the Orignal Page Format (Landscape or portrait)
        /// </summary>
        public string? OrignalPageFormat { get; set; }
        /// <summary>
        /// Get/Set the Control Size Format (Small or Standard or Large or ExtraLarge)
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Get/Set the IsDefaultRequired
        /// </summary>
        public bool? IsDefaultRequired { get; set; }

        /// <summary>
        /// Get/Set the CustomToolTip
        /// </summary>
        public string? CustomToolTip { get; set; }

        /// <summary>
        /// Get/Set the FontTypeMeasurement
        /// </summary>
        public string? FontTypeMeasurement { get; set; }

    }
    /// <summary>
    /// Entity for the controls dependency
    /// </summary>
    public class ControlDependencyPOCO
    {
        /// <summary>
        /// Get/Set Control ID for which dependency to create
        /// </summary>
        public Guid ControlID { get; set; }
        /// <summary>
        /// Get/Set the controls parent id of the dependency
        /// </summary>
        public Guid ControlParentID { get; set; }
        /// <summary>
        /// Get/Set the condition id of the dependency
        /// </summary>
        public Guid ConditionID { get; set; }
        /// <summary>
        /// Get/Set the condition text for Control - Text and Condition - Specific Text
        /// </summary>
        public string? ConditionText { get; set; }
        /// <summary>
        /// Get/Set the color group code for the dependency
        /// </summary>
        public int GroupCode { get; set; }
    }
    /// <summary>
    /// Entity for the control options in case of Dropdown control
    /// </summary>
    public class ControlOptionsPOCO
    {
        /// <summary>
        /// Get/Set the ID for dropdown control option
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Get/Set the order of item from the list of control options
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Get/Set the option text of dropdown control
        /// </summary>
        public string? OptionText { get; set; }
    }
    /// <summary>
    /// Entity for control styles
    /// </summary>
    public class ControlStylePOCO
    {
        /// <summary>
        /// Get/Set the font id of the control
        /// </summary>
        public Guid FontID { get; set; }
        /// <summary>
        /// Get/Set the font size of the control
        /// </summary>
        public int FontSize { get; set; }
        /// <summary>
        /// Get/Set the font color of the control
        /// </summary>
        public string? FontColor { get; set; }
        /// <summary>
        /// Get/Set if the control style is bold
        /// </summary>
        public bool IsBold { get; set; }
        /// <summary>
        /// Get/Set if the control style is underlined
        /// </summary>
        public bool IsUnderline { get; set; }
        /// <summary>
        /// Get/Set if the control style is italic
        /// </summary>
        public bool IsItalic { get; set; }

        public string? AdditionalValidationName { get; set; }

        public string? AdditionalValidationOption { get; set; }
    }
    /// <summary>
    /// Get Set Control Information
    /// </summary>
    public class ControlInfo
    {
        /// <summary>
        /// This willl return the Template Control ID
        /// </summary>
        public Guid ControlID { get; set; }
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
        public bool? IsRequired { get; set; }
        /// <summary>
        /// hide control
        /// </summary>
        public bool DeactivateControl { get; set; }
        /// <summary>
        /// Get Set Control HTML Id
        /// </summary>
        public string? ControlHtmlId { get; set; }
        /// <summary>
        /// Get Set Recipient ID
        /// </summary>
        public string? RecipientID { get; set; }
        /// <summary>
        /// Get Set Recipient Email
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// Get Set Recipient Name
        /// </summary>
        public string? Name { get; set; }

        public string? CustomToolTip { get; set; }


        /// <summary>
        /// Get/Set the FontTypeMeasurement
        /// </summary>
        public string? FontTypeMeasurement { get; set; }
    }
}
