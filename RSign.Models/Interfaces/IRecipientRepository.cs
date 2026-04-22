using RSign.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IRecipientRepository
    {
        List<RecipientDetails> GetActiveRecipientData(Guid iD, bool loadAllRecipient = false);
        Recipients GetEntity(Guid recipientId);
        SignerStatus GetSignerSignedStatusId(Guid recipientId);
        Guid GetSignerStatusId(Guid iD);
        SignerStatus GetSignerStatus(Guid recipientId);
        bool Save(SignerStatus signerstatus);
        bool SaveRecipientDetail(Guid recipientId, Guid statusId, string IpAddress, string CopyEmailID = "");
        bool Save(Recipients recipient);
        SignerSignature GetSignerSignature(Guid recipientId);
        bool SaveEmailQueue(EmailQueue emailqueue);
        bool SaveEmailQueueRecipients(EmailQueueRecipients emailqueuerecipients);
        bool SaveEmailQueueAttachments(EmailQueueAttachment emailQueueAttachment);
        List<EmailQueue> GetEmailQueueData(Guid EnvelopeID, string EmailType);
        List<vw_ActiveRecipientWithoutHistory> GetEnvelopeSignerRecipientByEmail(Guid envelopeId, string emailAddress);
        Guid GetSignerPrimaryStatusId(Guid recipientId);
        bool ModifyLastRecipientEntry(Guid recipientId, Guid statusId, string IpAddress);
        bool Save(SignerSignature signerDetails);
        bool AddDelegatedSigner(Guid recipientId, Guid statusId, Guid delegatedToId, string IPaddress);
        IQueryable<Recipients> GetAllRecipients(Guid envelopeID);
        Recipients GetSenderDetails(Guid envelopeId);
        bool AddSignerRemark(Guid recipientId, string remark, string IPaddress, int? declineReasonID, string CopyEmail = "");
        int UpdateRecipientsforOutOfOffice(Guid envelopeId);
        IQueryable<Recipients> GetAll(Guid id);
        SignerStatus GetCopySignerStatus(Guid ID);
        byte[] GetInitialSignatureValue(Guid recipientId);
        bool AddSignerStatus(Guid recipientId, Guid statusId, string IPaddress, string copyEmail = "");
        bool AddOrModifyLastRecipientEntry(Guid recipientId, Guid statusId, string IpAddress);
        bool SaveRecipientDetailForEmailConfirm(Guid recipientId, Guid statusId, string IpAddress);
        IQueryable<SignerStatus> GetAllSignerStatus(Guid recipientId);
        bool ModifyLastRecipientSignedEntry(Guid recipientId, Guid statusId, string IpAddress, RSignDbContext dbContext);
        Recipients GetRecipientByCode(string recipientCode);
        Task<APIRecipientEntityModel> GetRecipientEntity(string emailID, Guid envelopeID, Guid? templateKey, string mobile = "");
        bool SaveRecipientDetailOnSend(Guid recipientId, Guid statusId, string IpAddress);
        List<SignerStatus> GetStatusList(List<Guid> recipientIds);
        List<SignerSignature> GetSignerSignatureList(List<Guid> recipientIds);
        List<SignerStatus> GetCopySignerStatusAllrecipients(List<Guid> recipientIds);
        void UpdateIsSendFinalDocumentOnDelegate(Recipients recipientDetai);
        string GetSignerFinalSubmitOTP(string EnvelopeId, string RecipientId);
        bool SaveSignerFinalSubmitOTP(SignerVerificationOTP signerVerification);
    }
}
