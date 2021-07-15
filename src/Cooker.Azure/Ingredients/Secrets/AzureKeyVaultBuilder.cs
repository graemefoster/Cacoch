using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Secrets
{
    public class AzureKeyVaultBuilder : IIngredientBuilder
    {
        public AzureKeyVaultBuilder(Cooker.Ingredients.Secrets.Secrets ingredient)
        {
            Ingredient = ingredient;
        }

        public IIngredient Ingredient { get; }


        public IRecipe CreateRecipe(IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            return new ArmRecipe<IntermediateOutput>(
                    new ArmDefinition(
                        "",
                        new Dictionary<string, object>()),
                    output => new IntermediateOutput())
                .Chain(i =>
                    new ArmRecipe<SecretsOutput>(
                        new ArmDefinition(
                            "",
                            new Dictionary<string, object>()),
                        o => new SecretsOutput(Ingredient.DisplayName)));
        }

        private class IntermediateOutput : ICookedIngredient
        {
        }
    }
}