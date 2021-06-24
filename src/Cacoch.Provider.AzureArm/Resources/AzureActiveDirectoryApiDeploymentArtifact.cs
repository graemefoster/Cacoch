using System;
using System.Threading.Tasks;
using Cacoch.Core.Provider;
using Microsoft.Graph;

namespace Cacoch.Provider.AzureArm.Resources
{
    internal class AzureActiveDirectoryApiDeploymentArtifact : IDeploymentArtifact
    {
        public string Name { get; }
        private readonly Func<GraphServiceClient, Task<IDeploymentOutput>> _actions;

        public AzureActiveDirectoryApiDeploymentArtifact(
            string name,
            Func<GraphServiceClient, Task<IDeploymentOutput>> actions)
        {
            Name = name;
            _actions = actions;
        }

        public Task<IDeploymentOutput> Deploy(GraphServiceClient client)
        {
            return _actions(client);
        }
    }
}