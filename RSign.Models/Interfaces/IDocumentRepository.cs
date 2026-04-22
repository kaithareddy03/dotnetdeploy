using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IDocumentRepository
    {
        IQueryable<Documents> GetAll(Guid envelopeID);
        bool SaveEmailLogsRecord(List<EmailLogs> emailLogs);
        void SaveDestinationRecord(Destination destinationRecord);
        bool Save(Documents document);
    }
}
