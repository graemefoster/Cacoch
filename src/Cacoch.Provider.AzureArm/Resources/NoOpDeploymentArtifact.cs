using System.Collections.Generic;
using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources
{
    internal class NoOpDeploymentArtifact : IDeploymentArtifact
    {
        public IEnumerable<IDeploymentArtifact> ChildArtifacts { get; }
    }
}