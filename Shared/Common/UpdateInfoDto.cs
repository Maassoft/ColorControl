namespace ColorControl.Shared.Common;

public class UpdateInfoDto
{
	public bool UpdateAvailable { get; set; }
	public string NewVersionNumber { get; set; }
	public string HtmlUrl { get; set; }
	public string DownloadUrl { get; set; }
}