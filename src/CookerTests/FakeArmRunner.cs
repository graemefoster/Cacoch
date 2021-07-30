using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Azure;

namespace CookerTests
{
    internal class FakeArmRunner : IArmRunner
    {
        public Task<object> Execute(string resourceGroup, string friendlyName, string template, Dictionary<string, object> parameters)
        {
            return Task.FromResult(new object());
        }
    }
}