using System.Collections.Generic;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public interface ICookBook
    {
        bool CanCook(IDictionary<ILineItem, IEdible> edibles);
        Recipe BuildCookingInstructions(IDictionary<ILineItem, IEdible> edibles);
    }
}