using System;
using Cooker.Kitchens.AzureArm;
using Cooker.Recipes.Storage;

namespace Cooker.Recipes
{
    public class Bookshelf
    {
        public ICookBook GetCookbookFor(ILineItem lineItem)
        {
            if (lineItem is Storage.Storage storage)
            {
                return new StorageCookBook(storage);
            }
            throw new NotSupportedException("Cannot build this recipe");

        }

    }
}