using Microsoft.Extensions.DependencyInjection;
using Utileco.UtiQ.DI;

namespace Utileco.UtiQ
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUtiQ(this IServiceCollection services,
            Action<UtiQServiceConfiguration> configuration)
        {
            var config = new UtiQServiceConfiguration();

            configuration.Invoke(config);

            if (!config.AssembliesToRegister.Any())
            {
                throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");
            }

            ServiceRegistrar.AddUtiQClasses(services, config);
            ServiceRegistrar.AddRequiredServices(services, config);

            return services;
        }
    }
}
