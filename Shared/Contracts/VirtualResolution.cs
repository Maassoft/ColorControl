namespace ColorControl.Shared.Contracts;

public class VirtualResolution
{
    public uint ActiveWidth { get; set; }
    public uint ActiveHeight { get; set; }
    public uint VirtualWidth { get; set; }
    public uint VirtualHeight { get; set; }

    public VirtualResolution(uint activeWidth, uint activeHeight)
    {
        ActiveWidth = activeWidth;
        ActiveHeight = activeHeight;
        VirtualWidth = activeWidth;
        VirtualHeight = activeHeight;

    }

    public VirtualResolution()
    {
    }

    public VirtualResolution(VirtualResolution resolution)
    {
        ActiveWidth = resolution.ActiveWidth;
        ActiveHeight = resolution.ActiveHeight;
        VirtualWidth = resolution.VirtualWidth;
        VirtualHeight = resolution.VirtualHeight;
    }

    public override string ToString()
    {
        if (ActiveWidth == 0 && ActiveHeight == 0)
        {
            return "Not set";
        }

        if (VirtualWidth == 0 && VirtualHeight == 0)
        {
            return GetActiveString();
        }

        return ActiveWidth == VirtualWidth && ActiveHeight == VirtualHeight ?
            GetActiveString() :
            $"{GetVirtualString()} ({GetActiveString()})";
    }

    public string GetActiveString()
    {
        return $"{ActiveWidth}x{ActiveHeight}";
    }

    public string GetVirtualString()
    {
        return $"{VirtualWidth}x{VirtualHeight}";
    }

    public bool IsDifferent(VirtualResolution resolution)
    {
        return ActiveWidth != resolution.ActiveWidth || ActiveHeight != resolution.ActiveHeight || VirtualWidth != resolution.VirtualWidth || VirtualHeight != resolution.VirtualHeight;
    }
}
