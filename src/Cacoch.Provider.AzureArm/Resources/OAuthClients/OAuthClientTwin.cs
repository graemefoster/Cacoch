using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Manifest.OAuthClient;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Resources.WebApp;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace Cacoch.Provider.AzureArm.Resources.OAuthClients
{
    public class OAuthClientTwin : IPlatformTwin<CacochOAuthClientResourceMetadata>
    {
        private readonly ILogger<OAuthClientTwin> _logger;
        private readonly CacochOAuthClientResourceMetadata _resource;

        public OAuthClientTwin(
            ILogger<OAuthClientTwin> logger,
            CacochOAuthClientResourceMetadata resource,
            AzurePlatformContext platformContext)
        {
            _logger = logger;
            _resource = resource;
            PlatformName = resource.Name + "-" + platformContext.ResourceGroupRandomId;
        }

        public Task<ValidationResult> Validate()
        {
            return Task.FromResult(ValidationResult.Success!);
        }

        public Task<IDeploymentArtifact> BuildDeploymentArtifact(IPlatformTwin[] allTwins)
        {
            return Task.FromResult(
                (IDeploymentArtifact) new AzureActiveDirectoryApiDeploymentArtifact(_resource.Name, async graphApi =>
                    {
                        var aadApplication = await FindAadApplication(graphApi);

                        if (aadApplication == null)
                        {
                            var applicationDefinition = new Application()
                            {
                                DisplayName = _resource.Name,
                                Tags = new[] {PlatformName},
                                SignInAudience = GetSignInAudience()
                            };
                            if (_resource.ClientType == OAuthClientType.Web)
                            {
                                applicationDefinition.Web = new WebApplication()
                                {
                                    RedirectUris = new [] {"http://localhost:5001/signin-oidc"},
                                    HomePageUrl = _resource.SignOnUri,
                                    ImplicitGrantSettings = new ImplicitGrantSettings()
                                    {
                                        EnableAccessTokenIssuance = false,
                                        EnableIdTokenIssuance = true
                                    }
                                };
                            }
                            else
                            {
                                applicationDefinition.PublicClient = new PublicClientApplication()
                                {
                                    RedirectUris = _resource.RedirectsFrom
                                };
                            }

                            var application = await graphApi.Applications.Request().AddAsync(applicationDefinition);

                            await graphApi.Applications[application.Id].Request().UpdateAsync(new Application
                            {
                                IdentifierUris = new[] {$"api://{application.AppId}"}
                            });

                            if (_resource.ClientType == OAuthClientType.Web)
                            {
                                var password = await graphApi.Applications[application.Id].AddPassword(
                                    new PasswordCredential()
                                    {
                                        DisplayName = "Speedway Client Secret",
                                    }).Request().PostAsync();

                                return new LastMinuteSecretOutput($"{_resource.Name}-ClientSecret", password.SecretText);
                            }
                        }

                        return new NoOutput();
                    },
                    async (graphApi, twins) =>
                    {
                        var redirects = _resource.RedirectsFrom?
                            .Select(x => $"https://{((WebAppOutput) twins[x]).HostName}/signin-oidc")
                            .ToArray() ?? Array.Empty<string>();

                        var application = await FindAadApplication(graphApi)!;
                        _logger.LogInformation("Adding redirect Uris: {Uris} to application {AppName}",
                            string.Join(",", redirects), application!.DisplayName);

                        var updatedApplication = new Application()
                        {
                            Web = new WebApplication()
                            {
                                RedirectUris = application!.Web.RedirectUris.Union(redirects)
                            }
                        };
                        await graphApi.Applications[application.Id].Request().UpdateAsync(updatedApplication);
                    }));
        }

        private async Task<Application?> FindAadApplication(GraphServiceClient graphApi)
        {
            var aadApplications = await graphApi.Applications.Request()
                .Filter($"tags/any(c:c eq '{PlatformName}')")
                .GetAsync();
            return aadApplications.Any() ? aadApplications.Single() : null;
        }

        public string PlatformName { get; }
        public string ResourceName => _resource.Name;

        // ReSharper disable once CommentTypo
        /// <summary>
        /// On Behalf of flow doesn't work if this isn't set properly. There goes 2 hours of my life.
        /// https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-app-manifest
        /// You end up with a application that has accessTokenAcceptedVersion set to 2. But this doesn't get a token that appears to work with on behalf of. You need it set to null (as
        /// per when you create apps from the CLI). Using AzureADMyOrg allows that. Maybe AzureADMultipleOrgs does as-well.  
        /// </summary>
        /// <returns></returns>
        private static string GetSignInAudience()
        {
            return "AzureADMyOrg";
        }
    }
}