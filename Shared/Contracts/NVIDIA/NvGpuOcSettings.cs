using ColorControl.Shared.Common;

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
        public NvGpuOcType Type { get; set; }
        public int GraphicsOffsetKHz { get; set; }
        public uint MaximumFrequencyKHz { get; set; }
        public uint MaximumVoltageUv { get; set; }
        public bool FrequencyPreferred { get; set; }
        public uint VoltageBoostPercent { get; set; }
        public uint PowerPCM { get; set; } = DefaultPowerPCM;

        public override string ToString()
        {
            var value = "";

            if (Type == NvGpuOcType.BoostLock)
            {
                value += $"GPU: Locked: {(GraphicsOffsetKHz >= 0 ? "+" : "-")}{GraphicsOffsetKHz.ToUnitString()} = {MaximumFrequencyKHz.ToUnitString()} @ {MaximumVoltageUv / 1000}mV";
            }
            else if (Type == NvGpuOcType.Curve)
            {
                value += $"GPU: Curve: {(GraphicsOffsetKHz >= 0 ? "+" : "-")}{GraphicsOffsetKHz.ToUnitString()}";

                if (MaximumVoltageUv > 0)
                {
                    value += $", UV: {MaximumFrequencyKHz.ToUnitString()} @ {MaximumVoltageUv / 1000}mV";
                }
            }
            else if (Type == NvGpuOcType.Offset)
            {
                value += $"GPU: {(GraphicsOffsetKHz >= 0 ? "+" : "-")}{GraphicsOffsetKHz.ToUnitString()}";
            }

            if (MemoryOffsetKHz != 0)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    value += ", ";
                }

                value += $"Memory: {(MemoryOffsetKHz >= 0 ? "+" : "-")}{MemoryOffsetKHz.ToUnitString()}";
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
