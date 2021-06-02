using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Cacoch.Provider.AzureArm.Resources.Storage;
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
            ServiceClientCredentials credentials,
            ILogger<AzureResourceGroupCreator> logger,
            IOptions<AzureArmSettings> settings)
        {
            _armDeployer = armDeployer;
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

        public async Task<string> GetResourceGroupRandomId(string resourceGroup)
        {
            if (_randomStrings.ContainsKey(resourceGroup))
            {
                return _randomStrings[resourceGroup];
            }
            
            var outputs = await _armDeployer.Deploy(
                resourceGroup,
                await typeof(AzureResourceGroupCreator).GetResourceContents("ResourceGroupRandomId"),
                new Dictionary<string, object>()
            );

            var resourceGroupRandomId = (string)((dynamic)outputs.Properties.Outputs).randomId.value;
            _randomStrings.Add(resourceGroup, resourceGroupRandomId);
            
            return resourceGroupRandomId;
        }
    }
}