using Microsoft.AspNetCore.Http.Features;
using OA.API.Filter;
using OA.API.Interface;
using OA.API.Model;
using OA.API.Models;
using OA.Core;
using System.Net;
using System.Text;

namespace OA.API.Middleware
{
    public class ApiProtocolMiddleware
    {
        IAuthorizeProvider _authorizeProvider;

        private readonly RequestDelegate _next;
        public ApiProtocolMiddleware(RequestDelegate next, IAuthorizeProvider authorizeProvider)
        { 
            _next = next;
            _authorizeProvider = authorizeProvider;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
                //终节点需要性判断
                if (endpoint == null)
                {
                    await _next(context);
                    return;
                }


                //请求授权处理
                var result = await _authorizeProvider.HandlerResult(context);

                //验证授权的结果
                var verifyresults = await _authorizeProvider.CheckResult(context);

                //授权不通过
                if (!verifyresults.IsAuthrized)
                {
                    await ProcessNoAuth(context, verifyresults);
                    return;
                }


                await _next.Invoke(context);

            }
            catch (Exception ex)
            {
                await ProcessError(context, ex);
            }
            finally
            {
            }
        }


        private async Task ProcessError(HttpContext context, Exception exception)
        {
            ApiResponseEntity rentity = null;
            if (exception is ApiException e)
            {
                rentity = new ApiResponseEntity()
                {
                    Code = "normal-error",
                    Msg = exception.Message,
                    Timestamp = Security.Timestamp,
                };
            }
            else
            {
                rentity = new ApiResponseEntity()
                {
                    Code = "normal-error",
                    Msg = exception.Message,
                    Timestamp = Security.Timestamp,
                };
            }
            var json = JsonUtils.ToJsonBeauty(rentity);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.Body.WriteAsync(json.Encode());
            await context.Response.Body.FlushAsync();
            await context.Response.CompleteAsync();
        }

        private async Task ProcessNoAuth(HttpContext context, AuthorizeResultEntity authResult)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json; charset=utf-8";
            var json = JsonUtils.ToJsonBeauty(new ApiResponseEntity()
            {
                Code = authResult.ErrorCode,
                Msg = authResult.ErrorMessage,
                Timestamp = Security.Timestamp,
            });
            await context.Response.Body.WriteAsync(json.Encode());
            await context.Response.Body.FlushAsync();
            await context.Response.CompleteAsync();
        }

    }
}
