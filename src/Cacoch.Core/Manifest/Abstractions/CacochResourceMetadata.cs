using System.Collections.Generic;

namespace Cacoch.Core.Manifest.Abstractions
{
    public abstract record CacochResourceMetadata(string Name, string FriendlyType,
        List<CacochResourceLinkMetadata> Links)
    {
    }
}