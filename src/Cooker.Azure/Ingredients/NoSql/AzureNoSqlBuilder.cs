using System;
using System.Collections.Generic;
using Cooker.Azure.Ingredients.Storage;
using Cooker.Azure.KitchenStations.AzureResourceManager;
using Cooker.Ingredients;
using Cooker.Ingredients.NoSql;
using Cooker.Kitchens;

namespace Cooker.Azure.Ingredients.NoSql
{
    public class AzureNoSqlBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public AzureNoSqlBuilder(NoSqlIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private NoSqlIngredient Ingredient { get; }


        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            PlatformEnvironment environment,
            Docket docket,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            var name = (Ingredient.Id + platformContext.Randomness).ToLowerInvariant();
            var armTemplate = typeof(AzureNoSqlBuilder).GetResourceContents("NoSql");
            var secondaryRegion = GetSecondaryRegion(platformContext.PrimaryLocation, out var newPrimaryRegion);

            var parameters = new Dictionary<string, object>
            {
                ["name"] = name,
                ["databaseName"] = Ingredient.TypedIngredientData!.Id,
                ["primaryRegion"] = newPrimaryRegion,
                ["secondaryRegion"] = secondaryRegion,
                ["containers"] = Ingredient.TypedIngredientData!.Containers,
            };

            return new ArmRecipe<NoSqlOutput>(
                new ArmDefinition(
                    $"nosql-{Ingredient.Id}",
                    armTemplate,
                    parameters),
                output => new NoSqlOutput(
                    Ingredient.TypedIngredientData,
                    (string)output["resourceId"],
                    (string)output["connectionString"]
                    )
            );
        }

        private string GetSecondaryRegion(string preferredRegion, out string primaryRegion)
        {
            switch (preferredRegion)
            {
                case "australiaeast":
                {
                    primaryRegion = "australiasoutheast"; //capacity issues in australia east in my subscription!!
                    return "australiaeast";
                }

                default: throw new NotSupportedException($"Need to define a secondary region for {preferredRegion}");
            }
        }
    }
}