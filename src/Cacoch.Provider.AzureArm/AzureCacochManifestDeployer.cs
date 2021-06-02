using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Azure;
using Cacoch.Provider.AzureArm.Resources;
using Microsoft.Extensions.Logging;

namespace Cacoch.Provider.AzureArm
{
    internal class AzureCacochManifestDeployer : ICacochManifestDeployer
    {
        private readonly IArmDeployer _armDeployer;
        private readonly ILogger<AzureCacochManifestDeployer> _logger;
        private readonly IResourceGroupCreator _azureResourceGroupCreator;
        private readonly CacochPlatform _cacochPlatform;

        public AzureCacochManifestDeployer(
            IArmDeployer armDeployer,
            ILogger<AzureCacochManifestDeployer> logger,
            IResourceGroupCreator azureResourceGroupCreator,
            CacochPlatform cacochPlatform)
        {
            _armDeployer = armDeployer;
            _logger = logger;
            _azureResourceGroupCreator = azureResourceGroupCreator;
            _cacochPlatform = cacochPlatform;
        }

        public async Task Deploy(Manifest manifest, IPlatformTwin[] twins)
        {
            await _cacochPlatform.BuildFoundation();
            await _azureResourceGroupCreator.CreateResourceGroupIfNotExists(manifest.Slug);
            var uniqueId = await _azureResourceGroupCreator.GetResourceGroupRandomId(manifest.Slug);

            foreach (var twin in twins)
            {
                _logger.LogDebug("  Deploying twin type {Type} for {Resource}", twin.GetType().Name, twin.Name);
                var deploymentArtifact = (AzureArmDeploymentArtifact) await twin.BuildDeploymentArtifact();
                await _armDeployer.DeployArm(manifest.Slug, deploymentArtifact.PlatformIdentifier,
                    deploymentArtifact.Arm,
                    deploymentArtifact.Parameters);
            }
        }
    }
}