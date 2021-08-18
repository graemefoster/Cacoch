using System.Collections.Generic;

namespace Cooker.Ingredients.Secrets
{
    public class SecretsIngredient : Ingredient<SecretsData>
    {
        internal SecretsIngredient(SecretsData secretsData) : base(secretsData)
        {
            RequiredSecrets = secretsData.Secrets;
        }

        public string[] RequiredSecrets { get; }

        public override bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            var satisfied = false;
            if (DepedencyHelper.IsSatisfied(DisplayName, edibles, out var displayName))
            {
                DisplayName = displayName!;
                satisfied = true;
            }

            return satisfied;
        }
    }
}