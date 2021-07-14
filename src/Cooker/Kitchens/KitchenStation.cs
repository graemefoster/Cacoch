using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public abstract class KitchenStation
    {
        public abstract Task<ILineItemOutput> CookRecipe(Docket docket, IRecipe recipe);
        public abstract bool CanCook(IRecipe recipe);
    }
}