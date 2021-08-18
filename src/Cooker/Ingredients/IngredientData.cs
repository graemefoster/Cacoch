
using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public abstract record IngredientData(string Id, string DisplayName)
    {
        public abstract IEnumerable<IIngredient> GatherIngredients();
    }
}