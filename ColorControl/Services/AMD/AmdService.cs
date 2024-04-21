using ATI.ADL;
using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Native;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Services.AMD
{
    class AmdService : GraphicsService<AmdPreset>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override string ServiceName => "AMD";

        protected override string PresetsBaseFilename => "AmdPresets.json";

        private ADLDisplayInfo _currentDisplay;

        public static readonly int SHORTCUTID_AMDQA = -201;

        public AmdService(GlobalContext globalContext) : base(globalContext)
        {
            LoadPresets();
        }

        public static async Task<bool> ExecutePresetAsync(string idOrName)
        {
            try
            {
                var globalContext = Program.ServiceProvider.GetRequiredService<GlobalContext>();

                var service = new AmdService(globalContext);

                var result = await service.ApplyPreset(idOrName);

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

        public override void InstallEventHandlers()
        {
            base.InstallEventHandlers();

            SetShortcuts(SHORTCUTID_AMDQA, _globalContext.Config.AmdQuickAccessShortcut);
        }

        protected override List<AmdPreset> GetDefaultPresets()
        {
            return AmdPreset.GetDefaultPresets();
        }

        private void SavePresets()
        {
            Utils.WriteObject(_presetsFilename, _presets);
        }

        public void GlobalSave()
        {
            SavePresets();
        }

        public override bool HasDisplaysAttached(bool reinitialize = false)
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

        public async Task<bool> ApplyPreset(string idOrName)
        {
            var preset = GetPresetByIdOrName(idOrName);
            if (preset != null)
            {
                return await ApplyPreset(preset);
            }
            else
            {
                return false;
            }
        }

        public override async Task<bool> ApplyPreset(AmdPreset preset)
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

            if (preset.DisplayConfig.ApplyRefreshRate || preset.DisplayConfig.ApplyResolution)
            {
                if (!SetMode(preset.DisplayConfig, true))
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

        public bool SetMode(DisplayConfig displayConfig, bool updateRegistry = false)
        {
            var display = GetCurrentDisplay();

            return SetMode(GetDisplayDeviceName(display), displayConfig, updateRegistry);
        }

        public List<Rational> GetAvailableRefreshRates(AmdPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var horizontal = 3840;
            var vertical = 2160;
            var display = GetCurrentDisplay();

            ADLWrapper.GetDisplayResolution(display, ref horizontal, ref vertical);

            return GetAvailableRefreshRatesV2(GetDisplayDeviceName(display), horizontal, vertical);
        }

        public List<VirtualResolution> GetAvailableResolutionsV2(AmdPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var display = GetCurrentDisplay();
            var deviceName = GetDisplayDeviceName(display);
            if (deviceName == null)
            {
                return [];
            }

            return GetAvailableResolutionsInternalV2(deviceName);
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
                var info = CCD.GetDisplayInfo(screen.DeviceName);
                Logger.Debug($"SCREEN[{index}]: {screen.DeviceName}, {info?.FriendlyName}, {screen.Primary}");
                index++;
            }
            return Screen.AllScreens.FirstOrDefault(x => CCD.GetDisplayInfo(x.DeviceName)?.FriendlyName.Equals(display.DisplayName) ?? false);
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
                var values = new List<string>
                {
                    "Current settings"
                };

                var screen = GetScreenForDisplay(display);

                var name = GetFullDisplayName(display);
                values.Add(name);

                var colorDepth = ADLColorDepth.UNKNOWN;
                ADLWrapper.GetDisplayColorDepth(display, ref colorDepth);

                var pixelFormat = ADLPixelFormat.UNKNOWN;
                ADLWrapper.GetDisplayPixelFormat(display, ref pixelFormat);

                var colorSettings = $"{colorDepth.GetDescription()}, {pixelFormat.GetDescription()}";
                values.Add(colorSettings);

                var displayConfig = CCD.GetDisplayConfig(screen?.DeviceName);

                values.Add($"{displayConfig.RefreshRate}Hz");

                values.Add($"{displayConfig.GetResolutionDesc()}");

                var ditherState = GetDithering(display);
                values.Add(ditherState.GetDescription());

                var hdrEnabled = IsHDREnabled();
                values.Add(hdrEnabled ? "Yes" : "No");

                var infoLine = string.Format("{0}: {1}, {2}Hz, {3}, HDR: {4}", name, colorSettings, displayConfig.RefreshRate, displayConfig.GetResolutionDesc(), hdrEnabled ? "Yes" : "No");

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
