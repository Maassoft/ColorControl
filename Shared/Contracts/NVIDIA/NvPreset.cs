using ColorControl.Shared.Common;
using Newtonsoft.Json;
using NStandard;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.GPU;
using System.Text;

namespace ColorControl.Shared.Contracts.NVIDIA
{
    public delegate string GetDriverSettingsDescription(Dictionary<uint, uint> driverSettings, bool useNewLines = false);

    public class NvPreset : PresetBase
    {
        public static event GetDriverSettingsDescription GetDriverSettingsDescription;

        public bool applyColorData { get; set; }
        public ColorData colorData { get; set; }
        public bool ApplyColorEnhancements { get; set; }
        public NvColorEnhancementSettings ColorEnhancementSettings { get; set; }
        public bool applyHDR { get; set; }
        public bool HDREnabled { get; set; }
        public bool toggleHDR { get; set; }
        public bool applyDithering { get; set; }
        public bool? ditheringEnabled { get; set; }

        [JsonIgnore]
        public NvDitherState NvDitherState
        {
            get
            {
                return ditheringEnabled switch
                {
                    null => NvDitherState.Auto,
                    true => NvDitherState.Enabled,
                    false => NvDitherState.Disabled,
                };
            }
            set
            {
                ditheringEnabled = value switch
                {
                    NvDitherState.Enabled => true,
                    NvDitherState.Disabled => false,
                    _ => null
                };
            }
        }

        public DisplayConfig DisplayConfig { get; set; }
        public bool primaryDisplay { get; set; }
        public string displayName { get; set; }
        public string DisplayId { get; set; }
        public uint ditheringBits { get; set; }

        [JsonIgnore]
        public NvDitherBits NvDitherBits
        {
            get => (NvDitherBits)ditheringBits;
            set => ditheringBits = (uint)value;
        }

        public uint ditheringMode { get; set; }

        [JsonIgnore]
        public NvDitherMode NvDitherMode
        {
            get => (NvDitherMode)ditheringMode;
            set => ditheringMode = (uint)value;
        }

        public bool applyOther { get; set; }
        public int? SDRBrightness { get; set; }
        public Scaling? scaling { get; set; }
        public bool applyDriverSettings { get; set; }
        public Dictionary<uint, uint> driverSettings { get; set; }
        public string DriverProfileName { get; set; }
        public bool applyOverclocking { get; set; }
        public List<NvGpuOcSettings> ocSettings { get; set; }

        public bool applyHdmiSettings { get; set; }
        public NvHdmiInfoFrameSettings HdmiInfoFrameSettings { get; set; }
        public NvColorProfileSettings ColorProfileSettings { get; set; }
        public NvHdrSettings HdrSettings { get; set; }
        public bool ApplyNovideoSettings { get; set; }
        public NovideoSettings NovideoSettings { get; set; }

        public bool SetDitherRegistryKey { get; set; }
        public bool RestartDriver { get; set; }

        public bool IsDisplayPreset { get; set; }
        [JsonIgnore]
        public Display Display { get; set; }
        [JsonIgnore]
        public string InfoLine { get; set; }
        public NvDitherState DitherState => ditheringEnabled.HasValue ? ditheringEnabled == true ? NvDitherState.Enabled : NvDitherState.Disabled : NvDitherState.Auto;
        [JsonIgnore]
        public bool IsStartupPreset { get; set; }
        public MonitorConnectionType MonitorConnectionType { get; set; }

        public static NvPreset DefaultPreset = new NvPreset();

        public NvPreset() : base()
        {
            applyColorData = false;
            colorData = new ColorData(dynamicRange: ColorDataDynamicRange.Auto, colorDepth: ColorDataDepth.Default);
            applyDithering = false;
            ditheringEnabled = true;
            applyHDR = false;
            HDREnabled = false;
            toggleHDR = false;
            DisplayConfig = new DisplayConfig();
            primaryDisplay = true;
            ditheringBits = 1;
            ditheringMode = 4;
            applyDriverSettings = false;
            driverSettings = new Dictionary<uint, uint>();
            ocSettings = new List<NvGpuOcSettings>();
            HdmiInfoFrameSettings = new NvHdmiInfoFrameSettings();
            ColorProfileSettings = new NvColorProfileSettings();
            ColorEnhancementSettings = new NvColorEnhancementSettings();
            HdrSettings = new NvHdrSettings();
            NovideoSettings = new NovideoSettings();
        }

        public NvPreset(ColorData colorData) : this()
        {
            id = GetNewId();
            this.colorData = colorData;
        }

        public NvPreset(NvPreset preset) : this()
        {
            id = GetNewId();

            Update(preset);
        }

        public void UpdateAutoApplySettings(NvPreset currentSettings = null, bool keepChanges = false)
        {
            currentSettings ??= DefaultPreset;

            applyColorData = keepChanges && applyColorData || colorData.IsDifferent(currentSettings.colorData);
            applyDithering = keepChanges && applyDithering || (ditheringEnabled != currentSettings.ditheringEnabled || ditheringMode != currentSettings.ditheringMode || ditheringBits != currentSettings.ditheringBits);
            applyHDR = keepChanges && applyHDR || (HDREnabled != currentSettings.HDREnabled || toggleHDR || SDRBrightness != currentSettings.SDRBrightness || HdrSettings.OutputMode != currentSettings.HdrSettings.OutputMode);
            DisplayConfig.ApplyResolution = keepChanges && DisplayConfig.ApplyResolution || (DisplayConfig.Resolution.IsDifferent(currentSettings.DisplayConfig.Resolution) ||
                    DisplayConfig.Scaling != currentSettings.DisplayConfig.Scaling || DisplayConfig.Rotation != currentSettings.DisplayConfig.Rotation || DisplayConfig.IsPrimary != null && DisplayConfig.IsPrimary != currentSettings.DisplayConfig.IsPrimary);
            DisplayConfig.ApplyRefreshRate = keepChanges && DisplayConfig.ApplyRefreshRate || !DisplayConfig.RefreshRate.Equals(currentSettings.DisplayConfig.RefreshRate);
            applyOther = keepChanges && applyOther || (scaling != currentSettings.scaling || ColorProfileSettings.ProfileName != currentSettings.ColorProfileSettings.ProfileName);
            applyDriverSettings = keepChanges && applyDriverSettings || driverSettings.Any(s => !currentSettings.driverSettings.Any(o => o.Key == s.Key && o.Value == s.Value));
            applyHdmiSettings = keepChanges && applyHdmiSettings || HdmiInfoFrameSettings.IsDifferent(currentSettings.HdmiInfoFrameSettings);
            ApplyNovideoSettings = keepChanges && ApplyNovideoSettings || (NovideoSettings.ApplyClamp && !HDREnabled || applyHDR && !HDREnabled && !NovideoSettings.ApplyClamp);
            ApplyColorEnhancements = keepChanges && ApplyColorEnhancements ||
                    ColorEnhancementSettings.DigitalVibranceLevel != currentSettings.ColorEnhancementSettings.DigitalVibranceLevel ||
                    ColorEnhancementSettings.HueAngle != currentSettings.ColorEnhancementSettings.HueAngle;
        }

        public void Update(NvPreset preset)
        {
            primaryDisplay = preset.primaryDisplay;
            displayName = preset.displayName;
            DisplayId = preset.DisplayId;
            IsDisplayPreset = preset.IsDisplayPreset;
            ShowInQuickAccess = preset.ShowInQuickAccess;

            var colorData = preset.colorData;
            this.colorData = new ColorData(colorData.ColorFormat, colorData.Colorimetry, dynamicRange: colorData.DynamicRange, colorDepth: colorData.ColorDepth,
                colorSelectionPolicy: preset.IsDisplayPreset && colorData.UseDefaultSettings ? ColorDataSelectionPolicy.Default : ColorDataSelectionPolicy.User);
            applyColorData = preset.applyColorData;

            applyHDR = preset.applyHDR;
            HDREnabled = preset.HDREnabled;
            toggleHDR = preset.toggleHDR;
            HdrSettings = new NvHdrSettings(preset.HdrSettings);

            applyDithering = preset.applyDithering;
            ditheringEnabled = preset.ditheringEnabled;

            DisplayConfig = new DisplayConfig(preset.DisplayConfig);

            applyDriverSettings = preset.applyDriverSettings;
            driverSettings = new Dictionary<uint, uint>(preset.driverSettings);
            DriverProfileName = preset.DriverProfileName;

            SDRBrightness = preset.SDRBrightness;
            scaling = preset.scaling;
            ColorProfileSettings = new NvColorProfileSettings(preset.ColorProfileSettings);
            applyOther = preset.applyOther;

            HdmiInfoFrameSettings = new NvHdmiInfoFrameSettings(preset.HdmiInfoFrameSettings);
            applyHdmiSettings = preset.applyHdmiSettings;

            ColorEnhancementSettings = new NvColorEnhancementSettings(preset.ColorEnhancementSettings);
            ApplyColorEnhancements = preset.ApplyColorEnhancements;

            NovideoSettings = new NovideoSettings(preset.NovideoSettings);
            ApplyNovideoSettings = preset.ApplyNovideoSettings;

            MonitorConnectionType = preset.MonitorConnectionType;
        }

        public NvPreset Clone()
        {
            var preset = new NvPreset(this);

            return preset;
        }

        public NvPreset CloneWithId()
        {
            var preset = Clone();
            preset.SetId(id);
            preset.name = name;
            preset.shortcut = shortcut;

            return preset;
        }

        public static string[] GetColumnNames()
        {
            return new[] { "Name", "Display|140", "Color settings (BPC, format, dyn. range, color space)|260", "Color enhancements", "Refresh rate|120", "Resolution|150", "Dithering", "HDR", "Driver settings|300", "HDMI settings|200", "Other|200", "Overclocking|300", "Shortcut", "Apply on startup" };
        }

        public override List<string> GetDisplayValues(Config config = null)
        {
            var isCurrent = IsDisplayPreset;

            var values = new List<string>
            {
                isCurrent ? "Current settings" : name
            };

            var display = isCurrent ? displayName : string.Format("{0}", primaryDisplay ? "Primary" : !displayName.IsNullOrEmpty() ? displayName : DisplayId);
            values.Add(display);

            var colorSettings = FormatDisplaySetting(string.Format("{0}, {1}, {2}, {3}", colorData.ColorDepth, colorData.ColorFormat, colorData.DynamicRange, colorData.Colorimetry), isCurrent, applyColorData);

            values.Add(colorSettings);

            values.Add(FormatDisplaySetting(GetColorEnhancementsDescription(), isCurrent, ApplyColorEnhancements));

            values.Add(FormatDisplaySetting(string.Format("{0}Hz", DisplayConfig.RefreshRate), isCurrent, DisplayConfig.ApplyRefreshRate));

            if (DisplayConfig.ApplyResolution || DisplayConfig.Resolution.ActiveWidth > 0)
            {
                values.Add(FormatDisplaySetting(string.Format("{0}", DisplayConfig.GetResolutionDesc()), isCurrent, DisplayConfig.ApplyResolution));
            }
            else
            {
                values.Add(isCurrent ? "" : "Excluded");
            }

            var dithering = GetDitheringDescription();

            values.Add(FormatDisplaySetting(dithering, isCurrent, applyDithering));

            var hdrDesc = GetHdrDescription();
            values.Add(FormatDisplaySetting(hdrDesc, isCurrent, applyHDR));

            values.Add(FormatDisplaySetting(GetDriverSettingsDescription?.Invoke(driverSettings) ?? "Unknown", isCurrent, applyDriverSettings));

            values.Add(FormatDisplaySetting(GetHdmiSettingsDescription(), isCurrent, applyHdmiSettings));

            values.Add(FormatDisplaySetting(GetOtherDescription(), isCurrent, applyOther));

            values.Add(FormatDisplaySetting(GetOverclockingSettingsDescription(), isCurrent, applyOverclocking));

            values.Add(isCurrent ? "" : shortcut);

            values.Add(isCurrent ? "" : string.Format("{0}", config?.NvPresetId_ApplyOnStartup == id ? "Yes" : string.Empty));

            if (isCurrent)
            {
                InfoLine = string.Format("{0}: {1}, {2}Hz, {3}, HDR: {4}", displayName, colorSettings, DisplayConfig.RefreshRate, DisplayConfig.GetResolutionDesc(), HDREnabled ? "Yes" : "No");
            }

            return values;
        }

        public string GetHdrDescription()
        {
            var description = HDREnabled ? $"Enabled{(HdrSettings.OutputMode.HasValue ? (HdrSettings.OutputMode == NvHdrSettings.NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_HDR10PLUS_GAMING ? " (HDR10+)" : " (HDR10)") : "")}" : "";
            if (SDRBrightness.HasValue)
            {
                description += $", SDR brightness: {SDRBrightness.Value}%";
            }
            description = toggleHDR ? "Toggle" : HDREnabled ? description : "Disabled";

            return description;
        }

        private static string FormatDisplaySetting(string values, bool isCurrent, bool isEnabled)
        {
            return string.Format("{0}{1}", isCurrent ? "" : isEnabled ? "Included: " : "Excluded: ", values);
        }

        public string GetOverclockingSettingsDescription(bool useNewLines = false)
        {
            if (ocSettings.Count == 0)
            {
                return "None";
            }

            var values = new List<string>();

            foreach (var ocSetting in ocSettings)
            {
                var value = ocSetting.ToString();

                values.Add($"{value}");
            }

            return string.Join(useNewLines ? "\r\n" : ", ", values);
        }

        public string GetDitheringDescription(string disabledText = "Disabled")
        {
            if (ditheringEnabled == null && ditheringBits == 0 && ditheringMode == 0)
            {
                return "Auto: Disabled";
            }

            if (ditheringEnabled is null or true)
            {
                var ditherBitsDescription = ((NvDitherBits)ditheringBits).GetDescription();
                var ditherModeDescription = ((NvDitherMode)ditheringMode).GetDescription();
                return string.Format("{0}{1} {2}", ditheringEnabled == null ? "Auto: " : "", ditherBitsDescription, ditherModeDescription);
            }

            return disabledText;
        }

        public string GetHdmiSettingsDescription()
        {
            var values = new List<string>();

            if (HdmiInfoFrameSettings.Colorimetry.HasValue)
            {
                values.Add($"Colorimetry: {HdmiInfoFrameSettings.Colorimetry.Value}");
            }
            if (HdmiInfoFrameSettings.ExtendedColorimetry.HasValue)
            {
                values.Add($"Ext. colorimetry: {HdmiInfoFrameSettings.ExtendedColorimetry.Value}");
            }
            if (HdmiInfoFrameSettings.ContentType.HasValue)
            {
                values.Add($"Content type: {HdmiInfoFrameSettings.ContentType.Value}");
            }

            if (!values.Any())
            {
                values.Add("None");
            }

            return string.Join(", ", values);
        }

        public string GetOtherDescription()
        {
            var values = new List<string>();

            if (scaling.HasValue)
            {
                values.Add($"Scaling: {scaling.Value.GetDescription()}");
            }
            if (!ColorProfileSettings.ProfileName.IsNullOrWhiteSpace())
            {
                values.Add($"Color profile: {ColorProfileSettings.ProfileName}");
            }

            if (!values.Any())
            {
                values.Add("None");
            }

            return string.Join(", ", values);
        }

        public string GetColorEnhancementsDescription()
        {
            var values = new List<string>();

            if (ColorEnhancementSettings.DigitalVibranceLevel != 50)
            {
                values.Add($"Digital vibrance: {ColorEnhancementSettings.DigitalVibranceLevel}%");
            }
            if (ColorEnhancementSettings.HueAngle != 0)
            {
                values.Add($"Hue: {ColorEnhancementSettings.HueAngle}°");
            }

            if (!values.Any())
            {
                values.Add("None");
            }

            return string.Join(", ", values);
        }

        public static ColorData DefaultColorData => new ColorData(ColorDataFormat.Auto, dynamicRange: ColorDataDynamicRange.Auto, colorDepth: ColorDataDepth.Default, colorSelectionPolicy: ColorDataSelectionPolicy.BestQuality);

        public static List<NvPreset> GetDefaultPresets()
        {
            var presets = new List<NvPreset>
            {
                new NvPreset(DefaultColorData)
            };

            for (var dynamicRange = ColorDataDynamicRange.VESA; dynamicRange <= ColorDataDynamicRange.CEA; dynamicRange++)
            {
                presets.Add(new NvPreset(new ColorData(ColorDataFormat.RGB, dynamicRange: dynamicRange, colorDepth: ColorDataDepth.BPC8, colorSelectionPolicy: ColorDataSelectionPolicy.User)));
                presets.Add(new NvPreset(new ColorData(ColorDataFormat.RGB, dynamicRange: dynamicRange, colorDepth: ColorDataDepth.BPC10, colorSelectionPolicy: ColorDataSelectionPolicy.User)));
                presets.Add(new NvPreset(new ColorData(ColorDataFormat.RGB, dynamicRange: dynamicRange, colorDepth: ColorDataDepth.BPC12, colorSelectionPolicy: ColorDataSelectionPolicy.User)));
            }

            for (var format = ColorDataFormat.YUV422; format <= ColorDataFormat.YUV420; format++)
            {
                presets.Add(new NvPreset(new ColorData(format, dynamicRange: ColorDataDynamicRange.Auto, colorDepth: ColorDataDepth.BPC8, colorSelectionPolicy: ColorDataSelectionPolicy.User)));
                presets.Add(new NvPreset(new ColorData(format, dynamicRange: ColorDataDynamicRange.Auto, colorDepth: ColorDataDepth.BPC10, colorSelectionPolicy: ColorDataSelectionPolicy.User)));
                presets.Add(new NvPreset(new ColorData(format, dynamicRange: ColorDataDynamicRange.Auto, colorDepth: ColorDataDepth.BPC12, colorSelectionPolicy: ColorDataSelectionPolicy.User)));
            }

            return presets;
        }

        public override string GetTextForMenuItem()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(name))
            {
                sb.Append(name);
                sb.Append(" / ");
            }
            else
            {
                if (displayName != null)
                {
                    sb.AppendFormat("Display: {0} / ", displayName);
                }
                if (applyColorData)
                {
                    var colorSettings = string.Format("Format: {0}, {1}, {2}", colorData.ColorDepth, colorData.ColorFormat, colorData.DynamicRange);
                    sb.Append(colorSettings);
                    sb.Append(" / ");
                }
                if (DisplayConfig.ApplyRefreshRate)
                {
                    sb.AppendFormat("{0}Hz", DisplayConfig.RefreshRate);
                    sb.Append(" / ");
                }
                if (DisplayConfig.ApplyResolution)
                {
                    sb.AppendFormat("{0}", DisplayConfig.GetResolutionDesc());
                    sb.Append(" / ");
                }
                if (applyDithering)
                {
                    var dithering = GetDitheringDescription("No");
                    sb.AppendFormat("Dithering: {0}", dithering);
                    sb.Append(" / ");
                }
                if (applyHDR)
                {
                    sb.AppendFormat("HDR: {0}", toggleHDR ? "Toggle" : HDREnabled ? "Enabled" : "Disabled");
                    sb.Append(" / ");
                }
                if (applyDriverSettings)
                {
                    sb.AppendFormat("DRS: {0}", GetDriverSettingsDescription?.Invoke(driverSettings) ?? "Unknown");
                    sb.Append(" / ");
                }
            }

            return sb.ToString(0, Math.Max(0, sb.Length - 3));
        }

        public static ColorData GenerateColorData(IDictionary<string, object> dictionary)
        {
            var format = ColorDataFormat.Auto;
            var dynamicRange = ColorDataDynamicRange.Auto;
            var colorDepth = ColorDataDepth.Default;
            var colorimetry = ColorDataColorimetry.Auto;
            var selectionPolicy = ColorDataSelectionPolicy.User;
            object value;
            if (dictionary.TryGetValue("ColorFormat", out value) && value != null)
            {
                format = (ColorDataFormat)Enum.ToObject(typeof(ColorDataFormat), value);
            }
            if (dictionary.TryGetValue("ColorDepth", out value) && value != null)
            {
                colorDepth = (ColorDataDepth)Enum.ToObject(typeof(ColorDataDepth), value);
            }
            if (dictionary.TryGetValue("Colorimetry", out value) && value != null)
            {
                colorimetry = (ColorDataColorimetry)Enum.ToObject(typeof(ColorDataColorimetry), value);
            }
            if (dictionary.TryGetValue("DynamicRange", out value) && value != null)
            {
                dynamicRange = (ColorDataDynamicRange)Enum.ToObject(typeof(ColorDataDynamicRange), value);
            }
            if (dictionary.TryGetValue("SelectionPolicy", out _))
            {
                selectionPolicy =
                    colorDepth == ColorDataDepth.Default &&
                    format >= ColorDataFormat.Default &&
                    colorimetry >= ColorDataColorimetry.Default &&
                    dynamicRange == ColorDataDynamicRange.Auto ?
                        ColorDataSelectionPolicy.BestQuality : selectionPolicy;
            }
            return new ColorData(format, dynamicRange: dynamicRange, colorimetry: colorimetry, colorDepth: colorDepth, colorSelectionPolicy: selectionPolicy, desktopColorDepth: ColorDataDesktopDepth.Default);
        }

        public void UpdateDriverSetting(uint settingId, uint settingValue)
        {
            driverSettings[settingId] = settingValue;
        }

        public void UpdateDriverSettings(IEnumerable<NvSettingItemDto> settings)
        {
            var changedSettings = settings.Where(s => s.Value != NvSettingConstants.UnsetDwordValue);
            var changedSettingsDictionary = changedSettings.ToDictionary(k => k.SettingId, v => v.Value);

            driverSettings.Clear();

            foreach (var key in changedSettingsDictionary.Keys)
            {
                driverSettings.Add(key, changedSettingsDictionary[key]);
            }
        }

        public void ResetDriverSetting(uint settingId)
        {
            driverSettings.Remove(settingId);
        }

        public void UpdateOverclockingSetting(NvGpuOcSettings newSettings)
        {
            var index = ocSettings.FindIndex(s => s.PCIIdentifier == newSettings.PCIIdentifier);

            if (index >= 0)
            {
                ocSettings[index] = newSettings;
            }
            else
            {
                ocSettings.Add(newSettings);
            }
        }
    }
}

