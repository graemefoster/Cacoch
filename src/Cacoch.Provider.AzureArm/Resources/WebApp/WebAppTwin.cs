using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Resources.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Cacoch.Provider.AzureArm.Resources.WebApp
{
    internal class WebAppTwin : IPlatformTwin<Core.Manifest.WebApp.WebApp>
    {
        private readonly Core.Manifest.WebApp.WebApp _resource;
        private readonly IOptions<AzureArmSettings> _settings;

        public WebAppTwin(IOptions<AzureArmSettings> settings, Core.Manifest.WebApp.WebApp resource,
            AzurePlatformContext platformContext)
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

        public async Task<IDeploymentArtifact> BuildDeploymentArtifact(IPlatformTwin[] allTwins)
        {
            var appSettings = PrepareAppSettings();

            return new AzureArmDeploymentArtifact(
                _resource.Name.ToLowerInvariant(),
                await typeof(WebAppTwin).GetResourceContents(),
                new Dictionary<string, object>()
                {
                    ["webAppName"] = PlatformName,
                    ["serverFarmId"] = _settings.Value.ServerFarmId!,
                    ["appSettings"] = appSettings.Select(x => new { name = x.Key, value = x.Value }).ToArray()
                },
                Array.Empty<IPlatformTwin>(),
                Array.Empty<AzureArmDeploymentArtifact>());
        }

        private Dictionary<string, string> PrepareAppSettings()
        {
            var userRequiredSettings =
                new Dictionary<string, string>(_resource.Configuration ?? new Dictionary<string, string>())
                {
                    ["WEBSITE_ENABLE_SYNC_UPDATE_SITE"] = "true",
                    ["WEBSITE_RUN_FROM_PACKAGE"] = "true"
                };
            return userRequiredSettings;
        }

        public string PlatformName { get; }
        public string ResourceName => _resource.Name;
    }
}