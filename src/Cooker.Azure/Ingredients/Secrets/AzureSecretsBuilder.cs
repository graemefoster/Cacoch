using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Security.KeyVault.Secrets;
using Cooker.Azure.KitchenStations.AzureResourceManager;
using Cooker.Azure.KitchenStations.Sdk;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Secrets
{
    public class AzureSecretsBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public AzureSecretsBuilder(SecretsIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private SecretsIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            PlatformEnvironment environment,
            Docket docket,
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
                    {
                        var allSecrets =
                            Ingredient.TypedIngredientData!.Secrets.Select(
                                    x => new SecretsData.KnownSecret(x, $"{Guid.NewGuid()}-{Guid.NewGuid()}"))
                                .Union(Ingredient.TypedIngredientData!.KnownSecrets ??
                                       Array.Empty<SecretsData.KnownSecret>())
                                .ToArray();

                        var secretsToCreate = allSecrets
                            .Where(x => !i.ExistingSecrets.Contains(x.Name))
                            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                            .ToArray();

                        return new ArmRecipe<SecretsOutput>(
                            new ArmDefinition(
                                $"secrets-{Ingredient.Id}",
                                typeof(AzureSecretsBuilder).GetResourceContents("Secrets"),
                                new Dictionary<string, object>()
                                {
                                    {
                                        "secrets", new
                                        {
                                            array = secretsToCreate
                                                .Select(x => new { name = x.Name, value = x.Value })
                                                .ToArray()
                                        }
                                    },
                                    { "vaultName", vaultName },
                                    { "secretsOfficerPrincipalId", platformContext.DeploymentPrincipalId }
                                }),
                            o =>
                            {
                                var vaultUrl = (string)o["vaultUrl"];
                                return new SecretsOutput(
                                    Ingredient.TypedIngredientData,
                                    (string)o["resourceId"],
                                    allSecrets.ToDictionary(x => x.Name, x => $"{vaultUrl}secrets/{x.Name}"));
                            });
                    });
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