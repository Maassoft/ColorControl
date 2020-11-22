using ATI.ADL;
using NvAPIWrapper.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ColorControl
{
    class AmdService : GraphicsService
    {
        private string CurrentDisplay;

        public AmdService()
        {
        }

        public void SetCurrentDisplay(AmdPreset preset)
        {
            CurrentDisplay = preset.displayName;
        }

        public string GetCurrentDisplay()
        {
            return CurrentDisplay;
        }

        public bool ApplyPreset(AmdPreset preset, Config config)
        {
            SetCurrentDisplay(preset);

            if (preset.applyHDR)
            {
                if (preset.toggleHDR || preset.HDREnabled != IsHDREnabled())
                {
                    ToggleHDR(config.DisplaySettingsDelay);
                }
            }

            if (preset.applyRefreshRate)
            {
                SetRefreshRate(preset.refreshRate);
            }

            if (preset.applyColorData)
            {
                var display = GetCurrentDisplay();
                // TODO: ADL call to set color format, depth, space
                // ADL_Display_PixelFormat_Set
                // ADL_Display_ColorDepth_Set

                //var data = new ColorData(ColorDataFormat.YUV444, ColorDataColorimetry.YCC709, ColorDataDynamicRange.Auto, ColorDataDepth.BPC8, preset.colorData.SelectionPolicy, ColorDataDesktopDepth.Default);
                //display.DisplayDevice.SetColorData(preset.colorData);
            }

            if (preset.applyDithering)
            {
                SetDithering(preset.ditheringEnabled);
            }

            return true;
        }

        public void SetDithering(bool enabled)
        {
            // TODO: ADL call to set dithering state
            // ADL_Display_DitherState_Get 
        }

        public bool GetDithering()
        {
            // TODO: ADL call to get dithering state
            // ADL_Display_DitherState_Set
            return false;
        }

        public bool SetRefreshRate(uint refreshRate)
        {
            var portrait = false;
            var horizontal = 3840;
            var vertical = 2160;
            var display = GetCurrentDisplay();

            ADLWrapper.GetDisplayResolution(display, ref horizontal, ref vertical);

            return SetRefreshRateInternal(display, refreshRate, portrait, horizontal, vertical);
        }

        public List<uint> GetAvailableRefreshRates(AmdPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var portrait = false;
            var horizontal = 3840;
            var vertical = 2160;
            var display = GetCurrentDisplay();

            ADLWrapper.GetDisplayResolution(display, ref horizontal, ref vertical);

            return GetAvailableRefreshRatesInternal(display, portrait, horizontal, vertical);
        }

        public bool IsHDREnabled()
        {
            // TODO: ADL call to get HDR state
            //var displayDevice = GetCurrentDisplay().DisplayDevice;
            //var hdr = displayDevice.HDRColorData;
            //return hdr?.HDRMode == ColorDataHDRMode.UHDA;
            return false;
        }

        public List<ADLDisplayInfo> GetDisplays()
        {
            return ADLWrapper.GetAllDisplays();
        }

        protected override void Initialize()
        {
            if (!ADLWrapper.Initialize())
            {
                throw new InvalidOperationException("Cannot load AMD API");
            }
        }

        protected override void Uninitialize()
        {
            ADLWrapper.Uninitialze();
        }
    }
}
