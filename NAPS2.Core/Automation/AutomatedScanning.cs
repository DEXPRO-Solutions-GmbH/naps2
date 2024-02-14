using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NAPS2.Barcode;
using NAPS2.Config;
using NAPS2.Dependencies;
using NAPS2.ImportExport;
using NAPS2.ImportExport.Email;
using NAPS2.ImportExport.Images;
using NAPS2.ImportExport.Pdf;
using NAPS2.Lang.ConsoleResources;
using NAPS2.Logging;
using NAPS2.Ocr;
using NAPS2.Operation;
using NAPS2.Scan;
using NAPS2.Scan.Images;
using NAPS2.Util;
using NAPS2.WinForms;

namespace NAPS2.Automation
{
    public class AutomatedScanning
    {
        private readonly IEmailProviderFactory emailProviderFactory;
        private readonly IProfileManager profileManager;
        private readonly IScanPerformer scanPerformer;
        private readonly IErrorOutput errorOutput;
        private readonly IScannedImageImporter scannedImageImporter;
        private readonly IUserConfigManager userConfigManager;
        private readonly PdfSettingsContainer pdfSettingsContainer;
        private readonly FileNamePlaceholders fileNamePlaceholders;
        private readonly ImageSettingsContainer imageSettingsContainer;
        private readonly IOperationFactory operationFactory;
        private readonly AppConfigManager appConfigManager;
        private readonly OcrManager ocrManager;
        private readonly IFormFactory formFactory;
        private readonly GhostscriptManager ghostscriptManager;
        private readonly string serviceExecutable;
        private readonly AutomatedScanningConfigManager automatedScanningConfigManager;
        private readonly BarcodeProcessor barcodeProcessor;
        private readonly AutomatedScanningOptions options;
        private List<List<ScannedImage>> scanList;
        private int pagesScanned;
        private int totalPagesScanned;
        private DateTime startTime;
        private List<string> actualImportPaths;
        private List<string> actualOutputPaths;
        private bool currentActionSuccessfullyCompleted;
        private OcrParams ocrParams;

        public AutomatedScanning(AutomatedScanningOptions options, string serviceExecutable, IProfileManager profileManager, IScanPerformer scanPerformer, IErrorOutput errorOutput, IEmailProviderFactory emailProviderFactory, IScannedImageImporter scannedImageImporter, IUserConfigManager userConfigManager, PdfSettingsContainer pdfSettingsContainer, FileNamePlaceholders fileNamePlaceholders, ImageSettingsContainer imageSettingsContainer, IOperationFactory operationFactory, AppConfigManager appConfigManager, OcrManager ocrManager, IFormFactory formFactory, GhostscriptManager ghostscriptManager, AutomatedScanningConfigManager automatedScanningConfigManager, BarcodeProcessor barcodeProcessor)
        {
            this.options = options;
            this.profileManager = profileManager;
            this.scanPerformer = scanPerformer;
            this.errorOutput = errorOutput;
            this.emailProviderFactory = emailProviderFactory;
            this.scannedImageImporter = scannedImageImporter;
            this.userConfigManager = userConfigManager;
            this.pdfSettingsContainer = pdfSettingsContainer;
            this.fileNamePlaceholders = fileNamePlaceholders;
            this.imageSettingsContainer = imageSettingsContainer;
            this.operationFactory = operationFactory;
            this.appConfigManager = appConfigManager;
            this.ocrManager = ocrManager;
            this.formFactory = formFactory;
            this.ghostscriptManager = ghostscriptManager;
            this.serviceExecutable = serviceExecutable;
            this.automatedScanningConfigManager = automatedScanningConfigManager;
            this.barcodeProcessor = barcodeProcessor;
        }

        public IEnumerable<ScannedImage> AllImages => scanList.SelectMany(x => x);

        private void OutputVerbose(string value, params object[] args)
        {
            if (options.Verbose)
            {
                Console.WriteLine(value, args);
            }
        }

        public async Task Execute()
        {
            if (!ValidateOptions())
            {
                return;
            }

            startTime = DateTime.Now;
            ConsoleOverwritePrompt.ForceOverwrite = options.ForceOverwrite;

            if (DoServiceInstall())
                return;

            if (options.Install != null)
            {
                InstallComponents();
                if (options.OutputPath == null && options.EmailFileName == null && !options.AutoSave)
                {
                    return;
                }
            }

            do
            {
                if (!string.IsNullOrEmpty(options.FolderPath))
                {
                    await ExecuteFolder();
                }
                else
                {
                    await ExecuteSingle();
                }
                if(options.ContinuousMode)
                {
                    Thread.Sleep(options.Delay > -1 ? options.Delay : 5000);
                }
            } while (options.ContinuousMode);
        }

        private bool DoServiceInstall()
        {
            var res = false;
            var install = options.ServiceInstall;
            var uninstall = options.ServiceUninstall;

            if (uninstall)
            {
                ManagedInstallerClass.InstallHelper(new[] { "/u", serviceExecutable });
                Console.WriteLine("Service uninstalled!");
                res = true;
            }

            if (install)
            {
                options.ServiceInstall = false;
                options.ServiceUninstall = false;
                options.Verbose = false;
                automatedScanningConfigManager.Options = options;
                automatedScanningConfigManager.Save();
                Console.WriteLine("Service config updated.");
                try
                {
                    ManagedInstallerClass.InstallHelper(new[] { serviceExecutable });
                    Console.WriteLine("Service installed!");
                }
                catch (Exception e)
                {
                    Log.FatalException("An error occurred.", e);
                    errorOutput.DisplayError("Service not installed!");
                }
                res = true;
            }

            return res;
        }

        private async Task ExecuteFolder()
        {
            var folderName = fileNamePlaceholders.SubstitutePlaceholders(options.FolderPath, startTime, false);
            if (!Directory.Exists(folderName))
            {
                return;
            }

            var files = Directory.GetFiles(folderName, options.FilePattern ?? "*", SearchOption.AllDirectories);
            if (files.Length < 0)
            {
                return;
            }

            if (options.MergeFolder)
            {
                options.ImportPath = string.Join(";", files);
                await ExecuteSingle();
            }
            else
            {
                foreach (var file in files)
                {
                    //Check for file availability... this can change until we try to read it, 
                    //but at least we can skip the file if it is already locked
                    if (IsFileAvailable(file))
                    {
                        options.ImportPath = file;
                        await ExecuteSingle();
                    }
                }
            }
        }

        private bool IsFileAvailable(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        private async Task ExecuteSingle()
        {
            currentActionSuccessfullyCompleted = false;
            try
            {
                /*if (!PreCheckOverwriteFile())
                {
                    return;
                }*/

                scanList = new List<List<ScannedImage>>();
                actualImportPaths = new List<string>();

                if (options.ImportPath != null)
                {
                    await ImportImages();
                }

                ConfigureOcr();

                if (options.Number > 0)
                {
                    if (!GetProfile(out ScanProfile profile))
                    {
                        return;
                    }

                    await PerformScan(profile);
                }

                ReorderScannedImages();

                var allOperationsSuccessfull = true;

                if (options.OutputPath != null)
                {
                    if (!await ExportScannedImages())
                        allOperationsSuccessfull = false;
                }

                if (options.EmailFileName != null)
                {
                    if (!await EmailScannedImages())
                        allOperationsSuccessfull = false;
                }

                if (options.DocumentsFileName != null)
                {
                    if (!await UploadScannedImages())
                        allOperationsSuccessfull = false;
                }

                currentActionSuccessfullyCompleted = allOperationsSuccessfull;

                foreach (var image in AllImages)
                {
                    image.Dispose();
                }

            }
            catch (Exception ex)
            {
                Log.FatalException("An error occurred.", ex);
                errorOutput.DisplayError(ConsoleResources.UnexpectedError);
            }
            finally
            {
                DoMoveOrDelete(currentActionSuccessfullyCompleted ? options.SuccessMove : options.ErrorMove, currentActionSuccessfullyCompleted ? options.SuccessDelete : options.ErrorDelete, actualImportPaths.ToArray());

                if (options.WaitForEnter)
                {
                    Console.ReadLine();
                }
            }
        }

        private void DoMoveOrDelete(string movePath, bool doDelete, params string[] filePaths)
        {
            try
            {
                var doMove = !string.IsNullOrEmpty(movePath);
                if (doDelete || doMove)
                {
                    if (doMove)
                    {
                        movePath = fileNamePlaceholders.SubstitutePlaceholders(movePath, startTime, false);
                        Directory.CreateDirectory(movePath);
                    }
                    foreach (var filePath in filePaths)
                    {
                        try
                        {
                            if (doMove)
                            {
                                var newFile = Path.Combine(movePath, Path.GetFileName(filePath));
                                var i = 1;
                                while (File.Exists(newFile))
                                    newFile = Path.Combine(movePath, Path.GetFileNameWithoutExtension(filePath) + "." + i++ + Path.GetExtension(filePath));
                                File.Move(filePath, newFile);
                            }
                            else if (doDelete)
                            {
                                File.Delete(filePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.FatalException("Error while performing move/delete of file " + filePath + ".", ex);
                            errorOutput.DisplayError("Error while performing move/delete of file " + filePath + ".");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.FatalException("Error while performing move/delete.", ex);
                errorOutput.DisplayError(ConsoleResources.UnexpectedError);
            }
        }

        private void ConfigureOcr()
        {
            bool canUseOcr = IsPdfFile(options.OutputPath) || IsPdfFile(options.EmailFileName);
            bool useOcr = canUseOcr && !options.DisableOcr && (options.EnableOcr || options.OcrLang != null || userConfigManager.Config.EnableOcr || appConfigManager.Config.OcrState == OcrState.Enabled);
            string ocrLanguageCode = useOcr ? (options.OcrLang ?? ocrManager.DefaultParams?.LanguageCode) : null;
            ocrParams = new OcrParams(ocrLanguageCode, ocrManager.DefaultParams?.Mode ?? OcrMode.Default);
        }

        private void InstallComponents()
        {
            var availableComponents = new List<IExternalComponent>();
            var ocrEngine = ocrManager.EngineToInstall;
            if (ocrEngine != null)
            {
                availableComponents.Add(ocrEngine.Component);
                availableComponents.AddRange(ocrEngine.LanguageComponents);
            }
            if (ghostscriptManager.IsSupported)
            {
                availableComponents.Add(ghostscriptManager.GhostscriptComponent);
            }

            var componentDict = availableComponents.ToDictionary(x => x.Id.ToLowerInvariant());
            var installId = options.Install.ToLowerInvariant();
            if (!componentDict.TryGetValue(installId, out var toInstall))
            {
                errorOutput.DisplayError(ConsoleResources.ComponentNotAvailable);
                return;
            }
            if (toInstall.IsInstalled)
            {
                errorOutput.DisplayError(ConsoleResources.ComponentAlreadyInstalled);
                return;
            }
            // Using a form here is not ideal (since this is supposed to be a console app), but good enough for now
            // Especially considering wia/twain often show forms anyway
            var progressForm = formFactory.Create<FDownloadProgress>();
            if (toInstall.Id.StartsWith("ocr-", StringComparison.InvariantCulture) && componentDict.TryGetValue("ocr", out var ocrExe) && !ocrExe.IsInstalled)
            {
                progressForm.QueueFile(ocrExe);
                OutputVerbose(ConsoleResources.Installing, ocrExe.Id);
            }
            progressForm.QueueFile(toInstall);
            OutputVerbose(ConsoleResources.Installing, toInstall.Id);
            progressForm.ShowDialog();
        }

        private void ReorderScannedImages()
        {
            var sep = options.SplitPatchT ? SaveSeparator.PatchT
                : options.SplitBarcode ? SaveSeparator.Barcode
                : options.SplitScans ? SaveSeparator.FilePerScan
                : options.SplitSize > 0 || options.Split ? SaveSeparator.FilePerPage
                : SaveSeparator.None;
            scanList = SaveSeparatorHelper.SeparateScans(scanList, sep, options.SplitSize).Where(x => x.Count > 0).ToList();

            foreach (var scan in scanList)
            {
                var imageList = new ScannedImageList(scan);
                var e = new List<int>();

                if (options.AltDeinterleave)
                {
                    imageList.AltDeinterleave(e);
                }
                else if (options.Deinterleave)
                {
                    imageList.Deinterleave(e);
                }
                else if (options.AltInterleave)
                {
                    imageList.AltInterleave(e);
                }
                else if (options.Interleave)
                {
                    imageList.Interleave(e);
                }

                if (options.Reverse)
                {
                    imageList.Reverse(e);
                }
            }
        }

        private bool PreCheckOverwriteFile()
        {
            if (options.OutputPath == null)
            {
                // Email, so no check needed
                return true;
            }
            var subPath = fileNamePlaceholders.SubstitutePlaceholders(options.OutputPath, startTime);
            if (IsPdfFile(subPath)
                && File.Exists(subPath)
                && !options.ForceOverwrite)
            {
                errorOutput.DisplayError(string.Format(ConsoleResources.FileAlreadyExists, Path.GetFullPath(subPath)));
                return false;
            }
            return true;
        }

        private async Task ImportImages()
        {
            OutputVerbose(ConsoleResources.Importing);

            ConsolePdfPasswordProvider.PasswordToProvide = options.ImportPassword;

            var filePaths = options.ImportPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            actualImportPaths.Clear();
            int i = 0;
            barcodeProcessor.NewBatch();
            foreach (var filePath in filePaths)
            {
                i++;
                try
                {
                    var importParams = new ImportParams
                    {
                        Slice = Slice.Parse(filePath, out string actualPath),
                        DetectPatchCodes = options.SplitPatchT,
                        DetectBarcodes = GetBarcodeDetectionMode(),
                        NoThumbnails = true
                    };
                    var images = await scannedImageImporter.Import(actualPath, importParams, (j, k) => { }, CancellationToken.None).ToList();
                    if (images.Count > 0)
                    {
                        actualImportPaths.Add(actualPath);
                        scanList.Add(images);
                    }
                    else
                    {
                        Log.Error(string.Format("No pages found while importing {0}", actualPath));
                        errorOutput.DisplayError(string.Format("No pages found while importing {0}", actualPath));
                        DoMoveOrDelete(options.ErrorMove, options.ErrorDelete, filePath);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(string.Format(ConsoleResources.ErrorImporting, filePath), ex);
                    errorOutput.DisplayError(string.Format(ConsoleResources.ErrorImporting, filePath));
                    DoMoveOrDelete(options.ErrorMove, options.ErrorDelete, filePath);
                    continue;
                }
                OutputVerbose(ConsoleResources.ImportedFile, i, filePaths.Length);
            }
        }

        private async Task<bool> UploadScannedImages()
        {
            if (scanList.Count == 0)
            {
                errorOutput.DisplayError(ConsoleResources.NoPagesToExport);
                return false;
            }

            OutputVerbose(ConsoleResources.Exporting);

            var tempFolder = new DirectoryInfo(Path.Combine(Paths.Temp, Path.GetRandomFileName()));
            tempFolder.Create();
            try
            {
                string targetPath = Path.Combine(tempFolder.FullName, options.DocumentsFileName);
                if (IsPdfFile(targetPath))
                {
                    //Upload pdf to documents
                    OutputVerbose(ConsoleResources.ExportingPDFToAttach);
                    return await DoExportToPdf(targetPath, false, true, false); // TODO: Support Squeeze
                }
                else
                {
                    throw new NotImplementedException("Uploading images to documents is not supported yet");
                }
            }
            finally
            {
                tempFolder.Delete(true);
            }

        }

        private async Task<bool> EmailScannedImages()
        {
            if (scanList.Count == 0)
            {
                errorOutput.DisplayError(ConsoleResources.NoPagesToEmail);
                return false;
            }


            OutputVerbose(ConsoleResources.Emailing);

            var message = new EmailMessage
            {
                Subject = fileNamePlaceholders.SubstitutePlaceholders(options.EmailSubject, startTime, false, barcode: scanList?.FirstOrDefault()?.FirstOrDefault()?.Barcodes?.FirstOrDefault()?.Text) ?? "",
                BodyText = fileNamePlaceholders.SubstitutePlaceholders(options.EmailBody, startTime, false, barcode: scanList?.FirstOrDefault()?.FirstOrDefault()?.Barcodes?.FirstOrDefault()?.Text),
                AutoSend = options.EmailAutoSend,
                SilentSend = options.EmailSilentSend
            };

            message.Recipients.AddRange(EmailRecipient.FromText(EmailRecipientType.To, options.EmailTo));
            message.Recipients.AddRange(EmailRecipient.FromText(EmailRecipientType.Cc, options.EmailCc));
            message.Recipients.AddRange(EmailRecipient.FromText(EmailRecipientType.Bcc, options.EmailBcc));

            var tempFolder = new DirectoryInfo(Path.Combine(Paths.Temp, Path.GetRandomFileName()));
            tempFolder.Create();
            try
            {
                string targetPath = Path.Combine(tempFolder.FullName, options.EmailFileName);
                if (IsPdfFile(targetPath))
                {
                    if (options.OutputPath != null && IsPdfFile(options.OutputPath))
                    {
                        // The scan has already been exported to PDF, so use that file
                        OutputVerbose(ConsoleResources.AttachingExportedPDF);
                        int digits = (int)Math.Floor(Math.Log10(scanList.Count)) + 1;
                        int i = 0;
                        foreach (var path in actualOutputPaths)
                        {
                            string attachmentName = fileNamePlaceholders.SubstitutePlaceholders(options.EmailFileName, startTime, false, i, scanList.Count > 1 ? digits : 0, barcode: scanList?[i]?.FirstOrDefault()?.Barcodes?.FirstOrDefault()?.Text);
                            message.Attachments.Add(new EmailAttachment
                            {
                                FilePath = path,
                                AttachmentName = attachmentName
                            });
                            i++;
                        }
                    }
                    else
                    {
                        // The scan hasn't bee exported to PDF yet, so it needs to be exported to the temp folder
                        OutputVerbose(ConsoleResources.ExportingPDFToAttach);
                        if (!await DoExportToPdf(targetPath, true, false, false))
                        {
                            OutputVerbose(ConsoleResources.EmailNotSent);
                            return false;
                        }
                        // Attach the PDF file
                        AttachFilesInFolder(tempFolder, message);
                    }
                }
                else
                {
                    // Export the images to the temp folder
                    // Don't bother to re-use previously exported images, because the possible different formats and multiple files makes it non-trivial,
                    // and exporting is pretty cheap anyway
                    OutputVerbose(ConsoleResources.ExportingImagesToAttach);
                    if (!await DoExportToImageFiles(targetPath))
                    {
                        OutputVerbose(ConsoleResources.EmailNotSent);
                        return false;
                    }
                    // Attach the image file(s)
                    AttachFilesInFolder(tempFolder, message);
                }

                OutputVerbose(ConsoleResources.SendingEmail);
                if (await emailProviderFactory.Default.SendEmail(message, (j, k) => { }, CancellationToken.None))
                {
                    OutputVerbose(ConsoleResources.EmailSent);
                    return true;
                }
                else
                {
                    OutputVerbose(ConsoleResources.EmailNotSent);
                    return false;
                }
            }
            finally
            {
                tempFolder.Delete(true);
            }
        }

        private void AttachFilesInFolder(DirectoryInfo folder, EmailMessage message)
        {
            foreach (var file in folder.EnumerateFiles())
            {
                OutputVerbose(ConsoleResources.Attaching, file.Name);
                message.Attachments.Add(new EmailAttachment
                {
                    FilePath = file.FullName,
                    AttachmentName = file.Name
                });
            }
        }

        public bool ValidateOptions()
        {
            // Most validation is done by the CommandLineParser library, but some constraints that can't be represented by that API need to be checked here
            if (options.OutputPath == null && options.EmailFileName == null && options.DocumentsFileName == null && options.Install == null && !options.AutoSave)
            {
                errorOutput.DisplayError(ConsoleResources.OutputOrEmailRequired);
                return false;
            }
            if (options.OutputPath == null && options.EmailFileName == null && options.DocumentsFileName == null && options.ImportPath != null)
            {
                errorOutput.DisplayError(ConsoleResources.OutputOrEmailRequiredForImport);
                return false;
            }
            return true;
        }

        private async Task<bool> ExportScannedImages()
        {
            if (scanList.Count == 0)
            {
                errorOutput.DisplayError(ConsoleResources.NoPagesToExport);
                return false;
            }

            OutputVerbose(ConsoleResources.Exporting);

            if (IsPdfFile(options.OutputPath))
            {
                return await ExportToPdf();
            }
            else
            {
                return await ExportToImageFiles();
            }
        }

        private bool IsPdfFile(string path)
        {
            if (path == null) return false;
            string extension = Path.GetExtension(path);
            Debug.Assert(extension != null);
            return extension.ToLower() == ".pdf";
        }

        private async Task<bool> ExportToImageFiles()
        {
            var path = fileNamePlaceholders.SubstitutePlaceholders(options.OutputPath, startTime);
            var res = await DoExportToImageFiles(options.OutputPath);
            OutputVerbose(ConsoleResources.FinishedSavingImages, Path.GetFullPath(path));
            return res;
        }

        private async Task<bool> DoExportToImageFiles(string outputPath)
        {
            // TODO: If I add new image settings this may break things
            imageSettingsContainer.ImageSettings = new ImageSettings
            {
                JpegQuality = options.JpegQuality,
                TiffCompression = Enum.TryParse<TiffCompression>(options.TiffComp, true, out var tc) ? tc : TiffCompression.Auto
            };

            foreach (var scan in scanList)
            {
                var op = operationFactory.Create<SaveImagesOperation>();
                if(options.SplitBarcode)
                {
                    BarcodeResult[] prevBarcodes = null;
                    foreach(var img in scan)
                    {
                        if (img.Barcodes == null || img.Barcodes.Length == 0)
                            img.Barcodes = prevBarcodes;
                        else
                            prevBarcodes = img.Barcodes;
                    }
                }
                int i = -1;
                op.StatusChanged += (sender, args) =>
                {
                    if (op.Status.CurrentProgress > i)
                    {
                        OutputVerbose(ConsoleResources.ExportingImage, op.Status.CurrentProgress + 1, scan.Count);
                        i = op.Status.CurrentProgress;
                    }
                };
                op.Start(outputPath, startTime, scan);
                if (!await op.Success)
                    return false;
            }
            return true;
        }

        private async Task<bool> ExportToPdf()
        {
            return await DoExportToPdf(options.OutputPath, false, false, false);
        }

        private async Task<bool> DoExportToPdf(string path, bool email, bool uploadToDocuments, bool uploadToSqueeze)
        {
            var metadata = options.UseSavedMetadata ? pdfSettingsContainer.PdfSettings.Metadata : new PdfMetadata();
            metadata.Creator = ConsoleResources.NAPS2;
            if (options.PdfTitle != null)
            {
                metadata.Title = options.PdfTitle;
            }
            if (options.PdfAuthor != null)
            {
                metadata.Author = options.PdfAuthor;
            }
            if (options.PdfSubject != null)
            {
                metadata.Subject = options.PdfSubject;
            }
            if (options.PdfKeywords != null)
            {
                metadata.Keywords = options.PdfKeywords;
            }

            var encryption = options.UseSavedEncryptConfig ? pdfSettingsContainer.PdfSettings.Encryption : new PdfEncryption();
            if (options.EncryptConfig != null)
            {
                try
                {
                    using (Stream configFileStream = File.OpenRead(options.EncryptConfig))
                    {
                        var serializer = new XmlSerializer(typeof(PdfEncryption));
                        encryption = (PdfEncryption)serializer.Deserialize(configFileStream);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ConsoleResources.CouldntLoadEncryptionConfig, ex);
                    errorOutput.DisplayError(ConsoleResources.CouldntLoadEncryptionConfig);
                }
            }

            var compat = PdfCompat.Default;
            if (options.PdfCompat != null)
            {
                var t = options.PdfCompat.Replace(" ", "").Replace("-", "");
                if (t.EndsWith("a1b", StringComparison.InvariantCultureIgnoreCase))
                {
                    compat = PdfCompat.PdfA1B;
                }
                else if (t.EndsWith("a2b", StringComparison.InvariantCultureIgnoreCase))
                {
                    compat = PdfCompat.PdfA2B;
                }
                else if (t.EndsWith("a3b", StringComparison.InvariantCultureIgnoreCase))
                {
                    compat = PdfCompat.PdfA3B;
                }
                else if (t.EndsWith("a3u", StringComparison.InvariantCultureIgnoreCase))
                {
                    compat = PdfCompat.PdfA3U;
                }
            }

            var pdfSettings = new PdfSettings { Metadata = metadata, Encryption = encryption, Compat = compat };

            int scanIndex = 0;
            actualOutputPaths = new List<string>();
            foreach (var fileContents in scanList)
            {
                var op = operationFactory.Create<SavePdfOperation>();
                int i = -1;
                op.StatusChanged += (sender, args) =>
                {
                    if (op.Status.CurrentProgress > i)
                    {
                        OutputVerbose(ConsoleResources.ExportingPage, op.Status.CurrentProgress + 1, fileContents.Count);
                        i = op.Status.CurrentProgress;
                    }
                };
                int digits = (int)Math.Floor(Math.Log10(scanList.Count)) + 1;
                string actualPath = fileNamePlaceholders.SubstitutePlaceholders(path, startTime, true, scanIndex++, scanList.Count > 1 ? digits : 0, barcode: fileContents?.FirstOrDefault()?.Barcodes?.FirstOrDefault()?.Text);
                op.Start(actualPath, startTime, fileContents, pdfSettings, ocrParams, email, null, uploadToDocuments, uploadToSqueeze);
                if (!await op.Success)
                {
                    return false;
                }
                actualOutputPaths.Add(actualPath);
                if (!email)
                {
                    OutputVerbose(ConsoleResources.SuccessfullySavedPdf, actualPath);
                }
            }
            return true;
        }

        private async Task PerformScan(ScanProfile profile)
        {
            OutputVerbose(ConsoleResources.BeginningScan);

            bool autoSaveEnabled = !appConfigManager.Config.DisableAutoSave && profile.EnableAutoSave && profile.AutoSaveSettings != null;
            if (options.AutoSave && !autoSaveEnabled)
            {
                errorOutput.DisplayError(ConsoleResources.AutoSaveNotEnabled);
                if (options.OutputPath == null && options.EmailFileName == null)
                {
                    return;
                }
            }

            totalPagesScanned = 0;
            foreach (int i in Enumerable.Range(1, options.Number))
            {
                if (options.Delay > 0)
                {
                    OutputVerbose(ConsoleResources.Waiting, options.Delay);
                    Thread.Sleep(options.Delay);
                }
                OutputVerbose(ConsoleResources.StartingScan, i, options.Number);
                pagesScanned = 0;
                scanList.Add(new List<ScannedImage>());
                var scanParams = new ScanParams
                {
                    NoUI = !options.Progress,
                    NoAutoSave = !options.AutoSave,
                    NoThumbnails = true,
                    DetectPatchCodes = options.SplitPatchT,
                    DetectBarcodes = GetBarcodeDetectionMode(),
                    DoOcr = ocrParams?.LanguageCode != null,
                    OcrParams = ocrParams
                };
                await scanPerformer.PerformScan(profile, scanParams, null, null, ReceiveScannedImage);
                OutputVerbose(ConsoleResources.PagesScanned, pagesScanned);
            }
        }

        private BarcodeDetectionMode GetBarcodeDetectionMode()
        {
            if (!options.SplitBarcode)
                return BarcodeDetectionMode.None;
            if (!options.RemoveBarcodePage)
                return BarcodeDetectionMode.Detect;
            return BarcodeDetectionMode.DetectAndRemovePage;
        }

        private bool GetProfile(out ScanProfile profile)
        {
            try
            {
                if (options.ProfileName == null)
                {
                    // If no profile is specified, use the default (if there is one)
                    profile = profileManager.Profiles.Single(x => x.IsDefault);
                }
                else
                {
                    // Use the profile with the specified name (try case-sensitive first, then case-insensitive)
                    profile = profileManager.Profiles.FirstOrDefault(x => x.DisplayName == options.ProfileName) ??
                              profileManager.Profiles.First(x => x.DisplayName.ToLower() == options.ProfileName.ToLower());
                }
            }
            catch (InvalidOperationException)
            {
                errorOutput.DisplayError(ConsoleResources.ProfileUnavailableOrAmbiguous);
                profile = null;
                return false;
            }
            return true;
        }

        public void ReceiveScannedImage(ScannedImage scannedImage)
        {
            scanList.Last().Add(scannedImage);
            pagesScanned++;
            totalPagesScanned++;
            OutputVerbose(ConsoleResources.ScannedPage, totalPagesScanned);
        }
    }
}
