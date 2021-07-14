using System.Collections.Generic;
using Cooker.Recipes;
using Cooker.Recipes.Storage;

namespace Cooker.Kitchens.AzureArm
{
    public class StorageTemplateRecipeBuilder : IRecipeBuilder
    {
        public StorageTemplateRecipeBuilder(Storage lineItem)
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
            return new ArmRecipe<StorageOutput>(LineItem, output => new StorageOutput(LineItem, name!));
        }
    }
}