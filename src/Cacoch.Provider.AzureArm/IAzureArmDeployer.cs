using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Models;

namespace Cacoch.Provider.AzureArm
{
    internal interface IAzureArmDeployer
    {
        Task<DeploymentExtended> DeployArm(string contents, Dictionary<string, object> parameters);
    }
}