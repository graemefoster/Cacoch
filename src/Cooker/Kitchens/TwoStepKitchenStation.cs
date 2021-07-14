using System.Linq;
using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    internal class TwoStepKitchenStation : KitchenStation
    {
        private readonly KitchenStation[] _allStations;

        public TwoStepKitchenStation(KitchenStation[] allStations)
        {
            _allStations = allStations;
        }
        
        public override async Task<ICookedIngredient> CookRecipe(Docket docket, IRecipe recipe)
        {
            var twoStepRecipe = (ITwoStepRecipe) recipe;
            var station = _allStations.Single(x => x.CanCook(twoStepRecipe.InitialStep));
            return await twoStepRecipe.Cook(docket, station);
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is ITwoStepRecipe;
        }
    }
}