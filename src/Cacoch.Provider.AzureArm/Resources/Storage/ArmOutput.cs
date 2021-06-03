using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources.Storage
{
    /// <summary>
    /// Requests an output from another IPlatformTwin
    /// </summary>
    public class ArmOutput
    {
        public IPlatformTwin Twin { get; }
        public string Name { get; }

        public ArmOutput(IPlatformTwin twin, string name)
        {
            Twin = twin;
            Name = name;
        }
    }
}