using System;
using System.Threading.Tasks;

namespace Cooker.Kitchens
{
    public class TwoStepRecipe<TIntermediate, TOutput> : Recipe<TOutput>, ITwoStepRecipe
    {
        private readonly Recipe<TIntermediate> _initial;
        private readonly Func<TIntermediate, Recipe<TOutput>> _next;

        public TwoStepRecipe(Recipe<TIntermediate> initial, Func<TIntermediate, Recipe<TOutput>> next)
        {
            _initial = initial;
            _next = next;
        }

        public async Task<IRecipe> Cook(KitchenStation station)
        {
            return _next((TIntermediate)await station.CookRecipe(_initial));
        }

        public IRecipe InitialStep => _initial;
    }
}