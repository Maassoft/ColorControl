using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColorControl
{
    class NvPreset : PresetBase
    {
        public bool applyColorData { get; set; }
        public ColorData colorData { get; set; }
        public bool applyHDR { get; set; }
        public bool HDREnabled { get; set; }
        public bool toggleHDR { get; set; }
        public bool applyDithering { get; set; }
        public bool ditheringEnabled { get; set; }
        public bool applyRefreshRate { get; set; }
        public uint refreshRate { get; set; }
        public bool primaryDisplay { get; set; }
        public string displayName { get; set; }
        public uint ditheringBits { get; set; }
        public uint ditheringMode { get; set; }

        public NvPreset()
        {
            applyColorData = true;
            applyDithering = false;
            ditheringEnabled = true;
            applyHDR = false;
            HDREnabled = false;
            toggleHDR = false;
            applyRefreshRate = false;
            refreshRate = 60;
            primaryDisplay = true;
            ditheringBits = 1;
            ditheringMode = 4;
        }

        public NvPreset(ColorData colorData) : this()
        {
            id = GetNewId();
            this.colorData = colorData;
        }

        public NvPreset(NvPreset preset): this()
        {
            id = GetNewId();

            primaryDisplay = preset.primaryDisplay;
            displayName = preset.displayName;

            var colorData = preset.colorData;
            this.colorData = new ColorData(colorData.ColorFormat, dynamicRange: colorData.DynamicRange, colorDepth: colorData.ColorDepth, colorSelectionPolicy: colorData.SelectionPolicy);
            applyColorData = preset.applyColorData;

            applyHDR = preset.applyHDR;
            HDREnabled = preset.HDREnabled;
            toggleHDR = preset.toggleHDR;
            applyDithering = preset.applyDithering;
            ditheringEnabled = preset.ditheringEnabled;
            applyRefreshRate = preset.applyRefreshRate;
            refreshRate = preset.refreshRate;
        }

        public NvPreset Clone()
        {
            var preset = new NvPreset(this);

            return preset;
        }

        public static string[] GetColumnNames()
        {
            //return new[] { "BPC", "Format", "Dynamic range", "Toggle HDR", "Shortcut" };
            return new[] { "Display|140", "Color settings (BPC, format, dyn. range, color space)|260", "Refresh rate|100", "Dithering", "HDR", "Shortcut" };
        }

        public List<string> GetDisplayValues()
        {
            var values = new List<string>();

            var display = string.Format("{0}", primaryDisplay ? "Primary" : displayName);
            values.Add(display);

            var colorSettings = string.Format("{0}: {1}, {2}, {3}, {4}", applyColorData ? "Included" : "Excluded", colorData.ColorDepth, colorData.ColorFormat, colorData.DynamicRange, colorData.Colorimetry);

            values.Add(colorSettings);
            values.Add(string.Format("{0}: {1}Hz", applyRefreshRate ? "Included" : "Excluded", refreshRate));

            var dithering = ditheringEnabled ? string.Empty : "Disabled";
            if (ditheringEnabled)
            {
                var ditherBitsDescription = ((NvDitherBits)ditheringBits).GetDescription();
                var ditherModeDescription = ((NvDitherMode)ditheringMode).GetDescription();
                dithering = string.Format("{0} {1}", ditherBitsDescription, ditherModeDescription);
            }

            values.Add(string.Format("{0}: {1}", applyDithering ? "Included" : "Excluded", dithering));
            values.Add(string.Format("{0}: {1}", applyHDR ? "Included" : "Excluded", toggleHDR ? "Toggle" : HDREnabled ? "Enabled" : "Disabled"));

            //values.Add(colorData.ColorDepth.ToString());
            //values.Add(colorData.ColorFormat.ToString());
            //values.Add(colorData.DynamicRange.ToString());
            //values.Add(toggleHDR.ToString());
            values.Add(shortcut);

            return values;
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

        public string GetTextForMenuItem()
        {
            var sb = new StringBuilder();
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
            if (applyDithering)
            {
                var dithering = ditheringEnabled ? string.Empty : "No";
                sb.AppendFormat("Dithering: {0}", dithering);
                if (ditheringEnabled)
                {
                    var ditherBitsDescription = ((NvDitherBits)ditheringBits).GetDescription();
                    var ditherModeDescription = ((NvDitherMode)ditheringMode).GetDescription();
                    sb.AppendFormat("{0} {1}", ditherBitsDescription, ditherModeDescription);
                }
                sb.Append(" / ");
            }
            if (applyHDR)
            {
                sb.AppendFormat("HDR: {0}", toggleHDR ? "Toggle" : HDREnabled ? "Enabled" : "Disabled");
                sb.Append(" / ");
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
            if (dictionary.TryGetValue("SelectionPolicy", out value))
            {
                selectionPolicy = (ColorDataSelectionPolicy)Enum.ToObject(typeof(ColorDataSelectionPolicy), value);
            }
            return new ColorData(format, dynamicRange: dynamicRange, colorimetry: colorimetry, colorDepth: colorDepth, colorSelectionPolicy: selectionPolicy);
        }

    }
}

