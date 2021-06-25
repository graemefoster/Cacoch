using System;
using System.Threading;
using Azure.Core;
using Azure.ResourceManager.Storage;
using Azure.Storage;
using Azure.Storage.Blobs;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Azure;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Rest;

namespace Cacoch.Provider.AzureArm
{
    public static class RegistrationEx
    {
        public static void RegisterCacochAzureArm(
            this IServiceCollection services,
            TokenCredential tokenCredential,
            IConfigurationSection configurationSection)
        {
            services.RegisterCacoch<AzurePlatformContext>(typeof(RegistrationEx).Assembly);

            services.AddSingleton(sp => new ResourceManagementClient(new TokenCredentials(
                tokenCredential.GetToken(new TokenRequestContext(new[]
                {
                    "https://management.core.windows.net/.default"
                }), CancellationToken.None).Token
            ))
            {
                SubscriptionId = sp.GetRequiredService<IOptions<AzureArmSettings>>().Value.SubscriptionId
            });
            services.AddSingleton<IResourceGroupCreator, AzureResourceGroupCreator>();

            services.AddSingleton(sp => new StorageManagementClient(
                sp.GetRequiredService<IOptions<AzureArmSettings>>().Value.SubscriptionId,
                tokenCredential));

            services.AddSingleton(sp => new GraphServiceClient(tokenCredential, new [] {"https://graph.microsoft.com/.default"}));

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<AzureArmSettings>>().Value;
                var key = sp.GetRequiredService<StorageManagementClient>()
                    .StorageAccounts
                    .ListKeys(settings.PlatformResources, settings.PlatformStorage, CancellationToken.None)
                    .Value.Keys[0].Value;

                return new BlobServiceClient(new Uri(sp.GetRequiredService<StorageManagementClient>()
                        .StorageAccounts.GetProperties(settings.PlatformResources, settings.PlatformStorage).Value
                        .PrimaryEndpoints.Blob),
                    new StorageSharedKeyCredential(settings.PlatformStorage, key));
            });

            services.AddTransient<IArmBatchBuilder, ArmBatchDeployer>();
            services.AddSingleton<IArmDeployer, ArmDeployer>();
            services.AddScoped<ICacochManifestDeployer<AzurePlatformContext>, AzureCacochManifestDeployer>();

            services.Configure<AzureArmSettings>(configurationSection);
        }
    }
}