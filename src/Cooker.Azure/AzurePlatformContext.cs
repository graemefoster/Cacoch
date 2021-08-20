using System;
using System.Security.Cryptography;
using System.Text;
using Cooker.Kitchens;

namespace Cooker.Azure
{
    public class AzurePlatformContext : IPlatformContext
    {
        private readonly AzureCookerSettings _settings;

        public AzurePlatformContext(
            Docket docket,
            AzureCookerSettings settings,
            PlatformEnvironment environment)
        {
            _settings = settings;
            
            Randomness = Convert.ToBase64String(SHA512
                .Create()
                .ComputeHash(Encoding.UTF8.GetBytes(docket.TableName)))[..5];

            ResourceGroupName = string.Format($"{docket.TableName}-{environment.ShortName}");
        }

        public string ResourceGroupName { get; }
        public string Randomness { get; }
        public string DeploymentPrincipalId => _settings.DeploymentPrincipalId;
        public string TenantId => _settings.TenantId;
    }
}