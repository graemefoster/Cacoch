using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Cooker.Azure;
using Cooker.Ingredients;
using Cooker.Ingredients.Functions;
using Cooker.Ingredients.NoSql;
using Cooker.Ingredients.OAuth2;
using Cooker.Ingredients.Secrets;
using Cooker.Ingredients.Storage;
using Cooker.Ingredients.WebApp;
using Cooker.Kitchens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Cacoch.Cli
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.WriteLine(@"                    __            
  _________  ____  / /_____  _____
 / ___/ __ \/ __ \/ //_/ _ \/ ___/
/ /__/ /_/ / /_/ / ,< /  __/ /    
\___/\____/\____/_/|_|\___/_/     
");


            var root = new RootCommand();
            root.AddCommand(new DeployCommand());


            var cmdLineBuilder = new CommandLineBuilder(root)
                    .UseHost(
                        Host.CreateDefaultBuilder,
                        hostBuilder =>
                        {
                            hostBuilder
                                .ConfigureServices((hb, svc) =>
                                {
                                    svc.RegisterAzureCooker(hb.Configuration);
                                    svc.AddSingleton<IConsole, SystemConsole>();
                                })
                                .ConfigureLogging(lb => lb.AddSerilog())
                                .UseCommandHandler<DeployCommand, DeployCommand.DeployCommandHandler>();
                        })
                    .UseHelp()
                    .UseDefaults()
                ;

            var cmdLineParser = cmdLineBuilder.Build();
            return cmdLineParser.InvokeAsync(args);

            //
            // await host.StartAsync();
            //
            // using (var _ = host.Services.CreateScope())
            // {
            //     var restaurant = host.Services.GetRequiredService<IRestaurant>();
            //
            //     if (args.Length == 1)
            //     {
            //     }
            //
            //     var docket = BuildTestDocket();
            //
            //
            //     var meal = await restaurant.PlaceOrder(new PlatformEnvironment("dev", "Development"), docket);
            //     Console.WriteLine(JsonConvert.SerializeObject(meal, Formatting.Indented));
            // }
        }

        private static Docket? BuildTestDocket()
        {
            var docket = new Docket(
                    new Guid("6e82545f-c7a6-4a3a-bf3d-cc34fd82e68b"),
                    "cacochtest",
                    new StorageData(
                        "storageone",
                        Array.Empty<string>(),
                        Array.Empty<string>(),
                        new[] { "my-container" }),
                    new NoSqlData("grfnosql1",
                        new NoSqlData.NoSqlContainer[] { new("container-1", "/id"), new("container-2", "/id") }),
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
                    new FunctionData(
                        "grffunctions1",
                        "Public", new Dictionary<string, string>
                        {
                            ["AzureWebJobsStorage__accountName"] = "[storageone.StorageName]",
                            ["Settings__GRF_CONNECTIONSTRING"] =
                                "@Microsoft.KeyVault(SecretUri=[grfsecretone2.SecretUrls.grfnosql1-connectionstring])"
                        }, new[]
                        {
                            new CookerLink("grfsecretone2", LinkAccess.Read),
                            new CookerLink("storageone", LinkAccess.ReadWrite),
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
            return docket;
        }
    }
}