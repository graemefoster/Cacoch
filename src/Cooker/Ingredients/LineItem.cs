namespace Cooker.Ingredients
{
    public abstract class LineItem<TOutput> : ILineItem where TOutput : ILineItemOutput
    {
        public string Id { get; }
        public string DisplayName { get; }

        protected LineItem(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }
}