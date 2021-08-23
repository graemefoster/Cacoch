using System.Collections.Generic;
using System.Linq;
using Cooker.Azure.Ingredients.WebApp;
using Cooker.Azure.KitchenStations.Arm;
using Cooker.Ingredients;
using Cooker.Ingredients.Functions;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Functions
{
    // ReSharper disable once UnusedType.Global
    public class FunctionConfigurationBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public FunctionConfigurationBuilder(FunctionConfigurationIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private FunctionConfigurationIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            PlatformEnvironment environment,
            Docket docket,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var template = typeof(FunctionConfigurationBuilder).GetResourceContents("FunctionConfiguration");

            ConfigureFunctionAppDefaults();

            return new ArmRecipe<EmptyOutput>(
                new ArmDefinition(
                    $"function-configuration-{Ingredient.Id}",
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

        private void ConfigureFunctionAppDefaults()
        {
            if (!Ingredient.TypedIngredientData!.Configuration.ContainsKey("FUNCTIONS_EXTENSION_VERSION"))
            {
                Ingredient.TypedIngredientData!.Configuration.Add("FUNCTIONS_EXTENSION_VERSION", "~3.0");
            }

            if (!Ingredient.TypedIngredientData!.Configuration.ContainsKey("FUNCTIONS_WORKER_RUNTIME"))
            {
                Ingredient.TypedIngredientData!.Configuration.Add("FUNCTIONS_WORKER_RUNTIME", "dotnet");
            }
        }
    }
}