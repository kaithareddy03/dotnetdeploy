using RSign.Common.Mailer;
using RSign.Models.APIModels;
using RSign.Models.APIModels.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RSign.Common.Helpers.Constants.StatusCode;

namespace RSign.Models.Interfaces
{
    public interface IGenericRepository
    {
        string GetDocumentFolderPath();
        string GetUniqueKey(string keyName, string languageCode);
        string GenerateRandomVerificationCode();
        string GetExpirySoonInDays();
        string GetNewMailTemplate(string mailTemplateName, string cultureInfo, string emailDisclaimer, int DisclaimerLocationId = 3, string type = "");
        string CreateVerificationCodeMailTemplate(Envelope envelope, string imageLogoURl, string fromEmailId, string userName, Recipients signer, string firstName, string mailTemplate, string signerEmail = "", string SigneruserName = "", string VerificationCode = "");
        string AppendFooterText(string mailTemplate, string toAddress, string mailTemplateName, string cultureInfo, string footerType = "FinalContractFooter", string type = "replace");
        void SaveEmailLogsRecord(List<ChilkatHelper.EmailLogs> emailLogs, out string EmailSentType, out int RetryCount);
        void SaveDestinationRecord(Destination destinationRecord);
        void UpdateEnvelopeCommonWebhookTransaction(Envelope envelopeObject, string EventCode, string SenderEmail, string RecipientId = null, string SignerStatusId = null, string DelegatedMessage = "", string NotificationType = "", string IntegrationType = "");
        bool UpdateEnvelopePrefillSigner(Guid envelopeID, string draftType);
        Envelope GetEntity(Guid envelopeID, bool isHistoryRequired = true, int IsEnvelopeArichived = 0);
        Envelope GetEntity(Guid envelopeID);
        Envelope GetEntityHistory(Guid envelopeID);
        Envelope GetEnvelopeRecipients(Guid envelopeID);
        bool UpdateEnvelopeStatus(Guid envelopeId, Guid envelopeStatusId);
        List<MailTemplateNew> GetMailTemplateCode();
        Dictionary<string, string> GetNewMailTemplateList(string mailTemplateName, string[] CultureInfoList, string emailDisclaimer, int DisclaimerLocationId = 3, string type="");
        List<LookupItem> GetpageBasedKeys(string pageName, string languageCode);
        Task<int?> CheckEnvelopeFromArchiveDatabase(string EDisplayCode, Guid EnvelopeId, string securityCode = "");
        List<ArichiveDBCultureInfo> GetCultureInfoFromArchiveDB(string EDisplayCode, Guid EnvelopeId);
        string GetSigningURL(string templateKey, string envelopeID, string recID, string recEmail, string copyEmail = "", bool IsSignerIdentity = false);
        string GetNewSigningURL(string templateKey, string envelopeID, string recID, string recEmail, string copyEmail, string senderEmail, bool? IsSignerIdentitiy, Guid recipientTypeID, bool isNewUrlApplicable);
        Envelope GetEnvelopeById(Guid envelopeID);
        Envelope GetEnvelopeInfo(Guid envelopeID);
        List<string> GetTemplatDocumentNames(Guid templateID);
        Template GetTemplateEntity(Guid templateID);
        Template GetTemplateDetails(Guid templateID);
        Template GetStaticTemplateDetails(Guid templateID);
        int GetMaxEnvelopeCode();       
        bool Save(Envelope envelope);
        void SetInitializeEnvelopeFlag();
        bool Save(EnvelopeStatus envelopeStatus);
        Envelope GetEnvelopeDisClaimerText(Guid envelopeID);
        LanguageKeyTranslationsModel GetLanguageKeyTranslations(TranslationsModel translationsModel);
        ArichiveEnvelopesInfo GetArchivedEnvelope(string EDisplayCode, Guid EnvelopeId, Guid RecipientId = default(Guid), string securityCode = "");

    }
}
