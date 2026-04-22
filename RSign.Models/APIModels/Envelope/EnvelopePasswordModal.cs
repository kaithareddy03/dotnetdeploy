using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSign.Models.APIModels.Envelope
{
    public class EnvelopePasswordModal
    {
        public EnvelopePasswordModal()
        {

        }
        public EnvelopePasswordModal(Guid eId, Guid cRecId)
        {
            this.EnvelopeID = eId;
            this.CurrentRecipientID = cRecId;
        }
        public EnvelopePasswordModal(Guid eId, Guid cRecId, bool isPwdRqdToOpen, string viewDocumentOptions)
        {
            this.EnvelopeID = eId;
            this.CurrentRecipientID = cRecId;
            this.IsPasswordRequiredToOpen = isPwdRqdToOpen;
            this.ViewDocumentOptions = viewDocumentOptions;
        }
        public EnvelopePasswordModal(Guid eId, Guid cRecId, bool isDiscEnb, string disc, bool isPwdRqd)
        {
            this.EnvelopeID = eId;
            this.CurrentRecipientID = cRecId;
            this.IsDisclaimerEnabled = isDiscEnb;
            this.Disclaimer = disc;
            this.IsPasswordRequiredToSign = isPwdRqd;
        }

        public EnvelopePasswordModal(Guid eId, Guid cRecId, bool isDiscEnb, string disc, string rEmail, string sEmail, bool sIsStatic, bool sIsFirstSigner, bool sIsPasswordMailToSigner)
        {
            this.EnvelopeID = eId;
            this.CurrentRecipientID = cRecId;
            this.IsDisclaimerEnabled = isDiscEnb;
            this.Disclaimer = disc;
            this.CurrentRecipientEmail = rEmail;
            this.SenderEmail = sEmail;
            this.IsStatic = sIsStatic;
            this.IsFirstSigner = sIsFirstSigner;
            this.IsPasswordMailToSigner = sIsPasswordMailToSigner;
        }
        public EnvelopePasswordModal(Guid eId, Guid cRecId, bool isDiscEnb, string disc, string rEmail, string sEmail, bool sIsStatic, bool sIsFirstSigner, bool sIsPasswordMailToSigner, string deliveryMode, string dialCode, string mobileNumber, string CountryCode)
        {
            this.EnvelopeID = eId;
            this.CurrentRecipientID = cRecId;
            this.IsDisclaimerEnabled = isDiscEnb;
            this.Disclaimer = disc;
            this.CurrentRecipientEmail = rEmail;
            this.SenderEmail = sEmail;
            this.IsStatic = sIsStatic;
            this.IsFirstSigner = sIsFirstSigner;
            this.IsPasswordMailToSigner = sIsPasswordMailToSigner;
            this.DeliveryMode = deliveryMode;
            this.DialCode = dialCode;
            this.MobileNumber = mobileNumber;
            this.CountryCode = CountryCode;
        }
        public Guid EnvelopeID { get; set; }
        public Guid CurrentRecipientID { get; set; }
        public bool IsDisclaimerEnabled { get; set; }
        public string? Disclaimer { get; set; }
        public bool IsPasswordRequiredToSign { get; set; }
        public bool IsPasswordRequiredToOpen { get; set; }
        public string? ViewDocumentOptions { get; set; }
        public string? CurrentRecipientEmail { get; set; }
        public bool IsPasswordMailToSigner { get; set; }
        public bool IsStatic { get; set; }
        public bool IsFirstSigner { get; set; }
        public string? SenderEmail { get; set; }
        public string DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string MobileNumber { get; set; }
        public string CountryCode { get; set; }
    }
}