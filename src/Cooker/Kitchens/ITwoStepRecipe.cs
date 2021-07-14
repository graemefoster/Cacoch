using System.Threading.Tasks;

namespace Cooker.Kitchens
{
    public interface ITwoStepRecipe
    {
        IRecipe InitialStep { get; }
        Task<IRecipe> Cook(Docket docket, KitchenStation station);
    }
}