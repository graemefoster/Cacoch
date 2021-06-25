using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources.WebApp
{
    internal class WebAppOutput : IDeploymentOutput
    {
        public WebAppOutput(string hostName)
        {
            HostName = hostName;
        }

        public string HostName { get; }
    }
}