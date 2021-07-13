using System;

namespace Cooker.Kitchens
{
    public class MultiStepRecipe : Recipe
    {
        private readonly Recipe _initialRecipe;
        private readonly Func<Recipe> _nextRecipe;

        public MultiStepRecipe(Recipe initialRecipe, Func<Recipe> nextRecipe)
        {
            _initialRecipe = initialRecipe;
            _nextRecipe = nextRecipe;
        }
    }
}