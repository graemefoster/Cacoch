using System.Collections.Generic;

namespace Cooker.Ingredients.NoSql
{
    [CookerResource("nosql")]
    public record NoSqlData(
        string Id,
        string[] Containers) : IngredientData(Id)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new NoSqlIngredient(this);
        }
    }
}