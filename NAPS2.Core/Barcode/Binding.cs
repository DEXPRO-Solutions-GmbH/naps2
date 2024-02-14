using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NAPS2.Scan.Images;

namespace NAPS2.Barcode
{
    /// <summary>
    /// Represents a continuous sequence of images. 
    /// </summary>
    public class Binding
    {
        public readonly List<ScannedImage> images;
        
        /// <summary>
        /// Index of the first image of this binding
        /// </summary>
        public readonly int firstImageIndex;
        
        /// <summary>
        /// The first barcode found in this binding
        /// </summary>
        public readonly BarcodeResult barcode;
        
        public Binding(List<ScannedImage> images, int firstImageIndex, BarcodeResult barcode = null)
        {
            this.images = images;
            this.barcode = barcode;

            if (firstImageIndex < 0)
            {
                throw new ArgumentException("First image index must be 0 or greater!");
            }
            
            this.firstImageIndex = firstImageIndex;
        }

        /// <returns>An enumerable range with the image indices of this bindings images</returns>
        public IEnumerable<int> Range()
        {
            return Enumerable.Range(firstImageIndex, images.Count);
        }
    }
}