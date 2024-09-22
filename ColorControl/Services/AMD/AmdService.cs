using ATI.ADL;
using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.AMD;
using ColorControl.Shared.Native;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        private string _configFilename;
        public AmdServiceConfig Config { get; private set; }

        public AmdService(GlobalContext globalContext) : base(globalContext)
        {
            LoadPresets();
            LoadConfig();
        }

        protected override void AfterPresetsLoaded()
        {
            // Force disable display preset on presets
            _presets.ForEach(p => p.IsDisplayPreset = false);
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

            SetShortcuts(SHORTCUTID_AMDQA, Config.QuickAccessShortcut);
        }

        private void LoadConfig()
        {
            _configFilename = Path.Combine(_dataPath, "AmdConfig.json");
            try
            {
                if (File.Exists(_configFilename))
                {
                    Config = JsonConvert.DeserializeObject<AmdServiceConfig>(File.ReadAllText(_configFilename));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"LoadConfig: {ex.Message}");
            }
            Config ??= new AmdServiceConfig();
        }

        protected override List<AmdPreset> GetDefaultPresets()
        {
            return AmdPreset.GetDefaultPresets();
        }

        public override List<string> GetInfo()
        {
            return [$"{_presets.Count} presets"];
        }

        public void GlobalSave()
        {
            SavePresets();
            SaveConfig();
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

                if (preset.SDRBrightness.HasValue)
                {
                    var deviceName = GetDisplayDeviceName(_currentDisplay);

                    SetSDRBrightness(deviceName, preset.SDRBrightness.Value);
                }
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

        public bool SetMode(VirtualResolution resolution = null, Rational refreshRate = null, bool updateRegistry = false, AmdPreset preset = null)
        {
            var display = preset == null ? GetCurrentDisplay() : GetPresetDisplay(preset);
            if (display.Equals(default(ADLDisplayInfo)))
            {
                return false;
            }

            return SetMode(GetDisplayDeviceName(display), resolution, refreshRate, updateRegistry);
        }

        public bool SetMode(DisplayConfig displayConfig, bool updateRegistry = false, AmdPreset preset = null)
        {
            var display = preset == null ? GetCurrentDisplay() : GetPresetDisplay(preset);
            if (display.DisplayID.DisplayPhysicalIndex == 0)
            {
                return false;
            }

            return SetMode(GetDisplayDeviceName(display), displayConfig, updateRegistry);
        }

        public ADLDisplayInfo GetPresetDisplay(AmdPreset preset)
        {
            return GetCurrentDisplay();
        }

        public List<Rational> GetAvailableRefreshRatesV2(AmdPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var display = _currentDisplay;
            if (_currentDisplay.DisplayID.DisplayLogicalAdapterIndex == 0)
            {
                return [];
            }

            var desktopRect = preset?.DisplayConfig.ApplyResolution == true && preset?.DisplayConfig.Resolution.ActiveWidth > 0 ?
                new Rectangle(0, 0, (int)preset.DisplayConfig.Resolution.ActiveWidth, (int)preset.DisplayConfig.Resolution.ActiveHeight) :
                GetDesktopRect(display);

            return GetAvailableRefreshRatesV2(Screen.PrimaryScreen.DeviceName, desktopRect.Width, desktopRect.Height);
        }

        private Rectangle GetDesktopRect(ADLDisplayInfo display)
        {
            // TODO: improve this
            var screen = Screen.AllScreens.First();

            return screen.Bounds;
        }

        public bool IsHDREnabled(ADLDisplayInfo display = default)
        {
            if (display.Equals(default(ADLDisplayInfo)))
            {
                display = GetCurrentDisplay();
            }

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

        public List<AmdDisplayInfo> GetSimpleDisplayInfos()
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

            return displays.Select(d => new AmdDisplayInfo(d, null, null)).ToList();
        }

        public List<AmdDisplayInfo> GetDisplayInfos()
        {
            var list = new List<AmdDisplayInfo>();

            var displayInfos = GetSimpleDisplayInfos();

            foreach (var displayInfo in displayInfos)
            {
                var preset = GetPresetForDisplay(displayInfo, displayInfos.Count());

                displayInfo.Values = preset.GetDisplayValues(_globalContext.Config);
                displayInfo.InfoLine = preset.InfoLine;

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

        public List<AmdPreset> GetDisplayPresets()
        {
            try
            {
                var list = new List<AmdPreset>();

                var displayInfos = GetSimpleDisplayInfos();

                foreach (var displayInfo in displayInfos)
                {
                    var preset = GetPresetForDisplay(displayInfo, displayInfos.Count());

                    displayInfo.Values = preset.GetDisplayValues(_globalContext.Config);
                    displayInfo.InfoLine = preset.InfoLine;

                    list.Add(preset);
                }

                return list;
            }
            catch (Exception e)
            {
                Logger.Error("Error while getting displays: " + e.ToLogString());
                return null;
            }
        }

        public AmdPreset GetPresetForDisplay(AmdDisplayInfo displayInfo, int displayCount = 1)
        {
            if (displayInfo == null)
            {
                return null;
            }

            var display = displayInfo.Display;

            var colorDepth = ADLColorDepth.UNKNOWN;
            ADLWrapper.GetDisplayColorDepth(display, ref colorDepth);

            var pixelFormat = ADLPixelFormat.UNKNOWN;
            ADLWrapper.GetDisplayPixelFormat(display, ref pixelFormat);

            var ditherState = GetDithering(display);

            var screen = GetScreenForDisplay(display);
            var displayConfig = CCD.GetDisplayConfig(screen?.DeviceName);

            var ccdDisplay = screen != null ? CCD.GetDisplayInfo(screen.DeviceName) : null;

            var hdrEnabled = IsHDREnabled(display);

            var preset = new AmdPreset
            {
                IsDisplayPreset = true,
                displayName = display.DisplayName,
                DisplayId = ccdDisplay?.DisplayId,
                primaryDisplay = screen?.Primary == true,
                colorDepth = colorDepth,
                pixelFormat = pixelFormat,
                ditherState = ditherState,
                DisplayConfig = displayConfig,
                SDRBrightness = hdrEnabled ? GetSDRBrightness(screen?.DeviceName) : null,
                HDREnabled = hdrEnabled,
            };

            return preset;
        }

        private void SaveConfig()
        {
            Utils.WriteObject(_configFilename, Config);
        }

        public AmdServiceConfig GetConfig()
        {
            return Config;
        }

        public bool UpdateConfig(AmdServiceConfig config)
        {
            Config.Update(config);

            SetShortcuts(SHORTCUTID_AMDQA, Config.QuickAccessShortcut);

            SaveConfig();

            return true;
        }

        public bool UpdatePreset(AmdPreset specPreset)
        {
            var currentPreset = _presets.FirstOrDefault(p => p.id == specPreset.id);

            if (currentPreset != null)
            {
                currentPreset.name = specPreset.name;
                currentPreset.shortcut = specPreset.shortcut;
                currentPreset.Update(specPreset);

                SavePresets();

                return true;
            }

            var newPreset = new AmdPreset(specPreset);
            newPreset.name = specPreset.name;
            newPreset.shortcut = specPreset.shortcut;

            _presets.Add(newPreset);

            SavePresets();

            return true;
        }
    }
}
