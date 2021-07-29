using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public abstract class Ingredient : IIngredient
    {
        public IngredientData OriginalIngredientData { get; }
        public string Id { get; }
        public string DisplayName { get; protected set; }
        
        /// <summary>
        /// Implementors should use this as a hook to evaluate expressions provided in parameters.
        /// If you are confident that you are ready to build platform resources then return true. 
        /// </summary>
        /// <param name="edibles"></param>
        /// <returns>True if ready to cook. False if not.</returns>
        public abstract bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles);

        protected Ingredient(IngredientData ingredientData)
        {
            OriginalIngredientData = ingredientData;
            Id = ingredientData.Id;
            DisplayName = ingredientData.DisplayName;
        }
    }
}