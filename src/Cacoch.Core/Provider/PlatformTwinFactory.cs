using System;
using Cacoch.Core.Manifest;
using Microsoft.Extensions.DependencyInjection;

namespace Cacoch.Core.Provider
{
    internal class PlatformTwinFactory<TPlatformContext> where TPlatformContext : IPlatformContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PlatformTwinDictionary _platformTwinDictionary;

        public PlatformTwinFactory(IServiceProvider serviceProvider, PlatformTwinDictionary platformTwinDictionary)
        {
            _serviceProvider = serviceProvider;
            _platformTwinDictionary = platformTwinDictionary;
        }

        public IPlatformTwin Build<T>(T resource, TPlatformContext platformContext) where T : IResource
        {
            var genericType = typeof(IPlatformTwin<>).MakeGenericType(resource.GetType());
            return (IPlatformTwin)ActivatorUtilities.CreateInstance(
                _serviceProvider,
                _platformTwinDictionary.Resolve(genericType), 
                resource,
                platformContext);
        }
    }
}