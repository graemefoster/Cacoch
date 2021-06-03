using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Cacoch.Core.Manifest;
using Cacoch.Core.Manifest.Abstractions;
using Cacoch.Core.Manifest.Storage;
using Cacoch.Core.Manifest.WebApp;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Rest;
using Serilog;

namespace Cacoch.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tokenCredential = new DefaultAzureCredential();
            var token = await tokenCredential.GetTokenAsync(new TokenRequestContext(new[]
                {"https://management.core.windows.net"}));
            var serviceClientCredentials = new TokenCredentials(token.Token);

            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hb, svc) =>
                {
                    svc.RegisterCacochAzureArm(serviceClientCredentials, tokenCredential,
                        hb.Configuration.GetSection("Cacoch"));
                })
                .ConfigureLogging(lb => lb.AddSerilog()).Build();

            await host.StartAsync();

            // using (var _ = host.Services.CreateScope()) {
            //     var builder = host.Services.GetService<IManifestDeployer>();
            //
            //     await builder!.Deploy(
            //         new Manifest(
            //             Guid.Parse("01010101-0101-0101-0101-010101010101"),
            //             "1.0",
            //             "cacochplatform",
            //             "Cacoch Platform",
            //             new List<IResource>
            //             {
            //                 new Storage("cacochstorage", Array.Empty<string>())
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
                            new Storage(
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
                            new WebApp("cacochapp", new Dictionary<string, string>()
                            {
                                {"CONFIG_SETTING_ONE", "ONE"},
                                {"CONFIG_SETTING_TWO", "TWO"},
                            })
                        }));
            }
        }
    }
}