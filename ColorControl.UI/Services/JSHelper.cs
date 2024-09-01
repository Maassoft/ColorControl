using Microsoft.JSInterop;

namespace ColorControl.UI.Services;

public class JSHelper(IJSRuntime jsRuntime)
{
    private IJSObjectReference? jsModule;

    public async Task<bool> IsFormValid(string id)
    {
        return await InvokeAsync<bool>("FormValid", id);
    }

    public async Task OpenModal(string id)
    {
        await InvokeVoidAsync("OpenModal", id);
    }

    public async Task CloseModal(string id)
    {
        await InvokeVoidAsync("CloseModal", id);
    }

    public async Task ShowToast(string id)
    {
        await InvokeVoidAsync("ShowToast", id);
    }

    private async ValueTask<T> InvokeAsync<T>(string identifier, params object?[]? names) where T : struct
    {
        jsModule = await LoadModule();

        return await jsModule.InvokeAsync<T>(identifier, names);
    }

    private async ValueTask InvokeVoidAsync(string identifier, params object?[]? names)
    {
        jsModule = await LoadModule();

        await jsModule.InvokeVoidAsync(identifier, names);
    }

    private async Task<IJSObjectReference> LoadModule()
    {
        return jsModule ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./helper.js");
    }
}