using System.Collections.Generic;
using System.Linq;

namespace Cooker.Ingredients.Link
{
    public class LinkIngredient : Ingredient<LinkData>
    {
        public LinkIngredient(LinkData ingredientData) : base(ingredientData)
        {
        }

        public override bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            var fromIngredient = edibles.Keys.SingleOrDefault(x => x.Id == TypedIngredientData.FromResource);
            var toIngredient = edibles.Keys.SingleOrDefault(x => x.Id == TypedIngredientData.ToResource);
            if (fromIngredient == null || toIngredient == null) return false;
            From = edibles[fromIngredient];
            To = edibles[toIngredient];
            return true;
        }

        public ICookedIngredient? To { get; private set; }

        public ICookedIngredient? From { get; private set; }
    }
}