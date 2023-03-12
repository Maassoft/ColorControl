using ColorControl.Common;
using ColorControl.Forms;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using static NvAPIWrapper.Native.GPU.Structures.PerformanceStates20ClockEntryV1;
using static NvAPIWrapper.Native.GPU.Structures.PrivateClockBoostLockV2;

namespace ColorControl.Services.NVIDIA
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
        public uint VoltageBoostPercent { get; set; }
        public uint PowerPCM { get; set; } = DefaultPowerPCM;

        public override string ToString()
        {
            var value = "";

            if (Type == NvGpuOcType.BoostLock)
            {
                value += $"GPU: Locked: {(GraphicsOffsetKHz >= 0 ? "+" : "-")}{GraphicsOffsetKHz.ToUnitString()} @ {MaximumVoltageUv}uv";
            }
            else if (Type == NvGpuOcType.Curve)
            {
                value += $"GPU: Curve: {(GraphicsOffsetKHz >= 0 ? "+" : "-")}{GraphicsOffsetKHz.ToUnitString()}";

                if (MaximumVoltageUv > 0)
                {
                    value += $", UV: {MaximumFrequencyKHz.ToUnitString()} @ {MaximumVoltageUv / 1000}mv";
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

    public class NvGpuInfo
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public PhysicalGPU GPU { get; }
        public List<ClockBoostLock> ClockBoostLocks { get; private set; }
        public List<IPerformanceStates20ClockEntry> ClockEntries { get; private set; }
        public ClockDomainInfo StockGraphicsClock { get; private set; }
        public ClockDomainInfo StockMemoryClock { get; private set; }
        public PrivateVFPCurveV3 CurveV3 { get; private set; }
        public PrivateVFPCurveV1 CurveV1 { get; private set; }
        public int? DefaultPowerInMilliWatts { get; private set; }
        public int MinPowerInMilliWatts { get; private set; }
        public int MaxPowerInMilliWatts { get; private set; }
        public int MinCoreStepInMHz { get; private set; } = 15;


        public static readonly PublicClockDomain[] Domains = new[] { PublicClockDomain.Graphics, PublicClockDomain.Memory };

        public static NvGpuInfo GetGpuInfo(string pciId)
        {
            var gpuHandles = GPUApi.EnumPhysicalGPUs();

            var gpus = gpuHandles.Select(h => new PhysicalGPU(h)).ToList();

            //var displays = Display.GetDisplays();
            //var gpus = displays.Select(d => d.DisplayDevice.PhysicalGPU).Distinct().ToList();

            var gpu = gpus.FirstOrDefault(g => g.BusInformation.PCIIdentifiers.ToString() == pciId);

            return GetGpuInfo(gpu);
        }

        public static NvGpuInfo GetGpuInfo(PhysicalGPU gpu)
        {
            var gpuInfo = new NvGpuInfo(gpu);

            return gpuInfo;
        }

        private NvGpuInfo(PhysicalGPU gpu)
        {
            GPU = gpu;

            StockGraphicsClock = GPU.BoostClockFrequencies.GraphicsClock;
            StockMemoryClock = GPU.BoostClockFrequencies.MemoryClock;

            var handle = gpu.Handle;

            //var xx = GPUApi.ClientPowerTopologyGetStatus(handle);

            var perf = GPUApi.GetPerformanceStates20(handle);
            var clock3D = perf.Clocks.FirstOrDefault(c => c.Key == PerformanceStateId.P0_3DPerformance);

            var boostLock = GPUApi.GetClockBoostLock(handle);

            ReadCurve();

            if (NvidiaML.IsAvailable || NvidiaML.Initialize())
            {
                var device = NvidiaML.NvmlDeviceGetHandleByIndex(0);
                DefaultPowerInMilliWatts = NvidiaML.NvmlDeviceGetPowerManagementDefaultLimit(device.Value);

                var powerLimits = GetPowerLimits();

                MinPowerInMilliWatts = (int)((decimal)DefaultPowerInMilliWatts / 100000 * powerLimits.Item1);
                MaxPowerInMilliWatts = (int)((decimal)DefaultPowerInMilliWatts / 100000 * powerLimits.Item2);

            }

            //var bla2 = GPUApi.GetClockBoostRanges(handle);
            //var bla3 = GPUApi.GetClockBoostTableV2(handle);

            //for (var i = 0; i < bla3.GPUDeltas.Count(); i++)
            //{
            //    if (bla3.GPUDeltas[i].ClockType == 0)
            //    {
            //        bla3.GPUDeltas[i].FrequencyDeltaInkHz = 145000;
            //    }
            //    else if (bla3.GPUDeltas[i].ClockType == 1)
            //    {
            //        //bla3.GPUDeltas[i]._FrequencyDeltaInkHz = 50000;
            //    }

            //}

            //GPUApi.SetClockBoostTableV2(handle, bla3);

            //var bla4 = GPUApi.GetAllClockFrequencies(handle);

            ClockBoostLocks = boostLock.ClockBoostLocks.Where(l => l.LockMode == ClockLockMode.Manual).ToList();
            ClockEntries = clock3D.Value.ToList();
        }

        public (uint, uint) GetPowerLimits()
        {
            var power = GPU.PerformanceControl.PowerLimitInformation.FirstOrDefault();

            if (power == null)
            {
                return (20000, 100000);
            }

            return (power.MinimumPowerInPCM, power.MaximumPowerInPCM);
        }

        public uint GetPowerUsageInMilliWatts()
        {
            if (DefaultPowerInMilliWatts.HasValue)
            {
                var powerInPCM = GPU.PowerTopologyInformation.PowerTopologyEntries.FirstOrDefault(e => e.Domain == PowerTopologyDomain.Board);
                var power = (uint)((decimal)powerInPCM.PowerUsageInPCM / 100000 * DefaultPowerInMilliWatts);

                return power;
            }

            return 0;
        }

        private void ReadCurve()
        {
            try
            {
                CurveV3 = GPUApi.GetVFPCurveV3(GPU.Handle);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);

                MinCoreStepInMHz = 12;

                var bla = GPUApi.GetClockBoostMask(GPU.Handle);

                var masks = bla._Masks;
                Logger.Debug("MASKS: " + masks[0]);

                Logger.Debug("MASK: " + bla._Unknown1[0]);

                CurveV1 = GPUApi.GetVFPCurve(GPU.Handle, masks);

                var length = CurveV1.GPUCurveEntries.Length;
                var minFreq = CurveV1.GPUCurveEntries.Min(e => e.FrequencyInkHz);
                var maxFreq = CurveV1.GPUCurveEntries.Max(e => e.FrequencyInkHz);
                var minV = CurveV1.GPUCurveEntries.Min(e => e.VoltageInMicroV);
                var maxV = CurveV1.GPUCurveEntries.Max(e => e.VoltageInMicroV);

                Logger.Debug($"CurveV1: L: {length}, MinF: {minFreq}, MaxF: {maxFreq}, MinV: {minV}, MaxV: {maxV}");
            }
        }

        public bool ApplyOcSettings(NvGpuOcSettings settings)
        {
            var pciId = GPU.BusInformation.PCIIdentifiers.ToString();

            if (settings.PCIIdentifier != pciId)
            {
                Logger.Warn($"Cannot apply OC-settings: PCI-identifier does not match GPU: {settings.PCIIdentifier}");

                return false;
            }

            ResetGpuClocks();

            if (settings.PowerPCM >= 0)
            {
                SetPower(settings.PowerPCM);
            }

            if (settings.MemoryOffsetKHz != 0)
            {
                SetMemoryClockOffsetDelta(settings.MemoryOffsetKHz);
            }

            if (settings.VoltageBoostPercent >= 0)
            {
                SetVoltageBoostPercent(settings.VoltageBoostPercent);
            }

            if (settings.Type == NvGpuOcType.None)
            {
                return true;
            }

            if (settings.Type == NvGpuOcType.BoostLock)
            {
                LockGpuClock(settings.MaximumVoltageUv, settings.GraphicsOffsetKHz);

                return true;
            }

            if (settings.Type == NvGpuOcType.Offset && settings.MaximumVoltageUv == 0)
            {
                SetGraphicsClockOffset(settings.GraphicsOffsetKHz);

                return true;
            }

            SetCurveOffset(settings.GraphicsOffsetKHz, settings.MaximumVoltageUv, settings.MaximumFrequencyKHz);

            return true;
        }

        public void SetPower(uint powerPercent)
        {
            if (powerPercent == 0)
            {
                powerPercent = 100000;
            }

            var entry = new PrivatePowerPoliciesStatusV1.PowerPolicyStatusEntry(powerPercent);
            var status = new PrivatePowerPoliciesStatusV1(new[] { entry });

            GPUApi.ClientPowerPoliciesSetStatus(GPU.Handle, status);
        }

        public override string ToString()
        {
            var gpuThermal = GPU.ThermalInformation.ThermalSensors.FirstOrDefault(s => s.Target == ThermalSettingsTarget.GPU);
            var gpuVoltage = GPUApi.GetCurrentVoltage(GPU.Handle).ValueInMicroVolt;

            try
            {
                var temps = GPUApi.QueryThermalSensors(GPU.Handle, 255u);
                var hotspotTemp = Math.Round(temps.Temperatures[1]);
                var memoryTemp = temps.Temperatures[7];
                var power = GetPowerUsageInMilliWatts();

                return $"GPU: {GPU.CurrentClockFrequencies.GraphicsClock.Frequency.ToUnitString()} @ {gpuVoltage / 1000}mv, {gpuThermal.CurrentTemperature}°/{hotspotTemp}°C, Mem: {GPU.CurrentClockFrequencies.MemoryClock.Frequency.ToUnitString()}, {memoryTemp}°C, Pwr: {power / 1000}W";
            }
            catch (Exception) { }

            return $"GPU: {GPU.CurrentClockFrequencies.GraphicsClock.Frequency.ToUnitString()} @ {gpuVoltage / 1000}mv, {gpuThermal.CurrentTemperature}°C, Mem: {GPU.CurrentClockFrequencies.MemoryClock.Frequency.ToUnitString()}";
        }

        public (int, int) GetDeltaClockMinMax(PublicClockDomain domain)
        {
            var delta = ClockEntries.FirstOrDefault(e => e.DomainId == domain)?.FrequencyDeltaInkHz;

            return (delta?.DeltaRange.Minimum ?? -502000, delta?.DeltaRange.Maximum ?? (domain == PublicClockDomain.Graphics ? 1000000 : 3000000));
        }

        public PerformanceStates20ParameterDelta? GetDeltaClockLimits(PublicClockDomain domain)
        {
            return ClockEntries.FirstOrDefault(e => e.DomainId == domain)?.FrequencyDeltaInkHz;
        }

        public bool HasLock(PublicClockDomain domain)
        {
            return ClockBoostLocks.Any(l => l.ClockDomain == domain && l.LockMode == ClockLockMode.Manual);
        }

        public int GetOffset(PublicClockDomain domain)
        {
            var entry = ClockEntries.FirstOrDefault(e => e.DomainId == domain);

            var value = entry?.FrequencyDeltaInkHz.DeltaValue ?? 0;

            if (value == 0 && domain == PublicClockDomain.Memory && entry != null)
            {
                var maxFreq = entry.SingleFrequency.FrequencyInkHz;
                var defaultFreq = GPU.BoostClockFrequencies.MemoryClock.Frequency;

                value = (int)(maxFreq - defaultFreq);
            }

            return value;
        }

        public (uint, int) GetLockedBoost(PublicClockDomain domain)
        {
            var boostLock = ClockBoostLocks.FirstOrDefault(l => l.ClockDomain == domain && l.LockMode == ClockLockMode.Manual);

            if (boostLock.LockMode != ClockLockMode.Manual)
            {
                return (0, 0);
            }

            var offset = GetOffset(domain);

            return (boostLock.VoltageInMicroV, offset);
        }

        public NvGpuOcSettings GetOverclockSettings()
        {
            var ocSettings = new NvGpuOcSettings();

            var power = GPUApi.ClientPowerPoliciesGetStatus(GPU.Handle);

            ocSettings.PowerPCM = power.PowerPolicyStatusEntries.FirstOrDefault(e => e.PerformanceStateId == PerformanceStateId.P0_3DPerformance).PowerTargetInPCM;

            var voltageBoost = GPUApi.GetCoreVoltageBoostPercent(GPU.Handle);

            ocSettings.VoltageBoostPercent = voltageBoost.Percent;

            foreach (var domain in Domains)
            {
                var locked = GetLockedBoost(domain);

                if (locked.Item1 != 0)
                {
                    if (ocSettings.Type == NvGpuOcType.None)
                    {
                        ocSettings.Type = NvGpuOcType.BoostLock;
                    }

                    ocSettings.MaximumVoltageUv = locked.Item1;

                    if (domain == PublicClockDomain.Graphics)
                    {
                        ocSettings.GraphicsOffsetKHz = locked.Item2;
                    }
                    else
                    {
                        ocSettings.MemoryOffsetKHz = locked.Item2;
                    }
                }

                if (ocSettings.Type == NvGpuOcType.None && domain == PublicClockDomain.Graphics && CurveV3.GPUCurveEntries != null)
                {
                    var entry = CurveV3.GPUCurveEntries.MaxBy(e => (int)e.FrequencyInkHz - (int)e.DefaultFrequencyInkHz);
                    var offsetCurve = (int)entry.FrequencyInkHz - (int)entry.DefaultFrequencyInkHz;
                    if (offsetCurve != 0)
                    {
                        var isChanged = CurveV3.GPUCurveEntries.Any(e => e.ClockType == 0 && e.DefaultFrequencyInkHz != 0 && ((int)e.FrequencyInkHz - (int)e.DefaultFrequencyInkHz) != offsetCurve);

                        ocSettings.Type = isChanged ? NvGpuOcType.Curve : NvGpuOcType.Offset;
                        ocSettings.GraphicsOffsetKHz = offsetCurve;

                        if (offsetCurve > 0 && ocSettings.Type == NvGpuOcType.Curve)
                        {
                            var maxOcEntry = CurveV3.GPUCurveEntries.MaxBy(e => e.FrequencyInkHz);
                            var maxDefEntry = CurveV3.GPUCurveEntries.MaxBy(e => e.DefaultFrequencyInkHz);

                            if (maxOcEntry.FrequencyInkHz < maxDefEntry.DefaultFrequencyInkHz)
                            {
                                ocSettings.MaximumVoltageUv = maxOcEntry.VoltageInMicroV;
                                ocSettings.MaximumFrequencyKHz = maxDefEntry.FrequencyInkHz;
                            }
                        }
                    }
                }

                if (ocSettings.Type == NvGpuOcType.None || domain == PublicClockDomain.Memory)
                {
                    var offset = GetOffset(domain);

                    if (offset != 0)
                    {
                        if (ocSettings.Type == NvGpuOcType.None)
                        {
                            ocSettings.Type = NvGpuOcType.Offset;
                        }

                        if (domain == PublicClockDomain.Graphics)
                        {
                            ocSettings.GraphicsOffsetKHz = offset;
                        }
                        else
                        {
                            ocSettings.MemoryOffsetKHz = offset;
                        }
                    }
                }
            }

            return ocSettings;
        }

        public void SetGpuClocks(uint lockedVoltageMicroV = 1000000, int coreOffsetKHz = 0, int memOffsetKHz = 0)
        {
            var handle = GPU.Handle;

            var perf = GPUApi.GetPerformanceStates20(handle);

            var lockv2 = new PrivateClockBoostLockV2(new[] { new ClockBoostLock(PublicClockDomain.Graphics, ClockLockMode.Manual, lockedVoltageMicroV) });

            var bla = GPUApi.GetClockBoostLock(handle);

            GPUApi.SetClockBoostLock(handle, lockv2);

            //var curve = GPUApi.GetVFPCurve(handle);

            var currentFreq = perf.Clocks.First().Value.First(v => v.DomainId == PublicClockDomain.Graphics).SingleFrequency;

            var delta = new PerformanceStates20ParameterDelta(coreOffsetKHz);
            //var delta2 = new PerformanceStates20ParameterDelta(0);
            //var delta3 = new PerformanceStates20ParameterDelta(0);
            //var single1 = new PerformanceStates20ClockDependentSingleFrequency(2670000);
            //var single2 = new PerformanceStates20ClockDependentSingleFrequency(2760000);
            //var single3 = new PerformanceStates20ClockDependentSingleFrequency(2775000);

            //var clockEntry1 = new PerformanceStates20ClockEntryV1(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics, delta, single1);
            //var clockEntry2 = new PerformanceStates20ClockEntryV1(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics, delta2, single2);
            //var clockEntry3 = new PerformanceStates20ClockEntryV1(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics, delta3, single3);

            var range = new PerformanceStates20ClockDependentFrequencyRange(2400000, 3000000, PerformanceVoltageDomain.Core, 950000, 1050000);
            var clockEntry = new PerformanceStates20ClockEntryV1(PublicClockDomain.Graphics, delta, range);

            var memDelta = new PerformanceStates20ParameterDelta(memOffsetKHz);
            var memClockEntry = new PerformanceStates20ClockEntryV1(PublicClockDomain.Memory, memDelta);

            //var range2 = new PerformanceStates20ClockDependentFrequencyRange(2800001, 3000000, NvAPIWrapper.Native.GPU.PerformanceVoltageDomain.Core, 1000000, 1000000);
            //var clockEntry2 = new PerformanceStates20ClockEntryV1(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics, delta2, range2);

            var deltaVoltage = new PerformanceStates20ParameterDelta(0);
            var voltage = new PerformanceStates20BaseVoltageEntryV1(PerformanceVoltageDomain.Core, deltaVoltage);

            var state = new PerformanceStates20InfoV1.PerformanceState20(PerformanceStateId.P0_3DPerformance, new[] { clockEntry, memClockEntry }, new[] { voltage });

            var newPerf = new PerformanceStates20InfoV3(new[] { state }, 2, 1);

            GPUApi.SetPerformanceStates20(handle, newPerf);
        }

        public void LockGpuClock(uint lockedVoltageMicroV = 1000000, int coreOffsetKHz = 0)
        {
            var handle = GPU.Handle;

            var lockv2 = new PrivateClockBoostLockV2(new[] { new ClockBoostLock(PublicClockDomain.Graphics, ClockLockMode.Manual, lockedVoltageMicroV) });

            GPUApi.SetClockBoostLock(handle, lockv2);

            var delta = new PerformanceStates20ParameterDelta(coreOffsetKHz);

            var range = new PerformanceStates20ClockDependentFrequencyRange(1200000, 3000000, PerformanceVoltageDomain.Core, 0, 0);
            var clockEntry = new PerformanceStates20ClockEntryV1(PublicClockDomain.Graphics, delta, range);

            var state = new PerformanceStates20InfoV1.PerformanceState20(PerformanceStateId.P0_3DPerformance, new[] { clockEntry }, Array.Empty<PerformanceStates20BaseVoltageEntryV1>());

            var newPerf = new PerformanceStates20InfoV3(new[] { state }, 1, 1);

            GPUApi.SetPerformanceStates20(handle, newPerf);
        }

        public void SetGraphicsClockOffset(int coreOffsetKHz = 0)
        {
            var delta = new PerformanceStates20ParameterDelta(coreOffsetKHz);

            var range = new PerformanceStates20ClockDependentFrequencyRange(1200000, 3000000, PerformanceVoltageDomain.Core, 0, 0);
            var clockEntry = new PerformanceStates20ClockEntryV1(PublicClockDomain.Graphics, delta, range);

            var state = new PerformanceStates20InfoV1.PerformanceState20(PerformanceStateId.P0_3DPerformance, new[] { clockEntry }, Array.Empty<PerformanceStates20BaseVoltageEntryV1>());

            var newPerf = new PerformanceStates20InfoV3(new[] { state }, 1, 0);

            GPUApi.SetPerformanceStates20(GPU.Handle, newPerf);
        }

        public void SetMemoryClockOffsetDelta(int memOffsetKHz = 0)
        {
            var handle = GPU.Handle;

            var memDelta = new PerformanceStates20ParameterDelta(memOffsetKHz);
            var memClockEntry = new PerformanceStates20ClockEntryV1(PublicClockDomain.Memory, memDelta);

            var state = new PerformanceStates20InfoV1.PerformanceState20(PerformanceStateId.P0_3DPerformance, new[] { memClockEntry }, Array.Empty<PerformanceStates20BaseVoltageEntryV1>());

            var newPerf = new PerformanceStates20InfoV3(new[] { state }, 1, 0);

            GPUApi.SetPerformanceStates20(handle, newPerf);
        }

        public void SetMemoryClockOffsetRange(int memOffsetKHz = 0)
        {
            var handle = GPU.Handle;

            var memDelta = new PerformanceStates20ParameterDelta(0);

            var defaultFreq = GPU.BoostClockFrequencies.MemoryClock.Frequency;
            var newFreq = (uint)(defaultFreq + memOffsetKHz);

            var memFreq = new PerformanceStates20ClockDependentFrequencyRange(newFreq, newFreq, PerformanceVoltageDomain.Undefined, 0, 0);
            var memFreq2 = new PerformanceStates20ClockDependentSingleFrequency(newFreq);
            var memClockEntry = new PerformanceStates20ClockEntryV1(PublicClockDomain.Memory, memDelta, memFreq);
            var memClockEntry2 = new PerformanceStates20ClockEntryV1(PublicClockDomain.Memory, memDelta, memFreq2);

            var state = new PerformanceStates20InfoV1.PerformanceState20(PerformanceStateId.P0_3DPerformance, new[] { memClockEntry, memClockEntry2 }, Array.Empty<PerformanceStates20BaseVoltageEntryV1>());

            var newPerf = new PerformanceStates20InfoV3(new[] { state }, 1, 0);

            GPUApi.SetPerformanceStates20(handle, newPerf);
        }

        public void SetCurveOffset(int offsetKHz = 0, uint maxVoltageUv = 0, uint maxFreqKHz = 0)
        {
            var normalizedOffset = (offsetKHz / 15000) * 15000;

            var handle = GPU.Handle;

            var boostTable = GPUApi.GetClockBoostTableV2(handle);

            var curveEntries = CurveV3.GPUCurveEntries;

            if (maxVoltageUv > 0)
            {
                var newCurveEntryVolt = curveEntries.FirstOrDefault(e => e.DefaultVoltageInMicroV == maxVoltageUv);
                var newFreq = (uint)(newCurveEntryVolt.DefaultFrequencyInkHz + offsetKHz);
                var newCurveEntryFreq = curveEntries.FirstOrDefault(e => e.DefaultFrequencyInkHz == newFreq);

                maxFreqKHz = newFreq;
            }

            for (var i = 0; i < boostTable.GPUDeltas.Count(); i++)
            {
                if (boostTable.GPUDeltas[i].ClockType != 0)
                {
                    break;
                }

                var curOffset = offsetKHz;

                if (maxFreqKHz > 0 && CurveV3.GPUCurveEntries?.Length > i)
                {
                    var curveEntry = CurveV3.GPUCurveEntries[i];

                    if (curveEntry.DefaultFrequencyInkHz + offsetKHz > maxFreqKHz)
                    {
                        curOffset += (int)(maxFreqKHz - (curveEntry.DefaultFrequencyInkHz + offsetKHz));
                    }
                }

                boostTable.GPUDeltas[i].FrequencyDeltaInkHz = curOffset;

            }
            GPUApi.SetClockBoostTableV2(handle, boostTable);

            ReadCurve();
        }

        public void ResetGpuClocks()
        {
            var handle = GPU.Handle;

            var lockv2 = new PrivateClockBoostLockV2(new[] { new ClockBoostLock(PublicClockDomain.Graphics, ClockLockMode.None, 0) });

            GPUApi.SetClockBoostLock(handle, lockv2);

            SetMemoryClockOffsetDelta(0);
            SetGraphicsClockOffset(0);
            SetCurveOffset(0);
            SetVoltageBoostPercent(0);
        }

        public uint GetGpuVoltage()
        {
            var voltage = GPUApi.GetCurrentVoltage(GPU.Handle);

            return voltage.ValueInMicroVolt;
        }

        public void SetVoltageBoostPercent(uint percent)
        {
            var voltBoost = new PrivateVoltageBoostPercentV1(percent);

            GPUApi.SetCoreVoltageBoostPercent(GPU.Handle, voltBoost);
        }

        public string GetGpuLimits()
        {
            var handle = GPU.Handle;

            var perf = GPUApi.GetPerformanceStates20(handle);
            var clock3D = perf.Clocks.FirstOrDefault(c => c.Key == NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance);

            var boostLock = GPUApi.GetClockBoostLock(handle);

            var clockFreqs = GPUApi.GetAllClockFrequencies(handle);

            //GPUApi.ClientPowerPoliciesGetStatus(handle);

            //var mask = GPUApi.GetClockBoostMask(handle);

            //var ranges = GPUApi.GetClockBoostRanges(handle);

            //var table = GPUApi.GetClockBoostTable(handle);

            var voltageBoost = GPUApi.GetCoreVoltageBoostPercent(handle);

            //var voltage = GPUApi.GetCurrentVoltage(handle);

            //var dynPerf = GPUApi.GetDynamicPerformanceStatesInfoEx(handle);

            //var pols = GPUApi.PerformancePoliciesGetInfo(handle);

            return string.Empty;
        }

        internal MessageForms.FieldDefinition GetCoreOffsetField(NvGpuOcSettings settings)
        {
            var graphicsDelta = GetDeltaClockMinMax(PublicClockDomain.Graphics);

            //var field = new MessageForms.FieldDefinition
            //{
            //    Label = $"Core offset in MHz ({graphicsDelta.Item1.ToUnitString()} to {graphicsDelta.Item2.ToUnitString()})",
            //    FieldType = MessageForms.FieldType.Numeric,
            //    MinValue = graphicsDelta.Item1 / 1000,
            //    MaxValue = graphicsDelta.Item2 / 1000,
            //    Value = settings.GraphicsOffsetKHz / 1000
            //};

            var field = new MessageForms.FieldDefinition
            {
                Label = $"Core offset in MHz ({graphicsDelta.Item1.ToUnitString()} to {graphicsDelta.Item2.ToUnitString()})",
                FieldType = MessageForms.FieldType.TrackBar,
                MinValue = graphicsDelta.Item1 / 1000,
                MaxValue = graphicsDelta.Item2 / 1000,
                Value = settings.GraphicsOffsetKHz / 1000,
                StepSize = MinCoreStepInMHz
            };

            return field;
        }

        internal MessageForms.FieldDefinition GetMemoryOffsetField(NvGpuOcSettings settings)
        {
            var memoryDelta = GetDeltaClockMinMax(PublicClockDomain.Memory);

            //var field = new MessageForms.FieldDefinition
            //{
            //    Label = $"Memory offset in MHz ({memoryDelta.Item1.ToUnitString()} to {memoryDelta.Item2.ToUnitString()})",
            //    FieldType = MessageForms.FieldType.Numeric,
            //    MinValue = memoryDelta.Item1 / 1000,
            //    MaxValue = memoryDelta.Item2 / 1000,
            //    Value = settings.MemoryOffsetKHz / 1000
            //};

            var field = new MessageForms.FieldDefinition
            {
                Label = $"Memory offset in MHz ({memoryDelta.Item1.ToUnitString()} to {memoryDelta.Item2.ToUnitString()})",
                FieldType = MessageForms.FieldType.TrackBar,
                MinValue = memoryDelta.Item1 / 1000,
                MaxValue = memoryDelta.Item2 / 1000,
                Value = settings.MemoryOffsetKHz / 1000,
                StepSize = 25
            };

            return field;
        }

        internal MessageForms.FieldDefinition GetPowerField(NvGpuOcSettings settings)
        {
            var powerLimits = GetPowerLimits();

            var defaultPowerLimitMw = DefaultPowerInMilliWatts;

            if (settings.PowerPCM == 0)
            {
                settings.PowerPCM = NvGpuOcSettings.DefaultPowerPCM;
            }
            else if (settings.PowerPCM < powerLimits.Item1)
            {
                settings.PowerPCM = powerLimits.Item1;
            }

            var powerField = defaultPowerLimitMw > 0 ?
                new MessageForms.FieldDefinition
                {
                    Label = $"Power limit in milli watts. Min: {MinPowerInMilliWatts}, max: {MaxPowerInMilliWatts}",
                    FieldType = MessageForms.FieldType.TrackBar,
                    Value = (int)((decimal)defaultPowerLimitMw / 100000 * settings.PowerPCM),
                    MinValue = MinPowerInMilliWatts,
                    MaxValue = MaxPowerInMilliWatts
                }
                : new MessageForms.FieldDefinition
                {
                    Label = $"Power limit in PCM (per cent mille). Min: {powerLimits.Item1}, max: {powerLimits.Item2}",
                    FieldType = MessageForms.FieldType.TrackBar,
                    Value = settings.PowerPCM,
                    MinValue = powerLimits.Item1,
                    MaxValue = powerLimits.Item2
                };

            return powerField;
        }

        internal MessageForms.FieldDefinition GetVoltageBoostField(NvGpuOcSettings settings)
        {
            var field = new MessageForms.FieldDefinition
            {
                Label = "Voltage boost in %",
                FieldType = MessageForms.FieldType.TrackBar,
                Value = settings.VoltageBoostPercent,
                MinValue = 0,
                MaxValue = 100
            };

            return field;
        }
    }
}
