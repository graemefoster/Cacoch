using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public record EmptyIngredientData() : IngredientData(string.Empty, string.Empty)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            throw new System.NotImplementedException();
        }
    }
}