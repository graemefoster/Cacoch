using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Cacoch.Core.Provider;
using Cacoch.Provider.AzureArm.Azure;

namespace Cacoch.Provider.AzureArm.Resources
{
    internal class AzureArmDeploymentArtifact : IDeploymentArtifact
    {
        internal string Name { get; }
        internal string Arm { get; }
        internal Dictionary<string, object> Parameters { get; }
        
        /// <summary>
        /// Outputs opf the ARM template that we want to make available in the PostProcess step.
        /// </summary>
        internal string[] ExposedOutputs { get; }

        public AzureArmDeploymentArtifact(
            string name,
            string arm,
            Dictionary<string, object> parameters,
            string[] exposedOutputs,
            Func<ArmDeploymentOutput, IDeploymentOutput> outputTransformer)
        {
            var hash = SHA512.Create().ComputeHash(Encoding.Default.GetBytes(arm));
            Hash = Convert.ToBase64String(hash);

            Name = name;
            Arm = arm;
            Parameters = parameters;
            ExposedOutputs = exposedOutputs;
            OutputTransformer = outputTransformer;
        }

        internal AzureArmDeploymentArtifact AddChildren(IEnumerable<AzureArmDeploymentArtifact> children)
        {
            ChildArtifacts.AddRange(children);
            foreach (var child in children)
            {
                child.Parent = this;
            }
            return this;
        }

        public AzureArmDeploymentArtifact? Parent { get; private set; }

        public string Hash { get; }

        public List<IDeploymentArtifact> ChildArtifacts { get; } = new();
        public Func<ArmDeploymentOutput, IDeploymentOutput> OutputTransformer { get; }

        public string NameForTemplate(ArmOutput armOutput)
        {
            if (armOutput.TemplateName != null) return armOutput.TemplateName;
            return Name;
        }
    }
}