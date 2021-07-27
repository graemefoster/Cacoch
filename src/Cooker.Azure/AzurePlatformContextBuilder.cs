using System.Threading.Tasks;
using Cooker.Kitchens;

namespace Cooker.Azure
{
    class AzurePlatformContextBuilder : IPlatformContextBuilder
    {
        public Task<IPlatformContext> Build(Docket docket)
        {
            return Task.FromResult((IPlatformContext) new AzurePlatformContext());
        }
    }
}