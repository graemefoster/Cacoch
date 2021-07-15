using System;
using System.IO;

namespace Cooker.Azure
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