using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Kitchens;
using Cooker.Recipes;

namespace Cooker
{
    public class Restaurant
    {
        private readonly RecipeBook _recipeBook;
        private readonly IEnumerable<Kitchen> _stations;

        public Restaurant(
            IEnumerable<Kitchen> stations,
            RecipeBook recipeBook)
        {
            _recipeBook = recipeBook;
            _stations = stations;
        }


        public async Task<Meal> PlaceOrder(Docket docket)
        {
            var allRemainingRecipes= docket.LineItems.Select(x => _recipeBook.Lookup(x)).ToList();
            var allBuiltRecipes = new Dictionary<ILineItem, IEdible>();

            while (allRemainingRecipes.Any())
            {
                var nextBatches = await Task.WhenAll(_stations.Select(async x => await x.PrepareBatch(allRemainingRecipes, allBuiltRecipes)));
                var newlyCooked = nextBatches.SelectMany(x => x.Keys).ToArray();
                
                foreach (var batch in nextBatches)
                {
                    foreach (var edibleItem in batch)
                    {
                        allBuiltRecipes.Add(edibleItem.Key, edibleItem.Value);
                    }
                }

                foreach (var cooked in newlyCooked)
                {
                    allRemainingRecipes.Remove(allRemainingRecipes.Single(x => x.LineItem == cooked));
                }
            }

            return new Meal(allBuiltRecipes);
        }
    }
}