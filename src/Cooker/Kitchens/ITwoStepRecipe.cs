using System.Threading.Tasks;

namespace Cooker.Kitchens
{
    public interface ITwoStepRecipe<TContext> where TContext: IPlatformContext
    {
        IRecipe InitialStep { get; }
        Task<IRecipe> Cook(TContext context, Docket docket, KitchenStation<TContext> station);
    }
}