using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Kitchens.AzureArm;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    public class Kitchen
    {
        private readonly IEnumerable<KitchenStation> _stations;

        public Kitchen(IEnumerable<KitchenStation> stations)
        {
            _stations = stations.ToArray();
        }

        /// <summary>
        /// Build what you can from the recipes that need preparing. Anything already cooked is provided to you
        /// to give you an opportunity to take a bit from it
        /// </summary>
        public async Task<Dictionary<ILineItem, ILineItemOutput>> CookNextRecipes(
            IDictionary<ILineItem, IRecipe> allRecipes)
        {
            var allCooks = allRecipes
                .Select(x =>
                    new
                    {
                        Input = x.Key,
                        Output = _stations.Single(s => s.CanCook(x.Value)).CookRecipe(x.Key, x.Value)
                    })
                .ToArray();

            await Task.WhenAll(allCooks.Select(x => x.Output));
            return allCooks.ToDictionary(x => x.Input, x => x.Output.Result);
        }
    }
}