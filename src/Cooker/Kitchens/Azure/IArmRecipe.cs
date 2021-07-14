using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Kitchens.Azure
{
    public interface IArmRecipe
    {
        Task<ICookedIngredient> Execute(Docket docket, IArmRunner armRunner);
    }
}