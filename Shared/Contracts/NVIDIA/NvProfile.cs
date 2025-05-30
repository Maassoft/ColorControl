namespace ColorControl.Shared.Contracts.NVIDIA;

public class NvProfile
{
    public bool IsBase { get; set; }
    public string Name { get; set; }

    public List<string> Apps { get; set; }

    public NvProfile()
    {
        Apps = [];
    }
}
