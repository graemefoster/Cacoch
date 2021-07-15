using System;
using System.Collections.Generic;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Azure.Ingredients.Storage;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Rest;

namespace Cooker.Azure
{
    public static class RegistrationEx
    {
        public static void RegisterAzureCooker(
            this IServiceCollection services)
        {
            var tokenCredential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions()
            {
            });

            services.RegisterCooker(
                new Dictionary<Type, Func<IIngredient, IIngredientBuilder>>
            {
                {typeof(Secrets), i => new AzureKeyVaultBuilder((Secrets) i)},
                {typeof(Storage), i => new AzureStorageBuilder((Storage) i)},
            });
            
            services.AddSingleton<IArmRunner, AzureResourceManagerArmRunner>();
            services.AddSingleton<KitchenStation, ArmKitchenStation>();

            services.AddSingleton(sp => new ResourceManagementClient(new TokenCredentials(
                tokenCredential.GetToken(new TokenRequestContext(new[]
                {
                    "https://management.core.windows.net/.default"
                }), CancellationToken.None).Token
            ))
            {
                SubscriptionId = sp.GetRequiredService<IOptions<AzureCookerSettings>>().Value.SubscriptionId
            });
        }
    }
}