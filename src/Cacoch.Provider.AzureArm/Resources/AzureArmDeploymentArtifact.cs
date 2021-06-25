using System;
using System.Collections.Generic;
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
            IEnumerable<AzureArmDeploymentArtifact> childArtifacts,
            Func<ArmDeploymentOutput, IDeploymentOutput> outputTransformer)
        {
            Name = name;
            Arm = arm;
            Parameters = parameters;
            ExposedOutputs = exposedOutputs;
            ChildArtifacts = childArtifacts;
            OutputTransformer = outputTransformer;
        }

        public IEnumerable<IDeploymentArtifact> ChildArtifacts { get; }
        public Func<ArmDeploymentOutput, IDeploymentOutput> OutputTransformer { get; }

        public string NameForTemplate(ArmOutput armOutput)
        {
            if (armOutput.TemplateName != null) return armOutput.TemplateName;
            return Name;
        }
    }
}