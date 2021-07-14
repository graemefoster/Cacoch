using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Ingredients.Storage;

namespace Cooker.Kitchens.Azure.Ingredients.Storage
{
    public class StorageIngredientBuilder : IIngredientBuilder
    {
        public StorageIngredientBuilder(Cooker.Ingredients.Storage.Storage ingredient)
        {
            Ingredient = ingredient;
        }

        public IIngredient Ingredient { get; }

        public bool CanCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            return DepedencyHelper.IsSatisfied(Ingredient.DisplayName, edibles, out _);
        }

        public IRecipe CreateRecipe(IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            DepedencyHelper.IsSatisfied(Ingredient.DisplayName, cooked, out var name);
            var armTemplate = typeof(StorageIngredientBuilder).GetResourceContents("Storage");
            return new ArmRecipe<StorageOutput>(
                new ArmDefinition(armTemplate,
                    new Dictionary<string, object>()),
                output => new StorageOutput(name!));
        }
    }
}