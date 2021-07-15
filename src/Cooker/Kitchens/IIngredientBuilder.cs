using System.Collections.Generic;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public interface IIngredientBuilder
    {
        /// <summary>
        /// Original item to cook
        /// </summary>
        IIngredient Ingredient { get; }

        /// <summary>
        /// Create a recipe which will be used to cook the item
        /// </summary>
        /// <param name="cooked"></param>
        /// <returns></returns>
        IRecipe CreateRecipe(IDictionary<IIngredient, ICookedIngredient> cooked);
    }
}