using eSign.Models.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

namespace RSign.Models
{
    public class RSignDbContext : DbContext
    {
        public RSignDbContext(IOptions<AppSettingsConfig> _configuration) : base(GetOptions(_configuration.Value.ConnectionStrings.RSignContext))
        {
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;
        }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<Envelope> Envelope { get; set; }
        public DbSet<Recipients> Recipients { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<DocumentContents> DocumentContents { get; set; }
        public DbSet<EnvelopeHistory> EnvelopeHistory { get; set; }
        public DbSet<EnvelopeContentHistory> EnvelopeContentHistory { get; set; }
        public DbSet<RecipientsHistory> RecipientsHistory { get; set; }
        public DbSet<DocumentsHistory> DocumentsHistory { get; set; }
        public DbSet<vw_LanguageKeyMapping> vw_LanguageKeyMapping { get; set; }
        public DbSet<ConditionalControlMapping> ConditionalControlMapping { get; set; }
        public DbSet<vw_ActiveRecipientWithoutHistory> vw_ActiveRecipientWithoutHistory { get; set; }
        public DbSet<SignerStatus> SignerStatus { get; set; }
        public DbSet<RecipientsDetail> RecipientsDetail { get; set; }
        public DbSet<ApplicationSetting> ApplicationSetting { get; set; }
        public DbSet<vw_MasterlanguageMapping> vw_MasterlanguageMapping { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<RuleConfLanguageMapping> RuleConfLanguageMapping { get; set; }
        public DbSet<FontList> FontList { get; set; }
        public DbSet<SettingsDetail> SettingsDetail { get; set; }
        public DbSet<Control> Control { get; set; }
        public DbSet<ControlStyle> ControlStyle { get; set; }
        public DbSet<SelectControlOptions> SelectControlOptions { get; set; }
        public DbSet<DelegatedControls> DelegatedControls { get; set; }
        public DbSet<NonRpostUser> NonRpostUser { get; set; }
        public DbSet<MaxCharacter> MaxCharacter { get; set; }
        public DbSet<Template> Template { get; set; }
        public DbSet<TemplateRoles> TemplateRoles { get; set; }
        public DbSet<TemplateDocuments> TemplateDocuments { get; set; }
        public DbSet<EnvelopeAdditionalUploadInfo> EnvelopeAdditionalUploadInfo { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<LanguageKeyDetails> LanguageKeyDetails { get; set; }
        public DbSet<TextType> TextType { get; set; }
        public DbSet<SignerSignature> SignerSignature { get; set; }
        public DbSet<EnvelopeSettingsDetail> EnvelopeSettingsDetail { get; set; }
        public DbSet<MailTemplateNew> MailTemplateNew { get; set; }
        public DbSet<EmailQueueRecipients> EmailQueueRecipients { get; set; }
        public DbSet<EmailQueueAttachment> EmailQueueAttachment { get; set; }
        public DbSet<EmailLogs> EmailLogs { get; set; }
        public DbSet<Destination> Destination { get; set; }
        public DbSet<EmailQueue> EmailQueue { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<SettingsExternalIntegration> SettingsExternalIntegration { get; set; }
        public DbSet<EnvelopeContent> EnvelopeContent { get; set; }
        public DbSet<RAppNotificationEvents> RAppNotificationEvents { get; set; }
        public DbSet<RecipientType> RecipientType { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<EnvelopeStatus> EnvelopeStatus { get; set; }
        public DbSet<TemplateDocumentContents> TemplateDocumentContents { get; set; }
        public DbSet<EnvelopeTemplateGroups> EnvelopeTemplateGroups { get; set; }
        public DbSet<DateFormat> DateFormat { get; set; }
        public DbSet<DropdownOptions> DropdownOptions { get; set; }
        public DbSet<SettingsDisplayLanguageMapping> SettingsDisplayLanguageMapping { get; set; }
        public DbSet<DatetoSignedDocNameSettingsOptions> DatetoSignedDocNameSettingsOptions { get; set; }       
        public DbSet<RecipientsDetailAPI> RecipientsDetailAPI { get; set; }
        public DbSet<EnvelopeContractStatus> EnvelopeContractStatus { get; set; }
        public DbSet<TemplateSelectControlOptions> TemplateSelectControlOptions { get; set; }
        public DbSet<TemplateControlStyle> TemplateControlStyle { get; set; }
        public DbSet<TemplateGroupDocumentUpload> TemplateGroupDocumentUpload { get; set; }
        public DbSet<ExpiryType> ExpiryType { get; set; }
        public DbSet<EnvelopeTemplateMapping> EnvelopeTemplateMapping { get; set; }
        public DbSet<vw_SigningInbox> vw_SigningInbox { get; set; }
        public DbSet<EnvelopeFolderMapping> EnvelopeFolderMapping { get; set; }
        public DbSet<TemplateFolderMapping> TemplateFolderMapping { get; set; }
        public DbSet<EnvelopeDocumentDeleteData> EnvelopeDocumentDeleteData { get; set; }
        public DbSet<DeletedEnvelopeFolderHistory> DeletedEnvelopeFolderHistory { get; set; }
        public DbSet<UserPlan> UserPlan { get; set; }
        public DbSet<SigningShortURL> SigningShortURL { get; set; }
        public DbSet<DialingCountryCodes> DialingCountryCodes { get; set; }
        public DbSet<SignerVerificationOTP> SignerVerificationOTP { get; set; }

        [NotMapped]
        public DbSet<ArichiveEnvelopesInfo> ArichiveEnvelopesInfo { get; set; }
        [NotMapped]
        public DbSet<APIRecipientEntityModel> APIRecipientEntityModel { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);                     
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Required since our model is a custom model and not a EF model , must be declared as hasnokey
                      
            modelBuilder.Entity<Customer>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<DelegatedControls>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<Plan>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<RestResponseUserInfo>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<ResultContent>(entity => { entity.HasNoKey(); });
            //modelBuilder.Entity<TemplateControlStyle>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<UserInfo>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<UserRoleType>(entity => { entity.HasNoKey(); });           
            modelBuilder.Entity<EmailQueueAttachment>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<RecipientsDetailAPI>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<ArichiveEnvelopesInfo>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<APIRecipientEntityModel>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<vw_SigningInbox>(entity => { entity.HasNoKey(); });          
            //modelBuilder.Entity<EnvelopeFolderMapping>(entity => { entity.HasNoKey(); });
            //modelBuilder.Entity<TemplateFolderMapping>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<List<object>>(entity => { entity.HasNoKey(); });          

            modelBuilder.Entity<Company>().Ignore(t => t.DomainList);
            modelBuilder.Entity<Recipients>().Ignore(t => t.RecipientHistory);
            modelBuilder.Entity<RestResponseUserInfo>().Ignore(t => t.DicLabelText);
            modelBuilder.Entity<SettingsDisplayLanguageMapping>().Ignore(t => t.SettingsDisplayLanguageMapping1);
            //modelBuilder.Entity<TemplateDocumentContents>().Ignore(t => t.TemplateControlStyle);
            modelBuilder.Entity<TemplateGroups>().Ignore(t => t.TemplateGroups1);
            modelBuilder.Entity<TemplateGroups>().Ignore(t => t.TemplateGroups2);
            modelBuilder.Entity<FontList>().Ignore(t => t.ControlStyle);
            modelBuilder.Entity<ResultContent>().Ignore(t => t.Customer);
            modelBuilder.Entity<ResultContent>().Ignore(t => t.Plan);
            modelBuilder.Entity<UserInfo>().Ignore(t => t.Customer);
            modelBuilder.Entity<UserInfo>().Ignore(t => t.Plan);
            modelBuilder.Entity<UserProfile>().Ignore(t => t.RestResponseUserInfo);
            modelBuilder.Entity<UserProfile>().Ignore(t => t.UserInfo);
            modelBuilder.Entity<UserProfile>().Ignore(t => t.UserRole);
            modelBuilder.Entity<RestResponseUserInfo>().Ignore(t => t.ResultContent);
            modelBuilder.Entity<FontList>().Ignore(t => t.TemplateControlStyle);
            modelBuilder.Entity<Envelope>().Ignore(t => t.SenderDetails);
            modelBuilder.Entity<Recipients>().Ignore(t => t.RecipientsDetail);
            modelBuilder.Entity<RecipientsDetail>().Ignore(t => t.Recipients);
            modelBuilder.Entity<Status>().Ignore(t => t.RecipientsDetail);            
        }

        //public IEnumerable<dynamic> GetDynamicResult(string commandText, params SqlParameter[] parameters)
        //{
        //    // Get the connection from DbContext
        //    var connection = Database.GetDbConnection();

        //    // Open the connection if isn't open
        //    if (connection.State != System.Data.ConnectionState.Open)
        //        connection.Open();

        //    using (var command = connection.CreateCommand())
        //    {
        //        command.CommandText = commandText;
        //        command.Connection = connection;

        //        if (parameters?.Length > 0)
        //        {
        //            foreach (var parameter in parameters)
        //            {
        //                command.Parameters.Add(parameter);
        //            }
        //        }

        //        using (var dataReader = command.ExecuteReader())
        //        {
        //            // List for column names
        //            var names = new List<string>();

        //            if (dataReader.HasRows)
        //            {
        //                // Add column names to list
        //                for (var i = 0; i < dataReader.VisibleFieldCount; i++)
        //                {
        //                    names.Add(dataReader.GetName(i));
        //                }

        //                while (dataReader.Read())
        //                {
        //                    // Create the dynamic result for each row
        //                    var result = new ExpandoObject() as IDictionary<string, object>;

        //                    foreach (var name in names)
        //                    {
        //                        // Add key-value pair
        //                        // key = column name
        //                        // value = column value
        //                        result.Add(name, dataReader[name]);
        //                    }

        //                    yield return result;
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
