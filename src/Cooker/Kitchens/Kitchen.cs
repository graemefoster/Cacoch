using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public class Kitchen<TContext> where TContext : IPlatformContext
    {
        private readonly KitchenStation<TContext>[] _stations;

        public Kitchen(IEnumerable<KitchenStation<TContext>> stations)
        {
            _stations = stations.Union(new[] { new TwoStepKitchenStation<TContext>(stations.ToArray()) }).ToArray();
        }

        /// <summary>
        /// Build what you can from the recipes that need preparing. Anything already cooked is provided to you
        /// to give you an opportunity to take a bit from it
        /// </summary>
        public (IIngredient input, Task<ICookedIngredient> output)[] CookNextRecipes(
            TContext context,
            Docket docket,
            IDictionary<IIngredient, IRecipe> allRecipes)
        {
            return allRecipes
                .Select(x =>
                    (x.Key, _stations.Single(s => s.CanCook(x.Value)).CookRecipe(context, docket, x.Value)))
                .ToArray();
        }
    }
}