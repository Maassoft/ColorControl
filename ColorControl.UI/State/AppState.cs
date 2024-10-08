﻿namespace ColorControl.UI;

public class AppState
{
    public string SelectedTheme { get; set; } = "light";

    public event Action? OnChange;

    public void SetTheme(string theme)
    {
        SelectedTheme = theme;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
