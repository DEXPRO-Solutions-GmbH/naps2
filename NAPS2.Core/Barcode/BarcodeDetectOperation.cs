using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAPS2.ImportExport.Documents;
using NAPS2.ImportExport.Email;
using NAPS2.Lang.Resources;
using NAPS2.Logging;
using NAPS2.Ocr;
using NAPS2.Operation;
using NAPS2.Scan.Images;
using NAPS2.Util;

namespace NAPS2.Barcode
{
    public class BarcodeDetectOperation : OperationBase
    {
        private readonly IBarcodeDetector barcodeDetector;
        private readonly ScannedImageRenderer scannedImageRenderer;

        public BarcodeDetectOperation(IBarcodeDetector barcodeDetector, ScannedImageRenderer scannedImageRenderer)
        {
            this.barcodeDetector = barcodeDetector;
            this.scannedImageRenderer = scannedImageRenderer;

            AllowCancel = true;
            AllowBackground = false;
        }

        public bool Start(IEnumerable<ScannedImage> imageEnumerable, BarcodeParams bcParams = null)
        {
            var images = imageEnumerable.ToArray();
            ProgressTitle = MiscResources.DetectBarcodeProgress;
            Status = new OperationStatus
            {
                StatusText = MiscResources.DetectBarcodeMessage,
                MaxProgress = images.Length,
                CurrentProgress = 0
            };
            
            RunAsync(async () =>
            {
                bool result = false;
                try
                {
                    var i = 0;
                    foreach (var image in images)
                    {
                        if (CancelToken.IsCancellationRequested)
                        {
                            break;
                        }

                        using (var bitmap = await scannedImageRenderer.Render(image))
                        {
                            image.Barcodes = barcodeDetector.Detect(bitmap, bcParams).ToArray();
                        }
                        
                        OnProgress(++i, images.Length);
                    }
                    
                    result = true;
                }
                catch (Exception ex)
                {
                    Log.ErrorException(MiscResources.Error, ex);
                    InvokeError(MiscResources.Error, ex);
                    result = false;
                }
                finally
                {
                    GC.Collect();
                }

                return result;
            });
            return true;
        }
    }
}
