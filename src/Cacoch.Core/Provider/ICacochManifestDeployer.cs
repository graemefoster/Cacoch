using System.Threading.Tasks;

namespace Cacoch.Core.Provider
{
    public interface ICacochManifestDeployer<TPlatformContext> where TPlatformContext: IPlatformContext
    {
        Task Deploy(Manifest.Manifest manifest, IPlatformTwin[] twins);
        Task<TPlatformContext> PrepareContext(Manifest.Manifest manifest);
    }
}