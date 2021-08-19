using System.Collections.Generic;

namespace Cooker.Ingredients.WebApp
{
    public record WebAppConfigurationDataInternal(
        string Id,
        string PlatformName,
        Dictionary<string, string> Configuration) : IngredientData(Id)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new WebAppConfigurationIngredient(this);
        }
    }
}