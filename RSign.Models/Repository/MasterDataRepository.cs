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
    public class MasterDataRepository : IMasterDataRepository
    {
        private readonly IOptions<AppSettingsConfig> _configuration;
        public MasterDataRepository(IOptions<AppSettingsConfig> configuration)
        {
            _configuration = configuration;
        }
        public List<Control> GetControlID()
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.Control.Where(a => a.IsDeleted == 0).OrderBy(a => a.Order).ToList();
            }
        }
        public bool ValidateDateFormatId(Guid id)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.DateFormat.Any(d => d.ID == id);
            }
        }

        public bool ValidateExpiryTypeId(Guid id)
        {
            using (var dbContext = new RSignDbContext(_configuration))
            {
                return dbContext.ExpiryType.Any(d => d.ID == id);
            }
        }
    }
}
