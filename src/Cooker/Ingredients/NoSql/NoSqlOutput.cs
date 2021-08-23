namespace Cooker.Ingredients.NoSql
{
    public record NoSqlOutput(NoSqlData Data, string PlatformId, string ConnectionString) : CookedIngredient<NoSqlData>(Data),
        IHavePlatformIdentity;
}