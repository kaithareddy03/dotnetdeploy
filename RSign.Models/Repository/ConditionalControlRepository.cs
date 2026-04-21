using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Common.Helpers;
using RSign.Models.APIModels;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Repository
{
    public class ConditionalControlRepository : IConditionalControlRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        private static IConfiguration _appConfiguration;
        RSignLogger rsignlog = new RSignLogger();
        LoggerModelNew loggerModelNew = new LoggerModelNew();
        public ConditionalControlRepository(IOptions<AppSettingsConfig> configuration, IConfiguration appConfiguration)
        {
            _configuration = configuration;
            _appConfiguration = appConfiguration;
            rsignlog = new RSignLogger(_appConfiguration);
        }

        public ConditionalControlsDetailsNew GetAllConditionalControl(string envelopeStage, Guid envelopeID, Guid controlID, EnvelopeDetails envelopeDetails, List<ConditionalControlMapping> conditionalControlMappings = null, bool checkConditionalControl = true)
        {
            try
            {
                ConditionalControlsDetailsNew conditionalDetails = new ConditionalControlsDetailsNew();
                if (envelopeStage == Constants.String.RSignStage.PrepareDraft || envelopeStage == Constants.String.RSignStage.PrepareEditTemplate || (envelopeStage == Constants.String.RSignStage.PrepareEnvelope && (envelopeDetails == null ? false : Convert.ToBoolean(envelopeDetails.IsEdited)) == true))
                {
                    foreach (var doc in envelopeDetails.DocumentDetails)
                    {
                        var documentContentDetails = doc.documentContentDetails.FirstOrDefault(dc => dc.ID == controlID);
                        if (documentContentDetails == null)
                            continue;
                        conditionalDetails = documentContentDetails.ConditionalControlsDetails;
                        break;
                    }
                    return conditionalDetails;
                }
                else
                {
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        ConditionalControlMapping controlCondition = new ConditionalControlMapping();
                        if (checkConditionalControl)
                        {
                            controlCondition = dbContext.ConditionalControlMapping.FirstOrDefault(m => m.DocumentControlId == controlID);
                        }
                        else if (conditionalControlMappings != null)
                        {
                            controlCondition = conditionalControlMappings.FirstOrDefault(m => m.DocumentControlId == controlID);
                        }

                        // var controlCondition = dbContext.ConditionalControlMapping.FirstOrDefault(m => m.DocumentControlId == controlID);
                        if (controlCondition != null && controlCondition.ID != null)
                        {
                            conditionalDetails.ID = controlCondition.ID;
                            conditionalDetails.ControlID = controlCondition.DocumentControlId;
                            conditionalDetails.ControllingFieldID = controlCondition.ParentId;
                            conditionalDetails.ControllingConditionID = controlCondition.RuleId;
                            conditionalDetails.ControllingSupportText = controlCondition.SpecificText;
                            conditionalDetails.GroupCode = controlCondition.GroupCode;
                        }
                        conditionalDetails.EnvelopeID = envelopeID;
                        conditionalDetails.DependentFields.AddRange(GetDependentConditions(envelopeStage, envelopeID, controlID, envelopeDetails));
                        return conditionalDetails;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DependentFieldsPOCO> GetDependentConditions(string envelopeStage, Guid envelopeID, Guid? controlID, EnvelopeDetails envelopeDetails)
        {
            try
            {
                List<DependentFieldsPOCO> dependentDetails = new List<DependentFieldsPOCO>();
                if (envelopeStage == Constants.String.RSignStage.PrepareDraft || envelopeStage == Constants.String.RSignStage.PrepareEditTemplate || (envelopeStage == Constants.String.RSignStage.PrepareEnvelope && (envelopeDetails == null ? false : Convert.ToBoolean(envelopeDetails.IsEdited)) == true))
                {
                    foreach (var doc in envelopeDetails.DocumentDetails)
                    {
                        var control = doc.documentContentDetails.FirstOrDefault(f => f.ID == controlID);
                        if (control != null)
                        {
                            dependentDetails.AddRange(control.ConditionalControlsDetails.DependentFields);
                            break;
                        }
                    }
                    return dependentDetails;
                }
                else
                {
                    using (var dbContext = new RSignDbContext(_configuration))
                    {
                        dependentDetails = (from m in dbContext.ConditionalControlMapping
                                            where m.EnvelopeId == envelopeID && m.ParentId == controlID
                                            select new DependentFieldsPOCO
                                            {
                                                ID = m.ID,
                                                ControlID = m.DocumentControlId,
                                                ConditionID = m.RuleId,
                                                SupportText = m.SpecificText
                                            }).Distinct().ToList();

                        return dependentDetails;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DependentFieldsPOCO GetControllingFieldOfControl(Guid controlID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    DependentFieldsPOCO controllingField = new DependentFieldsPOCO();
                    var field = dbContext.ConditionalControlMapping.FirstOrDefault(c => c.DocumentControlId == controlID);
                    if (field == null)
                        return controllingField;
                    controllingField.ControlID = field.ParentId.HasValue ? field.ParentId.Value : Guid.Empty;
                    controllingField.ConditionID = field.RuleId;
                    controllingField.SupportText = field.SpecificText;
                    controllingField.ID = field.ID;
                    return controllingField;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveConditionalControlForSigner(ConditionalControlsDetailsNew conditionalControls)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var ownerControlCondition = conditionalControls.ID == Guid.Empty ? null : dbContext.ConditionalControlMapping.FirstOrDefault(f => f.DocumentControlId == conditionalControls.ControlID);
                    if (ownerControlCondition == null)
                    {
                        ConditionalControlMapping conditionalControlMapping = new ConditionalControlMapping();
                        conditionalControlMapping.ID = Guid.NewGuid();
                        conditionalControlMapping.EnvelopeId = conditionalControls.EnvelopeID;
                        conditionalControlMapping.ParentId = conditionalControls.ControllingFieldID;
                        conditionalControlMapping.DocumentControlId = conditionalControls.ControlID;
                        conditionalControlMapping.RuleId = null;
                        conditionalControlMapping.UpdatedOn = DateTime.Now;
                        conditionalControlMapping.SpecificText = null;
                        conditionalControlMapping.GroupCode = conditionalControls.GroupCode;
                        dbContext.ConditionalControlMapping.Add(conditionalControlMapping);
                        dbContext.SaveChanges();
                    }
                    var oldConditionList = dbContext.ConditionalControlMapping.Where(c => c.ParentId == conditionalControls.ControlID).ToList();
                    foreach (var childCond in conditionalControls.DependentFields)
                    {
                        var condition = childCond.ID == Guid.Empty ? null : dbContext.ConditionalControlMapping.FirstOrDefault(f => f.DocumentControlId == childCond.ControlID);
                        if (condition == null)
                        {
                            //New condition to be add
                            ConditionalControlMapping conditionalControlMapping = new ConditionalControlMapping();
                            conditionalControlMapping.ID = Guid.NewGuid();
                            conditionalControlMapping.EnvelopeId = conditionalControls.EnvelopeID;
                            conditionalControlMapping.ParentId = conditionalControls.ControlID;
                            conditionalControlMapping.DocumentControlId = childCond.ControlID;
                            conditionalControlMapping.RuleId = childCond.ConditionID;
                            conditionalControlMapping.UpdatedOn = DateTime.Now;
                            conditionalControlMapping.SpecificText = childCond.SupportText;
                            conditionalControlMapping.GroupCode = null;
                            dbContext.ConditionalControlMapping.Add(conditionalControlMapping);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            //Update old condition     
                            var tempFirst = oldConditionList.FirstOrDefault(f => f.DocumentControlId == childCond.ControlID);
                            if (tempFirst != null)
                                oldConditionList.Remove(tempFirst);
                            condition.ParentId = conditionalControls.ControlID;
                            condition.DocumentControlId = childCond.ControlID;
                            condition.RuleId = childCond.ConditionID;
                            condition.UpdatedOn = DateTime.Now;
                            condition.SpecificText = childCond.SupportText;
                            condition.GroupCode = null;

                            if (dbContext.Entry(condition).State == EntityState.Unchanged)
                                dbContext.Entry(condition).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        var docContent = dbContext.DocumentContents.FirstOrDefault(d => d.ID == childCond.ControlID);
                        docContent.ControlHtmlData = docContent.ControlHtmlData.Replace("data-selected=" + "\"" + Convert.ToString(docContent.Required) + "\"",
                            "data-selected=" + "\"" + Convert.ToString(childCond.IsRequired) + "\"");
                        docContent.Required = childCond.IsRequired;
                        if (dbContext.Entry(docContent).State == EntityState.Unchanged)
                            dbContext.Entry(docContent).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    foreach (var condToDel in oldConditionList)
                    {
                        var delete = dbContext.ConditionalControlMapping.FirstOrDefault(m => m.DocumentControlId == condToDel.DocumentControlId);
                        if (!dbContext.ConditionalControlMapping.Any(a => a.ParentId == delete.DocumentControlId))
                            dbContext.ConditionalControlMapping.Remove(delete);
                        else
                        {
                            delete.ParentId = null;
                            delete.RuleId = null;
                            delete.SpecificText = null;
                            delete.UpdatedOn = DateTime.Now;
                            if (dbContext.Entry(delete).State == EntityState.Unchanged)
                                dbContext.Entry(delete).State = EntityState.Modified;
                        }
                        dbContext.SaveChanges();
                    }

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveConditionalControl(ConditionalControlsDetailsNew conditionalControls)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    var ownerControlCondition = conditionalControls.ID == Guid.Empty ? null : dbContext.ConditionalControlMapping.FirstOrDefault(f => f.DocumentControlId == conditionalControls.ControlID);
                    if (ownerControlCondition == null)
                    {
                        ConditionalControlMapping conditionalControlMapping = new ConditionalControlMapping();
                        conditionalControlMapping.ID = Guid.NewGuid();
                        conditionalControlMapping.EnvelopeId = conditionalControls.EnvelopeID;
                        conditionalControlMapping.ParentId = conditionalControls.ControllingFieldID;
                        conditionalControlMapping.DocumentControlId = conditionalControls.ControlID;
                        conditionalControlMapping.RuleId = null;
                        conditionalControlMapping.UpdatedOn = DateTime.Now;
                        conditionalControlMapping.SpecificText = null;
                        conditionalControlMapping.GroupCode = conditionalControls.GroupCode;
                        dbContext.ConditionalControlMapping.Add(conditionalControlMapping);
                        dbContext.SaveChanges();
                    }
                    var oldConditionList = dbContext.ConditionalControlMapping.Where(c => c.ParentId == conditionalControls.ControlID).ToList();
                    foreach (var childCond in conditionalControls.DependentFields)
                    {
                        var condition = childCond.ID == Guid.Empty ? null : dbContext.ConditionalControlMapping.FirstOrDefault(f => f.DocumentControlId == childCond.ControlID);
                        if (condition == null)
                        {//New condition to be add
                            ConditionalControlMapping conditionalControlMapping = new ConditionalControlMapping();
                            conditionalControlMapping.ID = Guid.NewGuid();
                            conditionalControlMapping.EnvelopeId = conditionalControls.EnvelopeID;
                            conditionalControlMapping.ParentId = conditionalControls.ControlID;
                            conditionalControlMapping.DocumentControlId = childCond.ControlID;
                            conditionalControlMapping.RuleId = childCond.ConditionID;
                            conditionalControlMapping.UpdatedOn = DateTime.Now;
                            conditionalControlMapping.SpecificText = childCond.SupportText;
                            conditionalControlMapping.GroupCode = null;
                            dbContext.ConditionalControlMapping.Add(conditionalControlMapping);
                            dbContext.SaveChanges();
                        }
                        else
                        {//Update old condition     
                            var tempFirst = oldConditionList.FirstOrDefault(f => f.DocumentControlId == childCond.ControlID);
                            if (tempFirst != null)
                                oldConditionList.Remove(tempFirst);
                            condition.ParentId = conditionalControls.ControlID;
                            condition.DocumentControlId = childCond.ControlID;
                            condition.RuleId = childCond.ConditionID;
                            condition.UpdatedOn = DateTime.Now;
                            condition.SpecificText = childCond.SupportText;
                            condition.GroupCode = null;

                            if (dbContext.Entry(condition).State == EntityState.Unchanged)
                                dbContext.Entry(condition).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        var docContent = dbContext.DocumentContents.FirstOrDefault(d => d.ID == childCond.ControlID);
                        //docContent.ControlHtmlData = docContent.ControlHtmlData.Replace("data-selected=" + "\"" + Convert.ToString(docContent.Required) + "\"",
                        //    "data-selected=" + "\"" + Convert.ToString(childCond.IsRequired) + "\"");
                        //docContent.Required = childCond.IsRequired;
                        if (dbContext.Entry(docContent).State == EntityState.Unchanged)
                            dbContext.Entry(docContent).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    foreach (var condToDel in oldConditionList)
                    {
                        var delete = dbContext.ConditionalControlMapping.FirstOrDefault(m => m.DocumentControlId == condToDel.DocumentControlId);
                        if (!dbContext.ConditionalControlMapping.Any(a => a.ParentId == delete.DocumentControlId))
                            dbContext.ConditionalControlMapping.Remove(delete);
                        else
                        {
                            delete.ParentId = null;
                            delete.RuleId = null;
                            delete.SpecificText = null;
                            delete.UpdatedOn = DateTime.Now;
                            if (dbContext.Entry(delete).State == EntityState.Unchanged)
                                dbContext.Entry(delete).State = EntityState.Modified;
                        }
                        dbContext.SaveChanges();
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
