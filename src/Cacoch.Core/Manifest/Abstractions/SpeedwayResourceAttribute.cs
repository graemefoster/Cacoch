using System;

namespace Cacoch.Core.Manifest.Abstractions
{
    public class CacochResourceAttribute : Attribute
    {
        public CacochResourceAttribute(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }

}