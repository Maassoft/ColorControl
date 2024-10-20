using Newtonsoft.Json;
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
    public uint MinimumPowerInPCM { get; set; }
    public uint MaximumPowerInPCM { get; set; }
    public int MinPowerInMilliWatts { get; set; }
    public int MaxPowerInMilliWatts { get; set; }
    public int MinCoreStepInMHz { get; set; } = 15;
    public int MinCoreDeltaInMHz { get; set; }
    [JsonIgnore]
    public int CorrectedMinCoreDeltaInMHz
    {
        get => (MinCoreDeltaInMHz / MinCoreStepInMHz) * MinCoreStepInMHz;
    }
    public int MaxCoreDeltaInMHz { get; set; }
    public int MinMemoryDeltaInMHz { get; set; }
    public int MaxMemoryDeltaInMHz { get; set; }
    public List<uint> CurveFrequenciesInMHz { get; set; } = new();
    public List<uint> CurveVoltagesInMv { get; set; } = new();
    [JsonIgnore]
    public uint MinCurveFrequencyInMHz => CurveFrequenciesInMHz.Any() ? CurveFrequenciesInMHz.Min() : 0;
    [JsonIgnore]
    public uint MaxCurveFrequencyInMHz => CurveFrequenciesInMHz.Any() ? CurveFrequenciesInMHz.Max() : 0;
    [JsonIgnore]
    public uint MinCurveVoltageInMv => CurveVoltagesInMv.Any() ? CurveVoltagesInMv.Min() : 0;
    [JsonIgnore]
    public uint MaxCurveVoltageInMv => CurveVoltagesInMv.Any() ? CurveVoltagesInMv.Max() : 0;
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

    public NvGpuOcSettings OverclockSettings { get; set; }
}
