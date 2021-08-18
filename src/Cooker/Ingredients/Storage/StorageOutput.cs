namespace Cooker.Ingredients.Storage
{
    public record StorageOutput(string PlatformId, string Name) : ICookedIngredient;
}