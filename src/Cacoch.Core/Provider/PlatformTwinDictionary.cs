using System;
using System.Collections.Generic;

namespace Cacoch.Core.Provider
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
}