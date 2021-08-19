using System.Collections.Generic;

namespace Cooker.Ingredients.Storage
{
    [CookerResource("secrets")]
    public record StorageData(
        string Id,
        string[] Tables,
        string[] Queues,
        string[] Containers) : IngredientData(Id)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new StorageIngredient(this);
        }
    }
}