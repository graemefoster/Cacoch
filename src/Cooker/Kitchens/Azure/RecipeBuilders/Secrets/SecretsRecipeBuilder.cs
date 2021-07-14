using System.Collections.Generic;
using Cooker.Recipes;
using Cooker.Recipes.Secrets;

namespace Cooker.Kitchens.Azure.RecipeBuilders.Secrets
{
    public class SecretsRecipeBuilder : IRecipeBuilder
    {
        public SecretsRecipeBuilder(Recipes.Secrets.Secrets lineItem)
        {
            LineItem = lineItem;
        }

        public ILineItem LineItem { get; }

        public bool CanCook(IDictionary<ILineItem, ILineItemOutput> edibles)
        {
            return DepedencyHelper.IsSatisfied(LineItem.DisplayName, edibles, out _);
        }

        public IRecipe CreateRecipe(IDictionary<ILineItem, ILineItemOutput> cooked)
        {
            DepedencyHelper.IsSatisfied(LineItem.DisplayName, cooked, out var name);

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

        private class IntermediateOutput : ILineItemOutput
        {
        }
    }
}