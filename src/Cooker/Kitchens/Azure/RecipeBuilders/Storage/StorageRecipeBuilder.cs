using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Ingredients.Storage;

namespace Cooker.Kitchens.Azure.RecipeBuilders.Storage
{
    public class StorageRecipeBuilder : IRecipeBuilder
    {
        public StorageRecipeBuilder(Ingredients.Storage.Storage lineItem)
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
            var armTemplate = typeof(StorageRecipeBuilder).GetResourceContents("Storage");
            return new ArmRecipe<StorageOutput>(
                new ArmDefinition(armTemplate,
                    new Dictionary<string, object>()),
                output => new StorageOutput(name!));
        }
    }
}