using System;
using Cooker.Kitchens;
using Cooker.Kitchens.Azure.Ingredients.Secrets;
using Cooker.Kitchens.Azure.Ingredients.Storage;

namespace Cooker.Ingredients
{
    public class CookbookLibrary
    {
        public IIngredientBuilder GetCookbookFor(IIngredient ingredient)
        {
            if (ingredient is Storage.Storage storage)
            {
                return new StorageIngredientBuilder(storage);
            }
            if (ingredient is Secrets.Secrets secrets)
            {
                return new AzureKeyVaultBuilder(secrets);
            }
            throw new NotSupportedException("Cannot build this recipe");

        }

    }
}