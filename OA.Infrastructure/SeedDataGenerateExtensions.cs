using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure
{
    public static class SeedDataGenerateExtensions
    {
        public static void GenerateSeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var generators = scope.ServiceProvider.GetServices<ISeedDataGenerator>();
            foreach (var a in generators)
            {
                a.Execute().Wait();
            }
        }

    }
}
