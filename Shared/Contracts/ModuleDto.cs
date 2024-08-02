namespace ColorControl.Shared.Contracts;

public class ModuleDto
{
    public bool IsActive { get; set; }
    public string DisplayName { get; set; }
    public string Link { get; set; }
    public List<string> Info { get; set; }
}
