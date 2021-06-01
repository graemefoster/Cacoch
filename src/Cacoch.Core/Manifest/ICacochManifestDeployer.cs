using System.Threading.Tasks;

namespace Cacoch.Core.Manifest
{
    public interface ICacochManifestDeployer
    {
        Task Deploy(Manifest manifest, IPlatformTwin[] twins);
    }
}