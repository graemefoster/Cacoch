using System;
using System.Collections.Generic;
using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Manifest
{
    public record Manifest(Guid Id, string ApiVersion, string Slug, string DisplayName, List<CacochResourceMetadata> Resources);
}