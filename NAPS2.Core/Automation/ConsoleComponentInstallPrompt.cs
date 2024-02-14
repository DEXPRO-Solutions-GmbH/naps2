using System;
using System.Collections.Generic;
using System.Linq;
using NAPS2.Dependencies;
using NAPS2.Lang.ConsoleResources;
using NAPS2.Util;

namespace NAPS2.Automation
{
    public class ConsoleComponentInstallPrompt : IComponentInstallPrompt
    {

        private readonly IErrorOutput errorOutput;

        public ConsoleComponentInstallPrompt(IErrorOutput errorOutput)
        {
            this.errorOutput = errorOutput;
        }

        public bool PromptToInstall(ExternalComponent component, string promptText)
        {
            errorOutput.DisplayError(string.Format(ConsoleResources.ComponentNeeded, component.Id));
            return false;
        }
    }
}
