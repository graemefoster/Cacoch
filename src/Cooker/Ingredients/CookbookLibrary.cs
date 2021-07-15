using System;
using System.Collections.Generic;
using Cooker.Kitchens;

namespace Cooker.Ingredients
{
    public class CookbookLibrary
    {
        private readonly IDictionary<Type, Func<IIngredient, IIngredientBuilder>> _ingredientBuilders;

        public CookbookLibrary(IDictionary<Type, Func<IIngredient, IIngredientBuilder>> ingredientBuilders)
        {
            _ingredientBuilders = ingredientBuilders;
        }

        public IIngredientBuilder GetCookbookFor(IIngredient ingredient)
        {
            if (_ingredientBuilders.TryGetValue(ingredient.GetType(), out var builder))
            {
                return builder(ingredient);
            }

            throw new NotSupportedException("Cannot build this recipe");
        }
    }
}