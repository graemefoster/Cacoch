using System.Collections.Generic;

namespace Cacoch.Core.Provider
{
    public interface IDeploymentArtifact
    {
        IEnumerable<IDeploymentArtifact> ChildArtifacts { get; }
    }
}