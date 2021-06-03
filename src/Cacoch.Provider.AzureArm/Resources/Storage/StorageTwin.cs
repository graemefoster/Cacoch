using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.ResourceManager.Storage.Models;
using Cacoch.Core.Manifest.Storage;
using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources.Storage
{
    public class StorageTwin : IPlatformTwin<Core.Manifest.Storage.CacochStorageResourceMetadata>
    {
        private readonly Core.Manifest.Storage.CacochStorageResourceMetadata _resource;

        public StorageTwin(Core.Manifest.Storage.CacochStorageResourceMetadata resource, AzurePlatformContext context)
        {
            _resource = resource;
            PlatformName = _resource.Name + context.ResourceGroupRandomId;
            PlatformName = PlatformName.Length > 24 ? PlatformName.Substring(0, 24) : PlatformName;
        }

        public Task<ValidationResult> Validate()
        {
            if (_resource.Name.Length is > 4 and < 16) return Task.FromResult(ValidationResult.Success!);
            return Task.FromResult(
                new ValidationResult("Azure Storage account names must be between 5 and 15 characters"));
        }

        public async Task<IDeploymentArtifact> BuildDeploymentArtifact(IPlatformTwin[] allTwins)
        {
            var linkTemplate = await typeof(StorageTwin).GetResourceContents("Link");

            var links = _resource.Links.OfType<StorageLink>().Select(x =>
                new {type = x.Access, requestor = allTwins.Single(t => t.ResourceName == x.Name)});

            return new AzureArmDeploymentArtifact(
                _resource.Name.ToLowerInvariant(),
                await typeof(StorageTwin).GetResourceContents(),
                new Dictionary<string, object>
                {
                    {"storageAccountName", PlatformName}
                },
                Array.Empty<IPlatformTwin>(),
                (await Containers())
                .Union(await Queues())
                .Union(await Tables())
                .Union(
                    links.Select(x =>
                    {
                        var assignmentDetails = $"{_resource.Name}-{_resource.FriendlyType}-link-{x.requestor.PlatformName}-{x.type}".ToLowerInvariant();
                        var hash = SHA512.Create().ComputeHash(Encoding.Default.GetBytes(assignmentDetails));
                        var guidRepresentingAssignment = new Guid(hash[..16]).ToString();

                        return new AzureArmDeploymentArtifact(
                            $"{_resource.Name}-link-{x.requestor.PlatformName}-{x.type}",
                            linkTemplate,
                            new Dictionary<string, object>
                            {
                                {"assignmentName", guidRepresentingAssignment},
                                {"storageName", PlatformName},
                                {"requestorPrincipalId", new ArmOutput(x.requestor, "identity")},
                            },
                            new[] {x.requestor},
                            Array.Empty<AzureArmDeploymentArtifact>());
                    })));
        }

        private async Task<IEnumerable<AzureArmDeploymentArtifact>> Containers()
        {
            return StorageChild(CacochStorageResourceContainerType.Storage,
                await typeof(StorageTwin).GetResourceContents("Container"));
        }

        private async Task<IEnumerable<AzureArmDeploymentArtifact>> Queues()
        {
            return StorageChild(CacochStorageResourceContainerType.Queue,
                await typeof(StorageTwin).GetResourceContents("Queue"));
        }

        private async Task<IEnumerable<AzureArmDeploymentArtifact>> Tables()
        {
            return StorageChild(CacochStorageResourceContainerType.Table,
                await typeof(StorageTwin).GetResourceContents("Table"));
        }

        private IEnumerable<AzureArmDeploymentArtifact> StorageChild(CacochStorageResourceContainerType type,
            string template)
        {
            return _resource.Containers.Where(x => x.Type == type).Select(x =>
                new AzureArmDeploymentArtifact(
                    x.Name,
                    template,
                    new Dictionary<string, object>
                    {
                        {"storageAccountName", PlatformName},
                        {"name", x.Name}
                    },
                    Array.Empty<IPlatformTwin>(),
                    Array.Empty<AzureArmDeploymentArtifact>()));
        }

        public string PlatformName { get; }
        public string ResourceName => _resource.Name;
    }
}