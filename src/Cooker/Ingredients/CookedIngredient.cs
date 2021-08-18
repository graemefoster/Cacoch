namespace Cooker.Ingredients
{
    public record CookedIngredient<TData>(TData Data) : ICookedIngredient where TData : IngredientData;
}