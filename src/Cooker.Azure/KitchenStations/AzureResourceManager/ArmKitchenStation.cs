using System.Threading.Tasks;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure.KitchenStations.AzureResourceManager
{
    public class ArmKitchenStation : KitchenStation<AzurePlatformContext>
    {
        private readonly IArmRunner _armRunner;

        public ArmKitchenStation(IArmRunner armRunner)
        {
            _armRunner = armRunner;
        }

        public override Task<ICookedIngredient> CookRecipe(AzurePlatformContext platformContext, Docket docket,
            IRecipe recipe)
        {
            return ((IArmRecipe) recipe).Execute(platformContext, docket, _armRunner);
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is IArmRecipe;
        }
    }
}