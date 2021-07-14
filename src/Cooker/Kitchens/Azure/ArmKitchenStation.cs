using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Kitchens.Azure
{
    public class ArmKitchenStation : KitchenStation
    {
        private readonly IArmRunner _armRunner;

        public ArmKitchenStation(IArmRunner armRunner)
        {
            _armRunner = armRunner;
        }
        
        public override Task<ILineItemOutput> CookRecipe(Docket docket, IRecipe recipe)
        {
            return ((IArmRecipe)recipe).Execute(docket, _armRunner);
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is IArmRecipe;
        }
    }
}