using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JwtViewer.ViewModels;
using JwtViewer.Views;

namespace JwtViewer;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = new MainWindowViewModel();
            var window = new MainWindow
            {
                DataContext = vm
            };
            desktop.MainWindow = window;
            window.Activate();
        }
        else
        {
            Console.WriteLine("OMG NOT Classic!");
        }

        base.OnFrameworkInitializationCompleted();
    }
}