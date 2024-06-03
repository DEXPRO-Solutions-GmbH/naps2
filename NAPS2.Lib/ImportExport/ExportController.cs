using Eto.Forms;
using NAPS2.EtoForms;
using NAPS2.EtoForms.Notifications;
using NAPS2.EtoForms.Ui;
using NAPS2.ImportExport.Email;
using NAPS2.ImportExport.Images;
using NAPS2.Pdf;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Eto.Forms;

namespace NAPS2.ImportExport;

public class ExportController : IExportController
{
    private readonly DialogHelper _dialogHelper;
    private readonly IOperationFactory _operationFactory;
    private readonly IFormFactory _formFactory;
    private readonly OperationProgress _operationProgress;
    private readonly Naps2Config _config;
    private readonly UiImageList _imageList;

    public ExportController(DialogHelper dialogHelper, IOperationFactory operationFactory, IFormFactory formFactory,
        OperationProgress operationProgress, Naps2Config config, UiImageList imageList)
    {
        _dialogHelper = dialogHelper;
        _operationFactory = operationFactory;
        _formFactory = formFactory;
        _operationProgress = operationProgress;
        _config = config;
        _imageList = imageList;
    }

    public async Task<bool> SavePdf(ICollection<UiImage> uiImages, ISaveNotify notify)
    {
        using var images = GetSnapshots(uiImages);
        if (!images.Any())
        {
            return false;
        }

        string savePath;
        var defaultFileName = _config.Get(c => c.PdfSettings.DefaultFileName);
        if (_config.Get(c => c.PdfSettings.SkipSavePrompt) && Path.IsPathRooted(defaultFileName))
        {
            savePath = defaultFileName!;
        }
        else
        {
            if (!_dialogHelper.PromptToSavePdf(defaultFileName, out savePath!))
            {
                return false;
            }
        }

        if (await DoSavePdf(images, notify, savePath))
        {
            MaybeDeleteAfterSaving(uiImages);
            return true;
        }
        return false;
    }

    public async Task<bool> SaveImages(ICollection<UiImage> uiImages, ISaveNotify notify)
    {
        using var images = GetSnapshots(uiImages);
        if (!images.Any())
        {
            return false;
        }

        string savePath;
        var defaultFileName = _config.Get(c => c.ImageSettings.DefaultFileName);
        if (_config.Get(c => c.ImageSettings.SkipSavePrompt) &&
            Path.IsPathRooted(defaultFileName))
        {
            savePath = defaultFileName!;
        }
        else
        {
            if (!_dialogHelper.PromptToSaveImage(defaultFileName, out savePath!))
            {
                return false;
            }
        }

        if (await DoSaveImages(images, notify, savePath))
        {
            MaybeDeleteAfterSaving(uiImages);
            return true;
        }
        return false;
    }

    public async Task<bool> SavePdfOrImages(ICollection<UiImage> uiImages, ISaveNotify notify)
    {
        // Note this path bypasses some of the pdf/image save options (e.g. default file name)
        using var images = GetSnapshots(uiImages);

        string savePath;
        var pdfDefaultFileName = _config.Get(c => c.PdfSettings.DefaultFileName);
        var imageDefaultFileName = _config.Get(c => c.ImageSettings.DefaultFileName);
        if (_config.Get(c => c.PdfSettings.SkipSavePrompt) && Path.IsPathRooted(pdfDefaultFileName))
        {
            savePath = pdfDefaultFileName!;
        }
        else if (_config.Get(c => c.ImageSettings.SkipSavePrompt) && Path.IsPathRooted(imageDefaultFileName))
        {
            savePath = imageDefaultFileName!;
        }
        else
        {
            var defaultFileName = string.IsNullOrWhiteSpace(pdfDefaultFileName)
                ? imageDefaultFileName
                : pdfDefaultFileName;
            if (!_dialogHelper.PromptToSavePdfOrImage(defaultFileName, out savePath!))
            {
                return false;
            }
        }

        if (Path.GetExtension(savePath).ToLowerInvariant() == ".pdf"
                ? await DoSavePdf(images, notify, savePath)
                : await DoSaveImages(images, notify, savePath))
        {
            MaybeDeleteAfterSaving(uiImages);
            return true;
        }
        return false;
    }

    public async Task<bool> EmailPdf(ICollection<UiImage> uiImages)
    {
        using var images = GetSnapshots(uiImages);
        if (!images.Any())
        {
            return false;
        }

        if (!_config.User.Has(c => c.EmailSetup.ProviderType))
        {
            // First email attempt; prompt for a provider
            var form = _formFactory.Create<EmailProviderForm>();
            Invoker.Current.Invoke(() => form.ShowModal());
            if (!form.Result)
            {
                return false;
            }
        }

        var invalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());
        var attachmentName = new string(_config.Get(c => c.EmailSettings.AttachmentName)
            .Where(x => !invalidChars.Contains(x)).ToArray());
        if (string.IsNullOrEmpty(attachmentName))
        {
            attachmentName = "Scan.pdf";
        }
        if (!attachmentName.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
        {
            attachmentName += ".pdf";
        }
        attachmentName = Placeholders.All.Substitute(attachmentName, false);

        if (await DoEmailPdf(images, attachmentName))
        {
            MaybeDeleteAfterSaving(uiImages);
            return true;
        }
        return false;
    }

    private async Task<bool> DoSavePdf(IList<ProcessedImage> images, ISaveNotify notify, string savePath)
    {
        var subSavePath = Placeholders.All.Substitute(savePath);
        var state = _imageList.CurrentState;
        if (await RunSavePdfOperation(subSavePath, images, originalFilename: savePath))
        {
            _imageList.MarkSaved(state, images);
            notify.PdfSaved(subSavePath);
            return true;
        }
        return false;
    }

    private async Task<bool> DoSaveImages(IList<ProcessedImage> images, ISaveNotify notify, string savePath)
    {
        var op = _operationFactory.Create<SaveImagesOperation>();
        var state = _imageList.CurrentState;
        if (op.Start(savePath, Placeholders.All.WithDate(DateTime.Now), images, _config.Get(c => c.ImageSettings),
                savePath))
        {
            _operationProgress.ShowProgress(op);
        }
        if (await op.Success)
        {
            _imageList.MarkSaved(state, images);
            notify.ImagesSaved(images.Count, op.FirstFileSaved!);
            return true;
        }
        return false;
    }

    private async Task<bool> DoEmailPdf(IList<ProcessedImage> images, string attachmentName)
    {
        var tempFolder = new DirectoryInfo(Path.Combine(Paths.Temp, Path.GetRandomFileName()));
        tempFolder.Create();
        try
        {
            string targetPath = Path.Combine(tempFolder.FullName, attachmentName);
            var state = _imageList.CurrentState;

            if (await RunSavePdfOperation(targetPath, images, new EmailMessage()))
            {
                _imageList.MarkSaved(state, images);
                return true;
            }
        }
        finally
        {
            tempFolder.Delete(true);
        }
        return false;
    }

    private async Task<bool> RunSavePdfOperation(string filename, IList<ProcessedImage> images,
        EmailMessage? emailMessage = null, string? originalFilename = null)
    {
        var op = _operationFactory.Create<SavePdfOperation>();

        if (op.Start(filename, Placeholders.All.WithDate(DateTime.Now), images, _config.Get(c => c.PdfSettings),
                _config.DefaultOcrParams(), emailMessage, originalFilename ?? filename))
        {
            _operationProgress.ShowProgress(op);
        }
        return await op.Success;
    }

    private DisposableList<ProcessedImage> GetSnapshots(IEnumerable<UiImage> uiImages)
    {
        return uiImages.Select(x => x.GetClonedImage()).ToDisposableList();
    }

    private void MaybeDeleteAfterSaving(ICollection<UiImage> uiImages)
    {
        if (_config.Get(c => c.DeleteAfterSaving))
        {
            _imageList.Mutate(new ImageListMutation.DeleteSelected(), ListSelection.From(uiImages));
        }
    }
    //Squeeze upload
    public async Task<bool> UploadPdfToSqueeze(ICollection<UiImage> uiImages, ISaveNotify notify)
    {
        using var images = GetSnapshots(uiImages);
        //var imageCount = images.Count;
        if (!images.Any())
        {
            return false;
        }

        // Save the Pdf to a temporary file
        var tempFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
        tempFolder.Create();
        string tempPdfPath = Path.Combine(tempFolder.FullName, "naps2Output.pdf");

        try
        {
            if (!await DoSavePdf(images, notify, tempPdfPath))
            {
                return false;
            }

            // Upload the Pdf
            var url = _config.Get(c => c.SqueezeSettings.SQZURL);
            var client = _config.Get(c => c.SqueezeSettings.SQZClient);
            var userName = _config.Get(c => c.SqueezeSettings.SQZUserName);
            var password = _config.Get(c => c.SqueezeSettings.SQZPassword);
            var batchClassId = _config.Get(c => c.SqueezeSettings.SQZClassID);
            var (uploadSuccess, reasonPhrase) = await UploadPdfFile(tempPdfPath, url, userName, password, batchClassId);

            if (uploadSuccess)
            {
                MaybeDeleteAfterSaving(uiImages);
                notify.PdfUploaded();
                return true;
            }
            else
            {
                MessageBox.Show($"Upload failed: {reasonPhrase}", MessageBoxType.Error);
                return false;
            }
        }
        finally
        {
            if (File.Exists(tempPdfPath))
            {
                File.Delete(tempPdfPath);
            }
            tempFolder.Delete(true);
        }
    }
    // Squeeze Upload
    private async Task<(bool IsSuccess, string? ReasonPhrase)> UploadPdfFile(string filePath, string url, string username, string password, int batchClassId)
    {
        try
        {
            using var httpClient = new HttpClient();
            var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            using var form = new MultipartFormDataContent();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var fileContent = new StreamContent(fileStream);

            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            form.Add(fileContent, "file", Path.GetFileName(filePath));

            // Add additional field here
            form.Add(new StringContent(batchClassId.ToString()), "batchClassId");

            HttpResponseMessage response = await httpClient.PostAsync(url, form);

            return (response.IsSuccessStatusCode, response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, MessageBoxType.Error);
            return (false, ex.Message);
        }
    }
}