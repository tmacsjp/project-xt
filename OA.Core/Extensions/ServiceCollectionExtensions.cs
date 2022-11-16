using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OA.Core.ApplicationToController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void AddModuleAllToDI(this IServiceCollection services, Type typeOfModule)
        {
            var types = typeOfModule.Assembly.GetTypes();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<DIActionAttribute>(true);
                if (attr == null)
                    continue;
                var fromType = attr.InterfaceType ?? type;
                var toType = attr.ImplementationType ?? type;
                services.Replace(new ServiceDescriptor(fromType, toType, attr.Lifetime));
            }
        }

        public static void AddModuleAllOpenApi(this IServiceCollection services, Type typeOfModule)
        {
            services.AddMvcCore(options =>
            {
                options.Conventions.Add(new OpenApiControllerRouteConvention());
                options.Conventions.Add(new OpenApiActionRouteConvention());
                //options.Conventions.Add(new OpenAuthorizeRouteConvention());
            }).ConfigureApplicationPartManager(options =>
            {
                options.FeatureProviders.Add(new OpenApiFeatureProvider());
                options.ApplicationParts.Add(new AssemblyPart(typeOfModule.Assembly));
            });
        }

    }
}
