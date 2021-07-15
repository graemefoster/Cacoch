using System.Collections.Generic;

namespace Cooker.Ingredients.Storage
{
    public class Storage : Ingredient
    {
        public Storage(string id, string displayName) : base(id, displayName)
        {
        }
        
        
        public override bool CanCook(IDictionary<IIngredient, ICookedIngredient> edibles)
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