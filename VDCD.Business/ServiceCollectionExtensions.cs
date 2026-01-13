using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace VDCD.Business
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            var assembly = Assembly.Load("VDCD.Business");

            var serviceTypes = assembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.Namespace == "VDCD.Business.Service");

            foreach (var type in serviceTypes)
            {
                services.AddScoped(type);
            }

            return services;
        }
    }
}
