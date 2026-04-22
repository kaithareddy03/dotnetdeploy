using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IMasterDataRepository
    {
        List<Control> GetControlID();
        public bool ValidateDateFormatId(Guid id);
        bool ValidateExpiryTypeId(Guid id);
    }
}
