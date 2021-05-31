using System.Threading.Tasks;

namespace Cacoch.Core.Provider
{
    public interface IManifestBuilder
    {
        Task Reflect(Manifest.Manifest manifest);
    }
}