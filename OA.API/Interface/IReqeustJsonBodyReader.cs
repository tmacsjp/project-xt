using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Interface
{
    public interface IReqeustJsonBodyReader
    {
        Task<JToken> Read(IServiceProvider provider, HttpContext context);
    }
}
