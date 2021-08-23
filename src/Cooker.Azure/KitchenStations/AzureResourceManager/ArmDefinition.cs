using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Kitchens;

namespace Cooker.Azure.KitchenStations.AzureResourceManager
{
    public class ArmDefinition
    {
        private readonly string _friendlyName;
        private readonly string _template;
        private readonly Dictionary<string, object> _parameters;

        public ArmDefinition(string friendlyName, string template, Dictionary<string, object> parameters)
        {
            _friendlyName = friendlyName;
            _template = template;
            _parameters = parameters;
        }

        public Task<IDictionary<string, object>> Execute(AzurePlatformContext azurePlatformContext, Docket docket, IArmRunner armRunner)
        {
            return armRunner.Execute(azurePlatformContext.ResourceGroupName, _friendlyName, _template, _parameters);
        }
    }
}