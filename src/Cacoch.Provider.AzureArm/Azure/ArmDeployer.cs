using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                        if (x.Value is { } stringValue)
                        {
                            return new
                            {
                                value = stringValue
                            };
                        }

                        throw new NotSupportedException($"Unsupported parameter type - {x.Value.GetType()}");
                    }))
                ));

            var response =  await _resourceClient.Deployments.CreateOrUpdateAsync(
                resourceGroup,
                "test-" + DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
                deployment);
            
            _logger.LogDebug("Finished deployment");
            return response;
        }
    }
}