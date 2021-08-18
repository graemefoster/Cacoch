using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public abstract class Ingredient<T> : IIngredient where T: IngredientData
    {
        public T TypedIngredientData { get; }
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

        protected Ingredient(T ingredientData)
        {
            TypedIngredientData = ingredientData;
            Id = ingredientData.Id;
            DisplayName = ingredientData.DisplayName;
            OriginalIngredientData = ingredientData;
        }
    }
}