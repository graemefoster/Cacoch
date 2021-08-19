using System;
using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Azure.KitchenStations.Sdk
{
    public interface IAzureResourcesSdk
    {
        Task<ICookedIngredient> Execute<TOutput>(AzurePlatformContext platformContext, Func<AzurePlatformContext, IAzureSdkProvider, Task<TOutput>> action) where TOutput : ICookedIngredient;
    }
}