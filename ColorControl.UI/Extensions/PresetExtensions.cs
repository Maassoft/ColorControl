using ColorControl.Shared.Contracts;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Cryptography;

namespace ColorControl.UI.Generics;

public static class PresetExtensions
{
	public static List<T>? OrderPresetsBy<T>(this List<T> presets, PresetOrder order) where T : PresetBase
	{
		if (presets == null)
		{
			return null;
		}

		if (order == PresetOrder.ByName)
		{
			presets = presets.OrderBy(p => p.name).ToList();
		}
		else if (order == PresetOrder.ByLastUsed)
		{
			presets = presets.OrderByDescending(p => p.LastUsed).ToList();
		}

		return presets;
	}

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
