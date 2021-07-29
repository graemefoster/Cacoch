using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Secrets
{
    public class AzureKeyVaultBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public AzureKeyVaultBuilder(Cooker.Ingredients.Secrets.SecretsIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private Cooker.Ingredients.Secrets.SecretsIngredient Ingredient { get; }


        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            return
                new AzureSecretSdkRecipe<IntermediateOutput>(
                        (_, _) => new IntermediateOutput())
                    .Then(i =>
                        new ArmRecipe<SecretsOutput>(
                            new ArmDefinition(
                                "",
                                new Dictionary<string, object>()),
                            o => new SecretsOutput(Ingredient.DisplayName))
                    );
        }

        private class IntermediateOutput : ICookedIngredient
        {
        }
    }

    public class AzureSecretSdkRecipe<TOutput> : Recipe<AzurePlatformContext, TOutput>,
        ISecretRecipe<AzurePlatformContext>
        where TOutput : ICookedIngredient
    {
        private readonly Func<AzurePlatformContext, ResourcesManagementClient, TOutput> _action;

        public AzureSecretSdkRecipe(Func<AzurePlatformContext, ResourcesManagementClient, TOutput> action)
        {
            _action = action;
        }

        public Task<ICookedIngredient> Execute(AzurePlatformContext context, Docket docket, IAzureResourcesSdk sdk)
        {
            return sdk.Execute(context, _action);
        }
    }
}