using System.Collections.Generic;

namespace Cooker.Ingredients.WebApp
{
    [CookerResource("webapp")]
    public record WebAppData(
            string Id,
            string Classification,
            Dictionary<string, string> Configuration,
            CookerLink[]? Links)
        : IngredientData(Id), ICanAccessOtherResources
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new WebAppIngredient(new WebAppDataInternal(Id, Classification, Links));
            yield return new WebAppConfigurationIngredient(new WebAppConfigurationDataInternal($"{Id}-configuration", $"[{Id}.PlatformName]", Configuration));
            foreach (var link in this.Gather())
            {
                yield return link;
            }
        }
    }
}