using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public class ArmKitchen : Kitchen
    {
        public override Task<Dictionary<ILineItem, IEdible>> PrepareBatch(
            IEnumerable<Recipe> allRecipes,
            IDictionary<ILineItem, IEdible> cookedRecipes)
        {
            var armRecipe = (ArmRecipe) allRecipes.Single();
            return Task.FromResult(new Dictionary<ILineItem, IEdible>()
            {
                {armRecipe.LineItem, armRecipe.Output(new object())}
            });
        }
    }
}