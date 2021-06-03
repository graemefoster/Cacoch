using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Resources;
using Cacoch.Provider.AzureArm.Resources.Storage;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal class ArmBatchDeployer : IArmBatchBuilder
    {
        private record InternalArmDetails(
            string HashString,
            string Name,
            Dictionary<string, object> Parameters,
            InternalArmDetails? DependsOn);

        private readonly StorageManagementClient _storageManagementClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IOptions<AzureArmSettings> _settings;
        private readonly IArmDeployer _armDeployer;
        private readonly ILogger<ArmBatchDeployer> _logger;

        private readonly IDictionary<IPlatformTwin, AzureArmDeploymentArtifact> _topLevels = new Dictionary<IPlatformTwin, AzureArmDeploymentArtifact>();
        private readonly IDictionary<object, string> _parameterMap = new Dictionary<object, string>();
        private readonly IList<InternalArmDetails> _armsToDeploy = new List<InternalArmDetails>();

        private readonly IDictionary<string, (string uid, string arm)> _uniqueArms =
            new Dictionary<string, (string uid, string arm)>();

        private bool _deployed;

        public ArmBatchDeployer(
            StorageManagementClient storageManagementClient,
            BlobServiceClient blobServiceClient,
            IOptions<AzureArmSettings> settings,
            IArmDeployer armDeployer,
            ILogger<ArmBatchDeployer> logger)
        {
            _storageManagementClient = storageManagementClient;
            _blobServiceClient = blobServiceClient;
            _settings = settings;
            _armDeployer = armDeployer;
            _logger = logger;
        }

        public void RegisterArm(IPlatformTwin twin, AzureArmDeploymentArtifact artifact)
        {
            _topLevels[twin] = artifact;
            RegisterArmRecursive(artifact, null);
        }

        private void RegisterArmRecursive(AzureArmDeploymentArtifact artifact, InternalArmDetails? parent)
        {
            var internalArmDetails = BuildInternalArmDetails(artifact, parent);
            MapInParametersToTemplate(artifact);
            RecurseChildArtifacts(artifact, internalArmDetails);
        }

        /// <summary>
        /// Arm templates often have children. This processes them, and marks them as dependent on their parent.
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="internalArmDetails"></param>
        private void RecurseChildArtifacts(AzureArmDeploymentArtifact artifact, InternalArmDetails? internalArmDetails)
        {
            foreach (var deploymentArtifact in artifact.ChildArtifacts)
            {
                var child = (AzureArmDeploymentArtifact) deploymentArtifact;
                RegisterArmRecursive(child, internalArmDetails);
            }
        }

        /// <summary>
        /// Parameters are stored external to the templates. We only pass unique parameters up, so there might be a 1-many of
        /// parameters to the template that use them.
        /// </summary>
        /// <param name="artifact"></param>
        private void MapInParametersToTemplate(AzureArmDeploymentArtifact artifact)
        {
            foreach (var parameter in artifact.Parameters)
            {
                if (!(parameter.Value is ArmOutput))
                {
                    if (!_parameterMap.ContainsKey(parameter.Value))
                    {
                        _parameterMap[parameter.Value] = Guid.NewGuid().ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Creates an intermediate object for every arm template... To save uploading all templates we take a hash of its contents, and
        /// only upload unique ones.
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private InternalArmDetails BuildInternalArmDetails(AzureArmDeploymentArtifact artifact, InternalArmDetails? parent)
        {
            var hash = SHA512.Create().ComputeHash(Encoding.Default.GetBytes(artifact.Arm));
            var hashString = Convert.ToBase64String(hash);
            if (!_uniqueArms.ContainsKey(hashString))
            {
                _uniqueArms.Add(hashString, (hashString.Substring(0, 10), artifact.Arm));
            }

            var internalArmDetails = new InternalArmDetails(
                hashString,
                artifact.Name,
                artifact.Parameters,
                parent);

            _armsToDeploy.Add(internalArmDetails);
            return internalArmDetails;
        }

        public async Task<DeploymentExtended> Deploy(string resourceGroup)
        {
            try
            {
                if (_deployed)
                {
                    throw new InvalidOperationException(
                        "Batch deployer has already deployed. Please create a new one for another deployment");
                }

                _logger.LogDebug("Ensuring arm template storage container exists");
                var container = await GetOrCreateTemplateStorageContainer();

                _logger.LogDebug("Getting blob container client");
                var containerClient = _blobServiceClient.GetBlobContainerClient(container.Name);

                _logger.LogDebug("Ensuring arm templates are in storage");
                await UploadAllTemplatesAsync(containerClient);
                var uniqueParameters = CreateParametersSectionOfMainTemplate();
                var deploymentResources = BuildAllDeploymentResources(containerClient);

                _logger.LogDebug("Uploaded templates. Initiating ARM deployment");

                return await _armDeployer.Deploy(resourceGroup,
                    (await typeof(AzureResourceGroupCreator).GetResourceContents("MainDeploymentTemplate"))
                    .Replace("{{deployments}}", JsonConvert.SerializeObject(deploymentResources))
                    .Replace("{{parameters}}", JsonConvert.SerializeObject(uniqueParameters)),
                    new Dictionary<string, object>(_parameterMap.Select(x =>
                        new KeyValuePair<string, object>(x.Value.ToString(), x.Key)))
                );
            }
            finally
            {
                _deployed = true;
            }
        }

        /// <summary>
        /// Creates a new deployment resource for every template to be deployed.
        /// </summary>
        /// <param name="containerClient"></param>
        /// <returns></returns>
        private object BuildAllDeploymentResources(BlobContainerClient containerClient)
        {
            var deploymentResources = _armsToDeploy.Select(x =>
            {
                var blobClient = containerClient.GetBlockBlobClient($"{_uniqueArms[x.HashString].uid}.json");

                var blobSasBuilder = new BlobSasBuilder(BlobSasPermissions.Read, DateTimeOffset.Now.AddHours(5))
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };
                var sasUri = blobClient.GenerateSasUri(blobSasBuilder);

                var dependsOn = x.DependsOn == null ? Array.Empty<string>() : new[] {x.DependsOn.Name};
                var parameterDependencies = x.Parameters.Select(x => x.Value).OfType<ArmOutput>().Select(x => _topLevels[x.Twin].Name).ToArray();

                return new
                {
                    type = "Microsoft.Resources/deployments",
                    apiVersion = "2020-10-01",
                    name = x.Name,
                    dependsOn = dependsOn.Union(parameterDependencies).ToArray(),
                    properties = new
                    {
                        mode = "Incremental",
                        templateLink = new
                        {
                            uri = sasUri.AbsoluteUri,
                            contentVersion = "1.0.0.0"
                        },
                        parameters = new Dictionary<string, object>(x.Parameters.Select(p =>
                        {
                            if (p.Value is ArmOutput armOutput)
                            {
                                return new KeyValuePair<string, object>(p.Key,
                                    new {value = $"[reference('{_topLevels[armOutput.Twin].Name}').outputs['{armOutput.Name}'].value]"});
                            }

                            return new KeyValuePair<string, object>(p.Key,
                                new {value = $"[parameters('{_parameterMap[p.Value].ToString()}')]"});
                        }))
                    }
                };
            }).ToArray();
            return deploymentResources;
        }

        /// <summary>
        /// Creates a parameter object declaring all unique parameters used by the template.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private Dictionary<string, object> CreateParametersSectionOfMainTemplate()
        {
            var uniqueParameters = new Dictionary<string, object>(_parameterMap.Select(p =>
            {
                var (key, value) = p;
                if (key is { } stringValue)
                {
                    return new KeyValuePair<string, object>(
                        value,
                        new
                        {
                            type = "string"
                        });
                }

                throw new NotSupportedException($"Unsupported parameter type - {key.GetType()}");
            }));
            return uniqueParameters;
        }

        /// <summary>
        /// Uploads all template to blob storage
        /// </summary>
        /// <param name="containerClient"></param>
        private async Task UploadAllTemplatesAsync(BlobContainerClient containerClient)
        {
            var allBlobs = new List<string>();
            await foreach (var blob in containerClient.GetBlobsAsync(BlobTraits.Metadata, BlobStates.All)!)
            {
                allBlobs.Add(blob.Name);
            }

            await Task.WhenAll(_uniqueArms.Where(x => !allBlobs.Contains($"{x.Value.uid.ToString()}.json")).Select(
                async x =>
                {
                    var blobName = $"{x.Value.uid.ToString()}.json";
                    await using var ms = new MemoryStream(Encoding.Default.GetBytes(x.Value.arm));
                    _logger.LogDebug(" Uploading arm to {Blob}", blobName);
                    await containerClient.UploadBlobAsync(blobName, ms);
                }));
        }

        private async Task<BlobContainer> GetOrCreateTemplateStorageContainer()
        {
            try
            {
                return await _storageManagementClient.BlobContainers.GetAsync(
                    _settings.Value.PlatformResources,
                    _settings.Value.PlatformStorage,
                    "templates");
            }
            catch (RequestFailedException re) when (re.Status == 404)
            {
                _logger.LogDebug("Could not find template storage container. Building it");

                return await _storageManagementClient.BlobContainers.CreateAsync(
                    _settings.Value.PlatformResources,
                    _settings.Value.PlatformStorage,
                    "templates",
                    new BlobContainer() {PublicAccess = PublicAccess.None});
            }
        }
    }
}