using System.Threading.Tasks;

namespace Cacoch.Core.Manifest
{
    public interface IPlatformDeployer
    {
        Task Deploy(Manifest manifest, IPlatformTwin[] twins);
    }
}