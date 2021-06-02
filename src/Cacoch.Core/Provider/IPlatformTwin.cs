using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;

namespace Cacoch.Core.Provider
{
    public interface IPlatformTwin<T> : IPlatformTwin where T : IResource
    {
        
    }

    public interface IPlatformTwin
    {
        Task<ValidationResult> Validate();

        Task<IDeploymentArtifact> BuildDeploymentArtifact();
        string PlatformName { get; }
    }
}