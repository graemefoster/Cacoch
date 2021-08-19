
using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public abstract record IngredientData(string Id)
    {
        public abstract IEnumerable<IIngredient> GatherIngredients();
    }
}