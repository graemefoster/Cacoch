using Azure.Core;
using Cacoch.Core.Manifest;
using Cacoch.Provider.AzureArm.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;

namespace Cacoch.Provider.AzureArm
{
    public static class RegistrationEx
    {
        public static void RegisterCacochAzureArm(
            this IServiceCollection services, 
            ServiceClientCredentials credentials, 
            TokenCredential tokenCredential,
            IConfigurationSection configurationSection)
        {
            services.AddSingleton(credentials);
            services.AddSingleton(tokenCredential);
            
            services.AddSingleton<IResourceGroupCreator, AzureResourceGroupCreator>();
            services.AddSingleton<IArmDeployer, ArmDeployer>();
            services.AddSingleton<ICacochManifestDeployer, AzureCacochManifestDeployer>();
            services.AddSingleton<CacochPlatform>();
            
            services.Configure<AzureArmSettings>(configurationSection);
        }
    }
}