using System.Threading.Tasks;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure.KitchenStations.Sdk
{
    public class AzureSdkKitchenStation : KitchenStation<AzurePlatformContext>
    {
        private readonly IAzureResourcesSdk _azureResourcesSdk;

        public AzureSdkKitchenStation(IAzureResourcesSdk azureResourcesSdk)
        {
            _azureResourcesSdk = azureResourcesSdk;
        }

        public override Task<ICookedIngredient> CookRecipe(AzurePlatformContext platformContext, Docket docket,
            IRecipe recipe)
        {
            return ((ISecretRecipe<AzurePlatformContext>) recipe).Execute(platformContext, docket, _azureResourcesSdk);
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is ISecretRecipe<AzurePlatformContext>;
        }
    }
}