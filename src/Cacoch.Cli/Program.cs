using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Cacoch.Core.Manifest;
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
            var token = await tokenCredential.GetTokenAsync(new TokenRequestContext(new[] {"https://management.core.windows.net"}));
            var serviceClientCredentials = new TokenCredentials(token.Token);

            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hb, svc) =>
                {
                    svc.RegisterCacoch(typeof(Cacoch.Provider.AzureArm.Resources.Storage.StorageTwin).Assembly);
                    svc.RegisterCacochAzureArm(serviceClientCredentials, tokenCredential, hb.Configuration.GetSection("Cacoch"));
                })
                .ConfigureLogging(lb => lb.AddSerilog()).Build();
            
            await host.StartAsync();

            var builder = host.Services.GetService<IManifestDeployer>();
                
            await builder!.Deploy(
                new Manifest(
                    Guid.Parse("03872adb-06cd-42c2-95ef-ef67d590aee3"),
                    "1.0",
                    "cacochtest",
                    "Cacoch Test",
                    new List<IResource>
                    {
                        new Storage("cacochstorage", Array.Empty<string>()),
                        new WebApp("cacochapp")
                    }));
        }
    }
}