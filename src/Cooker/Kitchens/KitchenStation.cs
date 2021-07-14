using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    public abstract class KitchenStation
    {
        public abstract Task<ILineItemOutput> CookRecipe(IRecipe recipe);
        public abstract bool CanCook(IRecipe recipe);
    }
}