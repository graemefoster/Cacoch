using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;

namespace Cooker.Kitchens.Azure.Ingredients.Secrets
{
    public class AzureKeyVaultBuilder : IIngredientBuilder
    {
        public AzureKeyVaultBuilder(Cooker.Ingredients.Secrets.Secrets ingredient)
        {
            Ingredient = ingredient;
        }

        public IIngredient Ingredient { get; }

        public bool CanCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            return DepedencyHelper.IsSatisfied(Ingredient.DisplayName, edibles, out _);
        }

        public IRecipe CreateRecipe(IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            DepedencyHelper.IsSatisfied(Ingredient.DisplayName, cooked, out var name);

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
                        o => new SecretsOutput(name!)));
        }

        private class IntermediateOutput : ICookedIngredient
        {
        }
    }
}