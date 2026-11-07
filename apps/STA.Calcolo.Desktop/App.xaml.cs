using System.Windows;
using STA.Calcolo.Desktop.Services;
using STA.Calcolo.Desktop.ViewModels;
namespace STA.Calcolo.Desktop;

public partial class App : System.Windows.Application
{
    private Mutex? instance;
    protected override void OnExit(ExitEventArgs e) {instance?.Dispose();base.OnExit(e);}
    protected override void OnStartup(StartupEventArgs e) { base.OnStartup(e);
        try {if(UpdateBootstrap.TryStart()) {Shutdown();return;}} catch(System.IO.IOException ex) {MessageBox.Show(ex.Message,"TA-Calcolo");Shutdown();return;}
        instance=new Mutex(false,STA.Calcolo.Updates.Windows.InstalledApplication.MutexName,out var first);
        if(!first){Shutdown();return;}
        var files = new FileService(); var windows = new WindowService(files); MainWindow = new MainWindow { DataContext = new MainViewModel(windows) }; MainWindow.Show(); }
}