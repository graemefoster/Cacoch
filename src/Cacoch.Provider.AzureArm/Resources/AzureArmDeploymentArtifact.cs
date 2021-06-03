using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Cacoch.Core.Manifest;
using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources
{
    internal class AzureArmDeploymentArtifact : IDeploymentArtifact
    {
        internal string Name { get; }
        internal string Arm { get; }
        internal Dictionary<string, object> Parameters { get; }

        public AzureArmDeploymentArtifact(
            string name,
            string arm,
            Dictionary<string, object> parameters,
            IEnumerable<IPlatformTwin> dependsOn,
            IEnumerable<AzureArmDeploymentArtifact> childArtifacts)
        {
            var hash = SHA512.Create().ComputeHash(Encoding.Default.GetBytes(arm));
            HashString = Convert.ToBase64String(hash);
            Name = name;
            Arm = arm;
            Parameters = parameters;
            ChildArtifacts = childArtifacts;
        }

        public string HashString { get; }

        public IEnumerable<IDeploymentArtifact> ChildArtifacts { get; }

        public string NameForTemplate(ArmOutput armOutput)
        {
            if (armOutput.TemplateName != null) return armOutput.TemplateName;
            return Name;
        }
    }
}