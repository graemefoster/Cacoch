using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Kitchens.Azure;

namespace CookerTests
{
    internal class FakeArmRunner : IArmRunner
    {
        public Task<object> Execute(string resourceGroup, string template, Dictionary<string, object> parameters)
        {
            return Task.FromResult(new object());
        }
    }
}