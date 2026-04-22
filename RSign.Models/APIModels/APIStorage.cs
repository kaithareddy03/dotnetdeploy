using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.APIModels
{
    class APIStorage
    {
    }

    public class StorageSettings
    {
        public int StorageSettingId { get; set; }
        public Guid UserId { get; set; }
        public bool EnableDownload { get; set; }
        public int Frequency { get; set; }
        public bool IsFinalContract { get; set; }
        public bool IsTransparency { get; set; }
        public bool IsSignerAttachment { get; set; }
        public List<StorageOptions> StorageOptions { get; set; }
    }

    public class StorageOptions
    {
        public int StorageSettingId { get; set; }
        public string? StorageType { get; set; }
        public string? StoragePath { get; set; }
    }

    public class APIResponseStorageSettings
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? Message { get; set; }
        public StorageSettings StorageSettings { get; set; }
        public int RSignDownloadDocumentsCount { get; set; }
    }
    public class APIResponseSaveDefaultStorageSettings
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? Message { get; set; }
        public bool Status { get; set; }
    }
    public class APILocalStoragePath
    {
        public string? Email { get; set; }
        public string? LocalStoragePath { get; set; }
    }
    public class APIResponseSaveStorageSettings
    {
        /// <summary>
        ///  This will return Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        ///  This will return Status Message.
        /// </summary>
        public string? StatusMessage { get; set; }
        /// <summary>
        /// This will return response message for corresponding  status code.
        /// </summary>
        public string? Message { get; set; }
    }

    public class APIResquestSaveStorageSettings
    {
        public StorageSettings StorageSettings { get; set; }
        public List<StorageOptions> StorageOptions { get; set; }
    }

    public class APIRequestStorageSetting
    {
        public bool EnableDownload { get; set; }
    }
    public class APIRequestDownloadFinalEnvelope
    {
        public DataTable dtEnvelopeId { get; set; }
        public string? Email { get; set; }
    }

    public class StorageDetailDBResponse
    {
        public int returnStatusCode { get; set; }      
        public string? StatusMessage { get; set; }
    }
}
