
namespace Cooker.Ingredients
{
    public abstract record IngredientData(string Id, string DisplayName)
    {
        public abstract IIngredient BuildIngredient();
    }
}