using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Azure;
using Cacoch.Provider.AzureArm.Resources;
using Cacoch.Provider.AzureArm.Resources.OAuthClients;
using Cacoch.Provider.AzureArm.Resources.Secrets;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace Cacoch.Provider.AzureArm
{
    internal class AzureCacochManifestDeployer : ICacochManifestDeployer<AzurePlatformContext>
    {
        private readonly IArmBatchBuilder _armBatchBuilder;
        private readonly ILogger<AzureCacochManifestDeployer> _logger;
        private readonly IResourceGroupCreator _azureResourceGroupCreator;
        private readonly GraphServiceClient _graphServiceClient;

        public AzureCacochManifestDeployer(
            IArmBatchBuilder armBatchBuilder,
            ILogger<AzureCacochManifestDeployer> logger,
            IResourceGroupCreator azureResourceGroupCreator,
            GraphServiceClient graphServiceClient)
        {
            _armBatchBuilder = armBatchBuilder;
            _logger = logger;
            _azureResourceGroupCreator = azureResourceGroupCreator;
            _graphServiceClient = graphServiceClient;
        }

        public async Task Deploy(Manifest manifest, IPlatformTwin[] twins)
        {
            var allDeploymentArtifacts = new List<IDeploymentArtifact>();
            foreach (var twin in twins)
            {
                _logger.LogDebug("Building arm - Twin type {Type}. Twin platform now:{PlatformName}", twin.GetType().Name, twin.PlatformName);
                if (twin is OAuthClientTwin oauthClient)
                {
                    allDeploymentArtifacts.Add(await oauthClient.BuildDeploymentArtifact(twins));
                }
            }

            var aadArtifacts = allDeploymentArtifacts.OfType<AzureActiveDirectoryApiDeploymentArtifact>().ToArray();
            var knownSecrets = twins.OfType<SecretsTwin>().Single(x => x.ResourceName == "secrets");
            _logger.LogInformation("Beginning Azure Active Directory Deployment");

            foreach (var aadArtifact in aadArtifacts)
            {
                var output = await aadArtifact.Deploy(_graphServiceClient);
                if (output is LastMinuteSecretOutput secrets)
                {
                    knownSecrets.AddLastMinuteSecret(secrets);
                }
            }

            _logger.LogInformation("Beginning Arm Deployment");
            foreach (var twin in twins)
            {
                var deploymentArtifact = await twin.BuildDeploymentArtifact(twins);
                if (deploymentArtifact is AzureArmDeploymentArtifact arm)
                {
                    _armBatchBuilder.RegisterArm(twin, arm);
                }
            }
            
            var armOutputs = await _armBatchBuilder.Deploy(manifest.Slug);
            
            foreach (var aadArtifact in aadArtifacts)
            {
                await aadArtifact.PostDeploy(_graphServiceClient, armOutputs);
            }


            _logger.LogInformation("Finished Azure Deployment");
        }

        public async Task<AzurePlatformContext> PrepareContext(Manifest manifest)
        {
            await _azureResourceGroupCreator.CreateResourceGroupIfNotExists(manifest.Slug);
            return new AzurePlatformContext(await _azureResourceGroupCreator.GetResourceGroupRandomId(manifest.Slug));
        }
    }
}