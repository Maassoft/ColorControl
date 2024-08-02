namespace ColorControl.Shared.Common;

public class InfoDto
{
	public string DataPath { get; set; }
	public DateTime StartTime { get; set; }
	public string ApplicationTitle { get; set; }
	public string ApplicationTitleAdmin { get; set; }
	public string LegalCopyright { get; set; }
	public bool UpdateAvailable { get; set; }
}