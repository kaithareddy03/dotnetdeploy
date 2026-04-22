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
    public class UserRepository : IUserRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        private readonly IGenericRepository _genericRepository;
        public UserRepository(IOptions<AppSettingsConfig> configuration, IGenericRepository genericRepository)
        {
            _configuration = configuration;
            _genericRepository = genericRepository;
        }
        public UserProfile GetUserProfileByUserID(Guid UserID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return dbContext.UserProfile.FirstOrDefault(p => p.UserID == UserID);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public UserProfile GetUserProfileByEmailID(string emailID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                emailID = string.IsNullOrEmpty(emailID) ? emailID : emailID.ToLower();
                return dbContext.UserProfile.FirstOrDefault(u => u.EmailID.ToLower() == emailID && u.ActiveTo == null);
            }
            //return dbContext.UserProfile.FirstOrDefault(u => u.EmailID.ToLower() == emailID);
        }
        public NonRpostUser GetNonRpostUser(string emailId)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.NonRpostUser.FirstOrDefault(u => u.EmailAddress.ToLower() == emailId.ToLower());
            }

        }
        public UserProfile GetLatestUserProfile(string emailID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                emailID = string.IsNullOrEmpty(emailID) ? emailID : emailID.ToLower();
                return dbContext.UserProfile.Where(u => u.EmailID.ToLower() == emailID && u.IsActive == true).OrderByDescending(o => o.ActiveFrom).FirstOrDefault();
            }
        }
        public UserProfile GetUserProfile(Guid userId)
        {
            return GetUserProfile(userId, null, false);
        }
        public UserProfile GetUserProfile(Guid userId, string EmailID, bool isAdditionalInfoRequired)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                UserProfile userProfile = dbContext.UserProfile.FirstOrDefault(u => u.UserID == userId);
                if (userProfile == null)
                    return null;

                if (isAdditionalInfoRequired)
                {
                    userProfile.TotalEnvelopeCount = dbContext.Envelope.Count(e => e.UserID == userId && e.IsEnvelope == true && e.EDisplayCode != null && !string.IsNullOrEmpty(e.EDisplayCode) && e.Subject != null && e.Subject != string.Empty && e.IsEnvelopeComplete == true && e.EnvelopeCode != 0 && e.IsDraft == null);
                    userProfile.CompletedValue = dbContext.Envelope.Count(e => e.StatusID == Constants.StatusCode.Envelope.Completed && e.UserID == userId && e.IsEnvelope == true && e.EDisplayCode != null && !string.IsNullOrEmpty(e.EDisplayCode) && e.Subject != null && e.Subject != string.Empty && e.IsEnvelopeComplete == true && e.EnvelopeCode != 0 && e.IsDraft == null);

                    userProfile.SentforSignatureValue = dbContext.Envelope.Count(e => e.StatusID == Constants.StatusCode.Envelope.Waiting_For_Signature && e.UserID == userId && e.IsEnvelope == true && e.EDisplayCode != null && !string.IsNullOrEmpty(e.EDisplayCode) && e.Subject != null && e.Subject != string.Empty && e.IsEnvelopeComplete == true && e.EnvelopeCode != 0 && e.IsDraft == null);
                    userProfile.Terminated = dbContext.Envelope.Count(e => e.StatusID == Constants.StatusCode.Envelope.Terminated && e.UserID == userId && e.IsEnvelope == true && e.EDisplayCode != null && !string.IsNullOrEmpty(e.EDisplayCode) && e.Subject != null && e.Subject != string.Empty && e.EnvelopeCode != 0 && e.IsEnvelopeComplete == true && e.IsDraft == null);

                    var AfterExpirydays = DateTime.Now.AddDays(double.Parse(_genericRepository.GetExpirySoonInDays()));

                    userProfile.ExpiringSoonValue = (from e in dbContext.Envelope
                                                     where e.UserID == userId && e.IsEnvelope == true && e.StatusID == Constants.StatusCode.Envelope.Waiting_For_Signature && e.ExpiryDate < AfterExpirydays && e.EDisplayCode != null && !string.IsNullOrEmpty(e.EDisplayCode) && e.Subject != null && e.Subject != string.Empty
                                                     && e.IsEnvelopeComplete == true && e.EnvelopeCode != 0 && e.IsDraft == null
                                                     select e).ToList().Where(e => e.ExpiryDate.AddDays(1) > DateTime.Now).Count();

                    int numberOFSignersWithIncomplete_And_Expired = (from e in dbContext.Envelope
                                                                     join r in dbContext.Recipients on e.ID equals r.EnvelopeID
                                                                     join s in dbContext.SignerStatus on r.ID equals s.RecipientID
                                                                     where e.UserID == userId && e.IsEnvelope == true && s.StatusID == Constants.StatusCode.Signer.Incomplete_and_Expired && e.StatusID == Constants.StatusCode.Envelope.Incomplete_and_Expired && !string.IsNullOrEmpty(e.EDisplayCode) && e.EDisplayCode != null && e.Subject != null && e.Subject != ""
                                                                     && e.EnvelopeCode != 0 && e.IsEnvelopeComplete == true && e.IsDraft == null && r.RecipientTypeID == Constants.RecipientType.Signer
                                                                     select s.RecipientID).Distinct().Count();
                    int totalSignersWithIncomplete_And_Expired = (from e in dbContext.Envelope
                                                                  join r in dbContext.Recipients on e.ID equals r.EnvelopeID
                                                                  where e.UserID == userId && e.IsEnvelope == true && e.StatusID == Constants.StatusCode.Envelope.Incomplete_and_Expired && r.RecipientTypeID == Constants.RecipientType.Signer && !string.IsNullOrEmpty(e.EDisplayCode) && e.EDisplayCode != null && e.Subject != null && e.Subject != string.Empty
                                                                 && e.EnvelopeCode != 0 && e.IsEnvelopeComplete == true && e.IsDraft == null
                                                                  select r).Count();
                    if (numberOFSignersWithIncomplete_And_Expired != 0 && totalSignersWithIncomplete_And_Expired != 0)
                    {
                        userProfile.ExpiredValue = double.Parse(string.Format("{0:0.00}", (((double)numberOFSignersWithIncomplete_And_Expired) / ((double)totalSignersWithIncomplete_And_Expired)) * 100));
                    }

                    int numberOfSignersWithAccepted = (from e in dbContext.Envelope
                                                       join r in dbContext.Recipients on e.ID equals r.EnvelopeID
                                                       join s in dbContext.SignerStatus on r.ID equals s.RecipientID
                                                       where e.UserID == userId && e.IsEnvelope == true && s.StatusID == Constants.StatusCode.Signer.Signed && e.EnvelopeCode != 0 && e.IsEnvelopeComplete == true && e.IsDraft == null
                                                       && r.RecipientTypeID == Constants.RecipientType.Signer && !string.IsNullOrEmpty(e.EDisplayCode) && e.EDisplayCode != null && !string.IsNullOrEmpty(e.Subject)
                                                       select s).Count();
                    int totalSigners = (from e in dbContext.Envelope
                                        join r in dbContext.Recipients on e.ID equals r.EnvelopeID
                                        where e.UserID == userId && e.IsEnvelope == true && r.RecipientTypeID == Constants.RecipientType.Signer && !string.IsNullOrEmpty(e.EDisplayCode) && !string.IsNullOrEmpty(e.Subject) && e.EnvelopeCode != 0 && e.IsEnvelopeComplete == true && e.IsDraft == null
                                        select r).Count();
                    if (numberOfSignersWithAccepted != 0 && totalSigners != 0)
                    {
                        userProfile.SignedValue = double.Parse(string.Format("{0:0.00}", (((double)numberOfSignersWithAccepted) / ((double)totalSigners)) * 100));
                    }
                    int numberOfSignersViewed = (from e in dbContext.Envelope
                                                 join r in dbContext.Recipients on e.ID equals r.EnvelopeID
                                                 where e.UserID == userId && e.IsEnvelope == true && r.SignerStatus.OrderByDescending(s => s.CreatedDateTime).Take(1).FirstOrDefault().StatusID == Constants.StatusCode.Signer.Viewed && e.StatusID == Constants.StatusCode.Envelope.Waiting_For_Signature
                                                 && r.RecipientTypeID == Constants.RecipientType.Signer && !string.IsNullOrEmpty(e.EDisplayCode) && e.EDisplayCode != null && !string.IsNullOrEmpty(e.Subject) && e.EnvelopeCode != 0 && e.IsEnvelopeComplete == true && e.IsDraft == null
                                                 select r).Count();

                    int totalSignersWithEnvelopesInWaitingForSignature = (from e in dbContext.Envelope
                                                                          join r in dbContext.Recipients on e.ID equals r.EnvelopeID
                                                                          where e.UserID == userId && e.IsEnvelope == true && e.StatusID == Constants.StatusCode.Envelope.Waiting_For_Signature && r.RecipientTypeID == Constants.RecipientType.Signer
                                                                          && !string.IsNullOrEmpty(e.EDisplayCode) && e.EDisplayCode != null && !string.IsNullOrEmpty(e.Subject) && e.EnvelopeCode != 0 && e.IsEnvelopeComplete == true && e.IsDraft == null
                                                                          select r).Count();


                    if (numberOfSignersViewed != 0 && totalSignersWithEnvelopesInWaitingForSignature != 0)
                    {
                        userProfile.ViewedValue = double.Parse(string.Format("{0:0.00}", (((double)numberOfSignersViewed) / ((double)totalSignersWithEnvelopesInWaitingForSignature)) * 100));
                    }

                    double finaldifferenceHours = 0;
                    var envelopeCompleted = (from e in dbContext.Envelope
                                             where e.UserID == userId && e.IsEnvelope == true && e.StatusID == Constants.StatusCode.Envelope.Completed && !string.IsNullOrEmpty(e.EDisplayCode) && e.EDisplayCode != null && e.Subject != "" && e.IsEnvelopeComplete == true
                                             && e.EnvelopeCode != null && e.IsDraft == null
                                             select e).ToList();

                    var timeDiffer = new TimeSpan();
                    foreach (var Envelope in envelopeCompleted)
                    {
                        timeDiffer = Envelope.ModifiedDateTime - Envelope.CreatedDateTime;
                        finaldifferenceHours += timeDiffer.TotalHours;
                    }
                    if (envelopeCompleted.Count() != 0 && finaldifferenceHours != 0)
                    {
                        userProfile.AverageTimeValue = double.Parse(string.Format("{0:0.00}", finaldifferenceHours / double.Parse(envelopeCompleted.Count().ToString())));
                    }
                }
                return userProfile;
            }
        }
        public bool Save(UserProfile userProfile)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                UserProfile user = dbContext.UserProfile.FirstOrDefault(u => u.UserID == userProfile.UserID);
                if (user == null)
                {
                    if (userProfile.CreatedDateTime == null)
                        userProfile.CreatedDateTime = DateTime.Now;
                    dbContext.UserProfile.Add(userProfile);
                }
                else
                {
                    user.MessageSignatureText = userProfile.MessageSignatureText;
                    user.IsAutoPopulateSignaturewhileSinging = userProfile.IsAutoPopulateSignaturewhileSinging;
                    if (userProfile.FirstName != null)
                        user.FirstName = userProfile.FirstName;
                    if (userProfile.LastName != null)
                        user.LastName = userProfile.LastName;
                    if (userProfile.Company != null)
                        user.Company = userProfile.Company;
                    if (userProfile.Photo != null)
                        user.Photo = userProfile.Photo;
                    if (userProfile.SignatureImage != null)
                        user.SignatureImage = userProfile.SignatureImage;
                    if (userProfile.Title != null)
                        user.Title = userProfile.Title;
                    if (userProfile.Initials != null)
                        user.Initials = userProfile.Initials;
                    if (user.CreatedDateTime == null)
                        user.CreatedDateTime = DateTime.Now;
                    if (!string.IsNullOrEmpty(userProfile.LanguageCode))
                        user.LanguageCode = userProfile.LanguageCode;
                    //Added by TParker- New Setting for Language in Company and Personal
                    if (userProfile.LanguageID != Guid.Empty || userProfile.LanguageID != null)
                        user.LanguageID = userProfile.LanguageID;
                    if (dbContext.Entry(user).State == EntityState.Unchanged)
                        dbContext.Entry(user).State = EntityState.Modified;
                }
                dbContext.SaveChanges();
                return true;
            }
        }
        public bool Save(NonRpostUser userDetails)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                NonRpostUser user = dbContext.NonRpostUser.Where(u => u.EmailAddress.ToLower() == userDetails.EmailAddress.ToLower()).FirstOrDefault();
                if (user == null)
                {
                    dbContext.NonRpostUser.Add(userDetails);
                }
                else
                {
                    if (userDetails.Company != null)
                        user.Company = userDetails.Company;
                    if (userDetails.EmailAddress != null)
                        user.EmailAddress = userDetails.EmailAddress;
                    if (userDetails.Name != null)
                        user.Name = userDetails.Name;
                    if (userDetails.Signature != null)
                        user.Signature = userDetails.Signature;
                    if (userDetails.Title != null)
                        user.Title = userDetails.Title;
                    if (userDetails.Initials != null)
                        user.Initials = userDetails.Initials;

                    dbContext.Entry(user).State = EntityState.Modified;
                }
                dbContext.SaveChanges();
                return true;
            }
        }
        public void CheckRpostUser(Recipients recipient)
        {
            var userProfile = GetUserProfileByEmailID(recipient.EmailAddress);
            if (userProfile != null) return;
            NonRpostUser nonRpostuser = GetNonRpostUser(recipient.EmailAddress);
            {
                if (nonRpostuser == null)
                {
                    var nonRpostUserObject = new NonRpostUser
                    {
                        Company = null,
                        CreatedDateTime = DateTime.Now,
                        EmailAddress = recipient.EmailAddress,
                        ID = Guid.NewGuid(),
                        Name = recipient.Name,
                        Signature = null,
                        Title = null,
                        Initials = null
                    };
                    Save(nonRpostUserObject);
                }
            }
        }
        public UserProfile GetSignerUserProfileByEmail(string signerEmail)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.UserProfile.Where(s => s.EmailID == signerEmail && s.IsActive == true).FirstOrDefault();
            }
        }
        public string GetUserNameByEmailid(string EmailId)
        {           
            using (var dbContext = new RSignDbContext(_configuration))
            {
                string userName = dbContext.UserProfile.Where(d => d.EmailID == EmailId && d.ActiveTo == null).SingleOrDefault().FirstName + " " + dbContext.UserProfile.Where(d => d.EmailID == EmailId && d.ActiveTo == null).SingleOrDefault().LastName;
                return userName;
            }
        }
        public List<UserProfile> GetUserProfileByEmailIDs(List<string> emails)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.UserProfile.AsEnumerable().Where(u => emails.Any(e => u.EmailID.Contains(e)) && u.IsActive).ToList();
            }
        }
        public List<NonRpostUser> GetNonRpostUserList(List<string> emails)
        { 
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.NonRpostUser.Where(u => emails.Contains(u.EmailAddress)).ToList();
            }
        }
    }
}
