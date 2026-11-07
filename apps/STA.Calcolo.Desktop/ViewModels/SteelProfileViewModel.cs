using STA.Calcolo.Desktop.Infrastructure;
using STA.Profilati.Core;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class SteelProfileViewModel : ObservableObject
{
    private readonly ModuleFieldViewModel selection;
    public IReadOnlyList<SteelProfile> Profiles=>ProfileCatalog.All;
    public SteelProfile? Model=>Profiles.SingleOrDefault(p=>p.Id==SelectedId);
    public string SelectedId {get=>selection.Value;set=>selection.Value=value;}
    public SteelProfileViewModel(ModuleFieldViewModel selection)
    {
        this.selection=selection;
        selection.PropertyChanged+=(_,_)=>{Changed(nameof(SelectedId));Changed(nameof(Model));};
    }
}
