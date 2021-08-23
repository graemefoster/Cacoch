namespace Cooker.Ingredients.Storage
{
    public record StorageOutput(
        StorageData Data, 
        string PlatformId,
        string StorageName) : CookedIngredient<StorageData>(Data),
        IHavePlatformIdentity;
}