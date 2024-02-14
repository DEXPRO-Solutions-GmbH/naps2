using NAPS2.Dependencies;
using NAPS2.Lang.ConsoleResources;
using NAPS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.Automation.Service
{
    public class ServiceComponentInstallPrompt : IComponentInstallPrompt
    {

        private readonly IErrorOutput errorOutput;

        public ServiceComponentInstallPrompt(IErrorOutput errorOutput)
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
