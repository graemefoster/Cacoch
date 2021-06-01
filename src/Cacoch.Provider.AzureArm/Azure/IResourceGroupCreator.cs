using System.Threading.Tasks;

namespace Cacoch.Provider.AzureArm.Azure
{
    internal interface IResourceGroupCreator
    {
        Task CreateResourceGroupIfNotExists(string resourceGroup);
        Task<string> GetResourceGroupRandomId(string resourceGroup);
    }
}