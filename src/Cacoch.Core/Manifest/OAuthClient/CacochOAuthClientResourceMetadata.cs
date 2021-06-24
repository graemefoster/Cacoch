using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cacoch.Core.Manifest.Abstractions;
using Cacoch.Core.Manifest.WebApp;

namespace Cacoch.Core.Manifest.OAuthClient
{
    /// <summary>
    /// KnownApplications are applications that when consented to, implicitly consent to the scopes in this application. Use the .default scope to ensure that the user can see exactly what they are consenting to!
    /// </summary>
    [CacochResource("oauthClient")]
    public record CacochOAuthClientResourceMetadata(
        string Name,
        OAuthClientType ClientType,
        string SignOnUri,
        string[]? ReplyUrls,
        string[]? RedirectsFrom,
        string[]? KnownApplications,
        List<OAuthRole>? Roles,
        List<OAuthScope>? Scopes,
        List<CacochResourceLinkMetadata>? Links) : CacochResourceMetadata(Name, "OAuth",
        Links ?? new List<CacochResourceLinkMetadata>())
    {
        private static readonly Type[] CanLinkToScopeTypes = new Type[]
        {
            typeof(CacochOAuthClientResourceMetadata),
        };

        private static readonly Type[] CanLinkToRoleTypes = new Type[]
        {
            typeof(CacochOAuthClientResourceMetadata),
            typeof(CacochWebAppResourceMetadata)
        };
    }
}