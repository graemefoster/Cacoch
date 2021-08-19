using System;
using Azure.Core;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using Microsoft.Graph;

namespace Cooker.Azure.KitchenStations.Sdk
{
    class AzureSdkProvider : IAzureSdkProvider
    {
        public TokenCredential Credential { get; }

        private readonly Lazy<ResourcesManagementClient> _resourcesManagementClient;
        private readonly Lazy<KeyVaultManagementClient> _keyVaultClient;
        private readonly Lazy<GraphServiceClient> _graphServiceClient;

        public AzureSdkProvider(TokenCredential credential, string subscriptionId)
        {
            Credential = credential;
            _resourcesManagementClient =
                new Lazy<ResourcesManagementClient>(() => new ResourcesManagementClient(subscriptionId, Credential));
            _keyVaultClient =
                new Lazy<KeyVaultManagementClient>(() => new KeyVaultManagementClient(subscriptionId, Credential));
            _graphServiceClient =
                new Lazy<GraphServiceClient>(() => new GraphServiceClient(credential));
        }

        public ResourcesManagementClient GetResourcesManagementClient()
        {
            return _resourcesManagementClient.Value;
        }

        public KeyVaultManagementClient GetKeyVaultManagementClient()
        {
            return _keyVaultClient.Value;
        }
        
        public GraphServiceClient GetAzureActiveDirectorySdk()
        {
            return _graphServiceClient.Value;
        }

    }
}