using System.Threading.Tasks;
using Cooker.Kitchens;

namespace Cooker.Azure
{
    class AzurePlatformContextBuilder : IPlatformContextBuilder<AzurePlatformContext>
    {
        public Task<AzurePlatformContext> Build(Docket docket, PlatformEnvironment platformEnvironment)
        {
            return Task.FromResult(new AzurePlatformContext(docket, platformEnvironment));
        }
    }
}