using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace OA.Core.ApplicationToController
{
    public class OpenApiControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsAssignableTo(typeof(ControllerBase)))
                return;
            var attr = controller.ControllerType.GetCustomAttribute<OpenServiceApiAttribute>();
            if (attr == null)
                return;
            if (controller.Filters.Any(x => x is ApiControllerAttribute))
            {
                controller.Filters.Add(new ApiControllerAttribute());
            }
            controller.ApiExplorer.IsVisible = true;
            if (string.IsNullOrWhiteSpace(attr.ServiceName))
            {
                controller.ControllerName = controller.ControllerType.GetOpenServiceApiName();
            }
            else
            {
                controller.ControllerName = attr.ServiceName;
            }
            if (controller.Selectors.Count == 0)
                controller.Selectors.Add(new SelectorModel());
            if (controller.Selectors[0].AttributeRouteModel == null)
            {
                controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel(new RouteAttribute("[controller]/[action]"));
            }
        }
    }

    public class OpenApiActionRouteConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerType.IsAssignableTo(typeof(ControllerBase)))
                return;
            if (action.Selectors.Count == 0)
                action.Selectors.Add(new SelectorModel());

            //application层  方法增加post约束
            if (!action.Selectors[0].ActionConstraints.Any(x => x is HttpMethodActionConstraint))
            {
                action.Selectors[0].ActionConstraints.Add(new HttpMethodActionConstraint(
                    new List<string>() {
                        HttpMethod.Post.Method
                    }));
            }
        }
    }

    //public class OpenAuthorizeRouteConvention : IActionModelConvention
    //{
    //    public void Apply(ActionModel action)
    //    {
    //        if (action.Selectors.Count == 0)
    //            action.Selectors.Add(new SelectorModel());
    //        if (!action.Selectors[0].EndpointMetadata.Any(x => x is IOutsoftsAuthorize))
    //        {
    //            if (action.Controller.Selectors.Count == 0)
    //                return;
    //            var attr = action.Controller.Selectors[0].EndpointMetadata.FirstOrDefault(x => x is IOutsoftsAuthorize) as IOutsoftsAuthorize;
    //            if (attr == null)
    //                return;
    //            action.Selectors[0].EndpointMetadata.Add(attr.Clone());
    //        }
    //    }
    //}
}
