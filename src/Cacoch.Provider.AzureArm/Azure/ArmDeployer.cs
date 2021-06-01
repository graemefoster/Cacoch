using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal class ArmDeployer : IArmDeployer
    {
        private readonly ResourceManagementClient _resourceClient;

        public ArmDeployer(
            ServiceClientCredentials credentials,
            IOptions<AzureArmSettings> settings)
        {
            _resourceClient = new ResourceManagementClient(credentials)
            {
                SubscriptionId = settings.Value.SubscriptionId
            };
        }

        public Task<DeploymentExtended> DeployArm(string resourceGroup, string name, string template, ReadOnlyDictionary<string, object> parameters)
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

            return _resourceClient.Deployments.CreateOrUpdateAsync(
                resourceGroup,
                name + "-" + DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
                new Deployment(
                    new DeploymentProperties(DeploymentMode.Incremental,
                        template,
                        null,
                        JsonConvert.SerializeObject(armParameters)
                    )));
        }

    }
}