using System.Threading.Tasks;
using Cacoch.Core.Provider;

namespace Cacoch.Core.Manifest
{
    public interface ICacochManifestDeployer<TPlatformContext> where TPlatformContext: IPlatformContext
    {
        Task Deploy(Manifest manifest, IPlatformTwin[] twins);
        Task<TPlatformContext> PrepareContext(Manifest manifest);
    }
}