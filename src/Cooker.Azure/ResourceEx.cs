using System;
using System.IO;

namespace Cooker.Azure
{
    public static class ResourceEx
    {
        public static string GetResourceContents(this Type relatedType, string? templateName = null, string extension = "json")
        {
            var resourceName = templateName == null
                ? relatedType.FullName + "." + extension
                : relatedType.Namespace + $".{templateName}.{extension}";

            using var stream = new StreamReader(relatedType.Assembly.GetManifestResourceStream(resourceName)!);
            return stream.ReadToEnd();
        }
    }
}