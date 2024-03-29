﻿using System.Collections.Generic;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    // public interface IIngredientBuilder<T> : IIngredientBuilder where T: IIngredient
    // {
    //     /// <summary>
    //     /// Original item to cook
    //     /// </summary>
    //     // ReSharper disable once UnusedMemberInSuper.Global
    //     T Ingredient { get; }
    // }

    public interface IRecipeBuilder<TContext>
        where TContext : IPlatformContext
    {
        /// <summary>
        /// Create a recipe which will be used to cook the item
        /// </summary>
        /// <param name="platformContext"></param>
        /// <param name="environment"></param>
        /// <param name="docket"></param>
        /// <param name="cooked"></param>
        /// <returns></returns>
        IRecipe CreateRecipe(
            TContext platformContext,
            PlatformEnvironment environment,
            Docket docket,
            IDictionary<IIngredient, ICookedIngredient> cooked);
    }
}