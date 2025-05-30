using NWin32;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ColorControl.Shared.Contracts.Game;

public enum GameStepType
{
    [Description("Pre-launch steps")]
    PreLaunch = 0,
    [Description("Post-launch steps")]
    PostLaunch = 1,
    [Description("Finalize steps")]
    Finalize = 2,
}

public enum GamePriorityClass
{
    Idle = NativeConstants.IDLE_PRIORITY_CLASS,
    BelowNormal = NativeConstants.BELOW_NORMAL_PRIORITY_CLASS,
    Normal = NativeConstants.NORMAL_PRIORITY_CLASS,
    AboveNormal = NativeConstants.ABOVE_NORMAL_PRIORITY_CLASS,
    High = NativeConstants.HIGH_PRIORITY_CLASS,
    RealTime = NativeConstants.REALTIME_PRIORITY_CLASS
}

public enum ProcessAutoAction
{
    None = 0,
    Suspend = 1,
    Restart = 2
}

public class AutoSettings
{
    public bool AllowAutoApply { get; set; }
    public ProcessAutoAction ProcessAutoAction { get; set; }

    public void Update(AutoSettings autoSettings)
    {
        AllowAutoApply = autoSettings.AllowAutoApply;
        ProcessAutoAction = autoSettings.ProcessAutoAction;
    }
}

public class GamePreset : PresetBase
{
    public string Path { get; set; }
    public string Parameters { get; set; }
    public bool RunAsAdministrator { get; set; }
    public List<string> PreLaunchSteps { get; set; }
    public List<string> PostLaunchSteps { get; set; }
    public List<string> FinalizeSteps { get; set; }
    public uint ProcessAffinityMask { get; set; }
    public uint ProcessPriorityClass { get; set; }

    [JsonIgnore]
    public GamePriorityClass GameProcessPriorityClass
    {
        get
        {
            return (GamePriorityClass)ProcessPriorityClass;
        }
        set
        {
            ProcessPriorityClass = (uint)value;
        }
    }

    public AutoSettings AutoSettings { get; set; }

    public GamePreset() : base()
    {
        PreLaunchSteps = new List<string>();
        PostLaunchSteps = new List<string>();
        FinalizeSteps = new List<string>();
        AutoSettings = new AutoSettings();
    }

    public GamePreset(GamePreset preset) : this()
    {
        id = GetNewId();

        Update(preset);
    }

    public void Update(GamePreset preset)
    {
        name = preset.name;
        ShowInQuickAccess = preset.ShowInQuickAccess;
        Path = preset.Path;
        Parameters = preset.Parameters;
        RunAsAdministrator = preset.RunAsAdministrator;
        ProcessAffinityMask = preset.ProcessAffinityMask;
        ProcessPriorityClass = preset.ProcessPriorityClass;
        AutoSettings.Update(preset.AutoSettings);
        PreLaunchSteps.Clear();
        PreLaunchSteps.AddRange(preset.PreLaunchSteps);
        PostLaunchSteps.Clear();
        PostLaunchSteps.AddRange(preset.PostLaunchSteps);
        FinalizeSteps.Clear();
        FinalizeSteps.AddRange(preset.FinalizeSteps);
    }

    public GamePreset Clone()
    {
        var preset = new GamePreset(this);

        preset.name += " (copy)";

        return preset;
    }

    public GamePreset CloneWithId()
    {
        var preset = new GamePreset(this);
        preset.SetId(id);
        preset.name = name;
        preset.shortcut = shortcut;

        return preset;
    }

    public static string[] GetColumnNames()
    {
        return new[] { "Name|160", "File/URI|400", "Parameters|200", "Pre-launch steps|300", "Post-launch steps|300", "Finalize steps|300" };
    }

    public override List<string> GetDisplayValues(Config config = null)
    {
        var values = new List<string>
        {
            name,
            Path,
            Parameters,

            string.Join(", ", PreLaunchSteps),
            string.Join(", ", PostLaunchSteps),
            string.Join(", ", FinalizeSteps)
        };

        return values;
    }
}

