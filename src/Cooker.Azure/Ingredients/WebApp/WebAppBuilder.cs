﻿using System.Collections.Generic;
using Cooker.Azure.KitchenStations.Arm;
using Cooker.Ingredients;
using Cooker.Ingredients.Storage;
using Cooker.Ingredients.WebApp;
using Cooker.Kitchens;
using Microsoft.Extensions.Options;

namespace Cooker.Azure.Ingredients.WebApp
{
    // ReSharper disable once UnusedType.Global
    public class WebAppBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        private readonly IOptions<AzureCookerSettings> _settings;

        public WebAppBuilder(WebAppIngredient ingredient, IOptions<AzureCookerSettings> settings)
        {
            _settings = settings;
            Ingredient = ingredient;
        }

        private WebAppIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var webAppName = (Ingredient.Id + "-" + platformContext.Randomness).ToLowerInvariant();
            var template = typeof(WebAppBuilder).GetResourceContents("WebApp");
            return new ArmRecipe<WebAppOutput>(
                new ArmDefinition(
                    $"webapp-{Ingredient.Id}",
                    template,
                    new Dictionary<string, object>()
                    {
                        { "name", webAppName },
                        {
                            "serverFarmId",
                            _settings.Value.PlatformAppServicePlans[Ingredient.TypedIngredientData.Classification]
                        },
                    }),
                output => new WebAppOutput(Ingredient.DisplayName));
        }
    }
}