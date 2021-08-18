using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public abstract class Ingredient<T> : IIngredient where T : IngredientData
    {
        protected T OriginalTypedIngredientData { get; }
        public T? TypedIngredientData { get; protected set; }
        public IngredientData OriginalIngredientData { get; }
        public string Id { get; }

        /// <summary>
        /// Implementors should use this as a hook to evaluate expressions provided in parameters.
        /// If you are confident that you are ready to build platform resources then return true. 
        /// </summary>
        /// <param name="edibles"></param>
        /// <returns>True if ready to cook. False if not.</returns>
        public virtual bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            if (DataCloner.Clone(OriginalTypedIngredientData, edibles, out var newData))
            {
                TypedIngredientData = newData!;
                return true;
            }

            return false;
        }

        protected Ingredient(T ingredientData)
        {
            OriginalTypedIngredientData = ingredientData;
            Id = ingredientData.Id;
            OriginalIngredientData = ingredientData;
        }
    }
}