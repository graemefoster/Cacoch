using System;
using System.Collections.Generic;
using System.Configuration;
using Azure.Identity;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Azure.Ingredients.Storage;
using Cooker.Ingredients.Secrets;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Cooker.Azure
{
    public static class RegistrationEx
    {
        public static void RegisterAzureCooker(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("AzureSettings");
            services.Configure<AzureCookerSettings>(configurationSection);
            
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

            services.AddSingleton<IAzureSdkProvider>(sp => new AzureSdkProvider(
                new DefaultAzureCredential(),
                sp.GetRequiredService<IOptions<AzureCookerSettings>>().Value.SubscriptionId!));
        }
    }
}