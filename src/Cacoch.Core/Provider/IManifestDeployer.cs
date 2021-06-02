using System.Threading.Tasks;

namespace Cacoch.Core.Provider
{
    public interface IManifestDeployer
    {
        Task Deploy(Manifest.Manifest manifest);
    }
}