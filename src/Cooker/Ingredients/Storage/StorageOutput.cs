namespace Cooker.Ingredients.Storage
{
    public record StorageOutput(StorageData Data) : CookedIngredient<StorageData>(Data);
}