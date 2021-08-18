namespace Cooker.Ingredients.WebApp
{
    public record WebAppOutput(WebAppData Original, string Identity) : CookedIngredient<WebAppData>(Original), IHaveRuntimeIdentity;
}