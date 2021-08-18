using System.Collections.Generic;

namespace Cooker.Ingredients.WebApp
{
    public class WebAppIngredient : Ingredient
    {
        internal WebAppIngredient(WebAppData data) : base(data)
        {
        }

        public override bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            var satisfied = false;
            if (DepedencyHelper.IsSatisfied(DisplayName, edibles, out var displayName))
            {
                DisplayName = displayName!;
                satisfied = true;
            }

            return satisfied;
        }
    }
}