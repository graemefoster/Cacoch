using System.Collections.Generic;

namespace Cacoch.Core.Manifest
{
    public interface IDeploymentArtifact
    {
        IEnumerable<IDeploymentArtifact> ChildArtifacts { get; }
    }
}