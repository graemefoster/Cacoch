using Cooker.Recipes;

namespace Cooker.Kitchens.Azure
{
    public interface IArmRecipe
    {
        ILineItemOutput Output(object armOutputs);
    }
}