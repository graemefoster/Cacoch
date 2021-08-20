namespace Cooker.Ingredients.NoSql
{
    public record NoSqlOutput(NoSqlData Data, string PlatformId) : CookedIngredient<NoSqlData>(Data),
        IHavePlatformIdentity;
}