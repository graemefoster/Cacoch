using System;
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
        private readonly Kitchen _kitchen;

        public Restaurant(
            Kitchen kitchen,
            CookbookLibrary<TContext> cookbookLibrary,
            IPlatformContextBuilder<TContext> platformContextBuilder)
        {
            _cookbookLibrary = cookbookLibrary;
            _platformContextBuilder = platformContextBuilder;
            _kitchen = kitchen;
        }


        public async Task<Meal> PlaceOrder(Docket docket)
        {
            var context = await _platformContextBuilder.Build(docket);

            var allRemainingInstructions =
                docket.LineItems.Select(x => new
                    {
                        Cookbook = _cookbookLibrary.GetCookbookFor(x),
                        Ingredient = x
                    })
                    .ToList();

            var cookedRecipes = new Dictionary<IIngredient, ICookedIngredient>();
            var intermediateRecipes = new Dictionary<IIngredient, IRecipe>();

            while (allRemainingInstructions.Any() || intermediateRecipes.Any())
            {
                var recipes =
                    allRemainingInstructions
                        .Where(x => x.Ingredient.PrepareForCook(cookedRecipes))
                        .Select(x => new
                            {LineItem = x.Ingredient, Instructions = x.Cookbook.CreateRecipe(context, cookedRecipes)})
                        .ToDictionary(x => x.LineItem, x => x.Instructions);

                foreach (var intermediateRecipe in intermediateRecipes)
                {
                    recipes.Add(intermediateRecipe.Key, intermediateRecipe.Value);
                }

                if (!recipes.Any())
                {
                    throw new InvalidOperationException(
                        "Remaining recipes but not can be built. Suspected dependency issue");
                }

                var cooked = await Task.WhenAll(_kitchen.CookNextRecipes(docket, recipes));

                intermediateRecipes = new Dictionary<IIngredient, IRecipe>();
                foreach (var batch in cooked)
                {
                    foreach (var edibleItem in batch)
                    {
                        if (edibleItem.Value is IRecipe edibleRecipe)
                        {
                            intermediateRecipes.Add(edibleItem.Key, edibleRecipe);
                        }
                        else
                        {
                            cookedRecipes.Add(edibleItem.Key, edibleItem.Value);
                        }
                    }
                }

                allRemainingInstructions.RemoveAll(rb => recipes.Keys.Contains(rb.Ingredient));
            }

            return new Meal(cookedRecipes);
        }

        // private class CookState
        // {
        //     public CookState(IIngredientBuilder builder, IIngredient ingredient)
        //     {
        //         Builder = builder;
        //         Ingredient = ingredient;
        //     }
        //
        //     public IIngredientBuilder Builder { get; }
        //     public IIngredient Ingredient { get; }
        // }
    }
}