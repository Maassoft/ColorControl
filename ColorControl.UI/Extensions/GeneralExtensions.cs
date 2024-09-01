using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Cryptography;

namespace ColorControl.UI.Generics;

public static class GeneralExtensions
{
    public static async ValueTask<TValue?> TryGet<TValue>(this ProtectedBrowserStorage storage, string key, TValue? def = default)
    {
        try
        {
            var result = await storage.GetAsync<TValue>(key);
            var value = result.Success ? result.Value : def;

            return value;
        }
        catch (CryptographicException)
        {
            await storage.DeleteAsync(key);
            return def;
        }
    }
}
