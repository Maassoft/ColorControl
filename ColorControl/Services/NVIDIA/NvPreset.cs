using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.NVIDIA;
using Newtonsoft.Json;
using nspector.Common;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColorControl.Services.NVIDIA
{
    class NvPreset : PresetBase
    {
        public static NvService NvService { get; set; }

        public bool applyColorData { get; set; }
        public ColorData colorData { get; set; }
        public bool applyHDR { get; set; }
        public bool HDREnabled { get; set; }
        public bool toggleHDR { get; set; }
        public bool applyDithering { get; set; }
        public bool? ditheringEnabled { get; set; }
        public bool applyRefreshRate { get; set; }
        public uint refreshRate { get; set; }
        public bool applyResolution { get; set; }
        public uint resolutionWidth { get; set; }
        public uint resolutionHeight { get; set; }
        public bool primaryDisplay { get; set; }
        public string displayName { get; set; }
        public uint ditheringBits { get; set; }
        public uint ditheringMode { get; set; }
        public bool applyOther { get; set; }
        public int? SDRBrightness { get; set; }
        public bool applyDriverSettings { get; set; }
        public Dictionary<uint, uint> driverSettings { get; set; }
        public bool applyOverclocking { get; set; }
        public List<NvGpuOcSettings> ocSettings { get; set; }

        public bool applyHdmiSettings { get; set; }
        public NvHdmiInfoFrameSettings HdmiInfoFrameSettings { get; set; }

        [JsonIgnore]
        public bool IsDisplayPreset => Display != null;
        [JsonIgnore]
        public Display Display { get; set; }
        [JsonIgnore]
        public string InfoLine { get; set; }
        public NvDitherState DitherState => ditheringEnabled.HasValue ? ditheringEnabled == true ? NvDitherState.Enabled : NvDitherState.Disabled : NvDitherState.Auto;

        public NvPreset() : base()
        {
            applyColorData = true;
            applyDithering = false;
            ditheringEnabled = true;
            applyHDR = false;
            HDREnabled = false;
            toggleHDR = false;
            applyRefreshRate = false;
            refreshRate = 60;
            applyResolution = false;
            resolutionWidth = 0;
            resolutionHeight = 0;
            primaryDisplay = true;
            ditheringBits = 1;
            ditheringMode = 4;
            applyDriverSettings = false;
            driverSettings = new Dictionary<uint, uint>();
            ocSettings = new List<NvGpuOcSettings>();
            HdmiInfoFrameSettings = new NvHdmiInfoFrameSettings();
        }

        public NvPreset(ColorData colorData) : this()
        {
            id = GetNewId();
            this.colorData = colorData;
        }

        public NvPreset(NvPreset preset) : this()
        {
            id = GetNewId();

            primaryDisplay = preset.primaryDisplay;
            displayName = preset.displayName;

            var colorData = preset.colorData;
            this.colorData = new ColorData(colorData.ColorFormat, dynamicRange: colorData.DynamicRange, colorDepth: colorData.ColorDepth, colorSelectionPolicy: ColorDataSelectionPolicy.User);
            applyColorData = preset.applyColorData;

            applyHDR = preset.applyHDR;
            HDREnabled = preset.HDREnabled;
            toggleHDR = preset.toggleHDR;
            applyDithering = preset.applyDithering;
            ditheringEnabled = preset.ditheringEnabled;
            applyRefreshRate = preset.applyRefreshRate;
            refreshRate = preset.refreshRate;
            applyResolution = preset.applyResolution;
            resolutionWidth = preset.resolutionWidth;
            resolutionHeight = preset.resolutionHeight;
            applyDriverSettings = preset.applyDriverSettings;
            driverSettings = new Dictionary<uint, uint>(preset.driverSettings);

            SDRBrightness = preset.SDRBrightness;
            applyOther = preset.applyOther;

            HdmiInfoFrameSettings = new NvHdmiInfoFrameSettings(preset.HdmiInfoFrameSettings);
            applyHdmiSettings = preset.applyHdmiSettings;
        }

        public NvPreset Clone()
        {
            var preset = new NvPreset(this);

            return preset;
        }

        public static string[] GetColumnNames()
        {
            return new[] { "Name", "Display|140", "Color settings (BPC, format, dyn. range, color space)|260", "Refresh rate|100", "Resolution|120", "Dithering", "HDR", "Driver settings|300", "HDMI settings|200", "Other|200", "Overclocking|300", "Shortcut", "Apply on startup" };
        }

        public override List<string> GetDisplayValues(Config config = null)
        {
            var isCurrent = IsDisplayPreset;

            var values = new List<string>
            {
                isCurrent ? "Current settings" : name
            };

            var display = isCurrent ? displayName : string.Format("{0}", primaryDisplay ? "Primary" : displayName);
            values.Add(display);

            var colorSettings = FormatDisplaySetting(string.Format("{0}, {1}, {2}, {3}", colorData.ColorDepth, colorData.ColorFormat, colorData.DynamicRange, colorData.Colorimetry), isCurrent, applyColorData);

            values.Add(colorSettings);
            values.Add(FormatDisplaySetting(string.Format("{0}Hz", refreshRate), isCurrent, applyRefreshRate));

            if (applyResolution || resolutionWidth > 0)
            {
                values.Add(FormatDisplaySetting(string.Format("{0}x{1}", resolutionWidth, resolutionHeight), isCurrent, applyResolution));
            }
            else
            {
                values.Add(isCurrent ? "" : "Excluded");
            }

            var dithering = GetDitheringDescription();

            values.Add(FormatDisplaySetting(dithering, isCurrent, applyDithering));
            values.Add(FormatDisplaySetting(toggleHDR ? "Toggle" : HDREnabled ? "Enabled" : "Disabled", isCurrent, applyHDR));

            values.Add(FormatDisplaySetting(GetDriverSettingsDescription(), isCurrent, applyDriverSettings));

            values.Add(FormatDisplaySetting(GetHdmiSettingsDescription(), isCurrent, applyHdmiSettings));

            values.Add(FormatDisplaySetting(GetOtherDescription(), isCurrent, applyOther));

            values.Add(FormatDisplaySetting(GetOverclockingSettingsDescription(), isCurrent, applyOverclocking));

            values.Add(isCurrent ? "" : shortcut);

            values.Add(isCurrent ? "" : string.Format("{0}", config?.NvPresetId_ApplyOnStartup == id ? "Yes" : string.Empty));

            if (isCurrent)
            {
                InfoLine = string.Format("{0}: {1}, {2}Hz, HDR: {3}", displayName, colorSettings, refreshRate, HDREnabled ? "Yes" : "No");
            }

            return values;
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

        public string GetDriverSettingsDescription(bool useNewLines = false)
        {
            if (driverSettings.Count == 0)
            {
                return "None";
            }

            var values = new List<string>();

            foreach (var driverSetting in driverSettings)
            {
                var settingMeta = NvService.GetSettingMeta(driverSetting.Key);

                var value = settingMeta.ToFriendlyName(intValue: driverSetting.Value, displayDefault: true);

                if (value == null)
                {
                    continue;
                }

                values.Add($"{settingMeta.SettingName}: {value}");
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

            if (SDRBrightness.HasValue && (HDREnabled || !IsDisplayPreset))
            {
                values.Add($"SDR brightness: {SDRBrightness.Value}%");
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
                if (applyRefreshRate)
                {
                    sb.AppendFormat("{0}Hz", refreshRate);
                    sb.Append(" / ");
                }
                if (applyResolution)
                {
                    sb.AppendFormat("{0}x{1}", resolutionWidth, resolutionHeight);
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
                    sb.AppendFormat("DRS: {0}", GetDriverSettingsDescription());
                    sb.Append(" / ");
                }
            }

            return sb.ToString(0, Math.Max(0, sb.Length - 3));
        }

        public static ColorData GenerateColorData(IDictionary<string, object> dictionary)
        {
            var format = ColorDataFormat.RGB;
            var dynamicRange = ColorDataDynamicRange.VESA;
            var colorDepth = ColorDataDepth.BPC8;
            var colorimetry = ColorDataColorimetry.Auto;
            var selectionPolicy = ColorDataSelectionPolicy.User;
            object value;
            if (dictionary.TryGetValue("ColorFormat", out value))
            {
                format = (ColorDataFormat)Enum.ToObject(typeof(ColorDataFormat), value);
            }
            if (dictionary.TryGetValue("ColorDepth", out value))
            {
                colorDepth = (ColorDataDepth)Enum.ToObject(typeof(ColorDataDepth), value);
            }
            if (dictionary.TryGetValue("Colorimetry", out value))
            {
                colorimetry = (ColorDataColorimetry)Enum.ToObject(typeof(ColorDataColorimetry), value);
            }
            if (dictionary.TryGetValue("DynamicRange", out value))
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

        internal void UpdateDriverSetting(SettingItem settingItem, uint settingValue)
        {
            driverSettings[settingItem.SettingId] = settingValue;
        }

        internal void ResetDriverSetting(uint settingId)
        {
            driverSettings.Remove(settingId);
        }

        internal void UpdateOverclockingSetting(NvGpuOcSettings newSettings)
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

