﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker
{
    public class Restaurant<TContext> : IRestaurant where TContext : IPlatformContext
    {
        private readonly CookbookLibrary<TContext> _cookbookLibrary;
        private readonly IPlatformContextBuilder<TContext> _platformContextBuilder;
        private readonly Kitchen<TContext> _kitchen;

        private record IngredientAndCookbook(IIngredient Ingredient, IRecipeBuilder<TContext> Cookbook);

        private record IngredientAndRecipe(IIngredient Ingredient, IRecipe Recipe);

        public Restaurant(
            Kitchen<TContext> kitchen,
            CookbookLibrary<TContext> cookbookLibrary,
            IPlatformContextBuilder<TContext> platformContextBuilder)
        {
            _cookbookLibrary = cookbookLibrary;
            _platformContextBuilder = platformContextBuilder;
            _kitchen = kitchen;
        }


        public async Task<Meal> PlaceOrder(PlatformEnvironment platformEnvironment, Docket docket)
        {
            var context = await _platformContextBuilder.Build(docket, platformEnvironment);

            var allRemainingInstructions =
                docket.LineItems
                    .SelectMany(x => x.GatherIngredients())
                    .Select(x => new IngredientAndCookbook(x, _cookbookLibrary.GetCookbookFor(x)))
                    .ToList();

            var cookedRecipes = new Dictionary<IIngredient, ICookedIngredient>();
            var twoStageRecipesBeingCooked = new List<IngredientAndRecipe>();

            while (allRemainingInstructions.Any() || twoStageRecipesBeingCooked.Any())
            {
                var recipesReadyForCooking =
                    CreateRecipesThatAreReadyForCooking(docket, platformEnvironment, allRemainingInstructions,
                        cookedRecipes, context);

                foreach (var intermediateRecipe in twoStageRecipesBeingCooked)
                {
                    recipesReadyForCooking.Add(intermediateRecipe.Ingredient, intermediateRecipe.Recipe);
                }

                if (!recipesReadyForCooking.Any())
                {
                    throw new InvalidOperationException(
                        $"Remaining recipes but not can be built. Suspected dependency issue. Remaining recipes: {string.Join(',', allRemainingInstructions.Select(x => x.Ingredient.Id))}.");
                }

                var cooked = await Task.WhenAll(_kitchen.CookNextRecipes(context, docket, recipesReadyForCooking));

                twoStageRecipesBeingCooked = new List<IngredientAndRecipe>();
                foreach (var batch in cooked)
                {
                    foreach (var edibleItem in batch)
                    {
                        if (edibleItem.Value is IRecipe edibleRecipe)
                        {
                            twoStageRecipesBeingCooked.Add(new IngredientAndRecipe(edibleItem.Key, edibleRecipe));
                        }
                        else
                        {
                            cookedRecipes.Add(edibleItem.Key, edibleItem.Value);
                        }
                    }
                }

                allRemainingInstructions.RemoveAll(rb => recipesReadyForCooking.Keys.Contains(rb.Ingredient));
            }

            return new Meal(cookedRecipes.ToDictionary(x => x.Key.OriginalIngredientData, x => x.Value));
        }

        private static Dictionary<IIngredient, IRecipe> CreateRecipesThatAreReadyForCooking(
            Docket docket,
            PlatformEnvironment environment,
            List<IngredientAndCookbook> allRemainingInstructions,
            Dictionary<IIngredient, ICookedIngredient> cookedRecipes,
            TContext context)
        {
            return allRemainingInstructions
                .Where(x => x.Ingredient.PrepareForCook(cookedRecipes))
                .Select(x =>
                    new IngredientAndRecipe(x.Ingredient,
                        x.Cookbook.CreateRecipe(context, environment, docket, cookedRecipes)))
                .ToDictionary(x => x.Ingredient, x => x.Recipe);
        }
    }
}