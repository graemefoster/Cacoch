using System.Collections.Generic;

namespace Cooker.Azure
{
    public class AzureCookerSettings
    {
        public string SubscriptionId { get; set; } = null!;
        public string DeploymentPrincipalId { get; set; } = null!;
        public string PrimaryLocation { get; set; } = null!;
        public Dictionary<string, string> PlatformAppServicePlans { get; set; } = null!;
    }
}