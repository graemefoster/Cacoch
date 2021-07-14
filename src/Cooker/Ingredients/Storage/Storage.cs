namespace Cooker.Ingredients.Storage
{
    public class Storage : LineItem<StorageOutput>
    {
        public Storage(string id, string displayName) : base(id, displayName)
        {
        }
    }
}