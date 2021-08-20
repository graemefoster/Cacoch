using System;
using System.Collections.Generic;
using Cooker.Azure.KitchenStations.Arm;
using Cooker.Ingredients;
using Cooker.Ingredients.Link;
using Cooker.Ingredients.Secrets;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Link
{
    // ReSharper disable once UnusedType.Global
    public class LinkBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        private string KeyVaultsSecretsUser = "4633458b-17de-408a-b874-0445c86b69e6";
        private string KeyVaultsSecretsOfficer = "b86a8fe4-44ce-4948-aee5-eccb2c155cd7";

        private string StorageBlobDataReader = "2a2b9908-6ea1-4ae2-8e65-a410df84e7d1";
        private string StorageQueueDataReader = "19e7f393-937e-4f77-808e-94535e297925";
        private string StorageTableDataReader = "76199698-9eea-4c19-bc75-cec21354c6b6";
        private string StorageBlobDataContributor = "ba92f5b4-2d11-453d-a403-e96b0029c9fe";
        private string StorageQueueDataContributor = "974c5e8b-45b9-4653-ba55-5f855dd0fb88";
        private string StorageTableDataContributor = "0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3";

        public LinkBuilder(LinkIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private LinkIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            PlatformEnvironment environment,
            Docket docket,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var linkName =
                $"{Ingredient.TypedIngredientData!.FromResource}-{Ingredient.TypedIngredientData.Access}-{Ingredient.TypedIngredientData.ToResource}-{platformContext.Randomness}";
            var template = typeof(LinkBuilder).GetResourceContents("Link");
            return new ArmRecipe<EmptyOutput>(
                new ArmDefinition(
                    $"link-{linkName}",
                    template,
                    new Dictionary<string, object>()
                    {
                        { "name", linkName },
                        { "scope", Ingredient.To!.PlatformId! },
                        { "requestorPrincipalId", ((IHaveRuntimeIdentity)Ingredient.From!).Identity },
                        { "roleIds", GetRoles(Ingredient.To!, Ingredient.TypedIngredientData.Access) }
                    }),
                output => new EmptyOutput());
        }

        private IEnumerable<string> GetRoles(ICookedIngredient ingredientTo, LinkAccess access)
        {
            if (ingredientTo is SecretsOutput)
            {
                if (access == LinkAccess.Read)
                {
                    yield return KeyVaultsSecretsUser;
                }
                else
                {
                    yield return KeyVaultsSecretsOfficer;
                }
            }

            if (ingredientTo is StorageOutput)
            {
                if (access == LinkAccess.Read)
                {
                    yield return StorageBlobDataReader;
                    yield return StorageQueueDataReader;
                    yield return StorageTableDataReader;
                }
                else
                {
                    yield return StorageBlobDataContributor;
                    yield return StorageQueueDataContributor;
                    yield return StorageTableDataContributor;
                }
            }
        }
    }
}