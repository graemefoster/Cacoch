using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Ingredients;
using Cooker.Kitchens;

namespace Cooker.Azure.KitchenStations.AzureResourceManager
{
    public class ArmRecipe<TOutput> : Recipe<AzurePlatformContext, TOutput>, IArmRecipe where TOutput: ICookedIngredient
    {
        private readonly ArmDefinition _arm;
        private readonly Func<IDictionary<string, object>, TOutput> _outputBuilder;

        public ArmRecipe(
            ArmDefinition arm,
            Func<IDictionary<string, object>, TOutput> outputBuilder)
        {
            _arm = arm;
            _outputBuilder = outputBuilder;
        }

        public async Task<ICookedIngredient> Execute(AzurePlatformContext platformContext, Docket docket, IArmRunner armRunner)
        {
            return _outputBuilder(await _arm.Execute(platformContext, docket, armRunner));
        }
    }
}