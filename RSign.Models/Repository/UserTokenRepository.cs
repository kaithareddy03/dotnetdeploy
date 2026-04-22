using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Repository
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        public UserTokenRepository(IOptions<AppSettingsConfig> configuration)
        {
            _configuration = configuration;
        }
        public string GetUserEmailByToken(string token)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.UserToken.Where(d => d.AuthToken == token).OrderByDescending(o => o.LastUpdated).FirstOrDefault().EmailId;
            }
        }
        public Guid GetUserProfileIDByEmail(string Email)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.UserProfile.Where(d => d.EmailID == Email && d.ActiveTo == null).FirstOrDefault().ID;
            }
        }
        public UserProfile GetUserProfileByEmail(string Email)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.UserProfile.Where(d => d.EmailID == Email && d.ActiveTo == null).FirstOrDefault();
            }
        }
        public Guid GetUserProfileUserIDByID(Guid ID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.UserProfile.Where(d => d.ID == ID && d.ActiveTo == null).SingleOrDefault().UserID;
            }
        }
        public UserProfile GetUserProfileByToken(string AuthToken)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    return (from p in dbContext.UserProfile
                            join t in dbContext.UserToken on p.UserID equals t.UserID
                            where t.AuthToken == AuthToken
                            select p).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
