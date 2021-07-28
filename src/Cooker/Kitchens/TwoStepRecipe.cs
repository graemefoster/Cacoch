using System;
using System.Threading.Tasks;

namespace Cooker.Kitchens
{
    public class TwoStepRecipe<TContext, TIntermediate, TOutput> : Recipe<TContext, TOutput>, 
        ITwoStepRecipe<TContext> where TContext: IPlatformContext
    {
        private readonly Recipe<TContext, TIntermediate> _initial;
        private readonly Func<TIntermediate, Recipe<TContext, TOutput>> _next;

        public TwoStepRecipe(Recipe<TContext, TIntermediate> initial, Func<TIntermediate, Recipe<TContext, TOutput>> next)
        {
            _initial = initial;
            _next = next;
        }

        public async Task<IRecipe> Cook(TContext context, Docket docket, KitchenStation<TContext> station)
        {
            return _next((TIntermediate)await station.CookRecipe(context, docket, _initial));
        }

        public IRecipe InitialStep => _initial;
    }
}