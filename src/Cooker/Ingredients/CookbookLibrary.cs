using System;
using System.Collections.Generic;
using Cooker.Kitchens;

namespace Cooker.Ingredients
{
    public class CookbookLibrary
    {
        private readonly IDictionary<Type, Type> _ingredientBuilders;

        public CookbookLibrary(IDictionary<Type, Type> ingredientBuilders)
        {
            _ingredientBuilders = ingredientBuilders;
        }

        public IIngredientBuilder GetCookbookFor<T>(T ingredient) where T : IIngredient
        {
            if (_ingredientBuilders.TryGetValue(ingredient.GetType(), out var builderType))
            {
                return (IIngredientBuilder) Activator.CreateInstance(
                    builderType,
                    ingredient)!;
            }

            throw new NotSupportedException("Cannot build this recipe");
        }
    }
}