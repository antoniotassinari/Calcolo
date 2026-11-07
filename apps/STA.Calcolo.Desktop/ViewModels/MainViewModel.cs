using STA.Calcolo.Application;
using STA.Calcolo.Desktop.Services;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class MainViewModel
{
    public UpdatesViewModel Updates {get;}=new();
    public IReadOnlyList<ModuleCardViewModel> Modules { get; }
    public MainViewModel(IWindowService windows) { Modules = [.. WorkspaceCatalog.Create().Select(m => new ModuleCardViewModel(m.Id, m.Title, m.Description, windows)), new("plinti", "Plinti · reazioni e MIDAS", "Progetti JSON, reazioni CSV e collegamento a MIDAS GEN NX.", windows), new("trave", "Travi prefabbricate", "Raccoglie sezioni e trefolature in un progetto interoperabile.", windows)]; }
}
