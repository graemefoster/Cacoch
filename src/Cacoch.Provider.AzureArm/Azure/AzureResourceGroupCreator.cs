using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Provider.AzureArm.Resources;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal class AzureResourceGroupCreator : IResourceGroupCreator
    {
        private readonly IArmDeployer _armDeployer;
        private readonly ILogger<AzureResourceGroupCreator> _logger;
        private readonly ResourceManagementClient _resourceClient;
        private readonly IOptions<AzureArmSettings> _settings;
        private readonly Dictionary<string, string> _randomStrings = new();

        public AzureResourceGroupCreator(
            IArmDeployer armDeployer,
            ResourceManagementClient resourceClient,
            ILogger<AzureResourceGroupCreator> logger,
            IOptions<AzureArmSettings> settings)
        {
            _armDeployer = armDeployer;
            _logger = logger;
            _resourceClient = resourceClient;
            _settings = settings;
        }

        public async Task CreateResourceGroupIfNotExists(string resourceGroup)
        {
            _logger.LogDebug("Ensuring resource group {ResourceGroup} in {Location} exists", resourceGroup,
                _settings.Value.PrimaryLocation);

            try
            {
                await _resourceClient.ResourceGroups.GetAsync(resourceGroup);
            }
            catch (RestException re)
            {
                await _resourceClient.ResourceGroups.CreateOrUpdateAsync(resourceGroup,
                    new ResourceGroup(_settings.Value.PrimaryLocation));
            }
        }

        public async Task<string> GetResourceGroupRandomId(string resourceGroup)
        {
            if (_randomStrings.ContainsKey(resourceGroup))
            {
                return _randomStrings[resourceGroup];
            }

            _logger.LogDebug("Fetching random-id to help with unique resource names");
            var outputs = await _armDeployer.Deploy(
                resourceGroup,
                await typeof(AzureResourceGroupCreator).GetResourceContents("ResourceGroupRandomId"),
                new Dictionary<string, object>()
            );

            var resourceGroupRandomId = outputs.Single().Value["randomId"];
            _randomStrings.Add(resourceGroup, resourceGroupRandomId);
            _logger.LogDebug("Fetched random id {RandomId} for resource group {ResourceGroup}", resourceGroupRandomId,
                resourceGroup);

            return resourceGroupRandomId;
        }
    }
}