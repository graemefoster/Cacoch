using System;
using System.IO;
using System.Threading.Tasks;

namespace Cooker.Kitchens.Azure
{
    public static class ResourceEx
    {
        public static string GetResourceContents(this Type relatedType, string? templateName = null)
        {
            var resourceName = templateName == null
                ? relatedType.FullName + ".json"
                : relatedType.Namespace + $".{templateName}.json";

            using var stream = new StreamReader(relatedType.Assembly.GetManifestResourceStream(resourceName)!);
            return stream.ReadToEnd();
        }
    }
}