using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public abstract class Ingredient : IIngredient
    {
        public string Id { get; }
        public string DisplayName { get; protected set; }
        public abstract bool CanCook(IDictionary<IIngredient, ICookedIngredient> edibles);

        protected Ingredient(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }
}