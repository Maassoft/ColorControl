using ColorControl.Shared.Contracts;
using ColorControl.UI.Services;

namespace ColorControl.UI;

public static class Blazor
{
    public static void Start(Config? config = null)
    {
        var builder = WebApplication.CreateBuilder();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddSingleton<AppState>(new AppState { SelectedTheme = (config?.UseDarkMode ?? true) ? "dark" : "light" });
        builder.Services.AddTransient<RpcUiClientService>();
        builder.Services.AddTransient<JSHelper>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<NotificationService>();
        builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(config?.UiPort > 0 ? config.UiPort : 5000));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
