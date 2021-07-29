using System.Collections.Generic;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public class Meal
    {
        public Dictionary<IngredientData, ICookedIngredient> CookedRecipes { get; }

        public Meal(Dictionary<IngredientData, ICookedIngredient> cookedRecipes)
        {
            CookedRecipes = cookedRecipes;
        }

        public ICookedIngredient this[IngredientData storage] => CookedRecipes[storage];
    }
}