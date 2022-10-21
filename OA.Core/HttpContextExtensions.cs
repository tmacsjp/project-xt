using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core
{ 
    public static class HttpContextExtensions
    {
        public static JToken GetJObjectBody(this HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue("_jobjectbody", out var body))
            {
                return body as JToken;
            }
            return null;
        }

        public static void SetJObjectBody(this HttpContext httpContext,JToken body)
        {
            httpContext.Items["_jobjectbody"] = body;
        }

    }
}
