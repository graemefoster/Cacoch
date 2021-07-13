using Cooker.Kitchens;

namespace Cooker.Recipes.Storage
{
    public class StorageOutput : IEdible
    {
        public StorageOutput(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}