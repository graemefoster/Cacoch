using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cooker.Azure.KitchenStations.AzureResourceManager
{
    public interface IArmRunner
    {
        Task<IDictionary<string, object>> Execute(string resourceGroup, string friendlyName, string template, Dictionary<string, object> parameters);
    }
}