using System.Collections.Generic;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;

namespace Cacoch.Provider.AzureArm.Resources
{
    internal class AzureArmDeploymentArtifact : IDeploymentArtifact
    {
        private readonly string _platformIdentifier;
        private readonly string _arm;
        private readonly Dictionary<string, object> _parameters;

        public AzureArmDeploymentArtifact(string platformIdentifier, string arm, Dictionary<string, object> parameters)
        {
            _platformIdentifier = platformIdentifier;
            _arm = arm;
            _parameters = parameters;
        }

        internal Task Deploy(IAzureArmDeployer armDeployer)
        {
            return armDeployer.DeployArm(_arm, _parameters);
        }
    }
}