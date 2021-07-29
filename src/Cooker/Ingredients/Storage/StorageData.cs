namespace Cooker.Ingredients.Storage
{
    public record StorageData(
        string Id,
        string DisplayName,
        string[] Tables,
        string[] Queues,
        string[] Containers) : IngredientData(Id, DisplayName)
    {
        public override IIngredient BuildIngredient()
        {
            return new Storage(this);
        }
    }
}