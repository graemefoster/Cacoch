namespace Cacoch.Provider.AzureArm.Resources.WebApp
{
    internal class WebAppOutput
    {
        public WebAppOutput(string hostName)
        {
            HostName = hostName;
        }

        public string HostName { get; }
    }
}