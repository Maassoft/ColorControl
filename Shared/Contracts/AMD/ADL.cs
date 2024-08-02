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

using System.Runtime.InteropServices;
using ADL_CONTEXT_HANDLE = nint;
using FARPROC = nint;
using HMODULE = nint;

#endregion Using

#region ATI.ADL

namespace ATI.ADL;

#region Export Struct

#region ADLAdapterInfo
/// <summary> ADLAdapterInfo Structure</summary>
[StructLayout(LayoutKind.Sequential)]
public struct ADLAdapterInfo
{
	/// <summary>The size of the structure</summary>
	int Size;
	/// <summary> Adapter Index</summary>
	public int AdapterIndex;
	/// <summary> Adapter UDID</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
	public string UDID;
	/// <summary> Adapter Bus Number</summary>
	public int BusNumber;
	/// <summary> Adapter Driver Number</summary>
	public int DriverNumber;
	/// <summary> Adapter Function Number</summary>
	public int FunctionNumber;
	/// <summary> Adapter Vendor ID</summary>
	public int VendorID;
	/// <summary> Adapter Adapter name</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
	public string AdapterName;
	/// <summary> Adapter Display name</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
	public string DisplayName;
	/// <summary> Adapter Present status</summary>
	public int Present;
	/// <summary> Adapter Exist status</summary>
	public int Exist;
	/// <summary> Adapter Driver Path</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
	public string DriverPath;
	/// <summary> Adapter Driver Ext Path</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
	public string DriverPathExt;
	/// <summary> Adapter PNP String</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
	public string PNPString;
	/// <summary> OS Display Index</summary>
	public int OSDisplayIndex;
}


/// <summary> ADLAdapterInfo Array</summary>
[StructLayout(LayoutKind.Sequential)]
public struct ADLAdapterInfoArray
{
	/// <summary> ADLAdapterInfo Array </summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = ADL.ADL_MAX_ADAPTERS)]
	public ADLAdapterInfo[] ADLAdapterInfo;
}
#endregion ADLAdapterInfo


#region ADLDisplayInfo
/// <summary> ADLDisplayID Structure</summary>
[StructLayout(LayoutKind.Sequential)]
public struct ADLDisplayID
{
	/// <summary> Display Logical Index </summary>
	public int DisplayLogicalIndex;
	/// <summary> Display Physical Index </summary>
	public int DisplayPhysicalIndex;
	/// <summary> Adapter Logical Index </summary>
	public int DisplayLogicalAdapterIndex;
	/// <summary> Adapter Physical Index </summary>
	public int DisplayPhysicalAdapterIndex;
}

/// <summary> ADLDisplayInfo Structure</summary>
[StructLayout(LayoutKind.Sequential)]
public struct ADLDisplayInfo
{
	/// <summary> Display Index </summary>
	public ADLDisplayID DisplayID;
	/// <summary> Display Controller Index </summary>
	public int DisplayControllerIndex;
	/// <summary> Display Name </summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
	public string DisplayName;
	/// <summary> Display Manufacturer Name </summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
	public string DisplayManufacturerName;
	/// <summary> Display Type : < The Display type. CRT, TV,CV,DFP are some of display types,</summary>
	public int DisplayType;
	/// <summary> Display output type </summary>
	public int DisplayOutputType;
	/// <summary> Connector type</summary>
	public int DisplayConnector;
	///<summary> Indicating the display info bits' mask.<summary>
	public int DisplayInfoMask;
	///<summary> Indicating the display info value.<summary>
	public int DisplayInfoValue;
}
#endregion ADLDisplayInfo

#endregion Export Struct

#region ADL Class
/// <summary> ADL Class</summary>
public static class ADL
{
	private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

	#region public Constant
	/// <summary> Define the maximum path</summary>
	public const int ADL_MAX_PATH = 256;
	/// <summary> Define the maximum adapters</summary>
	public const int ADL_MAX_ADAPTERS = 40 /* 150 */;
	/// <summary> Define the maximum displays</summary>
	public const int ADL_MAX_DISPLAYS = 40 /* 150 */;
	/// <summary> Define the maximum device name length</summary>
	public const int ADL_MAX_DEVICENAME = 32;
	/// <summary> Define the successful</summary>
	public const int ADL_SUCCESS = 0;
	/// <summary> Define the failure</summary>
	public const int ADL_FAIL = -1;
	/// <summary> Define the driver ok</summary>
	public const int ADL_DRIVER_OK = 0;
	/// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
	public const int ADL_MAX_GLSYNC_PORTS = 8;
	/// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
	public const int ADL_MAX_GLSYNC_PORT_LEDS = 8;
	/// <summary> Maximum number of ADLMOdes for the adapter </summary>
	public const int ADL_MAX_NUM_DISPLAYMODES = 1024;

	public static Dictionary<Type, object> ADLDelegates = new Dictionary<Type, object>();

	#endregion public Constant

	#region Export Delegates
	/// <summary> ADL Memory allocation function allows ADL to callback for memory allocation</summary>
	/// <param name="size">input size</param>
	/// <returns> retrun ADL Error Code</returns>
	public delegate HMODULE ADL_Main_Memory_Alloc(int size);

	// ///// <summary> ADL Create Function to create ADL Data</summary>
	/// <param name="callback">Call back functin pointer which is ised to allocate memeory </param>
	/// <param name="enumConnectedAdapters">If it is 1, then ADL will only retuen the physical exist adapters </param>
	///// <returns> retrun ADL Error Code</returns>
	public delegate int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

	public delegate int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, ref ADL_CONTEXT_HANDLE context);

	/// <summary> ADL Destroy Function to free up ADL Data</summary>
	/// <returns> retrun ADL Error Code</returns>
	public delegate int ADL_Main_Control_Destroy();

	public delegate int ADL2_Main_Control_Destroy(ADL_CONTEXT_HANDLE context);

	/// <summary> ADL Function to get the number of adapters</summary>
	/// <param name="numAdapters">return number of adapters</param>
	/// <returns> retrun ADL Error Code</returns>
	public delegate int ADL2_Adapter_NumberOfAdapters_Get(ADL_CONTEXT_HANDLE context, ref int numAdapters);

	/// <summary> ADL Function to get the GPU adapter information</summary>
	/// <param name="info">return GPU adapter information</param>
	/// <param name="inputSize">the size of the GPU adapter struct</param>
	/// <returns> retrun ADL Error Code</returns>
	public delegate int ADL2_Adapter_AdapterInfo_Get(ADL_CONTEXT_HANDLE context, HMODULE info, int inputSize);

	/// <summary> Function to determine if the adapter is active or not.</summary>
	/// <remarks>The function is used to check if the adapter associated with iAdapterIndex is active</remarks>  
	/// <param name="adapterIndex"> Adapter Index.</param>
	/// <param name="status"> Status of the adapter. True: Active; False: Dsiabled</param>
	/// <returns>Non zero is successfull</returns> 
	public delegate int ADL2_Adapter_Active_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, ref int status);

	/// <summary>Get display information based on adapter index</summary>
	/// <param name="adapterIndex">Adapter Index</param>
	/// <param name="numDisplays">return the total number of supported displays</param>
	/// <param name="displayInfoArray">return ADLDisplayInfo Array for supported displays' information</param>
	/// <param name="forceDetect">force detect or not</param>
	/// <returns>return ADL Error Code</returns>
	public delegate int ADL2_Display_DisplayInfo_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, ref int numDisplays, out HMODULE displayInfoArray, int forceDetect);

	public delegate int ADL2_Display_Size_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, ref int lpWidth, ref int lpHeight, ref int lpDefaultWidth, ref int lpDefaultHeight, ref int lpMinWidth, ref int lpMinHeight, ref int lpMaxWidth, ref int lpMaxHeight, ref int lpStepWidth, ref int lpStepHeight);

	public delegate int ADL2_Display_PixelFormat_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, ref int pixelFormat);
	public delegate int ADL2_Display_PixelFormat_Set(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, int pixelFormat);

	public delegate int ADL2_Display_ColorDepth_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, ref int colorDepth);
	public delegate int ADL2_Display_ColorDepth_Set(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, int colorDepth);

	public delegate int ADL2_Display_DitherState_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, ref int ditherState);
	public delegate int ADL2_Display_DitherState_Set(ADL_CONTEXT_HANDLE context, int adapterIndex, int displayIndex, int ditherState);

	public delegate int ADL2_Display_HDRState_Get(ADL_CONTEXT_HANDLE context, int adapterIndex, ADLDisplayID displayID, ref int support, ref int enable);
	public delegate int ADL2_Display_HDRState_Set(ADL_CONTEXT_HANDLE context, int adapterIndex, ADLDisplayID displayID, int enable);

	#endregion Export Delegates

	#region Class ADLImport
	/// <summary> ADLImport class</summary>
	private static class ADLImport
	{
		#region public Constant
		/// <summary> Atiadlxx_FileName </summary>
		public const string Atiadlxx_FileName = "atiadlxx.dll";
		/// <summary> Kernel32_FileName </summary>
		public const string Kernel32_FileName = "kernel32.dll";
		#endregion public Constant

		#region DLLImport
		[DllImport(Kernel32_FileName)]
		public static extern HMODULE GetModuleHandle(string moduleName);

		[DllImport(Atiadlxx_FileName)]
		public static extern int ADL_Main_Control_IsFunctionValid(HMODULE module, string procName);

		[DllImport(Atiadlxx_FileName)]
		public static extern FARPROC ADL_Main_Control_GetProcAddress(HMODULE module, string procName);

		#endregion DLLImport
	}
	#endregion Class ADLImport

	#region Class ADLCheckLibrary
	/// <summary> ADLCheckLibrary class</summary>
	private class ADLCheckLibrary
	{
		#region Private Members
		private HMODULE ADLLibrary = HMODULE.Zero;
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
				if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(HMODULE.Zero, "ADL_Main_Control_Create"))
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
			if (HMODULE.Zero != ADLCheckLibrary_.ADLLibrary)
			{
				var del = GetDelegate<ADL_Main_Control_Destroy>();
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
		public static bool IsFunctionValid(string functionName)
		{
			var result = false;
			if (HMODULE.Zero != ADLCheckLibrary_.ADLLibrary)
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
		public static FARPROC GetProcAddress(string functionName)
		{
			var result = HMODULE.Zero;
			if (HMODULE.Zero != ADLCheckLibrary_.ADLLibrary)
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
	public static ADL_Main_Memory_Alloc ADL_Main_Memory_Alloc_Func = ADL_Main_Memory_Alloc_;
	/// <summary> Build in memory allocation function</summary>
	/// <param name="size">input size</param>
	/// <returns>return the memory buffer</returns>
	private static HMODULE ADL_Main_Memory_Alloc_(int size)
	{
		HMODULE result = Marshal.AllocCoTaskMem(size);
		return result;
	}
	#endregion ADL_Main_Memory_Alloc

	#region ADL_Main_Memory_Free
	/// <summary> Build in memory free function</summary>
	/// <param name="buffer">input buffer</param>
	public static void ADL_Main_Memory_Free(HMODULE buffer)
	{
		if (HMODULE.Zero != buffer)
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

#endregion ATI.ADL