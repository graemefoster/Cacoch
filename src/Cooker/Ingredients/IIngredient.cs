using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public interface IIngredient
    {
        string Id { get; }
        string DisplayName { get; }
        bool CanCook(IDictionary<IIngredient, ICookedIngredient> edibles);
    }
}