using System;

namespace Cooker.Kitchens
{
    public static class RecipeEx
    {
        public static Recipe<TOutput> Chain<TIntermediate, TOutput>(
            this Recipe<TIntermediate> initial,
            Func<TIntermediate, Recipe<TOutput>> next)
        {
            return new TwoStepRecipe<TIntermediate, TOutput>(initial, next);
        }
    }
}