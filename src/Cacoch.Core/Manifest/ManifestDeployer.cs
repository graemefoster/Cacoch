using System.Linq;
using System.Threading.Tasks;

namespace Cacoch.Core.Manifest
{
    internal class ManifestDeployer : IManifestDeployer
    {
        private readonly PlatformTwinFactory _platformTwinFactory;

        public ManifestDeployer(PlatformTwinFactory platformTwinFactory)
        {
            _platformTwinFactory = platformTwinFactory;
        }

        public async Task Deploy(Manifest manifest)
        {
            var twins = manifest.Resources.Select(x => _platformTwinFactory.Build<IResource>(x));
            var validation = twins.Select(x => x.Validate()).ToArray();
        }
    }
}