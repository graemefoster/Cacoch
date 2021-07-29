namespace Cooker.Ingredients.Storage
{
    public record StorageOutput(string Name) : ICookedIngredient;

    [CookerResource("secrets")]
    public record StorageData(
        string Id,
        string DisplayName,
        string[] Tables,
        string[] Queues,
        string[] Containers) : IngredientData(Id, DisplayName)
    {
        public override IIngredient BuildIngredient()
        {
            return new StorageIngredient(this);
        }
    }
}