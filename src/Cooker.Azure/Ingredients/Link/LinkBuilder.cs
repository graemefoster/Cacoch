using System;
using System.Collections.Generic;
using Cooker.Azure.KitchenStations.Arm;
using Cooker.Ingredients;
using Cooker.Ingredients.Link;
using Cooker.Ingredients.Secrets;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.Link
{
    // ReSharper disable once UnusedType.Global
    public class LinkBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        private string KeyVaultsSecretsUser = "4633458b-17de-408a-b874-0445c86b69e6";

        public LinkBuilder(LinkIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private LinkIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var linkName = $"{Ingredient.TypedIngredientData!.FromResource}-{Ingredient.TypedIngredientData.Access}-{Ingredient.TypedIngredientData.ToResource}-{platformContext.Randomness}";
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
                        { "roleId", GetRole(Ingredient.To!, Ingredient.TypedIngredientData.Access) }
                    }),
                output => new EmptyOutput());
        }

        private string GetRole(ICookedIngredient ingredientTo, LinkAccess access)
        {
            if (ingredientTo is SecretsOutput)
            {
                if (access == LinkAccess.Read)
                {
                    return KeyVaultsSecretsUser;
                }
            }

            throw new NotSupportedException();
        }
    }
}