using System;

namespace Cooker
{
    public class PlatformEnvironment
    {
        public string ShortName { get; }
        public string DisplayName { get; }

        public PlatformEnvironment(string shortName, string displayName)
        {
            if (shortName.Length != 3) throw new ArgumentException("Short name must be 3 characters");
            ShortName = shortName;
            DisplayName = displayName;
        }
    }
}