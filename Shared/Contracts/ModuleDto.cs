namespace ColorControl.Shared.Contracts;

public class ModuleDto
{
	public bool IsActive { get; set; }
	public string DisplayName { get; set; }
	public string Link { get; set; }
	public List<string> Info { get; set; }

	public string GetIconClass()
	{
		if (DisplayName.Contains("NVIDIA"))
		{
			return "nvidia";
		}
		if (DisplayName.Contains("AMD"))
		{
			return "gpu-card";
		}
		if (DisplayName.Contains("LG") || DisplayName.Contains("Samsung"))
		{
			return "tv";
		}
		if (DisplayName.Contains("Game"))
		{
			return "controller";
		}

		return "plus-square-fill";
	}
}
