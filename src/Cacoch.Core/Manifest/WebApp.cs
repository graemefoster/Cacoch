using System.Collections.Generic;
using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Manifest
{
    public record WebApp(string Name) : CacochResourceMetadata(Name, "WebApp", new List<CacochResourceLinkMetadata>());
}