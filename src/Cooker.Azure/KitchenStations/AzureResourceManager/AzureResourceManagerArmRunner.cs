using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources.Models;
using Cooker.Azure.KitchenStations.Sdk;
using Cooker.Kitchens;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cooker.Azure.KitchenStations.AzureResourceManager
{
    internal class AzureResourceManagerArmRunner : IArmRunner
    {
        private readonly ILogger<AzureResourceManagerArmRunner> _logger;
        private readonly IAzureSdkProvider _resourceClient;

        public AzureResourceManagerArmRunner(
            IAzureSdkProvider resourceClient,
            ILogger<AzureResourceManagerArmRunner> logger)
        {
            _logger = logger;
            _resourceClient = resourceClient;
        }

        public async Task<IDictionary<string, object>> Execute(
            string resourceGroup, 
            string friendlyName,
            string template, 
            Dictionary<string, object> parameters)
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
                                    value = (object)secret.Secret
                                };
                            }

                            return new
                            {
                                value = x.Value
                            };
                        }))
                }
            );

            var deploymentName =
                $"{friendlyName.AtMost(49)}-{DateTimeOffset.Now.ToString("yyyyMMddHHmmss")}";
            var resourceClient = _resourceClient.GetResourcesManagementClient();

            _logger.LogDebug("Beginning ARM deployment {DeploymentName}", deploymentName);
            var deploymentTask = await resourceClient.Deployments.StartCreateOrUpdateAsync(
                resourceGroup,
                deploymentName,
                deployment).ConfigureAwait(false);

            await deploymentTask.WaitForCompletionResponseAsync();
            _logger.LogDebug("Finished ARM deployment {DeploymentName}", deploymentName);
            
            var outputs = 
                deploymentTask.Value.Properties.Outputs as IDictionary<string, object> 
                ?? new Dictionary<string, object>();

            return outputs.ToDictionary(x => x.Key, x => ((IDictionary<string, object>)x.Value)["value"]);
        }
    }
}