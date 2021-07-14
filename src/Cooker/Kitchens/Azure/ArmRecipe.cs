using System;
using System.Threading.Tasks;
using Cooker.Recipes;

namespace Cooker.Kitchens.Azure
{
    public class ArmRecipe<TOutput> : Recipe<TOutput>, IArmRecipe where TOutput: ILineItemOutput
    {
        private readonly ArmDefinition _arm;
        private readonly Func<object, TOutput> _outputBuilder;

        public ArmRecipe(
            ArmDefinition arm,
            Func<object, TOutput> outputBuilder)
        {
            _arm = arm;
            _outputBuilder = outputBuilder;
        }

        public ILineItemOutput Output(object armOutputs)
        {
            return _outputBuilder(armOutputs);
        }

        public async Task<ILineItemOutput> Execute(IArmRunner armRunner)
        {
            return _outputBuilder(await _arm.Execute(armRunner));
        }
    }
}