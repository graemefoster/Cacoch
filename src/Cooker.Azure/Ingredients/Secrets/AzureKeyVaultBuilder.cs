using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Secrets
{
    public class AzureKeyVaultBuilder : IIngredientBuilder<Cooker.Ingredients.Secrets.Secrets>
    {
        public AzureKeyVaultBuilder(Cooker.Ingredients.Secrets.Secrets ingredient)
        {
            Ingredient = ingredient;
        }

        public Cooker.Ingredients.Secrets.Secrets Ingredient { get; }


        public IRecipe CreateRecipe(IPlatformContext platformContext,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            return
                new AzureSecretSdkRecipe<IntermediateOutput>(
                        client => new IntermediateOutput())
                    .Then(i => new ArmRecipe<SecretsOutput>(
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

    public class AzureSecretSdkRecipe<TOutput> : Recipe<TOutput>, ISecretRecipe where TOutput : ICookedIngredient
    {
        private readonly Func<SecretClient, TOutput> _action;

        public AzureSecretSdkRecipe(Func<SecretClient, TOutput> action)
        {
            _action = action;
        }

        public Task<ICookedIngredient> Execute(Docket docket, ISecretSdk sdk)
        {
            return sdk.Execute(_action);
        }
    }
}