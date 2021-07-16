using System.Threading.Tasks;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure
{
    public class AzureSdkKitchenStation : KitchenStation
    {
        private readonly ISecretSdk _secretSdk;

        public AzureSdkKitchenStation(ISecretSdk secretSdk)
        {
            _secretSdk = secretSdk;
        }
        
        public override Task<ICookedIngredient> CookRecipe(Docket docket, IRecipe recipe)
        {
            return ((ISecretRecipe)recipe).Execute(docket, _secretSdk);
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is ISecretRecipe;
        }
    }
}