using ColorControl.Shared.Contracts;

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
}
