using ColorControl.Services.Common;
using ColorControl.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColorControl.Services.AMD
{
    class AmdPreset : PresetBase
    {
        public bool applyColorData { get; set; }
        public ADLColorDepth colorDepth { get; set; }
        public ADLPixelFormat pixelFormat { get; set; }
        public bool applyHDR { get; set; }
        public bool HDREnabled { get; set; }
        public bool toggleHDR { get; set; }
        public bool applyDithering { get; set; }
        public ADLDitherState ditherState { get; set; }
        public bool applyRefreshRate { get; set; }
        public uint refreshRate { get; set; }
        public bool primaryDisplay { get; set; }
        public string displayName { get; set; }

        public AmdPreset() : base()
        {
            applyColorData = true;
            applyDithering = false;
            ditherState = ADLDitherState.DRIVER_DEFAULT;
            applyHDR = false;
            HDREnabled = false;
            toggleHDR = false;
            applyRefreshRate = false;
            refreshRate = 60;
            primaryDisplay = true;
        }

        public AmdPreset(AmdPreset preset) : this()
        {
            id = GetNewId();

            primaryDisplay = preset.primaryDisplay;
            displayName = preset.displayName;

            applyColorData = preset.applyColorData;
            colorDepth = preset.colorDepth;
            pixelFormat = preset.pixelFormat;

            applyHDR = preset.applyHDR;
            HDREnabled = preset.HDREnabled;
            toggleHDR = preset.toggleHDR;
            applyDithering = preset.applyDithering;
            ditherState = preset.ditherState;
            applyRefreshRate = preset.applyRefreshRate;
            refreshRate = preset.refreshRate;
        }

        public AmdPreset Clone()
        {
            var preset = new AmdPreset(this);

            return preset;
        }

        public static string[] GetColumnNames()
        {
            //return new[] { "BPC", "Format", "Dynamic range", "Toggle HDR", "Shortcut" };
            return new[] { "Name", "Display|140", "Color settings (BPC, format)|260", "Refresh rate|100", "Dithering", "HDR", "Shortcut", "Apply on startup" };
        }

        public override List<string> GetDisplayValues(Config config = null)
        {
            var values = new List<string>();

            values.Add(name);

            var display = string.Format("{0}", primaryDisplay ? "Primary" : displayName);
            values.Add(display);

            var colorSettings = string.Format("{0}: {1}, {2}", applyColorData ? "Included" : "Excluded", colorDepth, pixelFormat);

            values.Add(colorSettings);
            values.Add(string.Format("{0}: {1}Hz", applyRefreshRate ? "Included" : "Excluded", refreshRate));
            values.Add(string.Format("{0}: {1}", applyDithering ? "Included" : "Excluded", ditherState));
            values.Add(string.Format("{0}: {1}", applyHDR ? "Included" : "Excluded", toggleHDR ? "Toggle" : HDREnabled ? "Enabled" : "Disabled"));
            values.Add(shortcut);
            values.Add(string.Format("{0}", config?.AmdPresetId_ApplyOnStartup == id ? "Yes" : string.Empty));

            return values;
        }

        public static List<AmdPreset> GetDefaultPresets()
        {
            var presets = new List<AmdPreset>();

            var preset = new AmdPreset();
            preset.colorDepth = ADLColorDepth.BPC8;
            preset.pixelFormat = ADLPixelFormat.RGB_FULL_RANGE;

            presets.Add(preset);

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
                    var colorSettings = string.Format("Format: {0}, {1}", colorDepth, pixelFormat);
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
                    sb.AppendFormat("Dithering: {0}", ditherState);
                    sb.Append(" / ");
                }
                if (applyHDR)
                {
                    sb.AppendFormat("HDR: {0}", toggleHDR ? "Toggle" : HDREnabled ? "Enabled" : "Disabled");
                    sb.Append(" / ");
                }
            }

            return sb.ToString(0, Math.Max(0, sb.Length - 3));
        }
    }
}

