using System.Collections.Generic;

namespace Cooker.Ingredients.Functions
{
    public record FunctionDataInternal(
        string Id,
        string Classification,
        CookerLink[]? Links) : IngredientData(Id),
        ICanAccessOtherResources
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new FunctionIngredient(this);
            foreach (var link in this.Gather())
            {
                yield return link;
            }
        }
    }
}