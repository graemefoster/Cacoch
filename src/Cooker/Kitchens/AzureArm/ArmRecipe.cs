using System;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public class ArmRecipe : Recipe
    {
        private readonly Func<object, ILineItemOutput> _outputBuilder;

        public ArmRecipe(ILineItem lineItem, Func<object, ILineItemOutput> outputBuilder)
        {
            _outputBuilder = outputBuilder;
        }

        public ILineItemOutput Output(object armOutputs)
        {
            return _outputBuilder(armOutputs);
        }
    }
}