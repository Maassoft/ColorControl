using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU thermal sensors status
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [StructureVersion(2)]
    public struct PrivateThermalSensorsV2 : IInitializable
    {
        internal StructureVersion _Version;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly uint _Mask;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly int[] _Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        private readonly int[] _Temperatures;

        /// <summary>
        ///     Creates a new instance of <see cref="PrivateThermalSensorsV2" />
        /// </summary>
        /// <param name="mask">A 32 bit unsigned integer flag containing sensors that are of interest</param>
        public PrivateThermalSensorsV2(uint mask)
        {
            this = typeof(PrivateThermalSensorsV2).Instantiate<PrivateThermalSensorsV2>();
            _Mask = mask;
        }

        public float[] Temperatures
        {
            get
            {
                return _Temperatures.Select((t) => t / 256.0f).ToArray();
            }
        }
    }
}