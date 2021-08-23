using System;
using System.Collections.Generic;

namespace Cooker.Ingredients.Link
{
    [CookerResource("link")]
    public record LinkData(string Id, string FromResource, string ToResource, LinkAccess Access) :
        IngredientData(Id)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            throw new NotSupportedException("This is internal use only");
        }
    }
}