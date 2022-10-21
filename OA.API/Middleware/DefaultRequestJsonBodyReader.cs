using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OA.API.Interface;
using OA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Middleware
{
    public class DefaultRequestJsonBodyReader : IReqeustJsonBodyReader
    {
        public async Task<JToken> Read(IServiceProvider provider, HttpContext context)
        {
            var hasJsonType = context.Request.HasJsonContentType();
            if (hasJsonType == false)
                return null;

            var cachejbody = context.GetJObjectBody();
            if (cachejbody != null)
                return cachejbody;

            //var accessor = context.RequestServices.GetService<IContextInfoAccessor>();
            //string body = accessor == null ? null : accessor.GetRequestJsonBody();
            //if (body == null)
            //{
            var jsonBodyStream = await TryCacheReqeustBody(context);
            if (jsonBodyStream == null)
                return null;
            var body = jsonBodyStream.ToArray().Decode();
            context.Request.Body = jsonBodyStream;
            //if (accessor != null)
            //accessor.SetRequestJsonBody(body);
            //}
            if (string.IsNullOrWhiteSpace(body))
                return null;
            try
            {
                var jbody = JsonUtils.ConvertToObject(body);
                context.SetJObjectBody(jbody);
                return jbody;
            }
            catch { }
            return null;
        }


        private async Task<MemoryStream> TryCacheReqeustBody(HttpContext context)
        {
            if (context.Request.Body == null)
                return null;
            if (!context.Request.Body.CanRead)
                return null;
            if (!context.Request.HasJsonContentType())
                return null;
            var ms = new MemoryStream((int)(context.Request.ContentLength ?? 0));
            await context.Request.Body.CopyToAsync(ms);
            return ms;
        }
    }
}
