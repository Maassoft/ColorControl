using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using NWin32;
using NWin32.NativeTypes;
using Shared.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static ColorControl.Shared.Native.WinApi;

namespace ColorControl.Services.Common;

abstract class GraphicsService<T> : ServiceBase<T> where T : PresetBase, new()
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private int _lastSetSDRBrightness = -1;
    private int _lastReadSDRBrightness = -1;

    internal class SafePhysicalMonitorHandle : SafeHandle
    {
        public SafePhysicalMonitorHandle(IntPtr handle) : base(IntPtr.Zero, true)
        {
            this.handle = handle; // IntPtr.Zero may be a valid handle.
        }

        public override bool IsInvalid => false; // The validity cannot be checked by the handle.

        protected override bool ReleaseHandle()
        {
            return true;
        }
    }

    public GraphicsService(GlobalContext globalContext) : base(globalContext)
    {
    }

    ~GraphicsService()
    {
        Uninitialize();
    }

    public abstract bool HasDisplaysAttached(bool reinitialize = false);

    public bool SetMode(string displayName, VirtualResolution resolution = null, Rational refreshRate = null, bool updateRegistry = false)
    {
        var displayConfig = CCD.GetDisplayConfig(displayName);

        if (refreshRate != null)
        {
            displayConfig.ApplyRefreshRate = true;
            displayConfig.RefreshRate = refreshRate;
        }

        if (resolution != null)
        {
            displayConfig.ApplyResolution = true;
            displayConfig.Resolution = resolution;
        }

        return SetMode(displayName, displayConfig, updateRegistry);
    }

    public bool SetMode(string displayName, DisplayConfig displayConfig, bool updateRegistry = false)
    {
        var currentDisplayConfig = CCD.GetDisplayConfig(displayName);

        if (displayConfig.ApplyRefreshRate)
        {
            currentDisplayConfig.RefreshRate = displayConfig.RefreshRate;
        }

        if (displayConfig.ApplyResolution)
        {
            currentDisplayConfig.Resolution = displayConfig.Resolution;
        }

        if (displayConfig.Scaling != CCD.DisplayConfigScaling.Zero)
        {
            currentDisplayConfig.Scaling = displayConfig.Scaling;
        }

        if (displayConfig.Rotation != CCD.DisplayConfigRotation.Zero)
        {
            currentDisplayConfig.Rotation = displayConfig.Rotation;
        }

        if (displayConfig.Connectivity != DisplayConnectivity.Unchanged)
        {
            currentDisplayConfig.Connectivity = displayConfig.Connectivity;
        }

        return CCD.SetDisplayConfig(displayName, currentDisplayConfig, updateRegistry);
    }

    public void SetSDRBrightness(string displayName, int brightnessPercent)
    {
        var hmon = FormUtils.GetMonitorForDisplayName(displayName);

        if (hmon == IntPtr.Zero)
        {
            hmon = NativeMethods.MonitorFromWindow(0, 1);
        }

        var brightness = 1 + (double)brightnessPercent / 20;

        WinApi.DwmpSDRToHDRBoostPtr(hmon, brightness);

        _lastSetSDRBrightness = brightnessPercent;
    }

    public int GetSDRBrightness(string displayName)
    {
        var whiteLevel = CCD.GetSDRWhiteLevel(displayName);

        if (whiteLevel >= 1000)
        {
            var newBrightnessPercent = (int)(whiteLevel - 1000) / 50;

            if (_lastSetSDRBrightness >= 0 && newBrightnessPercent == _lastReadSDRBrightness)
            {
                return _lastSetSDRBrightness;
            }

            _lastReadSDRBrightness = newBrightnessPercent;

            return newBrightnessPercent;
        }

        return 0;
    }

    protected List<Rational> GetAvailableRefreshRatesV2(string displayName, int horizontal, int vertical)
    {
        var dxWrapper = new DXWrapper();

        var modes = dxWrapper.GetModes(displayName, (uint)horizontal, (uint)vertical);

        var refreshRates = modes.Select(m => m.RefreshRate).DistinctBy(r => r.ToString())
            .Select(r => new Rational(r.Numerator, r.Denominator));

        return refreshRates.ToList();
    }

    protected List<VirtualResolution> GetAvailableResolutionsInternalV2(string displayName, Rational refreshRate = null)
    {
        var dxWrapper = new DXWrapper();

        var modes = dxWrapper.GetModes(displayName);

        var refreshRates = modes.DistinctBy(m => $"{m.Resolution.width}x{m.Resolution.height}")
            .Select(m => new VirtualResolution(m.Resolution.width, m.Resolution.height));

        return refreshRates.ToList();
    }

    protected bool SetMonitorScaling()
    {
        var devMode = new DEVMODEW();
        devMode.dmSize = (ushort)Marshal.SizeOf<DEVMODEW>();
        devMode.dmFields = NativeConstants.DM_LOGPIXELS;
        devMode.dmLogPixels = 96 * 96 / 100;

        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<DEVMODEW>());

        Marshal.StructureToPtr(devMode, ptr, false);

        if (NativeMethods.ChangeDisplaySettingsExW(null, ptr, 0, 0, 0) == NativeConstants.DISP_CHANGE_SUCCESSFUL)
        {
            var result = NativeMethods.SystemParametersInfoW(0x009F, uint.MaxValue - 6, 0, NativeConstants.SPIF_SENDCHANGE | NativeConstants.SPIF_UPDATEINIFILE);
        }

        return true;

    }

    protected static IEnumerable<SafePhysicalMonitorHandle> EnumeratePhysicalMonitors(IntPtr monitorHandle, bool verbose = false)
    {
        if (!GetNumberOfPhysicalMonitorsFromHMONITOR(
            monitorHandle,
            out uint count))
        {
            Debug.WriteLine($"Failed to get the number of physical monitors. {WinError.GetMessage()}");
            yield break;
        }
        if (count == 0)
        {
            yield break;
        }

        var physicalMonitors = new PHYSICAL_MONITOR[count];

        try
        {
            if (!GetPhysicalMonitorsFromHMONITOR(
                monitorHandle,
                count,
                physicalMonitors))
            {
                Debug.WriteLine($"Failed to get an array of physical monitors. {WinError.GetMessage()}");
                yield break;
            }

            int monitorIndex = 0;

            foreach (var physicalMonitor in physicalMonitors)
            {
                var handle = new SafePhysicalMonitorHandle(physicalMonitor.hPhysicalMonitor);

                //Debug.WriteLine($"Description: {physicalMonitor.szPhysicalMonitorDescription}");
                //Debug.WriteLine($"Handle: {physicalMonitor.hPhysicalMonitor}");

                yield return handle;

                monitorIndex++;
            }
        }
        finally
        {
            // The physical monitor handles should be destroyed at a later stage.
        }
    }

    public static GenericResult<VcpInfo> ReadDDC(string displayName, byte vcpCode)
    {
        // Do this via: GetVCPFeatureAndVCPFeatureReply: https://learn.microsoft.com/en-us/windows/win32/api/lowlevelmonitorconfigurationapi/nf-lowlevelmonitorconfigurationapi-getvcpfeatureandvcpfeaturereply

        var handle = GetPhysicalMonitorHandle(displayName);

        if (handle == -1)
        {
            return null;
        }

        try
        {
            var result = GetVCPFeatureAndVCPFeatureReply(handle, vcpCode, out var codeType, out var currentValue, out var maxValue);

            if (!result)
            {
                var (errorCode, msg) = WinError.GetCodeMessage();

                Logger.Warn("Error while reading VCP code of display: {0}", msg);

                return GenericResult<VcpInfo>.FromError(msg);
            }

            return GenericResult<VcpInfo>.FromSuccess(new VcpInfo
            {
                Value = currentValue,
                MaxValue = maxValue,
                CodeType = codeType
            });
        }
        finally
        {
            //first.DangerousRelease();
        }
    }

    public static async Task<GenericBoolResult> WriteDDC(string displayName, byte vcpCode, uint value, DdcSetting.ValueChangeTypes changeType = DdcSetting.ValueChangeTypes.Fixed, bool validate = true)
    {
        var handle = GetPhysicalMonitorHandle(displayName);

        if (handle == -1)
        {
            return GenericBoolResult.FromError("No monitor handle");
        }

        if (changeType != DdcSetting.ValueChangeTypes.Fixed)
        {
            var readResult = ReadDDC(displayName, vcpCode);
            if (!readResult.HasResult)
            {
                return GenericBoolResult.FromError("Cannot apply increase or decrease");
            }

            var currentValue = readResult.Result.Value;

            if (changeType == DdcSetting.ValueChangeTypes.Increase)
            {
                value += currentValue;

                if (value > readResult.Result.MaxValue)
                {
                    value = readResult.Result.MaxValue;
                }
            }
            else if (changeType == DdcSetting.ValueChangeTypes.Decrease)
            {
                if (((int)currentValue - value) < 0)
                {
                    value = 0;
                }
                else
                {
                    value = currentValue - value;
                }
            }
        }

        var result = SetVCPFeature(handle, vcpCode, value);

        if (!result)
        {
            var (errorCode, msg) = WinError.GetCodeMessage();

            Logger.Warn("Error while writing VCP code of display: {0}", msg);

            return GenericBoolResult.FromError(msg);
        }

        if (validate)
        {
            await Task.Delay(50);

            var readResult = ReadDDC(displayName, vcpCode);
            if (readResult.Result?.Value != value)
            {
                return GenericBoolResult.FromError("The monitor did not apply the specified value");
            }
        }

        return GenericBoolResult.Success;
    }

    public static nint GetPhysicalMonitorHandle(string displayName)
    {
        if (displayName == null)
        {
            return -1;
        }

        var hMonitor = FormUtils.GetMonitorForDisplayName(displayName);

        var physicalHandles = EnumeratePhysicalMonitors(hMonitor);

        var first = physicalHandles.FirstOrDefault();

        if (first == null)
        {
            return -1;
        }

        return first.DangerousGetHandle();
    }
}
