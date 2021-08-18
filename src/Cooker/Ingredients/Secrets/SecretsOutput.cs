using System.Collections.Generic;

namespace Cooker.Ingredients.Secrets
{
    public record SecretsOutput(string PlatformId, string Name, Dictionary<string, string> SecretUrls) : ICookedIngredient;

    public record EmptyOutput(string PlatformId) : ICookedIngredient;
}