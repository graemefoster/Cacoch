using System.Collections.Generic;
using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Manifest.Storage
{
    [CacochResource("storage")]
    public record CacochStorageResourceMetadata(string Name,
        IList<CacochStorageResourceContainer> Containers,
        List<CacochResourceLinkMetadata>? Links) : CacochResourceMetadata(Name, "Storage",
        Links ?? new List<CacochResourceLinkMetadata>());
}