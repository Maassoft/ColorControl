using ColorControl.Shared.Services;

namespace ColorControl.UI;

public static class Blazor
{
    public static void Start()
    {
        var builder = WebApplication.CreateBuilder();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddSingleton<AppState>();
        builder.Services.AddSingleton<OptionsService>();

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
