using System.Collections.Generic;

namespace Cooker.Ingredients.Secrets
{
    public class Secrets : Ingredient
    {
        public Secrets(string id, string displayName) : base(id, displayName)
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