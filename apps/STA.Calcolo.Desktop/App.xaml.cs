using System.Windows;
using STA.Calcolo.Desktop.Services;
using STA.Calcolo.Desktop.ViewModels;
namespace STA.Calcolo.Desktop;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e) { base.OnStartup(e); var files = new FileService(); var windows = new WindowService(files); MainWindow = new MainWindow { DataContext = new MainViewModel(windows) }; MainWindow.Show(); }
}