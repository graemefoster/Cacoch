using System.Collections.Generic;
using Cooker.Azure.KitchenStations.Arm;
using Cooker.Ingredients;
using Cooker.Ingredients.WebApp;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.WebApp
{
    // ReSharper disable once UnusedType.Global
    public class WebAppConfigurationBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public WebAppConfigurationBuilder(WebAppConfigurationIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private WebAppConfigurationIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            PlatformEnvironment environment,
            Docket docket,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var template = typeof(WebAppBuilder).GetResourceContents("WebAppConfiguration");
            return new ArmRecipe<EmptyOutput>(
                new ArmDefinition(
                    $"webapp-configuration-{Ingredient.Id}",
                    template,
                    new Dictionary<string, object>()
                    {
                        { "name", Ingredient.TypedIngredientData!.PlatformName },
                        {
                            "appSettings", Ingredient.TypedIngredientData!.Configuration
                        },
                    }),
                _ => new EmptyOutput());
        }
    }
}