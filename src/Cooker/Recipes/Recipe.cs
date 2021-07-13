namespace Cooker.Recipes
{
    public abstract class Recipe
    {
        public ILineItem LineItem { get; }

        public Recipe(ILineItem lineItem)
        {
            LineItem = lineItem;
        }
    }
}