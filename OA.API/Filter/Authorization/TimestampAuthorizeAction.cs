using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OA.API.Interface;
using OA.API.Models;
using OA.API.Enum;
using OA.Core;

namespace OA.API.Filter.Authorization
{
    public class TimestampAuthorizeAction : IAuthorizeAction
    {
        public async Task HandlerResult(HttpContext httpContext, AuthorizeResultEntity resultEntity)
        {
            var timestampauth = new AuthorizeResultItem(AuthorizeType.Timestamp);
            resultEntity.Results.Add(AuthorizeType.Timestamp, timestampauth);
            if (string.IsNullOrWhiteSpace(resultEntity.Timestamp))
            {
                timestampauth.Code = "auth-timestamp-noarg";
                timestampauth.Message = "无时间戳";
                return;
            }
            var timestamp = resultEntity.Timestamp.StrToLong();
            if (ConstantConfigs.SafeSeconds == 0)
            {
                timestampauth.IsPass = true;
                return;
            }

            var seconds = Math.Abs(Security.Timestamp - timestamp);
            if (seconds > ConstantConfigs.SafeSeconds)
            {
                timestampauth.Code = "auth-timestamp-timeout";
                timestampauth.Message = "timeout";
                return;
            }

            timestampauth.IsPass = true;
            return;
        }

        public async Task<AuthorizeResultItem> CheckResult(HttpContext httpContext, AuthorizeResultEntity resultEntity)
        {
            if (!resultEntity.AuthorizeType.HasFlag(AuthorizeType.Timestamp))
                return AuthorizeResultItem.Pass;
            var item = resultEntity.Results.ContainsKey(AuthorizeType.Timestamp) ? resultEntity.Results[AuthorizeType.Timestamp] : null;
            if (item == null)
                return new AuthorizeResultItem { Code = "auth-timestamp-noarg", Message = "无时间戳" };
            return item;
        }
    }
}
