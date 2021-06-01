using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;

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
            if (_resource.Name.Length is > 4 and < 12) return Task.FromResult(ValidationResult.Success);
            return Task.FromResult(new ValidationResult("Azure Storage account names must be between 5 and 11 characters"));
        }

        public async Task<IDeploymentArtifact> BuildDeploymentArtifact()
        {
            return new AzureArmDeploymentArtifact(_resource.Name.ToLowerInvariant(), await GetResourceContents(), new Dictionary<string, object>()
            {
                {"storageAccountName", _resource.Name}
            });
        }

        private static Task<string> GetResourceContents()
        {
            using var stream = new StreamReader(typeof(StorageTwin).Assembly.GetManifestResourceStream(typeof(StorageTwin).FullName + ".json")!);
            return stream.ReadToEndAsync();
        }
    }
}