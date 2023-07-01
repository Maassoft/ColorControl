using NWin32;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ColorControl.Shared.Native
{
    // This class takes care of wrapping "Connecting and Configuring Displays(CCD) Win32 API"
    public static class CCD
    {
        private static bool? IsHDRActive = null;

        public enum DisplayTopology
        {
            Internal,
            External,
            Extend,
            Clone
        }

        public static DisplayTopology GetDisplayTopology()
        {
            uint numPathArrayElements;
            uint numModeInfoArrayElements;

            NativeMethods.GetDisplayConfigBufferSizes(QueryDisplayFlags.DatabaseCurrent, out numPathArrayElements, out numModeInfoArrayElements);

            var pathArray = new DisplayConfigPathInfo[numPathArrayElements];
            var modeArray = new DisplayConfigModeInfo[numModeInfoArrayElements];

            DisplayConfigTopologyId displayTopology;

            NativeMethods.QueryDisplayConfig(QueryDisplayFlags.DatabaseCurrent, ref numPathArrayElements, pathArray, ref numModeInfoArrayElements, modeArray, out displayTopology);

            switch (displayTopology)
            {
                case DisplayConfigTopologyId.External: return DisplayTopology.External;
                case DisplayConfigTopologyId.Internal: return DisplayTopology.Internal;
                case DisplayConfigTopologyId.Extend: return DisplayTopology.Extend;
            }

            return DisplayTopology.Clone;
        }

        public static void SetDisplayTopology(DisplayTopology displayTopology)
        {
            switch (displayTopology)
            {
                case DisplayTopology.External:
                    NativeMethods.SetDisplayConfig(0, null, 0, null, (SdcFlags.Apply | SdcFlags.TopologyExternal));
                    break;
                case DisplayTopology.Internal:
                    NativeMethods.SetDisplayConfig(0, null, 0, null, (SdcFlags.Apply | SdcFlags.TopologyInternal));
                    break;
                case DisplayTopology.Extend:
                    NativeMethods.SetDisplayConfig(0, null, 0, null, (SdcFlags.Apply | SdcFlags.TopologyExtend));
                    break;
                case DisplayTopology.Clone:
                    NativeMethods.SetDisplayConfig(0, null, 0, null, (SdcFlags.Apply | SdcFlags.TopologyClone));
                    break;
            }
        }

        public static void SetHDRState(bool enabled, string displayName = null, float? SDRWhiteLevelInNits = null)
        {
            uint pathCount, modeCount;

            var err = NativeMethods.GetDisplayConfigBufferSizes(QueryDisplayFlags.OnlyActivePaths, out pathCount, out modeCount);
            if (err == NativeConstants.ERROR_SUCCESS)
            {
                var pathsArray = new DisplayConfigPathInfo[pathCount];
                var modesArray = new DisplayConfigModeInfo[modeCount];

                DisplayConfigTopologyId displayTopology;

                err = NativeMethods.QueryDisplayConfig(QueryDisplayFlags.DatabaseCurrent, ref pathCount, pathsArray, ref modeCount, modesArray, out displayTopology);
                if (err == NativeConstants.ERROR_SUCCESS)
                {
                    foreach (var path in pathsArray)
                    {
                        if (!MatchDisplayNames(path, displayName))
                        {
                            continue;
                        }

                        var setpacket = new DISPLAYCONFIG_SET_ADVANCED_COLOR_INFO();
                        setpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                        setpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE;
                        setpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_INFO>();

                        var requestpacket = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                        requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                        requestpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                        requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();

                        for (int i = 0; i < modeCount; i++)
                        {
                            if (modesArray[i].infoType == DisplayConfigModeInfoType.Target)
                            {
                                setpacket.header.adapterId = modesArray[i].adapterId;
                                setpacket.header.id = modesArray[i].id;
                                requestpacket.header.adapterId = modesArray[i].adapterId;
                                requestpacket.header.id = modesArray[i].id;

                                if (NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket) == NativeConstants.ERROR_SUCCESS && requestpacket.advancedColorSupported)
                                {
                                    setpacket.enableAdvancedColor = enabled ? 1U : 0;
                                    NativeMethods.DisplayConfigSetDeviceInfo(ref setpacket);

                                    IsHDRActive = enabled;
                                }
                            }
                        }
                    }
                }
            }

            if (err != NativeConstants.ERROR_SUCCESS)
            {
                throw new Win32Exception(err);
            }
        }

        public static uint GetSDRWhiteLevel(string displayName = null)
        {
            uint pathCount, modeCount;

            var err = NativeMethods.GetDisplayConfigBufferSizes(QueryDisplayFlags.OnlyActivePaths, out pathCount, out modeCount);
            if (err == NativeConstants.ERROR_SUCCESS)
            {
                var pathsArray = new DisplayConfigPathInfo[pathCount];
                var modesArray = new DisplayConfigModeInfo[modeCount];

                DisplayConfigTopologyId displayTopology;

                err = NativeMethods.QueryDisplayConfig(QueryDisplayFlags.DatabaseCurrent, ref pathCount, pathsArray, ref modeCount, modesArray, out displayTopology);
                if (err == NativeConstants.ERROR_SUCCESS)
                {
                    foreach (var path in pathsArray)
                    {
                        if (!MatchDisplayNames(path, displayName))
                        {
                            continue;
                        }

                        var requestpacket = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                        requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                        requestpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                        requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();

                        for (int i = 0; i < modeCount; i++)
                        {
                            if (modesArray[i].infoType == DisplayConfigModeInfoType.Target)
                            {
                                requestpacket.header.adapterId = modesArray[i].adapterId;
                                requestpacket.header.id = modesArray[i].id;

                                err = NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket);

                                if (err == NativeConstants.ERROR_SUCCESS && requestpacket.advancedColorSupported)
                                {
                                    return GetSDRWhiteLevelForDisplayConfig(modesArray[i]);
                                }
                            }
                        }
                    }
                }
            }

            if (err != NativeConstants.ERROR_SUCCESS)
            {
                throw new Win32Exception(err);
            }

            return 0;
        }

        public static bool IsHDREnabled(string displayName = null)
        {
            if (IsHDRActive.HasValue)
            {
                return IsHDRActive.Value;
            }

            uint pathCount, modeCount;

            var err = NativeMethods.GetDisplayConfigBufferSizes(QueryDisplayFlags.OnlyActivePaths, out pathCount, out modeCount);
            if (err == NativeConstants.ERROR_SUCCESS)
            {
                var pathsArray = new DisplayConfigPathInfo[pathCount];
                var modesArray = new DisplayConfigModeInfo[modeCount];

                err = NativeMethods.QueryDisplayConfig(QueryDisplayFlags.DatabaseCurrent, ref pathCount, pathsArray, ref modeCount, modesArray, out _);
                if (err == NativeConstants.ERROR_SUCCESS)
                {
                    foreach (var path in pathsArray)
                    {
                        if (!MatchDisplayNames(path, displayName))
                        {
                            continue;
                        }

                        var requestpacket = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                        requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                        requestpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                        requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();

                        for (int i = 0; i < modeCount; i++)
                        {
                            if (modesArray[i].infoType == DisplayConfigModeInfoType.Target)
                            {
                                requestpacket.header.adapterId = modesArray[i].adapterId;
                                requestpacket.header.id = modesArray[i].id;

                                err = NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket);

                                IsHDRActive = err == NativeConstants.ERROR_SUCCESS && requestpacket.advancedColorEnabled;

                                return IsHDRActive.Value;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool MatchDisplayNames(DisplayConfigPathInfo path, string displayName)
        {
            if (displayName == null)
            {
                return true;
            }

            // get display name
            var info = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
            info.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
            info.header.size = Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
            info.header.adapterId = path.sourceInfo.adapterId;
            info.header.id = path.sourceInfo.id;

            var err = NativeMethods.DisplayConfigGetDeviceInfo(ref info);
            if (err != NativeConstants.ERROR_SUCCESS)
            {
                return false;
            }

            var deviceName = info.viewGdiDeviceName;
            if (!EqualDisplayNames(deviceName, displayName))
            {
                return false;
            }

            return true;
        }

        private static uint GetSDRWhiteLevelForDisplayConfig(DisplayConfigModeInfo displayConfigModeInfo)
        {
            var requestpacket = new DISPLAYCONFIG_SDR_WHITE_LEVEL();
            requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
            requestpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL;
            requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_SDR_WHITE_LEVEL>();
            requestpacket.header.adapterId = displayConfigModeInfo.adapterId;
            requestpacket.header.id = displayConfigModeInfo.id;

            var error = NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket);

            if (error == NativeConstants.ERROR_SUCCESS)
            {
                return requestpacket.SDRWhiteLevel;
            }

            return 0;
        }

        private static bool EqualDisplayNames(string displayName1, string displayName2)
        {
            displayName1 = displayName1.Replace("\\", string.Empty);
            displayName2 = displayName2.Replace("\\", string.Empty);

            return displayName1.Equals(displayName2, StringComparison.OrdinalIgnoreCase);
        }

        private enum DISPLAYCONFIG_MODE_INFO_TYPE
        {
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
            DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3,
        }

        private enum DISPLAYCONFIG_DEVICE_INFO_TYPE
        {
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
            DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION = 7,
            DISPLAYCONFIG_DEVICE_INFO_SET_SUPPORT_VIRTUAL_RESOLUTION = 8,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9,
            DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10,
            DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11,
            DISPLAYCONFIG_DEVICE_INFO_SET_SDR_WHITE_LEVEL = 12,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
            public int size;
            public LUID adapterId;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct LUID
        {
            uint LowPart;
            uint HighPart;
        }

        [Flags]
        enum DisplayConfigVideoOutputTechnology : uint
        {
            Other = 4294967295, // -1
            Hd15 = 0,
            Svideo = 1,
            CompositeVideo = 2,
            ComponentVideo = 3,
            Dvi = 4,
            Hdmi = 5,
            Lvds = 6,
            DJpn = 8,
            Sdi = 9,
            DisplayportExternal = 10,
            DisplayportEmbedded = 11,
            UdiExternal = 12,
            UdiEmbedded = 13,
            Sdtvdongle = 14,
            Internal = 0x80000000,
            ForceUint32 = 0xFFFFFFFF
        }

        #region SdcFlags enum

        [Flags]
        enum SdcFlags : uint
        {
            Zero = 0,

            TopologyInternal = 0x00000001,
            TopologyClone = 0x00000002,
            TopologyExtend = 0x00000004,
            TopologyExternal = 0x00000008,
            TopologySupplied = 0x00000010,

            UseSuppliedDisplayConfig = 0x00000020,
            Validate = 0x00000040,
            Apply = 0x00000080,
            NoOptimization = 0x00000100,
            SaveToDatabase = 0x00000200,
            AllowChanges = 0x00000400,
            PathPersistIfRequired = 0x00000800,
            ForceModeEnumeration = 0x00001000,
            AllowPathOrderChanges = 0x00002000,

            UseDatabaseCurrent = TopologyInternal | TopologyClone | TopologyExtend | TopologyExternal
        }

        [Flags]
        enum DisplayConfigFlags : uint
        {
            Zero = 0x0,
            PathActive = 0x00000001
        }

        [Flags]
        enum DisplayConfigSourceStatus
        {
            Zero = 0x0,
            InUse = 0x00000001
        }

        [Flags]
        enum DisplayConfigTargetStatus : uint
        {
            Zero = 0x0,

            InUse = 0x00000001,
            FORCIBLE = 0x00000002,
            ForcedAvailabilityBoot = 0x00000004,
            ForcedAvailabilityPath = 0x00000008,
            ForcedAvailabilitySystem = 0x00000010,
        }

        [Flags]
        enum DisplayConfigRotation : uint
        {
            Zero = 0x0,

            Identity = 1,
            Rotate90 = 2,
            Rotate180 = 3,
            Rotate270 = 4,
            ForceUint32 = 0xFFFFFFFF
        }

        [Flags]
        enum DisplayConfigPixelFormat : uint
        {
            Zero = 0x0,

            Pixelformat8Bpp = 1,
            Pixelformat16Bpp = 2,
            Pixelformat24Bpp = 3,
            Pixelformat32Bpp = 4,
            PixelformatNongdi = 5,
            PixelformatForceUint32 = 0xffffffff
        }

        [Flags]
        enum DisplayConfigScaling : uint
        {
            Zero = 0x0,

            Identity = 1,
            Centered = 2,
            Stretched = 3,
            Aspectratiocenteredmax = 4,
            Custom = 5,
            Preferred = 128,
            ForceUint32 = 0xFFFFFFFF
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigRational
        {
            uint numerator;
            uint denominator;
        }

        [Flags]
        enum DisplayConfigScanLineOrdering : uint
        {
            Unspecified = 0,
            Progressive = 1,
            Interlaced = 2,
            InterlacedUpperfieldfirst = Interlaced,
            InterlacedLowerfieldfirst = 3,
            ForceUint32 = 0xFFFFFFFF
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigPathInfo
        {
            public DisplayConfigPathSourceInfo sourceInfo;
            public DisplayConfigPathTargetInfo targetInfo;
            public uint flags;
        }

        [Flags]
        enum DisplayConfigModeInfoType : uint
        {
            Zero = 0,

            Source = 1,
            Target = 2,
            ForceUint32 = 0xFFFFFFFF
        }

        [StructLayout(LayoutKind.Explicit)]
        struct DisplayConfigModeInfo
        {
            [FieldOffset(0)]
            public DisplayConfigModeInfoType infoType;

            [FieldOffset(4)]
            public uint id;

            [FieldOffset(8)]
            public LUID adapterId;

            [FieldOffset(16)]
            public DisplayConfigTargetMode targetMode;

            [FieldOffset(16)]
            public DisplayConfigSourceMode sourceMode;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfig2DRegion
        {
            uint cx;
            uint cy;
        }

        [Flags]
        enum D3DmdtVideoSignalStandard : uint
        {
            Uninitialized = 0,
            VesaDmt = 1,
            VesaGtf = 2,
            VesaCvt = 3,
            Ibm = 4,
            Apple = 5,
            NtscM = 6,
            NtscJ = 7,
            Ntsc443 = 8,
            PalB = 9,
            PalB1 = 10,
            PalG = 11,
            PalH = 12,
            PalI = 13,
            PalD = 14,
            PalN = 15,
            PalNc = 16,
            SecamB = 17,
            SecamD = 18,
            SecamG = 19,
            SecamH = 20,
            SecamK = 21,
            SecamK1 = 22,
            SecamL = 23,
            SecamL1 = 24,
            Eia861 = 25,
            Eia861A = 26,
            Eia861B = 27,
            PalK = 28,
            PalK1 = 29,
            PalL = 30,
            PalM = 31,
            Other = 255
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigVideoSignalInfo
        {
            long pixelRate;
            DisplayConfigRational hSyncFreq;
            DisplayConfigRational vSyncFreq;
            DisplayConfig2DRegion activeSize;
            DisplayConfig2DRegion totalSize;

            D3DmdtVideoSignalStandard videoStandard;
            DisplayConfigScanLineOrdering ScanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigTargetMode
        {
            DisplayConfigVideoSignalInfo targetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PointL
        {
            int x;
            int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigSourceMode
        {
            uint width;
            uint height;
            DisplayConfigPixelFormat pixelFormat;
            PointL position;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigPathSourceInfo
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;

            public DisplayConfigSourceStatus statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigPathTargetInfo
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public DisplayConfigVideoOutputTechnology outputTechnology;
            public DisplayConfigRotation rotation;
            public DisplayConfigScaling scaling;
            public DisplayConfigRational refreshRate;
            public DisplayConfigScanLineOrdering scanLineOrdering;

            public bool targetAvailable;
            public DisplayConfigTargetStatus statusFlags;
        }

        [Flags]
        enum QueryDisplayFlags : uint
        {
            Zero = 0x0,

            AllPaths = 0x00000001,
            OnlyActivePaths = 0x00000002,
            DatabaseCurrent = 0x00000004
        }

        [Flags]
        enum DisplayConfigTopologyId : uint
        {
            Zero = 0x0,

            Internal = 0x00000001,
            Clone = 0x00000002,
            Extend = 0x00000004,
            External = 0x00000008,
            ForceUint32 = 0xFFFFFFFF
        }

        private enum DISPLAYCONFIG_COLOR_ENCODING
        {
            DISPLAYCONFIG_COLOR_ENCODING_RGB = 0,
            DISPLAYCONFIG_COLOR_ENCODING_YCBCR444 = 1,
            DISPLAYCONFIG_COLOR_ENCODING_YCBCR422 = 2,
            DISPLAYCONFIG_COLOR_ENCODING_YCBCR420 = 3,
            DISPLAYCONFIG_COLOR_ENCODING_INTENSITY = 4,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public uint value;
            public DISPLAYCONFIG_COLOR_ENCODING colorEncoding;
            public int bitsPerColorChannel;

            public bool advancedColorSupported => (value & 0x1) == 0x1;
            public bool advancedColorEnabled => (value & 0x2) == 0x2;
            public bool wideColorEnforced => (value & 0x4) == 0x4;
            public bool advancedColorForceDisabled => (value & 0x8) == 0x8;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_SET_ADVANCED_COLOR_INFO
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public uint enableAdvancedColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_SDR_WHITE_LEVEL
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public uint SDRWhiteLevel;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint value;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
            public DisplayConfigVideoOutputTechnology outputTechnology;
            public ushort edidManufactureId;
            public ushort edidProductCodeId;
            public uint connectorInstance;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string monitorFriendlyDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string monitorDevicePat;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_SOURCE_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string viewGdiDeviceName;
        }

        #endregion

        static class NativeMethods
        {
            [DllImport("User32.dll")]
            public static extern int GetDisplayConfigBufferSizes(QueryDisplayFlags flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

            [DllImport("User32.dll")]
            public static extern int SetDisplayConfig(uint numPathArrayElements, [In] DisplayConfigPathInfo[] pathArray, uint numModeInfoArrayElements, [In] DisplayConfigModeInfo[] modeInfoArray, SdcFlags flags);

            [DllImport("User32.dll")]
            public static extern int QueryDisplayConfig(QueryDisplayFlags flags, ref uint numPathArrayElements, [In, Out] DisplayConfigPathInfo[] pathInfoArray, ref uint modeInfoArrayElements, [In, Out] DisplayConfigModeInfo[] modeInfoArray, out DisplayConfigTopologyId id);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO requestPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigSetDeviceInfo(ref DISPLAYCONFIG_SET_ADVANCED_COLOR_INFO setPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME requestPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DEVICE_NAME requestPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SDR_WHITE_LEVEL requestPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigSetDeviceInfo(ref DISPLAYCONFIG_SDR_WHITE_LEVEL setPacket);
        }
    }
}
