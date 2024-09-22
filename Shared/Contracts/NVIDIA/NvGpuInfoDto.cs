using NvAPIWrapper.Native.GPU.Structures;
using static NvAPIWrapper.Native.GPU.Structures.PrivateClockBoostLockV2;

namespace ColorControl.Shared.Contracts.NVIDIA;

public class NvGpuInfoDto
{
	public List<ClockBoostLock> ClockBoostLocks { get; set; } = new();
	public ClockDomainInfo StockGraphicsClock { get; set; }
	public ClockDomainInfo StockMemoryClock { get; set; }
	public PrivateVFPCurveV3 CurveV3 { get; set; }
	public PrivateVFPCurveV1 CurveV1 { get; set; }
	public int? DefaultPowerInMilliWatts { get; set; }
	public int MinPowerInMilliWatts { get; set; }
	public int MaxPowerInMilliWatts { get; set; }
	public int MinCoreStepInMHz { get; set; } = 15;
	public List<NvGpuOcType> SupportedOcTypes { get; set; } = new();

	public string PCIIdentifier { get; set; }
	public string Name { get; set; }
	public uint GpuFrequencyInKhz { get; set; }
	public uint MemoryFrequencyInKhz { get; set; }
	public uint VoltageInMv { get; set; }
	public double PowerInWatts { get; set; }
	public double? GpuTemperature { get; set; }
	public double? HotSpotTemperature { get; set; }
	public double? MemoryTemperature { get; set; }
}
