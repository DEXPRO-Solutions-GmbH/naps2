using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NAPS2.Config
{
    public class AppConfigManager : ConfigManager<AppConfig>
    {
        private const string SEND_REG_KEY = @"SOFTWARE\NAPS2\Send";
        private const string SEND_REG_KEY_64 = @"SOFTWARE\WOW6432Node\NAPS2\Send";
        private const string SEND_TARGET_REG_NAME = "Target";
        private const string SEND_ARGS_REG_NAME = "Args";

        public AppConfigManager()
            : base("appsettings.xml", Paths.Executable, null, () => new AppConfig { Version = AppConfig.CURRENT_VERSION })
        {
        }

        public override void Load()
        {
            base.Load();
            if(string.IsNullOrWhiteSpace(Config.SendTarget))
            {
                using (var key86 = Registry.LocalMachine.OpenSubKey(SEND_REG_KEY))
                using (var key64 = Registry.LocalMachine.OpenSubKey(SEND_REG_KEY_64))
                {
                    var key = key86 ?? key64;
                    if (key != null)
                    {
                        var target = key.GetValue(SEND_TARGET_REG_NAME) as string;
                        var args = key.GetValue(SEND_ARGS_REG_NAME) as string;
                        Config.SendTarget = target ?? Config.SendTarget;
                        Config.SendArguments = args ?? Config.SendArguments;
                    }
                }
            }
        }

        public new AppConfig Config => base.Config;
    }
}
