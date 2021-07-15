using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker
{
    public class Restaurant
    {
        private readonly CookbookLibrary _cookbookLibrary;
        private readonly Kitchen _kitchen;

        public Restaurant(
            Kitchen kitchen,
            CookbookLibrary cookbookLibrary)
        {
            _cookbookLibrary = cookbookLibrary;
            _kitchen = kitchen;
        }


        public async Task<Meal> PlaceOrder(Docket docket)
        {
            var allRemainingInstructions= docket.LineItems.Select(x => _cookbookLibrary.GetCookbookFor(x)).ToList();
            var cookedRecipes = new Dictionary<IIngredient, ICookedIngredient>();
            var intermediateRecipes = new Dictionary<IIngredient, IRecipe>();

            while (allRemainingInstructions.Any() || intermediateRecipes.Any())
            {
                var recipes = 
                    allRemainingInstructions
                        .Where(x => x.Ingredient.PrepareForCook(cookedRecipes))
                        .Select(x => new {LineItem = x.Ingredient, Instructions = x.CreateRecipe(cookedRecipes) })
                        .ToDictionary(x => x.LineItem, x => x.Instructions);

                foreach (var intermediateRecipe in intermediateRecipes)
                {
                    recipes.Add(intermediateRecipe.Key, intermediateRecipe.Value);
                }
                
                if (!recipes.Any())
                {
                    throw new InvalidOperationException("Remaining recipes but not can be built. Suspected dependency issue");
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
    }
}