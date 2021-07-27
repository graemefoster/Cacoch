using System.Threading.Tasks;
using Cooker.Kitchens;

namespace Cooker
{
    public interface IPlatformContextBuilder
    {
        Task<IPlatformContext> Build(Docket docket);
    }
}