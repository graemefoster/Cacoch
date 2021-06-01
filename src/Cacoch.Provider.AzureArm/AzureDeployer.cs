using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Cacoch.Provider.AzureArm.Resources;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Cacoch.Provider.AzureArm
{
    internal class AzureDeployer : IPlatformDeployer, IAzureArmDeployer
    {
        private readonly ILogger<AzureDeployer> _logger;
        private readonly ResourceManagementClient _resourceClient;

        public AzureDeployer(ServiceClientCredentials credentials, ILogger<AzureDeployer> logger, IOptions<AzureArmSettings> settings)
        {
            _logger = logger;
            _resourceClient = new ResourceManagementClient(credentials)
            {
                SubscriptionId = settings.Value.SubscriptionId
            };
        }

        Task<DeploymentExtended> IAzureArmDeployer.DeployArm(string contents, Dictionary<string, object> parameters)
        {
            var armParameters = new Dictionary<string, object>(parameters.Select(x =>
            {
                if (x.Value is { } stringValue)
                {
                    return new KeyValuePair<string, object>(
                        x.Key,
                        new
                        {
                            value = stringValue
                        });
                }

                throw new NotSupportedException($"Unsupported parameter type - {x.Value.GetType()}");
            }));

            return _resourceClient.Deployments.CreateOrUpdateAsync("CacochTest", Guid.NewGuid().ToString(),
                new Deployment(
                    new DeploymentProperties(DeploymentMode.Incremental,
                        contents,
                        null,
                        JsonConvert.SerializeObject(armParameters)
                    )));
        }

        public async Task Deploy(Manifest manifest, IPlatformTwin[] twins)
        {
            await CreateResourceGroupIfNotExists(manifest.Slug);

            foreach (var twin in twins)
            {
                var deploymentArtifact = (AzureArmDeploymentArtifact) await twin.BuildDeploymentArtifact();
                await deploymentArtifact.Deploy(this);
            }
        }

        private async Task CreateResourceGroupIfNotExists(string manifestSlug)
        {
            //TODO - abstract location away
            _logger.LogDebug("Ensuring resource group {ResourceGroup} exists", manifestSlug);
            await _resourceClient.ResourceGroups.CreateOrUpdateAsync(manifestSlug, new ResourceGroup("australiaeast"));
        }
    }
}