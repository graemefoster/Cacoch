using System;
using System.Net;

namespace Cooker.Kitchens
{
    public static class RecipeEx
    {
        public static Recipe<TContext, TOutput> Then<TContext, TIntermediate, TOutput>(
            this Recipe<TContext, TIntermediate> initial,
            Func<TIntermediate, Recipe<TContext, TOutput>> next) where TContext: IPlatformContext
        {
            return new TwoStepRecipe<TContext,TIntermediate, TOutput>(initial, next);
        }
    }
}