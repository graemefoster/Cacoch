using System;
using Cooker.Kitchens.AzureArm;
using Cooker.Recipes.Storage;

namespace Cooker.Recipes
{
    public class RecipeBook
    {
        public Recipe Lookup(ILineItem lineItem)
        {
            if (lineItem is Storage.Storage storage)
            {
                return new ArmRecipe(storage, CreateArmFor(storage), o => new StorageOutput(storage.Name));
            }

            throw new NotSupportedException("Cannot build this recipe");
        }

        private string CreateArmFor(Storage.Storage storage)
        {
            return @"{""type"" : ""ARM!""}";
        }
    }
}