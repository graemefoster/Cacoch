using System.Collections.Generic;
using System.Threading.Tasks;
using Cacoch.Provider.AzureArm.Resources;
using Microsoft.Azure.Management.ResourceManager.Models;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal interface IArmBatchBuilder
    {
        void RegisterArm(AzureArmDeploymentArtifact artifact);
        Task<DeploymentExtended> Deploy(string resourceGroup);
    }

    internal interface IArmDeployer
    {
        Task<DeploymentExtended> Deploy(string resourceGroup, string template, Dictionary<string, object> parameters);
    }
}