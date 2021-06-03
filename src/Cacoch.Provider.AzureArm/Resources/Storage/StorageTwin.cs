using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cacoch.Core.Manifest.Storage;
using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources.Storage
{
    public class StorageTwin : IPlatformTwin<Core.Manifest.Storage.Storage>
    {
        private readonly Core.Manifest.Storage.Storage _resource;

        public StorageTwin(Core.Manifest.Storage.Storage resource, AzurePlatformContext context)
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
            var containerTemplate = await typeof(StorageTwin).GetResourceContents("Container");
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
                _resource.Containers.Select(x => new AzureArmDeploymentArtifact(
                    x.Name,
                    containerTemplate,
                    new Dictionary<string, object>
                    {
                        {"storageAccountName", PlatformName},
                        {"containerName", x.Name}
                    },
                    Array.Empty<IPlatformTwin>(),
                    Array.Empty<AzureArmDeploymentArtifact>())).Union(
                    links.Select(x =>
                    {
                        var assignmentDetails = $"{_resource.Name}-link-{x.requestor.PlatformName}-{x.type}";
                        var hash = SHA512.Create().ComputeHash(Encoding.Default.GetBytes(assignmentDetails));
                        var guidRepresentingAssignment = new Guid(hash[0..16]).ToString();
                        
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

        public string PlatformName { get; }
        public string ResourceName => _resource.Name;
    }
}