﻿using System.Runtime.InteropServices;

namespace nspector.Native
{
    internal class NativeArrayHelper
    {
        public static T GetArrayItemData<T>(IntPtr sourcePointer)
        {
            return (T)Marshal.PtrToStructure(sourcePointer, typeof(T));
        }

        public static T[] GetArrayData<T>(IntPtr sourcePointer, int itemCount)
        {
            var lstResult = new T[itemCount];
            if (sourcePointer != IntPtr.Zero && itemCount > 0)
            {
                var sizeOfItem = Marshal.SizeOf(typeof(T));
                for (int i = 0; i < itemCount; i++)
                {
                    lstResult[i] = GetArrayItemData<T>(sourcePointer + (sizeOfItem * i));
                }
            }
            return lstResult;
        }

        public static void SetArrayData<T>(T[] items, out IntPtr targetPointer)
        {
            if (items != null && items.Length > 0)
            {
                var sizeOfItem = Marshal.SizeOf(typeof(T));
                targetPointer = Marshal.AllocHGlobal(sizeOfItem * items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    Marshal.StructureToPtr(items[i], targetPointer + (sizeOfItem * i), true);
                }
            }
            else
            {
                targetPointer = IntPtr.Zero;
            }
        }

        public static unsafe void SetArrayDataNative<T>(T[] items, out IntPtr targetPointer)
        {
            if (items != null && items.Length > 0)
            {
                var sizeOfItem = Marshal.SizeOf(typeof(T));
                targetPointer = Marshal.AllocHGlobal(sizeOfItem * items.Length);

                new Span<T>(items, 0, items.Length).CopyTo(new Span<T>((void*)targetPointer, items.Length));
            }
            else
            {
                targetPointer = IntPtr.Zero;
            }
        }
    }
}
