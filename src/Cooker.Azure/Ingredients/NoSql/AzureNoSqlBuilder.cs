using System;
using System.Collections.Generic;
using Cooker.Azure.Ingredients.Storage;
using Cooker.Azure.KitchenStations.Arm;
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

            var parameters = new Dictionary<string, object>
            {
                ["name"] = name,
                ["primaryRegion"] = platformContext.PrimaryLocation,
                ["secondaryRegion"] = GetSecondaryRegion(platformContext.PrimaryLocation),
                ["databaseName"] = name,
                ["containers"] = Ingredient.TypedIngredientData!.Containers,
            };

            return new ArmRecipe<NoSqlOutput>(
                new ArmDefinition(
                    $"nosql-{Ingredient.Id}",
                    armTemplate,
                    parameters),
                output => new NoSqlOutput(
                    Ingredient.TypedIngredientData,
                    (string)output["resourceId"])
            );
        }

        private string GetSecondaryRegion(string region)
        {
            switch (region)
            {
                case "australiaeast": return "australiasoutheast";
                default: throw new NotSupportedException($"Need to define a secondary region for {region}");
            }
        }
    }
}