using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Manifest.OAuthClient
{
    [CacochResource("oauthRoleLink")]
    public record OAuthRoleLink(string Name, string[] Roles) : CacochResourceLinkMetadata(Name);
}