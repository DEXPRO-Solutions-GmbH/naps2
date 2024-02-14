using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAPS2.ImportExport.Documents;
using NAPS2.ImportExport.Email;
using NAPS2.ImportExport.Squeeze;
using NAPS2.Lang.Resources;
using NAPS2.Logging;
using NAPS2.Ocr;
using NAPS2.Operation;
using NAPS2.Scan.Images;
using NAPS2.Util;

namespace NAPS2.ImportExport.Pdf
{
    public class SavePdfOperation : OperationBase
    {
        private readonly FileNamePlaceholders fileNamePlaceholders;
        private readonly DocumentsUploader documentsUploader;
        private readonly SqueezeClient squeezeUploader;
        private readonly IPdfExporter pdfExporter;
        private readonly IOverwritePrompt overwritePrompt;
        private readonly IEmailProviderFactory emailProviderFactory;

        public SavePdfOperation(FileNamePlaceholders fileNamePlaceholders, IPdfExporter pdfExporter, IOverwritePrompt overwritePrompt, IEmailProviderFactory emailProviderFactory, DocumentsUploader documentsUploader, SqueezeClient squeezeUploader)
        {
            this.fileNamePlaceholders = fileNamePlaceholders;
            this.pdfExporter = pdfExporter;
            this.overwritePrompt = overwritePrompt;
            this.emailProviderFactory = emailProviderFactory;
            this.documentsUploader = documentsUploader;
            this.squeezeUploader = squeezeUploader;

            AllowCancel = true;
            AllowBackground = true;
        }
        public string FirstFileSaved { get; private set; }

        public bool StartSqueeze(string fileName, DateTime dateTime, ICollection<ScannedImage> images, PdfSettings pdfSettings, OcrParams ocrParams, bool email, EmailMessage emailMessage, Dictionary<string, string> fields)
        {
            var x = Start(fileName, dateTime, images, pdfSettings, ocrParams, email, emailMessage, false, true, fields);
            return x;
        }

        public bool StartDocuments(string fileName, DateTime dateTime, ICollection<ScannedImage> images, PdfSettings pdfSettings, OcrParams ocrParams, bool email, EmailMessage emailMessage)
        {
            return Start(fileName, dateTime, images, pdfSettings, ocrParams, email, emailMessage, true, false);
        }

        public bool Start(string fileName, DateTime dateTime, ICollection<ScannedImage> images, PdfSettings pdfSettings, OcrParams ocrParams, bool email, EmailMessage emailMessage, bool uploadToDocuments, bool uploadToSquzeeze)
        {
            return Start(fileName, dateTime, images, pdfSettings, ocrParams, email, emailMessage, uploadToDocuments, uploadToSquzeeze, null);
        }

        private bool Start(string fileName, DateTime dateTime, ICollection<ScannedImage> images, PdfSettings pdfSettings, OcrParams ocrParams, bool email, EmailMessage emailMessage, bool uploadToDocuments, bool uploadToSquzeeze, Dictionary<string, string> squeezeFields)
        {
            ProgressTitle = email ? MiscResources.EmailPdfProgress : uploadToDocuments ? MiscResources.UploadPdfProgress : MiscResources.SavePdfProgress;
            Status = new OperationStatus
            {
                MaxProgress = images.Count
            };

            if (Directory.Exists(fileNamePlaceholders.SubstitutePlaceholders(fileName, dateTime, barcode: images.FirstOrDefault()?.Barcodes?.FirstOrDefault()?.Text)))
            {
                // Not supposed to be a directory, but ok...
                fileName = Path.Combine(fileName, "$(n).pdf");
            }

            var singleFile = !pdfSettings.SinglePagePdf || images.Count == 1;
            var subFileName = fileNamePlaceholders.SubstitutePlaceholders(fileName, dateTime, barcode: images.FirstOrDefault()?.Barcodes?.FirstOrDefault()?.Text);
            if (singleFile)
            {
                if (File.Exists(subFileName) && overwritePrompt.ConfirmOverwrite(subFileName) != DialogResult.Yes)
                {
                    return false;
                }
            }

            var snapshots = images.Select(x => x.Preserve()).ToList();
            var snapshotsByFile = pdfSettings.SinglePagePdf ? snapshots.Select(x => new[] { x }).ToArray() : new[] { snapshots.ToArray() };
            RunAsync(async () =>
            {
                bool result = false;
                
                // Save to fs
                try
                {
                    int digits = (int)Math.Floor(Math.Log10(snapshots.Count)) + 1;
                    int i = 0;
                    foreach (var snapshotArray in snapshotsByFile)
                    {
                        subFileName = fileNamePlaceholders.SubstitutePlaceholders(fileName, dateTime, true, i, singleFile ? 0 : digits);
                        Status.StatusText = string.Format(MiscResources.SavingFormat, Path.GetFileName(subFileName));
                        InvokeStatusChanged();
                        if (singleFile && IsFileInUse(subFileName, out var ex))
                        {
                            InvokeError(MiscResources.FileInUse, ex);
                            break;
                        }

                        var progress = singleFile ? OnProgress : (ProgressHandler)((j, k) => { });
                        result = await pdfExporter.Export(subFileName, snapshotArray, pdfSettings, ocrParams, progress, CancelToken);
                        if (!result || CancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        emailMessage?.Attachments.Add(new EmailAttachment
                        {
                            FilePath = subFileName,
                            AttachmentName = Path.GetFileName(subFileName)
                        });
                        if (i == 0)
                        {
                            FirstFileSaved = subFileName;
                        }
                        i++;
                        if (!singleFile)
                        {
                            OnProgress(i, snapshotsByFile.Length);
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    InvokeError(MiscResources.DontHavePermission, ex);
                }
                catch (IOException ex)
                {
                    if (File.Exists(subFileName))
                    {
                        InvokeError(MiscResources.FileInUse, ex);
                    }
                    else
                    {
                        Log.ErrorException(MiscResources.ErrorSaving, ex);
                        InvokeError(MiscResources.ErrorSaving, ex);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(MiscResources.ErrorSaving, ex);
                    InvokeError(MiscResources.ErrorSaving, ex);
                    result = false;
                }
                finally
                {
                    snapshots.ForEach(s => s.Dispose());
                    GC.Collect();
                }

                // Handle sending mail
                if (result && !CancelToken.IsCancellationRequested && email && emailMessage != null)
                {
                    Status.StatusText = MiscResources.UploadingEmail;
                    Status.CurrentProgress = 0;
                    Status.MaxProgress = 1;
                    Status.ProgressType = OperationProgressType.MB;
                    InvokeStatusChanged();

                    try
                    {
                        result = await emailProviderFactory.Default.SendEmail(emailMessage, OnProgress, CancelToken);
                    }
                    catch (OperationCanceledException)
                    {
                        result = false;
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException(MiscResources.ErrorEmailing, ex);
                        InvokeError(MiscResources.ErrorEmailing, ex);
                        result = false;
                    }
                }

                // Handle uploading to DOCUMENTS
                if (result && uploadToDocuments)
                {
                    Status.StatusText = MiscResources.UploadingDocuments;
                    Status.CurrentProgress = 0;
                    Status.MaxProgress = 1;
                    Status.ProgressType = OperationProgressType.MB;
                    InvokeStatusChanged();

                    try
                    {
                        result = await documentsUploader.Upload(subFileName, OnProgress, CancelToken);
                    }
                    catch (OperationCanceledException)
                    {
                        result = false;
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException(MiscResources.ErrorUploading, ex);
                        InvokeError(MiscResources.ErrorUploading, ex);
                        result = false;
                    }
                }

                // Handle uploading to SQUEEZE
                if (result && uploadToSquzeeze)
                {
                    Status.StatusText = MiscResources.UploadingSqueeze;
                    Status.CurrentProgress = 0;
                    Status.MaxProgress = 1;
                    Status.ProgressType = OperationProgressType.MB;
                    InvokeStatusChanged();

                    try
                    {
                        result = await squeezeUploader.Soap_CreateFile(subFileName, squeezeFields, OnProgress, CancelToken);
                    }
                    catch (OperationCanceledException ex)
                    {
                        result = false;
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException(MiscResources.ErrorUploading, ex);
                        InvokeError(MiscResources.ErrorUploading, ex);
                        result = false;
                    }
                }

                return result;
            });
            Success.ContinueWith(task =>
            {
                if (task.Result)
                {
                    if (email)
                    {
                        Log.Event(EventType.Email, new Event
                        {
                            Name = MiscResources.EmailPdf,
                            Pages = snapshots.Count,
                            FileFormat = ".pdf"
                        });
                    }
                    else
                    {
                        Log.Event(EventType.SavePdf, new Event
                        {
                            Name = MiscResources.SavePdf,
                            Pages = snapshots.Count,
                            FileFormat = ".pdf"
                        });
                    }
                }
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return true;
        }

        private bool IsFileInUse(string filePath, out Exception exception)
        {
            // TODO: Generalize this for images too
            exception = null;
            if (File.Exists(filePath))
            {
                try
                {
                    using (new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                    }
                }
                catch (IOException ex)
                {
                    exception = ex;
                    return true;
                }
            }
            return false;
        }
    }
}
