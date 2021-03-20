using ATI.ADL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ADL_CONTEXT_HANDLE = System.IntPtr;

namespace ColorControl
{
    enum ADLPixelFormat
    {
        UNKNOWN = 0,
        RGB_FULL_RANGE = 1,
        YCRCB444 = 2,
        YCRCB422 = 3,
        RGB_LIMITED_RANGE = 4,
        YCRCB420 = 8
    }

    enum ADLColorDepth
    {
        UNKNOWN = 0,
        BPC6 = 1,
        BPC8 = 2,
        BPC10 = 3,
        BPC12 = 4,
        BPC14 = 5,
        BPC16 = 6
    }

    enum ADLDitherState
    {
        DISABLED = 0,
        DRIVER_DEFAULT = 1,
        FM6 = 2,
        FM8 = 3,
        FM10 = 4,
        DITH6 = 5,
        DITH8 = 6,
        DITH10 = 7,
        DITH6_NO_FRAME_RAND = 8,
        DITH8_NO_FRAME_RAND = 9,
        DITH10_NO_FRAME_RAND = 10,
        TRUN6 = 11,
        TRUN8 = 12,
        TRUN10 = 13,
        TRUN8_DITH8 = 14,
        TRUN10_DUTH6 = 15,
        TRUN10_FM8 = 16,
        TRUN10_FM6 = 17,
        TRUN10_DITH8_FM6 = 18,
        DITH10_FM8 = 19,
        DITH10_FM6 = 20,
        TRUN8_DITH6 = 21,
        TRUN8_FM6 = 22,
        DITH8_FM6 = 23
    }

    class ADLWrapper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static ADL_CONTEXT_HANDLE context = IntPtr.Zero;
        
        public static bool Initialize()
        {
            int ADLRet = -1;

            Logger.Debug("Initialize");

            var del = ADL.GetDelegate<ADL.ADL_Main_Control_Create>();

            if (del != null)
            {
                // Second parameter is 1: Get only the present adapters
                ADLRet = del(ADL.ADL_Main_Memory_Alloc_Func, 1);
                CheckError(ADLRet, nameof(ADL.ADL_Main_Control_Create));
            }

            Logger.Debug("ADLRet1: " + ADLRet);

            var del2 = ADL.GetDelegate<ADL.ADL2_Main_Control_Create>();

            if (del2 != null)
            {
                // Second parameter is 1: Get only the present adapters
                ADLRet = del2(ADL.ADL_Main_Memory_Alloc_Func, 1, ref context);
                Logger.Debug("ADLRet2: " + ADLRet);
                CheckError(ADLRet, nameof(ADL.ADL2_Main_Control_Create));
            }

            return (ADL.ADL_SUCCESS == ADLRet);
        }

        public static void Uninitialze()
        {
            if (!ADL.ADLDelegates.Any())
            {
                return;
            }

            var del = ADL.GetDelegate<ADL.ADL_Main_Control_Destroy>();

            if (null != del)
            {
                del();
            }

            var del2 = ADL.GetDelegate<ADL.ADL2_Main_Control_Destroy>();

            if (null != del2 && context != IntPtr.Zero)
            {
                del2(context);
            }
        }

        public static ADLAdapterInfoArray? GetAdapters()
        {
            int ADLRet = -1;
            int NumberOfAdapters = 0;

            var delNumberOfAdapters = ADL.GetDelegate<ADL.ADL_Adapter_NumberOfAdapters_Get>();

            if (null != delNumberOfAdapters)
            {
                delNumberOfAdapters(ref NumberOfAdapters);
            }
            Logger.Debug("Number Of Adapters: " + NumberOfAdapters.ToString() + "\n");

            if (NumberOfAdapters > 0)
            {
                // Get OS adpater info from ADL
                ADLAdapterInfoArray OSAdapterInfoData;
                OSAdapterInfoData = new ADLAdapterInfoArray();

                var delAdapterInfo = ADL.GetDelegate<ADL.ADL_Adapter_AdapterInfo_Get>();

                if (null != delAdapterInfo)
                {
                    IntPtr AdapterBuffer = IntPtr.Zero;
                    int size = Marshal.SizeOf(OSAdapterInfoData);
                    AdapterBuffer = Marshal.AllocCoTaskMem((int)size);
                    Marshal.StructureToPtr(OSAdapterInfoData, AdapterBuffer, false);

                    ADLRet = delAdapterInfo(AdapterBuffer, size);
                    if (ADL.ADL_SUCCESS == ADLRet)
                    {
                        OSAdapterInfoData = (ADLAdapterInfoArray)Marshal.PtrToStructure(AdapterBuffer, OSAdapterInfoData.GetType());
                        int IsActive = 0;

                        var delActive = ADL.GetDelegate<ADL.ADL_Adapter_Active_Get>();

                        for (int i = 0; i < NumberOfAdapters; i++)
                        {
                            // Check if the adapter is active
                            if (null != delActive)
                                ADLRet = delActive(OSAdapterInfoData.ADLAdapterInfo[i].AdapterIndex, ref IsActive);

                            if (ADL.ADL_SUCCESS == ADLRet)
                            {

                            }
                        }
                    }
                    else
                    {
                        Logger.Debug("ADL_Adapter_AdapterInfo_Get() returned error code " + ADLRet.ToString());
                    }
                    // Release the memory for the AdapterInfo structure
                    if (IntPtr.Zero != AdapterBuffer)
                    {
                        Marshal.FreeCoTaskMem(AdapterBuffer);
                    }
                }
                return OSAdapterInfoData;
            }
            return null;
        }

        public static List<ADLDisplayInfo> GetAllDisplays(bool connected = true)
        {
            int ADLRet;
            int NumberOfDisplays = 0;

            var DisplayInfoData = new List<ADLDisplayInfo>();

            var OSAdapterInfoData = GetAdapters();
            if (OSAdapterInfoData == null || !OSAdapterInfoData.Value.ADLAdapterInfo.Any())
            {
                return DisplayInfoData;
            }
            var primaryAdapter = OSAdapterInfoData.Value.ADLAdapterInfo.First();

            ADLDisplayInfo oneDisplayInfo = new ADLDisplayInfo();

            var del = ADL.GetDelegate<ADL.ADL_Display_DisplayInfo_Get>();

            if (null != del)
            {
                IntPtr DisplayBuffer = IntPtr.Zero;
                int j = 0;

                // Force the display detection and get the Display Info. Use 0 as last parameter to NOT force detection
                ADLRet = del(primaryAdapter.AdapterIndex, ref NumberOfDisplays, out DisplayBuffer, 1);
                if (ADL.ADL_SUCCESS == ADLRet)
                {
                    for (j = 0; j < NumberOfDisplays; j++)
                    {
                        oneDisplayInfo = (ADLDisplayInfo)Marshal.PtrToStructure(new IntPtr(DisplayBuffer.ToInt64() + j * Marshal.SizeOf(oneDisplayInfo)), oneDisplayInfo.GetType());
                        if (!connected || (1 == (oneDisplayInfo.DisplayInfoValue & 1))) {
                            DisplayInfoData.Add(oneDisplayInfo);
                        }
                    }
                    Logger.Debug("Total Number of Displays supported: " + NumberOfDisplays.ToString());
                    Logger.Debug("DispID  DispPhyId AdpID  Type OutType  CnctType Connected  Mapped  InfoValue DisplayName ");

                    for (j = 0; j < DisplayInfoData.Count; j++)
                    {
                        int InfoValue = DisplayInfoData[j].DisplayInfoValue;
                        string StrConnected = (1 == (InfoValue & 1)) ? "Yes" : "No ";
                        string StrMapped = (2 == (InfoValue & 2)) ? "Yes" : "No ";
                        int AdpID = DisplayInfoData[j].DisplayID.DisplayLogicalAdapterIndex;
                        string StrAdpID = (AdpID < 0) ? "--" : AdpID.ToString("d2");

                        Logger.Debug(DisplayInfoData[j].DisplayID.DisplayLogicalIndex.ToString() + "       " +
                                     DisplayInfoData[j].DisplayID.DisplayPhysicalIndex.ToString() + "       " +
                                        StrAdpID + "      " +
                                        DisplayInfoData[j].DisplayType.ToString() + "      " +
                                        DisplayInfoData[j].DisplayOutputType.ToString() + "      " +
                                        DisplayInfoData[j].DisplayConnector.ToString() + "        " +
                                        StrConnected + "        " +
                                        StrMapped + "      " +
                                        InfoValue.ToString("x4") + "   " +
                                        DisplayInfoData[j].DisplayName.ToString());
                    }
                }
                else
                {
                    Logger.Debug("ADL_Display_DisplayInfo_Get() returned error code " + ADLRet.ToString());
                }
                // Release the memory for the DisplayInfo structure
                if (IntPtr.Zero != DisplayBuffer)
                    Marshal.FreeCoTaskMem(DisplayBuffer);
            }
            return DisplayInfoData;
        }

        private static bool CheckError(int ADLRet, string functionName)
        {
            var result = ADLRet == ADL.ADL_SUCCESS;

            if (!result)
            {
                Logger.Error($"Error calling ADL ({functionName}): {ADLRet}");
            }

            return result;
        }

        public static bool GetDisplayResolution(ADLDisplayInfo display, ref int horizontal, ref int vertical)
        {
            var screen = Screen.PrimaryScreen;
            var bounds = screen.Bounds;

            horizontal = bounds.Width;
            vertical = bounds.Height;
            
            Logger.Debug($"GetDisplayResolution: {horizontal}x{vertical}");

            //var del = ADL.GetDelegate<ADL.ADL_Display_Size_Get>();

            //if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            //{
            //    int lpDefaultWidth, lpDefaultHeight, lpMinWidth, lpMinHeight, lpMaxWidth, lpMaxHeight, lpStepWidth, lpStepHeight;
            //    lpDefaultWidth = lpDefaultHeight = lpMinWidth = lpMinHeight = lpMaxWidth = lpMaxHeight = lpStepWidth = lpStepHeight = 0;
            //    var ADLRet = del(display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID.DisplayLogicalIndex, ref horizontal, ref vertical, ref lpDefaultWidth, ref lpDefaultHeight, ref lpMinWidth, ref lpMinHeight, ref lpMaxWidth, ref lpMaxHeight, ref lpStepWidth, ref lpStepHeight);

            //    return CheckError(ADLRet, nameof(ADL.ADL_Display_Size_Get));
            //}
            return true;
        }

        public static bool GetDisplayPixelFormat(ADLDisplayInfo display, ref ADLPixelFormat pixelFormat)
        {
            var del = ADL.GetDelegate<ADL.ADL_Display_PixelFormat_Get>();

            if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            {
                int pixelFormatInt = 0;
                var ADLRet = del(display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID.DisplayLogicalIndex, ref pixelFormatInt);

                pixelFormat = (ADLPixelFormat)pixelFormatInt;

                return CheckError(ADLRet, nameof(ADL.ADL_Display_PixelFormat_Get));
            }
            return false;
        }

        public static bool SetDisplayPixelFormat(ADLDisplayInfo display, ADLPixelFormat pixelFormat)
        {
            var del = ADL.GetDelegate<ADL.ADL_Display_PixelFormat_Set>();

            if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            {
                var ADLRet = del(display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID.DisplayLogicalIndex, (int)pixelFormat);

                return CheckError(ADLRet, nameof(ADL.ADL_Display_PixelFormat_Set));
            }
            return false;
        }

        public static bool GetDisplayColorDepth(ADLDisplayInfo display, ref ADLColorDepth colorDepth)
        {
            var del = ADL.GetDelegate<ADL.ADL_Display_ColorDepth_Get>();

            if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            {
                int colorDepthInt = 0;
                var ADLRet = del(display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID.DisplayLogicalIndex, ref colorDepthInt);

                colorDepth = (ADLColorDepth)colorDepthInt;

                return CheckError(ADLRet, nameof(ADL.ADL_Display_ColorDepth_Get));
            }
            return false;
        }

        public static bool SetDisplayColorDepth(ADLDisplayInfo display, ADLColorDepth colorDepth)
        {
            var del = ADL.GetDelegate<ADL.ADL_Display_ColorDepth_Set>();

            if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            {
                var ADLRet = del(display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID.DisplayLogicalIndex, (int)colorDepth);

                return CheckError(ADLRet, nameof(ADL.ADL_Display_ColorDepth_Set));
            }
            return false;
        }

        public static bool GetDisplayDitherState(ADLDisplayInfo display, ref ADLDitherState ditherState)
        {
            var del = ADL.GetDelegate<ADL.ADL_Display_DitherState_Get>();

            if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            {
                var ditherStateInt = 0;
                var ADLRet = del(display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID.DisplayLogicalIndex, ref ditherStateInt);

                ditherState = (ADLDitherState)ditherStateInt;

                return CheckError(ADLRet, nameof(ADL.ADL_Display_DitherState_Get));
            }
            return false;
        }

        public static bool SetDisplayDitherState(ADLDisplayInfo display, ADLDitherState ditherState)
        {
            var del = ADL.GetDelegate<ADL.ADL_Display_DitherState_Set>();

            if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            {
                var ADLRet = del(display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID.DisplayLogicalIndex, (int)ditherState);

                return CheckError(ADLRet, nameof(ADL.ADL_Display_DitherState_Set));
            }
            return false;
        }

        public static bool GetDisplayHDRState(ADLDisplayInfo display, ref bool supported, ref bool enabled)
        {
            var del = ADL.GetDelegate<ADL.ADL2_Display_HDRState_Get>();

            if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            {
                var supportedInt = 0;
                var enabledInt = 0;
                var ADLRet = del(context, display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID, ref supportedInt, ref enabledInt);

                supported = supportedInt != 0;
                enabled = enabledInt != 0;

                return CheckError(ADLRet, nameof(ADL.ADL2_Display_HDRState_Get));
            }
            return false;
        }

        public static bool SetDisplayHDRState(ADLDisplayInfo display, bool enabled)
        {
            var del = ADL.GetDelegate<ADL.ADL2_Display_HDRState_Set>();

            if (display.DisplayID.DisplayPhysicalIndex >= 0 && del != null)
            {
                var ADLRet = del(context, display.DisplayID.DisplayLogicalAdapterIndex, display.DisplayID, enabled ? 1 : 0);

                return CheckError(ADLRet, nameof(ADL.ADL2_Display_HDRState_Set));
            }
            return false;
        }
    }
}
