using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public class ArmKitchenStation : KitchenStation
    {
        public override Task<ILineItemOutput> CookRecipe(ILineItem item, IRecipe recipe)
        {
            return Task.FromResult(((IArmRecipe)recipe).Output(new object()));
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is IArmRecipe;
        }
    }
}