namespace Cooker.Ingredients.Storage
{
    public record StorageOutput(StorageData Data, string PlatformId) : CookedIngredient<StorageData>(Data),
        IHavePlatformIdentity;
}