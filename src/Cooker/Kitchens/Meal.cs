using System.Collections.Generic;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    public class Meal
    {
        public Dictionary<ILineItem, IEdible> CookedRecipes { get; }

        public Meal(Dictionary<ILineItem,IEdible> cookedRecipes)
        {
            CookedRecipes = cookedRecipes;
        }

        public IEdible this[ILineItem storage] => CookedRecipes[storage];
    }
}