using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Cooker.Kitchens;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cooker.Azure
{
    internal class AzureResourceManagerArmRunner : IArmRunner
    {
        private readonly ILogger<AzureResourceManagerArmRunner> _logger;
        private readonly ResourcesManagementClient _resourceClient;

        public AzureResourceManagerArmRunner(
            ResourcesManagementClient resourceClient,
            ILogger<AzureResourceManagerArmRunner> logger)
        {
            _logger = logger;
            _resourceClient = resourceClient;
        }

        public async Task<object> Execute(string resourceGroup, string template, Dictionary<string, object> parameters)
        {
            var deployment = new Deployment(
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters =
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
                }
            );

            var random = Guid.NewGuid().ToString().Substring(0, 4);
            var deploymentName = $"{resourceGroup}-" + DateTimeOffset.Now.ToString($"yyyy-MM-dd-HH-mm-ss-{random}");
            var deploymentTask = await _resourceClient.Deployments.StartCreateOrUpdateAsync(
                resourceGroup,
                deploymentName,
                deployment).ConfigureAwait(false);

            var response = await deploymentTask.WaitForCompletionResponseAsync();
            _logger.LogDebug("Finished deployment {DeploymentName}", deploymentName);

            return deploymentTask.Value.Properties.Outputs;
        }
    }
}