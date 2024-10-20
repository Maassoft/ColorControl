using System.Diagnostics;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using SIZE_T = System.Int64;

namespace Shared.Native;

public static class Guids
{
    public const string IDXGIObject = "aec22fb8-76f3-4639-9be0-28eb43a67a2e";
    public const string IDXGIDeviceSubObject = "3d3e0379-f9de-4d58-bb6c-18d62992f1a6";
    public const string IDXGIResource = "035f3ab4-482e-4e50-b41f-8a7f8bd8960b";
    public const string IDXGIKeyedMutex = "9d8e1289-d7b3-465f-8126-250e349af85d";
    public const string IDXGISurface = "cafcb56c-6ac3-4889-bf47-9e23bbd260ec";
    public const string IDXGISurface1 = "4AE63092-6327-4c1b-80AE-BFE12EA32B86";
    public const string IDXGIAdapter = "2411e7e1-12ac-4ccf-bd14-9798e8534dc0";
    public const string IDXGIOutput = "ae02eedb-c735-4690-8d52-5a8dc20213aa";
    public const string IDXGISwapChain = "310d36a0-d2e7-4c0a-aa04-6a9d23b8886a";
    public const string IDXGIFactory = "7b7166ec-21c7-44ae-b21a-c9ae321ae369";
    public const string IDXGIDevice = "54ec77fa-1377-44e6-8c32-88fd5f44c84c";
    public const string IDXGIFactory1 = "770aae78-f26f-4dba-a829-253c83d1b387";
    public const string IDXGIAdapter1 = "29038f61-3839-4626-91fd-086879011a05";
    public const string IDXGIDevice1 = "77db970f-6276-48ba-ba28-070143b4392c";
    public const string IDXGIInfoQuery = "D67441C7-672A-476f-9E82-CD55B44949CE";

    #region dxgi1_2.h
    public const string IDXGIDisplayControl = "ea9dbf1a-c88e-4486-854a-98aa0138f30c";
    public const string IDXGIOutputDuplication = "191cfac3-a341-470d-b26e-a864f428319c";
    public const string IDXGISurface2 = "aba496dd-b617-4cb8-a866-bc44d7eb1fa2";
    public const string IDXGIResource1 = "30961379-4609-4a41-998e-54fe567ee0c1";
    public const string IDXGIDevice2 = "05008617-fbfd-4051-a790-144884b4f6a9";
    public const string IDXGISwapChain1 = "790a45f7-0d42-4876-983a-0a55cfe6f4aa";
    public const string IDXGIFactory2 = "50c83a1c-e072-4c48-87b0-3630fa36a6d0";
    public const string IDXGIAdapter2 = "0AA1AE0A-FA0E-4B84-8644-E05FF8E5ACB5";
    public const string IDXGIOutput1 = "00cddea8-939b-4b83-a340-a685226666cc";
    #endregion

    #region dxgi1_3.h
    public const string IDXGIDevice3 = "6007896c-3244-4afd-bf18-a6d3beda5023";
    public const string IDXGISwapChain2 = "a8be2ac4-199f-4946-b331-79599fb98de7";
    public const string IDXGIOutput2 = "595e39d1-2724-4663-99b1-da969de28364";
    public const string IDXGIFactory3 = "25483823-cd46-4c7d-86ca-47aa95b837bd";
    public const string IDXGIDecodeSwapChain = "2633066b-4514-4c7a-8fd8-12ea98059d18";
    public const string IDXGIFactoryMedia = "41e7d1f2-a591-4f7b-a2e5-fa9c843e1c12";
    public const string IDXGISwapChainMedia = "dd95b90b-f05f-4f6a-bd65-25bfb264bd84";
    public const string IDXGIOutput3 = "8a6bb301-7e7e-41F4-a8e0-5b32f7f99b18";
    #endregion

    #region dxgi1_4.h
    public const string IDXGISwapChain3 = "94d99bdb-f1f8-4ab0-b236-7da0170edab1";
    public const string IDXGIOutput4 = "dc7dca35-2196-414d-9F53-617884032a60";
    public const string IDXGIFactory4 = "1bc6ea02-ef36-464f-bf0c-21ca39e5168a";
    public const string IDXGIAdapter3 = "645967A4-1392-4310-A798-8053CE3E93FD";
    #endregion

    #region dxgi1_5.h
    public const string IDXGIOutput5 = "80A07424-AB52-42EB-833C-0C42FD282D98";
    #endregion

    public const string DEBUG_ALL = "e48ae283-da80-490b-87e6-43e9a9cfda08";
    public const string DEBUG_DX = "35cdd7fc-13b2-421d-a5d7-7e4451287d64";
    public const string DEBUG_DXGI = "25cddaa4-b1c6-47e1-ac3e-98875b5a2e2a";
    public const string DEBUG_APP = "06cd6e01-4219-4ebd-8709-27ed23360c62";
    public const string DEBUG_D3D11 = "4b99317b-ac39-4aa6-bb0b-baa04784798f";
}


public enum ADAPTER_FLAG
{
    NONE = 0,
    REMOTE = 1,
    SOFTWARE = 2,
}

public enum FEATURE_LEVEL
{
    LEVEL_1_0_CORE = 0x1000,
    LEVEL_9_1 = 0x9100,
    LEVEL_9_2 = 0x9200,
    LEVEL_9_3 = 0x9300,
    LEVEL_10_0 = 0xa000,
    LEVEL_10_1 = 0xa100,
    LEVEL_11_0 = 0xb000,
    LEVEL_11_1 = 0xb100,
    LEVEL_12_0 = 0xc000,
    LEVEL_12_1 = 0xc100
}

[Flags]
public enum PRESENT
{
    DEFAULT = 0,
    TEST = 0x00000001,
    DO_NOT_SEQUENCE = 0x00000002,
    RESTART = 0x00000004,
    DO_NOT_WAIT = 0x00000008,
    RESTRICT_TO_OUTPUT = 0x00000010,
    STEREO_PREFER_RIGHT = 0x00000020,
    STEREO_TEMPORARY_MONO = 0x00000040,
    USE_DURATION = 0x00000100,
    ALLOW_TEARING = 0x00000200,
}

[Flags]
public enum USAGE
{
    SHADER_INPUT = 1 << (0 + 4),
    RENDER_TARGET_OUTPUT = 1 << (1 + 4),
    BACK_BUFFER = 1 << (2 + 4),
    SHARED = 1 << (3 + 4),
    READ_ONLY = 1 << (4 + 4),
    DISCARD_ON_PRESENT = 1 << (5 + 4),
    UNORDERED_ACCESS = 1 << (6 + 4),
}

public enum MODE_SCANLINE_ORDER
{
    UNSPECIFIED = 0,
    PROGRESSIVE = 1,
    UPPER_FIELD_FIRST = 2,
    LOWER_FIELD_FIRST = 3,
}

public enum MODE_SCALING
{
    UNSPECIFIED = 0,
    CENTERED = 1,
    STRETCHED = 2,
}

public enum MODE_ROTATION
{
    UNSPECIFIED = 0,
    IDENTITY = 1,
    ROTATE90 = 2,
    ROTATE180 = 3,
    ROTATE270 = 4,
}

public enum RESOURCE_PRIORITY
{
    MINIMUM = 0x28000000,
    LOW = 0x50000000,
    NORMAL = 0x78000000,
    HIGH = unchecked((int)0xa0000000),
    MAXIMUM = unchecked((int)0xc8000000),
}

public enum RESIDENCY
{
    FULLY_RESIDENT = 1,
    RESIDENT_IN_SHARED_MEMORY = 2,
    EVICTED_TO_DISK = 3
}

public enum SWAP_EFFECT
{
    DISCARD = 0,
    SEQUENTIAL = 1,
    FLIP_SEQUENTIAL = 3,
    FLIP_DISCARD = 4
}

public enum SWAP_CHAIN_FLAG
{
    NONE = 0,
    NONPREROTATED = 1,
    ALLOW_MODE_SWITCH = 2,
    GDI_COMPATIBLE = 4
}

[Flags]
public enum ENUM_MODES
{
    INTERLACED = 1,
    SCALING = 2,
    STEREO = 4,
    DISABLED_STEREO = 8,
}

[Flags]
public enum MWA
{
    DEFAULT = 0,
    NO_WINDOW_CHANGES = 1 << 0,
    NO_ALT_ENTER = 1 << 1,
    NO_PRINT_SCREEN = 1 << 2,
    VALID = 0x7
}

public enum FORMAT
{
    UNKNOWN = 0,
    R32G32B32A32_TYPELESS = 1,
    R32G32B32A32_FLOAT = 2,
    R32G32B32A32_UINT = 3,
    R32G32B32A32_SINT = 4,
    R32G32B32_TYPELESS = 5,
    R32G32B32_FLOAT = 6,
    R32G32B32_UINT = 7,
    R32G32B32_SINT = 8,
    R16G16B16A16_TYPELESS = 9,
    R16G16B16A16_FLOAT = 10,
    R16G16B16A16_UNORM = 11,
    R16G16B16A16_UINT = 12,
    R16G16B16A16_SNORM = 13,
    R16G16B16A16_SINT = 14,
    R32G32_TYPELESS = 15,
    R32G32_FLOAT = 16,
    R32G32_UINT = 17,
    R32G32_SINT = 18,
    R32G8X24_TYPELESS = 19,
    D32_FLOAT_S8X24_UINT = 20,
    R32_FLOAT_X8X24_TYPELESS = 21,
    X32_TYPELESS_G8X24_UINT = 22,
    R10G10B10A2_TYPELESS = 23,
    R10G10B10A2_UNORM = 24,
    R10G10B10A2_UINT = 25,
    R11G11B10_FLOAT = 26,
    R8G8B8A8_TYPELESS = 27,
    R8G8B8A8_UNORM = 28,
    R8G8B8A8_UNORM_SRGB = 29,
    R8G8B8A8_UINT = 30,
    R8G8B8A8_SNORM = 31,
    R8G8B8A8_SINT = 32,
    R16G16_TYPELESS = 33,
    R16G16_FLOAT = 34,
    R16G16_UNORM = 35,
    R16G16_UINT = 36,
    R16G16_SNORM = 37,
    R16G16_SINT = 38,
    R32_TYPELESS = 39,
    D32_FLOAT = 40,
    R32_FLOAT = 41,
    R32_UINT = 42,
    R32_SINT = 43,
    R24G8_TYPELESS = 44,
    D24_UNORM_S8_UINT = 45,
    R24_UNORM_X8_TYPELESS = 46,
    X24_TYPELESS_G8_UINT = 47,
    R8G8_TYPELESS = 48,
    R8G8_UNORM = 49,
    R8G8_UINT = 50,
    R8G8_SNORM = 51,
    R8G8_SINT = 52,
    R16_TYPELESS = 53,
    R16_FLOAT = 54,
    D16_UNORM = 55,
    R16_UNORM = 56,
    R16_UINT = 57,
    R16_SNORM = 58,
    R16_SINT = 59,
    R8_TYPELESS = 60,
    R8_UNORM = 61,
    R8_UINT = 62,
    R8_SNORM = 63,
    R8_SINT = 64,
    A8_UNORM = 65,
    R1_UNORM = 66,
    R9G9B9E5_SHAREDEXP = 67,
    R8G8_B8G8_UNORM = 68,
    G8R8_G8B8_UNORM = 69,
    BC1_TYPELESS = 70,
    BC1_UNORM = 71,
    BC1_UNORM_SRGB = 72,
    BC2_TYPELESS = 73,
    BC2_UNORM = 74,
    BC2_UNORM_SRGB = 75,
    BC3_TYPELESS = 76,
    BC3_UNORM = 77,
    BC3_UNORM_SRGB = 78,
    BC4_TYPELESS = 79,
    BC4_UNORM = 80,
    BC4_SNORM = 81,
    BC5_TYPELESS = 82,
    BC5_UNORM = 83,
    BC5_SNORM = 84,
    B5G6R5_UNORM = 85,
    B5G5R5A1_UNORM = 86,
    B8G8R8A8_UNORM = 87,
    B8G8R8X8_UNORM = 88,
    R10G10B10_XR_BIAS_A2_UNORM = 89,
    B8G8R8A8_TYPELESS = 90,
    B8G8R8A8_UNORM_SRGB = 91,
    B8G8R8X8_TYPELESS = 92,
    B8G8R8X8_UNORM_SRGB = 93,
    BC6H_TYPELESS = 94,
    BC6H_UF16 = 95,
    BC6H_SF16 = 96,
    BC7_TYPELESS = 97,
    BC7_UNORM = 98,
    BC7_UNORM_SRGB = 99,
    AYUV = 100,
    Y410 = 101,
    Y416 = 102,
    NV12 = 103,
    P010 = 104,
    P016 = 105,
    OPAQUE_420 = 106,
    YUY2 = 107,
    Y210 = 108,
    Y216 = 109,
    NV11 = 110,
    AI44 = 111,
    IA44 = 112,
    P8 = 113,
    A8P8 = 114,
    B4G4R4A4_UNORM = 115,
    P208 = 130,
    V208 = 131,
    V408 = 132,
}

#region dxgidebug.h

public enum INFO_QUEUE_MESSAGE_CATEGORY
{
    UNKNOWN = 0,
    MISCELLANEOUS,
    INITIALIZATION,
    CLEANUP,
    COMPILATION,
    STATE_CREATION,
    STATE_SETTING,
    STATE_GETTING,
    RESOURCE_MANIPULATION,
    EXECUTION,
    SHADER
}

public enum INFO_QUEUE_MESSAGE_SEVERITY
{
    CORRUPTION = 0,
    ERROR,
    WARNING,
    INFO,
    MESSAGE
}
#endregion

#region dxgi1_2.h
public enum ALPHA_MODE
{
    UNSPECIFIED,
    PREMULTIPLIED,
    STRAIGHT,
    IGNORE,
}
public enum OFFER_RESOURCE_PRIORITY
{
    LOW = 1,
    NORMAL,
    HIGH
}
[Flags]
public enum SHARED_RESOURCE_FLAG
{
    READ = unchecked((int)0x80000000),
    WRITE = 1
}
public enum GRAPHICS_PREEMPTION_GRANULARITY
{
    DMA_BUFFER,
    PRIMITIVE,
    TRIANGLE,
    PIXEL,
    INSTRUCTION
}
public enum COMPUTE_PREEMPTION_GRANULARITY
{
    DMA_BUFFER,
    DISPATCH,
    THREAD_GROUP,
    THREAD,
    INSTRUCTION
}
#endregion

#region dxgi1_3.h
[Flags]
public enum MULTIPLANE_OVERLAY_YCbCr_FLAGS
{
    NOMINAL_RANGE = 0x1,
    BT709 = 0x2,
    xvYCC = 0x4
}
public enum FRAME_PRESENTATION_MODE
{
    COMPOSED,
    OVERLAY,
    NONE,
    COMPOSITION_FAILURE
}
[Flags]
public enum OVERLAY_SUPPORT_FLAG
{
    DIRECT = 0x1,
    SCALING = 0x2
}
public enum CREATE_FACTORY
{
    NONE,
    DEBUG,
}
#endregion

#region dxgi1_4.h
[Flags]
public enum SWAP_CHAIN_COLOR_SPACE_SUPPORT_FLAG
{
    PRESENT = 0x1,
    OVERLAY_PRESENT = 0x2
}
public enum COLOR_SPACE_TYPE
{
    RGB_FULL_G22_NONE_P709,
    RGB_FULL_G10_NONE_P709,
    RGB_STUDIO_G22_NONE_P709,
    RGB_STUDIO_G22_NONE_P2020,
    RESERVED,
    YCBCR_FULL_G22_NONE_P709_X601,
    YCBCR_STUDIO_G22_LEFT_P601,
    YCBCR_FULL_G22_LEFT_P601,
    YCBCR_STUDIO_G22_LEFT_P709,
    YCBCR_FULL_G22_LEFT_P709,
    YCBCR_STUDIO_G22_LEFT_P2020,
    YCBCR_FULL_G22_LEFT_P2020,
    CUSTOM = unchecked((int)0xFFFFFFFF)
}
public enum OVERLAY_COLOR_SPACE_SUPPORT_FLAG
{
    PRESENT = 0x1
}
public enum MEMORY_SEGMENT_GROUP
{
    LOCAL,
    NON_LOCAL
}
#endregion

[StructLayout(LayoutKind.Sequential)]
public struct FRAME_STATISTICS
{
    public uint PresentCount;
    public uint PresentRefreshCount;
    public uint SyncRefreshCount;
    public long SyncQPCTime;
    public long SyncGPUTime;
}

[StructLayout(LayoutKind.Sequential)]
public struct MAPPED_RECT
{
    public int Pitch;
    public byte[] pBits;
}

[StructLayout(LayoutKind.Sequential)]
public struct LUID
    : IEquatable<LUID>
{
    public int LowPart;
    public int HighPart;



    #region overload ------------------------------------------------------
    public static bool operator ==(LUID left, LUID right)
    {
        return left.LowPart == right.LowPart && left.HighPart == right.HighPart;
    }
    public static bool operator !=(LUID left, LUID right)
    {
        return left.LowPart != right.LowPart || left.HighPart != right.HighPart;
    }
    #endregion

    #region override ------------------------------------------------------
    public bool Equals(LUID right)
    {
        return LowPart == right.LowPart && HighPart == right.HighPart;
    }
    public override bool Equals(object obj)
    {
        if (obj is LUID right)
        {
            return LowPart == right.LowPart && HighPart == right.HighPart;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return LowPart ^ HighPart;
    }
    public override string ToString()
    {
        return string.Format("{0:X8}{1:X8}", HighPart, LowPart);
    }
    #endregion
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct ADAPTER_DESC
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string Description;
    public uint VendorId;
    public uint DeviceId;
    public uint SubSysId;
    public uint Revision;
    public SIZE_T DedicatedVideoMemory;
    public SIZE_T DedicatedSystemMemory;
    public SIZE_T SharedSystemMemory;
    public LUID AdapterLuid;

    public override string ToString()
    {
        return string.Format(
            "{{ Description: {0:-128}, VendorId: 0x{1:X8}, DeviceId: 0x{2:X8}, SubSysId: 0x{3:X8}, Revision: 0x{4:X8}, DedicatedVideoMemory: {5,20}bytes, DedicatedSystemMemory: {6,20}bytes, SharedSystemMemory: {7,20}bytes, AdapterLuid: 0x{8} }}",
            Description,
            VendorId,
            DeviceId,
            SubSysId,
            Revision,
            DedicatedVideoMemory,
            DedicatedSystemMemory,
            SharedSystemMemory,
            AdapterLuid);
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct ADAPTER_DESC1
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string Description;
    public uint VendorId;
    public uint DeviceId;
    public uint SubSysId;
    public uint Revision;
    public SIZE_T DedicatedVideoMemory;
    public SIZE_T DedicatedSystemMemory;
    public SIZE_T SharedSystemMemory;
    public LUID AdapterLuid;
    public ADAPTER_FLAG Flags;

    public override string ToString()
    {
        return string.Format(
            "{{ Description: {0:-128}, VendorId: 0x{1:X8}, DeviceId: 0x{2:X8}, SubSysId: 0x{3:X8}, Revision: 0x{4:X8}, DedicatedVideoMemory: {5,20}bytes, DedicatedSystemMemory: {6,20}bytes, SharedSystemMemory: {7,20}bytes, AdapterLuid: 0x{8}, Flags:0x{9:X8} }}",
            Description,
            VendorId,
            DeviceId,
            SubSysId,
            Revision,
            DedicatedVideoMemory,
            DedicatedSystemMemory,
            SharedSystemMemory,
            AdapterLuid,
            Flags);
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct OUTPUT_DESC
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string DeviceName;
    public RECT DesktopCoordinates;
    [MarshalAs(UnmanagedType.Bool)]
    public bool AttachedToDesktop;
    public MODE_ROTATION Rotation;
    public IntPtr Monitor;

    public override string ToString()
    {
        return string.Format(
            "{{ DeviceName: {0,-32}, DesktopCoordinates: {1}, AttachedToDesktop: {2}, Rotation: {3}, MonitorHandle: 0x{4:X16} }}",
            DeviceName,
            DesktopCoordinates,
            AttachedToDesktop,
            Enum.GetName(typeof(MODE_ROTATION), Rotation),
            Monitor);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct SHARED_RESOURCE
{
    public IntPtr Handle;
}

[StructLayout(LayoutKind.Sequential)]
public struct SURFACE_DESC
{
    public uint Width;
    public uint Height;
    public FORMAT Format;
    public SAMPLE_DESC SampleDesc;
}

[StructLayout(LayoutKind.Sequential)]
public struct SWAP_CHAIN_DESC
{
    public MODE_DESC BufferDesc;
    public SAMPLE_DESC SampleDesc;
    public USAGE BufferUsage;
    public uint BufferCount;
    public IntPtr OutputWindow;
    [MarshalAs(UnmanagedType.Bool)]
    public bool Windowed;
    public SWAP_EFFECT SwapEffect;
    public SWAP_CHAIN_FLAG Flags;

    public SWAP_CHAIN_DESC(Control control, bool windowed, USAGE usage = USAGE.RENDER_TARGET_OUTPUT)
    {
        Debug.Assert(control != null);

        var size = control.ClientSize;

        OutputWindow = control.Handle;
        BufferDesc = new MODE_DESC
        {
            Width = unchecked((uint)size.Width),
            Height = unchecked((uint)size.Height)
        };

        SampleDesc = new SAMPLE_DESC
        {
            Count = 1
        };

        Windowed = windowed;
        BufferUsage = usage;
        BufferCount = 2;
        SwapEffect = SWAP_EFFECT.DISCARD;
        Flags = SWAP_CHAIN_FLAG.NONE;
    }
    public SWAP_CHAIN_DESC(in SWAP_CHAIN_DESC src)
    {
        this.BufferDesc = src.BufferDesc;
        this.SampleDesc = src.SampleDesc;
        this.BufferUsage = src.BufferUsage;
        this.BufferCount = src.BufferCount;
        this.OutputWindow = src.OutputWindow;
        this.Windowed = src.Windowed;
        this.SwapEffect = src.SwapEffect;
        this.Flags = src.Flags;
    }
    public void SetMode(in MODE_DESC mode)
    {
        BufferDesc = mode;
    }
    public override string ToString()
    {
        return string.Format("{0}, c:{1}, w:{2}", BufferDesc, BufferCount, Windowed);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct COLORVALUE
{
    public float r;
    public float g;
    public float b;
    public float a;



    public COLORVALUE(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
    public COLORVALUE(Color color)
        : this(color, color.A)
    {
    }
    public COLORVALUE(Color color, int alpha)
    {
        this.r = color.R / 256f;
        this.g = color.G / 256f;
        this.b = color.B / 256f;
        this.a = alpha / 256f;
    }
    public static implicit operator COLORVALUE(Color color)
    {
        return new COLORVALUE(color);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct RGB
{
    public float Red;
    public float Green;
    public float Blue;
}

[StructLayout(LayoutKind.Sequential)]
public struct GAMMA_CONTROL
{
    public RGB Scale;
    public RGB Offset;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025)]
    public RGB[] GammaCurve;
}

[StructLayout(LayoutKind.Sequential)]
public struct GAMMA_CONTROL_CAPABILITIES
{
    [MarshalAs(UnmanagedType.Bool)]
    public bool ScaleAndOffsetSupported;
    public float MaxConvertedValue;
    public float MinConvertedValue;
    public uint NumGammaControlPoints;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025)]
    public float[] ControlPointPositions;
}

[StructLayout(LayoutKind.Sequential)]
public struct RATIONAL
    : IEquatable<RATIONAL>
    , IComparable<RATIONAL>
{
    public uint Numerator;
    public uint Denominator;



    public RATIONAL(uint numerator, uint denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }


    #region override ------------------------------------------------------
    public override string ToString()
    {
        if (Denominator == 0)
        {
            Denominator = 1;
        }
        return string.Format("{0,2:N}Hz", Numerator / (float)Denominator);
    }
    public override int GetHashCode()
    {
        return unchecked((int)(Numerator ^ Denominator));
    }
    public override bool Equals(object other)
    {
        if (other is RATIONAL rational)
        {
            return Equals(rational);
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region overload ------------------------------------------------------
    public static bool operator ==(RATIONAL l, RATIONAL r)
    {
        return l.Equals(r);
    }
    public static bool operator !=(RATIONAL l, RATIONAL r)
    {
        return !l.Equals(r);
    }
    #endregion

    #region implements ----------------------------------------------------
    public bool Equals(RATIONAL other)
    {
        return Numerator == other.Numerator
            && Denominator == other.Denominator;
    }
    public int CompareTo(RATIONAL other)
    {
        var l = Numerator / (float)Denominator;
        var r = other.Numerator / (float)other.Denominator;

        if (l < r)
        {
            return -1;
        }
        else if (l > r)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    #endregion
}

[StructLayout(LayoutKind.Sequential)]
public struct MODE_DESC
    : IEquatable<MODE_DESC>
{
    #region field ---------------------------------------------------------------
    public RESOLUTION Resolution;
    public RATIONAL RefreshRate;
    public FORMAT Format;
    public MODE_SCANLINE_ORDER ScanlineOrdering;
    public MODE_SCALING Scaling;
    #endregion



    #region property ------------------------------------------------------------
    public uint Width
    {
        get => Resolution.width;
        set => Resolution.width = value;
    }
    public uint Height
    {
        get => Resolution.height;
        set => Resolution.height = value;
    }
    #endregion

    #region override ------------------------------------------------------------
    public override string ToString()
    {
        return string.Format(
            "{{ Resolution : {0}, RefreshRate: {1}, Format: {2,-20}, ScanlineOrdering: {3,-16}, Scaling: {4,-16} }}",
            Resolution,
            RefreshRate,
            Enum.GetName(typeof(FORMAT), Format),
            Enum.GetName(typeof(MODE_SCANLINE_ORDER), ScanlineOrdering),
            Enum.GetName(typeof(MODE_SCALING), Scaling));
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override bool Equals(object other)
    {
        if (other is MODE_DESC desc)
        {
            return Equals(desc);
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region overload ------------------------------------------------------------
    public bool Equals(MODE_DESC other)
    {
        return (this.Resolution == other.Resolution
            && this.RefreshRate == other.RefreshRate
            && this.Format == other.Format
        //  &&      this.ScanlineOrdering == other.ScanlineOrdering
        //  &&      this.Scaling == other.Scaling
        );
    }
    public static bool operator ==(MODE_DESC l, MODE_DESC r)
    {
        return l.Equals(r);
    }
    public static bool operator !=(MODE_DESC l, MODE_DESC r)
    {
        return !l.Equals(r);
    }
    #endregion
}

[StructLayout(LayoutKind.Sequential)]
public struct RESOLUTION
    : IComparable<RESOLUTION>
{
    #region field ---------------------------------------------------------------
    public uint width;
    public uint height;
    #endregion



    #region construction --------------------------------------------------------
    public RESOLUTION(Size size)
    {
        width = (uint)size.Width;
        height = (uint)size.Height;
    }
    public RESOLUTION(uint width, uint height)
    {
        this.width = width;
        this.height = height;
    }
    #endregion

    #region override ------------------------------------------------------------
    public override bool Equals(object obj)
    {
        if (obj is RESOLUTION res)
        {
            return res.width == width && res.height == height;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return unchecked((int)(width ^ height));
    }
    public override string ToString()
    {
        return string.Format("{0}x{1}", width, height);
    }
    #endregion

    #region operator ------------------------------------------------------------
    public static bool operator ==(RESOLUTION l, RESOLUTION r)
    {
        return l.Equals(r);
    }
    public static bool operator !=(RESOLUTION l, RESOLUTION r)
    {
        return !l.Equals(r);
    }
    public static implicit operator Size(RESOLUTION l)
    {
        return new Size((int)l.width, (int)l.height);
    }
    #endregion

    #region IComparable implementation ------------------------------------------
    public int CompareTo(RESOLUTION other)
    {
        if (width < other.width)
        {
            return -1;
        }
        else if (width > other.width)
        {
            return 1;
        }
        else if (height < other.height)
        {
            return -1;
        }
        else if (height > other.height)
        {
            return 1;
        }
        return 0;
    }
    #endregion
}

[StructLayout(LayoutKind.Sequential)]
public struct SAMPLE_DESC
{
    public uint Count;
    public uint Quality;

    public SAMPLE_DESC(uint count, uint quality)
    {
        Count = count;
        Quality = quality;
    }
}

#region dxgidebug.h
[StructLayout(LayoutKind.Sequential)]
public struct INFO_QUEUE_MESSAGE
{
    public Guid Producer;
    public INFO_QUEUE_MESSAGE_CATEGORY Category;
    public INFO_QUEUE_MESSAGE_SEVERITY Severity;
    public int ID;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pDescription;
    public SIZE_T DescriptionByteLength;
}

[StructLayout(LayoutKind.Sequential)]
public struct INFO_QUEUE_FILTER_DESC
{
    public int NumCategories;
    [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 1)]
    public INFO_QUEUE_MESSAGE_CATEGORY[] pCategoryList;
    public int NumSeverities;
    [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 3)]
    public INFO_QUEUE_MESSAGE_SEVERITY[] pSeverityList;
    public int NumIDs;
    [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 5)]
    public int[] pIDList;
}

[StructLayout(LayoutKind.Sequential)]
public class INFO_QUEUE_FILTER
{
    public INFO_QUEUE_FILTER_DESC AllowList;
    public INFO_QUEUE_FILTER_DESC DenyList;
}
#endregion

#region dxgi1_2.h
[StructLayout(LayoutKind.Sequential)]
public struct OUTDUPL_MOVE_RECT
{
    public POINT SourcePoint;
    public RECT DestinationRect;
}
[StructLayout(LayoutKind.Sequential)]
public struct OUTDUPL_DESC
{
    public MODE_DESC ModeDesc;
    public MODE_ROTATION Rotation;
    [MarshalAs(UnmanagedType.Bool)]
    public bool DesktopImageInSystemMemory;
}

[StructLayout(LayoutKind.Sequential)]
public struct OUTDUPL_POINTER_POSITION
{
    public POINT Position;
    [MarshalAs(UnmanagedType.Bool)]
    public bool Visible;
}

[StructLayout(LayoutKind.Sequential)]
public struct OUTDUPL_POINTER_SHAPE_INFO
{
    public uint Type;
    public uint Width;
    public uint Height;
    public uint Pitch;
    public POINT HotSpot;
}

[StructLayout(LayoutKind.Sequential)]
public struct OUTDUPL_FRAME_INFO
{
    public long LastPresentTime;
    public long LastMouseUpdateTime;
    public uint AccumulatedFrames;
    public bool RectsCoalesced;
    public bool ProtectedContentMaskedOut;
    public OUTDUPL_POINTER_POSITION PointerPosition;
    public uint TotalMetadataBufferSize;
    public uint PointerShapeBufferSize;
}

[StructLayout(LayoutKind.Sequential)]
public struct MODE_DESC1
    : IEquatable<MODE_DESC1>
{
    #region field ---------------------------------------------------------------
    public RESOLUTION Resolution;
    public RATIONAL RefreshRate;
    public FORMAT Format;
    public MODE_SCANLINE_ORDER ScanlineOrdering;
    public MODE_SCALING Scaling;
    [MarshalAs(UnmanagedType.Bool)]
    public bool Stereo;
    #endregion



    #region property ------------------------------------------------------------
    public uint Width
    {
        get => Resolution.width;
        set => Resolution.width = value;
    }
    public uint Height
    {
        get => Resolution.height;
        set => Resolution.height = value;
    }
    #endregion

    #region override ------------------------------------------------------------
    public override string ToString()
    {
        return string.Format(
            "{{ Resolution : {0}, RefreshRate: {1}, Format: {2,-20}, ScanlineOrdering: {3,-16}, Scaling: {4,-16} }}",
            Resolution,
            RefreshRate,
            Enum.GetName(typeof(FORMAT), Format),
            Enum.GetName(typeof(MODE_SCANLINE_ORDER), ScanlineOrdering),
            Enum.GetName(typeof(MODE_SCALING), Scaling));
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override bool Equals(object other)
    {
        if (other is MODE_DESC1 desc)
        {
            return Equals(desc);
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region overload ------------------------------------------------------------
    public bool Equals(MODE_DESC1 other)
    {
        return (this.Resolution == other.Resolution
            && this.RefreshRate == other.RefreshRate
            && this.Format == other.Format
            && this.Stereo == other.Stereo
        //  &&      this.ScanlineOrdering == other.ScanlineOrdering
        //  &&      this.Scaling == other.Scaling
        );
    }
    public static bool operator ==(MODE_DESC1 l, MODE_DESC1 r)
    {
        return l.Equals(r);
    }
    public static bool operator !=(MODE_DESC1 l, MODE_DESC1 r)
    {
        return !l.Equals(r);
    }
    public static implicit operator MODE_DESC(MODE_DESC1 src)
    {
        var result = new MODE_DESC
        {
            Resolution = src.Resolution,
            RefreshRate = src.RefreshRate,
            Format = src.Format,
            ScanlineOrdering = src.ScanlineOrdering,
            Scaling = src.Scaling
        };
        return result;
    }
    #endregion
}
[StructLayout(LayoutKind.Sequential)]
public struct SWAP_CHAIN_DESC1
{
    public RESOLUTION Resolution;
    public FORMAT Format;
    [MarshalAs(UnmanagedType.Bool)]
    public bool Stereo;
    public SAMPLE_DESC SampleDesc;
    public USAGE BufferUsage;
    public uint BufferCount;
    public MODE_SCALING Scaling;
    public SWAP_EFFECT SwapEffect;
    public ALPHA_MODE AlphaMode;
    public SWAP_CHAIN_FLAG Flags;



    public SWAP_CHAIN_DESC1(Control control, int count = 2, USAGE usage = USAGE.RENDER_TARGET_OUTPUT)
    {
        Debug.Assert(control != null);

        var size = control.ClientSize;

        Format = FORMAT.R8G8B8A8_UNORM;
        Resolution = new RESOLUTION
        {
            width = unchecked((uint)size.Width),
            height = unchecked((uint)size.Height)
        };
        Stereo = false;
        SampleDesc = new SAMPLE_DESC
        {
            Count = 1
        };
        Scaling = MODE_SCALING.UNSPECIFIED;
        SwapEffect = SWAP_EFFECT.FLIP_DISCARD;
        AlphaMode = ALPHA_MODE.IGNORE;
        BufferUsage = usage;
        BufferCount = unchecked((uint)count);
        Flags = SWAP_CHAIN_FLAG.NONE;
    }
    public SWAP_CHAIN_DESC1(in SWAP_CHAIN_DESC1 src)
    {
        this.Resolution = src.Resolution;
        this.Format = src.Format;
        this.Stereo = src.Stereo;
        this.SampleDesc = src.SampleDesc;
        this.BufferUsage = src.BufferUsage;
        this.BufferCount = src.BufferCount;
        this.Scaling = src.Scaling;
        this.SwapEffect = src.SwapEffect;
        this.AlphaMode = src.AlphaMode;
        this.Flags = src.Flags;
    }
    public override string ToString()
    {
        return string.Format("{0}, c:{1}, f:{2}", Resolution, BufferCount, Format);
    }
}
[StructLayout(LayoutKind.Sequential)]
public struct SWAP_CHAIN_FULLSCREEN_DESC
{
    public RATIONAL RefreshRate;
    public MODE_SCANLINE_ORDER ScanlineOrdering;
    public MODE_SCALING Scaling;
    [MarshalAs(UnmanagedType.Bool)]
    public bool Windowed;


    public SWAP_CHAIN_FULLSCREEN_DESC(in RATIONAL refresh, bool windowed = true)
    {
        RefreshRate = refresh;
        ScanlineOrdering = MODE_SCANLINE_ORDER.PROGRESSIVE;
        Scaling = MODE_SCALING.CENTERED;
        Windowed = windowed;
    }
    public SWAP_CHAIN_FULLSCREEN_DESC(in SWAP_CHAIN_FULLSCREEN_DESC src)
    {
        RefreshRate = src.RefreshRate;
        ScanlineOrdering = src.ScanlineOrdering;
        Scaling = src.Scaling;
        Windowed = src.Windowed;
    }
}
[StructLayout(LayoutKind.Sequential)]
public struct PRESENT_PARAMETERS
{
    public uint DirtyRectsCount;
    [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 0)]
    public RECT[] pDirtyRects;
    [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 0)]
    public RECT[] pScrollRect;
    [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 0)]
    public POINT[] pScrollOffset;
}
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct ADAPTER_DESC2
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string Description;
    public uint VendorId;
    public uint DeviceId;
    public uint SubSysId;
    public uint Revision;
    public SIZE_T DedicatedVideoMemory;
    public SIZE_T DedicatedSystemMemory;
    public SIZE_T SharedSystemMemory;
    public LUID AdapterLuid;
    public ADAPTER_FLAG Flags;
    public GRAPHICS_PREEMPTION_GRANULARITY GraphicsPreemptionGranularity;
    public COMPUTE_PREEMPTION_GRANULARITY ComputePreemptionGranularity;


    public override string ToString()
    {
        return string.Format(
            "{{ Description: {0:-128}, VendorId: 0x{1:X8}, DeviceId: 0x{2:X8}, SubSysId: 0x{3:X8}, Revision: 0x{4:X8}, DedicatedVideoMemory: {5,20}bytes, DedicatedSystemMemory: {6,20}bytes, SharedSystemMemory: {7,20}bytes, AdapterLuid: 0x{8}, Flags:0x{9:X8}, GraphicsPreemptionGranularity:{10}, ComputePreemptionGranularity:{11} }}",
            Description,
            VendorId,
            DeviceId,
            SubSysId,
            Revision,
            DedicatedVideoMemory,
            DedicatedSystemMemory,
            SharedSystemMemory,
            AdapterLuid,
            Flags,
            GraphicsPreemptionGranularity,
            ComputePreemptionGranularity);
    }
}
#endregion

#region dxgi1_3.h
[StructLayout(LayoutKind.Sequential)]
public struct DECODE_SWAP_CHAIN_DESC
{
    public uint Flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct FRAME_STATISTICS_MEDIA
{
    public uint PresentCount;
    public uint PresentRefreshCount;
    public uint SyncRefreshCount;
    public long SyncQPCTime;
    public long SyncGPUTime;
    public FRAME_PRESENTATION_MODE CompositionMode;
    public uint ApprovedPresentDuration;
}
#endregion

#region dxgi1_4.h
[StructLayout(LayoutKind.Sequential)]
public struct QUERY_VIDEO_MEMORY_INFO
{
    public ulong Budget;
    public ulong CurrentUsage;
    public ulong AvailableForReservation;
    public ulong CurrentReservation;
}
#endregion

public class DXNativeMethods : SafeNativeMethods
{
    internal const string DLL_NAME = "dxgi.dll";

    [DllImport(DLL_NAME)]
    internal static extern int CreateDXGIFactory(
                in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                    out object              ppFactory);

    [DllImport(DLL_NAME)]
    internal static extern int CreateDXGIFactory1(
                in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                    out object              ppFactory);

    [DllImport(DLL_NAME)]
    internal static extern int CreateDXGIFactory2(
                    CREATE_FACTORY Flags,
                in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                    out object              ppFactory);

#if DEBUG
    internal const string DEBUG_DLL_NAME = "dxgidebug.dll";

    [DllImport(DEBUG_DLL_NAME)]
    internal static extern int DXGIGetDebugInterface(
                in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                    out object              ppDebug);

    [DllImport(DEBUG_DLL_NAME)]
    internal static extern int DXGIGetDebugInterface1(
                    uint Flags,
                in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                    out object              ppDebug);
#endif
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIObject)]
public interface IDXGIObject
{
    void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    void GetParent(
                        in Guid Name,
                        out object ppParent);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIDeviceSubObject)]
public interface IDXGIDeviceSubObject
    : IDXGIObject
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    #endregion
    void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIResource)]
public interface IDXGIResource
    : IDXGIDeviceSubObject
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    #endregion
    void GetSharedHandle(
                out IntPtr pSharedHandle);

    void GetUsage(
                out USAGE pUsage);

    void SetEvictionPriority(
                    RESOURCE_PRIORITY EvictionPriority);

    void GetEvictionPriority(
                out RESOURCE_PRIORITY pEvictionPriority);

}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIKeyedMutex)]
public interface IDXGIKeyedMutex
    : IDXGIDeviceSubObject
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    #endregion
    void AcquireSync(
                        ulong Key,
                        uint dwMilliseconds);

    void ReleaseSync(
                        ulong Key);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGISurface)]
public interface IDXGISurface
    : IDXGIDeviceSubObject
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    #endregion
    void GetDesc(
                out SURFACE_DESC pDesc);
    void Map(
                out MAPPED_RECT pLockedRect,
                    int MapFlags);
    void Unmap();
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGISurface1)]
public interface IDXGISurface1
    : IDXGISurface
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    new void GetDesc(
                        out SURFACE_DESC pDesc);
    new void Map(
                        out MAPPED_RECT pLockedRect,
                            int MapFlags);
    new void Unmap();
    #endregion
    void GetDC(
        [MarshalAs( UnmanagedType.Bool )]
                        bool        Discard,
                out IntPtr phdc);
    void ReleaseDC(
        [Optional]
                    in  RECT        pDirtyRect);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIAdapter)]
public interface IDXGIAdapter
    : IDXGIObject
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    #endregion
    [PreserveSig]
    int EnumOutputs(
                        uint Output,
                    out IDXGIOutput ppOutput);
    void GetDesc(
                    out ADAPTER_DESC pDesc);
    [PreserveSig]
    int CheckInterfaceSupport(
                    in Guid InterfaceName,
                    out long pUMDVersion);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIOutput)]
public interface IDXGIOutput
    : IDXGIObject
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    #endregion
    void GetDesc(
                        out OUTPUT_DESC pDesc);
    void GetDisplayModeList(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC[]                 pDesc);
    void FindClosestMatchingMode(
                        in MODE_DESC pModeToMatch,
                        out MODE_DESC pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                      pConcernedDevice);
    void TakeOwnership(
        [MarshalAs( UnmanagedType.Interface )]
                            object                      pDevice,
        [MarshalAs( UnmanagedType.Bool )]
                            bool                        Exclusive);
    void ReleaseOwnership();
    void GetGammaControlCapabilities(
                    out GAMMA_CONTROL_CAPABILITIES pGammaCaps);
    void SetGammaControl(
                    in GAMMA_CONTROL pArray);
    void GetGammaControl(
                    out GAMMA_CONTROL pArray);
    void SetDisplaySurface(
                        IDXGISurface pScanoutSurface);
    void GetDisplaySurfaceData(
                    out IDXGISurface pDestination);
    void GetFrameStatistics(
                    out FRAME_STATISTICS pStats);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGISwapChain)]
public interface IDXGISwapChain
    : IDXGIDeviceSubObject
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    #endregion
    [PreserveSig]
    int Present(
                            uint SyncInterval,
                            PRESENT Flags);
    void GetBuffer(
                        uint Buffer,
                    in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                        out object                                          ppSurface);
    void SetFullscreenState(
        [MarshalAs( UnmanagedType.Bool )]
                            bool                                            Fullscreen,
        [Optional] IDXGIOutput pTarget);
    void GetFullscreenState(
        [MarshalAs( UnmanagedType.Bool )]
                        out bool            pFullscreen,
        [Optional] ref IDXGIOutput ppTarget);
    void GetDesc(
                    out SWAP_CHAIN_DESC pDesc);
    void ResizeBuffers(
                            uint BufferCount,
                            uint Width,
                            uint Height,
                            FORMAT NewFormat,
                            SWAP_CHAIN_FLAG SwapChainFlags);
    void ResizeTarget(
                        in MODE_DESC pNewTargetParameters);
    void GetContainingOutput(
                        out IDXGIOutput ppOutput);
    void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    void GetLastPresentCount(
                        out uint pLastPresentCount);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIFactory)]
public interface IDXGIFactory
    : IDXGIObject
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);
    #endregion

    [PreserveSig]
    int EnumAdapters(
                            uint Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                            out IDXGIAdapter                                ppAdapter);
    void MakeWindowAssociation(
                        IntPtr WindowHandle,
                        MWA Flags);
    void GetWindowAssociation(
                    out IntPtr pWindowHandle);
    void CreateSwapChain(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                          pDevice,
                    in SWAP_CHAIN_DESC pDesc,
                    out IDXGISwapChain ppSwapChain);
    void CreateSoftwareAdapter(
                        IntPtr Module,
                    out IDXGIAdapter ppAdapter);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIDevice)]
public interface IDXGIDevice
    : IDXGIObject
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);
    #endregion

    void GetAdapter(
                    out IDXGIAdapter pAdapter);
    void CreateSurface(
                    in SURFACE_DESC pDesc,
                        uint NumSurfaces,
                        USAGE Usage,
        [Optional] in SHARED_RESOURCE pSharedResource,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGISurface                                    ppSurface);
    void QueryResourceResidency(
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 2)]
                            object[]                                        ppResources,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 2)]
                        out RESIDENCY[]                                     pResidencyStatus,
                        uint NumResources);
    void SetGPUThreadPriority(
                        int Priority);
    void GetGPUThreadPriority(
                    out int pPriority);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIFactory1)]
public interface IDXGIFactory1
    : IDXGIFactory
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);

    [PreserveSig]
    new int EnumAdapters(
                        uint Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGIAdapter                                ppAdapter);

    new void MakeWindowAssociation(
                        IntPtr WindowHandle,
                        MWA Flags);

    new void GetWindowAssociation(
                    out IntPtr pWindowHandle);

    new void CreateSwapChain(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                    in SWAP_CHAIN_DESC pDesc,
                    out IDXGISwapChain ppSwapChain);

    new void CreateSoftwareAdapter(
                        IntPtr Module,
                    out IDXGIAdapter ppAdapter);
    #endregion
    [PreserveSig]
    int EnumAdapters1(
                            int Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                            out IDXGIAdapter1                               ppAdapter);
    [return: MarshalAs(UnmanagedType.Bool)]
    bool IsCurrent();
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIAdapter1)]
public interface IDXGIAdapter1
    : IDXGIAdapter
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);

    [PreserveSig]
    new int EnumOutputs(
                        uint Output,
                    out IDXGIOutput ppOutput);

    new void GetDesc(
                    out ADAPTER_DESC pDesc);

    new void CheckInterfaceSupport(
                    in Guid InterfaceName,
                    out long pUMDVersion);
    #endregion

    void GetDesc1(
                        out ADAPTER_DESC1 pDesc);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIDevice1)]
public interface IDXGIDevice1
    : IDXGIDevice
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);

    new void GetAdapter(
                    out IDXGIAdapter pAdapter);

    new void CreateSurface(
                    in SURFACE_DESC pDesc,
                        uint NumSurfaces,
                        USAGE Usage,
                    in SHARED_RESOURCE pSharedResource,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGISurface                                ppSurface);

    new void QueryResourceResidency(
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 2)]
                            object[]                                    ppResources,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 2)]
                        out RESIDENCY[]                                 pResidencyStatus,
                        uint NumResources);

    new void SetGPUThreadPriority(
                        int Priority);

    new void GetGPUThreadPriority(
                    out int pPriority);
    #endregion

    void SetMaximumFrameLatency(
                            int MaxLatency);
    void GetMaximumFrameLatency(
                        out int pMaxLatency);
}

#region dxgidebug.h
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIInfoQuery)]
public interface IDXGIInfoQueue
{
    void SetMessageCountLimit(
                        in Guid Producer,
                            long MessageCountLimit);
    void ClearStoredMessages(
                        in Guid Producer);
    void GetMessage(
                        in Guid Producer,
                            long MessageIndex,
        [Optional] out INFO_QUEUE_MESSAGE pMessage,
                        ref SIZE_T pMessageByteLength);
    [PreserveSig]
    long GetNumStoredMessagesAllowedByRetrievalFilters(
                        in Guid Producer);
    [PreserveSig]
    long GetNumStoredMessages(
                        in Guid Producer);
    [PreserveSig]
    long GetNumMessagesDiscardedByMessageCountLimit(
                        in Guid Producer);
    [PreserveSig]
    long GetMessageCountLimit(
                        in Guid Producer);
    [PreserveSig]
    long GetNumMessagesAllowedByStorageFilter(
                        in Guid Producer);
    [PreserveSig]
    long GetNumMessagesDeniedByStorageFilter(
                        in Guid Producer);
    void AddStorageFilterEntries(
                        in Guid Producer,
                        in INFO_QUEUE_FILTER pFilter);
    void GetStorageFilter(
                        in Guid Producer,
        [Optional] out INFO_QUEUE_FILTER pFilter,
                        ref SIZE_T pFilterByteLength);
    void ClearStorageFilter(
                        in Guid Producer);
    void PushEmptyStorageFilter(
                        in Guid Producer);
    void PushDenyAllStorageFilter(
                        in Guid Producer);
    void PushCopyOfStorageFilter(
                        in Guid Producer);
    void PushStorageFilter(
                        in Guid Producer,
                        in INFO_QUEUE_FILTER pFilter);
    void PopStorageFilter(
                        in Guid Producer);
    [PreserveSig]
    int GetStorageFilterStackSize(
                        in Guid Producer);
    void AddRetrievalFilterEntries(
                        in Guid Producer,
                        in INFO_QUEUE_FILTER pFilter);
    void GetRetrievalFilter(
                        in Guid Producer,
        [Optional] out INFO_QUEUE_FILTER pFilter,
                        ref SIZE_T pFilterByteLength);
    void ClearRetrievalFilter(
                        in Guid Producer);
    void PushEmptyRetrievalFilter(
                        in Guid Producer);
    void PushDenyAllRetrievalFilter(
                        in Guid Producer);
    void PushCopyOfRetrievalFilter(
                        in Guid Producer);
    void PushRetrievalFilter(
                        in Guid Producer,
                        in INFO_QUEUE_FILTER pFilter);
    void PopRetrievalFilter(
                        in Guid Producer);
    [PreserveSig]
    int GetRetrievalFilterStackSize(
                        in Guid Producer);
    void AddMessage(
                        in Guid Producer,
                            INFO_QUEUE_MESSAGE_CATEGORY Category,
                            INFO_QUEUE_MESSAGE_SEVERITY Severity,
                            int ID,
        [MarshalAs( UnmanagedType.LPStr )]
                                string                                      pDescription);
    void AddApplicationMessage(
                            INFO_QUEUE_MESSAGE_SEVERITY Severity,
        [MarshalAs( UnmanagedType.LPStr )]
                                string                                      pDescription);
    void SetBreakOnCategory(
                        in Guid Producer,
                            INFO_QUEUE_MESSAGE_CATEGORY Category,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                        bEnable);
    void SetBreakOnSeverity(
                        in Guid Producer,
                            INFO_QUEUE_MESSAGE_SEVERITY Severity,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                        bEnable);
    void SetBreakOnID(
                        in Guid Producer,
                            int ID,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                        bEnable);
    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetBreakOnCategory(
                        in Guid Producer,
                            INFO_QUEUE_MESSAGE_CATEGORY Category);
    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetBreakOnSeverity(
                        in Guid Producer,
                            INFO_QUEUE_MESSAGE_SEVERITY Severity);
    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetBreakOnID(
                        in Guid Producer,
                            int ID);
    void SetMuteDebugOutput(
                        in Guid Producer,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                        bMute);
    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetMuteDebugOutput(
                        in Guid Producer);
}
#endregion

#region dxgi1_2.h
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIDisplayControl)]
public interface IDXGIDisplayControl
{
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    bool IsStereoEnabled();
    [PreserveSig]
    void SetStereoEnabled(
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    enabled);

}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIOutputDuplication)]
public interface IDXGIOutputDuplication
    : IDXGIObject
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);
    #endregion

    [PreserveSig]
    void GetDesc(
                    out OUTDUPL_DESC pDesc);

    void AcquireNextFrame(
                        uint TimeoutInMilliseconds,
                    out OUTDUPL_FRAME_INFO pFrameInfo,
        [Optional] out IDXGIResource ppDesktopResource);

    void GetFrameDirtyRects(
                        uint DirtyRectsBufferSize,
        [Out] RECT[] pDirtyRectsBuffer,
                    out uint pDirtyRectsBufferSizeRequired);

    void GetFrameMoveRects(
                        uint MoveRectsBufferSize,
        [Out] OUTDUPL_MOVE_RECT[] pMoveRectBuffer,
                    out uint pMoveRectsBufferSizeRequired);

    void GetFramePointerShape(
                        uint PointerShapeBufferSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )]
            [Out]           byte[]                                      pPointerShapeBuffer,
                    out uint pPointerShapeBufferSizeRequired,
                    out OUTDUPL_POINTER_SHAPE_INFO pPointerShapeInfo);

    void MapDesktopSurface(
                    out MAPPED_RECT pLockedRect);

    void UnMapDesktopSurface();

    void ReleaseFrame();
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGISurface2)]
public interface IDXGISurface2
    : IDXGISurface1
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    new void GetDesc(
                        out SURFACE_DESC pDesc);
    new void Map(
                        out MAPPED_RECT pLockedRect,
                            int MapFlags);
    new void Unmap();
    new void GetDC(
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                        Discard,
                        out IntPtr phdc);

    new void ReleaseDC(
        [Optional] in RECT pDirtyRect);
    #endregion

    void GetResource(
                        in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                            out object                                      ppParentResource,
                        in uint pSubresourceIndex);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIResource1)]
public interface IDXGIResource1
    : IDXGIResource
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            out object                                  ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    new void GetSharedHandle(
                        out IntPtr pSharedHandle);
    new void GetUsage(
                        out USAGE pUsage);
    new void SetEvictionPriority(
                            RESOURCE_PRIORITY EvictionPriority);
    new void GetEvictionPriority(
                        out RESOURCE_PRIORITY pEvictionPriority);
    #endregion

    void CreateSubresourceSurface(
                            uint index,
                        out IDXGISurface2 ppSurface);

    void CreateSharedHandle(
        [Optional] IntPtr pAttributes, // SECURITY_ATTRIBUTES*
                            SHARED_RESOURCE_FLAG dwAccess,
        [MarshalAs( UnmanagedType.LPWStr )]
            [Optional]          string                                  lpName,
                        out IntPtr pHandle);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIDevice2)]
public interface IDXGIDevice2
    : IDXGIDevice1
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);

    new void GetAdapter(
                    out IDXGIAdapter pAdapter);

    new void CreateSurface(
                    in SURFACE_DESC pDesc,
                        uint NumSurfaces,
                        USAGE Usage,
                    in SHARED_RESOURCE pSharedResource,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGISurface                                ppSurface);

    new void QueryResourceResidency(
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 2)]
                            object[]                                    ppResources,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 2)]
                        out RESIDENCY[]                                 pResidencyStatus,
                        uint NumResources);

    new void SetGPUThreadPriority(
                        int Priority);

    new void GetGPUThreadPriority(
                    out int pPriority);
    new void SetMaximumFrameLatency(
                        int MaxLatency);
    new void GetMaximumFrameLatency(
                    out int pMaxLatency);
    #endregion

    void OfferResources(
                        uint NumResources,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )]
                            IDXGIResource[]                             ppResources,
                        OFFER_RESOURCE_PRIORITY Priority);

    void ReclaimResources(
                        uint NumResources,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )]
                            IDXGIResource[]                             ppResources,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Bool, SizeParamIndex = 0 )]
                        out bool[]                                      pDiscarded);

    void EnqueueSetEvent(
                        IntPtr hEvent);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGISwapChain1)]
public interface IDXGISwapChain1
    : IDXGISwapChain
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    [PreserveSig]
    new int Present(
                            uint SyncInterval,
                            PRESENT Flags);
    new void GetBuffer(
                            uint Buffer,
                        in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                            out object                                  ppSurface);
    new void SetFullscreenState(
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    Fullscreen,
        [Optional] IDXGIOutput pTarget);
    new void GetFullscreenState(
        [MarshalAs( UnmanagedType.Bool )]
                            out bool                                    pFullscreen,
        [Optional] ref IDXGIOutput ppTarget);
    new void GetDesc(
                        out SWAP_CHAIN_DESC pDesc);
    new void ResizeBuffers(
                            uint BufferCount,
                            uint Width,
                            uint Height,
                            FORMAT NewFormat,
                            SWAP_CHAIN_FLAG SwapChainFlags);
    new void ResizeTarget(
                        in MODE_DESC pNewTargetParameters);
    new void GetContainingOutput(
                        out IDXGIOutput ppOutput);
    new void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    new void GetLastPresentCount(
                        out uint pLastPresentCount);
    #endregion

    void GetDesc1(
                        out SWAP_CHAIN_DESC1 pDesc);

    void GetFullscreenDesc(
                        out SWAP_CHAIN_FULLSCREEN_DESC pDesc);

    void GetHwnd(
                        out IntPtr pHwnd);

    void GetCoreWindow(
                        in Guid refiid,
                        out object ppUnk);

    [PreserveSig]
    int Present1(
                            uint SyncInterval,
                            PRESENT PresentFlags,
                        in PRESENT_PARAMETERS pPresentParameters);

    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    bool IsTemporaryMonoSupported();

    void GetRestrictToOutput(
                        out IDXGIOutput ppRestrictToOutput);

    void SetBackgroundColor(
                        in COLORVALUE pColor);

    void GetBackgroundColor(
                        out COLORVALUE pColor);

    void SetRotation(
                            MODE_ROTATION Rotation);

    void GetRotation(
                        out MODE_ROTATION pRotation);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIFactory2)]
public interface IDXGIFactory2
    : IDXGIFactory1
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);

    [PreserveSig]
    new int EnumAdapters(
                        uint Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGIAdapter                                ppAdapter);

    new void MakeWindowAssociation(
                        IntPtr WindowHandle,
                        MWA Flags);

    new void GetWindowAssociation(
                    out IntPtr pWindowHandle);

    new void CreateSwapChain(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                    in SWAP_CHAIN_DESC pDesc,
                    out IDXGISwapChain ppSwapChain);

    new void CreateSoftwareAdapter(
                        IntPtr Module,
                    out IDXGIAdapter ppAdapter);
    [PreserveSig]
    new int EnumAdapters1(
                        int Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGIAdapter1                               ppAdapter);
    [return: MarshalAs(UnmanagedType.Bool)]
    new bool IsCurrent();
    #endregion

    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    bool IsWindowedStereoEnabled();

    void CreateSwapChainForHwnd(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                        IntPtr hWnd,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] in SWAP_CHAIN_FULLSCREEN_DESC pFullscreenDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);

    void CreateSwapChainForCoreWindow(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                        object pWindow,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);

    void GetSharedResourceAdapterLuid(
                        IntPtr hResource,
                    out LUID pLuid);

    void RegisterStereoStatusWindow(
                        IntPtr WindowHandle,
                        uint wMsg,
                    out uint pdwCookie);

    void RegisterStereoStatusEvent(
                        IntPtr hEvent,
                    out uint pdwCookie);

    void UnregisterStereoStatus(
                        uint dwCookie);

    void RegisterOcclusionStatusWindow(
                        IntPtr WindowHandle,
                        uint wMsg,
                    out uint pdwCookie);

    void RegisterOcclusionStatusEvent(
                        IntPtr hEvent,
                    out uint pdwCookie);

    void UnregisterOcclusionStatus(
                        uint dwCookie);

    void CreateSwapChainForComposition(
                        IDXGIDevice pDevice,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIAdapter2)]
public interface IDXGIAdapter2
    : IDXGIAdapter1
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);

    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);

    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);

    new void GetParent(
                        in Guid Name,
                        out object ppParent);

    [PreserveSig]
    new int EnumOutputs(
                            uint Output,
                        out IDXGIOutput ppOutput);

    new void GetDesc(
                        out ADAPTER_DESC pDesc);

    new void CheckInterfaceSupport(
                        in Guid InterfaceName,
                        out long pUMDVersion);
    new void GetDesc1(
                        out ADAPTER_DESC1 pDesc);

    #endregion

    void GetDesc2(
                    out ADAPTER_DESC2 pDesc);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIOutput1)]
public interface IDXGIOutput1
    : IDXGIOutput
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);

    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);

    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);

    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDesc(
                        out OUTPUT_DESC pDesc);
    new void GetDisplayModeList(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC[]                             pDesc);
    new void FindClosestMatchingMode(
                        in MODE_DESC pModeToMatch,
                        out MODE_DESC pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);
    void WaitForVBlank();
    new void TakeOwnership(
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pDevice,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    Exclusive);
    new void ReleaseOwnership();
    new void GetGammaControlCapabilities(
                        out GAMMA_CONTROL_CAPABILITIES pGammaCaps);
    new void SetGammaControl(
                        in GAMMA_CONTROL pArray);
    new void GetGammaControl(
                        out GAMMA_CONTROL pArray);
    new void SetDisplaySurface(
                            IDXGISurface pScanoutSurface);
    new void GetDisplaySurfaceData(
                        out IDXGISurface pDestination);
    new void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    #endregion

    void GetDisplayModeList1(
                        FORMAT EnumFormat,
                        ENUM_MODES Flags,
                    ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out] MODE_DESC1[]                                pDesc);

    void FindClosestMatchingMode1(
                        in MODE_DESC1 pModeToMatch,
                        out MODE_DESC1 pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);

    void GetDisplaySurfaceData1(
                            IDXGIResource pDestination);

    void DuplicateOutput(
                            IDXGIDevice pDevice,
                        out IDXGIOutputDuplication ppOutputDuplication);
}
#endregion

#region dxgi1_3.h
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIDevice3)]
public interface IDXGIDevice3
    : IDXGIDevice2
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);

    new void GetAdapter(
                    out IDXGIAdapter pAdapter);

    new void CreateSurface(
                    in SURFACE_DESC pDesc,
                        uint NumSurfaces,
                        USAGE Usage,
                    in SHARED_RESOURCE pSharedResource,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGISurface                                ppSurface);

    new void QueryResourceResidency(
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 2)]
                            object[]                                    ppResources,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 2)]
                        out RESIDENCY[]                                 pResidencyStatus,
                        uint NumResources);

    new void SetGPUThreadPriority(
                        int Priority);

    new void GetGPUThreadPriority(
                    out int pPriority);
    new void SetMaximumFrameLatency(
                        int MaxLatency);
    new void GetMaximumFrameLatency(
                    out int pMaxLatency);
    new void OfferResources(
                        uint NumResources,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )]
                            IDXGIResource[]                             ppResources,
                        OFFER_RESOURCE_PRIORITY Priority);

    new void ReclaimResources(
                        uint NumResources,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )]
                            IDXGIResource[]                             ppResources,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Bool, SizeParamIndex = 0 )]
                        out bool[]                                      pDiscarded);

    new void EnqueueSetEvent(
                        IntPtr hEvent);
    #endregion

    void Trim();
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGISwapChain2)]
public interface IDXGISwapChain2
    : IDXGISwapChain1
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    [PreserveSig]
    new int Present(
                            uint SyncInterval,
                            PRESENT Flags);
    new void GetBuffer(
                            uint Buffer,
                        in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                            out object                                  ppSurface);
    new void SetFullscreenState(
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    Fullscreen,
        [Optional] IDXGIOutput pTarget);
    new void GetFullscreenState(
        [MarshalAs( UnmanagedType.Bool )]
                            out bool                                    pFullscreen,
        [Optional] ref IDXGIOutput ppTarget);
    new void GetDesc(
                        out SWAP_CHAIN_DESC pDesc);
    new void ResizeBuffers(
                            uint BufferCount,
                            uint Width,
                            uint Height,
                            FORMAT NewFormat,
                            SWAP_CHAIN_FLAG SwapChainFlags);
    new void ResizeTarget(
                        in MODE_DESC pNewTargetParameters);
    new void GetContainingOutput(
                        out IDXGIOutput ppOutput);
    new void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    new void GetLastPresentCount(
                        out uint pLastPresentCount);
    new void GetDesc1(
                        out SWAP_CHAIN_DESC1 pDesc);

    new void GetFullscreenDesc(
                        out SWAP_CHAIN_FULLSCREEN_DESC pDesc);

    new void GetHwnd(
                        out IntPtr pHwnd);

    new void GetCoreWindow(
                        in Guid refiid,
                        out object ppUnk);

    [PreserveSig]
    new int Present1(
                            uint SyncInterval,
                            PRESENT PresentFlags,
                        in PRESENT_PARAMETERS pPresentParameters);

    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    new bool IsTemporaryMonoSupported();

    new void GetRestrictToOutput(
                        out IDXGIOutput ppRestrictToOutput);

    new void SetBackgroundColor(
                        in COLORVALUE pColor);

    new void GetBackgroundColor(
                        out COLORVALUE pColor);

    new void SetRotation(
                            MODE_ROTATION Rotation);

    new void GetRotation(
                        out MODE_ROTATION pRotation);
    #endregion

    void SetSourceSize(
                            uint Width,
                            uint Height);

    void GetSourceSize(
                        out uint pWidth,
                        out uint pHeight);

    void SetMaximumFrameLatency(
                            uint MaxLatency);

    void GetMaximumFrameLatency(
                        out uint pMaxLatency);

    [PreserveSig]
    IntPtr GetFrameLatencyWaitableObject();
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIOutput2)]
public interface IDXGIOutput2
    : IDXGIOutput1
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);

    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);

    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);

    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDesc(
                        out OUTPUT_DESC pDesc);
    new void GetDisplayModeList(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC[]                             pDesc);
    new void FindClosestMatchingMode(
                        in MODE_DESC pModeToMatch,
                        out MODE_DESC pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);
    new void WaitForVBlank();
    new void TakeOwnership(
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pDevice,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    Exclusive);
    new void ReleaseOwnership();
    new void GetGammaControlCapabilities(
                        out GAMMA_CONTROL_CAPABILITIES pGammaCaps);
    new void SetGammaControl(
                        in GAMMA_CONTROL pArray);
    new void GetGammaControl(
                        out GAMMA_CONTROL pArray);
    new void SetDisplaySurface(
                            IDXGISurface pScanoutSurface);
    new void GetDisplaySurfaceData(
                        out IDXGISurface pDestination);
    new void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    new void GetDisplayModeList1(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC1[]                            pDesc);

    new void FindClosestMatchingMode1(
                        in MODE_DESC1 pModeToMatch,
                        out MODE_DESC1 pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);

    new void GetDisplaySurfaceData1(
                            IDXGIResource pDestination);

    new void DuplicateOutput(
                            IDXGIDevice pDevice,
                        out IDXGIOutputDuplication ppOutputDuplication);
    #endregion

    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    bool SupportsOverlays();
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIFactory3)]
public interface IDXGIFactory3
    : IDXGIFactory2
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);

    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);

    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);

    new void GetParent(
                    in Guid Name,
                    out object ppParent);

    [PreserveSig]
    new int EnumAdapters(
                        uint Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGIAdapter                                ppAdapter);

    new void MakeWindowAssociation(
                        IntPtr WindowHandle,
                        MWA Flags);

    new void GetWindowAssociation(
                    out IntPtr pWindowHandle);

    new void CreateSwapChain(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                    in SWAP_CHAIN_DESC pDesc,
                    out IDXGISwapChain ppSwapChain);

    new void CreateSoftwareAdapter(
                        IntPtr Module,
                    out IDXGIAdapter ppAdapter);
    [PreserveSig]
    new int EnumAdapters1(
                        int Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGIAdapter1                               ppAdapter);
    [return: MarshalAs(UnmanagedType.Bool)]
    new bool IsCurrent();
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    new bool IsWindowedStereoEnabled();
    new void CreateSwapChainForHwnd(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                        IntPtr hWnd,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] in SWAP_CHAIN_FULLSCREEN_DESC pFullscreenDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);
    new void CreateSwapChainForCoreWindow(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                        object pWindow,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);
    new void GetSharedResourceAdapterLuid(
                        IntPtr hResource,
                    out LUID pLuid);
    new void RegisterStereoStatusWindow(
                        IntPtr WindowHandle,
                        uint wMsg,
                    out uint pdwCookie);
    new void RegisterStereoStatusEvent(
                        IntPtr hEvent,
                    out uint pdwCookie);
    new void UnregisterStereoStatus(
                        uint dwCookie);
    new void RegisterOcclusionStatusWindow(
                        IntPtr WindowHandle,
                        uint wMsg,
                    out uint pdwCookie);
    new void RegisterOcclusionStatusEvent(
                        IntPtr hEvent,
                    out uint pdwCookie);
    new void UnregisterOcclusionStatus(
                        uint dwCookie);
    new void CreateSwapChainForComposition(
                        IDXGIDevice pDevice,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);
    #endregion

    [PreserveSig]
    [return: MarshalAs(UnmanagedType.U4)]
    uint GetCreationFlags();
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIDecodeSwapChain)]
public interface IDXGIDecodeSwapChain
{
    void PresentBuffer(
                        uint BufferToPresent,
                        uint SyncInterval,
                        PRESENT Flags);

    void SetSourceRect(
                    in RECT pRect);

    void SetTargetRect(
                    in RECT pRect);

    void SetDestSize(
                        uint Width,
                        uint Height);

    void GetSourceRect(
                    out RECT pRect);

    void GetTargetRect(
                    out RECT pRect);

    void GetDestSize(
                    out uint pWidth,
                    out uint pHeight);

    void SetColorSpace(
                        MULTIPLANE_OVERLAY_YCbCr_FLAGS ColorSpace);

    [PreserveSig]
    MULTIPLANE_OVERLAY_YCbCr_FLAGS GetColorSpace();
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIFactoryMedia)]
public interface IDXGIFactoryMedia
{
    void CreateSwapChainForCompositionSurfaceHandle(
                        IDXGIDevice3 pDevice,
        [Optional] IntPtr hSurface,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);

    void CreateDecodeSwapChainForCompositionSurfaceHandle(
                        IDXGIDevice3 pDevice,
        [Optional] IntPtr hSurface,
                    in DECODE_SWAP_CHAIN_DESC pDesc,
                        IDXGIResource pYuvDecodeBuffers,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGIDecodeSwapChain ppSwapChain);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGISwapChainMedia)]
public interface IDXGISwapChainMedia
{
    void GetFrameStatisticsMedia(
                    out FRAME_STATISTICS_MEDIA pStats);

    void SetPresentDuration(
                        uint Duration);

    void CheckPresentDurationSupport(
                        uint DesiredPresentDuration,
                    out uint pClosestSmallerPresentDuration,
                    out uint pClosestLargerPresentDuration);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIOutput2)]
public interface IDXGIOutput3
    : IDXGIOutput2
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);

    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);

    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);

    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDesc(
                        out OUTPUT_DESC pDesc);
    new void GetDisplayModeList(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC[]                             pDesc);
    new void FindClosestMatchingMode(
                        in MODE_DESC pModeToMatch,
                        out MODE_DESC pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);
    new void WaitForVBlank();
    new void TakeOwnership(
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pDevice,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    Exclusive);
    new void ReleaseOwnership();
    new void GetGammaControlCapabilities(
                        out GAMMA_CONTROL_CAPABILITIES pGammaCaps);
    new void SetGammaControl(
                        in GAMMA_CONTROL pArray);
    new void GetGammaControl(
                        out GAMMA_CONTROL pArray);
    new void SetDisplaySurface(
                            IDXGISurface pScanoutSurface);
    new void GetDisplaySurfaceData(
                        out IDXGISurface pDestination);
    new void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    new void GetDisplayModeList1(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC1[]                            pDesc);

    new void FindClosestMatchingMode1(
                        in MODE_DESC1 pModeToMatch,
                        out MODE_DESC1 pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);

    new void GetDisplaySurfaceData1(
                            IDXGIResource pDestination);

    new void DuplicateOutput(
                            IDXGIDevice pDevice,
                        out IDXGIOutputDuplication ppOutputDuplication);
    #endregion

    void CheckOverlaySupport(
                            FORMAT EnumFormat,
                            IDXGIDevice pConcernedDevice,
                        out OVERLAY_SUPPORT_FLAG pFlags);
}
#endregion

#region dxgi1_4.h
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGISwapChain3)]
public interface IDXGISwapChain3
    : IDXGISwapChain2
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
                        out object ppParent);
    new void GetDevice(
                        in Guid Name,
                        out IDXGIDevice ppDevice);
    [PreserveSig]
    new int Present(
                            uint SyncInterval,
                            PRESENT Flags);
    new void GetBuffer(
                            uint Buffer,
                        in Guid riid,
        [MarshalAs( UnmanagedType.Interface )]
                            out object                                  ppSurface);
    new void SetFullscreenState(
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    Fullscreen,
        [Optional] IDXGIOutput pTarget);
    new void GetFullscreenState(
        [MarshalAs( UnmanagedType.Bool )]
                            out bool                                    pFullscreen,
        [Optional] ref IDXGIOutput ppTarget);
    new void GetDesc(
                        out SWAP_CHAIN_DESC pDesc);
    new void ResizeBuffers(
                            uint BufferCount,
                            uint Width,
                            uint Height,
                            FORMAT NewFormat,
                            SWAP_CHAIN_FLAG SwapChainFlags);
    new void ResizeTarget(
                        in MODE_DESC pNewTargetParameters);
    new void GetContainingOutput(
                        out IDXGIOutput ppOutput);
    new void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    new void GetLastPresentCount(
                        out uint pLastPresentCount);
    new void GetDesc1(
                        out SWAP_CHAIN_DESC1 pDesc);
    new void GetFullscreenDesc(
                        out SWAP_CHAIN_FULLSCREEN_DESC pDesc);
    new void GetHwnd(
                        out IntPtr pHwnd);
    new void GetCoreWindow(
                        in Guid refiid,
        [MarshalAs( UnmanagedType.Interface )]
                            out object                                  ppUnk);
    [PreserveSig]
    new int Present1(
                            uint SyncInterval,
                            PRESENT PresentFlags,
                        in PRESENT_PARAMETERS pPresentParameters);
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    new bool IsTemporaryMonoSupported();
    new void GetRestrictToOutput(
                        out IDXGIOutput ppRestrictToOutput);
    new void SetBackgroundColor(
                        in COLORVALUE pColor);
    new void GetBackgroundColor(
                        out COLORVALUE pColor);
    new void SetRotation(
                            MODE_ROTATION Rotation);
    new void GetRotation(
                        out MODE_ROTATION pRotation);
    new void SetSourceSize(
                            uint Width,
                            uint Height);
    new void GetSourceSize(
                        out uint pWidth,
                        out uint pHeight);
    new void SetMaximumFrameLatency(
                            uint MaxLatency);
    new void GetMaximumFrameLatency(
                        out uint pMaxLatency);
    [PreserveSig]
    new IntPtr GetFrameLatencyWaitableObject();
    #endregion

    [PreserveSig]
    [return: MarshalAs(UnmanagedType.U4)]
    uint GetCurrentBackBufferIndex();

    void CheckColorSpaceSupport(
                            COLOR_SPACE_TYPE ColorSpace,
                        out SWAP_CHAIN_COLOR_SPACE_SUPPORT_FLAG pColorSpaceSupport);

    void SetColorSpace1(
                            COLOR_SPACE_TYPE ColorSpace);

    //void ResizeBuffers1(
    //                        uint BufferCount,
    //                        uint Width,
    //                        uint Height,
    //                        FORMAT Format,
    //                        SWAP_CHAIN_FLAG SwapChainFlags,
    //    [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4, SizeParamIndex = 0 )]
    //                            uint[]                                  pCreationNodeMask,
    //    [MarshalAs( UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof( Utility.CustomArrayMarshaller<Direct3DA.ID3D12CommandQueue> ) )]
    //                            object[]                                ppPresentQueue);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIOutput4)]
public interface IDXGIOutput4
    : IDXGIOutput3
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);

    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);

    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);

    new void GetParent(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            out object                                  ppParent);
    new void GetDesc(
                        out OUTPUT_DESC pDesc);
    new void GetDisplayModeList(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC[]                             pDesc);
    new void FindClosestMatchingMode(
                        in MODE_DESC pModeToMatch,
                        out MODE_DESC pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);
    new void WaitForVBlank();
    new void TakeOwnership(
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pDevice,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    Exclusive);
    new void ReleaseOwnership();
    new void GetGammaControlCapabilities(
                        out GAMMA_CONTROL_CAPABILITIES pGammaCaps);
    new void SetGammaControl(
                        in GAMMA_CONTROL pArray);
    new void GetGammaControl(
                        out GAMMA_CONTROL pArray);
    new void SetDisplaySurface(
                            IDXGISurface pScanoutSurface);
    new void GetDisplaySurfaceData(
                        out IDXGISurface pDestination);
    new void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    new void GetDisplayModeList1(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC1[]                            pDesc);
    new void FindClosestMatchingMode1(
                        in MODE_DESC1 pModeToMatch,
                        out MODE_DESC1 pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);
    new void GetDisplaySurfaceData1(
                            IDXGIResource pDestination);
    new void DuplicateOutput(
                            IDXGIDevice pDevice,
                        out IDXGIOutputDuplication ppOutputDuplication);
    new void CheckOverlaySupport(
                            FORMAT EnumFormat,
                            IDXGIDevice pConcernedDevice,
                        out OVERLAY_SUPPORT_FLAG pFlags);
    #endregion

    void CheckOverlayColorSpaceSupport(
                            FORMAT Format,
                            COLOR_SPACE_TYPE ColorSpace,
                            IDXGIDevice pConcernedDevice,
                        out OVERLAY_COLOR_SPACE_SUPPORT_FLAG pFlags);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIFactory4)]
public interface IDXGIFactory4
    : IDXGIFactory3
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);
    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);
    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);
    new void GetParent(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                        out object                                      ppParent);
    [PreserveSig]
    new int EnumAdapters(
                        uint Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGIAdapter                                ppAdapter);
    new void MakeWindowAssociation(
                        IntPtr WindowHandle,
                        MWA Flags);
    new void GetWindowAssociation(
                    out IntPtr pWindowHandle);
    new void CreateSwapChain(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                    in SWAP_CHAIN_DESC pDesc,
                    out IDXGISwapChain ppSwapChain);
    new void CreateSoftwareAdapter(
                        IntPtr Module,
                    out IDXGIAdapter ppAdapter);
    [PreserveSig]
    new int EnumAdapters1(
                        int Adapter,
        [MarshalAs( UnmanagedType.Interface )]
                        out IDXGIAdapter1                               ppAdapter);
    [return: MarshalAs(UnmanagedType.Bool)]
    new bool IsCurrent();
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    new bool IsWindowedStereoEnabled();
    new void CreateSwapChainForHwnd(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                       pDevice,
                        IntPtr hWnd,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] in SWAP_CHAIN_FULLSCREEN_DESC pFullscreenDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);
    new void CreateSwapChainForCoreWindow(
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pDevice,
                        object pWindow,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);
    new void GetSharedResourceAdapterLuid(
                        IntPtr hResource,
                    out LUID pLuid);
    new void RegisterStereoStatusWindow(
                        IntPtr WindowHandle,
                        uint wMsg,
                    out uint pdwCookie);
    new void RegisterStereoStatusEvent(
                        IntPtr hEvent,
                    out uint pdwCookie);
    new void UnregisterStereoStatus(
                        uint dwCookie);
    new void RegisterOcclusionStatusWindow(
                        IntPtr WindowHandle,
                        uint wMsg,
                    out uint pdwCookie);
    new void RegisterOcclusionStatusEvent(
                        IntPtr hEvent,
                    out uint pdwCookie);
    new void UnregisterOcclusionStatus(
                        uint dwCookie);
    new void CreateSwapChainForComposition(
                        IDXGIDevice pDevice,
                    in SWAP_CHAIN_DESC1 pDesc,
        [Optional] IDXGIOutput pRestrictToOutput,
                    out IDXGISwapChain1 ppSwapChain);
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.U4)]
    new uint GetCreationFlags();
    #endregion

    void EnumAdapterByLuid(
                        in LUID AdapterLuid,
                        in Guid riid,
                        out IDXGIAdapter ppvAdapter);

    void EnumWarpAdapter(
                        in Guid riid,
                        out IDXGIAdapter ppvAdapter);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIAdapter3)]
public interface IDXGIAdapter3
    : IDXGIAdapter2
{
    #region basic interface
    new void SetPrivateData(
                    in Guid Name,
                        uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                            byte[]                                      pData);
    new void SetPrivateDataInterface(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            object                                      pUnknown);
    new void GetPrivateData(
                    in Guid Name,
                    ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out] byte[]                                      pData);
    new void GetParent(
                    in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                        out object                                      ppParent);
    [PreserveSig]
    new int EnumOutputs(
                        uint Output,
                    out IDXGIOutput ppOutput);
    new void GetDesc(
                    out ADAPTER_DESC pDesc);
    new void CheckInterfaceSupport(
                    in Guid InterfaceName,
                    out long pUMDVersion);
    new void GetDesc1(
                    out ADAPTER_DESC1 pDesc);
    new void GetDesc2(
                    out ADAPTER_DESC2 pDesc);
    #endregion

    void RegisterHardwareContentProtectionTeardownStatusEvent(
                        IntPtr hEvent,
                    out uint pdwCookie);

    void UnregisterHardwareContentProtectionTeardownStatus(
                        uint dwCookie);

    void QueryVideoMemoryInfo(
                        uint NodeIndex,
                        MEMORY_SEGMENT_GROUP MemorySegmentGroup,
                    out QUERY_VIDEO_MEMORY_INFO pVideoMemoryInfo);

    void SetVideoMemoryReservation(
                        uint NodeIndex,
                        MEMORY_SEGMENT_GROUP MemorySegmentGroup,
                        ulong Reservation);

    void RegisterVideoMemoryBudgetChangeNotificationEvent(
                        IntPtr hEvent,
                    out uint pdwCookie);

    void UnregisterVideoMemoryBudgetChangeNotification(
                        uint dwCookie);
}
#endregion

#region dxgi1_5.h
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Guids.IDXGIOutput5)]
public interface IDXGIOutput5
    : IDXGIOutput4
{
    #region basic interface
    new void SetPrivateData(
                        in Guid Name,
                            uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
                                byte[]                                  pData);
    new void SetPrivateDataInterface(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pUnknown);
    new void GetPrivateData(
                        in Guid Name,
                        ref uint DataSize,
        [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )]
            [Optional, Out]     byte[]                                  pData);
    new void GetParent(
                        in Guid Name,
        [MarshalAs( UnmanagedType.Interface )]
                            out object                                  ppParent);
    new void GetDesc(
                        out OUTPUT_DESC pDesc);
    new void GetDisplayModeList(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC[]                             pDesc);
    new void FindClosestMatchingMode(
                        in MODE_DESC pModeToMatch,
                        out MODE_DESC pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);
    new void WaitForVBlank();
    new void TakeOwnership(
        [MarshalAs( UnmanagedType.Interface )]
                                object                                  pDevice,
        [MarshalAs( UnmanagedType.Bool )]
                                bool                                    Exclusive);
    new void ReleaseOwnership();
    new void GetGammaControlCapabilities(
                        out GAMMA_CONTROL_CAPABILITIES pGammaCaps);
    new void SetGammaControl(
                        in GAMMA_CONTROL pArray);
    new void GetGammaControl(
                        out GAMMA_CONTROL pArray);
    new void SetDisplaySurface(
                            IDXGISurface pScanoutSurface);
    new void GetDisplaySurfaceData(
                        out IDXGISurface pDestination);
    new void GetFrameStatistics(
                        out FRAME_STATISTICS pStats);
    new void GetDisplayModeList1(
                            FORMAT EnumFormat,
                            ENUM_MODES Flags,
                        ref uint pNumModes,
        [MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2 )]
            [Optional, Out]     MODE_DESC1[]                            pDesc);
    new void FindClosestMatchingMode1(
                        in MODE_DESC1 pModeToMatch,
                        out MODE_DESC1 pClosestMatch,
        [MarshalAs( UnmanagedType.Interface )]
            [Optional]          object                                  pConcernedDevice);
    new void GetDisplaySurfaceData1(
                            IDXGIResource pDestination);
    new void DuplicateOutput(
                            IDXGIDevice pDevice,
                        out IDXGIOutputDuplication ppOutputDuplication);
    new void CheckOverlaySupport(
                            FORMAT EnumFormat,
                            IDXGIDevice pConcernedDevice,
                        out OVERLAY_SUPPORT_FLAG pFlags);
    new void CheckOverlayColorSpaceSupport(
                            FORMAT Format,
                            COLOR_SPACE_TYPE ColorSpace,
                            IDXGIDevice pConcernedDevice,
                        out OVERLAY_COLOR_SPACE_SUPPORT_FLAG pFlags);
    #endregion

    void DuplicateOutput1(
                            IDXGIDevice pDevice,
                            uint Flags,
                            uint SupportedFormatsCount,
                            FORMAT[] pSupportedFormats,
                        out IDXGIOutputDuplication ppOutputDuplication);
}
#endregion

public class SafeNativeMethods
{
    public static bool Succeeded(int hr)
    {
        return hr >= 0;
    }
    public static bool Failed(int hr)
    {
        if (hr < 0)
        {
            //Debug.WriteLine(FromHresult<SCODE>(hr), "[OLE2]");
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool Ok(int hr)
    {
        return hr == 0;
    }
    public static bool Is(int hr, int code)
    {
        return hr == code;
    }
    public static void Throw(int hr)
    {
        Marshal.ThrowExceptionForHR(hr);
    }
    public static void SafeRelease<InterfaceType>(ref InterfaceType @interface)
    {
        if (@interface != null)
        {
            if (Marshal.IsComObject(@interface))
            {
                int count = Marshal.ReleaseComObject(@interface);
                Debug.Assert(count >= 0);
            }
            @interface = default;
        }
    }
    private delegate int AddRefFunc(IntPtr unknown);

    public static int AddRef(object @interface)
    {
        var unkn = Marshal.GetIUnknownForObject(@interface);
        var func = GetDelegates<AddRefFunc>(unkn, 1);
        return func(unkn);
    }
    public static T GetDelegates<T>(IntPtr unknown, int index)
        where T : class
    {
        var vtbl = Marshal.ReadIntPtr(unknown);
        var func = Marshal.ReadIntPtr(vtbl, IntPtr.Size * index);
        return Marshal.GetDelegateForFunctionPointer<T>(func);
    }
    //public static T CoCreateInstance<T>(in Guid riid)
    //{
    //    CoCreateInstance(
    //            riid,
    //            null,
    //            NativeConstants.CLSCTX_INPROC,
    //            typeof(T).GUID,
    //        out object result);

    //    return (T)result;
    //}
    //
    //public static string FromHresult<ScodeT>(int hr)
    //    where ScodeT : SCODE
    //{
    //    for (var type = typeof(ScodeT); type != null; type = type.BaseType)
    //    {
    //        foreach (var desc in type.GetFields())
    //        {
    //            if (desc.FieldType == typeof(int))
    //            {
    //                if ((int)desc.GetValue(desc) == hr)
    //                {
    //                    return desc.Name;
    //                }
    //            }
    //        }
    //    }
    //    return hr.ToString("X8");
    //}
}

public class DXWrapper : DXNativeMethods
{
    private IDXGIFactory1 @interface;
    //private readonly IList<Adapter> adapters = new List<Adapter>(1);

    public IEnumerable<MODE_DESC1> GetModes(string displayName, uint width = 0, uint height = 0)
    {
        int hr = CreateDXGIFactory1(
                            //#if DEBUG
                            //                            CREATE_FACTORY.DEBUG,
                            //#else
                            //                    CREATE_FACTORY.NONE,
                            //#endif
                            typeof(IDXGIFactory1).GUID,
                        out var result);

        if (Failed(hr))
        {
            Throw(hr);
        }
        else
        {
            @interface = result as IDXGIFactory1;
        }
        if (@interface == null)
        {
            throw new NotImplementedException("IDXGIFactory2");
        }
        else
        {
            SetPrivateString(@interface, "Factory2");
        }

        return GetModesFromAdapter(displayName, width, height);
    }

    private IDXGIOutput1 GetOutputForDisplay(string displayName)
    {
        Debug.Assert(@interface != null);

        for (var i = 0; ; i++)
        {
            if (!Ok(@interface.EnumAdapters1(i, out var adapter)))
            {
                break;
            }
            if (adapter == null)
            {
                continue;
            }

            //ADAPTER_DESC1 aDAPTER_DESC1 = new ADAPTER_DESC1();
            //adapter.GetDesc1(out aDAPTER_DESC1);

            for (uint o = 0; ; o++)
            {
                if (!Ok(adapter.EnumOutputs(o, out var output)))
                {
                    break;
                }

                var output1 = output as IDXGIOutput1;

                if (output1 == null)
                {
                    continue;
                }

                var outputDesc = new OUTPUT_DESC();
                output1.GetDesc(out outputDesc);

                if (outputDesc.DeviceName == displayName)
                {
                    return output1;
                }
            }

        }

        return null;
    }


    private IEnumerable<MODE_DESC1> GetModesFromAdapter(string displayName, uint width = 0, uint height = 0)
    {
        var output = GetOutputForDisplay(displayName);

        if (output == null)
        {
            return null;
        }

        uint numModes = 0;
        output.GetDisplayModeList1(FORMAT.R16G16B16A16_FLOAT, ENUM_MODES.SCALING, ref numModes, null);

        var modes = new MODE_DESC1[numModes];
        output.GetDisplayModeList1(FORMAT.R16G16B16A16_FLOAT, ENUM_MODES.SCALING, ref numModes, modes);

        var filteredModes = modes.AsEnumerable();

        if (width > 0)
        {
            filteredModes = filteredModes.Where(m => m.Width == width);
        }

        if (height > 0)
        {
            filteredModes = filteredModes.Where(m => m.Height == height);
        }

        return filteredModes;
    }

    #region d3d11/12, dxgi debug ------------------------------------------
    internal static readonly Guid wkpdid = new(Guids.DEBUG_DXGI);
    internal static readonly System.Text.Encoding encoding = new System.Text.UnicodeEncoding();
    [Conditional("DEBUG")]
    public static void SetPrivateString(IDXGIObject child, string text)
    {
        Debug.Assert(child != null);
        Debug.Assert(text != null);
        var bytes = encoding.GetBytes(text);
        child.SetPrivateData(wkpdid, unchecked((uint)bytes.Length), bytes);
    }
    #endregion
}

