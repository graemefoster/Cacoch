namespace Cooker.Ingredients
{
    public interface IHavePlatformIdentity: ICookedIngredient
    {
        string PlatformId { get; }
    }
}