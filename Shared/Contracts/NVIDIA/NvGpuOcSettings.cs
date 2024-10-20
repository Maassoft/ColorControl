using ColorControl.Shared.Common;
using Newtonsoft.Json;

namespace ColorControl.Shared.Contracts.NVIDIA
{
    public enum NvGpuOcType
    {
        None = 0,
        Offset = 1,
        Curve = 2,
        BoostLock = 3
    }

    public class NvGpuOcSettings
    {
        public static readonly uint DefaultPowerPCM = 100000;

        public string PCIIdentifier { get; set; }
        public int MemoryOffsetKHz { get; set; }
        [JsonIgnore]
        public int MemoryOffsetMHz
        {
            get => MemoryOffsetKHz / 1000;
            set => MemoryOffsetKHz = value * 1000;
        }
        public NvGpuOcType Type { get; set; }
        public int GraphicsOffsetKHz { get; set; }
        [JsonIgnore]
        public int GraphicsOffsetMHz
        {
            get => GraphicsOffsetKHz / 1000;
            set => GraphicsOffsetKHz = value * 1000;
        }
        public uint MaximumFrequencyKHz { get; set; }
        [JsonIgnore]
        public uint MaximumFrequencyMHz
        {
            get => MaximumFrequencyKHz / 1000;
            set => MaximumFrequencyKHz = value * 1000;
        }
        public uint MaximumVoltageUv { get; set; }
        [JsonIgnore]
        public uint MaximumVoltageMv
        {
            get => MaximumVoltageUv / 1000;
            set => MaximumVoltageUv = value * 1000;
        }
        public bool FrequencyPreferred { get; set; }
        public uint VoltageBoostPercent { get; set; }
        public uint PowerPCM { get; set; } = DefaultPowerPCM;

        public NvGpuOcSettings()
        {
        }

        public NvGpuOcSettings(NvGpuOcSettings settings)
        {
            PCIIdentifier = settings.PCIIdentifier;
            MemoryOffsetKHz = settings.MemoryOffsetKHz;
            Type = settings.Type;
            GraphicsOffsetKHz = settings.GraphicsOffsetKHz;
            MaximumFrequencyKHz = settings.MaximumFrequencyKHz;
            MaximumVoltageUv = settings.MaximumVoltageUv;
            FrequencyPreferred = settings.FrequencyPreferred;
            VoltageBoostPercent = settings.VoltageBoostPercent;
            PowerPCM = settings.PowerPCM;
        }

        public override string ToString()
        {
            var value = "";

            if (Type == NvGpuOcType.BoostLock)
            {
                value += $"GPU: Locked: {(GraphicsOffsetKHz >= 0 ? "+" : "-")}{GraphicsOffsetKHz.ToKiloUnitString()} = {MaximumFrequencyKHz.ToKiloUnitString()} @ {MaximumVoltageUv / 1000}mV";
            }
            else if (Type == NvGpuOcType.Curve)
            {
                value += $"GPU: Curve: {(GraphicsOffsetKHz >= 0 ? "+" : "-")}{GraphicsOffsetKHz.ToKiloUnitString()}";

                if (MaximumVoltageUv > 0)
                {
                    value += $", UV: {MaximumFrequencyKHz.ToKiloUnitString()} @ {MaximumVoltageUv / 1000}mV";
                }
            }
            else if (Type == NvGpuOcType.Offset)
            {
                value += $"GPU: {(GraphicsOffsetKHz >= 0 ? "+" : "-")}{GraphicsOffsetKHz.ToKiloUnitString()}";
            }

            if (MemoryOffsetKHz != 0)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    value += ", ";
                }

                value += $"Memory: {(MemoryOffsetKHz >= 0 ? "+" : "-")}{MemoryOffsetKHz.ToKiloUnitString()}";
            }

            if (PowerPCM != 100000)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    value += ", ";
                }

                value += $"Power: {PowerPCM / 1000}%";
            }

            if (VoltageBoostPercent > 0)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    value += ", ";
                }

                value += $"Voltage: {VoltageBoostPercent}%";
            }

            if (string.IsNullOrEmpty(value))
            {
                value = "None";
            }

            return value;
        }
    }
}
