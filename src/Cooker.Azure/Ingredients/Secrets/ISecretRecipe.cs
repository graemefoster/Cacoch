using System.Threading.Tasks;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Secrets
{
    public interface ISecretRecipe
    {
        Task<ICookedIngredient> Execute(Docket docket, IAzureResourcesSdk sdk);
    }
}