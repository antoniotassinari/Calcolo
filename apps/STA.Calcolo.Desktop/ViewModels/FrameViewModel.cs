using STA.Calcolo.Desktop.Infrastructure;
using STA.Muratura.Core;
using STA.Profilati.Core;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class FrameViewModel : ObservableObject
{
    private readonly MasonryFieldsViewModel editor;
    private string label="Cerchiatura";
    public string Label {get=>label;set=>Set(ref label,value);}
    public IReadOnlyList<ModuleFieldViewModel> Fields=>editor.Fields;
    public IReadOnlyDictionary<string,ModuleFieldViewModel> ByKey=>editor.ByKey;
    public SteelProfileViewModel Profile {get;}
    public bool Enabled {get=>ByKey["Enabled"].Value=="Sì";set=>ByKey["Enabled"].Value=value?"Sì":"No";}
    public IReadOnlyList<ProfileProposal> Proposals {get;private set;}=[];
    private ProfileProposal? selected;
    public ProfileProposal? SelectedProposal {get=>selected;set{Set(ref selected,value);UseProfileCommand.Refresh();}}
    public RelayCommand DesignCommand {get;}
    public RelayCommand UseProfileCommand {get;}
    public FrameViewModel(FrameInput model,Func<(StateResult Before,StateResult After)> states,Action calculate,Action<string> status)
    {
        editor=new(model,[("Enabled","Includi cerchiatura",["No","Sì"]),("Profile","Profilo OPPO",ProfileCatalog.All.Select(x=>x.Id).ToArray()),("H","Altezza telaio [cm]",null),("NColumns","Numero colonne",null),("NProfiles","Profili per colonna",null),("E","E acciaio [daN/cm²]",null),("Fy","fyk [MPa]",null),("Gamma","γM0",null),("Portal","Schema del portale",["Chiuso","Aperto"])]);
        Profile=new(ByKey["Profile"]);
        DesignCommand=new(()=>{try{calculate();var pair=states();Proposals=FrameCalculator.Design(Build(),pair.Before,pair.After);Changed(nameof(Proposals));status("Ricerca completata considerando le altre cerchiature attive: selezionare una proposta disponibile.");}catch(Exception e){ClearProposals();status(e.Message);}});
        UseProfileCommand=new(()=>{var id=SelectedProposal!.Candidate!.Profile.Id;Profile.SelectedId=id;calculate();},()=>SelectedProposal?.Candidate!=null);
        editor.PropertyChanged+=(_,_)=>{Changed(nameof(Fields));Changed(nameof(Enabled));};
    }
    public void ClearProposals(){if(Proposals.Count==0&&selected==null)return;Proposals=[];selected=null;Changed(nameof(Proposals));Changed(nameof(SelectedProposal));UseProfileCommand.Refresh();}
    public FrameInput Build(){var model=new FrameInput();editor.Apply(model);return model;}
}
