using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Notification
{
    public class PlanUnitsViewModel
    {
        public PlanUnitsViewModel()
        {
        }
        public PlanUnitsViewModel(string ClientId, string envelopeId)
        {
            this.ClientId = ClientId;
            this.EnvelopeId = envelopeId;
        }
        public string EnvelopeId { get; set; }
        public string ClientId { get; set; }
    }
    public class LockUnitsViewModel
    {
        public LockUnitsViewModel()
        {
        }
        public LockUnitsViewModel(string ClientId, string senderAddress, string envelopeId, int envelopeSize, int numberOfRecipients)
        {
            this.ClientId = ClientId;
            this.SenderAddress = senderAddress;
            this.EnvelopeId = envelopeId;
            this.EnvelopeSize = envelopeSize;
            this.NumberOfRecipients = numberOfRecipients;
        }
        public string ClientId { get; set; }
        public string SenderAddress { get; set; }
        public string EnvelopeId { get; set; }
        public int EnvelopeSize { get; set; }
        public int NumberOfRecipients { get; set; }
    }
}
