using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public interface IIngredient
    {
        IngredientData OriginalIngredientData { get; }
        string Id { get; }

        /// <summary>
        /// Opportunity to evaluate any expressions in incoming properties
        /// and set them based on baked ingredients.
        /// </summary>
        /// <param name="edibles">All baked ingredients</param>
        /// <returns>True if ready to bake (all dependencies satisfied)</returns>
        bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles);
    }
}