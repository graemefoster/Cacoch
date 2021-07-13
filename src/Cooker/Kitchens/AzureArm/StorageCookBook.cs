using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            var name = _lineItem.Name;
            if (name.Contains("["))
            {
                var expression = name[(name.IndexOf("[", StringComparison.Ordinal) + 1)..name.LastIndexOf("]", StringComparison.Ordinal)]!;
                var dependencyLineItem = expression.Split(".")[0];
                return edibles.Keys.Any(x => x.Name == dependencyLineItem);
            }
            return true;
        }

        public Recipe BuildCookingInstructions(IDictionary<ILineItem, IEdible> edibles)
        {
            return new ArmRecipe(_lineItem, "", output => new StorageOutput(GetName(edibles)));
        }

        private string GetName(IDictionary<ILineItem, IEdible> edibles)
        {
            var name = _lineItem.Name;
            if (name.Contains("["))
            {
                var expression = name[(name.IndexOf("[", StringComparison.Ordinal) + 1)..name.LastIndexOf("]", StringComparison.Ordinal)]!;
                var dependencyLineItemName = expression.Split(".")[0];
                var dependencyLineItem = edibles.Keys.Single(x => x.Name == dependencyLineItemName);
                return _lineItem.Name.Replace($"[{expression}]", edibles[dependencyLineItem].Name);
            }

            return name;
        }
    }
}