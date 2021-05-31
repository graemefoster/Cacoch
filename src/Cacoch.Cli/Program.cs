using System;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cacoch.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (var host = new HostBuilder()
                .ConfigureServices(svc =>
                {
                    svc.RegisterCacoch(typeof(Cacoch.Provider.AzureArm.Resources.Storage.StorageTwin).Assembly);
                }).Build())
            {
                await host.StartAsync();
                var builder = host.Services.GetService<IManifestDeployer>();
                await builder!.Deploy(new Manifest()
                {
                    Resources = new [] { new Storage("testy")}
                });
            }
            
        }
    }
}