using LittleCms;
using System;
using System.IO;

namespace MHC2Gen;

public class MHC2Wrapper
{
    public enum NamedGamut
    {
        sRGB,
        AdobeRGB,
        P3D65,
        BT2020
    }

    public static byte[] GenerateSdrAcmProfile(GenerateProfileCommand command)
    {
        var profile = IccProfile.Create_sRGB();

        var context = new DeviceIccContext(profile);

        var newProfile = context.CreateIcc(command);

        return newProfile.GetBytes();

    }

    public static (double MinNits, double MaxNits) GetMinMaxLuminance(string profileName)
    {
        var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), $@"System32\spool\drivers\color\{profileName}");

        var bytes = File.ReadAllBytes(fileName);

        var profile = IccProfile.Open(bytes.AsSpan());

        var deviceContext = new DeviceIccContext(profile);

        return (deviceContext.min_nits, deviceContext.max_nits);
    }

    public static GenerateProfileCommand LoadProfile(string fileName, bool isHDRProfile)
    {
        if (fileName.IndexOf("\\") == -1 || !File.Exists(fileName))
        {
            fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), $@"System32\spool\drivers\color\{fileName}");
        }

        var bytes = File.ReadAllBytes(fileName);

        var profile = IccProfile.Open(bytes.AsSpan());

        var deviceContext = new DeviceIccContext(profile);

        return new GenerateProfileCommand
        {
            IsHDRProfile = isHDRProfile,
            BlackLuminance = deviceContext.min_nits,
            WhiteLuminance = deviceContext.max_nits,
            ColorGamut = deviceContext.ExtraInfoTag?.TargetGamut ?? ColorGamut.Native,
            DevicePrimaries = new RgbPrimaries(deviceContext.ProfilePrimaries.Red, deviceContext.ProfilePrimaries.Green, deviceContext.ProfilePrimaries.Blue, deviceContext.ProfilePrimaries.White),
            MinCLL = deviceContext.MHC2?.MinCLL ?? deviceContext.min_nits,
            MaxCLL = deviceContext.MHC2?.MaxCLL ?? deviceContext.max_nits,
            SDRMinBrightness = deviceContext.ExtraInfoTag?.SDRMinBrightness ?? 0,
            SDRMaxBrightness = deviceContext.ExtraInfoTag?.SDRMaxBrightness ?? 100,
            SDRTransferFunction = deviceContext.ExtraInfoTag?.SDRTransferFunction ?? SDRTransferFunction.PurePower,
            SDRBrightnessBoost = deviceContext.ExtraInfoTag?.SDRBrightnessBoost ?? 0,
            Gamma = deviceContext.ExtraInfoTag?.Gamma ?? 2.2
        };
    }
}
