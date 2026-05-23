using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Forgia.UI.ViewModels;
using Forgia.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Forgia.UI;

public class App : Application
{
    public static IServiceProvider Services { get; set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowVm = Services.GetRequiredService<MainWindowViewModel>();
            var mainWindow = new MainWindow { DataContext = mainWindowVm };
            desktop.MainWindow = mainWindow;
            _ = mainWindowVm.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
