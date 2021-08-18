using System.Linq;
using Azure.Identity;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Azure.KitchenStations.Arm;
using Cooker.Azure.KitchenStations.Sdk;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;
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

            var recipeBuilders = typeof(SecretsIngredient)
                .Assembly
                .GetTypes()
                .Where(x => x.IsAssignableTo(typeof(IIngredient)))
                .Where(x => !x.IsAbstract)
                .ToDictionary(
                    x => x,
                    x => typeof(RegistrationEx)
                        .Assembly
                        .GetTypes()
                        .Where(b => b.IsAssignableTo(typeof(IRecipeBuilder<AzurePlatformContext>)))
                        .Single(b => b.GetConstructors()
                            .Any(c => 
                                c.GetParameters()
                                    .Any(p => p.ParameterType == x)))
                );

            services.RegisterCooker<AzurePlatformContext>(recipeBuilders);
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