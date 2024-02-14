﻿using NAPS2.Scan;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NAPS2.Scan.Images.Transforms;
using NAPS2.Barcode;

namespace NAPS2.Recovery
{
    [Serializable]
    [KnownType("KnownTypes")]
    public class RecoveryIndexImage
    {
        public string FileName { get; set; }

        public List<Transform> TransformList { get; set; }

        public ScanBitDepth BitDepth { get; set; }

        public BarcodeResult[] Barcodes { get; set; }

        public PatchCode PatchCode { get; set; }

        public bool HighQuality { get; set; }

        // ReSharper disable once UnusedMember.Local
        private static Type[] KnownTypes() => Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Transform))).ToArray();
    }
}