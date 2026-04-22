using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        public CompanyRepository(IOptions<AppSettingsConfig> configuration)
        {
            _configuration = configuration;
        }
        public Company GetCompanyForUserID(Guid UserID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                var company = (from u in dbContext.UserProfile
                               join c in dbContext.Company on u.CompanyID equals c.ID
                               where u.UserID == UserID
                               select c).FirstOrDefault();
                return company;
            }
        }
        public Company GetCompanyProfileByEnvelopeID(Guid envelopeID)
        {
            try
            {
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    Company company = new Company();
                    company = (from c in dbContext.Company
                               join u in dbContext.UserProfile on c.ID equals u.CompanyID
                               join e in dbContext.Envelope on u.UserID equals e.UserID
                               where e.ID == envelopeID
                               select c).FirstOrDefault();
                    return company;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Company GetCompanyByID(Guid? CompanyID)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.Company.FirstOrDefault(u => u.ID == CompanyID);
            }
        }
    }
}
