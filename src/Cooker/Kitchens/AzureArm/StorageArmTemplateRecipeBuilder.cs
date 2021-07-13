using System.Collections.Generic;
using Cooker.Recipes;
using Cooker.Recipes.Storage;

namespace Cooker.Kitchens.AzureArm
{
    public class StorageArmTemplateRecipeBuilder : IRecipeBuilder
    {

        public StorageArmTemplateRecipeBuilder(Storage lineItem)
        {
            LineItem = lineItem;
        }

        public ILineItem LineItem { get; }

        public bool CanCook(IDictionary<ILineItem, ILineItemOutput> edibles)
        {
            return DepedencyHelper.IsSatisfied(LineItem.Name, edibles, out _);
        }

        public Recipe BuildCookingInstructions(IDictionary<ILineItem, ILineItemOutput> edibles)
        {
            DepedencyHelper.IsSatisfied(LineItem.Name, edibles, out var name);
            return new ArmRecipe(LineItem, output => new StorageOutput(name!));
        }
    }
}