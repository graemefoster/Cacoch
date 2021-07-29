using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public abstract class Ingredient : IIngredient
    {
        public IngredientData OriginalIngredientData { get; }
        public string Id { get; }
        public string DisplayName { get; protected set; }
        public abstract bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles);

        protected Ingredient(IngredientData ingredientData)
        {
            OriginalIngredientData = ingredientData;
            Id = ingredientData.Id;
            DisplayName = ingredientData.DisplayName;
        }
    }
}