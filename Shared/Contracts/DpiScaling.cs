namespace ColorControl.Shared.Contracts;

public class DpiScaling
{
    public bool ApplyScaling { get; set; }
    public uint Percentage { get; set; }

    public DpiScaling()
    {
        Percentage = 100;
    }

    public DpiScaling(DpiScaling dpiScaling)
    {
        ApplyScaling = dpiScaling.ApplyScaling;
        Percentage = dpiScaling.Percentage;
    }

    public override string ToString()
    {
        return $"{Percentage:0}%";
    }
}
