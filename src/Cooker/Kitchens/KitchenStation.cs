using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    public abstract class KitchenStation
    {

        public abstract Task<Dictionary<ILineItem, ILineItemOutput>> CookNextRecipes(
            IDictionary<ILineItem, Recipe> allRecipes);
    }
}