using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;
using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU boost frequency curve
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(3)]
    public struct PrivateVFPCurveV3 : IInitializable
    {
        internal const int MaxNumberOfMasks = 4;
        internal const int MaxNumberOfUnknown1 = 21;
        internal const int MaxNumberOfGPUCurveEntries = 255;

        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfMasks)]
        public uint[] _Masks;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown1)]
        internal readonly uint[] _Unknown1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfGPUCurveEntries)]
        internal readonly VFPCurveEntry[] _GPUCurveEntries;

        /// <summary>
        ///     Gets the list of GPU curve entries
        /// </summary>
        public VFPCurveEntry[] GPUCurveEntries
        {
            get => _GPUCurveEntries;
        }

        /// <summary>
        ///     Contains information regarding a boost frequency curve entry
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct VFPoint
        {
            internal uint _FrequencyInkHz;
            internal uint _VoltageInUv;
        }

        /// <summary>
        ///     Contains information regarding a boost frequency curve entry
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct VFPCurveEntry
        {
            internal const int MaxNumberOfUnknown1 = 8;
            internal const int MaxNumberOfUnknown2 = 72;

            internal uint _ClockType;
            internal VFPoint _Point;
            internal VFPoint _PointDefault;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown1)]
            internal readonly uint[] _Unknown1;

            internal VFPoint _PointOverclocked;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown2)]
            internal readonly uint[] _Unknown2;

            /// <summary>
            ///     Gets the frequency in kHz
            /// </summary>
            public uint FrequencyInkHz
            {
                get => _Point._FrequencyInkHz;
            }

            /// <summary>
            ///     Gets the voltage in uV
            /// </summary>
            public uint VoltageInMicroV
            {
                get => _Point._VoltageInUv;
            }

            public uint DefaultFrequencyInkHz
            {
                get => _PointDefault._FrequencyInkHz;
            }

            /// <summary>
            ///     Gets the voltage in uV
            /// </summary>
            public uint DefaultVoltageInMicroV
            {
                get => _PointDefault._VoltageInUv;
            }

            public uint ClockType
            {
                get => _ClockType;
            }

        }
    }
}