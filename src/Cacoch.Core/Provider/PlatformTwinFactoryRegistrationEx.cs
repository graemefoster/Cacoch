using System.Linq;
using System.Reflection;
using Cacoch.Core.Manifest;
using Microsoft.Extensions.DependencyInjection;

namespace Cacoch.Core.Provider
{
    public static class PlatformTwinFactoryRegistrationEx
    {
        public static void RegisterCacoch<TPlatformContext>(this IServiceCollection services, Assembly providerAssembly) where TPlatformContext : IPlatformContext
        {
            services.AddSingleton<PlatformTwinFactory<TPlatformContext>>();
            services.AddScoped<IManifestDeployer, ManifestDeployer<TPlatformContext>>();
            var mappingDictionary = new PlatformTwinDictionary();
            
            var twins = providerAssembly.DefinedTypes
                .Where(x =>
                    x.IsClass &&
                    !x.IsAbstract && 
                    x.ImplementedInterfaces.Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IPlatformTwin<>)))
                .ToArray();

            foreach (var twin in twins)
            {
                var serviceType = twin.ImplementedInterfaces.Single(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IPlatformTwin<>));
                mappingDictionary.Map(serviceType, twin);
                services.AddTransient(
                    serviceType,
                    twin);
            }

            services.AddSingleton(mappingDictionary);
        }
    }
}