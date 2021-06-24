using System.Collections.Generic;
using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Azure
{
    public class ArmDeploymentOutput : IDeploymentOutput
    {
        private readonly IDictionary<string, string> _outputs;

        public ArmDeploymentOutput(IDictionary<string, string> outputs)
        {
            _outputs = outputs;
        }
        
        internal string this[string key] => _outputs[key];
    }
}