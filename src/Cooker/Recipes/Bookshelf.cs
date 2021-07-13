using System;
using Cooker.Kitchens.AzureArm;

namespace Cooker.Recipes
{
    public class Bookshelf
    {
        public IRecipeBuilder GetCookbookFor(ILineItem lineItem)
        {
            if (lineItem is Storage.Storage storage)
            {
                return new StorageArmTemplateRecipeBuilder(storage);
            }
            throw new NotSupportedException("Cannot build this recipe");

        }

    }
}