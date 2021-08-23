using System.Collections.Generic;

namespace Cooker.Ingredients.Functions
{
    public record FunctionConfigurationDataInternal(
        string Id,
        string PlatformName,
        Dictionary<string, string> Configuration) : IngredientData(Id)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new FunctionConfigurationIngredient(this);
        }
    }
}