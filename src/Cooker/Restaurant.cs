using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Kitchens;
using Cooker.Recipes;

namespace Cooker
{
    public class Restaurant
    {
        private readonly Bookshelf _bookshelf;
        private readonly Kitchen _kitchen;

        public Restaurant(
            Kitchen kitchen,
            Bookshelf bookshelf)
        {
            _bookshelf = bookshelf;
            _kitchen = kitchen;
        }


        public async Task<Meal> PlaceOrder(Docket docket)
        {
            var allRemainingInstructions= docket.LineItems.Select(x => _bookshelf.GetCookbookFor(x)).ToList();
            var cookedRecipes = new Dictionary<ILineItem, ILineItemOutput>();

            while (allRemainingInstructions.Any())
            {
                var recipes = 
                    allRemainingInstructions
                        .Where(x => x.CanCook(cookedRecipes))
                        .Select(x => new {x.LineItem, Instructions = x.BuildCookingInstructions(cookedRecipes) })
                        .ToDictionary(x => x.LineItem, x => x.Instructions);

                if (!recipes.Any())
                {
                    throw new InvalidOperationException("Remaining recipes but not can be built. Suspected dependency issue");
                }
                var cooked = await Task.WhenAll(_kitchen.CookNextRecipes(recipes));
                
                foreach (var batch in cooked)
                {
                    foreach (var edibleItem in batch)
                    {
                        cookedRecipes.Add(edibleItem.Key, edibleItem.Value);
                    }
                }
                allRemainingInstructions.RemoveAll(rb => cookedRecipes.Keys.Contains(rb.LineItem));
            }

            return new Meal(cookedRecipes);
        }
    }
}