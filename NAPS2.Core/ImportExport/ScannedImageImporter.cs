using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NAPS2.Barcode;
using NAPS2.ImportExport.Images;
using NAPS2.ImportExport.Pdf;
using NAPS2.Scan.Images;
using NAPS2.Util;

namespace NAPS2.ImportExport
{
    public class ScannedImageImporter : IScannedImageImporter
    {
        private readonly IScannedImageImporter pdfImporter;
        private readonly IScannedImageImporter imageImporter;

        public ScannedImageImporter(IPdfImporter pdfImporter, IImageImporter imageImporter)
        {
            this.pdfImporter = pdfImporter;
            this.imageImporter = imageImporter;
        }

        public ScannedImageSource Import(string filePath, ImportParams importParams, ProgressHandler progressCallback, CancellationToken cancelToken)
        {
            ScannedImageSource source;
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            switch (Path.GetExtension(filePath).ToLowerInvariant())
            {
                case ".pdf":
                    source = pdfImporter.Import(filePath, importParams, progressCallback, cancelToken);
                    break;
                default:
                    source = imageImporter.Import(filePath, importParams, progressCallback, cancelToken);
                    break;
            }
            return source;
        }
    }
}
