using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAPS2.Config;
using NAPS2.ImportExport;
using NAPS2.ImportExport.Documents;
using NAPS2.ImportExport.Email;
using NAPS2.ImportExport.Images;
using NAPS2.ImportExport.Pdf;
using NAPS2.ImportExport.Squeeze;
using NAPS2.Lang.Resources;
using NAPS2.Ocr;
using NAPS2.Operation;
using NAPS2.Scan.Images;
using NAPS2.Util;
using Binding = NAPS2.Barcode.Binding;

namespace NAPS2.WinForms
{
    public class WinFormsExportHelper
    {
        private readonly PdfSettingsContainer pdfSettingsContainer;
        private readonly DocumentsSettingsContainer documentsSettingsContainer;
        private readonly SqueezeSettingsContainer squeezeSettingsContainer;
        private readonly ImageSettingsContainer imageSettingsContainer;
        private readonly EmailSettingsContainer emailSettingsContainer;
        private readonly DialogHelper dialogHelper;
        private readonly FileNamePlaceholders fileNamePlaceholders;
        private readonly ChangeTracker changeTracker;
        private readonly IOperationFactory operationFactory;
        private readonly IFormFactory formFactory;
        private readonly OcrManager ocrManager;
        private readonly IEmailProviderFactory emailProviderFactory;
        private readonly IOperationProgress operationProgress;
        private readonly IUserConfigManager userConfigManager;
        private readonly AppConfigManager appConfigManager;

        public WinFormsExportHelper(PdfSettingsContainer pdfSettingsContainer, DocumentsSettingsContainer documentsSettingsContainer, SqueezeSettingsContainer squeezeSettingsContainer, ImageSettingsContainer imageSettingsContainer, EmailSettingsContainer emailSettingsContainer, DialogHelper dialogHelper, FileNamePlaceholders fileNamePlaceholders, ChangeTracker changeTracker, IOperationFactory operationFactory, IFormFactory formFactory, OcrManager ocrManager, IEmailProviderFactory emailProviderFactory, IOperationProgress operationProgress, IUserConfigManager userConfigManager, AppConfigManager appConfigManager)
        {
            this.pdfSettingsContainer = pdfSettingsContainer;
            this.documentsSettingsContainer = documentsSettingsContainer;
            this.squeezeSettingsContainer = squeezeSettingsContainer;
            this.imageSettingsContainer = imageSettingsContainer;
            this.emailSettingsContainer = emailSettingsContainer;
            this.dialogHelper = dialogHelper;
            this.fileNamePlaceholders = fileNamePlaceholders;
            this.changeTracker = changeTracker;
            this.operationFactory = operationFactory;
            this.formFactory = formFactory;
            this.ocrManager = ocrManager;
            this.emailProviderFactory = emailProviderFactory;
            this.operationProgress = operationProgress;
            this.userConfigManager = userConfigManager;
            this.appConfigManager = appConfigManager;
        }

        public async Task<bool> SavePdf(List<ScannedImage> images, ISaveNotify notify)
        {
            if (images.Any())
            {
                string savePath;

                var pdfSettings = pdfSettingsContainer.PdfSettings;
                if (pdfSettings.SkipSavePrompt && Path.IsPathRooted(pdfSettings.DefaultFileName))
                {
                    savePath = pdfSettings.DefaultFileName;
                }
                else
                {
                    if (!dialogHelper.PromptToSavePdf(pdfSettings.DefaultFileName, out savePath))
                    {
                        return false;
                    }
                }

                var subSavePath = fileNamePlaceholders.SubstitutePlaceholders(savePath, DateTime.Now, barcode: images.FirstOrDefault()?.Barcodes?.FirstOrDefault()?.Text);
                var changeToken = changeTracker.State;
                string firstFileSaved = await ExportPdf(subSavePath, images, false, null);
                if (firstFileSaved != null)
                {
                    changeTracker.Saved(changeToken);
                    notify?.PdfSaved(firstFileSaved);
                    return true;
                }
            }
            return false;
        }

        public async Task<string> ExportPdfToSqueeze(string filename, List<ScannedImage> images, bool email, EmailMessage emailMessage, Dictionary<string, string> fields)
        {
            var op = operationFactory.Create<SavePdfOperation>();

            var pdfSettings = pdfSettingsContainer.PdfSettings;
            pdfSettings.Metadata.Creator = MiscResources.NAPS2;
            if (op.StartSqueeze(filename, DateTime.Now, images, pdfSettings, ocrManager.DefaultParams, email, emailMessage, fields))
            {
                operationProgress.ShowProgress(op);
            }
            return await op.Success ? op.FirstFileSaved : null;
        }

        public async Task<string> ExportPdfToDocuments(string filename, List<ScannedImage> images, bool email, EmailMessage emailMessage)
        {
            var op = operationFactory.Create<SavePdfOperation>();

            var pdfSettings = pdfSettingsContainer.PdfSettings;
            pdfSettings.Metadata.Creator = MiscResources.NAPS2;
            if (op.StartDocuments(filename, DateTime.Now, images, pdfSettings, ocrManager.DefaultParams, email, emailMessage))
            {
                operationProgress.ShowProgress(op);
            }
            return await op.Success ? op.FirstFileSaved : null;
        }

        public async Task<string> ExportPdf(string filename, List<ScannedImage> images, bool email, EmailMessage emailMessage)
        {
            var op = operationFactory.Create<SavePdfOperation>();

            var pdfSettings = pdfSettingsContainer.PdfSettings;
            pdfSettings.Metadata.Creator = MiscResources.NAPS2;
            if (op.Start(filename, DateTime.Now, images, pdfSettings, ocrManager.DefaultParams, email, emailMessage, false, false))
            {
                operationProgress.ShowProgress(op);
            }
            return await op.Success ? op.FirstFileSaved : null;
        }

        public async Task<bool> SaveImages(List<ScannedImage> images, ISaveNotify notify)
        {
            if (images.Any())
            {
                string savePath;

                var imageSettings = imageSettingsContainer.ImageSettings;
                if (imageSettings.SkipSavePrompt && Path.IsPathRooted(imageSettings.DefaultFileName))
                {
                    savePath = imageSettings.DefaultFileName;
                }
                else
                {
                    if (!dialogHelper.PromptToSaveImage(imageSettings.DefaultFileName, out savePath))
                    {
                        return false;
                    }
                }

                var op = operationFactory.Create<SaveImagesOperation>();
                var changeToken = changeTracker.State;
                if (op.Start(savePath, DateTime.Now, images))
                {
                    operationProgress.ShowProgress(op);
                }
                if (await op.Success)
                {
                    changeTracker.Saved(changeToken);
                    notify?.ImagesSaved(images.Count, op.FirstFileSaved);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> EmailPDF(List<ScannedImage> images)
        {
            if (!images.Any())
            {
                return false;
            }

            if (userConfigManager.Config.EmailSetup == null)
            {
                // First run; prompt for a 
                var form = formFactory.Create<FEmailProvider>();
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
            }

            var emailSettings = emailSettingsContainer.EmailSettings;
            var invalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());
            var attachmentName = new string(emailSettings.AttachmentName.Where(x => !invalidChars.Contains(x)).ToArray());
            if (string.IsNullOrEmpty(attachmentName))
            {
                attachmentName = "Scan.pdf";
            }
            if (!attachmentName.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                attachmentName += ".pdf";
            }
            attachmentName = fileNamePlaceholders.SubstitutePlaceholders(attachmentName, DateTime.Now, false, barcode: images.FirstOrDefault()?.Barcodes?.FirstOrDefault()?.Text);

            var tempFolder = new DirectoryInfo(Path.Combine(Paths.Temp, Path.GetRandomFileName()));
            tempFolder.Create();
            try
            {
                string targetPath = Path.Combine(tempFolder.FullName, attachmentName);
                var changeToken = changeTracker.State;

                var message = new EmailMessage();
                if (await ExportPdf(targetPath, images, true, message) != null)
                {
                    changeTracker.Saved(changeToken);
                    return true;
                }
            }
            finally
            {
                tempFolder.Delete(true);
            }
            return false;
        }

        public Task<bool> UploadPDFToSqueeze(List<ScannedImage> images)
        {
            return UploadPDFToSqueeze(images, new Dictionary<string, string>());
        }

        public async Task<bool> UploadPDFToSqueeze(List<ScannedImage> images, Dictionary<string, string> fields)
        {
            if (!images.Any())
            {
                return false;
            }
            
            var squeezeSettings = squeezeSettingsContainer.SqueezeSettings;
            if (squeezeSettings == null || squeezeSettings.IsMissingDetails)
            {
                // First run; prompt for a 
                var form = formFactory.Create<FSqueezeSettings>();
                if (form.ShowDialog() != DialogResult.OK)
                    return false;
            }

            var attachmentName = "Upload.pdf";
            var tempFolder = new DirectoryInfo(Path.Combine(Paths.Temp, Path.GetRandomFileName()));
            tempFolder.Create();
            try
            {
                string targetPath = Path.Combine(tempFolder.FullName, attachmentName);
                var changeToken = changeTracker.State;

                if (await ExportPdfToSqueeze(targetPath, images, false, null, fields) != null)
                {
                    changeTracker.Saved(changeToken);
                    return true;
                }
            }
            finally
            {
                tempFolder.Delete(true);
            }
            return false;
        }

        public async Task<bool> UploadPDFToDocuments(List<ScannedImage> images)
        {
            if (!images.Any())
            {
                return false;
            }

            var documentsSettings = documentsSettingsContainer.DocumentsSettings;
            if (documentsSettings == null || documentsSettings.IsMissingDetails)
            {
                // First run; prompt for a 
                var form = formFactory.Create<FDocumentsSettings>();
                if (form.ShowDialog() != DialogResult.OK)
                    return false;
            }

            var attachmentName = "Upload.pdf";
            var tempFolder = new DirectoryInfo(Path.Combine(Paths.Temp, Path.GetRandomFileName()));
            tempFolder.Create();
            try
            {
                string targetPath = Path.Combine(tempFolder.FullName, attachmentName);
                var changeToken = changeTracker.State;

                if (await ExportPdfToDocuments(targetPath, images, false, null) != null)
                {
                    changeTracker.Saved(changeToken);
                    return true;
                }
            }
            finally
            {
                tempFolder.Delete(true);
            }
            return false;
        }

        public async Task<bool> SendPDF(List<ScannedImage> images)
        {
            if (!images.Any())
            {
                return false;
            }

            var attachmentName = "Upload.pdf";
            var tempFolder = new DirectoryInfo(Path.Combine(Paths.Temp, Path.GetRandomFileName()));
            tempFolder.Create();
            try
            {
                string targetPath = Path.Combine(tempFolder.FullName, attachmentName);
                var changeToken = changeTracker.State;

                if (await ExportPdf(targetPath, images, false, null) != null)
                {
                    changeTracker.Saved(changeToken);
                    var args = appConfigManager.Config.SendArguments;
                    if(string.IsNullOrWhiteSpace(args))
                        args = "{0}";
                    args = string.Format(args, targetPath);
                    Process.Start(appConfigManager.Config.SendTarget, args);
                    return true;
                }
            }
            finally
            {
                //tempFolder.Delete(true);
            }
            return false;
        }
    }
}
