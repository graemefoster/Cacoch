using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cacoch.Core.Manifest
{
    internal class ManifestDeployer : IManifestDeployer
    {
        private readonly PlatformTwinFactory _platformTwinFactory;
        private readonly ICacochManifestDeployer _cacochManifestDeployer;

        public ManifestDeployer(
            PlatformTwinFactory platformTwinFactory, 
            ICacochManifestDeployer cacochManifestDeployer)
        {
            _platformTwinFactory = platformTwinFactory;
            _cacochManifestDeployer = cacochManifestDeployer;
        }

        public async Task Deploy(Manifest manifest)
        {
            var twins = manifest.Resources.Select(x => _platformTwinFactory.Build(x)).ToArray();
            var validation = await Task.WhenAll(twins.Select(x => x.Validate()).ToArray());
            if (validation.Any(x => x != ValidationResult.Success))
            {
                throw new ArgumentException(
                    $"Unable to create platform twins{Environment.NewLine}{string.Join(Environment.NewLine, validation.Where(x => x != ValidationResult.Success).Select(x => x.ErrorMessage))}");
            }
            await _cacochManifestDeployer.Deploy(manifest, twins);
        }
    }
}