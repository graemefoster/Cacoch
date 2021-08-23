using System.Collections.Generic;
using Cooker.Azure.KitchenStations.Arm;
using Cooker.Ingredients;
using Cooker.Ingredients.Functions;
using Cooker.Kitchens;
using Microsoft.Extensions.Options;

namespace Cooker.Azure.Ingredients.Functions
{
    // ReSharper disable once UnusedType.Global
    public class FunctionBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        private readonly IOptions<AzureCookerSettings> _settings;

        public FunctionBuilder(FunctionIngredient ingredient, IOptions<AzureCookerSettings> settings)
        {
            _settings = settings;
            Ingredient = ingredient;
        }

        private FunctionIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            PlatformEnvironment environment,
            Docket docket,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var webAppName = $"{Ingredient.Id}-{environment.ShortName}-{platformContext.Randomness}".ToLowerInvariant();
            var template = typeof(FunctionBuilder).GetResourceContents("Function");
            return new ArmRecipe<FunctionOutput>(
                new ArmDefinition(
                    $"webapp-{Ingredient.Id}",
                    template,
                    new Dictionary<string, object>()
                    {
                        { "name", webAppName },
                        {
                            "serverFarmId",
                            _settings.Value.PlatformAppServicePlans[Ingredient.TypedIngredientData!.Classification]
                        },
                    }),
                output => new FunctionOutput(
                    Ingredient.TypedIngredientData,
                    webAppName,
                    (string)output["hostName"],
                    (string)output["servicePrincipalId"]
                ));
        }
    }
}