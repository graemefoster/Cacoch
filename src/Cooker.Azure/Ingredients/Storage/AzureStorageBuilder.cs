using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Storage
{
    public class AzureStorageBuilder : IIngredientBuilder
    {
        public AzureStorageBuilder(Cooker.Ingredients.Storage.Storage ingredient)
        {
            Ingredient = ingredient;
        }

        public IIngredient Ingredient { get; }


        public IRecipe CreateRecipe(IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var armTemplate = typeof(AzureStorageBuilder).GetResourceContents("Storage");
            return new ArmRecipe<StorageOutput>(
                new ArmDefinition(armTemplate,
                    new Dictionary<string, object>()),
                output => new StorageOutput(Ingredient.DisplayName));
        }
    }
}