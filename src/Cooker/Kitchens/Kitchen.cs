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
            _stations = stations;
        }

        /// <summary>
        /// Build what you can from the recipes that need preparing. Anything already cooked is provided to you
        /// to give you an opportunity to take a bit from it
        /// </summary>
        public async Task<Dictionary<ILineItem, ILineItemOutput>> CookNextRecipes(
            IDictionary<ILineItem, Recipe> allRecipes)
        {
            var stationOutputs = await Task.WhenAll(_stations.Select(station => station.CookNextRecipes(allRecipes)));
            var mergedOutputs = new Dictionary<ILineItem, ILineItemOutput>();
            foreach (var stationOutput in stationOutputs)
            {
                foreach (var itemOutput in stationOutput)
                {
                    mergedOutputs.Add(itemOutput.Key, itemOutput.Value);
                }
            }

            return mergedOutputs;
        }
    }
}