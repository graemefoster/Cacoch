using System.Collections.Generic;
using Cooker.Recipes;
using Cooker.Recipes.Storage;

namespace Cooker.Kitchens.Azure.RecipeBuilders
{
    public class StorageRecipeBuilder : IRecipeBuilder
    {
        public StorageRecipeBuilder(Storage lineItem)
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
            return new ArmRecipe<StorageOutput>(output => new StorageOutput(name!));
        }
    }
}