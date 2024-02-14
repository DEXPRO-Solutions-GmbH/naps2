using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.Barcode
{
    public enum BarcodeDetectionMode : int
    {
        None = 0,
        Detect = 1,
        DetectAndRemovePage = 2
    }

    public static class BarcodeDetectionModeExtensions
    {
        public static BarcodeDetectionMode SelectHighest(this BarcodeDetectionMode mode, params BarcodeDetectionMode[] otherModes)
        {
            return (BarcodeDetectionMode)Math.Max((int)mode, otherModes.Max(m => (int)m));
        }
    }
}
