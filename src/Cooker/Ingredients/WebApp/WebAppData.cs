using System.Collections.Generic;

namespace Cooker.Ingredients.WebApp
{
    [CookerResource("webapp")]
    public record WebAppData(
            string Id,
            string Classification,
            Dictionary<string, string> Configuration,
            IEnumerable<CookerLink>? Links)
        : IngredientData(Id), ICanAccessOtherResources
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