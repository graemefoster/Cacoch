using System;
using System.Threading.Tasks;
using Cooker;
using Cooker.Azure;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Cacoch.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hb, svc) =>
                {
                    svc.RegisterAzureCooker();
                    svc.Configure<AzureCookerSettings>(hb.Configuration.GetSection("Cacoch"));
                })
                .ConfigureLogging(lb => lb.AddSerilog()).Build();

            await host.StartAsync();

            using (var _ = host.Services.CreateScope())
            {
                var restaurant = host.Services.GetRequiredService<IRestaurant>();
                var docket = new Docket("cacochtest",
                    new Storage(
                        "storageone",
                        "Storage One",
                        Array.Empty<string>(),
                        Array.Empty<string>(),
                        new[] {"my-container"}),
                    new Storage(
                        "storagetwo",
                        "Storage Two",
                        Array.Empty<string>(),
                        Array.Empty<string>(),
                        new[] {"my-container-2"}));

                var meal = await restaurant.PlaceOrder(new PlatformEnvironment("dev", "Development"), docket);

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