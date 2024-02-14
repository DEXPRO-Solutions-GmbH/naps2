using NAPS2.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.Barcode
{
    public class BarcodeSettingsContainer
    {
        private readonly IUserConfigManager userConfigManager;

        private BarcodeParams localBarcodeParams;

        public BarcodeSettingsContainer(IUserConfigManager userConfigManager)
        {
            this.userConfigManager = userConfigManager;
        }

        public BarcodeParams BarcodeParams
        {
            get => localBarcodeParams ?? userConfigManager.Config.BarcodeParams ?? new BarcodeParams();
            set => localBarcodeParams = value;
        }
    }
}
