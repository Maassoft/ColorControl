namespace ColorControl.UI.Pages.Generic;

public abstract class SummaryComponentBase : CustomComponentBase
{
    protected abstract Task Refresh();

    public async Task RefreshExternal()
    {
        await Refresh();

        await InvokeAsync(StateHasChanged);
    }
}