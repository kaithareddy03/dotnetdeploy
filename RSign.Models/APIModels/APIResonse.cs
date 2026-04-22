using RSign.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.APIModels
{
    public class InfoResultResonse
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// this will retrun status messaga
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return true/false
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// this will return message
        /// </summary>
        public string? message { get; set; }
        /// <summary>
        /// this will return reposne url
        /// </summary>
        public string? returnUrl { get; set; }
        /// <summary>
        /// this will return column
        /// </summary>
        public string? field { get; set; }
        /// <summary>
        /// this will retrun object
        /// </summary>
        public object data { get; set; }
        public APIRecipientEntity recpDetail { get; set; }
        /// <summary>
        /// This will retrun message type
        /// </summary>
        public MessageType type { get; set; }
        /// <summary>
        /// this will retrun post redirect URL
        /// </summary>
        public bool postSigningLogin { get; set; }
        public string? InfoSenderEmail { get; set; }
        public string? DGReturnURL { get; set; }
        public bool showDGPopup { get; set; }
        public bool isDGFlow { get; set; }
        public string? EncryptedQueryString { get; set; }
        public bool isSignedDocumentService { get; set; }
        public string? postSigningUrl { get; set; }
        public string? LanguageID { get; set; }
        public string? EncryptSender { get; set; }
        public string? EncryptEnvelopeId { get; set; }
        public string? EncryptRecipientId { get; set; }   
        public bool IsEnvelopePurging { get; set; }
        public string EnableMessageToMobile { get; set; }
        public Guid EnvelopeStatus { get; set; }
        public Guid? AttachSignedPdfID { get; set; }

    }

    public class MasterEnvelopeStatsResonse
    {
        /// <summary>
        /// This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// this will retrun status messaga
        /// </summary>
        public string? StatusMessage { get; set; }       
        /// <summary>
        /// this will retrun object
        /// </summary>
        public object Data { get; set; }       

    }
}
