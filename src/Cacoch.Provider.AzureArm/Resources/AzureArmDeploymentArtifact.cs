using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cacoch.Core.Manifest;

namespace Cacoch.Provider.AzureArm.Resources
{
    internal class AzureArmDeploymentArtifact : IDeploymentArtifact
    {
        internal string PlatformIdentifier { get; }
        internal string Arm { get; }
        internal ReadOnlyDictionary<string, object> Parameters { get; }

        public AzureArmDeploymentArtifact(
            string platformIdentifier, 
            string arm, 
            Dictionary<string, object> parameters,
            IEnumerable<AzureArmDeploymentArtifact> childArtifacts)
        {
            PlatformIdentifier = platformIdentifier;
            Arm = arm;
            Parameters = new ReadOnlyDictionary<string, object>(parameters);
            ChildArtifacts = childArtifacts;
        }

        public IEnumerable<IDeploymentArtifact> ChildArtifacts { get; }
    }
}