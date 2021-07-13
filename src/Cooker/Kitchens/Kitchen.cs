using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    public abstract class Kitchen
    {
        /// <summary>
        /// Build what you can from the recipes that need preparing. Anything already cooked is provided to you
        /// to give you an opportunity to take a bit from it
        /// </summary>
        /// <param name="recipes"></param>
        /// <returns></returns>
        public abstract Task<Dictionary<ILineItem, IEdible>> CookNextRecipes(
            IEnumerable<Recipe> recipes);
    }
}