using System.Collections.Generic;
using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Manifest.WebApp
{
    public record WebApp(string Name, IDictionary<string, string>? Configuration) : CacochResourceMetadata(Name, "WebApp", new List<CacochResourceLinkMetadata>());
}