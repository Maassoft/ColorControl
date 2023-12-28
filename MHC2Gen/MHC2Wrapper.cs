using LittleCms;

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
}
