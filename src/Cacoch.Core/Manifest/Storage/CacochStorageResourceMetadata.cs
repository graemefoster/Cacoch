using System.Collections.Generic;
using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Manifest.Storage
{
    public record Storage(string Name,
        IList<CacochStorageResourceContainer> Containers,
        List<CacochResourceLinkMetadata>? Links) : CacochResourceMetadata(Name, "Storage",
        Links ?? new List<CacochResourceLinkMetadata>());
}