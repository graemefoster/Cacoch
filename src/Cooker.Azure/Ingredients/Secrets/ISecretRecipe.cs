using System.Threading.Tasks;
using Cooker.Azure.KitchenStations.Sdk;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Secrets
{
    public interface ISecretRecipe<TContext> where TContext: IPlatformContext
    {
        Task<ICookedIngredient> Execute(TContext context, Docket docket, IAzureResourcesSdk sdk);
    }
}