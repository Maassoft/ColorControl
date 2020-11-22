using ATI.ADL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ColorControl
{
    class ADLWrapper
    {
        public static bool Initialize()
        {
            int ADLRet = -1;

            if (null != ADL.ADL_Main_Control_Create)
            {
                // Second parameter is 1: Get only the present adapters
                ADLRet = ADL.ADL_Main_Control_Create(ADL.ADL_Main_Memory_Alloc, 1);
            }
            return (ADL.ADL_SUCCESS == ADLRet);
        }

        public static void Uninitialze()
        {
            if (null != ADL.ADL_Main_Control_Destroy)
            {
                ADL.ADL_Main_Control_Destroy();
            }
        }

        public static ADLAdapterInfoArray? GetAdapters()
        {
            int ADLRet = -1;
            int NumberOfAdapters = 0;

            if (null != ADL.ADL_Adapter_NumberOfAdapters_Get)
            {
                ADL.ADL_Adapter_NumberOfAdapters_Get(ref NumberOfAdapters);
            }
            Console.WriteLine("Number Of Adapters: " + NumberOfAdapters.ToString() + "\n");

            if (NumberOfAdapters > 0)
            {
                // Get OS adpater info from ADL
                ADLAdapterInfoArray OSAdapterInfoData;
                OSAdapterInfoData = new ADLAdapterInfoArray();

                if (null != ADL.ADL_Adapter_AdapterInfo_Get)
                {
                    IntPtr AdapterBuffer = IntPtr.Zero;
                    int size = Marshal.SizeOf(OSAdapterInfoData);
                    AdapterBuffer = Marshal.AllocCoTaskMem((int)size);
                    Marshal.StructureToPtr(OSAdapterInfoData, AdapterBuffer, false);

                    if (null != ADL.ADL_Adapter_AdapterInfo_Get)
                    {
                        ADLRet = ADL.ADL_Adapter_AdapterInfo_Get(AdapterBuffer, size);
                        if (ADL.ADL_SUCCESS == ADLRet)
                        {
                            OSAdapterInfoData = (ADLAdapterInfoArray)Marshal.PtrToStructure(AdapterBuffer, OSAdapterInfoData.GetType());
                            int IsActive = 0;

                            for (int i = 0; i < NumberOfAdapters; i++)
                            {
                                // Check if the adapter is active
                                if (null != ADL.ADL_Adapter_Active_Get)
                                    ADLRet = ADL.ADL_Adapter_Active_Get(OSAdapterInfoData.ADLAdapterInfo[i].AdapterIndex, ref IsActive);

                                if (ADL.ADL_SUCCESS == ADLRet)
                                {

                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("ADL_Adapter_AdapterInfo_Get() returned error code " + ADLRet.ToString());
                        }
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

        public static List<ADLDisplayInfo> GetAllDisplays()
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

            if (null != ADL.ADL_Display_DisplayInfo_Get)
            {
                IntPtr DisplayBuffer = IntPtr.Zero;
                int j = 0;

                // Force the display detection and get the Display Info. Use 0 as last parameter to NOT force detection
                ADLRet = ADL.ADL_Display_DisplayInfo_Get(primaryAdapter.AdapterIndex, ref NumberOfDisplays, out DisplayBuffer, 1);
                if (ADL.ADL_SUCCESS == ADLRet)
                {
                    for (j = 0; j < NumberOfDisplays; j++)
                    {
                        oneDisplayInfo = (ADLDisplayInfo)Marshal.PtrToStructure(new IntPtr(DisplayBuffer.ToInt32() + j * Marshal.SizeOf(oneDisplayInfo)), oneDisplayInfo.GetType());
                        DisplayInfoData.Add(oneDisplayInfo);
                    }
                    Console.WriteLine("\nTotal Number of Displays supported: " + NumberOfDisplays.ToString());
                    Console.WriteLine("\nDispID  AdpID  Type OutType  CnctType Connected  Mapped  InfoValue DisplayName ");

                    for (j = 0; j < NumberOfDisplays; j++)
                    {
                        int InfoValue = DisplayInfoData[j].DisplayInfoValue;
                        string StrConnected = (1 == (InfoValue & 1)) ? "Yes" : "No ";
                        string StrMapped = (2 == (InfoValue & 2)) ? "Yes" : "No ";
                        int AdpID = DisplayInfoData[j].DisplayID.DisplayLogicalAdapterIndex;
                        string StrAdpID = (AdpID < 0) ? "--" : AdpID.ToString("d2");

                        Console.WriteLine(DisplayInfoData[j].DisplayID.DisplayLogicalIndex.ToString() + "        " +
                                             StrAdpID + "      " +
                                             DisplayInfoData[j].DisplayType.ToString() + "      " +
                                             DisplayInfoData[j].DisplayOutputType.ToString() + "      " +
                                             DisplayInfoData[j].DisplayConnector.ToString() + "        " +
                                             StrConnected + "        " +
                                             StrMapped + "      " +
                                             InfoValue.ToString("x4") + "   " +
                                             DisplayInfoData[j].DisplayName.ToString());
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("ADL_Display_DisplayInfo_Get() returned error code " + ADLRet.ToString());
                }
                // Release the memory for the DisplayInfo structure
                if (IntPtr.Zero != DisplayBuffer)
                    Marshal.FreeCoTaskMem(DisplayBuffer);
            }
            return DisplayInfoData;
        }

        public static bool GetDisplayResolution(string displayName, ref int horizontal, ref int vertical)
        {
            var displays = GetAllDisplays();
            var display = displays.FirstOrDefault(d => d.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
            if (display.DisplayID.DisplayPhysicalIndex >= 0 && ADL.ADL_Display_Size_Get != null)
            {
                int lpDefaultWidth, lpDefaultHeight, lpMinWidth, lpMinHeight, lpMaxWidth, lpMaxHeight, lpStepWidth, lpStepHeight;
                lpDefaultWidth = lpDefaultHeight = lpMinWidth = lpMinHeight = lpMaxWidth = lpMaxHeight = lpStepWidth = lpStepHeight = 0;
                var ADLRet = ADL.ADL_Display_Size_Get(display.DisplayID.DisplayPhysicalAdapterIndex, display.DisplayID.DisplayPhysicalIndex, ref horizontal, ref vertical, ref lpDefaultWidth, ref lpDefaultHeight, ref lpMinWidth, ref lpMinHeight, ref lpMaxWidth, ref lpMaxHeight, ref lpStepWidth, ref lpStepHeight);

                return ADLRet == ADL.ADL_SUCCESS;
            }
            return false;
        }
    }
}
