using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Cacoch.Core.Manifest
{
    public interface IPlatformTwin<T> : IPlatformTwin where T : IResource
    {
        
    }

    public interface IPlatformTwin
    {
        Task<ValidationResult> Validate();

        Task<IDeploymentArtifact> BuildDeploymentArtifact();
        string Name { get; }
    }

    public interface IDeploymentArtifact
    {
        IEnumerable<IDeploymentArtifact> ChildArtifacts { get; }
    }
}