using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public class ArmKitchen : Kitchen
    {
        public override Task<Dictionary<ILineItem, IEdible>> CookNextRecipes(
            IEnumerable<Recipe> allRecipes)
        {
            var response = new Dictionary<ILineItem, IEdible>();
            foreach (var recipe in allRecipes.OfType<ArmRecipe>())
            {
                response.Add(recipe.LineItem, recipe.Output(new object()));
            }

            return Task.FromResult(response);
        }
    }
}