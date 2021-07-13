using System;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    public class ArmRecipe : Recipe
    {
        private readonly Func<object, IEdible> _outputBuilder;

        public ArmRecipe(ILineItem lineItem, string createArmFor, Func<object, IEdible> outputBuilder): base(lineItem)
        {
            _outputBuilder = outputBuilder;
        }

        public IEdible Output(object armOutputs)
        {
            return _outputBuilder(armOutputs);
        }
    }
}