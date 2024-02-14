using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NAPS2.Barcode
{
    public class BarcodeParams
    {
        public string RegexFilter { get; set; }

        public BarcodeFormat[] AllowedFormatFilter { get; set; }

        public RectangleF[] RegionFilter { get; set; }

        public string[] ValueBlacklist { get; set; }
    }
}
