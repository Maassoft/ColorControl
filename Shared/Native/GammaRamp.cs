using System.Runtime.InteropServices;

namespace ColorControl.Shared.Native;

public static class GammaRamp
{
	private const int Size = 256;

	public const double MinGamma = 0.30;
	public const double MaxGamma = 2.80;

	[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr CreateDC(string driver, string device, string output, IntPtr initData);

	[DllImport("gdi32.dll")]
	private static extern bool DeleteDC(IntPtr hdc);

	[DllImport("gdi32.dll", SetLastError = true)]
	private static extern bool SetDeviceGammaRamp(IntPtr hdc, ushort[] ramp);

	// Brightness and contrast are percentages with 50 as neutral, same as the NVIDIA Control Panel
	public static ushort[] Build(int brightness, int contrast, double gamma)
	{
		var ramp = new ushort[Size * 3];

		var offset = (brightness - 50) / 100.0;
		var c = (contrast - 50) / 100.0;
		var slope = c >= 0 ? 1 / (1 - c) : 1 + c;
		var exponent = 1 / Math.Clamp(gamma, MinGamma, MaxGamma);

		for (var i = 0; i < Size; i++)
		{
			var x = i / (double)(Size - 1);

			var value = (x - 0.5) * slope + 0.5 + offset;
			value = Math.Pow(Math.Clamp(value, 0, 1), exponent);

			var entry = (ushort)Math.Round(value * ushort.MaxValue);

			ramp[i] = entry;
			ramp[Size + i] = entry;
			ramp[Size * 2 + i] = entry;
		}

		return ramp;
	}

	public static bool Apply(string displayName, ushort[] ramp)
	{
		var hdc = CreateDC(displayName, null, null, IntPtr.Zero);
		if (hdc == IntPtr.Zero)
		{
			return false;
		}

		try
		{
			return SetDeviceGammaRamp(hdc, ramp);
		}
		finally
		{
			DeleteDC(hdc);
		}
	}
}
