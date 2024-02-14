using NAPS2.Scan.Images;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NAPS2.Barcode
{
    public class BarcodeProcessor
    {
        private readonly IBarcodeDetector barcodeDetector;

        private BarcodeResult[] currentBarcodes = null;

        public BarcodeProcessor(IBarcodeDetector barcodeDetector)
        {
            this.barcodeDetector = barcodeDetector;
        }

        public void NewBatch()
        {
            currentBarcodes = null;
        }

        public bool ProcessPage(ScannedImage image, BarcodeDetectionMode mode, Bitmap bitmap = null, BarcodeParams barcodeParams = null)
        {
            if (mode == BarcodeDetectionMode.None || image == null)
                return false;

            if (image.Barcodes == null && bitmap != null)
                image.Barcodes = barcodeDetector.Detect(bitmap, barcodeParams).ToArray();

            if (mode == BarcodeDetectionMode.Detect)
                return false;

            if (image.Barcodes?.Length > 0)
            {
                currentBarcodes = image.Barcodes;
                return true;
            }

            image.Barcodes = currentBarcodes;
            currentBarcodes = null;
            return false;
        }
    }

    public static class ScannedImageSourceBarcodeExtensions
    {
        public static void ProcessBarcodesAndPut(this ScannedImageSource.Concrete source, ScannedImage image, BarcodeDetectionMode mode, BarcodeProcessor processor)
        {
            if (!processor.ProcessPage(image, mode))
                source.Put(image);
            else
                image.Dispose();
        }
    }
}
