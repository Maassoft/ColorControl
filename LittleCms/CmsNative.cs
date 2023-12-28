using LittleCms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0649
#pragma warning disable CS0169

namespace LittleCms
{
    internal unsafe partial class CmsNative
    {
        private const int LCMS_VERSION = 2131;
        private const string LIB = "lcms2";

        public static readonly Encoding WcharEncoding = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Encoding.Unicode : Encoding.UTF32;

        // Get LittleCMS version (for shared objects) -----------------------------------------------------------------------------

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int cmsGetEncodedCMMversion();


        // Context handling --------------------------------------------------------------------------------------------------------

        // Each context holds its owns globals and its own plug-ins. There is a global context with the id = 0 for lecacy compatibility
        // though using the global context is not recommended. Proper context handling makes lcms more thread-safe.
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateContext(IntPtr Plugin, IntPtr UserData);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsDeleteContext(IntPtr ContextID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsDupContext(IntPtr ContextID, IntPtr NewUserData);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsGetContextUserData(IntPtr ContextID);

        // Plug-In registering  --------------------------------------------------------------------------------------------------

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsPlugin(void* Plugin);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsPluginTHR(IntPtr ContextID, void* Plugin);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsUnregisterPlugins();
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsUnregisterPluginsTHR(IntPtr ContextID);

        // Error logging ----------------------------------------------------------------------------------------------------------

        // There is no error handling at all. When a function fails, it returns proper value.
        // For example, all create functions does return NULL on failure. Other may return FALSE.
        // It may be interesting, for the developer, to know why the function is failing.
        // for that reason, lcms2 does offer a logging function. This function will get
        // an ENGLISH string with some clues on what is going wrong. You can show this
        // info to the end user if you wish, or just create some sort of log on disk.
        // The logging function should NOT terminate the program, as this obviously can leave
        // unfreed resources. It is the programmer's responsibility to check each function
        // return code to make sure it didn't fail.


        // Error logger is called with the ContextID when a message is raised. This gives the
        // chance to know which thread is responsible of the warning and any environment associated
        // with it. Non-multithreading applications may safely ignore this parameter.
        // Note that under certain special circumstances, ContextID may be NULL.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void cmsLogErrorHandlerFunction(IntPtr ContextID, CmsError ErrorCode, [In, MarshalAs(UnmanagedType.LPStr)] string Text);

        // Allows user to set any specific logger
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetLogErrorHandler(cmsLogErrorHandlerFunction Fn);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetLogErrorHandlerTHR(IntPtr ContextID, cmsLogErrorHandlerFunction Fn);

        // Returns pointers to constant structs
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ref readonly CIEXYZ cmsD50_XYZ();
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ref readonly CIExyY cmsD50_xyY();

        // Colorimetric space conversions
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsXYZ2xyY(out CIExyY Dest, in CIEXYZ Source);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsxyY2XYZ(CIEXYZ* Dest, in CIExyY Source);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsXYZ2Lab(in CIEXYZ WhitePoint, out CIELab Lab, in CIEXYZ xyz);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsLab2XYZ(in CIEXYZ WhitePoint, out CIEXYZ xyz, in CIELab Lab);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsLab2LCh(out CIELCh LCh, in CIELab Lab);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsLCh2Lab(out CIELab Lab, in CIELCh LCh);

        // Encoding /Decoding on PCS
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsLabEncoded2Float(out CIELab Lab, in ushort wLab_3);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsLabEncoded2FloatV2(out CIELab Lab, in ushort wLab_3);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsFloat2LabEncoded(out ushort wLab_3, in CIELab Lab);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsFloat2LabEncodedV2(out ushort wLab_3, in CIELab Lab);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsXYZEncoded2Float(out CIEXYZ fxyz, in ushort XYZ_3);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsFloat2XYZEncoded(out ushort XYZ_3, in CIEXYZ fXYZ);


        // DeltaE metrics
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsDeltaE(in CIELab Lab1, in CIELab Lab2);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsCIE94DeltaE(in CIELab Lab1, in CIELab Lab2);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsBFDdeltaE(in CIELab Lab1, in CIELab Lab2);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsCMCdeltaE(in CIELab Lab1, in CIELab Lab2, double l, double c);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsCIE2000DeltaE(in CIELab Lab1, in CIELab Lab2, double Kl, double Kc, double Kh);

        // Temperature <-> Chromaticity (Black body)
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsWhitePointFromTemp(out CIExyY WhitePoint, double TempK);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsTempFromWhitePoint(double* TempK, in CIExyY WhitePoint);

        // Chromatic adaptation
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsAdaptToIlluminant(out CIEXYZ Result, in CIEXYZ SourceWhitePt,
                                                                                   in CIEXYZ Illuminant,
                                                                                   in CIEXYZ Value);

        // CIECAM02 ---------------------------------------------------------------------------------------------------

        // Viewing conditions. Please note those are CAM model viewing conditions, and not the ICC tag viewing
        // conditions, which I'm naming cmsICCViewingConditions to make differences evident. Unfortunately, the tag
        // cannot deal with surround La, Yb and D value so is basically useless to store CAM02 viewing conditions.

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCIECAM02Init(IntPtr ContextID, in cmsViewingConditions pVC);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsCIECAM02Done(IntPtr hModel);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsCIECAM02Forward(IntPtr hModel, in CIEXYZ pIn, out JCh pOut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsCIECAM02Reverse(IntPtr hModel, in JCh pIn, out CIEXYZ pOut);


        // Tone curves -----------------------------------------------------------------------------------------

        // This describes a curve segment. For a table of supported types, see the manual. User can increase the number of
        // available types by using a proper plug-in. Parametric segments allow 10 parameters at most
        public struct cmsCurveSegment
        {
            public float x0, x1;           // Domain; for x0 < x <= x1
            public int Type;             // Parametric type, Type == 0 means sampled segment. Negative values are reserved
            public fixed double Params[10];       // Parameters if Type != 0
            public uint nGridPoints;      // Number of grid points if Type == 0
            public float* SampledPoints;    // Points to an array of floats if Type == 0
        }

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsBuildSegmentedToneCurve(IntPtr ContextID, uint nSegments, in cmsCurveSegment Segments);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsBuildParametricToneCurve(IntPtr ContextID, int Type, in double Params);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsBuildGamma(IntPtr ContextID, double Gamma);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsBuildTabulatedToneCurve16(IntPtr ContextID, uint nEntries, in ushort values);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsBuildTabulatedToneCurveFloat(IntPtr ContextID, uint nEntries, in float values);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsFreeToneCurve(IntPtr Curve);
        //[DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        //public static extern void cmsFreeToneCurveTriple(IntPtr Curve[3]);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsDupToneCurve(IntPtr Src);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsReverseToneCurve(IntPtr InGamma);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsReverseToneCurveEx(uint nResultSamples, IntPtr InGamma);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsJoinToneCurve(IntPtr ContextID, IntPtr X, IntPtr Y, uint nPoints);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsSmoothToneCurve(IntPtr Tab, double lambda);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern float cmsEvalToneCurveFloat(IntPtr Curve, float v);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ushort cmsEvalToneCurve16(IntPtr Curve, ushort v);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIsToneCurveMultisegment(IntPtr InGamma);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIsToneCurveLinear(IntPtr Curve);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIsToneCurveMonotonic(IntPtr t);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIsToneCurveDescending(IntPtr t);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int cmsGetToneCurveParametricType(IntPtr t);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsEstimateGamma(IntPtr t, double Precision);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ref readonly double cmsGetToneCurveParams(IntPtr t);

        // Tone curve tabular estimation
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetToneCurveEstimatedTableEntries(IntPtr t);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ref readonly ushort cmsGetToneCurveEstimatedTable(IntPtr t);

        // Implements pipelines of multi-processing elements -------------------------------------------------------------

        // Those are hi-level pipelines
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsPipelineAlloc(IntPtr ContextID, uint InputChannels, uint OutputChannels);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsPipelineFree(IntPtr lut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsPipelineDup(IntPtr Orig);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsGetPipelineContextID(IntPtr lut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsPipelineInputChannels(IntPtr lut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsPipelineOutputChannels(IntPtr lut);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsPipelineStageCount(IntPtr lut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsPipelineGetPtrToFirstStage(IntPtr lut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsPipelineGetPtrToLastStage(IntPtr lut);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsPipelineEval16(in ushort In, out ushort Out, IntPtr lut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsPipelineEvalFloat(in float In, out float Out, IntPtr lut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsPipelineEvalReverseFloat(ref float Target, ref float Result, ref float Hint, IntPtr lut);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsPipelineCat(IntPtr l1, IntPtr l2);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsPipelineSetSaveAs8bitsFlag(IntPtr lut, bool On);
    }



    internal unsafe partial class CmsNative
    {
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsPipelineInsertStage(IntPtr lut, StageLoc loc, IntPtr mpe);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsPipelineUnlinkStage(IntPtr lut, StageLoc loc, out IntPtr mpe);

        // This function is quite useful to analyze the structure of a Pipeline and retrieve the Stage elements
        // that conform the Pipeline. It should be called with the Pipeline, the number of expected elements and
        // then a list of expected types followed with a list of double pointers to Stage elements. If
        // the function founds a match with current pipeline, it fills the pointers and returns TRUE
        // if not, returns FALSE without touching anything.
        [DllImport(LIB, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern bool cmsPipelineCheckAndRetreiveStages(IntPtr Lut, uint n, __arglist);

        // Matrix has double precision and CLUT has only float precision. That is because an ICC profile can encode
        // matrices with far more precision that CLUTS
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageAllocIdentity(IntPtr ContextID, uint nChannels);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageAllocToneCurves(IntPtr ContextID, uint nChannels, in IntPtr Curves);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageAllocMatrix(IntPtr ContextID, uint Rows, uint Cols, in double Matrix, in double Offset);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageAllocCLut16bit(IntPtr ContextID, uint nGridPoints, uint inputChan, uint outputChan, in ushort Table);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageAllocCLutFloat(IntPtr ContextID, uint nGridPoints, uint inputChan, uint outputChan, in float Table);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageAllocCLut16bitGranular(IntPtr ContextID, in uint clutPoints, uint inputChan, uint outputChan, in ushort Table);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageAllocCLutFloatGranular(IntPtr ContextID, in uint clutPoints, uint inputChan, uint outputChan, in float Table);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageDup(IntPtr mpe);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsStageFree(IntPtr mpe);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageNext(IntPtr mpe);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsStageInputChannels(IntPtr mpe);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsStageOutputChannels(IntPtr mpe);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern StageSignature cmsStageType(IntPtr mpe);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsStageData(IntPtr mpe);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsGetStageContextID(IntPtr mpe);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int cmsSAMPLER16(ushort* In, ushort* Out, IntPtr Cargo);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int cmsSAMPLERFLOAT(float* In, float* Out, IntPtr Cargo);

        const uint SAMPLER_INSPECT = 0x01000000;

        // For CLUT only
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsStageSampleCLut16bit(IntPtr mpe, cmsSAMPLER16 Sampler, void* Cargo, uint dwFlags);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsStageSampleCLutFloat(IntPtr mpe, cmsSAMPLERFLOAT Sampler, void* Cargo, uint dwFlags);

        // Slicers
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsSliceSpace16(uint nInputs, in uint clutPoints,
                                                           cmsSAMPLER16 Sampler, void* Cargo);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsSliceSpaceFloat(uint nInputs, in uint clutPoints,
                                                           cmsSAMPLERFLOAT Sampler, void* Cargo);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsMLUalloc(IntPtr ContextID, uint nItems);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsMLUfree(IntPtr mlu);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsMLUdup(IntPtr mlu);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern bool cmsMLUsetASCII(IntPtr mlu,
                                                  in uint LanguageCode, in uint CountryCode,
                                                  string ASCIIString);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsMLUsetWide(IntPtr mlu,
                                                  in uint LanguageCode, in uint CountryCode,
                                                  in byte WideString);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsMLUgetASCII(IntPtr mlu,
                                                  in uint LanguageCode, in uint CountryCode,
                                                  byte* Buffer, uint BufferSize);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsMLUgetWide(IntPtr mlu,
                                                 in uint LanguageCode, in uint CountryCode,
                                                 byte* Buffer, uint BufferSize);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsMLUgetTranslation(IntPtr mlu,
                                                         in uint LanguageCode, in uint CountryCode,
                                                         ref byte ObtainedLanguage_3, ref byte ObtainedCountry_3);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsMLUtranslationsCount(IntPtr mlu);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsMLUtranslationsCodes(IntPtr mlu,
                                                                     uint idx,
                                                                     ref byte LanguageCode_3,
                                                                     ref byte CountryCode_3);


        struct cmsUcrBg
        {
            IntPtr UcrToneCurve;
            IntPtr BgToneCurve;
            IntPtr DescMLU;
        }

        public const int cmsPRINTER_DEFAULT_SCREENS = 0x0001;
        public const int cmsFREQUENCE_UNITS_LINES_CM = 0x0000;
        public const int cmsFREQUENCE_UNITS_LINES_INCH = 0x0002;


        public struct cmsDICTentry
        {
            public cmsDICTentry* Next;
            IntPtr DisplayNameMLU;
            IntPtr DisplayValueMLU;
            byte* NameWcharT;
            byte* ValueWcharT;
        }

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsDictAlloc(IntPtr ContextID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsDictFree(IntPtr hDict);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsDictDup(IntPtr hDict);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsDictAddEntry(IntPtr hDict, void* Name, void* Value, IntPtr DisplayNameMLU, IntPtr DisplayValueMLU);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern cmsDICTentry* cmsDictGetEntryList(IntPtr hDict);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern cmsDICTentry* cmsDictNextEntry(cmsDICTentry* e);

        // Access to Profile data ----------------------------------------------------------------------------------------------
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateProfilePlaceholder(IntPtr ContextID);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsGetProfileContextID(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int cmsGetTagCount(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern TagSignature cmsGetTagSignature(IntPtr hProfile, uint n);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIsTag(IntPtr hProfile, TagSignature sig);

        // Read and write pre-formatted data
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void* cmsReadTag(IntPtr hProfile, TagSignature sig);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsWriteTag(IntPtr hProfile, TagSignature sig, void* data);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsLinkTag(IntPtr hProfile, TagSignature sig, TagSignature dest);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern TagSignature cmsTagLinkedTo(IntPtr hProfile, TagSignature sig);

        // Read and write raw data
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsReadRawTag(IntPtr hProfile, TagSignature sig, void* Buffer, uint BufferSize);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsWriteRawTag(IntPtr hProfile, TagSignature sig, in byte data, uint Size);


        // Access header data
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern HeaderFlags cmsGetHeaderFlags(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsGetHeaderAttributes(IntPtr hProfile, out ulong Flags);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsGetHeaderProfileID(IntPtr hProfile, out Guid ProfileID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsGetHeaderCreationDateTime(IntPtr hProfile, out CmsStructTm Dest);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetHeaderRenderingIntent(IntPtr hProfile);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetHeaderFlags(IntPtr hProfile, HeaderFlags Flags);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetHeaderManufacturer(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetHeaderManufacturer(IntPtr hProfile, uint manufacturer);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetHeaderCreator(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetHeaderModel(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetHeaderModel(IntPtr hProfile, uint model);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetHeaderAttributes(IntPtr hProfile, ulong Flags);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetHeaderProfileID(IntPtr hProfile, in Guid ProfileID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetHeaderRenderingIntent(IntPtr hProfile, uint RenderingIntent);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ColorSpaceSignature
                                  cmsGetPCS(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetPCS(IntPtr hProfile, ColorSpaceSignature pcs);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ColorSpaceSignature
                                  cmsGetColorSpace(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetColorSpace(IntPtr hProfile, ColorSpaceSignature sig);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ProfileClassSignature
                                  cmsGetDeviceClass(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetDeviceClass(IntPtr hProfile, ProfileClassSignature sig);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetProfileVersion(IntPtr hProfile, double Version);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsGetProfileVersion(IntPtr hProfile);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetEncodedICCversion(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetEncodedICCversion(IntPtr hProfile, uint Version);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIsIntentSupported(IntPtr hProfile, uint Intent, ProfileUsedDirection UsedDirection);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIsMatrixShaper(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIsCLUT(IntPtr hProfile, uint Intent, ProfileUsedDirection UsedDirection);

        // Translate form/to our notation to ICC
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern ColorSpaceSignature _cmsICCcolorSpace(int OurNotation);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int _cmsLCMScolorSpace(ColorSpaceSignature ProfileSpace);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsChannelsOf(ColorSpaceSignature ColorSpace);

        // Build a suitable formatter for the colorspace of this profile. nBytes=1 means 8 bits, nBytes=2 means 16 bits. 
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsFormatterForColorspaceOfProfile(IntPtr hProfile, uint nBytes, bool lIsFloat);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsFormatterForPCSOfProfile(IntPtr hProfile, uint nBytes, bool lIsFloat);

        // Localized info
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetProfileInfo(IntPtr hProfile, InfoType Info,
                                                                in uint LanguageCode, in uint CountryCode,
                                                                void* Buffer, uint BufferSize);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetProfileInfoASCII(IntPtr hProfile, InfoType Info,
                                                                    in uint LanguageCode, in uint CountryCode,
                                                                    byte* Buffer, uint BufferSize);

        // MD5 message digest --------------------------------------------------------------------------------------------------

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsMD5computeID(IntPtr hProfile);

        // IO handlers ----------------------------------------------------------------------------------------------------------
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr cmsOpenIOhandlerFromMem(IntPtr ContextID, void* Buffer, uint size, string AccessMode);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsOpenIOhandlerFromNULL(IntPtr ContextID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsGetProfileIOhandler(IntPtr hProfile);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsCloseIOhandler(IntPtr io);


        // Profile high level functions ------------------------------------------------------------------------------------------

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsOpenProfileFromMem(void* MemPtr, uint dwSize);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsOpenProfileFromMemTHR(IntPtr ContextID, void* MemPtr, uint dwSize);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsOpenProfileFromIOhandlerTHR(IntPtr ContextID, IntPtr io);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsOpenProfileFromIOhandler2THR(IntPtr ContextID, IntPtr io, bool write);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsCloseProfile(IntPtr hProfile);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsSaveProfileToMem(IntPtr hProfile, void* MemPtr, ref uint BytesNeeded);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsSaveProfileToIOhandler(IntPtr hProfile, IntPtr io);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateRGBProfileTHR(IntPtr ContextID,
                                                   in CIExyY WhitePoint,
                                                   in CIExyYTRIPLE Primaries,
                                                   ref IntPtr TransferFunction3);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateRGBProfile(in CIExyY WhitePoint,
                                                   in CIExyYTRIPLE Primaries,
                                                   ref IntPtr TransferFunction);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateGrayProfileTHR(IntPtr ContextID,
                                                            in CIExyY WhitePoint,
                                                    IntPtr TransferFunction);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateGrayProfile(in CIExyY WhitePoint,
                                                    IntPtr TransferFunction);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateLinearizationDeviceLinkTHR(IntPtr ContextID,
                                                                        ColorSpaceSignature ColorSpace,
                                                                        ref IntPtr TransferFunctions);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateLinearizationDeviceLink(ColorSpaceSignature ColorSpace,
                                                                        ref IntPtr TransferFunctions);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateInkLimitingDeviceLinkTHR(IntPtr ContextID,
                                                                      ColorSpaceSignature ColorSpace, double Limit);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateInkLimitingDeviceLink(ColorSpaceSignature ColorSpace, double Limit);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateLab2ProfileTHR(IntPtr ContextID, in CIExyY WhitePoint);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateLab2Profile(in CIExyY WhitePoint);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateLab4ProfileTHR(IntPtr ContextID, in CIExyY WhitePoint);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateLab4Profile(in CIExyY WhitePoint);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateXYZProfileTHR(IntPtr ContextID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateXYZProfile();

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreate_sRGBProfileTHR(IntPtr ContextID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreate_sRGBProfile();

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateBCHSWabstractProfileTHR(IntPtr ContextID,
                                                                     uint nLUTPoints,
                                                                     double Bright,
                                                                     double Contrast,
                                                                     double Hue,
                                                                     double Saturation,
                                                                     uint TempSrc,
                                                                     uint TempDest);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateBCHSWabstractProfile(uint nLUTPoints,
                                                                     double Bright,
                                                                     double Contrast,
                                                                     double Hue,
                                                                     double Saturation,
                                                                     uint TempSrc,
                                                                     uint TempDest);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateNULLProfileTHR(IntPtr ContextID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateNULLProfile();

        // Converts a transform to a devicelink profile
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsTransform2DeviceLink(IntPtr hTransform, double Version, uint dwFlags);

        // Call with NULL as parameters to get the intent count
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetSupportedIntents(uint nMax, uint* Codes, byte** Descriptions);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetSupportedIntentsTHR(IntPtr ContextID, uint nMax, uint* Codes, byte** Descriptions);


        // Transforms ---------------------------------------------------------------------------------------------------

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateTransformTHR(IntPtr ContextID,
                                                          IntPtr Input,
                                                          uint InputFormat,
                                                          IntPtr Output,
                                                          uint OutputFormat,
                                                          uint Intent,
                                                          uint dwFlags);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateTransform(IntPtr Input,
                                                          uint InputFormat,
                                                          IntPtr Output,
                                                          uint OutputFormat,
                                                          uint Intent,
                                                          uint dwFlags);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateProofingTransformTHR(IntPtr ContextID,
                                                          IntPtr Input,
                                                          uint InputFormat,
                                                          IntPtr Output,
                                                          uint OutputFormat,
                                                          IntPtr Proofing,
                                                          uint Intent,
                                                          uint ProofingIntent,
                                                          uint dwFlags);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateProofingTransform(IntPtr Input,
                                                          uint InputFormat,
                                                          IntPtr Output,
                                                          uint OutputFormat,
                                                          IntPtr Proofing,
                                                          uint Intent,
                                                          uint ProofingIntent,
                                                          uint dwFlags);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateMultiprofileTransformTHR(IntPtr ContextID,
                                                          ref IntPtr hProfiles,
                                                          uint nProfiles,
                                                          uint InputFormat,
                                                          uint OutputFormat,
                                                          uint Intent,
                                                          uint dwFlags);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateMultiprofileTransform(ref IntPtr hProfiles,
                                                          uint nProfiles,
                                                          uint InputFormat,
                                                          uint OutputFormat,
                                                          uint Intent,
                                                          uint dwFlags);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsCreateExtendedTransform(IntPtr ContextID,
                                                           uint nProfiles, ref IntPtr hProfiles,
                                                           ref bool BPC,
                                                           ref uint Intents,
                                                           ref double AdaptationStates,
                                                           IntPtr hGamutProfile,
                                                           uint nGamutPCSposition,
                                                           uint InputFormat,
                                                           uint OutputFormat,
                                                           uint dwFlags);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsDeleteTransform(IntPtr hTransform);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsDoTransform(IntPtr Transform,
                                                 void* InputBuffer,
                                                 void* OutputBuffer,
                                                 uint Size);

        [Obsolete]
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsDoTransformStride(IntPtr Transform,   // Deprecated
                                                 void* InputBuffer,
                                                 void* OutputBuffer,
                                                 uint Size,
                                                 uint Stride);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsDoTransformLineStride(IntPtr Transform,
                                                 void* InputBuffer,
                                                 void* OutputBuffer,
                                                 uint PixelsPerLine,
                                                 uint LineCount,
                                                 uint BytesPerLineIn,
                                                 uint BytesPerLineOut,
                                                 uint BytesPerPlaneIn,
                                                 uint BytesPerPlaneOut);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetAlarmCodes(ref ushort NewAlarm_cmsMAXCHANNELS);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsGetAlarmCodes(ref ushort NewAlarm_cmsMAXCHANNELS);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsSetAlarmCodesTHR(IntPtr ContextID,
                                                          ref ushort NewAlarm_cmsMAXCHANNELS);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsGetAlarmCodesTHR(IntPtr ContextID,
                                                                  ref ushort NewAlarm_cmsMAXCHANNELS);



        // Adaptation state for absolute colorimetric intent
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsSetAdaptationState(double d);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsSetAdaptationStateTHR(IntPtr ContextID, double d);



        // Grab the ContextID from an open transform. Returns NULL if a NULL transform is passed
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsGetTransformContextID(IntPtr hTransform);

        // Grab the input/output formats
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetTransformInputFormat(IntPtr hTransform);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetTransformOutputFormat(IntPtr hTransform);

        // For backwards compatibility
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsChangeBuffersFormat(IntPtr hTransform,
                                                                 uint InputFormat,
                                                                 uint OutputFormat);

        // PostScript ColorRenderingDictionary and ColorSpaceArray ----------------------------------------------------

        // lcms2 unified method to access postscript color resources
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetPostScriptColorResource(IntPtr ContextID,
                                                                PSResourceType Type,
                                                                IntPtr hProfile,
                                                                uint Intent,
                                                                uint dwFlags,
                                                                IntPtr io);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetPostScriptCSA(IntPtr ContextID, IntPtr hProfile, uint Intent, uint dwFlags, void* Buffer, uint dwBufferLen);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsGetPostScriptCRD(IntPtr ContextID, IntPtr hProfile, uint Intent, uint dwFlags, void* Buffer, uint dwBufferLen);


        // IT8.7 / CGATS.17-200x handling -----------------------------------------------------------------------------

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsIT8Alloc(IntPtr ContextID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsIT8Free(IntPtr hIT8);

        // Tables
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsIT8TableCount(IntPtr hIT8);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int cmsIT8SetTable(IntPtr hIT8, uint nTable);

        // Persistence

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsIT8LoadFromMem(IntPtr ContextID, void* Ptr, uint len);
        //[DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        //public static extern IntPtr cmsIT8LoadFromIOhandler(IntPtr ContextID, IntPtr io);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SaveToMem(IntPtr hIT8, void* MemPtr, uint* BytesNeeded);

        // Properties
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string cmsIT8GetSheetType(IntPtr hIT8);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetSheetType(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Type);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetComment(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cComment);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetPropertyStr(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cProp, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Str);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetPropertyDbl(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cProp, double Val);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetPropertyHex(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cProp, uint Val);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetPropertyMulti(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Key, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string SubKey, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Buffer);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetPropertyUncooked(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Key, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Buffer);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        public static extern string cmsIT8GetProperty(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cProp);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsIT8GetPropertyDbl(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cProp);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        public static extern string cmsIT8GetPropertyMulti(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Key, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string SubKey);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsIT8EnumProperties(IntPtr hIT8, out byte** PropertyNames);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern uint cmsIT8EnumPropertyMulti(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cProp, out byte** SubpropertyNames);

        // Datasets
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        public static extern string cmsIT8GetDataRowCol(IntPtr hIT8, int row, int col);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsIT8GetDataRowColDbl(IntPtr hIT8, int row, int col);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetDataRowCol(IntPtr hIT8, int row, int col,
                                                [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Val);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetDataRowColDbl(IntPtr hIT8, int row, int col,
                                                        double Val);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        public static extern string cmsIT8GetData(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cPatch, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cSample);


        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsIT8GetDataDbl(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cPatch, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cSample);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetData(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cPatch,
                                                [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cSample,
                                                [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Val);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetDataDbl(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cPatch,
                                                [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cSample,
                                                double Val);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int cmsIT8FindDataFormat(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cSample);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetDataFormat(IntPtr hIT8, int n, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Sample);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int cmsIT8EnumDataFormat(IntPtr hIT8, out byte** SampleNames);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        public static extern string cmsIT8GetPatchName(IntPtr hIT8, int nPatch, void* buffer);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int cmsIT8GetPatchByName(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cPatch);

        // The LABEL extension
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern int cmsIT8SetTableByLabel(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cSet, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cField, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string ExpectedType);

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsIT8SetIndexColumn(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string cSample);

        // Formatter for double
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsIT8DefineDblFormat(IntPtr hIT8, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string Formatter);


        // Gamut boundary description routines ------------------------------------------------------------------------------

        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr cmsGBDAlloc(IntPtr ContextID);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern void cmsGBDFree(IntPtr hGBD);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsGDBAddPoint(IntPtr hGBD, in CIELab Lab);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsGDBCompute(IntPtr hGDB, uint dwFlags);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsGDBCheckPoint(IntPtr hGBD, in CIELab Lab);

        // Feature detection  ----------------------------------------------------------------------------------------------

        // Estimate the black point
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsDetectBlackPoint(out CIEXYZ BlackPoint, IntPtr hProfile, uint Intent, uint dwFlags);
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsDetectDestinationBlackPoint(out CIEXYZ BlackPoint, IntPtr hProfile, uint Intent, uint dwFlags);

        // Estimate total area coverage
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsDetectTAC(IntPtr hProfile);

        // Estimate gamma space, always positive. Returns -1 on error.
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern double cmsDetectRGBProfileGamma(IntPtr hProfile, double threshold);

        // Poor man's gamut mapping
        [DllImport(LIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool cmsDesaturateLab(ref CIELab Lab,
                                                           double amax, double amin,
                                                           double bmax, double bmin);
    }

    
}

