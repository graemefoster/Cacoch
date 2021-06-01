using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Cacoch.Provider.AzureArm.Resources.Storage;
using Microsoft.Extensions.Options;

namespace Cacoch.Provider.AzureArm.Resources.WebApp
{
    internal class WebAppTwin : IPlatformTwin<Core.Manifest.WebApp>
    {
        private readonly Core.Manifest.WebApp _resource;
        private readonly IOptions<AzureArmSettings> _settings;

        public WebAppTwin(Core.Manifest.WebApp resource, IOptions<AzureArmSettings> settings)
        {
            _resource = resource;
            _settings = settings;
        }

        public Task<ValidationResult> Validate()
        {
            if (_resource.Name.Length is > 4 and < 16) return Task.FromResult(ValidationResult.Success);
            return Task.FromResult(new ValidationResult("Web App names must be between 5 and 11 characters"));
        }

        public async Task<IDeploymentArtifact> BuildDeploymentArtifact()
        {
            return new AzureArmDeploymentArtifact(_resource.Name.ToLowerInvariant(), await GetResourceContents(), new Dictionary<string, object>()
            {
                {"webAppName", _resource.Name},
                {"serverFarmId", _settings.Value.ServerFarmId }
            });
        }

        private static Task<string> GetResourceContents()
        {
            using var stream = new StreamReader(typeof(StorageTwin).Assembly.GetManifestResourceStream(typeof(WebAppTwin).FullName + ".json")!);
            return stream.ReadToEndAsync();
        }
        
        public string Name => _resource.Name;
    }
}