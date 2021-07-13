using System.Collections.Generic;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public interface IRecipeBuilder
    {
        /// <summary>
        /// Original item to cook
        /// </summary>
        ILineItem LineItem { get; }

        /// <summary>
        /// Based on existing cooked items, decide if you are ready to cook or not 
        /// </summary>
        /// <param name="edibles"></param>
        /// <returns></returns>
        bool CanCook(IDictionary<ILineItem, ILineItemOutput> edibles);
        
        /// <summary>
        /// Create a recipe which will be used to cook the item
        /// </summary>
        /// <param name="edibles"></param>
        /// <returns></returns>
        Recipe BuildCookingInstructions(IDictionary<ILineItem, ILineItemOutput> edibles);
    }
}