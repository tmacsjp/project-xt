using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.ApplicationToController
{
    public class OpenApiFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var iscon = base.IsController(typeInfo);
            if (iscon)
                return true;
            var attr = typeInfo.GetCustomAttribute<OpenServiceApiAttribute>(true);
            if (attr == null)
                return false;
            return true;
        }
    }
}
