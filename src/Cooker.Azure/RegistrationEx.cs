using System;
using System.Collections.Generic;
using Azure.Identity;
using Azure.ResourceManager.Resources;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Azure.Ingredients.Storage;
using Cooker.Ingredients.Secrets;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cooker.Azure
{
    public static class RegistrationEx
    {
        public static void RegisterAzureCooker(
            this IServiceCollection services)
        {
            services.RegisterCooker<AzurePlatformContext>(
                new Dictionary<Type, Type>
                {
                    {typeof(SecretsIngredient), typeof(AzureKeyVaultBuilder)},
                    {typeof(StorageIngredient), typeof(AzureStorageBuilder)},
                });

            services.AddSingleton<IArmRunner, AzureResourceManagerArmRunner>();
            services.AddSingleton<IAzureResourcesSdk, AzureResourcesSdk>();
            services.AddSingleton<KitchenStation<AzurePlatformContext>, ArmKitchenStation>();
            services.AddSingleton<KitchenStation<AzurePlatformContext>, AzureSdkKitchenStation>();
            services.AddSingleton<IPlatformContextBuilder<AzurePlatformContext>, AzurePlatformContextBuilder>();

            services.AddSingleton(sp => new ResourcesManagementClient(
                sp.GetRequiredService<IOptions<AzureCookerSettings>>().Value.SubscriptionId,
                new DefaultAzureCredential()));
        }
    }
}