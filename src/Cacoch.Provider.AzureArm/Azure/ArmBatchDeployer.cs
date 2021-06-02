using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Cacoch.Provider.AzureArm.Resources;
using Cacoch.Provider.AzureArm.Resources.Storage;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal class ArmBatchDeployer : IArmBatchBuilder
    {
        private readonly StorageManagementClient _storageManagementClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IOptions<AzureArmSettings> _settings;
        private readonly IArmDeployer _armDeployer;
        private readonly ILogger<ArmBatchDeployer> _logger;
        private readonly IList<AzureArmDeploymentArtifact> _armsToDeploy = new List<AzureArmDeploymentArtifact>();

        public ArmBatchDeployer(
            ServiceClientCredentials credentials,
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

        public void RegisterArm(AzureArmDeploymentArtifact artifact)
        {
            _armsToDeploy.Add(artifact);
        }

        public async Task<DeploymentExtended> Deploy(string resourceGroup)
        {
            _logger.LogDebug("Ensuring arm template storage container exists");
            var container = await GetOrCreateTemplateStorageContainer();
            _logger.LogDebug("Getting blob container client");
            var containerClient = _blobServiceClient.GetBlobContainerClient(container.Name);
            var deploymentTemplateFolder = resourceGroup + "-" + Guid.NewGuid().ToString().ToLowerInvariant();
            _logger.LogDebug("Will place templates in {Folder}", deploymentTemplateFolder);

            await Task.WhenAll(_armsToDeploy.Select(async x =>
            {
                await using var ms = new MemoryStream(Encoding.Default.GetBytes(x.Arm));
                var blobName = $"{deploymentTemplateFolder}/{x.PlatformIdentifier}.json";
                _logger.LogDebug(" Uploading arm for {PlatformResource} to {Blob}", x.PlatformIdentifier, blobName);
                await containerClient.UploadBlobAsync(blobName, ms);
            }));

            var deploymentResources = _armsToDeploy.Select(x =>
            {
                var blobClient = containerClient.GetBlockBlobClient($"{deploymentTemplateFolder}/{x.PlatformIdentifier}.json");
                var blobSasBuilder = new BlobSasBuilder(BlobSasPermissions.Read,
                    DateTimeOffset.Now.AddHours(5))
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };
                var sasUri = blobClient.GenerateSasUri(blobSasBuilder);

                return new
                {
                    type = "Microsoft.Resources/deployments",
                    apiVersion = "2020-10-01",
                    name = x.PlatformIdentifier,
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
                    }
                };
            }).ToArray();

            _armsToDeploy.Clear();
            _logger.LogDebug("Uploaded templates. Initiating ARM deployment");
            return await _armDeployer.Deploy(resourceGroup,
                (await typeof(AzureResourceGroupCreator).GetResourceContents("MainDeploymentTemplate")).Replace(
                    "{{deployments}}",
                    JsonConvert.SerializeObject(deploymentResources)), null);
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
                _logger.LogDebug("  Could not find template storage container. Building it");

                return await _storageManagementClient.BlobContainers.CreateAsync(
                    _settings.Value.PlatformResources,
                    _settings.Value.PlatformStorage,
                    "templates",
                    new BlobContainer() {PublicAccess = PublicAccess.None});
            }
        }
    }
}