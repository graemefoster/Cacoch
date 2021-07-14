using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens.Azure
{
    public class ArmKitchenStation : KitchenStation
    {
        public override Task<ILineItemOutput> CookRecipe(IRecipe recipe)
        {
            return Task.FromResult(((IArmRecipe)recipe).Output(new object()));
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is IArmRecipe;
        }
    }
}