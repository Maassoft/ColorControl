using System;

#pragma warning disable CS0649
#pragma warning disable CS0169

namespace LittleCms
{
    public enum TagSignature
    {
        AToB0Tag = 0x41324230,  // 'A2B0'
        AToB1Tag = 0x41324231,  // 'A2B1'
        AToB2Tag = 0x41324232,  // 'A2B2'
        BlueColorantTag = 0x6258595A,  // 'bXYZ'
        BlueMatrixColumnTag = 0x6258595A,  // 'bXYZ'
        BlueTRCTag = 0x62545243,  // 'bTRC'
        BToA0Tag = 0x42324130,  // 'B2A0'
        BToA1Tag = 0x42324131,  // 'B2A1'
        BToA2Tag = 0x42324132,  // 'B2A2'
        CalibrationDateTimeTag = 0x63616C74,  // 'calt'
        CharTargetTag = 0x74617267,  // 'targ'
        ChromaticAdaptationTag = 0x63686164,  // 'chad'
        ChromaticityTag = 0x6368726D,  // 'chrm'
        ColorantOrderTag = 0x636C726F,  // 'clro'
        ColorantTableTag = 0x636C7274,  // 'clrt'
        ColorantTableOutTag = 0x636C6F74,  // 'clot'
        ColorimetricIntentImageStateTag = 0x63696973,  // 'ciis'
        CopyrightTag = 0x63707274,  // 'cprt'
        CrdInfoTag = 0x63726469,  // 'crdi'
        DataTag = 0x64617461,  // 'data'
        DateTimeTag = 0x6474696D,  // 'dtim'
        DeviceMfgDescTag = 0x646D6E64,  // 'dmnd'
        DeviceModelDescTag = 0x646D6464,  // 'dmdd'
        DeviceSettingsTag = 0x64657673,  // 'devs'
        DToB0Tag = 0x44324230,  // 'D2B0'
        DToB1Tag = 0x44324231,  // 'D2B1'
        DToB2Tag = 0x44324232,  // 'D2B2'
        DToB3Tag = 0x44324233,  // 'D2B3'
        BToD0Tag = 0x42324430,  // 'B2D0'
        BToD1Tag = 0x42324431,  // 'B2D1'
        BToD2Tag = 0x42324432,  // 'B2D2'
        BToD3Tag = 0x42324433,  // 'B2D3'
        GamutTag = 0x67616D74,  // 'gamt'
        GrayTRCTag = 0x6b545243,  // 'kTRC'
        GreenColorantTag = 0x6758595A,  // 'gXYZ'
        GreenMatrixColumnTag = 0x6758595A,  // 'gXYZ'
        GreenTRCTag = 0x67545243,  // 'gTRC'
        LuminanceTag = 0x6C756d69,  // 'lumi'
        MeasurementTag = 0x6D656173,  // 'meas'
        MediaBlackPointTag = 0x626B7074,  // 'bkpt'
        MediaWhitePointTag = 0x77747074,  // 'wtpt'
        MHC2 = 0x4D484332,  // 'MHC2'
        NamedColorTag = 0x6E636f6C,  // 'ncol' // Deprecated by the ICC
        NamedColor2Tag = 0x6E636C32,  // 'ncl2'
        OutputResponseTag = 0x72657370,  // 'resp'
        PerceptualRenderingIntentGamutTag = 0x72696730,  // 'rig0'
        Preview0Tag = 0x70726530,  // 'pre0'
        Preview1Tag = 0x70726531,  // 'pre1'
        Preview2Tag = 0x70726532,  // 'pre2'
        ProfileDescriptionTag = 0x64657363,  // 'desc'
        ProfileDescriptionMLTag = 0x6473636d,  // 'dscm'
        ProfileSequenceDescTag = 0x70736571,  // 'pseq'
        ProfileSequenceIdTag = 0x70736964,  // 'psid'
        Ps2CRD0Tag = 0x70736430,  // 'psd0'
        Ps2CRD1Tag = 0x70736431,  // 'psd1'
        Ps2CRD2Tag = 0x70736432,  // 'psd2'
        Ps2CRD3Tag = 0x70736433,  // 'psd3'
        Ps2CSATag = 0x70733273,  // 'ps2s'
        Ps2RenderingIntentTag = 0x70733269,  // 'ps2i'
        RedColorantTag = 0x7258595A,  // 'rXYZ'
        RedMatrixColumnTag = 0x7258595A,  // 'rXYZ'
        RedTRCTag = 0x72545243,  // 'rTRC'
        SaturationRenderingIntentGamutTag = 0x72696732,  // 'rig2'
        ScreeningDescTag = 0x73637264,  // 'scrd'
        ScreeningTag = 0x7363726E,  // 'scrn'
        TechnologyTag = 0x74656368,  // 'tech'
        UcrBgTag = 0x62666420,  // 'bfd '
        ViewingCondDescTag = 0x76756564,  // 'vued'
        ViewingConditionsTag = 0x76696577,  // 'view'
        VcgtTag = 0x76636774,  // 'vcgt'
        MetaTag = 0x6D657461,  // 'meta'
        ArgyllArtsTag = 0x61727473   // 'arts'
    }

    /// <summary>
    /// ICC Color spaces
    /// </summary>
    public enum ColorSpaceSignature
    {
        XYZ = 0x58595A20,  // 'XYZ '
        Lab = 0x4C616220,  // 'Lab '
        Luv = 0x4C757620,  // 'Luv '
        YCbCr = 0x59436272,  // 'YCbr'
        Yxy = 0x59787920,  // 'Yxy '
        Rgb = 0x52474220,  // 'RGB '
        Gray = 0x47524159,  // 'GRAY'
        Hsv = 0x48535620,  // 'HSV '
        Hls = 0x484C5320,  // 'HLS '
        Cmyk = 0x434D594B,  // 'CMYK'
        Cmy = 0x434D5920,  // 'CMY '
        MCH1 = 0x4D434831,  // 'MCH1'
        MCH2 = 0x4D434832,  // 'MCH2'
        MCH3 = 0x4D434833,  // 'MCH3'
        MCH4 = 0x4D434834,  // 'MCH4'
        MCH5 = 0x4D434835,  // 'MCH5'
        MCH6 = 0x4D434836,  // 'MCH6'
        MCH7 = 0x4D434837,  // 'MCH7'
        MCH8 = 0x4D434838,  // 'MCH8'
        MCH9 = 0x4D434839,  // 'MCH9'
        MCHA = 0x4D434841,  // 'MCHA'
        MCHB = 0x4D434842,  // 'MCHB'
        MCHC = 0x4D434843,  // 'MCHC'
        MCHD = 0x4D434844,  // 'MCHD'
        MCHE = 0x4D434845,  // 'MCHE'
        MCHF = 0x4D434846,  // 'MCHF'
        Named = 0x6e6d636c,  // 'nmcl'
        MultiColor1 = 0x31434C52,  // '1CLR'
        MultiColor2 = 0x32434C52,  // '2CLR'
        MultiColor3 = 0x33434C52,  // '3CLR'
        MultiColor4 = 0x34434C52,  // '4CLR'
        MultiColor5 = 0x35434C52,  // '5CLR'
        MultiColor6 = 0x36434C52,  // '6CLR'
        MultiColor7 = 0x37434C52,  // '7CLR'
        MultiColor8 = 0x38434C52,  // '8CLR'
        MultiColor9 = 0x39434C52,  // '9CLR'
        MultiColor10 = 0x41434C52,  // 'ACLR'
        MultiColor11 = 0x42434C52,  // 'BCLR'
        MultiColor12 = 0x43434C52,  // 'CCLR'
        MultiColor13 = 0x44434C52,  // 'DCLR'
        MultiColor14 = 0x45434C52,  // 'ECLR'
        MultiColor15 = 0x46434C52,  // 'FCLR'
        LuvK = 0x4C75764B   // 'LuvK'
    }

    /// <summary>
    /// ICC Profile Class
    /// </summary>
    public enum ProfileClassSignature
    {
        Input = 0x73636E72,  // 'scnr'
        Display = 0x6D6E7472,  // 'mntr'
        Output = 0x70727472,  // 'prtr'
        Link = 0x6C696E6B,  // 'link'
        Abstract = 0x61627374,  // 'abst'
        ColorSpace = 0x73706163,  // 'spac'
        NamedColor = 0x6e6d636c   // 'nmcl'

    }
    public struct CIEXYZ
    {
        public double X;
        public double Y;
        public double Z;

        public static CIEXYZ operator +(in CIEXYZ a, in CIEXYZ b)
        {
            return new()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
        }

        public static CIEXYZ operator -(in CIEXYZ a, in CIEXYZ b)
        {
            return new()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
        }

        public static CIEXYZ operator -(in CIEXYZ a)
        {
            return new()
            {
                X = -a.X,
                Y = -a.Y,
                Z = -a.Z
            };
        }
    }

    public struct CIExyY
    {
        public double x;
        public double y;
        public double Y;
    }
    public enum InfoType
    {
        Description = 0,
        Manufacturer = 1,
        Model = 2,
        Copyright = 3
    }

    public enum ProfileUsedDirection
    {
        AsInput = 0,
        AsOutput = 1,
        AsProof = 2,
    }

    public struct CIEXYZTRIPLE
    {
        public CIEXYZ Red;
        public CIEXYZ Green;
        public CIEXYZ Blue;
    }

    public struct CIExyYTRIPLE
    {
        public CIExyY Red;
        public CIExyY Green;
        public CIExyY Blue;
    }

    public struct ICCData
    {
        public uint Flag;
        public Memory<byte> Data;
    }

    internal struct _ICCDataHeader
    {
        public uint len;
        public uint flag;
    }

    internal unsafe struct _ICCData
    {
        public uint len;
        public uint flag;
        public fixed byte data[1];

        internal static unsafe ICCData ToManaged(_ICCData* ptr)
        {
            var result = new ICCData();
            result.Flag = ptr->flag;
            result.Data = new ReadOnlySpan<byte>(ptr->data, (int)ptr->len).ToArray();
            return result;
        }
    }

    public struct CIELab
    {
        public double L;
        public double a;
        public double b;
    }

    public struct CIELCh
    {
        public double L;
        public double C;
        public double h;
    }

    public struct JCh
    {
        public double J;
        public double C;
        public double h;
    }

    public enum IlluminantType
    {
        UNKNOWN = 0x0000000,
        D50 = 0x0000001,
        D65 = 0x0000002,
        D93 = 0x0000003,
        F2 = 0x0000004,
        D55 = 0x0000005,
        A = 0x0000006,
        E = 0x0000007,
        F8 = 0x0000008,
    }

    public struct ICCMeasurementConditions
    {
        public uint Observer;    // 0 = unknown, 1=CIE 1931, 2=CIE 1964
        public CIEXYZ Backing;     // Value of backing
        public uint Geometry;    // 0=unknown, 1=45/0, 0/45 2=0d, d/0
        public double Flare;       // 0..1.0
        public IlluminantType IlluminantType;

    }

    public struct ICCViewingConditions
    {
        public CIEXYZ IlluminantXYZ;   // Not the same struct as CAM02,
        public CIEXYZ SurroundXYZ;     // This is for storing the tag
        public IlluminantType IlluminantType;  // viewing condition
    }

    public enum HeaderFlags
    {
        EmbeddedProfile = 1,
        UseWithEmbeddedDataOnly = 2
    }

    internal struct CmsStructTm
    {
        public int tm_sec;   // seconds after the minute - [0, 60] including leap second
        public int tm_min;   // minutes after the hour - [0, 59]
        public int tm_hour;  // hours since midnight - [0, 23]
        public int tm_mday;  // day of the month - [1, 31]
        public int tm_mon;   // months since January - [0, 11]
        public int tm_year;  // years since 1900
        public int tm_wday;  // days since Sunday - [0, 6]
        public int tm_yday;  // days since January 1 - [0, 365]
        public int tm_isdst; // daylight savings time flag

        public DateTime ToDateTime() => new DateTime(tm_year, tm_mon + 1, tm_mday, tm_hour, tm_min, tm_sec);

        public static CmsStructTm FromDateTime(DateTime d)
        {
            return new CmsStructTm
            {
                tm_year = d.Year,
                tm_mon = d.Month - 1,
                tm_mday = d.Day,
                tm_hour = d.Hour,
                tm_min = d.Minute,
                tm_sec = d.Second,
                tm_wday = (int)d.DayOfWeek,
                tm_yday = d.DayOfYear,
                tm_isdst = d.IsDaylightSavingTime() ? 1 : 0
            };
        }
    }

    public enum SpotShape
    {
        Unknown = 0,
        PrinterDefault = 1,
        Round = 2,
        Diamond = 3,
        Ellipse = 4,
        Line = 5,
        Square = 6,
        Cross = 7,
    }

    public struct cmsScreeningChannel
    {
        public double Frequency;
        public double ScreenAngle;
        public SpotShape SpotShape;
    }


    /// <summary>
    /// Where to place/locate the stages in the pipeline chain
    /// </summary>
    public enum StageLoc
    {
        AtBegin,
        AtEnd,
    }

    /// <summary>
    /// Multi process elements types
    /// </summary>
    public enum StageSignature
    {
        CurveSetElemType = 0x63767374,  //'cvst'
        MatrixElemType = 0x6D617466,  //'matf'
        CLutElemType = 0x636C7574,  //'clut'

        BAcsElemType = 0x62414353,  // 'bACS'
        EAcsElemType = 0x65414353,  // 'eACS'

        // Custom from here, not in the ICC Spec
        XYZ2LabElemType = 0x6C327820,  // 'l2x '
        Lab2XYZElemType = 0x78326C20,  // 'x2l '
        NamedColorElemType = 0x6E636C20,  // 'ncl '
        LabV2toV4 = 0x32203420,  // '2 4 '
        LabV4toV2 = 0x34203220,  // '4 2 '

        // Identities
        IdentityElemType = 0x69646E20,  // 'idn '

        // Float to floatPCS
        Lab2FloatPCS = 0x64326C20,  // 'd2l '
        FloatPCS2Lab = 0x6C326420,  // 'l2d '
        XYZ2FloatPCS = 0x64327820,  // 'd2x '
        FloatPCS2XYZ = 0x78326420,  // 'x2d '  
        ClipNegativesElemType = 0x636c7020   // 'clp '
    }

    public struct cmsViewingConditions
    {
        public const int AVG_SURROUND = 1;
        public const int DIM_SURROUND = 2;
        public const int DARK_SURROUND = 3;
        public const int CUTSHEET_SURROUND = 4;

        public const double D_CALCULATE = (-1);

        public CIEXYZ whitePoint;
        public double Yb;
        public double La;
        public uint surround;
        public double D_value;
    }

    public enum CmsError
    {
        UNDEFINED = 0,
        FILE = 1,
        RANGE = 2,
        INTERNAL = 3,
        NULL = 4,
        READ = 5,
        SEEK = 6,
        WRITE = 7,
        UNKNOWN_EXTENSION = 8,
        COLORSPACE_CHECK = 9,
        ALREADY_DEFINED = 10,
        BAD_SIGNATURE = 11,
        CORRUPTION_DETECTED = 12,
        NOT_SUITABLE = 13,
    }

    public enum RenderingIntent
    {
        // ICC Intents
        PERCEPTUAL = 0,
        RELATIVE_COLORIMETRIC = 1,
        SATURATION = 2,
        ABSOLUTE_COLORIMETRIC = 3,

        CPST_STANDARD_DISPLAY_COLOR_MODE = 7,
        CPST_EXTENDED_DISPLAY_COLOR_MODE = 8,

        // Non-ICC intents
        PRESERVE_K_ONLY_PERCEPTUAL = 10,
        PRESERVE_K_ONLY_RELATIVE_COLORIMETRIC = 11,
        PRESERVE_K_ONLY_SATURATION = 12,
        PRESERVE_K_PLANE_PERCEPTUAL = 13,
        PRESERVE_K_PLANE_RELATIVE_COLORIMETRIC = 14,
        PRESERVE_K_PLANE_SATURATION = 15,
    }

    public enum TransformFlags
    {
        NOCACHE = 0x0040,   // Inhibit 1-pixel cache
        NOOPTIMIZE = 0x0100,   // Inhibit optimizations
        NULLTRANSFORM = 0x0200,   // Don't transform anyway

        // Proofing flags
        GAMUTCHECK = 0x1000,   // Out of Gamut alarm
        SOFTPROOFING = 0x4000,   // Do softproofing

        // Misc
        BLACKPOINTCOMPENSATION = 0x2000,
        NOWHITEONWHITEFIXUP = 0x0004,   // Don't fix scum dot
        HIGHRESPRECALC = 0x0400,   // Use more memory to give better accuracy
        LOWRESPRECALC = 0x0800,   // Use less memory to minimize resources

        // For devicelink creation
        DEVICELINK_8BITS = 0x0008,   // Create 8 bits devicelinks
        GUESSDEVICECLASS = 0x0020,   // Guess device class (for transform2devicelink)
        KEEP_SEQUENCE = 0x0080,   // Keep profile sequence for devicelink creation

        // Specific to a particular optimizations
        FORCE_CLUT = 0x0002,   // Force CLUT optimization
        CLUT_POST_LINEARIZATION = 0x0001,   // create postlinearization tables if possible
        CLUT_PRE_LINEARIZATION = 0x0010,   // create prelinearization tables if possible

        // Specific to unbounded mode
        NONEGATIVES = 0x8000,    // Prevent negative numbers in floating point transforms

        // Copy alpha channels when transforming      
        COPY_ALPHA = 0x04000000,

        // Fine-tune control over number of gridpoints
        // #define cmsFLAGS_GRIDPOINTS(n)           (((n) & 0xFF) << 16)

        // CRD special
        NODEFAULTRESOURCEDEF = 0x01000000
    }

    public enum PSResourceType
    {
        CSA,
        CRD
    }

    public enum PixelType
    {
        /// <summary>
        /// Don't check colorspace
        /// </summary>
        ANY = 0,

        // 1 & 2 are reserved

        GRAY = 3,
        RGB = 4,
        CMY = 5,
        CMYK = 6,
        YCbCr = 7,

        /// <summary>
        /// Lu'v'
        /// </summary>
        YUV = 8,

        XYZ = 9,
        Lab = 10,

        /// <summary>
        /// Lu'v'K
        /// </summary>
        YUVK = 11,

        HSV = 12,
        HLS = 13,
        Yxy = 14,
        MCH1 = 15,
        MCH2 = 16,
        MCH3 = 17,
        MCH4 = 18,
        MCH5 = 19,
        MCH6 = 20,
        MCH7 = 21,
        MCH8 = 22,
        MCH9 = 23,
        MCH10 = 24,
        MCH11 = 25,
        MCH12 = 26,
        MCH13 = 27,
        MCH14 = 28,
        MCH15 = 29,
        LabV2 = 30,
    }
}


