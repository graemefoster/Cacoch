using System;
using System.Collections.Generic;
using Cooker.Kitchens.AzureArm;

namespace Cooker.Recipes
{
    public class CookbookLibrary
    {
        public IRecipeBuilder GetCookbookFor(ILineItem lineItem)
        {
            if (lineItem is Storage.Storage storage)
            {
                return new StorageTemplateRecipeBuilder(storage);
            }
            if (lineItem is Secrets.Secrets secrets)
            {
                return new SecretsTemplateRecipeBuilder(secrets);
            }
            throw new NotSupportedException("Cannot build this recipe");

        }

    }
}