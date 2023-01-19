#region Copyright

/*******************************************************************************
 Copyright(c) 2008 - 2009 Advanced Micro Devices, Inc. All Rights Reserved.
 Copyright (c) 2002 - 2006  ATI Technologies Inc. All Rights Reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
 ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDED BUT NOT LIMITED TO
 THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
 PARTICULAR PURPOSE.
 
 File:        ADL.cs
 
 Purpose:     Implements ADL interface 
 
 Description: Implements some of the methods defined in ADL interface.
              
 ********************************************************************************/

#endregion Copyright

#region Using

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ADL_CONTEXT_HANDLE = System.IntPtr;
using FARPROC = System.IntPtr;
using HMODULE = System.IntPtr;

#endregion Using

#region ATI.ADL

namespace ATI.ADL
{
    #region Export Struct

    #region ADLAdapterInfo
    /// <summary> ADLAdapterInfo Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfo
    {
        /// <summary>The size of the structure</summary>
        int Size;
        /// <summary> Adapter Index</summary>
        internal int AdapterIndex;
        /// <summary> Adapter UDID</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string UDID;
        /// <summary> Adapter Bus Number</summary>
        internal int BusNumber;
        /// <summary> Adapter Driver Number</summary>
        internal int DriverNumber;
        /// <summary> Adapter Function Number</summary>
        internal int FunctionNumber;
        /// <summary> Adapter Vendor ID</summary>
        internal int VendorID;
        /// <summary> Adapter Adapter name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string AdapterName;
        /// <summary> Adapter Display name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayName;
        /// <summary> Adapter Present status</summary>
        internal int Present;
        /// <summary> Adapter Exist status</summary>
        internal int Exist;
        /// <summary> Adapter Driver Path</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DriverPath;
        /// <summary> Adapter Driver Ext Path</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DriverPathExt;
        /// <summary> Adapter PNP String</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string PNPString;
        /// <summary> OS Display Index</summary>
        internal int OSDisplayIndex;
    }


    /// <summary> ADLAdapterInfo Array</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfoArray
    {
        /// <summary> ADLAdapterInfo Array </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ADL.ADL_MAX_ADAPTERS)]
        internal ADLAdapterInfo[] ADLAdapterInfo;
    }
    #endregion ADLAdapterInfo


    #region ADLDisplayInfo
    /// <summary> ADLDisplayID Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLDisplayID
    {
        /// <summary> Display Logical Index </summary>
        internal int DisplayLogicalIndex;
        /// <summary> Display Physical Index </summary>
        internal int DisplayPhysicalIndex;
        /// <summary> Adapter Logical Index </summary>
        internal int DisplayLogicalAdapterIndex;
        /// <summary> Adapter Physical Index </summary>
        internal int DisplayPhysicalAdapterIndex;
    }

    /// <summary> ADLDisplayInfo Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLDisplayInfo
    {
        /// <summary> Display Index </summary>
        internal ADLDisplayID DisplayID;
        /// <summary> Display Controller Index </summary>
        internal int DisplayControllerIndex;
        /// <summary> Display Name </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayName;
        /// <summary> Display Manufacturer Name </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayManufacturerName;
        /// <summary> Display Type : < The Display type. CRT, TV,CV,DFP are some of display types,</summary>
        internal int DisplayType;
        /// <summary> Display output type </summary>
        internal int DisplayOutputType;
        /// <summary> Connector type</summary>
        internal int DisplayConnector;
        ///<summary> Indicating the display info bits' mask.<summary>
        internal int DisplayInfoMask;
        ///<summary> Indicating the display info value.<summary>
        internal int DisplayInfoValue;
    }
    #endregion ADLDisplayInfo

    #endregion Export Struct

    #region ADL Class
    /// <summary> ADL Class</summary>
    internal static class ADL
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        #region Internal Constant
        /// <summary> Define the maximum path</summary>
        internal const int ADL_MAX_PATH = 256;
        /// <summary> Define the maximum adapters</summary>
        internal const int ADL_MAX_ADAPTERS = 40 /* 150 */;
        /// <summary> Define the maximum displays</summary>
        internal const int ADL_MAX_DISPLAYS = 40 /* 150 */;
        /// <summary> Define the maximum device name length</summary>
        internal const int ADL_MAX_DEVICENAME = 32;
        /// <summary> Define the successful</summary>
        internal const int ADL_SUCCESS = 0;
        /// <summary> Define the failure</summary>
        internal const int ADL_FAIL = -1;
        /// <summary> Define the driver ok</summary>
        internal const int ADL_DRIVER_OK = 0;
        /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
        internal const int ADL_MAX_GLSYNC_PORTS = 8;
        /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
        internal const int ADL_MAX_GLSYNC_PORT_LEDS = 8;
        /// <summary> Maximum number of ADLMOdes for the adapter </summary>
        internal const int ADL_MAX_NUM_DISPLAYMODES = 1024;

        internal static Dictionary<Type, object> ADLDelegates = new Dictionary<Type, object>();

        #endregion Internal Constant

        #region Export Delegates
        /// <summary> ADL Memory allocation function allows ADL to callback for memory allocation</summary>
        /// <param name="size">input size</param>
        /// <returns> retrun ADL Error Code</returns>
        internal delegate IntPtr ADL_Main_Memory_Alloc(int size);

        // ///// <summary> ADL Create Function to create ADL Data</summary>
        /// <param name="callback">Call back functin pointer which is ised to allocate memeory </param>
        /// <param name="enumConnectedAdapters">If it is 1, then ADL will only retuen the physical exist adapters </param>
        ///// <returns> retrun ADL Error Code</returns>
        internal delegate int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

        internal delegate int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, ref ADL_CONTEXT_HANDLE context);

        /// <summary> ADL Destroy Function to free up ADL Data</summary>
        /// <returns> retrun ADL Error Code</returns>
        internal delegate int ADL_Main_Control_Destroy();

        internal delegate int ADL2_Main_Control_Destroy(ADL_CONTEXT_HANDLE context);

        /// <summary> ADL Function to get the number of adapters</summary>
        /// <param name="numAdapters">return number of adapters</param>
        /// <returns> retrun ADL Error Code</returns>
        internal delegate int ADL2_Adapter_NumberOfAdapters_Get(ADL_CONTEXT_HANDLE context, ref int numAdapters);

        /// <summary> ADL Function to get the GPU adapter information</summary>
        /// <param name="info">return GPU adapter information</param>
        /// <param name="inputSize">the size of the GPU adapter struct</param>
        /// <returns> retrun ADL Error Code</returns>
        internal delegate int ADL2_Adapter_AdapterInfo_Get(ADL_CONTEXT_HANDLE context, IntPtr info, int inputSize);

        /// <summary> Function to determine if the adapter is active or not.</summary>
        /// <remarks>The function is used to check if the adapter associated with iAdapterIndex is active</remarks>  
        /// <param name="adapterIndex"> Adapter Index.</param>
        /// <param name="status"> Status of the adapter. True: Active; False: Dsiabled</param>
        /// <returns>Non zero is successfull</returns> 
        internal delegate int ADL2_Adapter_Active_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, ref int status);

        /// <summary>Get display information based on adapter index</summary>
        /// <param name="adapterIndex">Adapter Index</param>
        /// <param name="numDisplays">return the total number of supported displays</param>
        /// <param name="displayInfoArray">return ADLDisplayInfo Array for supported displays' information</param>
        /// <param name="forceDetect">force detect or not</param>
        /// <returns>return ADL Error Code</returns>
        internal delegate int ADL2_Display_DisplayInfo_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);

        internal delegate int ADL2_Display_Size_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, ref int lpWidth, ref int lpHeight, ref int lpDefaultWidth, ref int lpDefaultHeight, ref int lpMinWidth, ref int lpMinHeight, ref int lpMaxWidth, ref int lpMaxHeight, ref int lpStepWidth, ref int lpStepHeight);

        internal delegate int ADL2_Display_PixelFormat_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, ref int pixelFormat);
        internal delegate int ADL2_Display_PixelFormat_Set(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, int pixelFormat);

        internal delegate int ADL2_Display_ColorDepth_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, ref int colorDepth);
        internal delegate int ADL2_Display_ColorDepth_Set(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, int colorDepth);

        internal delegate int ADL2_Display_DitherState_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, ref int ditherState);
        internal delegate int ADL2_Display_DitherState_Set(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, int ditherState);

        internal delegate int ADL2_Display_HDRState_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, ADLDisplayID displayID, ref int support, ref int enable);
        internal delegate int ADL2_Display_HDRState_Set(ADL_CONTEXT_HANDLE context, int adapterIndex, ADLDisplayID displayID, int enable);

        #endregion Export Delegates

        #region Class ADLImport
        /// <summary> ADLImport class</summary>
        private static class ADLImport
        {
            #region Internal Constant
            /// <summary> Atiadlxx_FileName </summary>
            internal const string Atiadlxx_FileName = "atiadlxx.dll";
            /// <summary> Kernel32_FileName </summary>
            internal const string Kernel32_FileName = "kernel32.dll";
            #endregion Internal Constant

            #region DLLImport
            [DllImport(Kernel32_FileName)]
            internal static extern HMODULE GetModuleHandle(string moduleName);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Main_Control_IsFunctionValid(HMODULE module, string procName);

            [DllImport(Atiadlxx_FileName)]
            internal static extern FARPROC ADL_Main_Control_GetProcAddress(HMODULE module, string procName);

            #endregion DLLImport
        }
        #endregion Class ADLImport

        #region Class ADLCheckLibrary
        /// <summary> ADLCheckLibrary class</summary>
        private class ADLCheckLibrary
        {
            #region Private Members
            private HMODULE ADLLibrary = System.IntPtr.Zero;
            #endregion Private Members

            #region Static Members
            /// <summary> new a private instance</summary>
            private static ADLCheckLibrary ADLCheckLibrary_ = new ADLCheckLibrary();
            #endregion Static Members

            #region Constructor
            /// <summary> Constructor</summary>
            private ADLCheckLibrary()
            {
                try
                {
                    if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(IntPtr.Zero, "ADL_Main_Control_Create"))
                    {
                        ADLLibrary = ADLImport.GetModuleHandle(ADLImport.Atiadlxx_FileName);
                    }
                }
                catch (DllNotFoundException) { }
                catch (EntryPointNotFoundException) { }
                catch (Exception) { }
            }
            #endregion Constructor

            #region Destructor
            /// <summary> Destructor to force calling ADL Destroy function before free up the ADL library</summary>
            ~ADLCheckLibrary()
            {
                if (IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    var del = ADL.GetDelegate<ADL.ADL_Main_Control_Destroy>();
                    if (del != null)
                    {
                        del();
                    }
                }
            }
            #endregion Destructor

            #region Static IsFunctionValid
            /// <summary> Check the import function to see it exists or not</summary>
            /// <param name="functionName"> function name</param>
            /// <returns>return true, if function exists</returns>
            internal static bool IsFunctionValid(string functionName)
            {
                var result = false;
                if (IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(ADLCheckLibrary_.ADLLibrary, functionName))
                    {
                        result = true;
                    }
                }
                return result;
            }
            #endregion Static IsFunctionValid

            #region Static GetProcAddress
            /// <summary> Get the unmanaged function pointer </summary>
            /// <param name="functionName"> function name</param>
            /// <returns>return function pointer, if function exists</returns>
            internal static FARPROC GetProcAddress(string functionName)
            {
                var result = IntPtr.Zero;
                if (IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    result = ADLImport.ADL_Main_Control_GetProcAddress(ADLCheckLibrary_.ADLLibrary, functionName);
                }
                return result;
            }
            #endregion Static GetProcAddress
        }
        #endregion Class ADLCheckLibrary

        #region Export Functions

        #region ADL_Main_Memory_Alloc
        /// <summary> Build in memory allocation function</summary>
        internal static ADL_Main_Memory_Alloc ADL_Main_Memory_Alloc_Func = ADL_Main_Memory_Alloc_;
        /// <summary> Build in memory allocation function</summary>
        /// <param name="size">input size</param>
        /// <returns>return the memory buffer</returns>
        private static IntPtr ADL_Main_Memory_Alloc_(int size)
        {
            IntPtr result = Marshal.AllocCoTaskMem(size);
            return result;
        }
        #endregion ADL_Main_Memory_Alloc

        #region ADL_Main_Memory_Free
        /// <summary> Build in memory free function</summary>
        /// <param name="buffer">input buffer</param>
        internal static void ADL_Main_Memory_Free(IntPtr buffer)
        {
            if (IntPtr.Zero != buffer)
            {
                Marshal.FreeCoTaskMem(buffer);
            }
        }
        #endregion ADL_Main_Memory_Free

        public static T GetDelegate<T>() where T : class
        {
            var type = typeof(T);

            lock (ADLDelegates)
            {
                if (ADLDelegates.ContainsKey(type))
                {
                    return ADLDelegates[type] as T;
                }

                var ptr = ADLCheckLibrary.GetProcAddress(type.Name);
                if (ptr != FARPROC.Zero)
                {
                    var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, type);
                    ADLDelegates.Add(type, delegateValue);

                    return delegateValue as T;
                }
            }

            Logger.Error($"ADL-function not supported: {type.Name}");

            return null;
            //throw new InvalidProgramException("ADL-function not supported");
        }

        #endregion Export Functions
    }
    #endregion ADL Class
}

#endregion ATI.ADL