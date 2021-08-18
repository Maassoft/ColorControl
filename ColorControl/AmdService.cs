using ATI.ADL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl
{
    class AmdService : GraphicsService<AmdPreset>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private ADLDisplayInfo _currentDisplay;

        public AmdService(string dataPath) : base(dataPath, "AmdPresets.json")
        {
            LoadPresets();
        }

        public static bool ExecutePresetAsync(string idOrName)
        {
            try
            {
                var service = new AmdService(Program.DataDir);

                var result = service.ApplyPreset(idOrName);

                if (!result)
                {
                    Console.WriteLine("Preset not found or error while executing.");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing preset: " + ex.ToLogString());
                return false;
            }
        }

        protected override List<AmdPreset> GetDefaultPresets()
        {
            return AmdPreset.GetDefaultPresets();
        }

        private void SavePresets()
        {
            try
            {
                var json = _JsonSerializer.Serialize(_presets);
                File.WriteAllText(_presetsFilename, json);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }
        }

        public void GlobalSave()
        {
            SavePresets();
        }

        public override bool HasDisplaysAttached()
        {
            try
            {
                return GetDisplays().Any();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetCurrentDisplay(AmdPreset preset)
        {
            var displays = GetDisplays();
            if (preset.primaryDisplay)
            {
                _currentDisplay = displays.FirstOrDefault();
            }
            else
            {
                _currentDisplay = displays.FirstOrDefault(x => x.DisplayName.Equals(preset.displayName));
            }
        }

        public ADLDisplayInfo GetCurrentDisplay()
        {
            if (string.IsNullOrEmpty(_currentDisplay.DisplayName))
            {
                _currentDisplay = GetDisplays().FirstOrDefault();
            }

            return _currentDisplay;
        }

        public bool ApplyPreset(string idOrName)
        {
            var preset = GetPresetByIdOrName(idOrName);
            if (preset != null)
            {
                return ApplyPreset(preset, Program.AppContext);
            }
            else
            {
                return false;
            }
        }

        public bool ApplyPreset(AmdPreset preset, AppContext appContext)
        {
            SetCurrentDisplay(preset);

            if (string.IsNullOrEmpty(_currentDisplay.DisplayName))
            {
                Logger.Error("Could not set current display");
                return false;
            }

            var result = true;

            if (preset.applyHDR)
            {
                var hdrEnabled = IsHDREnabled();
                if (preset.toggleHDR)
                {
                    result = ADLWrapper.SetDisplayHDRState(_currentDisplay, !hdrEnabled);
                }
                else
                {
                    result = ADLWrapper.SetDisplayHDRState(_currentDisplay, preset.HDREnabled);
                }

                //if (preset.toggleHDR || preset.HDREnabled != hdrEnabled)
                //{
                //    ToggleHDR(config.DisplaySettingsDelay);
                //}
            }

            if (preset.applyRefreshRate)
            {
                if (!SetRefreshRate(preset.refreshRate))
                {
                    result = false;
                }
            }

            if (preset.applyColorData)
            {
                if (!ADLWrapper.SetDisplayColorDepth(_currentDisplay, preset.colorDepth))
                {
                    Logger.Error("Could not set color depth");
                    result = false;
                }
                if (preset.pixelFormat != ADLPixelFormat.UNKNOWN && !ADLWrapper.SetDisplayPixelFormat(_currentDisplay, preset.pixelFormat))
                {
                    Logger.Error("Could not set pixel format");
                    result = false;
                }
            }

            if (preset.applyDithering)
            {
                if (!SetDithering(preset.ditherState))
                {
                    result = false;
                }
            }

            _lastAppliedPreset = preset;

            PresetApplied();

            return result;
        }

        public bool SetDithering(ADLDitherState ditherState)
        {
            var display = GetCurrentDisplay();

            return ADLWrapper.SetDisplayDitherState(display, ditherState);
        }

        public ADLDitherState GetDithering(ADLDisplayInfo display = default(ADLDisplayInfo))
        {
            if (string.IsNullOrEmpty(display.DisplayName))
            {
                display = GetCurrentDisplay();
            }

            var ditherState = ADLDitherState.DRIVER_DEFAULT;

            if (ADLWrapper.GetDisplayDitherState(display, ref ditherState))
            {
                return ditherState;
            }

            return ditherState;
        }

        public string GetDisplayDeviceName(ADLDisplayInfo display)
        {
            return Screen.PrimaryScreen.DeviceName;
        }

        public bool SetRefreshRate(uint refreshRate)
        {
            var horizontal = 3840;
            var vertical = 2160;
            var display = GetCurrentDisplay();

            ADLWrapper.GetDisplayResolution(display, ref horizontal, ref vertical);

            var portrait = vertical > horizontal;

            return SetRefreshRateInternal(GetDisplayDeviceName(display), refreshRate, portrait, horizontal, vertical);
        }

        public List<uint> GetAvailableRefreshRates(AmdPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var horizontal = 3840;
            var vertical = 2160;
            var display = GetCurrentDisplay();

            ADLWrapper.GetDisplayResolution(display, ref horizontal, ref vertical);

            var portrait = vertical > horizontal;

            return GetAvailableRefreshRatesInternal(GetDisplayDeviceName(display), portrait, horizontal, vertical);
        }

        public bool IsHDREnabled()
        {
            var display = GetCurrentDisplay();

            var supported = false;
            var enabled = false;
            ADLWrapper.GetDisplayHDRState(display, ref supported, ref enabled);

            Logger.Debug($"IsHDREnabled: supported {supported}, enabled {enabled}");

            return enabled;
        }

        public List<ADLDisplayInfo> GetDisplays()
        {
            var displays = ADLWrapper.GetAllDisplays();

            //if (!displays.Any())
            //{
            //    displays.Add(new ADLDisplayInfo
            //    {
            //        DisplayName = "Dummy"
            //    });
            //}

            return displays;
        }

        public string GetFullDisplayName(ADLDisplayInfo display)
        {
            var name = display.DisplayName;

            var screen = GetScreenForDisplay(display);
            if (screen != null)
            {
                name = $"{screen.DeviceName} ({name})";
            }

            return name;
        }

        public Screen GetScreenForDisplay(ADLDisplayInfo display)
        {
            var index = 0;
            foreach (var screen in Screen.AllScreens)
            {
                Logger.Debug($"SCREEN[{index}]: {screen.DeviceName}, {screen.DeviceFriendlyName()}, {screen.Primary}");
                index++;
            }
            return Screen.AllScreens.FirstOrDefault(x => x.DeviceFriendlyName().Equals(display.DisplayName));
        }

        public List<AmdDisplayInfo> GetDisplayInfos()
        {
            List<ADLDisplayInfo> displays;
            try
            {
                displays = GetDisplays();
            }
            catch (Exception e)
            {
                Logger.Error("Error while getting displays: " + e.ToLogString());
                return null;
            }

            var list = new List<AmdDisplayInfo>();

            foreach (var display in displays)
            {
                var values = new List<string>();

                values.Add("Current settings");

                var screen = GetScreenForDisplay(display);

                var name = GetFullDisplayName(display);
                values.Add(name);

                var colorDepth = ADLColorDepth.UNKNOWN;
                ADLWrapper.GetDisplayColorDepth(display, ref colorDepth);

                var pixelFormat = ADLPixelFormat.UNKNOWN;
                ADLWrapper.GetDisplayPixelFormat(display, ref pixelFormat);

                var colorSettings = $"{colorDepth}, {pixelFormat}";
                values.Add(colorSettings);

                var refreshRate = GetCurrentRefreshRate(screen?.DeviceName);
                values.Add($"{refreshRate}Hz");

                var ditherState = GetDithering(display);
                values.Add(ditherState.ToString());

                var hdrEnabled = IsHDREnabled();
                values.Add(hdrEnabled ? "Yes" : "No");

                var infoLine = string.Format("{0}: {1}, {2}Hz, HDR: {3}", name, colorSettings, refreshRate, hdrEnabled ? "Yes" : "No");

                var displayInfo = new AmdDisplayInfo(display, values, infoLine);

                list.Add(displayInfo);
            }

            return list;
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
