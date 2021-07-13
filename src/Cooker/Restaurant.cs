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
        private readonly IEnumerable<Kitchen> _stations;

        public Restaurant(
            IEnumerable<Kitchen> stations,
            Bookshelf bookshelf)
        {
            _bookshelf = bookshelf;
            _stations = stations;
        }


        public async Task<Meal> PlaceOrder(Docket docket)
        {
            var allRemainingCookbooks= docket.LineItems.Select(x => _bookshelf.GetCookbookFor(x)).ToList();
            var allBuiltRecipes = new Dictionary<ILineItem, IEdible>();

            while (allRemainingCookbooks.Any())
            {
                var readyToCook = allRemainingCookbooks.Where(x => x.CanCook(allBuiltRecipes)).ToArray();
                if (readyToCook.Length == 0)
                {
                    throw new InvalidOperationException("Remaining recipes but not can be built. Suspected dependency issue");
                }

                var recipes = readyToCook.Select(x => x.BuildCookingInstructions(allBuiltRecipes));
                var nextBatches = await Task.WhenAll(_stations.Select(async x => await x.CookNextRecipes(recipes)));
                
                foreach (var batch in nextBatches)
                {
                    foreach (var edibleItem in batch)
                    {
                        allBuiltRecipes.Add(edibleItem.Key, edibleItem.Value);
                    }
                }
                allRemainingCookbooks.RemoveAll(readyToCook.Contains);
            }

            return new Meal(allBuiltRecipes);
        }
    }
}