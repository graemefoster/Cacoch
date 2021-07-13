using Cooker.Kitchens;

namespace Cooker.Recipes
{
    public interface ILineItem
    {
        string Name { get; }
    }

    public abstract class LineItem<TOutput> : ILineItem where TOutput : IEdible
    {
        public string Name { get; }

        protected LineItem(string name)
        {
            Name = name;
        }
    }
}