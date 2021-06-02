using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Resources.Storage;
using Microsoft.Extensions.Options;

namespace Cacoch.Provider.AzureArm.Resources.WebApp
{
    internal class WebAppTwin : IPlatformTwin<Core.Manifest.WebApp>
    {
        private readonly Core.Manifest.WebApp _resource;
        private readonly IOptions<AzureArmSettings> _settings;

        public WebAppTwin(IOptions<AzureArmSettings> settings, Core.Manifest.WebApp resource, AzurePlatformContext platformContext)
        {
            _resource = resource;
            _settings = settings;
            PlatformName = resource.Name + "-" + platformContext.ResourceGroupRandomId;
        }

        public Task<ValidationResult> Validate()
        {
            if (_resource.Name.Length is > 4 and < 16) return Task.FromResult(ValidationResult.Success!);
            return Task.FromResult(new ValidationResult("Web App names must be between 5 and 15 characters"));
        }

        public async Task<IDeploymentArtifact> BuildDeploymentArtifact()
        {
            return new AzureArmDeploymentArtifact(
                _resource.Name.ToLowerInvariant(), 
                await typeof(WebAppTwin).GetResourceContents(),
                new Dictionary<string, object>()
                {
                    {"webAppName", PlatformName},
                    {"serverFarmId", _settings.Value.ServerFarmId!}
                },
                Array.Empty<AzureArmDeploymentArtifact>());
        }

        public string PlatformName { get; }
    }
}