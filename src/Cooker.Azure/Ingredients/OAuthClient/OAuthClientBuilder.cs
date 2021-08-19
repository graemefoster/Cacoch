using System.Collections.Generic;
using System.Linq;
using Cooker.Azure.KitchenStations.Sdk;
using Cooker.Ingredients;
using Cooker.Ingredients.OAuth2;
using Cooker.Kitchens;
using Microsoft.Graph;

namespace Cooker.Azure.Ingredients.OAuthClient
{
    public class OAuthClientBuilder : IRecipeBuilder<AzurePlatformContext>
    {
        public OAuthClientBuilder(OAuthClientIngredient ingredient)
        {
            Ingredient = ingredient;
        }

        private OAuthClientIngredient Ingredient { get; }

        public IRecipe CreateRecipe(
            AzurePlatformContext platformContext,
            PlatformEnvironment environment,
            Docket docket,
            IDictionary<IIngredient, ICookedIngredient> cooked)
        {
            return
                new AzureSdkRecipe<OAuthClientOutput>(async (ctx, armc) =>
                {
                    var graphClient = armc.GetAzureActiveDirectorySdk();

                    var name = $"{Ingredient.Id}-{environment.ShortName}-{ctx.Randomness}";
                    var aadApplications = await graphClient.Applications.Request()
                        .Filter($"tags/any(c:c eq '{docket.Id}')")
                        .GetAsync();

                    var aadApplication = aadApplications.SingleOrDefault(x =>
                        x.Tags.Contains(name) && x.Tags.Contains(environment.ShortName));

                    if (aadApplication != null)
                    {
                        return new OAuthClientOutput(Ingredient.TypedIngredientData!, aadApplication.AppId, null);
                    }

                    aadApplication = await graphClient.Applications.Request().AddAsync(new Application()
                    {
                        Id = name,
                        DisplayName = Ingredient.TypedIngredientData!.DisplayName,
                        Tags = new[] { docket.Id.ToString(), name, environment.ShortName }
                    });

                    var password = await graphClient.Applications[aadApplication.Id].AddPassword(
                        new PasswordCredential()
                        {
                            DisplayName = "Cooker Client Secret",
                        }).Request().PostAsync();

                    return new OAuthClientOutput(Ingredient.TypedIngredientData!, aadApplication.AppId,
                        password.SecretText);
                });
        }
    }
}