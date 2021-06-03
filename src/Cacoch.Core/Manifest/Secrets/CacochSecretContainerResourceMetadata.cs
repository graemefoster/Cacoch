using System.Collections.Generic;
using Cacoch.Core.Manifest.Abstractions;
using Cacoch.Core.Manifest.Storage;

namespace Cacoch.Core.Manifest.Secrets
{
    [CacochResource("secrets")]
    public record CacochSecretContainerResourceMetadata
        (string Name, List<CacochResourceLinkMetadata>? Links) : CacochResourceMetadata(Name, "Secrets",
            Links ?? new List<CacochResourceLinkMetadata>());

    [CacochResource("secretsLink")]
    public record SecretsLink(string Name, LinkAccess Access) : CacochResourceLinkMetadata(Name);

}
