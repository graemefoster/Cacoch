using System;
using System.Threading;
using Azure.Core;
using Azure.ResourceManager.Storage;
using Azure.Storage;
using Azure.Storage.Blobs;
using Cacoch.Core.Manifest;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Azure;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            services.RegisterCacoch<AzurePlatformContext>(typeof(RegistrationEx).Assembly);

            services.AddSingleton(credentials);
            services.AddSingleton(tokenCredential);

            services.AddSingleton<IResourceGroupCreator, AzureResourceGroupCreator>();
            services.AddSingleton(sp => new StorageManagementClient(
                sp.GetRequiredService<IOptions<AzureArmSettings>>().Value.SubscriptionId,
                tokenCredential));

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

            services.AddScoped<IArmBatchBuilder, ArmBatchDeployer>();
            services.AddSingleton<IArmDeployer, ArmDeployer>();
            services.AddScoped<ICacochManifestDeployer<AzurePlatformContext>, AzureCacochManifestDeployer>();

            services.Configure<AzureArmSettings>(configurationSection);
        }
    }
}