using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DIActionAttribute : Attribute
    {
        public Type? InterfaceType { get; set; }
        public Type? ImplementationType { get; set; }
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
        public DIActionAttribute(Type interfaceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            InterfaceType = interfaceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }
        public DIActionAttribute(Type interfaceType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            InterfaceType = interfaceType;
            Lifetime = lifetime;
        }
        public DIActionAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Lifetime = lifetime;
        }
    }
}
