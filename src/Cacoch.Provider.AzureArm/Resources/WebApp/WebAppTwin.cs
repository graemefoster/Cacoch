using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Resources.Secrets;
using Microsoft.Extensions.Options;

namespace Cacoch.Provider.AzureArm.Resources.WebApp
{
    internal class WebAppTwin : IPlatformTwin<Core.Manifest.WebApp.CacochWebAppResourceMetadata>
    {
        private readonly Core.Manifest.WebApp.CacochWebAppResourceMetadata _resource;
        private readonly IOptions<AzureArmSettings> _settings;

        public WebAppTwin(IOptions<AzureArmSettings> settings,
            Core.Manifest.WebApp.CacochWebAppResourceMetadata resource,
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
            var appSettings = PrepareAppSettings(allTwins, out ArmOutputNameValueObjectArray requiredSecretReferences);

            return new AzureArmDeploymentArtifact(
                _resource.Name.ToLowerInvariant(),
                await typeof(WebAppTwin).GetResourceContents(),
                new Dictionary<string, object>()
                {
                    ["webAppName"] = PlatformName,
                    ["serverFarmId"] = _settings.Value.ServerFarmId!,
                    ["appSettings"] = appSettings.Select(x => new {name = x.Key, value = x.Value}).ToArray(),
                    ["secretReferences"] = requiredSecretReferences
                },
                new[] {"hostName"},
                Array.Empty<AzureArmDeploymentArtifact>());
        }

        public Task<IDeploymentArtifact?> PostDeployBuildDeploymentArtifact(
            IDictionary<string, IDeploymentOutput> allTwins)
        {
            return Task.FromResult(default(IDeploymentArtifact));
        }

        public Task<IDeploymentArtifact> PostDeployBuildDeploymentArtifact(IPlatformTwin[] allTwins)
        {
            return Task.FromResult(default(IDeploymentArtifact)!);
        }

        private Dictionary<string, string> PrepareAppSettings(IPlatformTwin[] allTwins,
            out ArmOutputNameValueObjectArray requiredInputs)
        {
            var requiredSecretReferences = new ArmOutputNameValueObjectArray();
            var settings = new Dictionary<string, string>()
            {
                ["WEBSITE_ENABLE_SYNC_UPDATE_SITE"] = "true",
                ["WEBSITE_RUN_FROM_PACKAGE"] = "true"
            };
            foreach (var configuredSetting in _resource.Configuration ?? new Dictionary<string, string>())
            {
                var val = configuredSetting.Value;
                if (val.StartsWith("[secret."))
                {
                    //we need to get a reference to a secret uri from the secret container.
                    var secretParts = val.Substring(1, val.Length - 2).Substring("secret.".Length).Split(".");
                    var vaultTwin = (SecretsTwin) allTwins.Single(x => x.ResourceName == secretParts[0]);

                    //secretReference gives us the arm expression to get the output from the secret template
                    var secretReference = vaultTwin.ArmOutputFor(secretParts[1]);

                    //an object will get passed in to this template. It will have all the secret uri's within it.
                    var parameterName = $"secreturi-{secretParts[1]}";
                    requiredSecretReferences.PropertySet[parameterName] = secretReference;
                }
                else
                {
                    settings[configuredSetting.Key] = configuredSetting.Value;
                }
            }

            requiredInputs = requiredSecretReferences;
            return settings;
        }

        public string PlatformName { get; }
        public string ResourceName => _resource.Name;
    }
}