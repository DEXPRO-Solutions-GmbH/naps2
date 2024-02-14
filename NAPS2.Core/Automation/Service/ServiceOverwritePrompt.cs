using NAPS2.Lang.ConsoleResources;
using NAPS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NAPS2.Automation.Service
{
    public class ServiceOverwritePrompt : IOverwritePrompt
    {
        public static bool ForceOverwrite { get; set; }

        private readonly IErrorOutput errorOutput;

        public ServiceOverwritePrompt(IErrorOutput errorOutput)
        {
            this.errorOutput = errorOutput;
        }

        public DialogResult ConfirmOverwrite(string path)
        {
            if (ForceOverwrite)
            {
                return DialogResult.Yes;
            }
            else
            {
                errorOutput.DisplayError(string.Format(ConsoleResources.FileAlreadyExists, path));
                return DialogResult.No;
            }
        }
    }
}
