namespace Cooker.Ingredients
{
    public abstract class Ingredient : IIngredient
    {
        public string Id { get; }
        public string DisplayName { get; }

        protected Ingredient(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }
}