﻿using System;
using System.Collections.Generic;
using System.Linq;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using Azure.Security.KeyVault.Secrets;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;
using Cooker.Kitchens;
using Microsoft.Extensions.Azure;

namespace Cooker.Azure.Ingredients.Secrets
{
    public class AzureKeyVaultBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public AzureKeyVaultBuilder(SecretsIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private SecretsIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var vaultName = (Ingredient.Id + platformContext.Randomness).ToLowerInvariant();

            return
                new AzureSdkRecipe<ExistingSecretsOutput>(async (ctx, armc) =>
                    {
                        await foreach (var vault in armc.GetKeyVaultManagementClient().Vaults
                            .ListByResourceGroupAsync(ctx.ResourceGroupName)
                            .AsPages())
                        {
                            var kv = vault.Values.SingleOrDefault(x => x.Name == vaultName);
                            if (kv != null)
                            {
                                var client = new SecretClient(new Uri(kv.Properties.VaultUri), armc.Credential);
                                var secrets = new List<string>();
                                await foreach (var page in client.GetPropertiesOfSecretsAsync().AsPages())
                                {
                                    secrets.AddRange(page.Values.Select(x => x.Name));
                                }

                                return new ExistingSecretsOutput(secrets.ToArray());
                            }
                        }

                        return new ExistingSecretsOutput(Array.Empty<string>());
                    })
                    .Then(i =>
                        new ArmRecipe<SecretsOutput>(
                            new ArmDefinition(
                                $"secrets-{Ingredient.Id}",
                                typeof(AzureKeyVaultBuilder).GetResourceContents("Secrets"),
                                new Dictionary<string, object>()
                                {
                                    {
                                        "secrets", new
                                        {
                                            array = Ingredient.RequiredSecrets
                                                .Except(i.ExistingSecrets)
                                                .Select(x => new
                                                    {name = x, value = $"{Guid.NewGuid()}-{Guid.NewGuid()}"})
                                                .ToArray()
                                        }
                                    },
                                    {"vaultName", vaultName},
                                    {"secretsOfficerPrincipalId", platformContext.DeploymentPrincipalId }
                                }),
                            o => new SecretsOutput(Ingredient.DisplayName))
                    );
        }

        public class ExistingSecretsOutput : ICookedIngredient
        {
            public string[] ExistingSecrets { get; }

            public ExistingSecretsOutput(string[] existingSecrets)
            {
                ExistingSecrets = existingSecrets;
            }
        }
    }
}