using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OA.API.Enum;
using OA.API.Interface;
using OA.API.Model;
using OA.API.Models;
using OA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Middleware
{
    [Authorize()]
    public class AuthorizeProvider : IAuthorizeProvider
    {
        IEnumerable<IAuthorizeAction> _authActions;
        public AuthorizeProvider(
            IEnumerable<IAuthorizeAction> authActions)
        {
            _authActions = authActions;
        }

        public async Task<AuthorizeResultEntity> HandlerResult(HttpContext httpContext)
        {
            var authresult = httpContext.GetAuthorizeResultSet();
            if (true)
            {
                var authResultBase64 = httpContext.GetHeaderValue("oa-auth", null);
                if (!string.IsNullOrEmpty(authResultBase64))
                {
                    var authResultDic = JsonUtils.ToModel<Dictionary<AuthorizeType, AuthorizeResultItem>>(Convert.FromBase64String(authResultBase64).Decode());
                    authresult.Results = authResultDic;
                    return authresult;
                }
            }
            foreach (var action in _authActions)
            {
                await action.HandlerResult(httpContext, authresult);
            }
            return authresult;
        }

        public async Task<AuthorizeResultEntity> CheckResult(HttpContext httpContext)
        {
            var authresult = httpContext.GetAuthorizeResultSet();
            authresult.IsAuthrized = true;
            foreach (var action in _authActions)
            {
                var c = await action.CheckResult(httpContext, authresult);
                if (c == null)
                    continue;
                if (!c.IsPass)
                {
                    authresult.IsAuthrized = false;
                    authresult.ErrorCode = c.Code;
                    authresult.ErrorMessage = c.Message;
                    break;
                }
            }
            return authresult;
        }
    }
}
