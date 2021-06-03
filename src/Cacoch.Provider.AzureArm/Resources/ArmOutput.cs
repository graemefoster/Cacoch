﻿using System.Collections.Generic;
using Cacoch.Core.Provider;

namespace Cacoch.Provider.AzureArm.Resources
{
    /// <summary>
    /// Requests an output from another IPlatformTwin
    /// </summary>
    internal class ArmOutput
    {
        public IPlatformTwin Twin { get; }
        public string? TemplateName { get; }
        public string TemplateOutputName { get; }

        public ArmOutput(IPlatformTwin twin, string? templateName, string templateOutputName)
        {
            Twin = twin;
            TemplateName = templateName;
            TemplateOutputName = templateOutputName;
        }
        public ArmOutput(IPlatformTwin twin, string templateOutputName)
        {
            Twin = twin;
            TemplateOutputName = templateOutputName;
        }
    }

    internal class ArmOutputNameValueObjectArray
    {
        public Dictionary<string, ArmOutput> PropertySet = new Dictionary<string, ArmOutput>();
    }
    
}