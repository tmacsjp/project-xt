using Microsoft.AspNetCore.Http;
using OA.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Interface
{
    public interface IAuthorizeAction
    {
        Task HandlerResult(HttpContext httpContext, AuthorizeResultEntity resultEntity);

        Task<AuthorizeResultItem> CheckResult(HttpContext httpContext, AuthorizeResultEntity resultEntity);
    }
}
