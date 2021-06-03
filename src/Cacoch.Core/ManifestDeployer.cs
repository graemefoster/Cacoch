using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cacoch.Core.Provider;
using Microsoft.Extensions.Logging;

namespace Cacoch.Core
{
    internal class ManifestDeployer<TPlatformContext> : IManifestDeployer where TPlatformContext : IPlatformContext
    {
        private readonly PlatformTwinFactory<TPlatformContext> _platformTwinFactory;
        private readonly ICacochManifestDeployer<TPlatformContext> _cacochManifestDeployer;
        private readonly ILogger<ManifestDeployer<TPlatformContext>> _logger;

        public ManifestDeployer(
            PlatformTwinFactory<TPlatformContext> platformTwinFactory, 
            ICacochManifestDeployer<TPlatformContext> cacochManifestDeployer,
            ILogger<ManifestDeployer<TPlatformContext>> logger)
        {
            _platformTwinFactory = platformTwinFactory;
            _cacochManifestDeployer = cacochManifestDeployer;
            _logger = logger;
        }

        public async Task Deploy(Manifest.Manifest manifest)
        {
            var context = await _cacochManifestDeployer.PrepareContext(manifest);

            var twins = manifest.Resources.Select(x =>
            {
                _logger.LogDebug("Building twin for {Resource}", x.Name);
                return _platformTwinFactory.Build(x, context);
            }).ToArray();

            var validation = await Task.WhenAll(twins.Select(x => x.Validate()).ToArray());
            if (validation.Any(x => x != ValidationResult.Success))
            {
                throw new ArgumentException(
                    $"Unable to create platform twins{Environment.NewLine}{string.Join(Environment.NewLine, validation.Where(x => x != ValidationResult.Success).Select(x => x.ErrorMessage))}");
            }

            _logger.LogDebug("Initiating deployment for {Manifest}", manifest.Slug);
            await _cacochManifestDeployer.Deploy(manifest, twins);
        }
    }
}