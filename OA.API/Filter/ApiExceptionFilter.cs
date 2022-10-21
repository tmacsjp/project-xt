using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using OA.API.Model;
using OA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Filter
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            if (context.Exception is ApiException ex)
            {
                context.Result = new ObjectResult(new ApiResponseEntity()
                {
                    Code = "error",
                    Msg = context.Exception.Message,
                });
            }
            else
            {
                var msg = context.Exception.Message;
                if (context.Exception.InnerException != null)
                {
                    msg += ";" + context.Exception.InnerException.Message;
                }
                context.Result = new ObjectResult(new ApiResponseEntity()
                {
                    Code = "error",
                    Msg = msg,
                });
            }
        }
    }
}
