using System.Collections.Generic;

namespace Cooker.Ingredients.Secrets
{
    public record SecretsOutput(SecretsData Data, string PlatformId, Dictionary<string, string> SecretUrls) : CookedIngredient<SecretsData>(Data), IHavePlatformIdentity;
}