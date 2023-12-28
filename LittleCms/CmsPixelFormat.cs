using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LittleCms
{
    public struct CmsPixelFormat
    {
        public uint Value;

        public static implicit operator uint(CmsPixelFormat x) => x.Value;

        // boolean fields
        const int shift_IsPremultipliedAlpha = 23;
        const int shift_IsFloatingPoint = 22;
        const int shift_IsOptimized = 21;
        const int shift_RotateFirstChannel = 14;
        const int shift_IsInvertedIntensity = 13;
        const int shift_IsPlanar = 12;
        const int shift_IsWordOrderSwapped = 11;
        const int shift_IsByteOrderSwapped = 10;

        // enum fields
        const uint mask_PixelType = 0b11111u;
        const int shift_PixelType = 16;

        // uint fields
        const uint mask_ExtraChannels = 0b111u;
        const int shift_ExtraChannels = 7;
        const uint mask_Channels = 0b1111u;
        const int shift_Channels = 3;
        const uint mask_BytesPerChannel = 0b111;
        const int shift_BytesPerChannel = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetBit(int shift)
        {
            var mask = 1u << shift;
            return (Value & mask) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBit(int shift, bool value)
        {
            var mask = 1u << shift;
            Value = (Value & (~mask)) | (value ? mask : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint GetBits(int shift, uint mask) => (Value >> shift) & mask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBits(int shift, uint mask, uint value)
        {
            var smask = mask << shift;
            value = (value & mask) << shift;
            Value = (Value & (~smask)) | value;
        }


        public bool IsPremultipliedAlpha
        {
            get => GetBit(shift_IsPremultipliedAlpha);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBit(shift_IsPremultipliedAlpha, value);
        }

        public bool IsFloatingPoint
        {
            get => GetBit(shift_IsFloatingPoint);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBit(shift_IsFloatingPoint, value);
        }
        public bool IsOptimized
        {
            get => GetBit(shift_IsOptimized);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBit(shift_IsOptimized, value);
        }
        public bool RotateFirstChannel
        {
            get => GetBit(shift_RotateFirstChannel);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBit(shift_RotateFirstChannel, value);
        }
        public bool IsPlanar
        {
            get => GetBit(shift_IsPlanar);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBit(shift_IsPlanar, value);
        }
        public bool IsInvertedIntensity
        {
            get => GetBit(shift_IsInvertedIntensity);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBit(shift_IsInvertedIntensity, value);
        }
        public bool IsWordOrderSwapped
        {
            get => GetBit(shift_IsWordOrderSwapped);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBit(shift_IsWordOrderSwapped, value);
        }
        public bool IsByteOrderSwapped
        {
            get => GetBit(shift_IsByteOrderSwapped);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBit(shift_IsByteOrderSwapped, value);
        }

        public PixelType PixelType
        {
            get => (PixelType)GetBits(shift_PixelType, mask_PixelType);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBits(shift_PixelType, mask_PixelType, (uint)value);
        }
        public int ExtraChannels
        {
            get => (int)GetBits(shift_ExtraChannels, mask_ExtraChannels);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBits(shift_ExtraChannels, mask_ExtraChannels, (uint)value);
        }
        public int Channels
        {
            get => (int)GetBits(shift_Channels, mask_Channels);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBits(shift_Channels, mask_Channels, (uint)value);
        }
        /// <summary>
        /// use 0 for double-precision IEEE-754 floating point.
        /// </summary>
        public int BytesPerChannel
        {
            get => (int)GetBits(shift_BytesPerChannel, mask_BytesPerChannel);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetBits(shift_BytesPerChannel, mask_BytesPerChannel, (uint)value);
        }


        public static CmsPixelFormat RGB8 => new CmsPixelFormat { PixelType = PixelType.RGB, Channels = 3, BytesPerChannel = 1 };
        public static CmsPixelFormat RGBDouble => new CmsPixelFormat { PixelType = PixelType.RGB, Channels = 3, BytesPerChannel = 0, IsFloatingPoint = true };
        public static CmsPixelFormat XYZDouble => new CmsPixelFormat { PixelType = PixelType.XYZ, Channels = 3, BytesPerChannel = 0, IsFloatingPoint = true };
    }
}
