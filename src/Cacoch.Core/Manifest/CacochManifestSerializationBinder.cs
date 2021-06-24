using System;
using System.Linq;
using System.Reflection;
using Cacoch.Core.Manifest.Abstractions;
using Newtonsoft.Json.Serialization;

namespace Cacoch.Core.Manifest
{
    public class CacochManifestSerializationBinder : ISerializationBinder
    {
        private readonly Type[] _knownTypes = typeof(CacochResourceMetadata).Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttribute<CacochResourceAttribute>() != null && !x.IsAbstract)
            .ToArray();

        public Type BindToType(string? assemblyName, string typeName)
        {
            return _knownTypes.SingleOrDefault(t => t.GetCustomAttribute<CacochResourceAttribute>()!.Type == typeName)!;
        }

        public void BindToName(Type serializedType, out string? assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.GetCustomAttribute<CacochResourceAttribute>()!.Type;
        }    
    }
}