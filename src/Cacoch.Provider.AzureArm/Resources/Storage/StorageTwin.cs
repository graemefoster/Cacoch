using System.ComponentModel.DataAnnotations;
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
            if (_resource.Name.Length > 4 && _resource.Name.Length < 12) return Task.FromResult(ValidationResult.Success);
            return Task.FromResult(new ValidationResult("Storage account names must be between 5 and 11 characters"));
        }
    }
}