namespace Cooker.Ingredients.WebApp
{
    public record WebAppOutput(string PlatformId, string Name, string Identity) : ICookedIngredient, IHaveRuntimeIdentity;
}