using System.Windows;
using STA.Calcolo.Updater.ViewModels;
using STA.Calcolo.Updater.Views;
using STA.Calcolo.Updates.Windows;
namespace STA.Calcolo.Updater;
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        string? Value(string key){var index=Array.IndexOf(e.Args,key);return index>=0&&index+1<e.Args.Length?e.Args[index+1]:null;}
        var root=Value("--root");
        if(root==null||!int.TryParse(Value("--parent"),out var parent)){Shutdown(2);return;}
        var vm=new UpdateViewModel();var window=new UpdateWindow {DataContext=vm};MainWindow=window;
        window.Closing+=(_,args)=>args.Cancel=vm.Working;
        window.Closed+=(_,_)=>Shutdown();
        vm.Completed+=launch=>{if(launch)InstalledApplication.Launch(root);Shutdown();};
        window.Show();await vm.StartAsync(root,parent);
    }
}
