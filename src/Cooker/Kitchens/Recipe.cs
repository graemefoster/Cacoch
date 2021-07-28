
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    // ReSharper disable once UnusedTypeParameter
    public abstract class Recipe<TContext, TOutput>: IRecipe where TContext: IPlatformContext
    {
    }

    public interface IRecipe : ICookedIngredient
    {
    }
}