using EDIDParser;
using Microsoft.Win32;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using System.Runtime.InteropServices;

/*
no NDAs violated here!
everything figured out from publicly available information and/or
throwing stuff at NVAPI and observing the results or errors
*/

namespace novideo_srgb
{
    public static class Novideo
    {
        /*
        observed pipeline: content degamma -> content to srgb -> srgb to monitor -> matrix2 -> matrix1 -> monitor gamma
        in hardware all the matrix stuff is done with a single matrix, i.e. these four multiplied together
        3x4 matrices cannot be multiplied with each other though, so only the 3x3 parts are combined "properly"
        and the offsets are simply added together
        */
        [StructLayout(LayoutKind.Sequential)]
        private struct Csc
        {
            public uint version; // 0x1007C for V1, 0x200A0 for V2

            public uint
                contentColorSpace; // built-in degamut/degamma transforms, 1 <= x <= 12, default 2 (probably srgb)

            public uint monitorColorSpace; // built-in gamut/gamma transforms, 0 <= x <= 12, default 0 (= csc disabled)
            public uint unknown1; // no idea, set to 0 by both get and set functions -> some type of error code?
            public uint unknown2; // also no idea, not modified by either function -> unused?
            public uint useMatrix1; // 1 to enable
            public unsafe fixed float matrix1[3 * 4]; // r/g/b gain and offset
            public uint useMatrix2;

            public unsafe fixed float matrix2[3 * 4];

            // v2 stuff
            public unsafe float* degamma; // pointer to degamma part of buffer (= first element)
            public unsafe float* regamma; // pointer to regamma part of buffer (= index 0x3000)

            public unsafe float*
                buffer; // float array of size 0x6000, contains interleaved rgb degamma followed by regamma

            public int bufferSize; // 0x6000
        }

        private const uint _NvAPI_GPU_GetColorSpaceConversion = 0x8159E87A;
        private const uint _NvAPI_GPU_SetColorSpaceConversion = 0x0FCABD23A;

        [DllImport("nvapi64", EntryPoint = "nvapi_QueryInterface")]
        private static extern IntPtr NvAPI_QueryInterface(uint id);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NvAPI_GPU_GetColorSpaceConversion_t(uint displayId,
            [MarshalAs(UnmanagedType.Struct)] ref Csc csc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NvAPI_GPU_SetColorSpaceConversion_t(uint displayId,
            [MarshalAs(UnmanagedType.Struct)] ref Csc csc);

        private static NvAPI_GPU_GetColorSpaceConversion_t NvAPI_GPU_GetColorSpaceConversion;
        private static NvAPI_GPU_SetColorSpaceConversion_t NvAPI_GPU_SetColorSpaceConversion;

        public struct ColorSpaceConversion
        {
            public uint contentColorSpace;
            public uint monitorColorSpace;
            public float[,] matrix1;
            public float[,] matrix2;
        }

        public static ColorSpaceConversion GetColorSpaceConversion(GPUOutput output)
        {
            var displayId = output.PhysicalGPU.GetDisplayDeviceByOutput(output).DisplayId;

            var csc = new Csc { version = 0x1007C };
            var status = NvAPI_GPU_GetColorSpaceConversion(displayId, ref csc);
            if (status != 0)
            {
                throw new Exception("NvAPI_GPU_GetColorSpaceConversion failed with error code " + status);
            }

            var result = new ColorSpaceConversion
            {
                contentColorSpace = csc.contentColorSpace,
                monitorColorSpace = csc.monitorColorSpace
            };

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    unsafe
                    {
                        if (csc.useMatrix1 == 1)
                        {
                            if (result.matrix1 == null) result.matrix1 = new float[3, 4];
                            result.matrix1[i, j] = csc.matrix1[i * 4 + j];
                        }

                        if (csc.useMatrix2 == 1)
                        {
                            if (result.matrix2 == null) result.matrix2 = new float[3, 4];
                            result.matrix2[i, j] = csc.matrix2[i * 4 + j];
                        }
                    }
                }
            }

            return result;
        }

        public static void SetColorSpaceConversion(GPUOutput output, ColorSpaceConversion conversion)
        {
            var displayId = output.PhysicalGPU.GetDisplayDeviceByOutput(output).DisplayId;

            var csc = new Csc
            {
                version = 0x1007C,
                contentColorSpace = conversion.contentColorSpace,
                monitorColorSpace = conversion.monitorColorSpace
            };

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    unsafe
                    {
                        if (conversion.matrix1 != null)
                        {
                            csc.useMatrix1 = 1;
                            csc.matrix1[i * 4 + j] = conversion.matrix1[i, j];
                        }

                        if (conversion.matrix2 != null)
                        {
                            csc.useMatrix2 = 1;
                            csc.matrix2[i * 4 + j] = conversion.matrix2[i, j];
                        }
                    }
                }
            }

            var status = NvAPI_GPU_SetColorSpaceConversion(displayId, ref csc);
            if (status != 0)
            {
                throw new Exception("NvAPI_GPU_SetColorSpaceConversion failed with error code " + status);
            }
        }

        public static void SetColorSpaceConversion(GPUOutput output, Matrix matrix)
        {
            SetColorSpaceConversion(output, MatrixToColorSpaceConversion(matrix));
        }

        public static unsafe void SetColorSpaceConversion(GPUOutput output, ICCMatrixProfile profile,
            Colorimetry.ColorSpace target,
            ToneCurve curve = null,
            bool disableOptimization = false)
        {
            var matrix = profile.matrix.Inverse() * Colorimetry.RGBToPCSXYZ(target);

            if (curve == null)
            {
                SetColorSpaceConversion(output, MatrixToColorSpaceConversion(matrix));
                return;
            }

            var displayId = output.PhysicalGPU.GetDisplayDeviceByOutput(output).DisplayId;
            var gamma = new float[2, 1024, 3];
            fixed (float* buffer = gamma)
            {
                var csc = new Csc
                {
                    version = 0x200A0,
                    contentColorSpace = 2,
                    monitorColorSpace = 2,
                    degamma = buffer,
                    regamma = buffer + 0x3000 / sizeof(float),
                    buffer = buffer,
                    bufferSize = 0x6000,
                };

                double nextIndex = -1;
                for (var i = 1; i < 1024; i++)
                {
                    var index = i / 1023d;

                    if (!disableOptimization)
                    {
                        var curr = i * 255 % 1023;
                        var next = (i + 1) * 255 % 1023;

                        if (nextIndex != -1)
                        {
                            index = nextIndex;
                            nextIndex = -1;
                        }
                        else if (next < curr)
                        {
                            nextIndex = (i + 1) * 255 / 1023 / 255d;
                            if (next != 0)
                            {
                                index = nextIndex;
                            }
                        }
                    }

                    var sample = (float)curve.SampleAt(index);

                    for (var j = 0; j < 3; j++)
                    {
                        gamma[0, i, j] = sample;
                    }
                }

                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        csc.matrix1[i * 4 + j] = (float)matrix[i, j];
                    }
                }

                csc.useMatrix1 = 1;

                for (var i = 0; i < 1024; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        var value = profile.trcs[j].SampleInverseAt(i / 1023d);

                        if (profile.vcgt != null)
                        {
                            value = profile.vcgt[j].SampleAt(value);
                        }

                        gamma[1, i, j] = (float)value;
                    }
                }

                var status = NvAPI_GPU_SetColorSpaceConversion(displayId, ref csc);
                if (status != 0)
                {
                    throw new Exception("NvAPI_GPU_SetColorSpaceConversion failed with error code " + status);
                }
            }
        }

        public static bool IsColorSpaceConversionActive(GPUOutput output)
        {
            return GetColorSpaceConversion(output).monitorColorSpace != 0;
        }

        public static void DisableColorSpaceConversion(GPUOutput output)
        {
            SetColorSpaceConversion(output, new ColorSpaceConversion { contentColorSpace = 2 });
        }

        public static EDID GetEDID(string path, Display display)
        {
            try
            {
                var registryPath = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Enum\\DISPLAY\\";
                registryPath += string.Join("\\", path.Split('#').Skip(1).Take(2));
                return new EDID((byte[])Registry.GetValue(registryPath + "\\Device Parameters", "EDID", null));
            }
            catch
            {
                return new EDID(display.Output.PhysicalGPU.ReadEDIDData(display.Output));
            }
        }

        private static ColorSpaceConversion MatrixToColorSpaceConversion(Matrix matrix)
        {
            var csc = new ColorSpaceConversion
            {
                contentColorSpace = 2,
                monitorColorSpace = 2,
                matrix1 = new float[3, 4]
            };

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    csc.matrix1[i, j] = (float)matrix[i, j];
                }
            }

            return csc;
        }

        static Novideo()
        {
            NvAPI_GPU_GetColorSpaceConversion =
                Marshal.GetDelegateForFunctionPointer<NvAPI_GPU_GetColorSpaceConversion_t>(
                    NvAPI_QueryInterface(_NvAPI_GPU_GetColorSpaceConversion));
            NvAPI_GPU_SetColorSpaceConversion =
                Marshal.GetDelegateForFunctionPointer<NvAPI_GPU_SetColorSpaceConversion_t>(
                    NvAPI_QueryInterface(_NvAPI_GPU_SetColorSpaceConversion));
        }
    }
}