namespace Cooker.Ingredients.Storage
{
    public class StorageOutput : ICookedIngredient
    {
        public StorageOutput(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}