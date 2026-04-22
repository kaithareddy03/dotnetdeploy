using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSign.Models.APIModels
{
    public class RCSIntegrationInfo
    {
    }
    public class RCSResponseMessage
    {
        public string? Status { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusText { get; set; }
        public List<Messages> Message { get; set; }
        public List<ResultContents> ResultContent { get; set; }

    }
    public class ResultContents
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
    }
    public class Messages
    {
        public string? Message { get; set; }
        public string? MessageId { get; set; }
    }

    public class RestResposeTokenInfo
    {
        public string? Status { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusText { get; set; }
        public List<Messages> Message { get; set; }
        public ResultContent ResultContent { get; set; }

    }
    public class ResultContent
    {
        public string? token_type { get; set; }
        public int expires_in { get; set; }
        public string? username { get; set; }
        public string? issued { get; set; }
        public string? expires { get; set; }
    }

    public class RPostUserStatusResponse
    {
        public string? Status { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusText { get; set; }
        public RestMessages Message { get; set; }
        public RestLoginResponse ResultContent { get; set; }

        public string? Success { get; set; }
    }
    public class RestMessages
    {
        public string? Message { get; set; }
        public string? MessageCode { get; set; }
    }
    public class RestLoginResponse
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public int expires_in { get; set; }
        public string? userName { get; set; }
        public string? issued { get; set; }
        public string? expires { get; set; }
        public string? refresh_token { get; set; }
        public string? refresh_expires { get; set; }
        public string? refresh_expires_in { get; set; }
        public string? emailaddress { get; set; }
        public bool isnewuser { get; set; }
        public bool rSign_IntegrationEnabled { get; set; }
    }

    public class RPostRefreshTokenStatusResponse
    {
        public string? Status { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusText { get; set; }
        public RestMessages Message { get; set; }
        public RestRefreshTokenResponse ResultContent { get; set; }

        public string? Success { get; set; }
    }

    public class RestRefreshTokenResponse
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public int expires_in { get; set; }
        public string? issued { get; set; }
        public string? expires { get; set; }
        public string? emailaddress { get; set; }
    }
    public class RestAPILoginResponseErrorMessage
    {
        public string? error { get; set; }
        public string? error_description { get; set; }
    }

}
