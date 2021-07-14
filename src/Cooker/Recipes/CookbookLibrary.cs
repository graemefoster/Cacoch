using System;
using System.Collections.Generic;
using Cooker.Kitchens;
using Cooker.Kitchens.Azure.RecipeBuilders;

namespace Cooker.Recipes
{
    public class CookbookLibrary
    {
        public IRecipeBuilder GetCookbookFor(ILineItem lineItem)
        {
            if (lineItem is Storage.Storage storage)
            {
                return new StorageRecipeBuilder(storage);
            }
            if (lineItem is Secrets.Secrets secrets)
            {
                return new SecretsRecipeBuilder(secrets);
            }
            throw new NotSupportedException("Cannot build this recipe");

        }

    }
}