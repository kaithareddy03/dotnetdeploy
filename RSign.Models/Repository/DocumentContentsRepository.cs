using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Repository
{
  public class DocumentContentsRepository : IDocumentContentsRepository
  {
    private readonly IOptions<AppSettingsConfig> _configuration;
    //private readonly IGenericRepository _genericRepository;
    public DocumentContentsRepository(IOptions<AppSettingsConfig> configuration)
    {
      _configuration = configuration;
      //_genericRepository = genericRepository;
    }
    public DelegatedControls GetDelegatedControls(Guid recipientId)
    {
      try
      {
        using (var dbContext = new RSignDbContext(_configuration))
        {
          return dbContext.DelegatedControls.Where(d => d.DelegatedRecipientID == recipientId).FirstOrDefault();
        }
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    /// <summary>
    /// Save Document Content object
    /// </summary>
    /// <param name="docContents"> document content object</param>
    /// <returns>true/false</returns>
    public bool Save(DocumentContents docContents)
    {
      try
      {
        using (var dbContext = new RSignDbContext(_configuration))
        {
          if (dbContext.DocumentContents.Any(dc => dc.ID == docContents.ID))
          {
            var documentContents = dbContext.DocumentContents.First(x => x.ID == docContents.ID);
            documentContents.ControlValue = docContents.ControlValue;
            documentContents.OriginalControlValue = docContents.OriginalControlValue;
            documentContents.LastModifiedBy = docContents.LastModifiedBy;
            documentContents.SignatureControlValue = docContents.SignatureControlValue;
            documentContents.Size = docContents.Size;

            // Below If logic is specific to New Signers UI to support expandable controls feature. To update the Width and Height of Text and Name control.
            if (documentContents.ControlID != null && documentContents.ControlID == Constants.Control.Text && !(Convert.ToBoolean(documentContents.IsFixedWidth)))
            {
              string controlContentType = EnvelopeHelper.GetTextType((Guid)documentContents.ControlType);
              if (controlContentType.ToLower() == "text" || controlContentType.ToLower() == "alpbhabet" || controlContentType.ToLower() == "numeric" || controlContentType.ToLower() == "email")
              {
                if (docContents.Height != null && docContents.Height != 0)
                {
                  documentContents.Height = docContents.Height;
                }
                if (docContents.Width != null && docContents.Width != 0)
                {
                  documentContents.Width = docContents.Width;
                }
              }
            }
            if (documentContents.ControlID != null && documentContents.ControlID == Constants.Control.Name && !(Convert.ToBoolean(documentContents.IsFixedWidth)))
            {
              if (docContents.Height != null && docContents.Height != 0)
              {
                documentContents.Height = docContents.Height;
              }
              if (docContents.Width != null && docContents.Width != 0)
              {
                documentContents.Width = docContents.Width;
              }
            }

            dbContext.Entry(documentContents).State = EntityState.Modified;
          }
          else
            dbContext.DocumentContents.Add(docContents);

          dbContext.SaveChanges();
          return true;
        }
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    /// <summary>
    /// Get control style object
    /// </summary>
    /// <param name="DocumentContentId"></param>
    /// <returns></returns>
    public ControlStyle GetControlStyle(Guid DocumentContentId)
    {
      using (var dbContext = new RSignDbContext(_configuration))
      {
        return dbContext.ControlStyle.Where(c => c.DocumentContentId == DocumentContentId).FirstOrDefault();
      }
    }

    /// <summary>
    /// Save control style
    /// </summary>
    /// <param name="controlStyleDetail"></param>
    /// <returns></returns>
    public bool Save(ControlStyle controlStyleDetail)
    {
      using (var dbContext = new RSignDbContext(_configuration))
      {
        ControlStyle controlStyleObject = dbContext.ControlStyle.Where(c => c.DocumentContentId == controlStyleDetail.DocumentContentId).FirstOrDefault();
        if (controlStyleObject == null)
        {
          dbContext.ControlStyle.Add(controlStyleDetail);
        }
        else
        {
          controlStyleObject.FontID = controlStyleDetail.FontID;
          controlStyleObject.FontColor = controlStyleDetail.FontColor;
          controlStyleObject.FontSize = controlStyleDetail.FontSize;
          controlStyleObject.IsBold = controlStyleDetail.IsBold;
          controlStyleObject.IsItalic = controlStyleDetail.IsItalic;

          if (dbContext.Entry(controlStyleObject).State == EntityState.Unchanged)
            dbContext.Entry(controlStyleObject).State = EntityState.Modified;
        }
        dbContext.SaveChanges();
        return true;
      }
    }

    /// <summary>
    /// Get list of dropdown options
    /// </summary>
    /// <param name="DocumentContentId"></param>
    /// <returns></returns>
    public IQueryable<SelectControlOptions> GetSelectControlOption(Guid DocumentContentId)
    {
      using (var dbContext = new RSignDbContext(_configuration))
      {
        List<SelectControlOptions> controlOption = new List<SelectControlOptions>();
        controlOption = dbContext.SelectControlOptions.Where(c => c.DocumentContentID == DocumentContentId).OrderBy(c => c.Order).ToList();
        return controlOption.AsQueryable();
      }
    }

    /// <summary>
    /// Save dropdown options 
    /// </summary>
    /// <param name="selectControlDetail"></param>
    /// <returns></returns>
    public bool Save(SelectControlOptions selectControlDetail)
    {
      using (var dbContext = new RSignDbContext(_configuration))
      {
        SelectControlOptions selectControlObject = dbContext.SelectControlOptions.Where(c => c.ID == selectControlDetail.ID).FirstOrDefault();
        if (selectControlObject == null)
        {
          dbContext.SelectControlOptions.Add(selectControlDetail);
        }
        else
        {
          selectControlObject.OptionText = selectControlDetail.OptionText;
          selectControlObject.Order = selectControlDetail.Order;

          if (dbContext.Entry(selectControlObject).State == EntityState.Unchanged)
            dbContext.Entry(selectControlObject).State = EntityState.Modified;
        }
        dbContext.SaveChanges();
        return true;
      }
    }

    public Control GetControlData(Guid ControlID)
    {
      using (var dbContext = new RSignDbContext(_configuration))
      {
        return dbContext.Control.Where(c => c.ID == ControlID).FirstOrDefault();
      }
    }
    public bool SaveDocumentContent(DocumentContents content, Guid documentID)
    {
      try
      {
        using (var dbContext = new RSignDbContext(_configuration))
        {
          if (dbContext.DocumentContents.Any(dc => dc.ID == content.ID))
          {
            var documentContents = dbContext.DocumentContents.First(x => x.ID == content.ID);
            documentContents.IsControlDeleted = content.IsControlDeleted;
            if (documentContents.ControlID == Constants.Control.DropDown)
            {
              List<SelectControlOptions> selectControlObject = dbContext.SelectControlOptions.Where(sc => sc.DocumentContentID == content.ID).ToList();
              foreach (var item in selectControlObject)
              {
                SelectControlOptions tempContent = new SelectControlOptions();
                tempContent = content.SelectControlOptions.Where(c => c.ID == item.ID).FirstOrDefault();
                if (tempContent != null)
                {
                  item.OptionText = tempContent.OptionText;
                  item.Order = (byte)tempContent.Order;
                }
                if (dbContext.Entry(item).State == EntityState.Unchanged)
                  dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
              }
              foreach (var selectControlOption in content.SelectControlOptions)
              {
                if (!selectControlObject.Any(s => s.ID == selectControlOption.ID))
                {
                  dbContext.SelectControlOptions.Add(selectControlOption);
                }
              }

              foreach (var obj in selectControlObject)
              {
                bool isAnyItemToRemove = false;
                isAnyItemToRemove = !content.SelectControlOptions.Any(x => x.ID == obj.ID);

                if (isAnyItemToRemove)
                {
                  DeleteSelectControlOption(obj.ID);
                  List<ConditionalControlMapping> conditionalControlMapping = dbContext.ConditionalControlMapping.Where(s => s.RuleId == obj.ID).ToList();

                  foreach (var item in conditionalControlMapping)
                  {
                    dbContext.ConditionalControlMapping.Remove(item);
                  }
                }
              }
            }
            else if (documentContents.ControlID != Constants.Control.Checkbox && documentContents.ControlID != Constants.Control.Radio && documentContents.ControlStyle != null)
              dbContext.Entry(documentContents).State = EntityState.Modified;
            dbContext.Entry(documentContents).State = EntityState.Modified;
            dbContext.SaveChanges();
          }
          else
          {
            DocumentContents docContents = new DocumentContents();
            docContents.ID = content.ID;
            docContents.DocumentID = documentID;
            docContents.Label = content.Label;
            docContents.ControlID = content.ControlID;
            docContents.RecipientID = content.RecipientID;
            docContents.ControlHtmlID = content.ControlHtmlID;
            docContents.IntControlId = content.IntControlId;
            docContents.Required = content.Required;
            docContents.SenderControlValue = content.SenderControlValue;
            docContents.DocumentPageNo = content.DocumentPageNo;
            docContents.PageNo = content.PageNo;
            docContents.TemplateDocumentPageNo = content.TemplateDocumentPageNo;
            docContents.TemplatePageNo = content.TemplatePageNo;
            docContents.XCoordinate = content.XCoordinate;
            docContents.YCoordinate = content.YCoordinate;
            docContents.ZCoordinate = content.ZCoordinate;
            docContents.LeftIndex = content.LeftIndex;
            docContents.TopIndex = content.TopIndex;
            docContents.ControlValue = content.ControlValue;
            docContents.OriginalControlValue = content.OriginalControlValue;
            docContents.Height = content.Height;
            docContents.Width = content.Width;
            docContents.GroupName = content.GroupName;
            docContents.ControlHtmlData = content.ControHtmlData;
            docContents.MaxLength = content.MaxLength;
            docContents.ControlType = content.ControlType;
            docContents.SignatureControlValue = content.SignatureControlValue;
            docContents.IsReadOnly = content.IsReadOnly;
            docContents.IsControlDeleted = content.IsControlDeleted;
            docContents.TabIndex = content.TabIndex;
            docContents.OrignalPageFormat = content.OrignalPageFormat;
            docContents.IsDefaultRequired = !string.IsNullOrEmpty(Convert.ToString(content.IsDefaultRequired)) ? content.IsDefaultRequired : false;
            docContents.CustomToolTip = !string.IsNullOrEmpty(Convert.ToString(content.CustomToolTip)) ? content.CustomToolTip : string.Empty;
            docContents.FontTypeMeasurement = !string.IsNullOrEmpty(Convert.ToString(content.FontTypeMeasurement)) ? content.FontTypeMeasurement : "px";
            docContents.IsFixedWidth = content.IsFixedWidth == null ? true : Convert.ToBoolean(content.IsFixedWidth) ? true : false;
            if (content.ControlID == Constants.Control.Checkbox || content.ControlID == Constants.Control.Checkbox)
            {
              docContents.Size = CheckControlSize(content.Size);
              docContents.Height = GetControlSize(docContents.Size);
              docContents.Width = GetControlSize(docContents.Size);
            }
            if (content.RecipientName != null)
            {
              docContents.RecName = content.RecipientName;
            }
            else
            {
              docContents.RecName = content.RecName;
            }
            docContents.RecipientName = content.RecipientName;
            docContents.ControlName = content.ControlName;
            docContents.MappedTemplateControlID = content.MappedTemplateControlID;
            if (content.ControlID == Constants.Control.DropDown)
            {
              if (content.ControlOptions != null)
              {
                foreach (var option in content.ControlOptions)
                {
                  SaveControlOptions(option, docContents.ID);

                }
              }
            }
            else if (docContents.ControlID != Constants.Control.Checkbox && docContents.ControlID != Constants.Control.Radio)
            {
              if (content.ControlStyle != null)
                SaveControlStyle(content.ControlStyle, docContents.ID);
            }
            dbContext.DocumentContents.Add(docContents);

          }
          dbContext.SaveChanges();
          return true;
        }
      }
      catch (Exception ex)
      {
        throw;
      }
    }
    public bool SaveControlStyle(ControlStyle controlStyleDetail, Guid DocumentContentID)
    {
      try
      {
        using (var dbContext = new RSignDbContext(_configuration))
        {
          ControlStyle controlStyleObject = dbContext.ControlStyle.Where(c => c.DocumentContentId == controlStyleDetail.DocumentContentId).FirstOrDefault();
          if (controlStyleObject == null)
          {
            ControlStyle newStyle = new ControlStyle();
            newStyle.DocumentContentId = DocumentContentID;
            newStyle.FontID = controlStyleDetail.FontID;
            newStyle.FontSize = controlStyleDetail.FontSize;
            newStyle.FontColor = controlStyleDetail.FontColor;
            newStyle.IsBold = controlStyleDetail.IsBold;
            newStyle.IsItalic = controlStyleDetail.IsItalic;
            newStyle.IsUnderline = controlStyleDetail.IsUnderline;
            newStyle.AdditionalValidationName = controlStyleDetail.AdditionalValidationName;
            newStyle.AdditionalValidationOption = controlStyleDetail.AdditionalValidationOption;
            dbContext.ControlStyle.Add(newStyle);
          }
          else
          {
            controlStyleObject.FontID = controlStyleDetail.FontID;
            controlStyleObject.FontColor = controlStyleDetail.FontColor;
            controlStyleObject.FontSize = controlStyleDetail.FontSize;
            controlStyleObject.IsBold = controlStyleDetail.IsBold;
            controlStyleObject.IsItalic = controlStyleDetail.IsItalic;
            controlStyleObject.IsUnderline = controlStyleDetail.IsUnderline;
            controlStyleObject.AdditionalValidationName = controlStyleDetail.AdditionalValidationName;
            controlStyleObject.AdditionalValidationOption = controlStyleDetail.AdditionalValidationOption;

            if (dbContext.Entry(controlStyleObject).State == EntityState.Unchanged)
              dbContext.Entry(controlStyleObject).State = EntityState.Modified;
          }
          dbContext.SaveChanges();
        }
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
    public bool SaveControlOptions(SelectControlOptions selectControlDetail, Guid DocumentContentID)
    {
      using (var dbContext = new RSignDbContext(_configuration))
      {
        SelectControlOptions selectControlObject = dbContext.SelectControlOptions.Where(c => c.DocumentContentID == selectControlDetail.DocumentContentID).FirstOrDefault();
        if (selectControlObject == null)
        {
          SelectControlOptions options = new SelectControlOptions();
          options.ID = selectControlDetail.ID;
          options.DocumentContentID = DocumentContentID;
          options.OptionText = selectControlDetail.OptionText;
          options.Order = selectControlDetail.Order;
          dbContext.SelectControlOptions.Add(options);
        }
        else
        {
          selectControlObject.OptionText = selectControlDetail.OptionText;
          selectControlObject.Order = selectControlDetail.Order;

          if (dbContext.Entry(selectControlObject).State == EntityState.Unchanged)
            dbContext.Entry(selectControlObject).State = EntityState.Modified;
        }
        dbContext.SaveChanges();
      }
      return true;
    }

    public bool DeleteSelectControlOption(Guid selectControlId)
    {
      try
      {
        using (var dbContext = new RSignDbContext(_configuration))
        {
          var selectControlOption = dbContext.SelectControlOptions.Where(s => s.ID == selectControlId).FirstOrDefault();
          dbContext.SelectControlOptions.Remove(selectControlOption);
          dbContext.SaveChanges();
        }

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
    public DocumentContents GetEntity(Guid documentId)
    {
      using (var dbContext = new RSignDbContext(_configuration))
      {
        return dbContext.DocumentContents.Where(d => d.ID == documentId).FirstOrDefault();
      }
    }
    public string CheckControlSize(string Size)
    {
      string ControlSize = Constants.SizeDropdownOptions.Standard;
      Size = string.IsNullOrEmpty(Size) ? Constants.SizeDropdownOptions.Standard : Size.Trim();
      switch (Size.Trim())
      {
        case Constants.SizeDropdownOptions.Standard:
        case Constants.SizeDropdownOptions.Small:
        case Constants.SizeDropdownOptions.Large:
        case Constants.SizeDropdownOptions.ExtraLarge:
          ControlSize = Size.Trim(); break;
        default:
          ControlSize = Constants.SizeDropdownOptions.Standard;
          break;

      }
      return ControlSize;
    }
    public int GetControlSize(string Size)
    {
      int height = 18;
      switch (Size)
      {
        case Constants.SizeDropdownOptions.Small: height = 16; break;
        case Constants.SizeDropdownOptions.Standard: height = 18; break;
        case Constants.SizeDropdownOptions.Large: height = 20; break;
        case Constants.SizeDropdownOptions.ExtraLarge: height = 22; break;
        default: height = 18; break;

      }
      return height;
    }
  }
}
