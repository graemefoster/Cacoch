using System.Linq;
using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    internal class TwoStepKitchenStation : KitchenStation
    {
        private readonly KitchenStation[] _allStations;

        public TwoStepKitchenStation(KitchenStation[] allStations)
        {
            _allStations = allStations;
        }
        
        public override async Task<ILineItemOutput> CookRecipe(IRecipe recipe)
        {
            var twoStepRecipe = (ITwoStepRecipe) recipe;
            var station = _allStations.Single(x => x.CanCook(twoStepRecipe.InitialStep));
            return await twoStepRecipe.Cook(station);
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is ITwoStepRecipe;
        }
    }
}