using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooker.Azure;
using Cooker.Azure.KitchenStations.Arm;

namespace CookerTests
{
    internal class FakeArmRunner : IArmRunner
    {
        private readonly Stack<IDictionary<string, object>> _stack = new Stack<IDictionary<string, object>>();

        public void Seed(object obj)
        {
            _stack.Push(
                obj.GetType().GetProperties().ToDictionary(
                    x => x.Name,
                    x => x.GetValue(obj)!));
        }

        public Task<IDictionary<string, object>> Execute(
            string resourceGroup,
            string friendlyName,
            string template,
            Dictionary<string, object> parameters)
        {
            return Task.FromResult(_stack.Pop());
        }
    }
}