using System;
using Azure.Core;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;

namespace Cooker.Azure
{
    class AzureSdkProvider : IAzureSdkProvider
    {
        public TokenCredential Credential { get; }

        private readonly Lazy<ResourcesManagementClient> _resourcesManagementClient;
        private readonly Lazy<KeyVaultManagementClient> _keyVaultClient;

        public AzureSdkProvider(TokenCredential credential, string subscriptionId)
        {
            Credential = credential;
            _resourcesManagementClient =
                new Lazy<ResourcesManagementClient>(() => new ResourcesManagementClient(subscriptionId, Credential));
            _keyVaultClient =
                new Lazy<KeyVaultManagementClient>(() => new KeyVaultManagementClient(subscriptionId, Credential));
        }

        public ResourcesManagementClient GetResourcesManagementClient()
        {
            return _resourcesManagementClient.Value;
        }

        public KeyVaultManagementClient GetKeyVaultManagementClient()
        {
            return _keyVaultClient.Value;
        }
    }
}