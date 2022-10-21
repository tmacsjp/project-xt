using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.ApplicationToController
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OpenServiceApiAttribute : Attribute
    {
        public string? ServiceName { get; set; }
        public OpenServiceApiAttribute()
        {
        }
        public OpenServiceApiAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }
    }
}
