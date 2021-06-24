using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Cacoch.Core.Manifest;
using Cacoch.Core.Manifest.Abstractions;
using Cacoch.Core.Manifest.OAuthClient;
using Cacoch.Core.Manifest.Secrets;
using Cacoch.Core.Manifest.Storage;
using Cacoch.Core.Manifest.WebApp;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Serilog;

namespace Cacoch.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tokenCredential = new DefaultAzureCredential();
            var token = await tokenCredential.GetTokenAsync(new TokenRequestContext(new[]
                {"https://management.core.windows.net/user_impersonation"}));
            var serviceClientCredentials = new TokenCredentials(token.Token);

            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hb, svc) =>
                {
                    svc.RegisterCacochAzureArm(serviceClientCredentials, tokenCredential,
                        hb.Configuration.GetSection("Cacoch"));
                })
                .ConfigureLogging(lb => lb.AddSerilog()).Build();

            await host.StartAsync();

            // using (var _ = host.Services.CreateScope())
            // {
            //     var builder = host.Services.GetService<IManifestDeployer>();
            //
            //     await builder!.Deploy(
            //         new Manifest(
            //             Guid.Parse("01010101-0101-0101-0101-010101010101"),
            //             "1.0",
            //             "cacochplatform",
            //             "Cacoch Platform",
            //             new List<CacochResourceMetadata>
            //             {
            //                 new CacochStorageResourceMetadata(
            //                     "cacochstorage",
            //                     Array.Empty<CacochStorageResourceContainer>(),
            //                     new List<CacochResourceLinkMetadata>())
            //             }));
            // }

            using (var _ = host.Services.CreateScope())
            {
                var builder = host.Services.GetService<IManifestDeployer>();

                await builder!.Deploy(
                    new Manifest(
                        Guid.Parse("03872adb-06cd-42c2-95ef-ef67d590aee3"),
                        "1.0",
                        "cacochtest",
                        "Cacoch Test",
                        new List<CacochResourceMetadata>
                        {
                            new CacochSecretContainerResourceMetadata("secrets",
                                new HashSet<string>()
                                {
                                    "secret-one",
                                    "secret-two",
                                    "secret-three",
                                    "secret-four",
                                    "secret-five",
                                    "secret-six",
                                    "secret-seven",
                                },
                                new List<CacochResourceLinkMetadata>
                                {
                                    new SecretsLink("cacochapp", LinkAccess.Read)
                                }),
                            new CacochStorageResourceMetadata(
                                "cacochstorage",
                                new[]
                                {
                                    new CacochStorageResourceContainer(CacochStorageResourceContainerType.Storage,
                                        "containerone"),
                                    new CacochStorageResourceContainer(CacochStorageResourceContainerType.Storage,
                                        "containertwo"),
                                    new CacochStorageResourceContainer(CacochStorageResourceContainerType.Table,
                                        "tableone"),
                                    new CacochStorageResourceContainer(CacochStorageResourceContainerType.Queue,
                                        "queueone")
                                }, new List<CacochResourceLinkMetadata>()
                                {
                                    new StorageLink("cacochapp", LinkAccess.ReadWrite)
                                }),
                            new CacochWebAppResourceMetadata("cacochapp", new Dictionary<string, string>()
                            {
                                {"CONFIG_SETTING_ONE", "ONE"},
                                {"CONFIG_SETTING_TWO", "TWO"},
                                {"SECRET_REFERENCE", "[secret.secrets.secret-two]"}
                            }),
                            new CacochOAuthClientResourceMetadata("cacochtestapp", OAuthClientType.Web,
                                "http://localhost:9000/", new[] {"http://localhost:9000/sign-oidc"},
                                new[] {"cacochapp"}, new[] {""}, new List<OAuthRole>(), new List<OAuthScope>(),
                                new List<CacochResourceLinkMetadata>())
                        }));
            }
        }
    }
}