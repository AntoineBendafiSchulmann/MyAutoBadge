using System;
using System.IO;
using System.Windows;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using MyAutoBadge.Models;
using MyAutoBadge.Services;

namespace MyAutoBadge;

public partial class App : Application
{
    public static IHost AppHost { get; private set; } = null!;

    public App()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        Env.Load();

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<AutomationOptions>(context.Configuration.GetSection("AppSettings:Automation"));
                services.Configure<HolidayOptions>(context.Configuration.GetSection("AppSettings:Holidays"));

                services.AddSingleton<BadgeService>();
                services.AddSingleton<SessionLockService>();
                services.AddSingleton<HolidaysService>();
                services.AddSingleton<WebAutomationService>();
                services.AddHostedService<DailyBadgeWorker>();
            })
            .UseSerilog((context, services, config) =>
            {
                Directory.CreateDirectory("Logs");
                config
                    .WriteTo.Console()
                    .WriteTo.File(
                        Path.Combine("Logs", "logs-.txt"),
                        rollingInterval: RollingInterval.Day,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(2),
                        retainedFileCountLimit: 5,
                        fileSizeLimitBytes: 10_000_000,
                        rollOnFileSizeLimit: true
                    );
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        AppHost.Dispose();
        base.OnExit(e);
    }
}