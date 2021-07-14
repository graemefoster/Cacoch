using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cooker.Kitchens.Azure
{
    class AzureResourceManagerArmRunner : IArmRunner
    {
        private readonly ILogger<AzureResourceManagerArmRunner> _logger;
        private readonly ResourceManagementClient _resourceClient;

        public AzureResourceManagerArmRunner(
            ResourceManagementClient resourceClient,
            ILogger<AzureResourceManagerArmRunner> logger)
        {
            _logger = logger;
            _resourceClient = resourceClient;
        }

        public async Task<object> Execute(string resourceGroup, string template, Dictionary<string, object> parameters)
        {
            var deployment = new Deployment(
                new DeploymentProperties(DeploymentMode.Incremental,
                    template,
                    null,
                    JsonConvert.SerializeObject(parameters?.ToDictionary(x => x.Key, x =>
                    {
                        if (x.Value is SecretParameter secret)
                        {
                            return new
                            {
                                value = (object) secret.Secret
                            };
                        }

                        return new
                        {
                            value = x.Value
                        };
                    }))
                ));

            var deploymentName = $"{resourceGroup}-" + DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var response = await _resourceClient.Deployments.CreateOrUpdateAsync(
                resourceGroup,
                deploymentName,
                deployment).ConfigureAwait(false);

            _logger.LogDebug("Finished deployment {DeploymentName}", deploymentName);

            return response.Properties.Outputs;
        }
    }
}