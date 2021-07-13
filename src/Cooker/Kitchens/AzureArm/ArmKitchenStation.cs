using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public class ArmKitchenStation : KitchenStation
    {
        public override Task<Dictionary<ILineItem, ILineItemOutput>> CookNextRecipes(
            IDictionary<ILineItem, Recipe> allRecipes)
        {
            var response = new Dictionary<ILineItem, ILineItemOutput>();
            foreach (var recipe in allRecipes.Where(x => x.Value is ArmRecipe))
            {
                response.Add(recipe.Key, ((ArmRecipe) recipe.Value).Output(new object()));
            }

            return Task.FromResult(response);
        }

    }
}