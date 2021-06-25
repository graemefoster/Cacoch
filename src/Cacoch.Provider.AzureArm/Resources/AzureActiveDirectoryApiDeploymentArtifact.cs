using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cacoch.Core.Provider;
using Microsoft.Graph;

namespace Cacoch.Provider.AzureArm.Resources
{
    internal class AzureActiveDirectoryApiDeploymentArtifact : IDeploymentArtifact
    {
        public string Name { get; }
        private readonly Func<GraphServiceClient, Task<IDeploymentOutput>> _actions;
        private readonly Func<GraphServiceClient, IDictionary<string, IDeploymentOutput>, Task> _postDeployActions;

        public AzureActiveDirectoryApiDeploymentArtifact(
            string name,
            Func<GraphServiceClient, Task<IDeploymentOutput>> actions,
            Func<GraphServiceClient, IDictionary<string, IDeploymentOutput>, Task> postDeployActions)
        {
            Name = name;
            _actions = actions;
            _postDeployActions = postDeployActions;
        }

        public Task<IDeploymentOutput> Deploy(GraphServiceClient client)
        {
            return _actions(client);
        }

        public Task PostDeploy(GraphServiceClient client, IDictionary<string, IDeploymentOutput> allTwins)
        {
            return _postDeployActions(client, allTwins);
        }
    }
}