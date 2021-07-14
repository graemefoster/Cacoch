using System.Threading.Tasks;
using Cooker.Ingredients;

namespace Cooker.Kitchens.Azure
{
    public interface IArmRecipe
    {
        Task<ILineItemOutput> Execute(Docket docket, IArmRunner armRunner);
    }
}