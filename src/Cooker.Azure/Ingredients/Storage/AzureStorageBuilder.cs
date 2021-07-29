﻿using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Storage
{
    public class AzureStorageBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public AzureStorageBuilder(Cooker.Ingredients.Storage.StorageIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private Cooker.Ingredients.Storage.StorageIngredient Ingredient { get; }


        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var armTemplate = typeof(AzureStorageBuilder).GetResourceContents("Storage");
            return new ArmRecipe<StorageOutput>(
                new ArmDefinition(armTemplate,
                    new Dictionary<string, object>()
                    {
                        {"storageAccountName", Ingredient.Id},
                        {"tables", Ingredient.Tables},
                        {"queues", Ingredient.Queues},
                        {"containers", Ingredient.Containers}
                    }),
                output => new StorageOutput(Ingredient.DisplayName));
        }
    }
}