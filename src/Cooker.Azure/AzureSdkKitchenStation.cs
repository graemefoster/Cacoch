using System.Threading.Tasks;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure
{
    public class AzureSdkKitchenStation : KitchenStation
    {
        private readonly IAzureResourcesSdk _azureResourcesSdk;

        public AzureSdkKitchenStation(IAzureResourcesSdk azureResourcesSdk)
        {
            _azureResourcesSdk = azureResourcesSdk;
        }
        
        public override Task<ICookedIngredient> CookRecipe(Docket docket, IRecipe recipe)
        {
            return ((ISecretRecipe)recipe).Execute(docket, _azureResourcesSdk);
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is ISecretRecipe;
        }
    }
}