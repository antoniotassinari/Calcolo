using STA.Calcolo.Desktop.Infrastructure;
using STA.Calcolo.Desktop.Services;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class ModuleCardViewModel(string id, string title, string description, IWindowService windows)
{
    public string Id => id; public string Title => title; public string Description => description;
    public RelayCommand OpenCommand { get; } = new(() => windows.Open(id));
}
