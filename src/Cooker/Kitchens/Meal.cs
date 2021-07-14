using System.Collections.Generic;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public class Meal
    {
        public Dictionary<IIngredient, ICookedIngredient> CookedRecipes { get; }

        public Meal(Dictionary<IIngredient, ICookedIngredient> cookedRecipes)
        {
            CookedRecipes = cookedRecipes;
        }

        public ICookedIngredient this[IIngredient storage] => CookedRecipes[storage];
    }
}