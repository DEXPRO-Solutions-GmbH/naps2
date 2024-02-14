using System;
using System.Collections.Generic;
using System.Linq;

namespace NAPS2.WinForms
{
    partial class FDesktop
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FDesktop));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomMouseCatcher = new System.Windows.Forms.Button();
            this.thumbnailList1 = new NAPS2.WinForms.ThumbnailList();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxView = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tStrip = new System.Windows.Forms.ToolStrip();
            this.tsScan = new System.Windows.Forms.ToolStripSplitButton();
            this.tsNewProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsBatchScan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsdSelectBatch = new System.Windows.Forms.ToolStripSplitButton();
            this.tsDetectBarcodes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdUploadDocuments = new System.Windows.Forms.ToolStripSplitButton();
            this.tsUploadDocumentsAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsUploadDocumentsSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdUploadSqueeze = new System.Windows.Forms.ToolStripSplitButton();
            this.tsUploadSqueezeAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsUploadSqueezeSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsUploadSqueezeBindings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdSavePDF = new System.Windows.Forms.ToolStripSplitButton();
            this.tsSavePDFAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSavePDFSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdSaveImages = new System.Windows.Forms.ToolStripSplitButton();
            this.tsSaveImagesAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSaveImagesSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdEmailPDF = new System.Windows.Forms.ToolStripSplitButton();
            this.tsEmailPDFAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEmailPDFSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPrint = new System.Windows.Forms.ToolStripButton();
            this.tsdSend = new System.Windows.Forms.ToolStripSplitButton();
            this.tsSendAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSendSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsdImage = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsCrop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsBrightnessContrast = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHueSaturation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsBlackWhite = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSharpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsReset = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdRotate = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsRotateLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.tsRotateRight = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.tsDeskew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsCustomRotation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMove = new NAPS2.WinForms.ToolStripDoubleButton();
            this.tsdReorder = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsInterleave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsDeinterleave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.tsAltInterleave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAltDeinterleave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsReverse = new System.Windows.Forms.ToolStripMenuItem();
            this.tsReverseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsReverseSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsDelete = new System.Windows.Forms.ToolStripButton();
            this.tsClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsdSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsProfiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tsBarcodeSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsOcr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tsPDFSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsImageSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsDocumentsSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSqueezeSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEmailSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.tsAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.tStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.BottomToolStripPanel, "toolStripContainer1.BottomToolStripPanel");
            this.toolStripContainer1.BottomToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.toolStripContainer1.BottomToolStripPanel.ForeColor = System.Drawing.Color.White;
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            this.toolStripContainer1.ContentPanel.Controls.Add(this.btnZoomIn);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.btnZoomOut);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.btnZoomMouseCatcher);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.thumbnailList1);
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.LeftToolStripPanel, "toolStripContainer1.LeftToolStripPanel");
            this.toolStripContainer1.LeftToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.toolStripContainer1.LeftToolStripPanel.ForeColor = System.Drawing.Color.White;
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.RightToolStripPanel, "toolStripContainer1.RightToolStripPanel");
            this.toolStripContainer1.RightToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.toolStripContainer1.RightToolStripPanel.ForeColor = System.Drawing.Color.White;
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.TopToolStripPanel, "toolStripContainer1.TopToolStripPanel");
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tStrip);
            this.toolStripContainer1.TopToolStripPanel.ForeColor = System.Drawing.Color.White;
            // 
            // btnZoomIn
            // 
            resources.ApplyResources(this.btnZoomIn, "btnZoomIn");
            this.btnZoomIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.btnZoomIn.FlatAppearance.BorderSize = 0;
            this.btnZoomIn.Image = global::NAPS2.Icons.zoom_in_white;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.UseVisualStyleBackColor = false;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            resources.ApplyResources(this.btnZoomOut, "btnZoomOut");
            this.btnZoomOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.btnZoomOut.FlatAppearance.BorderSize = 0;
            this.btnZoomOut.Image = global::NAPS2.Icons.zoom_out_white;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.UseVisualStyleBackColor = false;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnZoomMouseCatcher
            // 
            resources.ApplyResources(this.btnZoomMouseCatcher, "btnZoomMouseCatcher");
            this.btnZoomMouseCatcher.BackColor = System.Drawing.Color.White;
            this.btnZoomMouseCatcher.Name = "btnZoomMouseCatcher";
            this.btnZoomMouseCatcher.UseVisualStyleBackColor = false;
            // 
            // thumbnailList1
            // 
            resources.ApplyResources(this.thumbnailList1, "thumbnailList1");
            this.thumbnailList1.AllowDrop = true;
            this.thumbnailList1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.thumbnailList1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.thumbnailList1.ContextMenuStrip = this.contextMenuStrip;
            this.thumbnailList1.ForeColor = System.Drawing.Color.White;
            this.thumbnailList1.HideSelection = false;
            this.thumbnailList1.Name = "thumbnailList1";
            this.thumbnailList1.ThumbnailRenderer = null;
            this.thumbnailList1.ThumbnailSize = new System.Drawing.Size(128, 128);
            this.thumbnailList1.UseCompatibleStateImageBehavior = false;
            this.thumbnailList1.ItemActivate += new System.EventHandler(this.thumbnailList1_ItemActivate);
            this.thumbnailList1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.thumbnailList1_ItemDrag);
            this.thumbnailList1.SelectedIndexChanged += new System.EventHandler(this.thumbnailList1_SelectedIndexChanged);
            this.thumbnailList1.DragDrop += new System.Windows.Forms.DragEventHandler(this.thumbnailList1_DragDrop);
            this.thumbnailList1.DragEnter += new System.Windows.Forms.DragEventHandler(this.thumbnailList1_DragEnter);
            this.thumbnailList1.DragOver += new System.Windows.Forms.DragEventHandler(this.thumbnailList1_DragOver);
            this.thumbnailList1.DragLeave += new System.EventHandler(this.thumbnailList1_DragLeave);
            this.thumbnailList1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.thumbnailList1_KeyDown);
            this.thumbnailList1.MouseLeave += new System.EventHandler(this.thumbnailList1_MouseLeave);
            this.thumbnailList1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.thumbnailList1_MouseMove);
            // 
            // contextMenuStrip
            // 
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxView,
            this.ctxSeparator1,
            this.ctxSelectAll,
            this.ctxCopy,
            this.ctxPaste,
            this.ctxSeparator2,
            this.ctxDelete});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // ctxView
            // 
            resources.ApplyResources(this.ctxView, "ctxView");
            this.ctxView.Name = "ctxView";
            this.ctxView.Click += new System.EventHandler(this.ctxView_Click);
            // 
            // ctxSeparator1
            // 
            resources.ApplyResources(this.ctxSeparator1, "ctxSeparator1");
            this.ctxSeparator1.Name = "ctxSeparator1";
            // 
            // ctxSelectAll
            // 
            resources.ApplyResources(this.ctxSelectAll, "ctxSelectAll");
            this.ctxSelectAll.Name = "ctxSelectAll";
            this.ctxSelectAll.Click += new System.EventHandler(this.ctxSelectAll_Click);
            // 
            // ctxCopy
            // 
            resources.ApplyResources(this.ctxCopy, "ctxCopy");
            this.ctxCopy.Name = "ctxCopy";
            this.ctxCopy.Click += new System.EventHandler(this.ctxCopy_Click);
            // 
            // ctxPaste
            // 
            resources.ApplyResources(this.ctxPaste, "ctxPaste");
            this.ctxPaste.Name = "ctxPaste";
            this.ctxPaste.Click += new System.EventHandler(this.ctxPaste_Click);
            // 
            // ctxSeparator2
            // 
            resources.ApplyResources(this.ctxSeparator2, "ctxSeparator2");
            this.ctxSeparator2.Name = "ctxSeparator2";
            // 
            // ctxDelete
            // 
            resources.ApplyResources(this.ctxDelete, "ctxDelete");
            this.ctxDelete.Name = "ctxDelete";
            this.ctxDelete.Click += new System.EventHandler(this.ctxDelete_Click);
            // 
            // tStrip
            // 
            resources.ApplyResources(this.tStrip, "tStrip");
            this.tStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.tStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsScan,
            this.tsImport,
            this.toolStripSeparator5,
            this.tsdSelectBatch,
            this.tsdUploadDocuments,
            this.tsdUploadSqueeze,
            this.tsdSavePDF,
            this.tsdSaveImages,
            this.tsdEmailPDF,
            this.tsPrint,
            this.tsdSend,
            this.toolStripSeparator4,
            this.tsdImage,
            this.tsdRotate,
            this.tsMove,
            this.tsdReorder,
            this.toolStripSeparator2,
            this.tsDelete,
            this.tsClear,
            this.toolStripSeparator3,
            this.tsdSettings});
            this.tStrip.Name = "tStrip";
            this.tStrip.ShowItemToolTips = false;
            this.tStrip.Stretch = true;
            this.tStrip.TabStop = true;
            this.tStrip.DockChanged += new System.EventHandler(this.tStrip_DockChanged);
            // 
            // tsScan
            // 
            resources.ApplyResources(this.tsScan, "tsScan");
            this.tsScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.tsScan.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsNewProfile,
            this.tsBatchScan});
            this.tsScan.Image = global::NAPS2.Icons.control_play_blue_white;
            this.tsScan.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.tsScan.Name = "tsScan";
            this.tsScan.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.tsScan.ButtonClick += new System.EventHandler(this.tsScan_ButtonClick);
            // 
            // tsNewProfile
            // 
            resources.ApplyResources(this.tsNewProfile, "tsNewProfile");
            this.tsNewProfile.Image = global::NAPS2.Icons.add_small;
            this.tsNewProfile.Name = "tsNewProfile";
            this.tsNewProfile.Click += new System.EventHandler(this.tsNewProfile_Click);
            // 
            // tsBatchScan
            // 
            resources.ApplyResources(this.tsBatchScan, "tsBatchScan");
            this.tsBatchScan.Image = global::NAPS2.Icons.application_cascade;
            this.tsBatchScan.Name = "tsBatchScan";
            this.tsBatchScan.Click += new System.EventHandler(this.tsBatchScan_Click);
            // 
            // tsImport
            // 
            resources.ApplyResources(this.tsImport, "tsImport");
            this.tsImport.Image = global::NAPS2.Icons.folder_picture_white;
            this.tsImport.Name = "tsImport";
            this.tsImport.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.tsImport.Click += new System.EventHandler(this.tsImport_Click);
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // tsdSelectBatch
            // 
            resources.ApplyResources(this.tsdSelectBatch, "tsdSelectBatch");
            this.tsdSelectBatch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsDetectBarcodes});
            this.tsdSelectBatch.Image = global::NAPS2.Icons.barcode_white;
            this.tsdSelectBatch.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.tsdSelectBatch.Name = "tsdSelectBatch";
            this.tsdSelectBatch.ButtonClick += new System.EventHandler(this.tsdSelectBatch_ButtonClick);
            // 
            // tsDetectBarcodes
            // 
            resources.ApplyResources(this.tsDetectBarcodes, "tsDetectBarcodes");
            this.tsDetectBarcodes.Name = "tsDetectBarcodes";
            this.tsDetectBarcodes.Click += new System.EventHandler(this.tsDetectBarcodes_Click);
            // 
            // tsdUploadDocuments
            // 
            resources.ApplyResources(this.tsdUploadDocuments, "tsdUploadDocuments");
            this.tsdUploadDocuments.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsUploadDocumentsAll,
            this.tsUploadDocumentsSelected});
            this.tsdUploadDocuments.Image = global::NAPS2.Icons.inbox_out_white;
            this.tsdUploadDocuments.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.tsdUploadDocuments.Name = "tsdUploadDocuments";
            this.tsdUploadDocuments.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsdUploadDocuments.ButtonClick += new System.EventHandler(this.tsdUploadDocuments_ButtonClick);
            // 
            // tsUploadDocumentsAll
            // 
            resources.ApplyResources(this.tsUploadDocumentsAll, "tsUploadDocumentsAll");
            this.tsUploadDocumentsAll.Name = "tsUploadDocumentsAll";
            this.tsUploadDocumentsAll.Click += new System.EventHandler(this.tsUploadDocumentsAll_Click);
            // 
            // tsUploadDocumentsSelected
            // 
            resources.ApplyResources(this.tsUploadDocumentsSelected, "tsUploadDocumentsSelected");
            this.tsUploadDocumentsSelected.Name = "tsUploadDocumentsSelected";
            this.tsUploadDocumentsSelected.Click += new System.EventHandler(this.tsUploadDocumentsSelected_Click);
            // 
            // tsdUploadSqueeze
            // 
            resources.ApplyResources(this.tsdUploadSqueeze, "tsdUploadSqueeze");
            this.tsdUploadSqueeze.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsUploadSqueezeAll,
            this.tsUploadSqueezeSelected,
            this.tsUploadSqueezeBindings});
            this.tsdUploadSqueeze.Image = global::NAPS2.Icons.inbox_out_white;
            this.tsdUploadSqueeze.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.tsdUploadSqueeze.Name = "tsdUploadSqueeze";
            this.tsdUploadSqueeze.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsdUploadSqueeze.ButtonClick += new System.EventHandler(this.tsdUploadSqueeze_ButtonClick);
            // 
            // tsUploadSqueezeAll
            // 
            resources.ApplyResources(this.tsUploadSqueezeAll, "tsUploadSqueezeAll");
            this.tsUploadSqueezeAll.Name = "tsUploadSqueezeAll";
            this.tsUploadSqueezeAll.Click += new System.EventHandler(this.tsUploadSqueezeAll_Click);
            // 
            // tsUploadSqueezeSelected
            // 
            resources.ApplyResources(this.tsUploadSqueezeSelected, "tsUploadSqueezeSelected");
            this.tsUploadSqueezeSelected.Name = "tsUploadSqueezeSelected";
            this.tsUploadSqueezeSelected.Click += new System.EventHandler(this.tsUploadSqueezeSelected_Click);
            // 
            // tsUploadSqueezeBindings
            // 
            resources.ApplyResources(this.tsUploadSqueezeBindings, "tsUploadSqueezeBindings");
            this.tsUploadSqueezeBindings.Name = "tsUploadSqueezeBindings";
            this.tsUploadSqueezeBindings.Click += new System.EventHandler(this.tsUploadSqueezeBindings_Click);
            // 
            // tsdSavePDF
            // 
            resources.ApplyResources(this.tsdSavePDF, "tsdSavePDF");
            this.tsdSavePDF.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsSavePDFAll,
            this.tsSavePDFSelected});
            this.tsdSavePDF.Image = global::NAPS2.Icons.file_extension_pdf_white;
            this.tsdSavePDF.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.tsdSavePDF.Name = "tsdSavePDF";
            this.tsdSavePDF.ButtonClick += new System.EventHandler(this.tsdSavePDF_ButtonClick);
            // 
            // tsSavePDFAll
            // 
            resources.ApplyResources(this.tsSavePDFAll, "tsSavePDFAll");
            this.tsSavePDFAll.Name = "tsSavePDFAll";
            this.tsSavePDFAll.Click += new System.EventHandler(this.tsSavePDFAll_Click);
            // 
            // tsSavePDFSelected
            // 
            resources.ApplyResources(this.tsSavePDFSelected, "tsSavePDFSelected");
            this.tsSavePDFSelected.Name = "tsSavePDFSelected";
            this.tsSavePDFSelected.Click += new System.EventHandler(this.tsSavePDFSelected_Click);
            // 
            // tsdSaveImages
            // 
            resources.ApplyResources(this.tsdSaveImages, "tsdSaveImages");
            this.tsdSaveImages.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsSaveImagesAll,
            this.tsSaveImagesSelected});
            this.tsdSaveImages.Image = global::NAPS2.Icons.pictures_white;
            this.tsdSaveImages.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.tsdSaveImages.Name = "tsdSaveImages";
            this.tsdSaveImages.ButtonClick += new System.EventHandler(this.tsdSaveImages_ButtonClick);
            // 
            // tsSaveImagesAll
            // 
            resources.ApplyResources(this.tsSaveImagesAll, "tsSaveImagesAll");
            this.tsSaveImagesAll.Name = "tsSaveImagesAll";
            this.tsSaveImagesAll.Click += new System.EventHandler(this.tsSaveImagesAll_Click);
            // 
            // tsSaveImagesSelected
            // 
            resources.ApplyResources(this.tsSaveImagesSelected, "tsSaveImagesSelected");
            this.tsSaveImagesSelected.Name = "tsSaveImagesSelected";
            this.tsSaveImagesSelected.Click += new System.EventHandler(this.tsSaveImagesSelected_Click);
            // 
            // tsdEmailPDF
            // 
            resources.ApplyResources(this.tsdEmailPDF, "tsdEmailPDF");
            this.tsdEmailPDF.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsEmailPDFAll,
            this.tsEmailPDFSelected});
            this.tsdEmailPDF.Image = global::NAPS2.Icons.mail_attachment_white;
            this.tsdEmailPDF.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.tsdEmailPDF.Name = "tsdEmailPDF";
            this.tsdEmailPDF.ButtonClick += new System.EventHandler(this.tsdEmailPDF_ButtonClick);
            // 
            // tsEmailPDFAll
            // 
            resources.ApplyResources(this.tsEmailPDFAll, "tsEmailPDFAll");
            this.tsEmailPDFAll.Name = "tsEmailPDFAll";
            this.tsEmailPDFAll.Click += new System.EventHandler(this.tsEmailPDFAll_Click);
            // 
            // tsEmailPDFSelected
            // 
            resources.ApplyResources(this.tsEmailPDFSelected, "tsEmailPDFSelected");
            this.tsEmailPDFSelected.Name = "tsEmailPDFSelected";
            this.tsEmailPDFSelected.Click += new System.EventHandler(this.tsEmailPDFSelected_Click);
            // 
            // tsPrint
            // 
            resources.ApplyResources(this.tsPrint, "tsPrint");
            this.tsPrint.Image = global::NAPS2.Icons.printer_white;
            this.tsPrint.Name = "tsPrint";
            this.tsPrint.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.tsPrint.Click += new System.EventHandler(this.tsPrint_Click);
            // 
            // tsdSend
            // 
            resources.ApplyResources(this.tsdSend, "tsdSend");
            this.tsdSend.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsSendAll,
            this.tsSendSelected});
            this.tsdSend.Image = global::NAPS2.Icons.inbox_out_white;
            this.tsdSend.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.tsdSend.Name = "tsdSend";
            this.tsdSend.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsdSend.ButtonClick += new System.EventHandler(this.tsdSend_ButtonClick);
            // 
            // tsSendAll
            // 
            resources.ApplyResources(this.tsSendAll, "tsSendAll");
            this.tsSendAll.Name = "tsSendAll";
            this.tsSendAll.Click += new System.EventHandler(this.tsSendAll_Click);
            // 
            // tsSendSelected
            // 
            resources.ApplyResources(this.tsSendSelected, "tsSendSelected");
            this.tsSendSelected.Name = "tsSendSelected";
            this.tsSendSelected.Click += new System.EventHandler(this.tsSendSelected_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // tsdImage
            // 
            resources.ApplyResources(this.tsdImage, "tsdImage");
            this.tsdImage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsView,
            this.toolStripSeparator6,
            this.tsCrop,
            this.tsBrightnessContrast,
            this.tsHueSaturation,
            this.tsBlackWhite,
            this.tsSharpen,
            this.toolStripSeparator7,
            this.tsReset});
            this.tsdImage.Image = global::NAPS2.Icons.pencil_white;
            this.tsdImage.Name = "tsdImage";
            this.tsdImage.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            // 
            // tsView
            // 
            resources.ApplyResources(this.tsView, "tsView");
            this.tsView.Name = "tsView";
            this.tsView.Click += new System.EventHandler(this.tsView_Click);
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // tsCrop
            // 
            resources.ApplyResources(this.tsCrop, "tsCrop");
            this.tsCrop.Image = global::NAPS2.Icons.transform_crop;
            this.tsCrop.Name = "tsCrop";
            this.tsCrop.Click += new System.EventHandler(this.tsCrop_Click);
            // 
            // tsBrightnessContrast
            // 
            resources.ApplyResources(this.tsBrightnessContrast, "tsBrightnessContrast");
            this.tsBrightnessContrast.Image = global::NAPS2.Icons.contrast_with_sun;
            this.tsBrightnessContrast.Name = "tsBrightnessContrast";
            this.tsBrightnessContrast.Click += new System.EventHandler(this.tsBrightnessContrast_Click);
            // 
            // tsHueSaturation
            // 
            resources.ApplyResources(this.tsHueSaturation, "tsHueSaturation");
            this.tsHueSaturation.Image = global::NAPS2.Icons.color_management;
            this.tsHueSaturation.Name = "tsHueSaturation";
            this.tsHueSaturation.Click += new System.EventHandler(this.tsHueSaturation_Click);
            // 
            // tsBlackWhite
            // 
            resources.ApplyResources(this.tsBlackWhite, "tsBlackWhite");
            this.tsBlackWhite.Image = global::NAPS2.Icons.contrast_high;
            this.tsBlackWhite.Name = "tsBlackWhite";
            this.tsBlackWhite.Click += new System.EventHandler(this.tsBlackWhite_Click);
            // 
            // tsSharpen
            // 
            resources.ApplyResources(this.tsSharpen, "tsSharpen");
            this.tsSharpen.Image = global::NAPS2.Icons.sharpen;
            this.tsSharpen.Name = "tsSharpen";
            this.tsSharpen.Click += new System.EventHandler(this.tsSharpen_Click);
            // 
            // toolStripSeparator7
            // 
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            // 
            // tsReset
            // 
            resources.ApplyResources(this.tsReset, "tsReset");
            this.tsReset.Name = "tsReset";
            this.tsReset.Click += new System.EventHandler(this.tsReset_Click);
            // 
            // tsdRotate
            // 
            resources.ApplyResources(this.tsdRotate, "tsdRotate");
            this.tsdRotate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsRotateLeft,
            this.tsRotateRight,
            this.tsFlip,
            this.tsDeskew,
            this.tsCustomRotation});
            this.tsdRotate.Image = global::NAPS2.Icons.arrow_rotate_anticlockwise_white;
            this.tsdRotate.Name = "tsdRotate";
            this.tsdRotate.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            // 
            // tsRotateLeft
            // 
            resources.ApplyResources(this.tsRotateLeft, "tsRotateLeft");
            this.tsRotateLeft.Image = global::NAPS2.Icons.arrow_rotate_anticlockwise_small;
            this.tsRotateLeft.Name = "tsRotateLeft";
            this.tsRotateLeft.Click += new System.EventHandler(this.tsRotateLeft_Click);
            // 
            // tsRotateRight
            // 
            resources.ApplyResources(this.tsRotateRight, "tsRotateRight");
            this.tsRotateRight.Image = global::NAPS2.Icons.arrow_rotate_clockwise_small;
            this.tsRotateRight.Name = "tsRotateRight";
            this.tsRotateRight.Click += new System.EventHandler(this.tsRotateRight_Click);
            // 
            // tsFlip
            // 
            resources.ApplyResources(this.tsFlip, "tsFlip");
            this.tsFlip.Image = global::NAPS2.Icons.arrow_switch_small;
            this.tsFlip.Name = "tsFlip";
            this.tsFlip.Click += new System.EventHandler(this.tsFlip_Click);
            // 
            // tsDeskew
            // 
            resources.ApplyResources(this.tsDeskew, "tsDeskew");
            this.tsDeskew.Name = "tsDeskew";
            this.tsDeskew.Click += new System.EventHandler(this.tsDeskew_Click);
            // 
            // tsCustomRotation
            // 
            resources.ApplyResources(this.tsCustomRotation, "tsCustomRotation");
            this.tsCustomRotation.Name = "tsCustomRotation";
            this.tsCustomRotation.Click += new System.EventHandler(this.tsCustomRotation_Click);
            // 
            // tsMove
            // 
            resources.ApplyResources(this.tsMove, "tsMove");
            this.tsMove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.tsMove.FirstImage = global::NAPS2.Icons.arrow_up_small_white;
            this.tsMove.MaxTextWidth = 80;
            this.tsMove.Name = "tsMove";
            this.tsMove.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.tsMove.SecondImage = global::NAPS2.Icons.arrow_down_small_white;
            this.tsMove.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsMove.FirstClick += new System.EventHandler(this.tsMove_FirstClick);
            this.tsMove.SecondClick += new System.EventHandler(this.tsMove_SecondClick);
            // 
            // tsdReorder
            // 
            resources.ApplyResources(this.tsdReorder, "tsdReorder");
            this.tsdReorder.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsInterleave,
            this.tsDeinterleave,
            this.toolStripSeparator12,
            this.tsAltInterleave,
            this.tsAltDeinterleave,
            this.toolStripSeparator1,
            this.tsReverse});
            this.tsdReorder.Image = global::NAPS2.Icons.arrow_refresh_white;
            this.tsdReorder.Name = "tsdReorder";
            this.tsdReorder.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            // 
            // tsInterleave
            // 
            resources.ApplyResources(this.tsInterleave, "tsInterleave");
            this.tsInterleave.Name = "tsInterleave";
            this.tsInterleave.Click += new System.EventHandler(this.tsInterleave_Click);
            // 
            // tsDeinterleave
            // 
            resources.ApplyResources(this.tsDeinterleave, "tsDeinterleave");
            this.tsDeinterleave.Name = "tsDeinterleave";
            this.tsDeinterleave.Click += new System.EventHandler(this.tsDeinterleave_Click);
            // 
            // toolStripSeparator12
            // 
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            // 
            // tsAltInterleave
            // 
            resources.ApplyResources(this.tsAltInterleave, "tsAltInterleave");
            this.tsAltInterleave.Name = "tsAltInterleave";
            this.tsAltInterleave.Click += new System.EventHandler(this.tsAltInterleave_Click);
            // 
            // tsAltDeinterleave
            // 
            resources.ApplyResources(this.tsAltDeinterleave, "tsAltDeinterleave");
            this.tsAltDeinterleave.Name = "tsAltDeinterleave";
            this.tsAltDeinterleave.Click += new System.EventHandler(this.tsAltDeinterleave_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // tsReverse
            // 
            resources.ApplyResources(this.tsReverse, "tsReverse");
            this.tsReverse.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsReverseAll,
            this.tsReverseSelected});
            this.tsReverse.Name = "tsReverse";
            // 
            // tsReverseAll
            // 
            resources.ApplyResources(this.tsReverseAll, "tsReverseAll");
            this.tsReverseAll.Name = "tsReverseAll";
            this.tsReverseAll.Click += new System.EventHandler(this.tsReverseAll_Click);
            // 
            // tsReverseSelected
            // 
            resources.ApplyResources(this.tsReverseSelected, "tsReverseSelected");
            this.tsReverseSelected.Name = "tsReverseSelected";
            this.tsReverseSelected.Click += new System.EventHandler(this.tsReverseSelected_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // tsDelete
            // 
            resources.ApplyResources(this.tsDelete, "tsDelete");
            this.tsDelete.Image = global::NAPS2.Icons.cross_white;
            this.tsDelete.Name = "tsDelete";
            this.tsDelete.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.tsDelete.Click += new System.EventHandler(this.tsDelete_Click);
            // 
            // tsClear
            // 
            resources.ApplyResources(this.tsClear, "tsClear");
            this.tsClear.Image = global::NAPS2.Icons.cancel_white;
            this.tsClear.Name = "tsClear";
            this.tsClear.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.tsClear.Click += new System.EventHandler(this.tsClear_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // tsdSettings
            // 
            resources.ApplyResources(this.tsdSettings, "tsdSettings");
            this.tsdSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsProfiles,
            this.tsBarcodeSettings,
            this.tsOcr,
            this.toolStripSeparator8,
            this.tsPDFSettings,
            this.tsImageSettings,
            this.tsDocumentsSettings,
            this.tsSqueezeSettings,
            this.tsEmailSettings,
            this.toolStripSeparator13,
            this.tsAbout,
            this.tsdLanguage});
            this.tsdSettings.Image = global::NAPS2.Icons.gearwheels_white;
            this.tsdSettings.Name = "tsdSettings";
            // 
            // tsProfiles
            // 
            resources.ApplyResources(this.tsProfiles, "tsProfiles");
            this.tsProfiles.Name = "tsProfiles";
            this.tsProfiles.Click += new System.EventHandler(this.tsProfiles_Click);
            // 
            // tsBarcodeSettings
            // 
            resources.ApplyResources(this.tsBarcodeSettings, "tsBarcodeSettings");
            this.tsBarcodeSettings.Name = "tsBarcodeSettings";
            this.tsBarcodeSettings.Click += new System.EventHandler(this.tsBarcodeSettings_Click);
            // 
            // tsOcr
            // 
            resources.ApplyResources(this.tsOcr, "tsOcr");
            this.tsOcr.Name = "tsOcr";
            this.tsOcr.Click += new System.EventHandler(this.tsOcr_Click);
            // 
            // toolStripSeparator8
            // 
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // tsPDFSettings
            // 
            resources.ApplyResources(this.tsPDFSettings, "tsPDFSettings");
            this.tsPDFSettings.Name = "tsPDFSettings";
            this.tsPDFSettings.Click += new System.EventHandler(this.tsPDFSettings_Click);
            // 
            // tsImageSettings
            // 
            resources.ApplyResources(this.tsImageSettings, "tsImageSettings");
            this.tsImageSettings.Name = "tsImageSettings";
            this.tsImageSettings.Click += new System.EventHandler(this.tsImageSettings_Click);
            // 
            // tsDocumentsSettings
            // 
            resources.ApplyResources(this.tsDocumentsSettings, "tsDocumentsSettings");
            this.tsDocumentsSettings.Name = "tsDocumentsSettings";
            this.tsDocumentsSettings.Click += new System.EventHandler(this.tsDocumentsSettings_Click);
            // 
            // tsSqueezeSettings
            // 
            resources.ApplyResources(this.tsSqueezeSettings, "tsSqueezeSettings");
            this.tsSqueezeSettings.Name = "tsSqueezeSettings";
            this.tsSqueezeSettings.Click += new System.EventHandler(this.tsSqueezeSettings_Click);
            // 
            // tsEmailSettings
            // 
            resources.ApplyResources(this.tsEmailSettings, "tsEmailSettings");
            this.tsEmailSettings.Name = "tsEmailSettings";
            this.tsEmailSettings.Click += new System.EventHandler(this.tsEmailSettings_Click);
            // 
            // toolStripSeparator13
            // 
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            // 
            // tsAbout
            // 
            resources.ApplyResources(this.tsAbout, "tsAbout");
            this.tsAbout.Name = "tsAbout";
            this.tsAbout.Click += new System.EventHandler(this.tsAbout_Click);
            // 
            // tsdLanguage
            // 
            resources.ApplyResources(this.tsdLanguage, "tsdLanguage");
            this.tsdLanguage.Name = "tsdLanguage";
            // 
            // FDesktop
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.toolStripContainer1);
            this.DoubleBuffered = true;
            this.Name = "FDesktop";
            this.Load += new System.EventHandler(this.FDesktop_Load);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.tStrip.ResumeLayout(false);
            this.tStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip tStrip;
        private System.Windows.Forms.ToolStripSplitButton tsScan;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private ThumbnailList thumbnailList1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private ToolStripDoubleButton tsMove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsClear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsDelete;
        private System.Windows.Forms.ToolStripButton tsImport;
        private System.Windows.Forms.ToolStripSplitButton tsdSavePDF;
        private System.Windows.Forms.ToolStripMenuItem tsSavePDFAll;
        private System.Windows.Forms.ToolStripMenuItem tsSavePDFSelected;
        private System.Windows.Forms.ToolStripDropDownButton tsdRotate;
        private System.Windows.Forms.ToolStripMenuItem tsRotateLeft;
        private System.Windows.Forms.ToolStripMenuItem tsRotateRight;
        private System.Windows.Forms.ToolStripMenuItem tsFlip;
        private System.Windows.Forms.ToolStripDropDownButton tsdReorder;
        private System.Windows.Forms.ToolStripMenuItem tsInterleave;
        private System.Windows.Forms.ToolStripMenuItem tsDeinterleave;
        private System.Windows.Forms.ToolStripSplitButton tsdSaveImages;
        private System.Windows.Forms.ToolStripMenuItem tsSaveImagesAll;
        private System.Windows.Forms.ToolStripMenuItem tsSaveImagesSelected;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ctxView;
        private System.Windows.Forms.ToolStripMenuItem ctxSelectAll;
        private System.Windows.Forms.ToolStripMenuItem ctxCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsReverse;
        private System.Windows.Forms.ToolStripMenuItem tsReverseAll;
        private System.Windows.Forms.ToolStripMenuItem tsReverseSelected;
        private System.Windows.Forms.ToolStripMenuItem tsNewProfile;
        private System.Windows.Forms.ToolStripDropDownButton tsdImage;
        private System.Windows.Forms.ToolStripMenuItem tsView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem tsBrightnessContrast;
        private System.Windows.Forms.ToolStripMenuItem tsCrop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem tsReset;
        private System.Windows.Forms.ToolStripMenuItem tsCustomRotation;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator1;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem tsAltInterleave;
        private System.Windows.Forms.ToolStripMenuItem tsAltDeinterleave;
        private System.Windows.Forms.ToolStripMenuItem tsBatchScan;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ctxDelete;
        private System.Windows.Forms.Button btnZoomMouseCatcher;
        private System.Windows.Forms.ToolStripMenuItem ctxPaste;
        private System.Windows.Forms.ToolStripButton tsPrint;
        private System.Windows.Forms.ToolStripMenuItem tsDeskew;
        private System.Windows.Forms.ToolStripMenuItem tsSharpen;
        private System.Windows.Forms.ToolStripMenuItem tsHueSaturation;
        private System.Windows.Forms.ToolStripMenuItem tsBlackWhite;
        private System.Windows.Forms.ToolStripSplitButton tsdEmailPDF;
        private System.Windows.Forms.ToolStripMenuItem tsEmailPDFAll;
        private System.Windows.Forms.ToolStripMenuItem tsEmailPDFSelected;
        private System.Windows.Forms.ToolStripSplitButton tsdUploadDocuments;
        private System.Windows.Forms.ToolStripMenuItem tsUploadDocumentsAll;
        private System.Windows.Forms.ToolStripMenuItem tsUploadDocumentsSelected;
        private System.Windows.Forms.ToolStripSplitButton tsdUploadSqueeze;
        private System.Windows.Forms.ToolStripMenuItem tsUploadSqueezeAll;
        private System.Windows.Forms.ToolStripMenuItem tsUploadSqueezeSelected;
        private System.Windows.Forms.ToolStripDropDownButton tsdSettings;
        private System.Windows.Forms.ToolStripMenuItem tsAbout;
        private System.Windows.Forms.ToolStripMenuItem tsdLanguage;
        private System.Windows.Forms.ToolStripMenuItem tsProfiles;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem tsPDFSettings;
        private System.Windows.Forms.ToolStripMenuItem tsImageSettings;
        private System.Windows.Forms.ToolStripMenuItem tsDocumentsSettings;
        private System.Windows.Forms.ToolStripMenuItem tsSqueezeSettings;
        private System.Windows.Forms.ToolStripMenuItem tsEmailSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem tsOcr;
        private System.Windows.Forms.ToolStripSplitButton tsdSelectBatch;
        private System.Windows.Forms.ToolStripMenuItem tsDetectBarcodes;
        private System.Windows.Forms.ToolStripMenuItem tsBarcodeSettings;
        private System.Windows.Forms.ToolStripSplitButton tsdSend;
        private System.Windows.Forms.ToolStripMenuItem tsSendAll;
        private System.Windows.Forms.ToolStripMenuItem tsSendSelected;
        private System.Windows.Forms.ToolStripMenuItem tsUploadSqueezeBindings;
    }
}

