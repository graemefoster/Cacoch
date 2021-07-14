using System;
using System.Threading.Tasks;
using Cooker.Ingredients;

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

        public async Task<ILineItemOutput> Execute(Docket docket, IArmRunner armRunner)
        {
            return _outputBuilder(await _arm.Execute(docket, armRunner));
        }
    }
}