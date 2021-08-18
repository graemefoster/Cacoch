namespace Cooker.Ingredients.Secrets
{
    public record SecretsOutput(string PlatformId, string Name) : ICookedIngredient;

    public record EmptyOutput(string PlatformId) : ICookedIngredient;
}