using System;
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
            var fromIngredient = edibles.Keys.SingleOrDefault(x => x.Id == OriginalTypedIngredientData.FromResource);
            var toIngredient = edibles.Keys.SingleOrDefault(x => x.Id == OriginalTypedIngredientData.ToResource);
            if (fromIngredient == null || toIngredient == null) return false;
            From = edibles[fromIngredient];
            To = edibles[toIngredient] as IHavePlatformIdentity
                 ?? throw new NotSupportedException(
                     $"Invalid attempt to link to {edibles[toIngredient].GetType().Name}. Target of a link must support IPlatformIdentity");

            TypedIngredientData = OriginalTypedIngredientData;
            return true;
        }

        public IHavePlatformIdentity? To { get; private set; }

        public ICookedIngredient? From { get; private set; }
    }
}