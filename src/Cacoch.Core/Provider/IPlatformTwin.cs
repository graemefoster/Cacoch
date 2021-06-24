using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Provider
{
    public interface IPlatformTwin<T> : IPlatformTwin where T : CacochResourceMetadata
    {
    }

    public interface IPlatformTwin
    {
        Task<ValidationResult> Validate();

        Task<IDeploymentArtifact> BuildDeploymentArtifact(IPlatformTwin[] allTwins);

        Task<IDeploymentArtifact?> PostDeployBuildDeploymentArtifact(IDictionary<string, IDeploymentOutput> allTwins);

        string PlatformName { get; }
        string ResourceName { get; }
    }
}