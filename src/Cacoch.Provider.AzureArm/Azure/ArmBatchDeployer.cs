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
using Cacoch.Core.Manifest.Secrets;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        private readonly IDictionary<IPlatformTwin, AzureArmDeploymentArtifact> _topLevels =
            new Dictionary<IPlatformTwin, AzureArmDeploymentArtifact>();

        private readonly IDictionary<object, string> _parameterMap = new Dictionary<object, string>();
        private readonly IList<AzureArmDeploymentArtifact> _armsToDeploy = new List<AzureArmDeploymentArtifact>();

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
            RegisterArmRecursive(artifact);
        }

        private void RegisterArmRecursive(AzureArmDeploymentArtifact artifact)
        {
            BuildInternalArmDetails(artifact);
            MapInParametersToTemplate(artifact);
            RecurseChildArtifacts(artifact);
        }

        /// <summary>
        /// Arm templates often have children. This processes them, and marks them as dependent on their parent.
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="internalArmDetails"></param>
        private void RecurseChildArtifacts(AzureArmDeploymentArtifact artifact)
        {
            foreach (var deploymentArtifact in artifact.ChildArtifacts)
            {
                var child = (AzureArmDeploymentArtifact) deploymentArtifact;
                RegisterArmRecursive(child);
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
                if (!(parameter.Value is ArmOutput) && !(parameter.Value is ArmOutputNameValueObjectArray) &&
                    !(parameter.Value is ArmFunction))
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
        /// <returns></returns>
        private AzureArmDeploymentArtifact BuildInternalArmDetails(
            AzureArmDeploymentArtifact artifact)
        {
            if (!_uniqueArms.ContainsKey(artifact.Hash))
            {
                _uniqueArms.Add(artifact.Hash, (artifact.Hash.Substring(0, 10), artifact.Arm));
            }

            _armsToDeploy.Add(artifact);
            return artifact;
        }

        public async Task<Dictionary<string, IDeploymentOutput>> Deploy(string resourceGroup)
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
                var outputs = CreateOutputsSectionOfMainTemplate();
                var deploymentResources = BuildAllDeploymentResources(containerClient);

                _logger.LogDebug("Uploaded templates. Initiating ARM deployment");

                var armTemplate =
                    (await typeof(ArmBatchDeployer).GetResourceContents("MainDeploymentTemplate"))
                    .Replace("{{deployments}}", JsonConvert.SerializeObject(deploymentResources))
                    .Replace("{{parameters}}", JsonConvert.SerializeObject(uniqueParameters))
                    .Replace("{{outputs}}", JsonConvert.SerializeObject(outputs));

                var armOutputsByTemplate = (await _armDeployer.Deploy(resourceGroup,
                        armTemplate,
                        new Dictionary<string, object>(
                            _parameterMap.Select(
                                x => new KeyValuePair<string, object>(x.Value.ToString(), x.Key)))))
                    .ToDictionary(x => x.Key, x => x.Value);

                var response = new Dictionary<string, IDeploymentOutput>();
                foreach (var output in armOutputsByTemplate)
                {
                    response.Add(output.Key,
                        _armsToDeploy.Single(x => x.Name == output.Key).OutputTransformer(output.Value));
                }
                return response;
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
                var blobClient = containerClient.GetBlockBlobClient($"{_uniqueArms[x.Hash].uid}.json");

                var blobSasBuilder = new BlobSasBuilder(BlobSasPermissions.Read, DateTimeOffset.Now.AddHours(5))
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };
                var sasUri = blobClient.GenerateSasUri(blobSasBuilder);

                var dependsOn = x.Parent == null ? Array.Empty<string>() : new[] {x.Parent.Name};

                //if we depend on
                var parameterDependencies = x.Parameters.Select(x => x.Value).OfType<ArmOutput>()
                    .Select(a => _topLevels[a.Twin].NameForTemplate(a)).ToArray();

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
                                var armTemplateName = _topLevels[armOutput.Twin].NameForTemplate(armOutput);
                                return new KeyValuePair<string, object>(p.Key,
                                    new
                                    {
                                        value =
                                            $"[reference('{armTemplateName}').outputs['{armOutput.TemplateOutputName}'].value]"
                                    });
                            }

                            if (p.Value is ArmFunction armFunction)
                            {
                                return new KeyValuePair<string, object>(p.Key,
                                    new
                                    {
                                        value = armFunction.Function
                                    });
                            }

                            if (p.Value is ArmOutputNameValueObjectArray armOutputObject)
                            {
                                return new KeyValuePair<string, object>(p.Key,
                                    new
                                    {
                                        value = armOutputObject.PropertySet.Select(o => new
                                        {
                                            name = o.Key,
                                            value =
                                                $"[reference('{_topLevels[o.Value.Twin].NameForTemplate(o.Value)}').outputs['{o.Value.TemplateOutputName}'].value]"
                                        })
                                    }
                                );
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
                if (key is string)
                {
                    return new KeyValuePair<string, object>(
                        value,
                        new
                        {
                            type = "string"
                        });
                }

                if (key is CacochSecret)
                {
                    return new KeyValuePair<string, object>(
                        value,
                        new
                        {
                            type = "secureString"
                        });
                }

                if (key is Array)
                {
                    return new KeyValuePair<string, object>(
                        value,
                        new
                        {
                            type = "array"
                        });
                }

                throw new NotSupportedException($"Unsupported parameter type - {key.GetType()}");
            }));
            return uniqueParameters;
        }

        /// <summary>
        /// Creates an outputs object collecting all parameters that resources want to expose to others in their pre-process step.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private Dictionary<string, object> CreateOutputsSectionOfMainTemplate()
        {
            var uniqueParameters = new Dictionary<string, object>(_armsToDeploy.SelectMany(p =>
                p.ExposedOutputs.Select(x =>
                    new KeyValuePair<string, object>(
                        $"{p.Name}_{x}",
                        new
                        {
                            type = "string",
                            value = $"[reference('{p.Name}').outputs['{x}'].value]"
                        }))
            ));
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