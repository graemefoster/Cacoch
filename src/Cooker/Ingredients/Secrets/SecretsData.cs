using System.Collections.Generic;
using System.ComponentModel;

namespace Cooker.Ingredients.Secrets
{
    [CookerResource("secrets")]
    public record SecretsData(
        string Id,
        [DefaultValue(new string[] { })] string[] Secrets,
        SecretsData.KnownSecret[]? KnownSecrets
    ) : IngredientData(Id)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new SecretsIngredient(this);
        }

        public record KnownSecret(string Name, string Value);
    }
}