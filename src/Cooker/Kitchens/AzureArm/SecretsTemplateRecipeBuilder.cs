using System.Collections.Generic;
using Cooker.Recipes;
using Cooker.Recipes.Secrets;
using Cooker.Recipes.Storage;

namespace Cooker.Kitchens.AzureArm
{
    public class SecretsTemplateRecipeBuilder : IRecipeBuilder
    {

        public SecretsTemplateRecipeBuilder(Secrets lineItem)
        {
            LineItem = lineItem;
        }

        public ILineItem LineItem { get; }

        public bool CanCook(IDictionary<ILineItem, ILineItemOutput> edibles)
        {
            return DepedencyHelper.IsSatisfied(LineItem.Name, edibles, out _);
        }

        public IRecipe CreateRecipe(IDictionary<ILineItem, ILineItemOutput> cooked)
        {
            DepedencyHelper.IsSatisfied(LineItem.Name, cooked, out var name);
            return new ArmRecipe<ArmRecipe<SecretsOutput>>(LineItem, output => new ArmRecipe<SecretsOutput>(LineItem, o => new SecretsOutput(LineItem, LineItem.Name)));
        }
    }
}