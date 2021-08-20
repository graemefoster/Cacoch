using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Ingredients;
using Cooker.Kitchens;
using Microsoft.Extensions.Logging;

namespace Cooker
{
    public class Restaurant<TContext> : IRestaurant where TContext : IPlatformContext
    {
        private readonly CookbookLibrary<TContext> _cookbookLibrary;
        private readonly IPlatformContextBuilder<TContext> _platformContextBuilder;
        private readonly ILogger<Restaurant<TContext>> _logger;
        private readonly Kitchen<TContext> _kitchen;

        private record IngredientAndCookbook(IIngredient Ingredient, IRecipeBuilder<TContext> Cookbook);

        private record IngredientAndRecipe(IIngredient Ingredient, IRecipe Recipe);

        public Restaurant(
            Kitchen<TContext> kitchen,
            CookbookLibrary<TContext> cookbookLibrary,
            IPlatformContextBuilder<TContext> platformContextBuilder,
            ILogger<Restaurant<TContext>> logger)
        {
            _cookbookLibrary = cookbookLibrary;
            _platformContextBuilder = platformContextBuilder;
            _logger = logger;
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

            var runningTasks = new Dictionary<Task<ICookedIngredient>, IIngredient>();
            
            _logger.LogInformation("Restaurant is cooking the meal");
            while (allRemainingInstructions.Any() || twoStageRecipesBeingCooked.Any())
            {
                var recipesReadyForCooking =
                    CreateRecipesThatAreReadyForCooking(docket, platformEnvironment, allRemainingInstructions,
                        cookedRecipes, context);

                foreach (var intermediateRecipe in twoStageRecipesBeingCooked)
                {
                    recipesReadyForCooking.Add(intermediateRecipe.Ingredient, intermediateRecipe.Recipe);
                }
                twoStageRecipesBeingCooked.Clear();

                allRemainingInstructions.RemoveAll(rb => recipesReadyForCooking.Keys.Contains(rb.Ingredient));

                if (!recipesReadyForCooking.Any() && !runningTasks.Any())
                {
                    throw new InvalidOperationException(
                        $"Remaining recipes but not can be built. Suspected dependency issue. Remaining recipes: {string.Join(',', allRemainingInstructions.Select(x => x.Ingredient.Id))}.");
                }

                if (recipesReadyForCooking.Any())
                {
                    _logger.LogInformation("Restaurant is cooking {Ingredients}", string.Join(",", recipesReadyForCooking.Select(x => x.Key.Id)));
                }

                var nextRecipesBeingCooked = _kitchen.CookNextRecipes(context, docket, recipesReadyForCooking);
                foreach (var recipe in nextRecipesBeingCooked)
                {
                    runningTasks.Add(recipe.output, recipe.input);
                }
                
                //wait for anyone of the things being cooked to finish. Then see if we can cook anything else.
                var nextFinished = await Task.WhenAny(runningTasks.Keys);
                var justCooked = await nextFinished;
                var justCookedIngredient = runningTasks[nextFinished];
                if (justCooked is IRecipe edibleRecipe)
                {
                    _logger.LogInformation("Ingredient {Ingredient} produced new recipe {Recipe}", justCookedIngredient.Id, edibleRecipe.GetType().Name);
                    twoStageRecipesBeingCooked.Add(new IngredientAndRecipe(justCookedIngredient, edibleRecipe));
                }
                else
                {
                    _logger.LogInformation("Ingredient {Ingredient} produced output {Edible}", justCookedIngredient.Id, justCooked.GetType().Name);
                    cookedRecipes.Add(justCookedIngredient, justCooked);
                }

                runningTasks.Remove(nextFinished);
            }
            _logger.LogInformation("Restaurant has cooked meal");
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