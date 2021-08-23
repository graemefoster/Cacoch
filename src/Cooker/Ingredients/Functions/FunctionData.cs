using System.Collections.Generic;

namespace Cooker.Ingredients.Functions
{
    [CookerResource("function")]
    public record FunctionData(
            string Id,
            string Classification,
            Dictionary<string, string> Configuration,
            CookerLink[]? Links)
        : IngredientData(Id), ICanAccessOtherResources
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new FunctionIngredient(new FunctionDataInternal(Id, Classification, Links));
            yield return new FunctionConfigurationIngredient(new FunctionConfigurationDataInternal($"{Id}-configuration", $"[{Id}.PlatformName]", Configuration));
            foreach (var link in this.Gather())
            {
                yield return link;
            }
        }
    }
}