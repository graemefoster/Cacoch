namespace Cooker.Ingredients.WebApp
{
    public record WebAppOutput(WebAppDataInternal Original, string PlatformName, string HostName, string Identity) : CookedIngredient<WebAppDataInternal>(Original), IHaveRuntimeIdentity;
}