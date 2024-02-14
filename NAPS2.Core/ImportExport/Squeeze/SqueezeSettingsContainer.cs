using NAPS2.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAPS2.ImportExport.Squeeze
{
    public class SqueezeSettingsContainer
    {
        private readonly IUserConfigManager userConfigManager;

        private SqueezeSettings localSqueezeSettings;

        public SqueezeSettingsContainer(IUserConfigManager userConfigManager)
        {
            this.userConfigManager = userConfigManager;
        }

        public SqueezeSettings SqueezeSettings
        {
            get => localSqueezeSettings ?? userConfigManager.Config.SqueezeSettings ?? new SqueezeSettings();
            set => localSqueezeSettings = value;
        }
    }
}
