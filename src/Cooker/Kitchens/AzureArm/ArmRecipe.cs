using System;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public class ArmRecipe<TOutput> : Recipe<TOutput>, IArmRecipe where TOutput: ILineItemOutput
    {
        private readonly Func<object, TOutput> _outputBuilder;

        public ArmRecipe(ILineItem lineItem, Func<object, TOutput> outputBuilder)
        {
            _outputBuilder = outputBuilder;
        }

        public ILineItemOutput Output(object armOutputs)
        {
            return _outputBuilder(armOutputs);
        }
    }

    public interface IArmRecipe: ILineItemOutput
    {
        ILineItemOutput Output(object armOutputs);
    }
}