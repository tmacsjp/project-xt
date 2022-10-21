using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Model
{
    public class ApiResponseEntity<T>
    {
        public const string Success = "success";
        public string Code { get; set; } = Success;
        public string Msg { get; set; }
        public long Timestamp { get; set; }
        public T Data { get; set; }
    }

    public class ApiResponseEntity : ApiResponseEntity<object>
    { }
}
