using RSign.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Notification
{
    public interface IRpostRestService
    {
        HttpResponseMessage RestUserInfo(string userEmail, string token, string tokenType = "Bearer");
        HttpResponseMessage RollBackLockUnits(string envelopeId, string token, string tokenType = "Bearer");
        HttpResponseMessage CommitStaticLinkUnit(string senderAddress, string envelopeId, int envelopeSize, int numberOfRecipients, string basicToken, string tokenType = "Bearer");
        HttpResponseMessage ValidateBotClick(BotClickDataModal botClickDataModal);
    }
}
