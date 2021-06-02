using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources.Storage
{
    public class StorageTwin : IPlatformTwin<Core.Manifest.Storage>
    {
        private readonly Core.Manifest.Storage _resource;

        public StorageTwin(Core.Manifest.Storage resource)
        {
            _resource = resource;
        }

        public Task<ValidationResult> Validate()
        {
            if (_resource.Name.Length is > 4 and < 16) return Task.FromResult(ValidationResult.Success);
            return Task.FromResult(new ValidationResult("Azure Storage account names must be between 5 and 11 characters"));
        }

        public async Task<IDeploymentArtifact> BuildDeploymentArtifact()
        {
            var containerTemplate = await typeof(StorageTwin).GetResourceContents("Container");
            
            return new AzureArmDeploymentArtifact(
                _resource.Name.ToLowerInvariant(),
                await typeof(StorageTwin).GetResourceContents(),
                new Dictionary<string, object>()
                {
                    {"storageAccountName", _resource.Name}
                },
                _resource.Containers.Select(x => new AzureArmDeploymentArtifact(
                    _resource.Name.ToLowerInvariant() + "/" + x,
                    containerTemplate,
                    new Dictionary<string, object>
                    {
                        {"storageAccountName", _resource.Name},
                        {"containerName", x}
                    },
                    Array.Empty<AzureArmDeploymentArtifact>())));
        }

        public string Name => _resource.Name;

    }

    public static class ResourceEx
    {
        public static Task<string> GetResourceContents(this Type relatedType, string templateName = null)
        {
            var resourceName = templateName == null
                ? relatedType.FullName + ".json"
                : relatedType.Namespace + $".{templateName}.json";

            using var stream = new StreamReader(relatedType.Assembly.GetManifestResourceStream(resourceName)!);
            return stream.ReadToEndAsync();
        }
    }
}