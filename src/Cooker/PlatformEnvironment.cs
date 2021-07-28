using System;

namespace Cooker
{
    public class PlatformEnvironment
    {
        public string Slug { get; }
        public string DisplayName { get; }

        public PlatformEnvironment(string slug, string displayName)
        {
            if (slug.Length != 3) throw new ArgumentException("Slug must be 3 characters");
            Slug = slug;
            DisplayName = displayName;
        }
    }
}