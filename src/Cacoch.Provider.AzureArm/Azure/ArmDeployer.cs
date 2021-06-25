using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Manifest.Secrets;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal class ArmDeployer : IArmDeployer
    {
        private readonly ILogger<ArmDeployer> _logger;
        private readonly ResourceManagementClient _resourceClient;

        public ArmDeployer(
            ResourceManagementClient resourceClient,
            ILogger<ArmDeployer> logger)
        {
            _logger = logger;
            _resourceClient = resourceClient;
        }

        public async Task<IDictionary<string, ArmDeploymentOutput>> Deploy(
            string resourceGroup,
            string template,
            Dictionary<string, object> parameters)
        {
            var deployment = new Deployment(
                new DeploymentProperties(DeploymentMode.Incremental,
                    template,
                    null,
                    JsonConvert.SerializeObject(parameters?.ToDictionary(x => x.Key, x =>
                    {
                        if (x.Value is CacochSecret secret)
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
                deployment);

            _logger.LogDebug("Finished deployment {DeploymentName}", deploymentName);

            if (response.Properties.Outputs is JObject outputs)
            {
                return ((IEnumerable<KeyValuePair<string, JToken>>) outputs)
                    .Select(x => new
                    {
                        Template = x.Key.Contains("_") ? x.Key.Split("_")[0] : "",
                        PropertyName = x.Key.Contains("_") ? x.Key.Split("_")[1] : x.Key,
                        PropertyValue = x.Value["value"].Value<string>()
                    })
                    .GroupBy(x => x.Template)
                    .ToDictionary(
                        x => x.Key,
                        x => new ArmDeploymentOutput(x.ToDictionary(y => y.PropertyName, y => y.PropertyValue)));
            }

            return new Dictionary<string, ArmDeploymentOutput>();
        }
    }
}