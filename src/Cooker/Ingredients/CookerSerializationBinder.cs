using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Cooker.Ingredients
{
    public class CookerSerializationBinder : ISerializationBinder
    {
        private readonly Type[] _knownTypes = typeof(IngredientData).Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttribute<CookerResourceAttribute>() != null && !x.IsAbstract)
            .ToArray();

        public Type BindToType(string? assemblyName, string typeName)
        {
            return _knownTypes.SingleOrDefault(t => t.GetCustomAttribute<CookerResourceAttribute>()!.Type == typeName)!;
        }

        public void BindToName(Type serializedType, out string? assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.GetCustomAttribute<CookerResourceAttribute>()!.Type;
        }    
    }
}