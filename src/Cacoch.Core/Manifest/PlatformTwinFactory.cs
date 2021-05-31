using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Cacoch.Core.Manifest
{
    internal class PlatformTwinDictionary
    {
        private IDictionary<Type, Type> _mapping = new Dictionary<Type, Type>();

        public void Map(Type service, Type impl)
        {
            _mapping.Add(service, impl);
        }

        public Type Resolve(Type t)
        {
            return _mapping[t];
        }
    }
    
    internal class PlatformTwinFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PlatformTwinDictionary _platformTwinDictionary;

        public PlatformTwinFactory(IServiceProvider serviceProvider, PlatformTwinDictionary platformTwinDictionary)
        {
            _serviceProvider = serviceProvider;
            _platformTwinDictionary = platformTwinDictionary;
        }

        public IPlatformTwin Build<T>(T resource) where T : IResource
        {
            var genericType = typeof(IPlatformTwin<>).MakeGenericType(resource.GetType());
            return (IPlatformTwin)ActivatorUtilities.CreateInstance(
                _serviceProvider,
                _platformTwinDictionary.Resolve(genericType), 
                resource);
        }
    }
}