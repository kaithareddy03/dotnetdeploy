using RSign.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Notification
{
    public interface IMobileSMSService
    {
        HttpResponseMessage AuthticateMobileService();
        HttpResponseMessage SendMobileSMS(List<SendMobileSMSModel> sendMobileSMSModelData, string token);
    }
}
