using RSign.Models.APIModels;
using RSign.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IEnvelopeRepository
    {
        //Envelope GetEntity(Guid envelopeID);
        //Envelope GetEnvelopeRecipients(Guid envelopeID);
        //Envelope GetEnvelopeEntity(Guid envelopeID);
        bool IsEnvelopeExist(string ID);
        bool IsEnvelopeHistoryExist(string ID);
        EnvelopeDBResponse InsertHistoryEnvelopeToPrimary(string envelopeId);
        EnvelopeDetails FillEnvelopeDetailsByEnvelopeEntity(Envelope envelopeObject, List<ConditionalControlMapping> conditionalControlMappingData = null, bool checkconditionalControl = true);
        EnvelopeDetails FillEnvelopeDetailsByEnvelopeId(Envelope envelopeObject);
        List<EnvelopeAdditionalUploadInfoDetails> GetEnvelopeAdditionalUploadInfoByEnvelopebyRecipeint(Guid masterEnvelopeID, Guid RecipientID, string additionalRecipients = null);
        int GetMaxUploadsID();
        EnvelopeSettingsDetail GetEnvelopeSettingsDetail(Guid envelopeID, int IsEnvelopeArichived = 0);
        Task<ErrorResponseModel> UpdateVerificationCodeEmail(UserVerificationModel userVerificationModel);
        EnvelopeAdditionalUploadInfo GetEnvelopeAdditionalUploadInfoByID(int ID, Guid RecipientID);
        bool UpdateEnvelopeAdditionalUploadInfo(EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo);
        bool RemoveEnvelopeAdditionalUploadInfo(EnvelopeAdditionalUploadInfo envelopeAdditionalUploadInfo);
        bool DeleteEnvelopeAdditionalUploadInfo(int uploadInfoId);
        EnvelopeAdditionalUploadInfo GetEnvelopeUploadInfoByID(long ID);
        List<TemplateGroupDocumentUploadDetails> GetEnvelopeAdditionalDocument(Guid masterEnvelopeID);
        void SaveEnvelopeAdditionalUploadInfo(EnvelopeAdditionalUploadInfo newObj);
        bool UpdateEnvelopePrefillSigner(Guid envelopeID, string draftType);
        EnvelopeContent GetEnvelopeContent(Guid envelopeID);
        bool Save(EnvelopeContent envelopeContent);
        List<EnvelopeAdditionalUploadInfoDetailsDelegate> GetEnvelopeAdditionalUploadInfoByDelegate(Guid masterEnvelopeID, Guid RecipientID);
        bool UpdateEnvelopeAdditionalUploadInfobyDelegate(EnvelopeAdditionalUploadInfoDetailsDelegate envelopeAdditionalUploadInfo);
        void UpdateRAppNotificationEvents(Recipients recipientDetail, Envelope envelope);
        bool Save(EnvelopeStatus envelopeStatus);
        bool RemovePrefillEnvelopeFromDrafts(Guid envelopeId);
        List<TemplateGroupDocumentUploadDetails> GetTemplateAdditionalDocument(Guid templateId);
        bool SaveEnvelopeAdditionalAttachment(List<TemplateGroupDocumentUploadDetails> envelopeAttachmentsList, Guid? EnvelopeID);
        List<string> GetEnvelopeAdditionalDocumentName(Guid masterEnvelopeID);
        bool UpdateEnvelopeAdditionalUploadInfoByInviteByEmailUserID(int ID, Guid RecipientID);
        List<DocumentContentDetails> getDocumentContentDetails(List<DocumentContents> documentContents);
        bool UpdateDocumentRequestRecipient(Guid oldRecipientId, Guid newRecipientId, Guid EnvelopeID, string newEmailId);       
        bool SaveEnvelopeTemplateMapping(EnvelopeTemplateMappingDetails mappingDetails);
        bool SaveEnvelopeSettingsDetail(AdminGeneralAndSystemSettings settings, Guid envelopeId);
        List<SignMultipleTemplateDetails> GetSigningInbox(Guid envelopeId);
        List<EnvelopeAdditionalUploadInfoDetails> GetEnvelopeAdditionalUploadInfoByEnvelope(Guid masterEnvelopeID);
        List<ConditionalControlMapping> GetConditionalControlMapping(Guid envelopeId);
        bool SaveEnvelopeFolderMapping(EnvelopeFolderMapping envelopeFolderMapping);
        EnvelopeGetEnvelopeHistoryByEnvelopeCode GetEnvelopeMetaDataWithHistory(Guid EnvelopeId, string EmailID, string userTimeZone, string userName);
       
    }
}
