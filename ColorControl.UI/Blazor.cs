using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.UI.Pages.Generic;
using ColorControl.UI.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using NStandard;
using System.Net;

namespace ColorControl.UI;

public static class Blazor
{
    private static WebApplication? CurrentApplication;

    public static CustomComponentBase? CurrentComponent { get; private set; }

    public static async Task Start(string[] args)
    {
        var mutexId = args.GetValueOrDefault(0);
        var useDarkMode = args.GetValueOrDefault(1) == "True";
        var uiPort = Utils.ParseInt(args.GetValueOrDefault(2));
        var allowRemoteConnections = args.GetValueOrDefault(3) == "True";

        await Stop();

        var builder = WebApplication.CreateBuilder();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddSingleton(new AppState { SelectedTheme = useDarkMode ? "dark" : "light" });
        builder.Services.AddTransient<RpcUiClientService>();
        builder.Services.AddTransient<JSHelper>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<NotificationService>();

        builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(3));

        var listenPort = uiPort > 0 ? uiPort : 0;

        builder.WebHost.ConfigureKestrel(options =>
        {
            if (allowRemoteConnections)
            {
                options.ListenAnyIP(listenPort);
            }
            else
            {
                options.Listen(IPAddress.Parse("127.0.0.1"), listenPort);
            }
        });

        builder.WebHost.UseStaticWebAssets();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        //app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        //app.UsePathBase("/");

        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();

        CurrentApplication = app;

        var _ = Task.Run(async () =>
        {
            await Task.Delay(500);

            var port = GetCurrentPort();

            if (port == -1)
            {
                return;
            }

            var rpcServer = app.Services.GetRequiredService<RpcUiClientService>();

            await rpcServer.CallAsync<bool>("OptionsService", "SetCurrentUiSettings", port, allowRemoteConnections);

            if (!mutexId.IsNullOrWhiteSpace())
            {
                Console.WriteLine("Waiting for mutex: " + mutexId);
                var mutex = new Mutex(false, mutexId, out var createdNew);
                if (createdNew)
                {
                    Console.WriteLine("Mutex was created, skipping.");
                    return;
                }
                try
                {
                    mutex.WaitOne();
                    Console.WriteLine("Waiting done.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Wait completed: " + ex.Message);
                }

                Environment.Exit(0);
            }
        });

        await app.RunAsync();
    }

    public static async Task Stop()
    {
        if (CurrentApplication != null)
        {
            await CurrentApplication.StopAsync();
            CurrentApplication = null;
        }
    }

    public static int GetCurrentPort(Config? config = null)
    {
        if (CurrentApplication == null)
        {
            return -1;
        }

        var server = CurrentApplication.Services.GetService<IServer>();
        var addressFeature = server?.Features.Get<IServerAddressesFeature>();

        if (addressFeature == null)
        {
            return -1;
        }

        foreach (var address in addressFeature.Addresses)
        {
            return int.Parse(address.Split(':').Last());
        }

        return -1;
    }

    public static void SetCurrentComponent(CustomComponentBase? customComponentBase)
    {
        CurrentComponent = customComponentBase;
    }

    public static void ClearCurrentComponent(CustomComponentBase customComponentBase)
    {
        if (CurrentComponent == customComponentBase)
        {
            CurrentComponent = null;
        }
    }
}
