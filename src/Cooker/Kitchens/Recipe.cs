
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    // ReSharper disable once UnusedTypeParameter
    public abstract class Recipe<TOutput>: IRecipe
    {
    }

    public interface IRecipe : ICookedIngredient
    {
    }
}