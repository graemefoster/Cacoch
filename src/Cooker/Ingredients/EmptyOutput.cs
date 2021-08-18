namespace Cooker.Ingredients
{
    public record EmptyOutput() : CookedIngredient<EmptyIngredientData>(new EmptyIngredientData());
}