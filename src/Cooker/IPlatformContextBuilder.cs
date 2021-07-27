using System.Threading.Tasks;
using Cooker.Kitchens;

namespace Cooker
{
    public interface IPlatformContextBuilder<TContext> where TContext: IPlatformContext
    {
        Task<TContext> Build(Docket docket);
    }
}