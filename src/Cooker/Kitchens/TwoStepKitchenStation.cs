using System.Linq;
using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    internal class TwoStepKitchenStation<TContext> : KitchenStation<TContext> where TContext: IPlatformContext
    {
        private readonly KitchenStation<TContext>[] _allStations;

        public TwoStepKitchenStation(KitchenStation<TContext>[] allStations)
        {
            _allStations = allStations;
        }
        
        public override async Task<ICookedIngredient> CookRecipe(TContext context, Docket docket, IRecipe recipe)
        {
            var twoStepRecipe = (ITwoStepRecipe<TContext>) recipe;
            var station = _allStations.Single(x => x.CanCook(twoStepRecipe.InitialStep));
            return await twoStepRecipe.Cook(context, docket, station);
        }

        public override bool CanCook(IRecipe recipe)
        {
            return recipe is ITwoStepRecipe<TContext>;
        }
    }
}