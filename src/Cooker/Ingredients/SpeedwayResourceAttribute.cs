using System;

namespace Cooker.Ingredients
{
    public class CookerResourceAttribute : Attribute
    {
        public CookerResourceAttribute(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }

}