using System;
using System.Threading.Tasks;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure
{
    public class AzureSdkRecipe<TOutput> : Recipe<AzurePlatformContext, TOutput>,
        ISecretRecipe<AzurePlatformContext>
        where TOutput : ICookedIngredient
    {
        private readonly Func<AzurePlatformContext, IAzureSdkProvider, Task<TOutput>> _action;

        public AzureSdkRecipe(Func<AzurePlatformContext, IAzureSdkProvider, Task<TOutput>> action)
        {
            _action = action;
        }

        public Task<ICookedIngredient> Execute(AzurePlatformContext context, Docket docket, IAzureResourcesSdk sdk)
        {
            return sdk.Execute(context, _action);
        }
    }
}