using System;
using Cooker.Recipes;

namespace Cooker.Kitchens.Azure
{
    public class ArmRecipe<TOutput> : Recipe<TOutput>, IArmRecipe where TOutput: ILineItemOutput
    {
        private readonly Func<object, TOutput> _outputBuilder;

        public ArmRecipe(Func<object, TOutput> outputBuilder)
        {
            _outputBuilder = outputBuilder;
        }

        public ILineItemOutput Output(object armOutputs)
        {
            return _outputBuilder(armOutputs);
        }
    }
}