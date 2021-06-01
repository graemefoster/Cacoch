using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Models;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal interface IArmDeployer
    {
        Task<DeploymentExtended> DeployArm(
            string resourceGroup,
            string name,
            string template,
            ReadOnlyDictionary<string, object> parameters);
    }
}