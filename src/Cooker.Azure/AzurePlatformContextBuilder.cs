using System.Threading.Tasks;
using Azure.ResourceManager.Resources.Models;
using Cooker.Azure.KitchenStations.Sdk;
using Cooker.Kitchens;
using Microsoft.Extensions.Options;

namespace Cooker.Azure
{
    class AzurePlatformContextBuilder : IPlatformContextBuilder<AzurePlatformContext>
    {
        private readonly IAzureSdkProvider _azureSdkProvider;
        private readonly IOptions<AzureCookerSettings> _platformSettings;

        public AzurePlatformContextBuilder(
            IAzureSdkProvider azureSdkProvider,
            IOptions<AzureCookerSettings> platformSettings)
        {
            _azureSdkProvider = azureSdkProvider;
            _platformSettings = platformSettings;
        }

        public async Task<AzurePlatformContext> Build(Docket docket, PlatformEnvironment platformEnvironment)
        {
            var azurePlatformContext = new AzurePlatformContext(docket, _platformSettings.Value, platformEnvironment);
            var rgProvider = _azureSdkProvider.GetResourcesManagementClient();
            await rgProvider.ResourceGroups.CheckExistenceAsync(azurePlatformContext.ResourceGroupName);

            await rgProvider.ResourceGroups.CreateOrUpdateAsync(
                azurePlatformContext.ResourceGroupName,
                new ResourceGroup(_platformSettings.Value.PrimaryLocation));

            return azurePlatformContext;
        }
    }
}