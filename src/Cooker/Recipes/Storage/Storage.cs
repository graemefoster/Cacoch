namespace Cooker.Recipes.Storage
{
    public class Storage : LineItem<StorageOutput>
    {
        public Storage(string id, string displayName) : base(id, displayName)
        {
        }
    }
}