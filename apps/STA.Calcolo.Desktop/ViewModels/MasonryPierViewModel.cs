using STA.Calcolo.Desktop.Infrastructure;
using STA.Muratura.Core;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class MasonryPierViewModel : ObservableObject
{
    private readonly MasonryPier model;
    private string label;
    public string Label {get=>label;set=>Set(ref label,value);}
    public MasonryFieldsViewModel Editor {get;}
    public MasonryMaterialViewModel Material {get;}
    public string MaterialId {get=>Material.MaterialId;set=>Material.MaterialId=value;}
    public string Level {get=>Material.Level;set=>Material.Level=value;}
    public bool LinearLoad=>Editor.ByKey["LoadMode"].Value=="Distribuito q";
    public bool HeightEditable {get;private set;}=true;
    public string GeometryText {get{try{var p=model with {};Editor.Apply(p);return $"Area {p.L*p.T:N2} cm² · h/l {p.H/p.L:N2}";}catch{return "Completare la geometria";}}}
    public RelayCommand RemoveCommand {get;}
    public MasonryPierViewModel(MasonryPier model,string label,Action<MasonryPierViewModel> remove)
    {
        this.model=model;this.label=label;
        Material=new(MasonryMaterialMapping.Read(model));
        Editor=new(model,[("H","Altezza [cm]",null),("L","Lunghezza [cm]",null),("T","Spessore [cm]",null),("LoadMode","Modalità del carico",["Risultante N","Distribuito q"]),("N","N compressione [kN]",null),("Q","q [kN/m]",null)]);
        Editor.PropertyChanged+=(_,_)=>{Changed(nameof(Editor));Changed(nameof(LinearLoad));Changed(nameof(GeometryText));};
        Material.PropertyChanged+=(_,_)=>{Changed(nameof(Material));Changed(nameof(MaterialId));Changed(nameof(Level));};
        RemoveCommand=new(()=>remove(this));
    }
    public void SetHeightEditable(bool value){HeightEditable=value;Changed(nameof(HeightEditable));}
    public MasonryPier Build(){var result=model with {};Editor.Apply(result);MasonryMaterialMapping.Apply(result,Material.Build());return result;}
    public void PresetAll(string id,string lc)=>Material.PresetAll(id,lc);
}
