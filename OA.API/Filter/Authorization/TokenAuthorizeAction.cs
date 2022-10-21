using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using OA.API.Interface;
using OA.API.Models;
using OA.Core;
using OA.API.Enum;

namespace OA.API.Filter.Authorization
{
    //public class TokenAuthorizeAction : IAuthorizeAction
    //{

    //    public async Task Check(HttpContext httpContext, AuthorizeResultEntity resultSet)
    //    {
    //        var tokenService = httpContext.RequestServices.GetRequiredService<IAuthorizeService>();
    //        var timestampauth = new AuthorizeResultItem(AuthorizeType.Token);
    //        resultSet.Results.Add(AuthorizeType.Token, timestampauth);
    //        if (string.IsNullOrWhiteSpace(resultSet.Token))
    //        {
    //            timestampauth.Code = "auth-token-noarg";
    //            timestampauth.Message = "无token";
    //            return;
    //        }

    //        var tokenInfo = await tokenService.GetTokenAsync(resultSet.Token);
    //        if (tokenInfo == null || (tokenInfo.Expire != null && tokenInfo.Expire.Value < Security.BeiJingNow))
    //        {
    //            timestampauth.Code = "auth-token-invalidate";
    //            timestampauth.Message = "无效token";
    //            return;
    //        }
    //        resultSet.Context.UserId = tokenInfo.UserId;
    //        resultSet.Context.UserName = tokenInfo.UserName;
    //        resultSet.Context.Token = tokenInfo.Token;
    //        timestampauth.IsPass = true;
    //        return;
    //    }
    //}
}
