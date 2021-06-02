using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest;

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

        public async Task<DeploymentExtended> Deploy(
            string resourceGroup,
            string arm,
            Dictionary<string, object> parameters)
        {
            return await _resourceClient.Deployments.CreateOrUpdateAsync(
                resourceGroup,
                "test-" + DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
                new Deployment(
                    new DeploymentProperties(DeploymentMode.Incremental,
                        arm,
                        null,
                        parameters == null
                            ? null
                            : new Dictionary<string, object>(parameters.Select(p =>
                            {
                                if (p.Value is { } stringValue)
                                {
                                    return new KeyValuePair<string, object>(
                                        p.Key,
                                        new
                                        {
                                            value = stringValue
                                        });
                                }

                                throw new NotSupportedException($"Unsupported parameter type - {p.Value.GetType()}");
                            }))
                    )));
        }
    }
}