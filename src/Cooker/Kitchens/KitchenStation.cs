using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public abstract class KitchenStation<TContext> where TContext: IPlatformContext
    {
        public abstract Task<ICookedIngredient> CookRecipe(TContext context, Docket docket, IRecipe recipe);
        public abstract bool CanCook(IRecipe recipe);
    }
}