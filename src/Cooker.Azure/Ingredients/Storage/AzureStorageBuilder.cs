using System.Collections.Generic;
using Cooker.Azure.KitchenStations.Arm;
using Cooker.Ingredients;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Storage
{
    public class AzureStorageBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public AzureStorageBuilder(StorageIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private StorageIngredient Ingredient { get; }


        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var storageName = (Ingredient.Id + platformContext.Randomness).ToLowerInvariant();
            var armTemplate = typeof(AzureStorageBuilder).GetResourceContents("Storage");
            return new ArmRecipe<StorageOutput>(
                new ArmDefinition(
                    $"storage-{Ingredient.Id}",
                    armTemplate,
                    new Dictionary<string, object>()
                    {
                        { "storageAccountName", storageName },
                        { "tables", Ingredient.Tables },
                        { "queues", Ingredient.Queues },
                        { "containers", Ingredient.Containers }
                    }),
                output => new StorageOutput(
                    (string)output["resourceId"],
                    Ingredient.DisplayName));
        }
    }
}