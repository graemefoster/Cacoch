using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal class AzureResourceGroupCreator : IResourceGroupCreator
    {
        private readonly ILogger<AzureResourceGroupCreator> _logger;
        private readonly ResourceManagementClient _resourceClient;
        private readonly IOptions<AzureArmSettings> _settings;

        public AzureResourceGroupCreator(
            ServiceClientCredentials credentials,
            ILogger<AzureResourceGroupCreator> logger,
            IOptions<AzureArmSettings> settings)
        {
            _logger = logger;
            _resourceClient = new ResourceManagementClient(credentials)
            {
                SubscriptionId = settings.Value.SubscriptionId
            };
            _settings = settings;
        }

        public async Task CreateResourceGroupIfNotExists(string resourceGroup)
        {
            _logger.LogDebug("Ensuring resource group {ResourceGroup} in {Location} exists", resourceGroup,
                _settings.Value.PrimaryLocation);
            
            await _resourceClient.ResourceGroups.CreateOrUpdateAsync(resourceGroup,
                new ResourceGroup(_settings.Value.PrimaryLocation));
        }
    }
}