using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Common.Enums
{
    public enum MessageType
    {
        Success = 1,
        Warning,
        Error,
        SuccessWarning,
        SuccessErrorMessage,
        ArchivedOrPurged
    }
}
