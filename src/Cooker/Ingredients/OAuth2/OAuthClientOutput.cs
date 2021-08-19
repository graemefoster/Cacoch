namespace Cooker.Ingredients.OAuth2
{
    public record OAuthClientOutput(
        OAuthClientData Data,
        string Identity,
        string? ClientSecret) : CookedIngredient<OAuthClientData>(Data), IHaveRuntimeIdentity;
}