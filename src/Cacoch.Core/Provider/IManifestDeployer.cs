using System.Threading.Tasks;

namespace Cacoch.Core.Manifest
{
    public interface IManifestDeployer
    {
        Task Deploy(Manifest manifest);
    }
}