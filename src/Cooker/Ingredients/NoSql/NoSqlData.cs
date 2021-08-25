using System.Collections.Generic;

namespace Cooker.Ingredients.NoSql
{
    [CookerResource("nosql")]
    public record NoSqlData(
        string Id,
        NoSqlData.NoSqlContainer[] Containers) : IngredientData(Id)
    {
        public record NoSqlContainer(string Name, string PartitionKey);

        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new NoSqlIngredient(this);
        }
    }
}