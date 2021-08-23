using System.Collections.Generic;

namespace Cooker.Ingredients.WebApp
{
    public record WebAppDataInternal(
        string Id,
        string Classification,
        CookerLink[]? Links) : IngredientData(Id),
        ICanAccessOtherResources
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new WebAppIngredient(this);
            foreach (var link in this.Gather())
            {
                yield return link;
            }
        }
    }
}