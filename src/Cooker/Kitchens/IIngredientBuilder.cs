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
        /// Based on existing cooked items, decide if you are ready to cook or not 
        /// </summary>
        /// <param name="edibles"></param>
        /// <returns></returns>
        bool CanCook(IDictionary<IIngredient, ICookedIngredient> edibles);
        
        /// <summary>
        /// Create a recipe which will be used to cook the item
        /// </summary>
        /// <param name="cooked"></param>
        /// <returns></returns>
        IRecipe CreateRecipe(IDictionary<IIngredient, ICookedIngredient> cooked);
    }
}