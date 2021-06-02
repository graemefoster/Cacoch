using Cacoch.Core.Manifest;

namespace Cacoch.Provider.AzureArm
{
    public class AzurePlatformContext : IPlatformContext
    {
        public string ResourceGroupRandomId { get; }

        public AzurePlatformContext(string resourceGroupRandomId)
        {
            ResourceGroupRandomId = resourceGroupRandomId;
        }
    }
}