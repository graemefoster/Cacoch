using System.Collections.Generic;
using Cooker.Recipes;
using Cooker.Recipes.Storage;

namespace Cooker.Kitchens.AzureArm
{
    public class StorageCookBook : ICookBook
    {
        private readonly Storage _lineItem;

        public StorageCookBook(Storage lineItem)
        {
            _lineItem = lineItem;
        }

        public bool CanCook(IDictionary<ILineItem, IEdible> edibles)
        {
            return DepedencyHelper.IsSatisfied(_lineItem.Name, edibles, out _);
        }

        public Recipe BuildCookingInstructions(IDictionary<ILineItem, IEdible> edibles)
        {
            DepedencyHelper.IsSatisfied(_lineItem.Name, edibles, out var name);
            return new ArmRecipe(_lineItem, "", output => new StorageOutput(name!));
        }
    }
}