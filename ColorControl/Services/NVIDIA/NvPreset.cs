using ColorControl.Common;
using ColorControl.Services.Common;
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
        public bool ditheringEnabled { get; set; }
        public bool applyRefreshRate { get; set; }
        public uint refreshRate { get; set; }
        public bool applyResolution { get; set; }
        public uint resolutionWidth { get; set; }
        public uint resolutionHeight { get; set; }
        public bool primaryDisplay { get; set; }
        public string displayName { get; set; }
        public uint ditheringBits { get; set; }
        public uint ditheringMode { get; set; }
        public bool applyDriverSettings { get; set; }
        public Dictionary<uint, uint> driverSettings { get; set; }

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
        }

        public NvPreset Clone()
        {
            var preset = new NvPreset(this);

            return preset;
        }

        public static string[] GetColumnNames()
        {
            return new[] { "Name", "Display|140", "Color settings (BPC, format, dyn. range, color space)|260", "Refresh rate|100", "Resolution|120", "Dithering", "HDR", "Driver settings|300", "Shortcut", "Apply on startup" };
        }

        public override List<string> GetDisplayValues(Config config = null)
        {
            var values = new List<string>();

            values.Add(name);

            var display = string.Format("{0}", primaryDisplay ? "Primary" : displayName);
            values.Add(display);

            var colorSettings = string.Format("{0}: {1}, {2}, {3}, {4}", applyColorData ? "Included" : "Excluded", colorData.ColorDepth, colorData.ColorFormat, colorData.DynamicRange, colorData.Colorimetry);

            values.Add(colorSettings);
            values.Add(string.Format("{0}: {1}Hz", applyRefreshRate ? "Included" : "Excluded", refreshRate));

            if (applyResolution || resolutionWidth > 0)
            {
                values.Add(string.Format("{0}: {1}x{2}", applyResolution ? "Included" : "Excluded", resolutionWidth, resolutionHeight));
            }
            else
            {
                values.Add(string.Format("Excluded"));
            }

            var dithering = GetDitheringDescription();

            values.Add(string.Format("{0}: {1}", applyDithering ? "Included" : "Excluded", dithering));
            values.Add(string.Format("{0}: {1}", applyHDR ? "Included" : "Excluded", toggleHDR ? "Toggle" : HDREnabled ? "Enabled" : "Disabled"));

            values.Add(string.Format("{0}: {1}", applyDriverSettings ? "Included" : "Excluded", GetDriverSettingsDescription()));

            values.Add(shortcut);

            values.Add(string.Format("{0}", config?.NvPresetId_ApplyOnStartup == id ? "Yes" : string.Empty));

            return values;
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

                var settingValue = settingMeta.DwordValues.FirstOrDefault(s => s.Value == driverSetting.Value);

                values.Add($"{settingMeta.SettingName}: {settingValue?.ValueName ?? "Unknown"}");
            }

            return string.Join(useNewLines ? "\r\n" : ", ", values);
        }

        public string GetDitheringDescription(string disabledText = "Disabled")
        {
            var dithering = ditheringEnabled ? string.Empty : disabledText;
            if (ditheringEnabled)
            {
                var ditherBitsDescription = ((NvDitherBits)ditheringBits).GetDescription();
                var ditherModeDescription = ((NvDitherMode)ditheringMode).GetDescription();
                dithering = string.Format("{0} {1}", ditherBitsDescription, ditherModeDescription);
            }
            return dithering;
        }

        public static List<NvPreset> GetDefaultPresets()
        {
            var presets = new List<NvPreset>();

            presets.Add(new NvPreset(new ColorData(ColorDataFormat.Auto, dynamicRange: ColorDataDynamicRange.Auto, colorDepth: ColorDataDepth.Default, colorSelectionPolicy: ColorDataSelectionPolicy.BestQuality)));

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
    }
}

