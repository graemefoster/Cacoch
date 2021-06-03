using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cacoch.Core.Manifest.Secrets;
using Cacoch.Core.Provider;
using Microsoft.Extensions.Options;

namespace Cacoch.Provider.AzureArm.Resources.Secrets
{
    internal class SecretsTwin : IPlatformTwin<CacochSecretContainerResourceMetadata>
    {
        private readonly CacochSecretContainerResourceMetadata _resource;
        private readonly IOptions<AzureArmSettings> _settings;

        public SecretsTwin(IOptions<AzureArmSettings> settings,
            CacochSecretContainerResourceMetadata resource,
            AzurePlatformContext platformContext)
        {
            _resource = resource;
            _settings = settings;
            PlatformName = resource.Name + "-" + platformContext.ResourceGroupRandomId;
        }

        public Task<ValidationResult> Validate()
        {
            if (_resource.Name.Length is > 4 and < 16) return Task.FromResult(ValidationResult.Success!);
            return Task.FromResult(new ValidationResult("Secret Container names must be between 5 and 15 characters"));
        }

        public async Task<IDeploymentArtifact> BuildDeploymentArtifact(IPlatformTwin[] allTwins)
        {
            var linkTemplate = await typeof(SecretsTwin).GetResourceContents("Link");

            var links = _resource.Links.OfType<SecretsLink>().Select(x =>
                new {type = x.Access, requestor = allTwins.Single(t => t.ResourceName == x.Name)});

            return new AzureArmDeploymentArtifact(
                _resource.Name.ToLowerInvariant(),
                await typeof(SecretsTwin).GetResourceContents(),
                new Dictionary<string, object>()
                {
                    ["vaultName"] = PlatformName,
                    ["location"] = _settings.Value.PrimaryLocation!
                },
                Array.Empty<IPlatformTwin>(),
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
                            {"vaultName", PlatformName},
                            {"requestorPrincipalId", new ArmOutput(x.requestor, "identity")},
                        },
                        new[] {x.requestor},
                        Array.Empty<AzureArmDeploymentArtifact>());
                })
            );
        }

        public string PlatformName { get; }
        public string ResourceName => _resource.Name;
    }
}