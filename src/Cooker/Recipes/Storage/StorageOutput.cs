using Cooker.Kitchens;

namespace Cooker.Recipes.Storage
{
    public class StorageOutput : ILineItemOutput
    {
        public StorageOutput(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}