using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cooker.Kitchens.Azure
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

        public Task<object> Execute(IArmRunner armRunner)
        {
            return armRunner.Execute(_template, _parameters);
        }
    }
}