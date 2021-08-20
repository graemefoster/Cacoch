using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

                    if (aadApplication == null)
                    {
                        aadApplication = await CreateAadApplication(environment, docket, graphClient, name);
                    }

                    var servicePrincipal = await CreateServicePrincipal(graphClient, aadApplication!);

                    string? password = null;
                    if (!aadApplication.PasswordCredentials.Any())
                    {
                        password = await CreateSecret(graphClient, aadApplication);
                    }

                    await UpdateAadApplication(graphClient, aadApplication);

                    return new OAuthClientOutput(
                        Ingredient.TypedIngredientData!,
                        platformContext.TenantId,
                        aadApplication.AppId, 
                        password);
                });
        }

        private async Task UpdateAadApplication(GraphServiceClient graphClient, Application aadApplication)
        {
            await graphClient.Applications[aadApplication.Id].Request().UpdateAsync(new Application()
            {
                IdentifierUris = new [] { $"api://{aadApplication.Id}" },
                SignInAudience = "AzureADMyOrg",
                Web = new WebApplication()
                {
                    RedirectUris = Ingredient.TypedIngredientData!.RedirectUrls,
                    ImplicitGrantSettings = new ImplicitGrantSettings()
                    {
                        EnableIdTokenIssuance = true,
                        EnableAccessTokenIssuance = false
                    }
                }
            });
        }

        private static async Task<string> CreateSecret(GraphServiceClient graphClient, Application aadApplication)
        {
            return (await graphClient.Applications[aadApplication.Id].AddPassword(
                new PasswordCredential()
                {
                    DisplayName = "Cooker Client Secret",
                }).Request().PostAsync()).SecretText;
        }

        private async Task<Application> CreateAadApplication(PlatformEnvironment environment, Docket docket, GraphServiceClient graphClient, string? name)
        {
            return await graphClient.Applications.Request().AddAsync(new Application()
            {
                Id = name,
                DisplayName = Ingredient.TypedIngredientData!.DisplayName,
                Tags = new[] { docket.Id.ToString(), name, environment.ShortName }
            });
        }
        
        private async Task<ServicePrincipal> CreateServicePrincipal(GraphServiceClient graphServiceClient, Application application)
        {
            var sps = await graphServiceClient.ServicePrincipals.Request().Filter($"appId eq '{application.AppId}'")
                .GetAsync();
            
            var sp = sps.SingleOrDefault() ?? await graphServiceClient.ServicePrincipals.Request().AddAsync(new ServicePrincipal()
            {
                AppId = application.AppId
            });

            return sp;
        }

    }
}