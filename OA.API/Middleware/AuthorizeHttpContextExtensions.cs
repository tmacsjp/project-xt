using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using OA.API.Enum;
using OA.API.Models;
using OA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Middleware
{
    public static class AuthorizeHttpContextExtensions
    {
        public static AuthorizeResultEntity GetAuthorizeResultSet(this HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue(nameof(AuthorizeResultEntity), out var value))
            {
                return (AuthorizeResultEntity)value;
            }
            AuthorizeResultEntity authresult = null;
            if (TryGetFromQueryUnion(httpContext, out authresult))
            { }
            else if (TryGetFromHeaderUnion(httpContext, out authresult))
            { }
            else
            {
                authresult = GetFromHeaderNormal(httpContext);
            }
            httpContext.Items.TryAdd(nameof(AuthorizeResultEntity), authresult);
            authresult.Context.ClientIP = httpContext.GetClientIP();
            return authresult;
        }

        private static bool TryGetFromQueryUnion(HttpContext httpContext, out AuthorizeResultEntity value)
        {
            value = null;
            string v = httpContext.Request.Query["u"];
            if (string.IsNullOrWhiteSpace(v))
                return false;
            try
            {
                var json = Base64UrlTextEncoder.Decode(System.Net.WebUtility.UrlDecode(v)).Decode();
                var model = JsonUtils.ToModel<AuthorizeResultEntity>(json);
                model.Results = new Dictionary<AuthorizeType, AuthorizeResultItem>();
                value = model;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryGetFromHeaderUnion(HttpContext httpContext, out AuthorizeResultEntity value)
        {
            value = null;
            var v = httpContext.GetHeaderValue("oa-u");
            if (string.IsNullOrWhiteSpace(v))
                return false;
            try
            {
                var json = Convert.FromBase64String(v).Decode();
                var model = JsonUtils.ToModel<AuthorizeResultEntity>(json);
                model.Results = new Dictionary<AuthorizeType, AuthorizeResultItem>();
                value = model;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static AuthorizeResultEntity GetFromHeaderNormal(HttpContext httpContext)
        {
            var authresult = new AuthorizeResultEntity();
            authresult.Timestamp = httpContext.GetHeaderValue("oa-timestamp");
            authresult.AppKey = httpContext.GetHeaderValue("oa-appkey");
            authresult.Sign = httpContext.GetHeaderValue("oa-sign");
            authresult.Token = httpContext.GetHeaderValue("oa-token");
            authresult.User = httpContext.GetHeaderValue("oa-user");
            return authresult;
        }

        public static string GetHeaderValue(this HttpContext httpContext, string key, string @default = null)
        {
            if (httpContext.Request.Headers.TryGetValue(key, out var value))
            {
                return value;
            }
            return @default;
        }
        public static string GetClientIP(this HttpContext httpContext)
        {
            var ip = string.Empty;
            ip = httpContext.Request.Headers["X-Forwarded-For"].ToString();
            if (string.IsNullOrEmpty(ip))
                ip = httpContext.Connection.RemoteIpAddress?.ToString();
            return ip;
        }
    }
}
