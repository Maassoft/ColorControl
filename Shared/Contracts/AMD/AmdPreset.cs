using System.Text;
using System.Text.Json.Serialization;

namespace ColorControl.Shared.Contracts.AMD;

public class AmdPreset : PresetBase
{
    public bool applyColorData { get; set; }
    public ADLColorDepth colorDepth { get; set; }
    public ADLPixelFormat pixelFormat { get; set; }
    public bool applyHDR { get; set; }
    public bool HDREnabled { get; set; }
    public bool toggleHDR { get; set; }
    public int? SDRBrightness { get; set; }
    public bool applyDithering { get; set; }
    public ADLDitherState ditherState { get; set; }
    public DisplayConfig DisplayConfig { get; set; }
    public bool primaryDisplay { get; set; }
    public string displayName { get; set; }
    public string DisplayId { get; set; }
    public bool IsDisplayPreset { get; set; }

    [JsonIgnore]
    public string InfoLine { get; private set; }

    public AmdPreset() : base()
    {
        applyColorData = true;
        applyDithering = false;
        ditherState = ADLDitherState.DRIVER_DEFAULT;
        applyHDR = false;
        HDREnabled = false;
        toggleHDR = false;
        primaryDisplay = true;
        DisplayConfig = new DisplayConfig();
    }

    public AmdPreset(AmdPreset preset) : this()
    {
        id = GetNewId();

        primaryDisplay = preset.primaryDisplay;
        displayName = preset.displayName;
        DisplayId = preset.DisplayId;

        applyColorData = preset.applyColorData;
        colorDepth = preset.colorDepth;
        pixelFormat = preset.pixelFormat;

        applyHDR = preset.applyHDR;
        HDREnabled = preset.HDREnabled;
        toggleHDR = preset.toggleHDR;
        SDRBrightness = preset.SDRBrightness;

        applyDithering = preset.applyDithering;
        ditherState = preset.ditherState;
        DisplayConfig = new DisplayConfig(DisplayConfig);
    }

    public AmdPreset Clone()
    {
        var preset = new AmdPreset(this);

        return preset;
    }

    public AmdPreset CloneWithId()
    {
        var preset = new AmdPreset(this);
        preset.SetId(id);
        preset.name = name;
        preset.shortcut = shortcut;

        return preset;
    }

    public static string[] GetColumnNames()
    {
        return new[] { "Name", "Display|140", "Color settings (BPC, format)|260", "Refresh rate|100", "Resolution|120", "Dithering", "HDR", "Shortcut", "Apply on startup" };
    }

    public override List<string> GetDisplayValues(Config config = null)
    {
        var isCurrent = IsDisplayPreset;

        var values = new List<string>
        {
            isCurrent ? "Current settings" : name
        };

        var fullDisplayName = displayName;
        if (DisplayId != null)
        {
            fullDisplayName += $" ({DisplayId})";
        }
        var display = string.Format("{0}", primaryDisplay && !IsDisplayPreset ? "Primary" : fullDisplayName);
        values.Add(display);

        var colorSettings = string.Format("{0}: {1}, {2}", applyColorData ? "Included" : "Excluded", colorDepth, pixelFormat);

        values.Add(colorSettings);
        values.Add(string.Format("{0}: {1}Hz", DisplayConfig.ApplyRefreshRate ? "Included" : "Excluded", DisplayConfig.RefreshRate));
        values.Add(string.Format("{0}{1}", DisplayConfig.ApplyResolution ? "Included" : "Excluded", DisplayConfig.ApplyResolution ? ": " + DisplayConfig.GetResolutionDesc() : ""));
        values.Add(string.Format("{0}: {1}", applyDithering ? "Included" : "Excluded", ditherState));
        values.Add(string.Format("{0}: {1}", applyHDR ? "Included" : "Excluded", toggleHDR ? "Toggle" : HDREnabled ? "Enabled" : "Disabled"));
        values.Add(shortcut);
        values.Add(string.Format("{0}", config?.AmdPresetId_ApplyOnStartup == id ? "Yes" : string.Empty));

        if (isCurrent)
        {
            InfoLine = string.Format("{0}: {1}, {2}Hz, {3}, HDR: {4}", name, colorSettings, DisplayConfig.RefreshRate, DisplayConfig.GetResolutionDesc(), HDREnabled ? "Yes" : "No");
        }

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

