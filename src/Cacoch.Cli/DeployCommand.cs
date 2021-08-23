using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading.Tasks;
using Cooker;
using Cooker.Ingredients;
using Cooker.Kitchens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cacoch.Cli
{
    internal class DeployCommand : Command
    {
        public DeployCommand() : base("deploy")
        {
            AddOption(new Option<string>($"--{nameof(DeployCommandHandler.ManifestFile).ToKebabCase()}"));
        }

        internal class DeployCommandHandler : ICommandHandler
        {
            private readonly IRestaurant _restaurant;
            public string? ManifestFile { get; set; }

            public DeployCommandHandler(IRestaurant restaurant)
            {
                _restaurant = restaurant;
            }

            public async Task<int> InvokeAsync(InvocationContext context)
            {
                var deserializer = new JsonSerializer()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    SerializationBinder = new CookerSerializationBinder(),
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                deserializer.Converters.Add(new StringEnumConverter());

                using var reader = new JsonTextReader(File.OpenText(ManifestFile!));
                var docket = deserializer.Deserialize<Docket>(reader);
                await _restaurant.PlaceOrder(new PlatformEnvironment("dev", "Development"), docket!);
                return 0;
            }
        }
    }
}