using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface ICompanyRepository
    {
        Company GetCompanyForUserID(Guid UserID);
        Company GetCompanyProfileByEnvelopeID(Guid envelopeID);
        Company GetCompanyByID(Guid? CompanyID);
    }
}
