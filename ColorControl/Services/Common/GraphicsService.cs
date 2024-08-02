using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using NWin32;
using Shared.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColorControl.Services.Common
{
    abstract class GraphicsService<T> : ServiceBase<T> where T : PresetBase, new()
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private int _lastSetSDRBrightness = -1;
        private int _lastReadSDRBrightness = -1;

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
    }
}
