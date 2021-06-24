using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Manifest.OAuthClient
{
    [CacochResource("oauthScopeLink")]
    public record OAuthScopeLink(string Name, string[] Scopes) : CacochResourceLinkMetadata(Name);
}