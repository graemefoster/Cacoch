using System.Threading.Tasks;
using Azure.ResourceManager.Storage;
using Cacoch.Core.Manifest;
using Cacoch.Provider.AzureArm.Azure;
using Cacoch.Provider.AzureArm.Resources;
using Cacoch.Provider.AzureArm.Resources.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cacoch.Provider.AzureArm
{
    internal class CacochPlatform
    {
        private readonly IResourceGroupCreator _azureResourceGroupCreator;
        private readonly ILogger<CacochPlatform> _logger;
        private readonly IArmDeployer _platformDeployer;
        private readonly IOptions<AzureArmSettings> _settings;

        public CacochPlatform(
            ILogger<CacochPlatform> logger,
            IArmDeployer platformDeployer,
            IResourceGroupCreator azureResourceGroupCreator,
            IOptions<AzureArmSettings> settings)
        {
            _azureResourceGroupCreator = azureResourceGroupCreator;
            _settings = settings;
            _logger = logger;
            _platformDeployer = platformDeployer;
        }

        internal async Task BuildFoundation()
        {
            await _azureResourceGroupCreator.CreateResourceGroupIfNotExists(_settings.Value.PlatformResources);
            var uniqueId = await _azureResourceGroupCreator.GetResourceGroupRandomId(_settings.Value.PlatformResources);

            var storageTwin =
                (AzureArmDeploymentArtifact) await new StorageTwin(
                        new Storage(_settings.Value.PlatformStorage + "-" + uniqueId,
                            new[] {"armtemplates"}))
                    .BuildDeploymentArtifact();

            await _platformDeployer.DeployArm(
                _settings.Value.PlatformResources,
                storageTwin.PlatformIdentifier,
                storageTwin.Arm,
                storageTwin.Parameters);
        }
    }
}