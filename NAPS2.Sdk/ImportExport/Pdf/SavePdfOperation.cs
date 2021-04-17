﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAPS2.Config;
using NAPS2.ImportExport.Email;
using NAPS2.Lang.Resources;
using NAPS2.Logging;
using NAPS2.Ocr;
using NAPS2.Operation;
using NAPS2.Images;
using NAPS2.Util;

namespace NAPS2.ImportExport.Pdf
{
    public class SavePdfOperation : OperationBase
    {
        private readonly PdfExporter _pdfExporter;
        private readonly OverwritePrompt _overwritePrompt;
        private readonly IEmailProviderFactory? _emailProviderFactory;

        public SavePdfOperation(PdfExporter pdfExporter, OverwritePrompt overwritePrompt, IEmailProviderFactory? emailProviderFactory = null)
        {
            _pdfExporter = pdfExporter;
            _overwritePrompt = overwritePrompt;
            _emailProviderFactory = emailProviderFactory;

            AllowCancel = true;
            AllowBackground = true;
        }

        public bool Start(string fileName, Placeholders placeholders, ICollection<ScannedImage> images, IConfigProvider<PdfSettings> pdfSettings, OcrContext ocrContext)
        {
            return Start(fileName, placeholders, images, pdfSettings, ocrContext, false, null);
        }

        public bool Start(string fileName, Placeholders placeholders, ICollection<ScannedImage> images, IConfigProvider<PdfSettings> pdfSettings, OcrContext ocrContext, bool email, EmailMessage? emailMessage)
        {
            ProgressTitle = email ? MiscResources.EmailPdfProgress : MiscResources.SavePdfProgress;
            var subFileName = placeholders.Substitute(fileName);
            Status = new OperationStatus
            {
                StatusText = string.Format(MiscResources.SavingFormat, Path.GetFileName(subFileName)),
                MaxProgress = images.Count
            };

            if (Directory.Exists(subFileName))
            {
                // Not supposed to be a directory, but ok...
                subFileName = placeholders.Substitute(Path.Combine(subFileName, "$(n).pdf"));
            }
            if (File.Exists(subFileName))
            {
                if (_overwritePrompt.ConfirmOverwrite(subFileName) != DialogResult.Yes)
                {
                    return false;
                }
            }

            var snapshots = images.Select(x => x.Preserve()).ToList();
            RunAsync(async () =>
            {
                bool result = false;
                try
                {
                    result = await _pdfExporter.Export(subFileName, snapshots, pdfSettings, ocrContext, OnProgress, CancelToken);
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
                }
                finally
                {
                    snapshots.ForEach(s => s.Dispose());
                    GC.Collect();
                }
                
                if (result && email && emailMessage != null && _emailProviderFactory != null)
                {
                    Status.StatusText = MiscResources.UploadingEmail;
                    Status.CurrentProgress = 0;
                    Status.MaxProgress = 1;
                    Status.ProgressType = OperationProgressType.MB;
                    InvokeStatusChanged();

                    try
                    {
                        result = await _emailProviderFactory.Default.SendEmail(emailMessage, OnProgress, CancelToken);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException(MiscResources.ErrorEmailing, ex);
                        InvokeError(MiscResources.ErrorEmailing, ex);
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
                        Log.Event(EventType.Email, new EventParams
                        {
                            Name = MiscResources.EmailPdf,
                            Pages = snapshots.Count,
                            FileFormat = ".pdf"
                        });
                    }
                    else
                    {
                        Log.Event(EventType.SavePdf, new EventParams
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
    }
}
