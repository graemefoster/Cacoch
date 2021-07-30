using Azure.Core;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;

namespace Cooker.Azure
{
    public interface IAzureSdkProvider
    {
        ResourcesManagementClient GetResourcesManagementClient();
        KeyVaultManagementClient GetKeyVaultManagementClient();
        TokenCredential Credential { get; }
    }
}