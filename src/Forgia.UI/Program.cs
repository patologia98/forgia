using System;
using System.IO;
using Avalonia;
using Forgia.Infrastructure.Persistence;
using Forgia.UI.Services;
using Forgia.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestPDF.Infrastructure;
using Serilog;

namespace Forgia.UI;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Forgia");
        Directory.CreateDirectory(appDataPath);

        QuestPDF.Settings.License = LicenseType.Community;

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(appDataPath, "logs", "forgia-.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            var dbPath = Path.Combine(appDataPath, "forgia.db");

            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((_, services) =>
                {
                    services.AddDbContextFactory<ForgiaDbContext>(options =>
                        options.UseSqlite($"Data Source={dbPath};Foreign Keys=True"));

                    services.AddSingleton<IFileDialogService, FileDialogService>();
                    services.AddTransient<SetupViewModel>();
                    services.AddTransient<NewQuoteViewModel>();
                    services.AddSingleton<MainWindowViewModel>();
                })
                .Build();

            using (var db = host.Services.GetRequiredService<IDbContextFactory<ForgiaDbContext>>()
                       .CreateDbContext())
            {
                db.Database.Migrate();
            }

            App.Services = host.Services;
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
