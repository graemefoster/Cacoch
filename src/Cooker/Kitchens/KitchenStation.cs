using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    public abstract class KitchenStation
    {
        public abstract Task<ILineItemOutput> CookRecipe(ILineItem item, IRecipe recipe1);
        public abstract bool CanCook(IRecipe recipe);
    }
}