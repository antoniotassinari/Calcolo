using System.Windows;
using STA.Calcolo.Application;
using STA.Calcolo.Desktop.ViewModels;
using STA.Calcolo.Desktop.Views;
namespace STA.Calcolo.Desktop.Services;

public sealed class WindowService(IFileService files) : IWindowService
{
    private readonly Dictionary<string, Window> open = [];
    private readonly Dictionary<string, object> sessions = [];
    public void Open(string moduleId)
    {
        if (open.TryGetValue(moduleId, out var existing)) { if (existing.WindowState == WindowState.Minimized) existing.WindowState = WindowState.Normal; existing.Activate(); return; }
        Window window = moduleId == "trave" ? new BeamWindow { DataContext = new BeamViewModel(files, this) } : moduleId == "plinti" ? new FootingWindow { DataContext = new FootingViewModel(files) } : new WorkspaceWindow { DataContext = new WorkspaceViewModel(WorkspaceCatalog.Create().Single(m => m.Id == moduleId), files) };
        if (sessions.TryGetValue(moduleId, out var context)) window.DataContext = context; else sessions.Add(moduleId, window.DataContext);
        window.Owner = System.Windows.Application.Current.MainWindow; open.Add(moduleId, window); window.Closed += (_, _) => open.Remove(moduleId); window.Show();
    }
}
