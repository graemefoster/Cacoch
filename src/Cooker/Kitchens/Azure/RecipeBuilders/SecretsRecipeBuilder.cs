using System.Collections.Generic;
using Cooker.Recipes;
using Cooker.Recipes.Secrets;

namespace Cooker.Kitchens.Azure.RecipeBuilders
{
    public class SecretsRecipeBuilder : IRecipeBuilder
    {

        public SecretsRecipeBuilder(Secrets lineItem)
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
            
            return new ArmRecipe<IntermediateOutput>(output => new IntermediateOutput())
                .Chain(i => new ArmRecipe<SecretsOutput>(o => new SecretsOutput(name!)));
        }

        private class IntermediateOutput : ILineItemOutput
        {
        }
    }
}