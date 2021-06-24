using System.Collections.Generic;
using System.Threading.Tasks;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Resources;
using Microsoft.Azure.Management.ResourceManager.Models;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal interface IArmBatchBuilder
    {
        void RegisterArm(IPlatformTwin twin, AzureArmDeploymentArtifact artifact);
        Task<Dictionary<string, IDeploymentOutput>> Deploy(string resourceGroup);
    }

    internal interface IArmDeployer
    {
        Task<IDictionary<string, ArmDeploymentOutput>> Deploy(
            string resourceGroup, 
            string template, 
            Dictionary<string, object> parameters);
    }
}