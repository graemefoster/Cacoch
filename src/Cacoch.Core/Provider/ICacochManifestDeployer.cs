using System.Threading.Tasks;

namespace Cacoch.Core.Provider
{
    public interface ICacochManifestDeployer
    {
        Task Deploy(Manifest.Manifest manifest, IPlatformTwin[] twins);
    }
}