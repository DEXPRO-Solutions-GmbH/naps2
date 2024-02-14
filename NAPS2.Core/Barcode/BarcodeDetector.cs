using NAPS2.Scan.Images;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZXing;

namespace NAPS2.Barcode
{
    public class ZXingBarcodeDetector: IBarcodeDetector
    {
        private readonly BarcodeSettingsContainer barcodeSettingsContainer;

        public ZXingBarcodeDetector(BarcodeSettingsContainer barcodeSettingsContainer)
        {
            this.barcodeSettingsContainer = barcodeSettingsContainer;
        }

        public IEnumerable<BarcodeResult> Detect(Bitmap bitmap, BarcodeParams opts = null)
        {
            opts = opts ?? barcodeSettingsContainer.BarcodeParams;
            Regex regex = null;
            if (!string.IsNullOrWhiteSpace(opts.RegexFilter))
                regex = new Regex(opts.RegexFilter);
            IBarcodeReader reader = new BarcodeReader();
            reader.Options.TryHarder = true;
            if (opts.AllowedFormatFilter?.Length > 0)
                reader.Options.PossibleFormats = opts.AllowedFormatFilter.Select(f => (ZXing.BarcodeFormat)f).ToArray();
            var scanResults = reader.DecodeMultiple(bitmap);
            if (scanResults != null)
            {
                foreach (var scanResult in scanResults)
                {
                    var barcodeFormat = (BarcodeFormat)scanResult.BarcodeFormat;
                    if (opts.ValueBlacklist?.Length > 0 && opts.ValueBlacklist.Contains(scanResult.Text))
                        continue;
                    if (opts.AllowedFormatFilter?.Length > 0 && !opts.AllowedFormatFilter.Contains(barcodeFormat))
                        continue;
                    if (opts.RegionFilter?.Length > 0 && !opts.RegionFilter.Any(region => scanResult.ResultPoints.All(point => region.Contains(point.X / bitmap.Width, point.Y / bitmap.Height))))
                        continue;
                    if (regex != null && !regex.IsMatch(scanResult.Text))
                        continue;
                    var barcodeResult = new BarcodeResult();
                    barcodeResult.BarcodeFormat = barcodeFormat;
                    barcodeResult.Text = scanResult.Text;
                    yield return barcodeResult;
                }
            }
        }
    }
}
