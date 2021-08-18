using System.Collections.Generic;

namespace Cooker.Ingredients.Storage
{
    [CookerResource("secrets")]
    public record StorageData(
        string Id,
        string DisplayName,
        string[] Tables,
        string[] Queues,
        string[] Containers) : IngredientData(Id, DisplayName)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new StorageIngredient(this);
        }
    }
}