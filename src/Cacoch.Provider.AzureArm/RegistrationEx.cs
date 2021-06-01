using Cacoch.Core.Manifest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;

namespace Cacoch.Provider.AzureArm
{
    public static class RegistrationEx
    {
        public static void RegisterCacochAzureArm(
            this IServiceCollection services, 
            ServiceClientCredentials credentials, 
            IConfigurationSection configurationSection)
        {
            services.AddSingleton(sp => new AzureDeployer(credentials, sp.GetRequiredService<ILogger<AzureDeployer>>(), sp.GetRequiredService<IOptions<AzureArmSettings>>()));
            services.AddSingleton<IAzureArmDeployer>(sp => sp.GetRequiredService<AzureDeployer>());
            services.AddSingleton<IPlatformDeployer>(sp => sp.GetRequiredService<AzureDeployer>());
            services.Configure<AzureArmSettings>(configurationSection);
        }
    }
}