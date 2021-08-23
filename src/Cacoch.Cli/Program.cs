using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker;
using Cooker.Azure;
using Cooker.Ingredients;
using Cooker.Ingredients.NoSql;
using Cooker.Ingredients.OAuth2;
using Cooker.Ingredients.Secrets;
using Cooker.Ingredients.Storage;
using Cooker.Ingredients.WebApp;
using Cooker.Kitchens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Cacoch.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hb, svc) => { svc.RegisterAzureCooker(hb.Configuration); })
                .ConfigureLogging(lb => lb.AddSerilog()).Build();

            await host.StartAsync();

            using (var _ = host.Services.CreateScope())
            {
                var restaurant = host.Services.GetRequiredService<IRestaurant>();
                var docket = new Docket(
                        new Guid("6e82545f-c7a6-4a3a-bf3d-cc34fd82e68b"),
                        "cacochtest",
                        new StorageData(
                            "storageone",
                            Array.Empty<string>(),
                            Array.Empty<string>(),
                            new[] { "my-container" }),
                        new NoSqlData("grfnosql1", new[] { "container-1", "container-2" }),
                        new SecretsData(
                            "grfsecretone2",
                            new[] { "secret-one" },
                            new[]
                            {
                                new SecretsData.KnownSecret("cacoch-test-client-secret",
                                    "[cacoch-test-client.ClientSecret]"),
                                new SecretsData.KnownSecret("grfnosql1-connectionstring",
                                    "[grfnosql1.ConnectionString]"),
                            }),
                        new OAuthClientData(
                            "cacoch-test-client",
                            "Cacoch Test OAuth Client",
                            new[]
                            {
                                "https://localhost:5001/signin-oidc", "https://[grfwebapp1.HostName]/signin-oidc"
                            }),
                        new WebAppData(
                            "grfwebapp1",
                            "Public",
                            new Dictionary<string, string>
                            {
                                ["setting1"] = "hello-world",
                                ["setting2"] = "@Microsoft.KeyVault(SecretUri=[grfsecretone2.SecretUrls.secret-one])",
                                ["AZUREAD__TENANTID"] = "[cacoch-test-client.Tenant]",
                                ["AZUREAD__CLIENTID"] = "[cacoch-test-client.Identity]",
                                ["AZUREAD__CLIENTSECRET"] =
                                    "@Microsoft.KeyVault(SecretUri=[grfsecretone2.SecretUrls.cacoch-test-client-secret])",
                                ["Settings__GRF_CONNECTIONSTRING"] =
                                    "@Microsoft.KeyVault(SecretUri=[grfsecretone2.SecretUrls.grfnosql1-connectionstring])",
                            },
                            new[]
                            {
                                new CookerLink("grfsecretone2", LinkAccess.Read),
                                new CookerLink("storageone", LinkAccess.ReadWrite),
                            }))
                    ;

                var meal = await restaurant.PlaceOrder(new PlatformEnvironment("dev", "Development"), docket);

                Console.WriteLine(JsonConvert.SerializeObject(meal, Formatting.Indented));
            }
        }
    }
}