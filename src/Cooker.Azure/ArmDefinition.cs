using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Kitchens;

namespace Cooker.Azure
{
    public class ArmDefinition
    {
        private readonly string _template;
        private readonly Dictionary<string, object> _parameters;

        public ArmDefinition(string template, Dictionary<string, object> parameters)
        {
            _template = template;
            _parameters = parameters;
        }

        public Task<object> Execute(AzurePlatformContext azurePlatformContext, Docket docket, IArmRunner armRunner)
        {
            return armRunner.Execute(azurePlatformContext.ResourceGroupName, _template, _parameters);
        }
    }
}