using Microsoft.AspNetCore.Http;
using OA.API.Model;
using OA.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Interface;

public interface IAuthorizeProvider
{
    Task<AuthorizeResultEntity> HandlerResult(HttpContext httpContext);

    Task<AuthorizeResultEntity> CheckResult(HttpContext httpContext);
}
