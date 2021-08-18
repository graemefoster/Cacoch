
using System;

namespace Cooker.Kitchens
{
    // ReSharper disable once UnusedTypeParameter
    public abstract class Recipe<TContext, TOutput>: IRecipe where TContext: IPlatformContext
    {
        public string? PlatformId => throw new NotSupportedException("Recipes do not have platform identifiers");
    }
}