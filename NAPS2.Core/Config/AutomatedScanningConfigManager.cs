using NAPS2.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.Config
{
    public class AutomatedScanningConfigManager : ConfigManager<AutomatedScanningOptions>
    {
        public AutomatedScanningConfigManager() : base("automation.xml", Paths.Executable, null, () => new AutomatedScanningOptions())
        { }

        public AutomatedScanningOptions Options
        {
            get => Config;
            set => Config = value;
        }
    }
}
