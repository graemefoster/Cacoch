
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public abstract class Recipe<TOutput>: IRecipe
    {
    }

    public interface IRecipe : ILineItemOutput
    {
    }
}