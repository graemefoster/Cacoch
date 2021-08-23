using System.Threading.Tasks;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure.KitchenStations.AzureResourceManager
{
    public interface IArmRecipe
    {
        Task<ICookedIngredient> Execute(AzurePlatformContext platformContext,  Docket docket, IArmRunner armRunner);
    }
}