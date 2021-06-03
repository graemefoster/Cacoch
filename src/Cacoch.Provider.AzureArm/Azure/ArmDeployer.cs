using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Manifest.Secrets;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal class ArmDeployer : IArmDeployer
    {
        private readonly ILogger<ArmDeployer> _logger;
        private readonly ResourceManagementClient _resourceClient;

        public ArmDeployer(
            ServiceClientCredentials credentials,
            IOptions<AzureArmSettings> settings, 
            ILogger<ArmDeployer> logger)
        {
            _logger = logger;
            _resourceClient = new ResourceManagementClient(credentials)
            {
                SubscriptionId = settings.Value.SubscriptionId
            };
        }

        public async Task<DeploymentExtended> Deploy(
            string resourceGroup,
            string arm,
            Dictionary<string, object> parameters)
        {
            var deployment = new Deployment(
                new DeploymentProperties(DeploymentMode.Incremental,
                    arm,
                    null,
                    JsonConvert.SerializeObject(parameters?.ToDictionary(x => x.Key, x =>
                    {
                        if (x.Value is CacochSecret secret)
                        {
                            return new
                            {
                                value = (object)secret.Secret
                            };
                        }
                        return new
                        {
                            value = x.Value
                        };
                    }))
                ));

            var deploymentName = $"{resourceGroup}-" + DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var response =  await _resourceClient.Deployments.CreateOrUpdateAsync(
                resourceGroup,
                deploymentName,
                deployment);
            
            _logger.LogDebug("Finished deployment {DeploymentName}", deploymentName);
            return response;
        }
    }
}