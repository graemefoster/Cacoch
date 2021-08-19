using Azure.Core;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using Microsoft.Graph;

namespace Cooker.Azure.KitchenStations.Sdk
{
    public interface IAzureSdkProvider
    {
        ResourcesManagementClient GetResourcesManagementClient();
        KeyVaultManagementClient GetKeyVaultManagementClient();
        TokenCredential Credential { get; }
        GraphServiceClient GetAzureActiveDirectorySdk();
    }
}