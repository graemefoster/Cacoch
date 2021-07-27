using System.Collections.Generic;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public interface IIngredientBuilder<T> : IIngredientBuilder where T: IIngredient
    {
        /// <summary>
        /// Original item to cook
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        T Ingredient { get; }
    }

    public interface IIngredientBuilder
    {
        /// <summary>
        /// Create a recipe which will be used to cook the item
        /// </summary>
        /// <param name="platformContext"></param>
        /// <param name="cooked"></param>
        /// <returns></returns>
        IRecipe CreateRecipe(IPlatformContext platformContext, IDictionary<IIngredient, ICookedIngredient> cooked);
    }
}