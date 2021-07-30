using System;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources;
using Cooker.Ingredients;

namespace Cooker.Azure.Ingredients.Secrets
{
    public interface IAzureResourcesSdk
    {
        Task<ICookedIngredient> Execute<TOutput>(AzurePlatformContext platformContext, Func<AzurePlatformContext, IAzureSdkProvider, Task<TOutput>> action) where TOutput : ICookedIngredient;
    }
}