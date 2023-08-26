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
        void InstallEventHandlers();
        Task RefreshDevices(bool connect = true, bool afterStartUp = false);
    }
}