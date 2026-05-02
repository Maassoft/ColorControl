using Microsoft.AspNetCore.Components;

namespace ColorControl.UI.Pages.Generic;

public abstract class CustomComponentBase : ComponentBase, IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        Blazor.ClearCurrentComponent(this);
    }

    protected override Task OnInitializedAsync()
    {
        Blazor.SetCurrentComponent(this);

        return Task.CompletedTask;
    }
}