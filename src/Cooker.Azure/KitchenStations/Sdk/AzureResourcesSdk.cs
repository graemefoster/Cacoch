using System;
using System.Threading.Tasks;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Ingredients;
using Microsoft.Extensions.Logging;

namespace Cooker.Azure.KitchenStations.Sdk
{
    class AzureResourcesSdk : IAzureResourcesSdk
    {
        private readonly IAzureSdkProvider _sdkProvider;
        private readonly ILogger<AzureResourcesSdk> _logger;

        public AzureResourcesSdk(IAzureSdkProvider sdkProvider, ILogger<AzureResourcesSdk> logger)
        {
            _sdkProvider = sdkProvider;
            _logger = logger;
        }
        public async Task<ICookedIngredient> Execute<TOutput>(AzurePlatformContext platformContext, Func<AzurePlatformContext, IAzureSdkProvider, Task<TOutput>> action) where TOutput : ICookedIngredient
        {
            _logger.LogDebug("Beginning execution of Azure SDK recipe");
            var response = await action(platformContext, _sdkProvider);
            _logger.LogDebug("Finished execution of Azure SDK recipe");
            return response;
        }
    }
}