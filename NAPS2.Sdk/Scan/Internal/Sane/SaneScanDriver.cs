﻿using System.Threading;
using Microsoft.Extensions.Logging;
using NAPS2.Images.Bitwise;
using NAPS2.Scan.Exceptions;
using NAPS2.Scan.Internal.Sane.Native;

namespace NAPS2.Scan.Internal.Sane;

internal class SaneScanDriver : IScanDriver
{
    private static string? _customConfigDir;
    
    private readonly ScanningContext _scanningContext;

    public SaneScanDriver(ScanningContext scanningContext)
    {
        _scanningContext = scanningContext;

#if NET6_0_OR_GREATER
        Installation = OperatingSystem.IsMacOS()
            ? new BundledSaneInstallation()
            : File.Exists("/.flatpak-info")
                ? new FlatpakSaneInstallation()
                : new SystemSaneInstallation();
        if (_customConfigDir == null && !OperatingSystem.IsWindows())
        {
            // SANE caches the SANE_CONFIG_DIR environment variable process-wide, which means that we can't willy-nilly
            // change the config dir. However, if we use a static directory name and only create the actual directory
            // when we want to use it, SANE will (without caching) use the directory when it exists, and fall back to
            // the default config dir otherwise.
            _customConfigDir = Path.Combine(_scanningContext.TempFolderPath, Path.GetRandomFileName());
            Installation.SetCustomConfigDir(_customConfigDir);
        }
#else
        Installation = null!;
#endif
    }

    private ISaneInstallation Installation { get; }

    public Task GetDevices(ScanOptions options, CancellationToken cancelToken, Action<ScanDevice> callback)
    {
        var localIPsTask = options.ExcludeLocalIPs ? LocalIPsHelper.Get() : null;

        void MaybeCallback(SaneDeviceInfo device)
        {
            if (options.ExcludeLocalIPs && GetIP(device) is { } ip && localIPsTask!.Result.Contains(ip))
            {
                return;
            }
            callback(GetScanDevice(device));
        }

        return Task.Run(() =>
        {
            // TODO: This is crashing after a delay for no apparent reason.
            // That's okay because we're in a worker process, but ideally we could fix it in SANE.
            using var client = new SaneClient(Installation);
            // TODO: We can use device.type and .vendor to help pick an icon etc.
            // https://sane-project.gitlab.io/standard/api.html#device-descriptor-type
            if (Installation.CanStreamDevices)
            {
                client.StreamDevices(MaybeCallback, cancelToken);
            }
            else
            {
                foreach (var device in client.GetDevices())
                {
                    MaybeCallback(device);
                }
            }
        });
    }

    private static ScanDevice GetScanDevice(SaneDeviceInfo device) =>
        new(Driver.Sane, device.Name, GetName(device));

    private static string GetName(SaneDeviceInfo device)
    {
        var backend = GetBackend(device.Name);
        // Special cases for sane-escl and sane-airscan.
        if (backend == "escl")
        {
            // We include the vendor as it's excluded from the model, and we include the full name instead of
            // just the backend as that has the IP address.
            return $"{device.Vendor} {device.Model} ({device.Name})";
        }
        if (backend == "airscan")
        {
            // We include the device type which has the IP address.
            return $"{device.Model} ({backend}:{device.Type})";
        }
        return $"{device.Model} ({backend})";
    }

    private string? GetIP(SaneDeviceInfo device)
    {
        var backend = GetBackend(device.Name);
        if (backend == "escl")
        {
            // Name is in the form "escl:http://xx.xx.xx.xx:yy"
            var uri = new Uri(device.Name.Substring(device.Name.IndexOf(":", StringComparison.InvariantCulture) + 1));
            return uri.Host;
        }
        if (backend == "airscan")
        {
            // Type is in the form "ip=xx.xx.xx.xx"
            return device.Type.Substring(3);
        }
        return null;
    }

    private static string GetBackend(string saneDeviceName)
    {
        return saneDeviceName.Split(':')[0];
    }

    public Task Scan(ScanOptions options, CancellationToken cancelToken, IScanEvents scanEvents,
        Action<IMemoryImage> callback)
    {
        return Task.Run(() =>
        {
            try
            {
                ScanWithSaneDevice(options, cancelToken, scanEvents, callback, options.Device!.ID);
            }
            catch (DeviceOfflineException)
            {
                // Some SANE backends (e.g. airscan, genesys) have inconsistent IDs so "device offline" might actually
                // just mean "device id has changed". We can query for a "backup" device that matches the name of the
                // original device, and assume it's the same physical device, which should generally be correct.
                string? backupDeviceId = QueryForBackupSaneDevice(options.Device!);
                if (backupDeviceId == null)
                {
                    throw;
                }
                ScanWithSaneDevice(options, cancelToken, scanEvents, callback, backupDeviceId);
            }
        });
    }

    void ScanWithSaneDevice(ScanOptions options, CancellationToken cancelToken, IScanEvents scanEvents,
        Action<IMemoryImage> callback, string deviceId)
    {
        bool hasAtLeastOneImage = false;
        try
        {
            using var client = new SaneClient(Installation);
            if (cancelToken.IsCancellationRequested) return;
            _scanningContext.Logger.LogDebug("Opening SANE Device \"{ID}\"", deviceId);
            using var device = client.OpenDevice(deviceId);
            if (cancelToken.IsCancellationRequested) return;
            var optionData = SetOptions(device, options);
            var cancelOnce = new Once(device.Cancel);
            cancelToken.Register(cancelOnce.Run);
            try
            {
                if (!optionData.IsFeeder)
                {
                    var image = ScanPage(device, scanEvents, optionData) ??
                                throw new DeviceException("SANE expected image");
                    callback(image);
                }
                else
                {
                    while (ScanPage(device, scanEvents, optionData) is { } image)
                    {
                        hasAtLeastOneImage = true;
                        callback(image);
                    }
                }
            }
            finally
            {
                cancelOnce.Run();
            }
        }
        catch (SaneException ex)
        {
            switch (ex.Status)
            {
                case SaneStatus.Good:
                case SaneStatus.Cancelled:
                    return;
                case SaneStatus.NoDocs:
                    if (!hasAtLeastOneImage)
                    {
                        throw new DeviceFeederEmptyException();
                    }

                    break;
                case SaneStatus.DeviceBusy:
                    throw new DeviceBusyException();
                case SaneStatus.Invalid:
                    // TODO: Maybe not always correct? e.g. when setting options
                    throw new DeviceOfflineException();
                case SaneStatus.Jammed:
                    throw new DevicePaperJamException();
                case SaneStatus.CoverOpen:
                    throw new DeviceCoverOpenException();
                default:
                    throw new DeviceException($"SANE error: {ex.Status}");
            }
        }
    }

    private string? QueryForBackupSaneDevice(ScanDevice device)
    {
        // If we couldn't get an ID match, we can call GetDevices again and see if we can find a name match.
        // This can be very slow (10+ seconds) if we have to query every single backend (the normal SANE behavior for
        // GetDevices). We can hack this by creating a temporary SANE config dir that only references the single backend
        // we need, so it ends up being only ~1s.
        _scanningContext.Logger.LogDebug(
            "SANE Device appears offline; re-querying in case of ID change for name \"{Name}\"", device.Name);
        string? tempConfigDir = MaybeCreateTempConfigDirForSingleBackend(GetBackend(device.ID));
        try
        {
            using var client = new SaneClient(Installation);
            var backupDevice = client.GetDevices()
                .FirstOrDefault(deviceInfo => GetName(deviceInfo) == device.Name);
            if (backupDevice.Name == null)
            {
                _scanningContext.Logger.LogDebug("No matching device found");
                return null;
            }
            return backupDevice.Name;
        }
        finally
        {
            if (tempConfigDir != null)
            {
                try
                {
                    Directory.Delete(tempConfigDir, true);
                }
                catch (Exception ex)
                {
                    _scanningContext.Logger.LogDebug(ex, "Error cleaning up temp SANE config dir");
                }
            }
        }
    }

    private string? MaybeCreateTempConfigDirForSingleBackend(string backendName)
    {
        if (!Directory.Exists(Installation.DefaultConfigDir) || _customConfigDir == null)
        {
            // Non-typical SANE installation where we don't know the config dir and can't do this optimization
            return null;
        }
        // SANE normally doesn't provide a way to only query a single backend - it's all or nothing.
        // However, there is a workaround - if we use the SANE_CONFIG_DIR environment variable, we can specify a custom
        // config dir, which can have a dll.conf file that only has a single backend specified.
        Directory.CreateDirectory(_customConfigDir);
        // Copy the backend.conf file in case there's any important backend-specific configuration
        var backendConfFile = $"{backendName}.conf";
        if (File.Exists(Path.Combine(Installation.DefaultConfigDir, backendConfFile)))
        {
            File.Copy(
                Path.Combine(Installation.DefaultConfigDir, backendConfFile),
                Path.Combine(_customConfigDir, backendConfFile));
        }
        // Create a dll.conf file with only the single backend name (normally it's all backends, one per line)
        File.WriteAllText(Path.Combine(_customConfigDir, "dll.conf"), backendName);
        // Create an empty dll.d dir so SANE doesn't use the default one
        Directory.CreateDirectory(Path.Combine(_customConfigDir, "dll.d"));
        _scanningContext.Logger.LogDebug("Create temp SANE config dir {Dir}", _customConfigDir);
        return _customConfigDir;
    }

    internal OptionData SetOptions(ISaneDevice device, ScanOptions options)
    {
        var controller = new SaneOptionController(device, _scanningContext.Logger);
        var optionData = new OptionData
        {
            IsFeeder = options.PaperSource is PaperSource.Feeder or PaperSource.Duplex
        };

        if (options.PaperSource == PaperSource.Auto)
        {
            if (!controller.TrySet(SaneOptionNames.SOURCE, SaneOptionMatchers.Flatbed))
            {
                optionData.IsFeeder = controller.TrySet(SaneOptionNames.SOURCE, SaneOptionMatchers.Feeder);
            }
        }
        else if (options.PaperSource == PaperSource.Flatbed)
        {
            controller.TrySet(SaneOptionNames.SOURCE, SaneOptionMatchers.Flatbed);
        }
        else if (options.PaperSource == PaperSource.Feeder)
        {
            // We could throw NoFeederSupportException on failure, except this might be a feeder-only scanner.
            controller.TrySet(SaneOptionNames.SOURCE, SaneOptionMatchers.Feeder);
        }
        else if (options.PaperSource == PaperSource.Duplex)
        {
            controller.TrySet(SaneOptionNames.SOURCE, SaneOptionMatchers.Duplex);
            controller.TrySet(SaneOptionNames.ADF_MODE1, SaneOptionMatchers.Duplex);
            controller.TrySet(SaneOptionNames.ADF_MODE2, SaneOptionMatchers.Duplex);
        }

        var mode = options.BitDepth switch
        {
            BitDepth.BlackAndWhite => SaneOptionMatchers.BlackAndWhite,
            BitDepth.Grayscale => SaneOptionMatchers.Grayscale,
            _ => SaneOptionMatchers.Color
        };
        controller.TrySet(SaneOptionNames.MODE, mode);

        SetResolution(options, controller, optionData);

        var scanAreaController = new SaneScanAreaController(controller);
        if (scanAreaController.CanSetArea)
        {
            var (minX, minY, maxX, maxY) = scanAreaController.GetBounds();
            var width = Math.Min((double) options.PageSize!.WidthInMm, maxX - minX);
            var height = Math.Min((double) options.PageSize.HeightInMm, maxY - minY);
            var deltaX = maxX - minX - width;
            var offsetX = options.PageAlign switch
            {
                HorizontalAlign.Left => deltaX,
                HorizontalAlign.Center => deltaX / 2,
                _ => 0
            };
            scanAreaController.SetArea(minX + offsetX, minY, minX + offsetX + width, minY + height);
        }

        return optionData;
    }

    private void SetResolution(ScanOptions options, SaneOptionController controller, OptionData optionData)
    {
        var targetDpi = GetClosestResolution(options.Dpi, controller);

        if (controller.TrySet(SaneOptionNames.RESOLUTION, targetDpi))
        {
            if (controller.TryGet(SaneOptionNames.RESOLUTION, out var res))
            {
                optionData.XRes = res;
                optionData.YRes = res;
            }
        }
        else
        {
            controller.TrySet(SaneOptionNames.X_RESOLUTION, targetDpi);
            controller.TrySet(SaneOptionNames.Y_RESOLUTION, targetDpi);
            if (controller.TryGet(SaneOptionNames.X_RESOLUTION, out var xRes))
            {
                optionData.XRes = xRes;
            }
            if (controller.TryGet(SaneOptionNames.Y_RESOLUTION, out var yRes))
            {
                optionData.YRes = yRes;
            }
        }
        if (optionData.XRes <= 0) optionData.XRes = targetDpi;
        if (optionData.YRes <= 0) optionData.YRes = targetDpi;
    }

    private double GetClosestResolution(int dpi, SaneOptionController controller)
    {
        var targetDpi = (double) dpi;
        var opt = controller.GetOption(SaneOptionNames.RESOLUTION) ??
                  controller.GetOption(SaneOptionNames.X_RESOLUTION) ??
                  controller.GetOption(SaneOptionNames.Y_RESOLUTION);
        if (opt != null)
        {
            if (opt.ConstraintType == SaneConstraintType.Range)
            {
                targetDpi = targetDpi.Clamp(opt.Range!.Min, opt.Range.Max);
                if (opt.Range.Quant != 0)
                {
                    targetDpi -= (targetDpi - opt.Range.Min) % opt.Range.Quant;
                }
            }
            if (opt.ConstraintType == SaneConstraintType.WordList && opt.WordList!.Any())
            {
                targetDpi = opt.WordList!.OrderBy(x => Math.Abs(x - targetDpi)).First();
            }
        }
        if ((int) targetDpi != dpi)
        {
            _scanningContext.Logger.LogDebug("Correcting DPI from {InDpi} to {OutDpi}", dpi, targetDpi);
        }
        return targetDpi;
    }

    internal IMemoryImage? ScanPage(ISaneDevice device, IScanEvents scanEvents, OptionData optionData)
    {
        var data = ScanFrame(device, scanEvents, 0, out var p);
        if (data == null)
        {
            return null;
        }

        var page = p.Frame is SaneFrameType.Red or SaneFrameType.Green or SaneFrameType.Blue
            ? ProcessMultiFrameImage(device, scanEvents, p, data.GetBuffer())
            : ProcessSingleFrameImage(p, data.GetBuffer());
        page.SetResolution((float) optionData.XRes, (float) optionData.YRes);
        return page;
    }

    private IMemoryImage ProcessSingleFrameImage(SaneReadParameters p, byte[] data)
    {
        var (pixelFormat, subPixelType) = (depth: p.Depth, frame: p.Frame) switch
        {
            (1, SaneFrameType.Gray) => (ImagePixelFormat.BW1, SubPixelType.InvertedBit),
            (8, SaneFrameType.Gray) => (ImagePixelFormat.Gray8, SubPixelType.Gray),
            (8, SaneFrameType.Rgb) => (ImagePixelFormat.RGB24, SubPixelType.Rgb),
            _ => throw new InvalidOperationException(
                $"Unsupported transfer format: {p.Depth} bits per sample, {p.Frame} frame")
        };
        var image = _scanningContext.ImageContext.Create(p.PixelsPerLine, p.Lines, pixelFormat);
        var pixelInfo = new PixelInfo(p.PixelsPerLine, p.Lines, subPixelType, p.BytesPerLine);
        new CopyBitwiseImageOp().Perform(data, pixelInfo, image);
        return image;
    }

    private IMemoryImage ProcessMultiFrameImage(ISaneDevice device, IScanEvents scanEvents, SaneReadParameters p,
        byte[] data)
    {
        var image = _scanningContext.ImageContext.Create(p.PixelsPerLine, p.Lines, ImagePixelFormat.RGB24);
        var pixelInfo = new PixelInfo(p.PixelsPerLine, p.Lines, SubPixelType.Gray, p.BytesPerLine);

        // Use the first buffer, then read two more buffers and use them so we get all 3 channels
        new CopyBitwiseImageOp { DestChannel = ToChannel(p.Frame) }.Perform(data, pixelInfo, image);
        ReadSingleChannelFrame(device, scanEvents, 1, pixelInfo, image);
        ReadSingleChannelFrame(device, scanEvents, 2, pixelInfo, image);
        return image;
    }

    private void ReadSingleChannelFrame(ISaneDevice device, IScanEvents scanEvents, int frame, PixelInfo pixelInfo,
        IMemoryImage image)
    {
        var data = ScanFrame(device, scanEvents, frame, out var p)
                   ?? throw new DeviceException("SANE unexpected last frame");
        new CopyBitwiseImageOp { DestChannel = ToChannel(p.Frame) }.Perform(data.GetBuffer(), pixelInfo, image);
    }

    private ColorChannel ToChannel(SaneFrameType frame) => frame switch
    {
        SaneFrameType.Red => ColorChannel.Red,
        SaneFrameType.Green => ColorChannel.Green,
        SaneFrameType.Blue => ColorChannel.Blue,
        _ => throw new ArgumentException()
    };

    internal MemoryStream? ScanFrame(ISaneDevice device, IScanEvents scanEvents, int frame, out SaneReadParameters p)
    {
        device.Start();
        if (frame == 0)
        {
            scanEvents.PageStart();
        }

        p = device.GetParameters();
        bool isMultiFrame = p.Frame is SaneFrameType.Red or SaneFrameType.Green or SaneFrameType.Blue;
        // p.Lines can be -1, in which case we don't know the frame size ahead of time
        var frameSize = p.Lines == -1 ? 0 : p.BytesPerLine * p.Lines;
        var currentProgress = frame * frameSize;
        var totalProgress = isMultiFrame ? frameSize * 3 : frameSize;
        var buffer = new byte[65536];
        if (totalProgress > 0)
        {
            scanEvents.PageProgress(currentProgress / (double) totalProgress);
        }

        var dataStream = new MemoryStream(frameSize);
        while (device.Read(buffer, out var len))
        {
            dataStream.Write(buffer, 0, len);
            currentProgress += len;
            if (totalProgress > 0)
            {
                scanEvents.PageProgress(currentProgress / (double) totalProgress);
            }
        }

        if (dataStream.Length == 0)
        {
            return null;
        }
        // Now that we've read the data we know the exact frame size and can work backwards to get the number of lines.
        p.Lines = (int) dataStream.Length / p.BytesPerLine;

        return dataStream;
    }

    internal class OptionData
    {
        public bool IsFeeder { get; set; }
        public double XRes { get; set; }
        public double YRes { get; set; }
    }

    //     if (options.BitDepth == BitDepth.Color)
    //     {
    //         ChooseStringOption("--mode", x => x == "Color");
    //         ChooseNumericOption("--depth", 8);
    //     }
    //     else if (options.BitDepth == BitDepth.Grayscale)
    //     {
    //         ChooseStringOption("--mode", x => x == "Gray");
    //         ChooseNumericOption("--depth", 8);
    //     }
    //     else if (options.BitDepth == BitDepth.BlackAndWhite)
    //     {
    //         if (!ChooseStringOption("--mode", x => x == "Lineart"))
    //         {
    //             ChooseStringOption("--mode", x => x == "Halftone");
    //         }
    //         ChooseNumericOption("--depth", 1);
    //         ChooseNumericOption("--threshold", (-options.Brightness + 1000) / 20m);
    //     }
}