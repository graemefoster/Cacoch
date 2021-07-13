using System.Collections.Generic;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    public class Meal
    {
        public Dictionary<ILineItem, ILineItemOutput> CookedRecipes { get; }

        public Meal(Dictionary<ILineItem, ILineItemOutput> cookedRecipes)
        {
            CookedRecipes = cookedRecipes;
        }

        public ILineItemOutput this[ILineItem storage] => CookedRecipes[storage];
    }
}