using System;
using System.Collections.Generic;

namespace Cooker.Ingredients.Link
{
    public record LinkData(string Id, string DisplayName, string FromResource, string ToResource, LinkAccess Access) :
        IngredientData(Id,
            DisplayName)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            throw new NotSupportedException("This is internal use only");
        }
    }
}