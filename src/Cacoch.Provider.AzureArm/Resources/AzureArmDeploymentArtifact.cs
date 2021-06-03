using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cacoch.Core.Manifest;

namespace Cacoch.Provider.AzureArm.Resources
{
    internal class AzureArmDeploymentArtifact : IDeploymentArtifact
    {
        internal string Name { get; }
        internal string Arm { get; }
        internal Dictionary<string, object> Parameters { get; }

        public AzureArmDeploymentArtifact(
            string name, 
            string arm, 
            Dictionary<string, object> parameters,
            IEnumerable<AzureArmDeploymentArtifact> childArtifacts)
        {
            Name = name;
            Arm = arm;
            Parameters = parameters;
            ChildArtifacts = childArtifacts;
        }

        public IEnumerable<IDeploymentArtifact> ChildArtifacts { get; }
    }
}