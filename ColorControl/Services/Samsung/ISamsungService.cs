using ColorControl.Shared.Contracts.Samsung;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColorControl.Services.Samsung
{
	internal interface ISamsungService
	{
		SamsungServiceConfig Config { get; }
		List<SamsungDevice> Devices { get; }
		SamsungDevice SelectedDevice { get; set; }
		string ServiceName { get; }

		Task<bool> ApplyPreset(SamsungPreset preset);
		Task<bool> ApplyPreset(string idOrName);
		SamsungDevice GetPresetDevice(SamsungPreset preset);
		void GlobalSave();
		Task<bool> RefreshDevices(bool connect = true, bool afterStartUp = false);
	}
}