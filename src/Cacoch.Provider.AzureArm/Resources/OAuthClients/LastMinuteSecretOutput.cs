using System.Collections.Generic;
using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources.OAuthClients
{
    public class LastMinuteSecretOutput : IDeploymentOutput
    {
        private readonly string _name;
        private readonly string _value;

        public LastMinuteSecretOutput(string name, string value)
        {
            _name = name;
            _value = value;
        }

        internal void AddTo(Dictionary<string, string> resourceKnownSecrets)
        {
            resourceKnownSecrets.Add(_name, _value);
        }
    }
}