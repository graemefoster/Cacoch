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
                        // new NoSqlData("grfnosql1", new[] { "container-1", "container-2" }),
                        new SecretsData(
                            "grfsecretone2",
                            new[] { "secret-one" },
                            new[]
                            {
                                new SecretsData.KnownSecret("cacoch-test-client-secret",
                                    "[cacoch-test-client.ClientSecret]")
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
                            },
                            new[]
                            {
                                new CookerLink("grfsecretone2", LinkAccess.Read),
                                new CookerLink("storageone", LinkAccess.ReadWrite),
                                // new CookerLink("grfnosql1", LinkAccess.ReadWrite),
                            }))
                    ;

                var meal = await restaurant.PlaceOrder(new PlatformEnvironment("dev", "Development"), docket);

                Console.WriteLine(JsonConvert.SerializeObject(meal, Formatting.Indented));

                // await builder!.Deploy(
                //     new Manifest(
                //         Guid.Parse("03872adb-06cd-42c2-95ef-ef67d590aee3"),
                //         "1.0",
                //         "cacochtest",
                //         "Cacoch Test",
                //         new List<CacochResourceMetadata>
                //         {
                //             new CacochSecretContainerResourceMetadata("secrets",
                //                 new HashSet<string>()
                //                 {
                //                     "secret-one",
                //                     "secret-two",
                //                     "secret-three",
                //                     "secret-four",
                //                     "secret-five",
                //                     "secret-six",
                //                     "secret-seven"
                //                 },
                //                 new Dictionary<string, string>(),
                //                 new List<CacochResourceLinkMetadata>
                //                 {
                //                     new SecretsLink("cacochapp", LinkAccess.Read)
                //                 }),
                //             new CacochStorageResourceMetadata(
                //                 "cacochstorage",
                //                 new[]
                //                 {
                //                     new CacochStorageResourceContainer(CacochStorageResourceContainerType.Storage,
                //                         "containerone"),
                //                     new CacochStorageResourceContainer(CacochStorageResourceContainerType.Storage,
                //                         "containertwo"),
                //                     new CacochStorageResourceContainer(CacochStorageResourceContainerType.Table,
                //                         "tableone"),
                //                     new CacochStorageResourceContainer(CacochStorageResourceContainerType.Queue,
                //                         "queueone")
                //                 }, new List<CacochResourceLinkMetadata>()
                //                 {
                //                     new StorageLink("cacochapp", LinkAccess.ReadWrite)
                //                 }),
                //             new CacochWebAppResourceMetadata("cacochapp", new Dictionary<string, string>()
                //             {
                //                 {"CONFIG_SETTING_ONE", "ONE"},
                //                 {"CONFIG_SETTING_TWO", "TWO"},
                //                 {"SECRET_REFERENCE", "[secret.secrets.secret-two]"},
                //                 {"AzureAD.ClientSecret", "[secret.secrets.cacochtestapp-ClientSecret]"},
                //             }),
                //             new CacochOAuthClientResourceMetadata("cacochtestapp", OAuthClientType.Web,
                //                 "http://localhost:9000/", new[] {"http://localhost:9000/sign-oidc"},
                //                 new[] {"cacochapp"}, new[] {""}, new List<OAuthRole>(), new List<OAuthScope>(),
                //                 new List<CacochResourceLinkMetadata>())
                //         }));
            }
        }
    }
}