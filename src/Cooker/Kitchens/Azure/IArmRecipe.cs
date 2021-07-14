using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens.Azure
{
    public interface IArmRecipe
    {
        Task<ILineItemOutput> Execute(IArmRunner armRunner);
    }
}