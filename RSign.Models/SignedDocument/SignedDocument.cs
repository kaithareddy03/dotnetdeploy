using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RSign.Common;
using RSign.Common.Enums;
using RSign.Common.Helpers;
using RSign.ManageDocument.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.ManageDocument.Models;
using RSign.Models.APIModels;
using RSign.Models.APIModels.Data;
using RSign.Models.APIModels.Envelope;
using RSign.Models.EmailQueueProcessor;
using RSign.Models.Interfaces;
using RSign.Models.SignedDocument;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using ChilkatHelper = RSign.Common.Mailer.ChilkatHelper;
using EmailQueueData = RSign.Models.EmailQueueProcessor.EmailQueueData;
using RSign.Notification;
using RSign.Models.Repository;
using System.Web.Mvc.Html;


namespace RSign.Models.SignedDocument
{
    public class SanboxRecipients
    {
        public Guid RecipientID { get; set; }
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public Guid RecipientTypeID { get; set; }
        public int? CCSignerType { get; set; }
        public string CultureInfo { get; set; }
        public string RecipientMobile { get; set; }
        public int? DeliveryMode { get; set; }
        public string DialCode { get; set; }
        public string CountryCode { get; set; }
    }
    public class SandboxMailTemplateSettings
    {
        public Guid EnvelopeId { get; set; }
        public Guid RecipientTypeId { get; set; }
        public Guid AttachedPdfOption { get; set; }
        public string UrlGetPdf { get; set; }
        public string RetrievalLink { get; set; }
        public List<MailTemplateNew> MailTemplateList { get; set; }
        public string MailTemplateSigner { get; set; }
        public int DateFormat { get; set; }
        public int ContractToGenerateOption { get; set; }
        public string TimeZone { get; set; }
        public string EmailDisclaimer { get; set; }
    }

}

