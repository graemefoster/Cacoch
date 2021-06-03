using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Azure;
using Cacoch.Provider.AzureArm.Resources;
using Microsoft.Extensions.Logging;

namespace Cacoch.Provider.AzureArm
{
    internal class AzureCacochManifestDeployer : ICacochManifestDeployer<AzurePlatformContext>
    {
        private readonly IArmBatchBuilder _armBatchBuilder;
        private readonly ILogger<AzureCacochManifestDeployer> _logger;
        private readonly IResourceGroupCreator _azureResourceGroupCreator;

        public AzureCacochManifestDeployer(
            IArmBatchBuilder armBatchBuilder,
            ILogger<AzureCacochManifestDeployer> logger,
            IResourceGroupCreator azureResourceGroupCreator)
        {
            _armBatchBuilder = armBatchBuilder;
            _logger = logger;
            _azureResourceGroupCreator = azureResourceGroupCreator;
        }

        public async Task Deploy(Manifest manifest, IPlatformTwin[] twins)
        {
            foreach (var twin in twins)
            {
                _logger.LogDebug("Building arm - Twin type {Type}. Twin platform now:{PlatformName}", twin.GetType().Name, twin.PlatformName);
                _armBatchBuilder.RegisterArm(twin, (AzureArmDeploymentArtifact) await twin.BuildDeploymentArtifact(twins));
            }

            _logger.LogInformation("Beginning Azure Deployment");
            await _armBatchBuilder.Deploy(manifest.Slug);
            _logger.LogInformation("Finished Azure Deployment");
        }

        public async Task<AzurePlatformContext> PrepareContext(Manifest manifest)
        {
            await _azureResourceGroupCreator.CreateResourceGroupIfNotExists(manifest.Slug);
            return new AzurePlatformContext(await _azureResourceGroupCreator.GetResourceGroupRandomId(manifest.Slug));
        }
    }
}