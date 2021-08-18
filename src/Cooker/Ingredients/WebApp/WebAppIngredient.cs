using System.Collections.Generic;

namespace Cooker.Ingredients.WebApp
{
    public class WebAppIngredient : Ingredient<WebAppData>
    {
        internal WebAppIngredient(WebAppData data) : base(data)
        {
        }
        
        public Dictionary<string, string> Configuration { get;private set; }

        public override bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            if (!DepedencyHelper.IsSatisfied(DisplayName, edibles, out var displayName))
            {
                return false;
            }
            DisplayName = displayName!;

            Configuration = new Dictionary<string, string>();
            foreach (var config in base.TypedIngredientData.Configuration)
            {
                if (!DepedencyHelper.IsSatisfied(config.Value, edibles, out var configValue))
                {
                    return false;
                }

                Configuration.Add(config.Key, configValue!);
            }

            return true;
        }
    }
}